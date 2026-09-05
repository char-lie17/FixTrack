# DiagnÃ³stico TÃ©cnico y Plan de CorrecciÃ³n

**Fecha:** 05/09/2026  
**Autor:** Carlos Castillo  
**Commit base:** `98ef50f` (snapshot inicial Henry Creel)

---

## ðŸ” Resumen Ejecutivo

Este documento detalla los bugs identificados en el snapshot inicial del proyecto FixTrack, su impacto, causa raÃ­z y plan de correcciÃ³n. Los bugs fueron reportados por Henry Creel como "problemas en la CRUD, los filtros no funcionan al 100%, y algunas tablas no se muestran bien, osea como que salen tapadas".

### Hallazgos principales

| Bug | Severidad | MÃ³dulo | Impacto | Estado |
|-----|-----------|---------|---------|--------|
| **#1** Tablas tapadas (z-order de docking) | ðŸ”´ **CRÃTICO** | Todos los listados + Dashboard | Grids cubrÃ­an header/barra/menÃº | âœ… **CORREGIDO** (`f99cd8c`) |
| **#2** Grid de reportes vacÃ­o | ðŸ”´ **CRÃTICO** | `FrmReportes` | Los reportes no se visualizaban | âœ… **CORREGIDO** (`d1e8c56`) |
| **#3** Filtro por ID numÃ©rico | ðŸŸ¡ **MENOR** | Varios DAL | Comportamiento confuso pero correcto | âœ… Documentado + UX |
| **#4** Botones desalineados | ðŸŸ¢ **COSMÃ‰TICO** | Varios formularios | EstÃ©tica UI no coincidÃ­a con mockups | âœ… **CORREGIDO** (`f99cd8c`) |

---

## ðŸ› Bug #1: Tablas Tapadas (Z-Order de Docking) â€” CAUSA RAÃZ REAL

### DescripciÃ³n

**Este es el bug principal que reportÃ³ Henry.** En todos los formularios de listado (Clientes, Dispositivos, Ã“rdenes, Pagos, Reportes, TÃ©cnicos, Usuarios), los grids aparecÃ­an cubriendo el header y la barra de filtros. En el Dashboard, el `panelContenido` cubrÃ­a el menÃº lateral.

**ReproducciÃ³n:**
1. Login como `admin` / `admin123`
2. Ir a cualquier mÃ³dulo (Clientes, Ã“rdenes, etc.)
3. **Resultado:** La tabla (DataGridView) tapa el encabezado azul y la barra de bÃºsqueda/filtros

### Causa raÃ­z (verificada empÃ­ricamente)

En WinForms, **el Ãºltimo control agregado al Form queda al frente del z-order y se dockeriza primero**. Cuando un control `Dock=Fill` se agrega al final:

```csharp
// CÃ³digo ORIGINAL (incorrecto):
Controls.Add(header);   // Dock=Top  â†’ queda al fondo
Controls.Add(barra);    // Dock=Top  â†’ queda al medio
Controls.Add(grid);     // Dock=Fill â†’ queda al FRENTE â†’ se dockeriza PRIMERO â†’ cubre todo
```

Resultado verificado con test de docking (Bounds del grid = `{0,0,800,600}`, el Ã¡rea completa):

```
Patron actual: DataGridView Dock=Fill Bounds={X=0,Y=0,Width=800,Height=600}  â† CUBRE TODO
```

### SoluciÃ³n aplicada

**Reordenar los `Controls.Add()` para que el control `Dock=Fill` se agregue PRIMERO:**

```csharp
// CÃ³digo CORREGIDO:
Controls.Add(grid);     // Dock=Fill â†’ al fondo â†’ se dockeriza Ãºltimo â†’ ocupa espacio restante
Controls.Add(barra);    // Dock=Top
Controls.Add(header);   // Dock=Top
```

Resultado verificado:

```
Patron corregido: DataGridView Dock=Fill Bounds={X=0,Y=116,Width=800,Height=484}  â† CORRECTO
Panel  Dock=Top Bounds={X=0,Y=56,Width=800,Height=60}
Panel  Dock=Top Bounds={X=0,Y=0,Width=800,Height=56}
```

### Formularios corregidos (11)

| Formulario | Cambio |
|-----------|--------|
| `FrmClientes` | grid â†’ barra â†’ header |
| `FrmDispositivos` | grid â†’ barra â†’ header |
| `FrmOrdenes` | grid â†’ barra â†’ header |
| `FrmPagos` | grid â†’ barra â†’ header |
| `FrmReportes` | grid â†’ barra â†’ header |
| `FrmTecnicos` | grid â†’ barra â†’ header |
| `FrmUsuarios` | grid â†’ barra â†’ header |
| `FrmDashboard` | contenido â†’ lateral â†’ barra (+ quitar `panelContenido.BringToFront()`) |
| `FrmClienteDetalle` | grid â†’ info â†’ etiqueta |
| `FrmOrdenDetalle` | mainLayout â†’ header |
| `FrmPagoDetalle` | grid â†’ header â†’ bottomPanel |

