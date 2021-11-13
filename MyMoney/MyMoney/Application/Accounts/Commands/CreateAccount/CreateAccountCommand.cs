﻿using MediatR;
using MyMoney.Application.Common;
using MyMoney.Application.Common.CloudBackup;
using MyMoney.Application.Common.Facades;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyMoney.Application.Accounts.Commands.CreateAccount
{
    public class CreateAccountCommand : IRequest
    {
        public CreateAccountCommand(string name, decimal currentBalance = 0, string note = "", bool isExcluded = false)
        {
            Name = name;
            CurrentBalance = currentBalance;
            Note = note;
            IsExcluded = isExcluded;
        }

        public string Name { get; private set; }
        public decimal CurrentBalance { get; private set; }
        public string Note { get; private set; }
        public bool IsExcluded { get; private set; }

        public class Handler : IRequestHandler<CreateAccountCommand>
        {
            private readonly IContextAdapter contextAdapter;
            private readonly IBackupService backupService;
            private readonly ISettingsFacade settingsFacade;

            public Handler(IContextAdapter contextAdapter,
                           IBackupService backupService,
                           ISettingsFacade settingsFacade)
            {
                this.contextAdapter = contextAdapter;
                this.backupService = backupService;
                this.settingsFacade = settingsFacade;
            }

            /// <inheritdoc/>
            public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
            {
                await contextAdapter.Context.Accounts.AddAsync(new Account(request.Name,
                                                                           request.CurrentBalance,
                                                                           request.Note,
                                                                           request.IsExcluded),
                                                               cancellationToken);
                await contextAdapter.Context.SaveChangesAsync(cancellationToken);

                settingsFacade.LastDatabaseUpdate = DateTime.Now;
                backupService.UploadBackupAsync().FireAndForgetSafeAsync();

                return Unit.Value;
            }
        }
    }
}
