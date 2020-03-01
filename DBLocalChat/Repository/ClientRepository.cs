using System.Threading.Tasks;

namespace DBLocalChat
{
    public class ClientRepository : BaseRepository<Client>, IClientRepository
    {
        public ClientRepository(DBLocalChatContext context) : base(context) { }

        public Task<Client> GetByName(string name) => FirstOrDefault(c => c.Name == name);
        
    }
}
