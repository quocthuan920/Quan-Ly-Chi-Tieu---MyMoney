using AutoMapper;
using MediatR;
using MyMoney.Application.Categories.Command.CreateCategory;
using MyMoney.Application.Common.Interfaces;
using System.Threading.Tasks;

namespace MyMoney.ViewModels.Categories
{
    public class AddCategoryViewModel : ModifyCategoryViewModel
    {
        private readonly IMediator mediator;

        public AddCategoryViewModel(IMediator mediator,
                                    IMapper mapper,
                                    IDialogService dialogService) : base(mediator, dialogService)
        {
            this.mediator = mediator;
        }

        protected override async Task SaveCategoryAsync()
        {
            await mediator.Send(new CreateCategoryCommand(SelectedCategory.Name, SelectedCategory.Note, SelectedCategory.RequireNote));
        }
    }
}
