using System.Drawing;
using System.Windows.Forms;
using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>Vista de detalle de un pago (readonly).</summary>
public partial class FrmPagoDetalle : Form
{
    private readonly DataGridView grid = new();

    public FrmPagoDetalle(int pagoId)
    {
        if (!Sesion.EsAdministrador && !Sesion.EsEmpleado)
        {
            MessageBox.Show("No tiene permiso para acceder a esta funcionalidad.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        Text = $"Detalle de pago #{pagoId}";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(620, 440);
        BackColor = Color.White;
        Font = Estilos.Fuente(9);

        var header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Estilos.Primario };
        header.Controls.Add(new Label { Text = Text, Font = Estilos.Fuente(12, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(16, 17) });

        // Panel de botones inferior
        var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.White, Padding = new Padding(0, 8, 16, 8) };
        var btnCerrar = new Button { Text = "Cerrar", Size = new Size(100, 36), Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
        Estilos.BotonPrincipal(btnCerrar);
        btnCerrar.Click += (_, _) => Close();
        bottomPanel.Controls.Add(btnCerrar);

        grid.Dock = DockStyle.Fill;
        UIHelper.ConfigurarGrilla(grid);

        // Orden correcto: grid (Fill) PRIMERO, luego los paneles con dock (Top/Bottom).
        // En WinForms el último control agregado queda al frente y se dockeriza primero,
        // por lo que un Dock=Fill agregado al final cubriría header y bottomPanel.
        Controls.Add(grid);
        Controls.Add(header);
        Controls.Add(bottomPanel);

        Cargar(pagoId);
    }

    private void Cargar(int pagoId)
    {
        UIHelper.EjecutarSeguro(this, () =>
        {
            var p = PagoDAL.ObtenerPorId(pagoId);
            if (p == null)
            {
                MessageBox.Show(this, "El pago no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }
            var o = OrdenServicioDAL.ObtenerPorId(p.OrdenID);

            grid.ColumnCount = 2;
            grid.Columns[0].Name = "Campo";
            grid.Columns[1].Name = "Valor";
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            var rows = new[]
            {
                new object[] { "Pago ID", p.PagoID.ToString() },
                new object[] { "Orden", p.OrdenID.ToString() },
                new object[] { "Fecha de pago", p.FechaPago.ToString("g") },
                new object[] { "Método", p.MetodoPago ?? "" },
                new object[] { "Monto", (p.Monto).ToString("C2") },
                new object[] { "Observaciones", p.Observaciones ?? "" },
                new object[] { "Cliente", o?.ClienteNombre ?? "" },
                new object[] { "Estado orden", o?.Estado ?? "" }
            };
            foreach (var r in rows)
                grid.Rows.Add(r);
        }, "Pagos");
    }
}