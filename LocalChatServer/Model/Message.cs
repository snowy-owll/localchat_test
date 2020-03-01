﻿using System;

namespace LocalChatServer.Model
{
    public class Message
    {
        public Message(ChatWCFService.Message message)
        {
            Content = message.Content;
            Sender = message.Sender;
            Time = message.Time;
        }

        public Message(DBLocalChat.Message message)
        {
            Content = message.Content;
            Sender = message.Client.Name;
            Time = message.Time;
        }

        public string Content { get; }
        public string Sender { get; }
        public DateTime Time { get; }
    }
}
