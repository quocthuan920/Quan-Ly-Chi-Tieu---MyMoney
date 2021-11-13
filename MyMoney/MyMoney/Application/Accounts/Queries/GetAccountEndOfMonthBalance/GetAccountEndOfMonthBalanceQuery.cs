﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using MyMoney.Application.Common;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Application.Common.QueryObjects;
using MyMoney.Domain;
using MyMoney.Domain.Entities;
using MyMoney.Domain.Exceptions;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyMoney.Application.Accounts.Queries.GetTotalEndOfMonthBalance
{
    public class GetAccountEndOfMonthBalanceQuery : IRequest<decimal>
    {
        public GetAccountEndOfMonthBalanceQuery(int accountId)
        {
            AccountId = accountId;
        }

        public int AccountId { get; }

        public class Handler : IRequestHandler<GetAccountEndOfMonthBalanceQuery, decimal>
        {
            private readonly Logger logManager = LogManager.GetCurrentClassLogger();

            private readonly IContextAdapter contextAdapter;
            private readonly ISystemDateHelper systemDateHelper;

            public Handler(IContextAdapter contextAdapter, ISystemDateHelper systemDateHelper)
            {
                this.contextAdapter = contextAdapter;
                this.systemDateHelper = systemDateHelper;
            }

            private int accountId;

            public async Task<decimal> Handle(GetAccountEndOfMonthBalanceQuery request, CancellationToken cancellationToken)
            {
                logManager.Info($"Calculate EndOfMonth Balance for account {request.AccountId}.");
                accountId = request.AccountId;

                Account? account = await contextAdapter.Context.Accounts.WithId(accountId).FirstAsync();
                decimal balance = await GetCurrentAccountBalanceAsync();

                foreach(Payment payment in await GetUnclearedPaymentsForThisMonthAsync())
                {
                    if(payment.ChargedAccount == null)
                    {
                        throw new InvalidOperationException($"Navigation Property not initialized properly: {nameof(payment.ChargedAccount)}");
                    }

                    balance = AddPaymentToBalance(payment, account, balance);
                }

                return balance;
            }

            public static decimal AddPaymentToBalance(Payment payment, Account account, decimal currentBalance)
            {
                switch(payment.Type)
                {
                    case PaymentType.Expense:
                        currentBalance -= payment.Amount;
                        break;

                    case PaymentType.Income:
                        currentBalance += payment.Amount;
                        break;

                    case PaymentType.Transfer:
                        currentBalance = CalculateBalanceForTransfer(account, currentBalance, payment);
                        break;

                    default:
                        throw new InvalidPaymentTypeException();
                }

                return currentBalance;
            }

            private static decimal CalculateBalanceForTransfer(Account account, decimal balance, Payment payment)
            {
                if(Equals(account.Id, payment.ChargedAccount!.Id))
                {
                    //Transfer from excluded account
                    balance -= payment.Amount;
                }

                if(payment.TargetAccount == null)
                {
                    throw new InvalidOperationException($"Navigation Property not initialized properly: {nameof(payment.TargetAccount)}");
                }

                if(Equals(account.Id, payment.TargetAccount.Id))
                {
                    //Transfer to excluded account
                    balance += payment.Amount;
                }

                return balance;
            }

            private async Task<decimal> GetCurrentAccountBalanceAsync()
            {
                return (await contextAdapter.Context
                                          .Accounts
                                          .WithId(accountId)
                                          .Select(x => x.CurrentBalance)
                                          .ToListAsync())
                                          .Sum();
            }

            private async Task<List<Payment>> GetUnclearedPaymentsForThisMonthAsync()
            {
                return await contextAdapter.Context
                                           .Payments
                                           .HasAccountId(accountId)
                                           .Include(x => x.ChargedAccount)
                                           .Include(x => x.TargetAccount)
                                           .AreNotCleared()
                                           .HasDateSmallerEqualsThan(HelperFunctions.GetEndOfMonth(systemDateHelper))
                                           .ToListAsync();
            }
        }
    }
}
