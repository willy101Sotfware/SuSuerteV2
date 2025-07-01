using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.Enumerables;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Domain.Variables;
using SuSuerteV2.Modals;
using SuSuerteV2.Presentation.UserControls.BetPlay;
using SuSuerteV2.Presentation.UserControls.Chance;
using SuSuerteV2.UserControls;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using static SuSuerteV2.Domain.UIServices.Transaction;
using static SuSuerteV2.Presentation.UserControls.Astro.ApuestaAstroUC;

namespace SuSuerteV2.Presentation.UserControls.Astro
{
    /// <summary>
    /// Lógica de interacción para ResumenAstroUC.xaml
    /// </summary>
    public partial class ResumenAstroUC : AppUserControl
    {

        Transaction _ts;
        private TimerGeneric _timer;
        private ModalWindow? _currentLoadModal = null;
        private ObservableCollection<LotteriesViewModel> lstLotteriesModel;
        private CollectionViewSource view = new CollectionViewSource();
        private SelectNumViewModel viewModel = new SelectNumViewModel();
        ApiIntegration _apiIntegration;
        public ResumenAstroUC()
        {
            InitializeComponent();
            _ts = Transaction.Instance;
            view = new CollectionViewSource();
            lstLotteriesModel = new ObservableCollection<LotteriesViewModel>();

            Directo1.Text = _ts.NumeroApostadoAstro;
            Pata1.Text = string.Format("{0:C0}", Convert.ToDecimal(_ts.ValorApostadoAstro));
            ValorT.Content = string.Format("{0:C0}", Convert.ToDecimal(_ts.ValorApostadoAstro));

            loadlotteries();
            LoadSignos();
            ActivateTimer();
        }


        private void loadlotteries()
        {
            string imagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "Buttons", _ts.NombreLoteria + ".png");

            // Verifica si el archivo de imagen existe
            if (File.Exists(imagePath))
            {
                // Crea un objeto BitmapImage
                BitmapImage bitmap = new BitmapImage();

                // Inicializa el BitmapImage
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.EndInit();

                // Asigna el BitmapImage al control Image
                imgLoteria2.Source = bitmap;
            }
            else
            {

            }

        }
        public void LoadSignos()
        {
            try
            {
                string loteriasPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "Buttons");

                // Itera sobre las claves del diccionario
                foreach (var nombreImagen in _ts.DicSginosSeleccionados.Keys)
                {
                    // Construye la ruta completa de la imagen
                    string rutaImagen = Path.Combine(loteriasPath, nombreImagen + ".png");

                    // Verifica si el archivo de imagen existe
                    if (File.Exists(rutaImagen))
                    {
                        // Carga la imagen y agrega un nuevo modelo a la lista
                        lstLotteriesModel.Add(new LotteriesViewModel
                        {
                            ImageData =Utils.LoadImages.LoadImageFromFile(new Uri(rutaImagen, UriKind.Absolute))
                        });
                    }
                    else
                    {

                    }
                }

                view.Source = lstLotteriesModel;
                this.DataContext = view;

            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, nameof(ResumenAstroUC), "LoadSignos", ex.Message);
             
            }
        }

        private void Btn_Cancel(object sender, EventArgs e)
        {
            SetCallBacksNull();
            _timer?.Stop();
            Navigator.Instance.NavigateTo(new MenuUC());
        }


        private void Btn_Continue(object sender, EventArgs e)
        {
            SetCallBacksNull();
            _timer?.Stop();
         
            InhabilitarVista();


            _ts.paymentProcess.Total = _ts.ValorApostadoAstro; 

            SendData();
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
                            _nav.NavigateTo(new PaymentAstroUC());
                        
                          
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
                        _nav.ShowModal("Ocurrió un error generando la transacción, inténtelo nuevamente.", ModalType.Error);
                        _nav.NavigateTo(new MenuUC());
                    }


                });
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, nameof(ResumenAstroUC), "SaveUser", ex.Message);
                _nav.CloseModal();
                _nav.ShowModal("Ocurrió un error generando la transacción, inténtelo nuevamente.", ModalType.Error);
                _nav.NavigateTo(new MenuUC());
            }

        }

        private async Task SendData()
        {
            ModalWindow? loadModal = null;
            try
            {
                loadModal = _nav.ShowLoadModal(Messages.VALIDATING_INFO);

                var tsCreated = await Api.CreateTransaction();
                if (tsCreated == null) throw new Exception("No se pudo enviar la transacción");

#if NO_PERIPHERALS
#else
                // Cada camara es una source incremental
                await VideoRecorder.Start(source: 0);
#endif

                if (loadModal != null)
                {
                    loadModal.Close();
                    loadModal = null;
                }

                if (_ts.TipoPago == TypePayment.Efectivo)
                {
                  Dispatcher.Invoke(() => _nav.NavigateTo(new PaymentAstroUC()));
                }

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
                _nav.ShowModal("Ocurrió un error validando la información. Por favor intente nuevamente.", new InfoModal());
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
