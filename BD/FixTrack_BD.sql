-- FixTrack - Script SQL Server ACTUALIZADO (Entregable 2)
-- Esquema identico al script original del Entregable 1.
-- Diferencia: las claves de los usuarios de prueba son hashes SHA-256 reales.
-- Credenciales demo: admin/admin123 - recepcion1/recepcion123 - luis.ortega/tecnico123 - karla.vega/tecnico123 - diego.salinas/tecnico123
-- TecnoFix Solutions

IF DB_ID(N'FixTrack') IS NOT NULL
BEGIN
    ALTER DATABASE FixTrack SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE FixTrack;
END
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE DATABASE FixTrack;
GO

USE FixTrack;
GO

-- CLIENTES
CREATE TABLE Clientes
(
    ClienteID INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(50) NOT NULL,
    Apellido NVARCHAR(50) NOT NULL,
    Telefono NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NULL,
    Direccion NVARCHAR(200) NULL,
    FechaRegistro DATETIME2 NOT NULL CONSTRAINT DF_Clientes_FechaRegistro DEFAULT (GETDATE()),
    Estado VARCHAR(20) NOT NULL CONSTRAINT DF_Clientes_Estado DEFAULT ('Activo'),

    CONSTRAINT PK_Clientes PRIMARY KEY (ClienteID),
    CONSTRAINT CK_Clientes_Estado CHECK (Estado IN ('Activo', 'Inactivo'))
);
GO

-- DISPOSITIVOS
CREATE TABLE Dispositivos
(
    DispositivoID INT IDENTITY(1,1) NOT NULL,
    ClienteID INT NOT NULL,
    Tipo NVARCHAR(50) NOT NULL,
    Marca NVARCHAR(50) NULL,
    Modelo NVARCHAR(50) NULL,
    NumeroSerie NVARCHAR(100) NULL,
    Descripcion NVARCHAR(300) NULL,
    FechaRegistro DATETIME2 NOT NULL CONSTRAINT DF_Dispositivos_FechaRegistro DEFAULT (GETDATE()),

    CONSTRAINT PK_Dispositivos PRIMARY KEY (DispositivoID),
    CONSTRAINT FK_Dispositivos_Clientes FOREIGN KEY (ClienteID)
        REFERENCES Clientes (ClienteID)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

-- TECNICOS
CREATE TABLE Tecnicos
(
    TecnicoID INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(50) NOT NULL,
    Apellido NVARCHAR(50) NOT NULL,
    Telefono NVARCHAR(20) NULL,
    Especialidad NVARCHAR(100) NULL,
    Estado VARCHAR(20) NOT NULL CONSTRAINT DF_Tecnicos_Estado DEFAULT ('Activo'),

    CONSTRAINT PK_Tecnicos PRIMARY KEY (TecnicoID),
    CONSTRAINT CK_Tecnicos_Estado CHECK (Estado IN ('Activo', 'Inactivo'))
);
GO

-- ORDENESSERVICIO
CREATE TABLE OrdenesServicio
(
    OrdenID INT IDENTITY(1,1) NOT NULL,
    DispositivoID INT NOT NULL,
    TecnicoID INT NULL,
    FechaIngreso DATETIME2 NOT NULL CONSTRAINT DF_OrdenesServicio_FechaIngreso DEFAULT (GETDATE()),
    ProblemaReportado NVARCHAR(500) NOT NULL,
    Diagnostico NVARCHAR(500) NULL,
    TrabajoRealizado NVARCHAR(500) NULL,
    Estado VARCHAR(20) NOT NULL CONSTRAINT DF_OrdenesServicio_Estado DEFAULT ('Pendiente'),
    CostoServicio DECIMAL(10,2) NOT NULL CONSTRAINT DF_OrdenesServicio_Costo DEFAULT (0),
    FechaFinalizacion DATETIME2 NULL,
    Observaciones NVARCHAR(500) NULL,

    CONSTRAINT PK_OrdenesServicio PRIMARY KEY (OrdenID),
    CONSTRAINT FK_OrdenesServicio_Dispositivos FOREIGN KEY (DispositivoID)
        REFERENCES Dispositivos (DispositivoID)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT FK_OrdenesServicio_Tecnicos FOREIGN KEY (TecnicoID)
        REFERENCES Tecnicos (TecnicoID)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT CK_OrdenesServicio_Estado CHECK (Estado IN
        ('Pendiente', 'En diagnostico', 'En reparacion', 'Listo', 'Entregado')),
    CONSTRAINT CK_OrdenesServicio_Costo CHECK (CostoServicio >= 0)
);
GO

-- PAGOS
CREATE TABLE Pagos
(
    PagoID INT IDENTITY(1,1) NOT NULL,
    OrdenID INT NOT NULL,
    FechaPago DATETIME2 NOT NULL CONSTRAINT DF_Pagos_FechaPago DEFAULT (GETDATE()),
    Monto DECIMAL(10,2) NOT NULL,
    MetodoPago VARCHAR(20) NOT NULL,
    Observaciones NVARCHAR(300) NULL,

    CONSTRAINT PK_Pagos PRIMARY KEY (PagoID),
    CONSTRAINT FK_Pagos_OrdenesServicio FOREIGN KEY (OrdenID)
        REFERENCES OrdenesServicio (OrdenID)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT CK_Pagos_Monto CHECK (Monto > 0),
    CONSTRAINT CK_Pagos_MetodoPago CHECK (MetodoPago IN ('Efectivo', 'Tarjeta', 'Transferencia'))
);
GO

-- USUARIOS
CREATE TABLE Usuarios
(
    UsuarioID INT IDENTITY(1,1) NOT NULL,
    NombreUsuario NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Rol VARCHAR(30) NOT NULL,
    Estado VARCHAR(20) NOT NULL CONSTRAINT DF_Usuarios_Estado DEFAULT ('Activo'),
    TecnicoID INT NULL,

    CONSTRAINT PK_Usuarios PRIMARY KEY (UsuarioID),
    CONSTRAINT UQ_Usuarios_NombreUsuario UNIQUE (NombreUsuario),
    CONSTRAINT FK_Usuarios_Tecnicos FOREIGN KEY (TecnicoID)
        REFERENCES Tecnicos (TecnicoID)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN ('Administrador', 'Empleado', 'Tecnico')),
    CONSTRAINT CK_Usuarios_Estado CHECK (Estado IN ('Activo', 'Inactivo'))
);
GO

