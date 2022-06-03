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
    public class Usuario_RecompensaController : ControllerBase
    {
        private readonly UnitableDbContext _context;

        public Usuario_RecompensaController(UnitableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseGeneric<ICollection<Usuario_Recompensa>>>> Get()
        {
            var response = new BaseResponseGeneric<ICollection<Usuario_Recompensa>>();

            try
            {
                response.Result = await _context.Usuario_Recompensas.ToListAsync();
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
        public async Task<ActionResult> Post(DtoUsuario_Recompensa request)
        {
            var entity = new Usuario_Recompensa
            {
                UsuarioId = request.UsuarioId,
                RecompensaId = request.RecompensaId,
                Usuario = await _context.Usuarios.FindAsync(request.UsuarioId),
                Recompensa = await _context.Recompensas.FindAsync(request.RecompensaId),

                Status = true
            };

            _context.Usuario_Recompensas.Add(entity);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Usuario_Recompensa/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int Usuario_RecompensaId)
        {
            var entity = await _context.Usuario_Recompensas.FindAsync(Usuario_RecompensaId);

            if (entity == null) return NotFound();

            _context.Usuario_Recompensas.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{Usuario_RecompensaId:int}")]
        public async Task<ActionResult> Put(int Usuario_RecompensaId, DtoUsuario_Recompensa request)
        {
            var Usuario_RecompensaFromDb = await _context.Usuario_Recompensas.FindAsync(Usuario_RecompensaId);

            if (Usuario_RecompensaFromDb == null) return NotFound();

            Usuario_RecompensaFromDb.UsuarioId = request.UsuarioId;
            Usuario_RecompensaFromDb.RecompensaId = request.RecompensaId;
            Usuario_RecompensaFromDb.Usuario = await _context.Usuarios.FindAsync(request.UsuarioId);
            Usuario_RecompensaFromDb.Recompensa = await _context.Recompensas.FindAsync(request.RecompensaId);

            _context.Usuario_Recompensas.Update(Usuario_RecompensaFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Usuario_Recompensa/{Usuario_RecompensaFromDb.Id}");

            return Ok(new { Id = Usuario_RecompensaId });
        }
    }
}
