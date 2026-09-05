using System.Security.Cryptography;
using System.Text;

namespace FixTrack.Datos;

/// <summary>
/// Utilidades de seguridad para contraseñas.
/// Algoritmo: SHA-256 (hexadecimal). Sin dependencias adicionales.
/// Nota: los datos de prueba del script original usan valores literales
/// "HASH_PRUEBA_...". El script actualizado (BD/FixTrack_BD.sql) almacena
/// hashes SHA-256 reales para credenciales de demostración documentadas.
/// </summary>
public static class Seguridad
{
    public static string Hashear(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? string.Empty));
        return Convert.ToHexString(bytes);
    }

    public static bool Verificar(string password, string? hashAlmacenado)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashAlmacenado))
        {
            return false;
        }
        return string.Equals(Hashear(password), hashAlmacenado, StringComparison.OrdinalIgnoreCase);
    }
}
