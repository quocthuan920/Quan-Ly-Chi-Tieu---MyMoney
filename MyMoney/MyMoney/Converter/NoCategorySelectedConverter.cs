using MyMoney.Application.Resources;
using MyMoney.Ui.ViewModels.Categories;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyMoney.Converter
{
    public class NoCategorySelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var category = (CategoryViewModel)value;

            if(category == null)
            {
                return Strings.SelectCategoryTitle;
            }

            return category.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
