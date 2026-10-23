using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FixTrack.Datos;

/// <summary>
/// Fábrica de <see cref="FixTrackDbContext"/> basada en la interfaz oficial
/// <see cref="IDbContextFactory{TContext}"/> de EF Core.
/// Cada operación de datos crea y libera su propio contexto (ciclo de vida por operación),
/// de modo que ninguna instancia se comparte entre tareas concurrentes.
/// Se inicializa una sola vez en Program.Main a partir de la cadena de conexión de appsettings.json.
/// </summary>
public sealed class FixTrackDbContextFactory : IDbContextFactory<FixTrackDbContext>,
    IDesignTimeDbContextFactory<FixTrackDbContext>
{
    private readonly string _connectionString;

    /// <summary>
    /// Constructor requerido por las herramientas de EF Core en tiempo de diseño
    /// (dotnet ef), que instancian la fábrica con Activator.CreateInstance.
    /// La cadena se ignora en ese contexto: solo se usa para generar migraciones.
    /// </summary>
    public FixTrackDbContextFactory()
        : this("Server=localhost;Database=FixTrack;TrustServerCertificate=True;Integrated Security=true;")
    {
    }

    public FixTrackDbContextFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public FixTrackDbContext CreateDbContext()
    {
        return new FixTrackDbContext(_connectionString);
    }

    /// <summary>Uso exclusivo de las herramientas de EF Core (dotnet ef) en tiempo de diseño.</summary>
    public FixTrackDbContext CreateDbContext(string[] args)
    {
        return new FixTrackDbContext(
            "Server=localhost;Database=FixTrack;TrustServerCertificate=True;Integrated Security=true;");
    }
}

/// <summary>
/// Punto de acceso estático a la fábrica de contextos, configurado una sola vez en la
/// composición de la aplicación (Program.Main). No guarda contexto alguno: solo la fábrica.
/// </summary>
public static class FixTrackDb
{
    private static FixTrackDbContextFactory? _factory;

    public static void Inicializar()
    {
        _factory = new FixTrackDbContextFactory(Conexion.GetConnectionString());
    }

    /// <summary>Crea un contexto nuevo para una operación. El llamador debe liberarlo con using/await using.</summary>
    public static FixTrackDbContext Crear()
    {
        return (_factory ?? throw new InvalidOperationException(
                "FixTrackDb no está inicializado. Llame a FixTrackDb.Inicializar() antes de usar la capa de datos."))
            .CreateDbContext();
    }
}
