using SuSuerteV2.Domain;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using VirtualKeyboard.Wpf;

namespace SuSuerteV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            try
            {

                var procesos = Process.GetProcessesByName("SuSuerteV2");

                var process = Process.GetCurrentProcess();

                VKeyboard.Listen<TextBox>(e => e.Text);

                foreach (var item in procesos)
                {

                    if (process.Id != item.Id)
                    {
                        item.Kill();
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.Exit += AppExit;
            this.DispatcherUnhandledException += OnUnhandledException;

        }
        private async void AppExit(object? sender, ExitEventArgs e)
        {
#if NO_PERIPHERALS
#else
            await VideoRecorder.Stop();
#endif
            EventLogger.SaveLog(EventType.Info, $"La aplicación se ha cerrado manualmente con codigo: {e.ApplicationExitCode}");


        }
        private async void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Muestra un mensaje de error
            MessageBox.Show("Ha ocurrido un error: " + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
#if NO_PERIPHERALS
#else
            await VideoRecorder.Stop();
#endif
            EventLogger.SaveLog(EventType.FatalError, $"Ocurrió un error fatal en la aplicación, excepción no manejada: {e.Exception.Message}", e.Exception);


            e.Handled = false;
        }

    }

}
