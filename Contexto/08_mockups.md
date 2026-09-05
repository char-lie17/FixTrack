# Mockups — Análisis Detallado de Cada Pantalla

> **Nota:** Este análisis se basa en la inspección visual real de los 15 archivos PNG mediante OCR (EasyOCR), cruzada con el documento de Arquitectura de Menús y la presentación comparativa.
> Los campos marcados con * son obligatorios.

---

## 01 · Login (01_login.png)

**Propósito:** Pantalla de autenticación donde el usuario ingresa sus credenciales.

**Elementos visuales confirmados:**
- Título: **FixTrack**
- Subtítulo: **Inicia sesión para continuar**
- Campo: **Nombre de usuario** (campo de texto)
- Campo: **Contraseña** (campo de contraseña)
- Botón: **Iniciar sesión**
- Texto inferior: **TecnoFix Solutions**
- Ejemplo de usuario pre-cargado: **ana.diaz**
- Mensaje de error visible: **Credenciales inválidas** / **Verifique usuario y contraseña**

**Operaciones:**
- Autenticar al usuario mediante nombre de usuario y contraseña
- Mostrar mensaje de error en caso de credenciales inválidas
- Identificar el rol del usuario para determinar el menú posterior

**Navegación:** → Dashboard (si autenticación exitosa) ← Cerrar sesión (error)

**Fuente:** `Entregables/Entregable_1/mockups/01_login.png`
**Referencia arquitectónica:** `FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Sección 2

---

## 02 · Dashboard (02_dashboard.png)

**Propósito:** Pantalla principal que proporciona una vista rápida del estado actual del negocio.

**Elementos visuales confirmados:**
- Título: **Panel de control**
- Usuario activo: **Ana Díaz**
- Menú lateral con: Inicio, OPERACIÓN (Clientes, Dispositivos, Órdenes, Pagos), ADMINISTRACIÓN (Técnicos, Usuarios), INFORMACIÓN (Reportes)

**Métricas confirmadas (5 indicadores):**
| Estado | Cantidad visible |
|--------|-----------------|
| Pendientes | 8 |
| En diagnóstico | 12 |
| En reparación | 8 |
| Listos | 15 |
| Entregados | 9 |

**Nota sobre métricas:** La definición arquitectónica original mostraba 3 métricas (Pendientes, Diagnóstico, En reparación). El mockup amplía a **5 métricas** (agrega Listos y Entregados). Las cantidades mostradas en el mockup (8, 12, 8, 15, 9) no corresponden a los datos de prueba del SQL.

**Tabla: Órdenes recientes**
Columnas: Orden, Cliente, Dispositivo, Técnico, Fecha ingreso, Estado, Costo
Órdenes visibles: ORD-1042, ORD-1041, ORD-1040, ORD-1039, ORD-1038

**Nota sobre datos:** Las órdenes del mockup usan el prefijo "ORD-" y números superiores a las del SQL de prueba (ORD-1042 vs OrdenID 1-9). El mockup utiliza datos de demostración distintos al script SQL.

**Fuente:** `Entregables/Entregable_1/mockups/02_dashboard.png`
**Referencia arquitectónica:** `FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Sección 3
**Referencia comparativa:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 2

**Operaciones:**
- Visualización de indicadores de estado
- Consulta de órdenes recientes

**Navegación:** ← Menú lateral (todos los módulos), → Cerrar sesión

---

## 03 · Gestión de Clientes — Listado (03_clientes_lista.png)

**Propósito:** Listar todos los clientes con opción de buscar y realizar acciones de gestión.

**Elementos visuales confirmados:**
- Título: **Clientes**
- Botón: **Nuevo cliente**
- Campo de búsqueda: **Buscar**
- Tabla con columnas: **ClienteID** (prefijo C-), **Nombre**, **Apellido**, **Teléfono**, **Email**, **Estado**
- La columna **Estado** es visible en la lista (Activo/Inactivo)
- Datos visibles: C-001, C-002, C-003 (Inactivo), C-004, C-005

**Columnas confirmadas:**
ClienteID | Nombre | Apellido | Teléfono | Email | Estado

**Nota sobre IDs:** Los clientes usan el prefijo **C-** (ej. C-001, C-002). La base de datos usa INT IDENTITY.

