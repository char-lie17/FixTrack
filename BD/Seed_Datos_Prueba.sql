-- FixTrack - Datos de prueba + procedimiento almacenado
-- Complemento para reconstruir la base mediante MIGRACIONES EF Core:
--   1) dotnet ef database update   (crea las 7 tablas + __EFMigrationsHistory)
--   2) Este script                (carga datos de prueba y el SP de reportes)
-- Credenciales demo: admin/admin123 - recepcion1/recepcion123 - luis.ortega/tecnico123 - karla.vega/tecnico123 - diego.salinas/tecnico123
-- Equivalente a las secciones de INSERT + CREATE PROCEDURE de FixTrack_BD.sql
-- (sin DROP/CREATE DATABASE: no destruye objetos ajenos que coexistan en la BD).

USE FixTrack;
GO

-- Requerido: el indice unico filtrado UQ_Usuarios_TecnicoID exige QUOTED_IDENTIFIER ON.
SET QUOTED_IDENTIFIER ON;
GO

-- CLIENTES
INSERT INTO Clientes (Nombre, Apellido, Telefono, Email, Direccion, Estado) VALUES
('Maria', 'Gonzalez', '8888-1234', 'maria.gonzalez@correo.com', 'De la Rotonda El Guegue, 2c al lago', 'Activo'),
('Carlos', 'Martinez', '8888-2345', 'carlos.martinez@correo.com', 'Barrio San Sebastian, casa #45', 'Activo'),
('Ana', 'Lopez', '8888-3456', NULL, NULL, 'Activo'),
('Jose', 'Ramirez', '8888-4567', 'jose.ramirez@correo.com', 'Reparto Schick, calle principal', 'Activo'),
('Lucia', 'Hernandez', '8888-5678', 'lucia.hernandez@correo.com', NULL, 'Activo'),
('Pedro', 'Sanchez', '8888-6789', NULL, 'Villa Fontana, 1ra etapa', 'Inactivo'),
('Fernanda', 'Castillo', '8888-7890', 'fernanda.castillo@correo.com', 'Altamira, casa #12', 'Activo'),
('Roberto', 'Mendoza', '8888-8901', NULL, NULL, 'Activo');
GO

-- DISPOSITIVOS
INSERT INTO Dispositivos (ClienteID, Tipo, Marca, Modelo, NumeroSerie, Descripcion) VALUES
(1, 'Laptop', 'HP', 'Pavilion 15', 'HP-SN-001122', 'No enciende'),
(1, 'Impresora', 'Epson', 'L3150', NULL, 'No imprime, atasca papel'),
(2, 'Laptop', 'Dell', 'Inspiron 14', 'DL-SN-334455', 'Pantalla con lineas'),
(3, 'PC de escritorio', 'Ensamblada', NULL, NULL, 'Se apaga sola'),
(4, 'Celular', 'Samsung', 'Galaxy A54', 'SM-SN-667788', 'Pantalla rota'),
(5, 'Laptop', 'Lenovo', 'ThinkPad E14', NULL, 'Muy lenta, posible virus'),
(6, 'Tablet', 'Apple', 'iPad 9na gen', 'AP-SN-998877', 'No carga bateria'),
(7, 'Laptop', 'Acer', 'Aspire 5', 'AC-SN-112233', 'Teclado no responde'),
(8, 'PC de escritorio', 'Ensamblada', NULL, NULL, 'No enciende, posible fuente danada');
GO

-- TECNICOS
INSERT INTO Tecnicos (Nombre, Apellido, Telefono, Especialidad, Estado) VALUES
('Luis', 'Ortega', '8777-1111', 'Hardware y reparacion de equipos', 'Activo'),
('Karla', 'Vega', '8777-2222', 'Software y eliminacion de virus', 'Activo'),
('Diego', 'Salinas', '8777-3333', 'Dispositivos moviles', 'Inactivo');
GO

