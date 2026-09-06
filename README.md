# FixTrack

**Sistema de Gestión de Reparaciones de Dispositivos Electrónicos**

Aplicación de escritorio desarrollada en **C# .NET 10.0** con **Windows Forms** y **ADO.NET** para TecnoFix Solutions. Gestiona el ciclo completo de órdenes de servicio, clientes, dispositivos, técnicos, pagos y reportes.

---

## 🚀 Estado del Proyecto

**Versión actual:** `v1.0-snapshot` (heredado de Henry Creel)

**Nota:** El snapshot inicial (`98ef50f`) se conserva como referencia. Las correcciones posteriores están documentadas en `BUGS_Y_CORRECCIONES.md`.

---

## 📋 Características

### Módulos principales

- **Dashboard:** Métricas en tiempo real (órdenes por estado), filtradas por rol
- **Clientes:** CRUD completo con búsqueda y filtro por estado
- **Dispositivos:** Registro de equipos asociados a clientes
- **Órdenes de Servicio:** Flujo completo de 5 estados (Pendiente → En diagnóstico → En reparación → Listo → Entregado)
- **Pagos:** Registro con validación de saldo, soporte para abonos parciales
- **Historial de órdenes:** Auditoría de creación, estados, ediciones y pagos por orden
- **Técnicos:** Gestión de personal (solo Administrador)
- **Usuarios:** Control de acceso basado en roles (solo Administrador)
- **Reportes:** 4 reportes con filtros de fecha y exportación a CSV

### Roles y permisos

| Módulo | Administrador | Empleado/Recepcionista | Técnico |
|--------|:------------:|:----------------------:|:-------:|
| Dashboard | ✅ | ✅ | ✅ (filtrado) |
| Clientes/Dispositivos | ✅ | ✅ | ❌ |
| Órdenes (todas) | ✅ | ✅ | ✅ (solo propias) |
| Pagos | ✅ | ✅ | ❌ |
| Técnicos/Usuarios | ✅ | ❌ | ❌ |
| Reportes | ✅ | ✅ | ❌ |

---

## 🛠️ Tecnologías

- **.NET 10.0** (Windows Forms)
- **ADO.NET** (sin ORM, consultas SQL parametrizadas)
- **SQL Server** (base de datos relacional)
- **Microsoft.Data.SqlClient** 6.1.1
- **SHA-256** para hashing de contraseñas

---

## 📦 Instalación y Configuración

### Requisitos previos

- Windows 10/11
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server 2019+ o SQL Server Express
- Visual Studio 2022+ (opcional, recomendado para desarrollo)

