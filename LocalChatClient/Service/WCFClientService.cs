using ChatWCFService;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace LocalChatClient.Service
{
    public class WCFClientService : IWCFClientService
    {
        public static WCFClientService Instance { get; } = new WCFClientService();
        private WCFClientService() { }

        private IChatService proxy;

        #region Events
        public event EventHandler<MessageReceivedEventArgs> MessageReceived = delegate { };
        public event EventHandler Disconnected = delegate { };
        #endregion

        public Client Client { get; private set; }

        public async Task<ConnectionStatus> Connect(string serverIp, string port, Client client)
        {
            if (IsConnected) return ConnectionStatus.ConnectionSuccessful;
            try
            {
                var callback = new ChatServiceCallback();
                callback.MessageReceived += (s, e) =>
                {
                    MessageReceived(this, e);
                };
                var instanceContext = new InstanceContext(callback);
                var binding = new NetTcpBinding(SecurityMode.None, true)
                {
                    CloseTimeout = new TimeSpan(0, 0, 5),
                    OpenTimeout = new TimeSpan(0, 0, 10),
                    SendTimeout = new TimeSpan(0, 0, 10)
                };
                binding.ReliableSession.InactivityTimeout = new TimeSpan(0, 0, 5);
                var address = $"net.tcp://{serverIp}:{port}/chat";
                var endpoint = new EndpointAddress(address);
                proxy = DuplexChannelFactory<IChatService>.CreateChannel(instanceContext, binding, endpoint);
                ((IClientChannel)proxy).Faulted += (s, e) => { Disconnect(); Disconnected(this, EventArgs.Empty); };
                ((IClientChannel)proxy).Closed += (s, e) => { Disconnect(); Disconnected(this, EventArgs.Empty); };
                var connected = await proxy.Connect(client);
                if (connected)
                {
                    Client = client;
                    return ConnectionStatus.ConnectionSuccessful;
                }
                return ConnectionStatus.ClientAlreadyConnected;
            }
            catch
            {
                return ConnectionStatus.ConnectionFailed;
            }
        }

        public void Disconnect()
        {
            var client = (IClientChannel)proxy;
            if (client?.State == CommunicationState.Opened)
                try { client.Close(); } catch { }
            Client = null;
            proxy = null;
        }

        public async Task<bool> SendMessage(string message)
        {
            try
            {
                if (!IsConnected) return false;
                await proxy.SendMessage(new Message(Client.Name, message, DateTime.Now));
                return true;
            }
            catch
            {
                Disconnect();
                Disconnected(this, EventArgs.Empty);
                return false;
            }
        }

        public Task<Message[]> GetLastMessages()
        {
            return proxy.GetRecentMessages(20);
        }

        public bool IsConnected
        {
            get => proxy != null && ((IClientChannel)proxy).State == CommunicationState.Opened;
        }
    }

    public enum ConnectionStatus
    {
        ConnectionSuccessful,
        ConnectionFailed,
        ClientAlreadyConnected
    }
}
