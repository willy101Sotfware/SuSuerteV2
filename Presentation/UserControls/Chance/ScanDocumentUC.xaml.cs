using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.ApiService.Models;
using SuSuerteV2.Domain.Enumerables;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Modals;
using SuSuerteV2.Presentation.UserControls.BetPlay;
using SuSuerteV2.UserControls;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SuSuerteV2.Presentation.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para ScanDocumentUC.xaml
    /// </summary>
    public partial class ScanDocumentUC : AppUserControl
    {
        private const string TIMER_INICIAL = "01:00";
        private const int ANIMATION_DELAY = 150;
        private const double SCALE_PRESSED = 0.95;
        private const double SCALE_NORMAL = 1.0;
        private DispatcherTimer _animationTimer;
        private Transaction _ts;
        private TimerGeneric _timer;
        ApiIntegration _apiIntegration = new ApiIntegration();
        private ModalWindow? _currentLoadModal = null;

        private ControlScanner _Cs;
        public ScanDocumentUC()
        {
            InitializeComponent();
            _ts = Transaction.Instance;
            InitializeTimer();
            ActivateScanner();
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
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanupResources();
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
        private void RestoreElementScale(FrameworkElement element)
        {
            ScaleElement(element, SCALE_NORMAL);
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



        private void ProcessByName(string elementName)
        {
            switch (elementName)
            {
                case "btnAtras":
                    ProcessAtras();
                    break;
            }
        }

        private void ProcessAtras()
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

        private void ActivateScanner()
        {
            try
            {

                Api.ControlScanner.callbackOut = Data =>
                {


                    UserDataChance usuario = new UserDataChance
                    {
                        TipoIdentificacion = ETypeIdentification.Cedula,
                        NumIdentificacion = Data.Document,
                        PrimerNombre = Data.FirstName,
                        SegundoNombre = Data.SecondName,
                        PrimerApellido = Data.LastName,
                        SegundoApellido = Data.SecondLastName,
                        FechaNacimiento = Data.Date,
                        Genero = Data.Gender[0]

                    };

                    DateTime fecha = DateTime.ParseExact(usuario.FechaNacimiento, "yyyyMMdd", CultureInfo.InvariantCulture);

                    // Convertir a formato deseado
                    string fechaFormateada = fecha.ToString("ddMMyyyy");

                    usuario.FechaNacimiento = fechaFormateada;

                    _ts.Documento = usuario.NumIdentificacion;
                    _ts.Name = $"{usuario.PrimerNombre} {usuario.PrimerApellido}".ToCapitalized();

                    // Hacer peticion para averiguar si el usuario está registrado

                    _ts.UserData = usuario;
                    Api.ControlScanner.ClosePortScanner();

                    // Calcular la edad comparando las fechas de nacimiento y actual
                    int edad = DateTime.Now.Year - fecha.Year;
                    if (DateTime.Now.Month < fecha.Month || (fecha.Month == DateTime.Now.Month && DateTime.Now.Day < fecha.Day))
                    {
                        edad--; // Aún no ha cumplido años este año
                    }

                    if (edad < 18)
                    {
                        SetCallBacksNull();
                        _timer?.Stop();
                        Navigator.Instance.ShowModal(
                               "Menores de edad no están permitidos",
                               ModalType.Error);


                        Navigator.Instance.NavigateTo(new MenuUC());
                        return;
                    }

                    //   calculamos la edad de la persona para validar si este es mayor de edad
                    //if ((DateTime.Now.Year - fecha.Year) < 18)
                    //{
                    //    Utilities.ShowModal("Menores de edad no están permitidos.", EModalType.Error);
                    //    Utilities.navigator.Navigate(UserControlView.Menu);
                    //    return;
                    //}

                    ConsultPayer();

                };

                Api.ControlScanner.callbackScanner = Reference =>
                {
                    if (string.IsNullOrEmpty(Reference)) return;

                    string[] datos = Reference.Split('\t');

                    EventLogger.SaveLog(EventType.Info, "Data Cedula" + Reference);

                    UserDataChance usuario = new UserDataChance
                    {
                        TipoIdentificacion = ETypeIdentification.Cedula,
                        NumIdentificacion = datos[0],
                        PrimerNombre = datos[3],
                        SegundoNombre = datos[4],
                        PrimerApellido = datos[1],
                        SegundoApellido = datos[2],
                        FechaNacimiento = datos[6],
                        Genero = Convert.ToChar(datos[5])

                    };


                    _ts.Documento = usuario.NumIdentificacion;
                    _ts.Name = $"{usuario.PrimerNombre} {usuario.PrimerApellido}".ToCapitalized();

                    // Hacer peticion para averiguar si el usuario está registrado

                    _ts.UserData = usuario;
                    Api.ControlScanner.ClosePortScanner();
                    DateTime fecha = DateTime.ParseExact(usuario.FechaNacimiento, "ddMMyyyy", CultureInfo.InvariantCulture);
                    // Calcular la edad comparando las fechas de nacimiento y actual
                    int edad = DateTime.Now.Year - fecha.Year;
                    if (DateTime.Now.Month < fecha.Month || (fecha.Month == DateTime.Now.Month && DateTime.Now.Day < fecha.Day))
                    {
                        edad--; // Aún no ha cumplido años este año
                    }

                    if (edad < 18)
                    {
                        SetCallBacksNull();
                        _timer.Stop();
                        Navigator.Instance.ShowModal(
                               "Menores de edad no están permitidos",
                               ModalType.Error);

                        Navigator.Instance.NavigateTo(new MenuUC());
                        return;
                    }
                    ConsultPayer();

                    //  Utilities.navigator.Navigate(UserControlView.Form, ts);



                };

                Api.ControlScanner.callbackErrorScanner = Error =>
                {
                    EventLogger.SaveLog(EventType.Info, "Error Scan Data Cedula" + Error);
                

                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Navigator.Instance.ShowModal(
                              "Error ",
                              ModalType.Error);
                  
                        ActivateScanner();
                    });
                };

                Navigator.Instance.ShowModal(
                             "Entre a escaner AdminPayPlus.ControlScanner.Start()",
                             ModalType.Error);

              
                Api.ControlScanner.flagScanner = 0;
                Api.ControlScanner.Start();
            }
            catch (Exception ex)
            {
                SetCallBacksNull();
                _timer.Stop();
                Api.ControlScanner.ClosePortScanner();

                EventLogger.SaveLog(EventType.Info, ex.Message + "   " + ex.StackTrace);

                Navigator.Instance.ShowModal(
                                 "Ocurrió un error activando el scanner, por favor intente mas tarde",
                                 ModalType.Error);


                Navigator.Instance.NavigateTo(new MenuUC());
            }


        }

        private void ConsultPayer()
        {
               ModalWindow? loadModal = null;
            try
            {

                InhabilitarVista();

                SetCallBacksNull();
                _timer?.Stop();


                RequestConsultCrmRegistro request = new RequestConsultCrmRegistro();

                request.DocumentType = "CC";
                request.Document = _ts.Documento;

                Task.Run(async () =>
                {


                    var Respuesta = await _apiIntegration.ConsultCrmRegistroRecargas(request);

                    // Ensure loadModal is not null before calling Close()
                    loadModal?.Close();

                    if (Respuesta != null)
                    {
                        _ts.ResponseConsultarCRMRegistro = Respuesta;
                        _ts.NombreUsuario = string.Concat(Respuesta.NOMBRES, " ", Respuesta.APELLIDOS);


                        if (_ts.Type == ETypeTramites.SuperChance)
                        {
                            Navigator.Instance.NavigateTo(new LoginChanceUC());
                          
                        }
                        else if (_ts.Type == ETypeTramites.Astro)
                        {
                            GetTypeAstro();
                        }




                    }
                    else
                    {
                        if (Navigator.Instance.ShowModal("Tus datos no estan registrados en el sistema para continuar te invitamos a llenar el siguiente formulario", ModalType.Info))
                        {
                            // Si el usuario acepta, redirigir al formulario de registro
                            Navigator.Instance.NavigateTo(new FormUC());
                        }
                        else
                        {
                            // Si el usuario cancela, redirigir al menú principal
                            Navigator.Instance.NavigateTo(new MenuUC());
                        }

                    }

                });
                Navigator.Instance.ShowModal("Estamos consultando la informacion un momento por favor", ModalType.Loading);

             

            }
            catch (Exception ex)
            {

            }
        }

        public void GetTypeAstro()
        {
            ModalWindow? loadModal = null;
            try
            {
                RequestConsultarSorteo Request = new RequestConsultarSorteo();

                Request.Transacciondistribuidorid = _ts.IdTransaccionApi.ToString();



                Task.Run(async () =>
                {

                    var Response = await _apiIntegration.ConsultarSorteo(Request);

                    // Ensure loadModal is not null before calling Close()
                    loadModal?.Close();
                    if (Response != null)
                    {
                        if (Response.Estado)
                        {
                            _ts.ResponseConsultarAstro = Response;
                            Navigator.Instance.NavigateTo(new LoteriasAstroUC());
                        

                        }
                        else
                        {
                            Navigator.Instance.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles, intenta nuevamente", ModalType.Error);
                            Navigator.Instance.NavigateTo(new MenuUC());
                        }
                    }
                    else
                    {
                        Navigator.Instance.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles,intenta nuevamente", ModalType.Error);
                       Navigator.Instance.NavigateTo(new MenuUC());
                    }


                });
                Navigator.Instance.ShowModal("Estamos validando información un momento por favor", ModalType.Loading);


               


            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "ScanDocumentUC GetTypeAstro Catch : " + ex.Message, null);
                loadModal?.Close();
                Navigator.Instance.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles, intenta nuevamente", ModalType.Error);

                Navigator.Instance.NavigateTo(new MenuUC());
            }
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