-- USUARIOS (hash SHA-256 real via HASHBYTES)
INSERT INTO Usuarios (NombreUsuario, PasswordHash, Rol, Estado, TecnicoID) VALUES
('admin', CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', 'admin123'), 2), 'Administrador', 'Activo', NULL),
('recepcion1', CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', 'recepcion123'), 2), 'Empleado', 'Activo', NULL),
('luis.ortega', CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', 'tecnico123'), 2), 'Tecnico', 'Activo', 1),
('karla.vega', CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', 'tecnico123'), 2), 'Tecnico', 'Activo', 2),
('diego.salinas', CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', 'tecnico123'), 2), 'Tecnico', 'Inactivo', 3);
GO

-- ORDENES DE SERVICIO
INSERT INTO OrdenesServicio (DispositivoID, TecnicoID, ProblemaReportado, Estado, CostoServicio)
VALUES (1, NULL, 'El equipo no enciende', 'Pendiente', 0);

INSERT INTO OrdenesServicio (DispositivoID, TecnicoID, ProblemaReportado, Diagnostico, Estado, CostoServicio)
VALUES (2, 2, 'No imprime y atasca el papel', 'Rodillos de arrastre desgastados', 'En diagnostico', 0);

INSERT INTO OrdenesServicio (DispositivoID, TecnicoID, ProblemaReportado, Diagnostico, TrabajoRealizado, Estado, CostoServicio)
VALUES (3, 1, 'Pantalla con lineas verticales', 'Cable flex de video danado', 'Reemplazo de cable flex en proceso', 'En reparacion', 45.00);

INSERT INTO OrdenesServicio (DispositivoID, TecnicoID, ProblemaReportado, Diagnostico, TrabajoRealizado, Estado, CostoServicio, FechaFinalizacion)
VALUES (5, 1, 'Pantalla rota, no responde al tacto', 'Panel tactil y modulo de pantalla danados', 'Reemplazo de modulo de pantalla completo', 'Listo', 85.00, DATEADD(DAY, -1, GETDATE()));

INSERT INTO OrdenesServicio (DispositivoID, TecnicoID, ProblemaReportado, Diagnostico, TrabajoRealizado, Estado, CostoServicio, FechaFinalizacion)
VALUES (4, 1, 'Se apaga sola de manera intermitente', 'Fuente de poder defectuosa', 'Reemplazo de fuente de alimentacion y pruebas de funcionamiento', 'Entregado', 60.00, DATEADD(DAY, -3, GETDATE()));

INSERT INTO OrdenesServicio (DispositivoID, TecnicoID, ProblemaReportado, Diagnostico, TrabajoRealizado, Estado, CostoServicio, FechaFinalizacion)
VALUES (6, 2, 'Equipo muy lento, posible virus', 'Sistema infectado con multiples virus', 'Formateo, reinstalacion de sistema operativo y antivirus', 'Entregado', 100.00, DATEADD(DAY, -5, GETDATE()));

INSERT INTO OrdenesServicio (DispositivoID, TecnicoID, ProblemaReportado, Estado, CostoServicio)
VALUES (7, NULL, 'No carga la bateria', 'Pendiente', 0);

INSERT INTO OrdenesServicio (DispositivoID, TecnicoID, ProblemaReportado, Diagnostico, Estado, CostoServicio)
VALUES (9, 2, 'No enciende, posible fuente danada', 'Fuente de poder quemada', 'En diagnostico', 0);
GO

-- PAGOS
INSERT INTO Pagos (OrdenID, Monto, MetodoPago, Observaciones) VALUES
(4, 85.00, 'Tarjeta', 'Pago total al retirar el equipo'),
(5, 60.00, 'Efectivo', 'Pago total en efectivo'),
(6, 50.00, 'Efectivo', 'Primer abono'),
(6, 50.00, 'Transferencia', 'Segundo abono, saldo cancelado'),
(3, 20.00, 'Tarjeta', 'Adelanto por repuesto');
GO

-- HISTORIAL
INSERT INTO HistorialOrdenes (OrdenID, UsuarioID, TipoCambio, EstadoNuevo, Comentario) VALUES
(1, 1, 'Creacion', 'Pendiente', 'Orden de demostracion creada'),
(2, 1, 'Creacion', 'En diagnostico', 'Orden de demostracion creada'),
(3, 1, 'Creacion', 'En reparacion', 'Orden de demostracion creada'),
(4, 1, 'Creacion', 'Listo', 'Orden de demostracion creada'),
(5, 1, 'Creacion', 'Entregado', 'Orden de demostracion creada'),
(6, 1, 'Creacion', 'Entregado', 'Orden de demostracion creada'),
(7, 1, 'Creacion', 'Pendiente', 'Orden de demostracion creada'),
(8, 1, 'Creacion', 'En diagnostico', 'Orden de demostracion creada'),
(6, 1, 'Pago', NULL, 'Pagos de demostracion registrados');
GO

-- ============================================================
-- PROCEDIMIENTO ALMACENADO (Reporte 1: ordenes por estado)
-- ============================================================
CREATE OR ALTER PROCEDURE sp_ReporteOrdenesPorEstado
    @Desde DATE,
    @Hasta DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        o.Estado,
        COUNT(*) AS Cantidad,
        SUM(o.CostoServicio) AS Subtotal
    FROM OrdenesServicio o
    WHERE o.FechaIngreso >= @Desde
      AND o.FechaIngreso < DATEADD(DAY, 1, @Hasta)
    GROUP BY o.Estado
    ORDER BY CASE o.Estado
        WHEN 'Pendiente' THEN 1
        WHEN 'En diagnostico' THEN 2
        WHEN 'En reparacion' THEN 3
        WHEN 'Listo' THEN 4
        WHEN 'Entregado' THEN 5
        ELSE 6 END;
END
GO
