# Diagnóstico Técnico y Plan de Corrección

**Fecha:** 05/09/2026  
**Autor:** Carlos Castillo  
**Commit base:** `98ef50f` (snapshot inicial Henry Creel)

---

## 🔍 Resumen Ejecutivo

Este documento detalla los bugs identificados en el snapshot inicial del proyecto FixTrack, su impacto, causa raíz y plan de corrección. Los bugs fueron reportados por Henry Creel como "problemas en la CRUD, los filtros no funcionan al 100%, y algunas tablas no se muestran bien, osea como que salen tapadas".

### Hallazgos principales

| Bug | Severidad | Módulo | Impacto | Estado |
|-----|-----------|---------|---------|--------|
| **#1** Grid de reportes vacío | 🔴 **CRÍTICO** | `FrmReportes` | Los reportes no se visualizan | Pendiente |
| **#2** Filtro por ID ignora otros criterios | 🟡 **MENOR** | Varios DAL | Comportamiento confuso pero funcional | Diseño |
| **#3** Botones desalineados | 🟢 **COSMÉTICO** | Varios formularios | Estética UI no coincide con mockups | Pendiente |

---

## 🐛 Bug #1: Grid de Reportes Vacío

### Descripción

Al generar cualquiera de los 4 reportes disponibles (Órdenes por estado, Órdenes por técnico, Servicios completados, Pagos registrados), el `DataGridView` aparece **completamente vacío** a pesar de que:
- Los datos se consultan correctamente desde SQL Server
- El `DataTable` tiene filas y columnas
- No hay excepciones lanzadas

**Reproducción:**
1. Login como `admin` / `admin123`
2. Ir a **Reportes**
3. Seleccionar cualquier reporte
4. Click en "Generar"
5. **Resultado:** Tabla vacía (sin columnas ni filas visibles)

### Causa raíz

**Ubicación:** `APP/Formularios/FrmReportes.cs:40-42`

```csharp
// 1. Grilla
grid.Dock = DockStyle.Fill;
UIHelper.ConfigurarGrilla(grid);  // ← Aquí está el problema
```

**Análisis de `UIHelper.ConfigurarGrilla()`** (`APP/Formularios/UIHelper.cs:29-43`):

```csharp
public static void ConfigurarGrilla(DataGridView grid)
{
    grid.ReadOnly = true;
    grid.AllowUserToAddRows = false;
    grid.AllowUserToDeleteRows = false;
    grid.AllowUserToResizeRows = false;
    grid.AutoGenerateColumns = false;  // ← ¡PROBLEMA!
    grid.RowHeadersVisible = false;
    grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    grid.MultiSelect = false;
    grid.BackgroundColor = Color.White;
    grid.BorderStyle = BorderStyle.None;
    grid.Font = Estilos.Fuente(9);
}
```

**El problema:** `AutoGenerateColumns = false` indica a WinForms que NO genere columnas automáticamente al asignar el `DataSource`. Esto requiere que el programador agregue columnas manualmente **antes** de asignar el `DataSource`.

**En todos los demás formularios** (Clientes, Órdenes, Pagos, etc.), después de llamar a `ConfigurarGrilla()`, se agregan columnas manualmente:

```csharp
// FrmClientes.cs (correcto)
UIHelper.ConfigurarGrilla(grid);
grid.Columns.Add(UIHelper.Col("ID", "ClienteID", 60));
grid.Columns.Add(UIHelper.Col("Nombre", "Nombre", 120));
// ... más columnas
```

**En `FrmReportes`**, esto **NO se hace**. El grid queda sin columnas, y al asignar `grid.DataSource = _tablaActual`, WinForms no genera columnas porque `AutoGenerateColumns = false`.

### Impacto

- **Severidad:** 🔴 **CRÍTICA**
- **Usuarios afectados:** Administrador, Empleado
- **Funcionalidad perdida:** Módulo completo de Reportes (gestión de métricas de negocio)
- **Workaround:** Ninguno disponible desde la UI

### Solución propuesta

**Opción A: Activar autogeneración de columnas** (recomendada para reportes dinámicos)

```csharp
// FrmReportes.cs:40-42
grid.Dock = DockStyle.Fill;
grid.ReadOnly = true;
grid.AllowUserToAddRows = false;
grid.AllowUserToDeleteRows = false;
grid.AutoGenerateColumns = true;  // ← Permitir autogeneración
grid.RowHeadersVisible = false;
grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
grid.BackgroundColor = Color.White;
grid.BorderStyle = BorderStyle.None;
grid.Font = Estilos.Fuente(9);
```

