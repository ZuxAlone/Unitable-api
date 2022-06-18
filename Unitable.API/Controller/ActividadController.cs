using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unitable.DataAccess;
using Unitable.Dto.Response;
using Unitable.Dto.Request;
using Unitable.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Unitable.API.Controller
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ActividadController: ControllerBase
    {
        private readonly UnitableDbContext _context;

        public ActividadController(UnitableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Actividad>> Get()
        {
            var todas_Actividades = await _context.Actividades.ToListAsync();

            return Ok(todas_Actividades);

        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Post(DtoActividad request)
        {
            var userPrincipal = GetUserPrincipal();

            TimeSpan dif = request.HoraFin - request.HoraIni;
            if (dif.TotalMinutes < 0) return Problem("La fecha inicial tiene que ser antes que la fecha final");
            if (request.HoraFin == request.HoraIni) return Problem("La fecha inicial y final tienen que ser diferentes");

            if(_context.Actividades.Count() > 0)
            {
                var coincidencias_igual = await _context.Actividades.Where(us => (us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni,request.HoraIni) == 0 && DateTime.Compare(us.HoraFin,request.HoraFin) == 0)).ToListAsync();
                var coincidencias_contenido = await _context.Actividades.Where(us => (us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) < 0 && DateTime.Compare(us.HoraFin, request.HoraFin) > 0 || us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) == 0 && DateTime.Compare(us.HoraFin, request.HoraFin) > 0 || us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) < 0 && DateTime.Compare(us.HoraFin, request.HoraFin) == 0)).ToListAsync();
                var coincidencias_conteniendo = await _context.Actividades.Where(us => (us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) > 0 && DateTime.Compare(us.HoraFin, request.HoraFin) < 0 || us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) == 0 && DateTime.Compare(us.HoraFin, request.HoraFin) < 0 || us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) > 0 && DateTime.Compare(us.HoraFin, request.HoraFin) == 0)).ToListAsync();
                var coincidencias_cruce = await _context.Actividades.Where(us => (us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) > 0 && DateTime.Compare(us.HoraFin, request.HoraFin) > 0 && DateTime.Compare(us.HoraIni, request.HoraFin) < 0 || us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) < 0 && DateTime.Compare(us.HoraFin, request.HoraFin) < 0 && DateTime.Compare(us.HoraFin, request.HoraIni) > 0)).ToListAsync();

                if (coincidencias_igual.Count() == 0)
                {
                    if (coincidencias_contenido.Count() == 0)
                    {
                        if (coincidencias_conteniendo.Count() == 0)
                        {
                            if(coincidencias_cruce.Count() == 0)
                            {
                                var entity = new Actividad
                                {
                                    Nombre = request.Nombre,
                                    Detalle = request.Detalle,
                                    HoraIni = request.HoraIni,
                                    HoraFin = request.HoraFin,

                                    DuracionMin = Math.Round(dif.TotalMinutes, 0),
                                    Activa = true,

                                    UsuarioId = userPrincipal.Id,
                                    TemaId = request.TemaId,

                                    Usuario = userPrincipal,
                                    Tema = await _context.Temas.FindAsync(request.TemaId),

                                    Status = true
                                };

                                _context.Actividades.Add(entity);
                                await _context.SaveChangesAsync();

                                HttpContext.Response.Headers.Add("location", $"/api/actividad/{entity.Id}");

                                return Ok(entity);
                            }
                            else
                            {
                                return Problem("Este horario se cruza con otro horario");
                            }
                        }
                        else
                        {
                            return Problem("Ya tiene una actividad dentro de este horario");
                        }
                    }
                    else
                    {
                        return Problem("No se puede llevar una actividad dentro del horario de otra actividad");
                    }
                }
                else
                {
                    return Problem("No se pueden llevar 2 actividades en el mismo horario");
                }
            }
            else
            {
                var entity = new Actividad
                {
                    Nombre = request.Nombre,
                    Detalle = request.Detalle,
                    HoraIni = request.HoraIni,
                    HoraFin = request.HoraFin,

                    DuracionMin = Math.Round(dif.TotalMinutes, 0),
                    Activa = true,

                    UsuarioId = userPrincipal.Id,
                    TemaId = request.TemaId,

                    Usuario = userPrincipal,
                    Tema = await _context.Temas.FindAsync(request.TemaId),

                    Status = true
                };

                _context.Actividades.Add(entity);
                await _context.SaveChangesAsync();

                HttpContext.Response.Headers.Add("location", $"/api/actividad/{entity.Id}");

                return Ok(entity);
            }  
        }

        [HttpDelete("{actividadId:int}")]
        public async Task<ActionResult> Delete(int actividadId)
        {
            var entity = await _context.Actividades.FindAsync(actividadId);

            if (entity == null) return NotFound();

            _context.Actividades.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{actividadId:int}")]
        public async Task<ActionResult> Put(int actividadId, DtoActividad request)
        {
            var actividadFromDb = await _context.Actividades.FindAsync(actividadId);

            if (actividadFromDb == null) return NotFound();

            TimeSpan dif = request.HoraFin - request.HoraIni;
            if (dif.TotalMinutes < 0) return Problem("La fecha inicial tiene que ser antes que la fecha final");
            if (request.HoraFin == request.HoraIni) return Problem("La fecha inicial y final tienen que ser diferentes");

            var coincidencias_igual = await _context.Actividades.Where(us => (us.UsuarioId == actividadFromDb.UsuarioId && DateTime.Compare(us.HoraIni, request.HoraIni) == 0 && DateTime.Compare(us.HoraFin, request.HoraFin) == 0)).ToListAsync();
            var coincidencias_contenido = await _context.Actividades.Where(us => (us.UsuarioId == actividadFromDb.UsuarioId && DateTime.Compare(us.HoraIni, request.HoraIni) < 0 && DateTime.Compare(us.HoraFin, request.HoraFin) > 0 || us.UsuarioId == actividadFromDb.UsuarioId && DateTime.Compare(us.HoraIni, request.HoraIni) == 0 && DateTime.Compare(us.HoraFin, request.HoraFin) > 0 || us.UsuarioId == actividadFromDb.UsuarioId && DateTime.Compare(us.HoraIni, request.HoraIni) < 0 && DateTime.Compare(us.HoraFin, request.HoraFin) == 0)).ToListAsync();
            var coincidencias_conteniendo = await _context.Actividades.Where(us => (us.UsuarioId == actividadFromDb.UsuarioId && DateTime.Compare(us.HoraIni, request.HoraIni) > 0 && DateTime.Compare(us.HoraFin, request.HoraFin) < 0 || us.UsuarioId == actividadFromDb.UsuarioId && DateTime.Compare(us.HoraIni, request.HoraIni) == 0 && DateTime.Compare(us.HoraFin, request.HoraFin) < 0 || us.UsuarioId == actividadFromDb.UsuarioId && DateTime.Compare(us.HoraIni, request.HoraIni) > 0 && DateTime.Compare(us.HoraFin, request.HoraFin) == 0)).ToListAsync();
            var coincidencias_cruce = await _context.Actividades.Where(us => (us.UsuarioId == actividadFromDb.UsuarioId && DateTime.Compare(us.HoraIni, request.HoraIni) > 0 && DateTime.Compare(us.HoraFin, request.HoraFin) > 0 && DateTime.Compare(us.HoraIni, request.HoraFin) < 0 || us.UsuarioId == actividadFromDb.UsuarioId && DateTime.Compare(us.HoraIni, request.HoraIni) < 0 && DateTime.Compare(us.HoraFin, request.HoraFin) < 0 && DateTime.Compare(us.HoraFin, request.HoraIni) > 0)).ToListAsync();

            if (coincidencias_igual.Count() == 0)
            {
                if (coincidencias_contenido.Count() == 0)
                {
                    if (coincidencias_conteniendo.Count() == 0)
                    {
                        if(coincidencias_cruce.Count() == 0)
                        {
                            actividadFromDb.Nombre = request.Nombre;
                            actividadFromDb.Detalle = request.Detalle;
                            actividadFromDb.HoraIni = request.HoraIni;
                            actividadFromDb.HoraFin = request.HoraFin;
                            actividadFromDb.DuracionMin = Math.Round(dif.TotalMinutes, 0);

                            _context.Actividades.Update(actividadFromDb);
                            await _context.SaveChangesAsync();

                            HttpContext.Response.Headers.Add("location", $"/api/actividad/{actividadFromDb.Id}");

                            return Ok(new { Id = actividadId });
                        }
                        else
                        {
                            return Problem("Este horario se cruza con otro horario");
                        }
                        
                    }
                    else
                    {
                        return Problem("Ya tiene una actividad dentro de este horario");
                    }
                }
                else
                {
                    return Problem("No se puede llevar una actividad dentro del horario de otra actividad");
                }
            }
            else
            {
                return Problem("No se pueden llevar 2 actividades en el mismo horario");
            }
        }

        [HttpPut("finish/{actividadId:int}")]
        [Authorize]
        public async Task<ActionResult> Finish(int actividadId)
        {
            var userPrincipal = GetUserPrincipal();

            var actividadFromDb = await _context.Actividades.FindAsync(actividadId);

            if (actividadFromDb == null) return NotFound();

            actividadFromDb.Activa = false;

            _context.Actividades.Update(actividadFromDb);

            HttpContext.Response.Headers.Add("location", $"/api/actividad/{actividadFromDb.Id}");

            var tests =  await _context.Tests.Where(us => (us.TemaId == actividadFromDb.TemaId)).ToListAsync();

            Random randomm = new Random();

            int r = randomm.Next(0, tests.Count);

            var test = tests[r];

            userPrincipal.NumActCompletas = userPrincipal.NumActCompletas + 1;
            userPrincipal.NumMonedas = userPrincipal.NumMonedas + 30;

            await _context.SaveChangesAsync();

            return Ok(test);
        }

        [HttpGet("actividades")]
        [Authorize]
        public async Task<ActionResult<Actividad>> GetActividadesByUsuario()
        {
            var userPrincipal = GetUserPrincipal();
            var mis_Actividades = await _context.Actividades.Where(us => (us.UsuarioId == userPrincipal.Id)).ToListAsync();

            return Ok(mis_Actividades);
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
