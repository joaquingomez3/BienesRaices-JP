using System.ComponentModel.DataAnnotations;

namespace bienesraices.Models
{
    public class Inquilino
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe tener entre 7 y 8 números")]
        public string Dni { get; set; } = "";

        [Required(ErrorMessage = "El Nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El Nombre no puede superar los 50 caracteres")]
        public string Nombre_completo { get; set; } = "";

        [Required(ErrorMessage = "El Teléfono es obligatorio")]
        [Phone(ErrorMessage = "Ingrese un teléfono válido")]
        public string Telefono { get; set; } = "";

        [Required(ErrorMessage = "El Email es obligatorio")]
        [EmailAddress(ErrorMessage = "El Email no tiene un formato válido")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La Dirección es obligatoria")]
        [StringLength(100, ErrorMessage = "La Dirección no puede superar los 100 caracteres")]
        public string Direccion { get; set; } = "";
        public int Estado { get; set; }
    }
}