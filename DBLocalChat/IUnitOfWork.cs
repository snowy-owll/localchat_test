using System.Threading.Tasks;

namespace DBLocalChat
{
    public interface IUnitOfWork
    {
        IClientRepository Clients { get; }
        IMessageRepository Messages { get; }

        Task Commit();
    }
}
