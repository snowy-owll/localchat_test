namespace LocalChatServer.Service
{
    public interface ISettingsService
    {
        string Name { get; set; }
        string Port { get; set; }

        void Save();
    }
}