using System.Collections.Generic;
using System.Threading.Tasks;

namespace DBLocalChat
{
    public interface IMessageRepository : IAsyncRepository<Message>
    {
        Task<IEnumerable<Message>> GetBeforeId(long id, int count);

        Task<IEnumerable<Message>> GetRecent(int count);
    }
}
