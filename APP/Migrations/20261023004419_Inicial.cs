using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixTrack.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClienteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClienteID);
                    table.CheckConstraint("CK_Clientes_Estado", "[Estado] IN ('Activo', 'Inactivo')");
                });

            migrationBuilder.CreateTable(
                name: "Tecnicos",
                columns: table => new
                {
                    TecnicoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Especialidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnicos", x => x.TecnicoID);
                    table.CheckConstraint("CK_Tecnicos_Estado", "[Estado] IN ('Activo', 'Inactivo')");
                });

            migrationBuilder.CreateTable(
                name: "Dispositivos",
                columns: table => new
                {
                    DispositivoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteID = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Modelo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NumeroSerie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositivos", x => x.DispositivoID);
                    table.ForeignKey(
                        name: "FK_Dispositivos_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ClienteID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Rol = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    Estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "Activo"),
                    TecnicoID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioID);
                    table.CheckConstraint("CK_Usuarios_Estado", "[Estado] IN ('Activo', 'Inactivo')");
                    table.CheckConstraint("CK_Usuarios_Rol", "[Rol] IN ('Administrador', 'Empleado', 'Tecnico')");
                    table.ForeignKey(
                        name: "FK_Usuarios_Tecnicos_TecnicoID",
                        column: x => x.TecnicoID,
                        principalTable: "Tecnicos",
                        principalColumn: "TecnicoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesServicio",
                columns: table => new
                {
                    OrdenID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispositivoID = table.Column<int>(type: "int", nullable: false),
                    TecnicoID = table.Column<int>(type: "int", nullable: true),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ProblemaReportado = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Diagnostico = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrabajoRealizado = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    CostoServicio = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    FechaFinalizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesServicio", x => x.OrdenID);
                    table.CheckConstraint("CK_OrdenesServicio_Costo", "[CostoServicio] >= 0");
                    table.CheckConstraint("CK_OrdenesServicio_Estado", "[Estado] IN ('Pendiente', 'En diagnostico', 'En reparacion', 'Listo', 'Entregado')");
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Dispositivos_DispositivoID",
                        column: x => x.DispositivoID,
                        principalTable: "Dispositivos",
                        principalColumn: "DispositivoID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Tecnicos_TecnicoID",
                        column: x => x.TecnicoID,
                        principalTable: "Tecnicos",
                        principalColumn: "TecnicoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialOrdenes",
                columns: table => new
                {
                    HistorialID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenID = table.Column<int>(type: "int", nullable: false),
                    UsuarioID = table.Column<int>(type: "int", nullable: true),
                    FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    TipoCambio = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    EstadoAnterior = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    EstadoNuevo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    CampoModificado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    ValorAnterior = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ValorNuevo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialOrdenes", x => x.HistorialID);
                    table.ForeignKey(
                        name: "FK_HistorialOrdenes_OrdenesServicio_OrdenID",
                        column: x => x.OrdenID,
                        principalTable: "OrdenesServicio",
                        principalColumn: "OrdenID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialOrdenes_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    PagoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenID = table.Column<int>(type: "int", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Monto = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    MetodoPago = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.PagoID);
                    table.CheckConstraint("CK_Pagos_MetodoPago", "[MetodoPago] IN ('Efectivo', 'Tarjeta', 'Transferencia')");
                    table.CheckConstraint("CK_Pagos_Monto", "[Monto] > 0");
                    table.ForeignKey(
                        name: "FK_Pagos_OrdenesServicio_OrdenID",
                        column: x => x.OrdenID,
                        principalTable: "OrdenesServicio",
                        principalColumn: "OrdenID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dispositivos_ClienteID",
                table: "Dispositivos",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialOrdenes_OrdenID_FechaCambio",
                table: "HistorialOrdenes",
                columns: new[] { "OrdenID", "FechaCambio" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialOrdenes_UsuarioID",
                table: "HistorialOrdenes",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_DispositivoID",
                table: "OrdenesServicio",
                column: "DispositivoID");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_Estado",
                table: "OrdenesServicio",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_TecnicoID",
                table: "OrdenesServicio",
                column: "TecnicoID");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_OrdenID",
                table: "Pagos",
                column: "OrdenID");

            migrationBuilder.CreateIndex(
                name: "UQ_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Usuarios_TecnicoID",
                table: "Usuarios",
                column: "TecnicoID",
                unique: true,
                filter: "[TecnicoID] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialOrdenes");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "OrdenesServicio");

            migrationBuilder.DropTable(
                name: "Dispositivos");

            migrationBuilder.DropTable(
                name: "Tecnicos");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
