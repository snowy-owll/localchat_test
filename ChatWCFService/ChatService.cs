using DBLocalChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ChatWCFService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false)]
    public class ChatService : IChatService
    {
        public ChatService(Client serverClient, IUnitOfWork db)
        {
            this.db = db;
            this.serverClient = serverClient;
            AddClientToDB(serverClient);
        }
        private readonly IUnitOfWork db;
        private readonly Client serverClient;
        private readonly Dictionary<string, (Client client, IChatServiceCallback callback)> clients = new Dictionary<string, (Client, IChatServiceCallback)>();

        private readonly object syncObj = new object();

        public event EventHandler<ClientEventArgs> ClientConnected = delegate { };
        public event EventHandler<ClientEventArgs> ClientDisconneced = delegate { };
        public event EventHandler<MessageEventArgs> MessageReceived = delegate { };

        private IChatServiceCallback CurrentCallback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();
            }
        }

        private bool IsClientConnected(Client client)
        {
            return clients.ContainsKey(client.Name) || client.Name == serverClient.Name;
        }

        private void AddClientToDB(Client client) 
        {
            if (db.Clients.GetByName(client.Name).Result == null)
                db.Clients.Add(new DBLocalChat.Client() { Name = client.Name });
        }

        public Task<bool> Connect(Client client)
        {
            if (!IsClientConnected(client))
            {
                lock (syncObj)
                {
                    clients.Add(client.Name, (client, CurrentCallback));
                    AddClientToDB(client);
                    void clientDisconnected(object s, EventArgs e)
                    {
                        foreach (var (cl, cb) in clients.Values)
                        {
                            if (cb == (IChatServiceCallback)s)
                            {
                                Disconnect(cl);
                                return;
                            }
                        }
                    }
                    OperationContext.Current.Channel.Closed += clientDisconnected;
                    OperationContext.Current.Channel.Faulted += clientDisconnected;
                    ClientConnected(this, new ClientEventArgs(client));
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }

        public Task Disconnect(Client client)
        {
            if (IsClientConnected(client))
            {
                lock (syncObj)
                {
                    clients.Remove(client.Name);
                    ClientDisconneced(this, new ClientEventArgs(client));
                }
            }
            return Task.FromResult<object>(null);
        }

        public async Task<Message[]> GetMessages(long fromId, int count)
        {
            return (await db.Messages.GetBeforeId(fromId, count))
                .ToList()
                .Select(c => new Message(c.Id, c.Client.Name, c.Content, c.Time))
                .ToArray();
        }

        public async Task<Message[]> GetRecentMessages(int count)
        {
            return (await db.Messages.GetRecent(count))
                .ToList()
                .Select(c => new Message(c.Id, c.Client.Name, c.Content, c.Time))
                .ToArray();
        }

        public async Task SendMessage(Message message)
        {
            
            var dbMessage = new DBLocalChat.Message((await db.Clients.GetByName(message.Sender)).Id, message.Time, message.Content);
            await db.Messages.Add(dbMessage);
            message.Id = dbMessage.Id;
            lock (syncObj)
            {
                foreach (var (client, callback) in clients.Values)
                {
                    try
                    {
                        callback.ReceiveMessage(message);
                    }
                    catch
                    {
                        Disconnect(client);
                    }
                }
            }
            MessageReceived(this, new MessageEventArgs(message));
        }

        public List<Client> GetConnectedClients()
        {
            return clients.Values.Select(c => c.client).Append(serverClient).ToList();
        }
    }
}
