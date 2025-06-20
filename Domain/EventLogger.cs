using Newtonsoft.Json;
using SuSuerteV2.Domain.Variables;
using System.IO;
using System.Runtime.CompilerServices;

namespace SuSuerteV2.Domain
{
    public static class EventLogger
    {
        public static void SaveLog(EventType type, string msg, object? obj = null,
            [CallerMemberName] string method = "", [CallerFilePath] string callerPath = "")
        {
            var _class = Path.GetFileNameWithoutExtension(callerPath);
            var _event = new Event
            {
                Time = DateTime.Now.ToString("hh:mm:ss.fff tt"),
                IdTransaction = 0,
                Type = type.ToString(),
                Class = _class,
                Method = method,
                Message = msg,
                Obj = obj,
            };

            // Solo log de aplicación y periféricos
            if (type.ToString().StartsWith("P"))
                WriteFile(_event, "Log_peripherals");
            else
                WriteFile(_event, "Log_app");
        }

        private static void WriteFile(Event evt, string folder)
        {
            try
            {
                var json = JsonConvert.SerializeObject(evt, Formatting.Indented);

                var logDir = Path.Combine(AppInfo.APP_DIR, folder);
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                var fileName = "Log" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
                var filePath = Path.Combine(logDir, fileName);

                if (!File.Exists(filePath))
                {
                    using (var archivo = File.CreateText(filePath))
                    {
                        // El using se encarga del Close()
                    }
                }

                using (var sw = File.AppendText(filePath))
                {
                    sw.WriteLine(json);
                }
            }
            catch (Exception ex)
            {
                // Log crítico: si falla el logger, intentar escribir en un archivo de emergencia
                try
                {
                    var emergencyPath = Path.Combine(AppInfo.APP_DIR, "emergency_log.txt");
                    File.AppendAllText(emergencyPath,
                        $"[{DateTime.Now}] LOGGER ERROR: {ex.Message} - Original event: {evt.Type} - {evt.Message}\n");
                }
                catch
                {
                    // Si incluso el log de emergencia falla, no hay mucho más que hacer
                }
            }
        }
    }

    public class Event
    {
        public string Time { get; set; }
        public int IdTransaction { get; set; }
        public string Type { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }
        public object? Obj { get; set; }
    }

    public enum EventType
    {
        FatalError,
        Error,
        Warning,
        Info,
        P_Acceptor,
        P_Arduino,
        P_Dispenser
    }
}