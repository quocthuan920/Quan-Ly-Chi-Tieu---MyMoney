using MediatR;
using MyMoney.Application.Common;
using MyMoney.Application.Common.CloudBackup;
using MyMoney.Application.Common.Facades;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyMoney.Application.Categories.Command.UpdateCategory
{
    public class UpdateCategoryCommand : IRequest
    {
        public UpdateCategoryCommand(Category category)
        {
            Category = category;
        }

        public Category Category { get; }

        public class Handler : IRequestHandler<UpdateCategoryCommand>
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

            public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
            {
                Category existingCategory = await contextAdapter.Context.Categories.FindAsync(request.Category.Id);

                existingCategory.UpdateData(request.Category.Name,
                                            request.Category.Note ?? "",
                                            request.Category.RequireNote);

                await contextAdapter.Context.SaveChangesAsync(cancellationToken);

                settingsFacade.LastDatabaseUpdate = DateTime.Now;
                backupService.UploadBackupAsync().FireAndForgetSafeAsync();

                return Unit.Value;
            }
        }
    }
}
