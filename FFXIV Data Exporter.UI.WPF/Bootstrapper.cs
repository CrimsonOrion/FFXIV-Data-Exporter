using Caliburn.Micro;

using FFXIV_Data_Exporter.Library;
using FFXIV_Data_Exporter.Library.Configuration;
using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Exd;
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

            var logfilePath = !string.IsNullOrEmpty(_configuration.GetSection("FilePaths").GetSection("LogfilePath").Value) ?
                _configuration.GetSection("FilePaths").GetSection("LogfilePath").Value :
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FFXIV Data Exporter Log.txt");

            var gamePath = !string.IsNullOrEmpty(_configuration.GetSection("FilePaths").GetSection("GamePath").Value) ?
                _configuration.GetSection("FilePaths").GetSection("GamePath").Value :
                Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "SquareEnix", "FINAL FANTASY XIV - A Realm Reborn")) ?
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "SquareEnix", "FINAL FANTASY XIV - A Realm Reborn") :
                "";

            var language = !string.IsNullOrEmpty(_configuration.GetSection("FilePaths").GetSection("Language").Value) ?
                _configuration.GetSection("FilePaths").GetSection("Language").Value :
                "english";

            var config = new FilePathsModel { LogfilePath = logfilePath, GamePath = gamePath, Language = language };

            ICustomLogger logger = new CustomLogger(new FileInfo(config.LogfilePath), true);

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
                .PerRequest<IWeather, Weather>()
                .PerRequest<IRipMusic, RipMusic>()
                .PerRequest<IAllExd, AllExd>()
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