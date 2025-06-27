using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.Enumerables;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Modals;
using SuSuerteV2.Presentation.UserControls.BetPlay;
using SuSuerteV2.UserControls;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SuSuerteV2.Presentation.UserControls
{
    /// <summary>
    /// Lógica de interacción para ElectronicInvoiceUC.xaml
    /// </summary>
    public partial class ElectronicInvoiceUC : AppUserControl
    {
        private const string TIMER_INICIAL = "03:59";
        private const int ANIMATION_DELAY = 150;
        private const double SCALE_PRESSED = 0.95;
        private const double SCALE_NORMAL = 1.0;
        private const decimal MONTO_MINIMO = 2000;
        private const decimal MONTO_MAXIMO = 500000;
        private const decimal INCREMENTO_VALIDO = 100;




        private TimerGeneric _timer;
        private DispatcherTimer _animationTimer;
        private Transaction _ts;
        private ModalWindow? _currentLoadModal = null;
        private List<SubProductoGeneral> subProductosKiosko;
        private ApiIntegration _apiIntegration;

        public ElectronicInvoiceUC()
        {
            InitializeComponent();
            _ts = Transaction.Instance;
            EventLogger.SaveLog(EventType.Info, nameof(ElectronicInvoiceUC), "Inicializando ElectronicInvoiceUC");
            ActivateTimer();
        }

        private void CancelButton(object sender, EventArgs e)
        {
            SetCallBacksNull();
            _timer?.Stop();


            if (_ts.Type == ETypeTramites.Astro)
            {
                EventLogger.SaveLog(EventType.Info,"ElectronicInvoice", " CancelButton Navegando a FinishAstro Tramite Astro", "OK", "");
                //Navigator.Instance.NavigateTo(new FinishAstroUC());
                return;

              
            }

            if (_ts.Type == ETypeTramites.Chance)
            {
                EventLogger.SaveLog(EventType.Info,"ElectronicInvoice", " CancelButton Navegando a Finish Chance Tramite Astro", "OK", "");
                //Navigator.Instance.NavigateTo(new FinishChanceUC());
 
                return;
            }
            //Navigator.Instance.NavigateTo(new SuccesRecaudoUC());
         

        }



        private async void ContinueButton(object sender, EventArgs e)
        {

            InhabilitarVista();


            EventLogger.SaveLog(EventType.Info,"ElectronicInvoice", "Entrando a ContinueButton ", "OK", "");
            if (!IsValid())
            {
               await  HabilitarVista();

                _nav.ShowModal("El correo suministrado no es válido. Intenta nuevamente.", ModalType.Error);
                return;
            }
            EventLogger.SaveLog(EventType.Info,"ElectronicInvoiceUC " + "El correo ingresdo es: " + "  " + Mail.Text.ToString());
            await SendMail(Mail.Text);


        }

        private async Task SendMail(string Mail)
        {
            try
            {
                SetCallBacksNull();
                _timer?.Stop();

                EventLogger.SaveLog(EventType.Info,"ElectronicInvoice", "Entrando a SendMail ", "OK", "" );

                byte[] bytesPDF = GeneratePDF();
                await _apiIntegration.sendMail(Mail, bytesPDF, _ts);
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Info,"SendMail  catch" + ex.Message + " " + ex.StackTrace);

                EventLogger.SaveLog(EventType.Info,"ElectronicInvoice", "Entro al catch del SendMail, por lo cual lo mas probable no se envio el comprobante al correo del usuario", "OK", "");


            }

            if (_ts.Type == ETypeTramites.Recaudos)
            {
                EventLogger.SaveLog(EventType.Info, "ElectronicInvoice", "Navegando a FinishAstro Tramite Astro", "OK", "");
               // Navigator.Instance.NavigateTo(new SuccesRecaudoUC());
                return;
            }


            if (_ts.Type == ETypeTramites.Astro)
            {
                EventLogger.SaveLog(EventType.Info, "ElectronicInvoice", "Navegando a FinishAstro Tramite Astro", "OK", "");
                //Navigator.Instance.NavigateTo(new FinishAstroUC());
            
                return;
            }


            if (_ts.Type == ETypeTramites.BetPlay)
            {
                EventLogger.SaveLog(EventType.Info, "ElectronicInvoice", "Navegando a Finish Tramite Betplay", "OK", "");
               // Navigator.Instance.NavigateTo(new FinishUC());
                return;
            }

            if (_ts.Type == ETypeTramites.RecargasCel || _ts.Type == ETypeTramites.PaquetesCel)
            {
                EventLogger.SaveLog(EventType.Info, "ElectronicInvoice", "Navegando a Finish Tramite Celular", "OK", "");
                //Navigator.Instance.NavigateTo(new FinishPaquetesUC());
                return;
            }


        }

        private async void SendMailChance()
        {
            try
            {
                await SendMail(_ts.UserData.Email); return;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "SendMailChance", ex.Message, ex.StackTrace);
              
            }
        }

        private bool IsValid()
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                        + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<=[a-z0-9])@"
                        + @"(?!\.)([-a-z0-9]+\.)*"
                        + @"[a-z]{2,6}$";

            return Regex.IsMatch(Mail.Text, pattern, RegexOptions.IgnoreCase);

        }

        private void OnFocus(object sender, RoutedEventArgs e)
        {
            var Element = sender as FrameworkElement;
            if (Element == null || !(Element.Parent is Grid parentGrid)) return;
            //  AppConfig.OpenKeyboard(false, (TextBox)sender, this);
            var Label = parentGrid?.Children.OfType<Label>().FirstOrDefault() ?? new Label();
            Label.Visibility = Visibility.Collapsed;

        }
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            var Element = sender as FrameworkElement;
            if (Element == null || !(Element.Parent is Grid parentGrid)) return;
            var Label = parentGrid?.Children.OfType<Label>().FirstOrDefault() ?? new Label();
            if (Element is TextBox textBox && !string.IsNullOrEmpty(textBox.Text)) return;
            Label.Visibility = Visibility.Visible;

        }
        private void Input_changed(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            var placeHolder = textBox.Tag as Label;
            if (placeHolder == null) return;

            placeHolder.Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private byte[] GeneratePDF()
        {

            try
            {

                Dictionary<string, string> Body = new Dictionary<string, string>();

                byte[] bytesPDF;

                if (_ts.Type == ETypeTramites.Recaudos)
                {
                    Body = new Dictionary<string, string>
                {
                    {"Producto:", $"{_ts.Company.Nombre}"},
                    {"Referencia:", $"{_ts.Referencia}"},
                    {"Codigo Seg:", $"{_ts.ResponseNotifyPayment.Codigoseguridad}"},
                    {"ID:", $"{_ts.ResponseNotifyPayment.Transaccionid}"},


                };

                    bytesPDF = Utils.PdfReaders.CreatePdfRecaudos(_ts, Body);

                }
                else if (_ts.Type == ETypeTramites.BetPlay)
                {

                    Body = new Dictionary<string, string>
                    {
                        {"Recaudo para terceros", $"Betplay"},
                         {"Producto:", $"Recarga BetPlay"},
                        {"ID:", $"{_ts.ResponseNotifyBetplay.ClienteId}"},
                        {"Referencia:", $"{_ts.ResponseNotifyBetplay.TransaccionId}"},
                    };

                    bytesPDF = Utils.PdfReaders.CreatePdfBetplay(_ts, Body);

                }
                else if (_ts.Type == ETypeTramites.RecargasCel)
                {

                    Body = new Dictionary<string, string>
                    {
                        {"Recaudo para terceros", $"Recargas " +_ts.Operador.ToString()},
                         {"Producto:", $"Recarga Celular"},
                        {"ID:", $"{_ts.ResponseNotificacionRecargaCel.Transaccionid}"},
                        {"Referencia:", $"{_ts.ResponseNotificacionRecargaCel.Numeroautorizacion}"},
                    };

                    bytesPDF = Utils.PdfReaders.CreatePdfRecargasCel(_ts, Body);

                }
                else if (_ts.Type == ETypeTramites.PaquetesCel)
                {

                    Body = new Dictionary<string, string>
                    {
                        {"Recaudo para terceros", $"Paquetes " +_ts.Operador.ToString()},
                         {"Producto:", $"Paquetes Celular"},
                        {"ID:", $"{_ts.ResponseNotificarPaquetes.Transaccionid}"},
                        {"Referencia:", $"{_ts.IdTransaccionApi}"},
                    };

                    bytesPDF = Utils.PdfReaders.CreatePdfPaquetesCel(_ts, Body);

                }

                else if (_ts.Type == ETypeTramites.Astro)
                {
                    LoadSignos();

                    Body = new Dictionary<string, string>
                    {
                        {"Producto: ", "SUPER astro"},
                        {"Número Apostado: ", _ts.ResponseVentaAstro.Listadodetalles.Detalle.Numeroapostado},
                        {"Loteria jugada: ", _ts.NombreLoteria},
                        {"Signos jugados:", LoadSignos()},
                        {"ID:", $"{_ts.ResponseVentaAstro.Transaccionid}"},
                        {"COD SEG:", $"{_ts.ResponseVentaAstro.Codigoseguridad}"},

                    };

                    bytesPDF = Utils.PdfReaders.CreatePdfAstro(_ts, Body);

                }


                else
                {


                    Body = new Dictionary<string, string>
                    {
                        {"Producto:", $"{_ts.ProductSelected.Nombre}"},
                        {"ID:", $"{_ts.ResponseNotifyChance.Transaccionid}"},
                        {"Referencia:", $"{_ts.IdTransaccionApi}"},
                    };

                    bytesPDF = Utils.PdfReaders.CreatePdfChance(_ts, Body);

                }

                if (bytesPDF == null) throw new Exception("No se logró enviar su comprobante, finaliza la transacción y comunicate con un asesor.");
                return bytesPDF;
            }

            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "GeneratePDF", ex.Message, ex.StackTrace);
          
                return null;
            }


        }
        private string LoadSignos()
        {
            // Verifica si hay signos seleccionados
            if (_ts.DicSginosSeleccionados != null && _ts.DicSginosSeleccionados.Count > 0)
            {
                // Concatena los nombres de los signos seleccionados separados por coma
                return string.Join(", ", _ts.DicSginosSeleccionados.Keys);
            }

            // Devuelve un mensaje si no hay signos seleccionados
            return "";
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
