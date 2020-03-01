using System;

namespace LocalChatServer.Model
{
    public class Notification
    {
        public Notification(string content, DateTime time, NotificationType type)
        {
            Content = content;
            Time = time;
            Type = type;
        }
        public string Content { get; }
        public DateTime Time { get; }
        public NotificationType Type { get; }
    }

    public enum NotificationType
    {
        ClientConnected,
        ClientDisconnected,
        ServerStarted,
        ServerStartFailed,
        ServerStopped
    }
}