-- Indice unico filtrado: garantiza 0..1 : 0..1 entre Usuarios y Tecnicos
CREATE UNIQUE INDEX UQ_Usuarios_TecnicoID
    ON Usuarios (TecnicoID)
    WHERE TecnicoID IS NOT NULL;
GO

-- HISTORIAL DE CAMBIOS DE ORDENES
CREATE TABLE HistorialOrdenes
(
    HistorialID INT IDENTITY(1,1) NOT NULL,
    OrdenID INT NOT NULL,
    UsuarioID INT NULL,
    FechaCambio DATETIME2 NOT NULL CONSTRAINT DF_HistorialOrdenes_FechaCambio DEFAULT (GETDATE()),
    TipoCambio VARCHAR(30) NOT NULL,
    EstadoAnterior VARCHAR(20) NULL,
    EstadoNuevo VARCHAR(20) NULL,
    CampoModificado VARCHAR(50) NULL,
    ValorAnterior NVARCHAR(1000) NULL,
    ValorNuevo NVARCHAR(1000) NULL,
    Comentario NVARCHAR(500) NULL,

    CONSTRAINT PK_HistorialOrdenes PRIMARY KEY (HistorialID),
    CONSTRAINT FK_HistorialOrdenes_Orden FOREIGN KEY (OrdenID)
        REFERENCES OrdenesServicio (OrdenID),
    CONSTRAINT FK_HistorialOrdenes_Usuario FOREIGN KEY (UsuarioID)
        REFERENCES Usuarios (UsuarioID)
);
GO

CREATE INDEX IX_HistorialOrdenes_OrdenID_FechaCambio
    ON HistorialOrdenes (OrdenID, FechaCambio DESC);
GO

CREATE INDEX IX_Dispositivos_ClienteID
    ON Dispositivos (ClienteID);
GO

CREATE INDEX IX_OrdenesServicio_DispositivoID
    ON OrdenesServicio (DispositivoID);
GO

CREATE INDEX IX_OrdenesServicio_TecnicoID
    ON OrdenesServicio (TecnicoID);
GO

CREATE INDEX IX_OrdenesServicio_Estado
    ON OrdenesServicio (Estado);
GO

CREATE INDEX IX_Pagos_OrdenID
    ON Pagos (OrdenID);
GO

