﻿using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediatR;
using MyMoney.Application.Accounts.Queries.GetAccountById;
using MyMoney.Application.Common.Messages;
using MyMoney.Application.Payments.Queries.GetPaymentsForAccountId;
using MyMoney.Application.Resources;
using MyMoney.Domain;
using MyMoney.Extensions;
using MyMoney.Presentation.Dialogs;
using MyMoney.Ui.Groups;
using MyMoney.Ui.ViewModels.Accounts;
using MyMoney.Ui.ViewModels.Payments;
using MyMoney.Views.Payments;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyMoney.ViewModels.Payments
{
    public class PaymentListViewModel : ViewModelBase
    {
        private AccountViewModel selectedAccount = new AccountViewModel();
        private ObservableCollection<DateListGroupCollection<PaymentViewModel>> payments = new ObservableCollection<DateListGroupCollection<PaymentViewModel>>();
        private PaymentListFilterChangedMessage lastMessage = new PaymentListFilterChangedMessage();

        private bool isRunning;

        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public PaymentListViewModel(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public AccountViewModel SelectedAccount
        {
            get => selectedAccount;
            set
            {
                selectedAccount = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<DateListGroupCollection<PaymentViewModel>> Payments
        {
            get => payments;
            private set
            {
                payments = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     List with the different recurrence types.
        ///     This has to have the same order as the enum
        /// </summary>
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

        public void Subscribe()
        {
            MessengerInstance.Register<ReloadMessage>(this, async (m) => await OnAppearingAsync(SelectedAccount.Id));
            MessengerInstance.Register<PaymentListFilterChangedMessage>(this, async message =>
            {
                lastMessage = message;
                await LoadPaymentsByMessageAsync();
            });
        }

        public void Unsubscribe()
        {
            MessengerInstance.Unregister<ReloadMessage>(this);
            MessengerInstance.Unregister<PaymentListFilterChangedMessage>(this);
        }

        public async Task OnAppearingAsync(int accountId)
        {
            SelectedAccount = mapper.Map<AccountViewModel>(await mediator.Send(new GetAccountByIdQuery(accountId)));
            await LoadPaymentsByMessageAsync();

        }

        private async Task LoadPaymentsByMessageAsync()
        {
            try
            {
                if(isRunning)
                {
                    return;
                }

                isRunning = true;

                List<PaymentViewModel>? paymentVms = mapper.Map<List<PaymentViewModel>>(
                await mediator.Send(new GetPaymentsForAccountIdQuery(SelectedAccount.Id,
                                                                     lastMessage.TimeRangeStart,
                                                                     lastMessage.TimeRangeEnd,
                                                                     lastMessage.IsClearedFilterActive,
                                                                     lastMessage.IsRecurringFilterActive)));

                paymentVms.ForEach(x => x.CurrentAccountId = SelectedAccount.Id);

                List<DateListGroupCollection<PaymentViewModel>> dailyItems = DateListGroupCollection<PaymentViewModel>
                   .CreateGroups(paymentVms,
                                 s => s.Date.ToString("D", CultureInfo.CurrentCulture),
                                 s => s.Date);

                dailyItems.ForEach(CalculateSubBalances);

                Payments = new ObservableCollection<DateListGroupCollection<PaymentViewModel>>(dailyItems);
            }
            finally
            {
                isRunning = false;
            }
        }

        private void CalculateSubBalances(DateListGroupCollection<PaymentViewModel> group)
        {
            group.Subtitle = string.Format(Strings.ExpenseAndIncomeTemplate,
                group.Where(x => x.Type == PaymentType.Expense
                    || (x.Type == PaymentType.Transfer
                        && x.ChargedAccount.Id == SelectedAccount.Id))
                .Sum(x => x.Amount),
                group.Where(x => x.Type == PaymentType.Income
                    || (x.Type == PaymentType.Transfer
                        && x.TargetAccount != null
                        && x.TargetAccount.Id == SelectedAccount.Id))
                .Sum(x => x.Amount));
        }

        public RelayCommand ShowFilterDialogCommand => new RelayCommand(async () => await new FilterPopup().ShowAsync());

        public RelayCommand GoToAddPaymentCommand => new RelayCommand(async () => await Shell.Current.GoToModalAsync(ViewModelLocator.AddPaymentRoute));

        public RelayCommand<PaymentViewModel> GoToEditPaymentCommand
            => new RelayCommand<PaymentViewModel>(async (paymentViewModel)
                => await Shell.Current.Navigation.PushModalAsync(new NavigationPage(new EditPaymentPage(paymentViewModel.Id)) { BarBackgroundColor = Color.Transparent }));
    }
}
