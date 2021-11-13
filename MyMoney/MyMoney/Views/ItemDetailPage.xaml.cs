using MyMoney.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace MyMoney.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}