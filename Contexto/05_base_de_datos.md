# Base de Datos — Estructura Completa

## Nombre de la base de datos

**FixTrack**

**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 1-15

## Script de creación

El archivo SQL primero elimina la base de datos si existe y luego la crea:

```sql
IF DB_ID(N'FixTrack') IS NOT NULL
BEGIN
    ALTER DATABASE FixTrack SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE FixTrack;
END
GO
CREATE DATABASE FixTrack;
GO
USE FixTrack;
GO
```

**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 1-15

## Resumen de tablas

| Tabla | Propósito | Registros de prueba | Fuente |
|-------|-----------|---------------------|--------|
| **Clientes** | Registro de clientes | 8 | Línea 18 |
| **Dispositivos** | Equipos de los clientes | 9 | Línea 179 |
| **Técnicos** | Personal técnico | 3 | Línea 191 |
| **OrdenesServicio** | Órdenes de reparación | 9 | Línea 205 |
| **Pagos** | Pagos asociados a órdenes | 5 | Línea 230 |
| **Usuarios** | Usuarios del sistema | 5 | Línea 197 |

## Tabla: Clientes

Propósito: Registrar y administrar los clientes del taller.

| Columna | Tipo de datos | Nullable | Default | Constraint | Descripción | Fuente |
|---------|--------------|----------|---------|------------|-------------|--------|
| ClienteID | INT IDENTITY(1,1) | NOT NULL | — | PK | Identificador único | Línea 20 |
| Nombre | NVARCHAR(50) | NOT NULL | — | — | Nombre del cliente | Línea 21 |
| Apellido | NVARCHAR(50) | NOT NULL | — | — | Apellido del cliente | Línea 22 |
| Telefono | NVARCHAR(20) | NOT NULL | — | — | Teléfono de contacto | Línea 23 |
| Email | NVARCHAR(100) | NULL | — | — | Correo electrónico | Línea 24 |
| Direccion | NVARCHAR(200) | NULL | — | — | Dirección física | Línea 25 |
| FechaRegistro | DATETIME2 | NOT NULL | GETDATE() | DF_Clientes_FechaRegistro | Fecha de registro | Línea 26 |
| Estado | VARCHAR(20) | NOT NULL | 'Activo' | DF_Clientes_Estado, CK_Clientes_Estado | Estado: 'Activo' o 'Inactivo' | Línea 27-30 |

**Primary Key:** `PK_Clientes (ClienteID)`

**Check Constraint:** `CK_Clientes_Estado (Estado IN ('Activo', 'Inactivo'))`

**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 18-31

## Tabla: Dispositivos

Propósito: Registrar los equipos de los clientes que requieren reparación.

| Columna | Tipo de datos | Nullable | Default | Constraint | Descripción | Fuente |
|---------|--------------|----------|---------|------------|-------------|--------|
| DispositivoID | INT IDENTITY(1,1) | NOT NULL | — | PK | Identificador único | Línea 37 |
| ClienteID | INT | NOT NULL | — | FK | Clave foránea a Clientes | Línea 38 |
| Tipo | NVARCHAR(50) | NOT NULL | — | — | Tipo de dispositivo (Laptop, Impresora, etc.) | Línea 39 |
| Marca | NVARCHAR(50) | NULL | — | — | Marca del dispositivo | Línea 40 |
| Modelo | NVARCHAR(50) | NULL | — | — | Modelo del dispositivo | Línea 41 |
| NumeroSerie | NVARCHAR(100) | NULL | — | — | Número de serie | Línea 42 |
| Descripcion | NVARCHAR(300) | NULL | — | — | Descripción del problema o estado | Línea 43 |
| FechaRegistro | DATETIME2 | NOT NULL | GETDATE() | DF_Dispositivos_FechaRegistro | Fecha de registro | Línea 44 |

**Primary Key:** `PK_Dispositivos (DispositivoID)`

**Foreign Key:** `FK_Dispositivos_Clientes (ClienteID) → Clientes(ClienteID)` — ON DELETE NO ACTION, ON UPDATE NO ACTION

**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 35-51

## Tabla: Técnicos

Propósito: Registrar el personal técnico que realiza las reparaciones.

