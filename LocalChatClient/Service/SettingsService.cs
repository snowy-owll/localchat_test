using LocalChatClient.Properties;

namespace LocalChatClient.Service
{
    class SettingsService : ISettingsService
    {
        public static SettingsService Instance { get; } = new SettingsService();

        private SettingsService() { }
        public string Name
        {
            get => Settings.Default.Name;
            set => Settings.Default.Name = value;
        }

        public string ServerPort
        {
            get => Settings.Default.ServerPort;
            set => Settings.Default.ServerPort = value;
        }

        public string ServerIp
        {
            get => Settings.Default.ServerIP;
            set => Settings.Default.ServerIP = value;
        }

        public void Save()
        {
            Settings.Default.Save();
        }
    }
}
