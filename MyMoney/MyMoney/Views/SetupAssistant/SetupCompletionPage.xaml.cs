using CommonServiceLocator;
using MyMoney.ViewModels.Payments;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyMoney.Views.SetupAssistant
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetupCompletionPage : ContentPage
    {
        public SetupCompletionPage()
        {
            InitializeComponent();
            BindingContext = ServiceLocator.Current.GetInstance<SetupCompletionViewModel>();
        }
    }
}