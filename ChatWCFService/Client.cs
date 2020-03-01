using System.Runtime.Serialization;

namespace ChatWCFService
{
    [DataContract]
    public class Client
    {
        public Client() { }
        public Client(string name) { Name = name; }
        
        [DataMember]
        public string Name { get; set; }
    }
}
