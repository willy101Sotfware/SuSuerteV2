using ControlzEx.Standard;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.ApiService.Models;
using SuSuerteV2.Domain.Enumerables;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Domain.Variables;
using SuSuerteV2.Modals;
using SuSuerteV2.UserControls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SuSuerteV2.Presentation.UserControls.BetPlay
{
    /// <summary>
    /// Lógica de interacción para RechargeUC.xaml
    /// </summary>
    public partial class RechargeUC : AppUserControl
    {
        #region Constantes
        private const string TIMER_INICIAL = "01:00";
        private const int ANIMATION_DELAY = 150;
        private const double SCALE_PRESSED = 0.95;
        private const double SCALE_NORMAL = 1.0;
        private const decimal MONTO_MINIMO = 2000;
        private const decimal MONTO_MAXIMO = 500000;
        private const decimal INCREMENTO_VALIDO = 100;
    
        #endregion

        #region Propiedades Privadas
        private TimerGeneric _timer;
        private DispatcherTimer _animationTimer;
        private Transaction _ts;
        private ApiIntegration _apiIntegration = new ();
        private ModalWindow? _currentLoadModal = null;
        private List<SubProductoGeneral> subProductosKiosko;
        private Navigator _nav = Navigator.Instance;
        #endregion

        #region Constructor
        public RechargeUC()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeComponents();
        }

       
        #endregion

        #region Eventos de Interacción
        /// <summary>
        /// Evento unificado para interacciones Mouse y Táctil
        /// </summary>
        private void UIElement_Interaction(object sender, MouseButtonEventArgs e)
        {
            try
            {
                e.Handled = true;

                if (!(sender is FrameworkElement element)) return;

                ApplyPressEffect(element);
                ProcessElementWithDelay(element);
            }
            catch (Exception ex)
            {
                LogError("UIElement_Interaction", ex);
            }
        }

        /// <summary>
        /// Evento cuando el control se descarga
        /// </summary>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanupResources();
        }

        /// <summary>
        /// Evento cuando el TextBox obtiene el foco
        /// </summary>
        private void focusTxtvalidar(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (TxtMonto.Text == "Valor")
                {
                    TxtMonto.Text = "";
                    textBox.Background = new SolidColorBrush(Colors.LightGray);
                    TxtMonto.Foreground = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    textBox.Background = new SolidColorBrush(Colors.LightGray);
                    TxtMonto.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        /// <summary>
        /// Evento cuando el TextBox pierde el foco
        /// </summary>
        private void TxtVal_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (string.IsNullOrWhiteSpace(TxtMonto.Text))
                {
                    TxtMonto.Text = "Valor";
                    TxtMonto.Foreground = new SolidColorBrush(Colors.Gray);
                    textBox.Background = new SolidColorBrush(Colors.White);
                }
                else
                {
                    TxtMonto.Foreground = new SolidColorBrush(Colors.Black);
                    textBox.Background = new SolidColorBrush(Colors.White);
                }
            }
        }

        /// <summary>
        /// Evento cuando cambia el texto del TextBox
        /// </summary>
        private void txtVal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtMonto.Text.Length > 15)
            {
                TxtMonto.Text = TxtMonto.Text.Remove(15, 1);
            }
        }
        #endregion

        #region Métodos de Procesamiento de Elementos
        /// <summary>
        /// Procesa el elemento clickeado con un pequeño retraso para la animación
        /// </summary>
        private void ProcessElementWithDelay(FrameworkElement element)
        {
            _animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(ANIMATION_DELAY)
            };

            _animationTimer.Tick += (s, args) =>
            {
                _animationTimer.Stop();
                _animationTimer = null;

                ProcessElementClick(element);
                RestoreElementScale(element);
            };

            _animationTimer.Start();
        }

        /// <summary>
        /// Procesa el click del elemento según su tipo
        /// </summary>
        private void ProcessElementClick(FrameworkElement element)
        {
            try
            {
                string elementName = element.Name ?? "";
                string tag = element.Tag?.ToString() ?? "";

                // Procesar elementos por Tag
                if (!string.IsNullOrEmpty(tag))
                {
                    ProcessByTag(tag);
                }

                // Procesar elementos por Nombre
                if (!string.IsNullOrEmpty(elementName))
                {
                    ProcessByName(elementName);
                }
            }
            catch (Exception ex)
            {
                LogError("ProcessElementClick", ex);
            }
        }

        /// <summary>
        /// Procesa elementos por su Tag
        /// </summary>
        private void ProcessByTag(string tag)
        {
            switch (tag)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    ProcessNumberInput(tag);
                    break;

                case "Borrar":
                    ProcessDeleteAll();
                    break;

                case "All":
                    ProcessDeleteOne();
                    break;
            }
        }

        /// <summary>
        /// Procesa elementos por su Nombre
        /// </summary>
        private void ProcessByName(string elementName)
        {
            switch (elementName)
            {
                case "btnContinuar":
                    ProcessContinuar();
                    break;

                case "btnCancelar":
                    ProcessCancelar();
                    break;

                case "btnAtras":
                    ProcessAtras();
                    break;
            }
        }
        #endregion

        #region Métodos de Acciones
        /// <summary>
        /// Procesa la entrada de un dígito numérico
        /// </summary>
        private void ProcessNumberInput(string digit)
        {
            try
            {
                TxtMonto.Text += digit;
            }
            catch (Exception ex)
            {
                LogError("ProcessNumberInput", ex);
            }
        }

        /// <summary>
        /// Procesa la acción de borrar todo
        /// </summary>
        private void ProcessDeleteAll()
        {
            try
            {
                TxtMonto.Text = string.Empty;
            }
            catch (Exception ex)
            {
                LogError("ProcessDeleteAll", ex);
            }
        }

        /// <summary>
        /// Procesa la acción de borrar un carácter
        /// </summary>
        private void ProcessDeleteOne()
        {
            try
            {
                string val = TxtMonto.Text;
                if (val.Length > 0)
                {
                    TxtMonto.Text = val.Remove(val.Length - 1);
                }
            }
            catch (Exception ex)
            {
                LogError("ProcessDeleteOne", ex);
            }
        }

        /// <summary>
        /// Procesa la acción de continuar
        /// </summary>
        private async void ProcessContinuar()
        {
            try
            {
                SetCallBacksNull();
                _timer?.Stop();

                EventLogger.SaveLog(EventType.Info, "RechargeUC", "Ingresa al evento ProcessContinuar", "OK");

                await InhabilitarVista();

                if (Validate())
                {
                    EventLogger.SaveLog(EventType.Info, "RechargeUC", "Ingresa a SendData", "OK");
                    SendData();
                }
                else
                {
                    // Mostrar mensaje de error
                    EventLogger.SaveLog(EventType.Error, "RechargeUC", "Validación falló", "Valor inválido");
                    ActivateTimer();
                    await HabilitarVista();
                }
            }
            catch (Exception ex)
            {
                LogError("ProcessContinuar", ex);
                ActivateTimer();
                await HabilitarVista();
            }
        }

        /// <summary>
        /// Procesa la acción de cancelar
        /// </summary>
        private void ProcessCancelar()
        {
            try
            {
                EventLogger.SaveLog(EventType.Info, "RechargeUC", "Ingresando al evento ProcessCancelar", "OK");
                SetCallBacksNull();
                _timer?.Stop();
                Navigator.Instance.NavigateTo(new MenuUC());
            }
            catch (Exception ex)
            {
                LogError("ProcessCancelar", ex);
            }
        }

        /// <summary>
        /// Procesa la acción de ir atrás
        /// </summary>
        private void ProcessAtras()
        {
            try
            {
                EventLogger.SaveLog(EventType.Info, "RechargeUC", "Ingresando al evento ProcessAtras", "OK");
                SetCallBacksNull();
                _timer?.Stop();
                Navigator.Instance.NavigateTo(new MenuUC());
            }
            catch (Exception ex)
            {
                LogError("ProcessAtras", ex);
            }
        }
        #endregion

        #region Métodos de Validación y Envío
        /// <summary>
        /// Valida el monto ingresado
        /// </summary>
        private bool Validate()
        {
            try
            {
                if (!string.IsNullOrEmpty(TxtMonto.Text))
                {
                    string value = TxtMonto.Text.Replace("$", "").Replace(",", "");

                    if (decimal.TryParse(value, out decimal amount))
                    {
                        if (amount >= MONTO_MINIMO && amount <= MONTO_MAXIMO && amount % INCREMENTO_VALIDO == 0)
                        {
                            if (_ts != null)
                            {
                                _ts.Total = amount;
                            }
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LogError("Validate", ex);
                return false;
            }
        }

        /// <summary>
        /// Envía los datos de la transacción
        /// </summary>

        private async Task SendData()
        {
            ModalWindow? loadModal = null;
            try
            {
                loadModal = _nav.ShowLoadModal(Messages.VALIDATING_INFO);
                var tsCreated = await Api.CreateTransaction() ?? throw new Exception("No se pudo enviar la transacción");
                if (loadModal != null)
                {
                    loadModal.Close();
                    loadModal = null;
                }

                ConsultInforBetplay();
            }
            catch (Exception ex)
            {
                if (loadModal != null)
                {
                    loadModal.Close();
                    loadModal = null;
                    EnableView();
                }
                EventLogger.SaveLog(EventType.Error, $"Ocurrió un error en tiempo de ejecución: {ex.Message}", ex);
                _nav.ShowModal("Ocurrió un error generando la transacción, inténtelo nuevamente", new InfoModal());
            }
        }
        /// <summary>
        /// Consulta información de BetPlay
        /// </summary>
        private async void ConsultInforBetplay()
        {
            _nav.ShowModal("Estamos validando los servicios, un momento por favor", new InfoModal());

            try
            {
                // Obtener token
                var requestToken = new RequesttokenBetplay
                {
                    Transaccionclienteid = _ts.IdTransaccionApi.ToString(),
                    Transacciondistribuidorid = _ts.IdTransaccionApi.ToString()
                };

                var respuestaToken = await ApiIntegration.GetTokenBetplay(requestToken);

                if (respuestaToken == null)
                {
                    ShowErrorAndNavigate("Este servicio no se encuentra disponible en este momento, intenta nuevamente.");
                    return;
                }

                _ts.ResponseTokenBetplay = respuestaToken;

                // Consultar subproductos
                var requestConsult = new RequestConsultSubproductBetplay
                {
                    Transacciondistribuidorid = _ts.IdTransaccionApi.ToString(),
                    Token = respuestaToken.Token
                };

                var respuesta = await ApiIntegration.GetConsultBetplay(requestConsult);

                // Log de la respuesta
                var serializedResponse = JsonConvert.SerializeObject(respuesta);
                EventLogger.SaveLog(EventType.Info, "MenuUC", "BtnBetplay - Respuesta al servicio GetConsultBetplay", "OK", serializedResponse);

                CloseLoadModal();

                if (respuesta?.Estado == true)
                {
                    _ts.Type = ETypeTramites.BetPlay;
                    subProductosKiosko = respuesta.Listadosubproductos.Subproducto;
                    _ts.ProductSelected = subProductosKiosko.FirstOrDefault(x => x.Nombre == "RECAUDO BETPLAY");

                    // Uncomment when ready to navigate
                    // Dispatcher.Invoke(() => NavigateTo(new ValidateUC()));
                }
                else if (respuesta != null)
                {
                    ShowErrorAndNavigate("Los servicios de Betplay no están disponibles, intenta nuevamente");
                }
                else
                {
                    ShowErrorAndNavigate("Este servicio no se encuentra disponible en este momento, intenta nuevamente.");
                }
            }
            catch (Exception ex)
            {
                CloseLoadModal();
                ShowErrorAndNavigate("Ocurrió un error generando la transacción, inténtelo nuevamente");
            }
        }

        private void ShowErrorAndNavigate(string message)
        {
            _nav.ShowModal(message, new InfoModal());
            Dispatcher.Invoke(() => NavigateTo(new MenuUC()));
        }
        #endregion

        #region Métodos de Efectos Visuales
        /// <summary>
        /// Aplica el efecto visual de presionar al elemento
        /// </summary>
        private void ApplyPressEffect(FrameworkElement element)
        {
            ScaleElement(element, SCALE_PRESSED);
        }

        /// <summary>
        /// Restaura la escala normal del elemento
        /// </summary>
        private void RestoreElementScale(FrameworkElement element)
        {
            ScaleElement(element, SCALE_NORMAL);
        }

        /// <summary>
        /// Aplica una transformación de escala al elemento
        /// </summary>
        private void ScaleElement(FrameworkElement element, double scale)
        {
            try
            {
                if (!(element.RenderTransform is ScaleTransform transform))
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
                LogError("ScaleElement", ex);
            }
        }
        #endregion

        #region Métodos de Control de Vista
        /// <summary>
        /// Inhabilita la vista de forma asíncrona
        /// </summary>
        private async Task InhabilitarVista()
        {
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                Opacity = 0.3;
                IsEnabled = false;
            }));
        }

        /// <summary>
        /// Habilita la vista de forma asíncrona
        /// </summary>
        private async Task HabilitarVista()
        {
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                Opacity = 1.0;
                IsEnabled = true;
            }));
        }
        #endregion

        #region Métodos de Timer
        /// <summary>
        /// Inicializa el timer del control
        /// </summary>
        private void InitializeTimer()
        {
            try
            {
                _timer = new TimerGeneric(TIMER_INICIAL);

                _timer.Tick += OnTimerTick;
                _timer.TimeOut += OnTimerTimeout;

                tbTimer.Text = TIMER_INICIAL;
                _timer.Start();
            }
            catch (Exception ex)
            {
                LogError("InitializeTimer", ex);
            }
        }

        /// <summary>
        /// Activa el timer
        /// </summary>
        private void ActivateTimer()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    tbTimer.Text = "00:60";
                    _timer = new TimerGeneric(tbTimer.Text);

                    _timer.Tick += OnTimerTick;
                    _timer.TimeOut += OnTimerTimeout;

                    _timer.Start();
                }));
            }
            catch (Exception ex)
            {
                LogError("ActivateTimer", ex);
            }
        }

        /// <summary>
        /// Maneja el evento Tick del timer
        /// </summary>
        private void OnTimerTick(string tiempo)
        {
            Dispatcher.Invoke(() =>
            {
                tbTimer.Text = tiempo;
            });
        }

        /// <summary>
        /// Maneja el evento TimeOut del timer
        /// </summary>
        private void OnTimerTimeout()
        {
            Dispatcher.Invoke(() =>
            {
                tbTimer.Text = "00:00";
                SetCallBacksNull();
                Navigator.Instance.NavigateTo(new MenuUC());
            });
        }

        /// <summary>
        /// Establece los callbacks del timer a null
        /// </summary>
        private void SetCallBacksNull()
        {
            try
            {
                if (_timer != null)
                {
                    _timer.Tick -= OnTimerTick;
                    _timer.TimeOut -= OnTimerTimeout;
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                LogError("SetCallBacksNull", ex);
            }
        }
        #endregion

        #region Métodos de Inicialización y Limpieza
        /// <summary>
        /// Inicializa los componentes adicionales
        /// </summary>
        private void InitializeComponents()
        {
            // Inicialización adicional si es necesaria
        }

        /// <summary>
        /// Limpia los recursos utilizados
        /// </summary>
        private void CleanupResources()
        {
            try
            {
                SetCallBacksNull();

                _timer?.Stop();
                _timer?.Dispose();
                _timer = null;

                _animationTimer?.Stop();
                _animationTimer = null;
            }
            catch (Exception ex)
            {
                LogError("CleanupResources", ex);
            }
        }
        #endregion

        #region Métodos de Utilidad
        /// <summary>
        /// Registra un error en el log
        /// </summary>
        private void LogError(string methodName, Exception ex)
        {
            EventLogger.SaveLog(EventType.Error, nameof(RechargeUC), methodName, ex.Message);
        }

        private void CloseLoadModal()
        {
            if (_currentLoadModal != null)
            {
                _currentLoadModal.Close();
                _currentLoadModal = null;
            }
        }

        #endregion
    }
}