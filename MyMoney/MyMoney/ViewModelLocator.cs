using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using MyMoney.Ui.ViewModels.Backup;
using MyMoney.Ui.ViewModels.Settings;
using MyMoney.ViewModels.Accounts;
using MyMoney.ViewModels.Budget;
using MyMoney.ViewModels.Categories;
using MyMoney.ViewModels.Dashboard;
using MyMoney.ViewModels.Dialogs;
using MyMoney.ViewModels.Payments;
using MyMoney.ViewModels.Statistics;
using MyMoney.Views.Accounts;
using MyMoney.Views.Backup;
using MyMoney.Views.Categories;
using MyMoney.Views.Dashboard;
using MyMoney.Views.Payments;
using MyMoney.Views.Settings;
using MyMoney.Views.SetupAssistant;
using MyMoney.Views.Statistics;

namespace MyMoney
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            if(!ServiceLocator.IsLocationProviderSet && ViewModelBase.IsInDesignModeStatic)
            {
                RegisterServices(new ContainerBuilder());
            }
        }

        public static void RegisterServices(ContainerBuilder registrations)
        {
            IContainer container = registrations.Build();

            if(container != null)
            {
                ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
            }
        }

        // Routes
        public static string DashboardRoute => $"Dashboard/{nameof(DashboardPage)}";
        public static string AccountListRoute => $"Account/{nameof(AccountListPage)}";
        public static string AddAccountRoute => $"Account/{nameof(AddAccountPage)}";
        public static string EditAccountRoute => $"Account/{nameof(EditAccountPage)}";
        public static string BudgetListRoute => $"Budget/{nameof(BudgetListPage)}";
        public static string PaymentListRoute => $"Payment/{nameof(PaymentListPage)}";
        public static string AddPaymentRoute => $"Payment/{nameof(AddPaymentPage)}";
        public static string CategoryListRoute => $"Category/{nameof(CategoryListPage)}";
        public static string SelectCategoryRoute => $"Category/{nameof(SelectCategoryPage)}";
        public static string AddCategoryRoute => $"Category/{nameof(AddCategoryPage)}";
        public static string BackupRoute => $"Backup/Show{nameof(BackupPage)}";
        public static string SettingsRoute => $"Settings/Show{nameof(SettingsPage)}";
        public static string StatisticCategorySpreadingRoute => $"Settings/Show{nameof(StatisticCategorySpreadingPage)}";
        public static string StatisticCategorySummaryRoute => $"Settings/Show{nameof(StatisticCategorySummaryPage)}";
        public static string PaymentForCategoryListRoute => $"Settings/Show{nameof(PaymentForCategoryListPage)}";
        public static string WelcomeViewRoute => $"Setup/{nameof(WelcomePage)}";
        public static string CategoryIntroductionRoute => $"Setup/{nameof(CategoryIntroductionPage)}";
        public static string SetupCompletionRoute => $"Setup/{nameof(SetupCompletionPage)}";

        // ViewModels
        public static DashboardViewModel DashboardViewModel => ServiceLocator.Current.GetInstance<DashboardViewModel>();
        public static AccountListViewModel AccountListViewModel => ServiceLocator.Current.GetInstance<AccountListViewModel>();
        public static AddAccountViewModel AddAccountViewModel => ServiceLocator.Current.GetInstance<AddAccountViewModel>();
        public static EditAccountViewModel EditAccountViewModel => ServiceLocator.Current.GetInstance<EditAccountViewModel>();
        public static PaymentListViewModel PaymentListViewModel => ServiceLocator.Current.GetInstance<PaymentListViewModel>();
        public static AddPaymentViewModel AddPaymentViewModel => ServiceLocator.Current.GetInstance<AddPaymentViewModel>();
        public static EditPaymentViewModel EditPaymentViewModel => ServiceLocator.Current.GetInstance<EditPaymentViewModel>();
        public static CategoryListViewModel CategoryListViewModel => ServiceLocator.Current.GetInstance<CategoryListViewModel>();
        public static SelectCategoryViewModel SelectCategoryViewModel => ServiceLocator.Current.GetInstance<SelectCategoryViewModel>();
        public static AddCategoryViewModel AddCategoryViewModel => ServiceLocator.Current.GetInstance<AddCategoryViewModel>();
        public static EditCategoryViewModel EditCategoryViewModel => ServiceLocator.Current.GetInstance<EditCategoryViewModel>();
        public static SelectDateRangeDialogViewModel SelectDateRangeDialogViewModel => ServiceLocator.Current.GetInstance<SelectDateRangeDialogViewModel>();
        public static SelectFilterDialogViewModel SelectFilterDialogViewModel => ServiceLocator.Current.GetInstance<SelectFilterDialogViewModel>();
        public static BackupViewModel BackupViewModel => ServiceLocator.Current.GetInstance<BackupViewModel>();
        public static StatisticCategorySpreadingViewModel StatisticCategorySpreadingViewModel => ServiceLocator.Current.GetInstance<StatisticCategorySpreadingViewModel>();
        public static StatisticCategorySummaryViewModel StatisticCategorySummaryViewModel => ServiceLocator.Current.GetInstance<StatisticCategorySummaryViewModel>();
        public static StatisticSelectorViewModel StatisticSelectorViewModel => ServiceLocator.Current.GetInstance<StatisticSelectorViewModel>();
        public static PaymentForCategoryListViewModel PaymentForCategoryListViewModel => ServiceLocator.Current.GetInstance<PaymentForCategoryListViewModel>();
        public static SettingsViewModel SettingsViewModel => ServiceLocator.Current.GetInstance<SettingsViewModel>();
    }
}