| Columna | Tipo de datos | Nullable | Default | Constraint | Descripción | Fuente |
|---------|--------------|----------|---------|------------|-------------|--------|
| TecnicoID | INT IDENTITY(1,1) | NOT NULL | — | PK | Identificador único | Línea 57 |
| Nombre | NVARCHAR(50) | NOT NULL | — | — | Nombre del técnico | Línea 58 |
| Apellido | NVARCHAR(50) | NOT NULL | — | — | Apellido del técnico | Línea 59 |
| Telefono | NVARCHAR(20) | NULL | — | — | Teléfono de contacto | Línea 60 |
| Especialidad | NVARCHAR(100) | NULL | — | — | Especialidad técnica | Línea 61 |
| Estado | VARCHAR(20) | NOT NULL | 'Activo' | DF_Tecnicos_Estado, CK_Tecnicos_Estado | Estado: 'Activo' o 'Inactivo' | Línea 62-65 |

**Primary Key:** `PK_Tecnicos (TecnicoID)`

**Check Constraint:** `CK_Tecnicos_Estado (Estado IN ('Activo', 'Inactivo'))`

**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 55-66

## Tabla: OrdenesServicio

Propósito: Registrar las órdenes de servicio (repuestos) y su seguimiento.

| Columna | Tipo de datos | Nullable | Default | Constraint | Descripción | Fuente |
|---------|--------------|----------|---------|------------|-------------|--------|
| OrdenID | INT IDENTITY(1,1) | NOT NULL | — | PK | Identificador único de la orden | Línea 72 |
| DispositivoID | INT | NOT NULL | — | FK | Dispositivo asociado | Línea 73 |
| TecnicoID | INT | NULL | — | FK | Técnico asignado (puede ser NULL) | Línea 74 |
| FechaIngreso | DATETIME2 | NOT NULL | GETDATE() | DF_OrdenesServicio_FechaIngreso | Fecha de ingreso de la orden | Línea 75 |
| ProblemaReportado | NVARCHAR(500) | NOT NULL | — | — | Problema reportado por el cliente | Línea 76 |
| Diagnostico | NVARCHAR(500) | NULL | — | — | Diagnóstico realizado por el técnico | Línea 77 |
| TrabajoRealizado | NVARCHAR(500) | NULL | — | — | Trabajo de reparación realizado | Línea 78 |
| Estado | VARCHAR(20) | NOT NULL | 'Pendiente' | DF_OrdenesServicio_Estado, CK_OrdenesServicio_Estado | Estado de la orden | Línea 79-93 |
| CostoServicio | DECIMAL(10,2) | NOT NULL | 0 | DF_OrdenesServicio_Costo, CK_OrdenesServicio_Costo | Costo del servicio (≥ 0) | Línea 80-95 |
| FechaFinalizacion | DATETIME2 | NULL | — | — | Fecha de finalización | Línea 81 |
| Observaciones | NVARCHAR(500) | NULL | — | — | Observaciones adicionales | Línea 82 |

**Primary Key:** `PK_OrdenesServicio (OrdenID)`

**Foreign Keys:**
- `FK_OrdenesServicio_Dispositivos (DispositivoID) → Dispositivos(DispositivoID)` — ON DELETE NO ACTION, ON UPDATE NO ACTION
- `FK_OrdenesServicio_Tecnicos (TecnicoID) → Tecnicos(TecnicoID)` — ON DELETE NO ACTION, ON UPDATE NO ACTION

**Check Constraints:**
- `CK_OrdenesServicio_Estado (Estado IN ('Pendiente', 'En diagnostico', 'En reparacion', 'Listo', 'Entregado'))`
- `CK_OrdenesServicio_Costo (CostoServicio >= 0)`

**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 70-96

## Tabla: Pagos

Propósito: Registrar los pagos asociados a las órdenes de servicio.

