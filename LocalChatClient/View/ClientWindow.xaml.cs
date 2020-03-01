using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace LocalChatClient.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        public ClientWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, HandleNotificationMessage);
            Closed += (s, e) =>
            {
                Messenger.Default.Send(new NotificationMessage("ClientWindowClosed"));
                Messenger.Default.Unregister<NotificationMessage>(this, HandleNotificationMessage);
            };
        }

        private void HandleNotificationMessage(NotificationMessage notificationMessage)
        {
            switch (notificationMessage.Notification)
            {
                case "CloseApp": { Close(); break; }
                case "Logout": { new LoginWindow().Show(); Close(); break; }
            }
        }
    }
}
