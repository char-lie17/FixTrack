namespace FixTrack.Modelos;

public class HistorialOrden
{
    public int HistorialID { get; set; }
    public int OrdenID { get; set; }
    public int? UsuarioID { get; set; }
    public DateTime FechaCambio { get; set; }
    public string TipoCambio { get; set; } = string.Empty;
    public string? EstadoAnterior { get; set; }
    public string? EstadoNuevo { get; set; }
    public string? CampoModificado { get; set; }
    public string? ValorAnterior { get; set; }
    public string? ValorNuevo { get; set; }
    public string? Comentario { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
}
