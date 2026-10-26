# Informe del Entregable — Concurrencia + EF Core + Reportes

**Proyecto:** FixTrack — Sistema de Gestión de Reparaciones de Dispositivos Electrónicos
**Asignatura:** Programación de Aplicaciones de Escritorio
**Fecha:** 24 de octubre de 2026

---

## 1. Objetivo del entregable

Demostrar en FixTrack:

1. **Concurrencia y asincronía** en WinForms (tareas en segundo plano, GUI responsiva,
   manejo de excepciones y cancelación) integradas en funcionalidades reales.
2. **EF Core (Code First)** como complemento/reemplazo de ADO.NET, con `DbContext`, entidades,
   relaciones, consultas LINQ y una **migración**.
3. **Reportes dinámicos** generados a partir de datos reales de la base de datos.
4. **Conservación** de las funcionalidades previas y de la arquitectura en capas.

---

## 2. Concurrencia y asincronía (WinForms)

### 2.1 Migración de la capa de datos a operaciones asíncronas

Los **7 DAL** (`ClienteDAL`, `DispositivoDAL`, `OrdenServicioDAL`, `PagoDAL`, `TecnicoDAL`,
`UsuarioDAL`, `ReportesDAL`) pasaron de ADO.NET síncrono a **EF Core con métodos `async`** con
firma `*Async(CancellationToken ct = default)`, de modo que ninguna operación de base de datos
bloquea el hilo de UI.

### 2.2 Formularios responsivos (19 formularios convertidos)

- **Listados** (`FrmClientes`, `FrmDispositivos`, `FrmOrdenes`, `FrmPagos`, `FrmTecnicos`,
  `FrmUsuarios`, `FrmReportes`): carga asíncrona con `CancellationTokenSource`; la cancelación se
  dispara en `FormClosed` para evitar fugas y cargas tardías.
- **Guardados** (`Frm*Formulario`, `FrmOrdenNueva`): el botón se deshabilita durante la
  operación, se usa `try/finally` para reactivarlo, y los errores se muestran sin tumbar la app.
- **Cargas perezosas**: combos de clientes/dispositivos/técnicos con `async`, detalles abiertos
  en el evento `Shown`.
- **Dashboard** (`FrmDashboard`): carga métricas en segundo plano notificando con
  `IProgress<string>`, con etiqueta de estado «Actualizando…».
- **Detalle de orden** (`FrmOrdenDetalle`): guarda con detección de **conflicto optimista**
  (fila afectada = 0) capturando el resultado booleano dentro del lambda de `EjecutarSeguroAsync`.

### 2.3 Manejo de excepciones y cancelación

- `UIHelper.EjecutarSeguroAsync` (y su versión genérica `where T : class`): ejecuta la
  operación, captura excepciones, muestra el mensaje amigable y **distingue
  `OperationCanceledException`** (cancelación limpia, no es error).
- Estado del botón siempre restaurado en `finally`.
- Ninguna excepción queda sin capturar en el camino de la UI.

### 2.4 Integración en funcionalidades reales

La concurrencia no está en código de prueba: está en el alta/edición de clientes, dispositivos,
órdenes, pagos, técnicos y usuarios, en la búsqueda y filtrado de los 7 listados, en el dashboard
y en los reportes.

---

## 3. EF Core (Code First)

| Elemento | Detalle |
|---|---|
| Paquete | Entity Framework Core **10.0.13** (SQL Server provider) |
| `DbContext` | `APP/Datos/FixTrackDbContext.cs` — 7 `DbSet`, configuración de PK/FK, CHECKs, DEFAULTs, índices (incluido el único **filtrado** `UQ_Usuarios_TecnicoID` y `IX_HistorialOrdenes_OrdenID_FechaCambio DESC`) |
| Relaciones | `Cliente → Dispositivos → OrdenesServicio → Pagos/HistorialOrdenes`; `Tecnico → OrdenesServicio`; `Tecnico ↔ Usuario` (1:0..1); comportamiento **Restrict** en borrados |
| Fábrica | `FixTrackDbContextFactory` (sin parámetros) para `dotnet ef` |
| Migración | `APP/Migrations/20261023004419_Inicial.cs` (+ Designer + Snapshot) — generada y **aplicada** |
| Inicialización | `FixTrackDb.Inicializar()` en `Program.Main` y en `TestRunner` |
| Consultas LINQ | Búsquedas, filtros, joins y agregaciones en todos los DAL y en `ReportesDAL` |

Coexistencia: se conservó `Conexion.cs`/`Seguridad.cs` (ADO.NET) para la infraestructura
puntual, cumpliendo el requisito de *complementar* sin romper lo previo.

---

## 4. Reportes dinámicos con datos reales

