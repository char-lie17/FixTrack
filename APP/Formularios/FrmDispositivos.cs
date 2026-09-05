using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

public partial class FrmDispositivos : Form
{
    private readonly TextBox txtBuscar = new();
    private readonly Button btnNuevo = new();
    private readonly Button btnEditar = new();
    private readonly DataGridView grid = new();

    public FrmDispositivos()
    {
        if (!Sesion.EsAdministrador && !Sesion.EsEmpleado)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        Text = "Dispositivos";
        InitializeUi();
        CargarDatos();
    }

    private void InitializeUi()
    {
        // 1. Grilla
        grid.Dock = DockStyle.Fill;
        UIHelper.ConfigurarGrilla(grid);
        grid.Columns.Add(UIHelper.Col("ID", "DispositivoID", 60));
        grid.Columns.Add(UIHelper.Col("Tipo", "Tipo", 130));
        grid.Columns.Add(UIHelper.Col("Marca", "Marca", 110));
        grid.Columns.Add(UIHelper.Col("Modelo", "Modelo", 120));
        grid.Columns.Add(UIHelper.Col("N. serie", "NumeroSerie", 120));
        grid.Columns.Add(UIHelper.Col("Cliente", "ClienteNombre", 170));
        grid.Columns.Add(UIHelper.Col("Descripcion", "Descripcion", 220));
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

        // Izquierda: Buscar
        var pnlBuscar = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        txtBuscar.Size = new Size(300, 28);
        txtBuscar.PlaceholderText = "Buscar por tipo, marca, modelo o serie (número = ID exacto)...";
        txtBuscar.TextChanged += (_, _) => CargarDatos();
        var lblBuscar = new Label { Text = "Buscar:", AutoSize = true, ForeColor = Estilos.Terciario, Margin = new Padding(0, 5, 8, 0), TextAlign = ContentAlignment.MiddleLeft };
        pnlBuscar.Controls.Add(lblBuscar);
        pnlBuscar.Controls.Add(txtBuscar);
        barraLayout.Controls.Add(pnlBuscar, 0, 0);

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

        btnEditar.Text = "Editar";
        btnEditar.Size = new Size(90, 36);
        btnEditar.Margin = new Padding(6, 0, 0, 0);
        Estilos.BotonSecundario(btnEditar);
        btnEditar.Click += BtnEditar_Click;
        pnlBotones.Controls.Add(btnEditar);

        btnNuevo.Text = "+ Nuevo dispositivo";
        btnNuevo.Size = new Size(150, 36);
        btnNuevo.Margin = new Padding(6, 0, 6, 0);
        Estilos.BotonPrincipal(btnNuevo);
        btnNuevo.Click += BtnNuevo_Click;
        pnlBotones.Controls.Add(btnNuevo);

        // 3. Encabezado
        var header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Estilos.Primario };
        var titulo = new Label { Text = "Dispositivos", Font = Estilos.Fuente(12, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(16, 17) };
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
            grid.DataSource = DispositivoDAL.Buscar(txtBuscar.Text.Trim());
            grid.ClearSelection();
        }, "Dispositivos");
    }

    private void BtnNuevo_Click(object? sender, EventArgs e)
    {
        using var f = new FrmDispositivoFormulario();
        if (f.ShowDialog(this) == DialogResult.OK) CargarDatos();
    }

    private void BtnEditar_Click(object? sender, EventArgs e)
    {
        if (grid.SelectedRows.Count == 0)
        {
            MessageBox.Show(this, "Seleccione un dispositivo.", "Dispositivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var id = Convert.ToInt32(grid.SelectedRows[0].Cells[0].Value);
        var d = DispositivoDAL.ObtenerPorId(id);
        if (d == null)
        {
            MessageBox.Show(this, "El dispositivo ya no existe.", "Dispositivos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        using var f = new FrmDispositivoFormulario(d);
        if (f.ShowDialog(this) == DialogResult.OK) CargarDatos();
    }
}