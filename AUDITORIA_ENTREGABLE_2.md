# AUDITORÍA — Estado del Entregable 2 (FixTrack)

**Fecha de auditoría:** 03/09/2026  
**Alcance:** Verificar qué se ha implementado, qué está pendiente, y si las tareas definidas en `FASE_3_PLANIFICACION.md` coinciden con las instrucciones originales del Entregable 2.

---

## 1. ¿El proyecto tiene tareas definidas además del contexto?

**SÍ.** El proyecto tiene tres capas de documentación:

| Documento | Rol |
|---|---|
| `Contexto/` (15 archivos) | Análisis del Entregable 1 (fuente de verdad del dominio) |
| `FASE_3_PLANIFICACION.md` (1481 líneas) | **Plan técnico del Entregable 2**: 15 fases de implementación (3.1–3.15) con criterios de aceptación, 17 formularios, 10 modelos, 8 DALs, plan de navegación, roles, validaciones, pruebas y riesgos |
| `Rules.md` | Reglas obligatorias de tecnología y calidad |

El propio plan declara: *"FASE 3 COMPLETADA — PLAN DE IMPLEMENTACIÓN PREPARADO"*.

---

## 2. Matriz de coincidencia: instrucciones originales vs. plan definido

| # | Requisito original (Entregable 2) | ¿Cubierto por el plan? | Dónde |
|---|---|---|---|
| 1.1 | Formularios en Windows Forms | SÍ | Sección 6 (17 formularios planificados), Rules §1 |
| 1.2 | Controles adecuados | SÍ | Sección 6 + `Contexto/09_identidad_visual.md` |
| 1.3 | Eventos asociados a controles | SÍ | Sección 6 (comportamiento por formulario) |
| 1.4 | Navegación entre formularios | SÍ | Sección 7 (flujo completo + por rol) |
| 1.5 | Validación de datos | SÍ | Sección 10 (por formulario + restricciones BD) |
| 1.6 | Mensajes, diálogos, confirmaciones | SÍ | Secciones 6 y 10 |
| 1.7 | Coherencia con Entregable 1 (mockups) | SÍ | Sección 18 (matriz de trazabilidad) + identidad visual |
| 2.1 | Conexión a SQL Server con ADO.NET | SÍ | Sección 4.2 |
| 2.2 | Usar la BD de Base de Datos I | SÍ | Sección 4 + script SQL del Entregable 1 |
| 2.3 | Consultas parametrizadas | SÍ | Sección 4.2 + Rules §9 |
| 2.4 | Procedimientos almacenados "cuando corresponda" | PARCIAL | La BD original **no tiene SPs** (confirmado en Informe Fase 1). El plan usa ADO.NET directo; no hay plan de crear SPs. Aceptable por el "cuando corresponda", pero conviene documentar la decisión (o agregar SPs al script actualizado) |
| 2.5 | CRUD completo (Crear, Leer, Modificar, Eliminar/baja) | SÍ | Sección 9 (CRUD por módulo). El "eliminar" se alinea con la baja lógica documentada |
| 2.6 | Consultas y filtros de información | SÍ | Sección 4.4 |
| 2.7 | Manejo de errores y excepciones | SÍ | Sección 4.6 + Rules §9 |
| 2.8 | Transacciones donde sean necesarias | SÍ | Sección 4.7 |
| 2.9 | Integración BD <-> formularios | SÍ | Secciones 3, 5 y 6 |
| 3.1 | Entregar proyecto VS completo que compile | PARCIAL | Los criterios de aceptación exigen compilación, pero **el plan no incluye explícitamente el paquete final de entrega** |
| 3.2 | Entregar **script actualizado** de la BD | NO | Ninguna fase menciona producir/actualizar el script SQL de la BD para la entrega |
| 4 | App funcional demostrando interacción UI <-> BD | SÍ | Criterios de aceptación por fase + Sección 14 (pruebas) |
| 5 | Todos los integrantes deben poder defenderlo | SÍ | Principios rectores: "código fácil de entender y defender" |

**Conclusión:** el plan definido **sí corresponde** a las instrucciones originales del Entregable 2 (coincidencia ~95%). Las brechas menores son: (a) no hay tarea explícita para el **script actualizado de la BD** exigido en la entrega, y (b) no hay tarea de empaquetado/entrega final.

---

## 3. Estado de implementacion (verificado sobre el codigo real)

