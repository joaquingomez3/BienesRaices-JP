using System.ComponentModel.DataAnnotations;
namespace bienesraices.Models
{
    public class Inmueble
    {
        [Display(Name = "Codigo Inmueble")]
        public int Id { get; set; }

        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = "";

        [Display(Name = "Uso")]
        public string Uso { get; set; } = "";

        [Display(Name = "Ambientes")]
        public int Ambientes { get; set; }

        [Display(Name = "Coordenadas")]
        public string Coordenadas { get; set; } = "";

        [Display(Name = "Precio ($)")]
        public decimal Precio { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "";

        [Display(Name = "Propietario")]
        public int Id_Propietario { get; set; }

        [Display(Name = "Tipo de Inmueble")]
        public int Id_Tipo { get; set; }

        [Display(Name = "Propietario")]
        public string PropietarioNombre { get; set; } = "";

        [Display(Name = "Tipo de Inmueble")]
        public string TipoInmuebleNombre { get; set; } = "";
    }
}