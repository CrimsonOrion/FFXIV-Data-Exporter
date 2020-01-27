using Caliburn.Micro;

using FFXIV_Data_Exporter.Library;
using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Logging;

using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.UI.WPF.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private string _status;

        private readonly ICustomLogger _logger;
        private readonly IRealm _realm;
        private readonly IWeather _weather;
        private readonly ISendMessageEvent _sendMessageEvent;

        public string Status { get => _status; set { _status = value; NotifyOfPropertyChange(() => Status); } }

        public ShellViewModel(ICustomLogger logger, IRealm realm, IWeather weather, ISendMessageEvent sendMessageEvent)
        {
            _logger = logger;
            _realm = realm;
            _weather = weather;
            _sendMessageEvent = sendMessageEvent;

            _sendMessageEvent.SentMessageEvent += new OnSendMessageHandler(UpdateStatus);
        }

        public async Task GetWeather(CancellationToken cancellationToken)
        {
            var weather = await _weather.GetWeatherAsync();

            weather.ForEach(async forcast => await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs(forcast)));
        }

        public async Task GetMoonPhase(CancellationToken cancellationToken) => await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs(await MoonPhase.CurrentMoonPhaseAsync()));

        public void UpdateRealmScreen() => ActivateItem(IoC.Get<UpdateRealmViewModel>());

        public void ExdScreen() => ActivateItem(IoC.Get<ExdViewModel>());

        public void MusicScreen() => ActivateItem(IoC.Get<MusicViewModel>());

        public void WeatherScreen()
        {
            //ActivateItem(IoC.Get<WeatherViewModel>());
        }

        public void SettingsScreen()
        {
            //ActivateItem(IoC.Get<SettingsViewModel>());
        }

        public void UpdateStatus(object sender, SendMessageEventArgs e) => Status = $"{e.Message}\r\n{Status}";
    }
}