using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>
/// Formulario crear/editar técnico.
/// Validaciones: Nombre y Apellido obligatorios.
/// </summary>
public partial class FrmTecnicoFormulario : Form
{
    private readonly Tecnico? _tecnico;
    private readonly TextBox txtNombre = new();
    private readonly TextBox txtApellido = new();
    private readonly TextBox txtTelefono = new();
    private readonly TextBox txtEspecialidad = new();

    public FrmTecnicoFormulario()
    {
        if (!Sesion.EsAdministrador)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        _tecnico = null;
        Text = "Nuevo técnico";
        BuildUi();
    }

    public FrmTecnicoFormulario(Tecnico tecnico)
    {
        if (!Sesion.EsAdministrador)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        _tecnico = tecnico;
        Text = $"Editar técnico — {tecnico.Nombre} {tecnico.Apellido}";
        BuildUi();
        txtNombre.Text = tecnico.Nombre;
        txtApellido.Text = tecnico.Apellido;
        txtTelefono.Text = tecnico.Telefono ?? string.Empty;
        txtEspecialidad.Text = tecnico.Especialidad ?? string.Empty;
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
            RowCount = 6,
            Padding = new Padding(24, 20, 24, 12),
            Margin = Padding.Empty,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        Controls.Add(layout);

        int row = 0;
        row = AgregarFila(layout, row, "Nombre *", txtNombre);
        row = AgregarFila(layout, row, "Apellido *", txtApellido);
        row = AgregarFila(layout, row, "Teléfono", txtTelefono);
        row = AgregarFila(layout, row, "Especialidad", txtEspecialidad);

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
        var especialidad = txtEspecialidad.Text.Trim();

        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
        {
            MessageBox.Show(this, "El nombre y el apellido son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_tecnico == null)
        {
            var ok = UIHelper.EjecutarSeguro(this, () =>
            {
                TecnicoDAL.Insertar(new Tecnico
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono,
                    Especialidad = string.IsNullOrWhiteSpace(especialidad) ? null : especialidad
                });
            }, "Técnicos");
            if (!ok) return;
            MessageBox.Show(this, "Técnico registrado exitosamente.", "Técnicos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            _tecnico.Nombre = nombre;
            _tecnico.Apellido = apellido;
            _tecnico.Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono;
            _tecnico.Especialidad = string.IsNullOrWhiteSpace(especialidad) ? null : especialidad;
            var ok = UIHelper.EjecutarSeguro(this, () => TecnicoDAL.Actualizar(_tecnico!), "Técnicos");
            if (!ok) return;
            MessageBox.Show(this, "Técnico modificado exitosamente.", "Técnicos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        DialogResult = DialogResult.OK;
    }
}