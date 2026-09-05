# FASE 3 — Planificación Técnica de la Implementación de FixTrack

## Advertencia

**EN ESTA FASE NO SE IMPLEMENTA NADA.**

No se crean formularios. No se escribe código funcional. No se crean clases definitivas. No se conecta SQL Server. No se modifica la base de datos. No se modifica el Entregable 1. No se comienza la implementación.

Este documento es exclusivamente un plan técnico de análisis, diseño y planificación para la implementación del Entregable 2 de FixTrack.

---

## 1. FUENTES CONSULTADAS

| Archivo | Propósito |
|---------|-----------|
| `Rules.md` | Reglas centrales de desarrollo |
| `Contexto/README.md` | Guía de navegación de la documentación |
| `Contexto/01_resumen_ejecutivo.md` | Resumen del proyecto |
| `Contexto/02_empresa_y_objetivo.md` | Empresa y objetivo del sistema |
| `Contexto/03_modulos.md` | Módulos del sistema |
| `Contexto/04_navegacion.md` | Navegación y menús |
| `Contexto/05_base_de_datos.md` | Estructura completa de la BD |
| `Contexto/07_relaciones.md` | Relaciones entre entidades |
| `Contexto/08_mockups.md` | Análisis detallado de cada pantalla |
| `Contexto/09_identidad_visual.md` | Paleta, tipografía, controles, iconografía |
| `Contexto/10_usuarios_roles.md` | Roles, accesos, datos de prueba |
| `Contexto/11_reglas_negocio.md` | Estados, flujo, reglas de negocio |
| `Contexto/12_decisiones_diseno.md` | Decisiones de diseño documentadas |
| `Contexto/13_incertidumbres.md` | Incertidumbres, conflictos, recomendaciones |
| `Contexto/14_grafo_relaciones.md` | Grafo completo de relaciones |
| `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` | Script SQL original |
| `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` | Arquitectura de menús |
| `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` | Manual de identidad visual |

Todas las etiquetas de certeza (CONFIRMADO, INFERIDO, PENDIENTE, CONFLICTO) se aplican a lo largo del documento.

---

## 2. PLAN GENERAL

### Enfoque

FixTrack es una aplicación de escritorio interna para TecnoFix Solutions que gestiona el ciclo de vida de reparaciones electrónicas: desde el registro del cliente y su dispositivo, pasando por la creación y seguimiento de órdenes de servicio, la asignación de técnicos, hasta el registro de pagos y la entrega final.

La implementación se basa exclusivamente en la estructura documentada en `Contexto/` y el script SQL del Entregable 1. No se inventan tablas, campos, tablas ni reglas.

El enfoque arquitectónico adoptado es el de **3 capas simples** con separación responsable:

1. **Capa de Presentación** → Formularios WinForms
2. **Capa de Dominio** → Modelos de datos (clases POCO)
3. **Capa de Acceso a Datos** → Componentes ADO.NET

No se incluye una capa de servicios porque la complejidad del sistema no la justifica. Agregar una capa innecesaria dificultaría la explicación en la defensa sin aportar valor académico para este alcance.

### Principios rectores

- **Simplicidad:** código fácil de entender para estudiantes y fácil de defender.
- **Fidelidad al contexto:** toda implementación se basa en lo documentado, sin inventos.
- **Consistencia visual:** todos los formularios respetan la identidad visual definida.
- **Seguridad de datos:** todas las consultas SQL parametrizadas, conexiones en `using`, excepciones manejadas.
- **Diferenciación de certezas:** cada decisión se marca como CONFIRMADO, INFERIDO, PENDIENTE o CONFLICTO.

---

## 3. ARQUITECTURA PROPUESTA

### Justificación de la estructura

Se propone una estructura de 4 carpetas dentro de `APP/`. La separación sigue el principio académico de responsabilidades únicas, pero se evita la sobreingeniería:

```
APP/
├── Formularios/
│   ├── Login/
│   │   └── FrmLogin.cs
│   ├── Dashboard/
│   │   └── FrmDashboard.cs
│   ├── Clientes/
│   │   ├── FrmClientes.cs
│   │   ├── FrmClienteDetalle.cs
│   │   └── FrmClienteFormulario.cs
│   ├── Dispositivos/
│   │   ├── FrmDispositivos.cs
│   │   └── FrmDispositivoFormulario.cs
│   ├── Ordenes/
│   │   ├── FrmOrdenes.cs
│   │   ├── FrmOrdenNueva.cs
│   │   └── FrmOrdenDetalle.cs
│   ├── Pagos/
│   │   ├── FrmPagos.cs
│   │   └── FrmPagoRegistrar.cs
│   ├── Tecnicos/
│   │   ├── FrmTecnicos.cs
│   │   └── FrmTecnicoFormulario.cs
│   ├── Usuarios/
│   │   ├── FrmUsuarios.cs
│   │   └── FrmUsuarioFormulario.cs
│   └── Reportes/
│       └── FrmReportes.cs
├── Modelos/
│   ├── Cliente.cs
│   ├── Dispositivo.cs
│   ├── Tecnico.cs
│   ├── OrdenServicio.cs
│   ├── Pago.cs
│   ├── Usuario.cs
│   └── EstadoOrden.cs (enum)
├── Datos/
│   ├── Conexion.cs
│   ├── ClienteDAL.cs
│   ├── DispositivoDAL.cs
│   ├── TecnicoDAL.cs
│   ├── OrdenServicioDAL.cs
│   ├── PagoDAL.cs
│   ├── UsuarioDAL.cs
│   └── ReportesDAL.cs
├── Recursos/
│   ├── Iconos/           (pendiente de obtener)
│   └── Logo/             (pendiente de obtener)
└── Configuracion/
    └── app.config        (cadena de conexión)
```

### Justificación de cada carpeta

| Carpeta | Propósito | Justificación |
|---------|-----------|---------------|
| `Formularios/` | Todos los formularios WinForms | Agrupados por módulo para facilitar la navegación y el mantenimiento |
| `Modelos/` | Clases POCO que mapean a tablas de la BD | Separación entre datos y lógica de presentación; facilita el paso de datos entre capas |
| `Datos/` | Clases DAL (Data Access Layer) con ADO.NET | Centraliza el acceso a la base de datos; facilita el mantenimiento de conexiones y consultas |
| `Recursos/` | Iconos, logos, imágenes | Separación de recursos visuales del código |
| `Configuracion/` | Archivo de configuración | Cadena de conexión y otros valores fuera del código fuente |

### Por qué NO hay capa de Servicios

- El sistema tiene 6 tablas y una lógica de negocio limitada (10 reglas de negocio).
- Una capa de servicios agregaría complejidad sin justificación académica para este alcance.
- La lógica de negocio puede residir en los DAL o directamente en los formularios, según la simplicidad del caso.
- Si en el futuro el sistema crece, la capa de servicios puede añadirse sin cambiar la estructura actual.

### Por qué NO hay namespace complejo

Se sugiere un namespace único `FixTrack` con sub-namespaces por carpeta:

```
FixTrack
├── FixTrack.Formularios
├── FixTrack.Modelos
├── FixTrack.Datos
└── FixTrack.Recursos
```

Esto simplifica las referencias entre clases y mantiene el código legible.

---

## 4. PLAN DE LA BASE DE DATOS

### 4.1 Cadena de conexión

**Origen:** `Contexto/05_base_de_datos.md` — La base de datos se llama `FixTrack` y se crea a partir del script `Base de Datos APP de escritorio.sql`.

**Estrategia:** La cadena de conexión se almacenará en `Configuracion/app.config` (archivo XML de configuración de .NET). Esto evita hardcodear la cadena en el código.

**Formato probable:**
```xml
<connectionStrings>
  <add name="FixTrackConnection" 
       connectionString="Server=.\SQLEXPRESS;Database=FixTrack;Integrated Security=True;" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

> **PENDIENTE:** El servidor exacto (`.\\SQLEXPRESS`, `localhost`, nombre de instancia) debe confirmarse con el entorno de desarrollo. El nombre de la base de datos es `FixTrack` (CONFIRMADO).

**Clase responsable:** `Datos/Conexion.cs` — Clase estática que expone una propiedad `GetConnectionString()` que lee desde `ConfigurationManager.ConnectionStrings`.

### 4.2 Estrategia ADO.NET

Se utilizará ADO.NET puro con los siguientes componentes:

| Componente | Uso | Clase .NET |
|------------|-----|------------|
| `SqlConnection` | Conexión a SQL Server | `System.Data.SqlClient.SqlConnection` |
| `SqlCommand` | Ejecución de consultas y procedimientos | `System.Data.SqlClient.SqlCommand` |
| `SqlDataReader` | Lectura secuencial de resultados | `System.Data.SqlClient.SqlDataReader` |
| `SqlDataAdapter` | Llenado de DataTables y DataGridViews | `System.Data.SqlClient.SqlDataAdapter` |
| `DataTable` | Almacenamiento temporal de resultados | `System.Data.DataTable` |
| `SqlParameter` | Parámetros en consultas parametrizadas | `System.Data.SqlClient.SqlParameter` |

**Principios:**
- Todas las conexiones dentro de bloques `using` para garantizar cierre automático.
- Todas las consultas parametrizadas con `SqlParameter` para prevenir inyección SQL.
- `CommandType.StoredProcedure` no aplica (no hay stored procedures en la BD).
- `CommandType.Text` para todas las consultas SQL.

### 4.3 Operaciones CRUD

Cada entidad tendrá un DAL con los métodos estándar:

```csharp
// Patrón genérico para cada DAL
public List<T> ObtenerTodos();
public T ObtenerPorId(int id);
public int Insertar(T entidad);
public int Actualizar(T entidad);
public int CambiarEstado(int id, string nuevoEstado);  // Para entidades con baja lógica
```

### 4.4 Consultas de búsqueda y filtros

| Módulo | Búsqueda | Filtros |
|--------|----------|---------|
| Clientes | Por Nombre, Apellido, Teléfono | Estado (Activo/Inactivo) |
| Dispositivos | Por Tipo, Marca, Modelo, Número de serie | — |
| Órdenes | Por OrdenID, Cliente, Dispositivo | Estado (filtro desplegable), rango de fechas |
| Pagos | Por OrdenID, Monto | Método de pago, rango de fechas |
| Técnicos | Por Nombre, Apellido, Especialidad | Estado (Activo/Inactivo) |
| Usuarios | Por NombreUsuario, Rol | Estado (Activo/Inactivo) |
| Reportes | — | Fecha Desde/Hasta, tipo de reporte |

### 4.5 Relaciones en la aplicación

Las relaciones de la BD se manejan en la capa de presentación mediante:

- **ComboBox** para navegación entre entidades relacionadas (ej. seleccionar Cliente al crear Dispositivo).
- **JOINs SQL** para mostrar datos relacionados en DataGridViews (ej. nombre de cliente en listado de órdenes).
- **Subformularios o paneles** para mostrar datos hijos (ej. dispositivos del cliente en FrmClienteDetalle).

### 4.6 Manejo de errores

- Try-catch alrededor de toda operación de base de datos.
- Mensajes al usuario con `MessageBox.Show()` para errores de conexión, validaciones y operaciones fallidas.
- Errores de conexión: mostrar mensaje técnico amigable + log (pendiente definir mecanismo de log).
- Errores de validación: resaltar campos con error (borde rojo según identidad visual).

### 4.7 Transacciones

Se requerirán transacciones en los siguientes escenarios:

| Escenario | Tablas involucradas | Tipo |
|-----------|---------------------|------|
| Crear orden + registrar pago en el mismo flujo | OrdenesServicio, Pagos | SqlTransaction |
| Cambio de estado que implica actualización múltiple | OrdenesServicio | SqlTransaction |
| Crear dispositivo que referencia cliente | Dispositivos | No requiere (FK maneja integridad) |

> **INFERIDO:** Las transacciones se manejarán con `SqlTransaction` explícito dentro de los DAL cuando una operación afecte múltiples tablas.

### 4.8 Consultas SQL existentes

El script SQL incluye consultas de ejemplo que pueden servir como base:

1. Ordenes por estado
2. Detalle de órdenes con joins
3. Órdenes finalizadas
4. Listado de pagos
5. Dispositivos por cliente
6. Órdenes por dispositivo
7. Pagos por orden
8. Órdenes sin técnico
9. Órdenes pendientes
10. Técnicos activos
11. Usuarios con técnicos

Estas consultas pueden adaptarse como base para las operaciones del DAL.

---

## 5. MAPEO BASE DE DATOS ↔ APLICACIÓN

| Módulo | Tabla(s) | Operaciones | Formularios | Estado |
|--------|----------|-------------|-------------|--------|
| **Login** | Usuarios | Autenticación | FrmLogin | CONFIRMADO |
| **Dashboard** | OrdenesServicio | Lectura (métricas, listado) | FrmDashboard | CONFIRMADO |
| **Clientes** | Clientes | CRUD / Cambiar estado | FrmClientes, FrmClienteDetalle, FrmClienteFormulario | CONFIRMADO |
| **Dispositivos** | Dispositivos | CRUD / Cambiar estado | FrmDispositivos, FrmDispositivoFormulario | CONFIRMADO |
| **Órdenes** | OrdenesServicio | Crear, Leer, Actualizar estado/diagnóstico/trabajo/costo | FrmOrdenes, FrmOrdenNueva, FrmOrdenDetalle | CONFIRMADO |
| **Pagos** | Pagos | Crear, Leer | FrmPagos, FrmPagoRegistrar | CONFIRMADO |
| **Técnicos** | Técnicos | CRUD / Cambiar estado | FrmTecnicos, FrmTecnicoFormulario | CONFIRMADO |
| **Usuarios** | Usuarios | CRUD / Cambiar estado / Asignar técnico | FrmUsuarios, FrmUsuarioFormulario | CONFIRMADO |
| **Reportes** | Todas las tablas | Consultas con filtros | FrmReportes | **PENDIENTE DE DECISIÓN** (submenús sin definir) |

**Detalles adicionales por módulo:**

### Clientes
| Operación | Descripción | Fuente |
|-----------|-------------|--------|
| Crear | FrmClienteFormulario con campos Nombre*, Apellido, Teléfono*, Email, Dirección | Mockup 04 |
| Listar/Buscar | FrmClientes con DataGridView, campo Buscar | Mockup 03 |
| Detalle | FrmClienteDetalle con información completa + dispositivos del cliente | Mockup 05 |
| Cambiar estado | Botón en FrmClientes lista → baja lógica (Estado: Activo → Inactivo) | Mockup 03, Confirmado en 11_reglas_negocio |
| Editar | FrmClienteFormulario pre-cargado con datos del cliente seleccionado | Mockup 04 |

### Dispositivos
| Operación | Descripción | Fuente |
|-----------|-------------|--------|
| Crear | FrmDispositivoFormulario con ComboBox Cliente*, Tipo*, Marca, Modelo, Número serie, Descripción | Mockup 07 |
| Listar/Buscar | FrmDispositivos con DataGridView, campo Buscar | Mockup 06 |
| Editar | FrmDispositivoFormulario pre-cargado | Mockup 07 |
| Cambiar estado | Botón en FrmDispositivos lista → baja lógica | Mockup 06, Confirmado en 11_reglas_negocio |

### Órdenes de servicio
| Operación | Descripción | Fuente |
|-----------|-------------|--------|
| Crear | FrmOrdenNueva: Dispositivo*, Problema reportado*, Estado=Pendiente (fijo), Técnico (opcional), Costo, Observaciones | Mockup 09 |
| Listar/Filtrar | FrmOrdenes con DataGridView, campo Buscar, filtro Estado (ComboBox "Todos los estados") | Mockup 08 |
| Detalle | FrmOrdenDetalle: seguimiento (diagnóstico, trabajo, costo), pagos asociados, botón Registrar pago | Mockup 10 |
| Actualizar estado | En FrmOrdenDetalle: avanzar estado | Mockup 10 |
| Actualizar diagnóstico/trabajo/costo | En FrmOrdenDetalle | Mockup 10 |

### Pagos
| Operación | Descripción | Fuente |
|-----------|-------------|--------|
| Listar/Buscar | FrmPagos con DataGridView, campo Buscar | Mockup 12 |
| Registrar | FrmPagoRegistrar: OrdenID, Monto*, Método* (Efectivo/Tarjeta/Transferencia), Fecha, Observaciones | Mockup 12 |
| Ver en detalle de orden | Tabla de pagos integrada en FrmOrdenDetalle | Mockup 10 |

### Técnicos
| Operación | Descripción | Fuente |
|-----------|-------------|--------|
| Crear | FrmTecnicoFormulario con Nombre*, Apellido*, Teléfono, Especialidad | Mockup 11 |
| Listar/Buscar | FrmTecnicos con DataGridView, campo Buscar | Mockup 11 |
| Editar | FrmTecnicoFormulario pre-cargado | Mockup 11 |
| Cambiar estado | Botón en FrmTecnicos lista → Activo/Inactivo | Mockup 11 |

### Usuarios
| Operación | Descripción | Fuente |
|-----------|-------------|--------|
| Crear | FrmUsuarioFormulario con NombreUsuario*, Password, Rol*, Estado, Técnico asociado (ComboBox) | Mockup 14 |
| Listar/Buscar | FrmUsuarios con DataGridView, campo Buscar | Mockup 14 |
| Editar | FrmUsuarioFormulario pre-cargado | Mockup 14 |
| Cambiar estado | Botón en FrmUsuarios lista → Activo/Inactivo | Mockup 14 |
| Asignar técnico | ComboBox Técnico asociado en FrmUsuarioFormulario (solo si Rol = Técnico) | Mockup 14 |

### Reportes
| Operación | Descripción | Fuente |
|-----------|-------------|--------|
| Generar | Seleccionar tipo de reporte, rango de fechas (Desde/Hasta), botón Generar | Mockup 13 |
| Exportar | Botón Exportar en FrmReportes | Mockup 13 |
| Ver resultados | Tabla con columnas: Estado, Cantidad de órdenes, Subtotal | Mockup 13 |

> **CONFLICTO / PENDIENTE:** El reporte oficial "Órdenes por estado" tiene columnas de cantidad y subtotal, pero el subtotal en el mockup muestra "$0.00" para Listo sin tener una cantidad, lo cual es inconsistente. El comportamiento exacto de totales requiere decisión.

---

## 6. PLAN DE FORMULARIOS

### 6.1 FrmLogin

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Login/FrmLogin.cs` |
| Propósito | Autenticar al usuario e identificar su rol |
| Módulo | Login |
| Controles principales | TextBox (NombreUsuario), TextBox (Password, PasswordChar='*'), Button (Iniciar sesión), Label (TecnoFix Solutions) |
| Datos que muestra | Campo de usuario, campo contraseña |
| Datos que recibe | NombreUsuario y Password del usuario |
| Operaciones | Validar credenciales contra tabla Usuarios, establecer sesión activa |
| Eventos principales | Click en "Iniciar sesión", Load del formulario |
| Navegación de entrada | Pantalla inicial de la aplicación |
| Navegación de salida | FrmDashboard (éxito) |
| Validaciones | NombreUsuario y Password obligatorios; mensaje "Credenciales inválidas" en caso de error |
| Consultas SQL | `SELECT UsuarioID, NombreUsuario, PasswordHash, Rol, TecnicoID, Estado FROM Usuarios WHERE NombreUsuario = @NombreUsuario` |
| Restricciones por rol | N/A (se ejecuta antes de determinar el rol) |
| Dependencia | Ninguna |

