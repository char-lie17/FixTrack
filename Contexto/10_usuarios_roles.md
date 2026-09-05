# Usuarios y Roles

## Roles definidos

El sistema contempla tres tipos de usuario:

| Rol | Nombre en BD | Descripción | Fuente |
|-----|--------------|-------------|--------|
| **Administrador** | Administrador | Acceso general a todos los módulos | SQL: CK_Usuarios_Rol |
| **Empleado / Recepcionista** | Empleado | Orientado a la operación | Arquitectura — Sección 13 |
| **Técnico** | Tecnico | Se concentra en sus órdenes asignadas | Arquitectura — Sección 13 |

**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Línea 135
**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 7

## Datos de prueba de usuarios

| NombreUsuario | Rol | Estado | TecnicoID | Fuente |
|---------------|-----|--------|-----------|--------|
| admin | Administrador | Activo | NULL | Línea 198 |
| recepcion1 | Empleado | Activo | NULL | Línea 199 |
| luis.ortega | Tecnico | Activo | 1 | Línea 200 |
| karla.vega | Tecnico | Activo | 2 | Línea 201 |
| diego.salinas | Tecnico | Inactivo | 3 | Línea 202 |

## Accesos por rol

| Rol | Accesos |
|-----|---------|
| **Administrador** | Inicio · Clientes · Dispositivos · Órdenes de servicio · Pagos · Técnicos · Usuarios · Reportes · Cerrar sesión |
| **Empleado / Recepcionista** | Inicio · Clientes · Dispositivos · Órdenes de servicio · Pagos · Reportes · Cerrar sesión |
| **Técnico** | Inicio · Mis órdenes · Actualizar servicio · Cerrar sesión |

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 7

## Relación con la base de datos

- La tabla `Usuarios` almacena el rol como VARCHAR(30) con check constraint.
- Los técnicos se asocian a usuarios mediante `Usuarios.TecnicoID → Tecnicos.TecnicoID`.
- Existe un índice único filtrado `UQ_Usuarios_TecnicoID` que garantiza que un técnico esté asociado a como máximo un usuario.

## Información confirmada

- **CONFIRMADO:** Los roles son exactamente 3: Administrador, Empleado, Técnico.
- **CONFIRMADO:** Técnico y Usuario tienen relación 1:0..1.
- **CONFIRMADO:** El Empleado/Recepcionista NO tiene acceso a Técnicos ni Usuarios.

## Información inferida

- **INFERIDO:** El Administrador es el único rol que puede gestionar usuarios y técnicos.
- **INFERIDO:** El Técnico solo puede ver sus propias órdenes (aunque la BD no limita esto con una constraint).

## Información desconocida

- **DESCONOCIDO:** Si existe autenticación multifactor o políticas de contraseña.
- **DESCONOCIDO:** Si hay permisos granulares dentro de cada módulo.
- **DESCONOCIDO:** Cómo se implementa la restricción «Mis órdenes» para el técnico (¿filtro por usuario logueado?).
- **DESCONOCIDO:** Si los usuarios Empleado pueden crear usuarios o técnicos.