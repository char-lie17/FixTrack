# Reconstrucción de la base de datos — FixTrack

Existen **dos caminos válidos** para reconstruir la base de datos `FixTrack` en SQL Server.
Ambos dejan el esquema idéntico al definido en `BD/FixTrack_BD.sql`.

> **Requisitos:** SQL Server (2016 o superior) y permisos de CREATE DATABASE
> (o bien de ALTER sobre la BD existente). La aplicación se conecta con
> `Server=localhost;Database=FixTrack;Integrated Security=true;`
> (cadena en `APP/appsettings.json`).

---

## Opción A — Migraciones de EF Core (recomendada para esta entrega)

Esta es la vía "Code First" que evidencia el uso de EF Core.

1. **Crear el esquema** (7 tablas + `__EFMigrationsHistory`):

   ```powershell
   cd APP
   dotnet tool install --global dotnet-ef   # solo la primera vez
   dotnet ef database update --project FixTrack.csproj --context FixTrackDbContext
   ```

2. **Cargar datos de prueba y el procedimiento almacenado:**

   ```powershell
   sqlcmd -S localhost -E -C -i ..\BD\Seed_Datos_Prueba.sql
   ```

   > `SET QUOTED_IDENTIFIER ON` ya incluido en el seed: es obligatorio porque
   > `Usuarios` tiene un índice único **filtrado** (`UQ_Usuarios_TecnicoID`).

3. **Verificar** (opcional):

   ```powershell
   dotnet run --project ..\APP\TestRunner\TestRunner.csproj
   # Se esperan 44 OK, 0 FALLO(S)
   ```

Si la BD `FixTrack` ya existe y no tiene las tablas del sistema, no hay que hacer nada
extra. Si existieran tablas con nombres equivalentes en otra mayúscula/minúscula
(p. ej. `clientes` de otro ejercicio), renombrarlas antes de migrar:

```sql
EXEC sp_rename 'clientes', 'clientes_ejercicio_anterior';
-- y renombrar sus constraints (PK_/DF_/CK_) que usen los mismos nombres.
```

## Opción B — Script SQL completo (camino histórico del Entregable 1/2)

```powershell
sqlcmd -S localhost -E -C -i FixTrack_BD.sql
```

**Atención:** este script **elimina y recrea** la BD `FixTrack` completa
(`DROP DATABASE` al inicio). Destruye cualquier otro objeto guardado en esa BD.
En una BD donde coexistan tablas de otros ejercicios, usar la **Opción A**.

---

## Credenciales de prueba

| Usuario | Contraseña | Rol |
|---|---|---|
| admin | admin123 | Administrador |
| recepcion1 | recepcion123 | Empleado |
| luis.ortega | tecnico123 | Técnico (TecnicoID 1) |
| karla.vega | tecnico123 | Técnico (TecnicoID 2) |
| diego.salinas | tecnico123 | Técnico inactivo (TecnicoID 3) |

## Contenido de los scripts

- `FixTrack_BD.sql` — script original: DROP/CREATE DATABASE + tablas + índices +
  datos de prueba + `sp_ReporteOrdenesPorEstado` + consultas de verificación.
- `Seed_Datos_Prueba.sql` — solo datos + SP (para la Opción A; no toca el esquema
  ni elimina la BD).
