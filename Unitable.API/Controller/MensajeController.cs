using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Entities;
using Unitable.Service;
using Unitable.Dto.Response;


namespace Unitable.API.Controller
{
    [ApiController]
    [Route("api/[Controller]")]
    public class MensajeController : ControllerBase
    {
        private readonly IMensajeService _mensajeService;
        private readonly UnitableDbContext _context;

        public MensajeController(IMensajeService mensajeService, UnitableDbContext context)
        {
            _mensajeService = mensajeService;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<BaseResponseGeneric<ICollection<Mensaje>>>> Get()
        {
            var userPrincipal = GetUserPrincipal();
            var response = await _mensajeService.GetMensajes(userPrincipal);
            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Post(DtoMensaje request)
        {
            var userPrincipal = GetUserPrincipal();
            var resm = await _mensajeService.Post(userPrincipal, request);

            if (resm.Success)
            {
                var entity = (Mensaje)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/Mensaje/{entity.Id}");
                return Ok(entity);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete(int MensajeId)
        {
            var userPrincipal = GetUserPrincipal();
            var entity = await _mensajeService.Delete(userPrincipal, MensajeId);

            if (entity == null) return NotFound("Mensaje #" + MensajeId + " Not Found");

            return Ok(entity);
        }

        [HttpPut("{MensajeId:int}")]
        [Authorize]
        public async Task<ActionResult> Put(int MensajeId, DtoMensaje request)
        {
            var userPrincipal = GetUserPrincipal();
            var MensajeFromDb = await _mensajeService.Put(userPrincipal, MensajeId, request);

            HttpContext.Response.Headers.Add("location", $"/api/Mensaje/{MensajeFromDb.Id}");

            return Ok(MensajeFromDb);
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