> **CONFIRMADO:** El login muestra "Credenciales inválidas" / "Verifique usuario y contraseña" en caso de error. (Mockup 01)

### 6.2 FrmDashboard

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Dashboard/FrmDashboard.cs` |
| Propósito | Pantalla de resumen del estado del negocio |
| Módulo | Dashboard |
| Controles principales | Labels (5 métricas), DataGridView (órdenes recientes), GroupBox (contendores) |
| Datos que muestra | 5 indicadores por estado (Pendientes, En diagnóstico, En reparación, Listos, Entregados), tabla de órdenes recientes |
| Datos que recibe | Ninguno (carga automática al abrirse) |
| Operaciones | Consultar métricas y órdenes recientes |
| Eventos principales | Load |
| Navegación de entrada | FrmLogin (tras autenticación exitosa) |
| Navegación de salida | Menú lateral (todos los módulos), Cerrar sesión |
| Validaciones | Ninguna (solo lectura) |
| Consultas SQL | 5 COUNT por estado, SELECT con JOIN de órdenes recientes |
| Restricciones por rol | Todos los roles ven el Dashboard |
| Dependencia | FrmLogin |

> **CONFIRMADO:** 5 métricas (no 3 como indicaba la arquitectura). (Mockup 02)

### 6.3 FrmClientes (listado)

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Clientes/FrmClientes.cs` |
| Propósito | Listar, buscar y gestionar clientes |
| Módulo | Clientes (Operación) |
| Controles principales | DataGridView, TextBox (Buscar), Button (Nuevo cliente), Button (Cambiar estado), Button (Buscar) |
| Datos que muestra | ClienteID, Nombre, Apellido, Teléfono, Email, Estado |
| Datos que recibe | Selección de fila en DataGridView |
| Operaciones | Buscar, crear nuevo, ver detalle, cambiar estado |
| Eventos principales | Click en Nuevo, Click en Cambiar estado, DoubleClick en fila (detalle), TextChanged en Buscar |
| Navegación de entrada | Menú lateral |
| Navegación de salida | FrmClienteDetalle, FrmClienteFormulario |
| Validaciones | Ninguna en listado |
| Consultas SQL | SELECT ClienteID, Nombre, Apellido, Telefono, Email, Estado FROM Clientes WHERE Nombre LIKE @Buscar OR Apellido LIKE @Buscar |
| Restricciones por rol | Administrador y Empleado/Recepcionista |
| Dependencia | FrmLogin, Dashboard |

### 6.4 FrmClienteDetalle

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Clientes/FrmClienteDetalle.cs` |
| Propósito | Ver información completa del cliente y sus dispositivos |
| Módulo | Clientes (Operación) |
| Controles principales | Labels (información del cliente), DataGridView (dispositivos del cliente), Button (Editar cliente), Button (Nuevo dispositivo) |
| Datos que muestra | Nombre, Apellido, Estado, Teléfono, Email, Dirección + tabla de dispositivos (DispositivoID, Tipo, Marca, Modelo, Número de serie, FechaRegistro) |
| Datos que recibe | ClienteID seleccionado |
| Operaciones | Ver información completa, ver dispositivos, editar cliente, crear nuevo dispositivo |
| Eventos principales | Load, Click en Editar cliente, Click en Nuevo dispositivo |
| Navegación de entrada | FrmClientes (DoubleClick o botón Detalle) |
| Navegación de salida | FrmClientes, FrmDispositivoFormulario |
| Validaciones | Ninguna (solo lectura) |
| Consultas SQL | SELECT del cliente + SELECT de dispositivos WHERE ClienteID = @ClienteID |
| Restricciones por rol | Administrador y Empleado/Recepcionista |
| Dependencia | FrmClientes |

### 6.5 FrmClienteFormulario

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Clientes/FrmClienteFormulario.cs` |
| Propósito | Crear o editar un cliente |
| Módulo | Clientes (Operación) |
| Controles principales | TextBox (Nombre*), TextBox (Apellido), TextBox (Teléfono*), TextBox (Email), TextBox (Dirección), ComboBox (Estado), Button (Guardar), Button (Cancelar) |
| Datos que muestra | Campos del cliente (pre-cargados al editar) |
| Datos que recibe | Todos los campos del cliente |
| Operaciones | Insertar o UPDATE de cliente |
| Eventos principales | Click en Guardar, Click en Cancelar |
| Navegación de entrada | FrmClientes (Nuevo) o FrmClienteDetalle (Editar) |
| Navegación de salida | FrmClientes |
| Validaciones | Nombre obligatorio, Teléfono obligatorio; Email opcional pero debe ser formato válido si se ingresa; Estado = Activo por defecto |
| Consultas SQL | INSERT INTO Clientes (Nombre, Apellido, Telefono, Email, Direccion, Estado, FechaRegistro) VALUES (...) / UPDATE Clientes SET ... WHERE ClienteID = @ClienteID |
| Restricciones por rol | Administrador y Empleado/Recepcionista |
| Dependencia | FrmClientes |

### 6.6 FrmDispositivos (listado)

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Dispositivos/FrmDispositivos.cs` |
| Propósito | Listar, buscar y gestionar dispositivos |
| Módulo | Dispositivos (Operación) |
| Controles principales | DataGridView, TextBox (Buscar), Button (Nuevo dispositivo), Button (Cambiar estado) |
| Datos que muestra | DispositivoID, Cliente, Tipo, Marca, Modelo, Número de serie |
| Datos que recibe | Selección de fila en DataGridView |
| Operaciones | Buscar, crear nuevo, cambiar estado, ver detalle |
| Eventos principales | Click en Nuevo, Click en Cambiar estado, DoubleClick en fila |
| Navegación de entrada | Menú lateral |
| Navegación de salida | FrmDispositivoFormulario |
| Validaciones | Ninguna en listado |
| Consultas SQL | SELECT con JOIN a Clientes para mostrar nombre del cliente |
| Restricciones por rol | Administrador y Empleado/Recepcionista |
| Dependencia | FrmLogin, Dashboard |

### 6.7 FrmDispositivoFormulario

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Dispositivos/FrmDispositivoFormulario.cs` |
| Propósito | Crear o editar un dispositivo |
| Módulo | Dispositivos (Operación) |
| Controles principales | ComboBox (Cliente*), ComboBox/TXT (Tipo*), TextBox (Marca), TextBox (Modelo), TextBox (Número de serie), TextBox multilinea (Descripción), Button (Guardar), Button (Cancelar) |
| Datos que muestra | Campos del dispositivo (pre-cargados al editar) |
| Datos que recibe | ClienteID, Tipo, Marca, Modelo, NumeroSerie, Descripcion |
| Operaciones | Insertar o UPDATE de dispositivo |
| Eventos principales | Click en Guardar, Click en Cancelar |
| Navegación de entrada | FrmDispositivos (Nuevo) o desde FrmClienteDetalle |
| Navegación de salida | FrmDispositivos |
| Validaciones | Cliente obligatorio (ComboBox), Tipo obligatorio; Número de serie opcional; Descripción opcional |
| Consultas SQL | INSERT INTO Dispositivos (ClienteID, Tipo, Marca, Modelo, NumeroSerie, Descripcion, FechaRegistro) VALUES (...) / UPDATE Dispositivos SET ... WHERE DispositivoID = @DispositivoID |
| Restricciones por rol | Administrador y Empleado/Recepcionista |
| Dependencia | FrmDispositivos, FrmClientes (para cargar ComboBox) |

