using System;

namespace FFXIV_Data_Exporter.Library.Events
{
    public class SendMessageEventArgs : EventArgs
    {
        public string Message { get; set; }
        public SendMessageEventArgs(string message) => Message = message;
    }
}