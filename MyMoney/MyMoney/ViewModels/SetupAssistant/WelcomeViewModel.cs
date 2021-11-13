﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MyMoney.Application.Common.Facades;
using MyMoney.Extensions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyMoney.ViewModels.SetupAssistant
{
    public class WelcomeViewModel : ViewModelBase
    {
        private readonly ISettingsFacade settingsFacade;

        public WelcomeViewModel(ISettingsFacade settingsFacade)
        {
            this.settingsFacade = settingsFacade;
        }

        public async Task InitAsync()
        {
            if(settingsFacade.IsSetupCompleted)
            {
                await Shell.Current.GoToAsync(ViewModelLocator.DashboardRoute);
            }
        }

        public RelayCommand GoToAddAccountCommand
            => new RelayCommand(async () => await Shell.Current.GoToModalAsync(ViewModelLocator.AddAccountRoute));

        public RelayCommand NextStepCommand => new RelayCommand(async ()
            => await Shell.Current.GoToAsync(ViewModelLocator.CategoryIntroductionRoute));
        public RelayCommand SkipCommand => new RelayCommand(SkipSetup);

        private void SkipSetup()
        {
            settingsFacade.IsSetupCompleted = true;
            Xamarin.Forms.Application.Current.MainPage = new AppShell();
        }
    }
}
