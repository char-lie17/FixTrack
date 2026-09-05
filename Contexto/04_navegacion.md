# Navegación y Arquitectura de Menús

## Flujo general de navegación

```
INICIO DE SESIÓN → Autenticación
                        ↓
               DASHBOARD / INICIO
                        ↓
               MENÚ LATERAL
                        ↓
         ┌──────────────┼──────────────┐
         ↓              ↓              ↓
     OPERACIÓN    ADMINISTRACIÓN    INFORMACIÓN
    ├ Clientes     ├ Técnicos       └ Reportes
    ├ Dispositivos ├ Usuarios
    ├ Órdenes
    └ Pagos
                        ↓
               PANTALLAS DE GESTIÓN
                        ↓
               CERRAR SESIÓN
                        ↓
               INICIO DE SESIÓN
```

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 8-9

## Menú lateral

La navegación principal utiliza un menú lateral que permanece disponible mientras el usuario navega. La barra lateral contiene:

```
┌──────────────────────────────────────────────────────────────┐
│ FIXTRACK                                                     │
│  Inicio                                                      │
│  OPERACIÓN                   CONTENIDO DE LA PANTALLA        │
│    Clientes                                                  │
│    Dispositivos                                              │
│    Órdenes                                                   │
│    Pagos                                                     │
│  ADMINISTRACIÓN                                              │
│    Técnicos                                                  │
│    Usuarios                                                  │
│  INFORMACIÓN                                                 │
│    Reportes                                                  │
│  Cerrar sesión                                               │
└──────────────────────────────────────────────────────────────┘
```

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 4

## Menú según tipo de usuario

| Rol | Accesos | Justificación |
|-----|---------|---------------|
| **Administrador** | Inicio · Clientes · Dispositivos · Órdenes de servicio · Pagos · Técnicos · Usuarios · Reportes · Cerrar sesión | Tendrá acceso general |
| **Empleado / Recepcionista** | Inicio · Clientes · Dispositivos · Órdenes de servicio · Pagos · Reportes · Cerrar sesión | Su menú estará orientado a la operación |
| **Técnico** | Inicio · Mis órdenes · Actualizar servicio · Cerrar sesión | Concentrarse en las órdenes asignadas |

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 7

> **Nota:** Los permisos exactos todavía son parte pendiente del proyecto y no deben presentarse como reglas definitivas de seguridad hasta que el grupo los apruebe.

## Mapa de navegación (descripción textual)

El mapa de navegación original muestra los siguientes elementos y su flujo:

1. **INICIO DE SESIÓN** — Pantalla de autenticación (usuario y contraseña)
2. **DASHBOARD** — Pantalla principal después de autenticación exitosa
3. **CLIENTES** — Listado de clientes, Nuevo/Editar cliente, Detalle del cliente
4. **DISPOSITIVOS** — Listado de dispositivos, Nuevo/Editar dispositivo
5. **ORDENES DE SERVICIO** — Listado de órdenes, Nueva orden, Detalle de orden (estado)
6. **TECNICOS** — Listado de técnicos, Nuevo/Editar técnico
7. **PAGOS** — Listado de pagos, Registrar pago
8. **REPORTES** — Panel de reportes
9. **USUARIOS** — Listado de usuarios, Nuevo/Editar usuario

**Fuente:** `Entregables/Entregable_1/mapa_navegacion-0 (1).pdf` — Página 1

## Relación entre navegación y módulos

| Elemento de navegación | Módulo correspondiente | Pantalla | Fuente |
|------------------------|------------------------|----------|--------|
| Inicio | Dashboard | Dashboard | Mapa de navegación |
| Clientes | Clientes | Listado, Formulario, Detalle | Mapa de navegación + Arquitectura |
| Dispositivos | Dispositivos | Listado, Formulario | Mapa de navegación + Arquitectura |
| Órdenes de servicio | Órdenes de Servicio | Listado, Nueva orden, Detalle | Mapa de navegación + Arquitectura |
| Pagos | Pagos | Listado, Registrar pago | Mapa de navegación + Arquitectura |
| Técnicos | Técnicos | Listado, Formulario | Mapa de navegación + Arquitectura |
| Reportes | Reportes | Panel de reportes | Mapa de navegación + Arquitectura |
| Usuarios | Usuarios | Listado, Formulario | Mapa de navegación + Arquitectura |

## Información confirmada

- **CONFIRMADO:** La navegación usa menú lateral como elemento principal.
- **CONFIRMADO:** Existen tres roles de usuario con diferentes menús.
- **CONFIRMADO:** El flujo es: Login → Dashboard → Menú lateral → Pantallas de gestión → Cerrar sesión.

## Información inferida

- **INFERIDO:** El técnico tiene acceso limitado porque solo necesita ver y actualizar sus órdenes asignadas.

## Información desconocida

- **DESCONOCIDO:** Submenús específicos dentro de Reportes.
- **DESCONOCIDO:** Permisos exactos por rol (pendientes de aprobación del equipo).
- **DESCONOCIDO:** Si existen permisos granulares dentro de cada módulo (solo lectura vs edición completa).