- Módulo **Reportes** conectado a `ReportesDAL` (LINQ + `sp_ReporteOrdenesPorEstado`).
- 4 reportes con filtros de fecha y exportación a CSV.
- Verificado con datos reales de la BD: «Órdenes por estado» devuelve las 5 filas de la
  distribución real (ver captura 3).

---

## 5. Evidencias de funcionamiento

*(Capturas tomadas sobre la aplicación ejecutándose contra la BD real `FixTrack` con datos de
prueba; archivos en la carpeta `Evidencias/`)*

| # | Archivo | Qué muestra |
|---|---|---|
| 1 | `01_login.png` | Login de la aplicación (autenticación asíncrona) |
| 2 | `02_dashboard.png` | Dashboard con métricas reales cargadas en segundo plano |
| 3 | `03_reportes.png` | Reporte dinámico «Órdenes por estado» con datos reales |
| 4 | `04_ordenes.png` | Listado de órdenes con estados y filtros (carga async) |
| 5 | `05_clientes.png` | Listado de clientes con búsqueda (carga async) |
| 6 | `06_ejecucion_TestRunner.txt` | Salida completa de las pruebas: **44 OK, 0 FALLO(S)** |
| 7 | `07_migraciones_EFCore.txt` | Listado de migraciones reconocidas por `dotnet ef` |
| 8 | `08_estado_BD_sqlcmd.txt` | Migración aplicada en `__EFMigrationsHistory` + conteos reales |

**Pruebas automatizadas:** `APP/TestRunner` ejecuta 44 verificaciones de extremo a extremo
(conexión, CRUD, estados, transiciones, pagos, historial y reportes) sobre la BD real:
`RESULTADO: 44 OK, 0 FALLO(S)`.

**Compilación:** `dotnet build FixTrack.sln` → 0 errores.

---

## 6. Base de datos (script e instrucciones)

Dos caminos documentados en `BD/INSTRUCCIONES_RECONSTRUCCION.md`:

- **Opción A (recomendada, Code First):** `dotnet ef database update` + `sqlcmd -i BD\Seed_Datos_Prueba.sql`.
- **Opción B (script completo):** `sqlcmd -i BD\FixTrack_BD.sql` — *advertencia: contiene
  `DROP DATABASE` y recrea la BD completa*.

Credenciales: `admin/admin123`, `recepcion1/recepcion123`, `luis.ortega|karla.vega|diego.salinas/tecnico123`.

---

## 7. Participación de los integrantes

| # | Integrante | Participación (funcionalidades / evidencias) |
|---|---|---|
| 1 | Henry David Lopez Sanchez | **Concurrencia y asincronía en WinForms:** conversión de los 19 formularios a `async/await`, `CancellationTokenSource` en los 7 listados (cancelación en `FormClosed`), guardados con botón deshabilitado y `finally`, y `UIHelper.EjecutarSeguroAsync` con distinción de `OperationCanceledException`. |
| 2 | Juan Pablo Sirias Espinoza | **EF Core (Code First):** `FixTrackDbContext` (7 `DbSet`, relaciones, CHECKs, DEFAULTs e índices), entidad/fábrica, generación y aplicación de la migración `20261023004419_Inicial`, `FixTrackDb.Inicializar()` y los scripts `Seed_Datos_Prueba.sql` e `INSTRUCCIONES_RECONSTRUCCION.md`. |
| 3 | Cris Angel Lumbi Rodriguez | **Capa de datos asíncrona:** migración de los 7 DAL a `*Async(CancellationToken ct = default)` con consultas LINQ; carga en segundo plano del Dashboard con `IProgress<string>` y detección de conflicto optimista en `FrmOrdenDetalle`. |
| 4 | Gabriel Eduardo Segovia Fonseca | **Reportes dinámicos:** `ReportesDAL` (LINQ + `sp_ReporteOrdenesPorEstado`), 4 reportes con filtros de fecha y exportación a CSV; verificación con datos reales y capturas de las evidencias (`Evidencias/01…05`). |
| 5 | Carlos Renato Castillo Lopez | **Coordinación, pruebas y documentación:** suite `TestRunner` (44 pruebas E2E: 44 OK / 0 fallos), revisión de la arquitectura en capas y del cumplimiento de la consigna, `Contexto del Proyecto.md` e informe del entregable. |

---

## 8. Desviaciones y notas

- La rama se creó como `Concurrencia-Y-EF-Core` (sin espacios) porque **Git no admite espacios en
  nombres de ref**; se conserva el resto de la consigna.
- `master` **no fue modificado** en ningún momento; no se hicieron merges ni pull requests.
- No se incluyeron credenciales ni secrets (autenticación integrada local).
- Las capturas se generaron por automatización de la UI (sin intervención manual) sobre una
  sesión real de la aplicación.
