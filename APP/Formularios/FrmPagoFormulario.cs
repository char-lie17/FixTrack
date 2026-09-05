using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>Registrar un pago asociado a una orden (mockup 11_pago_formulario).</summary>
public partial class FrmPagoFormulario : Form
{
    private readonly int? _ordenId;
    private readonly ComboBox cboOrden = new();
    private readonly NumericUpDown numMonto = new();
    private readonly ComboBox cboMetodo = new();
    private readonly TextBox txtObservaciones = new();
    private readonly Label lblOrden = new();

    public FrmPagoFormulario(int? ordenId = null)
    {
        if (!Sesion.EsAdministrador && !Sesion.EsEmpleado)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        _ordenId = ordenId;
        Text = ordenId.HasValue ? $"Registrar pago — Orden #{ordenId.Value}" : "Registrar nuevo pago";
        ClientSize = new Size(480, 440);
        StartPosition = FormStartPosition.CenterParent;
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
        if (_ordenId.HasValue)
        {
            lblOrden.Text = $"Orden #{_ordenId.Value}";
            lblOrden.AutoSize = true;
            lblOrden.ForeColor = Estilos.Terciario;
            lblOrden.Margin = new Padding(0, 6, 12, 6);
            lblOrden.Anchor = AnchorStyles.Left;
            layout.Controls.Add(lblOrden, 0, row);
            layout.Controls.Add(new Panel { Dock = DockStyle.Fill }, 1, row); // Placeholder
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            row++;
        }
        else
        {
            row = AgregarFila(layout, row, "Orden *", cboOrden);
        }

        row = AgregarFila(layout, row, "Método de pago *", cboMetodo);
        row = AgregarFila(layout, row, "Monto *", numMonto);
        row = AgregarFila(layout, row, "Observaciones", txtObservaciones);

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

        if (!_ordenId.HasValue)
        {
            var ordenes = OrdenServicioDAL.Buscar(null, null, null, null);
            cboOrden.DataSource = ordenes;
            cboOrden.DisplayMember = "DescripcionCombo";
            cboOrden.ValueMember = "OrdenID";
            cboOrden.Dock = DockStyle.Fill;
        }

        cboMetodo.Items.AddRange(MetodoPagoTexto.Valores);
        cboMetodo.SelectedIndex = 0;
        cboMetodo.Dock = DockStyle.Fill;

        numMonto.Maximum = 9999999m;
        numMonto.DecimalPlaces = 2;
        numMonto.ThousandsSeparator = true;
        numMonto.Dock = DockStyle.Fill;
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
        if (!_ordenId.HasValue && cboOrden.SelectedValue == null)
        {
            MessageBox.Show(this, "Debe seleccionar una orden.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (numMonto.Value <= 0)
        {
            MessageBox.Show(this, "El monto debe ser mayor que cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var ordenId = _ordenId ?? Convert.ToInt32(cboOrden.SelectedValue);

        // Validar que el pago no exceda el saldo pendiente
        var totalPagado = PagoDAL.ObtenerTotalPagado(ordenId);
        var orden = OrdenServicioDAL.ObtenerPorId(ordenId);
        if (orden != null)
        {
            var saldo = orden.CostoServicio - totalPagado;
            if (numMonto.Value > saldo)
            {
                MessageBox.Show(this, $"El pago no puede exceder el saldo pendiente. Saldo disponible: {saldo:C2}.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        var pago = new Pago
        {
            OrdenID = ordenId,
            Monto = numMonto.Value,
            MetodoPago = cboMetodo.SelectedItem?.ToString() ?? "Efectivo",
            Observaciones = string.IsNullOrWhiteSpace(txtObservaciones.Text) ? null : txtObservaciones.Text.Trim()
        };

        var ok = UIHelper.EjecutarSeguro(this, () =>
        {
            var id = PagoDAL.Insertar(pago);
            if (id <= 0) throw new ApplicationException("No se pudo registrar el pago.");
        }, "Pagos");
        if (!ok) return;

        MessageBox.Show(this, "Pago registrado exitosamente.", "Pagos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
    }
}