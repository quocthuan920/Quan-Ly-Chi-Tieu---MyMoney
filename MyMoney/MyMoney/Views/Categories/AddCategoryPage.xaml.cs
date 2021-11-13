using MyMoney.Application.Resources;
using MyMoney.ViewModels.Categories;
using Xamarin.Forms;

namespace MyMoney.Views.Categories
{
    public partial class AddCategoryPage
    {
        private AddCategoryViewModel ViewModel => (AddCategoryViewModel)BindingContext;

        public AddCategoryPage()
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.AddCategoryViewModel;

            var cancelItem = new ToolbarItem
            {
                Command = new Command(async () => await Navigation.PopModalAsync()),
                Text = Strings.CancelLabel,
                Priority = -1,
                Order = ToolbarItemOrder.Primary
            };

            var saveItem = new ToolbarItem
            {
                Command = new Command(() => ViewModel.SaveCommand.Execute(null)),
                Text = Strings.SaveLabel,
                Priority = 1,
                Order = ToolbarItemOrder.Primary
            };

            ToolbarItems.Add(cancelItem);
            ToolbarItems.Add(saveItem);
        }
    }
}
