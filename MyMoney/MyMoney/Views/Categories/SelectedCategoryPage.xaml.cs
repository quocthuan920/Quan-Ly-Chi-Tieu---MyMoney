using MyMoney.Application.Resources;
using MyMoney.ViewModels.Categories;
using Xamarin.Forms;

using MyMoney.Extensions;
namespace MyMoney.Views.Categories
{
    public partial class SelectCategoryPage : ContentPage
    {
        private SelectCategoryViewModel ViewModel => (SelectCategoryViewModel)BindingContext;

        public SelectCategoryPage()
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.SelectCategoryViewModel;

            var cancelItem = new ToolbarItem
            {
                Command = new Command(async () => await Navigation.PopModalAsync()),
                Text = Strings.CancelLabel,
                IconImageSource = ImageSource.FromFile("previous.png"),
                Priority = -1,
                Order = ToolbarItemOrder.Primary
            };
            var addCategory = new ToolbarItem
            {
                Command = new Command(async () => await Shell.Current.GoToModalAsync(ViewModelLocator.AddCategoryRoute)),
                IconImageSource = ImageSource.FromFile("plus.png"),
                Text = "Thêm",
                Priority = -1,
                Order = ToolbarItemOrder.Primary
            };

            ToolbarItems.Add(cancelItem);
            ToolbarItems.Add(addCategory);
        }

        protected override async void OnAppearing() => await ViewModel.InitializeAsync();
    }
}