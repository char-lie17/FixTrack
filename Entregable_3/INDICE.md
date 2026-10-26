# Entrega — Concurrencia + EF Core + Reportes (FixTrack)

**Fecha:** 24 de octubre de 2026 · **Rama:** `Concurrencia-Y-EF-Core` · **Repositorio:** https://github.com/char-lie17/FixTrack.git

## Contenido de esta carpeta

| Ruta | Descripción |
|---|---|
| `FixTrack.zip` | Proyecto completo de Visual Studio (código fuente, sin `bin/`, `obj/` ni `.git/`). 111 archivos, ~5.2 MB. Descomprimir y abrir `APP/FixTrack.sln`. |
| `Documentos/Informe_Entregable.pdf` | Informe del entregable con las 5 capturas incrustadas y la participación de los 5 integrantes (5 páginas). |
| `Documentos/Informe_Entregable.md` | Misma información en Markdown (fuente editable del PDF). |
| `Contexto del Proyecto.md` | Contexto consolidado de todo el proyecto: negocio, alcance, modelo de datos, arquitectura, historial y estado actual. |
| `Evidencias/01…05_*.png` | Capturas de la app ejecutándose con datos reales: login, dashboard, reportes, órdenes, clientes. |
| `Evidencias/06_ejecucion_TestRunner.txt` | Salida de las 44 pruebas E2E: **44 OK, 0 FALLO(S)**. |
| `Evidencias/07_migraciones_EFCore.txt` | Listado de migraciones reconocidas por `dotnet ef`. |
| `Evidencias/08_estado_BD_sqlcmd.txt` | Migración aplicada en `__EFMigrationsHistory` + conteos reales de las 7 tablas. |
| `BD/FixTrack_BD.sql` | Script SQL original del proyecto (**contiene `DROP DATABASE`**). |
| `BD/Seed_Datos_Prueba.sql` | Datos de prueba + `sp_ReporteOrdenesPorEstado` (no destruye la BD). |
| `BD/INSTRUCCIONES_RECONSTRUCCION.md` | Las dos vías de reconstrucción (migraciones recomendadas / script) y credenciales. |

## Resumen de verificación

- `dotnet build FixTrack.sln` → **0 errores**
- `dotnet ef database update` → migración `20261023004419_Inicial` aplicada
- `APP/TestRunner` → **44 OK, 0 FALLO(S)** sobre la BD real
- Aplicación funcionando con datos reales (capturas en `Evidencias/`)
- `master` intacto en `1f20666`; todo el trabajo en `Concurrencia-Y-EF-Core`

## Pendiente antes de entregar

1. **Demo en clase:** preparar la sesión en vivo (login con `admin/admin123`).
