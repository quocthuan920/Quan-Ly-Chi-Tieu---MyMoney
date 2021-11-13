using MyMoney.Ui.ViewModels.Backup;

namespace MyMoney.Views.Backup
{
    public partial class BackupPage
    {
        public BackupViewModel ViewModel => (BackupViewModel)BindingContext;

        public BackupPage()
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.BackupViewModel;
        }

        protected override void OnAppearing() => ViewModel.InitializeCommand.Execute(null);
    }
}
