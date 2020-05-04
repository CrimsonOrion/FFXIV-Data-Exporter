using Caliburn.Micro;

using FFXIV_Data_Exporter.Library;
using FFXIV_Data_Exporter.Library.Configuration;
using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Exd;
using FFXIV_Data_Exporter.Library.Exporting.SQL;
using FFXIV_Data_Exporter.Library.Logging;
using FFXIV_Data_Exporter.Library.Music;
using FFXIV_Data_Exporter.UI.WPF.ViewModels;

using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace FFXIV_Data_Exporter.UI.WPF
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();
        private IConfigurationRoot _configuration;

        public Bootstrapper() => Initialize();

        protected override void Configure()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", false, true)
                .Build();

            var logfilePath = !string.IsNullOrEmpty(_configuration.GetSection("File Paths").GetSection("Logfile Path").Value) ?
                _configuration.GetSection("File Paths").GetSection("Logfile Path").Value :
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FFXIV Data Exporter Log.txt");

            var gamePath = !string.IsNullOrEmpty(_configuration.GetSection("File Paths").GetSection("Game Path").Value) ?
                _configuration.GetSection("File Paths").GetSection("Game Path").Value :
                Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "SquareEnix", "FINAL FANTASY XIV - A Realm Reborn")) ?
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "SquareEnix", "FINAL FANTASY XIV - A Realm Reborn") :
                "";

            var language = !string.IsNullOrEmpty(_configuration.GetSection("ExportSettings").GetSection("Language").Value) ?
                _configuration.GetSection("ExportSettings").GetSection("Language").Value :
                "english";

            var filepaths = new FilePathsModel { LogfilePath = logfilePath, GamePath = gamePath };
            var exportsettings = new ExdSettingsModel { Language = language };

            var config = new AppConfiguration { FilePaths = filepaths, ExportSettings = exportsettings };

            ICustomLogger logger = new CustomLogger(new FileInfo(config.FilePaths.LogfilePath), true);

            _container
                .Instance(_container)
                .Instance(config)
                .Instance(logger)
                ;

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<ISendMessageEvent, SendMessageEvent>()
                .Singleton<IRealm, Realm>()
                ;

            _container
                .PerRequest<IAllExd, AllExd>()
                .PerRequest<IRipMusic, RipMusic>()
                .PerRequest<IOggToScd, OggToScd>()
                .PerRequest<IOggToWav, OggToWav>()
                .PerRequest<IWavToMP3, WavToMP3>()
                .PerRequest<IWeather, Weather>()
                .PerRequest<IMSSqlExport, MSSqlExport>()
                ;

            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e) => DisplayRootViewFor<ShellViewModel>();

        protected override object GetInstance(Type service, string key) => _container.GetInstance(service, key);

        protected override IEnumerable<object> GetAllInstances(Type service) => _container.GetAllInstances(service);

        protected override void BuildUp(object instance) => base.BuildUp(instance);
    }
}