using System.Data;
using FixTrack.Modelos;
using Microsoft.EntityFrameworkCore;

namespace FixTrack.Datos;

/// <summary>
/// Reportes migrados a EF Core. Se conserva el contrato de salida (DataTable) para no
/// modificar la interfaz, y la columna de resultados de cada reporte es idéntica a la versión ADO.NET.
/// </summary>
public static class ReportesDAL
{
    // Tipos de fila intermedios (propiedades seteables para que EF materialice el resultado).
    public sealed class FilaEstado
    {
        public string Estado { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }

    public sealed class FilaTecnico
    {
        public string Tecnico { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }

    public sealed class FilaServicio
    {
        public int Cantidad { get; set; }
        public decimal TotalFacturado { get; set; }
        public DateTime? PrimerFinalizado { get; set; }
        public DateTime? UltimoFinalizado { get; set; }
    }

    public sealed class FilaPago
    {
        public string MetodoPago { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }

    /// <summary>Reporte 1: cantidades y subtotales de órdenes agrupadas por estado.
    /// Sigue ejecutando el procedimiento almacenado sp_ReporteOrdenesPorEstado, ahora invocado a través de EF Core.</summary>
    public static async Task<DataTable> ObtenerOrdenesPorEstadoAsync(DateTime desde, DateTime hasta, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        var filas = await db.Database
            .SqlQuery<FilaEstado>($"EXEC dbo.sp_ReporteOrdenesPorEstado @Desde = {desde.Date}, @Hasta = {hasta.Date}")
            .ToListAsync(ct);
        return ADataTable(filas, "Estado", "Cantidad", "Subtotal");
    }

    /// <summary>Reporte 2: cantidades de órdenes por técnico asignado.</summary>
    public static async Task<DataTable> ObtenerOrdenesPorTecnicoAsync(DateTime desde, DateTime hasta, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        var filas = await db.OrdenesServicio.AsNoTracking()
            .Where(o => o.FechaIngreso >= desde.Date && o.FechaIngreso < hasta.Date.AddDays(1))
            .GroupBy(o => o.TecnicoID == null
                ? "Sin asignar"
                : o.Tecnico!.Nombre + " " + o.Tecnico!.Apellido)
            .Select(g => new FilaTecnico
            {
                Tecnico = g.Key,
                Cantidad = g.Count(),
                Subtotal = g.Sum(o => o.CostoServicio)
            })
            .OrderBy(x => x.Tecnico)
            .ToListAsync(ct);
        return ADataTable(filas, "Tecnico", "Cantidad", "Subtotal");
    }

    /// <summary>Reporte 3: servicios entregados en el rango.</summary>
    public static async Task<DataTable> ObtenerServiciosCompletadosAsync(DateTime desde, DateTime hasta, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        var fila = await db.OrdenesServicio.AsNoTracking()
            .Where(o => o.Estado == "Entregado"
                     && o.FechaFinalizacion >= desde.Date
                     && o.FechaFinalizacion < hasta.Date.AddDays(1))
            .GroupBy(_ => 1)
            .Select(g => new FilaServicio
            {
                Cantidad = g.Count(),
                TotalFacturado = g.Sum(o => o.CostoServicio),
                PrimerFinalizado = g.Min(o => o.FechaFinalizacion),
                UltimoFinalizado = g.Max(o => o.FechaFinalizacion)
            })
            .FirstOrDefaultAsync(ct) ?? new FilaServicio
            {
                Cantidad = 0,
                TotalFacturado = 0m,
                PrimerFinalizado = null,
                UltimoFinalizado = null
            };
        return ADataTable(new[] { fila }, "Cantidad", "TotalFacturado", "PrimerFinalizado", "UltimoFinalizado");
    }

    /// <summary>Reporte 4: pagos registrados con total acumulado.</summary>
    public static async Task<DataTable> ObtenerPagosRegistradosAsync(DateTime desde, DateTime hasta, CancellationToken ct = default)
    {
        await using var db = FixTrackDb.Crear();
        var filas = await db.Pagos.AsNoTracking()
            .Where(p => p.FechaPago >= desde.Date && p.FechaPago < hasta.Date.AddDays(1))
            .GroupBy(p => p.MetodoPago)
            .Select(g => new FilaPago
            {
                MetodoPago = g.Key,
                Cantidad = g.Count(),
                Total = g.Sum(p => p.Monto)
            })
            .OrderBy(x => x.MetodoPago)
            .ToListAsync(ct);
        return ADataTable(filas, "MetodoPago", "Cantidad", "Total");
    }

    /// <summary>Convierte las filas de resultado en un DataTable con las columnas esperadas por la interfaz.</summary>
    private static DataTable ADataTable<T>(IEnumerable<T> filas, params string[] columnas) where T : class
    {
        var tabla = new DataTable();
        foreach (var columna in columnas)
        {
            var tipo = typeof(T).GetProperty(columna)?.PropertyType ?? typeof(object);
            tabla.Columns.Add(columna, Nullable.GetUnderlyingType(tipo) ?? tipo);
        }
        foreach (var fila in filas)
        {
            var valores = new object?[columnas.Length];
            for (var i = 0; i < columnas.Length; i++)
            {
                valores[i] = typeof(T).GetProperty(columnas[i])?.GetValue(fila);
            }
            tabla.Rows.Add(valores);
        }
        return tabla;
    }
}
