namespace FixTrack.Modelos;

public class Tecnico
{
    public int TecnicoID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Especialidad { get; set; }
    public string Estado { get; set; } = "Activo";

    // Navegación EF Core (relación Tecnicos 1 — * OrdenesServicio). No genera columna nueva.
    public ICollection<OrdenServicio> Ordenes { get; set; } = new List<OrdenServicio>();

    // Ayuda para ComboBox
    public string NombreCompleto => $"{Nombre} {Apellido}";
}