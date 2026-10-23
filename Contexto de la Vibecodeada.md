# Contexto de la Vibecodeada — FixTrack (Concurrencia, Asincronía y EF Core)

Registro técnico de la intervención sobre el proyecto FixTrack.
Este archivo se crea al iniciar la implementación y se completa con los cambios reales al finalizar.

---

## 1. Estado inicial del proyecto y arquitectura encontrada

- **Aplicación:** WinForms (.NET 10, `net10.0-windows`), C# con nullable habilitado.
- **Persistencia:** ADO.NET puro (`Microsoft.Data.SqlClient` 6.1.1) con clases DAL estáticas en `APP/Datos/`.
- **Configuración:** `appsettings.json` con cadena de conexión leída por `Conexion.cs` (Microsoft.Extensions.Configuration.Json 8.0.0).
- **Modelos:** POCOs simples en `APP/Modelos/` (Cliente, Dispositivo, Tecnico, OrdenServicio, Pago, Usuario, HistorialOrden) con propiedades de visualización calculadas.
- **Formularios:** 19 formularios en `APP/Formularios/` construidos íntegramente en código (sin .Designer.cs con controles visuales propios; solo .resx vacíos o casi vacíos).
- **Base de datos:** SQL Server, esquema definido en `BD/FixTrack_BD.sql` (7 tablas, 7 FKs, CHECK constraints, índice único filtrado `UQ_Usuarios_TecnicoID`, procedimiento almacenado `sp_ReporteOrdenesPorEstado`, datos de prueba).
- **Seguridad:** Hash SHA-256 en `Seguridad.cs`; permisos por rol (`Sesion`) en UI; consultas parametrizadas.
- **Concurrencia existente (ADÓ):** `UPDLOCK, HOLDLOCK` en pagos y cambio de estado; transacciones explícitas en Insertar, ActualizarDetalle, ActualizarEstado, RegistrarPagoSeguro, InsertarConPagoInicial.
- **Proyecto auxiliar:** `APP/TestRunner/` (consola de smoke tests, excluida del .csproj principal).
- **Rama por defecto del remoto:** `master` (no existe `main`).
- **Nota de Rules.md:** §1 prohíbe EF Core "sin autorización explícita". La consigna académica de esta rama autoriza explícitamente la migración a EF Core; se documenta aquí como excepción autorizada para la Unidad III.

## 2. Rama Git utilizada

- Repositorio clonado de `https://github.com/char-lie17/FixTrack.git`.
- La consigna pide partir de `origin/main`, pero el remoto solo tiene `master` (HEAD -> origin/master). Se parte de `origin/master` tras `git fetch origin`.
- El nombre exacto pedido `Concurrencia Y EF Core` **no es válido en Git** (los nombres de rama no admiten espacios). Se creó la rama equivalente más próxima: **`Concurrencia-Y-EF-Core`**, basada en `origin/master`.
- `main` no existe en el remoto y no fue modificada; no se hicieron commits, merges ni pushes hacia `master`.

## 3. Diagnóstico previo (Etapa A)

### Operaciones de datos identificadas
- CRUD completo: Clientes, Dispositivos, Técnicos, Usuarios.
- Órdenes: listado con filtros (texto/estado/fechas/técnico), detalle, inserción (opcionalmente con pago inicial transaccional), edición de detalle con historial por campo, cambio de estado con validación de transición e historial.
- Pagos: búsqueda con filtros, inserción validada contra saldo con bloqueo de orden, total pagado.
- Reportes: 4 consultas (una vía SP) que devuelven `DataTable`.
- Dashboard: conteo por estado (general o por técnico) y 10 órdenes recientes.
- Login: consulta por nombre de usuario + verificación de hash.

