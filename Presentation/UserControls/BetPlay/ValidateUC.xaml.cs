using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Modals;
using SuSuerteV2.UserControls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SuSuerteV2.Presentation.UserControls.BetPlay
{
    /// <summary>
    /// Lógica de interacción para ValidateUC.xaml
    /// </summary>
    public partial class ValidateUC : AppUserControl
    {

        private const string TIMER_INICIAL = "01:00";
        private const int ANIMATION_DELAY = 150;
        private const double SCALE_PRESSED = 0.95;
        private const double SCALE_NORMAL = 1.0;
        private const decimal MONTO_MINIMO = 2000;
        private const decimal MONTO_MAXIMO = 500000;
        private const decimal INCREMENTO_VALIDO = 100;
        private TimerGeneric _timer;
        private DispatcherTimer _animationTimer;
        private Navigator _nav = Navigator.Instance;
        private Transaction _ts;
        private ModalWindow? _currentLoadModal = null;


        public ValidateUC()
        {
            InitializeComponent();
            InitializeTimer();
            TxtCedula.Text = string.Concat(_ts.Documento.ToString());
            TxtMonto.Text = string.Concat(String.Format("{0:C0}", Convert.ToDecimal(_ts.Total)));
            EventLogger.SaveLog(EventType.Info, "ValidateUC", "Inicializando ValidateUC", "OK");
        }


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
        private void ProcessElementClick(FrameworkElement element)
        {
            try
            {
                string elementName = element.Name ?? "";
                string tag = element.Tag?.ToString() ?? "";


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

            }
        }


        private async void ProcessContinuar()
        {
            try
            {
                SetCallBacksNull();
                _timer?.Stop();
                EventLogger.SaveLog(EventType.Info, "ValidateUC", "Ingresando al evento ProcessContinuar", "OK");
                await InhabilitarVista();

                Navigator.Instance.ShowModal(
                                "Estamos verificando la transacción un momento por favor.",
                                ModalType.Info);


              

                var isValidateMoney = await Api.Validate();
                EventLogger.SaveLog(EventType.Info, "ValidateUC", "Consulta el método ValidateMoney", "OK", isValidateMoney.ToString());

                CloseLoadModal();

                if (isValidateMoney != false)
                {
                    EventLogger.SaveLog(EventType.Info, "ValidateUC", "Navegando a ValidateToken", "OK");
                    ValidateToken();
                }
                else
                {
                    Navigator.Instance.ShowModal(
                                "En estos momentos la máquina no cuenta con suficiente cargue para esta operación",
                                ModalType.Error);


            
                    Navigator.Instance.NavigateTo(new MenuUC());
                }
            }
            catch (Exception ex)
            {
                LogError("ProcessContinuar", ex);
                CloseLoadModal();
                Navigator.Instance.ShowModal(
                              "En estos momentos los servicios de Betplay no están disponibles",
                              ModalType.Error);


        
                Navigator.Instance.NavigateTo(new MenuUC());
            }
        }


        private async void ValidateToken()
        {

            try
            {

                string modifiedId = "1" + _ts.IdTransaccionApi.ToString();
                _ts.Transacciondistribuidorid = ulong.Parse(modifiedId);


                RequesttokenBetplay requesttokenBetplay = new RequesttokenBetplay();

                requesttokenBetplay.Transaccionclienteid = modifiedId;
                requesttokenBetplay.Transacciondistribuidorid = modifiedId;

                var RespuestaToken = await ApiIntegration.GetTokenBetplay(requesttokenBetplay);

                if (RespuestaToken != null)
                {

                    _ts.ResponseTokenBetplay = RespuestaToken;

                    //Navigator.Instance.NavigateTo(new PaymentBetplayUC());

                  

                }
                else
                {
                    Navigator.Instance.ShowModal(
                         "Este servicio no se encuentra disponible en este momento, intenta nuevamente",
                         ModalType.Error);



                    Navigator.Instance.NavigateTo(new MenuUC());
                }

            }
            catch (Exception ex)
            {
                CloseLoadModal();

                Navigator.Instance.ShowModal(
                     "En estos momentos los servicios de Betplay no estan disponibles, intenta nuevamente",
                     ModalType.Error);



                Navigator.Instance.NavigateTo(new MenuUC());


            }


        }

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
                    tbTimer.Text = "00:00";
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

