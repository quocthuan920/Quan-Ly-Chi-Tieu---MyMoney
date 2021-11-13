using GalaSoft.MvvmLight.Command;
using MediatR;
using MyMoney.Common;
using MyMoney.Presentation.Dialogs;
using MyMoney.Ui.ViewModels.Statistics;
using SkiaSharp;
using System;
using System.Threading.Tasks;

namespace MyMoney.ViewModels.Statistics
{
    public abstract class MobileStatisticViewModel : StatisticViewModel
    {
        protected MobileStatisticViewModel(IMediator mediator)
            : base(mediator)
        {
        }

        protected MobileStatisticViewModel(DateTime startDate, DateTime endDate, IMediator mediator)
            : base(startDate, endDate, mediator)
        {
            BackgroundColor = SKColor.Parse(ResourceHelper.CurrentBackgroundColor.ToHex());
        }

        public RelayCommand ShowFilterDialogCommand => new RelayCommand(async () => await ShowFilterDialogAsync());

        protected SKColor BackgroundColor { get; }

        private async Task ShowFilterDialogAsync() => await new DateSelectionPopup().ShowAsync();
    }
}
