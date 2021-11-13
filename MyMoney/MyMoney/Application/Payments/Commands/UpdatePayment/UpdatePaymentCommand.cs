﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using MyMoney.Application.Common;
using MyMoney.Application.Common.CloudBackup;
using MyMoney.Application.Common.Facades;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Domain;
using MyMoney.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyMoney.Application.Payments.Commands.UpdatePayment
{
    public class UpdatePaymentCommand : IRequest
    {
        [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Intended")]
        public UpdatePaymentCommand(int id,
                                    DateTime date,
                                    decimal amount,
                                    bool isCleared,
                                    PaymentType type,
                                    string note,
                                    bool isRecurring,
                                    int categoryId,
                                    int chargedAccountId,
                                    int targetAccountId,
                                    bool updateRecurringPayment,
                                    PaymentRecurrence? recurrence,
                                    bool? isEndless,
                                    DateTime? endDate)
        {
            Id = id;
            Date = date;
            Amount = amount;
            IsCleared = isCleared;
            Type = type;
            Note = note;
            IsRecurring = isRecurring;
            CategoryId = categoryId;
            ChargedAccountId = chargedAccountId;
            TargetAccountId = targetAccountId;
            UpdateRecurringPayment = updateRecurringPayment;
            PaymentRecurrence = recurrence;
            IsEndless = isEndless;
            EndDate = endDate;
        }

        public int Id { get; private set; }

        public DateTime Date { get; private set; }

        public decimal Amount { get; private set; }

        public bool IsCleared { get; private set; }

        public PaymentType Type { get; private set; }

        public string Note { get; private set; }

        public bool IsRecurring { get; private set; }

        public int CategoryId { get; private set; }

        public int ChargedAccountId { get; private set; }

        public int TargetAccountId { get; private set; }

        public PaymentRecurrence? PaymentRecurrence { get; private set; }

        public bool? IsEndless { get; private set; }

        public DateTime? EndDate { get; private set; }

        public bool UpdateRecurringPayment { get; private set; }

        public class Handler : IRequestHandler<UpdatePaymentCommand>
        {
            private readonly IContextAdapter contextAdapter;
            private readonly IBackupService backupService;
            private readonly ISettingsFacade settingsFacade;

            public Handler(IContextAdapter contextAdapter,
                           IBackupService backupService,
                           ISettingsFacade settingsFacade)
            {
                this.contextAdapter = contextAdapter;
                this.backupService = backupService;
                this.settingsFacade = settingsFacade;
            }

            public async Task<Unit> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
            {
                Payment existingPayment = await contextAdapter.Context
                                                              .Payments
                                                              .Include(x => x.ChargedAccount)
                                                              .Include(x => x.TargetAccount)
                                                              .Include(x => x.Category)
                                                              .Include(x => x.RecurringPayment)
                                                              .FirstAsync(x => x.Id == request.Id);

                if(existingPayment == null)
                {
                    return Unit.Value;
                }

                var chargedAccount = await contextAdapter.Context.Accounts.FindAsync(request.ChargedAccountId);
                var targetAccount = await contextAdapter.Context.Accounts.FindAsync(request.TargetAccountId);

                existingPayment.UpdatePayment(request.Date,
                                              request.Amount,
                                              request.Type,
                                              chargedAccount,
                                              targetAccount,
                                              await contextAdapter.Context.Categories.FindAsync(request.CategoryId),
                                              request.Note);

                if(request.IsRecurring && request.UpdateRecurringPayment && request.PaymentRecurrence.HasValue)
                {
                    HandleRecurringPayment(request, existingPayment);
                }
                else if(!request.IsRecurring && existingPayment.RecurringPayment != null)
                {
                    contextAdapter.Context.RecurringPayments
                                          .Remove(existingPayment.RecurringPayment!);

                    List<Payment> linkedPayments = contextAdapter.Context.Payments
                                                                         .Where(x => x.IsRecurring)
                                                                         .Where(x => x.RecurringPayment!.Id == existingPayment.RecurringPayment!.Id)
                                                                         .ToList();

                    linkedPayments.ForEach(x => x.RemoveRecurringPayment());
                }

                await contextAdapter.Context.SaveChangesAsync(cancellationToken);

                settingsFacade.LastDatabaseUpdate = DateTime.Now;
                backupService.UploadBackupAsync().FireAndForgetSafeAsync();

                return Unit.Value;
            }

            private static void HandleRecurringPayment(UpdatePaymentCommand request, Payment existingPayment)
            {
                if(existingPayment.IsRecurring)
                {
                    existingPayment.RecurringPayment!
                                   .UpdateRecurringPayment(request.Amount,
                                                           request.PaymentRecurrence ?? existingPayment.RecurringPayment.Recurrence,
                                                           existingPayment.ChargedAccount,
                                                           request.Note,
                                                           request.IsEndless.HasValue && request.IsEndless.Value
                                                           ? null
                                                           : request.EndDate,
                                                           existingPayment.TargetAccount,
                                                           existingPayment.Category);
                }
                else
                {
                    if(!request.PaymentRecurrence.HasValue)
                    {
                        throw new ArgumentNullException(nameof(request.PaymentRecurrence));
                    }

                    existingPayment.AddRecurringPayment(request.PaymentRecurrence.Value,
                                                        request.IsEndless.HasValue && request.IsEndless.Value
                                                        ? null
                                                        : request.EndDate);
                }
            }
        }
    }
}
