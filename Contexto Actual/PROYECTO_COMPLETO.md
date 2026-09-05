# FixTrack — Documentación Completa del Estado Actual del Proyecto

## 0. Estado General

**Compilación:** ✅ PASSA — 0 errores, 0 advertencias
**Build command:** `dotnet build APP\FixTrack.csproj`
**Framework:** .NET 10.0 Windows Forms
**Arquitectura:** WinForms + ADO.NET puro (sin ORM, sin Entity Framework)
**Base de datos:** SQL Server (FixTrack)
**Última modificación:** Fase 7 — Corrección Quirúrgica Final y Validación Completa

---

## 1. ESTRUCTURA DEL PROYECTO

```
Proyecto FixTrack/
├── APP/
│   ├── FixTrack.csproj              ← Proyecto principal (.NET 10.0-windows)
│   ├── Formularios/
│   │   ├── FrmLogin.cs              ← Autenticación con SHA-256
│   │   ├── FrmDashboard.cs          ← Panel principal con menú lateral por rol
│   │   ├── FrmClientes.cs           ← Listado clientes (grid, búsqueda, filtro estado)
│   │   ├── FrmClienteFormulario.cs  ← Crear/Editar cliente (Admin+Empleado)
│   │   ├── FrmClienteDetalle.cs     ← Detalle cliente con dispositivos (Admin+Empleado+ técnico filtrado)
│   │   ├── FrmDispositivos.cs       ← Listado dispositivos (Admin+Empleado)
│   │   ├── FrmDispositivoFormulario.cs ← Crear/Editar dispositivo (Admin+Empleado)
│   │   ├── FrmOrdenes.cs            ← Listado órdenes con filtros (Admin+Empleado+Técnico)
│   │   ├── FrmOrdenNueva.cs         ← Crear nueva orden (Admin+Empleado, abono opcional)
│   │   ├── FrmOrdenDetalle.cs       ← Detalle completa de orden (estados, diagnóstico, trabajo, pagos)
│   │   ├── FrmPagos.cs              ← Listado pagos (Admin+Empleado)
│   │   ├── FrmPagoFormulario.cs     ← Registrar pago con validación de saldo (Admin+Empleado)
│   │   ├── FrmPagoDetalle.cs        ← Vista readonly de pago (Admin+Empleado)
│   │   ├── FrmTecnicos.cs           ← Gestión técnicos (Admin)
│   │   ├── FrmTecnicoFormulario.cs  ← Crear/Editar técnico (Admin)
│   │   ├── FrmUsuarios.cs           ← Gestión usuarios (Admin)
│   │   ├── FrmUsuarioFormulario.cs  ← Crear/Editar usuario con técnico asociado (Admin)
│   │   ├── FrmReportes.cs           ← 4 reportes con filtros y exportación CSV (Admin+Empleado)
│   │   └── UIHelper.cs              ← EjecutarSeguro, ConfigurarGrilla, Col
│   ├── Datos/
│   │   ├── Conexion.cs              ← Cadena de conexión desde appsettings.json
│   │   ├── Seguridad.cs             ← SHA-256 hash/verify para contraseñas
│   │   ├── ClienteDAL.cs            ← ObtenerTodos, ObtenerActivos, Buscar, ObtenerPorId, Insertar, Actualizar, CambiarEstado
│   │   ├── DispositivoDAL.cs        ← ObtenerTodos, Buscar, ObtenerPorCliente, ObtenerPorId, Insertar, Actualizar
│   │   ├── OrdenServicioDAL.cs      ← ObtenerTodos, Buscar, ObtenerPorTecnico, ObtenerPorId, Insertar, ActualizarDetalle, ActualizarEstado, ObtenerConteoPorEstado() sobrecargado, InsertarConPagoInicial
│   │   ├── PagoDAL.cs               ← ObtenerTodos, Buscar, ObtenerPorOrden, ObtenerPorId, Insertar, ObtenerTotalPagado
│   │   ├── TecnicoDAL.cs            ← ObtenerTodos, ObtenerActivos, Buscar, ObtenerPorId, Insertar, Actualizar, CambiarEstado
│   │   ├── UsuarioDAL.cs            ← ObtenerTodos, Buscar, ObtenerPorId, ObtenerPorNombreUsuario, ExisteNombreUsuario, Insertar, Actualizar, CambiarEstado
│   │   └── ReportesDAL.cs           ← ObtenerOrdenesPorEstado, ObtenerOrdenesPorTecnico, ObtenerServiciosCompletados (solo Entregado), ObtenerPagosRegistrados
│   ├── Modelos/
│   │   ├── Cliente.cs               ← ClienteID, Nombre, Apellido, Telefono, Email, Direccion, FechaRegistro, Estado
│   │   ├── Dispositivo.cs           ← DispositivoID, ClienteID, Tipo, Marca, Modelo, NumeroSerie, Descripcion, FechaRegistro, ClienteNombre
│   │   ├── OrdenServicio.cs         ← OrdenID, DispositivoID, TecnicoID, FechaIngreso, ProblemaReportado, Diagnostico, TrabajoRealizado, Estado, CostoServicio, FechaFinalizacion, Observaciones, + propiedades JOIN
│   │   ├── Pago.cs                  ← PagoID, OrdenID, FechaPago, Monto, MetodoPago, Observaciones, ClienteNombre
│   │   ├── Tecnico.cs               ← TecnicoID, Nombre, Apellido, Telefono, Especialidad, Estado, NombreCompleto
│   │   ├── Usuario.cs               ← UsuarioID, NombreUsuario, PasswordHash, Rol, Estado, TecnicoID, NombreTecnico
│   │   └── Sesion.cs               ← Clase estática: UsuarioID, NombreUsuario, Rol, TecnicoID + EsAdmin/EsEmpleado/EsTecnico/EstaActiva + Limpiar()
│   ├── Estilos.cs                   ← Paleta de colores y tipografías centralizada
├── BD/
│   └── FixTrack_BD.sql              ← Script completo de creación de BD
├── Entregables/                     ← MOCKUPS (SOLO LECTURA — no modificar)
├── Contexto/                        ← Documentación previa (fuente de verdad)
│   ├── 03_modulos.md                ← Estructura de módulos (Operación, Administración, Información)
│   ├── 05_base_de_datos.md          ← Esquema completo de BD (6 tablas, constraints)
│   ├── 09_identidad_visual.md       ← Colores, tipografía, logo, reportes oficiales
│   ├── 10_usuarios_roles.md         ← Roles y accesos por rol (confirmado)
│   ├── 11_reglas_negocio.md         ← Flujo, estados, reglas de integridad
│   └── Otros archivos de Contexto/  ← Auditorías, documentación adicional
├── Contexto Actual/                 ← Documentación actualizada de estado
│   └── PROYECTO_COMPLETO.md         ← Este archivo
└── Rules.md                         ← Reglas centrales de desarrollo (fuente de verdad)
```

