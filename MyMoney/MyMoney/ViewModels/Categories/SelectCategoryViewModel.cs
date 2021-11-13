﻿using AutoMapper;
using GalaSoft.MvvmLight.Command;
using MediatR;
using MyMoney.Messages;
using MyMoney.Ui.ViewModels.Categories;

namespace MyMoney.ViewModels.Categories
{
    public class SelectCategoryViewModel : CategoryListViewModel
    {
        public SelectCategoryViewModel(IMediator mediator, IMapper mapper) : base(mediator, mapper)
        {
        }

        public RelayCommand<CategoryViewModel> SelectCategoryCommand => new RelayCommand<CategoryViewModel>(async (c) =>
        {
            MessengerInstance.Send(new CategorySelectedMessage(c.Id));
            await App.Current.MainPage.Navigation.PopModalAsync();
        });
    }
}
