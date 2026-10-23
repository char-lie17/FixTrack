using FixTrack.Modelos;
using Microsoft.EntityFrameworkCore;

namespace FixTrack.Datos;

/// <summary>Acceso a datos de dispositivos sobre EF Core (migrado desde ADO.NET).</summary>
public static class DispositivoDAL
{
    /// <summary>Proyección compartida: solo los campos necesarios, con el nombre del cliente calculado en BD.</summary>
    private static IQueryable<Dispositivo> Proyectar(IQueryable<Dispositivo> query) =>
        query.Select(d => new Dispositivo
        {
            DispositivoID = d.DispositivoID,
            ClienteID = d.ClienteID,
            Tipo = d.Tipo,
            Marca = d.Marca,
            Modelo = d.Modelo,
            NumeroSerie = d.NumeroSerie,
            Descripcion = d.Descripcion,
            FechaRegistro = d.FechaRegistro,
            ClienteNombre = d.Cliente.Nombre + " " + d.Cliente.Apellido
        });

    public static async Task<List<Dispositivo>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await Proyectar(db.Dispositivos.AsNoTracking())
            .OrderByDescending(d => d.FechaRegistro).ThenByDescending(d => d.DispositivoID)
            .ToListAsync(ct);
    }

    public static async Task<List<Dispositivo>> BuscarAsync(string? texto, CancellationToken ct = default)
    {
        var textoLimpio = texto?.Trim();
        var esId = int.TryParse(textoLimpio, out var idBuscado);

        await using var db = FixTrackDb.Crear();
        var query = db.Dispositivos.AsNoTracking();

        if (esId)
            query = query.Where(d => d.DispositivoID == idBuscado);
        else if (!string.IsNullOrWhiteSpace(textoLimpio))
        {
            var t = $"%{textoLimpio}%";
            query = query.Where(d => EF.Functions.Like(d.Tipo, t)
                                  || EF.Functions.Like(d.Marca ?? "", t)
                                  || EF.Functions.Like(d.Modelo ?? "", t)
                                  || EF.Functions.Like(d.NumeroSerie ?? "", t)
                                  || EF.Functions.Like(d.Cliente.Nombre, t)
                                  || EF.Functions.Like(d.Cliente.Apellido, t)
                                  || EF.Functions.Like(d.Cliente.Nombre + " " + d.Cliente.Apellido, t));
        }

        return await Proyectar(query)
            .OrderByDescending(d => d.FechaRegistro).ThenByDescending(d => d.DispositivoID)
            .ToListAsync(ct);
    }

    public static async Task<List<Dispositivo>> ObtenerPorClienteAsync(int clienteId, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await Proyectar(db.Dispositivos.AsNoTracking().Where(d => d.ClienteID == clienteId))
            .OrderByDescending(d => d.FechaRegistro).ThenByDescending(d => d.DispositivoID)
            .ToListAsync(ct);
    }

    public static async Task<Dispositivo?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await Proyectar(db.Dispositivos.AsNoTracking())
            .FirstOrDefaultAsync(d => d.DispositivoID == id, ct);
    }

    public static async Task<int> InsertarAsync(Dispositivo d, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        d.FechaRegistro = DateTime.Now;
        db.Dispositivos.Add(d);
        await db.SaveChangesAsync(ct);
        return d.DispositivoID;
    }

    public static async Task<bool> ActualizarAsync(Dispositivo d, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        var existente = await db.Dispositivos
            .FirstOrDefaultAsync(x => x.DispositivoID == d.DispositivoID, ct);
        if (existente == null) return false;

        existente.ClienteID = d.ClienteID;
        existente.Tipo = d.Tipo;
        existente.Marca = d.Marca;
        existente.Modelo = d.Modelo;
        existente.NumeroSerie = d.NumeroSerie;
        existente.Descripcion = d.Descripcion;
        await db.SaveChangesAsync(ct);
        return true;
    }
}
