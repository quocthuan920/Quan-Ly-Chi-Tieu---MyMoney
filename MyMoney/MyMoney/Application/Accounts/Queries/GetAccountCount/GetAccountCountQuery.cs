using MediatR;
using Microsoft.EntityFrameworkCore;
using MyMoney.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace MyMoney.Application.Accounts.Queries.GetAccountCount
{
    public class GetAccountCountQuery : IRequest<int>
    {
        public class Handler : IRequestHandler<GetAccountCountQuery, int>
        {
            private readonly IContextAdapter contextAdapter;

            public Handler(IContextAdapter contextAdapter)
            {
                this.contextAdapter = contextAdapter;
            }

            public async Task<int> Handle(GetAccountCountQuery request, CancellationToken cancellationToken) => await contextAdapter.Context.Accounts.CountAsync(cancellationToken);
        }
    }
}
