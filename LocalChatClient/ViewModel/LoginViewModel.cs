using ChatWCFService;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using LocalChatClient.Service;
using System.Text.RegularExpressions;

namespace LocalChatClient.ViewModel
{
    public class LoginViewModel : ValidatedViewModelBase
    {
        public LoginViewModel(ISettingsService settingsService, IWCFClientService clientService, IDialogService dialogService)
        {
            this.settingsService = settingsService;
            this.clientService = clientService;
            this.dialogService = dialogService;
            ClientName = settingsService.Name;
            ServerIp = settingsService.ServerIp;
            ServerPort = settingsService.ServerPort;

            AddValidationRule(() => ClientName, () =>
            {
                if (string.IsNullOrEmpty(ClientName))
                    return "Client name cannot be empty";
                if (ClientName.Length < 3)
                    return "Client name must be longer than 3 characters";
                if (ClientName.Length > 15)
                    return "Client name must be no longer than 15 characters";
                return null;
            });

            AddValidationRule(() => ServerPort, () =>
            {
                if (string.IsNullOrEmpty(ServerPort))
                    return "Server port cannot be empty";
                if (!int.TryParse(ServerPort, out int port))
                    return "Server port must be number";
                if (port < 0 || port > 65535)
                    return "Server port must be greater than 0 and less than 65535";
                return null;
            });

            var ipPattern = "^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            AddValidationRule(() => ServerIp, () =>
            {
                if (string.IsNullOrEmpty(ServerIp))
                    return "Server ip cannot be empty";
                if (!Regex.IsMatch(ServerIp, ipPattern, RegexOptions.Compiled))
                    return "Server address is incorrect";
                return null;
            });

            Validate();
        }

        #region Private fields
        private readonly ISettingsService settingsService;
        private readonly IWCFClientService clientService;
        private readonly IDialogService dialogService;
        #endregion

        #region Properties
        private string clientName;
        public string ClientName
        {
            get => clientName;
            set => Set(ref clientName, value);
        }

        private bool connection;
        public bool Connection
        {
            get => connection;
            set => Set(ref connection, value);
        }

        private string serverIp;
        public string ServerIp
        {
            get => serverIp;
            set => Set(ref serverIp, value);
        }

        private string serverPort;
        public string ServerPort
        {
            get => serverPort;
            set => Set(ref serverPort, value);
        }
        #endregion

        #region Commands
        private RelayCommand connect;
        public RelayCommand Connect
        {
            get => connect ?? (connect = new RelayCommand(async () =>
            {
                Connection = true;
                var s = await clientService.Connect(ServerIp, ServerPort, new Client(ClientName));
                if(s == ConnectionStatus.ConnectionSuccessful)
                {
                    settingsService.Name = ClientName;
                    settingsService.ServerIp = ServerIp;
                    settingsService.ServerPort = ServerPort;
                    settingsService.Save();
                    Messenger.Default.Send(new NotificationMessage("OpenClientWindow"));
                } else if (s == ConnectionStatus.ClientAlreadyConnected)
                {
                    dialogService.ShowMessage($"Client with the name '{ClientName}' already connected.");
                } else
                {
                    dialogService.ShowMessage("Connection failed. Check server connection settings.");
                }
                Connection = false;
            }, () => IsValid && !Connection));
        }
        #endregion
    }
}
