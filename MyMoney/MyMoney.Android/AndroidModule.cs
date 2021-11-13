using Autofac;
using GalaSoft.MvvmLight.Messaging;
using MyMoney.Droid.Src;
using Plugin.Toasts;

namespace MyMoney.Droid
{
    public class AndroidModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GraphClientFactory>().AsImplementedInterfaces();
            builder.RegisterType<DroidAppInformation>().AsImplementedInterfaces();
            builder.RegisterType<ToastNotification>().AsImplementedInterfaces();
            builder.Register(c => new FileStoreIoBase(Android.App.Application.Context.FilesDir.Path)).AsImplementedInterfaces();
            builder.RegisterInstance(Messenger.Default).AsImplementedInterfaces();

            builder.RegisterModule<MyMoneyModule>();
        }
    }
}
