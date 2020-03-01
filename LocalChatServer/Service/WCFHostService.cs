using ChatWCFService;
using DBLocalChat;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;

namespace LocalChatServer.Service
{
    class WCFHostService : IWCFHostService
    {
        public WCFHostService(IUnitOfWork db) { this.db = db; }

        private ServiceHost serviceHost;
        private IChatService chatService;
        private readonly IUnitOfWork db;

        #region Events
        public event EventHandler<ClientEventArgs> ClientConnected = delegate { };
        public event EventHandler<ClientEventArgs> ClientDisconneced = delegate { };
        public event EventHandler<MessageEventArgs> MessageReceived = delegate { };
        #endregion

        public List<ChatWCFService.Client> GetConnectedClients()
        {
            return chatService.GetConnectedClients();
        }

        public bool Start(string port, ChatWCFService.Client client)
        {
            Uri tcpAdrs = new Uri($"net.tcp://localhost:{port}");
            chatService = new ChatService(client, db);
            chatService.ClientConnected += (s, e) =>
            {
                ClientConnected(this, e);
            };
            chatService.ClientDisconneced += (s, e) =>
            {
                ClientDisconneced(this, e);
            };
            chatService.MessageReceived += (s, e) =>
            {
                MessageReceived(this, e);
            };
            serviceHost = new ServiceHost(chatService, tcpAdrs);
            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None, true)
            {
                CloseTimeout = new TimeSpan(0, 0, 5),
                OpenTimeout = new TimeSpan(0, 0, 5),
                SendTimeout = new TimeSpan(0, 0, 10),
                ReceiveTimeout = TimeSpan.MaxValue
            };
            tcpBinding.ReliableSession.InactivityTimeout = new TimeSpan(0, 0, 10);
            serviceHost.AddServiceEndpoint(typeof(IChatService), tcpBinding, "chat");

            ServiceMetadataBehavior serviceMetadata = new ServiceMetadataBehavior
            {
                HttpGetEnabled = false
            };
            serviceHost.Description.Behaviors.Add(serviceMetadata);

            serviceHost.AddServiceEndpoint(typeof(IMetadataExchange),
                MetadataExchangeBindings.CreateMexTcpBinding(),
                "mex");

            try
            {
                serviceHost.Open();
                return true;
            }
            catch
            {
                serviceHost = null;
                chatService = null;
                return false;
            }
        }

        public void Stop()
        {
            serviceHost?.Close(TimeSpan.Zero);
            serviceHost = null;
            chatService = null;
        }
        
        public Task SendMessage(ChatWCFService.Message message)
        {
            return chatService.SendMessage(message);
        }
    }
}
