using System.Linq;
using FixTrack.Datos;
using FixTrack.Modelos;

namespace FixTrack.Formularios;

/// <summary>
/// Detalle de cliente con sus dispositivos (mockup 05_cliente_detalle).
/// La carga (consulta de permisos y dispositivos) se hace de forma asíncrona al mostrarse
/// el diálogo, para no bloquear el hilo de interfaz.
/// </summary>
public partial class FrmClienteDetalle : Form
{
    private readonly int _clienteId;
    private readonly DataGridView grid = new();

    public FrmClienteDetalle(int clienteId)
    {
        _clienteId = clienteId;
        Text = "Detalle del cliente";
        ClientSize = new Size(760, 520);
        StartPosition = FormStartPosition.CenterParent;
        Font = Estilos.Fuente(9);
        BuildUi();
        Shown += async (_, _) => await CargarAsync();
    }

    private void BuildUi()
    {
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
            Name = "lblNombre",
            Text = string.Empty,
            Font = Estilos.Fuente(12, FontStyle.Bold),
            ForeColor = Estilos.Primario,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 6)
        });

        infoLayout.Controls.Add(new Label
        {
            Name = "lblTelefonoEstado",
            Text = "Cargando…",
            AutoSize = true,
            ForeColor = Estilos.Terciario,
            Margin = new Padding(0, 0, 0, 4)
        });

        infoLayout.Controls.Add(new Label
        {
            Name = "lblEmail",
            Text = string.Empty,
            AutoSize = true,
            ForeColor = Estilos.Terciario,
            Margin = new Padding(0, 0, 0, 4)
        });

        infoLayout.Controls.Add(new Label
        {
            Name = "lblDireccion",
            Text = string.Empty,
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
        grid.UseWaitCursor = true;

        // Orden correcto: grid (Fill) PRIMERO, luego lblDisp e info (Top).
        // En WinForms el último control agregado queda al frente y se dockeriza primero,
        // por lo que un Dock=Fill agregado al final cubriría la info y la etiqueta.
        Controls.Add(grid);
        Controls.Add(lblDisp);
        Controls.Add(info);
    }

    private async Task CargarAsync()
    {
        try
        {
            // Control de acceso del rol técnico: solo dispositivos con órdenes propias.
            if (Sesion.EsTecnico && Sesion.TecnicoID.HasValue)
            {
                var misOrdenes = await OrdenServicioDAL.ObtenerPorTecnicoAsync(Sesion.TecnicoID.Value);
                var dispositivosCliente = await DispositivoDAL.ObtenerPorClienteAsync(_clienteId);
                var idsDispositivosAutorizados = new HashSet<int>(misOrdenes.Select(o => o.DispositivoID));
                var dispositivosAutorizados = dispositivosCliente.Where(d => idsDispositivosAutorizados.Contains(d.DispositivoID)).ToList();
                if (dispositivosAutorizados.Count == 0 && dispositivosCliente.Count > 0)
                {
                    MessageBox.Show("No puede acceder a dispositivos que no le correspondan.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    return;
                }
            }

            var c = await ClienteDAL.ObtenerPorIdAsync(_clienteId);
            if (c == null)
            {
                MessageBox.Show(this, "El cliente ya no existe.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            var infoLayout = Controls.OfType<Panel>()
                .SelectMany(p => p.Controls.OfType<FlowLayoutPanel>())
                .FirstOrDefault();
            if (infoLayout != null)
            {
                ActualizarLabel(infoLayout, "lblNombre", $"{c.Nombre} {c.Apellido}");
                ActualizarLabel(infoLayout, "lblTelefonoEstado", $"Teléfono: {c.Telefono}    Estado: {c.Estado}");
                ActualizarLabel(infoLayout, "lblEmail", $"Email: {c.Email ?? "—"}");
                ActualizarLabel(infoLayout, "lblDireccion", $"Dirección: {c.Direccion ?? "—"}  ·  Registro: {c.FechaRegistro:dd/MM/yyyy}");
            }

            List<Dispositivo> dispositivos;
            if (Sesion.EsTecnico && Sesion.TecnicoID.HasValue)
            {
                var misOrdenes = await OrdenServicioDAL.ObtenerPorTecnicoAsync(Sesion.TecnicoID.Value);
                var idsAutorizados = new HashSet<int>(misOrdenes.Select(o => o.DispositivoID));
                var todos = await DispositivoDAL.ObtenerPorClienteAsync(_clienteId);
                dispositivos = todos.Where(d => idsAutorizados.Contains(d.DispositivoID)).ToList();
            }
            else
            {
                dispositivos = await DispositivoDAL.ObtenerPorClienteAsync(_clienteId);
            }
            grid.DataSource = dispositivos;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this,
                $"No se pudo cargar el detalle del cliente.\n\n{ex.Message}",
                "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            grid.UseWaitCursor = false;
        }
    }

    private static void ActualizarLabel(Control contenedor, string nombre, string texto)
    {
        var lbl = contenedor.Controls.Find(nombre, searchAllChildren: true).FirstOrDefault();
        if (lbl != null) lbl.Text = texto;
    }
}
