﻿using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;

namespace MyMoney.Ui.ViewModels.Settings
{
    public interface ISettingsViewModel
    {
        ObservableCollection<CultureInfo> AvailableCultures { get; }

        CultureInfo SelectedCulture { get; set; }

        Task InitializeAsync();
    }
}
