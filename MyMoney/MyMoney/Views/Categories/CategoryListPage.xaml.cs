using MyMoney.ViewModels.Categories;
using Xamarin.Forms;
namespace MyMoney.Views.Categories
{
    public partial class CategoryListPage : ContentPage
    {
        private CategoryListViewModel ViewModel => (CategoryListViewModel)BindingContext;

        public CategoryListPage()
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.CategoryListViewModel;

        }

        protected override async void OnAppearing() => await ViewModel.InitializeAsync();
    }
}
