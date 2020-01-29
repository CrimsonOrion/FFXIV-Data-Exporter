using Caliburn.Micro;
using FFXIV_Data_Exporter.Library.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFXIV_Data_Exporter.UI.WPF.ViewModels
{
    public class SettingsViewModel : Screen
    {
        Configuration _config;

        public SettingsViewModel(Configuration config)
        {
            _config = config;
        }
    }
}