using AutoMapper;
using MediatR;
using MyMoney.Application.Accounts.Commands.CreateAccount;
using MyMoney.Application.Accounts.Queries.GetIfAccountWithNameExists;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Application.Resources;
using System.Threading.Tasks;

namespace MyMoney.ViewModels.Accounts
{
    public class AddAccountViewModel : ModifyAccountViewModel
    {
        private readonly IMediator mediator;
        private readonly IDialogService dialogService;

        public AddAccountViewModel(IMediator mediator,
                                   IMapper mapper,
                                   IDialogService dialogService)
            : base(dialogService)
        {
            this.mediator = mediator;
            this.dialogService = dialogService;
        }

        protected override async Task SaveAccountAsync()
        {
            if(await mediator.Send(new GetIfAccountWithNameExistsQuery(SelectedAccountVm.Name)))
            {
                await dialogService.ShowMessageAsync(Strings.DuplicatedNameTitle, Strings.DuplicateAccountMessage);
                return;
            }

            await mediator.Send(new CreateAccountCommand(SelectedAccountVm.Name,
                                                         SelectedAccountVm.CurrentBalance,
                                                         SelectedAccountVm.Note,
                                                         SelectedAccountVm.IsExcluded));
        }
    }
}
