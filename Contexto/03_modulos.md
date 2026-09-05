# Módulos del Sistema

FixTrack está organizado en módulos agrupados en tres categorías principales dentro del menú lateral, más el Dashboard y la opción de Cerrar sesión.

## Estructura de módulos

### Grupo «Operación»

Módulos directamente relacionados con el proceso operativo de una reparación:

| Módulo | Descripción | Fuente |
|--------|-------------|--------|
| **Clientes** | Registro y administración de clientes del taller | Arquitectura de Menús — Sección 6 |
| **Dispositivos** | Control de equipos recibidos para reparación | Arquitectura de Menús — Sección 7 |
| **Órdenes de Servicio** | Seguimiento completo de cada reparación | Arquitectura de Menús — Sección 8 |
| **Pagos** | Registro de pagos asociados a órdenes de servicio | Arquitectura de Menús — Sección 9 |

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 5

### Grupo «Administración»

Elementos relacionados con la administración de las personas que utilizan o trabajan dentro del sistema:

| Módulo | Descripción | Fuente |
|--------|-------------|--------|
| **Técnicos** | Gestión del personal técnico | Arquitectura de Menús — Sección 11 |
| **Usuarios** | Gestión de cuentas de acceso al sistema | Arquitectura de Menús — Sección 12 |

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 6-7

### Grupo «Información»

| Módulo | Descripción | Fuente |
|--------|-------------|--------|
| **Reportes** | Panel de reportes (módulo en crecimiento) | Arquitectura de Menús — Sección 14 |

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 7

## Dashboard

Pantalla principal que se muestra después del login exitoso. Proporciona una vista rápida del estado actual del negocio:
- Métricas por estado de órdenes
- Lista de órdenes recientes

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 3

## Flujo del proceso de negocio

```
Cliente → Dispositivo → Orden de servicio → Técnico → Diagnóstico / reparación → Estado → Pago → Entrega
```

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 5

## Información confirmada

- **CONFIRMADO:** Los módulos están agrupados en Operación, Administración e Información.
- **CONFIRMADO:** El Dashboard no es un módulo de gestión, sino una vista de resumen.
- **CONFIRMADO:** Reportes tiene contenido funcional en el mockup (filtros de fecha, botón Generar, tabla de resultados con totales por estado).
- **CONFIRMADO:** Los mockups usan prefijos alfanuméricos para los IDs (C-, D-, T-, ORD-, PAG-, U-).

## Información inferida

- **INFERIDO:** Los módulos de Operación son los de mayor uso diario del sistema.
- **INFERIDO:** Los módulos de Administración son accedidos principalmente por el rol Administrador.

## Información desconocida

- **DESCONOCIDO:** Submenús específicos dentro de Reportes (el mockup muestra contenido pero no se ha definido un conjunto completo de submenús).
- **DESCONOCIDO:** Si los prefijos de ID son un requisito de la base de datos o solo un formato de visualización.