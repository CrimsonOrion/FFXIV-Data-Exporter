using Caliburn.Micro;

using FFXIV_Data_Exporter.Library;
using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.UI.WPF.ViewModels
{
    public class WeatherViewModel : Screen
    {
        private readonly ICustomLogger _logger;
        private readonly IWeather _weather;
        private readonly ISendMessageEvent _sendMessageEvent;
        private string _status;

        public string Status { get => _status; set { _status = value; NotifyOfPropertyChange(() => Status); } }

        public WeatherViewModel(ICustomLogger logger, ISendMessageEvent sendMessageEvent, IWeather weather)
        {
            _logger = logger;
            _weather = weather;
            _sendMessageEvent = sendMessageEvent;

            _sendMessageEvent.SentMessageEvent += new OnSendMessageHandler(UpdateStatus);
        }

        public async Task GetForcast(CancellationToken cancellationToken)
        {
            try
            {
                await _weather.GetWeatherAsync();
            }
            catch (Exception ex)
            {
                await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs($"Error getting forcast:\r\n{ex.Message}"));
                _logger.LogError(ex, "Error getting forcast.");
            }
        }

        public async Task GetMoonPhase(CancellationToken cancellationToken)
        {
            try
            {
                await _weather.GetMoonPhaseAsync();
            }
            catch (Exception ex)
            {
                await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs($"Error getting moon phase:\r\n{ex.Message}"));
                _logger.LogError(ex, "Error getting moon phase.");
            }
        }

        public void UpdateStatus(object sender, SendMessageEventArgs e) => Status = $"{e.Message}\r\n{Status}";
    }
}