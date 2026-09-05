using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>
/// Pantalla principal tras el login (mockup 02_dashboard).
/// Contiene el menú lateral por rol, las 5 métricas del dashboard y la tabla de
/// órdenes recientes. El panel de contenido aloja los formularios de cada módulo.
/// </summary>
public partial class FrmDashboard : Form
{
    private readonly Panel panelLateral = new();
    private readonly Panel panelContenido = new();
    private readonly List<Button> botonesMenu = new();
    private Label? lblUsuario;
    private Form? _formularioActual;
    private readonly FrmLogin? _loginRef;

    public FrmDashboard(FrmLogin? loginRef = null)
    {
        _loginRef = loginRef;
        Text = "FixTrack — Panel de control";
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        BackColor = Estilos.Neutro;
        Font = Estilos.Fuente(9);
        CrearEstructura();
        CargarInicio();
    }

    private void CrearEstructura()
    {
        // Barra superior - usando TableLayoutPanel para layout responsivo
        var barra = new Panel
        {
            Dock = DockStyle.Top,
            Height = 56,
            BackColor = Estilos.Primario,
            Padding = new Padding(16, 0, 16, 0)
        };

        var barraLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Padding = Padding.Empty,
            Margin = Padding.Empty
        };
        barraLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Título
        barraLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Espaciador
        barraLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Usuario + botón
        barra.Controls.Add(barraLayout);

        var lblTitulo = new Label
        {
            Text = "FixTrack",
            Font = Estilos.Fuente(16, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Margin = new Padding(0, 14, 0, 0),
            Anchor = AnchorStyles.Left
        };
        barraLayout.Controls.Add(lblTitulo, 0, 0);

        // Panel derecho: usuario + botón cerrar
        var pnlDerecha = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            Margin = new Padding(0, 10, 0, 0),
            Anchor = AnchorStyles.Right
        };
        barraLayout.Controls.Add(pnlDerecha, 2, 0);

        lblUsuario = new Label
        {
            Text = $"Usuario: {Sesion.NombreUsuario} · {RolEtiqueta(Sesion.Rol)}",
            Font = Estilos.Fuente(9),
            ForeColor = Color.White,
            AutoSize = true,
            Margin = new Padding(0, 5, 20, 0),
            TextAlign = ContentAlignment.MiddleRight
        };
        pnlDerecha.Controls.Add(lblUsuario);

        var btnCerrar = new Button
        {
            Text = "Cerrar sesión",
            Size = new Size(120, 34),
            Margin = new Padding(0, 0, 0, 0)
        };
        Estilos.BotonSecundario(btnCerrar);
        btnCerrar.BackColor = Estilos.Terciario;
        btnCerrar.Click += BtnCerrarSesion_Click;
        pnlDerecha.Controls.Add(btnCerrar);

        // Menú lateral
        panelLateral.Dock = DockStyle.Left;
        panelLateral.Width = 200;
        panelLateral.MinimumSize = new Size(180, 0);
        panelLateral.BackColor = Estilos.Terciario;

        // Contenido
        panelContenido.Dock = DockStyle.Fill;
        panelContenido.BackColor = Estilos.Neutro;
        panelContenido.Padding = Padding.Empty;

        // Orden correcto: contenido (Fill) PRIMERO, luego lateral (Left), luego barra (Top).
        // En WinForms el último control agregado queda al frente y se dockeriza primero,
        // por lo que un Dock=Fill agregado al final cubriría el menú lateral y la barra
        // (bug de tablas tapadas que también afectaba al dashboard).
        Controls.Add(panelContenido);
        Controls.Add(panelLateral);
        Controls.Add(barra);

