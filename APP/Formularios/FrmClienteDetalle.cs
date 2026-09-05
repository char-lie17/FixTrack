using System.Linq;
using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>
/// Detalle de cliente con sus dispositivos (mockup 05_cliente_detalle).
/// </summary>
public partial class FrmClienteDetalle : Form
{
    private readonly int _clienteId;
    private readonly DataGridView grid = new();

public FrmClienteDetalle(int clienteId)
    {
        if (Sesion.EsTecnico && Sesion.TecnicoID.HasValue)
        {
            var misOrdenes = OrdenServicioDAL.ObtenerPorTecnico(Sesion.TecnicoID.Value);
            var dispositivosCliente = DispositivoDAL.ObtenerPorCliente(clienteId);
            var idsDispositivosAutorizados = new HashSet<int>(misOrdenes.Select(o => o.DispositivoID));
            var dispositivosAutorizados = dispositivosCliente.Where(d => idsDispositivosAutorizados.Contains(d.DispositivoID)).ToList();
            if (dispositivosAutorizados.Count == 0 && dispositivosCliente.Count > 0)
            {
                MessageBox.Show("No puede acceder a dispositivos que no le correspondan.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }
        }
        _clienteId = clienteId;
        Text = "Detalle del cliente";
        ClientSize = new Size(760, 520);
        StartPosition = FormStartPosition.CenterParent;
        Font = Estilos.Fuente(9);
        BuildUi();
    }

    private void BuildUi()
    {
        UIHelper.EjecutarSeguro(this, () =>
        {
            var c = ClienteDAL.ObtenerPorId(_clienteId);
            if (c == null)
            {
                MessageBox.Show(this, "El cliente ya no existe.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            // Panel info con FlowLayoutPanel para layout responsivo
            var info = new Panel { Dock = DockStyle.Top, Height = 150, Padding = new Padding(16) };
            var infoLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Margin = new Padding(0, 4, 0, 4)
            };
            info.Controls.Add(infoLayout);

            infoLayout.Controls.Add(new Label
            {
                Text = $"{c.Nombre} {c.Apellido}",
                Font = Estilos.Fuente(12, FontStyle.Bold),
                ForeColor = Estilos.Primario,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 6)
            });

            infoLayout.Controls.Add(new Label
            {
                Text = $"Teléfono: {c.Telefono}    Estado: {c.Estado}",
                AutoSize = true,
                ForeColor = Estilos.Terciario,
                Margin = new Padding(0, 0, 0, 4)
            });

            infoLayout.Controls.Add(new Label
            {
                Text = $"Email: {c.Email ?? "—"}",
                AutoSize = true,
                ForeColor = Estilos.Terciario,
                Margin = new Padding(0, 0, 0, 4)
            });

            infoLayout.Controls.Add(new Label
            {
                Text = $"Dirección: {c.Direccion ?? "—"}  ·  Registro: {c.FechaRegistro:dd/MM/yyyy}",
                AutoSize = true,
                ForeColor = Estilos.Terciario
            });

            // Label dispositivos
            var lblDisp = new Label
            {
                Text = "Dispositivos",
                Font = Estilos.Fuente(10, FontStyle.Bold),
                ForeColor = Estilos.Terciario,
                AutoSize = true,
                Dock = DockStyle.Top,
                Padding = new Padding(16, 12, 0, 0)
            };

            // Grid
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AutoGenerateColumns = false;
            grid.RowHeadersVisible = false;
            grid.BackgroundColor = Color.White;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "DispositivoID", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tipo", DataPropertyName = "Tipo", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Marca", DataPropertyName = "Marca", Width = 110 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Modelo", DataPropertyName = "Modelo", Width = 130 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nº serie", DataPropertyName = "NumeroSerie", Width = 130 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Descripción", DataPropertyName = "Descripcion", Width = 190 });
            if (Sesion.EsTecnico && Sesion.TecnicoID.HasValue)
            {
                var misOrdenes = OrdenServicioDAL.ObtenerPorTecnico(Sesion.TecnicoID.Value);
                var idsAutorizados = new HashSet<int>(misOrdenes.Select(o => o.DispositivoID));
                var todos = DispositivoDAL.ObtenerPorCliente(_clienteId);
                grid.DataSource = todos.Where(d => idsAutorizados.Contains(d.DispositivoID)).ToList();
            }
            else
            {
                grid.DataSource = DispositivoDAL.ObtenerPorCliente(_clienteId);
            }

            // Orden correcto: grid (Fill) PRIMERO, luego lblDisp e info (Top).
            // En WinForms el último control agregado queda al frente y se dockeriza primero,
            // por lo que un Dock=Fill agregado al final cubriría la info y la etiqueta.
            Controls.Add(grid);
            Controls.Add(lblDisp);
            Controls.Add(info);
        }, "Clientes");
    }
}