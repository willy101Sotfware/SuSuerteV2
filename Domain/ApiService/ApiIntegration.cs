using Newtonsoft.Json;
using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SuSuerteV2.ApiService
{
    public class ApiIntegration
    {
        private static string _baseAddress;
        private static HttpClient _client;
        private static string _aplicacion;
        private static string _dispositivo;

        static ApiIntegration()
        {
            _baseAddress = AppConfig.Get("basseAddressIntegration");
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseAddress);
            _aplicacion = Assembly.GetCallingAssembly().GetName().Name;
        }

        private static async Task<ResponseGeneric> GetRequest(string endpoint, StringContent content)
        {
            try
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                    _client.BaseAddress = new Uri(_baseAddress);
                }

                _client.Timeout = TimeSpan.FromMilliseconds(25000);

                EventLogger.SaveLog(EventType.Info, $"Enviando solicitud POST a: {endpoint}", null);

                using (var tempClient = new HttpClient())
                {
                    tempClient.BaseAddress = _client.BaseAddress;
                    tempClient.Timeout = _client.Timeout;

                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    var response = await tempClient.PostAsync(endpoint, content);
                    EventLogger.SaveLog(EventType.Info, $"Código de estado HTTP: {response.StatusCode}", null);

                    var result = await response.Content.ReadAsStringAsync();
                    EventLogger.SaveLog(EventType.Info, $"Respuesta recibida: {result}", null);

                    if (string.IsNullOrEmpty(result))
                    {
                        EventLogger.SaveLog(EventType.Warning, "La respuesta está vacía", null);
                        return null;
                    }

                    try
                    {
                        var deserializedResponse = JsonConvert.DeserializeObject<ResponseGeneric>(result);
                        return deserializedResponse;
                    }
                    catch (Exception ex)
                    {
                        EventLogger.SaveLog(EventType.Error, $"Error al deserializar la respuesta: {ex.Message}. Respuesta: {result}", ex);
                        return null;
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Timeout al conectar con la API: {ex.Message}", ex);
                return null;
            }
            catch (HttpRequestException ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error de conexión HTTP: {ex.Message}", ex);
                return null;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en GetRequest: {ex.Message}", ex);
                return null;
            }
        }

        public static async Task<ResponseGeneric> GetDataFormulario(object requestData, string endpoint)
        {
            string payload = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            return await GetRequest(endpoint, content);
        }

        public static async Task<ResponseGetProducts> GetProductsBetPlay()
        {
            string endpoint = AppConfig.Get("GetProductsBetPlay");
            var response = await GetRequest(endpoint, new StringContent("", Encoding.UTF8, "application/json"));

            if (response != null)
            {
                var x = JsonConvert.SerializeObject(response.data);
                var data = JsonConvert.DeserializeObject<ResponseGetProducts>(x);
                EventLogger.SaveLog(EventType.Info, $"Respuesta al método GetProductsBetPlay: {x}", null);
                return data;
            }
            return null;
        }

        public static async Task<ResponseGetProducts> GetProductsChance()
        {
            string endpoint = AppConfig.Get("GetProductsChance");
            var response = await GetRequest(endpoint, new StringContent("", Encoding.UTF8, "application/json"));

            if (response != null)
            {
                var x = JsonConvert.SerializeObject(response.data);
                var data = JsonConvert.DeserializeObject<ResponseGetProducts>(x);
                EventLogger.SaveLog(EventType.Info, $"Respuesta al método GetProductsChance: {x}", null);
                return data;
            }
            return null;
        }

        public static async Task<ResponseGetLotteries> GetLotteries(RequestGetLotteries machine)
        {
            string endpoint = AppConfig.Get("GetLotteries");
            var payload = JsonConvert.SerializeObject(machine);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await GetRequest(endpoint, content);

            if (response != null)
            {
                var x = JsonConvert.SerializeObject(response.data);
                var data = JsonConvert.DeserializeObject<ResponseGetLotteries>(x);
                EventLogger.SaveLog(EventType.Info, $"Respuesta al método GetLotteries: {x}", null);
                return data;
            }
            return null;
        }

        public static async Task<ResponseTypeChance> TypeChance(IdProducto machine)
        {
            string endpoint = AppConfig.Get("TypeChance");
            var payload = JsonConvert.SerializeObject(machine);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await GetRequest(endpoint, content);

            if (response != null)   
            {
                var x = JsonConvert.SerializeObject(response.data);
                var data = JsonConvert.DeserializeObject<ResponseTypeChance>(x);
                EventLogger.SaveLog(EventType.Info, $"Respuesta al método TypeChance: {x}", null);
                return data;
            }
            return null;
        }

        public static async Task<ResponseValidateChance> ValidateChance(RequestValidateChance machine)
        {
            string endpoint = AppConfig.Get("ValidateChance");
            var payload = JsonConvert.SerializeObject(machine);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await GetRequest(endpoint, content);

            if (response != null)
            {
                var x = JsonConvert.SerializeObject(response.data);
                var data = JsonConvert.DeserializeObject<ResponseValidateChance>(x);
                EventLogger.SaveLog(EventType.Info, $"Respuesta al método ValidateChance: {x}", null);
                return data;
            }
            return null;
        }

        public static async Task<ResponseAwardsChance> AwardsChance(RequestAwardsChance machine)
        {
            string endpoint = AppConfig.Get("AwardsChance");
            var payload = JsonConvert.SerializeObject(machine);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await GetRequest(endpoint, content);

            if (response != null)
            {
                var x = JsonConvert.SerializeObject(response.data);
                var data = JsonConvert.DeserializeObject<ResponseAwardsChance>(x);
                EventLogger.SaveLog(EventType.Info, $"Respuesta al método AwardsChance: {x}", null);
                return data;
            }
            return null;
        }

        public static async Task<ResponseNotifyChance> NotifyChance(RequestNotifyChance machine)
        {
            string endpoint = AppConfig.Get("NotifyChance");
            var payload = JsonConvert.SerializeObject(machine);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await GetRequest(endpoint, content);

            if (response != null)
            {
                var x = JsonConvert.SerializeObject(response.data);
                string jsonLimpio = System.Text.RegularExpressions.Regex.Unescape(x).Trim('"');
                jsonLimpio = jsonLimpio.Replace(@"\", "");
                var data = JsonConvert.DeserializeObject<ResponseNotifyChance>(jsonLimpio);
                EventLogger.SaveLog(EventType.Info, $"Respuesta al método NotifyChance: {jsonLimpio}", null);
                return data;
            }
            return null;
        }

        // Continúa implementando otros métodos siguiendo el mismo patrón
    }
}
