using ChatWCFService;
using LocalChatClient.Service;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LocalChatTests.LocalChatClientTests
{
    static class Mock
    {
        public static Mock<ISettingsService> MockSettingsService(string serverIp, string serverPort)
        {
            return MockSettingsService("Test client", serverIp, serverPort);
        }

        public static Mock<ISettingsService> MockSettingsService(string clientName, string serverIp, string serverPort)
        {
            var mock = new Mock<ISettingsService>();
            mock.SetupProperty(s => s.Name, clientName);
            mock.SetupProperty(s => s.ServerIp, serverIp);
            mock.SetupProperty(s => s.ServerPort, serverPort);
            return mock;
        }

        public static Mock<IWCFClientService> MockWCFClientService()
        {
            return MockWCFClientService(ConnectionStatus.ConnectionSuccessful);
        }


        public static Mock<IWCFClientService> MockWCFClientService(ConnectionStatus connectionStatus)
        {
            var mock = new Mock<IWCFClientService>();
            mock.Setup(c => c.Connect(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Client>()))
                .Returns<string, string, Client>((ip, port, client) =>
                Task.FromResult(connectionStatus));
            return mock;
        }

        public static Mock<IDialogService> MockDialogService()
        {
            return MockDialogService(new Action(() => { }));
        }

        public static Mock<IDialogService> MockDialogService(Action messageShowed)
        {
            var mock = new Mock<IDialogService>();
            mock.Setup(d => d.ShowMessage(It.IsAny<string>())).Callback(messageShowed);
            return mock;
        }
    }
}
