using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>
/// Gestión de clientes (mockups 03, 04 y 05).
/// Listado con búsqueda, filtro por estado y CRUD completo.
/// </summary>
public partial class FrmClientes : Form
{
    private readonly TextBox txtBuscar = new();
    private readonly ComboBox cboEstado = new();
    private readonly Button btnNuevo = new();
    private readonly Button btnEditar = new();
    private readonly Button btnEstado = new();
    private readonly Button btnDetalle = new();
    private readonly DataGridView grid = new();

    public FrmClientes()
    {
        if (!Sesion.EsAdministrador && !Sesion.EsEmpleado)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        Text = "Clientes";
        InitializeUi();
        CargarDatos();
    }

    private void InitializeUi()
    {
        // 1. Grilla (Dock=Fill, se agrega primero para z-order correcto)
        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AutoGenerateColumns = false;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.BackgroundColor = Color.White;
        grid.Columns.Add(Col("ID", "ClienteID", 60));
        grid.Columns.Add(Col("Nombre", "Nombre", 120));
        grid.Columns.Add(Col("Apellido", "Apellido", 120));
        grid.Columns.Add(Col("Teléfono", "Telefono", 110));
        grid.Columns.Add(Col("Email", "Email", 180));
        grid.Columns.Add(Col("Dirección", "Direccion", 220));
        grid.Columns.Add(Col("Estado", "Estado", 90));
        grid.DoubleClick += (_, _) => BtnDetalle_Click(null, EventArgs.Empty);

        // 2. Barra de acciones (Dock=Top) - usando FlowLayoutPanel simple
        var barra = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, Padding = new Padding(12, 6, 12, 6) };
        
