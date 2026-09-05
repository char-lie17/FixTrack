using System.Text;
using FixTrack.Modelos;
using Microsoft.Data.SqlClient;

namespace FixTrack.Datos;

public static class UsuarioDAL
{
    private const string SelectConJoin = @"
SELECT u.UsuarioID, u.NombreUsuario, u.PasswordHash, u.Rol, u.Estado, u.TecnicoID,
       t.Nombre + ' ' + t.Apellido AS NombreTecnico
FROM Usuarios u
LEFT JOIN Tecnicos t ON u.TecnicoID = t.TecnicoID";

    public static List<Usuario> ObtenerTodos()
    {
        try
        {
            return Buscar(null, null);
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener la lista de usuarios.", ex);
        }
    }

    public static List<Usuario> Buscar(string? texto, string? estado)
    {
        try
        {
            var textoLimpio = texto?.Trim();
            var esId = int.TryParse(textoLimpio, out var idBuscado);

            var sql = new StringBuilder(SelectConJoin);
            sql.Append(" WHERE 1 = 1");
            if (esId)
                sql.Append(" AND u.UsuarioID = @UsuarioIDBuscado");
            else if (!string.IsNullOrWhiteSpace(textoLimpio))
                sql.Append(" AND (u.NombreUsuario LIKE @Texto OR u.Rol LIKE @Texto)");
            if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
                sql.Append(" AND u.Estado = @Estado");
            sql.Append(" ORDER BY u.NombreUsuario");

            var lista = new List<Usuario>();
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(sql.ToString(), conn);
            if (esId)
                cmd.Parameters.AddWithValue("@UsuarioIDBuscado", idBuscado);
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
            throw new ApplicationException("Error al buscar usuarios.", ex);
        }
    }

    public static Usuario? ObtenerPorId(int id)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(SelectConJoin + " WHERE u.UsuarioID = @UsuarioID", conn);
            cmd.Parameters.AddWithValue("@UsuarioID", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Leer(reader) : null;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener el usuario.", ex);
        }
    }

    /// <summary>Consulta del login: obtiene usuario, hash y rol por nombre de usuario.</summary>
    public static Usuario? ObtenerPorNombreUsuario(string nombreUsuario)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(SelectConJoin + " WHERE u.NombreUsuario = @NombreUsuario", conn);
            cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Leer(reader) : null;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al obtener el usuario por nombre.", ex);
        }
    }

    public static bool ExisteNombreUsuario(string nombreUsuario, int excluirUsuarioId = 0)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(@"
SELECT COUNT(1) FROM Usuarios
WHERE NombreUsuario = @NombreUsuario AND UsuarioID <> @ExcluirId", conn);
            cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
            cmd.Parameters.AddWithValue("@ExcluirId", excluirUsuarioId);
            return (int)cmd.ExecuteScalar() > 0;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al verificar el nombre de usuario.", ex);
        }
    }

    public static int Insertar(Usuario u)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(@"
INSERT INTO Usuarios (NombreUsuario, PasswordHash, Rol, Estado, TecnicoID)
VALUES (@NombreUsuario, @PasswordHash, @Rol, 'Activo', @TecnicoID);
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn);
            cmd.Parameters.AddWithValue("@NombreUsuario", u.NombreUsuario);
            cmd.Parameters.AddWithValue("@PasswordHash", u.PasswordHash);
            cmd.Parameters.AddWithValue("@Rol", u.Rol);
            cmd.Parameters.AddWithValue("@TecnicoID", (object?)u.TecnicoID ?? DBNull.Value);
            return (int)cmd.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al registrar el usuario.", ex);
        }
    }

    public static bool Actualizar(Usuario u)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(@"
UPDATE Usuarios SET NombreUsuario = @NombreUsuario,
    PasswordHash = @PasswordHash, Rol = @Rol, TecnicoID = @TecnicoID
WHERE UsuarioID = @UsuarioID", conn);
            cmd.Parameters.AddWithValue("@NombreUsuario", u.NombreUsuario);
            cmd.Parameters.AddWithValue("@PasswordHash", u.PasswordHash);
            cmd.Parameters.AddWithValue("@Rol", u.Rol);
            cmd.Parameters.AddWithValue("@TecnicoID", (object?)u.TecnicoID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UsuarioID", u.UsuarioID);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al actualizar el usuario.", ex);
        }
    }

    public static bool CambiarEstado(int id)
    {
        try
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var cmd = new SqlCommand(@"
UPDATE Usuarios SET Estado =
    CASE WHEN Estado = 'Activo' THEN 'Inactivo' ELSE 'Activo' END
WHERE UsuarioID = @UsuarioID", conn);
            cmd.Parameters.AddWithValue("@UsuarioID", id);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException("Error al cambiar el estado del usuario.", ex);
        }
    }

    private static Usuario Leer(SqlDataReader r) => new()
    {
        UsuarioID = r.GetInt32(0),
        NombreUsuario = r.GetString(1),
        PasswordHash = r.GetString(2),
        Rol = r.GetString(3),
        Estado = r.GetString(4),
        TecnicoID = r.IsDBNull(5) ? null : r.GetInt32(5),
        NombreTecnico = r.IsDBNull(6) ? string.Empty : r.GetString(6)
    };
}