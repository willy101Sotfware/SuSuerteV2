using SuSuerteV2.Domain;
using SuSuerteV2.Domain.Enumerables;
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
    /// Lógica de interacción para PaymentBetplayUC.xaml
    /// </summary>
    public partial class PaymentBetplayUC : AppUserControl
    {
      
        private const int ANIMATION_DELAY = 150;
        private const double SCALE_PRESSED = 0.95;
        private const double SCALE_NORMAL = 1.0;
        private const decimal MONTO_MINIMO = 2000;
        private const decimal MONTO_MAXIMO = 500000;
        private const decimal INCREMENTO_VALIDO = 100;


        private ETypeTramites _ETypeTramites;
        private Transaction _transaction;
        private ModalWindow? _currentLoadModal = null;
        private TimerGeneric _timer;
        private DispatcherTimer _animationTimer;
        public PaymentBetplayUC()
        {
            InitializeComponent();
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
        private void ProcessByName(string elementName)
        {
            switch (elementName)
            {
         
                case "btnCancelar":
                    ProcessCancelar();
                    break;

            
            }
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

    }
}
