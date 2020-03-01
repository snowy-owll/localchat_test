using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBLocalChat
{
    [Table("Client")]
    public class Client
    {
        [Column("ID")]
        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}
