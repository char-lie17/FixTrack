using System.Linq;

namespace FixTrack.Modelos;

/// <summary>
/// Los 5 estados válidos de una orden de servicio (CK_OrdenesServicio_Estado).
/// </summary>
public enum EstadoOrden
{
    Pendiente,
    EnDiagnostico,
    EnReparacion,
    Listo,
    Entregado
}

/// <summary>
/// Utilidades para convertir entre el enum y los valores almacenados en la BD.
/// Los valores de BD no llevan acentos ("En diagnostico", "En reparacion");
/// las etiquetas de interfaz usan la ortografía correcta.
/// </summary>
public static class EstadoOrdenTexto
{
    public static readonly string[] ValoresBD =
    {
        "Pendiente", "En diagnostico", "En reparacion", "Listo", "Entregado"
    };

    public static readonly string[] Etiquetas =
    {
        "Pendiente", "En diagnóstico", "En reparación", "Listo", "Entregado"
    };

    public static string ATexto(EstadoOrden estado) => estado switch
    {
        EstadoOrden.EnDiagnostico => "En diagnostico",
        EstadoOrden.EnReparacion => "En reparacion",
        _ => estado.ToString()
    };

    public static string Etiqueta(EstadoOrden estado) => estado switch
    {
        EstadoOrden.EnDiagnostico => "En diagnóstico",
        EstadoOrden.EnReparacion => "En reparación",
        _ => estado.ToString()
    };

    public static EstadoOrden DesdeTexto(string texto)
    {
        var limpio = (texto ?? string.Empty).Trim().ToLowerInvariant();
        return limpio switch
        {
            "en diagnostico" or "en diagnóstico" => EstadoOrden.EnDiagnostico,
            "en reparacion" or "en reparación" => EstadoOrden.EnReparacion,
            "listo" => EstadoOrden.Listo,
            "entregado" => EstadoOrden.Entregado,
            _ => EstadoOrden.Pendiente
        };
    }

    /// <summary>
    /// Todos los estados como pares (Valor real de BD, Etiqueta con tilde para mostrar),
    /// listos para llenar un ComboBox con la ortografía correcta sin dejar de usar el
    /// valor real de la BD al filtrar/guardar (hallazgo h de la auditoría).
    /// </summary>
    public static IReadOnlyList<EstadoItem> ItemsParaCombo =>
        ValoresBD.Zip(Etiquetas, (valor, etiqueta) => new EstadoItem(valor, etiqueta)).ToArray();
}

/// <summary>Par (valor de BD, etiqueta visible) para usar como Item de un ComboBox.
/// ToString() devuelve la etiqueta, así que el ComboBox la muestra automáticamente.</summary>
public sealed record EstadoItem(string Valor, string Etiqueta)
{
    public override string ToString() => Etiqueta;
}
