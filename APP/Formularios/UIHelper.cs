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

    /// <summary>
    /// Versión asíncrona de <see cref="EjecutarSeguro"/>.
    /// Distingue la cancelación (OperationCanceledException) de los errores reales:
    /// cancelar una operación de lectura no es un error y no se muestra diálogo.
    /// </summary>
    public static async Task<bool> EjecutarSeguroAsync(Form owner, Func<Task> accion, string titulo = "Error")
    {
        try
        {
            await accion();
            return true;
        }
        catch (OperationCanceledException)
        {
            // Cancelación solicitada (búsqueda reemplazada o formulario cerrado): no es un error.
            return false;
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

    /// <summary>
    /// Similar a <see cref="EjecutarSeguroAsync"/> pero devuelve el resultado.
    /// Devuelve null si la operación fue cancelada o falló (el error ya se mostró al usuario).
    /// </summary>
    public static async Task<T?> EjecutarSeguroAsync<T>(Form owner, Func<Task<T>> accion, string titulo = "Error")
        where T : class
    {
        try
        {
            return await accion();
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (Exception ex)
        {
            MessageBox.Show(owner,
                $"No se pudo completar la operación.\n\n{ex.Message}",
                titulo,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return null;
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
