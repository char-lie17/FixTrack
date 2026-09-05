using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>
/// Formulario crear/editar usuario.
/// Validaciones: NombreUsuario obligatorio, Password obligatorio al crear, Rol obligatorio.
/// Hash SHA-256 en contraseña. Validación de unicidad de NombreUsuario y asociación único técnico-usuario.
/// </summary>
public partial class FrmUsuarioFormulario : Form
{
    private readonly Usuario? _usuario;
    private readonly TextBox txtNombreUsuario = new();
    private readonly TextBox txtPassword = new();
    private readonly ComboBox cboRol = new();
    private readonly ComboBox cboTecnico = new();
    private readonly Label lblPasswordHint = new();

    public FrmUsuarioFormulario()
    {
        if (!Sesion.EsAdministrador)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        _usuario = null;
        Text = "Nuevo usuario";
        BuildUi();
    }

    public FrmUsuarioFormulario(Usuario usuario)
    {
        if (!Sesion.EsAdministrador)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        _usuario = usuario;
        Text = $"Editar usuario — {usuario.NombreUsuario}";
        BuildUi();
        txtNombreUsuario.Text = usuario.NombreUsuario;
        txtNombreUsuario.ReadOnly = true; // No se permite cambiar el nombre de usuario al editar
        cboRol.SelectedItem = usuario.Rol;
        if (usuario.TecnicoID.HasValue)
        {
            SeleccionarTecnico(usuario.TecnicoID.Value);
        }
        lblPasswordHint.Visible = true;
        txtPassword.PasswordChar = '•';
        txtPassword.PlaceholderText = "Dejar vacío para no cambiar la contraseña";
    }

