namespace bienesraices.Models
{
    public class Propietario
    {
        public int Id { get; set; }
        public string Dni { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Email { get; set; } = "";
        public string Direccion { get; set; } = "";
        public int Estado { get; set; }
    }
}