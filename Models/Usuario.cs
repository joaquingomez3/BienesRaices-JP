using System.ComponentModel.DataAnnotations;
namespace bienesraices.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        public string Nombre_usuario { get; set; } = "";

        [Display(Name = "Apellido")]
        public string Apellido_usuario { get; set; } = "";
        public string Email { get; set; } = "";

        [Display(Name = "Contraseña")]
        public string? Password { get; set; }
        public int Id_tipo_usuario { get; set; }
        public bool Activo { get; set; }

        public string? Foto { get; set; }
        public string RolUsuario { get; set; } = "";
    }
}