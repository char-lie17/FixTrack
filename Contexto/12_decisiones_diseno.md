# Decisiones de Diseño

## Decisión 1: Menú lateral en lugar de otro esquema de navegación

**Razón:** FixTrack tiene varios módulos y el menú lateral permite mantenerlos visibles y organizados sin ocupar demasiado espacio horizontal. La barra lateral permanece disponible mientras el usuario navega.

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 4

## Decisión 2: Agrupación por procesos (no por tablas)

**Razón:** La arquitectura de menús está diseñada alrededor de los procesos reales de TecnoFix Solutions, no alrededor de las tablas de la base de datos. Los módulos agrupan funcionalidades relacionadas.

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 3

## Decisión 3: Pantalla de gestión con acciones integradas

**Razón:** Registrar, editar, eliminar y consultar son acciones sobre las entidades, no módulos diferentes. El menú indica qué área del sistema se está utilizando y la pantalla de gestión proporciona las acciones disponibles dentro de esa área.

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 5

## Decisión 4: Sin submenús para órdenes de servicio

**Razón:** No se necesitan submenús como Nueva orden, Seguimiento, Actualizar estado o Consultar orden: todas esas acciones pertenecen a la gestión de una orden. El seguimiento ocurre dentro de la gestión de la orden, especialmente mediante su estado.

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 5

## Decisión 5: Reportes sin submenús definidos

**Razón:** El equipo todavía no ha definido cuáles serán los reportes concretos. Reportes es un módulo preparado para crecer, sin inventar funciones que todavía no han sido aprobadas.

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 7

## Decisión 6: Cerrar sesión separado visualmente

**Razón:** No es un módulo de negocio, por lo que no pertenece a Operación, Administración ni Información.

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 8

## Decisión 7: Cambiar estado en lugar de Eliminar

**Razón:** El mockup implementa baja lógica (Cambiar estado) en lugar de borrado físico (Eliminar) para Clientes y Dispositivos. Esto es consistente con la decisión de la BD de usar ON DELETE NO ACTION.

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slides 3, 4

## Decisión 8: C# + Windows Forms

**Razón:** El manual de identidad visual está diseñado exclusivamente para formularios Windows Forms. La asignatura es Programación de Aplicaciones de Escritorio.

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 1-2

## Decisión 9: Segoe UI como única fuente tipográfica

**Razón:** Es la fuente sans-serif predeterminada de Windows, optimizada para interfaces de usuario en pantalla. Su amplio soporte de pesos permite establecer jerarquías visuales sin fuentes adicionales. Al ser nativa del sistema operativo, no requiere instalación ni licenciamiento.

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 4

## Decisión 10: Dashboard basado en datos existentes

**Razón:** El Dashboard se compone de información existente en la base de datos (órdenes recientes, conteo por estado), no de métricas inventadas.

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 13

## Decisión 11: Identidad visual no decorativa

**Razón:** La identidad visual no es un elemento decorativo: es una herramienta práctica que facilita el desarrollo y proyecta profesionalismo en el entregable académico.

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 2

## Información confirmada

- **CONFIRMADO:** Todas las decisiones anteriores están explícitamente documentadas en las fuentes.
- **CONFIRMADO:** La arquitectura está basada en procesos, no en tablas.

## Información inferida

- **INFERIDO:** Las decisiones buscan facilitar el mantenimiento y la consistencia visual.

## Información desconocida

- **DESCONOCIDO:** Si existen decisiones de diseño adicionales no documentadas.
- **DESCONOCIDO:** Criterios para la elección de WinForms sobre otras tecnologías de escritorio.