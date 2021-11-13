﻿using GalaSoft.MvvmLight;
using MyMoney.Application;
using MyMoney.Application.Common.Facades;
using MyMoney.Application.Common.Interfaces;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MyMoney.Ui.ViewModels.Settings
{
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly ISettingsFacade settingsFacade;
        private readonly IDialogService dialogService;

        public SettingsViewModel(ISettingsFacade settingsFacade,
                                 IDialogService dialogService)
        {
            this.settingsFacade = settingsFacade;
            this.dialogService = dialogService;

            AvailableCultures = new ObservableCollection<CultureInfo>();
        }

        public async Task InitializeAsync() => await LoadAvailableCulturesAsync();

        private CultureInfo selectedCulture = CultureHelper.CurrentCulture;

        public CultureInfo SelectedCulture
        {
            get => selectedCulture;
            set
            {
                if(value == null)
                {
                    return;
                }

                if(selectedCulture == value)
                {
                    return;
                }

                selectedCulture = value;
                settingsFacade.DefaultCulture = selectedCulture.Name;
                CultureHelper.CurrentCulture = selectedCulture;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<CultureInfo> AvailableCultures { get; }

        private async Task LoadAvailableCulturesAsync()
        {
            await dialogService.ShowLoadingDialogAsync();

            CultureInfo.GetCultures(CultureTypes.AllCultures).OrderBy(x => x.Name).ToList().ForEach(AvailableCultures.Add);
            SelectedCulture = AvailableCultures.First(x => x.Name == settingsFacade.DefaultCulture);

            await dialogService.HideLoadingDialogAsync();
        }
    }
}
