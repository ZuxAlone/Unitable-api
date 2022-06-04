using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Dto.Response;
using Unitable.Entities;

namespace Unitable.API.Controller
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ChatController : ControllerBase
    {
        private readonly UnitableDbContext _context;

        public ChatController(UnitableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseGeneric<ICollection<Chat>>>> Get()
        {
            var response = new BaseResponseGeneric<ICollection<Chat>>();
            try
            {
                response.Result = await _context.Chats.ToListAsync();
                response.Success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(DtoChat request)
        {
            var entity = new Chat
            {
                Detalle = request.Detalle,
                CantMensajes = 0,
                Status = true
            };

            _context.Chats.Add(entity);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Tema/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int ChatId)
        {
            var entity = await _context.Chats.FindAsync(ChatId);
            if (entity == null) return NotFound();
            _context.Chats.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{ChatId:int}")]
        public async Task<ActionResult> Put(int ChatId, DtoChat request)
        {
            var ChatFromDb = await _context.Chats.FindAsync(ChatId);
            if (ChatFromDb == null) return NotFound();

            ChatFromDb.Detalle = request.Detalle;

            _context.Chats.Update(ChatFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Chat/{ChatFromDb.Id}");

            return Ok(new { Id = ChatId });
        }
    }
}
