using Microsoft.EntityFrameworkCore;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Dto.Response;
using Unitable.Entities;

namespace Unitable.Service
{
    public class ActividadService : IActividadService
    {
        private readonly UnitableDbContext _context;

        public ActividadService(UnitableDbContext context)
        {
            _context = context;
        }

        public async Task<List<Actividad>> Get()
        {
            var todas_Actividades = await _context.Actividades.ToListAsync();

            return todas_Actividades;
        }

        public async Task<BaseResponseGeneric<Actividad>> Post(Usuario userPrincipal, DtoActividad request)
        {
            var res = new BaseResponseGeneric<Actividad>();

            var ActividadNameRepetido = await _context.Actividades.Where(us => (us.UsuarioId == userPrincipal.Id && us.Nombre == request.Nombre)).ToListAsync();
            if (ActividadNameRepetido.Count != 0)
            {
                res.Success = false;
                res.Errors.Add("Ya existe un curso con este nombre");
                return res;
            }

            TimeSpan dif = request.HoraFin - request.HoraIni;
            if (dif.TotalMinutes < 0) {
                res.Success = false;
                res.Errors.Add("La fecha inicial tiene que ser antes que la fecha final");
                return res;
            }
            if (request.HoraFin == request.HoraIni)
            {
                res.Success = false;
                res.Errors.Add("La fecha inicial y final tienen que ser diferentes");
                return res;
            }

            if (_context.Actividades.Count() > 0)
            {
                var coincidencias_igual = await _context.Actividades.Where(us => (us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) == 0 && DateTime.Compare(us.HoraFin, request.HoraFin) == 0)).ToListAsync();
                var coincidencias_contenido = await _context.Actividades.Where(us => (us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) < 0 && DateTime.Compare(us.HoraFin, request.HoraFin) > 0 || us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) == 0 && DateTime.Compare(us.HoraFin, request.HoraFin) > 0 || us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) < 0 && DateTime.Compare(us.HoraFin, request.HoraFin) == 0)).ToListAsync();
                var coincidencias_conteniendo = await _context.Actividades.Where(us => (us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) > 0 && DateTime.Compare(us.HoraFin, request.HoraFin) < 0 || us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) == 0 && DateTime.Compare(us.HoraFin, request.HoraFin) < 0 || us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) > 0 && DateTime.Compare(us.HoraFin, request.HoraFin) == 0)).ToListAsync();
                var coincidencias_cruce = await _context.Actividades.Where(us => (us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) > 0 && DateTime.Compare(us.HoraFin, request.HoraFin) > 0 && DateTime.Compare(us.HoraIni, request.HoraFin) < 0 || us.UsuarioId == userPrincipal.Id && DateTime.Compare(us.HoraIni, request.HoraIni) < 0 && DateTime.Compare(us.HoraFin, request.HoraFin) < 0 && DateTime.Compare(us.HoraFin, request.HoraIni) > 0)).ToListAsync();

                if (coincidencias_igual.Count() == 0)
                {
                    if (coincidencias_contenido.Count() == 0)
                    {
                        if (coincidencias_conteniendo.Count() == 0)
                        {
                            if (coincidencias_cruce.Count() == 0)
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

                                res.Success = true;
                                res.Result = entity;
                                return res;
                            }
                            else
                            {
                                res.Success = false;
                                res.Errors.Add("Este horario se cruza con otro horario");
                                return res;
                            }
                        }
                        else
                        {
                            res.Success = false;
                            res.Errors.Add("Ya tiene una actividad dentro de este horario");
                            return res;
                        }
                    }
                    else
                    {
                        res.Success = false;
                        res.Errors.Add("No se puede llevar una actividad dentro del horario de otra actividad");
                        return res;
                    }
                }
                else
                {
                    res.Success = false;
                    res.Errors.Add("No se pueden llevar 2 actividades en el mismo horario");
                    return res;
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

                res.Success = true;
                res.Result = entity;
                return res;
            }
        }

        public async Task<Actividad> Delete(int actividadId)
        {
            var entity = await _context.Actividades.FindAsync(actividadId);

            if (entity != null)
            {
                _context.Actividades.Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            else
            {
                return null;
            }
        }

        public async Task<BaseResponseGeneric<Actividad>> Put(int actividadId, DtoActividad request)
        {
            var res = new BaseResponseGeneric<Actividad>();
            var actividadFromDb = await _context.Actividades.FindAsync(actividadId);

            if (actividadFromDb == null)
            {
                res.Success = false;
                res.Errors.Add("El valor no esta definido");
                return res;
            }

            var ActividadNameRepetido = await _context.Actividades.Where(us => (us.UsuarioId == actividadFromDb.UsuarioId && us.Nombre == request.Nombre)).ToListAsync();
            if (ActividadNameRepetido.Count != 0)
            {
                res.Success = false;
                res.Errors.Add("Ya existe un curso con este nombre");
                return res;
            }

            TimeSpan dif = request.HoraFin - request.HoraIni;
            if (dif.TotalMinutes < 0)
            {
                res.Success = false;
                res.Errors.Add("La fecha inicial tiene que ser antes que la fecha final");
                return res;
            }
            if (request.HoraFin == request.HoraIni)
            {
                res.Success = false;
                res.Errors.Add("La fecha inicial y final tienen que ser diferentes");
                return res;
            }

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
                        if (coincidencias_cruce.Count() == 0)
                        {
                            actividadFromDb.Nombre = request.Nombre;
                            actividadFromDb.Detalle = request.Detalle;
                            actividadFromDb.HoraIni = request.HoraIni;
                            actividadFromDb.HoraFin = request.HoraFin;
                            actividadFromDb.DuracionMin = Math.Round(dif.TotalMinutes, 0);

                            _context.Actividades.Update(actividadFromDb);
                            await _context.SaveChangesAsync();

                            res.Success = true;
                            res.Result = actividadFromDb;
                            return res;
                        }
                        else
                        {
                            res.Success = false;
                            res.Errors.Add("Este horario se cruza con otro horario");
                            return res;
                        }
                    }
                    else
                    {
                        res.Success = false;
                        res.Errors.Add("Ya tiene una actividad dentro de este horario");
                        return res;
                    }
                }
                else
                {
                    res.Success = false;
                    res.Errors.Add("No se puede llevar una actividad dentro del horario de otra actividad");
                    return res;
                }
            }
            else
            {
                res.Success = false;
                res.Errors.Add("No se pueden llevar 2 actividades en el mismo horario");
                return res;
            }
        }

        public async Task<BaseResponseGeneric<Test>> Finish(Usuario userPrincipal, int actividadId)
        {
            var res = new BaseResponseGeneric<Test>();
            var actividadFromDb = await _context.Actividades.FindAsync(actividadId);

            if (actividadFromDb == null) {
                res.Success = false;
                res.Errors.Add("El valor no esta definido");
                return res;
            }

            actividadFromDb.Activa = false;

            _context.Actividades.Update(actividadFromDb);

            var tests = await _context.Tests.Where(us => (us.TemaId == actividadFromDb.TemaId)).ToListAsync();

            Random randomm = new Random();

            int r = (randomm.Next(0, tests.Count));

            var test = tests[r];

            await _context.SaveChangesAsync();

            res.Success = true;
            res.Result = test;
            return res;
        }

        public async Task<List<Actividad>> GetActividadesByUsuario(Usuario userPrincipal)
        {
            var mis_Actividades = await _context.Actividades.Where(us => (us.UsuarioId == userPrincipal.Id)).ToListAsync();

            return mis_Actividades;
        }
    }
}
