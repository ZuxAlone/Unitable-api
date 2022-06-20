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
    public class RecompensaController : ControllerBase
    {
        private readonly UnitableDbContext _context;

        public RecompensaController(UnitableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Recompensa>> Get()
        {
            var userPrincipal = GetUserPrincipal();
            var usuario_recompesas = await _context.Usuario_Recompensas.Where(us => (us.UsuarioId == userPrincipal.Id)).ToListAsync();

            List<Recompensa> recompensas = new List<Recompensa>();
            List<Recompensa> recompensasnot = new List<Recompensa>();

            recompensasnot = _context.Recompensas.ToList();

            foreach (var usuario_recompesa in usuario_recompesas)
            {
                recompensas.Add(await _context.Recompensas.FindAsync(usuario_recompesa.RecompensaId));
            }

            recompensasnot = recompensasnot.Except(recompensas).ToList();

            return Ok(recompensasnot);

        }

        [HttpPost]
        public async Task<ActionResult> Post(DtoRecompensa request)
        {
            var entity = new Recompensa
            {
                Nombre = request.Nombre,
                Detalle = request.Detalle,
                PrecioMon = request.PrecioMon,

                Status = true
            };

            _context.Recompensas.Add(entity);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Recompensa/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int RecompensaId)
        {
            var entity = await _context.Recompensas.FindAsync(RecompensaId);


            if (entity == null) return NotFound();

            foreach (Usuario_Recompensa element in _context.Usuario_Recompensas)
            {
                if (element.RecompensaId == RecompensaId)
                {
                    _context.Usuario_Recompensas.Remove(element);
                }
            }

            _context.Recompensas.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{RecompensaId:int}")]
        public async Task<ActionResult> Put(int RecompensaId, DtoRecompensa request)
        {
            var RecompensaFromDb = await _context.Recompensas.FindAsync(RecompensaId);

            if (RecompensaFromDb == null) return NotFound();

            RecompensaFromDb.Nombre = request.Nombre;
            RecompensaFromDb.Detalle = request.Detalle;
            RecompensaFromDb.PrecioMon = request.PrecioMon;

            _context.Recompensas.Update(RecompensaFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Recompensa/{RecompensaFromDb.Id}");

            return Ok(new { Id = RecompensaId });
        }

        [HttpPost("buy/{recompensaId:int}")]
        [Authorize]
        public async Task<ActionResult<Usuario_Recompensa>> BuyRecompensa(int recompensaId)
        {
            var userPrincipal = GetUserPrincipal();

            var recompensaDb = await _context.Recompensas.FindAsync(recompensaId);

            if (recompensaDb == null) return NotFound(new { mensaje = "No existe esta recompensa" });
            if (userPrincipal.NumMonedas < recompensaDb.PrecioMon) return Problem("No tienes monedas suficientes");

            userPrincipal.NumMonedas -= recompensaDb.PrecioMon;

            var usuario_recompensa = new Usuario_Recompensa
            {
                UsuarioId = userPrincipal.Id,
                RecompensaId = recompensaId,
                Usuario = userPrincipal,
                Recompensa = recompensaDb,
                Status = true
            };

            _context.Usuario_Recompensas.Add(usuario_recompensa);
            await _context.SaveChangesAsync();

            return Ok(usuario_recompensa);
        }

        [HttpGet("recompesas")]
        [Authorize]
        public async Task<ActionResult<Usuario>> GetRecompensasByUsuario()
        {
            var userPrincipal = GetUserPrincipal();
            var usuario_recompesas =await _context.Usuario_Recompensas.Where(us => (us.UsuarioId == userPrincipal.Id)).ToListAsync();

            List<Recompensa> recompensas = new List<Recompensa>();

            foreach(var usuario_recompesa in usuario_recompesas)
            {
                recompensas.Add(await _context.Recompensas.FindAsync(usuario_recompesa.RecompensaId));
            }

            return Ok(recompensas);

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
