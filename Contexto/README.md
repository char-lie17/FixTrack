# Contexto — FixTrack

## Descripción

Esta carpeta contiene la **documentación de contexto** del proyecto FixTrack. Su propósito es permitir que cualquier desarrollador o IA comprenda el proyecto completo sin tener que reinterpretar desde cero todos los archivos originales del Entregable 1.

## Estructura de archivos

| Archivo | Contenido |
|---------|-----------|
| `01_resumen_ejecutivo.md` | Resumen general del proyecto: qué es FixTrack, tecnología, objetivo |
| `02_empresa_y_objetivo.md` | Descripción de TecnoFix Solutions, objetivo del sistema |
| `03_modulos.md` | Todos los módulos del sistema agrupados por categoría |
| `04_navegacion.md` | Flujo de navegación, menú lateral, menús por rol, mapa de navegación |
| `05_base_de_datos.md` | Estructura completa de la base de datos: tablas, columnas, constraints, índices, datos de prueba, consultas (incluye análisis detallado de cada tabla) |
| `07_relaciones.md` | Relaciones entre entidades, cardinalidades, diagrama de relaciones |
| `08_mockups.md` | Análisis detallado de cada pantalla mockup |
| `09_identidad_visual.md` | Paleta de colores, tipografía, logotipo, controles WinForms, iconografía |
| `10_usuarios_roles.md` | Roles de usuario, accesos por rol, datos de prueba |
| `11_reglas_negocio.md` | Estados, flujo de negocio, reglas de la base de datos |
| `12_decisiones_diseno.md` | Decisiones de diseño documentadas con justificación |
| `13_incertidumbres.md` | Información confirmada, inferencias, desconocidas, conflictos y recomendaciones |
| `14_grafo_relaciones.md` | Grafo completo de conexiones entre todas las partes del sistema |

## Fuentes primarias

> **IMPORTANTE:** Toda la información de esta documentación proviene de los siguientes archivos originales que NO deben modificarse:

```
Entregables/Entregable_1/
├── Base de Datos APP de escritorio.sql          ← Estructura de BD
├── Descripcion_empresa_negocio_FixTrack.pdf      ← Descripción de la empresa
├── mapa_navegacion-0 (1).pdf                     ← Mapa de navegación
├── FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf  ← Identidad visual
├── mockups/
│   ├── FixTrack_Arquitectura_de_Menus_Entregable.pdf  ← Arquitectura de menús
│   ├── FixTrack_Comparativa_Arquitectura_vs_Mockups.pptx  ← Comparativa mockups
│   ├── 01_login.png
│   ├── 02_dashboard.png
│   ├── 03_clientes_lista.png
│   ├── 04_cliente_formulario.png
│   ├── 05_cliente_detalle.png
│   ├── 06_dispositivos_lista.png
│   ├── 07_dispositivo_formulario.png
│   ├── 08_ordenes_lista.png
│   ├── 09_orden_nueva.png
│   ├── 10_orden_detalle.png
│   ├── 11_tecnicos.png
│   ├── 12_pagos.png
│   ├── 13_reportes.png
│   ├── 14_usuarios.png
│   └── 15_menu_manual.png
```

## Cómo usar esta documentación

1. **Para una visión general:** Comience por `01_resumen_ejecutivo.md`.
2. **Para entender la empresa:** Lea `02_empresa_y_objetivo.md`.
3. **Para entender los módulos:** Lea `03_modulos.md` y `04_navegacion.md`.
4. **Para la base de datos:** Lea `05_base_de_datos.md` y `07_relaciones.md`.
5. **Para la interfaz:** Lea `08_mockups.md` y `09_identidad_visual.md`.
6. **Para usuarios y roles:** Lea `10_usuarios_roles.md`.
7. **Para reglas de negocio:** Lea `11_reglas_negocio.md`.
8. **Para decisiones de diseño:** Lea `12_decisiones_diseno.md`.
9. **Para incertidumbres y conflictos:** Lea `13_incertidumbres.md`.
10. **Para el grafo completo:** Lea `14_grafo_relaciones.md`.

## Convenciones de etiquetas

Cada archivo utiliza etiquetas para indicar el nivel de certeza de la información:

- **CONFIRMADO:** Información que aparece explícitamente en una fuente.
- **INFERIDO:** Información que puede deducirse razonablemente, pero no aparece explícitamente.
- **DESCONOCIDO:** Información para la cual no existe suficiente evidencia en las fuentes.

## Relación entre documentos

```
Documentos originales
    ↓
Contexto/ (esta carpeta)
    ↓
Desarrollador/IA que implementa el proyecto
```

## Cómo contribuir

- No modifique los archivos originales del Entregable 1.
- Si encuentra nueva información en los documentos originales, agréguela a esta documentación indicando su fuente.
- Actualice las etiquetas CONFIRMADO/INFERIDO/DESCONOCIDO según corresponda.
- Para resolver conflictos entre documentos, consulte `13_incertidumbres.md`.