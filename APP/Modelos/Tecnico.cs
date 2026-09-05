namespace FixTrack.Modelos;

public class Tecnico
{
    public int TecnicoID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Especialidad { get; set; }
    public string Estado { get; set; } = "Activo";

    // Ayuda para ComboBox
    public string NombreCompleto => $"{Nombre} {Apellido}";
}