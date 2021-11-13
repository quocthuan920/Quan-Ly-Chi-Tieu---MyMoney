﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MyMoney.Application.Common.Extensions;
using MyMoney.Application.Common.Messages;
using System;

namespace MyMoney.ViewModels.Dialogs
{
    public interface ISelectDateRangeDialogViewModel
    {
        DateTime StartDate { get; set; }

        DateTime EndDate { get; set; }

        RelayCommand DoneCommand { get; set; }
    }

    public class SelectDateRangeDialogViewModel : ViewModelBase
    {
        private DateTime startDate;
        private DateTime endDate;

        public SelectDateRangeDialogViewModel()
        {
            StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            EndDate = DateTime.Today.GetLastDayOfMonth();
        }

        /// <summary>
        ///     Initalize the viewmodel with a previous sent message.
        /// </summary>
        public void Initialize(DateSelectedMessage message)
        {
            StartDate = message.StartDate;
            EndDate = message.EndDate;
        }

        /// <summary>
        /// Start Date for the custom date range
        /// </summary>
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// End Date for the custom date range
        /// </summary>
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Selects the dates and notifies observer via the MessageHub
        /// </summary>
        public RelayCommand DoneCommand => new RelayCommand(Done);

        private void Done() => MessengerInstance.Send(new DateSelectedMessage(this, StartDate, EndDate));
    }
}
