using System;

namespace MyMoney.Application
{
    public class SystemDateHelper : ISystemDateHelper
    {
        public DateTime Today => DateTime.Today;
    }
}
