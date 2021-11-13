﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MyMoney.Extensions;
using Xamarin.Forms;

namespace MyMoney.ViewModels.SetupAssistant
{
    public class CategoryIntroductionViewModel : ViewModelBase
    {
        public RelayCommand GoToAddCategoryCommand
            => new RelayCommand(async () => await Shell.Current.GoToModalAsync(ViewModelLocator.AddCategoryRoute));

        public RelayCommand NextStepCommand
            => new RelayCommand(async () => await Shell.Current.GoToAsync(ViewModelLocator.SetupCompletionRoute));

        public RelayCommand BackCommand
            => new RelayCommand(async () => await Shell.Current.Navigation.PopAsync());
    }
}
