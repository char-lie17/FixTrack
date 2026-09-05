using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>
/// Reportes (13_reportes.png). Acceso Administrador y Empleado.
/// 4 reportes oficiales con filtros de fecha y exportación a CSV.
/// </summary>
public partial class FrmReportes : Form
{
    private readonly ComboBox cboReporte = new();
    private readonly DateTimePicker dtDesde = new();
    private readonly DateTimePicker dtHasta = new();
    private readonly Button btnGenerar = new();
    private readonly Button btnExportar = new();
    private readonly DataGridView grid = new();
    private DataTable? _tablaActual;

    public FrmReportes()
    {
        if (!Sesion.EsAdministrador && !Sesion.EsEmpleado)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        Text = "Reportes";
        InitializeUi();
        EstablecerRangoPorDefecto();
    }

    private void InitializeUi()
    {
        // 1. Grilla
        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.AutoGenerateColumns = true;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.Font = Estilos.Fuente(9);

        // 2. Barra de controles (dos filas en TableLayoutPanel)
        var barra = new Panel { Dock = DockStyle.Top, Height = 110, BackColor = Color.White, Padding = new Padding(12, 6, 12, 6) };
        var barraLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = Padding.Empty,
            Margin = Padding.Empty
        };
        barraLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Fila 1: selector
        barraLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Fila 2: fechas + botones
        barra.Controls.Add(barraLayout);

