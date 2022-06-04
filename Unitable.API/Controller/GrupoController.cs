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
    public class GrupoController : ControllerBase
    {
        private readonly UnitableDbContext _context;

        public GrupoController(UnitableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseGeneric<ICollection<Grupo>>>> Get()
        {
            var response = new BaseResponseGeneric<ICollection<Grupo>>();

            try
            {
                response.Result = await _context.Grupos.ToListAsync();
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
        public async Task<ActionResult> Post(DtoGrupo request)
        {
            var TemaFromDb = await _context.Temas.FindAsync(request.TemaId);

            var entityChat = new Chat
            {
                Detalle = request.DetalleChat,
                CantMensajes = 0,
                Status = true
            };

            _context.Chats.Add(entityChat);
            await _context.SaveChangesAsync();

            var entity = new Grupo
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                FechaCreacion = DateTime.Now,
                NumUsuarios = 0,
                
                TemaId = TemaFromDb.Id,
                Tema = TemaFromDb,

                ChatId = entityChat.Id,
                Chat = entityChat,
                
                Status = true
            };

            _context.Grupos.Add(entity);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Grupo/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int GrupoId)
        {
            var entity = await _context.Grupos.FindAsync(GrupoId);
            if (entity == null) return NotFound();
            var chatEntity = await _context.Chats.FindAsync(entity.ChatId);
            if (entity != null)
                _context.Chats.Remove(chatEntity);
            _context.Grupos.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{GrupoId:int}")]
        public async Task<ActionResult> Put(int GrupoId, DtoGrupo request)
        {
            var GrupoFromDb = await _context.Grupos.FindAsync(GrupoId);
            if (GrupoFromDb == null) return NotFound();

            GrupoFromDb.Nombre = request.Nombre;
            GrupoFromDb.Descripcion = request.Descripcion;
            GrupoFromDb.TemaId = request.TemaId;
            GrupoFromDb.Tema = await _context.Temas.FindAsync(request.TemaId);

            var ChatFromDb = await _context.Chats.FindAsync(GrupoFromDb.ChatId);
            if (ChatFromDb != null)
            {
                ChatFromDb.Detalle = request.DetalleChat;
                _context.Chats.Update(ChatFromDb);
            }

            _context.Grupos.Update(GrupoFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Grupo/{GrupoFromDb.Id}");

            return Ok(new { Id = GrupoId });
        }
    }

}