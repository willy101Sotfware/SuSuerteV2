using SuSuerteV2.Domain;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Modals;
using SuSuerteV2.UserControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SuSuerteV2.Presentation.UserControls.BetPlay
{
    /// <summary>
    /// Lógica de interacción para LoginUC.xaml
    /// </summary>
    public partial class LoginUC : AppUserControl
    {
        private TimerGeneric _timer;
        private const string STR_TIMER = "01:00";
        public bool txtcedula = false;
        public bool txtvalidar = false;

        public LoginUC()
        {
            InitializeComponent();
            InitializeTimer();
        }

        // EVENTO UNIFICADO - Funciona para Mouse y Táctil
        private void UIElement_Interaction(object sender, MouseButtonEventArgs e)
        {
            try
            {
                e.Handled = true; // Detiene la propagación del evento

                if (!(sender is FrameworkElement element)) return;

                // Efecto visual al presionar
                ScaleElement(element, 0.95);

                // Procesa el elemento presionado después de un pequeño retraso
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(150);
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    ProcessElementClick(element);
                    ScaleElement(element, 1.0); // Restaura el tamaño
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "LoginUC", "Error en interacción", ex.Message);
            }
        }

        private void ProcessElementClick(FrameworkElement element)
        {
            try
            {
                string elementName = element.Name;
                string tag = element.Tag?.ToString() ?? "";

                // Procesar según el tipo de elemento
                if (element.Name.StartsWith("btn") || !string.IsNullOrEmpty(tag))
                {
                    switch (tag)
                    {
                        case "0":
                        case "1":
                        case "2":
                        case "3":
                        case "4":
                        case "5":
                        case "6":
                        case "7":
                        case "8":
                        case "9":
                            ProcessKeyboardInput(tag);
                            break;

                        case "Borrar":
                            ProcessDeleteAll();
                            break;

                        case "All":
                            ProcessDeleteOne();
                            break;
                    }
                }

                // Procesar botones por nombre
                switch (elementName)
                {
                    case "btnContinuar":
                        ProcessContinuar();
                        break;

                    case "btnCancelar":
                        ProcessCancelar();
                        break;

                    case "btnAtras":
                        ProcessAtras();
                        break;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "LoginUC", "ProcessElementClick", ex.Message);
            }
        }

        private void ScaleElement(FrameworkElement element, double scale)
        {
            try
            {
                var transform = element.RenderTransform as ScaleTransform;
                if (transform == null)
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
                EventLogger.SaveLog(EventType.Error, "LoginUC", "Error aplicando efecto visual", ex.Message);
            }
        }

        // ========== LÓGICA DE PROCESAMIENTO ==========

        private void ProcessKeyboardInput(string digit)
        {
            try
            {
                if (txtcedula)
                {
                    TxtCedula.Text += digit;
                }

                if (txtvalidar)
                {
                    TxtValidate.Text += digit;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "LoginUC", "ProcessKeyboardInput", ex.Message);
            }
        }

        private void ProcessDeleteOne()
        {
            try
            {
                if (txtcedula)
                {
                    string val = TxtCedula.Text;
                    if (val.Length > 0)
                    {
                        TxtCedula.Text = val.Remove(val.Length - 1);
                    }
                }

                if (txtvalidar)
                {
                    string val2 = TxtValidate.Text;
                    if (val2.Length > 0)
                    {
                        TxtValidate.Text = val2.Remove(val2.Length - 1);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "LoginUC", "ProcessDeleteOne", ex.Message);
            }
        }

        private void ProcessDeleteAll()
        {
            try
            {
                if (txtcedula)
                {
                    TxtCedula.Text = string.Empty;
                }

                if (txtvalidar)
                {
                    TxtValidate.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "LoginUC", "ProcessDeleteAll", ex.Message);
            }
        }

        private void ProcessContinuar()
        {
            try
            {
                _timer.Stop();
                EventLogger.SaveLog(EventType.Info, "LoginUC", "ProcessContinuar", "Ingresando al evento ProcessContinuar", null);

                InhabilitarVista();

                if (Validate())
                {
                    Transaction.Instance.Documento = TxtCedula.Text;
                    EventLogger.SaveLog(EventType.Info, "LoginUC", "Navegando a Recharge", "OK");
                    Navigator.Instance.NavigateTo(new RechargeUC());
                }
                else
                {
                    EventLogger.SaveLog(EventType.Error, "LoginUC", "El documento ingresado no coincide", "Error al validar documento", null);
                    _timer.Start();
                    HabilitarVista();
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "LoginUC", "ProcessContinuar", ex.Message);
                _timer.Start();
                HabilitarVista();
            }
        }

        private void ProcessCancelar()
        {
            try
            {
                EventLogger.SaveLog(EventType.Info, "LoginUC", "ProcessCancelar", "Ingresando al evento ProcessCancelar", null);
                _timer.Stop();
                Navigator.Instance.NavigateTo(new MenuUC());
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "LoginUC", "ProcessCancelar", ex.Message);
            }
        }

        private void ProcessAtras()
        {
            try
            {
                EventLogger.SaveLog(EventType.Info, "LoginUC", "ProcessAtras", "Ingresando al evento ProcessAtras", null);
                _timer.Stop();
                Navigator.Instance.NavigateTo(new MenuUC());
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "LoginUC", "ProcessAtras", ex.Message);
            }
        }

        // ========== VALIDACIÓN ==========

        private bool Validate()
        {
            try
            {
                if (!string.IsNullOrEmpty(TxtValidate.Text) && !string.IsNullOrEmpty(TxtCedula.Text))
                {
                    return TxtValidate.Text == TxtCedula.Text;
                }
                return false;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "LoginUC", "Validate", ex.Message);
                return false;
            }
        }

        // ========== MANEJO DE FOCUS ==========

        private void focusTxtCedula(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                txtcedula = true;
                txtvalidar = false;
                if (TxtCedula.Text == "Número de cédula")
                {
                    TxtCedula.Text = "";
                    textBox.Background = new SolidColorBrush(Colors.LightGray);
                    TxtCedula.Foreground = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    textBox.Background = new SolidColorBrush(Colors.LightGray);
                    TxtCedula.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void focusTxtvalidar(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                txtcedula = false;
                txtvalidar = true;
                if (TxtValidate.Text == "Confirmar")
                {
                    TxtValidate.Text = "";
                    textBox.Background = new SolidColorBrush(Colors.LightGray);
                    TxtValidate.Foreground = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    textBox.Background = new SolidColorBrush(Colors.LightGray);
                    TxtValidate.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void TxtNumCel_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (string.IsNullOrWhiteSpace(TxtCedula.Text))
                {
                    TxtCedula.Text = "Número de cédula";
                    TxtCedula.Foreground = new SolidColorBrush(Colors.Gray);
                    textBox.Background = new SolidColorBrush(Colors.White);
                }
                else
                {
                    TxtCedula.Foreground = new SolidColorBrush(Colors.Black);
                    textBox.Background = new SolidColorBrush(Colors.White);
                }
            }
        }

        private void TxtVal_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (string.IsNullOrWhiteSpace(TxtValidate.Text))
                {
                    TxtValidate.Text = "Confirmar";
                    TxtValidate.Foreground = new SolidColorBrush(Colors.Gray);
                    textBox.Background = new SolidColorBrush(Colors.White);
                }
                else
                {
                    TxtValidate.Foreground = new SolidColorBrush(Colors.Black);
                    textBox.Background = new SolidColorBrush(Colors.White);
                }
            }
        }

        // ========== MÉTODOS DE UTILIDAD ==========

        private void InhabilitarVista()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 0.3;
                this.IsEnabled = false;
            });
        }

        private void HabilitarVista()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 1;
                this.IsEnabled = true;
            });
        }

        // ========== TIMER ==========

        private void InitializeTimer()
        {
            _timer = new TimerGeneric(STR_TIMER);

            _timer.Tick += (tiempo) => {
                Dispatcher.Invoke(() => {
                    tbTimer.Text = tiempo;
                });
            };

            _timer.TimeOut += () => {
                Dispatcher.Invoke(() => {
                    tbTimer.Text = "00:00";
                    Navigator.Instance.NavigateTo(new MenuUC());
                });
            };

            tbTimer.Text = "01:00";
            _timer.Start();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer?.Stop();
            _timer?.Dispose();
        }
    }
}