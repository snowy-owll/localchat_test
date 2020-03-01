using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DBLocalChat
{
    public class MessageRepository:BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(DBLocalChatContext context): base(context) { }

        public async Task<IEnumerable<Message>> GetBeforeId(long id, int count)
        {
            return await Context.Set<Message>()
                .Where(e => e.Id < id)
                .OrderByDescending(e => e.Id)
                .Take(count)
                .OrderBy(e=>e.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetRecent(int count)
        {
            return await Context.Set<Message>()
                .OrderByDescending(e => e.Id)
                .Take(count)
                .OrderBy(e => e.Id)
                .ToListAsync();
        }
    }
}
