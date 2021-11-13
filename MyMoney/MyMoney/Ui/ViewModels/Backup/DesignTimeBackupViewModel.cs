﻿using GalaSoft.MvvmLight.Command;
using System;

namespace MyMoney.Ui.ViewModels.Backup
{
    public class DesignTimeBackupViewModel : IBackupViewModel
    {
        public RelayCommand InitializeCommand { get; } = null!;

        public RelayCommand LoginCommand { get; } = null!;

        public RelayCommand LogoutCommand { get; } = null!;

        public RelayCommand BackupCommand { get; } = null!;

        public RelayCommand RestoreCommand { get; } = null!;

        public DateTime BackupLastModified { get; }

        public bool IsLoadingBackupAvailability { get; }

        public bool IsLoggedIn { get; } = true;

        public bool BackupAvailable { get; }

        public bool IsAutoBackupEnabled { get; } = false;
    }
}
