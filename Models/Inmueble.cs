namespace bienesraices.Models
{
    public class Inmueble
    {
        public int Id { get; set; }
        public string Direccion { get; set; } = "";
        public string Uso { get; set; } = "";
        public int Ambientes { get; set; }
        public string Coordenadas { get; set; } = "";

        public decimal Precio { get; set; }

        public string Estado { get; set; } = "";
        public int Id_propietario { get; set; }
        public int Id_tipo { get; set; }
    }
}