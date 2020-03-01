using LocalChatClient.View;
using System.Windows;

namespace LocalChatClient.Service
{
    class DialogService : IDialogService
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "LocalChat Client");
        }

        public bool OpenSettings()
        {
            var settingsWindow = new SettingsWindow
            {
                Owner = Application.Current.MainWindow
            };
            return settingsWindow.ShowDialog().Value;
        }
    }
}
