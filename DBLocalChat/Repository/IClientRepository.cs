using System.Threading.Tasks;

namespace DBLocalChat
{
    public interface IClientRepository : IAsyncRepository<Client>
    {
        Task<Client> GetByName(string name);
    }
}
