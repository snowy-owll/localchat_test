using GalaSoft.MvvmLight.Messaging;
using LocalChatClient.Service;
using LocalChatClient.ViewModel;
using Xunit;

namespace LocalChatTests.LocalChatClientTests
{
    public class LoginViewModelTests
    {
        [Fact]
        public void PropertiesHasValuesWhenSettingsViewModelCreated()
        {
            var clientName = "Client name";
            var serverIp = "1.1.1.1";
            var serverPort = "55677";
            var settingsService = Mock.MockSettingsService(clientName, serverIp, serverPort);
            var wcfService = Mock.MockWCFClientService();
            var dialogService = Mock.MockDialogService();
            var loginViewModel = new LoginViewModel(settingsService.Object, wcfService.Object, dialogService.Object);
            Assert.Equal(clientName, loginViewModel.ClientName);
            Assert.Equal(serverIp, loginViewModel.ServerIp);
            Assert.Equal(serverPort, loginViewModel.ServerPort);
        }


        [Theory]
        [InlineData("", false)]
        [InlineData("aa", false)]
        [InlineData("aaa", true)]
        [InlineData("aaaaaaaaa", true)]
        [InlineData("aaaaaaaaaaaaaaa", true)]
        [InlineData("aaaaaaaaaaaaaaaa", false)]
        public void CheckClientNameValidation(string clientName, bool isValidExpected)
        {
            var settingsService = Mock.MockSettingsService(clientName, "1.2.3.4", "55677");
            var wcfService = Mock.MockWCFClientService();
            var dialogService = Mock.MockDialogService();
            var loginViewModel = new LoginViewModel(settingsService.Object, wcfService.Object, dialogService.Object);
            Assert.Equal(loginViewModel.IsValid, isValidExpected);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("aa", false)]
        [InlineData("1", false)]
        [InlineData("1.1.1", false)]
        [InlineData("0.0.0.0", true)]
        [InlineData("126.127.128.129", true)]
        [InlineData("255.255.255.255", true)]
        [InlineData("256.256.256.256", false)]
        [InlineData("-1.-1.-1.-1", false)]
        public void CheckServerIpValidation(string serverIp, bool isValidExpected)
        {
            var settingsService = Mock.MockSettingsService("Client name", serverIp, "55677");
            var wcfService = Mock.MockWCFClientService();
            var dialogService = Mock.MockDialogService();
            var loginViewModel = new LoginViewModel(settingsService.Object, wcfService.Object, dialogService.Object);
            Assert.Equal(loginViewModel.IsValid, isValidExpected);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("a", false)]
        [InlineData("5a", false)]
        [InlineData("-1", false)]
        [InlineData("0", true)]
        [InlineData("30000", true)]
        [InlineData("65535", true)]
        [InlineData("65536", false)]
        public void CheckServerPortValidation(string serverPort, bool isValidExpected)
        {
            var settingsService = Mock.MockSettingsService("Client name", "1.2.3.4", serverPort);
            var wcfService = Mock.MockWCFClientService();
            var dialogService = Mock.MockDialogService();
            var loginViewModel = new LoginViewModel(settingsService.Object, wcfService.Object, dialogService.Object);
            Assert.Equal(loginViewModel.IsValid, isValidExpected);
        }

        [Theory]
        [InlineData("", "", "", false)]
        [InlineData("Client name", "", "", false)]
        [InlineData("", "1.2.3.4", "", false)]
        [InlineData("", "", "55677", false)]
        [InlineData("Client name", "11.12.13.14", "55677", true)]
        public void CheckCanExecuteConnect(string newClientName, string newServerIp, string newServerPort, bool canExecuteExpected)
        {
            var settingsService = Mock.MockSettingsService("Client name", "1.2.3.4", "55677");
            var wcfService = Mock.MockWCFClientService();
            var dialogService = Mock.MockDialogService();
            var loginViewModel = new LoginViewModel(settingsService.Object, wcfService.Object, dialogService.Object)
            {
                ClientName = newClientName,
                ServerIp = newServerIp,
                ServerPort = newServerPort
            };
            loginViewModel.Validate();
            loginViewModel.Connect.RaiseCanExecuteChanged();
            Assert.Equal(canExecuteExpected, loginViewModel.Connect.CanExecute(null));
        }

        [Theory]
        [InlineData(ConnectionStatus.ClientAlreadyConnected, true)]
        [InlineData(ConnectionStatus.ConnectionFailed, true)]
        public void MessageShowedWhenConnectionFailed(ConnectionStatus connectionStatus, bool expected)
        {
            var settingsService = Mock.MockSettingsService("Client name", "1.2.3.4", "55677");
            var wcfService = Mock.MockWCFClientService(connectionStatus);
            var messageShowed = false;
            var dialogService = Mock.MockDialogService(() => { messageShowed = true; });
            var loginViewModel = new LoginViewModel(settingsService.Object, wcfService.Object, dialogService.Object);
            loginViewModel.Connect.Execute(null);
            Assert.Equal(expected, messageShowed);
        }

        [Fact]
        public void NotificationSendedWhenConnectionSuccessful()
        {
            var notificationSended = false;
            Messenger.Default.Register<NotificationMessage>(this, (m) =>
            {
                if (m.Notification == "OpenClientWindow") notificationSended = true;
            });
            var settingsService = Mock.MockSettingsService("Client name", "1.2.3.4", "55677");
            var wcfService = Mock.MockWCFClientService(ConnectionStatus.ConnectionSuccessful);
            var dialogService = Mock.MockDialogService();
            var loginViewModel = new LoginViewModel(settingsService.Object, wcfService.Object, dialogService.Object);
            loginViewModel.Connect.Execute(null);
            Assert.True(notificationSended);
        }
    }
}
