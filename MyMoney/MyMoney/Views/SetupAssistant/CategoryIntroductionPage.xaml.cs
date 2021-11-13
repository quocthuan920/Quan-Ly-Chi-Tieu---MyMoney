using CommonServiceLocator;
using MyMoney.ViewModels.SetupAssistant;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyMoney.Views.SetupAssistant
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategoryIntroductionPage : ContentPage
    {
        public CategoryIntroductionPage()
        {
            InitializeComponent();
            BindingContext = ServiceLocator.Current.GetInstance<CategoryIntroductionViewModel>();
        }
    }
}