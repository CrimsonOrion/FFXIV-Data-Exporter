using Caliburn.Micro;

using FFXIV_Data_Exporter.Library;
using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.UI.WPF.ViewModels
{
    public class UpdateRealmViewModel : Screen
    {

        private readonly ICustomLogger _logger;
        private readonly IRealm _realm;
        private readonly ISendMessageEvent _sendMessageEvent;
        private string _status;

        public string Status { get => _status; set { _status = value; NotifyOfPropertyChange(() => Status); } }

        public UpdateRealmViewModel(ICustomLogger logger, IRealm realm, ISendMessageEvent sendMessageEvent)
        {
            _logger = logger;
            _realm = realm;
            _sendMessageEvent = sendMessageEvent;

            _sendMessageEvent.SentMessageEvent += new OnSendMessageHandler(UpdateStatus);
        }

        public async Task UpdateRealm(CancellationToken cancellationToken)
        {
            try
            {
                await _realm.Update();
            }
            catch (Exception ex)
            {
                await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs($"Error updating:\r\n{ex.Message}"));
                _logger.LogError(ex, "Error updating.");
            }
        }

        public void UpdateStatus(object sender, SendMessageEventArgs e) => Status = $"{e.Message}\r\n{Status}";
    }
}