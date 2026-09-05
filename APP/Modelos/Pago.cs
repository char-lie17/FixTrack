namespace FixTrack.Modelos;

public class Pago
{
    public int PagoID { get; set; }
    public int OrdenID { get; set; }
    public DateTime FechaPago { get; set; }
    public decimal Monto { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public string? Observaciones { get; set; }

    // Propiedad de visualización (JOIN con Clientes en las consultas de listado)
    public string ClienteNombre { get; set; } = string.Empty;
}