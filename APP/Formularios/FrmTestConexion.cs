// Herramienta de diagnóstico interno — NO forma parte de la entrega funcional.
// Útil para verificar configuración de conexión, consultas parametrizadas y manejo de errores
// durante el desarrollo. No es accesible desde el menú principal ni desde el flujo normal de usuario.
using FixTrack.Datos;

namespace FixTrack.Formularios;

public partial class FrmTestConexion : Form
{
    public FrmTestConexion()
    {
        Text = "FASE 5 — Prueba de Conexión SQL Server";
        Size = new System.Drawing.Size(700, 500);
        StartPosition = FormStartPosition.CenterScreen;
        InitializeControls();
    }

    private void InitializeControls()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 7,
            Padding = new System.Windows.Forms.Padding(12),
            AutoScroll = true
        };
        layout.ColumnStyles.Add(new ColumnStyle(System.Windows.Forms.SizeType.Percent, 100));

        int row = 0;

        var titleLabel = new Label
        {
            Text = "Pruebas de Capa de Datos — FixTrack",
            Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold),
            ForeColor = System.Drawing.Color.FromArgb(0x2B, 0x2D, 0x42),
            AutoSize = true
        };
        layout.Controls.Add(titleLabel, 0, row++);

        var connInfo = new Label
        {
            Text = "Cadena de conexión configurada en appsettings.json",
            Font = new System.Drawing.Font("Segoe UI", 8),
            ForeColor = System.Drawing.Color.FromArgb(0x2B, 0x2D, 0x42),
            AutoSize = false,
            MaximumSize = new System.Drawing.Size(650, 60)
        };
        layout.Controls.Add(connInfo, 0, row++);

        var btnConfig = new Button
        {
            Text = "PRUEBA 1 — Leer Configuración",
            BackColor = System.Drawing.Color.FromArgb(0x2C, 0x5F, 0x8A),
            ForeColor = System.Drawing.Color.White,
            FlatStyle = FlatStyle.Flat,
            Height = 36,
            Tag = "config"
        };
        btnConfig.Click += BtnTest_Click;
        layout.Controls.Add(btnConfig, 0, row++);

        var btnConn = new Button
        {
            Text = "PRUEBA 2 — Abrir Conexión",
            BackColor = System.Drawing.Color.FromArgb(0x2C, 0x5F, 0x8A),
            ForeColor = System.Drawing.Color.White,
            FlatStyle = FlatStyle.Flat,
            Height = 36,
            Tag = "connection"
        };
        btnConn.Click += BtnTest_Click;
        layout.Controls.Add(btnConn, 0, row++);

        var btnSelect = new Button
        {
            Text = "PRUEBA 3 — Consulta SELECT (Clientes)",
            BackColor = System.Drawing.Color.FromArgb(0x2C, 0x5F, 0x8A),
            ForeColor = System.Drawing.Color.White,
            FlatStyle = FlatStyle.Flat,
            Height = 36,
            Tag = "select"
        };
        btnSelect.Click += BtnTest_Click;
        layout.Controls.Add(btnSelect, 0, row++);

        var btnParam = new Button
        {
            Text = "PRUEBA 4 — Consulta Parametrizada",
            BackColor = System.Drawing.Color.FromArgb(0x2C, 0x5F, 0x8A),
            ForeColor = System.Drawing.Color.White,
            FlatStyle = FlatStyle.Flat,
            Height = 36,
            Tag = "param"
        };
        btnParam.Click += BtnTest_Click;
        layout.Controls.Add(btnParam, 0, row++);

        var btnError = new Button
        {
            Text = "PRUEBA 5 — Error Controlado",
            BackColor = System.Drawing.Color.FromArgb(0xD6, 0x45, 0x45),
            ForeColor = System.Drawing.Color.White,
            FlatStyle = FlatStyle.Flat,
            Height = 36,
            Tag = "error"
        };
        btnError.Click += BtnTest_Click;
        layout.Controls.Add(btnError, 0, row++);

        var txtResult = new TextBox
        {
            Name = "txtResult",
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new System.Drawing.Font("Segoe UI", 9),
            BackColor = System.Drawing.Color.FromArgb(0xF4, 0xF6, 0xF8),
            Dock = DockStyle.Fill
        };
        layout.Controls.Add(txtResult, 0, row);

        Controls.Add(layout);
    }

    private void BtnTest_Click(object? sender, EventArgs e)
    {
        var btn = sender as Button;
        var tag = btn?.Tag?.ToString();
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"--- {btn?.Text} ---");
        sb.AppendLine($"Fecha: {DateTime.Now:HH:mm:ss}");

        try
        {
            switch (tag)
            {
                case "config":
                    var connStr = Conexion.GetConnectionString();
                    sb.AppendLine("OK - Configuración leída correctamente");
                    sb.AppendLine("  Fuente: appsettings.json");
                    sb.AppendLine("✓ Se encontró la cadena de conexión");
                    break;

                case "connection":
                    using (var conn = Conexion.ObtenerConexion())
                    {
                        conn.Open();
                        sb.AppendLine("OK - Conexión abierta correctamente");
                        sb.AppendLine($"  Estado: {conn.State}");
                        sb.AppendLine($"  DataSource: {conn.DataSource}");
                        sb.AppendLine($"  Database: {conn.Database}");
                        conn.Close();
                        sb.AppendLine("OK - Conexión cerrada correctamente");
                    }
                    break;

                case "select":
                    using (var conn = Conexion.ObtenerConexion())
                    {
                        conn.Open();
                        var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                            "SELECT TOP 3 ClienteID, Nombre, Apellido, Telefono, Estado FROM Clientes ORDER BY ClienteID",
                            conn);
                        using (var reader = cmd.ExecuteReader())
                        {
                            sb.AppendLine("OK - Consulta SELECT ejecutada correctamente");
                            sb.AppendLine("  Tabla: Clientes");
                            while (reader.Read())
                            {
                                sb.AppendLine($"  Registro: ID={reader["ClienteID"]}, " +
                                    $"Nombre={reader["Nombre"]}, " +
                                    $"Apellido={reader["Apellido"]}, " +
                                    $"Teléfono={reader["Telefono"]}, " +
                                    $"Estado={reader["Estado"]}");
                            }
                        }
                    }
                    break;

                case "param":
                    using (var conn = Conexion.ObtenerConexion())
                    {
                        conn.Open();
                        var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                            "SELECT TOP 3 ClienteID, Nombre, Apellido, Estado " +
                            "FROM Clientes WHERE Estado = @Estado AND ClienteID > @MinID ORDER BY ClienteID",
                            conn);
                        cmd.Parameters.AddWithValue("@Estado", "Activo");
                        cmd.Parameters.AddWithValue("@MinID", 0);
                        using (var reader = cmd.ExecuteReader())
                        {
                            sb.AppendLine("OK - Consulta parametrizada ejecutada");
                            sb.AppendLine("  Parámetros: @Estado='Activo', @MinID=0");
                            sb.AppendLine("  ✓ Sin concatenación de strings (sin riesgo de SQL Injection)");
                            while (reader.Read())
                            {
                                sb.AppendLine($"  Registro: ID={reader["ClienteID"]}, " +
                                    $"Nombre={reader["Nombre"]}, Estado={reader["Estado"]}");
                            }
                        }
                    }
                    break;

                case "error":
                    try
                    {
                        using (var conn = new Microsoft.Data.SqlClient.SqlConnection(
                            "Server=localhost;Database=BaseInexistente;TrustServerCertificate=True;Integrated Security=true;"))
                        {
                            conn.Open();
                        }
                    }
                    catch (Microsoft.Data.SqlClient.SqlException ex)
                    {
                        sb.AppendLine("OK - Error controlado capturado correctamente");
                        sb.AppendLine($"  Tipo: {ex.GetType().Name}");
                        sb.AppendLine($"  Número de error SQL: {ex.Number}");
                        sb.AppendLine("✓ Mensaje genérico, sin datos sensibles");
                    }
                    break;
            }

            sb.AppendLine();
            sb.AppendLine("RESULTADO: ÉXITO");
        }
        catch (Exception ex)
        {
            sb.AppendLine($"ERROR: {ex.GetType().Name}: {ex.Message}");
            sb.AppendLine();
            sb.AppendLine("RESULTADO: FALLO");
        }

        var txtResult = Controls.Find("txtResult", true).FirstOrDefault() as TextBox;
        if (txtResult != null)
        {
            txtResult.AppendText(sb.ToString());
            txtResult.AppendText(new string('-', 60) + Environment.NewLine);
        }
    }
}