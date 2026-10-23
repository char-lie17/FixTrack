using FixTrack.Modelos;
using Microsoft.EntityFrameworkCore;

namespace FixTrack.Datos;

/// <summary>
/// Acceso a datos de órdenes de servicio sobre EF Core (migrado desde ADO.NET).
/// Conserva las transacciones, el historial de cambios y la validación de transiciones de estado
/// de la versión original.
/// </summary>
public static class OrdenServicioDAL
{
    /// <summary>
    /// Ordenación por flujo real de una orden (Pendiente → En diagnostico → En reparacion →
    /// Listo → Entregado) en vez del orden alfabético (hallazgo f de la auditoría).
    /// </summary>
    private static int OrdenFlujo(string estado) => estado switch
    {
        "Pendiente" => 1,
        "En diagnostico" => 2,
        "En reparacion" => 3,
        "Listo" => 4,
        "Entregado" => 5,
        _ => 6
    };

    /// <summary>Proyección compartida de listados: cliente, dispositivo y técnico en una sola consulta (sin N+1).</summary>
    private static IQueryable<OrdenServicio> Proyectar(IQueryable<OrdenServicio> query) =>
        query.Select(o => new OrdenServicio
        {
            OrdenID = o.OrdenID,
            DispositivoID = o.DispositivoID,
            TecnicoID = o.TecnicoID,
            FechaIngreso = o.FechaIngreso,
            ProblemaReportado = o.ProblemaReportado,
            Diagnostico = o.Diagnostico,
            TrabajoRealizado = o.TrabajoRealizado,
            Estado = o.Estado,
            CostoServicio = o.CostoServicio,
            FechaFinalizacion = o.FechaFinalizacion,
            Observaciones = o.Observaciones,
            ClienteNombre = o.Dispositivo.Cliente.Nombre + " " + o.Dispositivo.Cliente.Apellido,
            DispositivoTexto = o.Dispositivo.Tipo + " " + (o.Dispositivo.Marca ?? "") + " " + (o.Dispositivo.Modelo ?? ""),
            TecnicoNombre = o.Tecnico != null ? o.Tecnico.Nombre + " " + o.Tecnico.Apellido : "Sin asignar"
        });