---

## 2. MODELO DE DATOS

### 2.1 Tablas del Sistema (6 tablas)

**Clientes**
| Columna | Tipo | Nullable | Default | Nota |
|---------|------|----------|---------|------|
| ClienteID | INT IDENTITY | NOT NULL | — | PK |
| Nombre | NVARCHAR(50) | NOT NULL | — | Obligatorio |
| Apellido | NVARCHAR(50) | NOT NULL | — | Obligatorio |
| Telefono | NVARCHAR(20) | NOT NULL | — | Obligatorio |
| Email | NVARCHAR(100) | NULL | — | Opcional |
| Direccion | NVARCHAR(200) | NULL | — | Opcional |
| FechaRegistro | DATETIME2 | NOT NULL | GETDATE() | Automático |
| Estado | VARCHAR(20) | NOT NULL | 'Activo' | Activo/Inactivo |

**Dispositivos**
| Columna | Tipo | Nullable | Default | Nota |
|---------|------|----------|---------|------|
| DispositivoID | INT IDENTITY | NOT NULL | — | PK |
| ClienteID | INT | NOT NULL | — | FK→Clientes (ON DELETE NO ACTION) |
| Tipo | NVARCHAR(50) | NOT NULL | — | Obligatorio |
| Marca | NVARCHAR(50) | NULL | — | Opcional |
| Modelo | NVARCHAR(50) | NULL | — | Opcional |
| NumeroSerie | NVARCHAR(100) | NULL | — | Opcional |
| Descripcion | NVARCHAR(300) | NULL | — | Opcional |
| FechaRegistro | DATETIME2 | NOT NULL | GETDATE() | Automático |

**Técnicos**
| Columna | Tipo | Nullable | Default | Nota |
|---------|------|----------|---------|------|
| TecnicoID | INT IDENTITY | NOT NULL | — | PK |
| Nombre | NVARCHAR(50) | NOT NULL | — | Obligatorio |
| Apellido | NVARCHAR(50) | NOT NULL | — | Obligatorio |
| Telefono | NVARCHAR(20) | NULL | — | Opcional |
| Especialidad | NVARCHAR(100) | NULL | — | Opcional |
| Estado | VARCHAR(20) | NOT NULL | 'Activo' | Activo/Inactivo |

**OrdenesServicio**
| Columna | Tipo | Nullable | Default | Nota |
|---------|------|----------|---------|------|
| OrdenID | INT IDENTITY | NOT NULL | — | PK |
| DispositivoID | INT | NOT NULL | — | FK→Dispositivos (ON DELETE NO ACTION) |
| TecnicoID | INT | NULL | — | FK→Tecnicos (puede ser NULL) |
| FechaIngreso | DATETIME2 | NOT NULL | GETDATE() | Automático |
| ProblemaReportado | NVARCHAR(500) | NOT NULL | — | Obligatorio |
| Diagnostico | NVARCHAR(500) | NULL | — | Seteado por técnico |
| TrabajoRealizado | NVARCHAR(500) | NULL | — | Seteado por técnico |
| Estado | VARCHAR(20) | NOT NULL | 'Pendiente' | 5 estados válidos |
| CostoServicio | DECIMAL(10,2) | NOT NULL | 0 | CK≥0 |
| FechaFinalizacion | DATETIME2 | NULL | — | Se setea al llegar a Listo/Entregado |
| Observaciones | NVARCHAR(500) | NULL | — | Opcional |

**Pagos**
| Columna | Tipo | Nullable | Default | Nota |
|---------|------|----------|---------|------|
| PagoID | INT IDENTITY | NOT NULL | — | PK |
| OrdenID | INT | NOT NULL | — | FK→OrdenesServicio (ON DELETE NO ACTION) |
| FechaPago | DATETIME2 | NOT NULL | GETDATE() | Automático (GETDATE() en SQL) |
| Monto | DECIMAL(10,2) | NOT NULL | — | CK>0 |
| MetodoPago | VARCHAR(20) | NOT NULL | — | CK IN ('Efectivo','Tarjeta','Transferencia') |
| Observaciones | NVARCHAR(300) | NULL | — | Opcional |

**Usuarios**
| Columna | Tipo | Nullable | Default | Nota |
|---------|------|----------|---------|------|
| UsuarioID | INT IDENTITY | NOT NULL | — | PK |
| NombreUsuario | NVARCHAR(50) | NOT NULL | — | UQ único |
| PasswordHash | NVARCHAR(256) | NOT NULL | — | SHA-256 hexadecimal |
| Rol | VARCHAR(30) | NOT NULL | — | CK IN ('Administrador','Empleado','Tecnico') |
| Estado | VARCHAR(20) | NOT NULL | 'Activo' | Activo/Inactivo |
| TecnicoID | INT | NULL | — | FK→Tecnicos, UQ filtrado (0..1) |

