using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Events
{
    public interface ISendMessageEvent
    {
        event OnSendMessageHandler SentMessageEvent;

        void OnSendMessageEvent(SendMessageEventArgs e);

        async Task OnSendMessageEventAsync(SendMessageEventArgs e) => await Task.Run(() => OnSendMessageEvent(e));
    }
}