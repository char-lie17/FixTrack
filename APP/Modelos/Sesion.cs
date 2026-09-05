namespace FixTrack.Modelos;

/// <summary>
/// Sesión estática del usuario autenticado. Se establece tras un login exitoso
/// y se limpia al cerrar sesión.
/// </summary>
public static class Sesion
{
    public static int UsuarioID { get; set; }
    public static string NombreUsuario { get; set; } = string.Empty;
    public static string Rol { get; set; } = string.Empty;
    public static int? TecnicoID { get; set; }

    public static bool EstaActiva => UsuarioID > 0;
    public static bool EsAdministrador => Rol == "Administrador";
    public static bool EsEmpleado => Rol == "Empleado";
    public static bool EsTecnico => Rol == "Tecnico";

    public static void Limpiar()
    {
        UsuarioID = 0;
        NombreUsuario = string.Empty;
        Rol = string.Empty;
        TecnicoID = null;
    }
}