### 6.8 FrmOrdenes (listado)

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Ordenes/FrmOrdenes.cs` |
| Propósito | Listar, buscar y filtrar órdenes de servicio |
| Módulo | Órdenes de servicio (Operación) |
| Controles principales | DataGridView, TextBox (Buscar), ComboBox (Filtro estado: "Todos los estados"), Button (Nueva orden), Button (Ver detalle) |
| Datos que muestra | OrdenID, Cliente, Dispositivo, Técnico, Fecha ingreso, Estado, Costo |
| Datos que recibe | Selección de fila; valor de filtro de estado |
| Operaciones | Buscar, filtrar por estado, crear nueva orden, ver detalle, actualizar estado |
| Eventos principales | Click en Nueva orden, SelectionChange en filtro estado, DoubleClick en fila (detalle) |
| Navegación de entrada | Menú lateral |
| Navegación de salida | FrmOrdenNueva, FrmOrdenDetalle |
| Validaciones | Ninguna en listado |
| Consultas SQL | SELECT con JOINs a Clientes, Dispositivos, Técnicos; ORDER BY Estado, FechaIngreso; WHERE Estado = @Estado (si filtro activo) |
| Restricciones por rol | Administrador y Empleado/Recepcionista |
| Dependencia | FrmLogin, Dashboard |

### 6.9 FrmOrdenNueva

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Ordenes/FrmOrdenNueva.cs` |
| Propósito | Crear una nueva orden de servicio |
| Módulo | Órdenes de servicio (Operación) |
| Controles principales | ComboBox (Dispositivo*), ComboBox (Técnico — opcional), DateTimePicker (Fecha de ingreso*), TextBox multilinea (Problema reportado*), Label (Estado = Pendiente, solo lectura), NumericUpDown o TextBox (Costo del servicio), TextBox multilinea (Observaciones), Button (Guardar orden), Button (Cancelar) |
| Datos que muestra | Campos del formulario vacíos (o precargados) |
| Datos que recibe | DispositivoID, TecnicoID, FechaIngreso, ProblemaReportado, CostoServicio, Observaciones |
| Operaciones | INSERT en OrdenesServicio con Estado = 'Pendiente' |
| Eventos principales | Click en Guardar orden, Click en Cancelar |
| Navegación de entrada | FrmOrdenes (Nueva orden) |
| Navegación de salida | FrmOrdenes |
| Validaciones | Dispositivo obligatorio, Problema reportado obligatorio; Costo ≥ 0 (CK_OrdenesServicio_Costo); FechaIngreso no posterior a hoy |
| Consultas SQL | INSERT INTO OrdenesServicio (DispositivoID, TecnicoID, FechaIngreso, ProblemaReportado, Estado, CostoServicio, Observaciones) VALUES (...) |
| Restricciones por rol | Administrador y Empleado/Recepcionista |
| Dependencia | FrmOrdenes, FrmDispositivos (para cargar ComboBox) |

> **CONFIRMADO:** El campo Estado se fija automáticamente al crear la orden con valor 'Pendiente'. (Mockup 09, Contexto)

### 6.10 FrmOrdenDetalle

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Ordenes/FrmOrdenDetalle.cs` |
| Propósito | Ver y gestionar el detalle completo de una orden |
| Módulo | Órdenes de servicio (Operación) |
| Controles principales | Labels (información de la orden), TextBox/Label (Problema reportado, Diagnóstico, Trabajo realizado, Estado, Costo), DateTimePicker (Fecha finalización — solo lectura), TextBox (Observaciones), DataGridView (pagos de la orden), Button (Registrar pago), Button (Guardar cambios en diagnóstico/trabajo/costo), Button (Cambiar estado), Button (Cancelar) |
| Datos que muestra | Cliente, Dispositivo, Estado, Técnico, Fecha ingreso, Problema reportado, Diagnóstico, Trabajo realizado, Estado, Costo, Fecha finalización, Observaciones + tabla de pagos (PagoID, Fecha, Monto, Método) |
| Datos que recibe | OrdenID |
| Operaciones | Ver seguimiento, actualizar diagnóstico/trabajo realizado/costo, registrar pago, avanzar estado |
| Eventos principales | Load, Click en Guardar cambios, Click en Cambiar estado, Click en Registrar pago |
| Navegación de entrada | FrmOrdenes (DoubleClick o botón Ver detalle) |
| Navegación de salida | FrmOrdenes |
| Validaciones | Costo ≥ 0; Monto de pago > 0 al registrar pago; Diagnóstico y Trabajo opcionales |
| Consultas SQL | SELECT de orden con JOINs + SELECT de pagos WHERE OrdenID = @OrdenID + UPDATE de diagnóstico/trabajo/costo + INSERT de pago |
| Restricciones por rol | Administrador y Empleado/Recepcionista |
| Dependencia | FrmOrdenes, FrmPagoRegistrar |

> **CONFLICTO:** El mockup integra la tabla de pagos dentro del detalle de orden (Mockup 10), pero la arquitectura define Pagos como módulo aparte. Ambos enfoques pueden coexistir: el botón "Registrar pago" navega a FrmPagoRegistrar o registra directamente desde el detalle. **PENDIENTE DE DECISIÓN.**

### 6.11 FrmPagos (listado)

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Pagos/FrmPagos.cs` |
| Propósito | Listar y buscar pagos |
| Módulo | Pagos (Operación) |
| Controles principales | DataGridView, TextBox (Buscar), Button (Registrar pago) |
| Datos que muestra | PagoID, Orden, Cliente, Fecha, Monto, Método |
| Datos que recibe | Selección de fila |
| Operaciones | Buscar, registrar nuevo pago, ver detalle |
| Eventos principales | Click en Registrar pago, DoubleClick en fila |
| Navegación de entrada | Menú lateral |
| Navegación de salida | FrmPagoRegistrar |
| Validaciones | Ninguna en listado |
| Consultas SQL | SELECT con JOINs a OrdenesServicio y Clientes |
| Restricciones por rol | Administrador y Empleado/Recepcionista |
| Dependencia | FrmLogin, Dashboard |

### 6.12 FrmPagoRegistrar

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Pagos/FrmPagoRegistrar.cs` |
| Propósito | Registrar un nuevo pago asociado a una orden |
| Módulo | Pagos (Operación) |
| Controles principales | ComboBox (OrdenID*), NumericUpDown (Monto*), ComboBox (Método*: Efectivo/Tarjeta/Transferencia), DateTimePicker (Fecha), TextBox (Observaciones), Button (Guardar), Button (Cancelar) |
| Datos que muestra | Formulario vacío |
| Datos que recibe | OrdenID, Monto, MetodoPago, FechaPago, Observaciones |
| Operaciones | INSERT en Pagos |
| Eventos principales | Click en Guardar, Click en Cancelar |
| Navegación de entrada | FrmPagos (Registrar pago) o FrmOrdenDetalle |
| Navegación de salida | FrmPagos o FrmOrdenDetalle |
| Validaciones | Orden obligatorio, Monto > 0 (CK_Pagos_Monto), Método obligatorio; Monto puede necesitar validación de que no excede el costo de la orden (PENDIENTE) |
| Consultas SQL | INSERT INTO Pagos (OrdenID, FechaPago, Monto, MetodoPago, Observaciones) VALUES (...) |
| Restricciones por rol | Administrador y Empleado/Recepcionista |
| Dependencia | FrmPagos, FrmOrdenes |

### 6.13 FrmTecnicos (listado)

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Técnicos/FrmTecnicos.cs` |
| Propósito | Listar y gestionar técnicos |
| Módulo | Técnicos (Administración) |
| Controles principales | DataGridView, TextBox (Buscar), Button (Nuevo técnico), Button (Cambiar estado) |
| Datos que muestra | TécnicoID, Nombre, Apellido, Teléfono, Especialidad, Estado |
| Datos que recibe | Selección de fila |
| Operaciones | Buscar, crear nuevo, editar, cambiar estado |
| Eventos principales | Click en Nuevo técnico, Click en Cambiar estado |
| Navegación de entrada | Menú lateral |
| Navegación de salida | FrmTecnicoFormulario |
| Validaciones | Ninguna en listado |
| Consultas SQL | SELECT de Técnicos |
| Restricciones por rol | Solo Administrador (INFERIDO) |
| Dependencia | FrmLogin, Dashboard |

### 6.14 FrmTecnicoFormulario

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Técnicos/FrmTecnicoFormulario.cs` |
| Propósito | Crear o editar un técnico |
| Módulo | Técnicos (Administración) |
| Controles principales | TextBox (Nombre*), TextBox (Apellido*), TextBox (Teléfono), TextBox (Especialidad), ComboBox (Estado: Activo/Inactivo), Button (Guardar), Button (Cancelar) |
| Datos que muestra | Campos del técnico (pre-cargados al editar) |
| Datos que recibe | Nombre, Apellido, Teléfono, Especialidad, Estado |
| Operaciones | Insertar o UPDATE de técnico |
| Eventos principales | Click en Guardar, Click en Cancelar |
| Navegación de entrada | FrmTecnicos (Nuevo) o FrmTecnicos (Editar) |
| Navegación de salida | FrmTecnicos |
| Validaciones | Nombre obligatorio, Apellido obligatorio |
| Consultas SQL | INSERT INTO Técnicos (Nombre, Apellido, Telefono, Especialidad, Estado) VALUES (...) / UPDATE Técnicos SET ... WHERE TecnicoID = @TecnicoID |
| Restricciones por rol | Solo Administrador (INFERIDO) |
| Dependencia | FrmTecnicos |

### 6.15 FrmUsuarios (listado)

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Usuarios/FrmUsuarios.cs` |
| Propósito | Listar y gestionar usuarios del sistema |
| Módulo | Usuarios (Administración) |
| Controles principales | DataGridView, TextBox (Buscar), Button (Nuevo usuario), Button (Cambiar estado) |
| Datos que muestra | UsuarioID, Nombre de usuario, Nombre, Rol, Estado, Técnico asociado |
| Datos que recibe | Selección de fila |
| Operaciones | Buscar, crear nuevo, editar, cambiar estado, asignar técnico |
| Eventos principales | Click en Nuevo usuario, Click en Cambiar estado |
| Navegación de entrada | Menú lateral |
| Navegación de salida | FrmUsuarioFormulario |
| Validaciones | Ninguna en listado |
| Consultas SQL | SELECT con JOIN a Técnicos para mostrar Técnico asociado |
| Restricciones por rol | Solo Administrador (INFERIDO) |
| Dependencia | FrmLogin, Dashboard |

### 6.16 FrmUsuarioFormulario

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Usuarios/FrmUsuarioFormulario.cs` |
| Propósito | Crear o editar un usuario y asignar rol/técnico |
| Módulo | Usuarios (Administración) |
| Controles principales | TextBox (NombreUsuario*), TextBox (Password*), ComboBox (Rol*: Administrador/Empleado/Técnico), ComboBox (Estado: Activo/Inactivo), ComboBox (Técnico asociado — solo visible si Rol = Técnico), Button (Guardar), Button (Cancelar) |
| Datos que muestra | Campos del usuario (pre-cargados al editar) |
| Datos que recibe | NombreUsuario, PasswordHash, Rol, Estado, TecnicoID |
| Operaciones | Insertar o UPDATE de usuario |
| Eventos principales | Click en Guardar, Click en Cancelar, SelectionChange en ComboBox Rol (mostrar/ocultar Técnico asociado) |
| Navegación de entrada | FrmUsuarios (Nuevo) o FrmUsuarios (Editar) |
| Navegación de salida | FrmUsuarios |
| Validaciones | NombreUsuario obligatorio (único), Password obligatorio, Rol obligatorio; Si Rol = Técnico, Técnico asociado obligatorio; NombreUsuario único (validar contra BD) |
| Consultas SQL | INSERT INTO Usuarios (NombreUsuario, PasswordHash, Rol, Estado, TecnicoID) VALUES (...) / UPDATE Usuarios SET ... WHERE UsuarioID = @UsuarioID |
| Restricciones por rol | Solo Administrador (INFERIDO) |
| Dependencia | FrmUsuarios, FrmTecnicos (para cargar ComboBox de técnicos) |

> **CONFLICTO:** El mockup muestra el rol como "Empleado Recepcionista" pero la BD y la arquitectura usan "Empleado". El Combobox debe mostrar "Empleado" (valor almacenado en BD) pero se puede considerar mostrar "Empleado / Recepcionista" como texto descriptivo. **PENDIENTE DE DECISIÓN.**

### 6.17 FrmReportes

| Atributo | Valor |
|----------|-------|
| Nombre | `Formularios/Reportes/FrmReportes.cs` |
| Propósito | Panel de consulta y generación de reportes |
| Módulo | Reportes (Información) |
| Controles principales | ComboBox (Tipo de reporte), DateTimePicker (Desde), DateTimePicker (Hasta), Button (Generar), Button (Exportar), DataGridView (resultados), Label (subtotales) |
| Datos que muestra | Tabla de resultados con columnas: Estado, Cantidad de órdenes, Subtotal |
| Datos que recibe | Tipo de reporte, rango de fechas |
| Operaciones | Generar reporte, exportar |
| Eventos principales | Click en Generar, Click en Exportar |
| Navegación de entrada | Menú lateral |
| Navegación de salida | Menú lateral |
| Validaciones | Fecha Desde ≤ Fecha Hasta |
| Consultas SQL | SELECT Estado, COUNT(*) as Cantidad, SUM(CostoServicio) as Subtotal FROM OrdenesServicio WHERE FechaIngreso BETWEEN @Desde AND @Hasta GROUP BY Estado |
| Restricciones por rol | Todos los roles (INFERIDO) |
| Dependencia | FrmLogin, Dashboard |

> **PENDIENTE DE DECISIÓN:** Los reportes oficiales definidos en la identidad visual son 4: (1) Órdenes por estado, (2) Órdenes por técnico, (3) Servicios completados, (4) Pagos registrados. Los submenús específicos dentro de Reportes no están definidos. El mockup muestra un solo formulario con filtros.

> **PENDIENTE DE DECISIÓN:** La exportación no está definida en el contexto (formato CSV, Excel, PDF). Se sugiere implementar exportación a CSV como opción básica.

---

## 7. PLAN DE NAVEGACIÓN

### Flujo completo

```
INICIO
  │
  ▼
┌──────────────┐
│  FrmLogin    │◄──────────────────────┐
│  (credencial)│                        │
└──────┬───────┘                        │
       │                                │
  Éxito │ Fail                           │
       ▼                                │
