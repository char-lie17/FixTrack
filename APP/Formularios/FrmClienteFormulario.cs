using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>
/// Formulario crear/editar cliente (mockup 04_cliente_formulario).
/// Validaciones: Nombre obligatorio, Teléfono obligatorio, Email con formato si se ingresa.
/// </summary>
public partial class FrmClienteFormulario : Form
{
    private readonly Cliente? _cliente;
    private readonly TextBox txtNombre = new();
    private readonly TextBox txtApellido = new();
    private readonly TextBox txtTelefono = new();
    private readonly TextBox txtEmail = new();
    private readonly TextBox txtDireccion = new();

    public FrmClienteFormulario()
    {
        if (!Sesion.EsAdministrador && !Sesion.EsEmpleado)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        _cliente = null;
        Text = "Nuevo cliente";
        BuildUi();
    }

    public FrmClienteFormulario(Cliente cliente)
    {
        if (!Sesion.EsAdministrador && !Sesion.EsEmpleado)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        _cliente = cliente;
        Text = $"Editar cliente — {cliente.Nombre} {cliente.Apellido}";
        BuildUi();
        txtNombre.Text = cliente.Nombre;
        txtApellido.Text = cliente.Apellido;
        txtTelefono.Text = cliente.Telefono;
        txtEmail.Text = cliente.Email ?? string.Empty;
        txtDireccion.Text = cliente.Direccion ?? string.Empty;
    }

    private void BuildUi()
    {
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(440, 380);
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
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Labels
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Campos (se expanden)
        Controls.Add(layout);

        int row = 0;
        row = AgregarFila(layout, row, "Nombre *", txtNombre);
        row = AgregarFila(layout, row, "Apellido", txtApellido);
        row = AgregarFila(layout, row, "Teléfono *", txtTelefono);
        row = AgregarFila(layout, row, "Email", txtEmail);
        row = AgregarFila(layout, row, "Dirección", txtDireccion);

        // Botones en fila final
        var btnPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 12, 0, 0)
        };
        layout.Controls.Add(btnPanel, 1, row); // Columna 1 (campos)

        var btnCancelar = new Button { Text = "Cancelar", Size = new Size(120, 36), Margin = new Padding(6, 0, 0, 0) };
        Estilos.BotonSecundario(btnCancelar);
        btnCancelar.Click += (_, _) => DialogResult = DialogResult.Cancel;
        btnPanel.Controls.Add(btnCancelar);

        var btnGuardar = new Button { Text = "Guardar", Size = new Size(120, 36) };
        Estilos.BotonPrincipal(btnGuardar);
        btnGuardar.Click += BtnGuardar_Click;
        btnPanel.Controls.Add(btnGuardar);

        AcceptButton = btnGuardar;
        CancelButton = btnCancelar;
    }

    private static int AgregarFila(TableLayoutPanel layout, int row, string etiqueta, TextBox campo)
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

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        var nombre = txtNombre.Text.Trim();
        var apellido = txtApellido.Text.Trim();
        var telefono = txtTelefono.Text.Trim();
        var email = txtEmail.Text.Trim();
        var direccion = txtDireccion.Text.Trim();

        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(telefono))
        {
            MessageBox.Show(this, "El nombre, el apellido y el teléfono son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!string.IsNullOrWhiteSpace(email) && !EsEmailValido(email))
        {
            MessageBox.Show(this, "El correo electrónico no tiene un formato válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_cliente == null)
        {
            var ok = UIHelper.EjecutarSeguro(this, () =>
            {
                ClienteDAL.Insertar(new Cliente
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    Telefono = telefono,
                    Email = string.IsNullOrWhiteSpace(email) ? null : email,
                    Direccion = string.IsNullOrWhiteSpace(direccion) ? null : direccion
                });
            }, "Clientes");
            if (!ok) return;
            MessageBox.Show(this, "Cliente registrado exitosamente.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            _cliente.Nombre = nombre;
            _cliente.Apellido = apellido;
            _cliente.Telefono = telefono;
            _cliente.Email = string.IsNullOrWhiteSpace(email) ? null : email;
            _cliente.Direccion = string.IsNullOrWhiteSpace(direccion) ? null : direccion;
            var ok = UIHelper.EjecutarSeguro(this, () => ClienteDAL.Actualizar(_cliente!), "Clientes");
            if (!ok) return;
            MessageBox.Show(this, "Cliente modificado exitosamente.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        DialogResult = DialogResult.OK;
    }

    private static bool EsEmailValido(string email) => email.Contains('@') && email.Contains('.');
}