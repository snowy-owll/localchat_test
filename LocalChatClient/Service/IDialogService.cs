namespace LocalChatClient.Service
{
    public interface IDialogService
    {
        bool OpenSettings();
        void ShowMessage(string message);
    }
}
