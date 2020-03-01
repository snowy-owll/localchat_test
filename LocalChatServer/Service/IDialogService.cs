namespace LocalChatServer.Service
{
    public interface IDialogService
    {
        bool OpenSettings();
        void ShowMessage(string message);
    }
}