**Datos del mockup (diferentes al SQL):**
- C-001 Ana Díaz, +51 987 654 321, ana.diaz@mail.com, Activo
- C-002 Luis Torres, +51 912 345 678, luis.torres@mail.com, Activo
- C-003 María López, +51 998 112 233, maria.lopez@mail.com, Inactivo
- C-004 Jorge Rivas, +51 955 667 788, jorge.rivas@mail.com, Activo
- C-005 Sofía Castro, +51 944 556 677, sofia.castro@mail.com, Activo

**Operaciones:**
- Buscar clientes
- Crear nuevo cliente
- Ver detalle del cliente
- Cambiar estado del cliente (baja lógica)

**Navegación:** ← Menú lateral → Detalle del cliente (05_cliente_detalle.png)

**Fuente:** `Entregables/Entregable_1/mockups/03_clientes_lista.png`
**Referencia arquitectónica:** `FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Sección 6
**Referencia comparativa:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 3

---

## 04 · Cliente — Formulario (04_cliente_formulario.png)

**Propósito:** Formulario para crear o editar un cliente.

**Elementos visuales confirmados:**
- Título: **Cliente**
- Botones: **Nuevo**, **Editar**
- Campos:
  - **Nombre*** (obligatorio)
  - **Apellido**
  - **Teléfono*** (obligatorio)
  - **Email** (opcional)
  - **Dirección**
  - **Estado** (desplegable: Activo)
- Botones: **Guardar**, **Cancelar**

**Datos de ejemplo visibles:**
- Nombre: Ana, Apellido: Díaz
- Teléfono: +51 987 654 321
- Email: ana.diaz@mail.com
- Dirección: Av. Principal 123, Lima
- Estado: Activo

**Fuente:** `Entregables/Entregable_1/mockups/04_cliente_formulario.png`

---

## 05 · Detalle del Cliente (05_cliente_detalle.png)

**Propósito:** Mostrar la información completa de un cliente seleccionado.

**Elementos visuales confirmados:**
- Título: **Cliente** + **Detalle**
- Información visible: Ana Díaz, Activo, +51 987 654 321, ana.diaz@mail.com, Av. Principal 123, Lima
- Sección: **Dispositivos del cliente**
- Tabla con columnas: **DispositivoID** (prefijo D-), **Tipo**, **Marca**, **Modelo**, **Número de serie**, **FechaRegistro**
- Datos: D-001 Laptop HP Pavilion 15 (HP5GH23456, 12/01/2026), D-002 Celular Samsung Galaxy A54 (SM-A54-8891, 20/02/2026)
- Botones: **Editar cliente**, **Nuevo dispositivo**

**Fuente:** `Entregables/Entregable_1/mockups/05_cliente_detalle.png`

**Operaciones:**
- Consultar información completa del cliente
- Ver dispositivos del cliente
- Editar cliente
- Crear nuevo dispositivo

---

## 06 · Gestión de Dispositivos — Listado (06_dispositivos_lista.png)

**Propósito:** Listar todos los dispositivos con opción de buscar y realizar acciones de gestión.

**Elementos visuales confirmados:**
- Título: **Dispositivos**
- Botón: **Nuevo dispositivo**
- Campo de búsqueda: **Buscar**
- Tabla con columnas: **DispositivoID** (prefijo D-), **Cliente**, **Tipo**, **Marca**, **Modelo**, **Número de serie**
- La columna **Estado** NO es visible en el listado de dispositivos
- Datos: D-001 Ana Díaz Laptop HP Pavilion 15, D-002 Luis Torres Celular Samsung Galaxy A54, D-003 María López Laptop Dell Inspiron 14, D-004 Jorge Rivas Celular Apple iPhone 12, D-005 Sofía Castro Laptop Lenovo Legion 5

**Nota sobre IDs:** Los dispositivos usan el prefijo **D-** (ej. D-001, D-002). La base de datos usa INT IDENTITY.

**Fuente:** `Entregables/Entregable_1/mockups/06_dispositivos_lista.png`
**Referencia arquitectónica:** `FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Sección 7
**Referencia comparativa:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 4

**Operaciones:**
- Buscar dispositivos
- Crear nuevo dispositivo
- Editar dispositivo existente
- Cambiar estado del dispositivo
- Ver detalle del dispositivo

---

