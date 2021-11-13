using MyMoney.Ui.ViewModels.Settings;

namespace MyMoney.Views.Settings
{
    public partial class SettingsPage
    {
        private SettingsViewModel ViewModel => (SettingsViewModel)BindingContext;
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.SettingsViewModel;
        }

        protected override async void OnAppearing() => await ViewModel.InitializeAsync();
    }
}