# Rules — FixTrack

Reglas centrales de desarrollo para FixTrack. Toda implementación futura debe respetar estos lineamientos sin inventar requisitos, tablas, campos ni reglas no soportados por la documentación en `Contexto/`.

---

## 1. Tecnología y herramientas

- **Lenguaje:** C# (.NET Framework o .NET 6+).
- **Interfaz:** Windows Forms (WinForms) exclusivamente.
  - Prohibido: WPF, MAUI, ASP.NET, Blazor, Electron, consola, terminal.
- **IDE:** Visual Studio.
- **Base de datos:** SQL Server (local o Express).
  - Prohibido: MySQL, PostgreSQL, SQLite, MongoDB, cualquier base NoSQL.
- **Acceso a datos:** ADO.NET (SqlConnection, SqlCommand, SqlDataReader, SqlDataAdapter).
  - Prohibido: Entity Framework, Dapper, NHibernate, ODBC, OleDb, cualquier ORM sin autorización explícita.
- **Controles:** Todos los controles de pantalla deben ser nativos de WinForms o componentes estándar de VS.

---

## 2. Estructura del proyecto

```
FixTrack_Proyecto/
├── Contexto/          # Documentación de análisis (NO modificar sin justificación)
├── Entregables/       # Entregable original (PROHIBIDO modificar)
│   └── Entregable_1/
├── APP/               # Implementación de la aplicación
└── Rules.md           # Este archivo
```

- `Entregables/Entregable_1/` es de solo lectura. No se modifica, borra ni sobreescribe.
- `Contexto/` es la fuente de verdad. Cualquier duda debe resolverse consultando sus archivos antes que cualquier otra fuente.
- `APP/` contiene todo el código fuente, formularios, recursos y configuración de la aplicación.

---

## 3. Nomenclatura y convenciones

### Identificadores
- Las tablas de base de datos usan **ID numéricos INT IDENTITY** (1, 2, 3…).
- Los mockups muestran **prefijos alfanuméricos** (C-001, D-001, T-001, ORD-001, PAG-001, U-001). Esta discrepancia está **sin resolver** — documentar en `Contexto/` antes de decidir.
- El **Estado** del cliente y técnico se gestiona con un campo de tipo string o enum con valores `Activo` / `Inactivo`.
- El **Estado** de la orden de servicio es un campo con exactamente 5 valores válidos.

### Nombres de archivos y clases
- Formularios: `Frm[Nombre].cs` (ej. `FrmLogin.cs`, `FrmDashboard.cs`, `FrmClientes.cs`).
- Clases de acceso a datos: `Dal[Nombre].cs` o `[Nombre]Repository.cs`.
- Clases de modelo/entidad: `[Nombre]Model.cs` o `[Nombre].cs`.
- Nombres de archivos en español, sin espacios, con guiones bajos si es necesario.

---

## 4. Base de datos

### Tablas (6 en total)
| Tabla | Propósito |
|---|---|
| `Clientes` | Registro de clientes |
| `Dispositivos` | Registro de dispositivos |
| `Técnicos` | Registro de técnicos |
| `OrdenesServicio` | Órdenes de servicio |
| `Pagos` | Pagos registrados |
| `Usuarios` | Usuarios del sistema y sus roles |

### Relaciones
- 5 Foreign Keys definidas.
- `ON DELETE NO ACTION` en Clientes y Dispositivos cuando existen órdenes asociadas.
- Un técnico puede estar asociado a como máximo un usuario (índice único filtrado).
- Una orden puede tener múltiples pagos asociados.

### Restricciones de integridad
- `CK_OrdenesServicio_Costo`: costo del servicio debe ser ≥ 0.
- `CK_Pagos_Monto`: monto del pago debe ser > 0.
- Estados de clientes y técnicos: solo `Activo` o `Inactivo`.

### Estados de órdenes de servicio (5 valores, exactos)
1. **Pendiente** — Creada pero no revisada por un técnico.
2. **En diagnóstico** — Técnico evaluando el dispositivo.
3. **En reparación** — Dispositivo siendo reparado activamente.
4. **Listo** — Reparación terminada, pendiente de entrega.
5. **Entregado** — Dispositivo entregado; orden cerrada.

> El estado se fija en **Pendiente** al crear la orden. El flujo entre estados está **sin definir** como camino obligatorio en la BD.

### Métodos de pago (3 valores, exactos)
- **Efectivo**
- **Tarjeta**
- **Transferencia**

### Otros campos clave en Usuarios
- Campo `Rol` con los roles del sistema.
- **Rol "Recepcionista"** confirmado en el mockup (el nombre exacto debe validarse en `Contexto/12_auditoria_consistencia.md`).
- Columna **"Técnico asociado"** visible en el mockup de Usuarios.

---

## 5. Roles y permisos

### Roles (3, definidos por rol del sistema)
| Rol | Permisos |
|---|---|
| **Administrador** | Acceso completo a todas las funcionalidades. |
| **Empleado / Recepcionista** | Crear órdenes, gestionar clientes/dispositivos, generar reportes. |
| **Técnico** | Ver órdenes asignadas, actualizar estado a "En diagnóstico" o "En reparación", registrar diagnóstico y costo de reparación. |

- Los técnicos solo pueden ver y actualizar las órdenes que les están asignadas.
- No se deben crear roles adicionales sin justificación documentada.
- La autenticación requiere nombre de usuario y contraseña; los mensajes de error como "Credenciales inválidas" deben replicarse.

---

## 6. Reglas de la interfaz de usuario

