using Unitable.Entities;

namespace Unitable.Dto.Request
{
    public class DtoEditUsuario
    {
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Carrera { get; set; }
        public TipoUsuario Tipo { get; set; }
    }
}
