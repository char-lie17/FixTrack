using FixTrack.Modelos;
using Microsoft.EntityFrameworkCore;

namespace FixTrack.Datos;

/// <summary>
/// Contexto EF Core (Code First) del sistema FixTrack.
/// Mapea el esquema existente definido en BD/FixTrack_BD.sql sin modificar nombres de
/// tablas, columnas ni restricciones. No se habilita carga perezosa (sin proxies y sin
/// navegaciones de carga implícita): las consultas usan joins/proyecciones explícitas.
/// </summary>
public class FixTrackDbContext : DbContext
{
    private readonly string _connectionString;

    public FixTrackDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Requerido por EF Core Design (migraciones). No se usa en tiempo de ejecución.
    public FixTrackDbContext(DbContextOptions<FixTrackDbContext> options) : base(options)
    {
        _connectionString = string.Empty;
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Dispositivo> Dispositivos => Set<Dispositivo>();
    public DbSet<Tecnico> Tecnicos => Set<Tecnico>();
    public DbSet<OrdenServicio> OrdenesServicio => Set<OrdenServicio>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<HistorialOrden> HistorialOrdenes => Set<HistorialOrden>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigurarCliente(modelBuilder);
        ConfigurarDispositivo(modelBuilder);
        ConfigurarTecnico(modelBuilder);
        ConfigurarOrdenServicio(modelBuilder);
        ConfigurarPago(modelBuilder);
        ConfigurarUsuario(modelBuilder);
        ConfigurarHistorialOrden(modelBuilder);
    }

    private static void ConfigurarCliente(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(e =>
        {
            e.ToTable("Clientes", t =>
                t.HasCheckConstraint("CK_Clientes_Estado", "[Estado] IN ('Activo', 'Inactivo')"));
            e.HasKey(c => c.ClienteID);
            e.Property(c => c.ClienteID).ValueGeneratedOnAdd();
            e.Property(c => c.Nombre).HasMaxLength(50).IsRequired();
            e.Property(c => c.Apellido).HasMaxLength(50).IsRequired();
            e.Property(c => c.Telefono).HasMaxLength(20).IsRequired();
            e.Property(c => c.Email).HasMaxLength(100);
            e.Property(c => c.Direccion).HasMaxLength(200);
            e.Property(c => c.FechaRegistro)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");
            e.Property(c => c.Estado)
                .HasMaxLength(20)
                .HasColumnType("varchar(20)")
                .IsRequired()
                .HasDefaultValue("Activo");
            e.Ignore(c => c.NombreCompleto);
        });
    }

    private static void ConfigurarDispositivo(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dispositivo>(e =>
        {
            e.ToTable("Dispositivos");
            e.HasKey(d => d.DispositivoID);
            e.Property(d => d.DispositivoID).ValueGeneratedOnAdd();
            e.Property(d => d.Tipo).HasMaxLength(50).IsRequired();
            e.Property(d => d.Marca).HasMaxLength(50);
            e.Property(d => d.Modelo).HasMaxLength(50);
            e.Property(d => d.NumeroSerie).HasMaxLength(100);
            e.Property(d => d.Descripcion).HasMaxLength(300);
            e.Property(d => d.FechaRegistro)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");
            e.Ignore(d => d.ClienteNombre);
            e.Ignore(d => d.DescripcionCombo);
            e.HasOne(d => d.Cliente)
                .WithMany(c => c.Dispositivos)
                .HasForeignKey(d => d.ClienteID)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(d => d.ClienteID);
        });
    }

    private static void ConfigurarTecnico(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tecnico>(e =>
        {
            e.ToTable("Tecnicos", t =>
                t.HasCheckConstraint("CK_Tecnicos_Estado", "[Estado] IN ('Activo', 'Inactivo')"));
            e.HasKey(t => t.TecnicoID);
            e.Property(t => t.TecnicoID).ValueGeneratedOnAdd();
            e.Property(t => t.Nombre).HasMaxLength(50).IsRequired();
            e.Property(t => t.Apellido).HasMaxLength(50).IsRequired();
            e.Property(t => t.Telefono).HasMaxLength(20);
            e.Property(t => t.Especialidad).HasMaxLength(100);
            e.Property(t => t.Estado)
                .HasMaxLength(20)
                .HasColumnType("varchar(20)")
                .IsRequired()
                .HasDefaultValue("Activo");
            e.Ignore(t => t.NombreCompleto);
        });
    }

