﻿using AutoMapper;
using MediatR;
using MyMoney.Application.Accounts.Queries.GetAccountById;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Application.Payments.Commands.CreatePayment;
using MyMoney.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace MyMoney.ViewModels.Payments
{
    public class AddPaymentViewModel : ModifyPaymentViewModel
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public AddPaymentViewModel(IMediator mediator,
                                   IMapper mapper,
                                   IDialogService dialogService)
            : base(mediator, mapper, dialogService)
        {
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public new async Task InitializeAsync()
        {
            await base.InitializeAsync();
            SelectedPayment.ChargedAccount = ChargedAccounts.FirstOrDefault();
        }

        protected override async Task SavePaymentAsync()
        {
            var payment = new Payment(SelectedPayment.Date,
                                        SelectedPayment.Amount,
                                        SelectedPayment.Type,
                                        await mediator.Send(new GetAccountByIdQuery(SelectedPayment.ChargedAccount.Id)),
                                        SelectedPayment.TargetAccount != null
                                            ? await mediator.Send(new GetAccountByIdQuery(SelectedPayment.TargetAccount.Id))
                                            : null,
                                        mapper.Map<Category>(SelectedPayment.Category),
                                        SelectedPayment.Note);

            if(SelectedPayment.IsRecurring && SelectedPayment.RecurringPayment != null)
            {
                payment.AddRecurringPayment(SelectedPayment.RecurringPayment.Recurrence, SelectedPayment.RecurringPayment.EndDate);
            }

            await mediator.Send(new CreatePaymentCommand(payment));
        }
    }
}
