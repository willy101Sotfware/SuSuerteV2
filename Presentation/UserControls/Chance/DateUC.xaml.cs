using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Modals;
using SuSuerteV2.UserControls;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using static SuSuerteV2.Domain.UIServices.Transaction;

namespace SuSuerteV2.Presentation.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para DateUC.xaml
    /// </summary>
    public partial class DateUC : AppUserControl
    {

        private const string TIMER_INICIAL = "00:59";
        private const int ANIMATION_DELAY = 150;
        private const double SCALE_PRESSED = 0.95;
        private const double SCALE_NORMAL = 1.0;
        private const decimal MONTO_MINIMO = 2000;
        private const decimal MONTO_MAXIMO = 500000;
        private const decimal INCREMENTO_VALIDO = 100;

        public Transaction _ts;
        private TimerGeneric _timer;
        private ApiIntegration _apiIntegration;
        private Navigator _nav;
        private BitmapImage bgSelect = new BitmapImage(new Uri("/Images/Backgrounds/bgSelectDia.png", UriKind.RelativeOrAbsolute));
        private BitmapImage bgNoSelect = new BitmapImage(new Uri("/Images/Backgrounds/bgNoSelectDia.png", UriKind.RelativeOrAbsolute));
        private DateTime selectedDateDT;
        private Task consultarLoterias;

        private string _selectedDate;
        private string selectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;

                bgDia1.Source = bgNoSelect;
                bgDia2.Source = bgNoSelect;
                bgDia3.Source = bgNoSelect;
                Dispatcher.BeginInvoke((Action)delegate
                {
                    switch (_selectedDate)
                    {
                        case "fecha1":
                            selectedDateDT = DateTime.Now.Date;
                            bgDia1.Source = bgSelect;
                            break;
                        case "fecha2":
                            selectedDateDT = DateTime.Now.Date.AddDays(1);
                            bgDia2.Source = bgSelect;
                            break;
                        case "fecha3":
                            selectedDateDT = DateTime.Now.Date.AddDays(2);
                            bgDia3.Source = bgSelect;
                            break;
                    }
                    UpdateLayout();
                });

            }
        }


        private List<Loteria> resLoterias = new List<Loteria>();
        private ObservableCollection<LotteriesViewModel> lstLotteriesModel;
        private CollectionViewSource view = new CollectionViewSource();




        public DateUC()
        {
           

            InitializeComponent();

            _ts = Transaction.Instance;
            txtValidaciones.Text = string.Empty;
            EventLogger.SaveLog(EventType.Info, "Inicializando DateUC");
            // Inicializar lista de modelos de loterías
            lstLotteriesModel = new ObservableCollection<LotteriesViewModel>();
            view.Source = lstLotteriesModel;

            // Establecer la fecha inicial y el estado de los días
            selectedDate = "fecha1";
            UpdateLayout();
            SetDays();

            // Cargar loterías asincrónicamente
            _ = LoadLotteriesAsync(); // No esperamos aquí porque el constructor no puede ser asincrónico

            ActivateTimer();

        }

        private async Task ConsultLotteriesAsync()
        {
            try
            {
                await Task.Delay(1000); // Esperar cualquier actualización de la fecha

                var requestObj = new RequestGetLotteries
                {
                    Id = _ts.ProductSelected.Id,
                    FechaS = selectedDateDT.ToString("dd/MM/yyyy"),
                    transaccion = _ts.IdTransaccionApi
                };

                var respuesta = await ApiIntegration.GetLotteries(requestObj);

                if (respuesta == null)
                {
                    throw new Exception("Respuesta nula de la API de integración.");
                }

                if (respuesta.estadoField != true)
                {
                    throw new Exception("Estado false, no hubo respuesta satisfactoria.");
                }

                resLoterias = respuesta.listadoloteriasField;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error al consultar loterías: " + ex.Message, ex);

                _nav.ShowModal("Ocurrió un error consultando los servicios de loterías", ModalType.Error);
                _nav.NavigateTo(new MenuUC());
              
            }
        }

        private async Task LoadLotteriesAsync()
        {
            try
            {
                // Inhabilitar la vista mientras se carga
                InhabilitarVista();

                // Consultar loterías
                await ConsultLotteriesAsync();

                // Crear el modelo de loterías
                lstLotteriesModel = new ObservableCollection<LotteriesViewModel>();
                foreach (var loteria in resLoterias)
                {
                    string loteriasPath = Path.Combine(
                        Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                        "Images",
                        "Loterias",
                        loteria.nombreField + ".png"
                    );

                    string loteriasSPath = Path.Combine(
                        Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                        "Images",
                        "LoteriasS",
                        loteria.nombreField + ".png"
                    );

                    if (File.Exists(loteriasPath))
                    {
                        lstLotteriesModel.Add(new LotteriesViewModel
                        {
                            ImageData = Utils.LoadImages.LoadImageFromFile(new Uri(loteriasPath)),
                            CodigoCodesa = loteria.codigoField.ToString(),
                            IdCodesa = loteria.idField.ToString(),
                            Title = loteria.nombrecortoField,
                            ImageS = Utils.LoadImages.LoadImageFromFile(new Uri(loteriasSPath)),
                            Image = Utils.LoadImages.LoadImageFromFile(new Uri(loteriasPath)),
                            Nombre = loteria.nombreField,
                            NombreCorto = loteria.nombrecortoField,
                            IsSelected = false
                        });
                    }
                }

                // Actualizar la vista en el hilo principal
                view.Source = lstLotteriesModel;
                this.DataContext = view;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error al cargar loterías: " + ex.Message, ex);
               
            }
            finally
            {
                EventLogger.SaveLog(EventType.Info, "DateUC LoadLotteriesAsync finally : " + selectedDateDT.ToString("dd/MM/yyyy"));
          
                HabilitarVista();
            }
        }

        private void SetDays()
        {
            DateTime hoy = DateTime.Now.Date;

            dia_mes1.Content = "Hoy";
            mes_ano1.Content = hoy.ToString("MMM/yy", new CultureInfo("es-CO")).ToUpper();

            dia_mes2.Content = hoy.AddDays(1).ToString("dd");
            mes_ano2.Content = hoy.AddDays(1).ToString("MMM/yy", new CultureInfo("es-CO")).ToUpper();

            dia_mes3.Content = hoy.AddDays(2).ToString("dd");
            mes_ano3.Content = hoy.AddDays(2).ToString("MMM/yy", new CultureInfo("es-CO")).ToUpper();
        }

        private async void SelectDay(object sender, EventArgs e)
        {
            if (sender is Label)
                selectedDate = (sender as Label).Tag.ToString();
            if (sender is Image)
                selectedDate = (sender as Image).Tag.ToString();
            await LoadLotteriesAsync();
        }






        private void BtnAtras(object sender, EventArgs e)
        {
            SetCallBacksNull();
            _timer.Stop();
            InhabilitarVista();
            Task.Run(() =>
            {
                Thread.Sleep(100);
                _nav.NavigateTo(new MenuUC());
               
            });
        }

        private void BtnSelectLottery(object sender, EventArgs e)
        {
            var data = sender as ListViewItem;
            LotteriesViewModel selectedLottery = (LotteriesViewModel)data.Content;

            selectedLottery.IsSelected = !selectedLottery.IsSelected;

            lvLotteries.Items.Refresh();
            txtValidaciones.Text = "";
        }

        private void BtnContinuar(object sender, EventArgs e)
        {
            SetCallBacksNull();
            _timer?.Stop();
         
            InhabilitarVista();

            if (lstLotteriesModel.Where(lottery => lottery.IsSelected).Count() <= 0)
            {
                txtValidaciones.Text = "*No se ha seleccionado ninguna loteria";
                HabilitarVista();
                ActivateTimer();
                return;
            }

            _ts.Fecha = selectedDateDT.ToString("dd/MM/yyyy"); // Todas las fechas se deben consultar asi
            _ts.ListaLoteriasSeleccionadas = lstLotteriesModel.Where(lot => lot.IsSelected).ToList();
            _nav.NavigateTo(new SelectNumUC());
       
        }

        private void BtnCancelar(object sender, EventArgs e)
        {


            bool noContinuar = _nav.ShowModal("¿Está seguro que desea cancelar la transacción?", ModalType.Info);
            if (noContinuar)
            {
                SetCallBacksNull();
                _timer?.Stop();
                _nav.NavigateTo(new MenuUC());
            
            }


        }



        private void InhabilitarVista()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 0.3;
                this.IsHitTestVisible = false;
            });
        }

        private void HabilitarVista()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 1;
                this.IsHitTestVisible = true;
            });
        }




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
                EventLogger.SaveLog(EventType.Error, "Error al inicializar el timer: " + ex.Message, ex);

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
