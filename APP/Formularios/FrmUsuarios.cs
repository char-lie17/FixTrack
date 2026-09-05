using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>
/// Administración de usuarios (14_usuarios.png). Acceso solo Administrador.
/// Listado con búsqueda, filtro por estado y CRUD completo.
/// </summary>
public partial class FrmUsuarios : Form
{
    private readonly TextBox txtBuscar = new();
    private readonly ComboBox cboEstado = new();
    private readonly Button btnNuevo = new();
    private readonly Button btnEditar = new();
    private readonly Button btnEstado = new();
    private readonly DataGridView grid = new();

    public FrmUsuarios()
    {
        if (!Sesion.EsAdministrador)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        Text = "Usuarios";
        InitializeUi();
        CargarDatos();
    }

    private void InitializeUi()
    {
        // 1. Grilla
        grid.Dock = DockStyle.Fill;
        UIHelper.ConfigurarGrilla(grid);
        grid.Columns.Add(UIHelper.Col("ID", "UsuarioID", 60));
        grid.Columns.Add(UIHelper.Col("Usuario", "NombreUsuario", 150));
        grid.Columns.Add(UIHelper.Col("Rol", "Rol", 120));
        grid.Columns.Add(UIHelper.Col("Estado", "Estado", 90));
        grid.Columns.Add(UIHelper.Col("Técnico asociado", "NombreTecnico", 200));
        grid.DoubleClick += (_, _) => BtnEditar_Click(null, EventArgs.Empty);

        // 2. Barra de acciones
        var barra = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, Padding = new Padding(12, 6, 12, 6) };
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

        // Izquierda: Buscar + Filtro
        var pnlBuscar = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        txtBuscar.Size = new Size(260, 28);
        txtBuscar.PlaceholderText = "Buscar por nombre de usuario o rol…";
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

        // Espaciador
        var spacer = new Panel { Size = new Size(20, 1) };
        barraLayout.Controls.Add(spacer);

        // Derecha: Botones
        var pnlBotones = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            Margin = new Padding(0, 4, 0, 4)
        };
        barraLayout.Controls.Add(pnlBotones);

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

        btnNuevo.Text = "+ Nuevo usuario";
        btnNuevo.Size = new Size(130, 36);
        btnNuevo.Margin = new Padding(6, 0, 6, 0);
        Estilos.BotonPrincipal(btnNuevo);
        btnNuevo.Click += BtnNuevo_Click;
        pnlBotones.Controls.Add(btnNuevo);

        // 3. Encabezado
        var header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Estilos.Primario };
        var titulo = new Label { Text = "Usuarios", Font = Estilos.Fuente(12, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(16, 17) };
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
            grid.DataSource = UsuarioDAL.Buscar(txtBuscar.Text.Trim(), cboEstado.SelectedItem?.ToString());
            grid.ClearSelection();
        }, "Usuarios");
    }

    private Usuario? UsuarioSeleccionado()
    {
        if (grid.SelectedRows.Count == 0) return null;
        var id = Convert.ToInt32(grid.SelectedRows[0].Cells[0].Value);
        return UsuarioDAL.ObtenerPorId(id);
    }

    private void BtnNuevo_Click(object? sender, EventArgs e)
    {
        using var f = new FrmUsuarioFormulario();
        if (f.ShowDialog(this) == DialogResult.OK) CargarDatos();
    }

    private void BtnEditar_Click(object? sender, EventArgs e)
    {
        var u = UsuarioSeleccionado();
        if (u == null)
        {
            MessageBox.Show(this, "Seleccione un usuario.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        using var f = new FrmUsuarioFormulario(u);
        if (f.ShowDialog(this) == DialogResult.OK) CargarDatos();
    }

    private void BtnEstado_Click(object? sender, EventArgs e)
    {
        var u = UsuarioSeleccionado();
        if (u == null)
        {
            MessageBox.Show(this, "Seleccione un usuario.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var nuevoEstado = u.Estado == "Activo" ? "Inactivo" : "Activo";
        if (MessageBox.Show(this, $"¿Cambiar el estado del usuario «{u.NombreUsuario}» a «{nuevoEstado}»?", "Cambiar estado",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }
        var ok = UIHelper.EjecutarSeguro(this, () =>
        {
            if (UsuarioDAL.CambiarEstado(u.UsuarioID))
            {
                CargarDatos();
            }
        }, "Usuarios");
    }
}