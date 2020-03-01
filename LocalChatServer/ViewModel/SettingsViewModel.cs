using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using LocalChatServer.Service;

namespace LocalChatServer.ViewModel
{
    public class SettingsViewModel : ValidatedViewModelBase
    {
        public SettingsViewModel(ISettingsService settings)
        {
            this.settings = settings;

            ClientName = settings.Name;
            Port = settings.Port;

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

            AddValidationRule(() => Port, () =>
            {
                if (string.IsNullOrEmpty(Port))
                    return "Port cannot be empty";
                if (!int.TryParse(Port, out int port))
                    return "Port must be number";
                if (port < 0 || port > 65535)
                    return "Port must be greater than 0 and less than 65535";
                return null;
            });

            Validate();
        }

        #region Private fields
        private readonly ISettingsService settings;
        #endregion


        #region Properties
        private string clientName;
        public string ClientName
        {
            get => clientName;
            set
            {
                Set(ref clientName, value);
            }
        }

        private string port;
        public string Port
        {
            get => port;
            set => Set(ref port, value);
        }

        public bool SettingsChanged
        {
            get => settings.Name != ClientName || settings.Port != Port;
        }
        #endregion

        #region Commands
        private RelayCommand save;
        public RelayCommand Save
        {
            get => save ?? (save = new RelayCommand(() =>
            {
                settings.Name = ClientName;
                settings.Port = Port;
                settings.Save();
                Messenger.Default.Send(new NotificationMessage("SaveSettingsWindow"));
            }, () => IsValid && SettingsChanged));
        }

        private RelayCommand cancel;
        public RelayCommand Cancel
        {
            get => cancel ?? (cancel = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage("CancelSettingsWindow"));
            }));
        }
        #endregion
    }
}