## 07 · Dispositivo — Formulario (07_dispositivo_formulario.png)

**Propósito:** Formulario para crear o editar un dispositivo.

**Elementos visuales confirmados:**
- Título: **Dispositivo**
- Botones: **Nuevo**, **Editar**
- Campos:
  - **Cliente*** (obligatorio, ComboBox) — ejemplo: Ana Díaz
  - **Tipo*** (obligatorio) — ejemplo: Laptop
  - **Marca** — ejemplo: HP
  - **Modelo** — ejemplo: Pavilion 15
  - **Número de serie** (opcional) — ejemplo: HP5GH23456
  - **Descripción** (campo multilínea) — ejemplo: "Laptop de uso personal. Se entrega con cargador original."
- Botones: **Guardar**, **Cancelar**

**Fuente:** `Entregables/Entregable_1/mockups/07_dispositivo_formulario.png`

**Operaciones:**
- Registrar nuevo dispositivo
- Editar datos del dispositivo
- Seleccionar cliente asociado

---

## 08 · Gestión de Órdenes de Servicio — Listado (08_ordenes_lista.png)

**Propósito:** Listar las órdenes de servicio con filtros y acciones de gestión.

**Elementos visuales confirmados:**
- Título: **Órdenes de servicio**
- Botón: **Nueva orden**
- Campo de búsqueda: **Buscar**
- Filtro: **Todos los estados** (desplegable)
- Tabla con columnas: **OrdenID** (prefijo ORD-), **Cliente**, **Dispositivo**, **Técnico**, **Fecha ingreso**, **Estado**, **Costo**
- Órdenes visibles:
  - ORD-1042: Ana Díaz / HP Pavilion 15 / Carlos Ruiz / 12/08/2026 / En reparación / 845.00
  - ORD-1041: Luis Torres / Samsung A54 / — / 11/08/2026 / Pendiente / $30.00
  - ORD-1040: María López / Dell Inspiron 14 / Carlos Ruiz / 10/08/2026 / En diagnóstico / $25.00
  - ORD-1039: Jorge Rivas / iPhone 12 / Lucía Gómez / 09/08/2026 / Listo / $60.00
  - ORD-1038: Ana Díaz / Lenovo Legion 5 / Lucía Gómez / 08/08/2026 / Entregado / $85.00

**Nota sobre IDs:** Las órdenes usan el prefijo **ORD-** (ej. ORD-1042). La base de datos usa INT IDENTITY. Los datos del mockup no coinciden con los datos de prueba del SQL.

**Fuente:** `Entregables/Entregable_1/mockups/08_ordenes_lista.png`
**Referencia arquitectónica:** `FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Sección 8
**Referencia comparativa:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 5

**Operaciones:**
- Crear nueva orden de servicio
- Buscar órdenes
- Filtrar por estado
- Ver detalle de una orden
- Actualizar estado de una orden

---

## 09 · Nueva Orden de Servicio (09_orden_nueva.png)

**Propósito:** Formulario para crear una nueva orden de servicio.

**Elementos visuales confirmados:**
- Título: **Nueva orden de servicio**
- Campos:
  - **Dispositivo*** (obligatorio, ComboBox) — ejemplo: HP Pavilion 15 (Ana Díaz)
  - **Técnico** (opcional) — ejemplo: Carlos Ruiz
  - **Fecha de ingreso** (DateTimePicker) — ejemplo: 14/08/2026
  - **Problema reportado*** (obligatorio, campo multilínea) — ejemplo: "El equipo no enciende. Se escucha el ventilador pero no arranca el sistema"
  - **Estado** (fijo al crear) — ejemplo: Pendiente
  - **Costo del servicio** (campo numérico) — ejemplo: 45.00
  - **Observaciones** (campo multilínea)
- Botones: **Guardar orden**, **Cancelar**

**Nota importante:** El campo Estado es **fijo al crear** la orden (valor inicial: Pendiente). Esto no se menciona explícitamente en la definición arquitectónica.

**Fuente:** `Entregables/Entregable_1/mockups/09_orden_nueva.png`

**Operaciones:**
- Registrar nueva orden de servicio
- Asignar dispositivo y técnico
- Registrar problema reportado
- El estado se fija automáticamente al crear

---

## 10 · Detalle de una Orden (10_orden_detalle.png)

**Propósito:** Mostrar y gestionar el detalle completo de una orden de servicio, incluyendo seguimiento y pagos.

**Elementos visuales confirmados:**
- Título: **Orden ORD-1042** + **Detalle y seguimiento**
- Usuario activo: **Ana Díaz**
- Menú lateral completo

**Información de la orden:**
- Cliente: Ana Díaz
- Dispositivo: HP Pavilion 15 (D-001)
- Estado: **En reparación**
- Técnico: Carlos Ruiz
- Fecha de ingreso: 12/08/2026

**Sección: Seguimiento**
- Problema reportado: "El equipo no enciende. Se escucha el ventilador pero no arranca el sistema"
- Diagnóstico: "Falla en fuente de poder; se reemplazará la fuente"
- Trabajo realizado: "Se instaló fuente nueva y se probó arranque"
- Estado: En reparación
- Costo del servicio: 845.00
- Fecha de finalización (solo lectura)
- Observaciones: Ninguna

**Sección: Pagos de la orden**
- Tabla con columnas: **PagoID** (prefijo PAG-), **Fecha**, **Monto**, **Método**
- PAG-2001: 12/08/2026, $25.00, Efectivo
- PAG-2002: 14/08/2026, $20.00, Transferencia
- Botón: **Registrar pago**

**Fuente:** `Entregables/Entregable_1/mockups/10_orden_detalle.png`
**Referencia arquitectónica:** `FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Sección 8.1
**Referencia comparativa:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 6

