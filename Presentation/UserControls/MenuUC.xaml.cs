using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.Enumerables;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Modals;
using SuSuerteV2.Presentation.UserControls.BetPlay;
using SuSuerteV2.UserControls;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SuSuerteV2.Presentation.UserControls
{
    public partial class MenuUC : AppUserControl
    {
        private readonly Transaction _ts;
        private readonly Task consultarProductos;
        private List<SubProductoGeneral> subProductosKiosko;
    

    

        public MenuUC()
        {
            InitializeComponent();
            Loaded += MenuUC_Loaded;
            Navigator.Instance.CloseModal();
        }

        private void MenuUC_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicialización cuando el control se carga
            EventLogger.SaveLog(EventType.Info, "Menú principal cargado");
        }

        // EVENTO UNIFICADO - Funciona para Mouse y Táctil
        private void UIElement_Interaction(object sender, MouseButtonEventArgs e)
        {
            try
            {
                e.Handled = true; // Detiene la propagación del evento

                if (!(sender is FrameworkElement element)) return;

                // Efecto visual al presionar
                ScaleElement(element, 0.95);

                // Procesa el botón presionado después de un pequeño retraso
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(150);
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    ProcessButtonClick(element.Name);
                    ScaleElement(element, 1.0); // Restaura el tamaño
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en interacción", ex);
            }
        }

        private void ProcessButtonClick(string buttonName)
        {
            switch (buttonName)
            {
                case "btnChance":
                    OpenChanceMenu();
                    break;

                case "btnRecaudos":
                    OpenRecaudosMenu();
                    break;

                case "btnBetplay":
                    OpenBetplayMenu();
                    break;

                case "btnAstro":
                    OpenAstroMenu();
                    break;

                case "btnRecargas":
                    OpenRecargasMenu();
                    break;

                default:
                    EventLogger.SaveLog(EventType.Warning, $"Botón no reconocido: {buttonName}");
                    break;
            }
        }

        private void ScaleElement(FrameworkElement element, double scale)
        {
            try
            {
                var transform = element.RenderTransform as ScaleTransform;
                if (transform == null)
                {
                    transform = new ScaleTransform();
                    element.RenderTransform = transform;
                    element.RenderTransformOrigin = new Point(0.5, 0.5);
                }
                transform.ScaleX = scale;
                transform.ScaleY = scale;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error aplicando efecto visual", ex);
            }
        }

        // ==========  LÓGICA ==========
     
  
        private async void OpenRecaudosMenu()
        {
            try
            {
                EventLogger.SaveLog(EventType.Info, "Iniciando proceso de Recaudos");

                // Mostrar modal de carga
                var loadingModal = Navigator.Instance.ShowLoadModal("Validando servicios de recaudos...");
                InhabilitarVista();

                await Task.Run(async () =>
                {
                    try
                    {
                        var request = new RequestGetRecaudos
                        {
                            IdTrx = _ts.IdTransaccionApi,
                            codigo = Convert.ToInt32(ConfigurationManager.AppSettings["IdDepartamento"])
                        };

                        var respuesta = await ApiIntegration.GetRecaudos(request);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Navigator.Instance.CloseModal();

                            if (respuesta?.Estado == true)
                            {
                                _ts.listadorecaudosField = respuesta.Listadorecaudos;
                                _ts.tramite = "ServiciosPublicos";

                                EventLogger.SaveLog(EventType.Info, "Recaudos disponibles",
                                 $"Cantidad: {respuesta.Listadorecaudos?.Recaudo?.Count ?? 0}");

                              //  Navigator.Instance.NavigateTo(new SelectCompanyUC(_ts));
                            }
                            else
                            {
                                Navigator.Instance.ShowModal(
                                    "El servicio de recaudos no está disponible en este momento. Intente nuevamente.",
                                    ModalType.Error);
                                HabilitarVista();
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Navigator.Instance.CloseModal();
                            EventLogger.SaveLog(EventType.Error, "Error en proceso de recaudos", ex);
                            Navigator.Instance.ShowModal(
                                "Ocurrió un error al consultar los recaudos disponibles",
                                ModalType.Error);
                            HabilitarVista();
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error inesperado en OpenRecaudosMenu", ex);
                Navigator.Instance.ShowModal(
                    "Ocurrió un error inesperado al acceder a recaudos",
                    ModalType.Error);
                HabilitarVista();
            }
        }

        private void OpenBetplayMenu()
        {
        
            try
            {
                EventLogger.SaveLog(EventType.Info, "Entrando a la opcion BtnBetplay");
                InhabilitarVista();
                NavigateTo(new LoginUC());

            }
            catch (Exception ex)
            {

                EventLogger.SaveLog(EventType.Error, "Error en la consulta de subproductos del kiosko", ex);
               
                Navigator.Instance.ShowModal("Este servicio no se encuentra disponible en este momento, intenta nuevamente", ModalType.Error);
            }
        }

        private void OpenAstroMenu()
        {
           
            InhabilitarVista();
            EventLogger.SaveLog(EventType.Info, "Abriendo menú Super Astro");
            _ts.Type = ETypeTramites.Astro;
           // NavigateTo(new ScanDocumentUC());
        }

        private async void OpenChanceMenu()
        {
            try
            {
                EventLogger.SaveLog(EventType.Info, "Iniciando proceso El Mejor Chance");

                // Mostrar modal de carga
                var loadingModal = Navigator.Instance.ShowLoadModal("Validando servicios disponibles...");
                InhabilitarVista();

                // Ejecutar en segundo plano
                await Task.Run(async () =>
                {
                    try
                    {
                        // Consultar productos
                        var respuesta = await ApiIntegration.GetProductsChance();
                        var respuestaConsulta = await ConsultSubProducts();

                        EventLogger.SaveLog(EventType.Info, "Respuesta ConsultSubProducts",
                            JsonConvert.SerializeObject(respuestaConsulta));

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Navigator.Instance.CloseModal(); // Cierra el loading

                            if (respuestaConsulta)
                            {
                                _ts.Type = ETypeTramites.SuperChance;
                                _ts.ProductSelected = subProductosKiosko.FirstOrDefault(x => x.Nombre == "EL MEJOR CHANCE");

                                if (_ts.ProductSelected != null)
                                {
                                    EventLogger.SaveLog(EventType.Info, $"Producto seleccionado: {_ts.ProductSelected.Nombre}");
                                    //NavigateTo(new ScanDocumentUC(_ts));
                                }
                                else
                                {
                                    Navigator.Instance.ShowModal("El servicio 'El Mejor Chance' no está disponible", ModalType.Error);
                                    HabilitarVista();
                                }
                            }
                            else
                            {
                                Navigator.Instance.ShowModal("El servicio no está disponible en este momento", ModalType.Error);
                                HabilitarVista();
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Navigator.Instance.CloseModal();
                            EventLogger.SaveLog(EventType.Error, "Error en El Mejor Chance", ex);
                            Navigator.Instance.ShowModal("Error al acceder al servicio. Intente nuevamente.", ModalType.Error);
                            HabilitarVista();
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error inesperado en OpenElMejorChance", ex);
                Navigator.Instance.ShowModal("Ocurrió un error inesperado", ModalType.Error);
                HabilitarVista();
            }
        }

        private void ShowInformation()
        {
            EventLogger.SaveLog(EventType.Info, "Mostrando información");

            // TODO: Agregar tu lógica aquí
            // Ejemplo: NavigateTo(new InformationUC());
        }

        private void OpenRecargasMenu()
        {
            InhabilitarVista();
            EventLogger.SaveLog(EventType.Info, "Abriendo menú Recargas");
          // NavigateTo(new ValidacionRegistroUC());
        }

        private void InhabilitarVista()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 0.3;
               this.IsEnabled = false;
            });
        }

        private void HabilitarVista()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 1;
                this.IsEnabled = true;
            });
        }
        private async Task<bool> ConsultSubProducts()
        {
            try
            {
                var respuesta = await ApiIntegration.GetProductsChance();



                if (respuesta == null || respuesta.Estado != true)
                {

                    return false;
                }

                subProductosKiosko = respuesta.Listadosubproductos.Subproducto;
                return true;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en la consulta de subproductos del kiosko", ex);
                return false;
            }
        }

    }
}