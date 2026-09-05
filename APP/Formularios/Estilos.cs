namespace FixTrack.Formularios;

/// <summary>
/// Centraliza los colores y tipografía definidos en la identidad visual
/// (Contexto/09_identidad_visual.md) para mantener coherencia en todos los formularios.
/// </summary>
public static class Estilos
{
    // Paleta principal
    public static readonly Color Primario = Color.FromArgb(0x2C, 0x5F, 0x8A);
    public static readonly Color Secundario = Color.FromArgb(0xFF, 0x6B, 0x35);
    public static readonly Color Terciario = Color.FromArgb(0x2B, 0x2D, 0x42);
    public static readonly Color Neutro = Color.FromArgb(0xF4, 0xF6, 0xF8);
    public static readonly Color GrisMedio = Color.FromArgb(0x6B, 0x72, 0x80);

    // Colores de estado de órdenes
    public static readonly Color Pendiente = Color.FromArgb(0xD6, 0x45, 0x45);
    public static readonly Color EnDiagnostico = Color.FromArgb(0x00, 0xA8, 0xE8);
    public static readonly Color EnReparacion = Color.FromArgb(0xF5, 0xA6, 0x23);
    public static readonly Color Listo = Color.FromArgb(0x2E, 0x7D, 0x32);
    public static readonly Color Entregado = Color.FromArgb(0x2B, 0x2D, 0x42);

    public static Color ColorDeEstado(string estado) => EstadoNormalizado(estado) switch
    {
        "Pendiente" => Pendiente,
        "En diagnostico" => EnDiagnostico,
        "En reparacion" => EnReparacion,
        "Listo" => Listo,
        "Entregado" => Entregado,
        _ => Color.Gray
    };

    public static Font Fuente(float tamano, FontStyle estilo = FontStyle.Regular)
        => new("Segoe UI", tamano, estilo);

    /// <summary>Estilo de botón secundario (azul Primario).</summary>
    public static void BotonSecundario(Button b)
    {
        b.BackColor = Primario;
        b.ForeColor = Color.White;
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderSize = 0;
        b.Font = Fuente(9, FontStyle.Bold);
        b.Cursor = Cursors.Hand;
        b.Height = 34;
    }

    /// <summary>Estilo de botón principal (naranja Secundario).</summary>
    public static void BotonPrincipal(Button b)
    {
        b.BackColor = Secundario;
        b.ForeColor = Color.White;
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderSize = 0;
        b.Font = Fuente(9, FontStyle.Bold);
        b.Cursor = Cursors.Hand;
        b.Height = 34;
    }

    private static string EstadoNormalizado(string estado)
    {
        return (estado ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "en diagnostico" or "en diagnóstico" => "En diagnostico",
            "en reparacion" or "en reparación" => "En reparacion",
            _ => estado ?? string.Empty
        };
    }
}
