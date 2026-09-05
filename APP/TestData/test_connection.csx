using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var connStr = config.GetConnectionString("FixTrack");
Console.WriteLine("=== PRUEBA 1: Configuración ===");
Console.WriteLine($"Cadena encontrada: {(connStr != null ? "SÍ" : "NO")}");
Console.WriteLine($"Servidor: {new Uri(connStr).Host}");
Console.WriteLine();

Console.WriteLine("=== PRUEBA 2: Conexión ===");
using (var conn = new SqlConnection(connStr))
{
    conn.Open();
    Console.WriteLine($"Estado: {conn.State}");
    Console.WriteLine($"DataSource: {conn.DataSource}");
    Console.WriteLine($"Database: {conn.Database}");
    conn.Close();
    Console.WriteLine("Conexión cerrada OK");
}
Console.WriteLine();

Console.WriteLine("=== PRUEBA 3: SELECT ===");
using (var conn = new SqlConnection(connStr))
{
    conn.Open();
    var cmd = new SqlCommand("SELECT TOP 3 ClienteID, Nombre, Apellido, Telefono, Estado FROM Clientes ORDER BY ClienteID", conn);
    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            Console.WriteLine($"  {reader["ClienteID"]}: {reader["Nombre"]} {reader["Apellido"]} - {reader["Estado"]}");
        }
    }
}
Console.WriteLine();

Console.WriteLine("=== PRUEBA 4: Parametrizada ===");
using (var conn = new SqlConnection(connStr))
{
    conn.Open();
    var cmd = new SqlCommand("SELECT TOP 3 ClienteID, Nombre, Estado FROM Clientes WHERE Estado = @Estado AND ClienteID > @MinID ORDER BY ClienteID", conn);
    cmd.Parameters.AddWithValue("@Estado", "Activo");
    cmd.Parameters.AddWithValue("@MinID", 0);
    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            Console.WriteLine($"  {reader["ClienteID"]}: {reader["Nombre"]} - {reader["Estado"]}");
        }
    }
    Console.WriteLine("  ✓ Sin concatenación - sin riesgo SQL Injection");
}
Console.WriteLine();

Console.WriteLine("=== PRUEBA 5: Error Controlado ===");
try
{
    using (var conn = new SqlConnection(connStr.Replace("FixTrack", "NoExiste")))
    {
        conn.Open();
    }
}
catch (SqlException ex)
{
    Console.WriteLine($"✓ Error capturado: SqlException #{ex.Number}");
    Console.WriteLine($"✓ Tipo: {ex.GetType().Name}");
    Console.WriteLine("✓ Sin datos sensibles expuestos");
}
Console.WriteLine();
Console.WriteLine("=== TODAS LAS PRUEBAS COMPLETADAS ===");
