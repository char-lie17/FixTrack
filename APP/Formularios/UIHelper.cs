namespace FixTrack.Formularios;

/// <summary>
/// Utilidades comunes de la capa de presentación.
/// Ejecuta operaciones de base de datos con manejo de errores uniforme
/// (Rules §9: excepciones manejadas con mensajes claros al usuario).
/// </summary>
public static class UIHelper
{
    /// <summary>Ejecuta una acción capturando y mostrando cualquier excepción.</summary>
    public static bool EjecutarSeguro(Form owner, Action accion, string titulo = "Error")
    {
        try
        {
            accion();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(owner,
                $"No se pudo completar la operación.\n\n{ex.Message}",
                titulo,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return false;
        }
    }

    /// <summary>Configura una grilla de listado (solo lectura, selección completa).</summary>
    public static void ConfigurarGrilla(DataGridView grid)
    {
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.AutoGenerateColumns = false;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.Font = Estilos.Fuente(9);
    }

    public static DataGridViewTextBoxColumn Col(string cabecera, string propiedad, int ancho)
        => new() { HeaderText = cabecera, DataPropertyName = propiedad, Width = ancho };
}
