using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Modals;
using SuSuerteV2.Presentation.UserControls.BetPlay;
using SuSuerteV2.UserControls;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;


namespace SuSuerteV2.Presentation.UserControls.Astro
{
    /// <summary>
    /// Lógica de interacción para SignosAstroUC.xaml
    /// </summary>
    public partial class SignosAstroUC : AppUserControl
    {
        private DispatcherTimer _animationTimer;
        private Transaction _ts;
        private TimerGeneric _timer;
        private ApiIntegration _apiIntegration;
        private ModalWindow? _currentLoadModal = null;


        public SignosAstroUC()
        {
            _ts = Transaction.Instance;
            InitializeComponent();
            ActivateTimer();
        }



        private async void Btn_ContinuarTouchDown(object sender, EventArgs e)
        {
            SetCallBacksNull();
            _timer?.Stop();
            EventLogger.SaveLog(EventType.Info, nameof(SignosAstroUC), "Btn_ContinuarTouchDown", "Inicia el proceso de continuar");
         
            _nav.NavigateTo(new ApuestaAstroUC());


        }


        private void Btn_CancelarTouchDown(object sender, EventArgs e)
        {

            SetCallBacksNull();
            _timer?.Stop();
            EventLogger.SaveLog(EventType.Info, nameof(SignosAstroUC), "Btn_CancelarTouchDown", "Ingresando al evento Btn_CancelarTouchDown");
            _nav.NavigateTo(new MenuUC());
        }

        private void Btn_AtrasTouchDown(object sender, EventArgs e)
        {
            EventLogger.SaveLog(EventType.Info, nameof(SignosAstroUC), "Btn_AtrasTouchDown", "Ingresando al evento Btn_AtrasTouchDown");
            SetCallBacksNull();
            _timer?.Stop();
            _nav.NavigateTo(new MenuUC());

        }

        private async void ImageSelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;

            if (image == null || image.Tag == null)
                return;

            string tag = image.Tag.ToString();

            switch (tag)
            {
                case "aries":
                    await Validarsignoselect(tag, 1, ariesselect);
                    break;

                case "tauro":
                    await Validarsignoselect(tag, 2, tauroselect);
                    break;

                case "geminis":
                    await Validarsignoselect(tag, 3, geminisselect);
                    break;
                case "cancer":
                    await Validarsignoselect(tag, 4, cancerselect);
                    break;
                case "leo":
                    await Validarsignoselect(tag, 5, leoselect);
                    break;
                case "virgo":
                    await Validarsignoselect(tag, 6, virgoselect);
                    break;
                case "libra":
                    await Validarsignoselect(tag, 7, libraselect);
                    break;
                case "escorpion":
                    await Validarsignoselect(tag, 8, escorpionselect);
                    break;
                case "sagitario":
                    await Validarsignoselect(tag, 9, sagitarioselect);
                    break;
                case "capricornio":
                    await Validarsignoselect(tag, 10, capricornioselect);
                    break;
                case "acuario":
                    await Validarsignoselect(tag, 11, acuarioselect);
                    break;
                case "piscis":
                    await Validarsignoselect(tag, 12, piscisselect);
                    break;

                default:



                    break;


                    // Agrega más casos según sea necesario
            }
        }


        private async Task<bool> Validarsignoselect(string tag, int id, UIElement signoselect)
        {
            try
            {

                if (signoselect.Visibility == Visibility.Hidden)
                {
                    signoselect.Visibility = Visibility.Visible;

                    _ts.DicSginosSeleccionados[tag] = id;
                    return true;
                }
                else
                {
                    signoselect.Visibility = Visibility.Hidden;

                    // Eliminar el ID del diccionario según el horario
                    if (_ts.DicSginosSeleccionados.ContainsKey(tag))
                    {
                        _ts.DicSginosSeleccionados.Remove(tag);
                    }

                    return false;
                }

            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, nameof(SignosAstroUC), "Validarsignoselect", ex.Message + " " + ex.StackTrace);
                _nav.NavigateTo(new MenuUC());
                return false;
         
            }




        }


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
