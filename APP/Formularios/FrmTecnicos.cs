using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>
/// Gestión de técnicos (11_tecnicos.png). Acceso solo Administrador.
/// Listado con búsqueda, filtro por estado y CRUD completo.
/// </summary>
public partial class FrmTecnicos : Form
{
    private readonly TextBox txtBuscar = new();
    private readonly ComboBox cboEstado = new();
    private readonly Button btnNuevo = new();
    private readonly Button btnEditar = new();
    private readonly Button btnEstado = new();
    private readonly DataGridView grid = new();

    public FrmTecnicos()
    {
        if (!Sesion.EsAdministrador)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        Text = "Técnicos";
        InitializeUi();
        CargarDatos();
    }

    private void InitializeUi()
    {
        // 1. Grilla
        grid.Dock = DockStyle.Fill;
        UIHelper.ConfigurarGrilla(grid);
        grid.Columns.Add(UIHelper.Col("ID", "TecnicoID", 60));
        grid.Columns.Add(UIHelper.Col("Nombre", "Nombre", 120));
        grid.Columns.Add(UIHelper.Col("Apellido", "Apellido", 120));
        grid.Columns.Add(UIHelper.Col("Teléfono", "Telefono", 110));
        grid.Columns.Add(UIHelper.Col("Especialidad", "Especialidad", 150));
        grid.Columns.Add(UIHelper.Col("Estado", "Estado", 90));
        grid.CellFormatting += Grid_CellFormatting;
        grid.DoubleClick += (_, _) => BtnEditar_Click(null, EventArgs.Empty);

        // 2. Barra de acciones
        var barra = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, Padding = new Padding(12, 6, 12, 6) };
        var barraLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = Padding.Empty,
            Margin = Padding.Empty
        };
        barraLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        barraLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        barra.Controls.Add(barraLayout);

        // Izquierda: Buscar + Filtro
        var pnlFiltros = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            Margin = Padding.Empty
        };
        
        var pnlBuscar = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        txtBuscar.Size = new Size(260, 28);
        txtBuscar.PlaceholderText = "Buscar por nombre, apellido, teléfono o especialidad (número = ID exacto)…";
        txtBuscar.TextChanged += (_, _) => CargarDatos();
        var lblBuscar = new Label { Text = "Buscar:", AutoSize = true, ForeColor = Estilos.Terciario, Margin = new Padding(0, 5, 8, 0), TextAlign = ContentAlignment.MiddleLeft };
        pnlBuscar.Controls.Add(lblBuscar);
        pnlBuscar.Controls.Add(txtBuscar);
        pnlFiltros.Controls.Add(pnlBuscar);

        var pnlEstado = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        cboEstado.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
        cboEstado.SelectedIndex = 0;
        cboEstado.Size = new Size(130, 28);
        cboEstado.SelectedIndexChanged += (_, _) => CargarDatos();
        cboEstado.DropDownStyle = ComboBoxStyle.DropDownList;
        var lblEstado = new Label { Text = "Estado:", AutoSize = true, ForeColor = Estilos.Terciario, Margin = new Padding(0, 5, 8, 0), TextAlign = ContentAlignment.MiddleLeft };
        pnlEstado.Controls.Add(lblEstado);
        pnlEstado.Controls.Add(cboEstado);
        pnlFiltros.Controls.Add(pnlEstado);
        
        barraLayout.Controls.Add(pnlFiltros, 0, 0);

        // Derecha: Botones anclados a la derecha
        var pnlBotones = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            Margin = new Padding(0, 4, 0, 4)
        };
        barraLayout.Controls.Add(pnlBotones, 1, 0);

        btnEstado.Text = "Cambiar estado";
        btnEstado.Size = new Size(130, 36);
        btnEstado.Margin = new Padding(6, 0, 0, 0);
        Estilos.BotonSecundario(btnEstado);
        btnEstado.Click += BtnEstado_Click;
        pnlBotones.Controls.Add(btnEstado);

        btnEditar.Text = "Editar";
        btnEditar.Size = new Size(90, 36);
        btnEditar.Margin = new Padding(6, 0, 6, 0);
        Estilos.BotonSecundario(btnEditar);
        btnEditar.Click += BtnEditar_Click;
        pnlBotones.Controls.Add(btnEditar);

        btnNuevo.Text = "+ Nuevo técnico";
        btnNuevo.Size = new Size(130, 36);
        btnNuevo.Margin = new Padding(6, 0, 6, 0);
        Estilos.BotonPrincipal(btnNuevo);
        btnNuevo.Click += BtnNuevo_Click;
        pnlBotones.Controls.Add(btnNuevo);

        // 3. Encabezado
        var header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Estilos.Primario };
        var titulo = new Label { Text = "Técnicos", Font = Estilos.Fuente(12, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(16, 17) };
        header.Controls.Add(titulo);

        // Orden correcto: grid (Fill) PRIMERO, luego barra, luego header.
        // En WinForms el último control agregado queda al frente y se dockeriza primero,
        // por lo que un Dock=Fill agregado al final cubriría header y barra (bug de tablas tapadas).
        Controls.Add(grid);
        Controls.Add(barra);
        Controls.Add(header);
    }

    private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
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

    private void CargarDatos()
    {
        UIHelper.EjecutarSeguro(this, () =>
        {
            grid.DataSource = TecnicoDAL.Buscar(txtBuscar.Text.Trim(), cboEstado.SelectedItem?.ToString());
            grid.ClearSelection();
        }, "Técnicos");
    }

    private Tecnico? TecnicoSeleccionado()
    {
        if (grid.SelectedRows.Count == 0) return null;
        var id = Convert.ToInt32(grid.SelectedRows[0].Cells[0].Value);
        return TecnicoDAL.ObtenerPorId(id);
    }

    private void BtnNuevo_Click(object? sender, EventArgs e)
    {
        using var f = new FrmTecnicoFormulario();
        if (f.ShowDialog(this) == DialogResult.OK) CargarDatos();
    }

    private void BtnEditar_Click(object? sender, EventArgs e)
    {
        var t = TecnicoSeleccionado();
        if (t == null)
        {
            MessageBox.Show(this, "Seleccione un técnico.", "Técnicos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        using var f = new FrmTecnicoFormulario(t);
        if (f.ShowDialog(this) == DialogResult.OK) CargarDatos();
    }

    private void BtnEstado_Click(object? sender, EventArgs e)
    {
        var t = TecnicoSeleccionado();
        if (t == null)
        {
            MessageBox.Show(this, "Seleccione un técnico.", "Técnicos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var nuevoEstado = t.Estado == "Activo" ? "Inactivo" : "Activo";
        if (MessageBox.Show(this, $"¿Cambiar el estado del técnico «{t.Nombre} {t.Apellido}» a «{nuevoEstado}»?", "Cambiar estado",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }
        var ok = UIHelper.EjecutarSeguro(this, () =>
        {
            if (TecnicoDAL.CambiarEstado(t.TecnicoID))
            {
                CargarDatos();
            }
        }, "Técnicos");
    }
}