| Fase del plan | Estado | Evidencia |
|---|---|---|
| 3.1 Preparacion del proyecto | COMPLETA (con desvios menores) | `FixTrack.sln`, `FixTrack.csproj` (WinForms, net10.0-windows), carpetas `Configuracion/`, `Modelos/`, `Formularios/`, `appsettings.json` |
| 3.2 Conexion ADO.NET | COMPLETA | `Configuracion/Conexion.cs` (cadena desde appsettings.json, `SqlConnection`), `Formularios/FrmTestConexion.cs` con 5 pruebas (config, conexion, SELECT, parametrizada, error controlado), `TestRunner/` y `TestData/test_connection.csx`. **Compila: 0 errores, 0 advertencias** (ver hallazgo 1) |
| 3.3 Modelos | ~55% | Existen: `Cliente.cs`, `Dispositivo.cs`, `Tecnico.cs`, `OrdenServicio.cs`, `Pago.cs`, `Usuario.cs` (POCOs mapeados a la BD). **Faltan:** `EstadoOrden.cs` (enum 5 estados), `MetodoPago.cs` (enum), `Sesion.cs`, `ReporteResultado.cs` |
| 3.4 DALs (acceso a datos) | NO INICIADA | Carpeta `Datos/` vacia (Conexion.cs vive en `Configuracion/`). Sin `ClienteDAL`, `OrdenServicioDAL`, etc. |
| 3.5 Login | NO INICIADA | No existe `FrmLogin` |
| 3.6 Dashboard | NO INICIADA | No existe `FrmDashboard` |
| 3.7 Clientes | NO INICIADA | Sin formularios ni DAL |
| 3.8 Dispositivos | NO INICIADA | idem |
| 3.9 Ordenes de servicio | NO INICIADA | idem |
| 3.10 Tecnicos | NO INICIADA | idem |
| 3.11 Pagos | NO INICIADA | idem |
| 3.12 Usuarios | NO INICIADA | idem |
| 3.13 Reportes | NO INICIADA | idem |
| 3.14 Integracion (navegacion, roles) | NO INICIADA | idem |
| 3.15 Pruebas finales | NO INICIADA | idem |

**Avance global estimado: ~15-20% del Entregable 2.** Se completo la infraestructura (proyecto + conexion probada contra SQL Server) y la mayoria de los modelos.

---

## 4. Pendiente (orden sugerido segun el propio plan)

1. Completar Modelos: enums `EstadoOrden` y `MetodoPago`, clases `Sesion` y `ReporteResultado` (Fase 3.3).
2. Crear los 7 DALs con CRUD parametrizado, filtros, manejo de errores y transacciones (Fase 3.4).
3. `FrmLogin` + sesion de usuario (Fase 3.5).
4. `FrmDashboard` con 5 metricas y ordenes recientes (Fase 3.6).
5. CRUDs con formularios: Clientes -> Dispositivos -> Ordenes -> Tecnicos -> Pagos -> Usuarios (Fases 3.7-3.12).
6. Reportes: 4 reportes oficiales + filtros de fecha + exportacion (Fase 3.13).
7. Integracion: menu, navegacion, permisos por rol, "Mis ordenes" del tecnico, cierre de sesion, identidad visual (Fase 3.14).
8. Pruebas finales y compilacion limpia (Fase 3.15).
9. **Preparar entrega:** proyecto completo + **script actualizado de la BD** (brecha detectada, ver seccion 2).

**Decisiones de equipo aun abiertas (10 puntos pendientes del plan, seccion 19):** columna Estado en Dispositivos, transiciones de estado de ordenes, submenus/formato de exportacion de Reportes, "Mis ordenes", permisos granulares, formato de IDs con prefijos, algoritmo de hasheo de contrasenas, politica de contrasenas, pagos integrados vs. modulo.

---

## 5. Hallazgos de la auditoria

