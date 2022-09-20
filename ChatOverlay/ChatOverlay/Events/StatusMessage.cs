using System;

namespace MaJiCSoft.ChatOverlay.Events
{
    public class StringMessage : PubSubEvent<StringMessage>
    {
        public string Message { get; set; }
    }

    public class StatusMessage : PubSubEvent<StatusMessage>
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Status { get; set; }
    }
}
