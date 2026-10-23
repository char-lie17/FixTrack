using FixTrack.Modelos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FixTrack.Datos;

/// <summary>Acceso a datos de usuarios sobre EF Core (migrado desde ADO.NET).</summary>
public static class UsuarioDAL
{
    /// <summary>Proyección compartida: incluye el hash para autenticación y el nombre del técnico asociado.</summary>
    private static IQueryable<Usuario> Proyectar(IQueryable<Usuario> query) =>
        query.Select(u => new Usuario
        {
            UsuarioID = u.UsuarioID,
            NombreUsuario = u.NombreUsuario,
            PasswordHash = u.PasswordHash,
            Rol = u.Rol,
            Estado = u.Estado,
            TecnicoID = u.TecnicoID,
            NombreTecnico = u.Tecnico != null ? u.Tecnico.Nombre + " " + u.Tecnico.Apellido : string.Empty
        });

    public static Task<List<Usuario>> ObtenerTodosAsync(CancellationToken ct = default)
        => BuscarAsync(null, null, ct);

    public static async Task<List<Usuario>> BuscarAsync(string? texto, string? estado, CancellationToken ct = default)
    {
        var textoLimpio = texto?.Trim();
        var esId = int.TryParse(textoLimpio, out var idBuscado);

        await using var db = FixTrackDb.Crear();
        var query = db.Usuarios.AsNoTracking();

        if (esId)
            query = query.Where(u => u.UsuarioID == idBuscado);
        else if (!string.IsNullOrWhiteSpace(textoLimpio))
        {
            var t = $"%{textoLimpio}%";
            query = query.Where(u => EF.Functions.Like(u.NombreUsuario, t)
                                  || EF.Functions.Like(u.Rol, t));
        }
        if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
            query = query.Where(u => u.Estado == estado);

        return await Proyectar(query)
            .OrderBy(u => u.NombreUsuario)
            .ToListAsync(ct);
    }

    public static async Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await Proyectar(db.Usuarios.AsNoTracking())
            .FirstOrDefaultAsync(u => u.UsuarioID == id, ct);
    }

    /// <summary>Consulta del login: obtiene usuario, hash y rol por nombre de usuario.</summary>
    public static async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await Proyectar(db.Usuarios.AsNoTracking())
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario, ct);
    }

    public static async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int excluirUsuarioId = 0,
        CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await db.Usuarios.AsNoTracking()
            .AnyAsync(u => u.NombreUsuario == nombreUsuario && u.UsuarioID != excluirUsuarioId, ct);
    }

    public static async Task<int> InsertarAsync(Usuario u, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        u.Estado = "Activo";
        db.Usuarios.Add(u);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            // UQ_Usuarios_NombreUsuario (o UQ_Usuarios_TecnicoID): carrera entre dos administradores.
            throw new ApplicationException("El nombre de usuario ya existe o el técnico está asociado a otro usuario.", ex);
        }
        return u.UsuarioID;
    }

    public static async Task<bool> ActualizarAsync(Usuario u, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        var existente = await db.Usuarios
            .FirstOrDefaultAsync(x => x.UsuarioID == u.UsuarioID, ct);
        if (existente == null) return false;

        existente.NombreUsuario = u.NombreUsuario;
        existente.PasswordHash = u.PasswordHash;
        existente.Rol = u.Rol;
        existente.TecnicoID = u.TecnicoID;
        // Estado no se toca: la edición conserva el valor de BD (mismo comportamiento anterior).
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new ApplicationException("El nombre de usuario ya existe o el técnico está asociado a otro usuario.", ex);
        }
        return true;
    }

    /// <summary>Baja lógica: alterna Activo/Inactivo.</summary>
    public static async Task<bool> CambiarEstadoAsync(int id, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.UsuarioID == id, ct);
        if (usuario == null) return false;
        usuario.Estado = usuario.Estado == "Activo" ? "Inactivo" : "Activo";
        await db.SaveChangesAsync(ct);
        return true;
    }
}
