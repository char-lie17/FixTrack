using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>Detalle de orden: cambio de estado, registro de pagos y observaciones (mockup 09_orden_detalle).</summary>
public partial class FrmOrdenDetalle : Form
{
    private readonly int _ordenId;
    private readonly TextBox txtCliente = new();
    private readonly TextBox txtDispositivo = new();
    private readonly TextBox txtTecnico = new();
    private readonly TextBox txtProblema = new();
    private readonly TextBox txtDiagnostico = new();
    private readonly TextBox txtTrabajoRealizado = new();
    private readonly TextBox txtObservaciones = new();
    private readonly NumericUpDown numCosto = new();
    private readonly ComboBox cboEstado = new();
    private readonly DateTimePicker dtFechaIngreso = new();
    private readonly DateTimePicker dtFechaFinalizacion = new();
    private readonly DataGridView gridPagos = new();
    private readonly DataGridView gridHistorial = new();
    private readonly Label lblTotalPagado = new();
    private readonly Label lblSaldo = new();
    private bool _cargando;
    private string _estadoActual = string.Empty;

    public FrmOrdenDetalle(int ordenId)
    {
        if (Sesion.EsTecnico && !Sesion.TecnicoID.HasValue)
        {
            MessageBox.Show("Su usuario no está asociado a un técnico. Contacte al administrador.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }

        if (Sesion.EsTecnico && Sesion.TecnicoID.HasValue)
        {
            var orden = OrdenServicioDAL.ObtenerPorId(ordenId);
            if (orden != null && orden.TecnicoID != Sesion.TecnicoID.Value)
            {
                MessageBox.Show("No puede acceder a órdenes de otros técnicos.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }
        }
        _ordenId = ordenId;
        Text = $"Detalle de orden #{ordenId}";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(900, 580);
        BackColor = Color.White;
        Font = Estilos.Fuente(9);
        InitializeUi();
        Cargar();
    }

    private void InitializeUi()
    {
        // Header
        var header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Estilos.Primario };
        header.Controls.Add(new Label { Text = Text, Font = Estilos.Fuente(12, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(16, 17) });

        // Layout principal: tabla de 2 columnas (izquierda: formulario, derecha: pagos)
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(16),
            Margin = Padding.Empty
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F)); // Izquierda
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F)); // Derecha

        // Panel izquierdo - formulario en TableLayoutPanel
        var leftLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 12,
            Padding = new Padding(0, 8, 0, 8),
            Margin = Padding.Empty,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        leftLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        leftLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainLayout.Controls.Add(leftLayout, 0, 0);

        int row = 0;
        row = AgregarFila(leftLayout, row, "Cliente", txtCliente);
        row = AgregarFila(leftLayout, row, "Dispositivo", txtDispositivo);
        row = AgregarFila(leftLayout, row, "Técnico", txtTecnico);
        row = AgregarFila(leftLayout, row, "Fecha ingreso", dtFechaIngreso);
        dtFechaIngreso.Format = DateTimePickerFormat.Short;
        dtFechaIngreso.Enabled = false;
        row = AgregarFila(leftLayout, row, "Fecha finalización", dtFechaFinalizacion);
        dtFechaFinalizacion.Format = DateTimePickerFormat.Short;
        dtFechaFinalizacion.Enabled = false;
        row = AgregarFila(leftLayout, row, "Problema reportado", txtProblema, 60);
        row = AgregarFila(leftLayout, row, "Diagnóstico", txtDiagnostico, 60);
        row = AgregarFila(leftLayout, row, "Trabajo realizado", txtTrabajoRealizado, 60);
        row = AgregarFila(leftLayout, row, "Observaciones", txtObservaciones, 60);
        row = AgregarFila(leftLayout, row, "Costo", numCosto);

        // Estado
        cboEstado.Items.AddRange(EstadoOrdenTexto.ItemsParaCombo.Cast<object>().ToArray());
        cboEstado.DropDownStyle = ComboBoxStyle.DropDownList;
        cboEstado.Dock = DockStyle.Fill;
        cboEstado.SelectedIndexChanged += CboEstado_SelectionChange;
        row = AgregarFila(leftLayout, row, "Estado", cboEstado);

        // Botones en fila final
        var btnPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 12, 0, 0)
        };
        leftLayout.Controls.Add(btnPanel, 1, row);

        var btnPago = new Button { Text = "+ Registrar pago", Size = new Size(150, 36), Margin = new Padding(0, 0, 12, 0) };
        Estilos.BotonPrincipal(btnPago);
        btnPago.Click += BtnPago_Click;
        btnPago.Visible = !Sesion.EsTecnico;
        btnPanel.Controls.Add(btnPago);

        var btnGuardar = new Button { Text = "Guardar cambios", Size = new Size(150, 36) };
        Estilos.BotonPrincipal(btnGuardar);
        btnGuardar.Click += BtnGuardar_Click;
        btnPanel.Controls.Add(btnGuardar);

        // Panel derecho - pagos
        var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8, 8, 0, 8) };
        mainLayout.Controls.Add(rightPanel, 1, 0);

        var rightLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = Padding.Empty,
            Margin = Padding.Empty
        };
        rightLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Título
        rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Pestañas (se expanden)
        rightLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Totales
        rightPanel.Controls.Add(rightLayout);

        var lblPagos = new Label { Text = "Pagos", Font = Estilos.Fuente(10, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 0, 0, 8) };
        rightLayout.Controls.Add(lblPagos, 0, 0);

        gridPagos.Dock = DockStyle.Fill;
        UIHelper.ConfigurarGrilla(gridPagos);
        gridPagos.Columns.Add(UIHelper.Col("Fecha", "FechaPago", 110));
        gridPagos.Columns.Add(UIHelper.Col("Monto", "Monto", 80));
        gridPagos.Columns.Add(UIHelper.Col("Método", "MetodoPago", 100));

        UIHelper.ConfigurarGrilla(gridHistorial);
        gridHistorial.Columns.Add(UIHelper.Col("Fecha", "FechaCambio", 120));
        gridHistorial.Columns.Add(UIHelper.Col("Tipo", "TipoCambio", 75));
        gridHistorial.Columns.Add(UIHelper.Col("Campo", "CampoModificado", 110));
        gridHistorial.Columns.Add(UIHelper.Col("Anterior", "ValorAnterior", 130));
        gridHistorial.Columns.Add(UIHelper.Col("Nuevo", "ValorNuevo", 130));
        gridHistorial.Columns.Add(UIHelper.Col("Usuario", "UsuarioNombre", 100));

        var pestañas = new TabControl { Dock = DockStyle.Fill };
        var pestañaPagos = new TabPage("Pagos") { Padding = new Padding(4) };
        var pestañaHistorial = new TabPage("Historial") { Padding = new Padding(4) };
        pestañaPagos.Controls.Add(gridPagos);
        pestañaHistorial.Controls.Add(gridHistorial);
        pestañas.TabPages.Add(pestañaPagos);
        pestañas.TabPages.Add(pestañaHistorial);
        rightLayout.Controls.Add(pestañas, 0, 1);

        var pnlTotales = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            Margin = new Padding(0, 12, 0, 0)
        };
        rightLayout.Controls.Add(pnlTotales, 0, 2);

        pnlTotales.Controls.Add(new Label { Text = "Total pagado:", AutoSize = true, Font = Estilos.Fuente(9), ForeColor = Estilos.Terciario });
        lblTotalPagado.Font = Estilos.Fuente(10, FontStyle.Bold);
        lblTotalPagado.AutoSize = true;
        lblTotalPagado.Margin = new Padding(0, 0, 0, 8);
        pnlTotales.Controls.Add(lblTotalPagado);

        pnlTotales.Controls.Add(new Label { Text = "Saldo:", AutoSize = true, Font = Estilos.Fuente(9), ForeColor = Estilos.Terciario });
        lblSaldo.Font = Estilos.Fuente(10, FontStyle.Bold);
        lblSaldo.AutoSize = true;
        pnlTotales.Controls.Add(lblSaldo);

        // Orden correcto: mainLayout (Fill) PRIMERO, luego header (Top).
        // En WinForms el último control agregado queda al frente y se dockeriza primero,
        // por lo que un Dock=Fill agregado al final cubriría el header (bug de tablas tapadas).
        Controls.Add(mainLayout);
        Controls.Add(header);
    }

    private static int AgregarFila(TableLayoutPanel layout, int row, string etiqueta, Control campo, int alto = 28)
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
        if (campo is TextBox tb && alto > 28)
        {
            tb.Multiline = true;
            tb.ScrollBars = ScrollBars.Vertical;
            tb.MinimumSize = new Size(0, alto);
        }
        layout.Controls.Add(campo, 1, row);

        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        return row + 1;
    }

    private void Cargar()
    {
        _cargando = true;
        try
        {
            UIHelper.EjecutarSeguro(this, () =>
            {
                var o = OrdenServicioDAL.ObtenerPorId(_ordenId);
                if (o == null)
                {
                    MessageBox.Show(this, "La orden no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    return;
                }
                txtCliente.Text = o.ClienteNombre ?? string.Empty;
                txtDispositivo.Text = o.DispositivoTexto ?? string.Empty;
                txtTecnico.Text = o.TecnicoNombre ?? string.Empty;
                txtProblema.Text = o.ProblemaReportado ?? string.Empty;
                txtObservaciones.Text = o.Observaciones ?? string.Empty;
                numCosto.Value = o.CostoServicio;
                SeleccionarEstado(o.Estado);
                _estadoActual = o.Estado;

                txtDiagnostico.Text = o.Diagnostico ?? string.Empty;
                txtTrabajoRealizado.Text = o.TrabajoRealizado ?? string.Empty;

                dtFechaIngreso.Value = o.FechaIngreso;
                dtFechaFinalizacion.Value = o.FechaFinalizacion ?? DateTime.Today;
                dtFechaFinalizacion.Enabled = Sesion.EsAdministrador || Sesion.EsEmpleado;

                var pagos = PagoDAL.ObtenerPorOrden(_ordenId);
                gridPagos.DataSource = pagos;
                gridHistorial.DataSource = OrdenServicioDAL.ObtenerHistorial(_ordenId);
                var total = pagos?.Sum(p => p.Monto) ?? 0m;
                lblTotalPagado.Text = total.ToString("C2");
                var saldo = o.CostoServicio - total;
                lblSaldo.Text = saldo.ToString("C2");
                lblSaldo.ForeColor = saldo <= 0 ? Color.Green : Color.Red;
            }, "Ordenes");
        }
        finally
        {
            _cargando = false;
        }
    }

    private void SeleccionarEstado(string estado)
    {
        for (int i = 0; i < cboEstado.Items.Count; i++)
            if (cboEstado.Items[i] is EstadoItem item && item.Valor == estado)
            {
                cboEstado.SelectedIndex = i;
                break;
            }
    }

    private void CboEstado_SelectionChange(object? sender, EventArgs e)
    {
        if (_cargando) return;
        if (cboEstado.SelectedItem is not EstadoItem item) return;

        // Validar transición según rol
        var nuevoEstado = item.Valor;
        if (Sesion.EsTecnico && nuevoEstado != "En diagnostico" && nuevoEstado != "En reparacion")
        {
            MessageBox.Show("El técnico solo puede cambiar el estado a 'En diagnóstico' o 'En reparación'.",
                "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            SeleccionarEstado(_estadoActual);
            return;
        }

        // Validar transiciones válidas (fuente de verdad centralizada)
        if (!EstadoOrdenTexto.EsTransicionValida(_estadoActual, nuevoEstado))
        {
            MessageBox.Show($"La transición de '{_estadoActual}' a '{nuevoEstado}' no es válida.",
                "Transición inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            SeleccionarEstado(_estadoActual);
            return;
        }

        // Confirmar
        if (MessageBox.Show(this, $"¿Cambiar el estado a «{item.Etiqueta}»?", "Confirmar cambio de estado",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            SeleccionarEstado(_estadoActual);
            return;
        }

        // Actualizar estado. Si la DAL no modifica filas, se revierte la selección.
        var actualizado = UIHelper.EjecutarSeguro(this, () =>
            OrdenServicioDAL.ActualizarEstado(_ordenId, nuevoEstado), "Ordenes");
        if (!actualizado)
        {
            SeleccionarEstado(_estadoActual);
            return;
        }
        _estadoActual = nuevoEstado;
    }

    private void BtnPago_Click(object? sender, EventArgs e)
    {
        using var f = new FrmPagoFormulario(_ordenId);
        if (f.ShowDialog(this) == DialogResult.OK) Cargar();
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        OrdenServicio? o = null;
        decimal totalPagado = 0m;

        UIHelper.EjecutarSeguro(this, () =>
        {
            o = OrdenServicioDAL.ObtenerPorId(_ordenId);
            if (o != null) totalPagado = PagoDAL.ObtenerTotalPagado(_ordenId);
        }, "Ordenes");

        if (o == null)
        {
            MessageBox.Show(this, "La orden no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (numCosto.Value < totalPagado)
        {
            MessageBox.Show(this, $"El costo no puede ser menor al total pagado ({totalPagado:C2}).",
                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        o.ProblemaReportado = txtProblema.Text.Trim();
        o.Diagnostico = string.IsNullOrWhiteSpace(txtDiagnostico.Text) ? null : txtDiagnostico.Text.Trim();
        o.TrabajoRealizado = string.IsNullOrWhiteSpace(txtTrabajoRealizado.Text) ? null : txtTrabajoRealizado.Text.Trim();
        o.Observaciones = string.IsNullOrWhiteSpace(txtObservaciones.Text) ? null : txtObservaciones.Text.Trim();
        o.CostoServicio = numCosto.Value;

        // La FechaFinalizacion es editable solo para Administrador/Empleado. Cuando el campo está
        // deshabilitado (p. ej. técnico) se conserva el valor original de BD para no borrar datos.
        if (dtFechaFinalizacion.Enabled)
        {
            o.FechaFinalizacion = dtFechaFinalizacion.Value;
        }

        var actualizado = UIHelper.EjecutarSeguro(this, () => OrdenServicioDAL.ActualizarDetalle(o), "Ordenes");
        if (actualizado)
            MessageBox.Show("Orden actualizada.", "Ordenes", MessageBoxButtons.OK, MessageBoxIcon.Information);
        else
            MessageBox.Show(this, "No se pudo actualizar la orden.", "Ordenes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
