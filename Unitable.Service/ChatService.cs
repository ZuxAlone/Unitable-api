using Microsoft.EntityFrameworkCore;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Entities;

namespace Unitable.Service
{
    public class ChatService : IChatService
    {
        private readonly UnitableDbContext _context;
        public ChatService(UnitableDbContext context)
        {
            _context = context;
        }

        public async Task<Chat> Delete(int ChatId)
        {
            var entity = await _context.Chats.FindAsync(ChatId);
            if (entity == null)
                return null;
            _context.Chats.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ICollection<Chat>> Get()
        {
            var response = await _context.Chats.ToListAsync();
            return response;
        }

        public async Task<Chat> Post(DtoChat request)
        {
            var entity = new Chat
            {
                Detalle = request.Detalle,
                CantMensajes = 0,
                Status = true
            };

            _context.Chats.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<Chat> Put(int ChatId, DtoChat request)
        {
            var ChatFromDb = await _context.Chats.FindAsync(ChatId);
            if (ChatFromDb == null)
                return null;
            ChatFromDb.Detalle = request.Detalle;
            _context.Chats.Update(ChatFromDb);
            await _context.SaveChangesAsync();
            return ChatFromDb;
        }
    }
}