1. **[CORREGIDO] El proyecto NO compilaba.** `TestRunner` (proyecto de consola auxiliar) esta anidado dentro de `APP\` y su `obj\Debug\...\*.AssemblyInfo.cs` autogenerado era incluido por el globbing de `FixTrack.csproj`, produciendo 8 errores `CS0579` (atributos duplicados). Se corrigio anadiendo en `FixTrack.csproj`:
   ```xml
   <Compile Remove="TestRunner\**" />
   <None Remove="TestRunner\**" />
   <EmbeddedResource Remove="TestRunner\**" />
   ```
   Verificado: `dotnet build` -> **0 errores, 0 advertencias**. Recomendacion a futuro: mover `TestRunner` fuera de `APP\` o agregarlo a la solucion como proyecto separado.
2. **Residuos de plantilla:** `Form1.cs`, `Form1.Designer.cs`, `Form1.resx` no se usan (Program.cs lanza `FrmTestConexion`). Eliminarlos antes de la entrega.
3. **Desvios aceptables respecto al plan (documentar):** `appsettings.json` en lugar de `app.config`; `Microsoft.Data.SqlClient` (NuGet) en lugar de `System.Data.SqlClient` - sigue siendo ADO.NET puro (sin ORM), cumple Rules seccion 1; target `net10.0-windows` (Rules permite .NET 6+).
4. **Ruta de Entregables:** la carpeta real se llama `Deliverables 1 (Entregable 1)`, pero Rules/Contexto la referencian como `Entregable_1`. Renombrar carpeta o actualizar referencias para evitar confusion.
5. **Carpetas vacias:** `Datos/` y `Recursos/` existen pero sin contenido (pendiente de las fases correspondientes).

---

## 6. Respuesta corta a la pregunta

- **Hay tareas definidas ademas del contexto?** SI: `FASE_3_PLANIFICACION.md` define el plan de trabajo completo del Entregable 2 en 15 fases.
- **Que se ha hecho?** Fases 3.1 y 3.2 completas (proyecto + conexion ADO.NET probada), modelos al ~55%. Compila correctamente tras la correccion del hallazgo 1.
- **Que esta pendiente?** Todo el nucleo del entregable: DALs con CRUD, Login, Dashboard, los 6 modulos, Reportes, navegacion/roles, pruebas y el paquete de entrega con el script actualizado de la BD.
- **Las tareas definidas coinciden con las instrucciones originales?** SI, ~95%. Solo faltan en el plan: la tarea explicita del **script actualizado de la BD** y el empaquetado final de entrega.

---

## 7. ACTUALIZACION POST-AUDITORIA (03/09/2026) — ESTADO ACTUALIZADO

Tras la auditoría se avanzó en la implementación siguiendo el orden del plan:

| Fase | Antes | Ahora |
|---|---|---|
| Infraestructura | Build roto (TestRunner anidado) | Build 0 errores/0 advertencias; cadena de conexion corregida a `Server=.\SQLEXPRESS` |
| 3.2 Conexion | Correcta | Validada contra SQL Server (SQLEXPRESS) real |
| 3.3 Modelos | ~55% | 100%: se anadieron `EstadoOrden` (enum), `MetodoPago` (enum), `Sesion`, `ReporteResultado` y propiedades de visualizacion para JOINS |
| 3.4 DALs | No iniciado | 100%: `ClienteDAL`, `DispositivoDAL`, `TecnicoDAL`, `OrdenServicioDAL`, `PagoDAL`, `UsuarioDAL`, `ReportesDAL` + `Seguridad` (SHA-256). Todas las consultas parametrizadas, conexiones en `using` |
| 3.5 Login | No iniciado | `FrmLogin` funcional (mockup 01): valida credenciales y estado, establece Sesion, navega a Dashboard |
| 3.6 Dashboard | No iniciado | `FrmDashboard` con menu lateral por rol (mockup 02): 5 metricas y ordenes recientes |
| 3.7 Clientes | No iniciado | `FrmClientes` + `FrmClienteFormulario` + `FrmClienteDetalle` (mockups 03-05): CRUD, busqueda, filtro estado, baja logica, detalle con dispositivos |
| 3.8-3.13 Modulos | No iniciado | Esqueletos navegables creados: Dispositivos, Ordenes, Pagos, Tecnicos, Usuarios, Reportes (CRUD pendiente) |
| Script BD actualizado | Faltante | Creado: `BD/FixTrack_BD.sql` con hashes SHA-256 reales (credenciales demo: admin/admin123, recepcion1/recepcion123, luis.ortega/tecnico123, karla.vega/tecnico123, diego.salinas/tecnico123) |
| Validacion de datos | - | `TestRunner` (ahora smoke test DAL): **26/26 pruebas OK contra SQL Server** (CRUD real) |

**Pendiente principal:** CRUD completo de Dispositivos, Ordenes (con detalle y cambio de estado), Pagos, Tecnicos, Usuarios y Reportes; integracion fina de roles; pruebas por rol y validaciones finales.

**Decisiones documentadas (la BD no las obliga, se documentan para la defensa):**
- Algoritmo de contrasenas: SHA-256 (hex). Sin dependencias externas.
- Transiciones de estado de ordenes: libres (la BD no las restringe); al pasar a Listo/Entregado se fija FechaFinalizacion.
- Exportacion de reportes: se sugiere CSV (pendiente de confirmar con el equipo).
- Cadena de conexion: `Server=.\SQLEXPRESS` (instancia local detectada; el plan ya preveia confirmarla en el entorno).
