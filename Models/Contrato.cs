namespace bienesraices.Models
{
    
    public class Contrato
    {
        public int Id { get; set; }
        public int Id_inquilino { get; set; }
        public int Id_inmueble { get; set; }
        public DateTime Fecha_inicio { get; set; }
        public DateTime Fecha_fin { get; set; }
        public string? Estado { get; set; }
        public DateTime? Fecha_terminacion { get; set; }
        public decimal Monto_mensual { get; set; }
        public int Id_usuario_creador { get; set; }
        public int? Id_usuario_finalizador { get; set; }

        public decimal? Multa { get; set; }

        public string? InquilinoContrato { get; set; }

        public string? InmuebleContrato { get; set; }

        public string? InmuebleUso { get; set; }
        public string? Creador { get; set; }

        public string? Finalizador { get; set; }


    }
}