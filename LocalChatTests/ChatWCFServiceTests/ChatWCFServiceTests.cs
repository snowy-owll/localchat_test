using ChatWCFService;
using DBLocalChat;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LocalChatTests.ChatWCFServiceTests
{
    public class ChatWCFServiceTests
    {
        private Mock<IUnitOfWork> MockUnitOfWork()
        {
            var clients = new List<DBLocalChat.Client>();
            var messages = new List<DBLocalChat.Message>();

            var mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository.Setup(c => c.GetByName(It.IsAny<string>()))
                .Returns<string>((c) => Task.Run(() => clients.Where((e) => e.Name == c).FirstOrDefault()));
            mockClientRepository.Setup(c => c.Add(It.IsAny<DBLocalChat.Client>()))
                .Returns<DBLocalChat.Client>((c) => Task.Run(() => { clients.Add(c); c.Id = clients.IndexOf(c); }));

            var mockMessageRepository = new Mock<IMessageRepository>();
            mockMessageRepository.Setup(m => m.Add(It.IsAny<DBLocalChat.Message>()))
                .Returns<DBLocalChat.Message>(m => Task.Run(() => { messages.Add(m); m.Id = messages.IndexOf(m); }));

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.SetupGet(d => d.Clients).Returns(mockClientRepository.Object);
            mockUnitOfWork.SetupGet(d => d.Messages).Returns(mockMessageRepository.Object);

            return mockUnitOfWork;
        }
        
        [Fact]
        public void ServerClientInConnectedClientsWhenChatServiceCreated()
        {
            var unitOfWork = MockUnitOfWork();

            var serverClientName = "Test server client";
            ChatService chatService = new ChatService(new ChatWCFService.Client(serverClientName), unitOfWork.Object);
            Assert.Equal(serverClientName, chatService.GetConnectedClients().FirstOrDefault()?.Name);
        }

        [Fact]
        public async void EventMessageReceivedRisedWhenSendMessageExecuted()
        {
            var unitOfWork = MockUnitOfWork();

            var serverClientName = "Test server client";
            ChatService chatService = new ChatService(new ChatWCFService.Client(serverClientName), unitOfWork.Object);
            bool messageReceived = false;
            chatService.MessageReceived += (s, e) => { messageReceived = true; };
            await chatService.SendMessage(new ChatWCFService.Message(serverClientName, "Test message", DateTime.Now));
            Assert.True(messageReceived);
        }
    }
}
