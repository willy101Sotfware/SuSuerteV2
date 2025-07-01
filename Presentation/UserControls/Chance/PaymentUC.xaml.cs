using Newtonsoft.Json.Linq;
using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.Enumerables;
using SuSuerteV2.Domain.Peripherals;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Domain.Variables;
using SuSuerteV2.Modals;
using SuSuerteV2.UserControls;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SuSuerteV2.Presentation.UserControls.Chance
{
    public partial class PaymentUC : AppUserControl
    {
        private Transaction _ts;
        private ArduinoController _peripherals;
        private PaymentViewModel _paymentViewModel;

        private bool _isPayCanceled = false;

        private ModalWindow? _currentLoadModal;

        private StateTransaction _tranStateTemp = StateTransaction.Iniciada;
        public PaymentUC()
        {
            InitializeComponent();

            EventLogger.SaveLog(EventType.Info, "Comienza proceso de pago, Iniciando PaymentUC.");


            _ts = Transaction.Instance;
            _ts.paymentProcess.DevueltaCorrecta = false;

#if NO_PERIPHERALS
            Button dynamicButton = new Button();

            // Set properties of the button
            dynamicButton.Content = "Add minor value";
            dynamicButton.Width = 100;
            dynamicButton.Height = 50;
            dynamicButton.VerticalAlignment = VerticalAlignment.Top;
            dynamicButton.HorizontalAlignment = HorizontalAlignment.Left;
            // Set background color
            dynamicButton.Background = new SolidColorBrush(Colors.Black); // Change to the desired color
            dynamicButton.Foreground = new SolidColorBrush(Colors.Blue); // Change to the desired color

            // Set border brush and thickness
            dynamicButton.BorderBrush = new SolidColorBrush(Colors.Black); // Change to the desired color
            dynamicButton.BorderThickness = new Thickness(2); // Change thickness as needed
            dynamicButton.Click += ExecuteScanner;

            Button dynamicButton2 = new Button();

            // Set properties of the button
            dynamicButton2.Content = "Add mid value";
            dynamicButton2.Width = 100;
            dynamicButton2.Height = 50;
            dynamicButton2.VerticalAlignment = VerticalAlignment.Top;
            dynamicButton2.HorizontalAlignment = HorizontalAlignment.Right;
            // Set background color
            dynamicButton2.Background = new SolidColorBrush(Colors.Black); // Change to the desired color
            dynamicButton2.Foreground = new SolidColorBrush(Colors.Blue); // Change to the desired color

            // Set border brush and thickness
            dynamicButton2.BorderBrush = new SolidColorBrush(Colors.Black); // Change to the desired color
            dynamicButton2.BorderThickness = new Thickness(2); // Change thickness as needed
            dynamicButton2.Click += ExecuteScanner2;

            void ExecuteScanner(object sender, EventArgs e)
            {
                OnCashIn(2000);
            }

            void ExecuteScanner2(object sender, EventArgs e)
            {
                OnCashIn(5000);
            }
            MainGrid.Children.Add(dynamicButton);
            MainGrid.Children.Add(dynamicButton2);
#else
            _peripherals = ArduinoController.Instance;
            _peripherals.CashIn += OnCashIn;
            _peripherals.CashDispensed += OnCashDispensed;
            _peripherals.DispenserReject += OnDispenserReject;
            _peripherals.PeripheralError += OnPeripheralError;
#endif


            Unloaded += OnUnloaded;
            Loaded += OnLoaded;

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

            InitViewModel();
#if NO_PERIPHERALS
#else
            _peripherals.StartAcceptance(_paymentViewModel.PayAmount);
#endif
        }
        private void InitViewModel()
        {

            _paymentViewModel = new PaymentViewModel
            {
                PayAmount = _ts.paymentProcess.Total,
                RemainingAmount = _ts.paymentProcess.Total,
                ReturnAmount = 0,
                EnteredAmount = 0,
                Denominations = new List<Denomination>(),
                DispensedAmount = 0
            };
            DataView.DataContext = _paymentViewModel;
            DataContext = _paymentViewModel;
            DataView.ItemsSource = _paymentViewModel.Denominations;

        }


        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
#if NO_PERIPHERALS
#else
            _peripherals.CashIn -= OnCashIn;
            _peripherals.CashDispensed -= OnCashDispensed;
            _peripherals.DispenserReject -= OnDispenserReject;
            _peripherals.PeripheralError -= OnPeripheralError;
#endif
        }


        #region Responses to Peripheral Events
        private async void OnCashIn(decimal value)
        {

            if (_paymentViewModel.IsPayCompleted) return;

            _paymentViewModel.EnteredAmount += value;

            _paymentViewModel.RefreshAmountsList(Convert.ToInt32(value), 1);

            SendTransactionDetail(TypeOperation.AP, value);

            _ = RefreshView(); // Se refresca la vista asincronamente

            if (_paymentViewModel.EnteredAmount < _paymentViewModel.PayAmount) return;

            //Finaliza pago cantidad completa
            _ = Dispatcher.BeginInvoke(() => cancelar.Visibility = Visibility.Collapsed);
#if NO_PERIPHERALS
#else
            await _peripherals.StopAceptance();
#endif

            _currentLoadModal = _nav.ShowLoadModal("Estamos procesando el pago...");
            await Task.Delay(3000);
            EventLogger.SaveLog(EventType.Info, "Iniciando Proceso de pago...");

            await PaymentProcess();
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

        private void OnDispenserReject(Dictionary<int, int> rejectData)
        {
            // Se registra el reject en la api
            SendRejectDetails(rejectData);

        }

        private void OnPeripheralError(Exception ex)
        {
            //TODO: Evaluar Si es necesario reportar errores de perifericos al Dashboard por que ya los errores de perifericos se reportan internamente
        }
        #endregion

        #region UI control methods
        private async Task RefreshView()
        {
            await Dispatcher.BeginInvoke(() =>
            {
                DataView.Items.Refresh();
            });
        }
        private void CloseLoadModal()
        {
            if (_currentLoadModal != null)
            {
                Dispatcher.Invoke(() =>
                {
                    _currentLoadModal.Close();
                    _currentLoadModal = null;
                });
            }
        }
        private async void Btn_CancelarTouchDown(object sender, EventArgs e)
        {
            _ = Dispatcher.BeginInvoke(() => cancelar.Visibility = Visibility.Collapsed);

            if (!_nav.ShowModal(Messages.CANCEL_TRANSACTION, new ConfirmationModal()))
            {
                _ = Dispatcher.BeginInvoke(() => cancelar.Visibility = Visibility.Visible);
                return;
            }
            EventLogger.SaveLog(EventType.Info, "Pago cancelado por el usuario.");
            await CancelPay();
        }

        #endregion

        #region Internal Operation process
        private async Task PaymentProcess()
        {


            NotifyChance();
            

        }






        public async Task NotifyChance()
        {
            try
            {
                bool isNotificationSuccess = false;

                // Preparación del request (parte específica de Chance)
                var request = new RequestNotifyChance
                {
                    Subproducto = new SubproductoNotify
                    {
                        CodigoServicio = _ts.ProductSelected.Codigoservicio,
                        Id = _ts.ProductSelected.Id
                    },
                    LstApuestas = new ListApuestasNotify { apuestas = new List<ApuestasNotify>() },
                    AsumeIva = false,
                    FechaSorteo = _ts.Fecha,
                    Transaccion = _ts.IdTransaccionApi,
                    codigoApostar = AppConfig.Get("CodData"),
                    idPagador = _ts.IdUser.ToString(),
                    cedula = _ts.paymentProcess.Documento
                };

                // Llenado de apuestas (tu lógica original de Chance)
                int lotId = 1;
                foreach (var chance in _ts.ListaChances)
                {
                    var apuesta = new ApuestasNotify
                    {
                        id = lotId++,
                        NumeroApostado = chance.Numero,
                        ValorDirecto = chance.Directo,
                        ValorCombinado = chance.Combinado,
                        ValorPata = chance.Pata,
                        ValorUna = chance.Una,
                        tipoChance = new TipoChanceNotifyM { Id = chance.TipoChance },
                        ListLoteriasValidate = new ListLoteriasNotify
                        {
                            loteria = chance.Loterias.Select(l => new LoteriaNotify { codigo = l.CodigoCodesa }).ToList()
                        }
                    };
                    request.LstApuestas.apuestas.Add(apuesta);
                }

                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    // Ejecución de la notificación
                    var respuesta = await ApiIntegration.NotifyChance(request);

                    if (respuesta == null || !respuesta.Estado)
                    {
                        _nav.CloseModal();
                        isNotificationSuccess = false;
                        _ts.EstadoTransaccion = StateTransaction.ErrorServicioTercero;
                        EventLogger.SaveLog(EventType.Error, "Error en notificación de chance", _ts);
                        _nav.ShowModal("No se pudo notificar el chance. Se realizará la devolución.", ModalType.Error);
                        await CancelPay();
                        return;
                    }

                    // Notificación exitosa
                    _ts.ResponseNotifyChance = respuesta;
                    _ts.EstadoTransaccion = StateTransaction.Aprobada;
                    EventLogger.SaveLog(EventType.Info, "Notificación de chance exitosa", _ts);
                    isNotificationSuccess = true;

                    if (isNotificationSuccess)
                    {
                        await FinishSuccessfulPay();
                        return;
                    }
                });
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en NotifyChance: {ex.Message}", _ts);
                _nav.ShowModal("Error crítico al notificar el chance", ModalType.Error);
                await CancelPay();
            }
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


        private void InhabilitarVista()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                Opacity = 0.3;
                IsHitTestVisible = false;
            });
        }


  

        private async Task SavePay()
        {
            try
            {
                _paymentViewModel.IsPayCompleted = true;
                _ts.DatosPago = _paymentViewModel;
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

                
                   _nav.NavigateTo (new ConfigUC());
                



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
        #endregion

        #region HTTP API Consume
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
                    _ts.paymentProcess.Documento += "Transacción aprobada, pero no se ha podido notificar el pago a la entidad correspondiente. ";
                    break;
                case StateTransaction.ErrorServicioTercero:
                    _ts.EstadoTransaccionVerb = "Declinada";
                    _ts.paymentProcess.Documento += $"Transacción cancelada ocurrió un error en el servicio tercero, No se realizó el pago.";
                    break;
                default:
                    break;
            }

            if (!_ts.paymentProcess.DevueltaCorrecta)
                _ts.paymentProcess.Documento += $"Ocurrió un error durante la devolución del dinero. Cantidad faltante {_paymentViewModel.RemainingAmount.ToString("C0")}";
        }

        #endregion


    }

    public class PaymentViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;


        #region Attributes
        private decimal _payAmount;

        private decimal _enteredAmount;

        private decimal _remainingAmount;

        private decimal _returnAmount;

        public decimal _dispensedAmount;

        public bool _isReturnSuccess;

        private List<Denomination> _denominations;


        public List<Denomination> Denominations
        {
            get { return _denominations; }
            set
            {
                _denominations = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Denominations)));
            }
        }

        public decimal PayAmount
        {
            get { return _payAmount; }
            set
            {
                if (_payAmount != value)
                {
                    _payAmount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PayAmount)));
                }
            }
        }


        public decimal EnteredAmount
        {
            get { return _enteredAmount; }
            set
            {
                if (_enteredAmount != value)
                {
                    _enteredAmount = value;
                    RemainingAmount = EnteredAmount < PayAmount ? PayAmount - EnteredAmount : 0;
                    ReturnAmount = EnteredAmount > PayAmount ? EnteredAmount - PayAmount : 0;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnteredAmount)));
                }
            }
        }

        public decimal RemainingAmount
        {
            get { return _remainingAmount; }
            set
            {
                if (_remainingAmount != value)
                {
                    _remainingAmount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemainingAmount)));
                }
            }
        }

        public decimal ReturnAmount
        {
            get { return _returnAmount; }
            set
            {
                if (_returnAmount != value)
                {
                    _returnAmount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReturnAmount)));
                }
            }
        }

        public decimal DispensedAmount
        {
            get { return _dispensedAmount; }
            set
            {
                if (_dispensedAmount != value)
                {
                    _dispensedAmount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DispensedAmount)));
                }
            }
        }

        public bool IsPayCompleted
        {
            get { return _isReturnSuccess; }
            set
            {
                if (_isReturnSuccess != value)
                {
                    _isReturnSuccess = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPayCompleted)));
                }
            }
        }

        #endregion

        #region Methods
        public void RefreshAmountsList(int denomination, int quantity)
        {

            var itemDenomination = Denominations.Where(d => d.DenominationValue == denomination).FirstOrDefault();
            if (itemDenomination == null)
            {
                Denominations.Add(new Denomination
                {
                    DenominationValue = denomination,
                    Quantity = quantity,
                    TotalDenomAmount = denomination * quantity,
                });
                return;
            }

            itemDenomination.Quantity += quantity;
            itemDenomination.TotalDenomAmount = denomination * itemDenomination.Quantity;
        }

        #endregion
    }

    public class Denomination
    {
        public decimal DenominationValue { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalDenomAmount { get; set; }
    }
}
