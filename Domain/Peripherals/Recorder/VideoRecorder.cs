using Newtonsoft.Json;
using SuSuerteV2.Domain.ApiService.Models;
using SuSuerteV2.Domain.UIServices;
using System.Net.Http;
using System.Text;

namespace SuSuerteV2.Domain.Peripherals.Recorder
{
    public static class VideoRecorder
    {
         
        const string START = "start";
        const string STOP = "stop";
        private static string _baseAddress;
        private static HttpClient _client;

        static VideoRecorder()
        {
            _baseAddress = "http://localhost:5000/";
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseAddress);
        }
        public static async Task<bool> Start(int source = 0)
        {
            var ts = Transaction.Instance;

            var filename = $"{ts.IdPaypad}_{ts.IdTransaccionApi}_{DateTime.Now.ToString("ddMMyyyy")}";

            var dataRequest = new
            {
                filename,
                source
            };

            return await RequestControlRecorder(dataRequest, START);

        }

        public static async Task<bool> Stop(int source = 0)
        {
            var dataRequest = new
            {
                source
            };

            return await RequestControlRecorder(dataRequest, STOP);
        }

        private static async Task<bool> RequestControlRecorder(object dataRequest, string endpoint)
        {
            try
            {
                string dataRequestStr = JsonConvert.SerializeObject(dataRequest);

                var content = new StringContent(dataRequestStr, Encoding.UTF8, "Application/json");

                var response = await _client.PostAsync(endpoint, content);

                var result = await response.Content.ReadAsStringAsync();
                if (result == null)
                {
                    EventLogger.SaveLog(EventType.Error, "No se obtuvo contenido de la api de grabador");
                    return false;

                }

                var requestresponse = JsonConvert.DeserializeObject<ApiResponse<string>>(result);
                if (requestresponse == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error deserializando la respuesta de la api de grabador");
                    return false;
                }

                if (requestresponse.statusCode == 200)
                {
                    EventLogger.SaveLog(EventType.Info, $"Api de grabador respondió correctamente {requestresponse.message}");
                    return true;
                }

                EventLogger.SaveLog(EventType.Error, "Api de video no respondió satisfactoriamente", requestresponse);
                return false;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Ocurrió un error en tiempo de ejecución {ex.Message}", ex);
                return false;
            }

        }

    }
}