        // Fila 1: Selector de reporte
        var filaReporte = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = true,
            Margin = new Padding(0, 0, 0, 8)
        };
        barraLayout.Controls.Add(filaReporte, 0, 0);

        var pnlReporte = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        cboReporte.DropDownStyle = ComboBoxStyle.DropDownList;
        cboReporte.Size = new Size(350, 28);
        cboReporte.Items.AddRange(new[]
        {
            "Órdenes por estado",
            "Órdenes por técnico",
            "Servicios completados",
            "Pagos registrados"
        });
        cboReporte.SelectedIndex = 0;
        var lblReporte = new Label { Text = "Reporte:", AutoSize = true, ForeColor = Estilos.Terciario, Margin = new Padding(0, 5, 8, 0), TextAlign = ContentAlignment.MiddleLeft };
        pnlReporte.Controls.Add(lblReporte);
        pnlReporte.Controls.Add(cboReporte);
        filaReporte.Controls.Add(pnlReporte);

        // Fila 2: Fechas y botones
        var filaControles = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = true
        };
        barraLayout.Controls.Add(filaControles, 0, 1);

        // Desde
        var pnlDesde = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        dtDesde.Format = DateTimePickerFormat.Short;
        dtDesde.Checked = true;
        dtDesde.Size = new Size(120, 28);
        var lblDesde = new Label { Text = "Desde:", AutoSize = true, ForeColor = Estilos.Terciario, Margin = new Padding(0, 5, 8, 0), TextAlign = ContentAlignment.MiddleLeft };
        pnlDesde.Controls.Add(lblDesde);
        pnlDesde.Controls.Add(dtDesde);
        filaControles.Controls.Add(pnlDesde);

        // Hasta
        var pnlHasta = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 12, 4), WrapContents = false };
        dtHasta.Format = DateTimePickerFormat.Short;
        dtHasta.Checked = true;
        dtHasta.Size = new Size(120, 28);
        var lblHasta = new Label { Text = "Hasta:", AutoSize = true, ForeColor = Estilos.Terciario, Margin = new Padding(0, 5, 8, 0), TextAlign = ContentAlignment.MiddleLeft };
        pnlHasta.Controls.Add(lblHasta);
        pnlHasta.Controls.Add(dtHasta);
        filaControles.Controls.Add(pnlHasta);

        // Espaciador
        var spacer = new Panel { Size = new Size(20, 1) };
        filaControles.Controls.Add(spacer);

        // Botones (RightToLeft)
        var pnlBotones = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            Margin = new Padding(0, 4, 0, 0)
        };
        filaControles.Controls.Add(pnlBotones);

        btnExportar.Text = "Exportar a CSV";
        btnExportar.Size = new Size(130, 36);
        btnExportar.Margin = new Padding(6, 0, 0, 0);
        Estilos.BotonSecundario(btnExportar);
        btnExportar.Click += BtnExportar_Click;
        pnlBotones.Controls.Add(btnExportar);

        btnGenerar.Text = "Generar";
        btnGenerar.Size = new Size(100, 36);
        btnGenerar.Margin = new Padding(6, 0, 6, 0);
        Estilos.BotonPrincipal(btnGenerar);
        btnGenerar.Click += BtnGenerar_Click;
        pnlBotones.Controls.Add(btnGenerar);

        // 3. Encabezado
        var header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Estilos.Primario };
        var titulo = new Label { Text = "Reportes", Font = Estilos.Fuente(12, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(16, 17) };
        header.Controls.Add(titulo);

        // Orden correcto: header (arriba del todo) -> barra -> grid (Fill, al final)
        Controls.Add(header);
        Controls.Add(barra);
        Controls.Add(grid);
    }

    private void EstablecerRangoPorDefecto()
    {
        dtHasta.Value = DateTime.Today;
        dtDesde.Value = DateTime.Today.AddDays(-30);
    }

    private void BtnGenerar_Click(object? sender, EventArgs e)
    {
        var desde = dtDesde.Value.Date;
        var hasta = dtHasta.Value.Date;

        if (desde > hasta)
        {
            MessageBox.Show(this, "La fecha 'Desde' no puede ser mayor que la fecha 'Hasta'.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var reporte = cboReporte.SelectedItem?.ToString() ?? string.Empty;

        UIHelper.EjecutarSeguro(this, () =>
        {
            _tablaActual = reporte switch
            {
                "Órdenes por estado" => ReportesDAL.ObtenerOrdenesPorEstado(desde, hasta),
                "Órdenes por técnico" => ReportesDAL.ObtenerOrdenesPorTecnico(desde, hasta),
                "Servicios completados" => ReportesDAL.ObtenerServiciosCompletados(desde, hasta),
                "Pagos registrados" => ReportesDAL.ObtenerPagosRegistrados(desde, hasta),
                _ => new DataTable()
            };

            grid.DataSource = _tablaActual;
            grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }, "Reportes");
    }

    private void BtnExportar_Click(object? sender, EventArgs e)
    {
        if (_tablaActual == null || _tablaActual.Rows.Count == 0)
        {
            MessageBox.Show(this, "No hay datos para exportar. Genere un reporte primero.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var nombreReporte = (cboReporte.SelectedItem?.ToString() ?? "Reporte").Replace(" ", "_");
        using var sfd = new SaveFileDialog
        {
            Filter = "CSV (*.csv)|*.csv",
            FileName = $"Reporte_{nombreReporte}_{DateTime.Now:yyyyMMdd}.csv",
            Title = "Guardar reporte como CSV"
        };

        if (sfd.ShowDialog(this) != DialogResult.OK) return;

        UIHelper.EjecutarSeguro(this, () =>
        {
            var csv = GenerarCsv(_tablaActual!);
            File.WriteAllText(sfd.FileName, csv, Encoding.UTF8);
            MessageBox.Show(this, $"Reporte exportado exitosamente a:\n{sfd.FileName}", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }, "Reportes");
    }

    private static string GenerarCsv(DataTable tabla)
    {
        var sb = new StringBuilder();
        var headers = tabla.Columns.Cast<DataColumn>().Select(c => EscapeCsv(c.ColumnName));
        sb.AppendLine(string.Join(",", headers));
        foreach (DataRow row in tabla.Rows)
        {
            var fields = tabla.Columns.Cast<DataColumn>().Select(c => EscapeCsv(row[c]?.ToString() ?? string.Empty));
            sb.AppendLine(string.Join(",", fields));
        }
        return sb.ToString();
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
        return value;
    }
}