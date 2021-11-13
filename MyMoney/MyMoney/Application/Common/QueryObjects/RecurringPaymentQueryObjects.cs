using MyMoney.Domain.Entities;
using System;
using System.Linq;

namespace MyMoney.Application.Common.QueryObjects
{
    public static class RecurringPaymentQueryObjects
    {
        public static IQueryable<RecurringPayment> IsNotExpired(this IQueryable<RecurringPayment> queryable) => queryable.Where(x => x.IsEndless || x.EndDate >= DateTime.Today);
    }
}