    private static void ConfigurarOrdenServicio(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrdenServicio>(e =>
        {
            e.ToTable("OrdenesServicio", t =>
            {
                t.HasCheckConstraint("CK_OrdenesServicio_Estado",
                    "[Estado] IN ('Pendiente', 'En diagnostico', 'En reparacion', 'Listo', 'Entregado')");
                t.HasCheckConstraint("CK_OrdenesServicio_Costo", "[CostoServicio] >= 0");
            });
            e.HasKey(o => o.OrdenID);
            e.Property(o => o.OrdenID).ValueGeneratedOnAdd();
            e.Property(o => o.FechaIngreso)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");
            e.Property(o => o.ProblemaReportado).HasMaxLength(500).IsRequired();
            e.Property(o => o.Diagnostico).HasMaxLength(500);
            e.Property(o => o.TrabajoRealizado).HasMaxLength(500);
            e.Property(o => o.Estado)
                .HasMaxLength(20)
                .HasColumnType("varchar(20)")
                .IsRequired()
                .HasDefaultValue("Pendiente");
            e.Property(o => o.CostoServicio)
                .HasPrecision(10, 2)
                .HasDefaultValue(0m);
            e.Property(o => o.FechaFinalizacion).HasColumnType("datetime2");
            e.Property(o => o.Observaciones).HasMaxLength(500);
            e.Ignore(o => o.ClienteNombre);
            e.Ignore(o => o.DispositivoTexto);
            e.Ignore(o => o.TecnicoNombre);
            e.Ignore(o => o.DescripcionCombo);

            e.HasOne(o => o.Dispositivo)
                .WithMany()
                .HasForeignKey(o => o.DispositivoID)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(o => o.Tecnico)
                .WithMany(t => t.Ordenes)
                .HasForeignKey(o => o.TecnicoID)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(o => o.DispositivoID);
            e.HasIndex(o => o.TecnicoID);
            e.HasIndex(o => o.Estado);
        });
    }

    private static void ConfigurarPago(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pago>(e =>
        {
            e.ToTable("Pagos", t =>
            {
                t.HasCheckConstraint("CK_Pagos_Monto", "[Monto] > 0");
                t.HasCheckConstraint("CK_Pagos_MetodoPago",
                    "[MetodoPago] IN ('Efectivo', 'Tarjeta', 'Transferencia')");
            });
            e.HasKey(p => p.PagoID);
            e.Property(p => p.PagoID).ValueGeneratedOnAdd();
            e.Property(p => p.FechaPago)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");
            e.Property(p => p.Monto).HasPrecision(10, 2);
            e.Property(p => p.MetodoPago)
                .HasMaxLength(20)
                .HasColumnType("varchar(20)")
                .IsRequired();
            e.Property(p => p.Observaciones).HasMaxLength(300);
            e.Ignore(p => p.ClienteNombre);
            e.HasOne(p => p.Orden)
                .WithMany()
                .HasForeignKey(p => p.OrdenID)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(p => p.OrdenID);
        });
    }

    private static void ConfigurarUsuario(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios", t =>
            {
                t.HasCheckConstraint("CK_Usuarios_Rol",
                    "[Rol] IN ('Administrador', 'Empleado', 'Tecnico')");
                t.HasCheckConstraint("CK_Usuarios_Estado", "[Estado] IN ('Activo', 'Inactivo')");
            });
            e.HasKey(u => u.UsuarioID);
            e.Property(u => u.UsuarioID).ValueGeneratedOnAdd();
            e.Property(u => u.NombreUsuario).HasMaxLength(50).IsRequired();
            e.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
            e.Property(u => u.Rol)
                .HasMaxLength(30)
                .HasColumnType("varchar(30)")
                .IsRequired();
            e.Property(u => u.Estado)
                .HasMaxLength(20)
                .HasColumnType("varchar(20)")
                .IsRequired()
                .HasDefaultValue("Activo");
            e.Ignore(u => u.NombreTecnico);

            e.HasOne(u => u.Tecnico)
                .WithMany()
                .HasForeignKey(u => u.TecnicoID)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(u => u.NombreUsuario).IsUnique()
                .HasDatabaseName("UQ_Usuarios_NombreUsuario");
            // Índice único filtrado: garantiza la relación 0..1 : 0..1 entre Usuarios y Tecnicos.
            e.HasIndex(u => u.TecnicoID).IsUnique()
                .HasFilter("[TecnicoID] IS NOT NULL")
                .HasDatabaseName("UQ_Usuarios_TecnicoID");
        });
    }

    private static void ConfigurarHistorialOrden(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HistorialOrden>(e =>
        {
            e.ToTable("HistorialOrdenes");
            e.HasKey(h => h.HistorialID);
            e.Property(h => h.HistorialID).ValueGeneratedOnAdd();
            e.Property(h => h.FechaCambio)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");
            e.Property(h => h.TipoCambio)
                .HasMaxLength(30)
                .HasColumnType("varchar(30)")
                .IsRequired();
            e.Property(h => h.EstadoAnterior).HasMaxLength(20).HasColumnType("varchar(20)");
            e.Property(h => h.EstadoNuevo).HasMaxLength(20).HasColumnType("varchar(20)");
            e.Property(h => h.CampoModificado).HasMaxLength(50).HasColumnType("varchar(50)");
            e.Property(h => h.ValorAnterior).HasMaxLength(1000);
            e.Property(h => h.ValorNuevo).HasMaxLength(1000);
            e.Property(h => h.Comentario).HasMaxLength(500);
            e.Ignore(h => h.UsuarioNombre);

            e.HasOne(h => h.Orden)
                .WithMany()
                .HasForeignKey(h => h.OrdenID)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(h => h.Usuario)
                .WithMany()
                .HasForeignKey(h => h.UsuarioID)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(h => new { h.OrdenID, h.FechaCambio })
                .IsDescending(false, true)
                .HasDatabaseName("IX_HistorialOrdenes_OrdenID_FechaCambio");
        });
    }
}
