using ChatWCFService;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using LocalChatClient.Service;
using System.Collections.ObjectModel;
using System.Linq;

namespace LocalChatClient.ViewModel
{
    public class ClientViewModel : ViewModelBase
    {
        public ClientViewModel(IWCFClientService clientService, ISettingsService settingsService, IDialogService dialogService)
        {
            this.clientService = clientService;
            this.settingsService = settingsService;
            this.dialogService = dialogService;

            ServerIp = settingsService.ServerIp;
            serverPort = settingsService.ServerPort;

            clientService.Disconnected += (s, e) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    DisconnectClient();
                });
            };

            clientService.MessageReceived += (s, e) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    Messages.Add(new Model.Message(e.Message));
                });
            };

            Messenger.Default.Register<NotificationMessage>(this, (m) => {
                switch (m.Notification)
                {
                    case "ClientWindowClosed": DisconnectClient(); break;
                }
            });

            if (clientService.IsConnected)
            {
                Client = clientService.Client;
                ClientConnected = true;
                DispatcherHelper.RunAsync(RequestRecentMessages);
            }
            else
            {
                Client = new Client(settingsService.Name);
                ConnectClient();
            }
        }

        #region Private fields
        private readonly IWCFClientService clientService;
        private readonly ISettingsService settingsService;
        private readonly IDialogService dialogService;
        #endregion

        #region Private methods
        private void DisconnectClient()
        {
            if (!ClientConnected || Connection) return;
            ServerIp = settingsService.ServerIp;
            ServerPort = settingsService.ServerPort;
            clientService.Disconnect();
            ClientConnected = false;
        }

        private async void ConnectClient()
        {
            if (ClientConnected) return;
            Connection = true;
            var s = await clientService.Connect(ServerIp, ServerPort, Client);
            if (s == ConnectionStatus.ConnectionSuccessful)
            {
                ClientConnected = true;
                if (Messages.Count == 0)
                {
                    RequestRecentMessages();
                }
            }
            else if (s == ConnectionStatus.ClientAlreadyConnected)
            {
                dialogService.ShowMessage($"Client with the name '{Client.Name}' already connected.");
            }
            else
            {
                dialogService.ShowMessage("Connection failed. Check server connection settings.");
            }
            Connection = false;
        }

        private async void RequestRecentMessages()
        {
            var messages = (await clientService.GetLastMessages()).Select(m=>new Model.Message(m)).ToList();
            messages.AddRange(Messages);
            Messages.Clear();
            foreach (var message in messages)
            {
                Messages.Add(message);
            }
        }
        #endregion

        #region Properties
        public ObservableCollection<Model.Message> Messages { get; } = new ObservableCollection<Model.Message>();

        private string newMessage;
        public string NewMessage
        {
            get => newMessage;
            set => Set(ref newMessage, value);
        }

        private Client client;
        public Client Client
        {
            get => client;
            set => Set(ref client, value);
        }

        private string serverPort;
        public string ServerPort
        {
            get => serverPort;
            set => Set(ref serverPort, value);
        }

        private string serverIp;
        public string ServerIp
        {
            get => serverIp;
            set => Set(ref serverIp, value);
        }

        private bool clientConnected;
        public bool ClientConnected {
            get => clientConnected;
            set => Set(ref clientConnected, value);
        }

        private bool connection;
        public bool Connection
        {
            get => connection;
            set => Set(ref connection, value);
        }

        private bool sending;
        public bool Sending
        {
            get => sending;
            set => Set(ref sending, value);
        }
        #endregion

        #region Commands
        private RelayCommand sendMessage;
        public RelayCommand SendMessage
        {
            get => sendMessage ?? (sendMessage = new RelayCommand(async () => {
                Sending = true;
                string message = NewMessage.Trim();
                NewMessage = string.Empty;
                if(!(await clientService.SendMessage(message)))
                {
                    dialogService.ShowMessage("Failed to send message. Reconnect to the server and resend the message.");
                    NewMessage = message;
                }
                Sending = false;
            }, () => !string.IsNullOrWhiteSpace(NewMessage) && ClientConnected && !Sending));
        }

        private RelayCommand close;
        public RelayCommand Close
        {
            get => close ?? (close = new RelayCommand(() => {
                DisconnectClient();
                Messenger.Default.Send(new NotificationMessage("CloseApp"));
            }));
        }

        private RelayCommand disconnect;
        public RelayCommand Disconnect
        {
            get => disconnect ?? (disconnect = new RelayCommand(() => {
                DisconnectClient();
            }, () => ClientConnected && !Sending));
        }

        private RelayCommand connect;
        public RelayCommand Connect
        {
            get => connect ?? (connect = new RelayCommand(() => {
                ConnectClient();
            }, () => !ClientConnected && !Connection));
        }

        private RelayCommand reconnect;
        public RelayCommand Reconnect
        {
            get => reconnect ?? (reconnect = new RelayCommand(() => {
                DisconnectClient();
                ConnectClient();
            }, () => ClientConnected && !Connection && !Sending));
        }

        private RelayCommand openSettings;
        public RelayCommand OpenSettings
        {
            get => openSettings ?? (openSettings = new RelayCommand(() => {
                if (dialogService.OpenSettings())
                {
                    if (ClientConnected || Connection)
                        dialogService.ShowMessage("New settings can be applied only after client reconnect");
                    else
                    {
                        ServerIp = settingsService.ServerIp;
                        ServerPort = settingsService.ServerPort;
                    }
                }
            }, ()=>!Connection && !Sending));
        }

        private RelayCommand logout;
        public RelayCommand Logout
        {
            get => logout ?? (logout = new RelayCommand(() =>
            {
                if (ClientConnected)
                    DisconnectClient();
                settingsService.Name = string.Empty;
                settingsService.Save();
                Messenger.Default.Send(new NotificationMessage("Logout"));
            }, ()=>!Connection && !Sending));
        }
        #endregion
    }
}