using System.Data;

namespace FixTrack.Modelos;

/// <summary>
/// Resultado de un reporte: un título descriptivo y la tabla de datos generada.
/// </summary>
public class ReporteResultado
{
    public string Titulo { get; set; } = string.Empty;
    public DataTable Datos { get; set; } = new();
}
