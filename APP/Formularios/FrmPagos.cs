using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>Gestión de pagos (mockup 10_pagos_lista). Listado con filtros por rango de fechas y método de pago.</summary>
public partial class FrmPagos : Form
{
    private readonly DateTimePicker dtDesde = new();
    private readonly DateTimePicker dtHasta = new();
    private readonly ComboBox cboMetodo = new();
    private readonly TextBox txtBuscar = new();
    private readonly Button btnNuevo = new();
    private readonly Button btnDetalle = new();
    private readonly DataGridView grid = new();

    public FrmPagos()
    {
        if (!Sesion.EsAdministrador && !Sesion.EsEmpleado)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        Text = "Pagos";
        InitializeUi();
        CargarDatos();
    }

    private void InitializeUi()
    {
        // 1. Grilla
        grid.Dock = DockStyle.Fill;
        UIHelper.ConfigurarGrilla(grid);
        grid.Columns.Add(UIHelper.Col("ID", "PagoID", 60));
        grid.Columns.Add(UIHelper.Col("Orden", "OrdenID", 70));
        grid.Columns.Add(UIHelper.Col("Fecha", "FechaPago", 110));
        grid.Columns.Add(UIHelper.Col("Monto", "Monto", 90));
        grid.Columns.Add(UIHelper.Col("Metodo", "MetodoPago", 110));
        grid.Columns.Add(UIHelper.Col("Cliente", "ClienteNombre", 180));
        grid.Columns.Add(UIHelper.Col("Observ.", "Observaciones", 180));
        grid.DoubleClick += (_, _) => BtnDetalle_Click(null, EventArgs.Empty);

        // 2. Barra de acciones (dos filas)
        var barra = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = Color.White, Padding = new Padding(12, 6, 12, 6) };
        var barraLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = Padding.Empty,
            Margin = Padding.Empty
        };
        barraLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        barraLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        barra.Controls.Add(barraLayout);

        // Fila 1: Filtros
        var filaFiltros = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = true,
            Margin = new Padding(0, 0, 0, 4)
        };
        barraLayout.Controls.Add(filaFiltros, 0, 0);

        // Desde
        var pnlDesde = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        dtDesde.Format = DateTimePickerFormat.Short;
        dtDesde.Checked = false;
        dtDesde.ShowCheckBox = true;
        dtDesde.Size = new Size(120, 28);
        dtDesde.ValueChanged += (_, _) => CargarDatos();
        var lblDesde = new Label { Text = "Desde:", AutoSize = true, ForeColor = Estilos.Terciario, Margin = new Padding(0, 5, 8, 0), TextAlign = ContentAlignment.MiddleLeft };
        pnlDesde.Controls.Add(lblDesde);
        pnlDesde.Controls.Add(dtDesde);
        filaFiltros.Controls.Add(pnlDesde);

        // Hasta
        var pnlHasta = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        dtHasta.Format = DateTimePickerFormat.Short;
        dtHasta.Checked = false;
        dtHasta.ShowCheckBox = true;
        dtHasta.Size = new Size(120, 28);
        dtHasta.ValueChanged += (_, _) => CargarDatos();
        var lblHasta = new Label { Text = "Hasta:", AutoSize = true, ForeColor = Estilos.Terciario, Margin = new Padding(0, 5, 8, 0), TextAlign = ContentAlignment.MiddleLeft };
        pnlHasta.Controls.Add(lblHasta);
        pnlHasta.Controls.Add(dtHasta);
        filaFiltros.Controls.Add(pnlHasta);

        // Método de pago
        var pnlMetodo = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        cboMetodo.Items.Add("Todos");
        cboMetodo.Items.AddRange(MetodoPagoTexto.Valores);
        cboMetodo.SelectedIndex = 0;
        cboMetodo.Size = new Size(140, 28);
        cboMetodo.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMetodo.SelectedIndexChanged += (_, _) => CargarDatos();
        var lblMetodo = new Label { Text = "Método:", AutoSize = true, ForeColor = Estilos.Terciario, Margin = new Padding(0, 5, 8, 0), TextAlign = ContentAlignment.MiddleLeft };
        pnlMetodo.Controls.Add(lblMetodo);
        pnlMetodo.Controls.Add(cboMetodo);
        filaFiltros.Controls.Add(pnlMetodo);

        // Buscar
        var pnlBuscar = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        txtBuscar.Size = new Size(240, 28);
        txtBuscar.PlaceholderText = "Buscar por cliente, orden, método (número = ID)...";
        txtBuscar.TextChanged += (_, _) => CargarDatos();
        var lblBuscar = new Label { Text = "Buscar:", AutoSize = true, ForeColor = Estilos.Terciario, Margin = new Padding(0, 5, 8, 0), TextAlign = ContentAlignment.MiddleLeft };
        pnlBuscar.Controls.Add(lblBuscar);
        pnlBuscar.Controls.Add(txtBuscar);
        filaFiltros.Controls.Add(pnlBuscar);

        // Fila 2: Botones (derecha)
        var filaBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            Margin = new Padding(0, 4, 0, 0)
        };
        barraLayout.Controls.Add(filaBotones, 0, 1);

        btnDetalle.Text = "Ver detalle";
        btnDetalle.Size = new Size(100, 36);
        btnDetalle.Margin = new Padding(6, 0, 0, 0);
        Estilos.BotonSecundario(btnDetalle);
        btnDetalle.Click += BtnDetalle_Click;
        filaBotones.Controls.Add(btnDetalle);

        btnNuevo.Text = "+ Nuevo pago";
        btnNuevo.Size = new Size(130, 36);
        btnNuevo.Margin = new Padding(6, 0, 6, 0);
        Estilos.BotonPrincipal(btnNuevo);
        btnNuevo.Click += BtnNuevo_Click;
        filaBotones.Controls.Add(btnNuevo);

        // 3. Encabezado
        var header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Estilos.Primario };
        var titulo = new Label { Text = "Pagos", Font = Estilos.Fuente(12, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(16, 17) };
        header.Controls.Add(titulo);

        // Orden correcto: grid (Fill) PRIMERO, luego barra, luego header.
        // En WinForms el último control agregado queda al frente y se dockeriza primero,
        // por lo que un Dock=Fill agregado al final cubriría header y barra (bug de tablas tapadas).
        Controls.Add(grid);
        Controls.Add(barra);
        Controls.Add(header);
    }

    private void CargarDatos()
    {
        UIHelper.EjecutarSeguro(this, () =>
        {
            DateTime? desde = dtDesde.Checked ? dtDesde.Value.Date : null;
            DateTime? hasta = dtHasta.Checked ? dtHasta.Value.Date : null;
            var metodo = cboMetodo.SelectedItem?.ToString();
            grid.DataSource = PagoDAL.Buscar(metodo, desde, hasta, txtBuscar.Text.Trim());
            grid.ClearSelection();
        }, "Pagos");
    }

    private int? PagoSeleccionado()
    {
        if (grid.SelectedRows.Count == 0) return null;
        return Convert.ToInt32(grid.SelectedRows[0].Cells[0].Value);
    }

    private void BtnNuevo_Click(object? sender, EventArgs e)
    {
        using var f = new FrmPagoFormulario();
        if (f.ShowDialog(this) == DialogResult.OK) CargarDatos();
    }

    private void BtnDetalle_Click(object? sender, EventArgs e)
    {
        var id = PagoSeleccionado();
        if (id == null)
        {
            MessageBox.Show(this, "Seleccione un pago.", "Pagos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        using var f = new FrmPagoDetalle(id.Value);
        f.ShowDialog(this);
        CargarDatos();
    }
}