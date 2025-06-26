using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Modals;
using SuSuerteV2.Presentation.UserControls.BetPlay;
using SuSuerteV2.UserControls;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SuSuerteV2.Presentation.UserControls.Astro
{
    /// <summary>
    /// Lógica de interacción para LoteriasAstroUC.xaml
    /// </summary>
    public partial class LoteriasAstroUC : AppUserControl
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
        private DateTime _birthDateUser;
        private ApiIntegration _apiIntegration;
        private ModalWindow? _currentLoadModal = null;
        bool solDisponible = false;
        bool lunaDisponible = false;

        public LoteriasAstroUC()
        {
            InitializeComponent();
            _ts = Transaction.Instance;
            ValidarEstadoLoteria();
            ActivateTimer();
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


        private void ValidarEstadoLoteria()
        {


            foreach (var nombre in _ts.ResponseConsultarAstro.Listadosorteos.Sorteo)
            {
                if (nombre.Descripcion == "ASTRO SOL")
                {
                    solDisponible = true;

                }
                else if (nombre.Descripcion == "ASTRO LUNA")
                {
                    lunaDisponible = true;
                }

            }

            if (solDisponible != true)
            {
                Sol.IsEnabled = false;
                Sol.Opacity = 0.5;
                loteriasol.Visibility = Visibility;
            }

            if (lunaDisponible != true)
            {
                Luna.IsEnabled = false;
                Luna.Opacity = 0.5;
                loteriaLuna.Visibility = Visibility;
            }

        }

        private void SeleccionarOpcion(object sender, EventArgs e)
        {
            if (!(sender is Image image) || image.Tag == null)
                return;

            string tag = image.Tag.ToString();
            string descripcionLoteria = "";
            string codigoLoteria = "";

            switch (tag)
            {
                case "Sol":
                    descripcionLoteria = "ASTRO SOL";
                    break;
                case "Luna":
                    descripcionLoteria = "ASTRO LUNA";
                    break;
                default:
                    return;
            }

            // Buscar la lotería correspondiente
            var loteria = _ts.ResponseConsultarAstro.Listadosorteos.Sorteo
                .FirstOrDefault(s => s.Descripcion == descripcionLoteria);

            if (loteria != null)
            {
                _ts.NombreLoteria = loteria.Descripcion;
                _ts.CodigoLoteria = loteria.Codigoloteria;
            }

            GetTypeSigno(tag);
        }

        public void GetTypeSigno(string tag)
        {
            try
            {

                SetCallBacksNull();
                _timer?.Stop();

                RequestConsultarSorteo Request = new RequestConsultarSorteo();

                Request.Transacciondistribuidorid = _ts.IdTransaccionApi.ToString();



                Task.Run(async () =>
                {

                    var Response = await _apiIntegration.ConsultarSigno(Request);

                    CloseLoadModal();
                

                    if (Response != null)
                    {
                        if (Response.Estado)
                        {
                            _ts.ResponseConsultarSignos = Response;
                            Navigator.Instance.NavigateTo(new SignosAstroUC());
                        

                        }
                        else
                        {
                            _nav.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles, intenta nuevamente", ModalType.Error);
                            Navigator.Instance.NavigateTo(new MenuUC());
                        }
                    }
                    else
                    {
                        _nav.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles,intenta nuevamente", ModalType.Error);
                        Navigator.Instance.NavigateTo(new MenuUC());
                    }


                });

                _nav.ShowModal("Estamos validando información un momento por favor", ModalType.Loading);


            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, nameof(LoteriasAstroUC), "GetTypeSigno", ex.Message);

                CloseLoadModal();
                _nav.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles, intenta nuevamente", ModalType.Error);
                Navigator.Instance.NavigateTo(new MenuUC());
            }
        }
        private void Btn_AtrasTouchDown(object sender, EventArgs e)
        {
            EventLogger.SaveLog(EventType.Info, nameof(LoteriasAstroUC), "Btn_AtrasTouchDown", "Ingresando al evento Btn_AtrasTouchDown");
            SetCallBacksNull();
            _timer?.Stop();
            Navigator.Instance.NavigateTo(new MenuUC());
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