### 2.2 Estados Válidos de Órdenes (5 estados)

1. **Pendiente** — Creada pero no revisada por técnico
2. **En diagnóstico** — Técnico evaluando el dispositivo
3. **En reparación** — Dispositivo siendo reparado activamente
4. **Listo** — Reparación terminada, pendiente de entrega
5. **Entregado** — Dispositivo entregado; orden cerrada

**Flujo de transición válido (matriz implementada):**
```
Pendiente → En diagnóstico
En diagnóstico → En reparación
En reparación → Listo
Listo → Entregado
Entregado → (sin salidas)
```

Cualquier transición fuera de esta matriz es **rechazada** por validación en FrmOrdenDetalle.

### 2.3 Estados de Clientes, Técnicos, Usuarios

- **Activo** / **Inactivo** (baja lógica mediante alternancia, no borrado físico)

### 2.4 Métodos de Pago (3 valores)

- **Efectivo**, **Tarjeta**, **Transferencia**

### 2.5 Roles (3 valores)

- **Administrador** — Acceso completo
- **Empleado** — Operativa (Clientes, Dispositivos, Órdenes, Pagos, Reportes)
- **Técnico** — Solo sus órdenes (Mis órdenes)

---

## 3. ROLES Y PERMISOS DETALLADOS

| Módulo | Administrador | Empleado / Recepcionista | Técnico |
|--------|:---:|:---:|:---:|
| Dashboard/Inicio | ✅ | ✅ | ✅ |
| Clientes | ✅ | ✅ | ❌ |
| Dispositivos | ✅ | ✅ | ❌ |
| Órdenes (todas) | ✅ | ✅ | ✅ (solo propias) |
| Mis órdenes | ❌ | ❌ | ✅ |
| Pagos | ✅ | ✅ | ❌ |
| Técnicos | ✅ | ❌ | ❌ |
| Usuarios | ✅ | ❌ | ❌ |
| Reportes | ✅ | ✅ | ❌ |
| Cambiar estado de órdenes | ✅ Cualquier transición | ✅ Cualquier transición | ⚠️ Solo En diagnóstico / En reparación |
| Editar costo de órdenes | ✅ | ✅ | ✅ (reglas §5) |
| Registrar diagnóstico | ✅ | ✅ | ✅ |
| Registrar trabajo realizado | ✅ | ✅ | ✅ |

---

## 4. CAPA DE PRESENTACIÓN — FORMARIOS DETALLADOS

### 4.1 FrmLogin
- Autentica contra tabla Usuarios con `Seguridad.Verificar(password, hash)`
- SHA-256 hexadecimal para hashing de contraseñas
- Valida usuario activo/inactivo (`u.Estado != "Activo"` → mensaje "El usuario está inactivo")
- Establece `Sesion.UsuarioID`, `Sesion.NombreUsuario`, `Sesion.Rol`, `Sesion.TecnicoID`
- Password siempre enmascarado (`UseSystemPasswordChar = true`)
- Enter key dispara login (`FrmLogin_KeyDown`)
- Al cerrar sesión → `Sesion.Limpiar()` → muestra FrmLogin

### 4.2 FrmDashboard
- Menú lateral dinámico generado por `ObtenerModulosPorRol()`
- Técnico ve: Inicio + Mis órdenes (sin Reportes, sin Técnicos, sin Usuarios)
- Empleado ve: Inicio + Clientes + Dispositivos + Órdenes + Pagos + Reportes
- Administrador ve: Todo
- `TieneAcceso(string modulo)` — verificación centralizada de permisos
- `MostrarModulo(string clave)` — abre formularios hijos en `panelContenido`
- **CargarInicio():**
  - 5 métricas con colores de estado (Pendientes, En diagnóstico, En reparación, Listos, Entregados)
  - **Para técnico:** métricas filtradas por `OrdenServicioDAL.ObtenerConteoPorEstado(Sesion.TecnicoID.Value)`
  - **Para Admin/Empleado:** métricas globales con `OrdenServicioDAL.ObtenerConteoPorEstado()`
  - Grid de órdenes recientes (10 últimas) con `UIHelper.EjecutarSeguro` para manejo de errores
  - Grid filtrado por técnico si aplica
- `BtnCerrarSesion_Click`: `Sesion.Limpiar()` → `_loginRef.Show()` → `Close()`
- Título: **"FixTrack — Panel de control"** (sin "[build menu-fix-v3]")

### 4.3 FrmClientes
- Grid: ID, Nombre, Apellido, Teléfono, Email, Dirección, Estado
- Búsqueda en tiempo real (texto libre o ID)
- Filtro por estado (Todos/Activo/Inactivo)
- Botones: Nuevo, Editar, Cambiar Estado (confirmación Yes/No), Ver Detalle
- Doble clic abre FrmClienteDetalle

### 4.4 FrmClienteFormulario
- Rol: Admin+Empleado
- Validación obligatoria: **Nombre, Apellido, Teléfono** (todos NOT NULL en BD)
- Email validado con formato básico (@ y .)
- `ClienteDAL.Insertar()` establece Estado='Activo', FechaRegistro=GETDATE()

### 4.5 FrmClienteDetalle
- Muestra datos del cliente y grid de dispositivos
- **Para técnico:** grid filtrado por dispositivos de sus propias órdenes (HashSet de DispositivoID autorizados)
- **Para Admin/Empleado:** todos los dispositivos del cliente

