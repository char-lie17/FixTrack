using System.Text;
using FixTrack.Modelos;
using Microsoft.Data.SqlClient;

namespace FixTrack.Datos;

public static class TecnicoDAL
{
    private const string Columnas =
        "TecnicoID, Nombre, Apellido, Telefono, Especialidad, Estado";

    public static List<Tecnico> ObtenerTodos()
    {
        try
        {
            var lista = new List<Tecnico>();
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand($"SELECT {Columnas} FROM Tecnicos ORDER BY Nombre, Apellido", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Leer(reader));
            return lista;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener la lista de técnicos.", ex);
        }
    }

    public static List<Tecnico> ObtenerActivos()
    {
        try
        {
            var lista = new List<Tecnico>();
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand($"SELECT {Columnas} FROM Tecnicos WHERE Estado = 'Activo' ORDER BY Nombre, Apellido", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Leer(reader));
            return lista;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener los técnicos activos.", ex);
        }
    }

    public static List<Tecnico> Buscar(string? texto, string? estado)
    {
        try
        {
            var textoLimpio = texto?.Trim();
            var esId = int.TryParse(textoLimpio, out var idBuscado);

            var sql = new StringBuilder($"SELECT {Columnas} FROM Tecnicos WHERE 1 = 1");
            if (esId)
                sql.Append(" AND TecnicoID = @TecnicoIDBuscado");
            else if (!string.IsNullOrWhiteSpace(textoLimpio))
                sql.Append(" AND (Nombre LIKE @Texto OR Apellido LIKE @Texto OR (Nombre + ' ' + Apellido) LIKE @Texto OR ISNULL(Especialidad, '') LIKE @Texto)");
            if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
                sql.Append(" AND Estado = @Estado");
            sql.Append(" ORDER BY Nombre, Apellido");

            var lista = new List<Tecnico>();
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(sql.ToString(), conn);
            if (esId)
                cmd.Parameters.AddWithValue("@TecnicoIDBuscado", idBuscado);
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
            throw new ApplicationException("Error al buscar técnicos.", ex);
        }
    }

    public static Tecnico? ObtenerPorId(int id)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand($"SELECT {Columnas} FROM Tecnicos WHERE TecnicoID = @TecnicoID", conn);
            cmd.Parameters.AddWithValue("@TecnicoID", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Leer(reader) : null;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener el técnico.", ex);
        }
    }

    public static int Insertar(Tecnico t)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(@"
INSERT INTO Tecnicos (Nombre, Apellido, Telefono, Especialidad, Estado)
VALUES (@Nombre, @Apellido, @Telefono, @Especialidad, 'Activo');
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn);
            cmd.Parameters.AddWithValue("@Nombre", t.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", t.Apellido);
            cmd.Parameters.AddWithValue("@Telefono", (object?)t.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Especialidad", (object?)t.Especialidad ?? DBNull.Value);
            return (int)cmd.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al registrar el técnico.", ex);
        }
    }

    public static bool Actualizar(Tecnico t)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(@"
UPDATE Tecnicos SET Nombre = @Nombre, Apellido = @Apellido, Telefono = @Telefono,
    Especialidad = @Especialidad
WHERE TecnicoID = @TecnicoID", conn);
            cmd.Parameters.AddWithValue("@Nombre", t.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", t.Apellido);
            cmd.Parameters.AddWithValue("@Telefono", (object?)t.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Especialidad", (object?)t.Especialidad ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TecnicoID", t.TecnicoID);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al actualizar el técnico.", ex);
        }
    }

    public static bool CambiarEstado(int id)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(@"
UPDATE Tecnicos SET Estado =
    CASE WHEN Estado = 'Activo' THEN 'Inactivo' ELSE 'Activo' END
WHERE TecnicoID = @TecnicoID", conn);
            cmd.Parameters.AddWithValue("@TecnicoID", id);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al cambiar el estado del técnico.", ex);
        }
    }

    private static Tecnico Leer(SqlDataReader r) => new()
    {
        TecnicoID = r.GetInt32(0),
        Nombre = r.GetString(1),
        Apellido = r.GetString(2),
        Telefono = r.IsDBNull(3) ? null : r.GetString(3),
        Especialidad = r.IsDBNull(4) ? null : r.GetString(4),
        Estado = r.GetString(5)
    };
}