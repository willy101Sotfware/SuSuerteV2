using SuSuerteV2.Domain;
using SuSuerteV2.UserControls;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace SuSuerteV2.Presentation.UserControls
{
    public partial class MainPublicityUC : AppUserControl
    {
        private ImageSleader _imageSleader;

        public MainPublicityUC()
        {
            InitializeComponent();
            Loaded += (s, e) => InitializePublicity();
        }

        private void InitializePublicity()
        {
            try
            {
                ConfiguratePublish();
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error,
                    "Error al inicializar publicidad",
                    ex,
                    nameof(InitializePublicity));
            }
        }

        private void ConfiguratePublish()
        {
            try
            {
                if (_imageSleader == null)
                {
                    string folder = Path.Combine(
                        Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                        "Images",
                        "Publish");

                    if (!Directory.Exists(folder))
                    {
                        EventLogger.SaveLog(EventType.Warning,
                            $"Carpeta de publicidad no encontrada: {folder}",
                            null,
                            nameof(ConfiguratePublish));
                        return;
                    }

                    List<string> imageFiles = Directory.GetFiles(folder, "*.jpg")
                                         .Concat(Directory.GetFiles(folder, "*.png"))
                                         .ToList();

                    if (imageFiles.Count == 0)
                    {
                        EventLogger.SaveLog(EventType.Warning,
                            "No se encontraron imágenes en la carpeta de publicidad",
                            new { Folder = folder },
                            nameof(ConfiguratePublish));
                        return;
                    }

                    _imageSleader = new ImageSleader(imageFiles, folder);
                    this.DataContext = _imageSleader.imageModel;

                    if (!int.TryParse(AppConfig.Get("TimerPublicity"), out int timerValue))
                    {
                        timerValue = 5000; // Valor por defecto
                        EventLogger.SaveLog(EventType.Warning,
                            "Valor inválido para TimerPublicity. Usando valor por defecto",
                            new { ConfigValue = AppConfig.Get("TimerPublicity") },
                            nameof(ConfiguratePublish));
                    }

                    _imageSleader.time = timerValue;
                    _imageSleader.isRotate = true;
                    _imageSleader.Start();

                    EventLogger.SaveLog(EventType.Info,
                        "Publicidad inicializada correctamente",
                        new
                        {
                            ImageCount = imageFiles.Count,
                            TimerInterval = timerValue
                        },
                        nameof(ConfiguratePublish));
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error,
                    "Error al configurar publicidad",
                    ex,
                    nameof(ConfiguratePublish));
                throw; // Re-lanzar para manejo superior si es necesario
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                GC.Collect();
                 Dispatcher.Invoke(() => NavigateTo(new MenuUC()));
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error,
                    "Error en interacción de publicidad",
                    ex,
                    nameof(Grid_MouseDown));
            }
        }
    }
}