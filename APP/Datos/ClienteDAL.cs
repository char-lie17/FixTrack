using FixTrack.Modelos;
using Microsoft.EntityFrameworkCore;

namespace FixTrack.Datos;

/// <summary>Acceso a datos de clientes sobre EF Core (migrado desde ADO.NET).</summary>
public static class ClienteDAL
{
    public static async Task<List<Cliente>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await db.Clientes.AsNoTracking()
            .OrderBy(c => c.Nombre).ThenBy(c => c.Apellido)
            .ToListAsync(ct);
    }

    /// <summary>Solo clientes con Estado = 'Activo'. Usado al crear un dispositivo nuevo,
    /// igual que TecnicoDAL.ObtenerActivosAsync() al asignar técnico (hallazgo g de la auditoría).</summary>
    public static async Task<List<Cliente>> ObtenerActivosAsync(CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await db.Clientes.AsNoTracking()
            .Where(c => c.Estado == "Activo")
            .OrderBy(c => c.Nombre).ThenBy(c => c.Apellido)
            .ToListAsync(ct);
    }

    public static async Task<List<Cliente>> BuscarAsync(string? texto, string? estado, CancellationToken ct = default)
    {
        var textoLimpio = texto?.Trim();
        var esId = int.TryParse(textoLimpio, out var idBuscado);

        await using var db = FixTrackDb.Crear();
        var query = db.Clientes.AsNoTracking().AsQueryable();

        if (esId)
        {
            // El usuario escribió solo números: lo más intuitivo es que busque el ID exacto.
            query = query.Where(c => c.ClienteID == idBuscado);
        }
        else if (!string.IsNullOrWhiteSpace(textoLimpio))
        {
            var t = $"%{textoLimpio}%";
            query = query.Where(c => EF.Functions.Like(c.Nombre, t)
                                  || EF.Functions.Like(c.Apellido, t)
                                  || EF.Functions.Like(c.Nombre + " " + c.Apellido, t)
                                  || EF.Functions.Like(c.Telefono, t));
        }
        if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
            query = query.Where(c => c.Estado == estado);

        return await query
            .OrderBy(c => c.Nombre).ThenBy(c => c.Apellido)
            .ToListAsync(ct);
    }

    public static async Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await db.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClienteID == id, ct);
    }

    public static async Task<int> InsertarAsync(Cliente c, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        // FechaRegistro y Estado se asignan igual que en la versión ADO.NET (GETDATE() y 'Activo'
        // quedan también como valores por defecto en BD por si se inserta fuera de esta capa).
        c.FechaRegistro = DateTime.Now;
        c.Estado = "Activo";
        db.Clientes.Add(c);
        await db.SaveChangesAsync(ct);
        return c.ClienteID;
    }

    public static async Task<bool> ActualizarAsync(Cliente c, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        var existente = await db.Clientes
            .FirstOrDefaultAsync(x => x.ClienteID == c.ClienteID, ct);
        if (existente == null) return false;

        existente.Nombre = c.Nombre;
        existente.Apellido = c.Apellido;
        existente.Telefono = c.Telefono;
        existente.Email = c.Email;
        existente.Direccion = c.Direccion;
        // Estado y FechaRegistro no se tocan: la edición conserva los valores de BD
        // (mismo comportamiento que el UPDATE anterior de ADO.NET).
        await db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>Baja lógica: alterna Activo/Inactivo.</summary>
    public static async Task<bool> CambiarEstadoAsync(int id, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        var cliente = await db.Clientes.FirstOrDefaultAsync(c => c.ClienteID == id, ct);
        if (cliente == null) return false;
        cliente.Estado = cliente.Estado == "Activo" ? "Inactivo" : "Activo";
        await db.SaveChangesAsync(ct);
        return true;
    }
}
