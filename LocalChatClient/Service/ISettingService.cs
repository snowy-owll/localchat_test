namespace LocalChatClient.Service
{
    public interface ISettingsService
    {
        string Name { get; set; }
        string ServerPort { get; set; }
        string ServerIp { get; set; }

        void Save();
    }
}