┌──────────────┐                        │
│  FrmDashboard│                        │
│  (Panel      │                        │
│   control)   │                        │
└──────┬───────┘                        │
       │                                │
       ▼                                │
┌──────────────┐                        │
│ MenuStrip    │                        │
│ lateral      │                        │
└──┬───┬───┬──┘                        │
   │   │   │                            │
   ▼   ▼   ▼                            │
 OPERACIÓN  ADMINISTRACIÓN  INFORMACIÓN   │
   │          │                │          │
   ├─ Clientes ├─ Técnicos ──┤ Reportes │
   ├─ Dispositivos├─ Usuarios──│          │
   ├─ Órdenes    │            │          │
   └─ Pagos      │            │          │
       │          │            │          │
       ▼          ▼            ▼          │
┌─────────────────────────────────────┐  │
│  Pantallas de gestión               │  │
│  (FrmXXX formularios)               │◄─┘
│  • Listados                         │
│  • Formularios Crear/Editar         │
│  • Detalles                         │
│  • Registros                        │
│                                     │
└─────────────────────────────────────┘
       │
       ▼
┌──────────────┐
│ Cerrar sesión│
│ (FrmLogin)   │
└──────────────┘
```

### Flujo detallado por formulario

| Formulario | Entrada | Salida | Condición |
|------------|---------|--------|-----------|
| FrmLogin | Inicio de aplicación | FrmDashboard (éxito) | Credenciales válidas |
| FrmLogin | FrmDashboard (cerrar sesión) | FrmLogin | Usuario hace clic en Cerrar sesión |
| FrmDashboard | FrmLogin (éxito) | Menú lateral | — |
| FrmClientes | Menú lateral → Clientes | FrmClienteDetalle, FrmClienteFormulario | — |
| FrmClienteDetalle | FrmClientes (DoubleClick) | FrmClientes, FrmDispositivoFormulario | Botón "Nuevo dispositivo" |
| FrmClienteFormulario | FrmClientes (Nuevo) o FrmClienteDetalle (Editar) | FrmClientes | Guardar o Cancelar |
| FrmDispositivos | Menú lateral → Dispositivos | FrmDispositivoFormulario | — |
| FrmDispositivoFormulario | FrmDispositivos (Nuevo) o FrmClienteDetalle (Nuevo dispositivo) | FrmDispositivos | Guardar o Cancelar |
| FrmOrdenes | Menú lateral → Órdenes | FrmOrdenNueva, FrmOrdenDetalle | — |
| FrmOrdenNueva | FrmOrdenes (Nueva orden) | FrmOrdenes | Guardar o Cancelar |
| FrmOrdenDetalle | FrmOrdenes (DoubleClick) | FrmOrdenes | — |
| FrmPagos | Menú lateral → Pagos | FrmPagoRegistrar | — |
| FrmPagoRegistrar | FrmPagos (Registrar) o FrmOrdenDetalle | FrmPagos o FrmOrdenDetalle | Guardar o Cancelar |
| FrmTecnicos | Menú lateral → Técnicos (Admin) | FrmTecnicoFormulario | Solo Admin |
| FrmTecnicoFormulario | FrmTecnicos (Nuevo/Editar) | FrmTecnicos | Guardar o Cancelar |
| FrmUsuarios | Menú lateral → Usuarios (Admin) | FrmUsuarioFormulario | Solo Admin |
| FrmUsuarioFormulario | FrmUsuarios (Nuevo/Editar) | FrmUsuarios | Guardar o Cancelar |
| FrmReportes | Menú lateral → Reportes | FrmReportes (permanece) | — |

### Navegación por rol

| Rol | Menú disponible | Formularios accesibles |
|-----|----------------|----------------------|
| **Administrador** | Inicio · Clientes · Dispositivos · Órdenes · Pagos · Técnicos · Usuarios · Reportes · Cerrar sesión | Todos los formularios |
| **Empleado / Recepcionista** | Inicio · Clientes · Dispositivos · Órdenes · Pagos · Reportes · Cerrar sesión | Todos excepto Técnicos y Usuarios |
| **Técnico** | Inicio · Mis órdenes · Actualizar servicio · Cerrar sesión | Dashboard (filtrado), FrmOrdenDetalle (solo las suyas) |

> **INFERIDO:** El rol Técnico accede a "Mis órdenes" y "Actualizar servicio" que se implementan como versiones filtradas de FrmOrdenes y FrmOrdenDetalle. La implementación exacta de estos menús requiere decisión: pueden ser formularios separados o variaciones de los formularios existentes con filtros.

> **PENDIENTE:** El menú del técnico ("Mis órdenes", "Actualizar servicio") no tiene formularios dedicados documentados en los mockups. Se infiere que son variaciones con filtros de los formularios existentes.

### Comportamiento de cierre de sesión

- Al hacer clic en "Cerrar sesión", se cierra la sesión actual.
- Se oculta el formulario actual y se muestra FrmLogin.
- No se cierra la aplicación (se reutiliza).

---

## 8. PLAN DE AUTENTICACIÓN Y ROLES

### 8.1 Login

| Aspecto | Detalle | Estado |
|---------|---------|--------|
| Campo usuario | `NombreUsuario` (NVARCHAR(50), único en BD) | CONFIRMADO |
| Campo contraseña | `PasswordHash` (NVARCHAR(256)) | CONFIRMADO |
| Validación | Consultar Usuarios WHERE NombreUsuario = @usr, verificar hash | CONFIRMADO |
| Error | MessageBox con "Credenciales inválidas" / "Verifique usuario y contraseña" | CONFIRMADO |
| Éxito | Almacenar UsuarioID, Rol, TecnicoID en sesión | INFERIDO |
| Sesión | Variable estática o propiedad en FrmDashboard | INFERIDO |

### 8.2 Identificación del usuario y rol

Tras el login exitoso, se almacenan en una clase de sesión estática:

```csharp
public static class Sesion
{
    public static int UsuarioID { get; set; }
    public static string NombreUsuario { get; set; }
    public static string Rol { get; set; }
    public static int? TecnicoID { get; set; }
    public static bool EsAdministrador => Rol == "Administrador";
    public static bool EsEmpleado => Rol == "Empleado";
    public static bool EsTecnico => Rol == "Tecnico";
}
```

### 8.3 Acceso a módulos por rol

| Módulo | Administrador | Empleado/Recepcionista | Técnico | Estado |
|--------|:---:|:---:|:---:|--------|
| Dashboard | ✅ | ✅ | ✅ | CONFIRMADO |
| Clientes | ✅ | ✅ | ❌ | CONFIRMADO |
| Dispositivos | ✅ | ✅ | ❌ | CONFIRMADO |
| Órdenes | ✅ | ✅ | ❌ | CONFIRMADO |
| Pagos | ✅ | ✅ | ❌ | CONFIRMADO |
| Técnicos | ✅ | ❌ | ❌ | INFERIDO |
| Usuarios | ✅ | ❌ | ❌ | INFERIDO |
| Reportes | ✅ | ✅ | ✅ | INFERIDO |
| Mis órdenes | ❌ | ❌ | ✅ | INFERIDO |
| Actualizar servicio | ❌ | ❌ | ✅ | INFERIDO |

### 8.4 Restricciones del Técnico

- El técnico solo puede ver y actualizar las órdenes que le están asignadas.
- El filtro se implementa con `WHERE TecnicoID = @TecnicoID` en las consultas.
- El técnico puede actualizar: Diagnóstico, Trabajo realizado, Estado (a "En diagnóstico" o "En reparación"), Costo.
- El técnico NO puede: crear órdenes, gestionar clientes/dispositivos, gestionar usuarios/técnicos.

> **PENDIENTE:** ¿Cómo se identifica el técnico en el filtro? Por `Sesion.TecnicoID` (mapeado desde Usuarios.TecnicoID). Esto está soportado por la estructura de la BD.

### 8.5 Restricciones del Empleado/Recepcionista

- Puede crear órdenes, gestionar clientes/dispositivos, registrar pagos, generar reportes.
- NO puede gestionar técnicos ni usuarios.

### 8.6 Contradicciones sobre roles

> **CONFLICTO #4:** El nombre del rol en la arquitectura es "Empleado" pero en el mockup de Usuarios se muestra como "Empleado Recepcionista". La BD usa `VARCHAR(30)` con check constraint `'Administrador', 'Empleado', 'Tecnico'`. El valor almacenado debe ser "Empleado" (según la BD). Se recomienda mostrar "Empleado" en el Combobox pero documentar que corresponde a "Empleado / Recepcionista".

> **PENDIENTE:** Confirmar si "Empleado Recepcionista" es solo una etiqueta visual para "Empleado" o si requiere un valor diferente en la BD.

---

## 9. PLAN DE CADA CRUD

### 9.1 Clientes

#### Crear
- **Formulario:** FrmClienteFormulario
- **Validaciones:** Nombre obligatorio, Teléfono obligatorio (NVARCHAR(20)), Email opcional (formato válido si se ingresa), Dirección opcional
- **Parámetros:** Nombre, Apellido, Telefono, Email, Direccion, Estado='Activo', FechaRegistro=GETDATE()
- **INSERT:** `INSERT INTO Clientes (Nombre, Apellido, Telefono, Email, Direccion, Estado, FechaRegistro) VALUES (@Nombre, @Apellido, @Telefono, @Email, @Direccion, @Estado, @FechaRegistro)`
- **Mensaje:** "Cliente registrado exitosamente" o "Error al registrar el cliente"

#### Consultar
- **SELECT:** `SELECT ClienteID, Nombre, Apellido, Telefono, Email, Estado FROM Clientes WHERE Nombre LIKE @Buscar OR Apellido LIKE @Buscar ORDER BY Nombre`
- **DataGridView:** ReadOnly, FullRowSelect, sin RowHeadersVisible
- **Filtros:** Campo Buscar (nombre o apellido), filtro Estado (ComboBox: Todos, Activo, Inactivo)
- **Relaciones:** En FrmClienteDetalle, consultar dispositivos con JOIN: `SELECT d.*, c.Nombre as NombreCliente FROM Dispositivos d INNER JOIN Clientes c ON d.ClienteID = c.ClienteID WHERE d.ClienteID = @ClienteID`

#### Modificar
- **Formulario:** FrmClienteFormulario (pre-cargado con datos del ClienteID seleccionado)
- **Identificación:** ClienteID desde la fila seleccionada en FrmClientes
- **Parámetros:** Todos los campos excepto ClienteID y FechaRegistro
- **UPDATE:** `UPDATE Clientes SET Nombre=@Nombre, Apellido=@Apellido, Telefono=@Telefono, Email=@Email, Direccion=@Direccion, Estado=@Estado WHERE ClienteID=@ClienteID`
- **Validaciones:** Mismas que creación

#### Cambiar estado (baja lógica)
- **Operación:** UPDATE del campo Estado de Activo a Inactivo (o viceversa)
- **UPDATE:** `UPDATE Clientes SET Estado = CASE WHEN Estado='Activo' THEN 'Inactivo' ELSE 'Activo' END WHERE ClienteID=@ClienteID`
- **Confirmación:** MessageBox de confirmación antes de ejecutar
- **Restricción FK:** ON DELETE NO ACTION impide borrado físico si hay dispositivos asociados; la baja lógica es la alternativa correcta.
- **Estado:** CONFIRMADO — política de baja lógica ("Cambiar estado")

### 9.2 Dispositivos

#### Crear
- **Formulario:** FrmDispositivoFormulario
- **Validaciones:** Cliente obligatorio (ComboBox), Tipo obligatorio
- **Parámetros:** ClienteID, Tipo, Marca, Modelo, NumeroSerie, Descripcion, FechaRegistro=GETDATE()
- **INSERT:** `INSERT INTO Dispositivos (ClienteID, Tipo, Marca, Modelo, NumeroSerie, Descripcion, FechaRegistro) VALUES (...)`

#### Consultar
- **SELECT:** `SELECT d.DispositivoID, c.Nombre as Cliente, d.Tipo, d.Marca, d.Modelo, d.NumeroSerie FROM Dispositivos d INNER JOIN Clientes c ON d.ClienteID = c.ClienteID ORDER BY d.FechaRegistro DESC`
- **Filtros:** Campo Buscar

#### Modificar
- **Formulario:** FrmDispositivoFormulario pre-cargado
- **UPDATE:** `UPDATE Dispositivos SET ClienteID=@ClienteID, Tipo=@Tipo, Marca=@Marca, Modelo=@Modelo, NumeroSerie=@NumeroSerie, Descripcion=@Descripcion WHERE DispositivoID=@DispositivoID`

#### Cambiar estado
> **CONFLICTO:** El mockup muestra botón "Cambiar estado" para dispositivos, pero la tabla Dispositivos no tiene columna Estado en la BD. La BD tiene Estado solo en Clientes, Técnicos y Usuarios.
> **PENDIENTE:** ¿Cómo se implementa la baja lógica para dispositivos sin columna Estado? Opciones: (a) Agregar columna Estado a Dispositivos (modifica BD — prohibido sin autorización), (b) El mockup muestra la columna Estado pero no existe en la BD, (c) La operación "Cambiar estado" para dispositivos no se implementa hasta resolver el conflicto. **REQUIERE DECISIÓN.**

### 9.3 Órdenes de Servicio

#### Crear
- **Formulario:** FrmOrdenNueva
- **Validaciones:** Dispositivo obligatorio, Problema reportado obligatorio, Costo ≥ 0
- **Parámetros:** DispositivoID, TecnicoID (NULL si no asigna), FechaIngreso, ProblemaReportado, Estado='Pendiente', CostoServicio, Observaciones
- **INSERT:** `INSERT INTO OrdenesServicio (DispositivoID, TecnicoID, FechaIngreso, ProblemaReportado, Estado, CostoServicio, Observaciones) VALUES (...)`
- **Nota:** Estado se fija en 'Pendiente' al crear. FechaFinalizacion se deja NULL.

#### Consultar
- **SELECT:** `SELECT o.OrdenID, c.Nombre as Cliente, d.Tipo as Dispositivo, t.Nombre as Tecnico, o.FechaIngreso, o.Estado, o.CostoServicio FROM OrdenesServicio o INNER JOIN Dispositivos d ON o.DispositivoID = d.DispositivoID INNER JOIN Clientes c ON d.ClienteID = c.ClienteID LEFT JOIN Tecnicos t ON o.TecnicoID = t.TecnicoID`
- **Filtros:** Campo Buscar, ComboBox estado ("Todos los estados", "Pendiente", "En diagnóstico", etc.)
- **ORDER BY:** Estado, FechaIngreso

#### Modificar
- **Formulario:** FrmOrdenDetalle
- **Operaciones:** Actualizar Diagnóstico, TrabajoRealizado, CostoServicio, Observaciones, FechaFinalizacion
- **UPDATE:** `UPDATE OrdenesServicio SET Diagnostico=@Diagnostico, TrabajoRealizado=@TrabajoRealizado, CostoServicio=@CostoServicio, Observaciones=@Observaciones, FechaFinalizacion=@FechaFinalizacion WHERE OrdenID=@OrdenID`
- **Validaciones:** CostoServicio ≥ 0

#### Actualizar estado
- **Operación:** UPDATE del campo Estado
- **Nota:** No hay restricción de BD sobre qué transiciones son válidas. Cualquier estado puede ir a cualquier otro según la documentación actual.
- **Validación de interfaz:** Confirmar cambio de estado antes de ejecutar.

> **PENDIENTE:** No se ha definido qué transiciones de estado son válidas. La BD no lo enforcea. ¿Se permitirá ir de "Pendiente" directamente a "En reparación" sin pasar por "En diagnóstico"? **REQUIERE DECISIÓN.**

### 9.4 Pagos

#### Crear
- **Formulario:** FrmPagoRegistrar
- **Validaciones:** OrdenID obligatorio, Monto > 0, Método obligatorio
- **Parámetros:** OrdenID, FechaPago=GETDATE(), Monto, MetodoPago, Observaciones
- **INSERT:** `INSERT INTO Pagos (OrdenID, FechaPago, Monto, MetodoPago, Observaciones) VALUES (...)`

#### Consultar
- **SELECT:** `SELECT p.PagoID, o.OrdenID, c.Nombre as Cliente, p.FechaPago, p.Monto, p.MetodoPago FROM Pagos p INNER JOIN OrdenesServicio o ON p.OrdenID = o.OrdenID INNER JOIN Dispositivos d ON o.DispositivoID = d.DispositivoID INNER JOIN Clientes c ON d.ClienteID = c.ClienteID ORDER BY p.FechaPago DESC`
- **Filtros:** Campo Buscar, ComboBox método de pago

#### Modificar / Eliminar
> **PENDIENTE:** No se documenta operación de edición ni eliminación de pagos en los mockups. Se infiere que solo se pueden registrar y consultar. **REQUIERE DECISIÓN.**

### 9.5 Técnicos

#### Crear
- **Formulario:** FrmTecnicoFormulario
- **Validaciones:** Nombre obligatorio, Apellido obligatorio
- **Parámetros:** Nombre, Apellido, Telefono, Especialidad, Estado='Activo'
- **INSERT:** `INSERT INTO Técnicos (Nombre, Apellido, Telefono, Especialidad, Estado) VALUES (...)`

#### Consultar
- **SELECT:** `SELECT * FROM Técnicos ORDER BY Nombre`
- **Filtros:** Campo Buscar, filtro Estado

#### Modificar
- **Formulario:** FrmTecnicoFormulario pre-cargado
- **UPDATE:** `UPDATE Técnicos SET Nombre=@Nombre, Apellido=@Apellido, Telefono=@Telefono, Especialidad=@Especialidad, Estado=@Estado WHERE TecnicoID=@TecnicoID`

#### Cambiar estado
- **Operación:** UPDATE del campo Estado (Activo ↔ Inactivo)
- **Confirmación:** MessageBox de confirmación
- **Nota:** Los técnicos inactivos pueden seguir teniendo órdenes asignadas históricamente (no hay restricción FK que lo impida).

### 9.6 Usuarios

#### Crear
- **Formulario:** FrmUsuarioFormulario
- **Validaciones:** NombreUsuario obligatorio y único, Password obligatorio, Rol obligatorio
- **Parámetros:** NombreUsuario, PasswordHash, Rol, Estado='Activo', TecnicoID (NULL si no es técnico)
- **INSERT:** `INSERT INTO Usuarios (NombreUsuario, PasswordHash, Rol, Estado, TecnicoID) VALUES (...)`
- **Nota:** Password se debe hashear antes de almacenar (ver sección de seguridad)

#### Consultar
- **SELECT:** `SELECT u.UsuarioID, u.NombreUsuario, u.Nombre, u.Rol, u.Estado, t.Nombre as TecnicoAsociado FROM Usuarios u LEFT JOIN Técnicos t ON u.TecnicoID = t.TecnicoID`

#### Modificar
- **Formulario:** FrmUsuarioFormulario pre-cargado
- **UPDATE:** `UPDATE Usuarios SET NombreUsuario=@NombreUsuario, PasswordHash=@PasswordHash, Rol=@Rol, Estado=@Estado, TecnicoID=@TecnicoID WHERE UsuarioID=@UsuarioID`
- **Validación:** Si cambia NombreUsuario, verificar unicidad

#### Cambiar estado
- **Operación:** UPDATE del campo Estado
- **Nota:** El usuario "diego.salinas" ya está Inactivo en la BD de prueba

#### Asignar técnico
- **Operación:** ComboBox con técnicos disponibles (solo si Rol = Técnico)
- **Validación:** Solo un técnico por usuario (enforzado por índice único filtrado en BD)
- **Restricción:** Solo Técnicos pueden tener TécnicoID asignado

### 9.7 Reportes

#### Generar
- **Formulario:** FrmReportes
- **Operación:** Consultas de aggregación con GROUP BY y filtros de fecha
- **Consultas:**
  - Órdenes por estado: `SELECT Estado, COUNT(*) as Cantidad, SUM(CostoServicio) as Subtotal FROM OrdenesServicio WHERE FechaIngreso BETWEEN @Desde AND @Hasta GROUP BY Estado`
  - Órdenes por técnico: `SELECT t.Nombre, COUNT(*) as Cantidad FROM OrdenesServicio o INNER JOIN Técnicos t ON o.TecnicoID = t.TecnicoID WHERE o.FechaIngreso BETWEEN @Desde AND @Hasta GROUP BY t.Nombre`
  - Servicios completados: `SELECT COUNT(*) as Cantidad FROM OrdenesServicio WHERE Estado IN ('Listo', 'Entregado') AND FechaFinalizacion BETWEEN @Desde AND @Hasta`
  - Pagos registrados: `SELECT COUNT(*) as Cantidad, SUM(Monto) as Total FROM Pagos WHERE FechaPago BETWEEN @Desde AND @Hasta`

> **PENDIENTE:** El subtotal del reporte "Órdenes por estado" para "Listo" muestra "$0.00" en el mockup sin tener una cantidad de órdenes listas. ¿Se muestra $0.00 o se oculta la fila? **REQUIERE DECISIÓN.**

> **PENDIENTE:** La exportación no está especificada en el contexto. Se sugiere CSV como formato básico. **REQUIERE DECISIÓN.**

---

## 10. PLAN DE VALIDACIONES

### 10.1 Validaciones de interfaz (por formulario)

| Formulario | Campo | Validación | Tipo |
|------------|-------|------------|------|
| Login | NombreUsuario | Obligatorio | Interfaz |
| Login | Password | Obligatorio | Interfaz |
| ClienteFormulario | Nombre | Obligatorio | Interfaz |
| ClienteFormulario | Teléfono | Obligatorio | Interfaz |
| ClienteFormulario | Email | Formato de email válido si se ingresa | Interfaz |
| DispositivoFormulario | Cliente | Obligatorio (ComboBox) | Interfaz |
| DispositivoFormulario | Tipo | Obligatorio | Interfaz |
| OrdenNueva | Dispositivo | Obligatorio (ComboBox) | Interfaz |
| OrdenNueva | Problema reportado | Obligatorio | Interfaz |
| OrdenNueva | CostoServicio | ≥ 0, numérico | Interfaz + BD |
| PagoRegistrar | OrdenID | Obligatorio (ComboBox) | Interfaz |
| PagoRegistrar | Monto | > 0, numérico | Interfaz + BD |
| PagoRegistrar | Método | Obligatorio (ComboBox) | Interfaz |
| TécnicoFormulario | Nombre | Obligatorio | Interfaz |
| TécnicoFormulario | Apellido | Obligatorio | Interfaz |
| UsuarioFormulario | NombreUsuario | Obligatorio, único | Interfaz + BD |
| UsuarioFormulario | Password | Obligatorio | Interfaz |
| UsuarioFormulario | Rol | Obligatorio (ComboBox) | Interfaz |
| Reportes | Desde | No posterior a Hasta | Interfaz |

### 10.2 Restricciones de base de datos

| Tabla | Constraint | Descripción | Estado |
|-------|-----------|-------------|--------|
| Clientes | CK_Clientes_Estado | Estado IN ('Activo', 'Inactivo') | CONFIRMADO |
| Dispositivos | FK_Dispositivos_Clientes | ON DELETE NO ACTION | CONFIRMADO |
| Técnicos | CK_Tecnicos_Estado | Estado IN ('Activo', 'Inactivo') | CONFIRMADO |
| OrdenesServicio | CK_OrdenesServicio_Estado | Estado IN (5 valores) | CONFIRMADO |
| OrdenesServicio | CK_OrdenesServicio_Costo | CostoServicio ≥ 0 | CONFIRMADO |
| Pagos | CK_Pagos_Monto | Monto > 0 | CONFIRMADO |
| Pagos | CK_Pagos_MetodoPago | Método IN ('Efectivo', 'Tarjeta', 'Transferencia') | CONFIRMADO |
| Pagos | FK_Pagos_OrdenesServicio | ON DELETE NO ACTION | CONFIRMADO |
| Usuarios | CK_Usuarios_Rol | Rol IN ('Administrador', 'Empleado', 'Técnico') | CONFIRMADO |
| Usuarios | CK_Usuarios_Estado | Estado IN ('Activo', 'Inactivo') | CONFIRMADO |
| Usuarios | UQ_Usuarios_NombreUsuario | NombreUsuario único | CONFIRMADO |
| Usuarios | UQ_Usuarios_TecnicoID | Índice único filtrado (1:1 técnico-usuario) | CONFIRMADO |

### 10.3 Diferencia entre validación de interfaz y restricción de BD

| Tipo | Propósito | Ejemplo | Ubicación |
|------|-----------|---------|-----------|
| Interfaz | Evitar envío de datos inválidos, mejor UX | Campo obligatorio resaltado | Formulario |
| BD | Garantizar integridad referencial | FK, CHECK constraints | Base de datos |

**Principio:** La validación de interfaz facilita la experiencia del usuario (mensajes inmediatos, resaltado de errores). La restricción de BD es la última línea de defensa. Ambas se implementan.

### 10.4 Validaciones especiales

- **CostoServicio:** Validar en interfaz (≥ 0) y la BD también lo enforcea con CK_OrdenesServicio_Costo.
- **Monto de pago:** Validar en interfaz (> 0) y la BD con CK_Pagos_Monto.
- **NombreUsuario único:** Verificar en interfaz con consulta a la BD antes de INSERT.
- **Estado de órdenes:** La interfaz debe mostrar los 5 estados pero la BD no restringe transiciones.

---

## 11. PLAN DE REPORTES

### 11.1 Información que puede consultar

Según `Contexto/09_identidad_visual.md`, los reportes oficiales son 4:

| # | Reporte | Fuente |
|---|---------|--------|
| 1 | Órdenes por estado | Manual Identidad Visual, Página 14 |
| 2 | Órdenes por técnico | Manual Identidad Visual, Página 14 |
| 3 | Servicios completados | Manual Identidad Visual, Página 14 |
| 4 | Pagos registrados | Manual Identidad Visual, Página 14 |

### 11.2 Filtros disponibles

- **Tipo de reporte:** ComboBox con los 4 reportes oficiales (CONFIRMADO)
- **Desde / Hasta:** DateTimePicker para rango de fechas (CONFIRMADO por mockup 13)
- **Generar:** Botón para ejecutar la consulta (CONFIRMADO por mockup 13)
- **Exportar:** Botón para exportar resultados (CONFIRMADO por mockup 13, formato PENDIENTE)

### 11.3 Tablas y consultas

| Reporte | Tablas origen | Consulta base |
|---------|---------------|---------------|
| Órdenes por estado | OrdenesServicio | GROUP BY Estado con COUNT y SUM |
| Órdenes por técnico | OrdenesServicio, Técnicos | GROUP BY Técnico con COUNT |
| Servicios completados | OrdenesServicio | WHERE Estado IN ('Listo','Entregado') con COUNT |
| Pagos registrados | Pagos | COUNT y SUM de Monto |

### 11.4 Totales

El mockup 13 muestra una tabla con columnas: **Estado**, **Cantidad de órdenes**, **Subtotal**.

> **CONFLICTO:** El subtotal para "Listo" muestra "$0.00" pero sin indicar cuántas órdenes hay en ese estado. El comportamiento exacto no está claro.

> **PENDIENTE DE DECISIÓN:** ¿Se muestran totales parciales por cada estado o solo un total general?

### 11.5 Posibles exportaciones

- **Formato sugerido:** CSV (simple, sin dependencias adicionales)
- **Contenido:** Los datos de la tabla de resultados
- **Nombre del archivo:** `FixTrack_Reporte_[tipo]_[fecha].csv`

> **PENDIENTE:** No se ha documentado formato de exportación en el contexto. CSV se sugiere por simplicidad.

---

## 12. ORDEN DE IMPLEMENTACIÓN

### FASE 3.1 — Preparación del proyecto
- Crear solución en Visual Studio con nombre `FixTrack`
- Crear proyecto Windows Forms (.NET Framework o .NET 6+)
- Configurar carpetas: Formularios/, Modelos/, Datos/, Recursos/, Configuracion/
- Crear archivo `app.config` con conexión a la BD
- Configurar referencias: System.Data.SqlClient
- **Criterio de aceptación:** Solución compila sin errores, estructura de carpetas creada.

### FASE 3.2 — Configuración de conexión ADO.NET
- Implementar `Datos/Conexion.cs` con lectura de `app.config`
- Implementar clase auxiliar de manejo de conexiones (métodos helper)
- Probar conexión a la base de datos `FixTrack`
- **Criterio de aceptación:** Conexión exitosa a SQL Server, lectura de cadena desde app.config.

### FASE 3.3 — Modelos
- Crear `Modelos/Cliente.cs`, `Modelos/Dispositivo.cs`, `Modelos/Tecnico.cs`, `Modelos/OrdenServicio.cs`, `Modelos/Pago.cs`, `Modelos/Usuario.cs`
- Crear `Modelos/EstadoOrden.cs` (enum con los 5 estados)
- Crear `Modelos/MetodoPago.cs` (enum con 3 métodos)
- Crear `Modelos/Sesion.cs` (clase estática de sesión)
- Crear `Modelos/ReporteResultado.cs` (clase para resultados de reportes)
- **Criterio de aceptación:** Todas las clases POCO creadas, propiedades mapeadas a columnas de la BD, compilan sin errores.

### FASE 3.4 — Acceso a datos (DAL)
- Crear `Datos/ClienteDAL.cs`, `Datos/DispositivoDAL.cs`, `Datos/TecnicoDAL.cs`, `Datos/OrdenServicioDAL.cs`, `Datos/PagoDAL.cs`, `Datos/UsuarioDAL.cs`, `Datos/ReportesDAL.cs`
- Implementar métodos CRUD para cada entidad
- Implementar consultas parametrizadas
- Implementar manejo de transacciones donde sea necesario
- **Criterio de aceptación:** Cada DAL tiene métodos ObtenerTodos, ObtenerPorId, Insertar, Actualizar. Pruebas unitarias básicas exitosas.

### FASE 3.5 — Login
- Crear `Formularios/Login/FrmLogin.cs`
- Implementar validación de credenciales
- Implementar establecimiento de sesión
- Implementar mensaje de error "Credenciales inválidas"
- **Criterio de aceptación:** Login funcional con 2 usuarios de prueba (admin, recepcion1). Error mostrado correctamente.

### FASE 3.6 — Dashboard
- Crear `Formularios/Dashboard/FrmDashboard.cs`
- Implementar carga de 5 métricas
- Implementar DataGridView de órdenes recientes
- Implementar navegación al menú lateral
- **Criterio de aceptación:** Dashboard muestra 5 indicadores y tabla de órdenes recientes. Navegación a módulos funcional.

### FASE 3.7 — Clientes
- Crear `Formularios/Clientes/FrmClientes.cs`
- Crear `Formularios/Clientes/FrmClienteDetalle.cs`
- Crear `Formularios/Clientes/FrmClienteFormulario.cs`
- Implementar CRUD completo
- Implementar baja lógica ("Cambiar estado")
- Implementar búsqueda y filtros
- **Criterio de aceptación:** Crear, listar, buscar, editar, cambiar estado funcional. Sin errores de compilación.

### FASE 3.8 — Dispositivos
- Crear `Formularios/Dispositivos/FrmDispositivos.cs`
- Crear `Formularios/Dispositivos/FrmDispositivoFormulario.cs`
- Implementar CRUD completo
- Implementar búsqueda y filtros
- **Criterio de aceptación:** Crear, listar, buscar, editar funcional. Nota: Cambiar estado de dispositivos PENDIENTE (requiere decisión sobre columna Estado).

### FASE 3.9 — Órdenes de servicio
- Crear `Formularios/Ordenes/FrmOrdenes.cs`
- Crear `Formularios/Ordenes/FrmOrdenNueva.cs`
- Crear `Formularios/Ordenes/FrmOrdenDetalle.cs`
- Implementar creación con estado fijo "Pendiente"
- Implementar filtro por estado
- Implementar actualización de diagnóstico, trabajo, costo
- Implementar navegación a registro de pagos
- **Criterio de aceptación:** Crear orden, filtrar por estado, ver detalle, actualizar campos funcionales.

### FASE 3.10 — Técnicos
- Crear `Formularios/Técnicos/FrmTecnicos.cs`
- Crear `Formularios/Técnicos/FrmTecnicoFormulario.cs`
- Implementar CRUD completo
- Implementar búsqueda, filtros, cambio de estado
- **Criterio de aceptación:** Crear, listar, buscar, editar, cambiar estado funcional. Acceso solo por Administrador.

### FASE 3.11 — Pagos
- Crear `Formularios/Pagos/FrmPagos.cs`
- Crear `Formularios/Pagos/FrmPagoRegistrar.cs`
- Implementar listado y búsqueda
- Implementar registro de pagos
- **Criterio de aceptación:** Listar pagos, registrar nuevo pago funcional. Monto > 0 y método válido validados.

### FASE 3.12 — Usuarios
- Crear `Formularios/Usuarios/FrmUsuarios.cs`
- Crear `Formularios/Usuarios/FrmUsuarioFormulario.cs`
- Implementar CRUD completo
- Implementar asignación de técnico asociado
- Implementar búsqueda, filtros, cambio de estado
- Implementar hasheo de contraseñas
- **Criterio de aceptación:** Crear, listar, buscar, editar, cambiar estado, asignar técnico funcional. Solo Administrador accede.

### FASE 3.13 — Reportes
- Crear `Formularios/Reportes/FrmReportes.cs`
- Implementar los 4 reportes oficiales
- Implementar filtros de fecha
- Implementar generación y exportación
- **Criterio de aceptación:** Los 4 reportes generan resultados correctos. Filtros de fecha funcionales. Exportación funcional.

### FASE 3.14 — Integración
- Conectar todos los formularios con la navegación correcta
- Implementar control de acceso por rol (habilitar/deshabilitar opciones de menú)
- Implementar "Mis órdenes" para el rol Técnico (filtrado)
- Implementar cierre de sesión
- Implementar coherencia visual (colores, tipografía, iconos)
- **Criterio de aceptación:** Toda la navegación funciona. Los roles filtran el menú correctamente. La apariencia visual es consistente.

### FASE 3.15 — Pruebas finales
- Pruebas de conexión a BD
- Pruebas de cada CRUD
- Pruebas de navegación completa
- Pruebas por rol
- Pruebas de validaciones
- Pruebas de errores y excepciones
- Ajustes visuales finales
- Compilación final sin errores ni advertencias
- **Criterio de aceptación:** Todos los módulos funcionan. Sin errores de compilación. Pruebas pasan.

---

## 13. DEPENDENCIAS

```
Modelos (clases POCO)
   │
   ├──→ Datos (DAL) ←── Configuración (cadena de conexión)
   │         │
   │         └──→ Conexión a SQL Server
   │
   ├──→ Formularios (capa de presentación)
   │         │
   │         ├── FrmLogin
   │         │      │
   │         │      └──→ FrmDashboard
   │         │               │
   │         │               ├── FrmClientes → FrmClienteDetalle → FrmClienteFormulario
   │         │               ├── FrmDispositivos → FrmDispositivoFormulario
   │         │               ├── FrmOrdenes → FrmOrdenNueva → FrmOrdenDetalle → FrmPagoRegistrar
   │         │               ├── FrmPagos → FrmPagoRegistrar
   │         │               ├── FrmTecnicos → FrmTecnicoFormulario  (Admin)
   │         │               ├── FrmUsuarios → FrmUsuarioFormulario  (Admin)
   │         │               └── FrmReportes
   │         │
   │         └── Menú lateral (filtrado por rol)
   │
   └── Sesión (clase estática)