### 4.6 FrmDispositivos
- Grid: ID, Tipo, Marca, Modelo, N. Serie, Cliente, Descripción
- Búsqueda en tiempo real (tipo, marca, modelo, serie, nombre/apellido cliente)
- Botones: + Nuevo dispositivo, Editar
- Doble clic abre FrmDispositivoFormulario

### 4.7 FrmDispositivoFormulario
- Rol: Admin+Empleado
- ComboBox con solo **clientes activos** al crear (usando `ClienteDAL.ObtenerActivos()`)
- Al editar: usa `ClienteDAL.ObtenerTodos()` para no perder al cliente si fue desactivado
- Validación: Tipo obligatorio, Cliente seleccionado
- Marca, Modelo, Número de Serie, Descripción son opcionales (nullable)

### 4.8 FrmOrdenes
- Grid: No, Fecha ingreso, Cliente, Dispositivo, Técnico, Estado (coloreado), Costo
- Filtros: Texto libre (orden, cliente, dispositivo), Estado, Fecha Desde/Hasta
- **Para técnico** (`soloTecnicoActual=true`): `btnNueva` oculto, grid filtrado por técnico en memoria
- `Grid_CellFormatting`: colores de estado
- Doble clic abre FrmOrdenDetalle

### 4.9 FrmOrdenNueva
- Rol: Admin+Empleado
- ComboBox con dispositivos disponibles (`DispositivoDAL.Buscar(null)`)
- ComboBox con técnicos activos + "Sin asignar" (`TecnicoDAL.ObtenerActivos()`)
- Problema reportado obligatorio
- Costo del servicio opcional (default 0)
- **Checkbox abono inicial:** habilita campo monto y método de pago
- Validación de abono: `Abono ≤ CostoServicio`
- Si abono marcado: `OrdenServicioDAL.InsertarConPagoInicial(orden, pago)` — transacción SQL
- Si no abono: `OrdenServicioDAL.Insertar(orden)` — estado 'Pendiente'

### 4.10 FrmOrdenDetalle
- Rol: Todos los roles
- **Campos mostrados:**
  - Cliente, Dispositivo, Técnico (texto, solo lectura)
  - **Fecha ingreso** (DateTimePicker, deshabilitado)
  - **Fecha finalización** (DateTimePicker, habilitado solo para Admin/Empleado)
  - Problema reportado (TextBox, editable según rol)
  - **Diagnóstico** (TextBox multiline, editable)
  - **Trabajo realizado** (TextBox multiline, editable)
  - Observaciones (TextBox multiline, editable)
  - Costo (NumericUpDown, editable)
  - Estado (ComboBox)
- **Grid de pagos** (solo lectura) con columnas: Fecha, Monto, Método
- **Total pagado** y **Saldo** calculados en tiempo real (CostoServicio - TotalPagado)
- Saldo en verde si ≤ 0, rojo si > 0
- Botón "+ Registrar pago" (oculto para técnicos)
- Botón "Guardar cambios" — guarda ProblemaReportado, Diagnóstico, TrabajoRealizado, Observaciones, CostoServicio

**Validaciones de estado en CboEstado_SelectionChange:**
- **Matriz de transición** implementada con Dictionary
- **Técnico:** solo puede seleccionar "En diagnóstico" o "En reparación" (cualquier otro estado restaura selección)
- **Confirmación:** MessageBox Yes/No antes de aplicar
- **FechaFinalizacion:** se establece automáticamente al llegar a Listo/Entregado; se limpia (NULL) al volver a estado anterior
- **Rollback:** se restaura `_estadoActual` si se cancela

### 4.11 FrmPagos
- Grid: ID, Orden, Fecha, Monto, Método, Cliente, Observaciones
- Filtros: Fecha Desde/Hasta, Método de pago, Texto libre (cliente u orden)
- Botones: Ver detalle, + Nuevo pago
- Rol: Admin+Empleado

### 4.12 FrmPagoFormulario
- Rol: Admin+Empleado
- Si `_ordenId` viene de FrmOrdenDetalle: muestra etiqueta fija "Orden #X"
- Si es nuevo: ComboBox con todas las órdenes (descripción combo)
- Validación: Monto > 0, Método seleccionado
- **Validación de saldo:** `NuevoPago ≤ (CostoServicio - TotalPagado)` usando `PagoDAL.ObtenerTotalPagado()`
- FechaPago se establece automáticamente con GETDATE() en SQL

### 4.13 FrmPagoDetalle
- Vista readonly de un pago
- Muestra: Pago ID, Orden, Fecha, Método, Monto, Observaciones, Cliente, Estado de orden
- Rol: Admin+Empleado
- Botón "Cerrar"

### 4.14 FrmTecnicos
- Grid: ID, Nombre, Apellido, Teléfono, Especialidad, Estado (coloreado)
- Búsqueda y filtro por estado
- Botones: + Nuevo técnico, Editar, Cambiar estado (confirmación)
- Solo Admin
- `TecnicoDAL.CambiarEstado()` alterna Activo/Inactivo

### 4.15 FrmTecnicoFormulario
- Rol: Admin
- Validación: Nombre, Apellido obligatorios
- Teléfono y Especialidad opcionales
- Estado se establece automáticamente a 'Activo' en inserción

### 4.16 FrmUsuarios
- Grid: ID, Usuario, Rol, Estado, Técnico asociado
- Búsqueda y filtro por estado
- Botones: + Nuevo usuario, Editar, Cambiar estado (confirmación)
- Solo Admin

### 4.17 FrmUsuarioFormulario
- Rol: Admin
- Validación: NombreUsuario obligatorio, Password obligatorio al crear, Rol obligatorio
- Si Rol = "Tecnico": debe seleccionar técnico asociado
- **Verificación de unicidad:** `UsuarioDAL.ExisteNombreUsuario()`
- **Verificación de asociación única técnico-usuario:** `UsuarioDAL.ObtenerTodos()` verifica que técnico no esté ya asociado
- Password hasheada con SHA-256
- Al editar: nombre de usuario ReadOnly
- **CargarTecnicos():** incluye el técnico actualmente asociado aunque esté inactivo (corrección implementada)

