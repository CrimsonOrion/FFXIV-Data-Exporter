using Caliburn.Micro;

using FFXIV_Data_Exporter.Library;
using FFXIV_Data_Exporter.UI.WPF.ViewModels;

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
        
        public Bootstrapper() => Initialize();

        protected override void Configure()
        {
            var logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FFXIV Data Exporter Log.txt");

            //ICustomLogger logger = new CustomLogger(new FileInfo(logFile), true);

            var path = Path.Combine(@"G:\", "SquareEnix", "FINAL FANTASY XIV - A Realm Reborn");

            var realm = new Realm(path, "english");
            _container
                .Instance(_container)
                .Instance(realm)
                ;

            //_container
            //    .PerRequest<IFirewallLogProcessor, FirewallLogProcessor>()
            //    ;

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
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