```

### Dependencias detalladas

| Módulo | Depende de | Razón |
|--------|------------|-------|
| FrmLogin | Ninguno | Primer formulario |
| FrmDashboard | FrmLogin, Modelos, Datos | Necesita datos de órdenes |
| Clientes | Datos (ClienteDAL) | Acceso a BD |
| Dispositivos | Datos (DispositivoDAL), Clientes (ComboBox) | Necesita lista de clientes |
| Órdenes | Datos (OrdenServicioDAL), Dispositivos (ComboBox) | Necesita lista de dispositivos |
| Pagos | Datos (PagoDAL), Órdenes (ComboBox) | Necesita lista de órdenes |
| Técnicos | Datos (TecnicoDAL) | Acceso a BD |
| Usuarios | Datos (UsuarioDAL), Técnicos (ComboBox) | Necesita lista de técnicos |
| Reportes | Datos (ReportesDAL) | Consultas a múltiples tablas |
| FrmOrdenDetalle | FrmOrdenes, Datos | Necesita datos de la orden seleccionada |
| FrmPagoRegistrar | FrmPagos o FrmOrdenDetalle, Datos | Necesita datos de la orden |

### Orden de implementación justificado

1. **Modelos y Datos** primero: sin clases y sin acceso a datos, ningún formulario puede funcionar.
2. **Login** antes que Dashboard: la autenticación precede a cualquier pantalla.
3. **Dashboard** antes que módulos: es la pantalla principal que enlaza al menú.
4. **Operación (Clientes → Dispositivos → Órdenes → Pagos)**: sigue el flujo de negocio natural.
5. **Administración (Técnicos → Usuarios)**: depende de la infraestructura de operativa.
6. **Reportes** al final: depende de que todos los módulos tengan datos para consultar.
7. **Integración** al final: une todo el sistema.

---

## 14. PRUEBAS

### 14.1 Pruebas de aplicación

| # | Prueba | Método | Criterio de éxito |
|---|--------|--------|-------------------|
| 1 | Apertura de la aplicación | Ejecutar el .exe | Se muestra FrmLogin |
| 2 | Navegación por menú | Clic en cada opción del menú | Se abre el formulario correcto |
| 3 | Botones principales | Clic en Guardar, Cancelar, Nuevo, Buscar | Responden correctamente |
| 4 | Formularios | Completar cada formulario | Campos funcionan, se guardan datos |
| 5 | Validaciones | Dejar campos obligatorios vacíos, ingresar valores inválidos | Mensajes de error, campos resaltados |
| 6 | Mensajes | Operaciones exitosas y fallidas | MessageBox con texto apropiado |
| 7 | DataGridView | Seleccionar filas, ordenar, scroll | ReadOnly, FullRowSelect, sin errores |

### 14.2 Pruebas de base de datos

| # | Prueba | Método | Criterio de éxito |
|---|--------|--------|-------------------|
| 1 | Conexión | Abrir y cerrar SqlConnection | Sin excepciones |
| 2 | SELECT | Obtener registros de cada tabla | Datos correctos |
| 3 | INSERT | Crear un registro de cada entidad | Registro creado con datos correctos |
| 4 | UPDATE | Modificar un registro existente | Datos actualizados |
| 5 | Cambio de estado | Cambiar Estado de Activo a Inactivo (y viceversa) en Clientes/Técnicos/Usuarios | Estado cambiado |
| 6 | Relaciones | Intentar borrar un cliente con dispositivos | ON DELETE NO ACTION bloquea |
| 7 | Errores | Conexión incorrecta, consulta con parámetros inválidos | Mensaje amigable sin crash |

### 14.3 Pruebas de integración

| # | Prueba | Método | Criterio de éxito |
|---|--------|--------|-------------------|
| 1 | Formulario → ADO.NET → SQL Server | Crear un cliente desde FrmClienteFormulario | Dato visible en BD y en DataGridView |
| 2 | SQL Server → Aplicación | Consultar registros y verificar en DataGridView | Datos mostrados correctamente |
| 3 | Navegación entre formularios | Recorrer el flujo completo Login → Dashboard → Módulos | Sin errores de navegación |
| 4 | Transacciones | Crear orden y registrar pago | Ambas operaciones confirmadas |

### 14.4 Pruebas por rol

| # | Prueba | Rol | Criterio de éxito |
|---|--------|-----|-------------------|
| 1 | Login como Administrador | Administrador | Acceso a todos los módulos |
| 2 | Login como Empleado | Empleado | Acceso a Operación + Reportes, sin Técnicos/Usuarios |
| 3 | Login como Técnico | Técnico | Acceso solo a órdenes asignadas |
| 4 | Navegación por rol | Cada rol | Menú filtrado correctamente |
| 5 | Intentar acceder a módulo restringido | Técnico | Opción de menú deshabilitada o invisible |

### 14.5 Plan de pruebas por módulo

Para cada módulo, las pruebas cubren:
- **Crear:** Insertar un registro nuevo → verificar en BD y en DataGridView
- **Listar:** Abrir el listado → verificar que muestra datos
- **Buscar:** Usar el campo Buscar → verificar resultados filtrados
- **Editar:** Modificar un registro → verificar cambios en BD
- **Cambiar estado (si aplica):** Cambiar estado → verificar actualización
- **Eliminar/borrar:** NO aplica para la mayoría (baja lógica)
- **Errores:** Simular errores (campos vacíos, conexión caída) → verificar manejo

---

## 15. CRITERIOS DE ACEPTACIÓN

### 15.1 Login
- formulario funciona;
- validación de credenciales funciona;
- mensaje "Credenciales inválidas" se muestra en caso de error;
- rol del usuario se establece correctamente;
- navegación a FrmDashboard después del login.

### 15.2 Dashboard
- formulario funciona;
- 5 métricas (Pendientes, En diagnóstico, En reparación, Listos, Entregados) se muestran;
- tabla de órdenes recientes se muestra;
- navegación a todos los módulos desde el menú funciona.

### 15.3 Clientes terminado cuando:
- formulario funciona;
- listado funciona;
- búsqueda funciona;
- creación funciona;
- edición funciona;
- cambio de estado funciona;
- validaciones funcionan;
- errores se manejan;
- SQL parametrizado;
- aplicación compila.

### 15.4 Dispositivos terminado cuando:
- formulario funciona;
- listado funciona;
- búsqueda funciona;
- creación funciona;
- edición funciona;
- validaciones funcionan;
- errores se manejan;
- SQL parametrizado;
- aplicación compila.

> **PENDIENTE:** Cambio de estado de dispositivos requiere decisión sobre columna Estado en la tabla Dispositivos.

### 15.5 Órdenes de servicio terminado cuando:
- formulario de creación funciona;
- estado se fija en "Pendiente" al crear;
- listado funciona;
- filtro por estado funciona;
- detalle de orden funciona;
- actualización de diagnóstico, trabajo y costo funciona;
- registro de pagos desde el detalle funciona;
- validaciones funcionan;
- errores se manejan;
- SQL parametrizado;
- aplicación compila.

### 15.6 Pagos terminado cuando:
- formulario funciona;
- listado funciona;
- registro de pago funciona;
- Monto > 0 validado;
- método de pago válido validado;
- errores se manejan;
- SQL parametrizado;
- aplicación compila.

### 15.7 Técnicos terminado cuando:
- formulario funciona;
- listado funciona;
- búsqueda funciona;
- creación funciona;
- edición funciona;
- cambio de estado funciona;
- validaciones funcionan;
- errores se manejan;
- SQL parametrizado;
- aplicación compila;
- acceso restringido a Administrador.

### 15.8 Usuarios terminado cuando:
- formulario funciona;
- listado funciona;
- búsqueda funciona;
- creación funciona;
- edición funciona;
- cambio de estado funciona;
- asignación de técnico asociado funciona;
- hasheo de contraseña funciona;
- validaciones funcionan;
- errores se manejan;
- SQL parametrizado;
- aplicación compila;
- acceso restringido a Administrador.

### 15.9 Reportes terminado cuando:
- los 4 reportes oficiales generan resultados;
- filtros de fecha funcionan;
- botón Generar funciona;
- botón Exportar funciona;
- tabla de resultados con totales se muestra;
- errores se manejan;
- SQL parametrizado;
- aplicación compila.

### 15.10 Integración terminado cuando:
- navegación completa funciona;
- menú filtrado por rol funciona;
- cierre de sesión funciona;
- coherencia visual aplicada;
- aplicación compila sin errores ni advertencias.

---

## 16. RIESGOS Y PUNTOS DELICADOS

| # | Problema | Impacto | Solución propuesta | Requiere autorización |
|---|----------|---------|-------------------|----------------------|
| 1 | **"Cambiar estado" vs "Eliminar" en Clientes/Dispositivos** | Conflicto entre arquitectura y mockups. Implementar DELETE puede fallar por FK. | Implementar "Cambiar estado" (baja lógica) consistente con ON DELETE NO ACTION. | No — decisión del equipo en FASE 3 |
| 2 | **Dispositivos sin columna Estado** | El mockup muestra "Cambiar estado" para dispositivos pero la BD no tiene columna Estado en Dispositivos. | No implementar cambio de estado para dispositivos hasta que se resuelva. Agregar columna requiere modificar BD (prohibido sin autorización). | **SÍ** — Modificar BD o aceptar que no hay cambio de estado para dispositivos |
| 3 | **IDs con prefijos alfanuméricos (C-, D-, T-, ORD-, PAG-, U-)** | La BD usa INT IDENTITY pero los mockups muestran prefijos. | Implementar IDs numéricos en la BD y generar el formato con prefijo como string de visualización en el formulario. | No — decisión de implementación |
| 4 | **Rol "Empleado" vs "Empleado Recepcionista"** | La BD almacena "Empleado", el mockup muestra "Empleado Recepcionista". | Almacenar "Empleado" en la BD y mostrar "Empleado / Recepcionista" como texto descriptivo en la interfaz. | No — decisión de presentación |
| 5 | **5 métricas vs 3 en Dashboard** | La arquitectura dice 3, el mockup muestra 5. | Implementar las 5 métricas del mockup (es una ampliación razonable). | No — decidir por el mockup |
| 6 | **Reportes sin submenús definidos** | La arquitectura dice que reportes no están definidos, pero el mockup muestra contenido funcional. | Implementar los 4 reportes oficiales del manual de identidad visual con el formulario único del mockup. | No — decidir por el manual de identidad visual |
| 7 | **Pagos integrados en detalle de orden vs módulo separado** | El mockup muestra pagos en el detalle de orden, la arquitectura los tiene como módulo aparte. | Ambos pueden coexistir: el botón "Registrar pago" navega a FrmPagos y se integra la tabla de pagos en FrmOrdenDetalle. | No — ambas funcionalidades |
| 8 | **Transiciones de estado de órdenes no definidas** | La BD no enforcea qué transiciones son válidas. | Permitir cualquier transición de estado (flexibilidad) pero documentar la limitación. Implementar confirmación al cambiar estado. | No — decisión de implementación |
| 9 | **Conexión SQL Server** | La cadena de conexión exacta no está definida en el contexto. | Usar `.\SQLEXPRESS` como servidor por defecto, permitir configuración en app.config. | No — configuración de entorno |
| 10 | **Hasheo de contraseñas** | La BD almacena PasswordHash (NVARCHAR(256)) pero no se especifica el algoritmo. | Usar un hash simple como SHA256 o incluso una comparación directa durante la FASE 3 (pendiente de decisión sobre seguridad). | No — decisión de implementación |
| 11 | **Exportación de reportes** | No hay formato definido en el contexto. | Implementar exportación a CSV como opción básica. | No — decisión de implementación |
| 12 | **"Mis órdenes" del técnico** | No hay documentación específica sobre cómo se implementa el filtrado. | Implementar como FrmOrdenes con filtro automático por Sesion.TecnicoID cuando el rol es Técnico. | No — decisión de implementación |
| 13 | **Iconos y logo** | No hay archivos de iconos específicos en el contexto. | Usar placeholders o generar iconos simples durante la implementación. | No — decisión de implementación |
| 14 | **Datos de prueba del mockup ≠ datos del SQL** | Los datos visuales son distintos a los del script SQL. | Usar los datos del script SQL para desarrollo. Los datos del mockup son solo de demostración visual. | No — decisión de implementación |
| 15 | **Autenticación y contraseñas** | No se especifica política de contraseñas, longitud mínima, ni si hay políticas de bloqueo. | Implementar autenticación básica con hash. No asumir políticas adicionales. | No — decisión de implementación |

---

## 17. ARCHIVOS QUE PROBABLEMENTE SE CREARÁN

### Estructura propuesta de `APP/`

```
APP/
├── Formularios/
│   ├── Login/
│   │   └── FrmLogin.cs
│   ├── Dashboard/
│   │   └── FrmDashboard.cs
│   ├── Clientes/
│   │   ├── FrmClientes.cs
│   │   ├── FrmClienteDetalle.cs
│   │   └── FrmClienteFormulario.cs
│   ├── Dispositivos/
│   │   ├── FrmDispositivos.cs
│   │   └── FrmDispositivoFormulario.cs
│   ├── Ordenes/
│   │   ├── FrmOrdenes.cs
│   │   ├── FrmOrdenNueva.cs
│   │   └── FrmOrdenDetalle.cs
│   ├── Pagos/
│   │   ├── FrmPagos.cs
│   │   └── FrmPagoRegistrar.cs
│   ├── Tecnicos/
│   │   ├── FrmTecnicos.cs
│   │   └── FrmTecnicoFormulario.cs
│   ├── Usuarios/
│   │   ├── FrmUsuarios.cs
│   │   └── FrmUsuarioFormulario.cs
│   └── Reportes/
│       └── FrmReportes.cs
├── Modelos/
│   ├── Cliente.cs
│   ├── Dispositivo.cs
│   ├── Tecnico.cs
│   ├── OrdenServicio.cs
│   ├── Pago.cs
│   ├── Usuario.cs
│   ├── EstadoOrden.cs
│   ├── MetodoPago.cs
│   ├── Sesion.cs
│   └── ReporteResultado.cs
├── Datos/
│   ├── Conexion.cs
│   ├── ClienteDAL.cs
│   ├── DispositivoDAL.cs
│   ├── TecnicoDAL.cs
│   ├── OrdenServicioDAL.cs
│   ├── PagoDAL.cs
│   ├── UsuarioDAL.cs
│   └── ReportesDAL.cs
├── Recursos/
│   ├── Iconos/
│   └── Logo/
└── Configuracion/
    └── app.config