**Operaciones:**
- Consultar detalle de la orden
- Ver seguimiento (diagnóstico, trabajo realizado)
- Ver y registrar pagos
- Avanzar el estado de la orden
- Actualizar diagnóstico, trabajo realizado, costo, observaciones

---

## 11 · Gestión de Técnicos (11_tecnicos.png)

**Propósito:** Listar y gestionar los técnicos del sistema.

**Elementos visuales confirmados:**
- Título: **Técnicos**
- Botón: **Nuevo técnico**
- Campo de búsqueda: **Buscar**
- Tabla con columnas: **TécnicoID** (prefijo T-), **Nombre**, **Apellido**, **Teléfono**, **Especialidad**, **Estado**
- Datos visibles:
  - T-01 Carlos Ruiz, +51 977 111 222, Laptops, Hardware, Activo
  - T-02 Lucía Gómez, +51 966 333 444, Celulares, Reparación, Activo
  - T-03 Pedro Mendoza, +51 955 555 666, Redes, Impresoras, Inactivo

**Nota sobre IDs:** Los técnicos usan el prefijo **T-** (ej. T-01). La base de datos usa INT IDENTITY.

**Nota sobre Especialidad:** El mockup muestra valores de especialidad diferentes a los del SQL de prueba. El SQL tiene "Hardware y reparacion de equipos", "Software y eliminacion de virus", "Dispositivos moviles". El mockup muestra "Laptops/Hardware", "Celulares/Reparación", "Redes/Impresoras".

**Fuente:** `Entregables/Entregable_1/mockups/11_tecnicos.png`
**Referencia arquitectónica:** `FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Sección 11
**Referencia comparativa:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 8

**Operaciones:**
- Crear nuevo técnico
- Editar técnico existente
- Ver detalle del técnico
- Cambiar estado del técnico

---

## 12 · Gestión de Pagos (12_pagos.png)

**Propósito:** Listar y registrar los pagos asociados a las órdenes de servicio.

**Elementos visuales confirmados:**
- Título: **Pagos**
- Botón: **Registrar pago**
- Campo de búsqueda: **Buscar**
- Tabla con columnas: **PagoID** (prefijo PAG-), **Orden**, **Cliente**, **Fecha**, **Monto**, **Método**
- Datos visibles:
  - PAG-2001 / ORD-1042 / Ana Díaz / 12/08/2026 / $25.00 / Efectivo
  - PAG-2002 / ORD-1042 / Ana Díaz / 14/08/2026 / $20.00 / Transferencia
  - PAG-1999 / ORD-1039 / Jorge Rivas / 09/08/2026 / $60.00 / Tarjeta
  - PAG-1998 / ORD-1038 / Ana Díaz / 08/08/2026 / $85.00 / Efectivo

**Fuente:** `Entregables/Entregable_1/mockups/12_pagos.png`
**Referencia arquitectónica:** `FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Sección 9
**Referencia comparativa:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 7

