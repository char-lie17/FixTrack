namespace FixTrack.Modelos;

public class Cliente
{
    public int ClienteID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Estado { get; set; } = "Activo";

    // Navegación EF Core (relación Clientes 1 — * Dispositivos). No genera columna nueva.
    public ICollection<Dispositivo> Dispositivos { get; set; } = new List<Dispositivo>();

    // Ayuda para ComboBox
    public string NombreCompleto => $"{Nombre} {Apellido}";
}