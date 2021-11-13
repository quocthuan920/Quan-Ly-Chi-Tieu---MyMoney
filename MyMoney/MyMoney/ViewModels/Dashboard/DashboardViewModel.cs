﻿using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediatR;
using MyMoney.Application.Accounts.Queries.GetAccounts;
using MyMoney.Application.Accounts.Queries.GetIncludedAccountBalanceSummary;
using MyMoney.Application.Accounts.Queries.GetTotalEndOfMonthBalance;
using MyMoney.Application.Common.Messages;
using MyMoney.Application.Payments.Queries.GetMonthlyIncome;
using MyMoney.Extensions;
using MyMoney.Ui.ViewModels.Accounts;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MyMoney.ViewModels.Dashboard
{
    public class DashboardViewModel : ViewModelBase
    {
        private decimal assets;
        private decimal endOfMonthBalance;
        private decimal monthlyIncomes;
        private decimal monthlyExpenses;
        private ObservableCollection<AccountViewModel> accounts = new ObservableCollection<AccountViewModel>();
        private ObservableCollection<DashboardBudgetEntryViewModel> budgetEntries = new ObservableCollection<DashboardBudgetEntryViewModel>();

        private bool isRunning;

        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public DashboardViewModel(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public void Subscribe() => MessengerInstance.Register<ReloadMessage>(this, async (m) => await InitializeAsync());

        public void Unsubscribe() => MessengerInstance.Unregister<ReloadMessage>(this);

        public async Task InitializeAsync()
        {
            if(isRunning)
            {
                return;
            }

            try
            {
                isRunning = true;
                Accounts = mapper.Map<ObservableCollection<AccountViewModel>>(await mediator.Send(new GetAccountsQuery()));
                Accounts.ForEach(async x => x.EndOfMonthBalance = await mediator.Send(new GetAccountEndOfMonthBalanceQuery(x.Id)));

                Assets = await mediator.Send(new GetIncludedAccountBalanceSummaryQuery());
                EndOfMonthBalance = await mediator.Send(new GetTotalEndOfMonthBalanceQuery());
                MonthlyExpenses = await mediator.Send(new GetMonthlyExpenseQuery());
                MonthlyIncomes = await mediator.Send(new GetMonthlyIncomeQuery());
            }
            finally
            {
                isRunning = false;
            }
        }

        public decimal Assets
        {
            get => assets;
            set
            {
                assets = value;
                RaisePropertyChanged();
            }
        }

        public decimal EndOfMonthBalance
        {
            get => endOfMonthBalance;
            set
            {
                endOfMonthBalance = value;
                RaisePropertyChanged();
            }
        }

        public decimal MonthlyIncomes
        {
            get => monthlyIncomes;
            set
            {
                monthlyIncomes = value;
                RaisePropertyChanged();
            }
        }

        public decimal MonthlyExpenses
        {
            get => monthlyExpenses;
            set
            {
                monthlyExpenses = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<DashboardBudgetEntryViewModel> BudgetEntries
        {
            get => budgetEntries;
            private set
            {
                if(budgetEntries == value)
                {
                    return;
                }

                budgetEntries = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<AccountViewModel> Accounts
        {
            get => accounts;
            private set
            {
                if(accounts == value)
                {
                    return;
                }

                accounts = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand GoToAddPaymentCommand => new RelayCommand(async () => await Shell.Current.GoToModalAsync(ViewModelLocator.AddPaymentRoute));
        public RelayCommand GoToAccountsCommand => new RelayCommand(async () => await Shell.Current.GoToAsync(ViewModelLocator.AccountListRoute));
        public RelayCommand GoToBudgetsCommand => new RelayCommand(async () => await Shell.Current.GoToAsync(ViewModelLocator.BudgetListRoute));

        public RelayCommand<AccountViewModel> GoToTransactionListCommand
            => new RelayCommand<AccountViewModel>(async (accountViewModel)
                => await Shell.Current.GoToAsync($"{ViewModelLocator.PaymentListRoute}?accountId={accountViewModel.Id}"));
    }
}
