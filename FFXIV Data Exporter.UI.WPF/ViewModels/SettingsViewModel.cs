using Caliburn.Micro;
using FFXIV_Data_Exporter.Library.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFXIV_Data_Exporter.UI.WPF.ViewModels
{
    public class SettingsViewModel : Screen
    {
        AppConfiguration _config;

        public SettingsViewModel(AppConfiguration config)
        {
            _config = config;
        }
    }
}