using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

public partial class FrmOrdenNueva : Form
{
    private readonly ComboBox cboDispositivo = new();
    private readonly ComboBox cboTecnico = new();
    private readonly TextBox txtProblema = new();
    private readonly NumericUpDown numCosto = new();
    private readonly TextBox txtObservaciones = new();
    private readonly CheckBox chkAbono = new();
    private readonly NumericUpDown numMontoAbono = new();
    private readonly ComboBox cboMetodo = new();

    public FrmOrdenNueva()
    {
        if (!Sesion.EsAdministrador && !Sesion.EsEmpleado)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        Text = "Nueva orden de servicio";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(500, 560);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;
        Font = Estilos.Fuente(9);
        BuildUi();
    }

    private void BuildUi()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 10,
            Padding = new Padding(24, 16, 24, 12),
            Margin = Padding.Empty,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        Controls.Add(layout);

        int row = 0;
        row = AgregarFila(layout, row, "Dispositivo *", cboDispositivo);
        row = AgregarFila(layout, row, "Técnico (opcional)", cboTecnico);
        row = AgregarFila(layout, row, "Problema reportado *", txtProblema);
        row = AgregarFila(layout, row, "Costo del servicio", numCosto);
        row = AgregarFila(layout, row, "Observaciones", txtObservaciones);

        // Checkbox abono
        chkAbono.Text = "Registrar abono inicial";
        chkAbono.AutoSize = true;
        chkAbono.Margin = new Padding(0, 8, 12, 8);
        chkAbono.Anchor = AnchorStyles.Left;
        chkAbono.CheckedChanged += (_, _) => HabilitarAbono();
        layout.Controls.Add(chkAbono, 0, row);
        layout.Controls.Add(new Panel { Dock = DockStyle.Fill }, 1, row); // Placeholder
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        row++;

        row = AgregarFila(layout, row, "Monto del abono", numMontoAbono);
        row = AgregarFila(layout, row, "Método de pago", cboMetodo);

        // Botones
        var btnPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 16, 0, 0)
        };
        layout.Controls.Add(btnPanel, 1, row);

        var btnCancelar = new Button { Text = "Cancelar", Size = new Size(150, 36), Margin = new Padding(6, 0, 0, 0) };
        Estilos.BotonSecundario(btnCancelar);
        btnCancelar.Click += (_, _) => DialogResult = DialogResult.Cancel;
        btnPanel.Controls.Add(btnCancelar);

        var btnGuardar = new Button { Text = "Guardar orden", Size = new Size(150, 36) };
        Estilos.BotonPrincipal(btnGuardar);
        btnGuardar.Click += BtnGuardar_Click;
        btnPanel.Controls.Add(btnGuardar);

        AcceptButton = btnGuardar;
        CancelButton = btnCancelar;

        // Cargar datos
        var dispositivos = DispositivoDAL.Buscar(null);
        cboDispositivo.DataSource = dispositivos;
        cboDispositivo.DisplayMember = "DescripcionCombo";
        cboDispositivo.ValueMember = "DispositivoID";
        cboDispositivo.Dock = DockStyle.Fill;

        var tecnicos = TecnicoDAL.ObtenerActivos();
        cboTecnico.Items.Add("Sin asignar");
        foreach (var t in tecnicos) cboTecnico.Items.Add(t);
        cboTecnico.DisplayMember = "NombreCompleto";
        cboTecnico.SelectedIndex = 0;
        cboTecnico.Dock = DockStyle.Fill;

        numCosto.Maximum = 999999m;
        numCosto.DecimalPlaces = 2;
        numCosto.ThousandsSeparator = true;
        numCosto.Dock = DockStyle.Fill;

        numMontoAbono.Maximum = 999999m;
        numMontoAbono.DecimalPlaces = 2;
        numMontoAbono.ThousandsSeparator = true;
        numMontoAbono.Dock = DockStyle.Fill;

        cboMetodo.Items.AddRange(MetodoPagoTexto.Valores);
        cboMetodo.SelectedIndex = 0;
        cboMetodo.Dock = DockStyle.Fill;

        HabilitarAbono();
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

    private void HabilitarAbono()
    {
        numMontoAbono.Enabled = chkAbono.Checked;
        cboMetodo.Enabled = chkAbono.Checked;
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (cboDispositivo.SelectedValue == null)
        {
            MessageBox.Show(this, "Debe seleccionar un dispositivo.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(txtProblema.Text))
        {
            MessageBox.Show(this, "El problema reportado es obligatorio.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (numMontoAbono.Enabled && numMontoAbono.Value <= 0)
        {
            MessageBox.Show(this, "El monto del abono debe ser mayor que cero.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        // Validar que el abono inicial no exceda el costo del servicio
        if (numMontoAbono.Enabled && numCosto.Value > 0 && numMontoAbono.Value > numCosto.Value)
        {
            MessageBox.Show(this, "El abono inicial no puede exceder el costo del servicio.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        // Si el costo es 0, no se permite abono positivo
        if (numMontoAbono.Enabled && numCosto.Value == 0 && numMontoAbono.Value > 0)
        {
            MessageBox.Show(this, "No se puede registrar un abono cuando el costo del servicio es 0.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var orden = new OrdenServicio
        {
            DispositivoID = (int)cboDispositivo.SelectedValue,
            TecnicoID = cboTecnico.SelectedItem is Tecnico t ? t.TecnicoID : null,
            ProblemaReportado = txtProblema.Text.Trim(),
            CostoServicio = numCosto.Value,
            Observaciones = string.IsNullOrWhiteSpace(txtObservaciones.Text) ? null : txtObservaciones.Text.Trim()
        };

        var resultado = false;
        if (chkAbono.Checked)
        {
            var pago = new Pago
            {
                Monto = numMontoAbono.Value,
                MetodoPago = cboMetodo.SelectedItem?.ToString() ?? "Efectivo"
            };
            resultado = UIHelper.EjecutarSeguro(this, () => OrdenServicioDAL.InsertarConPagoInicial(orden, pago), "Ordenes");
        }
        else
        {
            resultado = UIHelper.EjecutarSeguro(this, () => OrdenServicioDAL.Insertar(orden), "Ordenes");
        }

        if (resultado)
        {
            MessageBox.Show(this, chkAbono.Checked
                ? "Orden registrada con abono inicial exitosamente."
                : "Orden registrada exitosamente.", "Ordenes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        }
    }
}