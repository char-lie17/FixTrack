using FixTrack.Datos;
using FixTrack.Formularios;

namespace FixTrack;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Composición: se crea una única fábrica de DbContext a partir de appsettings.json.
        // Cada operación de datos obtiene su propio contexto; ninguno se comparte entre tareas.
        FixTrackDb.Inicializar();

        Application.Run(new FrmLogin());
    }
}