```

### Nomenclatura de clases

| Carpeta | Archivo | Propósito |
|---------|---------|-----------|
| Modelos | `Cliente.cs` | Clase POCO con propiedades: ClienteID, Nombre, Apellido, Telefono, Email, Direccion, FechaRegistro, Estado |
| Modelos | `Dispositivo.cs` | Clase POCO con propiedades: DispositivoID, ClienteID, Tipo, Marca, Modelo, NumeroSerie, Descripcion, FechaRegistro |
| Modelos | `Tecnico.cs` | Clase POCO con propiedades: TecnicoID, Nombre, Apellido, Telefono, Especialidad, Estado |
| Modelos | `OrdenServicio.cs` | Clase POCO con propiedades: OrdenID, DispositivoID, TecnicoID, FechaIngreso, ProblemaReportado, Diagnostico, TrabajoRealizado, Estado, CostoServicio, FechaFinalizacion, Observaciones |
| Modelos | `Pago.cs` | Clase POCO con propiedades: PagoID, OrdenID, FechaPago, Monto, MetodoPago, Observaciones |
| Modelos | `Usuario.cs` | Clase POCO con propiedades: UsuarioID, NombreUsuario, PasswordHash, Rol, Estado, TecnicoID |
| Modelos | `EstadoOrden.cs` | Enum: Pendiente, EnDiagnostico, EnReparacion, Listo, Entregado |
| Modelos | `MetodoPago.cs` | Enum: Efectivo, Tarjeta, Transferencia |
| Modelos | `Sesion.cs` | Clase estática: UsuarioID, NombreUsuario, Rol, TecnicoID |
| Datos | `Conexion.cs` | Clase estática: GetConnectionString() |
| Datos | `*DAL.cs` | Clase de acceso a datos para cada entidad |
| Formularios | `Frm*.cs` | Cada formulario WinForms |

### Archivo app.config

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <connectionStrings>
    <add name="FixTrackConnection"
         connectionString="Server=.\SQLEXPRESS;Database=FixTrack;Integrated Security=True;"
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2" />
  </startup>
</configuration>
```

