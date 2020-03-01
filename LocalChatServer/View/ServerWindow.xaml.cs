using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace LocalChatServer.View
{
    /// <summary>
    /// Interaction logic for ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        public ServerWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (m) => {
                switch (m.Notification)
                {
                    case "CloseApp": { Close(); break; }
                }
            });
            Closed += (s, e) =>
            {
                Messenger.Default.Send(new NotificationMessage("ServerWindowClosed"));
            };
        }
    }
}