### Archivos y mÃ©todos

- `APP/Formularios/*.cs` â€” todos los `InitializeUi()` / `BuildUi()` / `CrearEstructura()`
- `APP/Formularios/FrmDashboard.cs:278` â€” eliminado `panelContenido.BringToFront()` que revertÃ­a el fix

### EstimaciÃ³n

- **Complejidad:** Baja
- **Tiempo real:** ~30 minutos
- **Archivos afectados:** 11

---

## ðŸ› Bug #2: Grid de Reportes VacÃ­o

### DescripciÃ³n

Al generar cualquiera de los 4 reportes disponibles (Ã“rdenes por estado, Ã“rdenes por tÃ©cnico, Servicios completados, Pagos registrados), el `DataGridView` aparecÃ­a **completamente vacÃ­o** a pesar de que los datos se consultaban correctamente desde SQL Server.

### Causa raÃ­z

**UbicaciÃ³n:** `APP/Formularios/FrmReportes.cs:40-42`

```csharp
// 1. Grilla
grid.Dock = DockStyle.Fill;
UIHelper.ConfigurarGrilla(grid);  // â† Establece AutoGenerateColumns = false
```

**AnÃ¡lisis de `UIHelper.ConfigurarGrilla()`** (`APP/Formularios/UIHelper.cs:29-43`):

```csharp
public static void ConfigurarGrilla(DataGridView grid)
{
    grid.ReadOnly = true;
    grid.AllowUserToAddRows = false;
    grid.AllowUserToDeleteRows = false;
    grid.AllowUserToResizeRows = false;
    grid.AutoGenerateColumns = false;  // â† Â¡PROBLEMA!
    grid.RowHeadersVisible = false;
    grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    grid.MultiSelect = false;
    grid.BackgroundColor = Color.White;
    grid.BorderStyle = BorderStyle.None;
    grid.Font = Estilos.Fuente(9);
}
```

**El problema:** `AutoGenerateColumns = false` indica a WinForms que NO genere columnas automÃ¡ticamente al asignar el `DataSource`. Esto requiere que el programador agregue columnas manualmente **antes** de asignar el `DataSource`.

**En todos los demÃ¡s formularios** (Clientes, Ã“rdenes, Pagos, etc.), despuÃ©s de llamar a `ConfigurarGrilla()`, se agregan columnas manualmente:

```csharp
// FrmClientes.cs (correcto)
UIHelper.ConfigurarGrilla(grid);
grid.Columns.Add(UIHelper.Col("ID", "ClienteID", 60));
// ... mÃ¡s columnas
```

**En `FrmReportes`**, esto **NO se hace**. El grid queda sin columnas, y al asignar `grid.DataSource = _tablaActual`, WinForms no genera columnas porque `AutoGenerateColumns = false`.

### SoluciÃ³n aplicada

Se habilitÃ³ `AutoGenerateColumns = true` en `FrmReportes` (commit `d1e8c56`):

```csharp
grid.AutoGenerateColumns = true;  // â† CORREGIDO
```

Como cada reporte tiene un esquema diferente, la autogeneraciÃ³n se adapta automÃ¡ticamente.

### EstimaciÃ³n

- **Complejidad:** Baja
- **Tiempo:** 5 minutos
- **Archivos afectados:** 1 (`FrmReportes.cs`)
- **LÃ­neas modificadas:** ~10

---

## ðŸ› Bug #3: Filtro por ID Ignora Otros Criterios

### DescripciÃ³n

En varios mÃ³dulos (Clientes, Dispositivos, Ã“rdenes, TÃ©cnicos, Usuarios, Pagos), cuando el usuario escribe un **nÃºmero** en el campo de bÃºsqueda, el sistema interpreta esto como bÃºsqueda por ID.

### Causa raÃ­z

**UbicaciÃ³n:** Varios archivos DAL (`ClienteDAL.Buscar()`, `DispositivoDAL.Buscar()`, etc.)

```csharp
var esId = int.TryParse(textoLimpio, out var idBuscado);
...
if (esId)
{
    sql.Append(" AND ClienteID = @ClienteIDBuscado");  // BÃºsqueda exacta por ID
}
else if (!string.IsNullOrWhiteSpace(textoLimpio))
{
    sql.Append(" AND (Nombre LIKE @Texto OR ...)");     // BÃºsqueda por texto
}
// Los filtros de estado/fechas se agregan DESPUÃ‰S y SÃ se aplican
if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
    sql.Append(" AND Estado = @Estado");
```

