using Newtonsoft.Json;
using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.Enumerables;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.UserControls;
using System.Globalization;
using System.Windows.Controls;

namespace SuSuerteV2.Presentation.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para OtpUC.xaml
    /// </summary>
    public partial class OtpUC : AppUserControl
    {

        public Transaction transaction;
        private TimerGeneric timer;
        private ApiIntegration _apiIntegration;

        public OtpUC(Transaction transaction)
        {
            InitializeComponent();
            this.transaction = transaction;
            ActivateTimer();
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
            timer.Stop();

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

                                Utilities.CloseModal();

                                if (ResponseInsert != null)
                                {

                                    if (ResponseInsert.ResponseCode == EResponseCode.OK)
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
                                        Utilities.ShowModal("Hubo un error al momento de guardar tus datos intenta nuevamente", EModalType.Error, false);
                                        Utilities.navigator.Navigate(UserControlView.Menu);
                                    }

                                }
                                else
                                {

                                    Utilities.ShowModal("Hubo un error al momento de guardar tus datos intenta nuevamente", EModalType.Error, false);
                                    Utilities.navigator.Navigate(UserControlView.Menu);

                                }


                            }
                            else
                            {
                                Utilities.CloseModal();
                                Utilities.ShowModal("No se logro validar el Codigo OTP Correctamente intenta nuevamente", EModalType.Error, false);
                                Utilities.navigator.Navigate(UserControlView.Menu);
                            }
                        }
                        else
                        {
                            Utilities.CloseModal();
                            Utilities.ShowModal("No se logro validar el Codigo OTP Correctamente intenta nuevamente", EModalType.Error, false);
                            Utilities.navigator.Navigate(UserControlView.Menu);
                        }


                    });

                    Utilities.ShowModal("Estamos verificando y organizando la informacion un momento por favor", EModalType.Preload, false);

                }
                else
                {
                    Utilities.ShowModal("Digita por favor el codigo OTP que fue enviado a tu numero de celular", EModalType.Error, false);
                    ActivateTimer();
                    HabilitarVista();
                }

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("OTPUC" + ex.Message + " " + ex.StackTrace);
                Utilities.CloseModal();
                Utilities.ShowModal("No se logro validar el Codigo OTP Correctamente intenta nuevamente", EModalType.Error, false);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }



        public void GetTypeAstro()
        {
            try
            {
                RequestConsultarSorteo Request = new RequestConsultarSorteo();

                Request.Transacciondistribuidorid = transaction.IdTransactionApi.ToString();



                Task.Run(async () =>
                {

                    var Response = await AdminPayPlus.ApiIntegration.ConsultarSorteo(Request);


                    Utilities.CloseModal();

                    if (Response != null)
                    {
                        if (Response.Estado)
                        {
                            transaction.ResponseConsultarAstro = Response;
                            Utilities.navigator.Navigate(UserControlView.ConsultarLoteriasAstro, transaction);

                        }
                        else
                        {
                            Utilities.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles, intenta nuevamente", EModalType.Error, true);
                            Utilities.navigator.Navigate(UserControlView.Menu);
                        }
                    }
                    else
                    {
                        Utilities.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles,intenta nuevamente", EModalType.Error, true);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }


                });

                Utilities.ShowModal("Estamos validando información un momento por favor", EModalType.Preload, false);


            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("LoginUC GetTypeChance Catch : " + ex.Message + null);
                Utilities.CloseModal();
                Utilities.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles, intenta nuevamente", EModalType.Error, true);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }




        public void GetTypeChance()
        {
            try
            {
                IdProducto Request = new IdProducto();

                Request.transaccion = transaction.IdTransactionApi;
                Request.Id = transaction.ProductSelected.Id;


                Task.Run(async () =>
                {

                    var Response = await AdminPayPlus.ApiIntegration.TypeChance(Request);

                    Utilities.CloseModal();

                    if (Response != null)
                    {
                        if (Response.estadoField)
                        {
                            transaction.TipoChance = Response;
                            transaction.TypeChance = Response.listadotipochanceField;
                            Utilities.navigator.Navigate(UserControlView.Dia, transaction);
                            //    GenerateOTP();

                        }
                        else
                        {
                            Utilities.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles, intenta nuevamente", EModalType.Error, false);
                            Utilities.navigator.Navigate(UserControlView.Menu);
                        }
                    }
                    else
                    {
                        Utilities.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles, intenta nuevamente", EModalType.Error, false);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }

                });

                Utilities.ShowModal("Estamos validando información un momento por favor", EModalType.Preload, false);

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("OtpUC GetTypeChance Catch : " + ex.Message + null);
                Utilities.CloseModal();
                Utilities.ShowModal("En estos momentos los servicios de Susuerte no estan disponibles, intenta nuevamente", EModalType.Error, false);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }

        #region Timer
        private void ActivateTimer()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    tbTimer.Text = "01:59";
                    timer = new TimerGeneric(tbTimer.Text);
                    timer.CallBackClose = response =>
                    {
                        SetCallBacksNull();
                        timer.CallBackStop?.Invoke(1);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                    };
                    timer.CallBackTimer = response =>
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            tbTimer.Text = response;
                        });
                    };
                    GC.Collect();
                });

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("OtpUC ActivateTimer Catch : " + ex.Message + null);
            }
        }

        private void SetCallBacksNull()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    timer.CallBackClose = null;
                    timer.CallBackTimer = null;

                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("OtpUC SetCallBacksNull Catch : " + ex.Message + null);
            }
        }
        #endregion



    }
}