    public static async Task<List<OrdenServicio>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        var lista = await Proyectar(db.OrdenesServicio.AsNoTracking()).ToListAsync(ct);
        return lista.OrderBy(o => OrdenFlujo(o.Estado))
            .ThenByDescending(o => o.FechaIngreso)
            .ThenByDescending(o => o.OrdenID)
            .ToList();
    }

    public static async Task<List<OrdenServicio>> BuscarAsync(string? texto, string? estado,
        DateTime? desde, DateTime? hasta, int? tecnicoId = null, CancellationToken ct = default)
    {
        var textoLimpio = texto?.Trim();
        var esId = int.TryParse(textoLimpio, out var idBuscado);

        await using var db = FixTrackDb.Crear();
        var query = db.OrdenesServicio.AsNoTracking();

        if (esId)
        {
            query = query.Where(o => o.OrdenID == idBuscado);
        }
        else if (!string.IsNullOrWhiteSpace(textoLimpio))
        {
            var t = $"%{textoLimpio}%";
            query = query.Where(o => EF.Functions.Like(o.Dispositivo.Cliente.Nombre, t)
                                  || EF.Functions.Like(o.Dispositivo.Cliente.Apellido, t)
                                  || EF.Functions.Like(o.Dispositivo.Cliente.Nombre + " " + o.Dispositivo.Cliente.Apellido, t)
                                  || EF.Functions.Like(o.Dispositivo.Tipo, t));
        }
        if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
            query = query.Where(o => o.Estado == estado);
        if (desde.HasValue)
            query = query.Where(o => o.FechaIngreso >= desde.Value);
        if (hasta.HasValue)
            query = query.Where(o => o.FechaIngreso < hasta.Value.AddDays(1));
        if (tecnicoId.HasValue)
            query = query.Where(o => o.TecnicoID == tecnicoId.Value);

        var lista = await Proyectar(query).ToListAsync(ct);
        return lista.OrderBy(o => OrdenFlujo(o.Estado))
            .ThenByDescending(o => o.FechaIngreso)
            .ThenByDescending(o => o.OrdenID)
            .ToList();
    }

    public static async Task<List<OrdenServicio>> ObtenerPorTecnicoAsync(int tecnicoId, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await Proyectar(db.OrdenesServicio.AsNoTracking().Where(o => o.TecnicoID == tecnicoId))
            .OrderByDescending(o => o.FechaIngreso)
            .ThenByDescending(o => o.OrdenID)
            .ToListAsync(ct);
    }

    public static async Task<OrdenServicio?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await Proyectar(db.OrdenesServicio.AsNoTracking())
            .FirstOrDefaultAsync(o => o.OrdenID == id, ct);
    }

    public static Task<List<Pago>> ObtenerPagosPorOrdenAsync(int ordenId, CancellationToken ct = default)
        => PagoDAL.ObtenerPorOrdenAsync(ordenId, ct);

    public static async Task<List<HistorialOrden>> ObtenerHistorialAsync(int ordenId, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await db.HistorialOrdenes.AsNoTracking()
            .Where(h => h.OrdenID == ordenId)
            .Select(h => new HistorialOrden
            {
                HistorialID = h.HistorialID,
                OrdenID = h.OrdenID,
                UsuarioID = h.UsuarioID,
                FechaCambio = h.FechaCambio,
                TipoCambio = h.TipoCambio,
                EstadoAnterior = h.EstadoAnterior,
                EstadoNuevo = h.EstadoNuevo,
                CampoModificado = h.CampoModificado,
                ValorAnterior = h.ValorAnterior,
                ValorNuevo = h.ValorNuevo,
                Comentario = h.Comentario,
                UsuarioNombre = h.Usuario != null ? h.Usuario.NombreUsuario : "Sistema"
            })
            .OrderByDescending(h => h.FechaCambio)
            .ThenByDescending(h => h.HistorialID)
            .ToListAsync(ct);
    }

    public static async Task<int> InsertarAsync(OrdenServicio o, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        // Orden + su historial de creación en una sola operación atómica (un único SaveChanges
        // participa de una única transacción implícita, igual que el BEGIN TRAN/COMMIT anterior).
        o.Estado = "Pendiente";
        o.FechaIngreso = DateTime.Now;
        db.OrdenesServicio.Add(o);
        db.HistorialOrdenes.Add(new HistorialOrden
        {
            Orden = o,
            UsuarioID = Sesion.UsuarioID > 0 ? Sesion.UsuarioID : null,
            TipoCambio = "Creacion",
            EstadoNuevo = "Pendiente",
            Comentario = "Orden creada",
            FechaCambio = DateTime.Now
        });
        await db.SaveChangesAsync(ct);
        return o.OrdenID;
    }

    public static async Task<bool> ActualizarDetalleAsync(OrdenServicio o, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();

        var anterior = await db.OrdenesServicio
            .FirstOrDefaultAsync(x => x.OrdenID == o.OrdenID, ct);
        if (anterior == null) return false;

        var cambios = new List<HistorialOrden>();
        void RegistrarCambio(string campo, string? valorAnterior, string? valorNuevo)
        {
            if (string.Equals(valorAnterior, valorNuevo, StringComparison.Ordinal)) return;
            cambios.Add(new HistorialOrden
            {
                OrdenID = o.OrdenID,
                UsuarioID = Sesion.UsuarioID > 0 ? Sesion.UsuarioID : null,
                TipoCambio = "Edicion",
                CampoModificado = campo,
                ValorAnterior = valorAnterior,
                ValorNuevo = valorNuevo,
                Comentario = "Campo actualizado",
                FechaCambio = DateTime.Now
            });
        }

        RegistrarCambio("ProblemaReportado", anterior.ProblemaReportado, o.ProblemaReportado);
        RegistrarCambio("Diagnostico", anterior.Diagnostico, o.Diagnostico);
        RegistrarCambio("TrabajoRealizado", anterior.TrabajoRealizado, o.TrabajoRealizado);
        RegistrarCambio("CostoServicio", anterior.CostoServicio.ToString("F2"), o.CostoServicio.ToString("F2"));
        RegistrarCambio("Observaciones", anterior.Observaciones, o.Observaciones);
        RegistrarCambio("FechaFinalizacion", anterior.FechaFinalizacion?.ToString("O"), o.FechaFinalizacion?.ToString("O"));

        anterior.ProblemaReportado = o.ProblemaReportado;
        anterior.Diagnostico = o.Diagnostico;
        anterior.TrabajoRealizado = o.TrabajoRealizado;
        anterior.CostoServicio = o.CostoServicio;
        anterior.Observaciones = o.Observaciones;
        anterior.FechaFinalizacion = o.FechaFinalizacion;

        if (cambios.Count > 0) db.HistorialOrdenes.AddRange(cambios);
        await db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>Obtiene el estado actual de una orden (usado para validar la transición antes de escribir).</summary>
    public static async Task<string?> ObtenerEstadoActualAsync(int id, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        return await db.OrdenesServicio.AsNoTracking()
            .Where(o => o.OrdenID == id)
            .Select(o => o.Estado)
            .FirstOrDefaultAsync(ct);
    }

    /// <summary>
    /// Cambia el estado de la orden validando la transición contra el flujo real.
    /// Al pasar a 'Listo' o 'Entregado' registra la FechaFinalizacion si aún no existe.
    /// Concurrencia optimista: la escritura exige que el estado leído siga vigente
    /// (WHERE Estado = estado leído); si otro usuario lo cambió primero, no se aplica nada.
    /// Devuelve false si la orden no existe o si la transición no es válida/conflictiva.
    /// </summary>
    public static async Task<bool> ActualizarEstadoAsync(int id, string nuevoEstado, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        await using var tx = await db.Database
            .BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);

        var orden = await db.OrdenesServicio
            .FirstOrDefaultAsync(o => o.OrdenID == id, ct);
        if (orden == null || !EstadoOrdenTexto.EsTransicionValida(orden.Estado, nuevoEstado))
        {
            await tx.RollbackAsync(ct);
            return false;
        }

        var estadoActual = orden.Estado;
        var nuevoEsFinal = EstadoOrdenTexto.EsEstadoFinal(nuevoEstado);

        // Actualización condicional: si el estado cambió entre la lectura y aquí (otro usuario),
        // no se afectan filas y se informa el conflicto en vez de sobrescribir silenciosamente.
        var afectadas = await db.OrdenesServicio
            .Where(x => x.OrdenID == id && x.Estado == estadoActual)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.Estado, nuevoEstado)
                .SetProperty(x => x.FechaFinalizacion,
                    x => nuevoEsFinal && x.FechaFinalizacion == null ? DateTime.Now
                       : nuevoEsFinal ? x.FechaFinalizacion
                       : null), ct);

        if (afectadas == 0)
        {
            await tx.RollbackAsync(ct);
            return false;
        }

        db.HistorialOrdenes.Add(new HistorialOrden
        {
            OrdenID = id,
            UsuarioID = Sesion.UsuarioID > 0 ? Sesion.UsuarioID : null,
            TipoCambio = "Estado",
            EstadoAnterior = estadoActual,
            EstadoNuevo = nuevoEstado,
            Comentario = "Cambio de estado",
            FechaCambio = DateTime.Now
        });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return true;
    }

    /// <summary>Conteo de órdenes por estado (para las 5 métricas del Dashboard).</summary>
    public static async Task<Dictionary<string, int>> ObtenerConteoPorEstadoAsync(CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        var filas = await db.OrdenesServicio.AsNoTracking()
            .GroupBy(o => o.Estado)
            .Select(g => new { g.Key, Cantidad = g.Count() })
            .ToListAsync(ct);
        return EnDiccionario(filas.Select(f => (f.Key, f.Cantidad)));
    }

    /// <summary>Conteo de órdenes por estado filtrado por técnico (para métricas del Dashboard).</summary>
    public static async Task<Dictionary<string, int>> ObtenerConteoPorEstadoAsync(int tecnicoId, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        var filas = await db.OrdenesServicio.AsNoTracking()
            .Where(o => o.TecnicoID == tecnicoId)
            .GroupBy(o => o.Estado)
            .Select(g => new { g.Key, Cantidad = g.Count() })
            .ToListAsync(ct);
        return EnDiccionario(filas.Select(f => (f.Key, f.Cantidad)));
    }

    private static Dictionary<string, int> EnDiccionario(IEnumerable<(string Key, int Cantidad)> filas)
    {
        var resultado = new Dictionary<string, int>
        {
            ["Pendiente"] = 0,
            ["En diagnostico"] = 0,
            ["En reparacion"] = 0,
            ["Listo"] = 0,
            ["Entregado"] = 0
        };
        foreach (var (clave, cantidad) in filas)
        {
            if (resultado.ContainsKey(clave)) resultado[clave] = cantidad;
        }
        return resultado;
    }

    /// <summary>
    /// Escenario transaccional del plan §4.7: crea la orden de servicio y registra
    /// el pago inicial en una misma transacción (Todo o Nada).
    /// </summary>
    public static async Task<int> InsertarConPagoInicialAsync(OrdenServicio o, Pago pago, CancellationToken ct = default)
    {
        if (pago.Monto <= 0 || pago.Monto > o.CostoServicio)
            throw new ApplicationException("El abono inicial debe ser mayor que cero y no exceder el costo del servicio.");

        await using var db = FixTrackDb.Crear();
        // Orden + historial de creación + pago + historial de pago: un único SaveChangesAsync,
        // por lo que todo se confirma o deshace en una sola transacción.
        o.Estado = "Pendiente";
        o.FechaIngreso = DateTime.Now;
        db.OrdenesServicio.Add(o);
        db.HistorialOrdenes.Add(new HistorialOrden
        {
            Orden = o,
            UsuarioID = Sesion.UsuarioID > 0 ? Sesion.UsuarioID : null,
            TipoCambio = "Creacion",
            EstadoNuevo = "Pendiente",
            Comentario = "Orden creada con abono inicial",
            FechaCambio = DateTime.Now
        });

        pago.Orden = o;
        pago.FechaPago = DateTime.Now;
        db.Pagos.Add(pago);
        db.HistorialOrdenes.Add(new HistorialOrden
        {
            Orden = o,
            UsuarioID = Sesion.UsuarioID > 0 ? Sesion.UsuarioID : null,
            TipoCambio = "Pago",
            CampoModificado = "Pago inicial",
            ValorNuevo = pago.Monto.ToString("F2"),
            Comentario = "Abono inicial registrado",
            FechaCambio = DateTime.Now
        });

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 547 }
            or Microsoft.Data.SqlClient.SqlException { Number: 515 })
        {
            // Violación de CHECK (Monto > 0) o de FK: mismo error de negocio que la versión anterior.
            throw new ApplicationException("No se pudo registrar la orden con el abono inicial. Verifique el monto y el costo del servicio.", ex);
        }
        return o.OrdenID;
    }
}
