using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace LocalChatClient.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
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
                case "OpenClientWindow":
                    new ClientWindow().Show();
                    Close();
                    break;
            }
        }
    }
}
