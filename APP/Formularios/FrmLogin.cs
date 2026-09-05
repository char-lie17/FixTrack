using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>
/// Pantalla de inicio: autentica al usuario contra la tabla Usuarios
/// (mockup 01_login). Al validar las credenciales establece Sesion y abre el Dashboard.
/// </summary>
public partial class FrmLogin : Form
{
    private readonly TextBox txtUsuario = new();
    private readonly TextBox txtPassword = new();
    private readonly Label lblMensaje = new();

    public FrmLogin()
    {
        Text = "FixTrack — Iniciar sesión";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(420, 520);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = Color.White;
        Font = Estilos.Fuente(9);
        KeyPreview = true;
        KeyDown += FrmLogin_KeyDown;
        InitializeControls();
    }

    private void InitializeControls()
    {
        // Panel principal con TableLayoutPanel para layout responsivo
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(48, 40, 48, 24),
            ColumnCount = 1,
            RowCount = 9,
            AutoSize = true
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Logo
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Subtítulo
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Espacio
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Usuario label
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Usuario textbox
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Password label
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Password textbox
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Botón
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Mensaje + pie (expandible)
        Controls.Add(layout);

        // Logo
        var lblLogo = new Label
        {
            Text = "FixTrack",
            Font = Estilos.Fuente(26, FontStyle.Bold),
            ForeColor = Estilos.Primario,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 4),
            Anchor = AnchorStyles.None
        };
        layout.Controls.Add(lblLogo, 0, 0);

        // Subtítulo
        var lblSub = new Label
        {
            Text = "Gestión de reparaciones",
            Font = Estilos.Fuente(10),
            ForeColor = Estilos.GrisMedio,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 20),
            Anchor = AnchorStyles.None
        };
        layout.Controls.Add(lblSub, 0, 1);

        // Usuario label
        var lblUsuario = new Label
        {
            Text = "Usuario",
            AutoSize = true,
            ForeColor = Estilos.Terciario,
            Margin = new Padding(0, 8, 0, 4)
        };
        layout.Controls.Add(lblUsuario, 0, 3);

        // Usuario textbox
        txtUsuario.Size = new Size(324, 28);
        txtUsuario.Font = Estilos.Fuente(10);
        txtUsuario.Name = "txtUsuario";
        txtUsuario.Margin = new Padding(0, 0, 0, 12);
        txtUsuario.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        layout.Controls.Add(txtUsuario, 0, 4);

        // Contraseña label
        var lblPassword = new Label
        {
            Text = "Contraseña",
            AutoSize = true,
            ForeColor = Estilos.Terciario,
            Margin = new Padding(0, 8, 0, 4)
        };
        layout.Controls.Add(lblPassword, 0, 5);

        // Contraseña textbox
        txtPassword.Size = new Size(324, 28);
        txtPassword.Font = Estilos.Fuente(10);
        txtPassword.UseSystemPasswordChar = true;
        txtPassword.Name = "txtPassword";
        txtPassword.Margin = new Padding(0, 0, 0, 20);
        txtPassword.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        layout.Controls.Add(txtPassword, 0, 6);

        // Botón Iniciar sesión
        var btnIngresar = new Button
        {
            Text = "Iniciar sesión",
            Name = "btnIngresar",
            Size = new Size(324, 38),
            Margin = new Padding(0, 0, 0, 16),
            Anchor = AnchorStyles.None
        };
        Estilos.BotonPrincipal(btnIngresar);
        btnIngresar.Click += BtnIngresar_Click;
        layout.Controls.Add(btnIngresar, 0, 7);

        // Panel inferior para mensaje y pie (se expande)
        var bottomPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 8, 0, 0)
        };
        layout.Controls.Add(bottomPanel, 0, 8);

        // Mensaje de error
        lblMensaje.Text = string.Empty;
        lblMensaje.ForeColor = Estilos.Pendiente;
        lblMensaje.Font = Estilos.Fuente(8.5f);
        lblMensaje.AutoSize = false;
        lblMensaje.Size = new Size(324, 40);
        lblMensaje.Location = new Point(0, 0);
        lblMensaje.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        bottomPanel.Controls.Add(lblMensaje);

        // Pie
        var lblPie = new Label
        {
            Text = "TecnoFix Solutions",
            Font = Estilos.Fuente(8),
            ForeColor = Estilos.GrisMedio,
            AutoSize = true,
            Location = new Point(0, 48)
        };
        bottomPanel.Controls.Add(lblPie);

        AcceptButton = btnIngresar;
    }

    private void BtnIngresar_Click(object? sender, EventArgs e)
    {
        var usuario = txtUsuario.Text.Trim();
        var password = txtPassword.Text;

        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrEmpty(password))
        {
            lblMensaje.Text = "Ingrese usuario y contraseña.";
            return;
        }

        try
        {
            var u = UsuarioDAL.ObtenerPorNombreUsuario(usuario);
            if (u == null || !Seguridad.Verificar(password, u.PasswordHash))
            {
                lblMensaje.Text = "Credenciales inválidas.";
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            if (u.Estado != "Activo")
            {
                lblMensaje.Text = "El usuario está inactivo.";
                return;
            }

            Sesion.UsuarioID = u.UsuarioID;
            Sesion.NombreUsuario = u.NombreUsuario;
            Sesion.Rol = u.Rol;
            Sesion.TecnicoID = u.TecnicoID;

            var dash = new FrmDashboard(this);
            dash.Show();
            Hide();
        }
        catch (Exception)
        {
            lblMensaje.Text = "Error al conectar con la base de datos.";
        }
    }

    private void FrmLogin_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            BtnIngresar_Click(this, EventArgs.Empty);
        }
    }
}