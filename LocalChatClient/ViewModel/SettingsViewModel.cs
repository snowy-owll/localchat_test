using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using LocalChatClient.Service;
using System.Text.RegularExpressions;

namespace LocalChatClient.ViewModel
{
    public class SettingsViewModel : ValidatedViewModelBase
    {
        public SettingsViewModel(ISettingsService settings)
        {
            this.settings = settings;

            ServerIp = settings.ServerIp;
            ServerPort = settings.ServerPort;

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
        private readonly ISettingsService settings;
        #endregion


        #region Properties
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

        public bool SettingsChanged
        {
            get => settings.ServerIp != ServerIp || settings.ServerPort != ServerPort;
        }
        #endregion

        #region Commands
        private RelayCommand save;
        public RelayCommand Save
        {
            get => save ?? (save = new RelayCommand(() =>
            {
                settings.ServerIp = ServerIp;
                settings.ServerPort = ServerPort;
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
