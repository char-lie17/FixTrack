using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

public partial class FrmDispositivoFormulario : Form
{
    private readonly Dispositivo? _dispositivo;
    private readonly ComboBox cboCliente = new();
    private readonly TextBox txtTipo = new();
    private readonly TextBox txtMarca = new();
    private readonly TextBox txtModelo = new();
    private readonly TextBox txtNumeroSerie = new();
    private readonly TextBox txtDescripcion = new();

    public FrmDispositivoFormulario()
    {
        if (!Sesion.EsAdministrador && !Sesion.EsEmpleado)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        _dispositivo = null;
        Text = "Nuevo dispositivo";
        BuildUi();
    }

    public FrmDispositivoFormulario(Dispositivo dispositivo)
    {
        if (!Sesion.EsAdministrador && !Sesion.EsEmpleado)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        _dispositivo = dispositivo;
        Text = $"Editar dispositivo — {dispositivo.Tipo} {dispositivo.Marca}";
        BuildUi();
        txtTipo.Text = dispositivo.Tipo;
        txtMarca.Text = dispositivo.Marca ?? string.Empty;
        txtModelo.Text = dispositivo.Modelo ?? string.Empty;
        txtNumeroSerie.Text = dispositivo.NumeroSerie ?? string.Empty;
        txtDescripcion.Text = dispositivo.Descripcion ?? string.Empty;
        SeleccionarCliente(dispositivo.ClienteID);
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
            RowCount = 8,
            Padding = new Padding(24, 20, 24, 12),
            Margin = Padding.Empty,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        Controls.Add(layout);

        int row = 0;
        row = AgregarFila(layout, row, "Cliente *", cboCliente);
        row = AgregarFila(layout, row, "Tipo *", txtTipo);
        row = AgregarFila(layout, row, "Marca", txtMarca);
        row = AgregarFila(layout, row, "Modelo", txtModelo);
        row = AgregarFila(layout, row, "Número de serie", txtNumeroSerie);
        row = AgregarFila(layout, row, "Descripción", txtDescripcion);

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

        // Cargar clientes en combo. Al registrar un dispositivo nuevo solo se ofrecen
        // clientes activos (misma regla que ya se aplicaba a técnicos en FrmOrdenNueva).
        // Al editar se usa ObtenerTodos() para no perder al cliente actual si fue
        // desactivado después de registrar el dispositivo (hallazgo g de la auditoría).
        var clientes = _dispositivo == null ? ClienteDAL.ObtenerActivos() : ClienteDAL.ObtenerTodos();
        cboCliente.DataSource = clientes;
        cboCliente.DisplayMember = "NombreCompleto";
        cboCliente.ValueMember = "ClienteID";
        cboCliente.Dock = DockStyle.Fill;
    }

    private void SeleccionarCliente(int clienteId)
    {
        foreach (var item in cboCliente.Items)
        {
            if (item is Cliente c && c.ClienteID == clienteId)
            {
                cboCliente.SelectedItem = item;
                return;
            }
        }
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

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (cboCliente.SelectedValue == null)
        {
            MessageBox.Show(this, "Debe seleccionar un cliente.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var tipo = txtTipo.Text.Trim();
        if (string.IsNullOrWhiteSpace(tipo))
        {
            MessageBox.Show(this, "El tipo es obligatorio.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var clienteId = (int)cboCliente.SelectedValue;
        var marca = txtMarca.Text.Trim();
        var modelo = txtModelo.Text.Trim();
        var serie = txtNumeroSerie.Text.Trim();
        var desc = txtDescripcion.Text.Trim();

        if (_dispositivo == null)
        {
            var ok = UIHelper.EjecutarSeguro(this, () =>
            {
                DispositivoDAL.Insertar(new Dispositivo
                {
                    ClienteID = clienteId,
                    Tipo = tipo,
                    Marca = string.IsNullOrWhiteSpace(marca) ? null : marca,
                    Modelo = string.IsNullOrWhiteSpace(modelo) ? null : modelo,
                    NumeroSerie = string.IsNullOrWhiteSpace(serie) ? null : serie,
                    Descripcion = string.IsNullOrWhiteSpace(desc) ? null : desc
                });
            }, "Dispositivos");
            if (!ok) return;
            MessageBox.Show(this, "Dispositivo registrado exitosamente.", "Dispositivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            _dispositivo.ClienteID = clienteId;
            _dispositivo.Tipo = tipo;
            _dispositivo.Marca = string.IsNullOrWhiteSpace(marca) ? null : marca;
            _dispositivo.Modelo = string.IsNullOrWhiteSpace(modelo) ? null : modelo;
            _dispositivo.NumeroSerie = string.IsNullOrWhiteSpace(serie) ? null : serie;
            _dispositivo.Descripcion = string.IsNullOrWhiteSpace(desc) ? null : desc;
            var ok = UIHelper.EjecutarSeguro(this, () => DispositivoDAL.Actualizar(_dispositivo!), "Dispositivos");
            if (!ok) return;
            MessageBox.Show(this, "Dispositivo modificado exitosamente.", "Dispositivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        DialogResult = DialogResult.OK;
    }
}