using System.ComponentModel.DataAnnotations;

namespace bienesraices.Models
{
    public class Inmueble
    {
        [Display(Name = "Codigo Inmueble")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(150, ErrorMessage = "La dirección no puede superar los 150 caracteres")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = "";

        [Required(ErrorMessage = "El uso del inmueble es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Uso")]
        public string Uso { get; set; } = "";

        [Required(ErrorMessage = "Debe ingresar la cantidad de ambientes")]
        [Range(1, 20, ErrorMessage = "Los ambientes deben estar entre 1 y 20")]
        [Display(Name = "Ambientes")]
        public int Ambientes { get; set; }

        [Display(Name = "Coordenadas")]
        [StringLength(50)]
        public string Coordenadas { get; set; } = "";

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo")]
        [Display(Name = "Precio ($)")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "";

        [Required(ErrorMessage = "Debe seleccionar un propietario")]
        [Display(Name = "Propietario")]
        public int Id_Propietario { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de inmueble")]
        [Display(Name = "Tipo de Inmueble")]
        public int Id_Tipo { get; set; }

        [Display(Name = "Propietario")]
        public string PropietarioNombre { get; set; } = "";

        [Display(Name = "Tipo de Inmueble")]
        public string TipoInmuebleNombre { get; set; } = "";

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres")]
        public string? Descripcion { get; set; }

        public Propietario? Duenio { get; set; }

        public List<FotoInmueble> Fotos { get; set; } = new List<FotoInmueble>();
    }
}