### Eventos WinForms que ejecutan consultas de forma síncrona (bloquean el hilo de interfaz)
- `FrmLogin.BtnIngresar_Click`
- `FrmDashboard.CargarInicio` (métricas + recientes)
- `FrmClientes.CargarDatos`, `ClienteSeleccionado`, `BtnEstado_Click`
- `FrmClienteFormulario.BtnGuardar_Click`, `FrmClienteDetalle` (constructor/BuildUi)
- `FrmDispositivos.CargarDatos`, `BtnEditar_Click`
- `FrmDispositivoFormulario.BuildUi` (combo clientes) y `BtnGuardar_Click`
- `FrmOrdenes.CargarDatos`
- `FrmOrdenNueva.BuildUi` (combos) y `BtnGuardar_Click`
- `FrmOrdenDetalle` (constructor autorización + `Cargar`), `CboEstado_SelectionChange`, `BtnGuardar_Click`
- `FrmPagos.CargarDatos`, `BtnDetalle_Click`
- `FrmPagoFormulario.BuildUi` (combo órdenes) y `BtnGuardar_Click`
- `FrmPagoDetalle.Cargar`
- `FrmTecnicos` / `FrmUsuarios` (`CargarDatos`, selección, `BtnEstado_Click`)
- `FrmTecnicoFormulario` / `FrmUsuarioFormulario` (`BtnGuardar_Click`, `CargarTecnicos`)
- `FrmReportes.BtnGenerar_Click`
- Búsquedas en `TextChanged` disparan recargas síncronas por cada tecla (sin cancelación de la consulta anterior).

### Riesgos de concurrencia detectados (existentes)
- Dos usuarios registrando pagos simultáneos sobre la misma orden: ya mitigado con `UPDLOCK, HOLDLOCK` + transacción.
- Cambio de estado simultáneo: ya mitigado con bloqueo + `WHERE Estado = @EstadoActual`.
- Búsquedas rápidas en la UI: sin cancelación; la última respuesta puede pisar una más reciente (se corregirá con `CancellationToken`).
- Ediciones de Clientes/Dispositivos/Técnicos: "última escritura gana" (sin token de concurrencia); se documenta, no se agrega `rowversion` por no requerirlo la funcionalidad actual.

### Limitaciones del entorno de trabajo
- No hay SQL Server escuchando en localhost:1433 en esta máquina: no será posible ejecutar la aplicación contra una BD real ni correr el TestRunner end-to-end. Se documentará como verificación pendiente.

---

## 4. Etapa B — EF Core Code First (capa de datos)

### Paquetes NuGet (agregados en `APP/FixTrack.csproj`)
- `Microsoft.EntityFrameworkCore` 10.0.13 (requerido)
- `Microsoft.EntityFrameworkCore.SqlServer` 10.0.13 (requerido)
- `Microsoft.EntityFrameworkCore.Design` 10.0.13 (solo herramientas de migración)
- `Microsoft.Data.SqlClient` actualizado de 6.1.1 a **6.1.7** (6.1.1 no cumple la dependencia mínima de EF 10 y rompía el restore con NU1605)

### Nuevos archivos
- **`APP/Datos/FixTrackDbContext.cs`** — `DbContext` con las 7 entidades:
  - Claves primarias con `ValueGeneratedOnAdd` (IDENTITY 1,1).
  - Longitudes, precisión `decimal(10,2)` y `varchar(20/30)` donde el esquema original los usa.
  - Defaults: `GETDATE()` (fechas), `'Activo'`, `'Pendiente'`, `0` (costo).
  - CHECK constraints (vía `ToTable(..., t => t.HasCheckConstraint(...))`, forma no obsoleta en EF 10): Estado de Clientes/Tecnicos/Usuarios, Rol, Estado/Costo de Ordenes, Monto/MetodoPago de Pagos.
  - Índice único `UQ_Usuarios_NombreUsuario`; índice único filtrado `UQ_Usuarios_TecnicoID` (`TecnicoID IS NOT NULL`); índice compuesto `IX_HistorialOrdenes_OrdenID_FechaCambio` con `FechaCambio` descendente.
  - FKs `HasOne/WithMany` con comportamiento `Restrict` (igual que el script original).
  - `Ignore()` de propiedades de visualización calculadas (no persistibles).
  - Sin lazy loading; lecturas con `AsNoTracking()`.
