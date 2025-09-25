namespace bienesraices.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public int Id_contrato { get; set; }
        public int Numero_pago { get; set; }
        public DateTime Fecha_pago { get; set; }
        public string Detalle { get; set; } = "";
        public decimal Importe { get; set; }
        public string Estado { get; set; } = "";
        public int Id_usuario_creador { get; set; }
        public int? Id_usuario_anulador { get; set; }

        public string? Anulador { get; set; }
    }
}