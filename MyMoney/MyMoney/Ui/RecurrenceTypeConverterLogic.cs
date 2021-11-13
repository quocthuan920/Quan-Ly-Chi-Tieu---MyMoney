﻿using MyMoney.Application.Resources;
using MyMoney.Domain;

namespace MyMoney.Ui
{
    public static class RecurrenceTypeConverterLogic
    {
        public static string GetStringForPaymentRecurrence(PaymentRecurrence passedEnum)
        {
            return passedEnum switch
            {
                PaymentRecurrence.Daily => Strings.DailyLabel,
                PaymentRecurrence.DailyWithoutWeekend => Strings.DailyWithoutWeekendLabel,
                PaymentRecurrence.Weekly => Strings.WeeklyLabel,
                PaymentRecurrence.Biweekly => Strings.BiweeklyLabel,
                PaymentRecurrence.Monthly => Strings.MonthlyLabel,
                PaymentRecurrence.Bimonthly => Strings.BimonthlyLabel,
                PaymentRecurrence.Quarterly => Strings.QuarterlyLabel,
                PaymentRecurrence.Biannually => Strings.BiannuallyLabel,
                PaymentRecurrence.Yearly => Strings.YearlyLabel,
                _ => string.Empty,
            };
        }
    }
}
