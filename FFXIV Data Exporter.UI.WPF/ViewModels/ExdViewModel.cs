using Caliburn.Micro;

using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Exd;
using FFXIV_Data_Exporter.Library.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.UI.WPF.ViewModels
{
    public class ExdViewModel : Screen
    {
        private readonly ICustomLogger _logger;
        private readonly ISendMessageEvent _sendMessageEvent;
        private readonly IAllExd _allExd;
        private string _status;

        public string Status { get => _status; set { _status = value; NotifyOfPropertyChange(() => Status); } }

        public ExdViewModel(ICustomLogger logger, ISendMessageEvent sendMessageEvent, IAllExd allExd)
        {
            _logger = logger;
            _allExd = allExd;
            _sendMessageEvent = sendMessageEvent;

            _sendMessageEvent.SentMessageEvent += new OnSendMessageHandler(UpdateStatus);
        }

        public async Task RipExd(CancellationToken cancellationToken)
        {
            try
            {
                await _allExd.RipAsync(cancellationToken);
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