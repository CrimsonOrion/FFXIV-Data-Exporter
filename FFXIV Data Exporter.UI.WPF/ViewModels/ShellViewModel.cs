using Caliburn.Micro;

using FFXIV_Data_Exporter.Library.Events;

namespace FFXIV_Data_Exporter.UI.WPF.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private string _status;

        private readonly ISendMessageEvent _sendMessageEvent;

        public string Status { get => _status; set { _status = value; NotifyOfPropertyChange(() => Status); } }

        public ShellViewModel(ISendMessageEvent sendMessageEvent)
        {
            _sendMessageEvent = sendMessageEvent;

            _sendMessageEvent.SentMessageEvent += new OnSendMessageHandler(UpdateStatus);
        }

        public void UpdateRealmScreen() => ActivateItem(IoC.Get<UpdateRealmViewModel>());

        public void ExdScreen() => ActivateItem(IoC.Get<ExdViewModel>());

        public void MusicScreen() => ActivateItem(IoC.Get<MusicViewModel>());

        public void WeatherScreen() => ActivateItem(IoC.Get<WeatherViewModel>());

        public void SettingsScreen()
        {
            //ActivateItem(IoC.Get<SettingsViewModel>());
        }

        public void UpdateStatus(object sender, SendMessageEventArgs e) => Status = $"{e.Message}\r\n{Status}";
    }
}