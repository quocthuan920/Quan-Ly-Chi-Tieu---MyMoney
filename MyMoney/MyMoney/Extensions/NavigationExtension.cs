﻿using MyMoney.Domain.Exceptions;
using NLog;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyMoney.Extensions
{
    public static class NavigationExtension
    {
        private readonly static Logger logger = LogManager.GetCurrentClassLogger();

        public static Task GoToModalAsync(this Shell shell, string route)
        {
            try
            {
                if(!(Routing.GetOrCreateContent(route) is Page page))
                {
                    return Task.CompletedTask;
                }

                return shell.Navigation.PushModalAsync(new NavigationPage(page)
                {
                    BarBackgroundColor = Color.Transparent
                });
            }
            catch(Exception ex)
            {
                var exception = new NavigationException($"Navigation to route {route} failed. ", ex);
                logger.Error(exception);
                throw exception;
            }
        }
    }
}
