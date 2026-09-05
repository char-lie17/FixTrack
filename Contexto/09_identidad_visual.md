# Identidad Visual

## Tecnología

FixTrack se implementa con **C#** y **Windows Forms (WinForms)** dentro de la asignatura Programación de Aplicaciones de Escritorio.

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 1-2

## Atributos de identidad

La identidad visual transmite cinco atributos:

1. **Tecnología** — mediante una paleta azul acero
2. **Profesionalismo** — mediante tipografías limpias y componentes bien definidos
3. **Confianza** — conseguida con colores estables y estados visuales claros
4. **Orden** — logrado con jerarquías consistentes y márgenes uniformes
5. **Facilidad de uso** — reflejada en iconos simples y una estructura de interfaz predecible

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 2

## Paleta de colores

### Colores principales

| Nombre | Color Hex | Uso |
|--------|-----------|-----|
| **Primario** | `#2C5F8A` | Barra MenuStrip, encabezados de tabla, títulos de sección, botones secundarios |
| **Secundario** | `#FF6B35` | Botones principales: guardar, nueva orden, cobrar |
| **Terciario** | `#2B2D42` | Texto general, iconos, bordes, barra lateral de navegación |
| **Neutro** | `#F4F6F8` | Filas alternas de DataGridView, fondos de GroupBox |

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 5-6

### Estados de órdenes de servicio

| Estado | Color Hex | Significado |
|--------|-----------|-------------|
| Pendiente | `#D64545` (Rojo) | La orden fue creada pero aún no fue revisada por un técnico |
| En diagnóstico | `#00A8E8` (Azul claro) | Un técnico está evaluando el dispositivo para determinar la falla |
| En reparación | `#F5A623` (Naranja) | El dispositivo está siendo reparado activamente |
| Listo | `#2E7D32` (Verde) | La reparación está terminada, pendiente de entrega al cliente |
| Entregado | `#2B2D42` (Gris oscuro) | El dispositivo fue entregado al cliente; la orden está cerrada |

> **Nota:** Estos colores se usan exclusivamente para representar el estado de las órdenes de servicio. No deben usarse con otros propósitos.

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 6

## Tipografía

**Fuente única:** Segoe UI (única familia tipográfica del sistema). Pesos disponibles: Light, SemiLight, Regular, SemiBold, Bold.

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 4

### Jerarquía tipográfica

