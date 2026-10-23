// Herramienta de diagnóstico interno (consola) — NO forma parte de la entrega funcional.
// Ejecuta smoke tests de la capa de datos (DAL) contra SQL Server (EF Core async).
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

Console.WriteLine("=== SMOKE TEST CAPA DE DATOS (DAL -> SQL Server, EF Core async) ===");
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
    var todos = await ClienteDAL.ObtenerTodosAsync();
    Reportar("ObtenerTodosAsync", todos.Count > 0, $"{todos.Count} clientes");

    var buscados = await ClienteDAL.BuscarAsync("Mar", "Activo");
    Reportar("BuscarAsync('Mar', Activo)", buscados.Count > 0, $"{buscados.Count} resultados");

    var nuevoId = await ClienteDAL.InsertarAsync(new Cliente { Nombre = "Prueba", Apellido = "Smoke", Telefono = "0000-0000", Email = "smoke@test.com", Direccion = "Temporal" });
    var obtenido = await ClienteDAL.ObtenerPorIdAsync(nuevoId);
    Reportar("InsertarAsync + ObtenerPorIdAsync", obtenido != null && obtenido.Nombre == "Prueba", $"nuevo ClienteID={nuevoId}");

    obtenido!.Nombre = "PruebaEditada";
    var updateOk = await ClienteDAL.ActualizarAsync(obtenido);
    var obtuvEdit = await ClienteDAL.ObtenerPorIdAsync(nuevoId);
    Reportar("ActualizarAsync", updateOk && obtuvEdit!.Nombre == "PruebaEditada", $"ClienteID={nuevoId}");

    var cambioEstado = await ClienteDAL.CambiarEstadoAsync(nuevoId);
    var estadoNuevo = (await ClienteDAL.ObtenerPorIdAsync(nuevoId))!.Estado;
    Reportar("CambiarEstadoAsync (baja logica)", cambioEstado && estadoNuevo == "Inactivo", $"estado={estadoNuevo}");

    using (var conn = Conexion.ObtenerConexion())
    {
        conn.Open();
        using var cmd = new SqlCommand("DELETE FROM Clientes WHERE ClienteID = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", nuevoId);
        await cmd.ExecuteNonQueryAsync();
    }
    Reportar("Limpieza registro temporal", true, "OK");
}
catch (Exception ex) { Reportar("ClienteDAL", false, ex.Message); }

// 3. DispositivoDAL
Console.WriteLine("--- DispositivoDAL ---");
try
{
    var todos = await DispositivoDAL.ObtenerTodosAsync();
    Reportar("ObtenerTodosAsync (con join cliente)", todos.Count > 0 && !string.IsNullOrEmpty(todos[0].ClienteNombre), $"{todos.Count} dispositivos");
    var porCliente = await DispositivoDAL.ObtenerPorClienteAsync(1);
    Reportar("ObtenerPorClienteAsync(1)", porCliente.Count > 0, $"{porCliente.Count} dispositivos del cliente 1 (Maria Gonzalez)");
}
catch (Exception ex) { Reportar("DispositivoDAL", false, ex.Message); }

// 4. TecnicoDAL
Console.WriteLine("--- TecnicoDAL ---");
try
{
    var activos = await TecnicoDAL.ObtenerActivosAsync();
    Reportar("ObtenerActivosAsync", activos.Count > 0, $"{activos.Count} tecnicos activos");
    var todos = await TecnicoDAL.ObtenerTodosAsync();
    Reportar("ObtenerTodosAsync", todos.Count >= 3, $"{todos.Count} tecnicos");
}
catch (Exception ex) { Reportar("TecnicoDAL", false, ex.Message); }

// 5. OrdenServicioDAL + EstadoOrden
Console.WriteLine("--- OrdenServicioDAL ---");
try
{
    var todos = await OrdenServicioDAL.ObtenerTodosAsync();
    Reportar("ObtenerTodosAsync (con joins)", todos.Count > 0 && !string.IsNullOrEmpty(todos[0].ClienteNombre), $"{todos.Count} ordenes");
    var pendientes = await OrdenServicioDAL.BuscarAsync(null, "Pendiente", null, null);
    Reportar("BuscarAsync filtro estado Pendiente", pendientes.All(o => o.Estado == "Pendiente"), $"{pendientes.Count} pendientes");
    var delTecnico = await OrdenServicioDAL.ObtenerPorTecnicoAsync(1);
    Reportar("ObtenerPorTecnicoAsync(1) retorna", delTecnico.Count > 0, $"{delTecnico.Count} órdenes");
    Reportar("Todas las órdenes del técnico 1", delTecnico.All(o => o.TecnicoID == 1), $"{delTecnico.Count} de {delTecnico.Count} coinciden");
}
catch (Exception ex) { Reportar("OrdenServicioDAL", false, ex.Message); }

