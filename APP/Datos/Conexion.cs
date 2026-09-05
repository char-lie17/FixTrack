using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FixTrack.Datos;

/// <summary>
/// Punto único de acceso a la cadena de conexión (appsettings.json) y creación de conexiones.
/// </summary>
public static class Conexion
{
    private static readonly IConfiguration Configuration;

    static Conexion()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public static string GetConnectionString()
    {
        return Configuration.GetConnectionString("FixTrack")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'FixTrack' en appsettings.json");
    }

    public static SqlConnection ObtenerConexion()
    {
        return new SqlConnection(GetConnectionString());
    }
}
