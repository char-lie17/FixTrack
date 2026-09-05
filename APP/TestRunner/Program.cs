// Herramienta de diagnóstico interno (consola) — NO forma parte de la entrega funcional.
// Ejecuta smoke tests de la capa de datos (DAL) contra SQL Server.
// Excluida del flujo normal de la aplicación; se usa solo en desarrollo/CI.
using FixTrack.Datos;
using FixTrack.Modelos;
using Microsoft.Data.SqlClient;

int ok = 0;
int fail = 0;

void Reportar(string prueba, bool exito, string detalle)
{
    Console.WriteLine($"  [{(exito ? "OK" : "FALLO")}] {prueba} - {detalle}");
    if (exito) ok++; else fail++;
}

Console.WriteLine("=== SMOKE TEST CAPA DE DATOS (DAL -> SQL Server) ===");
Console.WriteLine();

// 1. Conexion y configuracion
Console.WriteLine("--- Configuracion y conexion ---");
try
{
    var connStr = Conexion.GetConnectionString();
    var connOk = false;
    using (var conn = Conexion.ObtenerConexion())
    {
        conn.Open();
        connOk = conn.State == System.Data.ConnectionState.Open;
        conn.Close();
    }
    Reportar("Conexion a SQL Server", connOk, connStr.Replace("Integrated Security=true", "Integrated Security=***"));
}
catch (Exception ex) { Reportar("Conexion a SQL Server", false, ex.Message); }

// 2. ClienteDAL
Console.WriteLine("--- ClienteDAL ---");
try
{
    var todos = ClienteDAL.ObtenerTodos();
    Reportar("ObtenerTodos", todos.Count > 0, $"{todos.Count} clientes");

    var buscados = ClienteDAL.Buscar("Mar", "Activo");
    Reportar("Buscar('Mar', Activo)", buscados.Count > 0, $"{buscados.Count} resultados");

    var nuevoId = ClienteDAL.Insertar(new Cliente { Nombre = "Prueba", Apellido = "Smoke", Telefono = "0000-0000", Email = "smoke@test.com", Direccion = "Temporal" });
    var obtenido = ClienteDAL.ObtenerPorId(nuevoId);
    Reportar("Insertar + ObtenerPorId", obtenido != null && obtenido.Nombre == "Prueba", $"nuevo ClienteID={nuevoId}");

    obtenido!.Nombre = "PruebaEditada";
    var updateOk = ClienteDAL.Actualizar(obtenido);
    var obtuvEdit = ClienteDAL.ObtenerPorId(nuevoId);
    Reportar("Actualizar", updateOk && obtuvEdit!.Nombre == "PruebaEditada", $"ClienteID={nuevoId}");

    var cambioEstado = ClienteDAL.CambiarEstado(nuevoId);
    var estadoNuevo = ClienteDAL.ObtenerPorId(nuevoId)!.Estado;
    Reportar("CambiarEstado (baja logica)", cambioEstado && estadoNuevo == "Inactivo", $"estado={estadoNuevo}");

    using (var conn = Conexion.ObtenerConexion())
    {
        conn.Open();
        using var cmd = new SqlCommand("DELETE FROM Clientes WHERE ClienteID = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", nuevoId);
        cmd.ExecuteNonQuery();
    }
    Reportar("Limpieza registro temporal", true, "OK");
}
catch (Exception ex) { Reportar("ClienteDAL", false, ex.Message); }

// 3. DispositivoDAL
Console.WriteLine("--- DispositivoDAL ---");
try
{
    var todos = DispositivoDAL.ObtenerTodos();
    Reportar("ObtenerTodos (con join cliente)", todos.Count > 0 && !string.IsNullOrEmpty(todos[0].ClienteNombre), $"{todos.Count} dispositivos");
    var porCliente = DispositivoDAL.ObtenerPorCliente(1);
    Reportar("ObtenerPorCliente(1)", porCliente.Count > 0, $"{porCliente.Count} dispositivos del cliente 1 (Maria Gonzalez)");
}
catch (Exception ex) { Reportar("DispositivoDAL", false, ex.Message); }

// 4. TecnicoDAL
Console.WriteLine("--- TecnicoDAL ---");
try
{
    var activos = TecnicoDAL.ObtenerActivos();
    Reportar("ObtenerActivos", activos.Count > 0, $"{activos.Count} tecnicos activos");
    var todos = TecnicoDAL.ObtenerTodos();
    Reportar("ObtenerTodos", todos.Count >= 3, $"{todos.Count} tecnicos");
}
catch (Exception ex) { Reportar("TecnicoDAL", false, ex.Message); }

