# Contexto del Proyecto — FixTrack

> Documento consolidado de contexto para el entregable **«Concurrencia + EF Core + Reportes»**.
> Reúne la información de todo el proyecto: negocio, alcance, modelo de datos, arquitectura,
> historial de desarrollo y estado actual.

---

## 1. Identificación

| Campo | Valor |
|---|---|
| **Proyecto** | FixTrack — Sistema de Gestión de Reparaciones de Dispositivos Electrónicos |
| **Empresa (ficticia)** | TecnoFix Solutions |
| **Asignatura** | Programación de Aplicaciones de Escritorio |
| **Repositorio** | https://github.com/char-lie17/FixTrack.git |
| **Rama del entregable** | `Concurrencia-Y-EF-Core` (rama principal de trabajo) |
| **Rama protegida** | `master` — sin modificaciones (último commit `1f20666`) |
| **Tecnologías** | C# · .NET 10.0 · Windows Forms · EF Core 10.0.13 · SQL Server |

---

## 2. Empresa y objetivo

**TecnoFix Solutions** es una empresa dedicada a la reparación y mantenimiento de computadoras y
otros dispositivos electrónicos. Gestiona información de clientes, dispositivos recibidos,
órdenes de servicio, técnicos, estados de reparación y pagos.

**Problema:** la información del proceso de servicio estaba dispersa; no había un registro
centralizado ni seguimiento de cada reparación desde la recepción hasta la entrega.

**Objetivo del sistema:** aplicación de escritorio interna que permita gestionar y consultar de
forma organizada la información generada durante el proceso de servicio, *desde el registro del
cliente y su dispositivo hasta la finalización y entrega del mismo*.

**Flujo de negocio:**

```
Cliente → Dispositivo → Orden de servicio → Técnico → Diagnóstico/Reparación → Estado → Pago → Entrega
```

---

## 3. Alcance y módulos

La interfaz se organiza en un menú lateral con tres grupos, más Dashboard y Cerrar sesión:

| Grupo | Módulo | Función |
|---|---|---|
| **Operación** | Clientes | CRUD con búsqueda y filtro por estado |
| | Dispositivos | Registro de equipos asociados a clientes |
| | Órdenes | Flujo completo de 5 estados: *Pendiente → En diagnóstico → En reparación → Listo → Entregado* |
| | Pagos | Registro con validación de saldo; soporta abonos parciales |
| **Administración** | Técnicos | Gestión del personal técnico (solo Administrador) |
| | Usuarios | Cuentas de acceso y roles (solo Administrador) |
| **Información** | Reportes | 4 reportes con filtros de fecha y exportación a CSV |
| — | Inicio (Dashboard) | Métricas en tiempo real de órdenes por estado, filtradas por rol |

Además: **Historial de órdenes** (auditoría de creación, cambios de estado, ediciones y pagos).

### Roles y permisos

| Módulo | Administrador | Empleado | Técnico |
|--------|:---:|:---:|:---:|
| Dashboard | ✅ | ✅ | ✅ (filtrado) |
| Clientes / Dispositivos | ✅ | ✅ | ❌ |
| Órdenes | ✅ | ✅ | ✅ (solo propias) |
| Pagos | ✅ | ✅ | ❌ |
| Técnicos / Usuarios | ✅ | ❌ | ❌ |
| Reportes | ✅ | ✅ | ❌ |

Roles (CHECK en BD): `Administrador`, `Empleado`, `Tecnico`. Relación `Usuarios (0..1) → Tecnicos (1)`
con índice único filtrado `UQ_Usuarios_TecnicoID` (un técnico, a lo sumo, un usuario).

---

## 4. Modelo de datos

7 tablas (esquema creado por migración EF Core `20261023004419_Inicial`, idéntico al script SQL):

| Tabla | Clave | Observaciones |
|---|---|---|
| `Clientes` | PK `ClienteID` | `Estado` (CHECK), `FechaRegistro` con DEFAULT |
| `Dispositivos` | PK `DispositivoID` | FK `ClienteID` → Clientes (Restrict) |
| `OrdenesServicio` | PK `OrdenID` | FK `DispositivoID`, `TecnicoID` (Restrict); CHECK de estados y totales |
| `Pagos` | PK `PagoID` | FK `OrdenID` → OrdenesServicio (Restrict) |
| `Tecnicos` | PK `TecnicoID` | — |
| `Usuarios` | PK `UsuarioID` | FK `TecnicoID` opcional; CHECK Rol/Estado; índice único filtrado |
| `HistorialOrdenes` | PK `HistorialID` | FK `OrdenID`; índice `IX_HistorialOrdenes_OrdenID_FechaCambio DESC` |

Relaciones: `Clientes 1→N Dispositivos`, `Dispositivos 1→N OrdenesServicio`,
`Tecnicos 1→N OrdenesServicio`, `OrdenesServicio 1→N Pagos`,
`OrdenesServicio 1→N HistorialOrdenes`, `Tecnicos 1→(0..1) Usuarios`.

Reportes: procedimiento almacenado `sp_ReporteOrdenesPorEstado` + consultas LINQ en `ReportesDAL`.

---

## 5. Arquitectura técnica

Patrón de capas (conservado del proyecto original):

```
Presentación (WinForms)   ← Formularios, validaciones UI, UIHelper
        ↓
Modelos (POCOs)            ← Cliente, Dispositivo, OrdenServicio, Pago, Tecnico, Usuario, Sesion...
        ↓
Datos (DAL)                ← EF Core 10 (DbContext) + ADO.NET puntual (Conexion/Seguridad)
        ↓
SQL Server                 ← Base de datos FixTrack
```