**ConclusiÃ³n:** Revisando el cÃ³digo, **los filtros de estado/fechas SÃ se aplican incluso con bÃºsqueda por ID**. El comportamiento es correcto por diseÃ±o (prioridad de bÃºsqueda exacta por ID).

### SoluciÃ³n aplicada

Mejora de UX (commit `f99cd8c`): placeholders actualizados para aclarar el comportamiento:

```csharp
txtBuscar.PlaceholderText = "Buscar por nombre, apellido o telÃ©fono (nÃºmero = ID exacto)â€¦";
```

### EstimaciÃ³n

- **Complejidad:** N/A (no era bug)
- **Tiempo:** 5 minutos (UX)
- **Archivos afectados:** 6 (placeholders)

---

## ðŸ› Bug #4: Botones Desalineados a la Derecha

### DescripciÃ³n

En los formularios de listado (Clientes, Dispositivos, TÃ©cnicos, Usuarios), los botones de acciÃ³n ("+ Nuevo cliente", "Editar", "Cambiar estado", etc.) no se alineaban a la derecha de la barra de herramientas.

### Causa raÃ­z

**UbicaciÃ³n:** Varios archivos de formularios (`FrmClientes.cs`, `FrmDispositivos.cs`, `FrmTecnicos.cs`, `FrmUsuarios.cs`)

```csharp
// Espaciador que NO funciona como flexible:
var spacer = new Panel { Size = new Size(20, 1) };  // Solo ocupa 20px fijos
barraLayout.Controls.Add(spacer);
```

En WinForms, un `Panel` con tamaÃ±o fijo **NO actÃºa como espaciador flexible** dentro de un `FlowLayoutPanel`.

### SoluciÃ³n aplicada (commit `f99cd8c`)

Se reemplazÃ³ el layout por `TableLayoutPanel` con columna flexible:

```csharp
var barraLayout = new TableLayoutPanel
{
    Dock = DockStyle.Fill,
    ColumnCount = 2,
    RowCount = 1
};
barraLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));      // Filtros (izquierda)
barraLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Botones (derecha)

var pnlBotones = new FlowLayoutPanel
{
    FlowDirection = FlowDirection.RightToLeft,
    Dock = DockStyle.Fill,
    AutoSize = true
};
```

### EstimaciÃ³n

- **Complejidad:** Media
- **Tiempo real:** ~30 minutos
- **Archivos afectados:** 4

---

## ðŸ“Š Resumen de Estimaciones

| Bug | Prioridad | Tiempo estimado | Archivos | LÃ­neas | Estado |
|-----|-----------|-----------------|----------|--------|--------|
| #1 Tablas tapadas (z-order) | ðŸ”´ P0 | 30 min | 11 | ~90 | âœ… CORREGIDO |
| #2 Grid reportes vacÃ­o | ðŸ”´ P0 | 5 min | 1 | ~10 | âœ… CORREGIDO |
| #3 Filtro por ID | ðŸŸ¡ P2 | 5 min (UX) | 6 | ~6 | âœ… Documentado |
| #4 Botones desalineados | ðŸŸ¢ P3 | 30 min | 4 | ~90 | âœ… CORREGIDO |

**Total:** 4 bugs, todos resueltos. Build: 0 errores, 0 warnings.

---

## ðŸ§ª Plan de Testing

### Tests de regresiÃ³n (post-correcciÃ³n)

Verificar que las correcciones **no afectan** otros formularios:

- âœ… FrmClientes grid muestra columnas Y header/barra visibles
- âœ… FrmDispositivos grid muestra columnas Y header/barra visibles
- âœ… FrmOrdenes grid muestra columnas Y header/barra visibles
- âœ… FrmPagos grid muestra columnas Y header/barra visibles
- âœ… FrmTecnicos grid muestra columnas Y header/barra visibles
- âœ… FrmUsuarios grid muestra columnas Y header/barra visibles
- âœ… FrmReportes grid muestra columnas (auto-generadas) Y header/barra visibles
- âœ… FrmDashboard menÃº lateral visible Y contenido en el Ã¡rea restante
- âœ… FrmOrdenDetalle header visible Y formulario en el Ã¡rea restante
- âœ… FrmClienteDetalle info visible Y grid de dispositivos abajo
- âœ… FrmPagoDetalle header visible Y grid de detalle visible

### Tests manuales (Bug #1)

1. Login como `admin` / `admin123`
2. Ir a cada mÃ³dulo y verificar que el grid NO tape el header ni la barra de filtros
3. En el Dashboard, verificar que el menÃº lateral sea visible y funcional

---

**Fin del documento**
