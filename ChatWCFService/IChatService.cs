using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ChatWCFService
{
    [ServiceContract(CallbackContract = typeof(IChatServiceCallback), SessionMode = SessionMode.Required)]
    public interface IChatService
    {
        [OperationContract(IsInitiating = true)]
        Task<bool> Connect(Client client);

        [OperationContract()]
        Task SendMessage(Message message);

        [OperationContract()]
        Task<Message[]> GetMessages(long fromId, int count);

        [OperationContract()]
        Task<Message[]> GetRecentMessages(int count);

        [OperationContract(IsOneWay = true, IsTerminating = true)]
        Task Disconnect(Client client);

        event EventHandler<ClientEventArgs> ClientConnected;
        event EventHandler<ClientEventArgs> ClientDisconneced;
        event EventHandler<MessageEventArgs> MessageReceived;
        List<Client> GetConnectedClients();
    }

    public class ClientEventArgs : EventArgs
    {
        public ClientEventArgs(Client client)
        {
            Client = client;
        }

        public Client Client { get; }
    }

    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}
