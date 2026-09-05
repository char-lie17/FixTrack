using System.Data;
using System.Text;
using FixTrack.Modelos;
using Microsoft.Data.SqlClient;

namespace FixTrack.Datos;

public static class OrdenServicioDAL
{
    private const string SelectConJoin = @"
SELECT o.OrdenID, o.DispositivoID, o.TecnicoID, o.FechaIngreso, o.ProblemaReportado,
       o.Diagnostico, o.TrabajoRealizado, o.Estado, o.CostoServicio, o.FechaFinalizacion,
       o.Observaciones,
       c.Nombre + ' ' + c.Apellido AS ClienteNombre,
       d.Tipo + ' ' + ISNULL(d.Marca, '') + ' ' + ISNULL(d.Modelo, '') AS DispositivoTexto,
       ISNULL(t.Nombre + ' ' + t.Apellido, 'Sin asignar') AS TecnicoNombre
FROM OrdenesServicio o
INNER JOIN Dispositivos d ON o.DispositivoID = d.DispositivoID
INNER JOIN Clientes c ON d.ClienteID = c.ClienteID
LEFT JOIN Tecnicos t ON o.TecnicoID = t.TecnicoID";

    public static List<OrdenServicio> ObtenerTodos()
    {
        var lista = new List<OrdenServicio>();
        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var cmd = new SqlCommand(SelectConJoin + " ORDER BY " + OrdenFlujoEstado + ", o.FechaIngreso DESC, o.OrdenID DESC", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) lista.Add(Leer(reader));
        return lista;
    }

