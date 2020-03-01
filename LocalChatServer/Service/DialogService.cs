using LocalChatServer.View;
using System.Windows;

namespace LocalChatServer.Service
{
    class DialogService : IDialogService
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "LocalChat Server");
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
