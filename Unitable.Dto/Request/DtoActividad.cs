namespace Unitable.Dto.Request
{
    public class DtoActividad
    {
        public string Nombre { get; set; }
        public string Detalle { get; set; }
        public DateTime HoraIni { get; set; }
        public DateTime HoraFin { get; set; }
        public int TemaId { get; set; }
    }
}
