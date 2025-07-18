using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.Enumerables;
using SuSuerteV2.Domain.Peripherals;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Domain.Variables;
using SuSuerteV2.Modals;
using SuSuerteV2.Presentation.UserControls.Chance;
using SuSuerteV2.UserControls;
using SuSuerteV2.Utils;
using System.ComponentModel;
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
        private ArduinoController _peripherals;
        private const int ANIMATION_DELAY = 150;
        private const double SCALE_PRESSED = 0.95;
        private const double SCALE_NORMAL = 1.0;
        private StateTransaction _tranStateTemp = StateTransaction.Iniciada;


        private ETypeTramites _ETypeTramites;
        private readonly Transaction _ts;
        private ModalWindow? _currentLoadModal = null;
        private TimerGeneric _timer;
        private DispatcherTimer _animationTimer;
        private PaymentViewModel _paymentViewModel;

        private bool _isPayCanceled = false;



        public PaymentBetplayUC()
        {
            InitializeComponent();
            _ts = Transaction.Instance;
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
                    ProcessCancelarAsync();
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
        /// 



        private async Task ProcessCancelarAsync()
        {
            _ = Dispatcher.BeginInvoke(() => BtnCancel.Visibility = Visibility.Collapsed);

            if (!_nav.ShowModal(Messages.CANCEL_TRANSACTION, new ConfirmationModal()))
            {
                _ = Dispatcher.BeginInvoke(() => BtnCancel.Visibility = Visibility.Visible);
                return;
            }
            EventLogger.SaveLog(EventType.Info, "Pago cancelado por el usuario.");
            await CancelPay();
        }

        private async Task CancelPay()
        {
            try
            {
                if (_paymentViewModel.IsPayCompleted) return;
#if NO_PERIPHERALS
#else
                await _peripherals.StopAceptance();
#endif
                _isPayCanceled = true;
                _tranStateTemp = StateTransaction.Cancelada;

                if (_paymentViewModel.EnteredAmount > 0)
                {
                    _paymentViewModel.ReturnAmount = _paymentViewModel.EnteredAmount;
                    _currentLoadModal = _nav.ShowLoadModal("Transacción cancelada. Devolución en curso...");
                    ReturnMoney(_paymentViewModel.EnteredAmount);
                }
                else
                {
                    _currentLoadModal = _nav.ShowLoadModal("Transacción cancelada");
                    _ts.paymentProcess.DevueltaCorrecta = true;
                    await SavePay();
                }

            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Ocurrió un error en tiempo de ejecución: {ex.Message}", ex);
            }
        }

        private async Task SavePay()
        {
            try
            {
                _paymentViewModel.IsPayCompleted = true;

                _ts.paymentProcess.TotalIngresado = _paymentViewModel.EnteredAmount;
                _ts.paymentProcess.TotalDevuelta = _paymentViewModel.DispensedAmount;

                SetTransactionDescription();


                if ((_tranStateTemp == StateTransaction.Aprobada || _tranStateTemp == StateTransaction.Cancelada)
                    && !_ts.paymentProcess.DevueltaCorrecta)
                {
                    // Si el estado de transacción es aprobada o cancelada y además hay error de devuelta se cambia a su respectivo estado
                    // CanceladoErrorDevuelta o AprobadaErrorDevuelta
                    _ts.EstadoTransaccion = (StateTransaction)((int)_tranStateTemp + 2);
                }
                else
                {
                    _ts.EstadoTransaccion = _tranStateTemp;
                }

                Api.UpdateTransaction();

                CloseLoadModal();

                if (_tranStateTemp == StateTransaction.Aprobada)
                {
                    Navigator.Instance.NavigateTo(new ElectronicInvoiceUC());


                    GC.Collect();

                }


            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Ocurrió un error en tiempo de ejecución: {ex.Message}", ex);
                if (!_isPayCanceled)
                {
                    EventLogger.SaveLog(EventType.Info, "Pago cancelado por error guardando el pago");
                    await CancelPay();
                }

                CloseLoadModal();
                _currentLoadModal = _nav.ShowLoadModal("Ocurrió un error fatal intentando reportar los datos del pago. Por favor comuníquese con soporte técnico.");
            }
        }



        private void ReturnMoney(decimal returnValue)
        {
            _ts.paymentProcess.DevueltaCorrecta = false;
#if NO_PERIPHERALS
            OnCashDispensed(returnValue, new Dictionary<int, int>());
#else
            _peripherals.StartDispenser(returnValue);
#endif

        }

        private async void OnCashDispensed(decimal totalDispensed, Dictionary<int, int> details)
        {

            _paymentViewModel.DispensedAmount = totalDispensed;

            _paymentViewModel.RemainingAmount = _paymentViewModel.ReturnAmount - _paymentViewModel.DispensedAmount;
            string strValueToReturn = _paymentViewModel.RemainingAmount.ToString("C0");

            SendDispenseDetails(details);

            CloseLoadModal();

            if (_paymentViewModel.DispensedAmount == _paymentViewModel.ReturnAmount)
            {
                _ts.paymentProcess.DevueltaCorrecta = true;
                await SavePay();
            }
            else
            {
                _currentLoadModal = _nav.ShowLoadModal("No se pudo entregar la totalidad del dinero hay un faltante de:" + $" {strValueToReturn} " + ".Por favor comunícate con un administrador.");
                await Task.Delay(10000); // Timer para mostrar la modal y que se pueda leer
                _ts.paymentProcess.DevueltaCorrecta = false;
                await SavePay();
            }

        }
        private void SendDispenseDetails(Dictionary<int, int> details)
        {

            foreach (var denom in details.Keys)
            {
                var quantity = details[denom];
                for (int i = 0; i < quantity; i++)
                {
                    SendTransactionDetail(TypeOperation.DP, Convert.ToDecimal(denom));
                }
            }
        }
        private void SendRejectDetails(Dictionary<int, int> details)
        {

            foreach (var denom in details.Keys)
            {
                var quantity = details[denom];
                for (int i = 0; i < quantity; i++)
                {
                    SendTransactionDetail(TypeOperation.Reject, Convert.ToDecimal(denom));
                }
            }
        }

        private void SendTransactionDetail(TypeOperation op, decimal denom)
        {
            try
            {
                EventLogger.SaveLog(EventType.Info, $"Enviando detalle a la api: Op: {op}, Denom: {denom.ToString("C0")}");
                Api.CreateTransactionDetail(op, (int)denom);

            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Ocurrió un error en tiempo de ejecución: {ex.Message}", ex);
            }
        }

        private void SetTransactionDescription()
        {
            switch (_tranStateTemp)
            {
                case StateTransaction.Aprobada:
                    _ts.EstadoTransaccionVerb = "Exitoso";
                    _ts.paymentProcess.Descripcion += "Transacción finalizada correctamente. ";
                    break;
                case StateTransaction.Cancelada:
                    _ts.EstadoTransaccionVerb = "Declinada";
                    _ts.paymentProcess.Descripcion += "Transacción Cancelada, No se realizó el pago.";
                    break;
                case StateTransaction.AprobadaSinNotificar:
                    _ts.EstadoTransaccionVerb = "Exitoso";
                    _ts.paymentProcess.Descripcion += "Transacción aprobada, pero no se ha podido notificar el pago a la entidad correspondiente. ";
                    break;
                case StateTransaction.ErrorServicioTercero:
                    _ts.EstadoTransaccionVerb = "Declinada";
                    _ts.paymentProcess.Descripcion += $"Transacción cancelada ocurrió un error en el servicio tercero, No se realizó el pago.";
                    break;
                default:
                    break;
            }

            if (!_ts.paymentProcess.DevueltaCorrecta)
                _ts.paymentProcess.Descripcion += $"Ocurrió un error durante la devolución del dinero. Cantidad faltante {_paymentViewModel.RemainingAmount.ToString("C0")}";
        }

        private async Task FinishSuccessfulPay()
        {
            if (_paymentViewModel.EnteredAmount > 0 && _paymentViewModel.ReturnAmount > 0)
            {

                CloseLoadModal();
                _currentLoadModal = _nav.ShowLoadModal("Pago completado con éxito devolución en curso...");
                await Task.Delay(3000);
                EventLogger.SaveLog(EventType.Info, $"Iniciando devuelta de {_paymentViewModel.ReturnAmount}");
                ReturnMoney(_paymentViewModel.ReturnAmount);
            }
            else
            {
                _ts.paymentProcess.DevueltaCorrecta = true;
                await SavePay();
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