### Principios generales
- **Máximo dos botones principales** por formulario (Aceptar / Cancelar o equivalentes).
- Botones destructivos deben usarse con moderación y preferiblemente requerir confirmación.
- Los DataGridView son **de solo lectura** para selección de fila.
- Los estados de las órdenes se muestran con **indicadores de color**.
- La política para Clientes y Dispositivos es de **baja lógica** (botón "Cambiar estado") y no de borrado físico.
- Los formularios deben tener coherencia visual con los mockups: logo, colores, tipografía.

### Formularios principales y su funcionalidad
| Formulario | Funcionalidad clave |
|---|---|
| **Dashboard** | Muestra 5 métricas (no 3 como indicaba el documento de arquitectura), estado de órdenes con indicadores de color, acceso rápido por rol. |
| **Clientes** | Alta, edición, búsqueda, baja lógica ("Cambiar estado"). |
| **Dispositivos** | Alta, edición, búsqueda, baja lógica ("Cambiar estado"). |
| **Órdenes de servicio** | Crear (estado = Pendiente), asignar técnico, actualizar estado, ver historial. |
| **Pagos** | Registrar pagos (≥ 0 para costo en orden, > 0 para monto de pago), seleccionar método (Efectivo/Tarjeta/Transferencia). |
| **Técnicos** | Alta, edición, búsqueda, gestión de estado (Activo/Inactivo). |
| **Reportes** | Filtros, generación, exportación, totales por estado. Contiene contenido funcional aunque la arquitectura lo tenía como "por definir". |
| **Login** | Validación de credenciales con mensaje de error "Credenciales inválidas" en caso fallido. |
| **Usuarios** | Gestión de usuarios, roles, y técnico asociado. |

### Métricas del Dashboard (5, confirmadas)
Las métricas del Dashboard deben ser exactamente 5, no 3 como indicaba el documento de arquitectura. Validar el listado exacto en `Contexto/03_dashboard_metrics.md`.

---

## 7. Reglas de negocio

1. El flujo operativo principal es: `Cliente → Dispositivo → Orden de servicio → Técnico → Diagnóstico/Reparación → Estado → Pago → Entrega`.
2. El costo del servicio debe ser ≥ 0.
3. El monto de un pago debe ser > 0.
4. Una orden puede tener múltiples pagos asociados.
5. No se puede eliminar un cliente con dispositivos registrados (ON DELETE NO ACTION).
6. El estado de una orden se establece en "Pendiente" al crearla.
7. Los clientes y dispositivos se dan de baja lógica ("Cambiar estado"), no se borran físicamente.
8. Un técnico puede estar asociado a como máximo un usuario.
9. Los técnicos inactivos pueden seguir teniendo órdenes asignadas históricamente.
10. El flujo de estados de órdenes no está enforceado como camino obligatorio por la BD.

---

## 8. Conflictos sin resolver

Los siguientes conflictos fueron identificados en el análisis y **NO deben resolverse unilateramente** en la implementación. Deben documentarse y escalarse:

| # | Conflicto | Fuentes |
|---|---|---|
| 1 | "Cambiar estado" vs "Eliminar" en Clientes/Dispositivos | Mockups vs Documento de Arquitectura |
| 2 | ID numérico INT IDENTITY (BD) vs prefijos alfanuméricos (mockups) | BD vs Mockups |
| 3 | 3 métricas en arquitectura vs 5 en Dashboard mockup | Documento Arquitectura vs Mockups |
| 4 | Rol "Empleado" vs "Empleado Recepcionista" | Arquitectura vs Mockups |
| 5 | Columna "Técnico asociado" en Usuarios no mencionada en arquitectura | Mockups vs Documento Arquitectura |
| 6 | Reportes: "Por definir" en arquitectura vs funcional en mockup | Documento Arquitectura vs Mockups |
| 7 | Nombre exacto del rol "Recepcionista" y si es sinónimo o rol separado | Mockups vs BD |
| 8 | Si el flujo de estados de órdenes es obligatorio o libre | BD vs Inferido |

> Estos conflictos deben resolverse por el equipo o el cliente antes de implementar las áreas afectadas.

---

## 9. Código y calidad

- Todo el código debe compilar sin errores ni advertencias en Visual Studio.
- Seguir las convenciones de nomenclatura de C# (PascalCase para clases, camelCase para variables, etc.).
- Los comentarios en el código deben ser mínimos y solo donde la lógica no sea autocomprehensible.
- Toda consulta SQL debe usar parámetros para evitar inyección SQL.
- Las conexiones a la base de datos deben cerrarse siempre (usar `using`).
- Las excepciones deben manejarse de forma elegante con mensajes al usuario.
- No se deben hardcodear cadenas de conexión, credenciales o valores de configuración.

---

## 10. Fuentes de referencia

Toda implementación debe basarse en:
- `Contexto/` — Documentación de análisis (fuente de verdad).
- `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Estructura real de la BD.
- `Entregables/Entregable_1/mockups/` — Prototipos visuales.
- `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Manual de identidad visual.
- `Entregables/Entregable_1/FixTrack_Manual_Usuarios_Entregable.pdf` — Manual de usuario.
- `Entregables/Entregable_1/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Documento de arquitectura (con conflictos documentados).

---

## 11. Límites y prohibiciones

- **NO** implementar funcionalidades, tablas, campos o reglas que no estén documentados en `Contexto/` o justificados explícitamente por el equipo/cliente.
- **NO** modificar archivos en `Entregables/Entregable_1/`.
- **NO** resolver conflictos sin resolver unilateralmente sin documentar la decisión en `Contexto/`.
- **NO** introducir tecnologías, frameworks o bibliotecas no aprobadas (ver sección 1).
- **NO** cambiar la estructura de la base de datos sin actualizar la documentación en `Contexto/`.
- **NO** asumir que las versiones mockup y el documento de arquitectura están correctos — siempre verificar contra la base de datos real y los mockups.
