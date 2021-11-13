﻿using MyMoney.Application;
using MyMoney.Domain;
using MyMoney.Ui.ViewModels.Payments;

namespace MyMoney.Ui.ConverterLogic
{
    public static class PaymentAmountConverterLogic
    {
        public static string GetAmountSign(PaymentViewModel paymentViewModel)
        {
            string sign;

            sign = paymentViewModel.Type == PaymentType.Transfer
                ? GetSignForTransfer(paymentViewModel)
                : GetSignForNonTransfer(paymentViewModel);

            return $"{sign} {paymentViewModel.Amount.ToString("C2", CultureHelper.CurrentCulture)}";
        }
        private static string GetSignForTransfer(PaymentViewModel payment)
            => payment.ChargedAccountId == payment.CurrentAccountId
                       ? "-"
                       : "+";

        private static string GetSignForNonTransfer(PaymentViewModel payment)
            => payment.Type == (int)PaymentType.Expense
                       ? "-"
                       : "+";
    }
}
