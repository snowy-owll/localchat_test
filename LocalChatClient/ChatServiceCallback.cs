using ChatWCFService;
using System;

namespace LocalChatClient
{
    public class ChatServiceCallback : IChatServiceCallback
    {
        public void ReceiveMessage(Message message)
        {
            MessageReceived(this, new MessageReceivedEventArgs(message));
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived = delegate { };
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}
