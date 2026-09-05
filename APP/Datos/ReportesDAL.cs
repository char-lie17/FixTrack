using System.Data;
using Microsoft.Data.SqlClient;

namespace FixTrack.Datos;

public static class ReportesDAL
{
    /// <summary>Reporte 1: cantidades y subtotales de órdenes agrupadas por estado.</summary>
    public static DataTable ObtenerOrdenesPorEstado(DateTime desde, DateTime hasta)
    {
        try
        {
            return Ejecutar(@"
SELECT o.Estado, COUNT(*) AS Cantidad, SUM(o.CostoServicio) AS Subtotal
FROM OrdenesServicio o
WHERE o.FechaIngreso >= @Desde AND o.FechaIngreso < DATEADD(DAY, 1, @Hasta)
GROUP BY o.Estado
ORDER BY o.Estado", desde, hasta);
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al generar el reporte de órdenes por estado.", ex);
        }
    }

    /// <summary>Reporte 2: cantidades de órdenes por técnico asignado.</summary>
    public static DataTable ObtenerOrdenesPorTecnico(DateTime desde, DateTime hasta)
    {
        try
        {
            return Ejecutar(@"
SELECT ISNULL(t.Nombre + ' ' + t.Apellido, 'Sin asignar') AS Tecnico,
       COUNT(*) AS Cantidad,
       SUM(o.CostoServicio) AS Subtotal
FROM OrdenesServicio o
LEFT JOIN Tecnicos t ON o.TecnicoID = t.TecnicoID
WHERE o.FechaIngreso >= @Desde AND o.FechaIngreso < DATEADD(DAY, 1, @Hasta)
GROUP BY t.Nombre, t.Apellido
ORDER BY Tecnico", desde, hasta);
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al generar el reporte de órdenes por técnico.", ex);
        }
    }

    /// <summary>Reporte 3: servicios completados (Listo o Entregado) en el rango.</summary>
    public static DataTable ObtenerServiciosCompletados(DateTime desde, DateTime hasta)
    {
        try
        {
            return Ejecutar(@"
SELECT COUNT(*) AS Cantidad,
       ISNULL(SUM(CostoServicio), 0) AS TotalFacturado,
       MIN(FechaFinalizacion) AS PrimerFinalizado,
       MAX(FechaFinalizacion) AS UltimoFinalizado
FROM OrdenesServicio
WHERE Estado = 'Entregado'
  AND FechaFinalizacion >= @Desde
  AND FechaFinalizacion < DATEADD(DAY, 1, @Hasta)", desde, hasta);
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al generar el reporte de servicios completados.", ex);
        }
    }

    /// <summary>Reporte 4: pagos registrados con total acumulado.</summary>
    public static DataTable ObtenerPagosRegistrados(DateTime desde, DateTime hasta)
    {
        try
        {
            return Ejecutar(@"
SELECT MetodoPago, COUNT(*) AS Cantidad, SUM(Monto) AS Total
FROM Pagos
WHERE FechaPago >= @Desde AND FechaPago < DATEADD(DAY, 1, @Hasta)
GROUP BY MetodoPago
ORDER BY MetodoPago", desde, hasta);
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al generar el reporte de pagos registrados.", ex);
        }
    }

    private static DataTable Ejecutar(string sql, DateTime desde, DateTime hasta)
    {
        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Desde", desde.Date);
        cmd.Parameters.AddWithValue("@Hasta", hasta.Date);
        using var adapter = new SqlDataAdapter(cmd);
        var tabla = new DataTable();
        adapter.Fill(tabla);
        return tabla;
    }
}