using ChatWCFService;

namespace LocalChatServer.Service
{
    public interface IWCFHostService
    {
        event System.EventHandler<ClientEventArgs> ClientConnected;
        event System.EventHandler<ClientEventArgs> ClientDisconneced;
        event System.EventHandler<MessageEventArgs> MessageReceived;

        System.Collections.Generic.List<Client> GetConnectedClients();

        System.Threading.Tasks.Task SendMessage(Message message);
        bool Start(string port, Client client);
        void Stop();
    }
}