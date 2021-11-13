using Autofac;
using Microsoft.Identity.Client;
using MyMoney.Application;
using MyMoney.Application.Common.Constants;
using MyMoney.AutoMapper;
using MyMoney.Persistence;
using MyMoney.Ui.ViewModels.Settings;
using System;
using Module = Autofac.Module;

namespace MyMoney
{
    public class MyMoneyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ApplicationModule>();
            builder.RegisterModule<PersistenceModule>();

            builder.RegisterInstance(AutoMapperFactory.Create());
            
            builder.Register(c => PublicClientApplicationBuilder
                                 .Create(AppConstants.MSAL_APPLICATION_ID)
                                 .WithRedirectUri($"msal{AppConstants.MSAL_APPLICATION_ID}://auth")
                                 .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                                 .Build());

            builder.RegisterAssemblyTypes(ThisAssembly)
                   .Where(t => t.Name.EndsWith("Service", StringComparison.CurrentCultureIgnoreCase))
                   .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(ThisAssembly)
                   .Where(t => !t.Name.StartsWith("DesignTime", StringComparison.CurrentCultureIgnoreCase))
                   .Where(t => t.Name.EndsWith("ViewModel", StringComparison.CurrentCultureIgnoreCase))
                   .AsSelf();

            builder.RegisterAssemblyTypes(typeof(SettingsViewModel).Assembly)
                   .Where(t => !t.Name.StartsWith("DesignTime", StringComparison.CurrentCultureIgnoreCase))
                   .Where(t => t.Name.EndsWith("ViewModel", StringComparison.CurrentCultureIgnoreCase))
                   .AsSelf();
        }
    }
}
