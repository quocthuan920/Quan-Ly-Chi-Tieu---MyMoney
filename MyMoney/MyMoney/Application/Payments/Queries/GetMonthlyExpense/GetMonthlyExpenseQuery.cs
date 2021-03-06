using MediatR;
using Microsoft.EntityFrameworkCore;
using MyMoney.Application.Common;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Application.Common.QueryObjects;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyMoney.Application.Payments.Queries.GetMonthlyIncome
{
    public class GetMonthlyExpenseQuery : IRequest<decimal>
    {
        public class Handler : IRequestHandler<GetMonthlyExpenseQuery, decimal>
        {
            private readonly IContextAdapter contextAdapter;
            private readonly ISystemDateHelper systemDateHelper;

            public Handler(IContextAdapter contextAdapter, ISystemDateHelper systemDateHelper)
            {
                this.contextAdapter = contextAdapter;
                this.systemDateHelper = systemDateHelper;
            }

            public async Task<decimal> Handle(GetMonthlyExpenseQuery request, CancellationToken cancellationToken)
            {
                return (await contextAdapter.Context
                                        .Payments
                                        .HasDateLargerEqualsThan(HelperFunctions.GetFirstDayMonth(systemDateHelper))
                                        .HasDateSmallerEqualsThan(HelperFunctions.GetEndOfMonth(systemDateHelper))
                                        .IsExpense()
                                        .Select(x => x.Amount)
                                        .ToListAsync(cancellationToken))
                                        .Sum();
            }
        }
    }
}
