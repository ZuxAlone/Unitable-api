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
    public class MensajeController : ControllerBase
    {
        private readonly UnitableDbContext _context;

        public MensajeController(UnitableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseGeneric<ICollection<Mensaje>>>> Get()
        {
            var response = new BaseResponseGeneric<ICollection<Mensaje>>();
            try
            {
                response.Result = await _context.Mensajes.ToListAsync();
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
        public async Task<ActionResult> Post(DtoMensaje request)
        {
            var UsuarioFromDb = await _context.Usuarios.FindAsync(request.UsuarioId);
            var ChatFromDb = await _context.Chats.FindAsync(request.ChatId);

            var entity = new Mensaje
            {
                MensajeTexto = request.MensajeTexto,
                HoraMensaje = DateTime.Now,
                UsuarioId = UsuarioFromDb.Id,
                ChatId = ChatFromDb.Id,
                Usuario = UsuarioFromDb,
                Chat = ChatFromDb,

                Status = true
            };

            _context.Mensajes.Add(entity);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Mensaje/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int MensajeId)
        {
            var entity = await _context.Mensajes.FindAsync(MensajeId);

            if (entity == null) return NotFound();

            _context.Mensajes.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{MensajeId:int}")]
        public async Task<ActionResult> Put(int MensajeId, DtoMensaje request)
        {
            var MensajeFromDb = await _context.Mensajes.FindAsync(MensajeId);
            if (MensajeFromDb == null) return NotFound();

            MensajeFromDb.MensajeTexto = request.MensajeTexto;

            _context.Mensajes.Update(MensajeFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Mensaje/{MensajeFromDb.Id}");

            return Ok(new { Id = MensajeId });
        }
    }
}
