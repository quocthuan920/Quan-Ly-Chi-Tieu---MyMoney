﻿using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediatR;
using MyMoney.Application.Accounts.Queries.GetAccounts;
using MyMoney.Application.Categories.Queries.GetCategoryById;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Application.Common.Messages;
using MyMoney.Application.Resources;
using MyMoney.Domain;
using MyMoney.Domain.Exceptions;
using MyMoney.Extensions;
using MyMoney.Messages;
using MyMoney.Ui.ViewModels.Accounts;
using MyMoney.Ui.ViewModels.Categories;
using MyMoney.Ui.ViewModels.Payments;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyMoney.ViewModels.Payments
{
    public abstract class ModifyPaymentViewModel : ViewModelBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private PaymentViewModel selectedPayment = new PaymentViewModel();

        private ObservableCollection<AccountViewModel> chargedAccounts = new ObservableCollection<AccountViewModel>();
        private ObservableCollection<AccountViewModel> targetAccounts = new ObservableCollection<AccountViewModel>();

        private readonly IMediator mediator;
        private readonly IMapper mapper;
        private readonly IDialogService dialogService;

        protected ModifyPaymentViewModel(IMediator mediator,
                                         IMapper mapper,
                                         IDialogService dialogService)
        {
            this.mediator = mediator;
            this.mapper = mapper;
            this.dialogService = dialogService;

            MessengerInstance.Register<CategorySelectedMessage>(this, async message => await ReceiveMessageAsync(message));
        }

        /// <summary>
        /// The currently selected PaymentViewModel
        /// </summary>
        public PaymentViewModel SelectedPayment
        {
            get => selectedPayment;
            set
            {
                selectedPayment = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gives access to all accounts for Charged Dropdown list
        /// </summary>
        public ObservableCollection<AccountViewModel> ChargedAccounts
        {
            get => chargedAccounts;
            private set
            {
                chargedAccounts = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gives access to all accounts for Target Dropdown list
        /// </summary>
        public ObservableCollection<AccountViewModel> TargetAccounts
        {
            get => targetAccounts;
            private set
            {
                targetAccounts = value;
                RaisePropertyChanged();
            }
        }

        protected virtual async Task InitializeAsync()
        {
            List<AccountViewModel>? accounts = mapper.Map<List<AccountViewModel>>(await mediator.Send(new GetAccountsQuery()));

            ChargedAccounts = new ObservableCollection<AccountViewModel>(accounts);
            TargetAccounts = new ObservableCollection<AccountViewModel>(accounts);
        }

        /// <summary>
        /// Indicates if the PaymentViewModel is a transfer.
        /// </summary>
        public bool IsTransfer => SelectedPayment.IsTransfer;

        [SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static",
            Justification = "Must be non-static in order for the binding to work")]
        public List<PaymentType> PaymentTypeList => new List<PaymentType>
        {
            PaymentType.Expense,
            PaymentType.Income,
            PaymentType.Transfer
        };

        /// <summary>
        /// List with the different recurrence types.     This has to have the same order as the enum
        /// </summary>
        [SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static",
             Justification = "Must be non-static in order for the binding to work")]
        public List<PaymentRecurrence> RecurrenceList => new List<PaymentRecurrence>
        {
            PaymentRecurrence.Daily,
            PaymentRecurrence.DailyWithoutWeekend,
            PaymentRecurrence.Weekly,
            PaymentRecurrence.Biweekly,
            PaymentRecurrence.Monthly,
            PaymentRecurrence.Bimonthly,
            PaymentRecurrence.Quarterly,
            PaymentRecurrence.Biannually,
            PaymentRecurrence.Yearly
        };

        public string AccountHeader
              => SelectedPayment?.Type == PaymentType.Income
                 ? Strings.TargetAccountLabel
                 : Strings.ChargedAccountLabel;

        public RelayCommand GoToSelectCategoryDialogCommand => new RelayCommand(async () => await Shell.Current.GoToModalAsync(ViewModelLocator.SelectCategoryRoute));
        public RelayCommand ResetCategoryCommand => new RelayCommand(() => SelectedPayment.Category = null);

        public RelayCommand SaveCommand => new RelayCommand(async () => await SavePaymentBaseAsync());

        protected abstract Task SavePaymentAsync();

        private async Task SavePaymentBaseAsync()
        {
            if(SelectedPayment.ChargedAccount == null)
            {
                await dialogService.ShowMessageAsync(Strings.MandatoryFieldEmptyTitle, Strings.AccountRequiredMessage);
                return;
            }

            if(SelectedPayment.Amount < 0)
            {
                await dialogService.ShowMessageAsync(Strings.AmountMayNotBeNegativeTitle, Strings.AmountMayNotBeNegativeMessage);
                return;
            }

            if((SelectedPayment.Category?.RequireNote == true) && string.IsNullOrEmpty(SelectedPayment.Note))
            {
                await dialogService.ShowMessageAsync(Strings.MandatoryFieldEmptyTitle, Strings.ANoteForPaymentIsRequired);
                return;
            }

            await dialogService.ShowLoadingDialogAsync(Strings.SavingPaymentMessage);

            try
            {
                await SavePaymentAsync();
                MessengerInstance.Send(new ReloadMessage());
                await App.Current.MainPage.Navigation.PopModalAsync();

            }
            catch(InvalidEndDateException)
            {
                await dialogService.ShowMessageAsync(Strings.InvalidEnddateTitle, Strings.InvalidEnddateMessage);
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                await dialogService.HideLoadingDialogAsync();
            }
        }

        private async Task ReceiveMessageAsync(CategorySelectedMessage message)
        {
            if(SelectedPayment == null || message == null)
            {
                return;
            }

            SelectedPayment.Category = mapper.Map<CategoryViewModel>(await mediator.Send(new GetCategoryByIdQuery(message.CategoryId)));
        }
    }
}
