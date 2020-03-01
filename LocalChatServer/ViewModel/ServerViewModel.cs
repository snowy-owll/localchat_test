using DBLocalChat;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using LocalChatServer.Model;
using LocalChatServer.Service;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace LocalChatServer.ViewModel
{
    public class ServerViewModel : ViewModelBase
    {
        public ServerViewModel(IWCFHostService hostService, IUnitOfWork db, ISettingsService settingsService, IDialogService dialogService)
        {
            this.hostService = hostService;
            this.db = db;
            this.settingsService = settingsService;
            this.dialogService = dialogService;

            Client = new ChatWCFService.Client(settingsService.Name);
            Port = settingsService.Port;
            hostService.ClientConnected += (s, e) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ConnectedClients.Add(e.Client);
                    Notifications.Add(new Notification($"Client {e.Client.Name} connected", DateTime.Now, NotificationType.ClientConnected));
                });
            };
            hostService.ClientDisconneced += (s, e) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var client = ConnectedClients.Where(c => c.Name == e.Client.Name).FirstOrDefault();
                    ConnectedClients.Remove(client);
                    Notifications.Add(new Notification($"Client {e.Client.Name} disconnected", DateTime.Now, NotificationType.ClientDisconnected));
                });
            };
            hostService.MessageReceived += (s, e) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    Messages?.Add(new Model.Message(e.Message));
                });
            };
            
            DispatcherHelper.RunAsync(async () =>
            {
                var messages = (await db.Messages.GetAll()).Select(m => new Model.Message(m));
                foreach (var message in messages)
                {
                    Messages.Add(message);
                }
            });
            Messenger.Default.Register<NotificationMessage>(this, (m) => {
                switch (m.Notification)
                {
                    case "ServerWindowClosed": StopServer(); break;
                }
            });
        }

        #region Private fields
        private readonly IWCFHostService hostService;
        private readonly IUnitOfWork db;
        private readonly ISettingsService settingsService;
        private readonly IDialogService dialogService;
        #endregion

        #region Private methods
        private void StopServer()
        {
            if (!ServerStarted) return;
            Client = (Client.Name == settingsService.Name) ? Client : new ChatWCFService.Client(settingsService.Name);
            Port = (Port == settingsService.Port) ? Port : settingsService.Port;
            ConnectedClients.Clear();
            hostService.Stop();
            ServerStarted = false;
            Notifications.Add(new Notification("Server stopped", DateTime.Now, NotificationType.ServerStopped));
        }

        private void StartServer()
        {
            if (string.IsNullOrEmpty(Client.Name))
            {
                dialogService.ShowMessage("Server client name cannot be empty. Please enter the name in the settings.");
                return;
            }
            if (hostService.Start(Port, Client))
            {
                ServerStarted = true;
                var clients = hostService.GetConnectedClients();
                clients.ForEach((c) => ConnectedClients.Add(c));
                Notifications.Add(new Notification("Server started", DateTime.Now, NotificationType.ServerStarted));
            }
            else
            {
                dialogService.ShowMessage($"Failed to start the server. Check port availability.");
                Notifications.Add(new Notification("Failed to start the server. Check port availability.", DateTime.Now, NotificationType.ServerStartFailed));
            }
        }
        #endregion

        #region Properties
        public ObservableCollection<ChatWCFService.Client> ConnectedClients { get; } = new ObservableCollection<ChatWCFService.Client>();
        public ObservableCollection<Model.Message> Messages { get; } = new ObservableCollection<Model.Message>();
        public ObservableCollection<Notification> Notifications { get; } = new ObservableCollection<Notification>();

        private string newMessage;
        public string NewMessage
        {
            get => newMessage;
            set => Set(ref newMessage, value);
        }

        private bool serverStarted;
        public bool ServerStarted
        {
            get => serverStarted;
            set => Set(ref serverStarted, value);
        }

        private ChatWCFService.Client client;
        public ChatWCFService.Client Client
        {
            get => client;
            set => Set(ref client, value);
        }

        private string port;
        public string Port
        {
            get => port;
            set => Set(ref port, value);
        }
        #endregion

        #region Commands
        private RelayCommand sendMessage;
        public RelayCommand SendMessage
        {
            get => sendMessage ?? (sendMessage = new RelayCommand(() => {
                string message = NewMessage.Trim();
                NewMessage = string.Empty;
                hostService.SendMessage(new ChatWCFService.Message(Client.Name, message, DateTime.Now));
            }, () => !string.IsNullOrWhiteSpace(NewMessage) && ServerStarted));
        }

        private RelayCommand close;
        public RelayCommand Close
        {
            get => close ?? (close = new RelayCommand(() => {
                StopServer();
                Messenger.Default.Send(new NotificationMessage("CloseApp"));
            }));
        }

        private RelayCommand stop;
        public RelayCommand Stop
        {
            get => stop ?? (stop = new RelayCommand(() => {
                StopServer();
            }, () => ServerStarted));
        }

        private RelayCommand start;
        public RelayCommand Start
        {
            get => start ?? (start = new RelayCommand(() => {
                StartServer();
            }, () => !ServerStarted));
        }

        private RelayCommand restart;
        public RelayCommand Restart
        {
            get => restart ?? (restart = new RelayCommand(() => {
                StopServer();
                StartServer();
            }, () => ServerStarted));
        }

        private RelayCommand openSettings;
        public RelayCommand OpenSettings
        {
            get => openSettings ?? (openSettings = new RelayCommand(() => {
                if(dialogService.OpenSettings())
                {
                    if (ServerStarted)
                        dialogService.ShowMessage("New settings can be applied only after server restart");
                    else
                    {
                        Client = new ChatWCFService.Client(settingsService.Name);
                        Port = settingsService.Port;
                    }
                }
            }));
        }
        #endregion
    }
}