        CrearMenuLateral();
    }

    private void CrearMenuLateral()
    {
        panelLateral.Controls.Clear();
        botonesMenu.Clear();

        const int margenIzq = 12;
        const int margenSup = 12;
        const int altoBoton = 38;
        const int espacio = 8;

        var modulos = ObtenerModulosPorRol();
        var y = margenSup;
        foreach (var (clave, etiqueta) in modulos)
        {
            var ancho = Math.Max(150, panelLateral.ClientSize.Width - margenIzq * 2);
            var btn = new Button
            {
                Text = etiqueta,
                Tag = clave,
                Location = new Point(margenIzq, y),
                Size = new Size(ancho, altoBoton),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = Estilos.Terciario,
                ForeColor = Color.White,
                Font = Estilos.Fuente(9),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                UseVisualStyleBackColor = false
            };
            btn.Click += BtnMenu_Click;
            panelLateral.Controls.Add(btn);
            botonesMenu.Add(btn);
            y += altoBoton + espacio;
        }

        // Ajustar ancho de botones al redimensionar panel lateral
        panelLateral.Resize += (_, _) =>
        {
            var ancho = Math.Max(150, panelLateral.ClientSize.Width - margenIzq * 2);
            foreach (var b in botonesMenu)
            {
                b.Width = ancho;
            }
        };
    }

    private List<(string Clave, string Etiqueta)> ObtenerModulosPorRol()
    {
        var opciones = new List<(string, string)>
        {
            ("inicio", "Inicio"),
            ("clientes", "Clientes"),
            ("dispositivos", "Dispositivos"),
            ("ordenes", "Órdenes"),
            ("pagos", "Pagos"),
            ("tecnicos", "Técnicos"),
            ("usuarios", "Usuarios"),
            ("reportes", "Reportes")
        };

        if (Sesion.EsTecnico)
        {
            return new List<(string, string)>
            {
                ("inicio", "Inicio"),
                ("misOrdenes", "Mis órdenes")
            };
        }

        if (Sesion.EsEmpleado)
        {
            opciones.RemoveAll(o => o.Item1 == "tecnicos" || o.Item1 == "usuarios");
        }

        return opciones;
    }

    private void BtnMenu_Click(object? sender, EventArgs e)
    {
        if (sender is not Button btn) return;
        var clave = btn.Tag?.ToString() ?? string.Empty;
        foreach (var b in botonesMenu)
        {
            b.BackColor = Estilos.Terciario;
        }
        btn.BackColor = Estilos.Primario;
        MostrarModulo(clave);
    }

    private bool TieneAcceso(string modulo)
    {
        return Sesion.EsAdministrador
            || (Sesion.EsEmpleado && new[] { "clientes", "dispositivos", "ordenes", "pagos", "reportes", "inicio" }.Contains(modulo))
            || (Sesion.EsTecnico && new[] { "inicio", "misOrdenes", "actualizarServicio", "reportes" }.Contains(modulo));
    }

    private void MostrarModulo(string clave)
    {
        if (!TieneAcceso(clave))
        {
            MessageBox.Show("No tiene permiso para acceder a este módulo.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        switch (clave)
        {
            case "inicio":
                CargarInicio();
                break;
            case "misOrdenes":
                AbrirFormularioHijo(new FrmOrdenes(soloTecnicoActual: true));
                break;
            case "clientes":
                AbrirFormularioHijo(new FrmClientes());
                break;
            case "dispositivos":
                AbrirFormularioHijo(new FrmDispositivos());
                break;
            case "ordenes":
                AbrirFormularioHijo(new FrmOrdenes());
                break;
            case "pagos":
                AbrirFormularioHijo(new FrmPagos());
                break;
            case "tecnicos":
                AbrirFormularioHijo(new FrmTecnicos());
                break;
            case "usuarios":
                AbrirFormularioHijo(new FrmUsuarios());
                break;
        }
    }

    private void AbrirFormularioHijo(Form hijo)
    {
        panelContenido.Controls.Clear();
        _formularioActual?.Dispose();
        _formularioActual = hijo;
        hijo.TopLevel = false;
        hijo.FormBorderStyle = FormBorderStyle.None;
        hijo.Dock = DockStyle.Fill;
        hijo.BackColor = Estilos.Neutro;
        hijo.Font = Estilos.Fuente(9);
        panelContenido.Controls.Add(hijo);
        hijo.Show();
        hijo.BringToFront();
    }

    private void CargarInicio()
    {
        panelContenido.Controls.Clear();
        _formularioActual?.Dispose();
        _formularioActual = null;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(4, 4, 4, 4),
            Margin = Padding.Empty
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 5F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        panelContenido.Controls.Add(layout);

        var titulo = new Label
        {
            Text = "Panel de control",
            Font = Estilos.Fuente(14, FontStyle.Bold),
            ForeColor = Estilos.Primario,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 16)
        };
        layout.Controls.Add(titulo, 0, 0);

        var metricasPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = new Padding(0, 0, 0, 16)
        };
        layout.Controls.Add(metricasPanel, 0, 1);

        var conteos = new Dictionary<string, int>
        {
            ["Pendiente"] = 0,
            ["En diagnostico"] = 0,
            ["En reparacion"] = 0,
            ["Listo"] = 0,
            ["Entregado"] = 0
        };
        UIHelper.EjecutarSeguro(this, () =>
        {
            conteos = Sesion.EsTecnico && Sesion.TecnicoID.HasValue
                ? OrdenServicioDAL.ObtenerConteoPorEstado(Sesion.TecnicoID.Value)
                : OrdenServicioDAL.ObtenerConteoPorEstado();
        }, "Ordenes");
        var metricas = new (string Etiqueta, int Valor, Color Color)[]
        {
            ("Pendientes", conteos["Pendiente"], Estilos.Pendiente),
            ("En diagnóstico", conteos["En diagnostico"], Estilos.EnDiagnostico),
            ("En reparación", conteos["En reparacion"], Estilos.EnReparacion),
            ("Listos", conteos["Listo"], Estilos.Listo),
            ("Entregados", conteos["Entregado"], Estilos.Entregado)
        };

        foreach (var (etiqueta, valor, color) in metricas)
        {
            var tarjeta = new Panel
            {
                Size = new Size(160, 96),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 12, 0)
            };
            var lblValor = new Label
            {
                Text = valor.ToString(),
                Font = Estilos.Fuente(22, FontStyle.Bold),
                ForeColor = color,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 52
            };
            var lblEtiqueta = new Label
            {
                Text = etiqueta,
                Font = Estilos.Fuente(8.5f),
                ForeColor = Estilos.GrisMedio,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            tarjeta.Controls.Add(lblEtiqueta);
            tarjeta.Controls.Add(lblValor);
            metricasPanel.Controls.Add(tarjeta);
        }

        var lblRecientes = new Label
        {
            Text = "Órdenes recientes",
            Font = Estilos.Fuente(11, FontStyle.Bold),
            ForeColor = Estilos.Terciario,
            AutoSize = true,
            Margin = new Padding(0, 8, 0, 8)
        };
        layout.Controls.Add(lblRecientes, 0, 3);

        var gridPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 0, 0) };
        layout.Controls.Add(gridPanel, 0, 4);

        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            Font = Estilos.Fuente(9)
        };
        grid.Columns.Add(Col("Orden", "OrdenID", 70));
        grid.Columns.Add(Col("Fecha", "FechaIngreso", 100));
        grid.Columns.Add(Col("Cliente", "ClienteNombre", 160));
        grid.Columns.Add(Col("Dispositivo", "DispositivoTexto", 200));
        grid.Columns.Add(Col("Técnico", "TecnicoNombre", 140));
        grid.Columns.Add(Col("Estado", "Estado", 110));
        grid.Columns.Add(Col("Costo", "CostoServicio", 90));
        grid.CellFormatting += Grid_CellFormatting;

        UIHelper.EjecutarSeguro(this, () =>
        {
            var recientes = Sesion.EsTecnico && Sesion.TecnicoID.HasValue
                ? OrdenServicioDAL.ObtenerPorTecnico(Sesion.TecnicoID.Value)
                    .OrderByDescending(o => o.FechaIngreso)
                    .Take(10)
                    .ToList()
                : OrdenServicioDAL.ObtenerTodos()
                    .OrderByDescending(o => o.FechaIngreso)
                    .Take(10)
                    .ToList();
            grid.DataSource = recientes;
        }, "Ordenes");

        gridPanel.Controls.Add(grid);
    }

    private static DataGridViewTextBoxColumn Col(string cabecera, string propiedad, int ancho)
    {
        return new DataGridViewTextBoxColumn
        {
            HeaderText = cabecera,
            DataPropertyName = propiedad,
            Width = ancho
        };
    }

    private static void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (sender is not DataGridView grid || grid.Columns[e.ColumnIndex].DataPropertyName != "Estado")
        {
            return;
        }
        var color = Estilos.ColorDeEstado(e.Value?.ToString() ?? string.Empty);
        e.CellStyle.BackColor = color;
        e.CellStyle.ForeColor = Color.White;
        e.CellStyle.Font = Estilos.Fuente(9, FontStyle.Bold);
    }

    private void MostrarEnConstruccion(string clave)
    {
        panelContenido.Controls.Clear();
        _formularioActual?.Dispose();
        _formularioActual = null;
        var lbl = new Label
        {
            Text = $"Módulo «{clave}» — en construcción.",
            Font = Estilos.Fuente(12),
            ForeColor = Estilos.GrisMedio,
            AutoSize = true,
            Location = new Point(16, 16)
        };
        panelContenido.Controls.Add(lbl);
    }

    private void BtnCerrarSesion_Click(object? sender, EventArgs e)
    {
        Sesion.Limpiar();
        if (_loginRef != null)
        {
            _loginRef.Show();
            _loginRef.BringToFront();
        }
        Close();
    }

    private static string RolEtiqueta(string rol) => rol switch
    {
        "Administrador" => "Administrador",
        "Empleado" => "Empleado (Recepcionista)",
        "Tecnico" => "Técnico",
        _ => rol
    };
}