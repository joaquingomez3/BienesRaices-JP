
namespace bienesraices.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre_usuario { get; set; } = "";
        public string Apellido_usuario { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public int Id_tipo_usuario { get; set; }
        public bool Activo { get; set; }
        public DateTime Creado { get; set; }
        public string? Foto { get; set; }
    }
}