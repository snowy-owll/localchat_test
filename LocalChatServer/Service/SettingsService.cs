using LocalChatServer.Properties;

namespace LocalChatServer.Service
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

        public string Port
        {
            get => Settings.Default.Port;
            set => Settings.Default.Port = value;
        }

        public void Save()
        {
            Settings.Default.Save();
        }
    }
}
