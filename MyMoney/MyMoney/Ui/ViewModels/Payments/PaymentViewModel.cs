﻿using AutoMapper;
using GalaSoft.MvvmLight;
using MyMoney.Application.Common.Interfaces.Mapping;
using MyMoney.Domain;
using MyMoney.Domain.Entities;
using MyMoney.Ui.ViewModels.Accounts;
using MyMoney.Ui.ViewModels.Categories;
using System;

namespace MyMoney.Ui.ViewModels.Payments
{
    public class PaymentViewModel : ViewModelBase, IHaveCustomMapping
    {
        private const decimal DECIMAL_DELTA = 0.01m;

        private int id;
        private int chargedAccountId;
        private int? targetAccountId;
        private DateTime date;
        private decimal amount;
        private bool isCleared;
        private PaymentType type;
        private string note = "";
        private bool isRecurring;
        private DateTime creationTime;
        private DateTime modificationDate;

        private AccountViewModel chargedAccount = null!;
        private AccountViewModel? targetAccount;
        private CategoryViewModel? categoryViewModel;
        private RecurringPaymentViewModel? recurringPaymentViewModel;

        public PaymentViewModel()
        {
            Date = DateTime.Today;
        }

        public int Id
        {
            get => id;
            set
            {
                if(id == value)
                {
                    return;
                }

                id = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// In case it's a expense or transfer the foreign key to the <see cref="AccountViewModel"/> who will be
        /// charged.     In case it's an income the  foreign key to the <see cref="AccountViewModel"/> who will be
        /// credited.
        /// </summary>
        public int ChargedAccountId
        {
            get => chargedAccountId;
            set
            {
                if(chargedAccountId == value)
                {
                    return;
                }

                chargedAccountId = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Foreign key to the account who will be credited by a transfer.     Not used for the other payment types.
        /// </summary>
        public int? TargetAccountId
        {
            get => targetAccountId;
            set
            {
                if(targetAccountId == value)
                {
                    return;
                }

                targetAccountId = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Date when this payment will be executed.
        /// </summary>
        public DateTime Date
        {
            get => date;
            set
            {
                if(date == value)
                {
                    return;
                }

                date = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Amount of the payment. Has to be >= 0. If the amount is charged or not is based on the payment type.
        /// </summary>
        public decimal Amount
        {
            get => amount;
            set
            {
                if(Math.Abs(amount - value) < DECIMAL_DELTA)
                {
                    return;
                }

                amount = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Indicates if this payment was already executed and the amount already credited or charged to the respective
        ///    account.
        /// </summary>
        public bool IsCleared
        {
            get => isCleared;
            set
            {
                if(isCleared == value)
                {
                    return;
                }

                isCleared = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Type of the payment <see cref="PaymentType"/>.
        /// </summary>
        public PaymentType Type
        {
            get => type;
            set
            {
                if(type == value)
                {
                    return;
                }

                type = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsTransfer));
            }
        }

        /// <summary>
        /// Additional notes to the payment.
        /// </summary>
        public string Note
        {
            get => note;
            set
            {
                if(note == value)
                {
                    return;
                }

                note = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Indicates if the payment will be repeated or if it's a uniquie payment.
        /// </summary>
        public bool IsRecurring
        {
            get => isRecurring;
            set
            {
                if(isRecurring == value)
                {
                    return;
                }

                isRecurring = value;

                RecurringPayment = isRecurring
                                   ? new RecurringPaymentViewModel()
                                   : null;

                RaisePropertyChanged();
            }
        }

        public DateTime CreationTime
        {
            get => creationTime;
            set
            {
                if(creationTime == value)
                {
                    return;
                }

                creationTime = value;
                RaisePropertyChanged();
            }
        }

        public DateTime ModificationDate
        {
            get => modificationDate;
            set
            {
                if(modificationDate == value)
                {
                    return;
                }

                modificationDate = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// In case it's a expense or transfer the account who will be charged.     In case it's an income the account
        ///   who will be credited.
        /// </summary>
        public AccountViewModel ChargedAccount
        {
            get => chargedAccount;
            set
            {
                if(chargedAccount == value)
                {
                    return;
                }

                chargedAccount = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The <see cref="AccountViewModel"/> who will be credited by a transfer.     Not used for the other payment
        ///  types.
        /// </summary>
        public AccountViewModel? TargetAccount
        {
            get => targetAccount;
            set
            {
                if(targetAccount == value)
                {
                    return;
                }

                targetAccount = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The <see cref="Category"/> for this payment
        /// </summary>
        public CategoryViewModel? Category
        {
            get => categoryViewModel;
            set
            {
                if(categoryViewModel == value)
                {
                    return;
                }

                categoryViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The <see cref="RecurringPayment"/> if it's recurring.
        /// </summary>
        public RecurringPaymentViewModel? RecurringPayment
        {
            get => recurringPaymentViewModel;
            set
            {
                if(recurringPaymentViewModel == value)
                {
                    return;
                }

                recurringPaymentViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// This is a shortcut to access if the payment is a transfer or not.
        /// </summary>
        public bool IsTransfer => Type == PaymentType.Transfer;

        private int currentAccountId;

        /// <summary>
        /// Id of the account who currently is used for that view.
        /// </summary>
        public int CurrentAccountId
        {
            get => currentAccountId;
            set
            {
                if(currentAccountId == value)
                {
                    return;
                }

                currentAccountId = value;
                RaisePropertyChanged();
            }
        }

        public void CreateMappings(Profile configuration) => configuration.CreateMap<Payment, PaymentViewModel>().ForMember(x => x.CurrentAccountId, opt => opt.Ignore()).ReverseMap();
    }
}