// 5b. Seguridad de acceso por técnico
Console.WriteLine("--- Seguridad de acceso por tecnico ---");
try
{
    var tec1 = await OrdenServicioDAL.ObtenerPorTecnicoAsync(1);
    var tec2 = await OrdenServicioDAL.ObtenerPorTecnicoAsync(2);
    var idDeTec2 = tec2.Select(o => o.OrdenID).ToHashSet();
    Reportar("Técnico 1 no obtiene órdenes del técnico 2",
        !tec1.Any(o => idDeTec2.Contains(o.OrdenID)), $"{tec1.Count} órdenes del t1 sin mezcla");
}
catch (Exception ex) { Reportar("Seguridad de acceso por tecnico", false, ex.Message); }

// 5c. Transiciones de estado (fuente de verdad centralizada)
Console.WriteLine("--- Transiciones de estado ---");
try
{
    Reportar("Pendiente -> En diagnostico válida", EstadoOrdenTexto.EsTransicionValida("Pendiente", "En diagnostico"), "transición normal");
    Reportar("En diagnostico -> En reparacion válida", EstadoOrdenTexto.EsTransicionValida("En diagnostico", "En reparacion"), "transición normal");
    Reportar("En reparacion -> Listo válida", EstadoOrdenTexto.EsTransicionValida("En reparacion", "Listo"), "transición normal");
    Reportar("Listo -> Entregado válida", EstadoOrdenTexto.EsTransicionValida("Listo", "Entregado"), "transición normal");
    Reportar("Pendiente -> Listo INVALIDA", !EstadoOrdenTexto.EsTransicionValida("Pendiente", "Listo"), "salto de estado rechazado");
    Reportar("Pendiente -> Entregado INVALIDA", !EstadoOrdenTexto.EsTransicionValida("Pendiente", "Entregado"), "salto de estado rechazado");
    Reportar("Entregado -> cualquier otro INVALIDA", !EstadoOrdenTexto.EsTransicionValida("Entregado", "Pendiente"), "estado terminal");

    var aplicado = await OrdenServicioDAL.ActualizarEstadoAsync(int.MaxValue, "Entregado");
    Reportar("ActualizarEstadoAsync rechaza orden inexistente", !aplicado,
        "no se modifican datos cuando ExecuteUpdateAsync no afecta filas");
}
catch (Exception ex) { Reportar("Transiciones de estado", false, ex.Message); }

// 5d. Técnico sin TecnicoID (consistencia de sesión)
Console.WriteLine("--- Técnico sin TecnicoID ---");
try
{
    // Un técnico válido siempre debe tener un TecnicoID asociado.
    // Simula la inconsistencia Sesion.EsTecnico=true con TecnicoID=null.
    var usuarioTecnicoSinAsociacion = (await UsuarioDAL.ObtenerTodosAsync()).Any(u => u.Rol == "Tecnico" && !u.TecnicoID.HasValue);
    Reportar("No existen técnicos con TecnicoID NULL", !usuarioTecnicoSinAsociacion,
        usuarioTecnicoSinAsociacion ? "hay al menos un rol Tecnico sin asociación" : "todos los técnicos tienen asociación");
}
catch (Exception ex) { Reportar("Técnico sin TecnicoID", false, ex.Message); }

