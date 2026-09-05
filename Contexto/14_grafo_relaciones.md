# Grafo de Relaciones Generales

Este grafo muestra cómo se conectan todas las partes del sistema FixTrack.

```
PROYECTO FIXTRACK
│
├→ EMPRESA
│   └→ TecnoFix Solutions
│       └→ Reparación y mantenimiento de equipos electrónicos
│
├→ USUARIOS Y ROLES
│   ├→ Administrador
│   ├→ Empleado / Recepcionista
│   └→ Técnico
│
├→ MÓDULOS (menú lateral)
│   │
│   ├→ OPERACIÓN
│   │   ├→ Clientes → Formulario → Detalle
│   │   ├→ Dispositivos → Formulario → Detalle
│   │   ├→ Órdenes de Servicio → Listado → Nueva orden → Detalle
│   │   └→ Pagos → Listado → Registrar pago
│   │
│   ├→ ADMINISTRACIÓN
│   │   ├→ Técnicos → Formulario → Detalle
│   │   └→ Usuarios → Formulario → Detalle
│   │
│   └→ INFORMACIÓN
│       └→ Reportes
│
├→ NAVEGACIÓN
│   ├→ INICIO DE SESIÓN → Autenticación
│   ├→ DASHBOARD → Vista de resumen
│   ├→ MENÚ LATERAL → Selección de módulo
│   ├→ PANTALLAS DE GESTIÓN → Acciones específicas
│   └→ CERRAR SESIÓN → INICIO DE SESIÓN
│
├→ BASE DE DATOS (FixTrack)
│   ├→ Clientes (1) ──── (M) Dispositivos
│   │       │
│   │       └─── (1) ──── (M) OrdenesServicio
│   │               │
│   │               ├─── (M) Pagos
│   │               └─── (M) Técnicos (0..1)
│   │                       └─── (1) Usuarios
│   │
│   └→ Datos de prueba (8 clientes, 9 dispositivos, 3 técnicos, 9 órdenes, 5 pagos, 5 usuarios)
│
├→ IDENTIDAD VISUAL
│   ├→ Colores: Primario #2C5F8A, Secundario #FF6B35, Terciario #2B2D42, Neutro #F4F6F8
│   ├→ Tipografía: Segoe UI
│   ├→ Logotipo: 4 variantes
│   ├→ Controles: MenuStrip, DataGridView, TextBox, Button, GroupBox
│   └→ Estados de órdenes: Pendiente, En diagnóstico, En reparación, Listo, Entregado
│
└→ IMPLEMENTACIÓN
    └→ C# + Windows Forms (WinForms)
```

## Flujo de datos

```
CLIENTE ingresa → LOGIN (identificación de rol)
    → DASHBOARD (resumen por estado)
    → Selecciona módulo desde MENÚ LATERAL
    → PANTALLA DE GESTIÓN (CRUD sobre tablas)
    → Opera (crea/edita/elimina/consulta)
    → Cambia estado de órdenes
    → Registra pagos
    → CERRAR SESIÓN → LOGIN
```

## Mapeo de módulos a tablas de la base de datos

| Módulo | Tabla(s) principal(es) | Relación con otras tablas |
|--------|------------------------|---------------------------|
| Clientes | Clientes | 1:M → Dispositivos |
| Dispositivos | Dispositivos | M:1 ← Clientes, 1:M → OrdenesServicio |
| Órdenes de Servicio | OrdenesServicio | M:1 ← Dispositivos, M:1 ← Técnicos, 1:M → Pagos |
| Pagos | Pagos | M:1 ← OrdenesServicio |
| Técnicos | Técnicos | 1:M → OrdenesServicio, 0..1 → Usuarios |
| Usuarios | Usuarios | 0..1 ← Técnicos |
| Reportes | Todas las tablas | Consultas combinadas |

## Mapeo de pantallas a operaciones CRUD

| Pantalla | Entidad | Operaciones CRUD disponibles |
|----------|---------|------------------------------|
| Login | Usuarios | Autenticación |
| Dashboard | OrdenesServicio | Lectura (métricas y listado) |
| Clientes (listado) | Clientes | Crear, Leer, Actualizar estado |
| Cliente (formulario) | Clientes | Crear, Editar |
| Detalle cliente | Clientes | Leer |
| Dispositivos (listado) | Dispositivos | Crear, Leer, Actualizar estado |
| Dispositivo (formulario) | Dispositivos | Crear, Editar |
| Órdenes (listado) | OrdenesServicio | Crear, Leer, Actualizar estado |
| Nueva orden | OrdenesServicio | Crear |
| Detalle orden | OrdenesServicio | Leer, Editar (diagnóstico, trabajo, costo) |
| Pagos (listado) | Pagos | Crear, Leer |
| Técnicos (listado) | Técnicos | Crear, Leer, Editar |
| Usuarios (listado) | Usuarios | Crear, Editar, Cambiar estado |
| Reportes | Todas | Leer (consultas) |

## Fuentes primarias

| Documento | Contenido | Ruta |
|-----------|-----------|------|
| SQL Script | Estructura de base de datos | `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` |
| Empresa PDF | Descripción de la empresa | `Entregables/Entregable_1/Descripcion_empresa_negocio_FixTrack.pdf` |
| Navegación PDF | Mapa de navegación | `Entregables/Entregable_1/mapa_navegacion-0 (1).pdf` |
| Arquitectura PDF | Definición de menús | `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` |
| Identidad Visual PDF | Manual de diseño | `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` |
| Comparativa PPTX | Mockups vs Arquitectura | `Entregables/Entregable_1/mockups/FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` |
| PNGs (15 archivos) | Mockups visuales | `Entregables/Entregable_1/mockups/*.png` |