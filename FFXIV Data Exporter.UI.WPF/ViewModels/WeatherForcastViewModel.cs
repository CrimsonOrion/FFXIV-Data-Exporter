using Caliburn.Micro;

using FFXIV_Data_Exporter.Library;
using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Logging;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.UI.WPF.ViewModels
{
    public class WeatherForcastViewModel : Screen
    {
        private readonly ICustomLogger _logger;
        private readonly ISendMessageEvent _sendMessageEvent;
        private readonly IRealm _realm;
        private readonly IWeather _weather;
        private string _status;
        private string _selectedPlaceNames;

        public BindableCollection<string> PlaceNames { get; } = new BindableCollection<string>();
        public string SelectedPlaceNames { get => _selectedPlaceNames; set { _selectedPlaceNames = value; NotifyOfPropertyChange(() => SelectedPlaceNames); } }
        public string Status { get => _status; set { _status = value; NotifyOfPropertyChange(() => Status); } }

        public WeatherForcastViewModel(ICustomLogger logger, ISendMessageEvent sendMessageEvent, IRealm realm, IWeather weather)
        {
            _logger = logger;
            _sendMessageEvent = sendMessageEvent;
            _realm = realm;
            _weather = weather;

            PlaceNames.AddRange(_weather.GetTerritoryPlaceNames());

            //_sendMessageEvent.SentMessageEvent += new OnSendMessageHandler(UpdateStatus);
        }

        public async Task GetForcast() => await _weather.GetWeatherAsync(DateTime.Now, new List<string> { SelectedPlaceNames }, 5, new CancellationToken());

        public void UpdateStatus(object sender, SendMessageEventArgs e) => _sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs($"Hi!:\r\n{e.Message}"));

        private void ErrorMessage(Exception ex, string message)
        {
            _sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs($"{message}:\r\n{ex.Message}"));
            _logger.LogError(ex, $"{message}.");
        }
    }
}