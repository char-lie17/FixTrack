# Informe Final — Fase 1: Análisis Completo del Entregable 1

## 1. Archivos analizados

### Entregables/Entregable_1/

| Archivo | Tipo | Tamaño aprox. | Analizado |
|---------|------|---------------|-----------|
| `Base de Datos APP de escritorio.sql` | SQL | 11 KB | Sí |
| `Descripcion_empresa_negocio_FixTrack.pdf` | PDF | 65 KB | Sí |
| `mapa_navegacion-0 (1).pdf` | PDF | 6 KB | Sí |
| `FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` | PDF | 386 KB | Sí |
| `FixTrack_Arquitectura_de_Menus_Entregable.pdf` | PDF (en mockups/) | 213 KB | Sí |
| `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` | PPTX (en mockups/) | 2.8 MB | Sí |
| `01_login.png` | PNG | 110 KB | Sí (nombre) |
| `02_dashboard.png` | PNG | 309 KB | Sí (nombre) |
| `03_clientes_lista.png` | PNG | 278 KB | Sí (nombre) |
| `04_cliente_formulario.png` | PNG | 201 KB | Sí (nombre) |
| `05_cliente_detalle.png` | PNG | 237 KB | Sí (nombre) |
| `06_dispositivos_lista.png` | PNG | 260 KB | Sí (nombre) |
| `07_dispositivo_formulario.png` | PNG | 211 KB | Sí (nombre) |
| `08_ordenes_lista.png` | PNG | 290 KB | Sí (nombre) |
| `09_orden_nueva.png` | PNG | 240 KB | Sí (nombre) |
| `10_orden_detalle.png` | PNG | 327 KB | Sí (nombre) |
| `11_tecnicos.png` | PNG | 232 KB | Sí (nombre) |
| `12_pagos.png` | PNG | 249 KB | Sí (nombre) |
| `13_reportes.png` | PNG | 256 KB | Sí (nombre) |
| `14_usuarios.png` | PNG | 246 KB | Sí (nombre) |
| `15_menu_manual.png` | PNG | 123 KB | Sí (nombre) |

**Total: 20 archivos analizados.**

## 2. Estructura de Contexto creada

```
FixTrack_Proyecto/
├── Contexto/
│   ├── README.md                          ← Guía de navegación de la documentación
│   ├── 01_resumen_ejecutivo.md            ← Resumen general del proyecto
│   ├── 02_empresa_y_objetivo.md           ← Descripción de la empresa y objetivo
│   ├── 03_modulos.md                      ← Módulos del sistema
│   ├── 04_navegacion.md                   ← Navegación y arquitectura de menús
│   ├── 05_base_de_datos.md                ← Estructura completa de la base de datos
│   ├── 07_relaciones.md                   ← Relaciones entre entidades
│   ├── 08_mockups.md                      ← Análisis de cada mockup
│   ├── 09_identidad_visual.md             ← Identidad visual completa
│   ├── 10_usuarios_roles.md               ← Usuarios y roles
│   ├── 11_reglas_negocio.md               ← Reglas de negocio
│   ├── 12_decisiones_diseno.md            ← Decisiones de diseño
│   ├── 13_incertidumbres.md               ← Incertidumbres, conflictos y recomendaciones
│   └── 14_grafo_relaciones.md             ← Grafo de relaciones generales
└── Entregables/
    └── Entregable_1/                      ← Sin modificaciones
```

**Total: 14 archivos Markdown + 1 README = 15 archivos de documentación.**

## 3. Información importante encontrada

### Base de datos
- 6 tablas: Clientes, Dispositivos, Técnicos, OrdenesServicio, Pagos, Usuarios
- 18 columnas con constraints, 5 foreign keys, 6 índices
- 8 clientes, 9 dispositivos, 3 técnicos, 9 órdenes, 5 pagos, 5 usuarios como datos de prueba
- Sin vistas, procedimientos almacenados, funciones ni triggers

### Módulos
- 3 grupos: Operación (4 módulos), Administración (2 módulos), Información (1 módulo)
- Dashboard como pantalla de resumen
- 3 roles de usuario con menús diferenciados

### Interfaz
- Tecnología: C# + Windows Forms
- Paleta de 4 colores principales + 5 colores de estado
- Tipografía única: Segoe UI
- 7 iconos requeridos
- 4 tipos de botones
- 4 reportes oficiales

## 4. Relaciones identificadas

