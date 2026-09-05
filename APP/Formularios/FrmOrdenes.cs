using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

public partial class FrmOrdenes : Form
{
    private readonly bool _soloTecnicoActual;
    private readonly TextBox txtBuscar = new();
    private readonly ComboBox cboEstado = new();
    private readonly DateTimePicker dtDesde = new();
    private readonly DateTimePicker dtHasta = new();
    private readonly Button btnNueva = new();
    private readonly Button btnVer = new();
    private readonly DataGridView grid = new();

    public FrmOrdenes(bool soloTecnicoActual = false)
    {
        if (!Sesion.EsAdministrador && !Sesion.EsEmpleado && !Sesion.EsTecnico)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        _soloTecnicoActual = soloTecnicoActual;
        Text = soloTecnicoActual ? "Mis ordenes" : "Ordenes de servicio";
        InitializeUi();
        CargarDatos();
    }

    private void InitializeUi()
    {
        // 1. Grilla
        grid.Dock = DockStyle.Fill;
        UIHelper.ConfigurarGrilla(grid);
        grid.Columns.Add(UIHelper.Col("No", "OrdenID", 60));
        grid.Columns.Add(UIHelper.Col("Fecha ingreso", "FechaIngreso", 110));
        grid.Columns.Add(UIHelper.Col("Cliente", "ClienteNombre", 160));
        grid.Columns.Add(UIHelper.Col("Dispositivo", "DispositivoTexto", 210));
        grid.Columns.Add(UIHelper.Col("Tecnico", "TecnicoNombre", 140));
        grid.Columns.Add(UIHelper.Col("Estado", "Estado", 120));
        grid.Columns.Add(UIHelper.Col("Costo", "CostoServicio", 90));
        grid.CellFormatting += Grid_CellFormatting;
        grid.DoubleClick += (_, _) => BtnVer_Click(null, EventArgs.Empty);

        // 2. Barra de acciones (dos filas en TableLayoutPanel)
        var barra = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = Color.White, Padding = new Padding(12, 6, 12, 6) };
        var barraLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = Padding.Empty,
            Margin = Padding.Empty
        };
        barraLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Filtros
        barraLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Botones
        barra.Controls.Add(barraLayout);

        // Fila 1: Filtros (FlowLayoutPanel con wrap)
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

        // Buscar
        var pnlBuscar = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        txtBuscar.Size = new Size(220, 28);
        txtBuscar.PlaceholderText = "Buscar orden, cliente, dispositivo (número = ID)...";
        txtBuscar.TextChanged += (_, _) => CargarDatos();
        var lblBuscar = new Label { Text = "Buscar:", AutoSize = true, ForeColor = Estilos.Terciario, Margin = new Padding(0, 5, 8, 0), TextAlign = ContentAlignment.MiddleLeft };
        pnlBuscar.Controls.Add(lblBuscar);
        pnlBuscar.Controls.Add(txtBuscar);
        filaFiltros.Controls.Add(pnlBuscar);

        // Estado
        var pnlEstado = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        cboEstado.Items.AddRange(EstadoTodos().Cast<object>().ToArray());
        cboEstado.SelectedIndex = 0;
        cboEstado.Size = new Size(140, 28);
        cboEstado.SelectedIndexChanged += (_, _) => CargarDatos();
        cboEstado.DropDownStyle = ComboBoxStyle.DropDownList;
        var lblEstado = new Label { Text = "Estado:", AutoSize = true, ForeColor = Estilos.Terciario, Margin = new Padding(0, 5, 8, 0), TextAlign = ContentAlignment.MiddleLeft };
        pnlEstado.Controls.Add(lblEstado);
        pnlEstado.Controls.Add(cboEstado);
        filaFiltros.Controls.Add(pnlEstado);

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

        // Fila 2: Botones (derecha con RightToLeft)
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

        btnVer.Text = "Ver detalle";
        btnVer.Size = new Size(110, 36);
        btnVer.Margin = new Padding(6, 0, 0, 0);
        Estilos.BotonSecundario(btnVer);
        btnVer.Click += BtnVer_Click;
        filaBotones.Controls.Add(btnVer);

        if (!_soloTecnicoActual)
        {
            btnNueva.Text = "+ Nueva orden";
            btnNueva.Size = new Size(130, 36);
            btnNueva.Margin = new Padding(6, 0, 6, 0);
            Estilos.BotonPrincipal(btnNueva);
            btnNueva.Click += BtnNueva_Click;
            filaBotones.Controls.Add(btnNueva);
        }

        // 3. Encabezado
        var header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Estilos.Primario };
        var titulo = new Label { Text = Text, Font = Estilos.Fuente(12, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(16, 17) };
        header.Controls.Add(titulo);

        // Orden correcto: grid (Fill) PRIMERO, luego barra, luego header.
        // En WinForms el último control agregado queda al frente y se dockeriza primero,
        // por lo que un Dock=Fill agregado al final cubriría header y barra (bug de tablas tapadas).
        Controls.Add(grid);
        Controls.Add(barra);
        Controls.Add(header);
    }

    private static EstadoItem[] EstadoTodos() =>
        new[] { new EstadoItem("Todos", "Todos") }.Concat(EstadoOrdenTexto.ItemsParaCombo).ToArray();

    private void CargarDatos()
    {
        UIHelper.EjecutarSeguro(this, () =>
        {
            DateTime? desde = dtDesde.Checked ? dtDesde.Value.Date : null;
            DateTime? hasta = dtHasta.Checked ? dtHasta.Value.Date : null;
            var estado = (cboEstado.SelectedItem as EstadoItem)?.Valor;

            if (_soloTecnicoActual && Sesion.TecnicoID.HasValue)
            {
                grid.DataSource = OrdenServicioDAL.Buscar(txtBuscar.Text.Trim(), estado, desde, hasta, Sesion.TecnicoID.Value);
            }
            else
            {
                grid.DataSource = OrdenServicioDAL.Buscar(txtBuscar.Text.Trim(), estado, desde, hasta);
            }
            grid.ClearSelection();
        }, "Ordenes");
    }

    private void BtnNueva_Click(object? sender, EventArgs e)
    {
        using var f = new FrmOrdenNueva();
        if (f.ShowDialog(this) == DialogResult.OK) CargarDatos();
    }

    private void BtnVer_Click(object? sender, EventArgs e)
    {
        if (grid.SelectedRows.Count == 0)
        {
            MessageBox.Show(this, "Seleccione una orden.", "Ordenes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var id = Convert.ToInt32(grid.SelectedRows[0].Cells[0].Value);
        using var f = new FrmOrdenDetalle(id);
        f.ShowDialog(this);
        CargarDatos();
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
}