- **`APP/Datos/FixTrackDbContextFactory.cs`** — doble rol:
  - `IDbContextFactory<FixTrackDbContext>` + punto de acceso estático `FixTrackDb` (`Inicializar()` desde `Program.Main`, `Crear()` devuelve un contexto por operación, nunca compartido entre tareas).
  - `IDesignTimeDbContextFactory<FixTrackDbContext>` con constructor público sin parámetros (requisito de `dotnet ef`) y cadena dummy; solo se usa para generar migraciones.
- **`APP/Migrations/`** — migración generada: `20261023004419_Inicial.cs` (+ Designer + snapshot). Crea las 7 tablas con todos los constraints, FKs e índices descritos arriba. Verificado manualmente contra `BD/FixTrack_BD.sql`.

### Cambios en modelos (`APP/Modelos/`)
- Se añadieron solo las navegaciones mínimas necesarias para las consultas LINQ (FKs): `Cliente.Dispositivos`, `Dispositivo.Cliente`, `Tecnico.Ordenes`, `OrdenServicio.Dispositivo/Tecnico`, `Pago.Orden`, `Usuario.Tecnico`, `HistorialOrden.Orden/Usuario`. No se alteraron reglas ni propiedades de negocio.

### Migración de los 7 DAL (estáticos → `*Async` con `CancellationToken`)
- Todas las clases DAL conservan su firma estática (arquitectura intacta) pero ahora exponen `*Async(CancellationToken ct = default)` sobre EF Core, con `await using var db = FixTrackDb.Crear()`.
- `ClienteDAL`, `DispositivoDAL`, `TecnicoDAL`, `UsuarioDAL`, `OrdenServicioDAL`, `PagoDAL`, `ReportesDAL` migrados.
- **`ReportesDAL`** conserva salida `DataTable`: el SP se ejecuta con `SqlQuery<FilaEstado>`; los otros 3 reportes usan LINQ y se materializan a `DataTable`.
- **Concurrencia en pagos** (`PagoDAL.RegistrarPagoSeguroAsync`): `BeginTransactionAsync(IsolationLevel.Serializable)` + `ExecuteSqlAsync("SELECT 1 FROM OrdenesServicio WITH (UPDLOCK, HOLDLOCK) ...")` — conserva el bloqueo original.
- **Cambio de estado** (`OrdenServicioDAL.ActualizarEstadoAsync`): `ExecuteUpdateAsync` con cláusula `WHERE Estado = <estado leído>`; 0 filas afectadas ⇒ conflicto optimista (sin rowversion; documentado).
- **Inserción transaccional** (`InsertarConPagoInicialAsync`): orden + pago en una transacción con rollback verificado (smoke test del TestRunner).
- `UsuarioDAL.InsertarAsync` captura `SqlException` 2601/2627 (unicidad) para mantener los mensajes de negocio.

## 5. Etapa C/D — Concurrencia y asincronía en la UI

