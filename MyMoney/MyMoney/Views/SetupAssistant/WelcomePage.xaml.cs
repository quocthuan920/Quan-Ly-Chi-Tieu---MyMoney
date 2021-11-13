using CommonServiceLocator;
using MyMoney.ViewModels.SetupAssistant;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyMoney.Views.SetupAssistant
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        private WelcomeViewModel ViewModel => (WelcomeViewModel)BindingContext;

        public WelcomePage()
        {
            InitializeComponent();
            BindingContext = ServiceLocator.Current.GetInstance<WelcomeViewModel>();
        }

        protected override async void OnAppearing() => await ViewModel.InitAsync();
    }
}
