using MediatR;
using Microsoft.EntityFrameworkCore;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Application.Common.QueryObjects;
using MyMoney.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyMoney.Application.Accounts.Queries.GetAccounts
{
    public class GetAccountsQuery : IRequest<List<Account>>
    {
        public class Handler : IRequestHandler<GetAccountsQuery, List<Account>>
        {
            private readonly IContextAdapter contextAdapter;

            public Handler(IContextAdapter contextAdapter)
            {
                this.contextAdapter = contextAdapter;
            }

            public async Task<List<Account>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
            {
                List<Account>? accounts = await contextAdapter.Context
                                                   .Accounts
                                                   .OrderByInclusion()
                                                   .OrderByName()
                                                   .ToListAsync(cancellationToken);

                return accounts;
            }
        }
    }
}