| Uso | Fuente | Tamaño | Peso | Color | Fuente |
|-----|--------|--------|------|-------|--------|
| Título de ventana | Segoe UI | 11-12 pt | Bold | Terciario (#2B2D42) | Página 5 |
| Título de sección | Segoe UI | 13-14 pt | SemiBold | Primario (#2C5F8A) | Página 5 |
| Subtítulo | Segoe UI | 11-12 pt | SemiBold | Terciario (#2B2D42) | Página 5 |
| Texto normal | Segoe UI | 9-10 pt | Regular | Terciario (#2B2D42) | Página 5 |
| Texto secundario | Segoe UI | 8 pt | Regular | Gris medio (#6B7280) | Página 5 |
| Texto de botón | Segoe UI | 9 pt | SemiBold | Blanco sobre fondo de color | Página 4 |
| Texto de tabla | Segoe UI | 8.5-9 pt | Regular | Terciario (#2B2D42) | Página 4 |

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 4-5

## Logotipo

El logotipo está compuesto por un icono con la letra F dentro de un cuadrado redondeado de color azul primario, y el nombre tipográfico «FixTrack» acompañado de «TecnoFix Solutions».

**Variantes:**
1. **Principal** — Formulario de login y pantalla de inicio de la aplicación
2. **Horizontal** — Barra superior de navegación (MenuStrip), encabezado de reportes, pie de página de informes
3. **Monocromática** — Contextos donde no se puede usar el color primario (impresión en escala de grises)
4. **En negativo** — Solo sobre fondos oscuros

**Área de protección:** Equivale a la altura de la letra F del icono (denominada X). Ningún texto, icono, borde ni otro elemento debe invadir esta zona.

**Tamaño mínimo:** Icono 24 px, texto «FixTrack» 10 pt.

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 3-4

## Controles WinForms

### MenuStrip
- Fondo terciario (#2B2D42), texto blanco, iconos 24 px, fuente Segoe UI SemiBold 9 pt
- Opción activa con fondo ligeramente más claro

### ToolStrip
- Fondo blanco, borde inferior, botones con iconos 24 px + texto herramientas

### DataGridView
- Encabezado: BackColor #2C5F8A, ForeColor White, SemiBold 8.5-9 pt
- Filas alternas: Neutro (#F4F6F8) / Blanco (#FFFFFF)
- Celdas: Terciario (#2B2D42), Regular 8.5-9 pt
- RowHeadersVisible: false, SelectionMode: FullRowSelect
- ReadOnly: true, BorderStyle: None o FixedSingle

### TextBox / ComboBox / DateTimePicker
- Fondo blanco, borde 1 px #D0D5DD, Segoe UI Regular 9.5 pt, alto 30 px
- Con foco: borde 1 px #2C5F8A
- Con error: borde 1 px #D64545
- Deshabilitado: borde 1 px #D0D5DD, fondo #F4F6F8

### Buttons
| Tipo | BackColor | ForeColor | Uso |
|------|-----------|-----------|-----|
| Principal | #FF6B35 | White | Guardar, crear, registrar, cobrar |
| Secundario | #2C5F8A | White | Buscar, consultar, ver detalles |
| Neutral | #F4F6F8 | #2B2D42 | Cancelar, cerrar, volver |
| Destructiva | #D64545 | White | Eliminar |

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 7-8

### Formulario estándar
- Título del formulario: Form.Text, Segoe UI Bold 11 pt, color terciario
- MenuStrip compartido en el formulario principal
- ToolStrip con botones de acciones frecuentes (nuevo, buscar)
- DataGridView con reglas visuales definidas
- GroupBox/Panel con borde 1 px #D0D5DD, Padding 12-16 px
- Altura estándar de campos: 30 px, separación vertical entre campos: 8-10 px

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 9-10

## Pantalla de Login

- Fondo blanco
- Panel central con logo en versión principal centrado arriba
- Dos campos: NombreUsuario y Password
- Botón principal «Iniciar sesión»
- Texto «TecnoFix Solutions» en 8 pt gris debajo
- Sin MenuStrip en esta pantalla

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 13

## Dashboard

- Título «Panel de control»
- Resumen de actividad con Labels para métricas clave
- DataGridView con órdenes de servicio más recientes
- Contenido organizado con GroupBox o Panel
- Se compone de información existente en la base de datos (órdenes recientes, conteo por estado)

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 13

## Iconografía

- Estilo: Lineal (outline), sin relleno
- Grosor del trazo: 1.5-2 px
- Forma: Geométrica y simplificada
- Esquinas: Ligeramente redondeadas (radio 1-2 px)
- Tamaño estándar: 24x24 px (MenuStrip y ToolStrip)
- Tamaño pequeño: 16x16 px (DataGridView y botones)
- Color: Terciario (#2B2D42) sobre fondos claros; blanco sobre fondos oscuros

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 7-8

## Iconos requeridos

| Icono | Módulo | Ubicación | Fuente |
|-------|--------|-----------|--------|
| Dashboard | Pantalla principal | MenuStrip, primera opción | Página 8 |
| Clientes | Módulo de clientes | MenuStrip > Gestión | Página 8 |
| Dispositivos | Módulo de dispositivos | MenuStrip > Gestión | Página 8 |
| Órdenes | Órdenes de servicio | MenuStrip > Gestión | Página 8 |
| Técnicos | Módulo de técnicos | MenuStrip > Gestión | Página 8 |
| Pagos | Módulo de pagos | MenuStrip, opción principal | Página 8 |
| Reportes | Reportes del sistema | MenuStrip, opción principal | Página 8 |
| Usuarios | Administración de usuarios | MenuStrip > Administración | Página 8 |

## Reportes del sistema

- Logo en versión horizontal en esquina superior izquierda
- Título del reporte en 14 pt Bold terciario
- Fecha de generación en 9 pt gris
- Tablas con mismas reglas que DataGridView
- Pie de página: «TecnoFix Solutions — FixTrack» centrado en 8 pt gris
- Número de página a la derecha

**Reportes oficiales:**
1. Órdenes por estado: cantidad de órdenes agrupadas por cada estado
2. Órdenes por técnico: distribución de órdenes asignadas a cada técnico
3. Servicios completados: órdenes con estado «Entregado» en un período determinado
4. Pagos registrados: pagos asociados a órdenes de servicio en un período determinado

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 14

## Información confirmada

- **CONFIRMADO:** La tecnología es C# + WinForms.
- **CONFIRMADO:** La familia tipográfica es Segoe UI.
- **CONFIRMADO:** Los colores principales son #2C5F8A, #FF6B35, #2B2D42, #F4F6F8.
- **CONFIRMADO:** Los colores de estado son 5, asignados a cada estado de orden.
- **CONFIRMADO:** El logo tiene 4 variantes oficiales.
- **CONFIRMADO:** Existen 4 tipos de botones definidos.
- **CONFIRMADO:** Hay 4 reportes oficiales definidos.

## Información inferida

- **INFERIDO:** El diseño busca una apariencia profesional y nativa de Windows, sin elementos web.

## Información desconocida

- **DESCONOCIDO:** Archivos de iconos específicos (formato .ico/.png con transparencia).
- **DESCONOCIDO:** Archivos de imagen del logo en resoluciones específicas.