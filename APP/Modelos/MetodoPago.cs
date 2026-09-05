namespace FixTrack.Modelos;

/// <summary>
/// Los 3 métodos de pago válidos (CK_Pagos_MetodoPago).
/// </summary>
public enum MetodoPago
{
    Efectivo,
    Tarjeta,
    Transferencia
}

public static class MetodoPagoTexto
{
    public static readonly string[] Valores =
    {
        "Efectivo", "Tarjeta", "Transferencia"
    };

    public static MetodoPago DesdeTexto(string texto)
    {
        return Enum.TryParse<MetodoPago>(texto ?? string.Empty, ignoreCase: true, out var metodo)
            ? metodo
            : MetodoPago.Efectivo;
    }
}
