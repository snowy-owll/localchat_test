using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBLocalChat
{
    [Table("Message")]
    public class Message
    {
        public Message() { }
        public Message (long idClient, DateTime time, string content)
        {
            IdClient = idClient;
            Time = time;
            Content = content;
        }
        
        [Column("ID")]
        public long Id { get; set; }
        [Column("id_client")]
        [ForeignKey("Client")]
        public long IdClient { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; }

        public virtual Client Client { get; set; }
    }
}
