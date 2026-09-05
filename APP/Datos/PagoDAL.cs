using System.Data;
using System.Text;
using FixTrack.Modelos;
using Microsoft.Data.SqlClient;

namespace FixTrack.Datos;

public static class PagoDAL
{
    private const string SelectConJoin = @"
SELECT p.PagoID, p.OrdenID, p.FechaPago, p.Monto, p.MetodoPago, p.Observaciones,
       c.Nombre + ' ' + c.Apellido AS ClienteNombre
FROM Pagos p
INNER JOIN OrdenesServicio o ON p.OrdenID = o.OrdenID
INNER JOIN Dispositivos d ON o.DispositivoID = d.DispositivoID
INNER JOIN Clientes c ON d.ClienteID = c.ClienteID";

    public static List<Pago> ObtenerTodos()
    {
        try
        {
            return Buscar(null, null, null, null);
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener la lista de pagos.", ex);
        }
    }

    public static List<Pago> Buscar(string? metodo, DateTime? desde, DateTime? hasta, string? texto)
    {
        try
        {
            var textoLimpio = texto?.Trim();
            var esId = int.TryParse(textoLimpio, out var idBuscado);

            var sql = new StringBuilder(SelectConJoin);
            sql.Append(" WHERE 1 = 1");
            if (!string.IsNullOrWhiteSpace(metodo) && metodo != "Todos")
                sql.Append(" AND p.MetodoPago = @Metodo");
            if (desde.HasValue)
                sql.Append(" AND p.FechaPago >= @Desde");
            if (hasta.HasValue)
                sql.Append(" AND p.FechaPago < DATEADD(DAY, 1, @Hasta)");
            if (esId)
                sql.Append(" AND p.OrdenID = @OrdenIDBuscado");
            else if (!string.IsNullOrWhiteSpace(textoLimpio))
                sql.Append(" AND (c.Nombre LIKE @Texto OR c.Apellido LIKE @Texto OR (c.Nombre + ' ' + c.Apellido) LIKE @Texto)");
            sql.Append(" ORDER BY p.FechaPago DESC, p.PagoID DESC");

            var lista = new List<Pago>();
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(sql.ToString(), conn);
            if (!string.IsNullOrWhiteSpace(metodo) && metodo != "Todos")
                cmd.Parameters.AddWithValue("@Metodo", metodo);
            if (desde.HasValue)
                cmd.Parameters.AddWithValue("@Desde", desde.Value);
            if (hasta.HasValue)
                cmd.Parameters.AddWithValue("@Hasta", hasta.Value);
            if (esId)
                cmd.Parameters.AddWithValue("@OrdenIDBuscado", idBuscado);
            else if (!string.IsNullOrWhiteSpace(textoLimpio))
                cmd.Parameters.AddWithValue("@Texto", $"%{textoLimpio}%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Leer(reader));
            return lista;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al buscar pagos.", ex);
        }
    }

    public static List<Pago> ObtenerPorOrden(int ordenId)
    {
        try
        {
            var lista = new List<Pago>();
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(SelectConJoin + " WHERE p.OrdenID = @OrdenID ORDER BY p.FechaPago, p.PagoID", conn);
            cmd.Parameters.AddWithValue("@OrdenID", ordenId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Leer(reader));
            return lista;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener los pagos de la orden.", ex);
        }
    }

    public static Pago? ObtenerPorId(int id)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(SelectConJoin + " WHERE p.PagoID = @PagoID", conn);
            cmd.Parameters.AddWithValue("@PagoID", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Leer(reader) : null;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener el pago.", ex);
        }
    }

    public static int Insertar(Pago p)
    {
        return RegistrarPagoSeguro(p);
    }

    /// <summary>Valida el saldo e inserta el pago bloqueando la orden en una sola transacción.</summary>
    public static int RegistrarPagoSeguro(Pago p)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                decimal costo;
                decimal totalPagado;
                using (var saldoCmd = new SqlCommand(@"
SELECT o.CostoServicio, ISNULL(SUM(p.Monto), 0)
FROM OrdenesServicio o WITH (UPDLOCK, HOLDLOCK)
LEFT JOIN Pagos p ON p.OrdenID = o.OrdenID
WHERE o.OrdenID = @OrdenID
GROUP BY o.CostoServicio", conn, tx))
                {
                    saldoCmd.Parameters.AddWithValue("@OrdenID", p.OrdenID);
                    using var reader = saldoCmd.ExecuteReader();
                    if (!reader.Read()) throw new ApplicationException("La orden seleccionada no existe.");
                    costo = reader.GetDecimal(0);
                    totalPagado = reader.GetDecimal(1);
                }

                var saldo = costo - totalPagado;
                if (p.Monto <= 0 || p.Monto > saldo)
                    throw new ApplicationException($"El pago no puede exceder el saldo pendiente. Saldo disponible: {saldo:C2}.");

                int pagoId;
                using (var cmd = new SqlCommand(@"
INSERT INTO Pagos (OrdenID, FechaPago, Monto, MetodoPago, Observaciones)
VALUES (@OrdenID, GETDATE(), @Monto, @MetodoPago, @Observaciones);
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@OrdenID", p.OrdenID);
                    cmd.Parameters.AddWithValue("@Monto", p.Monto);
                    cmd.Parameters.AddWithValue("@MetodoPago", p.MetodoPago);
                    cmd.Parameters.AddWithValue("@Observaciones", (object?)p.Observaciones ?? DBNull.Value);
                    pagoId = (int)cmd.ExecuteScalar();
                }

                using (var historialCmd = new SqlCommand(@"
INSERT INTO HistorialOrdenes (OrdenID, UsuarioID, TipoCambio, CampoModificado, ValorNuevo, Comentario)
VALUES (@OrdenID, @UsuarioID, 'Pago', 'Pago', @ValorNuevo, 'Pago registrado')", conn, tx))
                {
                    historialCmd.Parameters.AddWithValue("@OrdenID", p.OrdenID);
                    historialCmd.Parameters.AddWithValue("@UsuarioID", Sesion.UsuarioID > 0 ? Sesion.UsuarioID : DBNull.Value);
                    historialCmd.Parameters.AddWithValue("@ValorNuevo", p.Monto.ToString("F2"));
                    historialCmd.ExecuteNonQuery();
                }

                tx.Commit();
                return pagoId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al registrar el pago.", ex);
        }
    }

    /// <summary>Total pagado acumulado de una orden (para mostrar saldo pendiente).</summary>
    public static decimal ObtenerTotalPagado(int ordenId)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand("SELECT ISNULL(SUM(Monto), 0) FROM Pagos WHERE OrdenID = @OrdenID", conn);
            cmd.Parameters.AddWithValue("@OrdenID", ordenId);
            return Convert.ToDecimal(cmd.ExecuteScalar());
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener el total pagado.", ex);
        }
    }

    private static Pago Leer(SqlDataReader r) => new()
    {
        PagoID = r.GetInt32(0),
        OrdenID = r.GetInt32(1),
        FechaPago = r.GetDateTime(2),
        Monto = r.GetDecimal(3),
        MetodoPago = r.GetString(4),
        Observaciones = r.IsDBNull(5) ? null : r.GetString(5),
        ClienteNombre = r.GetString(6)
    };
}