> **PENDIENTE:** El servidor y versión de .NET Framework se confirmarán con el entorno de desarrollo.

---

## 18. MATRIZ DE TRAZABILIDAD

| Requisito | Fuente | Módulo | Implementación prevista | Estado |
|-----------|--------|--------|-------------------------|--------|
| Login con credenciales | Mockup 01, Contexto | Login | FrmLogin + validación de Usuarios | PENDIENTE |
| Dashboard con 5 métricas | Mockup 02, Contexto | Dashboard | FrmDashboard con 5 Labels + DataGridView | PENDIENTE |
| Gestión de clientes (CRUD) | Mockups 03-05, Contexto | Clientes | FrmClientes, FrmClienteDetalle, FrmClienteFormulario | PENDIENTE |
| Baja lógica en clientes | Mockup 03, Contexto | Clientes | Botón "Cambiar estado" → UPDATE Estado | PENDIENTE |
| Gestión de dispositivos (CRUD) | Mockups 06-07, Contexto | Dispositivos | FrmDispositivos, FrmDispositivoFormulario | PENDIENTE |
| Cambio de estado en dispositivos | Mockup 06 | Dispositivos | PENDIENTE DE DECISIÓN (sin columna Estado en BD) | PENDIENTE |
| Órdenes de servicio (CRUD) | Mockups 08-10, Contexto | Órdenes | FrmOrdenes, FrmOrdenNueva, FrmOrdenDetalle | PENDIENTE |
| Estado fijo Pendiente al crear | Mockup 09 | Órdenes | INSERT con Estado='Pendiente' | PENDIENTE |
| Diagnóstico/trabajo/costo en detalle | Mockup 10 | Órdenes | FrmOrdenDetalle con campos editables | PENDIENTE |
| Pagos (CRUD) | Mockups 10, 12, Contexto | Pagos | FrmPagos, FrmPagoRegistrar | PENDIENTE |
| Pagos en detalle de orden | Mockup 10 | Órdenes | DataGridView en FrmOrdenDetalle + botón Registrar | PENDIENTE |
| Gestión de técnicos (CRUD) | Mockup 11, Contexto | Técnicos | FrmTecnicos, FrmTecnicoFormulario | PENDIENTE |
| Gestión de usuarios (CRUD) | Mockup 14, Contexto | Usuarios | FrmUsuarios, FrmUsuarioFormulario | PENDIENTE |
| Asignación de técnico a usuario | Mockup 14 | Usuarios | ComboBox en FrmUsuarioFormulario | PENDIENTE |
| Rol "Empleado Recepcionista" | Mockup 14 | Usuarios | Mostrar "Empleado" con texto descriptivo | CONFLICTO/PENDIENTE |
| Reportes (4 oficiales) | Identidad Visual P14, Mockup 13 | Reportes | FrmReportes con filtros y 4 consultas | PENDIENTE |
| Exportación de reportes | Mockup 13 | Reportes | Exportar a CSV | PENDIENTE |
| Filtro por estado en órdenes | Mockup 08 | Órdenes | ComboBox en FrmOrdenes | PENDIENTE |
| Búsqueda en todos los listados | Mockups | Todos | TextBox Buscar en cada listado | PENDIENTE |
| Menú lateral por rol | Contexto 04, 10 | Todos | Menú filtrado según Sesion.Rol | PENDIENTE |
| Cerrar sesión | Contexto 04 | Login | Botón Cerrar sesión → FrmLogin | PENDIENTE |
| Indicadores de color por estado | Identidad Visual P6 | Órdenes/Dashboard | Labels con BackColor según estado | PENDIENTE |
| Controles WinForms estándar | Identidad Visual P7-9 | Todos | MenuStrip, DataGridView, TextBox, Button | PENDIENTE |
| Paleta de colores | Identidad Visual P5-6 | Todos | Primario #2C5F8A, Secundario #FF6B35, etc. | PENDIENTE |
| Tipografía Segoe UI | Identidad Visual P4 | Todos | Font configurado en cada formulario | PENDIENTE |
| Logo FixTrack | Identidad Visual P3 | Login, Dashboard | Imagen del logo | PENDIENTE |
| Iconos | Identidad Visual P7-8 | MenuStrip | Iconos 24px | PENDIENTE |
| Validaciones de formulario | Varios mockups | Todos | Validación por campo en cada formulario | PENDIENTE |
| SQL parametrizado | Rules.md | Datos | Todos los DAL usan SqlParameter | PENDIENTE |
| ON DELETE NO ACTION | BD SQL | Dispositivos, Ordenes, Pagos | Manejado por BD, UI no permite borrado | CONFIRMADO |
| Conexión a SQL Server | Contexto 05, Rules.md | Datos | Conexion.cs desde app.config | PENDIENTE |
| Transacciones | Contexto 07 | Pagos/Órdenes | SqlTransaction en operaciones múltiples | PENDIENTE |
| Sesión de usuario | Contexto 10 | Login/Dashboard | Clase Sesion estática | PENDIENTE |
| Hasheo de contraseñas | BD (PasswordHash) | Login | Hash al almacenar, comparar hash al login | PENDIENTE |
| Mis órdenes para técnico | Contexto 04, 10 | Órdenes | FrmOrdenes con filtro Sesion.TecnicoID | PENDIENTE |
| Actualizar servicio para técnico | Contexto 04 | Órdenes | FrmOrdenDetalle filtrado | PENDIENTE |
| 18 columnas con constraints | Contexto 05 | Modelos | POO mapea todas las columnas | PENDIENTE |
| 5 FK y 6 índices | Contexto 05 | Datos | Joins e índices en consultas | PENDIENTE |

