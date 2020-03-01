using System.Threading.Tasks;

namespace DBLocalChat
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DBLocalChatContext context;
        private ClientRepository clients;
        private MessageRepository messages;

        public UnitOfWork(DBLocalChatContext context)
        {
            this.context = context;
        }

        public IClientRepository Clients => clients ?? (clients = new ClientRepository(context));

        public IMessageRepository Messages => messages ?? (messages = new MessageRepository(context));

        public Task Commit() => context.SaveChangesAsync();
    }
}