    /// <summary>
    /// Expresión ORDER BY que respeta el flujo real de una orden (Pendiente → En diagnostico →
    /// En reparacion → Listo → Entregado) en vez del orden alfabético por defecto de o.Estado
    /// (hallazgo f de la auditoría).
    /// </summary>
    private const string OrdenFlujoEstado = @"CASE o.Estado
        WHEN 'Pendiente' THEN 1
        WHEN 'En diagnostico' THEN 2
        WHEN 'En reparacion' THEN 3
        WHEN 'Listo' THEN 4
        WHEN 'Entregado' THEN 5
        ELSE 6 END";

public static List<OrdenServicio> Buscar(string? texto, string? estado, DateTime? desde, DateTime? hasta, int? tecnicoId = null)
    {
        var textoLimpio = texto?.Trim();
        var esId = int.TryParse(textoLimpio, out var idBuscado);

        var sql = new StringBuilder(SelectConJoin);
        sql.Append(" WHERE 1 = 1");
        if (esId)
        {
            sql.Append(" AND o.OrdenID = @OrdenIDBuscado");
        }
        else if (!string.IsNullOrWhiteSpace(textoLimpio))
        {
            sql.Append(" AND (c.Nombre LIKE @Texto OR c.Apellido LIKE @Texto OR (c.Nombre + ' ' + c.Apellido) LIKE @Texto OR d.Tipo LIKE @Texto)");
        }
        if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
            sql.Append(" AND o.Estado = @Estado");
        if (desde.HasValue)
            sql.Append(" AND o.FechaIngreso >= @Desde");
        if (hasta.HasValue)
            sql.Append(" AND o.FechaIngreso < DATEADD(DAY, 1, @Hasta)");
        if (tecnicoId.HasValue)
            sql.Append(" AND o.TecnicoID = @TecnicoID");
        sql.Append(" ORDER BY ").Append(OrdenFlujoEstado).Append(", o.FechaIngreso DESC, o.OrdenID DESC");

        var lista = new List<OrdenServicio>();
        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var cmd = new SqlCommand(sql.ToString(), conn);
        if (esId)
            cmd.Parameters.AddWithValue("@OrdenIDBuscado", idBuscado);
        else if (!string.IsNullOrWhiteSpace(textoLimpio))
            cmd.Parameters.AddWithValue("@Texto", $"%{textoLimpio}%");
        if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
            cmd.Parameters.AddWithValue("@Estado", estado);
        if (desde.HasValue)
            cmd.Parameters.AddWithValue("@Desde", desde.Value);
        if (hasta.HasValue)
            cmd.Parameters.AddWithValue("@Hasta", hasta.Value);
        if (tecnicoId.HasValue)
            cmd.Parameters.AddWithValue("@TecnicoID", tecnicoId.Value);
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) lista.Add(Leer(reader));
        return lista;
    }

    public static List<OrdenServicio> ObtenerPorTecnico(int tecnicoId)
    {
        var lista = new List<OrdenServicio>();
        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var cmd = new SqlCommand(SelectConJoin + " WHERE o.TecnicoID = @TecnicoID ORDER BY o.FechaIngreso DESC, o.OrdenID DESC", conn);
        cmd.Parameters.AddWithValue("@TecnicoID", tecnicoId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) lista.Add(Leer(reader));
        return lista;
    }

    public static OrdenServicio? ObtenerPorId(int id)
    {
        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var cmd = new SqlCommand(SelectConJoin + " WHERE o.OrdenID = @OrdenID", conn);
        cmd.Parameters.AddWithValue("@OrdenID", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Leer(reader) : null;
    }

    public static List<Pago> ObtenerPagosPorOrden(int ordenId)
    {
        return PagoDAL.ObtenerPorOrden(ordenId);
    }

    public static int Insertar(OrdenServicio o)
    {
        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var cmd = new SqlCommand(@"
INSERT INTO OrdenesServicio (DispositivoID, TecnicoID, FechaIngreso, ProblemaReportado,
                             Estado, CostoServicio, Observaciones)
VALUES (@DispositivoID, @TecnicoID, GETDATE(), @ProblemaReportado,
        'Pendiente', @CostoServicio, @Observaciones);
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn);
        cmd.Parameters.AddWithValue("@DispositivoID", o.DispositivoID);
        cmd.Parameters.AddWithValue("@TecnicoID", (object?)o.TecnicoID ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProblemaReportado", o.ProblemaReportado);
        cmd.Parameters.AddWithValue("@CostoServicio", o.CostoServicio);
        cmd.Parameters.AddWithValue("@Observaciones", (object?)o.Observaciones ?? DBNull.Value);
        return (int)cmd.ExecuteScalar();
    }

    public static bool ActualizarDetalle(OrdenServicio o)
    {
        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var cmd = new SqlCommand(@"
UPDATE OrdenesServicio SET Diagnostico = @Diagnostico, TrabajoRealizado = @TrabajoRealizado,
    CostoServicio = @CostoServicio, Observaciones = @Observaciones,
    FechaFinalizacion = @FechaFinalizacion
WHERE OrdenID = @OrdenID", conn);
        cmd.Parameters.AddWithValue("@Diagnostico", (object?)o.Diagnostico ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@TrabajoRealizado", (object?)o.TrabajoRealizado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CostoServicio", o.CostoServicio);
        cmd.Parameters.AddWithValue("@Observaciones", (object?)o.Observaciones ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FechaFinalizacion", (object?)o.FechaFinalizacion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@OrdenID", o.OrdenID);
        return cmd.ExecuteNonQuery() > 0;
    }

    /// <summary>
    /// Obtiene el estado actual de una orden (usado para validar la transición antes de escribir).
    /// </summary>
    public static string? ObtenerEstadoActual(int id)
    {
        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var cmd = new SqlCommand("SELECT Estado FROM OrdenesServicio WHERE OrdenID = @OrdenID", conn);
        cmd.Parameters.AddWithValue("@OrdenID", id);
        var resultado = cmd.ExecuteScalar();
        return resultado == DBNull.Value || resultado == null ? null : (string)resultado;
    }

    /// <summary>
    /// Cambia el estado de la orden validando la transición contra el flujo real.
    /// Al pasar a 'Listo' o 'Entregado' se registra la FechaFinalizacion si aún no existe.
    /// Devuelve false si la orden no existe o si la transición no es válida.
    /// </summary>
    public static bool ActualizarEstado(int id, string nuevoEstado)
    {
        var estadoActual = ObtenerEstadoActual(id);
        if (estadoActual == null || !EstadoOrdenTexto.EsTransicionValida(estadoActual, nuevoEstado))
            return false;

        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var cmd = new SqlCommand(@"
UPDATE OrdenesServicio SET Estado = @Estado,
    FechaFinalizacion = CASE
        WHEN @Estado IN ('Listo', 'Entregado') AND FechaFinalizacion IS NULL THEN GETDATE()
        WHEN @Estado NOT IN ('Listo', 'Entregado') THEN NULL
        ELSE FechaFinalizacion END
WHERE OrdenID = @OrdenID AND Estado = @EstadoActual", conn);
        cmd.Parameters.AddWithValue("@EstadoActual", estadoActual);
        cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
        cmd.Parameters.AddWithValue("@OrdenID", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    private static OrdenServicio Leer(SqlDataReader r) => new()
    {
        OrdenID = r.GetInt32(0),
        DispositivoID = r.GetInt32(1),
        TecnicoID = r.IsDBNull(2) ? null : r.GetInt32(2),
        FechaIngreso = r.GetDateTime(3),
        ProblemaReportado = r.GetString(4),
        Diagnostico = r.IsDBNull(5) ? null : r.GetString(5),
        TrabajoRealizado = r.IsDBNull(6) ? null : r.GetString(6),
        Estado = r.GetString(7),
        CostoServicio = r.GetDecimal(8),
        FechaFinalizacion = r.IsDBNull(9) ? null : r.GetDateTime(9),
        Observaciones = r.IsDBNull(10) ? null : r.GetString(10),
        ClienteNombre = r.GetString(11),
        DispositivoTexto = r.GetString(12),
        TecnicoNombre = r.GetString(13)
    };
/// <summary>Conteo de órdenes por estado (para las 5 métricas del Dashboard).</summary>
    public static Dictionary<string, int> ObtenerConteoPorEstado()
    {
        var resultado = new Dictionary<string, int>
        {
            ["Pendiente"] = 0,
            ["En diagnostico"] = 0,
            ["En reparacion"] = 0,
            ["Listo"] = 0,
            ["Entregado"] = 0
        };
        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var cmd = new SqlCommand("SELECT Estado, COUNT(*) FROM OrdenesServicio GROUP BY Estado", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var valor = reader.GetString(0);
            if (resultado.ContainsKey(valor)) resultado[valor] = reader.GetInt32(1);
        }
        return resultado;
    }

    /// <summary>Conteo de órdenes por estado filtrado por técnico (para métricas del Dashboard).</summary>
    public static Dictionary<string, int> ObtenerConteoPorEstado(int tecnicoId)
    {
        var resultado = new Dictionary<string, int>
        {
            ["Pendiente"] = 0,
            ["En diagnostico"] = 0,
            ["En reparacion"] = 0,
            ["Listo"] = 0,
            ["Entregado"] = 0
        };
        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var cmd = new SqlCommand(@"
SELECT Estado, COUNT(*) FROM OrdenesServicio
WHERE TecnicoID = @TecnicoID
GROUP BY Estado", conn);
        cmd.Parameters.AddWithValue("@TecnicoID", tecnicoId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var valor = reader.GetString(0);
if (resultado.ContainsKey(valor)) resultado[valor] = reader.GetInt32(1);
        }
        return resultado;
    }

    /// <summary>
    /// Escenario transaccional del plan §4.7: crea la orden de servicio y registra
    /// el pago inicial en una misma transacción (Todo o Nada).
    /// </summary>
    public static int InsertarConPagoInicial(OrdenServicio o, Pago pago)
    {
        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            int ordenId;
            using (var cmd = new SqlCommand(@"
INSERT INTO OrdenesServicio (DispositivoID, TecnicoID, FechaIngreso, ProblemaReportado,
                             Estado, CostoServicio, Observaciones)
VALUES (@DispositivoID, @TecnicoID, GETDATE(), @ProblemaReportado,
        'Pendiente', @CostoServicio, @Observaciones);
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tx))
            {
                cmd.Parameters.AddWithValue("@DispositivoID", o.DispositivoID);
                cmd.Parameters.AddWithValue("@TecnicoID", (object?)o.TecnicoID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProblemaReportado", o.ProblemaReportado);
                cmd.Parameters.AddWithValue("@CostoServicio", o.CostoServicio);
                cmd.Parameters.AddWithValue("@Observaciones", (object?)o.Observaciones ?? DBNull.Value);
                ordenId = (int)cmd.ExecuteScalar();
            }

            using (var cmd = new SqlCommand(@"
INSERT INTO Pagos (OrdenID, FechaPago, Monto, MetodoPago, Observaciones)
VALUES (@OrdenID, GETDATE(), @Monto, @MetodoPago, @Observaciones);", conn, tx))
            {
                cmd.Parameters.AddWithValue("@OrdenID", ordenId);
                cmd.Parameters.AddWithValue("@Monto", pago.Monto);
                cmd.Parameters.AddWithValue("@MetodoPago", pago.MetodoPago);
                cmd.Parameters.AddWithValue("@Observaciones", (object?)pago.Observaciones ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }

            tx.Commit();
            return ordenId;
        }
        catch
        {
            tx.Rollback();
throw;
        }
    }
}
