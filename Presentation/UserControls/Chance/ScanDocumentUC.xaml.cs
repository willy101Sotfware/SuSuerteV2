using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.Models;
using SuSuerteV2.Domain.Enumerables;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.UserControls;
using System.Globalization;

namespace SuSuerteV2.Presentation.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para ScanDocumentUC.xaml
    /// </summary>
    public partial class ScanDocumentUC : AppUserControl
    {
        private Transaction _ts;

        private ControlScanner  _Cs;
        public ScanDocumentUC()
        {
            InitializeComponent();
            _ts = Transaction.Instance;

            ActivateScanner();
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
                        timer.CallBackStop?.Invoke(1);

                        Utilities.ShowModal("Menores de edad no están permitidos.", EModalType.Error,true);
                        Utilities.navigator.Navigate(UserControlView.Menu);
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

                AdminPayPlus.ControlScanner.callbackScanner = Reference =>
                {
                    if (string.IsNullOrEmpty(Reference)) return;

                    string[] datos = Reference.Split('\t');

                    AdminPayPlus.SaveLog("Data Cedula" + Reference);

                    UserDataChance usuario = new UserDataChance
                    {
                        TipoIdentificacion = ETypeIdentification.Cedula,
                        NumIdentificacion = datos[0],
                        PrimerNombre = datos[3],
                        SegundoNombre= datos[4],
                        PrimerApellido = datos[1],
                        SegundoApellido = datos[2],
                        FechaNacimiento = datos[6],
                        Genero = Convert.ToChar(datos[5])

                    };


                    ts.Document = usuario.NumIdentificacion;
                    ts.Name = $"{usuario.PrimerNombre} {usuario.PrimerApellido}".ToCapitalized();

                    // Hacer peticion para averiguar si el usuario está registrado

                    ts.UserData= usuario;
                    AdminPayPlus.ControlScanner.ClosePortScanner();
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
                        timer.CallBackStop?.Invoke(1);

                        Utilities.ShowModal("Menores de edad no están permitidos.", EModalType.Error,true);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                        return;
                    }
                    ConsultPayer();

                    //  Utilities.navigator.Navigate(UserControlView.Form, ts);



                };

                AdminPayPlus.ControlScanner.callbackErrorScanner = Error =>
                {

                    AdminPayPlus.SaveLog("Error Scan Data Cedula" + Error);

                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Utilities.ShowModal(Error, EModalType.Error,true);
                        ActivateScanner();
                    });
                };

                AdminPayPlus.SaveLog("Entre a escaner AdminPayPlus.ControlScanner.Start();");
                AdminPayPlus.ControlScanner.flagScanner = 0;
                AdminPayPlus.ControlScanner.Start();
            }
            catch (Exception ex)
            {
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
                AdminPayPlus.ControlScanner.ClosePortScanner();
                AdminPayPlus.SaveLog(ex.Message + "   "  + ex.StackTrace);
                Utilities.ShowModal("Ocurrió un error activando el scanner, por favor intente mas tarde", EModalType.Error, true);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }


        }

    }
}