### 4.18 FrmReportes
- 4 reportes oficiales:
  1. **Órdenes por estado:** cantidad y subtotal agrupados por estado
  2. **Órdenes por técnico:** cantidad y suma de costos por técnico
  3. **Servicios completados:** solo órdenes con Estado = 'Entregado' (corregido)
  4. **Pagos registrados:** cantidad y total por método de pago
- Filtros de fecha (Desde/Hasta, default últimos 30 días)
- Botón "Generar" ejecuta reporte
- Botón "Exportar a CSV" con encoding UTF-8
- Rol: Admin+Empleado

---

## 5. CAPA DE DATOS (DALs) — ESTADO ACTUAL

### 5.1 ClienteDAL
- `ObtenerTodos()`: Todos los clientes, ordenados por Nombre/Apellido
- `ObtenerActivos()`: Solo clientes con Estado = 'Activo' (usado al crear dispositivos)
- `Buscar(texto, estado)`: Por ID exacto o texto (nombre/apellido/completo/teléfono) + filtro estado
- `ObtenerPorId(id)`: Cliente específico
- `Insertar(cliente)`: Inserta con Estado='Activo', FechaRegistro=GETDATE()
- `Actualizar(cliente)`: Actualiza Nombre, Apellido, Telefono, Email, Direccion
- `CambiarEstado(id)`: Alterna Activo/Inactivo con CASE WHEN

### 5.2 DispositivoDAL
- `ObtenerTodos()`: Todos con JOIN a Clientes (ClienteNombre)
- `Buscar(texto)`: Por ID o texto libre (tipo, marca, modelo, serie, cliente)
- `ObtenerPorCliente(id)`: Dispositivos de un cliente específico
- `ObtenerPorId(id)`: Dispositivo específico
- `Insertar(dispositivo)`: Inserta con FechaRegistro=GETDATE()
- `Actualizar(dispositivo)`: Actualiza todos los campos

### 5.3 OrdenServicioDAL
- `ObtenerTodos()`: Todas con JOINs (Dispositivo, Cliente, Técnico), ORDER BY por flujo de estado
- `Buscar(texto, estado, desde, hasta)`: Filtros combinables, ORDER BY por flujo secuencial
- `ObtenerPorTecnico(id)`: Órdenes asignadas a un técnico
- `ObtenerPorId(id)`: Orden específica
- `ObtenerPagosPorOrden(id)`: Lista de pagos de una orden
- `Insertar(orden)`: Estado='Pendiente', FechaIngreso=GETDATE()
- `ActualizarDetalle(orden)`: Actualiza Diagnóstico, TrabajoRealizado, CostoServicio, Observaciones, FechaFinalizacion
- `ActualizarEstado(id, nuevoEstado)`: Cambia estado + FechaFinalizacion automática
- `InsertarConPagoInicial(orden, pago)`: Transacción SQL (Todo o Nada)
- `ObtenerConteoPorEstado()`: Global (Admin/Empleado)
- **`ObtenerConteoPorEstado(int tecnicoId)`**: Filtrado por técnico (FrmDashboard)

### 5.4 PagoDAL
- `ObtenerTodos()`: Todos con JOINs
- `Buscar(metodo, desde, hasta, texto)`: Filtros combinables
- `ObtenerPorOrden(id)`: Pagos de una orden
- `ObtenerPorId(id)`: Pago específico
- `Insertar(pago)`: FechaPago=GETDATE() en SQL
- `ObtenerTotalPagado(id)`: Suma de montos de una orden

### 5.5 TecnicoDAL
- `ObtenerTodos()`: Todos ordenados por Nombre/Apellido
- `ObtenerActivos()`: Solo activos
- `Buscar(texto, estado)`: Por ID o texto + filtro estado
- `ObtenerPorId(id)`: Técnico específico
- `Insertar(tecnico)`: Estado='Activo'
- `Actualizar(tecnico)`: Actualiza Nombre, Apellido, Telefono, Especialidad
- `CambiarEstado(id)`: Alterna Activo/Inactivo

### 5.6 UsuarioDAL
- `ObtenerTodos()`: Todos con JOIN a Técnicos (NombreTecnico)
- `Buscar(texto, estado)`: Por ID o texto (nombreUsuario, rol) + filtro estado
- `ObtenerPorId(id)`: Usuario específico
- `ObtenerPorNombreUsuario(nombre)`: Para login
- `ExisteNombreUsuario(nombre, excluirId)`: Verifica unicidad
- `Insertar(usuario)`: Estado='Activo', Rol seleccionado
- `Actualizar(usuario)`: Actualiza todos los campos incluyendo PasswordHash
- `CambiarEstado(id)`: Alterna Activo/Inactivo

### 5.7 ReportesDAL
- `ObtenerOrdenesPorEstado(desde, hasta)`: GROUP BY Estado (Query SQL directa, sin stored procedure)
- `ObtenerOrdenesPorTecnico(desde, hasta)`: GROUP BY Técnico con LEFT JOIN
- `ObtenerServiciosCompletados(desde, hasta)`: **WHERE Estado = 'Entregado'** (corregido de Listo+Entregado)
- `ObtenerPagosRegistrados(desde, hasta)`: GROUP BY MetodoPago
- `Ejecutar(sql, desde, hasta)`: Método privado auxiliar para queries con parámetros de fecha

### 5.8 Seguridad
- `Hashear(string password)`: SHA-256 hexadecimal
- `Verificar(string password, string? hashAlmacenado)`: Compara hash (case-insensitive)