**Ventaja:** Cada reporte tiene esquema diferente, la autogeneración se adapta automáticamente.

**Opción B: Definir columnas manualmente por reporte**

Agregar lógica en `BtnGenerar_Click` para recrear columnas según el reporte:

```csharp
private void BtnGenerar_Click(object? sender, EventArgs e)
{
    // ... código existente ...
    
    grid.Columns.Clear();
    
    // Configurar columnas según el reporte
    switch (reporte)
    {
        case "Órdenes por estado":
            grid.Columns.Add(UIHelper.Col("Estado", "Estado", 150));
            grid.Columns.Add(UIHelper.Col("Cantidad", "Cantidad", 100));
            grid.Columns.Add(UIHelper.Col("Subtotal", "Subtotal", 120));
            break;
        // ... otros casos
    }
    
    grid.DataSource = _tablaActual;
}
```

**Desventaja:** Más código, mantenimiento complejo.

**Decisión:** **Opción A** (autogeneración).

### Estimación

- **Complejidad:** Baja
- **Tiempo:** 5 minutos
- **Archivos afectados:** 1 (`FrmReportes.cs`)
- **Líneas modificadas:** ~10

---

## 🐛 Bug #2: Filtro por ID Ignora Otros Criterios

### Descripción

En varios módulos (Clientes, Dispositivos, Órdenes, Técnicos, Usuarios, Pagos), cuando el usuario escribe un **número** en el campo de búsqueda, el sistema interpreta esto como búsqueda por ID y **ignora** los demás filtros (estado, fechas, método de pago).

**Ejemplo:**

1. Ir a **Órdenes**
2. Filtrar por estado: **"En reparación"**
3. Filtrar por fecha: **01/08/2026 - 31/08/2026**
4. Buscar: **"5"**
5. **Resultado esperado:** Orden #5 solo si está "En reparación" y en el rango de fechas
6. **Resultado real:** Orden #5 sin importar estado ni fechas

### Causa raíz

**Ubicación:** Varios archivos DAL (`ClienteDAL.Buscar()`, `DispositivoDAL.Buscar()`, etc.)

**Ejemplo de `ClienteDAL.cs:51-72`:**

```csharp
public static List<Cliente> Buscar(string? texto, string? estado)
{
    var textoLimpio = texto?.Trim();
    var esId = int.TryParse(textoLimpio, out var idBuscado);

    var sql = new StringBuilder($"SELECT {Columnas} FROM Clientes WHERE 1 = 1");
    if (esId)
    {
        // Solo busca por ID, ignora estado
        sql.Append(" AND ClienteID = @ClienteIDBuscado");
    }
    else if (!string.IsNullOrWhiteSpace(textoLimpio))
    {
        sql.Append(" AND (Nombre LIKE @Texto OR Apellido LIKE @Texto ...)");
    }
    
    // El filtro de estado se aplica DESPUÉS
    if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
        sql.Append(" AND Estado = @Estado");  // ← No se alcanza si esId=true
    
    // ...
}
```

**El problema:** La lógica de `if (esId) {...} else if (...)` hace que la búsqueda por ID **excluya** la búsqueda textual, pero el filtro de estado se agrega **después** de la estructura `if/else`, por lo que **sí se aplica**. Sin embargo, en algunos DAL (como `OrdenServicioDAL`), la estructura es más compleja y el problema es real.

**Verificación en `OrdenServicioDAL.cs:46-69`:**

```csharp
if (esId)
{
    sql.Append(" AND o.OrdenID = @OrdenIDBuscado");
}
else if (!string.IsNullOrWhiteSpace(textoLimpio))
{
    sql.Append(" AND (c.Nombre LIKE @Texto OR ...)");
}
// Los filtros de estado/fechas se agregan DESPUÉS, así que sí se aplican
if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
    sql.Append(" AND o.Estado = @Estado");
if (desde.HasValue)
    sql.Append(" AND o.FechaIngreso >= @Desde");
// ...
```

**Conclusión:** Revisando el código, **los filtros de estado/fechas SÍ se aplican incluso con búsqueda por ID**. El bug reportado no existe en el código actual. Es posible que:

