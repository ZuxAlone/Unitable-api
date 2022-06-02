using System.ComponentModel.DataAnnotations;

namespace Unitable.Entities
{
    public enum TipoUsuario
    {
        ESTUDIANTE, COACH
    }

    public class Usuario : EntityBase
    {
        [StringLength(100)]
        [Required]
        public string Nombres { get; set; }
        [StringLength(100)]
        [Required]
        public string Apellidos { get; set; }
        [StringLength(100)]
        [Required]
        public string Correo { get; set; }
        [StringLength(50)]
        [Required]
        public string Password { get; set; }
        [StringLength(50)]
        [Required]
        public string Carrera { get; set; }
        public int NumActCompletas { get; set; }
        public int NumTestAprobados { get; set; }
        public int NumMonedas { get; set; }
        public bool IsPremium { get; set; }
        [Required]
        [EnumDataType(typeof(TipoUsuario))]
        public TipoUsuario Tipo { get; set; }
    }
}