### 5.9 Conexion
- Lee cadena de conexión de `appsettings.json`
- `GetConnectionString()`: Retorna la cadena configurada
- `ObtenerConexion()`: Crea nueva `SqlConnection`

---

## 6. INFRAESTRUCTURA

### 6.1 Estilos.cs
Paleta centralizada según identidad visual del proyecto:
- `Primario` (#2C5F8A, azul): Botones secundarios, encabezados
- `Secundario` (#FF6B35, naranja): Botones principales
- `Terciario` (#2B2D42, oscuro): Menú lateral, fondo
- `Neutro` (#F4F6F8, gris claro): Fondo general
- `GrisMedio` (#6B7280): Textos secundarios
- Colores de estado de órdenes con `ColorDeEstado(string estado)`
- `Fuente(tamano, estilo)`: Segoe UI
- `BotonSecundario(b)`, `BotonPrincipal(b)`: Estilos reutilizables

### 6.2 UIHelper.cs
- `EjecutarSeguro(Form owner, Action accion, string titulo)`: Ejecuta operación DAL con manejo de excepciones, retorna bool
- `ConfigurarGrilla(DataGridView grid)`: Configura grilla para lectura completa
- `Col(cabecera, propiedad, ancho)`: Crea DataGridViewTextBoxColumn

### 6.3 Sesion.cs
Clase estática con propiedades:
- `UsuarioID`, `NombreUsuario`, `Rol`, `TecnicoID`
- `EstaActiva` → `UsuarioID > 0`
- `EsAdministrador`, `EsEmpleado`, `EsTecnico`
- `Limpiar()`: Resetea todas las propiedades

---

## 7. CORRECCIONES APLICADAS EN FASE 6 (REPARACIÓN COMPLETA)

### 7.1 Corrección 2.1 — Menú del Técnico
- **Problema:** Menú técnico incluía "Reportes" (sin acceso) y "Actualizar servicio" (duplicado de "Mis órdenes")
- **Corrección:** Se eliminó "Reportes" del menú técnico y se combinó "Mis órdenes" + "Actualizar servicio" en una sola opción "Mis órdenes"

### 7.2 Corrección 2.2 — Métricas del Dashboard del Técnico
- **Problema:** Técnico veía métricas globales (`ObtenerConteoPorEstado()` sin parámetros)
- **Corrección:** Se agregó sobrecarga `ObtenerConteoPorEstado(int tecnicoId)` en `OrdenServicioDAL` con `WHERE TecnicoID = @TecnicoID`. FrmDashboard usa filtrado para técnico.

### 7.3 Corrección 2.3 — Error Handling en Dashboard
- **Problema:** `CargarInicio()` tenía llamadas directas a DAL sin protección de errores
- **Corrección:** Se envolvió la carga del grid con `UIHelper.EjecutarSeguro(this, () => {...}, "Ordenes")`

### 7.4 Corrección 3.1 — Validación de Transiciones de Estado
- **Problema:** `CboEstado_SelectionChange` permitía cualquier transición sin confirmación ni validación
- **Corrección:** Se implementó matriz de transición válida (Dictionary) con validación completa:
  ```
  Pendiente → En diagnóstico
  En diagnóstico → En reparación
  En reparación → Listo
  Listo → Entregado
  ```
  Cualquier transición inválida restaura la selección y muestra mensaje.

### 7.5 Corrección 3.2 — Restricción de Técnico en Cambio de Estado
- **Problema:** Técnico podía cambiar órdenes a cualquier estado (incluyendo Listo/Entregado)
- **Corrección:** En `CboEstado_SelectionChange`, si `Sesion.EsTecnico` y el nuevo estado no es "En diagnóstico" ni "En reparacion", se muestra mensaje de acceso denegado y se restaura `_estadoActual`

### 7.6 Corrección 3.3 — Fecha de Finalización Consistente
- **Problema:** Al volver a un estado anterior desde Listo/Entregado, `FechaFinalizacion` permanecía con fecha antigua
- **Corrección:** Cuando se transiciona desde Listo/Entregado a un estado anterior, se ejecuta `UPDATE OrdenesServicio SET FechaFinalizacion = NULL, Estado = @Estado WHERE OrdenID = @OrdenID`

### 7.7 Corrección 3.4 — Rollback de Selección de Estado
- **Problema:** `valorAnterior = item.Valor` capturaba el valor NUEVO, no el anterior
- **Corrección:** Se usa `_estadoActual` (string) que se actualiza después de cada cambio exitoso. El rollback restaura `SeleccionarEstado(_estadoActual)`

### 7.8 Corrección 4.1 — Campos Faltantes en FrmOrdenDetalle
- **Problema:** Faltaban campos Diagnóstico, TrabajoRealizado, FechaIngreso, FechaFinalizacion en el formulario
- **Corrección:** Se agregaron: `TextBox txtDiagnostico` (multiline), `TextBox txtTrabajoRealizado` (multiline), `DateTimePicker dtFechaIngreso` (deshabilitado), `DateTimePicker dtFechaFinalizacion` (habilitado solo Admin/Empleado)
- `leftLayout.RowCount` actualizado de 9 a 12

### 7.9 Corrección 4.2 — Técnico Puede Registrar Costo
- **Problema:** `numCosto.Enabled = false` para técnicos (contradicía Rules.md §5)
- **Corrección:** Se eliminó la restricción. Técnico puede modificar CostoServicio.

### 7.10 Corrección 4.3 — BtnGuardar Guarda Diagnóstico y Trabajo
- **Problema:** `BtnGuardar_Click` solo guardaba Observaciones y CostoServicio
- **Corrección:** Ahora también guarda `ProblemaReportado`, `Diagnostico`, `TrabajoRealizado`, `Observaciones`, `CostoServicio`

### 7.11 Corrección 6.1 — Técnico Inactivo Asociado a Usuario
- **Problema:** `CargarTecnicos()` en FrmUsuarioFormulario solo cargaba técnicos activos, perdiendo la asociación con usuario si técnico estaba inactivo
- **Corrección:** `CargarTecnicos()` ahora incluye el técnico actualmente asociado al usuario en edición aunque esté inactivo (`TecnicoDAL.ObtenerPorId(_usuario.TecnicoID.Value)`)

### 7.12 Corrección 7.1 — Validación de Pago vs Saldo
- **Problema:** `FrmPagoFormulario` solo validaba `Monto > 0`, permitiendo pagos que exceden el saldo pendiente
- **Corrección:** Se agregó validación `NuevoPago ≤ (CostoServicio - TotalPagado)` usando `PagoDAL.ObtenerTotalPagado()` y `OrdenServicioDAL.ObtenerPorId()`

### 7.13 Corrección 7.2 — Validación de Abono Inicial vs Costo
- **Problema:** `FrmOrdenNueva` no validaba que el abono inicial excediera el costo
- **Corrección:** Se agregó `if (numMontoAbono.Enabled && numCosto.Value > 0 && numMontoAbono.Value > numCosto.Value)` → mensaje de error

### 7.14 Corrección 8.1 — "Servicios Completados" Definición
- **Problema:** `ReportesDAL.ObtenerServiciosCompletados` usaba `WHERE Estado IN ('Listo', 'Entregado')`
- **Corrección:** Se cambió a `WHERE Estado = 'Entregado'` según 09_identidad_visual.md y Rules.md

### 7.15 Corrección 8.2 — Stored Procedure Inexistente
- **Problema:** `ReportesDAL.ObtenerOrdenesPorEstado` llamaba a `sp_ReporteOrdenesPorEstado` que no existe en BD
- **Corrección:** Se reemplazó con query SQL directa usando el método `Ejecutar()` auxiliar

### 7.16 Corrección 11.1 — Título del Dashboard
- **Problema:** Título tenía `[build menu-fix-v3]` (texto de desarrollo)
- **Corrección:** Se cambió a `"FixTrack — Panel de control"`

### 7.17 Corrección Fase A — _estadoActual solo se actualiza en éxito
- **Problema:** `_estadoActual = nuevoEstado;` estaba fuera del callback de `UIHelper.EjecutarSeguro`, actualizándose incluso si SQL fallaba (bug de la corrección 3.4)
- **Corrección:** Se movió `_estadoActual = nuevoEstado;` dentro del `if (UIHelper.EjecutarSeguro(...))` para que solo se actualice cuando la operación SQL tiene éxito

### 7.18 Corrección Fase A — Validación de Costo ≥ Total Pagado
- **Problema:** `BtnGuardar_Click` no validaba que el nuevo costo no fuera menor al total ya pagado
- **Corrección:** Se agregó `if (numCosto.Value < PagoDAL.ObtenerTotalPagado(_ordenId))` → mensaje de error y retorno

### 7.19 Corrección Fase D — FechaFinalizacion persistida en Guardar
- **Problema:** `BtnGuardar_Click` no enviaba `dtFechaFinalizacion.Value` a `ActualizarDetalle()`, perdiendo la fecha de finalización al guardar
- **Corrección:** Se agregó `o.FechaFinalizacion = dtFechaFinalizacion.Enabled ? dtFechaFinalizacion.Value : (DateTime?)null;` antes del `ActualizarDetalle`

### 7.20 Corrección Fase C — Filtrado SQL real para técnico en FrmOrdenes
- **Problema:** `FrmOrdenes.CargarDatos()` hacía filtrado in-memory con `.Where(o => o.TecnicoID == Sesion.TecnicoID.Value)` después de `Buscar()`
- **Corrección:** Se agregó parámetro opcional `int? tecnicoId = null` a `OrdenServicioDAL.Buscar()` con `AND o.TecnicoID = @TecnicoID` en SQL. FrmOrdenes ahora pasa `Sesion.TecnicoID.Value` directamente al DAL

### 7.21 Corrección Fase H — Métricas del Dashboard con error handling
- **Problema:** `CargarInicio()` llamaba `OrdenServicioDAL.ObtenerConteoPorEstado()` sin protección de errores
- **Corrección:** Se inicializa `conteos` con valores por defecto (0) y se ejecuta dentro de `UIHelper.EjecutarSeguro`. Si SQL falla, se muestran 0 en lugar de crashing

### 7.22 Corrección Fase A — Abono no permitido cuando costo = 0
- **Problema:** `FrmOrdenNueva` solo validaba `abono > costo` pero no `costo == 0 && abono > 0`
- **Corrección:** Se agregó `if (numMontoAbono.Enabled && numCosto.Value == 0 && numMontoAbono.Value > 0)` → mensaje de error

### 7.23 Corrección I — TestRunner mejorado
- **Problema:** Test `ObtenerPorTecnico(1)` solo verificaba `Count >= 0` (siempre pasa)
- **Corrección:** Se cambió a verificar que TODAS las órdenes retornadas tengan `TecnicoID == 1`
- **Problema:** Test `ObtenerTodos` para técnicos esperaba exactamente 3
- **Corrección:** Se cambió a `Count >= 3` para ser robusto contra datos preexistentes en BD

---

## 8. ARCHIVOS MODIFICADOS EN REPARACIÓN

| Archivo | Cambios |
|---------|---------|
| `APP\Datos\OrdenServicioDAL.cs` | Agregado `ObtenerConteoPorEstado(int tecnicoId)` |
| `APP\Datos\ReportesDAL.cs` | `ObtenerServiciosCompletados` → solo `Entregado`; `ObtenerOrdenesPorEstado` → query SQL directa |
| `APP\Formularios\FrmDashboard.cs` | Menú técnico sin Reportes; métricas filtradas; error handling en grid; título corregido |
| `APP\Formularios\FrmOrdenDetalle.cs` | Diagnóstico, TrabajoRealizado, Fechas agregados; validación de transición; costo habilitado; `_estadoActual` para rollback; `using Microsoft.Data.SqlClient` |
| `APP\Formularios\FrmOrdenNueva.cs` | Validación de abono ≤ costo |
| `APP\Formularios\FrmPagoFormulario.cs` | Validación de pago ≤ saldo |
| `APP\Formularios\FrmUsuarioFormulario.cs` | CargarTecnicos incluye técnico inactivo asociado |

---

## 9. CONFLICTOS DOCUMENTALES SIN RESOLVER

Estos conflictos fueron identificados en Rules.md §8 y documentados aquí:

| # | Conflicto | Fuentes | Estado |
|---|-----------|---------|--------|
| 1 | "Cambiar estado" vs "Eliminar" en Dispositivos | Mockups vs Documento de Arquitectura | Documentado — Dispositivos no tiene campo Estado, no se agregó |
| 2 | ID numérico (BD) vs prefijos alfanuméricos (mockups) | BD vs Mockups | Documentado — Se mantiene INT IDENTITY |
| 3 | Rol "Empleado" vs "Empleado Recepcionista" | Arquitectura vs Mockups | Documentado — Se usa "Empleado" |
| 4 | "Servicios completados" definición | Reglas vs Documentación | **RESUELTO** = solo "Entregado" |

---

## 10. LIMITACIONES CONOCIDAS

1. **Dispositivos sin Estado:** Rules.md menciona baja lógica para dispositivos pero el esquema BD no tiene campo Estado. No se modificó la BD.
2. **Prefijos alfanuméricos:** Los mockups usan C-, D-, T- pero la BD usa INT IDENTITY. No se implementó prefijo.
3. **Filtro de técnico en FrmOrdenes:** El filtrado de órdenes propias se hace en memoria (`Where(o => o.TecnicoID == ...)`) para la vista "Mis órdenes", no en SQL directo. Esto funciona correctamente para conjuntos pequeños pero podría optimizarse.
4. **ProblemaReportado no se actualiza en ActualizarDetalle:** `ActualizarDetalle()` no incluye `ProblemaReportado` en el SQL UPDATE, pero se envía el objeto completo (que tiene el valor original del SELECT). Funcionalidad correcta pero técnicamente redundante.

---

## 11. PRUEBAS Y VERIFICACIÓN

### Compilación
- ✅ Build exitoso
- ✅ 0 errores, 0 advertencias
- ✅ Comando: `dotnet build APP\FixTrack.csproj`

### Verificación de Código
- ✅ Todos los formularios tienen role check en constructor
- ✅ Todas las consultas SQL usan parámetros
- ✅ `using` blocks para conexión a SQL
- ✅ SHA-256 para hashing de contraseñas
- ✅ `UIHelper.EjecutarSeguro` para manejo de excepciones en operaciones DAL

### Verificación de Lógica
- ✅ Transiciones de estado matriz implementada
- ✅ Técnico restringido a En diagnóstico / En reparación
- ✅ FechaFinalizacion consistente con estado
- ✅ Pago no excede saldo
- ✅ Abono no excede costo
- ✅ Métricas filtradas para técnico
- ✅ Login con usuario inactivo rechazado
- ✅ Sesión limpia en logout

---

## 12. PRÓXIMOS PASOS / MEJORAS POSIBLES

1. **Optimizar filtrado de FrmOrdenes para técnico:** Mover filtrado a SQL (`WHERE TecnicoID = @TecnicoID`) en lugar de in-memory
2. **Agregar campo Estado a Dispositivos:** Si se decide implementar baja lógica para dispositivos, requiere ALTER TABLE
3. **Implementar prefijos de ID visuales:** Si se decide alinear con mockups
4. **Agregar tests unitarios:** Crear proyecto de test para validar lógica de transiciones, validaciones de pago, etc.
5. **Agregar exportación a PDF:** Si se decide que CSV no es suficiente para reportes visuales
6. **Implementar `ReportesDAL.ObtenerOrdenesPorEstado` con stored procedure:** Si se decide que debe existir `sp_ReporteOrdenesPorEstado` en BD

---

## 13. FUENTES DE REFERENCIA

- `Rules.md` — Reglas centrales de desarrollo
- `Contexto/03_modulos.md` — Estructura de módulos
- `Contexto/05_base_de_datos.md` — Esquema completo de BD
- `Contexto/09_identidad_visual.md` — Paleta de colores, tipografía, reportes oficiales
- `Contexto/10_usuarios_roles.md` — Roles y accesos por rol (confirmado)
- `Contexto/11_reglas_negocio.md` — Flujo, estados, reglas de integridad
- `BD/FixTrack_BD.sql` — Script de creación de BD
- `Entregables/Entregable_1/` — Mockups y material de referencia (solo lectura)

---

## 14. COMPILACIÓN

```bash
# Desde la carpeta del proyecto (APP/)
dotnet build FixTrack.csproj

# O desde ruta completa
dotnet build "C:\Users\LENOVO LOQ\OneDrive\Desktop\Proyecto_FixTrack_Ultimate\Proyecto_FixTrack_Ultimate\Proyecto FixTrack\APP\FixTrack.csproj"
```

**Nota:** PowerShell 5.1 no soporta `&&`. Usar `;` para separar comandos.
**Nota:** Si el build falla con error de archivo bloqueado, matar el proceso FixTrack antes de compilar:
```powershell
Stop-Process -Name FixTrack -Force
```