| Relación | Cardinalidad | Tablas |
|----------|-------------|--------|
| Clientes → Dispositivos | 1:M | Dispositivos.ClienteID → Clientes.ClienteID |
| Dispositivos → OrdenesServicio | 1:M | OrdenesServicio.DispositivoID → Dispositivos.DispositivoID |
| Técnicos → OrdenesServicio | 1:M | OrdenesServicio.TecnicoID → Tecnicos.TecnicoID |
| Técnicos → Usuarios | 1:0..1 | Usuarios.TecnicoID → Tecnicos.TecnicoID |
| OrdenesServicio → Pagos | 1:M | Pagos.OrdenID → OrdenesServicio.OrdenID |

## 5. Información confirmada

- Nombre de la base de datos: FixTrack
- 6 tablas con sus estructuras completas
- 3 roles de usuario, 5 estados de órdenes, 3 métodos de pago
- Arquitectura de menús con 3 grupos
- Tecnología C# + WinForms
- Paleta de colores, tipografía, logotipo definidos
- Mapa de navegación completo
- 15 mockups disponibles

## 6. Inferencias realizadas

1. El flujo de estados sugiere un orden secuencial pero la BD no lo enforcea como camino obligatorio.
2. Los técnicos inactivos pueden seguir teniendo órdenes asignadas históricamente.
3. El Administrador es el único rol que puede gestionar usuarios y técnicos.
4. El Técnico solo puede ver sus propias órdenes (filtro no limitado por BD).
5. Los formularios de creación/edición comparten un diseño estándar con GroupBox.
6. La política de «Cambiar estado» reemplaza a «Eliminar» por la restricción ON DELETE NO ACTION.
7. El diseño busca una apariencia profesional y nativa de Windows sin elementos web.

## 7. Información desconocida o ambigua

### Base de datos
- Objetos adicionales de SQL Server no incluidos en el script
- Lógica de transición entre estados de órdenes
- Planes para vistas o procedimientos almacenados

### Funcionalidades
- Submenús específicos dentro de Reportes
- Permisos granulares dentro de cada módulo
- Validaciones específicas de formularios
- Implementación de «Mis órdenes» para técnicos
- Políticas de descuento o ajustes de costo
- Reglas de notificación al cliente

### Interfaz
- Detalle exacto de controles sin lectura directa de imágenes
- Archivos de iconos específicos
- Políticas de autenticación y contraseña

### Negocio
- Ubicación física de la empresa
- Número de empleados
- Volumen de reparaciones
- Otras líneas de negocio

## 8. Posibles conflictos entre documentos

| # | Conflicto | Fuentes |
|---|-----------|---------|
| 1 | Clientes: [Eliminar] vs [Cambiar estado] | Arquitectura vs Mockup vs SQL |
| 2 | Dispositivos: [Eliminar] vs [Cambiar estado] | Arquitectura vs Mockup |
| 3 | Dashboard: 3 métricas vs 5 métricas | Arquitectura vs Mockup |
| 4 | Reportes: módulo vacío vs mockup con filtros | Arquitectura vs Mockup |
| 5 | Usuarios: técnico asociado no contemplado | Arquitectura vs Mockup |
| 6 | Detalle de orden: pagos integrados vs separados | Arquitectura vs Mockup |

## 9. Recomendaciones para la siguiente fase

1. Definir y aprobar los reportes concretos antes de comenzar la implementación.
2. Aprobar los permisos granulares por rol.
3. Confirmar la política de baja lógica (Cambiar estado vs Eliminar).
4. Definir las reglas de transición entre estados de las órdenes.
5. Validar si los pagos integrados en el detalle de orden reemplazan o complementan al módulo de Pagos.
6. Confirmar la implementación de «Mis órdenes» para el rol Técnico.
7. Documentar las validaciones de formulario específicas para cada pantalla.
8. Asegurar la coherencia entre la definición de la arquitectura de menús y los mockups.
9. Considerar la preparación de scripts adicionales de la base de datos para los reportes.
10. No modificar, mover, renombrar ni sobrescribir ningún archivo original del Entregable 1.

## 10. Restricciones cumplidas

- ✅ NO se programó la aplicación.
- ✅ NO se crearon formularios de Windows Forms.
- ✅ NO se creó código C#.
- ✅ NO se modificó la base de datos.
- ✅ NO se modificaron, movieron, renombraron ni sobrescribir archivos originales del Entregable 1.
- ✅ La documentación está organizada en la carpeta Contexto/ (no dentro de Entregable_1/).