-- Datos de prueba

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

INSERT INTO Tecnicos (Nombre, Apellido, Telefono, Especialidad, Estado) VALUES
('Luis', 'Ortega', '8777-1111', 'Hardware y reparacion de equipos', 'Activo'),
('Karla', 'Vega', '8777-2222', 'Software y eliminacion de virus', 'Activo'),
('Diego', 'Salinas', '8777-3333', 'Dispositivos moviles', 'Inactivo');
GO

INSERT INTO Usuarios (NombreUsuario, PasswordHash, Rol, Estado, TecnicoID) VALUES
('admin', CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', 'admin123'), 2), 'Administrador', 'Activo', NULL),
('recepcion1', CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', 'recepcion123'), 2), 'Empleado', 'Activo', NULL),
('luis.ortega', CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', 'tecnico123'), 2), 'Tecnico', 'Activo', 1),
('karla.vega', CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', 'tecnico123'), 2), 'Tecnico', 'Activo', 2),
('diego.salinas', CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', 'tecnico123'), 2), 'Tecnico', 'Inactivo', 3);
GO

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

INSERT INTO Pagos (OrdenID, Monto, MetodoPago, Observaciones) VALUES
(4, 85.00, 'Tarjeta', 'Pago total al retirar el equipo'),
(5, 60.00, 'Efectivo', 'Pago total en efectivo'),
(6, 50.00, 'Efectivo', 'Primer abono'),
(6, 50.00, 'Transferencia', 'Segundo abono, saldo cancelado'),
(3, 20.00, 'Tarjeta', 'Adelanto por repuesto');
GO

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

-- Consultas

SELECT
    OrdenID,
    Estado,
    FechaIngreso,
    CostoServicio
FROM OrdenesServicio
ORDER BY Estado, FechaIngreso;
GO

SELECT
    os.OrdenID,
    ISNULL(t.Nombre + ' ' + t.Apellido, 'Sin asignar') AS Tecnico,
    d.Tipo + ' ' + ISNULL(d.Marca, '') + ' ' + ISNULL(d.Modelo, '') AS Dispositivo,
    os.Estado
FROM OrdenesServicio os
INNER JOIN Dispositivos d ON d.DispositivoID = os.DispositivoID
LEFT JOIN Tecnicos t ON t.TecnicoID = os.TecnicoID
ORDER BY Tecnico, os.OrdenID;
GO

-- Se incluye Listo y Entregado: ambos ya tienen FechaFinalizacion registrada
SELECT
    OrdenID,
    Estado,
    FechaIngreso,
    FechaFinalizacion,
    CostoServicio
FROM OrdenesServicio
WHERE Estado IN ('Listo', 'Entregado')
ORDER BY FechaFinalizacion DESC;
GO

SELECT
    PagoID,
    OrdenID,
    FechaPago,
    Monto,
    MetodoPago
FROM Pagos
ORDER BY FechaPago DESC;
GO

SELECT DispositivoID, Tipo, Marca, Modelo, NumeroSerie, FechaRegistro
FROM Dispositivos
WHERE ClienteID = 1;
GO

SELECT OrdenID, FechaIngreso, ProblemaReportado, Estado, CostoServicio
FROM OrdenesServicio
WHERE DispositivoID = 1
ORDER BY FechaIngreso DESC;
GO

SELECT PagoID, FechaPago, Monto, MetodoPago
FROM Pagos
WHERE OrdenID = 6
ORDER BY FechaPago;
GO

SELECT OrdenID, ProblemaReportado, Estado, FechaIngreso
FROM OrdenesServicio
WHERE TecnicoID IS NULL;
GO

SELECT OrdenID, ProblemaReportado, FechaIngreso
FROM OrdenesServicio
WHERE Estado = 'Pendiente';
GO

SELECT TecnicoID, Nombre, Apellido, Especialidad
FROM Tecnicos
WHERE Estado = 'Activo';
GO

SELECT
    u.UsuarioID,
    u.NombreUsuario,
    u.Rol,
    t.TecnicoID,
    t.Nombre + ' ' + t.Apellido AS NombreTecnico
FROM Usuarios u
LEFT JOIN Tecnicos t ON t.TecnicoID = u.TecnicoID;
GO

-- ============================================================
-- PROCEDIMIENTOS ALMACENADOS
-- ============================================================

-- Reporte 1: Órdenes por estado en un rango de fechas
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
