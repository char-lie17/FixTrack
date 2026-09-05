-- Migración idempotente para instalaciones existentes de FixTrack.
-- Ejecutar sobre la base de datos FixTrack antes de usar la nueva versión.
USE FixTrack;
GO

IF OBJECT_ID(N'dbo.HistorialOrdenes', N'U') IS NULL
BEGIN
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
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_HistorialOrdenes_OrdenID_FechaCambio'
      AND object_id = OBJECT_ID(N'dbo.HistorialOrdenes')
)
BEGIN
    CREATE INDEX IX_HistorialOrdenes_OrdenID_FechaCambio
        ON HistorialOrdenes (OrdenID, FechaCambio DESC);
END;
GO
