# FixTrack

**Sistema de Gestión de Reparaciones de Dispositivos Electrónicos**

Aplicación de escritorio desarrollada en **C# .NET 10.0** con **Windows Forms** y **ADO.NET** para TecnoFix Solutions. Gestiona el ciclo completo de órdenes de servicio, clientes, dispositivos, técnicos, pagos y reportes.

---

## 🚀 Estado del Proyecto

**Versión actual:** `v1.0-snapshot` (heredado de Henry Creel)

⚠️ **Nota importante:** Este commit inicial (`98ef50f`) preserva el estado tal cual fue entregado. Contiene bugs conocidos que serán corregidos en commits posteriores. Ver sección [Bugs Conocidos](#-bugs-conocidos).

---

## 📋 Características

### Módulos principales

- **Dashboard:** Métricas en tiempo real (órdenes por estado), filtradas por rol
- **Clientes:** CRUD completo con búsqueda y filtro por estado
- **Dispositivos:** Registro de equipos asociados a clientes
- **Órdenes de Servicio:** Flujo completo de 5 estados (Pendiente → En diagnóstico → En reparación → Listo → Entregado)
- **Pagos:** Registro con validación de saldo, soporte para abonos parciales
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

### **Bug #1: Tabla de Reportes vacía** (crítico)

**Módulo:** `FrmReportes`  
**Síntoma:** Al generar cualquier reporte, la tabla (DataGridView) aparece completamente vacía, aunque los datos se cargan correctamente.

**Causa raíz:** `UIHelper.ConfigurarGrilla()` establece `AutoGenerateColumns = false`, pero `FrmReportes` nunca agrega columnas manualmente. Al asignar un `DataTable` como `DataSource`, el grid no tiene columnas para mostrar.

**Ubicación:** `APP/Formularios/FrmReportes.cs:40-42`

**Estado:** Pendiente de corrección

---

### **Bug #2: Filtros de búsqueda (menor)**

**Módulo:** Varios (Órdenes, Clientes, etc.)  
**Síntoma:** En algunos casos, al buscar por ID numérico, los filtros de estado/fecha se ignoran.

**Causa raíz:** Lógica intencional en los DAL (`Buscar()` prioriza búsqueda exacta por ID cuando el texto es numérico), pero puede resultar confusa.

**Estado:** Comportamiento por diseño, puede mejorarse en UX

---

### **Bug #3: Layout de botones (cosmético)**

**Módulo:** Varios formularios de lista  
**Síntoma:** Los botones ("+ Nuevo cliente", etc.) no se alinean completamente a la derecha como en los mockups.

**Causa raíz:** Los `spacer` (`Panel { Size = new Size(20,1) }`) en `FlowLayoutPanel` no funcionan como espaciadores flexibles.

**Estado:** Cosmético, no afecta funcionalidad

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
│   Base de Datos (SQL Server)│  ← 6 tablas relacionales
└─────────────────────────────┘
```

### Modelo de datos

6 tablas principales:

- **Clientes** (1) → (N) **Dispositivos**
- **Dispositivos** (1) → (N) **OrdenesServicio**
- **Tecnicos** (1) → (N) **OrdenesServicio**
- **Tecnicos** (1) ← (0..1) **Usuarios** (relación 1:1 opcional)
- **OrdenesServicio** (1) → (N) **Pagos**

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

### Próximas correcciones (commit siguiente)

- [x] ~~Subir snapshot inicial~~ (este commit)
- [ ] **Arreglar FrmReportes:** Agregar `AutoGenerateColumns = true` o definir columnas manualmente
- [ ] **Mejorar layout:** Usar `Dock = Fill` con `FlowLayoutPanel.AutoSize` correcto
- [ ] **Agregar documentación inline:** Comentarios XML en clases públicas
- [ ] **Tests unitarios:** Proyecto de test para validar lógica de negocio

### Futuras mejoras

- [ ] Implementar Entity Framework Core (reemplazar ADO.NET puro)
- [ ] Agregar campo `Estado` a tabla `Dispositivos` (baja lógica)
- [ ] Exportar reportes a PDF (actualmente solo CSV)
- [ ] Notificaciones/alertas para órdenes antiguas
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

- **Desarrollo inicial:** Henry Creel
- **Revisión y documentación:** Carlos Castillo (char-lie17)
- **Empresa ficticia:** TecnoFix Solutions
- **Contexto académico:** Proyecto de Programación Orientada a Objetos

---

## 📞 Contacto

Para preguntas o soporte:
- **GitHub Issues:** [char-lie17/FixTrack/issues](https://github.com/char-lie17/FixTrack/issues)
- **Email:** carlosrenato05200@gmail.com

---

**Última actualización:** 05/09/2026  
**Versión:** 1.0-snapshot (pre-fixes)
