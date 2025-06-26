using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.ApiService.Models;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Modals;
using SuSuerteV2.Presentation.UserControls.BetPlay;
using SuSuerteV2.UserControls;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace SuSuerteV2.Presentation.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para FormUC.xaml
    /// </summary>
    public partial class FormUC : AppUserControl
    {
        private FormViewModel viewData = new FormViewModel();
        private bool _aceptaTratamientoDatos;
        private bool aceptaTratamientoDatos
        {
            get
            {
                return _aceptaTratamientoDatos;
            }
            set
            {
                _aceptaTratamientoDatos = value;
                viewData.imgPathCheck = value ? "/Images/Buttons/chec_selec.png" : "/Images/Buttons/chec.png";
            }
        }
        private const string TIMER_INICIAL = "01:00";
        private const int ANIMATION_DELAY = 150;
        private const double SCALE_PRESSED = 0.95;
        private const double SCALE_NORMAL = 1.0;
        private const decimal MONTO_MINIMO = 2000;
        private const decimal MONTO_MAXIMO = 500000;
        private const decimal INCREMENTO_VALIDO = 100;

        private DispatcherTimer _animationTimer;
        private Transaction _ts;
        private TimerGeneric _timer;
        private DateTime _birthDateUser;
        private ApiIntegration _apiIntegration;
        private ModalWindow? _currentLoadModal = null;


        public FormUC()
        {
            InitializeComponent();
            InitializeTimer();
            _ts = Transaction.Instance;
            this.btnCheckDatos.DataContext = viewData;

            aceptaTratamientoDatos = false;
            _birthDateUser = DateTime.ParseExact(_ts.UserData.FechaNacimiento, "ddMMyyyy", null);
            SetTextBoxes();
            SetComboBoxes();
        }
        private void SetTextBoxes()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                txtNombre.Text = _ts.UserData.PrimerNombre.ToCapitalized();
                txtSegundoNombre.Text = _ts.UserData.SegundoNombre.ToCapitalized();
                txtApellido.Text = _ts.UserData.PrimerApellido.ToCapitalized();
                txtSegundoApellido.Text = _ts.UserData.SegundoApellido.ToCapitalized();
                txtNumId.Text = _ts.UserData.NumIdentificacion;

            });
        }
        private void SetComboBoxes()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                // Tipo de identificación
                List<string> tiposId = new List<string>
                {
                    "Cédula de ciudadanía",
                    "Cédula de Extrajería"
                };
                typeDocument.ItemsSource = tiposId;
                typeDocument.SelectedIndex = (int)_ts.UserData.TipoIdentificacion;

                //Tipo de genero
                List<string> tiposGenero = new List<string>
                {
                    "Masculino",
                    "Femenino"
                };
                typeGenre.ItemsSource = tiposGenero;
                typeGenre.SelectedIndex = tiposGenero.IndexOf(tiposGenero.Where(genero => genero.StartsWith(_ts.UserData.Genero.ToString())).FirstOrDefault());

                //Fecha de nacimiento : dia.
                List<int> listaDiasMes = Enumerable.Range(1, 31).ToList();
                fecha_dia.ItemsSource = listaDiasMes;
                fecha_dia.SelectedIndex = _birthDateUser.Day - 1; // El -1 es por el indice que comienza en 0

                //Fecha de nacimiento : mes.
                List<int> listaMeses = Enumerable.Range(1, 12).ToList();
                fecha_mes.ItemsSource = listaMeses;
                fecha_mes.SelectedIndex = _birthDateUser.Month - 1; // El -1 es por el indice que comienza en 0

                //Fecha de nacimiento : año.
                int anoInicio = 1940;
                List<int> listaAnos = Enumerable.Range(anoInicio, DateTime.Now.Year - (anoInicio + 18)).ToList();
                fecha_ano.ItemsSource = listaAnos;
                fecha_ano.SelectedIndex = _birthDateUser.Year - anoInicio;

            });
        }

        public class FormViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private string _imgPathCheck { get; set; } = "/Images/Buttons/chec.png";

            public string imgPathCheck
            {
                get
                {
                    return _imgPathCheck;
                }
                set
                {
                    _imgPathCheck = value;
                    OnPropertyChanged(nameof(imgPathCheck));
                }
            }


        }
        private void HandleButtonAction(object sender, InputEventArgs e)
        {
            if (e.Handled) return;

            var button = (Image)sender;
            string actionType = button.Tag?.ToString() ?? string.Empty;

            switch (actionType)
            {
                case "Back":
                    HandleBackAction();
                    break;

                case "Cancel":
                    HandleCancelAction();
                    break;

                case "Continue":
                    HandleContinueAction();
                    break;

                default:
                    Debug.WriteLine($"Acción no reconocida: {actionType}");
                    return;
            }

            e.Handled = true;
        }

        private void HandleBackAction()
        {
         
            Navigator.Instance.NavigateTo(new MenuUC());
          
        }
        private void EventTxtSegundoNombre(object sender, EventArgs e)
        {
            // Utilities.OpenKeyboard(false, (TextBox)sender, this, 300, 780);
        }

        private void HandleCancelAction()
        {
            // Lógica para cancelar
            MessageBoxResult result = MessageBox.Show(
                "¿Está seguro que desea cancelar el registro?",
                "Confirmar",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                ResetForm();
                Navigator.Instance.NavigateTo(new MenuUC());
            }
        }

        private void HandleContinueAction()
        {
            SetCallBacksNull();
            _timer?.Stop();


            if (IsValidPhone())
            {
                if (IsValid())
                {
                    if (aceptaTratamientoDatos)
                    {
                        InhabilitarVista();

                        ; GenerateOTP();
                    }
                    else
                    {
                        _nav.ShowModal("Debe Aceptar el tratamiento de Datos para poder continuar con el proceso", ModalType.Error);
                        ActivateTimer();
                    }
                }
                else
                {
                    _nav.ShowModal("Debes de ingresar un correo electrónico válido", ModalType.Error);
                    ActivateTimer();
                }

            }
            else
            {
                _nav.ShowModal("Debes de ingresar un número de celular válido", ModalType.Error);
                ActivateTimer();
            }
        }

        public void 
            GenerateOTP()
        {
            try
            {

                RequestGetOTP request = new RequestGetOTP();

                request.FlowID = AppConfig.Get("FlowID");
                request.dataToReplace = new List<DataOTP>();

                DataOTP data = new DataOTP()
                {
                    Nombre = txtNombre.Text,
                    Number = "57" + txtNumCel.Text,
                };

                request.dataToReplace.Add(data);

                _ts.UserData.Cel = txtNumCel.Text;
                _ts.UserData.Email = txtEmaill.Text;

                Task.Run(async () =>
                {



                    var response = await _apiIntegration.GetOTP(request);

                    CloseLoadModal();

                    if (response.status == "ok")
                    {
                        Navigator.Instance.NavigateTo(new OtpUC());
                       
                    }
                    else
                    {
                        _nav.ShowModal("Hubo un error al momento de generar el Codigo OTP intenta nuevamente", ModalType.Error);
                        Navigator.Instance.NavigateTo(new MenuUC());
                    }


                });

                _nav.ShowModal("Estamos generando un código de verificación un momento por favor", ModalType.Loading);



            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, nameof(FormUC), "GenerateOTP", ex.Message);

                CloseLoadModal();
                _nav.ShowModal("Hubo un error al momento de generar el Codigo OTP intenta nuevamente",ModalType.Error );
                Navigator.Instance.NavigateTo(new MenuUC());
            }
        }

        private void BtnCheckTratamientoDatos(object sender, EventArgs e)
        {
            aceptaTratamientoDatos = !aceptaTratamientoDatos; // Al hacer este cambio se cambia la imagen también
        }

        private bool IsValidPhone()
        {
            // Patrón para validar el número de celular.
            string phonePattern = @"^\d{10}$"; // Ejemplo para un número de celular de 10 dígitos.

            // Validar el número de celular.
            bool isPhoneValid = Regex.IsMatch(txtNumCel.Text, phonePattern);

            // Retornar verdadero solo si ambos son válidos.
            return isPhoneValid;
        }

        private bool IsValid()
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                        + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<=[a-z0-9])@"
                        + @"(?!\.)([-a-z0-9]+\.)*"
                        + @"[a-z]{2,6}$";

            return Regex.IsMatch(txtEmaill.Text, pattern, RegexOptions.IgnoreCase);

        }


        // Métodos auxiliares
        private bool ValidateForm()
        {
            // Implementa validación de campos
            return !string.IsNullOrEmpty(txtNombre.Text)
                   && !string.IsNullOrEmpty(txtApellido.Text);
        }

        private void SaveFormData()
        {
            // Guarda los datos del formulario
        }

        private void ResetForm()
        {
            // Limpia el formulario
            txtNombre.Text = string.Empty;
            txtApellido.Text = string.Empty;
            // ... otros campos
        }



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
        /// Inicializa los componentes adicionales
        /// </summary>
        private void InitializeComponents()
        {
            // Inicialización adicional si es necesaria
        }

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
    }
}
