using MyMoney.ViewModels.Budget;
using MyMoney.Views.Accounts;
using MyMoney.Views.Backup;
using MyMoney.Views.Categories;
using MyMoney.Views.Dashboard;
using MyMoney.Views.Payments;
using MyMoney.Views.Settings;
using MyMoney.Views.SetupAssistant;
using MyMoney.Views.Statistics;
using Xamarin.Forms;

namespace MyMoney
{
    public partial class AppShell : Shell
    {

        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }
        private void RegisterRoutes()
        {
            Routing.RegisterRoute(ViewModelLocator.WelcomeViewRoute, typeof(WelcomePage));
            Routing.RegisterRoute(ViewModelLocator.CategoryIntroductionRoute, typeof(CategoryIntroductionPage));
            Routing.RegisterRoute(ViewModelLocator.SetupCompletionRoute, typeof(SetupCompletionPage));

            Routing.RegisterRoute(ViewModelLocator.DashboardRoute, typeof(DashboardPage));
            Routing.RegisterRoute(ViewModelLocator.AccountListRoute, typeof(AccountListPage));
            Routing.RegisterRoute(ViewModelLocator.AddAccountRoute, typeof(AddAccountPage));
            Routing.RegisterRoute(ViewModelLocator.EditAccountRoute, typeof(EditAccountPage));
            Routing.RegisterRoute(ViewModelLocator.BudgetListRoute, typeof(BudgetListPage));
            Routing.RegisterRoute(ViewModelLocator.PaymentListRoute, typeof(PaymentListPage));
            Routing.RegisterRoute(ViewModelLocator.CategoryListRoute, typeof(CategoryListPage));
            Routing.RegisterRoute(ViewModelLocator.SelectCategoryRoute, typeof(SelectCategoryPage));
            Routing.RegisterRoute(ViewModelLocator.AddCategoryRoute, typeof(AddCategoryPage));
            Routing.RegisterRoute(ViewModelLocator.AddPaymentRoute, typeof(AddPaymentPage));
            Routing.RegisterRoute(ViewModelLocator.BackupRoute, typeof(BackupPage));
            Routing.RegisterRoute(ViewModelLocator.SettingsRoute, typeof(SettingsPage));
            Routing.RegisterRoute(ViewModelLocator.StatisticCategorySpreadingRoute, typeof(StatisticCategorySpreadingPage));
            Routing.RegisterRoute(ViewModelLocator.StatisticCategorySummaryRoute, typeof(StatisticCategorySummaryPage));
            Routing.RegisterRoute(ViewModelLocator.PaymentForCategoryListRoute, typeof(PaymentForCategoryListPage));
        }
       
    }
}