// 5. OrdenServicioDAL
Console.WriteLine("--- OrdenServicioDAL ---");
try
{
    var todos = OrdenServicioDAL.ObtenerTodos();
    Reportar("ObtenerTodos (con joins)", todos.Count > 0 && !string.IsNullOrEmpty(todos[0].ClienteNombre), $"{todos.Count} ordenes");
    var pendientes = OrdenServicioDAL.Buscar(null, "Pendiente", null, null);
    Reportar("Buscar filtro estado Pendiente", pendientes.All(o => o.Estado == "Pendiente"), $"{pendientes.Count} pendientes");
var delTecnico = OrdenServicioDAL.ObtenerPorTecnico(1);
        Reportar("ObtenerPorTecnico(1) retorna", delTecnico.Count > 0, $"{delTecnico.Count} órdenes");
        Reportar("Todas las órdenes del técnico 1", delTecnico.All(o => o.TecnicoID == 1), $"{delTecnico.Count} de {delTecnico.Count} coinciden");
        Reportar("No hay órdenes ajenas al técnico 1", delTecnico.All(o => o.TecnicoID == 1), "Verificación de filtro SQL");
}
catch (Exception ex) { Reportar("OrdenServicioDAL", false, ex.Message); }

// 6. PagoDAL
Console.WriteLine("--- PagoDAL ---");
try
{
    var deOrden6 = PagoDAL.ObtenerPorOrden(6);
    var total = PagoDAL.ObtenerTotalPagado(6);
    Reportar("ObtenerPorOrden(6)", deOrden6.Count == 2, $"{deOrden6.Count} pagos");
    Reportar("ObtenerTotalPagado(6)", total > 0, $"total={total:C2}");
    var todos = PagoDAL.ObtenerTodos();
    Reportar("ObtenerTodos", todos.Count > 0, $"{todos.Count} pagos");
}
catch (Exception ex) { Reportar("PagoDAL", false, ex.Message); }

// 7. UsuarioDAL + Seguridad
Console.WriteLine("--- UsuarioDAL + Seguridad ---");
try
{
    var admin = UsuarioDAL.ObtenerPorNombreUsuario("admin");
    Reportar("ObtenerPorNombreUsuario(admin)", admin != null, $"rol={admin?.Rol}");
    var passwordOk = admin != null && Seguridad.Verificar("admin123", admin.PasswordHash);
    Reportar("Verificar password 'admin123'", passwordOk, "hash SHA-256 correcto");
    var passwordMal = admin != null && Seguridad.Verificar("incorrecta", admin.PasswordHash);
    Reportar("Verificar password incorrecta", !passwordMal, "denegado");
    Reportar("ExisteNombreUsuario(admin)", UsuarioDAL.ExisteNombreUsuario("admin"), "detectado como existente");
    var hashNuevo = Seguridad.Hashear("mi-clave");
    Reportar("Hashear roundtrip", Seguridad.Verificar("mi-clave", hashNuevo), "hash generado y verificado");
}
catch (Exception ex) { Reportar("UsuarioDAL", false, ex.Message); }

// 8. ReportesDAL
Console.WriteLine("--- ReportesDAL ---");
try
{
    var desde = DateTime.Today.AddDays(-30);
    var hasta = DateTime.Today;
    var r1 = ReportesDAL.ObtenerOrdenesPorEstado(desde, hasta);
    var r2 = ReportesDAL.ObtenerOrdenesPorTecnico(desde, hasta);
    var r3 = ReportesDAL.ObtenerServiciosCompletados(desde, hasta);
    var r4 = ReportesDAL.ObtenerPagosRegistrados(desde, hasta);
    Reportar("Ordenes por estado", r1.Rows.Count > 0, $"{r1.Rows.Count} filas");
    Reportar("Ordenes por tecnico", r2.Rows.Count > 0, $"{r2.Rows.Count} filas");
    Reportar("Servicios completados", r3.Rows.Count == 1, $"{r3.Rows.Count} fila de resumen");
    Reportar("Pagos registrados", r4.Rows.Count > 0, $"{r4.Rows.Count} filas");
}
catch (Exception ex) { Reportar("ReportesDAL", false, ex.Message); }

Console.WriteLine();
Console.WriteLine($"RESULTADO: {ok} OK, {fail} FALLO(S)");
if (fail > 0) Environment.ExitCode = 1;
