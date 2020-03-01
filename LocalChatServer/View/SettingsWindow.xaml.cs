using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace LocalChatServer.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, HandleNotificationMessage);
            Closed += (s, e) =>
            {
                Messenger.Default.Unregister<NotificationMessage>(this, HandleNotificationMessage);
            };
        }

        private void HandleNotificationMessage(NotificationMessage notificationMessage)
        {
            switch (notificationMessage.Notification)
            {
                case "SaveSettingsWindow":
                    DialogResult = true;
                    Close();
                    break;
                case "CancelSettingsWindow":
                    Close();
                    break;
            }
        }
    }
}
