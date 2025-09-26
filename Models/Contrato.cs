using System.ComponentModel.DataAnnotations;

namespace bienesraices.Models
{
    public class Contrato
    {
        [Display(Name = "Código Contrato")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inquilino")]
        [Display(Name = "Inquilino")]
        public int Id_inquilino { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inmueble")]
        [Display(Name = "Inmueble")]
        public int Id_inmueble { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime Fecha_inicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Fin")]

        public DateTime Fecha_fin { get; set; }

        [StringLength(50)]
        [Display(Name = "Estado")]
        public string? Estado { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Terminación")]
        public DateTime? Fecha_terminacion { get; set; }

        [Required(ErrorMessage = "Debe ingresar el monto mensual")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto mensual debe ser positivo")]
        [Display(Name = "Monto Mensual ($)")]
        public decimal Monto_mensual { get; set; }

        [Required]
        [Display(Name = "Usuario Creador")]
        public int Id_usuario_creador { get; set; }

        [Display(Name = "Usuario Finalizador")]
        public int? Id_usuario_finalizador { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La multa debe ser positiva")]
        public decimal? Multa { get; set; }

        public string? InquilinoContrato { get; set; }
        public string? InmuebleContrato { get; set; }
        public string? InmuebleUso { get; set; }
        public string? Creador { get; set; }
        public string? Finalizador { get; set; }

      
    }
    
}
