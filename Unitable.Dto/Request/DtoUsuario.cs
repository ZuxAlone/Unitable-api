using Unitable.Entities;

namespace Unitable.Dto.Request
{
    public class DtoUsuario
    {
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public string Carrera { get; set; }
        public TipoUsuario Tipo { get; set; }
    }
}
