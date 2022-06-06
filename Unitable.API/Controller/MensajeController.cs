using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        [Authorize]
        public async Task<ActionResult<BaseResponseGeneric<ICollection<Mensaje>>>> Get()
        {
            var response = new BaseResponseGeneric<ICollection<Mensaje>>();
            try
            {
                var userPrincipal = GetUserPrincipal();


                response.Result = await _context.Mensajes.Where(msg => (msg.UsuarioId == userPrincipal.Id)).ToListAsync();
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
        [Authorize]
        public async Task<ActionResult> Post(DtoMensaje request)
        {
            var userPrincipal = GetUserPrincipal();
            var ChatFromDb = await _context.Chats.FindAsync(request.ChatId);
            if (ChatFromDb == null)
                return NotFound("Chat #" + request.ChatId + " Not Found");

            var grupoFromDb = await _context.Grupos.Where(gr => gr.ChatId == ChatFromDb.Id).ToListAsync();
            if (grupoFromDb.Count == 0)
                return NotFound("Chat #" + request.ChatId + " No tiene grupo");
            var grupo = grupoFromDb.ElementAt(0);

            var userGrupo = await _context.Usuario_Grupos.Where( ug => ug.UsuarioId == userPrincipal.Id && ug.GrupoId == grupo.Id).ToListAsync();

            if (userGrupo.Count == 0)
                return NotFound("Usuario #" + userPrincipal.Id + " No pertenece al grupo");

            var entity = new Mensaje
            {
                MensajeTexto = request.MensajeTexto,
                HoraMensaje = DateTime.Now,
                UsuarioId = userPrincipal.Id,
                Usuario = userPrincipal,
                ChatId = ChatFromDb.Id,
                Chat = ChatFromDb,

                Status = true
            };

            _context.Mensajes.Add(entity);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Mensaje/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete(int MensajeId)
        {
            var entity = await _context.Mensajes.FindAsync(MensajeId);

            if (entity == null) return NotFound();

            _context.Mensajes.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{MensajeId:int}")]
        [Authorize]
        public async Task<ActionResult> Put(int MensajeId, DtoMensaje request)
        {
            var userPrincipal = GetUserPrincipal();
            var MensajeFromDb = await _context.Mensajes.FindAsync(MensajeId);
            if (MensajeFromDb == null || userPrincipal.Id != MensajeFromDb.UsuarioId) return NotFound();

            MensajeFromDb.MensajeTexto = request.MensajeTexto;

            _context.Mensajes.Update(MensajeFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Mensaje/{MensajeFromDb.Id}");

            return Ok(new { Id = MensajeId });
        }

        private Usuario GetUserPrincipal()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var correo = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            var usuario = _context.Usuarios.FirstOrDefault(user => user.Correo == correo);
            return usuario;
        }
    }
}
