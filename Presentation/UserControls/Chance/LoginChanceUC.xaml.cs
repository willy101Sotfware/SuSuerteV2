using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.Enumerables;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Modals;
using SuSuerteV2.Presentation.UserControls.BetPlay;
using SuSuerteV2.UserControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SuSuerteV2.Presentation.UserControls.Chance
{
 

    /// <summary>
    /// Lógica de interacción para LoginChanceUC.xaml
    /// </summary>
    public partial class LoginChanceUC : AppUserControl
    {
        private const string TIMER_INICIAL = "01:00";
        private const int ANIMATION_DELAY = 150;
        private const double SCALE_PRESSED = 0.95;
        private const double SCALE_NORMAL = 1.0;
        private const decimal MONTO_MINIMO = 2000;
        private const decimal MONTO_MAXIMO = 500000;
        private const decimal INCREMENTO_VALIDO = 100;

        private DispatcherTimer _animationTimer;
        private Transaction _ts;
        private TimerGeneric _timer;
        private ModalWindow? _currentLoadModal = null;
        private ApiIntegration _apiIntegration;

        public LoginChanceUC()
        {
            InitializeComponent();
            InitializeTimer();
            _ts = Transaction.Instance;

        }

        private void UIElement_Interaction(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                // Identificar el elemento por su Source (Image) o Tag
                string elementId = "";

                if (element is Image img && img.Source != null)
                {
                    elementId = img.Source.ToString();
                }
                else if (element.Tag != null)
                {
                    elementId = element.Tag.ToString();
                }

                switch (elementId)
                {
                    case var source when source.Contains("btnatras.png"):
                        BtnAtras_Click();
                        break;

                    case var source when source.Contains("btnCancelar.png"):
                        BtnCancelar_Click();
                        break;

                    case var source when source.Contains("btnContinuar.png"):
                        BtnContinuar_Click();
                        break;

                    case "9": // Tag del botón Editar
                        BtnEditar_Click();
                        break;

                    default:
                        // Elemento no reconocido
                        break;
                }
            }
        }

        private void BtnAtras_Click()
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

        private void BtnCancelar_Click()
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

        private void BtnContinuar_Click()
        {
            InhabilitarVista();

            SetCallBacksNull();
            _timer.Stop();  
            ValidateUpdateData();
        }

        private void BtnEditar_Click()
        {
            // Lógica para botón Editar
        }

        public void ValidateUpdateData()
        {
       
            try
            {



                if (_ts.ResponseConsultarCRMRegistro.CELULAR != txtCelularForm.Text || _ts.ResponseConsultarCRMRegistro.EMAIL != txtEmailForm.Text)
                {

                    RequestUpdateCrmRegistro request = new RequestUpdateCrmRegistro();

                    request.IDENTIFICACION = _ts.ResponseConsultarCRMRegistro.IDENTIFICACION;
                    request.TIPO_IDENTIFICACION = _ts.ResponseConsultarCRMRegistro.TIPO_IDENTIFICACION;
                    request.NOMBRES = _ts.ResponseConsultarCRMRegistro.NOMBRES;
                    request.APELLIDOS = _ts.ResponseConsultarCRMRegistro.APELLIDOS;
                    request.CELULAR = txtCelularForm.Text;
                    request.EMAIL = txtEmailForm.Text;



                    Task.Run(async () =>
                    {
                     
                        var Response = await _apiIntegration.UpdateRegistroCRM(request);

                        CloseLoadModal();

                        if (Response != null)
                        {

                            if (_ts.Type == ETypeTramites.SuperChance)
                            {

                                GetTypeChance();
                            }
                            else if (_ts.Type == ETypeTramites.Astro)
                            {
                                GetTypeAstro();
                            }
                        }

                        else
                        {
                            _nav.ShowModal("no se pudo actualizar los datos de manerra correcta, intenta nuevamente",ModalType.Warning);
                            Navigator.Instance.NavigateTo(new MenuUC());
                        }

                    });

                    _nav.ShowModal("Estamos actualizando tus datos, un momento por favor", ModalType.Loading);

                }
                else
                {

                    if (_ts.Type == ETypeTramites.SuperChance)
                    {

                        GetTypeChance();
                    }
                    else if (_ts.Type == ETypeTramites.Astro)
                    {
                        GetTypeAstro();
                    }

                }

            }
            catch (Exception ex)
            {
                CloseLoadModal();
             
                _nav.ShowModal("no se pudo actualizar los datos de manerra correcta, intenta nuevamente", ModalType.Error );
                Navigator.Instance.NavigateTo(new MenuUC());
            }

        }

        private void GetTypeAstro()
        {
            throw new NotImplementedException();
        }

        private void GetTypeChance()
        {
            throw new NotImplementedException();
        }




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


        private void InhabilitarVista()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 0.3;
                //this.btnChance.IsEnabled = false;
                //this.btnRecaudos.IsEnabled = false;
                this.IsEnabled = false;
            });
        }

        private void HabilitarVista()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 1;
                //this.btnChance.IsEnabled = true;
                //this.btnRecaudos.IsEnabled = true;
                this.IsEnabled = true;
            });
        }


    }
}
