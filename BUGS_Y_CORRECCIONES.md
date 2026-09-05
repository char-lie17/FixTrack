# Diagnóstico Técnico y Correcciones de FixTrack

**Fecha:** 05/09/2026  
**Base inicial:** `98ef50f`
**Estado de esta revisión:** correcciones críticas implementadas en `c4172f5` y verificadas mediante build y smoke tests cuando SQL Server está disponible.

## Resumen

La aplicación es una solución WinForms con ADO.NET y SQL Server. La arquitectura, el esquema de seis tablas y la transacción de orden más pago inicial se conservan. Esta última fase se concentró en corregir regresiones y evitar que la interfaz informe éxito cuando la base de datos no modificó ninguna fila.

## Correcciones aplicadas

| Problema | Estado | Solución |
|---|---|---|
| Grids tapaban headers y filtros | Corregido | Se reordenaron los controles con `Dock=Fill` antes de los controles `Dock=Top`. |
| Dashboard con z-order incorrecto | Corregido | Se usa el orden contenido, lateral y barra superior; no se fuerza `BringToFront()` sobre el contenido. |
| Botones desalineados | Corregido | Las barras usan `TableLayoutPanel` con columna flexible. |
| Reportes sin columnas | Corregido | `FrmReportes` habilita columnas automáticas para sus `DataTable` variables. |
| Órdenes de técnicos sin filtro SQL | Corregido | La consulta aplica `TecnicoID` directamente en SQL. |
| Fecha de finalización borrada por técnicos | Corregido | Si el campo está deshabilitado, se conserva el valor cargado desde la base de datos. |
| Resultado de `ActualizarEstado()` ignorado | Corregido | La UI revierte la selección cuando la DAL devuelve `false`. |
| SQL directo en `FrmOrdenDetalle` | Corregido | La actualización de estado y fecha está centralizada en `OrdenServicioDAL`. |
| Transiciones protegidas solo en la UI | Corregido | `EstadoOrdenTexto.EsTransicionValida()` se usa tanto en la UI como en la DAL. |
| Resultado de `ActualizarDetalle()` ignorado | Corregido | Solo se muestra éxito cuando la DAL devuelve `true`. |
| Técnico sin `TecnicoID` | Corregido | El login y el detalle de orden rechazan esa sesión inconsistente. |
| Permisos fantasma del técnico | Corregido | El técnico solo tiene `inicio` y `misOrdenes`. |
| Conversión frágil de técnico asociado | Corregido | Se usa conversión explícita de `SelectedValue`, incluyendo `DBNull`. |
| Métricas mostradas como cero ante error | Corregido | Se muestra un mensaje de no disponibilidad en vez de datos engañosos. |
| Reporte por estado no usaba el procedimiento existente | Corregido | `ReportesDAL` ejecuta `sp_ReporteOrdenesPorEstado` como procedimiento almacenado. |
| Comentario de servicios completados inconsistente | Corregido | El comentario indica que el reporte incluye órdenes `Entregado`, igual que el SQL. |
| Transacción de orden y pago inicial | Conservado y reforzado | Se mantiene `SqlTransaction`; el pago debe afectar exactamente una fila o la operación falla y hace rollback. |

## Pruebas de regresión

`APP/TestRunner` comprueba la conexión, operaciones básicas de DAL, filtros de técnico, transiciones, usuarios, pagos, rollback y los cuatro reportes. Las pruebas no modifican la orden real `OrdenID = 1`; la comprobación de actualización sin filas usa `int.MaxValue`.

Las pruebas que requieren SQL Server deben ejecutarse con una base de datos creada desde `BD/FixTrack_BD.sql` y una cadena configurada en `APP/appsettings.json`.

## Decisiones conservadas

- No se agregó `Dispositivos.Estado`, porque la base de datos y los mockups mantienen una discrepancia pendiente de decisión.
- No se modificó `Entregables/`, que permanece como referencia de solo lectura.
- No se migró a Entity Framework, WPF ni otra tecnología distinta de WinForms y ADO.NET.
- Se conserva SHA-256 para mantener compatibilidad con el esquema académico actual.

## Verificación

Antes de declarar una entrega final deben ejecutarse:

1. `dotnet build APP/FixTrack.sln` o el build desde Visual Studio.
2. `dotnet run --project APP/TestRunner/TestRunner.csproj` con SQL Server disponible.
3. Pruebas manuales de login, roles, órdenes, fechas, pagos, reportes y layout.

No se debe interpretar que una operación fue correcta solamente porque no lanzó una excepción; siempre debe verificarse su resultado real.
