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

    public static List<HistorialOrden> ObtenerHistorial(int ordenId)
    {
        var lista = new List<HistorialOrden>();
        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var cmd = new SqlCommand(@"
SELECT h.HistorialID, h.OrdenID, h.UsuarioID, h.FechaCambio, h.TipoCambio,
       h.EstadoAnterior, h.EstadoNuevo, h.CampoModificado, h.ValorAnterior,
       h.ValorNuevo, h.Comentario, ISNULL(u.NombreUsuario, 'Sistema')
FROM HistorialOrdenes h
LEFT JOIN Usuarios u ON h.UsuarioID = u.UsuarioID
WHERE h.OrdenID = @OrdenID
ORDER BY h.FechaCambio DESC, h.HistorialID DESC", conn);
        cmd.Parameters.AddWithValue("@OrdenID", ordenId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) lista.Add(LeerHistorial(reader));
        return lista;
    }

    public static int Insertar(OrdenServicio o)
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
                AgregarParametrosOrden(cmd, o);
                ordenId = (int)cmd.ExecuteScalar();
            }
            InsertarHistorial(conn, tx, ordenId, "Creacion", null, "Pendiente", null, null, null, "Orden creada");
            tx.Commit();
            return ordenId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public static bool ActualizarDetalle(OrdenServicio o)
    {
        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            var anterior = ObtenerPorId(conn, tx, o.OrdenID);
            if (anterior == null)
            {
                tx.Rollback();
                return false;
            }

            using var cmd = new SqlCommand(@"
UPDATE OrdenesServicio SET ProblemaReportado = @ProblemaReportado,
    Diagnostico = @Diagnostico, TrabajoRealizado = @TrabajoRealizado,
    CostoServicio = @CostoServicio, Observaciones = @Observaciones,
    FechaFinalizacion = @FechaFinalizacion
WHERE OrdenID = @OrdenID", conn, tx);
            cmd.Parameters.AddWithValue("@ProblemaReportado", o.ProblemaReportado);
            cmd.Parameters.AddWithValue("@Diagnostico", (object?)o.Diagnostico ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TrabajoRealizado", (object?)o.TrabajoRealizado ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CostoServicio", o.CostoServicio);
            cmd.Parameters.AddWithValue("@Observaciones", (object?)o.Observaciones ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaFinalizacion", (object?)o.FechaFinalizacion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OrdenID", o.OrdenID);
            if (cmd.ExecuteNonQuery() == 0)
            {
                tx.Rollback();
                return false;
            }

            RegistrarCambio(conn, tx, o.OrdenID, "ProblemaReportado", anterior.ProblemaReportado, o.ProblemaReportado);
            RegistrarCambio(conn, tx, o.OrdenID, "Diagnostico", anterior.Diagnostico, o.Diagnostico);
            RegistrarCambio(conn, tx, o.OrdenID, "TrabajoRealizado", anterior.TrabajoRealizado, o.TrabajoRealizado);
            RegistrarCambio(conn, tx, o.OrdenID, "CostoServicio", anterior.CostoServicio.ToString("F2"), o.CostoServicio.ToString("F2"));
            RegistrarCambio(conn, tx, o.OrdenID, "Observaciones", anterior.Observaciones, o.Observaciones);
            RegistrarCambio(conn, tx, o.OrdenID, "FechaFinalizacion", anterior.FechaFinalizacion?.ToString("O"), o.FechaFinalizacion?.ToString("O"));
            tx.Commit();
            return true;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
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
        using var conn = Conexion.ObtenerConexion();
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            using var estadoCmd = new SqlCommand("SELECT Estado FROM OrdenesServicio WITH (UPDLOCK, HOLDLOCK) WHERE OrdenID = @OrdenID", conn, tx);
            estadoCmd.Parameters.AddWithValue("@OrdenID", id);
            var estadoResultado = estadoCmd.ExecuteScalar();
            var estadoActual = estadoResultado as string;
            if (estadoActual == null || !EstadoOrdenTexto.EsTransicionValida(estadoActual, nuevoEstado))
            {
                tx.Rollback();
                return false;
            }

            using var cmd = new SqlCommand(@"
UPDATE OrdenesServicio SET Estado = @Estado,
    FechaFinalizacion = CASE
        WHEN @Estado IN ('Listo', 'Entregado') AND FechaFinalizacion IS NULL THEN GETDATE()
        WHEN @Estado NOT IN ('Listo', 'Entregado') THEN NULL
        ELSE FechaFinalizacion END
WHERE OrdenID = @OrdenID AND Estado = @EstadoActual", conn, tx);
            cmd.Parameters.AddWithValue("@EstadoActual", estadoActual);
            cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
            cmd.Parameters.AddWithValue("@OrdenID", id);
            if (cmd.ExecuteNonQuery() == 0)
            {
                tx.Rollback();
                return false;
            }
            InsertarHistorial(conn, tx, id, "Estado", estadoActual, nuevoEstado, null, null, null, "Cambio de estado");
            tx.Commit();
            return true;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    private static void AgregarParametrosOrden(SqlCommand cmd, OrdenServicio o)
    {
        cmd.Parameters.AddWithValue("@DispositivoID", o.DispositivoID);
        cmd.Parameters.AddWithValue("@TecnicoID", (object?)o.TecnicoID ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProblemaReportado", o.ProblemaReportado);
        cmd.Parameters.AddWithValue("@CostoServicio", o.CostoServicio);
        cmd.Parameters.AddWithValue("@Observaciones", (object?)o.Observaciones ?? DBNull.Value);
    }

    private static OrdenServicio? ObtenerPorId(SqlConnection conn, SqlTransaction tx, int id)
    {
        using var cmd = new SqlCommand(SelectConJoin + " WHERE o.OrdenID = @OrdenID", conn, tx);
        cmd.Parameters.AddWithValue("@OrdenID", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Leer(reader) : null;
    }

    private static void RegistrarCambio(SqlConnection conn, SqlTransaction tx, int ordenId, string campo, string? anterior, string? nuevo)
    {
        if (string.Equals(anterior, nuevo, StringComparison.Ordinal)) return;
        InsertarHistorial(conn, tx, ordenId, "Edicion", null, null, campo, anterior, nuevo, "Campo actualizado");
    }

    private static void InsertarHistorial(SqlConnection conn, SqlTransaction tx, int ordenId, string tipo, string? estadoAnterior,
        string? estadoNuevo, string? campo, string? valorAnterior, string? valorNuevo, string? comentario)
    {
        using var cmd = new SqlCommand(@"
INSERT INTO HistorialOrdenes (OrdenID, UsuarioID, TipoCambio, EstadoAnterior, EstadoNuevo,
                              CampoModificado, ValorAnterior, ValorNuevo, Comentario)
VALUES (@OrdenID, @UsuarioID, @TipoCambio, @EstadoAnterior, @EstadoNuevo,
        @Campo, @ValorAnterior, @ValorNuevo, @Comentario)", conn, tx);
        cmd.Parameters.AddWithValue("@OrdenID", ordenId);
        cmd.Parameters.AddWithValue("@UsuarioID", Sesion.UsuarioID > 0 ? Sesion.UsuarioID : DBNull.Value);
        cmd.Parameters.AddWithValue("@TipoCambio", tipo);
        cmd.Parameters.AddWithValue("@EstadoAnterior", (object?)estadoAnterior ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@EstadoNuevo", (object?)estadoNuevo ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Campo", (object?)campo ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ValorAnterior", (object?)valorAnterior ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ValorNuevo", (object?)valorNuevo ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Comentario", (object?)comentario ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    private static HistorialOrden LeerHistorial(SqlDataReader r) => new()
    {
        HistorialID = r.GetInt32(0),
        OrdenID = r.GetInt32(1),
        UsuarioID = r.IsDBNull(2) ? null : r.GetInt32(2),
        FechaCambio = r.GetDateTime(3),
        TipoCambio = r.GetString(4),
        EstadoAnterior = r.IsDBNull(5) ? null : r.GetString(5),
        EstadoNuevo = r.IsDBNull(6) ? null : r.GetString(6),
        CampoModificado = r.IsDBNull(7) ? null : r.GetString(7),
        ValorAnterior = r.IsDBNull(8) ? null : r.GetString(8),
        ValorNuevo = r.IsDBNull(9) ? null : r.GetString(9),
        Comentario = r.IsDBNull(10) ? null : r.GetString(10),
        UsuarioNombre = r.GetString(11)
    };

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
            if (pago.Monto <= 0 || pago.Monto > o.CostoServicio)
                throw new ApplicationException("El abono inicial debe ser mayor que cero y no exceder el costo del servicio.");

            int ordenId;
            using (var cmd = new SqlCommand(@"
INSERT INTO OrdenesServicio (DispositivoID, TecnicoID, FechaIngreso, ProblemaReportado,
                             Estado, CostoServicio, Observaciones)
VALUES (@DispositivoID, @TecnicoID, GETDATE(), @ProblemaReportado,
        'Pendiente', @CostoServicio, @Observaciones);
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tx))
            {
                AgregarParametrosOrden(cmd, o);
                ordenId = (int)cmd.ExecuteScalar();
            }
            InsertarHistorial(conn, tx, ordenId, "Creacion", null, "Pendiente", null, null, null, "Orden creada con abono inicial");

            using (var cmd = new SqlCommand(@"
INSERT INTO Pagos (OrdenID, FechaPago, Monto, MetodoPago, Observaciones)
VALUES (@OrdenID, GETDATE(), @Monto, @MetodoPago, @Observaciones);", conn, tx))
            {
                cmd.Parameters.AddWithValue("@OrdenID", ordenId);
                cmd.Parameters.AddWithValue("@Monto", pago.Monto);
                cmd.Parameters.AddWithValue("@MetodoPago", pago.MetodoPago);
                cmd.Parameters.AddWithValue("@Observaciones", (object?)pago.Observaciones ?? DBNull.Value);
                if (cmd.ExecuteNonQuery() != 1)
                    throw new ApplicationException("No se pudo registrar el pago inicial.");
            }
            InsertarHistorial(conn, tx, ordenId, "Pago", null, null, "Pago inicial", null, pago.Monto.ToString("F2"), "Abono inicial registrado");

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
