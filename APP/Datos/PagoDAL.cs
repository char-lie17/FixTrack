using FixTrack.Modelos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FixTrack.Datos;

/// <summary>
/// Acceso a datos de pagos sobre EF Core (migrado desde ADO.NET).
/// La escritura de pagos conserva el bloqueo de la orden y la validación de saldo
/// dentro de una única transacción, igual que la versión original.
/// </summary>
public static class PagoDAL
{
    /// <summary>Proyección compartida: cliente dueño de la orden (una sola consulta, sin N+1).</summary>
    private static IQueryable<Pago> Proyectar(IQueryable<Pago> query) =>
        query.Select(p => new Pago
        {
            PagoID = p.PagoID,
            OrdenID = p.OrdenID,
            FechaPago = p.FechaPago,
            Monto = p.Monto,
            MetodoPago = p.MetodoPago,
            Observaciones = p.Observaciones,
            ClienteNombre = p.Orden.Dispositivo.Cliente.Nombre + " " + p.Orden.Dispositivo.Cliente.Apellido
        });

    public static Task<List<Pago>> ObtenerTodosAsync(CancellationToken ct = default)
        => BuscarAsync(null, null, null, null, ct);

    public static async Task<List<Pago>> BuscarAsync(string? metodo, DateTime? desde, DateTime? hasta,
        string? texto, CancellationToken ct = default)
    {
        var textoLimpio = texto?.Trim();
        var esId = int.TryParse(textoLimpio, out var idBuscado);

        await using var db = FixTrackDb.Crear();
        var query = db.Pagos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(metodo) && metodo != "Todos")
            query = query.Where(p => p.MetodoPago == metodo);
        if (desde.HasValue)
            query = query.Where(p => p.FechaPago >= desde.Value);
        if (hasta.HasValue)
            query = query.Where(p => p.FechaPago < hasta.Value.AddDays(1));
        if (esId)
            query = query.Where(p => p.OrdenID == idBuscado);
        else if (!string.IsNullOrWhiteSpace(textoLimpio))
        {
            var t = $"%{textoLimpio}%";
            query = query.Where(p => EF.Functions.Like(p.Orden.Dispositivo.Cliente.Nombre, t)
                                  || EF.Functions.Like(p.Orden.Dispositivo.Cliente.Apellido, t)
                                  || EF.Functions.Like(p.Orden.Dispositivo.Cliente.Nombre + " " + p.Orden.Dispositivo.Cliente.Apellido, t));
        }

        return await Proyectar(query)
            .OrderByDescending(p => p.FechaPago).ThenByDescending(p => p.PagoID)
            .ToListAsync(ct);
    }

    public static async Task<List<Pago>> ObtenerPorOrdenAsync(int ordenId, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await Proyectar(db.Pagos.AsNoTracking().Where(p => p.OrdenID == ordenId))
            .OrderBy(p => p.FechaPago).ThenBy(p => p.PagoID)
            .ToListAsync(ct);
    }

    public static async Task<Pago?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await Proyectar(db.Pagos.AsNoTracking())
            .FirstOrDefaultAsync(p => p.PagoID == id, ct);
    }

    public static Task<int> InsertarAsync(Pago p, CancellationToken ct = default)
        => RegistrarPagoSeguroAsync(p, ct);

    /// <summary>
    /// Valida el saldo e inserta el pago bloqueando la orden en una sola transacción.
    /// La concurrencia entre pagos simultáneos se controla con el mismo bloqueo
    /// UPDLOCK/HOLDLOCK que usaba la versión ADO.NET: dos pagos simultáneos sobre la
    /// misma orden se serializan y ninguno puede exceder el saldo real.
    /// </summary>
    public static async Task<int> RegistrarPagoSeguroAsync(Pago p, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        await using var tx = await db.Database
            .BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);

        // Bloqueo de la fila de la orden hasta confirmar la transacción (equivalente al
        // SELECT ... WITH (UPDLOCK, HOLDLOCK) de la versión ADO.NET).
        await db.Database.ExecuteSqlAsync(
            $"SELECT 1 FROM OrdenesServicio WITH (UPDLOCK, HOLDLOCK) WHERE OrdenID = {p.OrdenID}",
            ct);

        var existe = await db.OrdenesServicio.AsNoTracking()
            .AnyAsync(o => o.OrdenID == p.OrdenID, ct);
        if (!existe) throw new ApplicationException("La orden seleccionada no existe.");

        var costo = await db.OrdenesServicio.AsNoTracking()
            .Where(o => o.OrdenID == p.OrdenID)
            .Select(o => o.CostoServicio)
            .FirstAsync(ct);

        var totalPagado = await db.Pagos.AsNoTracking()
            .Where(x => x.OrdenID == p.OrdenID)
            .SumAsync(x => (decimal?)x.Monto, ct) ?? 0m;

        var saldo = costo - totalPagado;
        if (p.Monto <= 0 || p.Monto > saldo)
            throw new ApplicationException($"El pago no puede exceder el saldo pendiente. Saldo disponible: {saldo:C2}.");

        p.FechaPago = DateTime.Now;
        db.Pagos.Add(p);
        db.HistorialOrdenes.Add(new HistorialOrden
        {
            OrdenID = p.OrdenID,
            UsuarioID = Sesion.UsuarioID > 0 ? Sesion.UsuarioID : null,
            TipoCambio = "Pago",
            CampoModificado = "Pago",
            ValorNuevo = p.Monto.ToString("F2"),
            Comentario = "Pago registrado",
            FechaCambio = DateTime.Now
        });

        try
        {
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 } or SqlException { Number: 515 })
        {
            await tx.RollbackAsync(ct);
            throw new ApplicationException("Error al registrar el pago: monto o referencia de orden inválidos.", ex);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
        return p.PagoID;
    }

    /// <summary>Total pagado acumulado de una orden (para mostrar saldo pendiente).</summary>
    public static async Task<decimal> ObtenerTotalPagadoAsync(int ordenId, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await db.Pagos.AsNoTracking()
            .Where(p => p.OrdenID == ordenId)
            .SumAsync(p => (decimal?)p.Monto, ct) ?? 0m;
    }
}
