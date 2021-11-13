using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediatR;
using MyMoney.Application.Categories.Queries.GetIfCategoryWithNameExists;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Application.Common.Messages;
using MyMoney.Application.Resources;
using MyMoney.Ui.ViewModels.Categories;
using System.Threading.Tasks;

namespace MyMoney.ViewModels.Categories
{
    public abstract class ModifyCategoryViewModel : ViewModelBase
    {
        private readonly IMediator mediator;
        private readonly IDialogService dialogService;

        protected ModifyCategoryViewModel(IMediator mediator,
                                          IDialogService dialogService)
        {
            this.mediator = mediator;
            this.dialogService = dialogService;
        }

        public RelayCommand SaveCommand => new RelayCommand(async () => await SaveCategoryBaseAsync());

        private CategoryViewModel selectedCategory = new CategoryViewModel();

        /// <summary>
        /// The currently selected CategoryViewModel
        /// </summary>
        public CategoryViewModel SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                RaisePropertyChanged();
            }
        }

        protected abstract Task SaveCategoryAsync();

        protected virtual async Task SaveCategoryBaseAsync()
        {
            if(string.IsNullOrEmpty(SelectedCategory.Name))
            {
                await dialogService.ShowMessageAsync(Strings.MandatoryFieldEmptyTitle, Strings.NameRequiredMessage);
                return;
            }

            if(await mediator.Send(new GetIfCategoryWithNameExistsQuery(SelectedCategory.Name)))
            {
                await dialogService.ShowMessageAsync(Strings.DuplicatedNameTitle, Strings.DuplicateCategoryMessage);
                return;
            }

            await dialogService.ShowLoadingDialogAsync(Strings.SavingCategoryMessage);
            await SaveCategoryAsync();
            MessengerInstance.Send(new ReloadMessage());
            await dialogService.HideLoadingDialogAsync();

            await App.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