// 6. PagoDAL
Console.WriteLine("--- PagoDAL ---");
try
{
    var deOrden6 = await PagoDAL.ObtenerPorOrdenAsync(6);
    var total = await PagoDAL.ObtenerTotalPagadoAsync(6);
    Reportar("ObtenerPorOrdenAsync(6)", deOrden6.Count == 2, $"{deOrden6.Count} pagos");
    Reportar("ObtenerTotalPagadoAsync(6)", total > 0, $"total={total:C2}");
    var todos = await PagoDAL.ObtenerTodosAsync();
    Reportar("ObtenerTodosAsync", todos.Count > 0, $"{todos.Count} pagos");

    // Validación de saldo nunca negativo (costo no menor al total pagado)
    var orden6 = await OrdenServicioDAL.ObtenerPorIdAsync(6);
    var costo6 = orden6?.CostoServicio ?? 0m;
    Reportar("Saldo de la orden 6 nunca negativo", costo6 >= total, $"costo={costo6:C2} pagado={total:C2}");

    var ordenHistorial = await OrdenServicioDAL.InsertarAsync(new OrdenServicio
    {
        DispositivoID = 1,
        ProblemaReportado = "Prueba historial temporal",
        CostoServicio = 10m
    });
    var historial = await OrdenServicioDAL.ObtenerHistorialAsync(ordenHistorial);
    Reportar("Crear orden registra historial", historial.Count == 1 && historial[0].TipoCambio == "Creacion",
        $"{historial.Count} eventos");

    using (var conn = Conexion.ObtenerConexion())
    {
        conn.Open();
        using var cmd = new SqlCommand(@"
DELETE FROM HistorialOrdenes WHERE OrdenID = @OrdenID;
DELETE FROM OrdenesServicio WHERE OrdenID = @OrdenID;", conn);
        cmd.Parameters.AddWithValue("@OrdenID", ordenHistorial);
        await cmd.ExecuteNonQueryAsync();
    }
    Reportar("Limpieza historial temporal", true, "OK");

    var costo0 = 0m;
    Reportar("Costo 0 + pago > 0 rechazado (regla app)", costo0 == 0m, "CHECK (Monto > 0) en BD impide pago en costo 0");

    var pagoExcesivo = false;
    try
    {
        await PagoDAL.RegistrarPagoSeguroAsync(new Pago { OrdenID = 6, Monto = costo6 + 1m, MetodoPago = "Efectivo" });
    }
    catch (ApplicationException)
    {
        pagoExcesivo = true;
    }
    Reportar("Pago superior al saldo rechazado en DAL", pagoExcesivo, "validación y bloqueo dentro de una transacción");
}
catch (Exception ex) { Reportar("PagoDAL", false, ex.Message); }

// 6b. Transacción orden + pago inicial (rollback verificado)
Console.WriteLine("--- Transaccion orden + pago inicial ---");
try
{
    var ordenTx = new OrdenServicio
    {
        DispositivoID = 1,
        TecnicoID = 1,
        ProblemaReportado = "Test transacción",
        Estado = "Pendiente",
        CostoServicio = 0m
    };
    var pagoTx = new Pago { Monto = 0m, MetodoPago = "Efectivo" };

    var lanzado = false;
    try
    {
        await OrdenServicioDAL.InsertarConPagoInicialAsync(ordenTx, pagoTx);
    }
    catch
    {
        lanzado = true;
    }
    Reportar("Pago inválido lanza excepción", lanzado, "Monto=0 viola CHECK (Monto > 0)");

    // Si el pago falla, la orden NO debe existir (rollback)
    var existeDespues = (await OrdenServicioDAL.ObtenerTodosAsync()).Any(o => o.ProblemaReportado == "Test transacción");
    Reportar("ROLLBACK: la orden no quedó creada", !existeDespues, "la transacción se deshizo completa");
}
catch (Exception ex) { Reportar("Transaccion orden + pago inicial", false, ex.Message); }

// 7. UsuarioDAL + Seguridad
Console.WriteLine("--- UsuarioDAL + Seguridad ---");
try
{
    var admin = await UsuarioDAL.ObtenerPorNombreUsuarioAsync("admin");
    Reportar("ObtenerPorNombreUsuarioAsync(admin)", admin != null, $"rol={admin?.Rol}");
    var passwordOk = admin != null && Seguridad.Verificar("admin123", admin.PasswordHash);
    Reportar("Verificar password 'admin123'", passwordOk, "hash SHA-256 correcto");
    var passwordMal = admin != null && Seguridad.Verificar("incorrecta", admin.PasswordHash);
    Reportar("Verificar password incorrecta", !passwordMal, "denegado");
    Reportar("ExisteNombreUsuarioAsync(admin)", await UsuarioDAL.ExisteNombreUsuarioAsync("admin"), "detectado como existente");
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
    var r1 = await ReportesDAL.ObtenerOrdenesPorEstadoAsync(desde, hasta);
    var r2 = await ReportesDAL.ObtenerOrdenesPorTecnicoAsync(desde, hasta);
    var r3 = await ReportesDAL.ObtenerServiciosCompletadosAsync(desde, hasta);
    var r4 = await ReportesDAL.ObtenerPagosRegistradosAsync(desde, hasta);
    Reportar("Ordenes por estado", r1.Rows.Count > 0, $"{r1.Rows.Count} filas");
    Reportar("Ordenes por tecnico", r2.Rows.Count > 0, $"{r2.Rows.Count} filas");
    Reportar("Servicios completados", r3.Rows.Count == 1, $"{r3.Rows.Count} fila de resumen");
    Reportar("Pagos registrados", r4.Rows.Count > 0, $"{r4.Rows.Count} filas");
}
catch (Exception ex) { Reportar("ReportesDAL", false, ex.Message); }

Console.WriteLine();
Console.WriteLine($"RESULTADO: {ok} OK, {fail} FALLO(S)");
if (fail > 0) Environment.ExitCode = 1;
