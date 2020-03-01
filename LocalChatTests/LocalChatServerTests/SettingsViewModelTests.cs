using GalaSoft.MvvmLight.Messaging;
using LocalChatServer.Service;
using LocalChatServer.ViewModel;
using Moq;
using Xunit;

namespace LocalChatTests.LocalChatServerTests
{
    public class SettingsViewModelTests
    {
        private Mock<ISettingsService> MockSettingsService(string clientName, string port)
        {
            var mockSettingsService = new Mock<ISettingsService>();
            mockSettingsService.SetupProperty(s => s.Name, clientName);
            mockSettingsService.SetupProperty(s => s.Port, port);
            return mockSettingsService;
        }
        
        [Fact]
        public void PropertiesHasValuesWhenSettingsViewModelCreated()
        {
            var clientName = "Server client";
            var port = "55677";
            var settingsService = MockSettingsService(clientName, port);
            var settingsViewModel = new SettingsViewModel(settingsService.Object);
            Assert.Equal(clientName, settingsViewModel.ClientName);
            Assert.Equal(port, settingsViewModel.Port);
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
            var settingsService = MockSettingsService(clientName, "55677");
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
        public void CheckPortValidation(string port, bool isValidExpected)
        {
            var settingsService = MockSettingsService("Server client", port);
            var settingsViewModel = new SettingsViewModel(settingsService.Object);
            Assert.Equal(settingsViewModel.IsValid, isValidExpected);
        }

        [Theory]
        [InlineData("", "", false)]
        [InlineData("Server client", "", false)]
        [InlineData("", "55677", false)]
        [InlineData("New name", "55677", true)]
        [InlineData("Server client", "5567", true)]
        [InlineData("New name", "5567", true)]
        public void CheckCanExecuteSave(string newClientName, string newPort, bool canExecuteExpected)
        {
            var settingsService = MockSettingsService("Server client", "55677");
            var settingsViewModel = new SettingsViewModel(settingsService.Object)
            {
                ClientName = newClientName,
                Port = newPort
            };
            settingsViewModel.Validate();
            settingsViewModel.Save.RaiseCanExecuteChanged();
            Assert.Equal(canExecuteExpected, settingsViewModel.Save.CanExecute(null));
        }

        [Fact]
        public void notificationToViewSendedWhenSave()
        {
            var notificationSended = false;
            Messenger.Default.Register<NotificationMessage>(this, (m) =>
            {
                if (m.Notification == "SaveSettingsWindow") notificationSended = true;
            });
            var settingsService = MockSettingsService("Server client", "55677");
            var settingsViewModel = new SettingsViewModel(settingsService.Object)
            {
                ClientName = "New name"
            };
            settingsViewModel.Save.Execute(null);
            Assert.True(notificationSended);
        }

        [Fact]
        public void notificationToViewSendedWhenCancel()
        {
            var notificationSended = false;
            Messenger.Default.Register<NotificationMessage>(this, (m) =>
            {
                if (m.Notification == "CancelSettingsWindow") notificationSended = true;
            });
            var settingsService = MockSettingsService("Server client", "55677");
            var settingsViewModel = new SettingsViewModel(settingsService.Object);
            settingsViewModel.Cancel.Execute(null);
            Assert.True(notificationSended);
        }
    }
}
