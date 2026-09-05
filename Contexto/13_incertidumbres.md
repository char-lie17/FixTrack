# Incertidumbres, Conflictos y Recomendaciones

## Información confirmada por los documentos

| Información | Fuente |
|-------------|--------|
| Nombre de la base de datos: FixTrack | SQL script |
| 6 tablas con sus columnas y constraints | SQL script |
| 3 roles de usuario: Administrador, Empleado, Técnico | SQL script + Arquitectura |
| 5 estados de órdenes | SQL script |
| 3 métodos de pago | SQL script |
| 4 reportes oficiales | Manual Identidad Visual |
| Tecnología: C# + WinForms | Manual Identidad Visual |
| Paleta de colores definida | Manual Identidad Visual |
| 3 grupos de menú: Operación, Administración, Información | Arquitectura de Menús |
| 15 mockups disponibles | Carpeta mockups |

## Inferencias realizadas

1. **INFERIDO:** El flujo de estados de órdenes sugiere un orden secuencial, pero la BD no lo enforcea como camino obligatorio.
2. **INFERIDO:** Los técnicos inactivos pueden seguir teniendo órdenes asignadas históricamente (no hay restricción que lo impida).
3. **INFERIDO:** El Administrador es el único rol que puede gestionar usuarios y técnicos.
4. **INFERIDO:** El Técnico solo puede ver sus propias órdenes (aunque la BD no limita esto con una constraint).
5. **INFERIDO:** Los formularios de creación/edición comparten un diseño estándar con campos agrupados en GroupBox.
6. **INFERIDO:** La política de «Cambiar estado» reemplaza a «Eliminar» debido a la restricción ON DELETE NO ACTION de la BD.

## Información desconocida o ambigua

### Base de datos
- DESCONOCIDO: Si existen otros objetos de SQL Server no incluidos en el archivo script.
- DESCONOCIDO: Si se planean vistas, procedimientos almacenados o funciones.
- DESCONOCIDO: Si existe lógica de transición entre estados de órdenes.

### Funcionalidades
- DESCONOCIDO: Submenús específicos dentro de Reportes.
- DESCONOCIDO: Permisos granulares dentro de cada módulo.
- DESCONOCIDO: Validaciones específicas implementadas en cada formulario.
- DESCONOCIDO: Cómo se implementa la restricción «Mis órdenes» para el técnico.
- DESCONOCIDO: Políticas de descuento o ajustes de costo.
- DESCONOCIDO: Reglas de notificación al cliente.

### Interfaz
- DESCONOCIDO: Detalle exacto de controles en cada formulario sin poder leer las imágenes directamente.
- DESCONOCIDO: Archivos de iconos específicos.
- DESCONOCIDO: Si existe autenticación multifactor o políticas de contraseña.

### Negocio
- DESCONOCIDO: Ubicación física de la empresa.
- DESCONOCIDO: Número de empleados o técnicos.
- DESCONOCIDO: Volumen de reparaciones mensuales.
- DESCONOCIDO: Si la empresa tiene otras líneas de negocio.

## Posibles conflictos entre documentos

### Conflicto 1: Clientes — Eliminar vs Cambiar estado
- **Arquitectura de Menús:** Incluye el botón [Eliminar] en la gestión de clientes.
- **Mockup:** Sustituye [Eliminar] por [Cambiar estado] (baja lógica).
- **Base de datos:** ON DELETE NO ACTION impide el borrado si hay dependencias.
- **Impacto:** Se recomienda implementar [Cambiar estado] como el enfoque correcto, consistente con la BD.

**Fuente:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 3

### Conflicto 2: Dispositivos — Eliminar vs Cambiar estado
- **Arquitectura de Menús:** Incluye [Eliminar].
- **Mockup:** Sustituye por [Cambiar estado].
- **Resolución sugerida:** Mismo enfoque que Clientes.

**Fuente:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 4

### Conflicto 3: Dashboard — Cantidad de métricas
- **Arquitectura de Menús:** Muestra 3 métricas (Pendientes, Diagnóstico, En reparación).
- **Mockup:** Amplía a 5 métricas (agrega Listos y Entregados).
- **Impacto:** El mockup incluye información que no estaba en la definición original. Es una ampliación razonable pero no estaba planificada.

**Fuente:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 2

### Conflicto 4: Reportes — Módulo vacío vs mockup con contenido
- **Arquitectura de Menús:** Reportes es un módulo preparado para crecer sin inventar funciones no aprobadas.
- **Mockup:** Propone filtros (rango/estado/técnico) y una tabla de reportes.
- **Impacto:** El mockup ya tiene contenido que necesita aprobación del equipo.

**Fuente:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 10

### Conflicto 5: Usuarios — Técnico asociado no contemplado en la definición
- **Arquitectura de Menús:** No menciona asignación de técnico asociado al usuario.
- **Mockup:** Agrega la asignación de «técnico asociado» al usuario.
- **Base de datos:** La columna `TecnicoID` en `Usuarios` ya soporta esta relación.

**Fuente:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 9

### Conflicto 6: Detalle de orden — Pagos integrados vs separados
- **Arquitectura de Menús:** La gestión de pagos es un módulo aparte.
- **Mockup:** Integra la tabla de pagos dentro del detalle de la orden.
- **Impacto:** Ambos enfoques pueden coexistir, pero requiere claridad sobre cómo navegan entre el detalle de orden y el módulo de pagos.

**Fuente:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 6

## Recomendaciones para la siguiente fase

1. **Definir y aprobar los reportes concretos** antes de comenzar la implementación, ya que el módulo de Reportes no tiene definición específica.
2. **Aprobar los permisos granulares por rol** ya que los permisos exactos todavía son parte pendiente.
3. **Confirmar la política de baja lógica** (Cambiar estado vs Eliminar) para Clientes y Dispositivos, ya que hay discrepancia entre la arquitectura y los mockups.
4. **Definir las reglas de transición entre estados** de las órdenes de servicio (¿qué transiciones son válidas?).
5. **Validar si los pagos integrados en el detalle de orden** reemplazan o complementan al módulo de Pagos independiente.
6. **Confirmar la implementación de «Mis órdenes»** para el rol Técnico (¿cómo se filtran?).
7. **Documentar las validaciones de formulario** específicas para cada pantalla.
8. **Asegurar la coherencia** entre la definición de la arquitectura de menús y los mockups antes de iniciar el desarrollo.
9. **Considerar la preparación de scripts adicionales** de la base de datos si se necesitan vistas o procedimientos almacenados para los reportes.
10. **No modificar, mover, renombrar ni sobrescribir ningún archivo original del Entregable 1.**