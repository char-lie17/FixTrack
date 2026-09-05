using System.Text;
using FixTrack.Modelos;
using Microsoft.Data.SqlClient;

namespace FixTrack.Datos;

public static class ClienteDAL
{
    private const string Columnas =
        "ClienteID, Nombre, Apellido, Telefono, Email, Direccion, FechaRegistro, Estado";

    public static List<Cliente> ObtenerTodos()
    {
        try
        {
            var lista = new List<Cliente>();
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand($"SELECT {Columnas} FROM Clientes ORDER BY Nombre, Apellido", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Leer(reader));
            return lista;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener la lista de clientes.", ex);
        }
    }

    /// <summary>Solo clientes con Estado = 'Activo'. Usado al crear un dispositivo nuevo,
    /// igual que TecnicoDAL.ObtenerActivos() al asignar técnico (hallazgo g de la auditoría:
    /// antes se aplicaba esta regla a técnicos pero no a clientes).</summary>
    public static List<Cliente> ObtenerActivos()
    {
        try
        {
            var lista = new List<Cliente>();
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand($"SELECT {Columnas} FROM Clientes WHERE Estado = 'Activo' ORDER BY Nombre, Apellido", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Leer(reader));
            return lista;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener los clientes activos.", ex);
        }
    }

    public static List<Cliente> Buscar(string? texto, string? estado)
    {
        try
        {
            var textoLimpio = texto?.Trim();
            var esId = int.TryParse(textoLimpio, out var idBuscado);

            var sql = new StringBuilder($"SELECT {Columnas} FROM Clientes WHERE 1 = 1");
            if (esId)
            {
                // El usuario escribió solo números: lo más intuitivo es que busque el ID exacto.
                sql.Append(" AND ClienteID = @ClienteIDBuscado");
            }
            else if (!string.IsNullOrWhiteSpace(textoLimpio))
            {
                // Búsqueda por texto: nombre, apellido, nombre completo (nombre + apellido) o teléfono.
                sql.Append(" AND (Nombre LIKE @Texto OR Apellido LIKE @Texto OR (Nombre + ' ' + Apellido) LIKE @Texto OR Telefono LIKE @Texto)");
            }
            if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
                sql.Append(" AND Estado = @Estado");
            sql.Append(" ORDER BY Nombre, Apellido");

            var lista = new List<Cliente>();
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(sql.ToString(), conn);
            if (esId)
                cmd.Parameters.AddWithValue("@ClienteIDBuscado", idBuscado);
            else if (!string.IsNullOrWhiteSpace(textoLimpio))
                cmd.Parameters.AddWithValue("@Texto", $"%{textoLimpio}%");
            if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
                cmd.Parameters.AddWithValue("@Estado", estado);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Leer(reader));
            return lista;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al buscar clientes.", ex);

        }
    }

    public static Cliente? ObtenerPorId(int id)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand($"SELECT {Columnas} FROM Clientes WHERE ClienteID = @ClienteID", conn);
            cmd.Parameters.AddWithValue("@ClienteID", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Leer(reader) : null;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener el cliente.", ex);
        }
    }

    public static int Insertar(Cliente c)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(@"
INSERT INTO Clientes (Nombre, Apellido, Telefono, Email, Direccion, Estado, FechaRegistro)
VALUES (@Nombre, @Apellido, @Telefono, @Email, @Direccion, 'Activo', GETDATE());
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn);
            cmd.Parameters.AddWithValue("@Nombre", c.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", c.Apellido);
            cmd.Parameters.AddWithValue("@Telefono", c.Telefono);
            cmd.Parameters.AddWithValue("@Email", (object?)c.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Direccion", (object?)c.Direccion ?? DBNull.Value);
            return (int)cmd.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al registrar el cliente.", ex);
        }
    }

    public static bool Actualizar(Cliente c)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(@"
UPDATE Clientes SET Nombre = @Nombre, Apellido = @Apellido, Telefono = @Telefono,
    Email = @Email, Direccion = @Direccion
WHERE ClienteID = @ClienteID", conn);
            cmd.Parameters.AddWithValue("@Nombre", c.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", c.Apellido);
            cmd.Parameters.AddWithValue("@Telefono", c.Telefono);
            cmd.Parameters.AddWithValue("@Email", (object?)c.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Direccion", (object?)c.Direccion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ClienteID", c.ClienteID);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al actualizar el cliente.", ex);
        }
    }

    /// <summary>Baja lógica: alterna Activo/Inactivo.</summary>
    public static bool CambiarEstado(int id)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(@"
UPDATE Clientes SET Estado =
    CASE WHEN Estado = 'Activo' THEN 'Inactivo' ELSE 'Activo' END
WHERE ClienteID = @ClienteID", conn);
            cmd.Parameters.AddWithValue("@ClienteID", id);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al cambiar el estado del cliente.", ex);
        }
    }

    private static Cliente Leer(SqlDataReader r) => new()
    {
        ClienteID = r.GetInt32(0),
        Nombre = r.GetString(1),
        Apellido = r.GetString(2),
        Telefono = r.GetString(3),
        Email = r.IsDBNull(4) ? null : r.GetString(4),
        Direccion = r.IsDBNull(5) ? null : r.GetString(5),
        FechaRegistro = r.GetDateTime(6),
        Estado = r.GetString(7)
    };
}