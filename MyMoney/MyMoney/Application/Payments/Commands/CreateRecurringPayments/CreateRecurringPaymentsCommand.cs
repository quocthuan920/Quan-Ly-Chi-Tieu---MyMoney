﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using MyMoney.Application.Common.Helpers;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Application.Common.QueryObjects;
using MyMoney.Domain.Entities;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyMoney.Application.Payments.Commands.CreateRecurringPayments
{
    public class CreateRecurringPaymentsCommand : IRequest
    {
        public class Handler : IRequestHandler<CreateRecurringPaymentsCommand>
        {
            private readonly static Logger logger = LogManager.GetCurrentClassLogger();

            private readonly IContextAdapter contextAdapter;

            public Handler(IContextAdapter contextAdapter)
            {
                this.contextAdapter = contextAdapter;
            }


            public async Task<Unit> Handle(CreateRecurringPaymentsCommand request, CancellationToken cancellationToken)
            {
                List<RecurringPayment> recurringPayments = await contextAdapter.Context
                                                                               .RecurringPayments
                                                                               .Include(x => x.ChargedAccount)
                                                                               .Include(x => x.TargetAccount)
                                                                               .Include(x => x.Category)
                                                                               .Include(x => x.RelatedPayments)
                                                                               .AsQueryable()
                                                                               .IsNotExpired()
                                                                               .ToListAsync();

                var recPaymentsToCreate = recurringPayments
                                                   .Where(x => x.RelatedPayments.Any())
                                                   .Where(x => RecurringPaymentHelper.CheckIfRepeatable(x.RelatedPayments
                                                                                                         .OrderByDescending(d => d.Date)
                                                                                                         .First()))
                                                   .Select(x => new Payment(RecurringPaymentHelper.GetPaymentDateFromRecurring(x),
                                                                            x.Amount,
                                                                            x.Type,
                                                                            x.ChargedAccount,
                                                                            x.TargetAccount,
                                                                            x.Category,
                                                                            x.Note ?? "",
                                                                            x))
                                                   .ToList();

                recPaymentsToCreate.ForEach(x => x.RecurringPayment?.SetLastRecurrenceCreatedDate());

                logger.Info($"Create {recPaymentsToCreate.Count} recurring payments.");

                contextAdapter.Context.Payments.AddRange(recPaymentsToCreate);
                await contextAdapter.Context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
