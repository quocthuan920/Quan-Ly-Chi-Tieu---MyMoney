using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediatR;
using MyMoney.Application.Common.Interfaces;
using MyMoney.Application.Categories.Command.DeleteCategoryById;
using MyMoney.Application.Categories.Queries.GetCategoryBySearchTerm;
using MyMoney.Application.Common.Messages;
using MyMoney.Application.Resources;
using MyMoney.Extensions;
using MyMoney.Ui.Groups;
using MyMoney.Ui.ViewModels.Categories;
using MyMoney.Views.Categories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyMoney.ViewModels.Categories
{
    public class CategoryListViewModel : ViewModelBase
    {
        private ObservableCollection<AlphaGroupListGroupCollection<CategoryViewModel>> categories = new ObservableCollection<AlphaGroupListGroupCollection<CategoryViewModel>>();

        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public CategoryListViewModel(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;

            MessengerInstance.Register<ReloadMessage>(this, async (m) => await InitializeAsync());
        }

        public async Task InitializeAsync() => await SearchCategoryAsync();

        public ObservableCollection<AlphaGroupListGroupCollection<CategoryViewModel>> Categories
        {
            get => categories;
            private set
            {
                categories = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand GoToAddCategoryCommand => new RelayCommand(async () => await Shell.Current.GoToModalAsync(ViewModelLocator.AddCategoryRoute));

        public RelayCommand<string> SearchCategoryCommand => new RelayCommand<string>(async (searchTerm) => await SearchCategoryAsync(searchTerm));

        private string searchTerm = string.Empty;
        private readonly IDialogService dialogService;

        public string SearchTerm
        {
            get => searchTerm;
            set
            {
                searchTerm = value;
                SearchCategoryCommand.Execute(searchTerm);
            }
        }

        private async Task SearchCategoryAsync(string searchTerm = "")
        {
            List<CategoryViewModel>? categorieVms = mapper.Map<List<CategoryViewModel>>(await mediator.Send(new GetCategoryBySearchTermQuery(searchTerm)));

            List<AlphaGroupListGroupCollection<CategoryViewModel>>? groups = AlphaGroupListGroupCollection<CategoryViewModel>.CreateGroups(categorieVms, CultureInfo.CurrentUICulture,
                s => string.IsNullOrEmpty(s.Name)
                    ? "-"
                    : s.Name[0].ToString(CultureInfo.InvariantCulture).ToUpper(CultureInfo.InvariantCulture));

            Categories = new ObservableCollection<AlphaGroupListGroupCollection<CategoryViewModel>>(groups);
        }

        public RelayCommand<CategoryViewModel> GoToEditCategoryCommand
            => new RelayCommand<CategoryViewModel>(async (categoryViewModel)
                => await Shell.Current.Navigation.PushModalAsync(new NavigationPage(new EditCategoryPage(categoryViewModel.Id)) { BarBackgroundColor = Color.Transparent }));
        public RelayCommand<CategoryViewModel> DeleteCategoryCommand
               => new RelayCommand<CategoryViewModel>(async (categoryViewModel)
                   => await DeleteCategoryAsync(categoryViewModel));

        private async Task DeleteCategoryAsync(CategoryViewModel categoryViewModel)
        {
            if (await dialogService.ShowConfirmMessageAsync(Strings.DeleteTitle,
                Strings.DeleteAccountConfirmationMessage,
                Strings.YesLabel,
                Strings.NoLabel))
            {
                await mediator.Send(new DeleteCategoryByIdCommand(categoryViewModel.Id));
               
            }
        }
    }
}
