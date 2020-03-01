using GalaSoft.MvvmLight.Messaging;
using LocalChatClient.ViewModel;
using Xunit;

namespace LocalChatTests.LocalChatClientTests
{
    public class SettingsViewModelTests
    {
        [Fact]
        public void PropertiesHasValuesWhenSettingsViewModelCreated()
        {
            var serverIp = "1.1.1.1";
            var serverPort = "55677";
            var settingsService = Mock.MockSettingsService(serverIp, serverPort);
            var settingsViewModel = new SettingsViewModel(settingsService.Object);
            Assert.Equal(serverIp, settingsViewModel.ServerIp);
            Assert.Equal(serverPort, settingsViewModel.ServerPort);
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
            var settingsService = Mock.MockSettingsService(serverIp, "55677");
            var settingsViewModel = new SettingsViewModel(settingsService.Object);
            Assert.Equal(settingsViewModel.IsValid, isValidExpected);
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
            var settingsService = Mock.MockSettingsService("1.2.3.4", serverPort);
            var settingsViewModel = new SettingsViewModel(settingsService.Object);
            Assert.Equal(settingsViewModel.IsValid, isValidExpected);
        }

        [Theory]
        [InlineData("", "", false)]
        [InlineData("1.2.3.4", "", false)]
        [InlineData("", "55677", false)]
        [InlineData("11.12.13.14", "55677", true)]
        [InlineData("1.2.3.4", "5567", true)]
        [InlineData("11.12.13.14", "5567", true)]
        public void CheckCanExecuteSave(string newServerIp, string newServerPort, bool canExecuteExpected)
        {
            var settingsService = Mock.MockSettingsService("1.2.3.4", "55677");
            var settingsViewModel = new SettingsViewModel(settingsService.Object)
            {
                ServerIp = newServerIp,
                ServerPort = newServerPort
            };
            settingsViewModel.Validate();
            settingsViewModel.Save.RaiseCanExecuteChanged();
            Assert.Equal(canExecuteExpected, settingsViewModel.Save.CanExecute(null));
        }

        [Fact]
        public void NotificationToViewSendedWhenSave()
        {
            var notificationSended = false;
            Messenger.Default.Register<NotificationMessage>(this, (m) =>
            {
                if (m.Notification == "SaveSettingsWindow") notificationSended = true;
            });
            var settingsService = Mock.MockSettingsService("1.2.3.4", "55677");
            var settingsViewModel = new SettingsViewModel(settingsService.Object)
            {
                ServerPort = "5567"
            };
            settingsViewModel.Save.Execute(null);
            Assert.True(notificationSended);
        }

        [Fact]
        public void NotificationToViewSendedWhenCancel()
        {
            var notificationSended = false;
            Messenger.Default.Register<NotificationMessage>(this, (m) =>
            {
                if (m.Notification == "CancelSettingsWindow") notificationSended = true;
            });
            var settingsService = Mock.MockSettingsService("1.2.3.4", "55677");
            var settingsViewModel = new SettingsViewModel(settingsService.Object);
            settingsViewModel.Cancel.Execute(null);
            Assert.True(notificationSended);
        }
    }
}
