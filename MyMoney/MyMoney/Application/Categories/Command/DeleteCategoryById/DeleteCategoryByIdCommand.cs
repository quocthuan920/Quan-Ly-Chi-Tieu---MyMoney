using MediatR;
using MyMoney.Application.Common;
using MyMoney.Application.Common.CloudBackup;
using MyMoney.Application.Common.Facades;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyMoney.Application.Categories.Command.DeleteCategoryById
{
    public class DeleteCategoryByIdCommand : IRequest
    {
        public DeleteCategoryByIdCommand(int categoryId)
        {
            CategoryId = categoryId;
        }

        public int CategoryId { get; }

        public class Handler : IRequestHandler<DeleteCategoryByIdCommand>
        {
            private readonly IContextAdapter contextAdapter;
            private readonly IBackupService backupService;
            private readonly ISettingsFacade settingsFacade;

            public Handler(IContextAdapter contextAdapter, IBackupService backupService, ISettingsFacade settingsFacade)
            {
                this.contextAdapter = contextAdapter;
                this.backupService = backupService;
                this.settingsFacade = settingsFacade;
            }

            public async Task<Unit> Handle(DeleteCategoryByIdCommand request, CancellationToken cancellationToken)
            {
                await backupService.RestoreBackupAsync();
                Category entityToDelete = await contextAdapter.Context.Categories.FindAsync(request.CategoryId);

                contextAdapter.Context.Categories.Remove(entityToDelete);
                await contextAdapter.Context.SaveChangesAsync(cancellationToken);

                settingsFacade.LastDatabaseUpdate = DateTime.Now;
                backupService.UploadBackupAsync().FireAndForgetSafeAsync();

                return Unit.Value;
            }
        }
    }
}