| Columna | Tipo de datos | Nullable | Default | Constraint | Descripción | Fuente |
|---------|--------------|----------|---------|------------|-------------|--------|
| PagoID | INT IDENTITY(1,1) | NOT NULL | — | PK | Identificador único del pago | Línea 102 |
| OrdenID | INT | NOT NULL | — | FK | Orden de servicio asociada | Línea 103 |
| FechaPago | DATETIME2 | NOT NULL | GETDATE() | DF_Pagos_FechaPago | Fecha del pago | Línea 104 |
| Monto | DECIMAL(10,2) | NOT NULL | — | CK_Pagos_Monto | Monto del pago (> 0) | Línea 105 |
| MetodoPago | VARCHAR(20) | NOT NULL | — | CK_Pagos_MetodoPago | Método: Efectivo, Tarjeta o Transferencia | Línea 106 |
| Observaciones | NVARCHAR(300) | NULL | — | — | Observaciones del pago | Línea 107 |

**Primary Key:** `PK_Pagos (PagoID)`

**Foreign Key:** `FK_Pagos_OrdenesServicio (OrdenID) → OrdenesServicio(OrdenID)` — ON DELETE NO ACTION, ON UPDATE NO ACTION

**Check Constraints:**
- `CK_Pagos_Monto (Monto > 0)`
- `CK_Pagos_MetodoPago (MetodoPago IN ('Efectivo', 'Tarjeta', 'Transferencia'))`

**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 99-116

## Tabla: Usuarios

Propósito: Registrar los usuarios del sistema y sus roles de acceso.

| Columna | Tipo de datos | Nullable | Default | Constraint | Descripción | Fuente |
|---------|--------------|----------|---------|------------|-------------|--------|
| UsuarioID | INT IDENTITY(1,1) | NOT NULL | — | PK | Identificador único | Línea 122 |
| NombreUsuario | NVARCHAR(50) | NOT NULL | — | UQ_Usuarios_NombreUsuario | Nombre de usuario único | Línea 123 |
| PasswordHash | NVARCHAR(256) | NOT NULL | — | — | Hash de la contraseña | Línea 124 |
| Rol | VARCHAR(30) | NOT NULL | — | CK_Usuarios_Rol | Rol: Administrador, Empleado o Tecnico | Línea 125 |
| Estado | VARCHAR(20) | NOT NULL | 'Activo' | DF_Usuarios_Estado, CK_Usuarios_Estado | Estado: Activo o Inactivo | Línea 126 |
| TecnicoID | INT | NULL | — | FK_Usuarios_Tecnicos, UQ_Usuarios_TecnicoID | Asociación a técnico (1:1) | Línea 127 |

**Primary Key:** `PK_Usuarios (UsuarioID)`

**Unique Constraint:** `UQ_Usuarios_NombreUsuario (NombreUsuario)`

**Foreign Key:** `FK_Usuarios_Tecnicos (TecnicoID) → Tecnicos(TecnicoID)` — ON DELETE NO ACTION, ON UPDATE NO ACTION

**Unique Index filtrado:** `UQ_Usuarios_TecnicoID ON Usuarios(TecnicoID) WHERE TecnicoID IS NOT NULL` (garantiza relación 0..1 entre Usuarios y Técnicos)

**Check Constraints:**
- `CK_Usuarios_Rol (Rol IN ('Administrador', 'Empleado', 'Tecnico'))`
- `CK_Usuarios_Estado (Estado IN ('Activo', 'Inactivo'))`

**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 119-144

## Índices

| Índice | Tabla | Columnas | Tipo | Propósito | Fuente |
|--------|-------|----------|------|-----------|--------|
| UQ_Usuarios_TecnicoID | Usuarios | TecnicoID | Unique filtrado | Relación 1:1 entre Usuarios y Técnicos | Línea 141-144 |
| IX_Dispositivos_ClienteID | Dispositivos | ClienteID | Regular | Optimizar consultas por cliente | Línea 146-148 |
| IX_OrdenesServicio_DispositivoID | OrdenesServicio | DispositivoID | Regular | Optimizar consultas por dispositivo | Línea 150-152 |
| IX_OrdenesServicio_TecnicoID | OrdenesServicio | TecnicoID | Regular | Optimizar consultas por técnico | Línea 154-156 |
| IX_OrdenesServicio_Estado | OrdenesServicio | Estado | Regular | Optimizar consultas por estado | Línea 158-160 |
| IX_Pagos_OrdenID | Pagos | OrdenID | Regular | Optimizar consultas por orden | Línea 162-164 |

