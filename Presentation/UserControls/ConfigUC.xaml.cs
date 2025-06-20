using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Domain.Variables;
using SuSuerteV2.Modals;
using SuSuerteV2.UserControls;
using System.ComponentModel;
using System.Windows;

namespace SuSuerteV2.Presentation.UserControls
{
    /// <summary>
    /// Lógica de interacción para ConfigUC.xaml
    /// </summary>
    public partial class ConfigUC : AppUserControl
    {
        private ConfigViewModel _viewModel;

        public ConfigUC()
        {

            InitializeComponent();


            DataContext = new ConfigViewModel();
            _viewModel = (ConfigViewModel)DataContext;

            Transaction.Reset();
            this.Loaded += OnLoaded;
        }
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            await InitPayPad();
        }

        private async Task InitPayPad()
        {
            EventLogger.SaveLog(EventType.Info, "Inicializando Pay+");
            try
            {
                // TODO: Finalizar cualquier grabación

                _viewModel.StatusMsg = Messages.LOGIN_IN;
                if (!await Api.Login())
                {
                    await Retry(Messages.NO_SERVICE + " No se logró iniciar sesión en los servicios de E-City.");
                    return;
                }

                _viewModel.StatusMsg = Messages.VALIDATING_PAYPLUS;
                if (!await Api.Validate())
                {
                    await Retry(Messages.NO_SERVICE + " No cuenta con suficiente dinero para operar.");
                    return;
                }
#if NO_PERIPHERALS
#else
                // Validación de perifericos
                _viewModel.StatusMsg = Messages.VALIDATING_PERIPHERALS;
                var peripheralController = ArduinoController.Instance;
                if (!await peripheralController.SendStart())
                {
                    await Retry(Messages.NO_SERVICE + " " + Messages.PERIPHERALS_FAILED_VALIDATE);
                    return;
                }

                peripheralController.StartAcceptance(0);
                await Task.Delay(1000);
                await peripheralController.StopAceptance();
#endif
                _viewModel.StatusMsg = "Exitoso";

                Dispatcher.Invoke(() => NavigateTo(new MainPublicityUC()));
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Ocurrió un error en tiempo de ejecución {ex.Message}", ex);
                await Retry(Messages.NO_SERVICE + " Ocurrió un error inesperado" + " Presiona continuar para intentar de nuevo.");
            }



        }

        private async Task Retry(string msgModal)
        {

            _nav.ShowModal(msgModal, new InfoModal());
            await InitPayPad();
        }

    }


    public class ConfigViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _statusMsg = string.Empty;
        public string StatusMsg
        {
            get
            {
                return _statusMsg;
            }
            set
            {
                if (_statusMsg != value)
                {
                    _statusMsg = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusMsg)));
                }

            }
        }

    }


}
