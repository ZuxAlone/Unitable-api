using Microsoft.EntityFrameworkCore;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Dto.Response;
using Unitable.Entities;

namespace Unitable.Service
{
    public class RecompensaService : IRecompensaService
    {
        private readonly UnitableDbContext _context;

        public RecompensaService(UnitableDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recompensa>> Get(Usuario userPrincipal)
        {
            var usuario_recompesas = await _context.Usuario_Recompensas.Where(us => (us.UsuarioId == userPrincipal.Id)).ToListAsync();

            List<Recompensa> recompensas = new List<Recompensa>();
            List<Recompensa> recompensasnot = new List<Recompensa>();

            recompensasnot = _context.Recompensas.ToList();

            foreach (var usuario_recompesa in usuario_recompesas)
            {
                recompensas.Add(await _context.Recompensas.FindAsync(usuario_recompesa.RecompensaId));
            }

            recompensasnot = recompensasnot.Except(recompensas).ToList();

            return recompensasnot;

        }

        public async Task<BaseResponseGeneric<Recompensa>> Post(DtoRecompensa request)
        {
            var recompensaNameRepetida = await _context.Recompensas.Where(us => (us.Nombre == request.Nombre)).ToListAsync();

            var resm = new BaseResponseGeneric<Recompensa>();

            if (recompensaNameRepetida.Count != 0)
            {
                resm.Success = false;
                resm.Errors.Add("Ya existe una recompensa con este nombre");
                return resm;
            }

            var entity = new Recompensa
            {
                Nombre = request.Nombre,
                Detalle = request.Detalle,
                PrecioMon = request.PrecioMon,

                Status = true
            };

            _context.Recompensas.Add(entity);
            await _context.SaveChangesAsync();

            resm.Success = true;
            resm.Result = entity;

            return resm;
        }

        public async Task<Recompensa> Delete(int RecompensaId)
        {
            var entity = await _context.Recompensas.FindAsync(RecompensaId);

            if (entity != null)
            {
                foreach (Usuario_Recompensa element in _context.Usuario_Recompensas)
                {
                    if (element.RecompensaId == RecompensaId)
                    {
                        _context.Usuario_Recompensas.Remove(element);
                    }
                }

                _context.Recompensas.Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            else
            {
                return null;
            }

        }

        public async Task<BaseResponseGeneric<Recompensa>> Put(int RecompensaId, DtoRecompensa request)
        {
            var resm = new BaseResponseGeneric<Recompensa>();
            var RecompensaFromDb = await _context.Recompensas.FindAsync(RecompensaId);

            if (RecompensaFromDb == null)
            {
                resm.Success = false;
                resm.Errors.Add("El valor no esta definido");
                return resm;
            }

            var RecompensaRepetida = await _context.Recompensas.Where(us => (us.Detalle == request.Detalle && us.Id != RecompensaId)).ToListAsync();

            if (RecompensaRepetida.Count != 0)
            {
                resm.Success = false;
                resm.Errors.Add("Ya existe esta recompensa");
                return resm;
            }

            RecompensaFromDb.Nombre = request.Nombre;
            RecompensaFromDb.Detalle = request.Detalle;
            RecompensaFromDb.PrecioMon = request.PrecioMon;

            _context.Recompensas.Update(RecompensaFromDb);
            await _context.SaveChangesAsync();

            resm.Success = true;
            resm.Result = RecompensaFromDb;
            return resm;
        }

        public async Task<BaseResponseGeneric<Usuario_Recompensa>> BuyRecompensa(Usuario userPrincipal, int recompensaId)
        {
            var res = new BaseResponseGeneric<Usuario_Recompensa>();
            var recompensaDb = await _context.Recompensas.FindAsync(recompensaId);

            if (recompensaDb == null)
            {
                res.Success = false;
                res.Errors.Add("El valor no esta definido");
                return res;
            }

            if (userPrincipal.NumMonedas < recompensaDb.PrecioMon)
            {
                res.Success = false;
                res.Errors.Add("No tienes monedas suficientes");
                return res;
            }

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

            res.Success = true;
            res.Result = usuario_recompensa;

            return res;
        }

        public async Task<List<Recompensa>> GetRecompensasByUsuario(Usuario userPrincipal)
        {
            var usuario_recompesas = await _context.Usuario_Recompensas.Where(us => (us.UsuarioId == userPrincipal.Id)).ToListAsync();

            List<Recompensa> recompensas = new List<Recompensa>();

            foreach (var usuario_recompesa in usuario_recompesas)
            {
                recompensas.Add(await _context.Recompensas.FindAsync(usuario_recompesa.RecompensaId));
            }

            return recompensas;
        }

        public async Task<Usuario_Recompensa> DeleteOfUsuario(int RecompensaId)
        {
            var entity = await _context.Usuario_Recompensas.FindAsync(RecompensaId);

            if (entity != null)
            {
                foreach (Usuario_Recompensa element in _context.Usuario_Recompensas)
                {
                    if (element.RecompensaId == RecompensaId)
                    {
                        _context.Usuario_Recompensas.Remove(element);
                    }
                }

                _context.Usuario_Recompensas.Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            else
            {
                return null;
            }

        }
    }
}
