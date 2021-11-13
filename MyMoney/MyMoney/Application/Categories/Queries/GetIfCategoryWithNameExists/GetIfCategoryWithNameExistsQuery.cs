using MediatR;
using Microsoft.EntityFrameworkCore;
using MyMoney.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace MyMoney.Application.Categories.Queries.GetIfCategoryWithNameExists
{
    public class GetIfCategoryWithNameExistsQuery : IRequest<bool>
    {
        public GetIfCategoryWithNameExistsQuery(string categoryName)
        {
            CategoryName = categoryName;
        }

        public string CategoryName { get; }

        public class Handler : IRequestHandler<GetIfCategoryWithNameExistsQuery, bool>
        {
            private readonly IEfCoreContext context;

            public Handler(IEfCoreContext context)
            {
                this.context = context;
            }

            public async Task<bool> Handle(GetIfCategoryWithNameExistsQuery request, CancellationToken cancellationToken)
                => await context.Categories.AnyAsync(x => x.Name == request.CategoryName, cancellationToken);
        }
    }
}
