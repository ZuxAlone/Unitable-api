using Microsoft.EntityFrameworkCore;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Dto.Response;
using Unitable.Entities;

namespace Unitable.Service
{
    public class MensajeService:IMensajeService
    {
        private readonly UnitableDbContext _context;

        public MensajeService(UnitableDbContext context)
        {
            _context = context;
        }

        public async Task<Mensaje> Delete(Usuario userPrincipal, int MensajeId)
        {
            var entity = await _context.Mensajes.FindAsync(MensajeId);

            if (entity != null)
            {
                _context.Mensajes.Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            else
            {
                return null;
            }
        }

        public async Task<ICollection<Mensaje>> GetMensajes(Usuario userPrincipal)
        {
            var response = await _context.Mensajes.Where(msg => (msg.UsuarioId == userPrincipal.Id)).ToListAsync();
            return response;
        }

        public async Task<ICollection<Mensaje>> GetMensajesFromChat(int ChatId)
        {
            var response = await _context.Mensajes.Where(msg => (msg.ChatId == ChatId)).ToListAsync();
            return response;
        }


        public async Task<BaseResponseGeneric<Mensaje>> Post(Usuario userPrincipal, DtoMensaje request) 
        {
            var res = new BaseResponseGeneric<Mensaje>();
            var ChatFromDb = await _context.Chats.FindAsync(request.ChatId);
            if (ChatFromDb == null) {
                res.Success = false;
                res.Errors.Add("Chat #" + request.ChatId + " Not Found");
                return res;
            }

            var grupoFromDb = await _context.Grupos.Where(gr => gr.ChatId == ChatFromDb.Id).ToListAsync();
            if (grupoFromDb.Count == 0)
            {
                res.Success = false;
                res.Errors.Add("Grupo Not Found");
                return res;
            }
            var grupo = grupoFromDb.ElementAt(0);

            var userGrupo = await _context.Usuario_Grupos.Where(ug => ug.UsuarioId == userPrincipal.Id && ug.GrupoId == grupo.Id).ToListAsync();

            if (userGrupo.Count == 0)
            {
                res.Success = false;
                res.Errors.Add("Usuario #" + userPrincipal.Id + " No pertenece al grupo");
                return res;
            }

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

            res.Success = true;
            res.Result = entity;

            return res;
        }

        public async Task<Mensaje> Put(Usuario userPrincipal, int MensajeId, DtoMensaje request)
        {
            var MensajeFromDb = await _context.Mensajes.FindAsync(MensajeId);
            if (MensajeFromDb == null || userPrincipal.Id != MensajeFromDb.UsuarioId) return null;

            MensajeFromDb.MensajeTexto = request.MensajeTexto;

            _context.Mensajes.Update(MensajeFromDb);
            await _context.SaveChangesAsync();

            return MensajeFromDb;
        }
    }
}
