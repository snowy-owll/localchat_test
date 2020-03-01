using GalaSoft.MvvmLight.Threading;
using LocalChatClient.Properties;
using System;
using System.Windows;

namespace LocalChatClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (string.IsNullOrEmpty(Settings.Default.Name))
            {
                this.StartupUri = new Uri(@"View\LoginWindow.xaml", UriKind.Relative);
            }
            else
            {
                this.StartupUri = new Uri(@"View\ClientWindow.xaml", UriKind.Relative);
            }
        }
    }
}
