using System.Globalization;

namespace MyMoney.Application
{
    public static class CultureHelper
    {
        public static CultureInfo CurrentCulture { get; set; } = CultureInfo.CurrentCulture;
    }
}
