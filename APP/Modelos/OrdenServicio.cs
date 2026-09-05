namespace FixTrack.Modelos;

public class OrdenServicio
{
    public int OrdenID { get; set; }
    public int DispositivoID { get; set; }
    public int? TecnicoID { get; set; }
    public DateTime FechaIngreso { get; set; }
    public string ProblemaReportado { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public string? TrabajoRealizado { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public decimal CostoServicio { get; set; }
    public DateTime? FechaFinalizacion { get; set; }
    public string? Observaciones { get; set; }

    // Propiedades de visualización (provienen de JOINs en las consultas de listado)
    public string ClienteNombre { get; set; } = string.Empty;
    public string DispositivoTexto { get; set; } = string.Empty;
    public string TecnicoNombre { get; set; } = string.Empty;

    // Ayuda para ComboBox: "Orden 5 — Maria Gonzalez — Laptop HP Pavilion 15 (Pendiente)"
    public string DescripcionCombo =>
        $"Orden {OrdenID} — {ClienteNombre} — {DispositivoTexto} ({Estado})";
}