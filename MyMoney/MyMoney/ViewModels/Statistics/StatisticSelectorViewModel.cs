﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MyMoney.Application.Resources;
using MyMoney.Domain;
using MyMoney.Ui.ViewModels.Statistics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyMoney.ViewModels.Statistics
{
    public class StatisticSelectorViewModel : ViewModelBase, IStatisticSelectorViewModel
    {
        /// <summary>
        /// All possible statistic to choose from
        /// </summary>
        public List<StatisticSelectorType> StatisticItems
                                           => new List<StatisticSelectorType>
        {
            
            new StatisticSelectorType
            {
                Name = Strings.CategorySpreadingLabel,
                Description = Strings.CategorieSpreadingDescription,
                Type = StatisticType.CategorySpreading
            },
            new StatisticSelectorType
            {
                Name = Strings.CategorySummaryLabel,
                Description = Strings.CategorySummaryDescription,
                Type = StatisticType.CategorySummary
            }
        };

        /// <summary>
        /// Navigates to the statistic view and shows the selected statistic
        /// </summary>
        public RelayCommand<StatisticSelectorType> GoToStatisticCommand
            => new RelayCommand<StatisticSelectorType>(async (s) => await GoToStatisticAsync(s));

        private async Task GoToStatisticAsync(StatisticSelectorType item)
        {
            if(item.Type == StatisticType.CategorySpreading)
            {
                await Shell.Current.GoToAsync(ViewModelLocator.StatisticCategorySpreadingRoute);
            }
            else if(item.Type == StatisticType.CategorySummary)
            {
                await Shell.Current.GoToAsync(ViewModelLocator.StatisticCategorySummaryRoute);
            }
        }
    }
}
