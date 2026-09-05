namespace FixTrack.Modelos;

public class Usuario
{
    public int UsuarioID { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string Estado { get; set; } = "Activo";
    public int? TecnicoID { get; set; }

    // Propiedad de visualización (LEFT JOIN con Tecnicos en las consultas de listado)
    public string NombreTecnico { get; set; } = string.Empty;
}