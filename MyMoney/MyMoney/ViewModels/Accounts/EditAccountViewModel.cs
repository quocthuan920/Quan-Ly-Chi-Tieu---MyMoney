using AutoMapper;
using GalaSoft.MvvmLight.Command;
using MediatR;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Application.Accounts.Commands.DeleteAccountById;
using MyMoney.Application.Accounts.Commands.UpdateAccount;
using MyMoney.Application.Accounts.Queries.GetAccountById;
using MyMoney.Application.Resources;
using MyMoney.Domain.Entities;
using MyMoney.Ui.ViewModels.Accounts;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MyMoney.ViewModels.Accounts
{
    public class EditAccountViewModel : ModifyAccountViewModel
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;
        private readonly IDialogService dialogService;
        public EditAccountViewModel(IMediator mediator,
                                    IMapper mapper,
                                    IDialogService dialogService)
            : base(dialogService)
        {
            this.mediator = mediator;
            this.mapper = mapper;
            this.dialogService = dialogService;
        }

        public async Task InitializeAsync(int accountId) => SelectedAccountVm = mapper.Map<AccountViewModel>(await mediator.Send(new GetAccountByIdQuery(accountId)));

        protected override async Task SaveAccountAsync() => await mediator.Send(new UpdateAccountCommand(mapper.Map<Account>(SelectedAccountVm)));
        public RelayCommand<AccountViewModel> DeleteCommand
            => new RelayCommand<AccountViewModel>(async (p) => await DeleteAccountAsync(p));
        private async Task DeleteAccountAsync(AccountViewModel account)
        {
            if (await dialogService.ShowConfirmMessageAsync(Strings.DeleteTitle, Strings.DeleteAccountConfirmationMessage))
            {
                var deleteCommand = new DeleteAccountByIdCommand(account.Id);
                
                try
                {
                    await dialogService.ShowLoadingDialogAsync();
                    await mediator.Send(deleteCommand);
                    await Shell.Current.Navigation.PopModalAsync();
                }
                finally
                {
                    await dialogService.HideLoadingDialogAsync();
                }
            }
        }
    }
}