**Operaciones:**
- Ver lista de pagos
- Registrar nuevo pago
- Ver detalle de un pago

---

## 13 · Reportes (13_reportes.png)

**Propósito:** Panel de consulta y generación de reportes.

**Elementos visuales confirmados:**
- Título: **Reportes**
- Filtros: **Reporte** (desplegable), **Desde**, **Hasta**, **Generar**, **Exportar**
- Reporte seleccionado: **Órdenes por estado**
- Período: 01/08/2026 al 14/08/2026

**Tabla de resultados:**
| Estado | Cantidad de órdenes | Subtotal |
|--------|---------------------|----------|
| Pendiente | 12 | $8,360.00 |
| En diagnóstico | 8 | $200.00 |
| En reparación | 15 | $675.00 |
| Listo | — | $540.00 |
| Entregado | 23 | $1,955.00 |

**Pie de página:** TecnoFix Solutions — FixTrack

**Nota importante:** A diferencia de lo indicado en la definición arquitectónica (que decía que el equipo no había definido reportes concretos), el mockup muestra un reporte funcional con filtros de fecha, botón Generar, botón Exportar y una tabla de resultados con totales por estado.

**Fuente:** `Entregables/Entregable_1/mockups/13_reportes.png`
**Referencia arquitectónica:** `FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Sección 14
**Referencia comparativa:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 10

**Operaciones:**
- Seleccionar tipo de reporte
- Definir rango de fechas (Desde/Hasta)
- Generar reporte
- Exportar reporte
- Ver tabla de resultados con totales

---

## 14 · Gestión de Usuarios (14_usuarios.png)

**Propósito:** Listar y gestionar los usuarios del sistema y sus roles.

**Elementos visuales confirmados:**
- Título: **Usuarios**
- Botón: **Nuevo usuario**
- Campo de búsqueda: **Buscar**
- Tabla con columnas: **UsuarioID** (prefijo U-), **Nombre de usuario**, **Nombre**, **Rol**, **Estado**, **Técnico asociado**

**Datos visibles:**
- U-01 / admin / Ana Díaz / Administrador / Activo / —
- U-02 / recepcion / Luis Torres / Empleado Recepcionista / Activo / —
- U-03 / carlos ruiz / Carlos Ruiz / Técnico / Activo / T-01
- U-04 / lucia.gomez / Lucía Gómez / Técnico / Activo / T-02
- U-05 / jorge.rivas / Jorge Rivas / Técnico / Inactivo / T-03

**Nota sobre columnas:** El mockup incluye la columna **Técnico asociado**, no contemplada en la definición arquitectónica original. Esto está soportado por la columna `TecnicoID` en la tabla `Usuarios` de la base de datos.

**Nota sobre el rol:** El mockup muestra el rol como **"Empleado Recepcionista"**, no solo "Empleado" como se indicaba en la definición arquitectónica.

**Nota sobre IDs:** Los usuarios usan el prefijo **U-** (ej. U-01). La base de datos usa INT IDENTITY.

**Fuente:** `Entregables/Entregable_1/mockups/14_usuarios.png`
**Referencia arquitectónica:** `FixTrack_Arquitectura_de_Menus_Entregable.pdf` — Sección 12
**Referencia comparativa:** `FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx` — Slide 9

**Operaciones:**
- Crear nuevo usuario
- Editar usuario existente
- Cambiar estado del usuario
- Asignar técnico asociado al usuario

---

## 15 · Menú Manual / Identidad Visual (15_menu_manual.png)

**Propósito:** Documentación de identidad visual del sistema.

**Elementos visuales confirmados:**
- Título: **FixTrack**
- Menú: Gestión, Dashboard, Reportes, Administración, Cerrar sesión
- Sub-menú Clientes: Nuevo, Buscar
- Tabla de ejemplo con columnas: ClienteID, Nombre, Apellido, Teléfono, Estado
- Datos de ejemplo: C-001 Ana Díaz, C-002 Luis Torres

**Fuente:** `Entregables/Entregable_1/mockups/15_menu_manual.png`
**Referencia:** `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf`

---

## Hallazgos clave del OCR (diferencias con documentación previa)

| Hallazgo | Detalle | Impacto |
|----------|---------|---------|
| Prefijos de ID | Los IDs visuales usan prefijos (C-, D-, T-, ORD-, PAG-, U-) mientras la BD usa INT IDENTITY | El sistema de IDs puede diferir entre la interfaz y la base de datos |
| 5 métricas en Dashboard | No 3 como decía la arquitectura original | Se confirma la ampliación del PPTX comparativa |
| "Empleado Recepcionista" | El rol se muestra así en el mockup, no solo "Empleado" | El nombre del rol puede diferir de la arquitectura |
| "Técnico asociado" en Usuarios | Columna visible en el mockup | No estaba en la definición arquitectónica |
| Reportes con contenido | El mockup muestra reportes funcionales con filtros y datos | Contradice la afirmación de que no se habían definido |
| Estado fijo al crear orden | El estado se fija automáticamente al crear | No se mencionaba en la arquitectura |
| "Descripción" en dispositivos | Campo visible en el formulario | No se mencionaba en la arquitectura para el formulario |
| "Fecha de finalización (solo lectura)" | Campo visible en detalle de orden | No se mencionaba como de solo lectura |
| Mensaje de error en Login | "Credenciales inválidas" | No se documentaba previamente |
| Datos del mockup ≠ datos del SQL | Los datos visibles son distintos a los INSERTs del SQL | El script SQL tiene datos de prueba distintos a los del mockup |

---

## Resumen de diferencias entre definición arquitectónica y mockups (actualizado)

| Pantalla | Diferencia | Fuente |
|----------|-----------|--------|
| Dashboard | 3 métricas vs 5 métricas | Slide 2 PPTX + OCR |
| Clientes | [Eliminar] vs [Cambiar estado] | Slide 3 PPTX |
| Dispositivos | [Eliminar] vs [Cambiar estado] | Slide 4 PPTX |
| Órdenes | Badges de color + paginación | Slide 5 PPTX |
| Detalle de orden | Tabla de pagos integrada | Slide 6 PPTX |
| Pagos | Misma tabla reutilizada en detalle | Slide 7 PPTX |
| Usuarios | Asignación de técnico asociado | Slide 9 PPTX |
| Reportes | Módulo con contenido funcional | Slide 10 PPTX + OCR |
| Login | Mensaje de error de credenciales | OCR |
| IDs | Prefijos visuales vs INT en BD | OCR |
| Rol | "Empleado Recepcionista" vs "Empleado" | OCR |
| Nueva orden | Estado fijo al crear | OCR |
| Dispositivo | Campo "Descripción" en formulario | OCR |
| Datos | Mockup ≠ datos de prueba SQL | OCR |

---

## Información confirmada

- **CONFIRMADO:** Existen 15 archivos PNG de mockups más 2 archivos PDF/architectura en la carpeta mockups.
- **CONFIRMADO:** Los mockups cubren todas las pantallas definidas en la arquitectura de menús.
- **CONFIRMADO:** La política de «Cambiar estado» reemplaza a «Eliminar» en Clientes y Dispositivos.
- **CONFIRMADO:** El Dashboard muestra 5 métricas de estado.
- **CONFIRMADO:** El Login tiene validación de credenciales con mensaje de error.
- **CONFIRMADO:** Los IDs visuales usan prefijos alfanuméricos.
- **CONFIRMADO:** Los reportes tienen contenido funcional con filtros y totales.
- **CONFIRMADO:** El campo Estado se fija automáticamente al crear una orden.

## Información inferida

- **INFERIDO:** Los formularios de creación/edición comparten un diseño estándar con campos agrupados en GroupBox.
- **INFERIDO:** Los prefijos de ID (C-, D-, T-, etc.) podrían implementarse como strings o como formato de visualización sobre los IDs numéricos de la BD.
- **INFERIDO:** Los datos del mockup son datos de demostración diferentes a los del script SQL de prueba.

## Información desconocida

- **DESCONOCIDO:** Si los prefijos de ID se almacenan como strings en la BD o se generan como formato de visualización.
- **DESCONOCIDO:** Las validaciones específicas implementadas en cada formulario.
- **DESCONOCIDO:** El formato exacto del campo "Técnico asociado" en la tabla Usuarios del mockup.
- **DESCONOCIDO:** Si existen más campos en el formulario de dispositivo que no se capturaron en el OCR.