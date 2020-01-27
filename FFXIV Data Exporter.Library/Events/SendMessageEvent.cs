namespace FFXIV_Data_Exporter.Library.Events
{
    public delegate void OnSendMessageHandler(object sender, SendMessageEventArgs e);
    public class SendMessageEvent : ISendMessageEvent
    {
        public event OnSendMessageHandler SentMessageEvent;

        public void OnSendMessageEvent(SendMessageEventArgs e) => SentMessageEvent(this, e);
    }
}