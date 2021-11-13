using AutoMapper;
using GalaSoft.MvvmLight.Command;
using MediatR;
using MyMoney.Application.Categories.Command.UpdateCategory;
using MyMoney.Application.Categories.Command.DeleteCategoryById;
using MyMoney.Application.Categories.Queries.GetCategoryById;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Application.Resources;
using MyMoney.Domain.Entities;
using MyMoney.Ui.ViewModels.Categories;
using System.Threading.Tasks;


using Xamarin.Forms;

namespace MyMoney.ViewModels.Categories
{
    public class EditCategoryViewModel : ModifyCategoryViewModel
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;
        private readonly IDialogService dialogService;
        public EditCategoryViewModel(IMediator mediator,
                                     IMapper mapper,
                                     IDialogService dialogService)
            : base(mediator, dialogService)
        {
            this.mediator = mediator;
            this.mapper = mapper;
            this.dialogService = dialogService;
        }

        public async Task InitializeAsync(int categoryId) => SelectedCategory = mapper.Map<CategoryViewModel>(await mediator.Send(new GetCategoryByIdQuery(categoryId)));

        protected override async Task SaveCategoryAsync() => await mediator.Send(new UpdateCategoryCommand(mapper.Map<Category>(SelectedCategory)));
        public RelayCommand<CategoryViewModel> DeleteCommand
            => new RelayCommand<CategoryViewModel>(async (p) => await DeleteCategoryAsync(p));
        private async Task DeleteCategoryAsync(CategoryViewModel category)
        {
            if (await dialogService.ShowConfirmMessageAsync(Strings.DeleteTitle, Strings.DeleteCategoryConfirmationMessage))
            {
                var deleteCommand = new DeleteCategoryByIdCommand(category.Id);

                try
                {
                    await dialogService.ShowLoadingDialogAsync();
                    await mediator.Send(deleteCommand);
                    await Shell.Current.Navigation.PopModalAsync();
                }
                finally
                {
                    await dialogService.HideLoadingDialogAsync();
                }
            }
        }
    }
}
