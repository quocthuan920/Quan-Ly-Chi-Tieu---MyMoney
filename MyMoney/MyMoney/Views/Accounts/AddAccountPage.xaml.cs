﻿using MyMoney.Application.Resources;
using MyMoney.ViewModels.Accounts;
using Xamarin.Forms;

namespace MyMoney.Views.Accounts
{
    public partial class AddAccountPage
    {
        private AddAccountViewModel ViewModel => (AddAccountViewModel)BindingContext;

        public AddAccountPage()
        {
            InitializeComponent();

            BindingContext = ViewModelLocator.AddAccountViewModel;

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
