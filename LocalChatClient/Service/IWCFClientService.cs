using System;
using System.Threading.Tasks;
using ChatWCFService;

namespace LocalChatClient.Service
{
    public interface IWCFClientService
    {
        bool IsConnected { get; }
        Client Client { get; }

        event EventHandler Disconnected;
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        Task<ConnectionStatus> Connect(string serverIp, string port, Client client);
        void Disconnect();
        Task<Message[]> GetLastMessages();
        Task<bool> SendMessage(string message);
    }
}