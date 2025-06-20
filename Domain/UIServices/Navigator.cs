using SuSuerteV2.Modals;
using System.Windows;
using System.Windows.Controls;

namespace SuSuerteV2.Domain.UIServices
{
    /// <summary>
    /// Servicio de navegación y manejo de ventanas modales.
    /// </summary>
    public class Navigator : IDisposable
    {
        private static readonly Lazy<Navigator> _lazyInstance = new Lazy<Navigator>(() => new Navigator());

        private MainWindow MainWindow { get; set; }
        private ModalWindow CurrentModal { get; set; }
        private readonly Queue<ModalWindow> _modalQueue = new Queue<ModalWindow>();
        private bool _disposed = false;

        private Navigator() { }

        /// <summary>
        /// Obtiene la instancia singleton del Navigator.
        /// </summary>
        public static Navigator Instance => _lazyInstance.Value;

        /// <summary>
        /// Indica si el navegador ha sido inicializado.
        /// </summary>
        public bool IsInitialized => MainWindow != null;

        /// <summary>
        /// Inicializa el navegador con la ventana principal.
        /// </summary>
        /// <param name="mainWindow">La ventana principal de la aplicación.</param>
        /// <exception cref="ArgumentNullException">Se lanza si mainWindow es null.</exception>
        public void Init(MainWindow mainWindow)
        {
            MainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
        }

        /// <summary>
        /// Navega a la vista especificada.
        /// </summary>
        /// <param name="view">La vista a la que navegar.</param>
        /// <exception cref="InvalidOperationException">Se lanza cuando el navegador no ha sido inicializado.</exception>
        public void NavigateTo(UserControl view)
        {
            if (MainWindow == null)
            {
                throw new InvalidOperationException("El navegador no ha sido inicializado. Llame al método Init() primero.");
            }

            if (MainWindow.Dispatcher.CheckAccess())
            {
                MainWindow.MainContainer.Content = view;
                return;
            }

            MainWindow.Dispatcher.Invoke(() =>
            {
                MainWindow.MainContainer.Content = view;
            });
        }

        /// <summary>
        /// Navega a la vista especificada de manera asíncrona.
        /// </summary>
        /// <param name="view">La vista a la que navegar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task NavigateToAsync(UserControl view)
        {
            if (MainWindow == null)
            {
                throw new InvalidOperationException("El navegador no ha sido inicializado.");
            }

            await MainWindow.Dispatcher.InvokeAsync(() =>
            {
                MainWindow.MainContainer.Content = view;
            });
        }

        /// <summary>
        /// Muestra un modal con el mensaje y tipo especificados.
        /// </summary>
        /// <param name="msg">El mensaje a mostrar.</param>
        /// <param name="type">El tipo de modal.</param>
        /// <returns>El resultado del modal.</returns>
        public bool ShowModal(string msg, ModalType type)
        {
            bool result = false;

            var model = new ModalViewModel
            {
                Title = "Estimado Cliente: ",
                Message = msg,
                TypeModal = type,
            };

            Application.Current.Dispatcher.Invoke(delegate
            {
                CurrentModal = new ModalWindow(model);
                CurrentModal.ShowDialog();
                if (CurrentModal.DialogResult.HasValue)
                {
                    result = CurrentModal.DialogResult.Value;
                }
                CurrentModal = null;
            });

            return result;
        }

        /// <summary>
        /// Muestra un modal de carga.
        /// </summary>
        /// <param name="msg">El mensaje a mostrar.</param>
        /// <returns>La ventana modal de carga.</returns>
        public ModalWindow ShowLoadModal(string msg)
        {
            ModalWindow loadWindow = null;

            var model = new ModalViewModel
            {
                Title = "Estimado Cliente: ",
                Message = msg,
                TypeModal = new LoadModal(),
            };

            Application.Current.Dispatcher.Invoke(delegate
            {
                loadWindow = new ModalWindow(model);
                CurrentModal = loadWindow;
                loadWindow.Show();
            });

            return loadWindow;
        }

        /// <summary>
        /// Cierra el modal actual.
        /// </summary>
        public void CloseModal()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (CurrentModal != null)
                {
                    CurrentModal.Close();
                    CurrentModal = null;
                }
            });
        }

        /// <summary>
        /// Reinicia el navegador, cerrando todos los modales y limpiando referencias.
        /// </summary>
        public void Reset()
        {
            CloseModal();
            MainWindow = null;
            _modalQueue.Clear();
        }

        /// <summary>
        /// Libera los recursos del navegador.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Reset();
                }
                _disposed = true;
            }
        }



    }
}