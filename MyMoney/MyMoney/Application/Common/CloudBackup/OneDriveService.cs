﻿using Microsoft.Graph;
using Microsoft.Identity.Client;
using MyMoney.Application.Common.Constants;
using MyMoney.Domain.Exceptions;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Logger = NLog.Logger;

namespace MyMoney.Application.Common.CloudBackup
{
    /// <inheritdoc/>
    public class OneDriveService : ICloudBackupService
    {
        private const int BACKUP_ARCHIVE_COUNT = 15;
        private const string BACKUP_NAME_TEMP = "MyMoney.db_upload";

        private const string ERROR_CODE_CANCELED = "authentication_canceled";
        private readonly string[] scopes = { "Files.ReadWrite", "User.ReadBasic.All", "Files.ReadWrite.All", "Sites.ReadWrite.All"};

        private readonly Logger logManager = LogManager.GetCurrentClassLogger();

        private readonly IPublicClientApplication publicClientApplication;
        private readonly IGraphClientFactory graphClientFactory;

        /// <summary>
        /// Constructor
        /// </summary>
        public OneDriveService(IPublicClientApplication publicClientApplication, IGraphClientFactory graphClientFactory)
        {
            this.publicClientApplication = publicClientApplication;
            this.graphClientFactory = graphClientFactory;
            this.UserAccount = new UserAccount();
        }

        private IGraphServiceClient GraphServiceClient { get; set; }

        private DriveItem ArchiveFolder { get; set; }
        
        public UserAccount UserAccount { get; set; }