        // Contenedor principal con FlowLayoutPanel (izquierda a derecha, con wrap)
        var barraLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = false,
            WrapContents = false,
            Padding = Padding.Empty,
            Margin = Padding.Empty
        };
        barra.Controls.Add(barraLayout);

        // --- LADO IZQUIERDO: Búsqueda y filtros ---
        var pnlBuscar = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        txtBuscar.Size = new Size(260, 28);
        txtBuscar.PlaceholderText = "Buscar por nombre, apellido o teléfono…";
        txtBuscar.TextChanged += (_, _) => CargarDatos();
        var lblBuscar = new Label { Text = "Buscar:", AutoSize = true, ForeColor = Estilos.Terciario, Margin = new Padding(0, 5, 8, 0), TextAlign = ContentAlignment.MiddleLeft };
        pnlBuscar.Controls.Add(lblBuscar);
        pnlBuscar.Controls.Add(txtBuscar);
        barraLayout.Controls.Add(pnlBuscar);

        var pnlEstado = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        cboEstado.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
        cboEstado.SelectedIndex = 0;
        cboEstado.Size = new Size(130, 28);
        cboEstado.SelectedIndexChanged += (_, _) => CargarDatos();
        cboEstado.DropDownStyle = ComboBoxStyle.DropDownList;
        var lblEstado = new Label { Text = "Estado:", AutoSize = true, ForeColor = Estilos.Terciario, Margin = new Padding(0, 5, 8, 0), TextAlign = ContentAlignment.MiddleLeft };
        pnlEstado.Controls.Add(lblEstado);
        pnlEstado.Controls.Add(cboEstado);
        barraLayout.Controls.Add(pnlEstado);

        // --- ESPACIADOR FLEXIBLE (empuja botones a la derecha) ---
        var spacer = new Panel { Size = new Size(20, 1) };
        barraLayout.Controls.Add(spacer);

        // --- LADO DERECHO: Botones (usamos otro FlowLayoutPanel con RightToLeft) ---
        var pnlBotones = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            Margin = new Padding(0, 4, 0, 4)
        };
        barraLayout.Controls.Add(pnlBotones);

        btnDetalle.Text = "Ver detalle";
        btnDetalle.Size = new Size(100, 36);
        btnDetalle.Margin = new Padding(6, 0, 0, 0);
        Estilos.BotonSecundario(btnDetalle);
        btnDetalle.Click += BtnDetalle_Click;
        pnlBotones.Controls.Add(btnDetalle);

        btnEstado.Text = "Cambiar estado";
        btnEstado.Size = new Size(130, 36);
        btnEstado.Margin = new Padding(6, 0, 6, 0);
        Estilos.BotonSecundario(btnEstado);
        btnEstado.Click += BtnEstado_Click;
        pnlBotones.Controls.Add(btnEstado);

        btnEditar.Text = "Editar";
        btnEditar.Size = new Size(90, 36);
        btnEditar.Margin = new Padding(6, 0, 6, 0);
        Estilos.BotonSecundario(btnEditar);
        btnEditar.Click += BtnEditar_Click;
        pnlBotones.Controls.Add(btnEditar);

        btnNuevo.Text = "+ Nuevo cliente";
        btnNuevo.Size = new Size(130, 36);
        btnNuevo.Margin = new Padding(6, 0, 0, 0);
        Estilos.BotonPrincipal(btnNuevo);
        btnNuevo.Click += BtnNuevo_Click;
        pnlBotones.Controls.Add(btnNuevo);

        // 3. Encabezado (Dock=Top)
        var header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Estilos.Primario };
        var titulo = new Label { Text = "Clientes", Font = Estilos.Fuente(12, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(16, 17) };
        header.Controls.Add(titulo);

        // Orden correcto: header (arriba del todo) -> barra -> grid (Fill, al final)
        Controls.Add(header);
        Controls.Add(barra);
        Controls.Add(grid);
    }

    private void CargarDatos()
    {
        UIHelper.EjecutarSeguro(this, () =>
        {
            grid.DataSource = ClienteDAL.Buscar(txtBuscar.Text.Trim(), cboEstado.SelectedItem?.ToString());
            grid.ClearSelection();
        }, "Clientes");
    }

    private Cliente? ClienteSeleccionado()
    {
        if (grid.SelectedRows.Count == 0) return null;
        var id = Convert.ToInt32(grid.SelectedRows[0].Cells[0].Value);
        return ClienteDAL.ObtenerPorId(id);
    }

    private void BtnNuevo_Click(object? sender, EventArgs e)
    {
        using var f = new FrmClienteFormulario();
        if (f.ShowDialog(this) == DialogResult.OK) CargarDatos();
    }

    private void BtnEditar_Click(object? sender, EventArgs e)
    {
        var c = ClienteSeleccionado();
        if (c == null)
        {
            MessageBox.Show(this, "Seleccione un cliente.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        using var f = new FrmClienteFormulario(c);
        if (f.ShowDialog(this) == DialogResult.OK) CargarDatos();
    }

    private void BtnEstado_Click(object? sender, EventArgs e)
    {
        var c = ClienteSeleccionado();
        if (c == null)
        {
            MessageBox.Show(this, "Seleccione un cliente.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var nuevoEstado = c.Estado == "Activo" ? "Inactivo" : "Activo";
        if (MessageBox.Show(this, $"¿Cambiar el estado del cliente «{c.Nombre} {c.Apellido}» a «{nuevoEstado}»?", "Cambiar estado",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }
        var ok = UIHelper.EjecutarSeguro(this, () =>
        {
            if (ClienteDAL.CambiarEstado(c.ClienteID))
            {
                CargarDatos();
            }
        }, "Clientes");
    }

    private void BtnDetalle_Click(object? sender, EventArgs e)
    {
        var c = ClienteSeleccionado();
        if (c == null)
        {
            MessageBox.Show(this, "Seleccione un cliente.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        using var f = new FrmClienteDetalle(c.ClienteID);
        f.ShowDialog(this);
    }

    private static DataGridViewTextBoxColumn Col(string cabecera, string propiedad, int ancho)
    {
        return new DataGridViewTextBoxColumn { HeaderText = cabecera, DataPropertyName = propiedad, Width = ancho };
    }
}