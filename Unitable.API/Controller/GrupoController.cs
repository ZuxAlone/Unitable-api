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
    public class GrupoController : ControllerBase
    {
        private readonly UnitableDbContext _context;

        public GrupoController(UnitableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<BaseResponseGeneric<ICollection<Grupo>>>> Get()
        {
            var response = new BaseResponseGeneric<ICollection<Grupo>>();
            var userPrincipal = GetUserPrincipal();
            try
            {
                var usuarioGrupo = await _context.Usuario_Grupos.Where(us => (us.UsuarioId == userPrincipal.Id)).ToListAsync();
                var indexGrupos = usuarioGrupo.Select(ug => ug.GrupoId).ToList();
                response.Result = await _context.Grupos.Where(gr => indexGrupos.Contains(gr.Id)).ToListAsync();
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
        public async Task<ActionResult> Post(DtoGrupo request)
        {
            var usuarioPrincipal = GetUserPrincipal();
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

            var usuario_grupo = new Usuario_Grupo
            {
                UsuarioId = usuarioPrincipal.Id,
                Usuario = usuarioPrincipal,

                GrupoId = entity.Id,
                Grupo = entity
            };
            _context.Usuario_Grupos.Add(usuario_grupo);

            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Grupo/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        [Authorize]
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

        [HttpPost("Join/{GrupoId:int}")]
        [Authorize]
        public async Task<ActionResult> JoinGrupo(int GrupoId)
        {
            var userPrincipal = GetUserPrincipal();

            var grupo = _context.Grupos.FirstOrDefault(gr => gr.Id == GrupoId);
            if (grupo == null)
                return NotFound("Grupo #" + GrupoId + " No Existe");
            
            var usuarioGrupo = await _context.Usuario_Grupos.FirstOrDefaultAsync(us => (us.GrupoId == GrupoId && us.UsuarioId == userPrincipal.Id));
            
            if (usuarioGrupo != null)
                return BadRequest("Usuario #" + userPrincipal.Id + " Ya se encuentra en el grupo #" + GrupoId);

            usuarioGrupo = new Usuario_Grupo
            {
                UsuarioId = userPrincipal.Id,
                Usuario = userPrincipal,
                GrupoId = grupo.Id,
                Grupo = grupo
            };

            _context.Usuario_Grupos.Add(usuarioGrupo);
            await _context.SaveChangesAsync();

            return Ok(new { Id = GrupoId });
        }

        [HttpPut("{GrupoId:int}")]
        [Authorize]
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

        private Usuario GetUserPrincipal()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var correo = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            var usuario = _context.Usuarios.FirstOrDefault(user => user.Correo == correo);
            return usuario;
        }
    }

}