- **`UIHelper`**: añadidos `EjecutarSeguroAsync(Form, Func<Task>, ...)` y `EjecutarSeguroAsync<T>(...) where T : class`, que distinguen `OperationCanceledException` (cancelación normal, sin diálogo de error) de errores reales.
- **`FrmLogin`**: `BtnIngresar_Click` → `async void`, botón deshabilitado durante el acceso + `finally`.
- **`FrmDashboard`**: carga en 2 etapas con `IProgress<string>` (fila extra `lblEstadoCarga` en el layout); métricas y recientes se pintan por etapa; etiqueta de estado/éxito/error.
- **Listados con búsqueda** (`FrmClientes`, `FrmDispositivos`, `FrmOrdenes`, `FrmPagos`, `FrmTecnicos`, `FrmUsuarios`): `CancellationTokenSource` por formulario; cada `TextChanged` cancela la búsqueda anterior, así una respuesta vieja nunca pisa a la más reciente; `FormClosed` cancela todo; `UseWaitCursor` durante la consulta.
- **`FrmReportes`**: `CancellationTokenSource` para cancelar la generación anterior; botón deshabilitado durante la consulta.
- **Formularios de edición** (`FrmClienteFormulario`, `FrmDispositivoFormulario`, `FrmTecnicoFormulario`, `FrmUsuarioFormulario`, `FrmPagoFormulario`, `FrmOrdenNueva`): `async void BtnGuardar_Click` con botón Guardar deshabilitado + `finally` (anti doble clic).
- **Combos perezosos**: los combos que se cargaban en `BuildUi` (dispositivos/técnicos de `FrmOrdenNueva`, clientes de `FrmDispositivoFormulario`, órdenes de `FrmPagoFormulario`, técnicos de `FrmUsuarioFormulario`) ahora se cargan en `async void` (`Cargar*Async`) sin bloquear la apertura del diálogo; la selección inicial se difiere al terminar la carga.
- **Detalle asíncrono al mostrarse**: `FrmClienteDetalle`, `FrmPagoDetalle` y `FrmOrdenDetalle` (verificación de acceso del técnico + carga) usan el evento `Shown` con `async` para no bloquear el constructor.
- **`FrmOrdenDetalle`**: `CargarAsync` con cancelación de recargas; cambio de estado y guardado son `async`, y la detección de conflicto optimista muestra diálogo y recarga si la DAL reporta 0 filas.
- **Sin cambios en**: `.Designer.cs` (no existen con visuales), `Seguridad.cs`, permisos por rol, `Rules.md`, `Entregables/`, `Contexto/`.

## 6. Etapa E — TestRunner y verificación

- `APP/TestRunner/Program.cs` migrado a los métodos `*Async` con `await` (top-level statements con `async` implícito).
- **Verificación ejecutada:**
  - `dotnet build APP/FixTrack.sln` → **0 errores** (12 warnings CS8602 preexistentes por nullable en las DAL).
  - `dotnet build APP/TestRunner/TestRunner.csproj` → **0 errores** (warnings NU1902/NU1903 de un paquete transitorio, preexistentes).
  - `dotnet tool install --global dotnet-ef` (10.0.13) + `dotnet ef migrations add Inicial` → migración generada y compilada.
- **Limitación del entorno (verificación pendiente):** no hay SQL Server en localhost:1433; por lo tanto no se pudo (a) ejecutar la aplicación contra una BD real, (b) aplicar la migración a una base existente, (c) correr el TestRunner end-to-end. Queda documentado como verificación pendiente en entorno con BD.

## 7. Desviaciones y decisiones registradas

1. **Nombre de rama:** la consigna pedía `Concurrencia Y EF Core`; Git no admite espacios. Se usó **`Concurrencia-Y-EF-Core`**.
2. **Rama base:** `origin/master` (el remoto no tiene `main`).
3. **EF Core:** autorizado explícitamente por la consigna académica pese a `Rules.md` §1.
4. **Estrategia para BD existente:** la migración es "de cero" (Cero/Inicial). Para una base ya poblada con el script `FixTrack_BD.sql`, aplicar solo la fila en `__EFMigrationsHistory` con el `ProductVersion` del snapshot (documentación oficial de EF Core). **Nunca** usar `EnsureDeleted()` ni aplicar migraciones automáticamente al inicio de la app (`Database.Migrate()` no se invoca en `Program.Main`).
5. **Concurrencia de Clientes/Dispositivos/Técnicos:** se mantiene "última escritura gana"; no se agrega `rowversion` por no requerirlo la funcionalidad actual (se documenta).
6. **`Conexion.cs` y `ObtenerConexion()`** se conservan: la cadena de conexión sigue viniendo de `appsettings.json` (la usa `FixTrackDb.Inicializar()`) y `FrmTestConexion` sigue funcionando como diagnóstico ADO.

---

*(Documento completado al finalizar la implementación de las etapas B a E.)*
