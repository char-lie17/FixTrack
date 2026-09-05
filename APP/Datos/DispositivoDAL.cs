using System.Text;
using FixTrack.Modelos;
using Microsoft.Data.SqlClient;

namespace FixTrack.Datos;

public static class DispositivoDAL
{
    private const string SelectConJoin = @"
SELECT d.DispositivoID, d.ClienteID, d.Tipo, d.Marca, d.Modelo, d.NumeroSerie,
       d.Descripcion, d.FechaRegistro,
       c.Nombre + ' ' + c.Apellido AS ClienteNombre
FROM Dispositivos d
INNER JOIN Clientes c ON d.ClienteID = c.ClienteID";

    public static List<Dispositivo> ObtenerTodos()
    {
        try
        {
            var lista = new List<Dispositivo>();
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(SelectConJoin + " ORDER BY d.FechaRegistro DESC, d.DispositivoID DESC", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Leer(reader));
            return lista;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener la lista de dispositivos.", ex);
        }
    }

    public static List<Dispositivo> Buscar(string? texto)
    {
        try
        {
            var textoLimpio = texto?.Trim();
            var esId = int.TryParse(textoLimpio, out var idBuscado);

            var sql = new StringBuilder(SelectConJoin);
            sql.Append(" WHERE 1 = 1");
            if (esId)
                sql.Append(" AND d.DispositivoID = @DispositivoIDBuscado");
            else if (!string.IsNullOrWhiteSpace(textoLimpio))
                sql.Append(" AND (d.Tipo LIKE @Texto OR ISNULL(d.Marca, '') LIKE @Texto OR ISNULL(d.Modelo, '') LIKE @Texto OR ISNULL(d.NumeroSerie, '') LIKE @Texto OR c.Nombre LIKE @Texto OR c.Apellido LIKE @Texto OR (c.Nombre + ' ' + c.Apellido) LIKE @Texto)");
            sql.Append(" ORDER BY d.FechaRegistro DESC, d.DispositivoID DESC");

            var lista = new List<Dispositivo>();
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(sql.ToString(), conn);
            if (esId)
                cmd.Parameters.AddWithValue("@DispositivoIDBuscado", idBuscado);
            else if (!string.IsNullOrWhiteSpace(textoLimpio))
                cmd.Parameters.AddWithValue("@Texto", $"%{textoLimpio}%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Leer(reader));
            return lista;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al buscar dispositivos.", ex);
        }
    }

    public static List<Dispositivo> ObtenerPorCliente(int clienteId)
    {
        try
        {
            var lista = new List<Dispositivo>();
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(SelectConJoin + " WHERE d.ClienteID = @ClienteID ORDER BY d.FechaRegistro DESC, d.DispositivoID DESC", conn);
            cmd.Parameters.AddWithValue("@ClienteID", clienteId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Leer(reader));
            return lista;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener los dispositivos del cliente.", ex);
        }
    }

    public static Dispositivo? ObtenerPorId(int id)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(SelectConJoin + " WHERE d.DispositivoID = @DispositivoID", conn);
            cmd.Parameters.AddWithValue("@DispositivoID", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Leer(reader) : null;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener el dispositivo.", ex);
        }
    }

    public static int Insertar(Dispositivo d)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(@"
INSERT INTO Dispositivos (ClienteID, Tipo, Marca, Modelo, NumeroSerie, Descripcion, FechaRegistro)
VALUES (@ClienteID, @Tipo, @Marca, @Modelo, @NumeroSerie, @Descripcion, GETDATE());
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn);
            cmd.Parameters.AddWithValue("@ClienteID", d.ClienteID);
            cmd.Parameters.AddWithValue("@Tipo", d.Tipo);
            cmd.Parameters.AddWithValue("@Marca", (object?)d.Marca ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Modelo", (object?)d.Modelo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NumeroSerie", (object?)d.NumeroSerie ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)d.Descripcion ?? DBNull.Value);
            return (int)cmd.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al registrar el dispositivo.", ex);
        }
    }

    public static bool Actualizar(Dispositivo d)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(@"
UPDATE Dispositivos SET ClienteID = @ClienteID, Tipo = @Tipo, Marca = @Marca,
    Modelo = @Modelo, NumeroSerie = @NumeroSerie, Descripcion = @Descripcion
WHERE DispositivoID = @DispositivoID", conn);
            cmd.Parameters.AddWithValue("@ClienteID", d.ClienteID);
            cmd.Parameters.AddWithValue("@Tipo", d.Tipo);
            cmd.Parameters.AddWithValue("@Marca", (object?)d.Marca ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Modelo", (object?)d.Modelo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NumeroSerie", (object?)d.NumeroSerie ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)d.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DispositivoID", d.DispositivoID);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al actualizar el dispositivo.", ex);
        }
    }

    private static Dispositivo Leer(SqlDataReader r) => new()
    {
        DispositivoID = r.GetInt32(0),
        ClienteID = r.GetInt32(1),
        Tipo = r.GetString(2),
        Marca = r.IsDBNull(3) ? null : r.GetString(3),
        Modelo = r.IsDBNull(4) ? null : r.GetString(4),
        NumeroSerie = r.IsDBNull(5) ? null : r.GetString(5),
        Descripcion = r.IsDBNull(6) ? null : r.GetString(6),
        FechaRegistro = r.GetDateTime(7),
        ClienteNombre = r.GetString(8)
    };
}