1. Henry probó una versión anterior que sí tenía este bug
2. La confusión proviene de que buscar "5" muestra solo la Orden #5 (comportamiento correcto) pero esperaba ver TODAS las órdenes que coincidan con estado/fecha Y que contengan "5" en algún campo

### Impacto

- **Severidad:** 🟡 **MENOR** (o inexistente)
- **Naturaleza:** Comportamiento por diseño (prioridad de búsqueda exacta)
- **Confusión UX:** Los usuarios pueden no entender que escribir un número cambia el modo de búsqueda

### Solución propuesta

**No requiere corrección de código**, pero puede mejorarse la UX:

**Opción 1: Agregar tooltip/placeholder explicativo**

```csharp
txtBuscar.PlaceholderText = "Buscar por nombre/apellido (o ID exacto si es número)";
```

**Opción 2: Agregar búsqueda combinada**

Cambiar lógica para que búsqueda por ID **también** busque por texto parcial:

```csharp
if (esId)
{
    sql.Append(" AND (ClienteID = @ClienteIDBuscado OR Nombre LIKE @Texto OR ...)");
}
else if (!string.IsNullOrWhiteSpace(textoLimpio))
{
    sql.Append(" AND (Nombre LIKE @Texto OR ...)");
}
```

**Decisión:** **No acción inmediata** (comportamiento correcto), documentar en README.

### Estimación

- **Complejidad:** N/A (no es bug)
- **Tiempo:** 0 minutos
- **Archivos afectados:** 0

---

## 🐛 Bug #3: Botones Desalineados a la Derecha

### Descripción

En los formularios de listado (Clientes, Dispositivos, Órdenes, Pagos, Técnicos, Usuarios), los botones de acción ("+ Nuevo cliente", "Editar", "Cambiar estado", etc.) **no se alinean completamente a la derecha** de la barra de herramientas, como se muestra en los mockups.

**Mockup esperado:**
```
[Buscar: _______] [Estado: Todos ▼]          [Cambiar estado] [Editar] [+ Nuevo cliente]
```

**Resultado actual:**
```
[Buscar: _______] [Estado: Todos ▼] [  ] [Cambiar estado] [Editar] [+ Nuevo cliente]
```

Hay un pequeño espacio, pero los botones no están "pegados" al borde derecho.

### Causa raíz

**Ubicación:** Varios archivos de formularios (`FrmClientes.cs:89-91`, `FrmDispositivos.cs:63-65`, etc.)

**Código problemático:**

```csharp
// Espaciador flexible (empuja botones a la derecha)
var spacer = new Panel { Size = new Size(20, 1) };
barraLayout.Controls.Add(spacer);
```

**El problema:** En WinForms, un `Panel` con tamaño fijo (`Size = new Size(20, 1)`) **NO actúa como espaciador flexible** dentro de un `FlowLayoutPanel`. Solo ocupa exactamente 20px.

Para lograr alineación derecha en `FlowLayoutPanel`, hay dos enfoques correctos:

**Enfoque A: Usar `FlowDirection = RightToLeft` en un sub-panel** (ya implementado)

Los formularios **ya usan** un sub-`FlowLayoutPanel` con `RightToLeft` para los botones:

```csharp
var pnlBotones = new FlowLayoutPanel
{
    FlowDirection = FlowDirection.RightToLeft,
    AutoSize = true,
    WrapContents = false,
    Margin = new Padding(0, 4, 0, 4)
};
barraLayout.Controls.Add(pnlBotones);
```

Esto **funciona correctamente** para alinear botones dentro del panel. El problema es que el `pnlBotones` en sí mismo no se ancla al borde derecho del contenedor padre.

**Enfoque B: Usar `Dock = DockStyle.Right` o `TableLayoutPanel` con columna flexible**

### Impacto

- **Severidad:** 🟢 **COSMÉTICO**
- **Usuarios afectados:** Todos
- **Funcionalidad afectada:** Ninguna (solo visual)
- **Comparación:** No coincide con mockups del Entregable 1

### Solución propuesta

**Reemplazar el `spacer` Panel por configuración correcta de `FlowLayoutPanel`:**

**Cambio en `FrmClientes.cs` (ejemplo):**