**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 140-164

## Datos de prueba (INSERTs)

### Clientes: 8 registros
- Incluye clientes activos e inactivos (Pedro Sánchez = Inactivo)
- Algunos clientes no tienen Email ni Dirección (Ana López, Roberto Mendoza)
**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 168-176

### Dispositivos: 9 registros
- Tipos: Laptop, Impresora, PC de escritorio, Celular, Tablet
- Marcas: HP, Epson, Dell, Ensamblada, Samsung, Lenovo, Apple, Acer
**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 179-188

### Técnicos: 3 registros
- Luis Ortega (Hardware y reparacion de equipos) — Activo
- Karla Vega (Software y eliminacion de virus) — Activo
- Diego Salinas (Dispositivos moviles) — Inactivo
**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 191-194

### Usuarios: 5 registros
- admin (Administrador) — sin técnico asociado
- recepcion1 (Empleado) — sin técnico asociado
- luis.ortega (Técnico) — asociado al técnico 1
- karla.vega (Técnico) — asociado al técnico 2
- diego.salinas (Técnico) — asociado al técnico 3 (Inactivo)
**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 197-202

### OrdenesServicio: 9 registros
- Estados diversos: Pendiente, En diagnostico, En reparacion, Listo, Entregado
- Costos desde 0 hasta 100.00
- Algunas órdenes sin técnico asignado
**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 205-228

### Pagos: 5 registros
- Montos variados, con múltiples pagos para la misma orden (OrdenID 6 tiene 2 pagos)
**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 230-235

## Consultas incluidas

El archivo SQL incluye las siguientes consultas de ejemplo:

1. **Ordenes por estado** — SELECT con ORDER BY Estado, FechaIngreso (Líneas 240-247)
2. **Detalle de órdenes con joins** — Inner Join Dispositivos, Left Join Tecnicos (Líneas 249-258)
3. **Órdenes finalizadas** — Filtrado por Listo y Entregado, ordenadas por FechaFinalizacion (Líneas 261-270)
4. **Listado de pagos** — Ordenados por FechaPago (Líneas 272-280)
5. **Dispositivos por cliente** — Filtrado por ClienteID = 1 (Líneas 282-285)
6. **Órdenes por dispositivo** — Filtrado por DispositivoID = 1 (Líneas 287-291)
7. **Pagos por orden** — Filtrado por OrdenID = 6 (Líneas 293-297)
8. **Órdenes sin técnico** — WHERE TecnicoID IS NULL (Líneas 299-302)
9. **Órdenes pendientes** — WHERE Estado = 'Pendiente' (Líneas 304-307)
10. **Técnicos activos** — WHERE Estado = 'Activo' (Líneas 309-312)
11. **Usuarios con técnicos** — Left Join Tecnicos (Líneas 314-322)

**Fuente:** `Entregables/Entregable_1/Base de Datos APP de escritorio.sql` — Líneas 238-322

## Información confirmada

- **CONFIRMADO:** No hay vistas, procedimientos almacenados, funciones ni triggers definidos.
- **CONFIRMADO:** El estado de las órdenes tiene 5 valores posibles.
- **CONFIRMADO:** Los métodos de pago son 3: Efectivo, Tarjeta, Transferencia.
- **CONFIRMADO:** Los roles de usuario son 3: Administrador, Empleado, Técnico.
- **CONFIRMADO:** Los estados de clientes y técnicos son 2: Activo, Inactivo.
- **CONFIRMADO:** Existe una relación 1:1 (o 0..1) entre Usuarios y Técnicos mediante el índice único filtrado.

## Información desconocida

- **DESCONOCIDO:** Si existen otros objetos de SQL Server no incluidos en el archivo script (synonyms, assemblies, etc.).
- **DESCONOCIDO:** Si los prefijos de ID visuales en los mockups (C-, D-, T-, ORD-, PAG-, U-) se almacenan como strings en la BD o se generan como formato de visualización sobre los IDs numéricos.
- **DESCONOCIDO:** Si los datos de prueba del SQL coinciden con los datos de demostración de los mockups (no coinciden: el mockup usa nombres como "Ana Díaz" y el SQL usa "Maria Gonzalez").