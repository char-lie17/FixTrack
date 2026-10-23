using FixTrack.Modelos;
using Microsoft.EntityFrameworkCore;

namespace FixTrack.Datos;

/// <summary>Acceso a datos de técnicos sobre EF Core (migrado desde ADO.NET).</summary>
public static class TecnicoDAL
{
    public static async Task<List<Tecnico>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await db.Tecnicos.AsNoTracking()
            .OrderBy(t => t.Nombre).ThenBy(t => t.Apellido)
            .ToListAsync(ct);
    }

    public static async Task<List<Tecnico>> ObtenerActivosAsync(CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await db.Tecnicos.AsNoTracking()
            .Where(t => t.Estado == "Activo")
            .OrderBy(t => t.Nombre).ThenBy(t => t.Apellido)
            .ToListAsync(ct);
    }

    public static async Task<List<Tecnico>> BuscarAsync(string? texto, string? estado, CancellationToken ct = default)
    {
        var textoLimpio = texto?.Trim();
        var esId = int.TryParse(textoLimpio, out var idBuscado);

        await using var db = FixTrackDb.Crear();
        var query = db.Tecnicos.AsNoTracking().AsQueryable();

        if (esId)
            query = query.Where(t => t.TecnicoID == idBuscado);
        else if (!string.IsNullOrWhiteSpace(textoLimpio))
        {
            var t = $"%{textoLimpio}%";
            query = query.Where(x => EF.Functions.Like(x.Nombre, t)
                                  || EF.Functions.Like(x.Apellido, t)
                                  || EF.Functions.Like(x.Nombre + " " + x.Apellido, t)
                                  || EF.Functions.Like(x.Especialidad ?? "", t));
        }
        if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
            query = query.Where(t => t.Estado == estado);

        return await query
            .OrderBy(t => t.Nombre).ThenBy(t => t.Apellido)
            .ToListAsync(ct);
    }

    public static async Task<Tecnico?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await db.Tecnicos.AsNoTracking()
            .FirstOrDefaultAsync(t => t.TecnicoID == id, ct);
    }

    public static async Task<int> InsertarAsync(Tecnico t, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        t.Estado = "Activo";
        db.Tecnicos.Add(t);
        await db.SaveChangesAsync(ct);
        return t.TecnicoID;
    }

    public static async Task<bool> ActualizarAsync(Tecnico t, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        var existente = await db.Tecnicos
            .FirstOrDefaultAsync(x => x.TecnicoID == t.TecnicoID, ct);
        if (existente == null) return false;

        existente.Nombre = t.Nombre;
        existente.Apellido = t.Apellido;
        existente.Telefono = t.Telefono;
        existente.Especialidad = t.Especialidad;
        await db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>Baja lógica: alterna Activo/Inactivo.</summary>
    public static async Task<bool> CambiarEstadoAsync(int id, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        var tecnico = await db.Tecnicos.FirstOrDefaultAsync(t => t.TecnicoID == id, ct);
        if (tecnico == null) return false;
        tecnico.Estado = tecnico.Estado == "Activo" ? "Inactivo" : "Activo";
        await db.SaveChangesAsync(ct);
        return true;
    }
}
