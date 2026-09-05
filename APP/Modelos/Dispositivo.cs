namespace FixTrack.Modelos;

public class Dispositivo
{
    public int DispositivoID { get; set; }
    public int ClienteID { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? NumeroSerie { get; set; }
    public string? Descripcion { get; set; }
    public DateTime FechaRegistro { get; set; }

    // Propiedad de visualización (JOIN con Clientes en las consultas de listado)
    public string ClienteNombre { get; set; } = string.Empty;

    // Ayuda para ComboBox: "Laptop HP Pavilion 15 — Maria Gonzalez"
    public string DescripcionCombo =>
        $"{Tipo} {Marca ?? string.Empty} {Modelo ?? string.Empty}".Trim() + $" — {ClienteNombre}";
}