```csharp
// ANTES (líneas 54-91):
var barraLayout = new FlowLayoutPanel
{
    Dock = DockStyle.Fill,
    FlowDirection = FlowDirection.LeftToRight,
    AutoSize = false,
    WrapContents = false,
    Padding = Padding.Empty,
    Margin = Padding.Empty
};

// ... controles de búsqueda ...

var spacer = new Panel { Size = new Size(20, 1) };  // ← No funciona
barraLayout.Controls.Add(spacer);

var pnlBotones = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, ... };
barraLayout.Controls.Add(pnlBotones);

// DESPUÉS:
var barraLayout = new TableLayoutPanel  // ← Cambiar a TableLayoutPanel
{
    Dock = DockStyle.Fill,
    ColumnCount = 2,
    RowCount = 1,
    Padding = Padding.Empty,
    Margin = Padding.Empty
};
barraLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));   // Filtros (izq)
barraLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Botones (der, expandible)

var pnlFiltros = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, ... };
// ... agregar búsqueda, estado ...
barraLayout.Controls.Add(pnlFiltros, 0, 0);

var pnlBotones = new FlowLayoutPanel { 
    FlowDirection = FlowDirection.RightToLeft,
    Dock = DockStyle.Right,  // ← Anclar a la derecha
    AutoSize = true,
    ...
};
// ... agregar botones ...
barraLayout.Controls.Add(pnlBotones, 1, 0);
```

### Estimación

- **Complejidad:** Media (requiere refactoring de layout)
- **Tiempo:** 30 minutos × 6 formularios = **3 horas**
- **Archivos afectados:** 6
  - `FrmClientes.cs`
  - `FrmDispositivos.cs`
  - `FrmOrdenes.cs`
  - `FrmPagos.cs`
  - `FrmTecnicos.cs`
  - `FrmUsuarios.cs`
- **Líneas modificadas:** ~40 por archivo = **240 líneas**

### Decisión

**Posponer** para release posterior (no crítico). Documentar como "known issue cosmético".

---

## 📊 Resumen de Estimaciones

| Bug | Prioridad | Tiempo estimado | Archivos | Líneas |
|-----|-----------|-----------------|----------|--------|
| #1 Grid reportes vacío | 🔴 P0 | 5 min | 1 | ~10 |
| #2 Filtro por ID | 🟡 P2 | 0 min (no bug) | 0 | 0 |
| #3 Botones desalineados | 🟢 P3 | 3 horas (pospuesto) | 6 | ~240 |

**Total para commit inmediato:** 5 minutos (solo Bug #1)

---

## 🛠️ Plan de Corrección

### Fase 1: Corrección crítica (este commit)

1. ✅ Documentar bugs (este archivo)
2. ⏳ Arreglar Bug #1 (FrmReportes)
3. ⏳ Agregar tests de UI para reportes
4. ⏳ Commit y push

### Fase 2: Mejoras UX (próximo release)

1. Agregar tooltips/placeholders explicativos (Bug #2)
2. Refactorizar layout de barras de herramientas (Bug #3)
3. Agregar validación de conexión al inicio

### Fase 3: Optimizaciones (futuro)

1. Migrar a Entity Framework Core
2. Agregar campo Estado a Dispositivos
3. Exportar reportes a PDF

---

## 🧪 Plan de Testing

### Tests manuales (Bug #1)

**Caso de prueba: Reporte "Órdenes por estado"**

1. Login como `admin` / `admin123`
2. Ir a Reportes
3. Seleccionar "Órdenes por estado"
4. Establecer rango: últimos 30 días
5. Click "Generar"
6. **Verificar:**
   - ✅ La tabla muestra 3 columnas: Estado, Cantidad, Subtotal
   - ✅ Hay al menos 1 fila de datos
   - ✅ Los valores son coherentes (Cantidad > 0)

Repetir para los 4 reportes.

### Tests de regresión

Verificar que la corrección **no afecta** otros formularios:

- ✅ FrmClientes grid muestra columnas
- ✅ FrmDispositivos grid muestra columnas
- ✅ FrmOrdenes grid muestra columnas
- ✅ FrmPagos grid muestra columnas
- ✅ FrmTecnicos grid muestra columnas
- ✅ FrmUsuarios grid muestra columnas

---

## 📝 Cambios en README

Actualizar sección de Bugs Conocidos:

```markdown
## 🐛 Bugs Conocidos

### ~~Bug #1: Tabla de Reportes vacía~~ ✅ CORREGIDO

**Estado:** Resuelto en commit `[hash]`
**Solución:** Habilitado `AutoGenerateColumns = true` en FrmReportes
```

---

**Fin del documento**
