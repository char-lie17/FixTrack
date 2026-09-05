# PLAN DE REPARACION — Completar el Entregable 2

Fecha: 03/09/2026
Objetivo: cerrar todas las brechas detectadas en AUDITORIA_CUMPLIMIENTO_ENTREGABLE_2.md.

## Brechas y fases de reparacion

| Fase | Brecha detectada | Accion | Criterio de aceptacion |
|---|---|---|---|
| R1 | Manejo de errores/excepciones en casi toda la app | Clase helper `UIHelper.EjecutarSeguro` + envolver todas las operaciones de BD de formularios | Ninguna excepcion no controlada; mensajes claros al usuario |
| R2 | 0 transacciones | `OrdenServicioDAL.InsertarConPagoInicial` (SqlTransaction) + uso en FrmOrdenNueva | Crear orden + abono inicial atomicos o se revierte todo |
| R3 | Dispositivos esqueleto | FrmDispositivos (listado+busqueda) + FrmDispositivoFormulario (crear/editar, validaciones) | CRUD funcional en UI |
| R4 | Ordenes esqueleto | FrmOrdenes (filtros) + FrmOrdenNueva + FrmOrdenDetalle (diagnostico, estado, pagos) | Flujo completo de ordenes en UI |
| R5 | Pagos esqueleto | FrmPagos (listado+filtros) + FrmPagoRegistrar (monto>0, metodo) | Registrar y listar pagos en UI |
| R6 | Tecnicos esqueleto | FrmTecnicos + FrmTecnicoFormulario + cambio de estado (solo Admin) | CRUD en UI, acceso solo Admin |
| R7 | Usuarios esqueleto | FrmUsuarios + FrmUsuarioFormulario (hash, rol, tecnico asociado, unicidad) | CRUD en UI, solo Admin |
| R8 | Reportes esqueleto | FrmReportes: 4 reportes oficiales + desde/hasta + exportacion CSV | Los 4 reportes generan resultados y se puede exportar |
| R9 | Modelos sin ayuda para combos | Propiedades computadas `DescripcionCombo`, `NombreCompleto` en modelos | Combos legibles |
| R10 | Validaciones/mensajes parciales | MessageBox de confirmacion/exito/error en todos los modulos | Cobertura completa |
| R11 | Pruebas por rol e integracion | Smoke test ampliado (incluye transaccion) + build + arranque | 0 errores build; smoke test pasa; app arranca |

## Decisiones respetadas (Rules + plan)
- Dispositivos: NO se implementa cambio de estado (BD sin columna Estado; conflicto sin resolver).
- Estructura BD intacta: no se agregan columnas ni tablas.
- Procedimientos almacenados: no aplican (BD original sin SPs; documentado).
- Toda consulta parametrizada; conexiones con using; excepciones manejadas en presentacion.
