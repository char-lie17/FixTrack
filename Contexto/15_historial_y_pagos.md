# Historial de órdenes y pagos seguros

## Decisión implementada

FixTrack registra ahora los cambios relevantes de cada orden en `HistorialOrdenes`.
Cada entrada conserva la orden, el usuario de sesión, la fecha, el tipo de cambio,
el campo afectado y sus valores anterior y nuevo.

La funcionalidad cubre:

- Creación de una orden.
- Cambios de estado.
- Edición de problema, diagnóstico, trabajo, costo, observaciones y fecha de finalización.
- Registro de pagos y abonos iniciales.

El detalle de la orden muestra el historial junto con los pagos.

## Pagos concurrentes

El registro de pagos usa una única transacción SQL. La orden se bloquea durante la
lectura del costo y del total pagado; después se valida el saldo y se inserta el pago.
Así dos usuarios no pueden aprobar simultáneamente pagos que superen el costo de
la orden.

## Instalaciones existentes

Las bases nuevas incluyen la tabla desde `BD/FixTrack_BD.sql`. Para una instalación
ya creada se debe ejecutar `BD/Actualizar_HistorialOrdenes.sql` una sola vez.

## Compatibilidad académica

Se mantiene WinForms, ADO.NET, SQL Server y la estructura de roles existente. La
seguridad de contraseñas no forma parte de esta ampliación.
