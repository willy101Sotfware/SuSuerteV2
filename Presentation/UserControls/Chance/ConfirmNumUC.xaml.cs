using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.Enumerables;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Modals;
using SuSuerteV2.UserControls;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows.Data;
using static SuSuerteV2.Domain.UIServices.Transaction;

namespace SuSuerteV2.Presentation.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para ConfirmNumUC.xaml
    /// </summary>
    public partial class ConfirmNumUC : AppUserControl
    {
        private const string TIMER_INICIAL = "00:59";
        private const int ANIMATION_DELAY = 150;
        private const double SCALE_PRESSED = 0.95;
        private const double SCALE_NORMAL = 1.0;
        private const decimal MONTO_MINIMO = 2000;
        private const decimal MONTO_MAXIMO = 500000;
        private const decimal INCREMENTO_VALIDO = 100;

        private TimerGeneric? _timer;
        private Transaction _ts;
        private ObservableCollection<LotteriesViewModel> lstLotteriesModel;
        private CollectionViewSource view = new CollectionViewSource();
        private ApiIntegration _apiIntegration;
        public ConfirmNumUC()
        {
            InitializeComponent();
            _ts = Transaction.Instance;
            view = new CollectionViewSource();
            lstLotteriesModel = new ObservableCollection<LotteriesViewModel>();
            ActivateTimer();
            GenerateTotal();
            CargarNumeros();
            LoadLotteries();
        }

      

  

        public void GenerateTotal()
        {
            try
            {

                int valor = 0;
                double iva = 0;

                foreach (var x in _ts.ListaChances)
                {
                    valor += x.GetTotalChance();
                }

                iva = valor * 0.19;

                _ts.Total = valor;

                int ValorTL = (int)(Convert.ToInt32(_ts.Total) - iva);

                Iva.Content = string.Format("{0:C0}", Convert.ToDecimal(iva));
                ValorT.Content = string.Format("{0:C0}", Convert.ToDecimal(_ts.Total));
                Valor.Content = string.Format("{0:C0}", Convert.ToDecimal(ValorTL));

            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error al generar el total: " + ex.Message, ex);
           
            }
        }

        public void LoadLotteries()
        {
            try
            {
                string loteriasPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "Loterias");

                foreach (var loteria in _ts.ListaLoteriasSeleccionadas)
                {
                    lstLotteriesModel.Add(new LotteriesViewModel
                    {
                        ImageData = Utils.LoadImages.LoadImageFromFile(new Uri(Path.Combine(loteriasPath, loteria.Nombre + ".png"), UriKind.Relative))
                    });
                }

                view.Source = lstLotteriesModel;
                this.DataContext = view;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Info, "ConfirmNumUC LoadLotteries Catch: " + ex.Message, ex);
           
            }
        }

        public void CargarNumeros()
        {

            try
            {

                switch (_ts.ListaChances.Count)
                {

                    case 1:

                        Num1.Text = _ts.ListaChances[0].Numero;
                        Directo1.Text = _ts.ListaChances[0].Directo.ToString("C0");
                        Combinado1.Text = _ts.ListaChances[0].Combinado.ToString("C0");
                        Pata1.Text = _ts.ListaChances[0].Pata.ToString("C0");
                        Una1.Text = _ts.ListaChances[0].Una.ToString("C0");

                        break;
                    case 2:
                        Num1.Text = _ts.ListaChances[0].Numero;
                        Directo1.Text = _ts.ListaChances[0].Directo.ToString("C0");
                        Combinado1.Text = _ts.ListaChances[0].Combinado.ToString("C0");
                        Pata1.Text = _ts.ListaChances[0].Pata.ToString("C0");
                        Una1.Text = _ts.ListaChances[0].Una.ToString("C0");

                        Num2.Text = _ts.ListaChances[1].Numero;
                        Directo2.Text = _ts.ListaChances[1].Directo.ToString("C0");
                        Combinado2.Text = _ts.ListaChances[1].Combinado.ToString("C0");
                        Pata2.Text = _ts.ListaChances[1].Pata.ToString("C0");
                        Una2.Text = _ts.ListaChances[1].Una.ToString("C0");
                        break;
                    case 3:
                        Num1.Text = _ts.ListaChances[0].Numero;
                        Directo1.Text = _ts.ListaChances[0].Directo.ToString("C0");
                        Combinado1.Text = _ts.ListaChances[0].Combinado.ToString("C0");
                        Pata1.Text = _ts.ListaChances[0].Pata.ToString("C0");
                        Una1.Text = _ts.ListaChances[0].Una.ToString("C0");

                        Num2.Text = _ts.ListaChances[1].Numero;
                        Directo2.Text = _ts.ListaChances[1].Directo.ToString("C0");
                        Combinado2.Text = _ts.ListaChances[1].Combinado.ToString("C0");
                        Pata2.Text = _ts.ListaChances[1].Pata.ToString("C0");
                        Una2.Text = _ts.ListaChances[1].Una.ToString("C0");

                        Num3.Text = _ts.ListaChances[2].Numero;
                        Directo3.Text = _ts.ListaChances[2].Directo.ToString("C0");
                        Combinado3.Text = _ts.ListaChances[2].Combinado.ToString("C0");
                        Pata3.Text = _ts.ListaChances[2].Pata.ToString("C0");
                        Una3.Text = _ts.ListaChances[2].Una.ToString("C0");

                        break;
                    case 4:
                        Num1.Text = _ts.ListaChances[0].Numero;
                        Directo1.Text = _ts.ListaChances[0].Directo.ToString("C0");
                        Combinado1.Text = _ts.ListaChances[0].Combinado.ToString("C0");
                        Pata1.Text = _ts.ListaChances[0].Pata.ToString("C0");
                        Una1.Text = _ts.ListaChances[0].Una.ToString("C0");

                        Num2.Text = _ts.ListaChances[1].Numero;
                        Directo2.Text = _ts.ListaChances[1].Directo.ToString("C0");
                        Combinado2.Text = _ts.ListaChances[1].Combinado.ToString("C0");
                        Pata2.Text = _ts.ListaChances[1].Pata.ToString("C0");
                        Una2.Text = _ts.ListaChances[1].Una.ToString("C0");

                        Num3.Text = _ts.ListaChances[2].Numero;
                        Directo3.Text = _ts.ListaChances[2].Directo.ToString("C0");
                        Combinado3.Text = _ts.ListaChances[2].Combinado.ToString("C0");
                        Pata3.Text = _ts.ListaChances[2].Pata.ToString("C0");
                        Una3.Text = _ts.ListaChances[2].Una.ToString("C0");

                        Num4.Text = _ts.ListaChances[3].Numero;
                        Directo4.Text = _ts.ListaChances[3].Directo.ToString("C0");
                        Combinado4.Text = _ts.ListaChances[3].Combinado.ToString("C0");
                        Pata4.Text = _ts.ListaChances[3].Pata.ToString("C0");
                        Una4.Text = _ts.ListaChances[3].Una.ToString("C0");
                        break;
                    case 5:
                        Num1.Text = _ts.ListaChances[0].Numero;
                        Directo1.Text = _ts.ListaChances[0].Directo.ToString("C0");
                        Combinado1.Text = _ts.ListaChances[0].Combinado.ToString("C0");
                        Pata1.Text = _ts.ListaChances[0].Pata.ToString("C0");
                        Una1.Text = _ts.ListaChances[0].Una.ToString("C0");

                        Num2.Text = _ts.ListaChances[1].Numero;
                        Directo2.Text = _ts.ListaChances[1].Directo.ToString("C0");
                        Combinado2.Text = _ts.ListaChances[1].Combinado.ToString("C0");
                        Pata2.Text = _ts.ListaChances[1].Pata.ToString("C0");
                        Una2.Text = _ts.ListaChances[1].Una.ToString("C0");

                        Num3.Text = _ts.ListaChances[2].Numero;
                        Directo3.Text = _ts.ListaChances[2].Directo.ToString("C0");
                        Combinado3.Text = _ts.ListaChances[2].Combinado.ToString("C0");
                        Pata3.Text = _ts.ListaChances[2].Pata.ToString("C0");
                        Una3.Text = _ts.ListaChances[2].Una.ToString("C0");

                        Num4.Text = _ts.ListaChances[3].Numero;
                        Directo4.Text = _ts.ListaChances[3].Directo.ToString("C0");
                        Combinado4.Text = _ts.ListaChances[3].Combinado.ToString("C0");
                        Pata4.Text = _ts.ListaChances[3].Pata.ToString("C0");
                        Una4.Text = _ts.ListaChances[3].Una.ToString("C0");

                        Num5.Text = _ts.ListaChances[4].Numero;
                        Directo5.Text = _ts.ListaChances[4].Directo.ToString("C0");
                        Combinado5.Text = _ts.ListaChances[4].Combinado.ToString("C0");
                        Pata5.Text = _ts.ListaChances[4].Pata.ToString("C0");
                        Una5.Text = _ts.ListaChances[4].Una.ToString("C0");
                        break;
                    case 6:
                        Num1.Text = _ts.ListaChances[0].Numero;
                        Directo1.Text = _ts.ListaChances[0].Directo.ToString("C0");
                        Combinado1.Text = _ts.ListaChances[0].Combinado.ToString("C0");
                        Pata1.Text = _ts.ListaChances[0].Pata.ToString("C0");
                        Una1.Text = _ts.ListaChances[0].Una.ToString("C0");

                        Num2.Text = _ts.ListaChances[1].Numero;
                        Directo2.Text = _ts.ListaChances[1].Directo.ToString("C0");
                        Combinado2.Text = _ts.ListaChances[1].Combinado.ToString("C0");
                        Pata2.Text = _ts.ListaChances[1].Pata.ToString("C0");
                        Una2.Text = _ts.ListaChances[1].Una.ToString("C0");

                        Num3.Text = _ts.ListaChances[2].Numero;
                        Directo3.Text = _ts.ListaChances[2].Directo.ToString("C0");
                        Combinado3.Text = _ts.ListaChances[2].Combinado.ToString("C0");
                        Pata3.Text = _ts.ListaChances[2].Pata.ToString("C0");
                        Una3.Text = _ts.ListaChances[2].Una.ToString("C0");

                        Num4.Text = _ts.ListaChances[3].Numero;
                        Directo4.Text = _ts.ListaChances[3].Directo.ToString("C0");
                        Combinado4.Text = _ts.ListaChances[3].Combinado.ToString("C0");
                        Pata4.Text = _ts.ListaChances[3].Pata.ToString("C0");
                        Una4.Text = _ts.ListaChances[3].Una.ToString("C0");

                        Num5.Text = _ts.ListaChances[4].Numero;
                        Directo5.Text = _ts.ListaChances[4].Directo.ToString("C0");
                        Combinado5.Text = _ts.ListaChances[4].Combinado.ToString("C0");
                        Pata5.Text = _ts.ListaChances[4].Pata.ToString("C0");
                        Una5.Text = _ts.ListaChances[4].Una.ToString("C0");

                        Num6.Text = _ts.ListaChances[5].Numero;
                        Directo6.Text = _ts.ListaChances[5].Directo.ToString("C0");
                        Combinado6.Text = _ts.ListaChances[5].Combinado.ToString("C0");
                        Pata6.Text = _ts.ListaChances[5].Pata.ToString("C0");
                        Una6.Text = _ts.ListaChances[5].Una.ToString("C0");
                        break;
                    case 7:
                        Num1.Text = _ts.ListaChances[0].Numero;
                        Directo1.Text = _ts.ListaChances[0].Directo.ToString("C0");
                        Combinado1.Text = _ts.ListaChances[0].Combinado.ToString("C0");
                        Pata1.Text = _ts.ListaChances[0].Pata.ToString("C0");
                        Una1.Text = _ts.ListaChances[0].Una.ToString("C0");

                        Num2.Text = _ts.ListaChances[1].Numero;
                        Directo2.Text = _ts.ListaChances[1].Directo.ToString("C0");
                        Combinado2.Text = _ts.ListaChances[1].Combinado.ToString("C0");
                        Pata2.Text = _ts.ListaChances[1].Pata.ToString("C0");
                        Una2.Text = _ts.ListaChances[1].Una.ToString("C0");

                        Num3.Text = _ts.ListaChances[2].Numero;
                        Directo3.Text = _ts.ListaChances[2].Directo.ToString("C0");
                        Combinado3.Text = _ts.ListaChances[2].Combinado.ToString("C0");
                        Pata3.Text = _ts.ListaChances[2].Pata.ToString("C0");
                        Una3.Text = _ts.ListaChances[2].Una.ToString("C0");

                        Num4.Text = _ts.ListaChances[3].Numero;
                        Directo4.Text = _ts.ListaChances[3].Directo.ToString("C0");
                        Combinado4.Text = _ts.ListaChances[3].Combinado.ToString("C0");
                        Pata4.Text = _ts.ListaChances[3].Pata.ToString("C0");
                        Una4.Text = _ts.ListaChances[3].Una.ToString("C0");

                        Num5.Text = _ts.ListaChances[4].Numero;
                        Directo5.Text = _ts.ListaChances[4].Directo.ToString("C0");
                        Combinado5.Text = _ts.ListaChances[4].Combinado.ToString("C0");
                        Pata5.Text = _ts.ListaChances[4].Pata.ToString("C0");
                        Una5.Text = _ts.ListaChances[4].Una.ToString("C0");

                        Num6.Text = _ts.ListaChances[5].Numero;
                        Directo6.Text = _ts.ListaChances[5].Directo.ToString("C0");
                        Combinado6.Text = _ts.ListaChances[5].Combinado.ToString("C0");
                        Pata6.Text = _ts.ListaChances[5].Pata.ToString("C0");
                        Una6.Text = _ts.ListaChances[5].Una.ToString("C0");

                        Num7.Text = _ts.ListaChances[6].Numero;
                        Directo7.Text = _ts.ListaChances[6].Directo.ToString("C0");
                        Combinado7.Text = _ts.ListaChances[6].Combinado.ToString("C0");
                        Pata7.Text = _ts.ListaChances[6].Pata.ToString("C0");
                        Una7.Text = _ts.ListaChances[6].Una.ToString("C0");
                        break;
                }
                ;

            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "ConfirmNumUC CargarNumeros: " + ex.Message, ex);
               
            }
        }

        private void Btn_Cancel(object sender, EventArgs e)
        {
            SetCallBacksNull();
            _timer?.Stop();
            _nav.NavigateTo(new MenuUC());
       
        }

        private void Btn_Continue(object sender, EventArgs e)
        {
            SetCallBacksNull();
           _timer?.Stop();

            InhabilitarVista();

            SendData();
        }

        public void GetValueAwards()
        {
            try
            {

                List<ApuestasAwardsR> LstApuestas = new List<ApuestasAwardsR>();

                foreach (var x in _ts.ListaChances)
                {

                    ApuestasAwardsR apuestas = new ApuestasAwardsR();

                    apuestas.iva = Convert.ToDouble(AppConfig.Get("iva_porcentaje"));
                    apuestas.ValorDirecto = x.Directo;
                    apuestas.ValorCombinado = x.Combinado;
                    apuestas.ValorPata = x.Pata;
                    apuestas.ValorUna = x.Una;

                    apuestas.tipoChance = new TipoChanceAwardsR
                    {
                        Id = x.TipoChance
                    };

                    LstApuestas.Add(apuestas);

                }
                ;


                RequestAwardsChance request = new RequestAwardsChance
                {

                    EsValorFijo = false,

                    Transaccion = _ts.IdTransaccionApi,

                };

                request.LstApuestas = new ListApuestasAwardsR()
                {
                    apuestas = new List<ApuestasAwardsR>()
                };

                request.LstApuestas.apuestas = LstApuestas;

                Task.Run(async () =>
                {


                    var response = await ApiIntegration.AwardsChance(request);

                    _nav.CloseModal();

                    if (response != null)
                    {

                        if (response.estado == true)
                        {

                            await Dispatcher.BeginInvoke((Action)delegate
                            {

                                ValorAward.Content = response.chance.Valortotalpremiosiniva.ToString("C0");

                            });

                        }




                    }


                });

                _nav.ShowModal("Estamos cargando la informacion un momento por favor", ModalType.Loading);



            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "ConfirmNumUC GetValueAwards: " + ex.Message, ex);
               
            }
        }


        private async void SendData()
        {
            try
            {
                EventLogger.SaveLog(EventType.Info, "ConfirmNumUC SendData", "Iniciando la ejecución de SendData");
                _nav.ShowModal("Estamos guardando la información, un momento por favor", ModalType.Loading);

                await Task.Run(async () =>
                {
                    try
                    {
                        // Configuración de la transacción
                        _ts.EstadoTransaccion = StateTransaction.Iniciada;
                        _ts.Type = ETypeTramites.Chance;
                        _ts.Tipo = ETransactionType.Payment;
                        _ts.TypeTramite = ETypeTramites.Chance;
                        _ts.Valor = _ts.Total.ToString("C0");

                        await Api.CreateTransaction();
                        EventLogger.SaveLog(EventType.Info, "ConfirmNumUC SendData", "Transacción creada con éxito");

                        // Manejo del resultado (se ejecutará en el contexto de UI automáticamente al salir del Task.Run)
                        if (_ts.IdTransaccionApi == 0)
                        {
                            _nav.CloseModal();
                            _nav.ShowModal("No se puede guardar la transacción, inténtelo nuevamente", ModalType.Error);
                            _nav.NavigateTo(new MenuUC());
                        }
                        else
                        {
                            SaveUser();
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLogger.SaveLog(EventType.Error, $"ConfirmNumUC SendData (Background): {ex.Message}", ex.StackTrace);
                        _nav.CloseModal();
                        _nav.ShowModal("No se puede guardar la transacción, intentelo más tarde.", ModalType.Error);
                        _nav.NavigateTo(new MenuUC());
                    }
                    finally
                    {
                        _nav.CloseModal();
                    }
                });
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"ConfirmNumUC SendData (UI Thread): {ex.Message}", ex.StackTrace);
                _nav.CloseModal();
                _nav.ShowModal("Error inesperado al procesar la transacción", ModalType.Error);
                _nav.NavigateTo(new MenuUC());
            }
        }
        public void SaveUser()
        {

            try
            {

                RequestSavePayer Request = new RequestSavePayer();

                Request.identificacion = Convert.ToInt32(_ts.ResponseConsultarCRMRegistro.IDENTIFICACION);
                Request.NombreUser = _ts.ResponseConsultarCRMRegistro.NOMBRES + " " + _ts.ResponseConsultarCRMRegistro.APELLIDOS;
                Request.Correo = _ts.ResponseConsultarCRMRegistro.EMAIL;
                Request.transaccion = _ts.IdTransaccionApi;



                Task.Run(async () =>
                {

                    var Response = await _apiIntegration.SavePayerChance(Request);


                    if (Response != null)
                    {

                        if (Response.Estado == true)
                        {
                            _nav.CloseModal();
                            _ts.IdUser = Response.Tercero.Id;
                            _nav.NavigateTo(new PaymentUC());
                       


                        }
                        else
                        {
                            _nav.CloseModal();
                            _nav.ShowModal("Ocurrió un error generando la transacción, inténtelo nuevamente.", ModalType.Error);
                            _nav.NavigateTo(new MenuUC());
                        }

                    }
                    else
                    {
                        _nav.CloseModal();
                        _nav.ShowModal("Ocurrió un error generando la transacción, inténtelo nuevamente.", ModalType.Error );
                        _nav.NavigateTo(new MenuUC());
                    }


                });
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "ConfirmNumUC SaveUser: " + ex.Message, ex);
             
                _nav.CloseModal();
                _nav.ShowModal("Ocurrió un error generando la transacción, inténtelo nuevamente.", ModalType.Error);
                _nav.NavigateTo(new MenuUC());
            }

        }

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
                EventLogger.SaveLog(EventType.Error, "Error al activar el timer: " + ex.Message, ex);

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
                EventLogger.SaveLog(EventType.Error, "Error al establecer callbacks a null: " + ex.Message, ex);

            }
        }




    }




}
