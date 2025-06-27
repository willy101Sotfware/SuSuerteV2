using Newtonsoft.Json;
using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.Enumerables;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Modals;
using SuSuerteV2.Presentation.UserControls.Astro;
using SuSuerteV2.UserControls;
using System.Globalization;
using System.Windows.Controls;
using String = System.String;

namespace SuSuerteV2.Presentation.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para OtpUC.xaml
    /// </summary>
    public partial class OtpUC : AppUserControl
    {
        private const string TIMER_INICIAL = "01:59";
        private const int ANIMATION_DELAY = 150;
        private const double SCALE_PRESSED = 0.95;
        private const double SCALE_NORMAL = 1.0;
        private const decimal MONTO_MINIMO = 2000;
        private const decimal MONTO_MAXIMO = 500000;
        private const decimal INCREMENTO_VALIDO = 100;

        public Transaction transaction;
        private TimerGeneric _timer;
        private ApiIntegration _apiIntegration;
        private Navigator _nav;

        public OtpUC()
        {
            InitializeComponent();
            transaction = Transaction.Instance;
            ActivateTimer();
            EventLogger.SaveLog(EventType.Info, "Iniciando OtpUC");

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


        private void BtnContinuar(object sender, EventArgs e)
        {
            InhabilitarVista();

            SetCallBacksNull();
            _timer.Stop();

            ValidateOTP();
        }

        private void BtnCancelar(object sender, EventArgs e)
        {
            Navigator.Instance.NavigateTo(new MenuUC());
        }


        private void Keyboard_TouchDown(object sender, EventArgs e)
        {
            try
            {
                Image image = (Image)sender;
                string Tag = image.Tag.ToString();
                TxtOTP.Text += Tag;
            }
            catch (Exception ex)
            {

            }
        }

        private void Btn_DeleteTouchDown(object sender, EventArgs e)
        {
            try
            {
                string val = TxtOTP.Text;

                if (val.Length > 0)
                {
                    TxtOTP.Text = val.Remove(val.Length - 1);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Btn_DeleteAllTouchDown(object sender, EventArgs e)
        {
            try
            {
                TxtOTP.Text = string.Empty;

            }
            catch (Exception ex)
            {

            }
        }

        public void ValidateOTP()
        {
            try
            {

                DateTime fecha = DateTime.ParseExact(transaction.UserData.FechaNacimiento, "ddMMyyyy", CultureInfo.InvariantCulture);

                // Convertir a formato deseado
                string fechaFormateada = fecha.ToString("yyyy-MM-dd");



                transaction.CrmCreate = new CrmCreateRegistro();

                transaction.CrmCreate.CELULAR = transaction.UserData.Cel;
                transaction.CrmCreate.EMAIL = transaction.UserData.Email;
                transaction.CrmCreate.IDENTIFICACION = transaction.UserData.NumIdentificacion;
                transaction.CrmCreate.NOMBRES = !string.IsNullOrWhiteSpace(transaction.UserData.PrimerNombre) && !string.IsNullOrWhiteSpace(transaction.UserData.SegundoNombre) ? string.Concat(transaction.UserData.PrimerNombre, " ", transaction.UserData.SegundoNombre) : transaction.UserData.PrimerNombre;
                transaction.CrmCreate.APELLIDOS = !string.IsNullOrWhiteSpace(transaction.UserData.PrimerApellido) && !string.IsNullOrWhiteSpace(transaction.UserData.SegundoApellido) ? string.Concat(transaction.UserData.PrimerApellido, " ", transaction.UserData.SegundoApellido) : transaction.UserData.PrimerApellido;
                transaction.CrmCreate.GENERO = transaction.UserData.Genero.ToString();
                transaction.CrmCreate.TIPO_IDENTIFICACION = "CC";
                transaction.CrmCreate.FECHA_NACIMIENTO = fechaFormateada;



                if (!string.IsNullOrEmpty(TxtOTP.Text))
                {
                    RequestVTOP request = new RequestVTOP();

                    request.otp = TxtOTP.Text;
                    request.recipient = String.Concat("57" + transaction.UserData.Cel);



                    Task.Run(async () =>
                    {

                        var Response = await _apiIntegration.ValidateOTP(request);
                        EventLogger.SaveLog(EventType.Info, "Response Validate OTP OtpUC" + JsonConvert.SerializeObject(Response));    

                     

                        if (Response != null)
                        {

                            EventLogger.SaveLog(EventType.Info, Response.statusCode.ToString() + " " + Response.message + " " + Response.data.validOtp);
                           

                            if (Response.statusCode == 200 && Response.message == "OK")
                            {


                                var ResponseInsert = await _apiIntegration.CrearRegistroCRM(transaction.CrmCreate);

                                
                               _nav.CloseModal();
                        

                                if (ResponseInsert != null)
                                {

                                    if (ResponseInsert != null)
                                    {

                                        transaction.ResponseConsultarCRMRegistro = new ResponseConsultarCRMRegistro();

                                        transaction.ResponseConsultarCRMRegistro.IDENTIFICACION = transaction.CrmCreate.IDENTIFICACION;
                                        transaction.ResponseConsultarCRMRegistro.NOMBRES = transaction.CrmCreate.NOMBRES;
                                        transaction.ResponseConsultarCRMRegistro.APELLIDOS = transaction.CrmCreate.APELLIDOS;
                                        transaction.ResponseConsultarCRMRegistro.EMAIL = transaction.CrmCreate.EMAIL;


                                        if (transaction.Type == ETypeTramites.SuperChance)
                                        {

                                            GetTypeChance();
                                        }
                                        else if (transaction.Type == ETypeTramites.Astro)
                                        {
                                            GetTypeAstro();
                                        }



                                    }
                                    else
                                    {

                                        _nav.ShowModal("Hubo un error al momento de guardar tus datos intenta nuevamente", ModalType.Error);
                                        _nav.NavigateTo(new MenuUC());
                                 
                                    }

                                }
                                else
                                {


                                    _nav.ShowModal("Hubo un error al momento de guardar tus datos intenta nuevamente", ModalType.Error);
                                    _nav.NavigateTo(new MenuUC());
                           

                                }


                            }
                            else
                            {
                                _nav.CloseModal();
                                EventLogger.SaveLog(EventType.Error, "Error al validar OTP: " + Response.message);
                                _nav.ShowModal("No se logro validar el Codigo OTP Correctamente intenta nuevamente", ModalType.Error);
                                _nav.NavigateTo(new MenuUC());
                               
                            }
                        }
                        else
                        {
                            _nav.CloseModal();
                            _nav.ShowModal("No se logro validar el Codigo OTP Correctamente intenta nuevamente", ModalType.Error);
                          
                        }


                    });


                    _nav.ShowModal("Estamos verificando y organizando la informacion un momento por favor", ModalType.Loading);

                }
                else
                {
                    _nav.ShowModal("Digita por favor el codigo OTP que fue enviado a tu numero de celular", ModalType.Error);
                    ActivateTimer();
                    HabilitarVista();
                }

            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "OtpUC ValidateOTP Catch: " + ex.Message + " " + ex.StackTrace);
      
                _nav.CloseModal();
                _nav.ShowModal("No se logro validar el Codigo OTP Correctamente intenta nuevamente", ModalType.Error);
                _nav.NavigateTo(new MenuUC());
              
            }
        }



        public void GetTypeAstro()
        {
            try
            {
                RequestConsultarSorteo Request = new RequestConsultarSorteo();

                Request.Transacciondistribuidorid = transaction.IdTransaccionApi.ToString();



                Task.Run(async () =>
                {

                    var Response = await _apiIntegration.ConsultarSorteo(Request);


                    _nav.CloseModal();

                    if (Response != null)
                    {
                        if (Response.Estado)
                        {
                            transaction.ResponseConsultarAstro = Response;
                            _nav.NavigateTo(new LoteriasAstroUC());
                      

                        }
                        else
                        {
                            _nav.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles, intenta nuevamente", ModalType.Error);
                            _nav.NavigateTo(new MenuUC());
             
                        }
                    }
                    else
                    {
                        _nav.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles,intenta nuevamente", ModalType.Error);
                        _nav.NavigateTo(new MenuUC());
                    
                    }


                });

                _nav.ShowModal("Estamos validando información un momento por favor", ModalType.Loading);


            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "OtpUC GetTypeAstro Catch: " + ex.Message + " " + ex.StackTrace);
                _nav.CloseModal();
                _nav.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles, intenta nuevamente", ModalType.Error);
                _nav.NavigateTo(new MenuUC());
            }
        }




        public void GetTypeChance()
        {
            try
            {
                IdProducto Request = new IdProducto();

                Request.transaccion = transaction.IdTransaccionApi;
                Request.Id = transaction.ProductSelected.Id;


                Task.Run(async () =>
                {

                    var Response = await ApiIntegration.TypeChance(Request);

                    _nav.CloseModal();

                    if (Response != null)
                    {
                        if (Response.estadoField)
                        {
                            transaction.TipoChance = Response;
                            transaction.TypeChance = Response.listadotipochanceField;
                            _nav.NavigateTo(new DateUC());
                      
                            //    GenerateOTP();

                        }
                        else
                        {
                            _nav.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles, intenta nuevamente", ModalType.Error);
                            _nav.NavigateTo(new MenuUC());
                         
                        }
                    }
                    else
                    {
                        _nav.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles, intenta nuevamente", ModalType.Error);
                        _nav.NavigateTo(new MenuUC());
                       
                    }

                });

                _nav.ShowModal("Estamos validando información un momento por favor", ModalType.Loading);

            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "OtpUC GetTypeChance Catch: " + ex.Message + " " + ex.StackTrace);
     
                _nav.CloseModal();
                _nav.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles, intenta nuevamente", ModalType.Error);
                _nav.NavigateTo(new MenuUC());
               
            }
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