        /// <summary>
        /// Login User to OneDrive.
        /// </summary>
        public async Task LoginAsync()
        {
            AuthenticationResult? authResult = null;
            IEnumerable<IAccount> accounts = await publicClientApplication.GetAccountsAsync();

            // let's see if we have a user in our belly already
            try
            {
                IAccount firstAccount = accounts.FirstOrDefault();
                authResult = await publicClientApplication.AcquireTokenSilent(scopes, firstAccount).ExecuteAsync();

                GraphServiceClient = graphClientFactory.CreateClient(authResult);
                var user = await GraphServiceClient.Me.Request().GetAsync();
                UserAccount.SetUserAccount(user);
            }
            catch(MsalUiRequiredException ex)
            {
                logManager.Debug(ex);
                // pop the browser for the interactive experience
                authResult = await publicClientApplication.AcquireTokenInteractive(scopes)
                    .WithAuthority("https://login.microsoftonline.com/2dff09ac-2b3b-4182-9953-2b548e0d0b39")
                                                          .WithParentActivityOrWindow(ParentActivityWrapper.ParentActivity) // this is required for Android
                                                          .ExecuteAsync();

                GraphServiceClient = graphClientFactory.CreateClient(authResult);
                var user = await GraphServiceClient.Me.Request().GetAsync();
                UserAccount = new UserAccount();
                UserAccount.SetUserAccount(user);
            }
            catch(MsalClientException ex)
            {
                if(ex.ErrorCode == ERROR_CODE_CANCELED)
                {
                    logManager.Info(ex);
                    throw new BackupOperationCanceledException();
                }

                logManager.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Logout User from OneDrive.
        /// </summary>
        public async Task LogoutAsync()
        {
            try
            {
                logManager.Info("Logout.");

                var accounts = (await publicClientApplication.GetAccountsAsync()).ToList();

                while(accounts.Any())
                {
                    await publicClientApplication.RemoveAsync(accounts.FirstOrDefault());
                    accounts = (await publicClientApplication.GetAccountsAsync()).ToList();
                }
            }
            catch(MsalClientException ex)
            {
                if(ex.ErrorCode == ERROR_CODE_CANCELED)
                {
                    logManager.Info(ex);
                    throw new BackupOperationCanceledException();
                }

                logManager.Error(ex);
                throw;
            }
            catch(Exception ex)
            {
                logManager.Error(ex);
                throw new BackupAuthenticationFailedException(ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UploadAsync(Stream dataToUpload)
        {
            try
            {
                logManager.Info("Upload Backup.");

                if(GraphServiceClient == null)
                {
                    await LoginAsync();
                    if(GraphServiceClient == null)
                    {
                        throw new BackupAuthenticationFailedException("Was not able to automatically login.");
                    }
                }

                DriveItem uploadedItem = await GraphServiceClient
                                        .Sites["root"]
                                        .Drive
                                        .Special
                                        .AppRoot
                                        .ItemWithPath(BACKUP_NAME_TEMP)
                                        .Content
                                        .Request()
                                        .PutAsync<DriveItem>(dataToUpload);

                await LoadArchiveFolderAsync();
                await DeleteCleanupOldBackupsAsync();
                await ArchiveCurrentBackupAsync();
                await RenameUploadedBackupAsync();

                return uploadedItem != null;
            }
            catch(MsalClientException ex)
            {
                if(ex.ErrorCode == ERROR_CODE_CANCELED)
                {
                    logManager.Info(ex);
                    await RestoreArchivedBackupInCaseOfErrorAsync();
                    throw new BackupOperationCanceledException(ex);
                }

                logManager.Error(ex);
                await RestoreArchivedBackupInCaseOfErrorAsync();
                throw;
            }
            catch(OperationCanceledException ex)
            {
                logManager.Info(ex);
                await RestoreArchivedBackupInCaseOfErrorAsync();
                throw new BackupOperationCanceledException(ex);
            }
            catch(Exception ex)
            {
                logManager.Error(ex);
                await RestoreArchivedBackupInCaseOfErrorAsync();
                throw new BackupAuthenticationFailedException(ex);
            }
        }

        private async Task RestoreArchivedBackupInCaseOfErrorAsync()
        {
            logManager.Info("Restore archived Backup.");

            if(GraphServiceClient == null)
            {
                throw new GraphClientNullException();
            }

            IDriveItemChildrenCollectionPage archivedBackups = await GraphServiceClient.Drive
                                                                                      .Items[ArchiveFolder?.Id]
                                                                                      .Children
                                                                                      .Request()
                                                                                      .GetAsync();

            if(!archivedBackups.Any())
            {
                logManager.Info("No backups found.");
                return;
            }

            DriveItem lastBackup = archivedBackups.OrderByDescending(x => x.CreatedDateTime).First();

            DriveItem? appRoot = await GraphServiceClient
                                        .Sites["root"]
                                        .Drive
                                        .Special
                                        .AppRoot
                                        .Request()
                                        .GetAsync();

            var updateItem = new DriveItem
            {
                ParentReference = new ItemReference { Id = appRoot.Id },
                Name = DatabaseConstants.BACKUP_NAME
            };

            await GraphServiceClient
                .Drive
                .Items[lastBackup.Id]
                .Request()
                .UpdateAsync(updateItem);
        }

        /// <inheritdoc/>
        public async Task<Stream> RestoreAsync(string backupName, string dbName)
        {
            try
            {
                logManager.Info("Restore Backup.");

                if(GraphServiceClient == null)
                {
                    await LoginAsync();
                    if(GraphServiceClient == null)
                    {
                        throw new BackupAuthenticationFailedException("Was not able to automatically login.");
                    }
                }

                DriveItem existingBackup = (await GraphServiceClient
                                                 .Sites["root"]
                                                 .Drive
                                                 .Special
                                                 .AppRoot.Children
                                                         .Request()
                                                         .GetAsync())
                   .FirstOrDefault(x => x.Name == backupName);

                if(existingBackup == null)
                {
                    throw new NoBackupFoundException($"No backup with the name {backupName} was found.");
                }

                return await GraphServiceClient.Drive.Items[existingBackup.Id].Content.Request().GetAsync();
            }
            catch(MsalClientException ex)
            {
                if(ex.ErrorCode == ERROR_CODE_CANCELED)
                {
                    logManager.Info(ex);
                    throw new BackupOperationCanceledException();
                }

                logManager.Error(ex);
                throw;
            }
            catch(Exception ex)
            {
                logManager.Error(ex);
                throw new BackupAuthenticationFailedException(ex);
            }
        }

        /// <inheritdoc/>
        public async Task<DateTime> GetBackupDateAsync()
        {
            try
            {
                logManager.Info("Get Backupdate.");

                if(GraphServiceClient == null)
                {
                    await LoginAsync();
                    if(GraphServiceClient == null)
                    {
                        return DateTime.MinValue;
                    }
                }

                DriveItem existingBackup = (await GraphServiceClient
                                                 .Sites["root"]
                                                 .Drive
                                                 .Special
                                                 .AppRoot.Children
                                                         .Request()
                                                         .GetAsync())
                   .FirstOrDefault(x => x.Name == DatabaseConstants.BACKUP_NAME);

                if(existingBackup != null)
                {
                    return existingBackup.LastModifiedDateTime?.DateTime ?? DateTime.MinValue;
                }

                return DateTime.MinValue;
            }
            catch(Exception ex)
            {
                logManager.Error(ex);
                return DateTime.MinValue;
            }
        }

        /// <inheritdoc/>
        public async Task<List<string>> GetFileNamesAsync()
        {
            try
            {
                logManager.Info("Get Filenames.");

                if(GraphServiceClient == null)
                {
                    await LoginAsync();
                    if(GraphServiceClient == null)
                    {
                        throw new BackupAuthenticationFailedException("Was not able to automatically login.");
                    }
                }

                return (await GraphServiceClient
                             .Sites["root"]
                             .Drive
                             .Special
                             .AppRoot.Children
                                     .Request()
                                     .GetAsync())
                      .Select(x => x.Name)
                      .ToList();
            }
            catch(Exception ex)
            {
                logManager.Error(ex);
                throw new BackupAuthenticationFailedException(ex);
            }
        }

        private async Task DeleteCleanupOldBackupsAsync()
        {
            logManager.Info("Cleanup old backups.");

            if(GraphServiceClient == null)
            {
                throw new GraphClientNullException();
            }

            IDriveItemChildrenCollectionPage archiveBackups = await GraphServiceClient.Drive
                                                                                      .Items[ArchiveFolder?.Id]
                                                                                      .Children
                                                                                      .Request()
                                                                                      .GetAsync();

            if(archiveBackups.Count < BACKUP_ARCHIVE_COUNT)
            {
                return;
            }

            DriveItem oldestBackup = archiveBackups.OrderByDescending(x => x.CreatedDateTime).Last();

            await GraphServiceClient.Drive
                                    .Items[oldestBackup?.Id]
                                    .Request()
                                    .DeleteAsync();
        }

        private async Task ArchiveCurrentBackupAsync()
        {
            logManager.Info("Archive Backup.");

            if(ArchiveFolder == null)
            {
                return;
            }

            if(GraphServiceClient == null)
            {
                throw new GraphClientNullException();
            }

            DriveItem currentBackup = (await GraphServiceClient
                                            .Sites["root"]
                                            .Drive
                                            .Special
                                            .AppRoot.Children
                                                    .Request()
                                                    .GetAsync())
               .FirstOrDefault(x => x.Name == DatabaseConstants.BACKUP_NAME);

            if(currentBackup == null)
            {
                return;
            }

            var updateItem = new DriveItem
            {
                ParentReference = new ItemReference { Id = ArchiveFolder.Id },
                Name = string.Format(CultureInfo.InvariantCulture,
                                                      DatabaseConstants.BACKUP_ARCHIVE_NAME,
                                                      DateTime.Now.ToString("yyyy-M-d_hh-mm-ssss", CultureInfo.InvariantCulture))
            };

            await GraphServiceClient
                 .Drive
                 .Items[currentBackup.Id]
                 .Request()
                 .UpdateAsync(updateItem);
        }

        private async Task RenameUploadedBackupAsync()
        {
            logManager.Info("Rename backup_upload.");

            if(GraphServiceClient == null)
            {
                throw new GraphClientNullException();
            }

            DriveItem backup_upload = (await GraphServiceClient
                                            .Sites["root"]
                                            .Drive
                                            .Special
                                            .AppRoot.Children
                                                    .Request()
                                                    .GetAsync())
               .FirstOrDefault(x => x.Name == BACKUP_NAME_TEMP);

            if(backup_upload == null)
            {
                return;
            }

            var updateItem = new DriveItem { Name = DatabaseConstants.BACKUP_NAME };

            await GraphServiceClient
                 .Drive
                 .Items[backup_upload.Id]
                 .Request()
                 .UpdateAsync(updateItem);
        }

        private async Task LoadArchiveFolderAsync()
        {
            if(ArchiveFolder != null)
            {
                return;
            }

            if(GraphServiceClient == null)
            {
                throw new GraphClientNullException();
            }

            ArchiveFolder = (await GraphServiceClient
                                  .Sites["root"]
                                  .Drive
                                  .Special
                                  .AppRoot.Children
                                          .Request()
                                          .GetAsync())
                           .CurrentPage
                           .FirstOrDefault(x => x.Name == DatabaseConstants.ARCHIVE_FOLDER_NAME);

            if(ArchiveFolder == null)
            {
                await CreateArchiveFolderAsync();
            }
        }

        private async Task CreateArchiveFolderAsync()
        {
            if(GraphServiceClient == null)
            {
                throw new GraphClientNullException();
            }

            var folderToCreate = new DriveItem
            {
                Name = DatabaseConstants.ARCHIVE_FOLDER_NAME,
                Folder = new Folder()
            };

            ArchiveFolder = await GraphServiceClient
                                 .Sites["root"]
                                 .Drive
                                 .Special
                                 .AppRoot.Children
                                         .Request()
                                         .AddAsync(folderToCreate);
        }
    }
}
