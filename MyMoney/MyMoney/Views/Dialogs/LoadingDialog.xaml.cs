﻿using Rg.Plugins.Popup.Extensions;
using System.Threading.Tasks;

namespace MyMoney.Views.Dialogs
{
    public partial class LoadingDialog
    {
        public LoadingDialog()
        {
            InitializeComponent();
        }

        internal static async Task<LoadingDialog> LoadingAsync()
        {
            var dialog = new LoadingDialog();
            await dialog.ShowAsync();

            return dialog;
        }

        public async Task ShowAsync() => await App.Current.MainPage.Navigation.PushPopupAsync(this);

        public async Task DismissAsync() => await App.Current.MainPage.Navigation.PopPopupAsync();
    }
}