### Pasos de instalación

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/char-lie17/FixTrack.git
   cd FixTrack
   ```

2. **Configurar la base de datos:**
   
   Ejecuta el script SQL para crear la base de datos y datos de prueba:
   ```bash
   sqlcmd -S localhost\SQLEXPRESS -i "BD\FixTrack_BD.sql"
   ```
   
   O desde SQL Server Management Studio, abre y ejecuta `BD\FixTrack_BD.sql`.

3. **Configurar la cadena de conexión:**
   
   Edita `APP\appsettings.json` y cambia el servidor por el tuyo:
   ```json
   {
     "ConnectionStrings": {
       "FixTrack": "Server=TU_SERVIDOR;Database=FixTrack;TrustServerCertificate=True;Integrated Security=true;"
     }
   }
   ```
   
   Ejemplos comunes:
   - SQL Server Express: `Server=localhost\SQLEXPRESS;...`
   - SQL Server local: `Server=localhost;...`
   - Por nombre de máquina: `Server=TU-PC\SQLEXPRESS;...`

4. **Compilar y ejecutar:**
   ```bash
   cd APP
   dotnet build
   dotnet run
   ```
   
   O abre `APP\FixTrack.sln` en Visual Studio y presiona F5.

### Credenciales de prueba

El script SQL incluye usuarios de demostración:

| Usuario | Contraseña | Rol |
|---------|------------|-----|
| `admin` | `admin123` | Administrador |
| `recepcion1` | `recepcion123` | Empleado |
| `luis.ortega` | `tecnico123` | Técnico |
| `karla.vega` | `tecnico123` | Técnico |

---

## 🗂️ Estructura del Proyecto

```
FixTrack/
├── APP/                          # Aplicación principal
│   ├── Datos/                    # Capa de acceso a datos (DAL)
│   │   ├── ClienteDAL.cs
│   │   ├── DispositivoDAL.cs
│   │   ├── OrdenServicioDAL.cs
│   │   ├── PagoDAL.cs
│   │   ├── TecnicoDAL.cs
│   │   ├── UsuarioDAL.cs
│   │   ├── ReportesDAL.cs
│   │   ├── Conexion.cs          # Gestión de conexión
│   │   └── Seguridad.cs         # Hashing SHA-256
│   ├── Formularios/              # Capa de presentación
│   │   ├── FrmLogin.cs
│   │   ├── FrmDashboard.cs
│   │   ├── FrmClientes.cs       # + Formulario, Detalle
│   │   ├── FrmDispositivos.cs   # + Formulario
│   │   ├── FrmOrdenes.cs        # + Nueva, Detalle
│   │   ├── FrmPagos.cs          # + Formulario, Detalle
│   │   ├── FrmTecnicos.cs       # + Formulario
│   │   ├── FrmUsuarios.cs       # + Formulario
│   │   ├── FrmReportes.cs
│   │   ├── Estilos.cs           # Paleta de colores
│   │   └── UIHelper.cs          # Utilidades UI
│   ├── Modelos/                  # Entidades de dominio
│   │   ├── Cliente.cs
│   │   ├── Dispositivo.cs
│   │   ├── OrdenServicio.cs
│   │   ├── Pago.cs
│   │   ├── Tecnico.cs
│   │   ├── Usuario.cs
│   │   ├── Sesion.cs            # Estado de sesión
│   │   ├── EstadoOrden.cs
│   │   └── MetodoPago.cs
│   ├── appsettings.json          # Configuración
│   └── Program.cs
├── BD/
│   └── FixTrack_BD.sql           # Script de creación de BD
├── Contexto/                     # Documentación de análisis
│   ├── 05_base_de_datos.md
│   ├── 10_usuarios_roles.md
│   ├── 11_reglas_negocio.md
│   └── ...
├── Contexto Actual/
│   └── PROYECTO_COMPLETO.md      # Estado documentado (677 líneas)
├── Entregables/                  # Mockups y diseño
└── Rules.md                      # Reglas de desarrollo
```

---

## 🐛 Bugs Conocidos

### **Bug #1: Tablas tapadas (z-order de docking)** ✅ CORREGIDO

**Módulo:** Todos los formularios de listado + Dashboard  
**Síntoma:** El grid (DataGridView) cubría el header y la barra de filtros — "las tablas salen tapadas".

**Causa raíz:** En WinForms el último control agregado queda al frente del z-order y se dockeriza primero. Los formularios agregaban el control `Dock=Fill` al final, por lo que cubría todo.

**Corrección (commit `f99cd8c`):** Se reordenaron los `Controls.Add()` en 11 formularios para agregar el control `Dock=Fill` primero:
- 7 listados: `FrmClientes`, `FrmDispositivos`, `FrmOrdenes`, `FrmPagos`, `FrmReportes`, `FrmTecnicos`, `FrmUsuarios`
- `FrmDashboard` (contenido → lateral → barra)
- Detalles: `FrmClienteDetalle`, `FrmOrdenDetalle`, `FrmPagoDetalle`

**Estado:** ✅ Resuelto y verificado

---

### **Bug #2: Tabla de Reportes vacía** ✅ CORREGIDO

**Módulo:** `FrmReportes`  
**Síntoma:** Al generar cualquier reporte, la tabla (DataGridView) aparecía vacía.

**Causa raíz:** `UIHelper.ConfigurarGrilla()` establece `AutoGenerateColumns = false`, pero `FrmReportes` no agregaba columnas manualmente.

**Corrección (commit `d1e8c56`):** Se habilitó `AutoGenerateColumns = true` en el grid de reportes.

**Estado:** ✅ Resuelto y verificado

---

### **Bug #3: Búsqueda por ID numérico**

**Módulo:** Varios (Órdenes, Clientes, etc.)  
**Síntoma:** Al buscar por ID numérico, se hace búsqueda exacta por ID.

**Causa raíz:** Comportamiento por diseño (`Buscar()` prioriza búsqueda exacta por ID cuando el texto es numérico). No es un bug.

**Mejora (commit `f99cd8c`):** Los placeholders ahora aclaran que un número busca por ID exacto.

**Estado:** ✅ Documentado (comportamiento correcto)

---

### **Bug #4: Botones desalineados a la derecha** ✅ CORREGIDO

**Módulo:** Varios formularios de lista  
**Síntoma:** Los botones ("+ Nuevo cliente", etc.) no se alineaban a la derecha.

**Causa raíz:** Los `spacer` (`Panel { Size = new Size(20,1) }`) en `FlowLayoutPanel` no funcionan como espaciadores flexibles.

**Corrección (commit `f99cd8c`):** Se reemplazó el layout por `TableLayoutPanel` con columna flexible (AutoSize + Percent) y el panel de botones anclado a la derecha con `RightToLeft`.

**Estado:** ✅ Resuelto

---

## 📖 Documentación

- **[PROYECTO_COMPLETO.md](Contexto%20Actual/PROYECTO_COMPLETO.md):** Documentación exhaustiva del estado actual (arquitectura, correcciones aplicadas, limitaciones)
- **[Rules.md](Rules.md):** Reglas centrales de desarrollo
- **[Contexto/](Contexto/):** Documentación de análisis y diseño (base de datos, roles, reglas de negocio, mockups)
- **Auditorías:** `AUDITORIA_ENTREGABLE_2.md`, `PLAN_REPARACION_ENTREGABLE_2.md`

---

## 🔧 Arquitectura

### Patrón de capas

```
┌─────────────────────────────┐
│   Presentación (Forms)      │  ← WinForms, validaciones UI
├─────────────────────────────┤
│   Modelos (Entidades)       │  ← POCOs con propiedades
├─────────────────────────────┤
│   Datos (DAL)               │  ← ADO.NET, consultas SQL
├─────────────────────────────┤
│   Base de Datos (SQL Server)│  ← 7 tablas relacionales
└─────────────────────────────┘
```

### Modelo de datos

7 tablas principales:

- **Clientes** (1) → (N) **Dispositivos**
- **Dispositivos** (1) → (N) **OrdenesServicio**
- **Tecnicos** (1) → (N) **OrdenesServicio**
- **Tecnicos** (1) ← (0..1) **Usuarios** (relación 1:1 opcional)
- **OrdenesServicio** (1) → (N) **Pagos**
- **OrdenesServicio** (1) → (N) **HistorialOrdenes**

Ver esquema completo en: `Contexto/05_base_de_datos.md`

---

## 🧪 Testing

### Base de datos de prueba

El script SQL (`BD/FixTrack_BD.sql`) incluye:
- 8 clientes (7 activos, 1 inactivo)
- 9 dispositivos
- 3 técnicos
- 5 usuarios (admin, empleado, 3 técnicos)
- 8 órdenes de servicio en varios estados
- 5 pagos de demostración

### Verificación de conexión

Ejecuta `FrmTestConexion` (formulario incluido) para probar la conexión a la base de datos antes de usar la aplicación.

---

## 🛣️ Roadmap

### Correcciones completadas

- [x] ~~Subir snapshot inicial~~
- [x] **Arreglar FrmReportes:** columnas automáticas para los reportes variables
- [x] **Corregir layout:** orden de docking y barras responsivas
- [x] **Corregir flujo de órdenes:** fechas, estados y resultados de DAL
- [x] **Ampliar smoke tests:** filtros, transiciones, pagos y rollback

### Próximas mejoras

- [ ] Añadir pruebas automatizadas aisladas para reglas de negocio sin SQL Server
- [x] Registrar historial de cambios de órdenes y pagos
- [x] Proteger los pagos concurrentes con una operación transaccional
- [ ] Documentar la decisión pendiente sobre `Dispositivos.Estado`

- [ ] Panel de configuración para cambiar servidor SQL desde la UI

---

## 🤝 Contribuir

Este proyecto fue desarrollado como entregable académico. Las contribuciones son bienvenidas:

1. Fork el repositorio
2. Crea una rama para tu feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit tus cambios (`git commit -m 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Abre un Pull Request

### Convenciones de código

- Seguir [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Usar SQL parametrizado (siempre)
- Mantener separación de capas (Presentación → Modelos → Datos)
- Commits en español (proyecto académico latinoamericano)

---

## 📄 Licencia

Este proyecto es de código abierto para fines educativos. TecnoFix Solutions es una empresa ficticia.

---

## 👥 Créditos

- **Desarrollo inicial:** Equipo de desarrollo
- **Revisión y documentación:** Equipo del proyecto
- **Empresa ficticia:** TecnoFix Solutions
- **Contexto académico:** Proyecto de Programación de Aplicaciones de Escritorio

---

## 📞 Contacto

Para preguntas o soporte:
- **GitHub Issues:** [char-lie17/FixTrack/issues](https://github.com/char-lie17/FixTrack/issues)

---

**Última actualización:** 05/09/2026  
**Versión:** 1.0-snapshot (pre-fixes)
