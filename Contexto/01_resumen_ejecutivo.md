# Resumen Ejecutivo — FixTrack

## Qué es FixTrack

FixTrack es un sistema de gestión interno para **TecnoFix Solutions**, una empresa dedicada a la reparación y mantenimiento de computadoras y dispositivos electrónicos. Es una aplicación de escritorio que permite organizar y centralizar la información de sus servicios de reparación.

## Qué problema resuelve

La empresa necesitaba un medio para registrar y consultar clientes y dispositivos, crear y dar seguimiento a órdenes de servicio, asignar técnicos, controlar el estado de las reparaciones y registrar los pagos asociados a los servicios realizados, todo de forma organizada y centralizada.

## Objetivo del sistema

Desarrollar una aplicación de escritorio que permita gestionar y consultar de forma organizada la información generada durante el proceso de servicio, desde el registro del cliente y su dispositivo hasta la finalización y entrega del mismo.

## Tecnología

- **Lenguaje:** C#
- **Framework:** Windows Forms (WinForms)
- **Base de datos:** SQL Server (archivo script: `Base de Datos APP de escritorio.sql`)
- **Tipo de aplicación:** Escritorio (desktop)
- **Asignatura:** Programación de Aplicaciones de Escritorio

## Fuente principal

`Entregables/Entregable_1/Base de Datos APP de escritorio.sql`
`Entregables/Entregable_1/Descripcion_empresa_negocio_FixTrack.pdf`

## Alcance

La aplicación es una herramienta interna para administrar clientes, dispositivos, órdenes de servicio, técnicos, pagos, usuarios y reportes del sistema. No incluye materiales de marketing ni branding corporativo general.

Fuente: `Entregables/Entregable_1/FixTrack_Manual_Identidad_Visual_Deliverable_organized (1).pdf` — Página 1-2

## Nota sobre identificadores

La base de datos utiliza columnas INT IDENTITY para las claves primarias. Sin embargo, los mockups muestran identificadores visuales con prefijos alfanuméricos (C-001, D-001, T-01, ORD-1042, PAG-2001, U-01). No se ha documentado si estos prefijos se almacenan como strings en la base de datos o se generan como formato de visualización.

Fuente: `Entregables/Entregable_1/mockups/*.png` (inspección visual OCR)