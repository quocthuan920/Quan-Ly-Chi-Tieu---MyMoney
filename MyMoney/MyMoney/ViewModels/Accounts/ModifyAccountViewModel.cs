using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Application.Common.Messages;
using MyMoney.Application.Resources;
using MyMoney.Ui.ViewModels.Accounts;
using System.Threading.Tasks;

namespace MyMoney.ViewModels.Accounts
{
    public abstract class ModifyAccountViewModel : ViewModelBase
    {
        private readonly IDialogService dialogService;

        protected ModifyAccountViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }

        private AccountViewModel selectedAccountVm = new AccountViewModel();

        /// <summary>
        /// The currently selected CategoryViewModel
        /// </summary>
        public AccountViewModel SelectedAccountVm
        {
            get => selectedAccountVm;
            set
            {
                selectedAccountVm = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand SaveCommand => new RelayCommand(async () => await SaveAccountBaseAsync());

        protected abstract Task SaveAccountAsync();

        private async Task SaveAccountBaseAsync()
        {
            if(string.IsNullOrWhiteSpace(SelectedAccountVm.Name))
            {
                await dialogService.ShowMessageAsync(Strings.MandatoryFieldEmptyTitle, Strings.NameRequiredMessage);
                return;
            }

            await dialogService.ShowLoadingDialogAsync(Strings.SavingAccountMessage);

            await SaveAccountAsync();
            MessengerInstance.Send(new ReloadMessage());

            await dialogService.HideLoadingDialogAsync();
            await App.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