- **Contexto:** `APP/Datos/FixTrackDbContext.cs` (+ `FixTrackDbContextFactory` para `dotnet ef`).
- **Inicialización:** `FixTrackDb.Inicializar()` en `Program.Main` (y en `TestRunner`) garantiza que
  la BD exista con esquema y semilla antes de usar la app.
- **Seguridad:** contraseñas con hash SHA-256 (`Seguridad.cs`); acceso con `Microsoft.Data.SqlClient`.
- **Estilos:** paleta centralizada en `Formularios/Estilos.cs`.
- **Proyecto de verificación:** `APP/TestRunner` — 44 pruebas de extremo a extremo sobre la BD real.

---

## 6. Historial de desarrollo

1. **Snapshot inicial** (`98ef50f`, de Henry Creel): app ADO.NET funcional con los 8 módulos.
2. **Entregables 1–2:** documentación de contexto (`Contexto/01`…`15`), identidad visual, mockups,
   auditorías y corrección de bugs conocidos (z-order de docking, columnas de reportes,
   alineación de botones) → `BUGS_Y_CORRECCIONES.md`, `AUDITORIA_ENTREGABLE_2.md`.
3. **Estados y reglas:** flujo de 5 estados de órdenes, historial de cambios, pagos transaccionales
   con protección contra concurrencia.
4. **Este entregable — Concurrencia + EF Core + Reportes** (rama `Concurrencia-Y-EF-Core`):
   - Migración de los 7 DAL de ADO.NET a **EF Core 10** con API **async/await** completa
     (`*Async(CancellationToken ct = default)`).
   - **19 formularios** convertidos a operaciones asíncronas con:
     - `CancellationTokenSource` en listados (cancelación al cerrar el formulario),
     - botón de guardado deshabilitado durante la operación + `finally`,
     - combos y detalles cargados con `async`/`Shown`,
     - `IProgress<string>` en el Dashboard,
     - detección de **conflicto optimista** en `FrmOrdenDetalle`,
     - `UIHelper.EjecutarSeguroAsync` para manejo unificado de excepciones
       (distingue `OperationCanceledException` de errores reales).
   - Migración EF Core generada (`APP/Migrations/20261023004419_Inicial.*`).
   - Reportes dinámicos conectados a datos reales (LINQ + SP).
   - `BD/Seed_Datos_Prueba.sql` + `BD/INSTRUCCIONES_RECONSTRUCCION.md` (vía migraciones o script).

---

## 7. Estado actual y verificación

| Verificación | Resultado |
|---|---|
| `dotnet build FixTrack.sln` | ✅ 0 errores (12 warnings CS8602 preexistentes) |
| `dotnet ef database update` | ✅ migración `20261023004419_Inicial` aplicada |
| `dotnet run --project APP/TestRunner` | ✅ **44 OK, 0 FALLO(S)** |
| BD real (sqlcmd) | ✅ 8 Clientes, 9 Dispositivos, 8 Órdenes, 5 Pagos, 5 Usuarios, 3 Técnicos, 9 Historial |
| Aplicación ejecutándose | ✅ login, dashboard, reportes y módulos con datos reales (capturas en `Evidencias/`) |
| `master` | ✅ intacto en `1f20666` (nada pusheado a master) |

**Credenciales de prueba:** `admin/admin123`, `recepcion1/recepcion123`,
`luis.ortega|karla.vega|diego.salinas/tecnico123`.

### Cómo ejecutar el proyecto

```powershell
# 1) BD por migraciones (recomendado)
cd APP
dotnet tool install --global dotnet-ef
dotnet ef database update
sqlcmd -S localhost -E -C -i ..\BD\Seed_Datos_Prueba.sql

# 2) Compilar y ejecutar
dotnet build FixTrack.sln
dotnet run
```

> ⚠️ `BD/FixTrack_BD.sql` contiene `DROP DATABASE` (recrea todo). Ver
> `BD/INSTRUCCIONES_RECONSTRUCCION.md` antes de ejecutarlo.

---

## 8. Reglas de trabajo y desviaciones

- **No se modificó `master`**; todo el trabajo vive en `Concurrencia-Y-EF-Core`.
- La rama se llama con guiones en lugar de espacios porque **Git no admite espacios en nombres de
  ref** (desviación documentada del nombre sugerido «Concurrencia Y EF Core»).
- Se conservaron `Entregables/` y `Contexto/` sin cambios, y la arquitectura en capas original.
- No se incluyeron secrets; la cadena de conexión usa autenticación integrada de Windows local.
- Matices técnicos documentados: `Controls.Find` usa `searchAllChildren`; el DAL genérico de
  `EjecutarSeguroAsync` descarta resultados (capturarlos dentro del lambda); los scripts `.ps1`
  deben guardarse en UTF-8 con BOM para PowerShell 5.1.

---

## 9. Estructura del repositorio

```
FixTrack/
├── APP/
│   ├── Datos/          ← DAL (EF Core): 7 DAL + FixTrackDbContext + Conexion + Seguridad
│   ├── Formularios/    ← 19 formularios async/await + Estilos + UIHelper
│   ├── Modelos/        ← POCos de dominio
│   ├── Migrations/     ← 20261023004419_Inicial (+ Designer, Snapshot)
│   ├── TestRunner/     ← 44 pruebas E2E
│   ├── appsettings.json
│   └── Program.cs      ← inicialización de BD + arranque
├── BD/                 ← FixTrack_BD.sql, Seed_Datos_Prueba.sql, INSTRUCCIONES_RECONSTRUCCION.md
├── Contexto/           ← documentación de análisis (01…15)
├── Contexto Actual/    ← PROYECTO_COMPLETO.md
├── Entregables/        ← mockups y diseños (intacto)
├── Contexto de la Vibecodeada.md  ← bitácora de este entregable (etapas B–E)
├── Rules.md
└── README.md
```
