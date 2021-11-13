using GalaSoft.MvvmLight.Command;
using MyMoney.Ui.ViewModels.Statistics;
using System.Collections.Generic;

namespace MyMoney.ViewModels.Statistics
{
    public interface IStatisticSelectorViewModel
    {
        List<StatisticSelectorType> StatisticItems { get; }

        RelayCommand<StatisticSelectorType> GoToStatisticCommand { get; }
    }
}
