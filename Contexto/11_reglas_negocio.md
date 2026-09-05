# Reglas de Negocio

## Proceso operativo principal

El flujo de negocio se define como:

```
Cliente → Dispositivo → Orden de servicio → Técnico → Diagnóstico / reparación → Estado → Pago → Entrega
```

**Fuente:** `Entregables/Entregable_1/mockups/FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Página 5

## Estados de las órdenes de servicio

Las órdenes de servicio tienen exactamente 5 estados posibles:

1. **Pendiente** — La orden fue creada pero aún no fue revisada por un técnico
2. **En diagnóstico** — Un técnico está evaluando el dispositivo para determinar la falla
3. **En reparación** — El dispositivo está siendo reparado activamente
4. **Listo** — La reparación está terminada, pendiente de entrega al cliente
5. **Entregado** — El dispositivo fue entregado al cliente; la orden está cerrada

**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Línea 93
**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 6

## Estados de clientes y técnicos

Tanto Clientes como Técnicos tienen 2 estados posibles:
- **Activo**
- **Inactivo**

**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 30, 65, 136

## Métodos de pago

Los pagos solo pueden registrarse con uno de estos 3 métodos:
- **Efectivo**
- **Tarjeta**
- **Transferencia**

**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Línea 115

## Reglas de la base de datos

- El costo del servicio debe ser ≥ 0 (`CK_OrdenesServicio_Costo`)
- El monto de un pago debe ser > 0 (`CK_Pagos_Monto`)
- Una orden puede tener múltiples pagos asociados
- Un técnico puede estar asociado a como máximo un usuario (índice único filtrado)
- La eliminación de un cliente o dispositivo está bloqueada si existen órdenes asociadas (ON DELETE NO ACTION)
- No se puede eliminar un cliente con dispositivos registrados

## Reglas de la interfaz

- No usar más de dos botones principales por formulario
- Los botones destructivos deben usarse con moderación y preferiblemente requerir confirmación
- La política de «Cambiar estado» reemplaza al borrado físico en Clientes y Dispositivos
- Los estados de las órdenes se muestran con indicadores de color
- Los DataGridView son de solo lectura para selección de fila

**Fuente:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 8, 10

## Información confirmada

- **CONFIRMADO:** Los 5 estados de orden están definidos explícitamente.
- **CONFIRMADO:** Los 3 métodos de pago están definidos explícitamente.
- **CONFIRMADO:** La política es de baja lógica (Cambiar estado) no borrado físico para Clientes y Dispositivos.
- **CONFIRMADO:** Los costos y montos tienen restricciones de integridad.

## Información inferida

- **INFERIDO:** El flujo de estados sugiere un orden secuencial, pero la BD no lo enforcea como camino obligatorio.
- **INFERIDO:** Los técnicos inactivos pueden seguir teniendo órdenes asignadas históricamente.

## Información desconocida

- **DESCONOCIDO:** Si existen reglas de negocio adicionales no documentadas.
- **DESCONOCIDO:** Lógica de transición entre estados (¿puede ir de Pendiente a En reparación sin pasar por Diagnóstico?).
- **DESCONOCIDO:** Políticas de descuento o ajustes de costo.
- **DESCONOCIDO:** Reglas de notificación al cliente.