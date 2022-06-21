using Microsoft.EntityFrameworkCore;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Entities;

namespace Unitable.Service
{
    public class GrupoService: IGrupoService
    {
        private readonly UnitableDbContext _context;
        public GrupoService(UnitableDbContext context)
        {
            _context = context;
        }

        public async Task<Grupo> Delete(int GrupoId)
        {
            var entity = await _context.Grupos.FindAsync(GrupoId);
            if (entity == null)
                return null;
            var chatEntity = await _context.Chats.FindAsync(entity.ChatId);
            if (entity != null)
                _context.Chats.Remove(chatEntity);
            _context.Grupos.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ICollection<Grupo>> Get(Usuario userPrincipal)
        {
            try
            {
                var usuarioGrupo = await _context.Usuario_Grupos.Where(us => (us.UsuarioId == userPrincipal.Id)).ToListAsync();
                var indexGrupos = usuarioGrupo.Select(ug => ug.GrupoId).ToList();
                var response = await _context.Grupos.Where(gr => indexGrupos.Contains(gr.Id)).ToListAsync();
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ICollection<Grupo>> GetOthers(Usuario userPrincipal)
        {
            try
            {
                var usuarioGrupo = await _context.Usuario_Grupos.Where(us => (us.UsuarioId == userPrincipal.Id)).ToListAsync();
                var indexGrupos = usuarioGrupo.Select(ug => ug.GrupoId).ToList();
                var response = await _context.Grupos.Where(gr => !indexGrupos.Contains(gr.Id)).ToListAsync();
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Grupo> GetById(int grupoId)
        {
            var entity = await _context.Grupos.FindAsync(grupoId);
            return entity;
        }

        public async Task<Usuario_Grupo> JoinGrupo(Usuario userPrincipal, int GrupoId)
        {
            var grupo = _context.Grupos.FirstOrDefault(gr => gr.Id == GrupoId);
            if (grupo == null)
                return null;

            var usuarioGrupo = await _context.Usuario_Grupos.FirstOrDefaultAsync(us => (us.GrupoId == GrupoId && us.UsuarioId == userPrincipal.Id));

            if (usuarioGrupo != null)
                return null;

            usuarioGrupo = new Usuario_Grupo
            {
                UsuarioId = userPrincipal.Id,
                Usuario = userPrincipal,
                GrupoId = grupo.Id,
                Grupo = grupo
            };

            _context.Usuario_Grupos.Add(usuarioGrupo);
            await _context.SaveChangesAsync();

            return usuarioGrupo;
        }

        public async Task<Grupo> Post(Usuario userPrincipal, DtoGrupo request)
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

            var usuario_grupo = new Usuario_Grupo
            {
                UsuarioId = userPrincipal.Id,
                Usuario = userPrincipal,

                GrupoId = entity.Id,
                Grupo = entity
            };
            _context.Usuario_Grupos.Add(usuario_grupo);

            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<Grupo> Put(int GrupoId, DtoGrupo request)
        {
            var GrupoFromDb = await _context.Grupos.FindAsync(GrupoId);
            if (GrupoFromDb == null)
                return null;

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

            return GrupoFromDb;
        }
    }
}
