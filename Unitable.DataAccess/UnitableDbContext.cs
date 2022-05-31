using Microsoft.EntityFrameworkCore;
using Unitable.Entities;

namespace Unitable.DataAccess;

public class UnitableDbContext : DbContext
{
    public UnitableDbContext()
    {
         
    }

    public UnitableDbContext(DbContextOptions<UnitableDbContext> options) : base(options)
    {

    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Actividad> Actividades { get; set; }
    public DbSet<Recompensa> Recompensas{ get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Curso> Cursos { get; set; }
    public DbSet<Grupo> Grupos { get; set; }
    public DbSet<Mensaje> Mensajes { get; set; }
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Respuesta> Respuestas { get; set; }
    public DbSet<Tema> Temas { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<Usuario_Grupo> Usuario_Grupos { get; set; }
    public DbSet<Usuario_Follow> Usuario_Follows { get; set; }
    public DbSet<Usuario_Recompensa> Usuario_Recompensas { get; set; }
}