---

## 19. PUNTOS PENDIENTES

Las siguientes decisiones requieren confirmación antes o durante la implementación:

### Decisiones de diseño

| # | Punto pendiente | Descripción | Impacto |
|---|----------------|-------------|---------|
| 1 | Columna Estado en Dispositivos | ¿Se agrega Estado a Dispositivos? Actualmente no existe en la BD. | Determina si se implementa "Cambiar estado" para dispositivos. |
| 2 | Transiciones de estado de órdenes | ¿Qué transiciones son válidas? La BD no las limita. | Determina la lógica de navegación de estados en FrmOrdenDetalle. |
| 3 | Submenús de Reportes | ¿Qué submenús tendrá Reportes? No están definidos. | Determina la estructura del formulario de reportes. |
| 4 | Exportación de reportes | ¿Formato CSV, Excel, PDF? No está definido. | Determina la implementación del botón Exportar. |
| 5 | Pagos integrados vs separados | ¿Cómo navegan el detalle de orden y el módulo de pagos? | Determina la navegación entre FrmOrdenDetalle y FrmPagos. |
| 6 | Implementación de "Mis órdenes" | ¿Cómo se filtran las órdenes del técnico? | Determina si son formularios separados o filtros. |
| 7 | Permisos granulares | ¿Dentro de cada módulo hay permisos de solo lectura vs edición? | Determina si algunos formularios son de solo lectura por rol. |
| 8 | Formato de IDs con prefijos | ¿Se almacenan como strings o se generan como visualización? | Determina cómo se muestran los IDs en las interfaces. |
| 9 | Algoritmo de hasheo de contraseñas | ¿SHA256, BCrypt, otro? | Determina la implementación de autenticación. |
| 10 | Política de contraseñas | ¿Longitud mínima, expiración, bloqueo? | Determina validaciones adicionales en FrmUsuarioFormulario. |

### Conflictos documentados

| # | Conflicto | Resolución propuesta |
|---|-----------|---------------------|
| 1 | "Cambiar estado" vs "Eliminar" | Implementar "Cambiar estado" (baja lógica). |
| 2 | IDs con prefijos vs INT IDENTITY | Generar formato de visualización sobre IDs numéricos. |
| 3 | 3 vs 5 métricas en Dashboard | Implementar 5 métricas del mockup. |
| 4 | "Empleado" vs "Empleado Recepcionista" | Mostrar "Empleado" en BD, "Empleado / Recepcionista" en UI. |
| 5 | Pagos integrados vs separados | Ambos coexisten: tabla en detalle + módulo independiente. |
| 6 | Dispositivos sin columna Estado | **PENDIENTE DE DECISIÓN.** No implementar cambio de estado hasta resolver. |

### Fuentes de referencia

- `Rules.md` — Reglas centrales de desarrollo
- `Contexto/13_incertidumbres.md` — Lista completa de incertidumbres y conflictos
- `Contexto/05_base_de_datos.md` — Estructura de BD para validar columnas y constraints
- `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Script SQL original

---

## 20. MATRIZ DE RIESGOS RESUMIDA

| Riesgo | Probabilidad | Impacto | Mitigación |
|--------|-------------|---------|------------|
| Dispositivos sin columna Estado | Alta | Medio | Implementar baja lógica solo para Clientes/Técnicos/Usuarios; dejar Dispositivos sin cambio de estado hasta decisión |
| Conexión SQL Server no disponible | Medio | Alto | Configurar app.config con cadena ajustable; probar conexión en FASE 3.2 |
| Conflictos no resueltos entre documentos | Alto | Medio | Documentar cada decisión en `Contexto/`; no resolver unilateralmente |
| Datos del mockup ≠ datos de la BD | Medio | Bajo | Usar datos de la BD para desarrollo; no preocuparse por coincidencia visual |
| Sobreingeniería de la arquitectura | Medio | Medio | Mantener 3 capas simples; evitar capas de servicios, repositorios, etc. |
| Falta de iconos/logo | Medio | Bajo | Usar placeholders; documentar como pendiente de recursos |
| Cambio de requisitos | Bajo | Alto | Seguir Rules.md y Contexto/; cualquier cambio documentarse |

---

## 21. INFORME FINAL

### PLAN GENERAL

FixTrack se implementará como una aplicación de escritorio C# + Windows Forms con una arquitectura de 3 capas simples (Presentación, Dominio, Acceso a Datos). La base de datos es SQL Server con 6 tablas (Clientes, Dispositivos, Técnicos, OrdenesServicio, Pagos, Usuarios), 5 Foreign Keys, y 6 índices. La implementación sigue exclusivamente la documentación de `Contexto/` y el script SQL del Entregable 1. No se inventan tablas, columnas, campos ni reglas.

### ARQUITECTURA PROPUESTA

```
APP/
├── Formularios/   # 17 formularios organizados por módulo
├── Modelos/       # 10 clases POCO + 2 enums + Sesion
├── Datos/         # 1 clase Conexion + 7 clases DAL
├── Recursos/      # Iconos y logo (pendientes)
└── Configuracion/ # app.config con cadena de conexión
```

Namespace raíz: `FixTrack` con sub-namespaces por capa.

### MÓDULOS

- **Operación:** Clientes, Dispositivos, Órdenes de Servicio, Pagos
- **Administración:** Técnicos, Usuarios
- **Información:** Reportes
- **Sistema:** Login, Dashboard, Cerrar sesión

### FORMULARIOS

17 formularios en total:
- 1 Login (FrmLogin)
- 1 Dashboard (FrmDashboard)
- 3 Clientes (Listado, Detalle, Formulario)
- 2 Dispositivos (Listado, Formulario)
- 3 Órdenes (Listado, Nueva, Detalle)
- 2 Pagos (Listado, Registrar)
- 2 Técnicos (Listado, Formulario)
- 2 Usuarios (Listado, Formulario)
- 1 Reportes

### BASE DE DATOS

- Nombre: `FixTrack`
- Acceso: ADO.NET puro (SqlConnection, SqlCommand, SqlDataReader, SqlDataAdapter)
- Conexión: desde `app.config`, todas las consultas parametrizadas
- Tablas: 6 con sus constraints, FKs e índices existentes
- Sin vistas, procedimientos almacenados, funciones ni triggers

### CRUD

| Módulo | CREATE | READ | UPDATE | Cambio de estado |
|--------|:------:|:----:|:------:|:----------------:|
| Clientes | ✅ | ✅ | ✅ | ✅ (baja lógica) |
| Dispositivos | ✅ | ✅ | ✅ | ⚠️ PENDIENTE |
| Órdenes | ✅ | ✅ | ✅ | ✅ (estado) |
| Pagos | ✅ | ✅ | ❓ PENDIENTE | ❌ |
| Técnicos | ✅ | ✅ | ✅ | ✅ (Activo/Inactivo) |
| Usuarios | ✅ | ✅ | ✅ | ✅ (Activo/Inactivo) |
| Reportes | ❌ | ✅ | ❌ | ❌ |

### NAVEGACIÓN

```
Login → Dashboard → Menú lateral → Formularios de gestión → Cerrar sesión → Login
```

Menú filtrado por rol:
- Administrador: todos los módulos
- Empleado/Recepcionista: Operación + Reportes
- Técnico: Inicio + Mis órdenes + Actualizar servicio

### ROLES

3 roles con acceso diferenciado:
- Administrador: acceso completo
- Empleado/Recepcionista: módulos de operación + reportes
- Técnico: solo sus órdenes asignadas

### VALIDACIONES

- **Interfaz:** campos obligatorios, formatos, rangos numéricos (Monto > 0, Costo ≥ 0), unicidad de NombreUsuario
- **BD:** CHECK constraints (estados, montos, costos), FK con ON DELETE NO ACTION, unique constraints
- **Seguridad:** todas las consultas parametrizadas, conexiones en `using`, excepciones manejadas

### ORDEN DE IMPLEMENTACIÓN

1. Preparación del proyecto (FASE 3.1)
2. Configuración de conexión ADO.NET (FASE 3.2)
3. Modelos (FASE 3.3)
4. Acceso a datos — DAL (FASE 3.4)
5. Login (FASE 3.5)
6. Dashboard (FASE 3.6)
7. Clientes (FASE 3.7)
8. Dispositivos (FASE 3.8)
9. Órdenes de servicio (FASE 3.9)
10. Técnicos (FASE 3.10)
11. Pagos (FASE 3.11)
12. Usuarios (FASE 3.12)
13. Reportes (FASE 3.13)
14. Integración (FASE 3.14)
15. Pruebas finales (FASE 3.15)

### DEPENDENCIAS

Modelos → Datos → Formularios. Los formularios dependen de los DAL y de otros formularios por navegación. Los reportes dependen de todos los módulos. La implementación sigue el orden: primero la infraestructura (modelos + datos), luego login + dashboard, luego módulos en orden de flujo de negocio, luego integración.

### PRUEBAS

Plan de pruebas en 4 categorías: aplicación (apertura, navegación, botones, formularios, validaciones, mensajes), base de datos (conexión, SELECT, INSERT, UPDATE, cambio de estado, relaciones, errores), integración (formulario → ADO.NET → SQL Server → aplicación), y roles (Administrador, Empleado, Técnico).

### RIESGOS

14 riesgos identificados, los principales son: dispositivos sin columna Estado en la BD, conflictos entre documentos no resueltos, y la conexión SQL Server requiere configuración. Todos los riesgos tienen mitigación propuesta.

### PUNTOS PENDIENTES

10 decisiones de diseño pendientes + 6 conflictos documentados. Los más críticos son: columna Estado en Dispositivos, transiciones de estado de órdenes, y permisos granulares por rol.

### ARCHIVOS PREVISTOS

Estructura propuesta de `APP/` con 17 formularios, 10 clases de modelo, 8 clases de datos, y archivo de configuración.

### ESTADO

FASE 3 COMPLETADA — PLAN DE IMPLEMENTACIÓN PREPARADO — IMPLEMENTACIÓN AÚN NO INICIADA

---

## ANEXO — ESTADO DE IMPLEMENTACION (actualizado 03/09/2026)

El plan original finalizaba con *"IMPLEMENTACION AUN NO INICIADA"*. A partir de la auditoria se avanzo:

- **3.1-3.2:** completas. Cadena de conexion configurada para `Server=.\SQLEXPRESS` (instancia real del equipo). Build: 0 errores / 0 advertencias.
- **3.3:** completa (se agregaron `EstadoOrden`, `MetodoPago`, `Sesion`, `ReporteResultado` y props de visualizacion en modelos).
- **3.4:** completa. 7 DALs con CRUD parametrizado + `Seguridad` (SHA-256). Validado con 26/26 pruebas en `TestRunner` contra SQL Server.
- **3.5:** `FrmLogin` funcional.
- **3.6:** `FrmDashboard` funcional (menu por rol, 5 metricas, ordenes recientes).
- **3.7:** Clientes completo (listado, formulario, detalle con dispositivos, baja logica).
- **3.8-3.13:** esqueletos de formularios creados (navegacion funcional); CRUD pendiente por modulo.
- **3.14-3.15:** pendientes.

**Entregable de BD actualizado:** `BD/FixTrack_BD.sql` (esquema identico al original; usuarios demo con hashes SHA-256 reales).