    private void BuildUi()
    {
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(480, 460);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;
        Font = Estilos.Fuente(9);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 7,
            Padding = new Padding(24, 20, 24, 12),
            Margin = Padding.Empty,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        Controls.Add(layout);

        int row = 0;
        row = AgregarFila(layout, row, "Nombre de usuario *", txtNombreUsuario);

        row = AgregarFila(layout, row, "Contraseña *", txtPassword);
        txtPassword.PasswordChar = '•';
        lblPasswordHint.Text = "Dejar vacío para no cambiar la contraseña";
        lblPasswordHint.Font = Estilos.Fuente(7.5f);
        lblPasswordHint.ForeColor = Estilos.GrisMedio;
        lblPasswordHint.AutoSize = true;
        lblPasswordHint.Margin = new Padding(0, 0, 0, 4);
        lblPasswordHint.Visible = false;
        layout.Controls.Add(lblPasswordHint, 1, row); // Debajo del password

        row = AgregarFila(layout, row, "Rol *", cboRol);
        cboRol.DropDownStyle = ComboBoxStyle.DropDownList;
        cboRol.Items.AddRange(new[] { "Administrador", "Empleado", "Tecnico" });
        cboRol.SelectedIndex = 0;
        cboRol.SelectedIndexChanged += CboRol_SelectedIndexChanged;
        cboRol.Dock = DockStyle.Fill;

        row = AgregarFila(layout, row, "Técnico asociado", cboTecnico);
        cboTecnico.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTecnico.Enabled = false;
        cboTecnico.Dock = DockStyle.Fill;
        CargarTecnicos();

        // Botones
        var btnPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 12, 0, 0)
        };
        layout.Controls.Add(btnPanel, 1, row);

        var btnCancelar = new Button { Text = "Cancelar", Size = new Size(140, 36), Margin = new Padding(6, 0, 0, 0) };
        Estilos.BotonSecundario(btnCancelar);
        btnCancelar.Click += (_, _) => DialogResult = DialogResult.Cancel;
        btnPanel.Controls.Add(btnCancelar);

        var btnGuardar = new Button { Text = "Guardar", Size = new Size(140, 36) };
        Estilos.BotonPrincipal(btnGuardar);
        btnGuardar.Click += BtnGuardar_Click;
        btnPanel.Controls.Add(btnGuardar);

        AcceptButton = btnGuardar;
        CancelButton = btnCancelar;
    }

    private static int AgregarFila(TableLayoutPanel layout, int row, string etiqueta, Control campo)
    {
        var lbl = new Label
        {
            Text = etiqueta,
            AutoSize = true,
            ForeColor = Estilos.Terciario,
            Margin = new Padding(0, 6, 12, 6),
            Anchor = AnchorStyles.Left,
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.Controls.Add(lbl, 0, row);

        campo.Dock = DockStyle.Fill;
        campo.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        campo.Margin = new Padding(0, 4, 0, 4);
        layout.Controls.Add(campo, 1, row);

        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        return row + 1;
    }

    private void CboRol_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var esTecnico = cboRol.SelectedItem?.ToString() == "Tecnico";
        cboTecnico.Enabled = esTecnico;
        if (!esTecnico)
        {
            cboTecnico.SelectedIndex = -1;
        }
    }

    private void CargarTecnicos()
    {
        var tecnicos = TecnicoDAL.ObtenerActivos();
        var lista = tecnicos.ToList();
        // Incluir técnico actualmente asociado aunque esté inactivo
        if (_usuario?.TecnicoID.HasValue == true)
        {
            var activo = lista.Any(t => t.TecnicoID == _usuario.TecnicoID.Value);
            if (!activo)
            {
                var inactivo = TecnicoDAL.ObtenerPorId(_usuario.TecnicoID.Value);
                if (inactivo != null) lista.Add(inactivo);
            }
        }
        cboTecnico.DataSource = lista;
        cboTecnico.DisplayMember = "NombreCompleto";
        cboTecnico.ValueMember = "TecnicoID";
        cboTecnico.SelectedIndex = -1;
    }

    private void SeleccionarTecnico(int tecnicoId)
    {
        foreach (var item in cboTecnico.Items)
        {
            if (item is Tecnico t && t.TecnicoID == tecnicoId)
            {
                cboTecnico.SelectedItem = item;
                return;
            }
        }
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        var nombreUsuario = txtNombreUsuario.Text.Trim();
        var password = txtPassword.Text;
        var rol = cboRol.SelectedItem?.ToString() ?? string.Empty;
        var seleccion = cboTecnico.SelectedValue;
        int? tecnicoId = seleccion == null || seleccion is DBNull ? null : Convert.ToInt32(seleccion);

        if (string.IsNullOrWhiteSpace(nombreUsuario))
        {
            MessageBox.Show(this, "El nombre de usuario es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_usuario == null && string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show(this, "La contraseña es obligatoria al crear un usuario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(rol))
        {
            MessageBox.Show(this, "El rol es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (rol == "Tecnico" && !tecnicoId.HasValue)
        {
            MessageBox.Show(this, "Debe seleccionar un técnico asociado para el rol Técnico.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var excluirId = _usuario?.UsuarioID ?? 0;
        if (UsuarioDAL.ExisteNombreUsuario(nombreUsuario, excluirId))
        {
            MessageBox.Show(this, "El nombre de usuario ya existe. Elija otro.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (tecnicoId.HasValue)
        {
            var todosUsuarios = UsuarioDAL.ObtenerTodos();
            var tecnicoYaAsociado = todosUsuarios.Any(u => u.TecnicoID == tecnicoId && u.UsuarioID != excluirId);
            if (tecnicoYaAsociado)
            {
                var tecnico = TecnicoDAL.ObtenerPorId(tecnicoId.Value);
                MessageBox.Show(this, $"El técnico «{tecnico?.NombreCompleto}» ya está asociado a otro usuario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        if (_usuario == null)
        {
            var hash = Seguridad.Hashear(password);
            var ok = UIHelper.EjecutarSeguro(this, () =>
            {
                UsuarioDAL.Insertar(new Usuario
                {
                    NombreUsuario = nombreUsuario,
                    PasswordHash = hash,
                    Rol = rol,
                    TecnicoID = tecnicoId
                });
            }, "Usuarios");
            if (!ok) return;
            MessageBox.Show(this, "Usuario registrado exitosamente.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            _usuario.NombreUsuario = nombreUsuario;
            _usuario.Rol = rol;
            _usuario.TecnicoID = tecnicoId;

            if (!string.IsNullOrWhiteSpace(password))
            {
                _usuario.PasswordHash = Seguridad.Hashear(password);
            }

            var ok = UIHelper.EjecutarSeguro(this, () => UsuarioDAL.Actualizar(_usuario!), "Usuarios");
            if (!ok) return;
            MessageBox.Show(this, "Usuario modificado exitosamente.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        DialogResult = DialogResult.OK;
    }
}