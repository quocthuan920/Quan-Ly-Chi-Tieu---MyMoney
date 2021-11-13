using MyMoney.Application;
using MyMoney.Ui.ViewModels.Accounts;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyMoney.Converter
{
    public class AccountNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is AccountViewModel Account)
                   ? string.Empty
                   : $"{Account.Name} ({Account.CurrentBalance.ToString("C", CultureHelper.CurrentCulture)})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
