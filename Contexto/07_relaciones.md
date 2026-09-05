# Relaciones entre Entidades

## Diagrama de relaciones (textual)

```
CLIENTES (1) ──── (M) DISPOSITIVOS
    │
    │
    └─── (1) ──── (M) ORDENES_SERVICIO  (vía DispositivoID)
                        │
    ┌───────────────────┤
    │                   │
    │              TECNICOS (0..1) ──── (1) USUARIOS
    │              (vía TecnicoID)
    │                   │
    └─── (1) ──── (M) PAGOS
              (vía OrdenID)
```

## Descripción detallada de relaciones

### 1. Clientes → Dispositivos (1:M)
Un cliente puede tener muchos dispositivos. Un dispositivo pertenece a un solo cliente.
- **Foreign Key:** `Dispositivos.ClienteID → Clientes.ClienteID`
- **ON DELETE:** NO ACTION
- **ON UPDATE:** NO ACTION
- **Índice:** `IX_Dispositivos_ClienteID` en `Dispositivos.ClienteID`
- **Fuente:** `Base de Datos APP de escritorio.sql` — Líneas 35-51

### 2. Dispositivos → OrdenesServicio (1:M)
Un dispositivo puede tener muchas órdenes de servicio. Una orden de servicio pertenece a un dispositivo.
- **Foreign Key:** `OrdenesServicio.DispositivoID → Dispositivos.DispositivoID`
- **ON DELETE:** NO ACTION
- **ON UPDATE:** NO ACTION
- **Índice:** `IX_OrdenesServicio_DispositivoID` en `OrdenesServicio.DispositivoID`
- **Fuente:** `Base de Datos APP de escritorio.sql` — Líneas 70-96

### 3. Técnicos → OrdenesServicio (1:M)
Un técnico puede tener muchas órdenes de servicio. Una orden de servicio puede no tener técnico asignado (nullable).
- **Foreign Key:** `OrdenesServicio.TecnicoID → Tecnicos.TecnicoID`
- **ON DELETE:** NO ACTION
- **ON UPDATE:** NO ACTION
- **Índice:** `IX_OrdenesServicio_TecnicoID` en `OrdenesServicio.TecnicoID`
- **Fuente:** `Base de Datos APP de escritorio.sql` — Líneas 70-96

### 4. Técnicos → Usuarios (1:0..1)
Un técnico puede estar asociado a como máximo un usuario. Un usuario (rol Técnico) puede estar asociado a un técnico.
- **Foreign Key:** `Usuarios.TecnicoID → Tecnicos.TecnicoID`
- **Unique Index filtrado:** `UQ_Usuarios_TecnicoID ON Usuarios(TecnicoID) WHERE TecnicoID IS NOT NULL`
- **Fuente:** `Base de Datos APP de escritorio.sql` — Líneas 119-144

### 5. OrdenesServicio → Pagos (1:M)
Una orden de servicio puede tener muchos pagos. Un pago pertenece a una sola orden de servicio.
- **Foreign Key:** `Pagos.OrdenID → OrdenesServicio.OrdenID`
- **ON DELETE:** NO ACTION
- **ON UPDATE:** NO ACTION
- **Índice:** `IX_Pagos_OrdenID` en `Pagos.OrdenID`
- **Fuente:** `Base de Datos APP de escritorio.sql` — Líneas 99-116

## Resumen de cardinalidades

| Relación | Cardinalidad | FK | Referencia |
|----------|-------------|-----|------------|
| Clientes → Dispositivos | 1:M | Dispositivos.ClienteID | Clientes.ClienteID |
| Dispositivos → OrdenesServicio | 1:M | OrdenesServicio.DispositivoID | Dispositivos.DispositivoID |
| Técnicos → OrdenesServicio | 1:M | OrdenesServicio.TecnicoID | Tecnicos.TecnicoID |
| Técnicos → Usuarios | 1:0..1 | Usuarios.TecnicoID | Tecnicos.TecnicoID |
| OrdenesServicio → Pagos | 1:M | Pagos.OrdenID | OrdenesServicio.OrdenID |

## Información confirmada

- **CONFIRMADO:** Todas las foreign keys usan ON DELETE NO ACTION y ON UPDATE NO ACTION.
- **CONFIRMADO:** La relación entre Usuarios y Técnicos es 1:0..1 (uno a cero o uno).
- **CONFIRMADO:** No hay relaciones circulares ni restricciones adicionales más allá de las FK declaradas.

## Información inferida

- **INFERIDO:** Una orden de servicio sin técnico asignado probablemente está esperando ser asignada a un técnico.
- **INFERIDO:** El sistema permite que un técnico esté inactivo pero siga teniendo órdenes asociadas (no hay restricción de integridad referencial que impida asignar un técnico inactivo).

## Información desconocida

- **DESCONOCIDO:** Si existen planes para agregar relaciones adicionales en el futuro.
- **DESCONOCIDO:** Si se contempla la posibilidad de que un dispositivo esté asociado a más de un cliente.