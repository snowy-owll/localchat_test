using System;
using System.Runtime.Serialization;

namespace ChatWCFService
{
    [DataContract]
    public class Message
    {
        public Message(long id, string sender, string content, DateTime time)
        {
            Id = id;
            Sender = sender;
            Content = content;
            Time = time;
        }

        public Message(string sender, string content, DateTime time)
        {
            Sender = sender;
            Content = content;
            Time = time;
        }

        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Sender { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public DateTime Time { get; set; }
    }
}
