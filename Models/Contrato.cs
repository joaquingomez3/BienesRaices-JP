namespace bienesraices.Models
{
    public class Contrato
    {
        public int Id { get; set; }
        public int Id_inquilino { get; set; }
        public int Id_inmueble { get; set; }
        public DateTime Fecha_inicio { get; set; }
        public DateTime Fecha_fin { get; set; }
        public DateTime? Fecha_terminacion { get; set; } // nullable por si puede ser null
        public decimal Monto_mensual { get; set; }
        public int Id_usuario_creador { get; set; }
        public int? Id_usuario_finalizador { get; set; }


    }
}