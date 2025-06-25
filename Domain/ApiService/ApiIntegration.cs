using Newtonsoft.Json;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.Enumerables;
using SuSuerteV2.Domain.UIServices;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SuSuerteV2.Domain.ApiService
{
    public class ApiIntegration
    {
        private static string _baseAddress;
        private static readonly string _baseAddressMail;
        private static HttpClient _client;

    

        static ApiIntegration()
        {
            _baseAddress = AppConfig.Get("basseAddressIntegration");
            _baseAddressMail = AppConfig.Get("basseAddressMail");
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseAddress);
        }

     

        private static async Task<ResponseGeneric> GetRequest<T>(string endpoint, StringContent content)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_baseAddress);
            client.Timeout = TimeSpan.FromMilliseconds(30000);

            try
            {
                var response = await client.PostAsync(endpoint, content);
                var result = await response.Content.ReadAsStringAsync();

                if (result == null)
                {
                    EventLogger.SaveLog(EventType.Error, "No se obtuvo contenido de la API");
                    return null;
                }

                var requestResponse = JsonConvert.DeserializeObject<ResponseGeneric>(result);
                if (requestResponse == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error deserializando la respuesta");
                    return null;
                }

                if (response.IsSuccessStatusCode)
                {
                    return requestResponse;
                }

                EventLogger.SaveLog(EventType.Error, "API no respondió satisfactoriamente", requestResponse);
                return null;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en GetRequest: {ex.Message}");
                return null;
            }
        }

        private static async Task<ResponseGeneric> GetRequestData<T>(string endpoint)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_baseAddress);
            client.Timeout = TimeSpan.FromMilliseconds(30000);

            try
            {
                var content = new StringContent("dato", Encoding.UTF8, "Application/json");
                var response = await client.PostAsync(endpoint, content);
                var result = await response.Content.ReadAsStringAsync();

                if (result == null)
                {
                    EventLogger.SaveLog(EventType.Error, "No se obtuvo contenido de la API");
                    return null;
                }

                var requestResponse = JsonConvert.DeserializeObject<ResponseGeneric>(result);
                if (requestResponse == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error deserializando la respuesta");
                    return null;
                }

                if (response.IsSuccessStatusCode)
                {
                    return requestResponse;
                }

                EventLogger.SaveLog(EventType.Error, "API no respondió satisfactoriamente", requestResponse);
                return null;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en GetRequestData: {ex.Message}");
                return null;
            }
        }


        public static async Task<ResponseGetProducts> GetProductsBetPlay()
        {
            try
            {
                string controller = AppConfig.Get("GetProductsBetPlay");
                var response = await GetRequestData<ResponseGeneric>(controller);

                if (response != null)
                {
                    var jsonData = JsonConvert.SerializeObject(response.data);
                    var data = JsonConvert.DeserializeObject<ResponseGetProducts>(jsonData);

                    EventLogger.SaveLog(EventType.Info, "GetProductsBetPlay ejecutado correctamente", jsonData);
                    return data;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en GetProductsBetPlay: {ex.Message}");
            }
            return null;
        }

        public static async Task<ResponseTokenBetplay> GetTokenBetplay(RequesttokenBetplay requesttoken)
        {
            try
            {
                string controller = AppConfig.Get("GetTokenBetplay");
                string payload = JsonConvert.SerializeObject(requesttoken);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");

                var response = await GetRequest<ResponseGeneric>(controller, content);

                if (response != null)
                {
                    var jsonData = JsonConvert.SerializeObject(response.data);
                    var data = JsonConvert.DeserializeObject<ResponseTokenBetplay>(jsonData);

                    EventLogger.SaveLog(EventType.Info, "GetTokenBetplay ejecutado correctamente", jsonData);

                    if (data.Token != null)
                    {
                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en GetTokenBetplay: {ex.Message}");
            }
            return null;
        }

        public static async Task<ResponseGetProducts> GetConsultBetplay(RequestConsultSubproductBetplay request)
        {
            try
            {
                string controller = AppConfig.Get("GetConsultBetplay");
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");

                var response = await GetRequest<ResponseGeneric>(controller, content);

                if (response != null)
                {
                    var jsonData = JsonConvert.SerializeObject(response.data);
                    var data = JsonConvert.DeserializeObject<ResponseGetProducts>(jsonData);

                    EventLogger.SaveLog(EventType.Info, "GetConsultBetplay ejecutado correctamente", jsonData);
                    return data;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en GetConsultBetplay: {ex.Message}");
            }
            return null;
        }

        public static async Task<ResponseNotifyBetPlay> NotifyPaymentBetplay(RequestNotifyBetplay request)
        {
            try
            {
                string controller = AppConfig.Get("NotifyRechargeBetplay");
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");

                var response = await GetRequest<ResponseGeneric>(controller, content);

                if (response != null)
                {
                    var jsonData = JsonConvert.SerializeObject(response.data);
                    string jsonLimpio = System.Text.RegularExpressions.Regex.Unescape(jsonData).Trim('"');
                    jsonLimpio = jsonLimpio.Replace(@"\", "");

                    var data = JsonConvert.DeserializeObject<ResponseNotifyBetPlay>(jsonLimpio);

                    EventLogger.SaveLog(EventType.Info, "NotifyPaymentBetplay ejecutado correctamente", jsonLimpio);
                    return data;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en NotifyPaymentBetplay: {ex.Message}");
            }
            return null;
        }



        public static async Task<ResponseGetProducts> GetProductsChance()
        {
            try
            {
                string controller = AppConfig.Get("GetProductsChance");
                var response = await GetRequestData<ResponseGeneric>(controller);

                if (response != null)
                {
                    var jsonData = JsonConvert.SerializeObject(response.data);
                    var data = JsonConvert.DeserializeObject<ResponseGetProducts>(jsonData);

                    EventLogger.SaveLog(EventType.Info, "GetProductsChance ejecutado correctamente", jsonData);
                    return data;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en GetProductsChance: {ex.Message}");
            }
            return null;
        }

        public static async Task<ResponseGetLotteries> GetLotteries(RequestGetLotteries request)
        {
            try
            {
                string controller = AppConfig.Get("GetLotteries");
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");

                var response = await GetRequest<ResponseGeneric>(controller, content);

                if (response != null)
                {
                    var jsonData = JsonConvert.SerializeObject(response.data);
                    var data = JsonConvert.DeserializeObject<ResponseGetLotteries>(jsonData);

                    EventLogger.SaveLog(EventType.Info, "GetLotteries ejecutado correctamente", jsonData);
                    return data;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en GetLotteries: {ex.Message}");
            }
            return null;
        }

        public static async Task<ResponseTypeChance> TypeChance(IdProducto request)
        {
            try
            {
                string controller = AppConfig.Get("TypeChance");
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");

                var response = await GetRequest<ResponseGeneric>(controller, content);

                if (response != null)
                {
                    var jsonData = JsonConvert.SerializeObject(response.data);
                    var data = JsonConvert.DeserializeObject<ResponseTypeChance>(jsonData);

                    EventLogger.SaveLog(EventType.Info, "TypeChance ejecutado correctamente", jsonData);
                    return data;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en TypeChance: {ex.Message}");
            }
            return null;
        }

        public static async Task<ResponseValidateChance> ValidateChance(RequestValidateChance request)
        {
            try
            {
                string controller = AppConfig.Get("ValidateChance");
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");

                var response = await GetRequest<ResponseGeneric>(controller, content);

                if (response != null)
                {
                    var jsonData = JsonConvert.SerializeObject(response.data);
                    var data = JsonConvert.DeserializeObject<ResponseValidateChance>(jsonData);

                    EventLogger.SaveLog(EventType.Info, "ValidateChance ejecutado correctamente", jsonData);
                    return data;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en ValidateChance: {ex.Message}");
            }
            return null;
        }

        public static async Task<ResponseAwardsChance> AwardsChance(RequestAwardsChance request)
        {
            try
            {
                string controller = AppConfig.Get("AwardsChance");
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");

                var response = await GetRequest<ResponseGeneric>(controller, content);

                if (response != null)
                {
                    var jsonData = JsonConvert.SerializeObject(response.data);
                    var data = JsonConvert.DeserializeObject<ResponseAwardsChance>(jsonData);

                    EventLogger.SaveLog(EventType.Info, "AwardsChance ejecutado correctamente", jsonData);
                    return data;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en AwardsChance: {ex.Message}");
            }
            return null;
        }

        public static async Task<ResponseNotifyChance> NotifyChance(RequestNotifyChance request)
        {
            try
            {
                string controller = AppConfig.Get("NotifyChance");
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");

                var response = await GetRequest<ResponseGeneric>(controller, content);

                if (response != null)
                {
                    var jsonData = JsonConvert.SerializeObject(response.data);
                    string jsonLimpio = System.Text.RegularExpressions.Regex.Unescape(jsonData).Trim('"');
                    jsonLimpio = jsonLimpio.Replace(@"\", "");

                    var data = JsonConvert.DeserializeObject<ResponseNotifyChance>(jsonLimpio);

                    EventLogger.SaveLog(EventType.Info, "NotifyChance ejecutado correctamente", jsonLimpio);
                    return data;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en NotifyChance: {ex.Message}");
            }
            return null;
        }

        public async Task<ResponseConsultarCRMRegistro> ConsultCrmRegistroRecargas(RequestConsultCrmRegistro request)
        {
            try
            {


                var y = JsonConvert.SerializeObject(request);
                EventLogger.SaveLog(EventType.Info, "ApiIntegration", "entrando a la ejecucion ConsultParametrosRecargas Request", y);


                string controller = AppConfig.Get("ConsultarCRMRegistro");

                _baseAddress = AppConfig.Get("basseAddressIntegration");

                var response = await GetRequestData<ResponseGeneric>(controller);
               

                if (response != null)
                {
                    var  x = JsonConvert.SerializeObject(response.data);
                 
                    EventLogger .SaveLog(EventType.Info, "ApiIntegration", "entrando a la ejecucion ConsultParametrosRecargas Response", x);
              

                  
                    if (response.data != null && !string.IsNullOrEmpty(x) && x != "null" && x != "[]")
                    {

                        string jsonLimpio = System.Text.RegularExpressions.Regex.Unescape(x).Trim('"');

                        jsonLimpio = jsonLimpio.Replace(@"\", "");

                        var ResponseData = JsonConvert.DeserializeObject<ResponseConsultarCRMRegistro>(jsonLimpio);

                        return ResponseData;


                    }
                    else
                    {

                        return null;
                    }



                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "ApiIntegration", "entrando a la ejecucion ConsultParametrosRecargas Catch", ex.Message);
               
            }
            return null;
        }

        public async Task<ResponseConsultarAstro> ConsultarSorteo(RequestConsultarSorteo Machine)
        {
            try
            {
                string controller = AppConfig.Get("Consultarsorteo");

                _baseAddress = AppConfig.Get("basseAddressIntegration");
                var response = await GetRequestData<ResponseGeneric>(controller);

            

                if (response != null)
                {
                    var x = JsonConvert.SerializeObject(response.data);

                    var requestData = JsonConvert.DeserializeObject<ResponseConsultarAstro>(x);
                    if (requestData == null)
                    {
                        EventLogger.SaveLog(EventType.Error, "ApiIntegration", "Respuesta al metodo ConsultarSorteo es nula o vacia", x);
                        return null;
                    }


                    return requestData;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "ApiIntegration", "entrando a la ejecucion ConsultarSorteo Catch", ex.Message);
               
            }
            return null;
        }

        public static async Task<ResponseGetRecaudo> GetRecaudos(RequestGetRecaudos request)
        {
            try
            {
                string controller = AppConfig.Get("GetRecaudo");
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");

                var response = await GetRequest<ResponseGeneric>(controller, content);

                if (response != null)
                {
                    var jsonData = JsonConvert.SerializeObject(response.data);
                    var data = JsonConvert.DeserializeObject<ResponseGetRecaudo>(jsonData);

                    EventLogger.SaveLog(EventType.Info, "GetRecaudos ejecutado correctamente", jsonData);
                    return data;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en GetRecaudos: {ex.Message}");
            }
            return null;
        }

        public static async Task<ResponseGetParameters> GetParameters(RequestGetParameters request)
        {
            try
            {
                string controller = AppConfig.Get("GetParameters");
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");

                var response = await GetRequest<ResponseGeneric>(controller, content);

                if (response != null)
                {
                    var jsonData = JsonConvert.SerializeObject(response.data);
                    var data = JsonConvert.DeserializeObject<ResponseGetParameters>(jsonData);

                    EventLogger.SaveLog(EventType.Info, "GetParameters ejecutado correctamente", jsonData);
                    return data;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en GetParameters: {ex.Message}");
            }
            return null;
        }

        public static async Task<ResponseConsultValue> ConsultValueRecaudo(RequestConsultValue request)
        {
            try
            {
                string controller = AppConfig.Get("ConsultRecaudo");
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");

                var response = await GetRequest<ResponseGeneric>(controller, content);

                if (response != null)
                {
                    var jsonData = JsonConvert.SerializeObject(response.data);
                    var data = JsonConvert.DeserializeObject<ResponseConsultValue>(jsonData);

                    EventLogger.SaveLog(EventType.Info, "ConsultValueRecaudo ejecutado correctamente", jsonData);
                    return data;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en ConsultValueRecaudo: {ex.Message}");
            }
            return null;
        }

        public static async Task<ResponseNotifyPayment> NotifyPaymentRecaudo(RequestNotifyRecaudo request)
        {
            try
            {
                string controller = AppConfig.Get("NotifyPay");
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");

                var response = await GetRequest<ResponseGeneric>(controller, content);

                if (response != null)
                {
                    var jsonData = JsonConvert.SerializeObject(response.data);
                    string jsonLimpio = System.Text.RegularExpressions.Regex.Unescape(jsonData).Trim('"');
                    jsonLimpio = jsonLimpio.Replace(@"\", "");

                    var data = JsonConvert.DeserializeObject<ResponseNotifyPayment>(jsonLimpio);

                    EventLogger.SaveLog(EventType.Info, "NotifyPaymentRecaudo ejecutado correctamente", jsonLimpio);
                    return data;
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en NotifyPaymentRecaudo: {ex.Message}");
            }
            return null;
        }


        public async Task sendMail(string userMail, byte[] bytesPDF, Transaction transaction)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(AppConfig.Get("basseAddressMail"));

            string Enpoint = AppConfig.Get("SendMail");
            string notificacion = transaction.Type == ETypeTramites.Recaudos ? "Recaudo" :
                                  transaction.Type == ETypeTramites.Chance ? "Chance" :
                                  transaction.Type == ETypeTramites.BetPlay ? "Betplay" :
                                  transaction.Type == ETypeTramites.RecargasCel ? "Recarga celular" :
                                  transaction.Type == ETypeTramites.PaquetesCel ? "Paquete celular" :
                                  transaction.Type == ETypeTramites.Astro ? "SUPER astro" :
                                   "";

            string nombreEmpresa = transaction.Type == ETypeTramites.Recaudos ? transaction.ResponseNotifyPayment.Empresa.Nombre :
                                   transaction.Type == ETypeTramites.Chance ? transaction.ResponseNotifyChance.Empresa.Nombre :
                                   transaction.Type == ETypeTramites.BetPlay ? transaction.ResponseNotifyBetplay.Empresa.Nombre :
                                   transaction.Type == ETypeTramites.RecargasCel ? transaction.ResponseNotificacionRecargaCel.Empresa.Nombre :
                                   transaction.Type == ETypeTramites.Astro ? transaction.ResponseVentaAstro.Empresa.Nombre :
                                   transaction.Type == ETypeTramites.PaquetesCel ? "SUSUERTE S.A." :
                                   "SUSUERTE S.A";

            decimal nitEmpresa = transaction.Type == ETypeTramites.Recaudos ? transaction.ResponseNotifyPayment.Empresa.Nit :
                                 transaction.Type == ETypeTramites.Chance ? transaction.ResponseNotifyChance.Empresa.Nit :
                                 transaction.Type == ETypeTramites.BetPlay ? Convert.ToDecimal(transaction.ResponseNotifyBetplay.Empresa.Nit) :
                                 transaction.Type == ETypeTramites.RecargasCel ? Convert.ToDecimal(transaction.ResponseNotificacionRecargaCel.Empresa.Nit) :
                                 transaction.Type == ETypeTramites.Astro ? Convert.ToDecimal(transaction.ResponseVentaAstro.Empresa.Nit) :
                                  transaction.Type == ETypeTramites.PaquetesCel ? 8100003178 :
                                   8100003178;



            string direccionEmpresa = transaction.Type == ETypeTramites.Recaudos ? transaction.ResponseNotifyPayment.Empresa.Direccion :
                                      transaction.Type == ETypeTramites.Chance ? transaction.ResponseNotifyChance.Empresa.Direccion :
                                      transaction.Type == ETypeTramites.BetPlay ? transaction.ResponseNotifyBetplay.Empresa.Direccion :
                                       transaction.Type == ETypeTramites.RecargasCel ? transaction.ResponseNotificacionRecargaCel.Empresa.Direccion :
                                       transaction.Type == ETypeTramites.Astro ? transaction.ResponseVentaAstro.Empresa.Direccion :
                                        transaction.Type == ETypeTramites.PaquetesCel ? "CRA 23C NO 64-32 MANIZALES" :
                                       "CRA 23C NO 64-32 MANIZALES";

            string Message = transaction.Type == ETypeTramites.Recaudos ? $"fue realizado exitosamente al servicio de {transaction.Company.Nombre} con fecha del" : "fue realizado exitosamente";
            string Fecha = transaction.Type == ETypeTramites.Recaudos ? DateTime.Now.ToString("dd-MM-yyyy") :
                           transaction.Type == ETypeTramites.Recaudos ? transaction.ResponseNotifyChance.Chance.Fechasorteo :
                           transaction.Type == ETypeTramites.BetPlay ? transaction.ResponseNotifyBetplay.Fecha :
                            transaction.Type == ETypeTramites.Astro ? transaction.ResponseVentaAstro.Fecha :
                           DateTime.Now.ToString("dd-MM-yyyy");

            decimal valor = transaction.Type == ETypeTramites.BetPlay ? (decimal)transaction.ResponseNotifyBetplay.ValorRecaudo :
                             transaction.Type == ETypeTramites.RecargasCel ? Convert.ToDecimal(transaction.Total) :
                             transaction.Type == ETypeTramites.PaquetesCel ? Convert.ToDecimal(transaction.Total) :
                              transaction.Type == ETypeTramites.Astro ? Convert.ToDecimal(transaction.Total) :
                             Convert.ToDecimal(transaction.Total);
            string formattedValue = valor.ToString("C0", CultureInfo.CurrentCulture);


            string htmlBody = $@"<!DOCTYPE html>
            <html lang=""es"">
            <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Notificación de {notificacion}</title>
            <style>
                *{{margin:0}}
                body {{ font-family: Arial, sans-serif; background-color: #fff; }}
                .container {{ width: 90%; margin: auto; padding: 20px; }}
            </style>
            </head>
            <body>
            <div class=""container"">
                <div style=""padding:2rem"">
                <h2 style=""color:#69c6d8"">{nombreEmpresa}</h2>
                <p  style=""color:#69c6d8; font-weight:bold"">Nit <span  style=""color:#0c739f; font-weight:bold"">{nitEmpresa}</span></p>
                <p style=""color:#69c6d8; font-weight:bold"">{direccionEmpresa}</p>
                <p style=""color:#69c6d8; font-weight:bold"">CONTRATO DE CONCESION No 001 de 2021</p>
                <h1>Notificación de {notificacion}</h1>
    
                </div>
                <div class=""content"">

                <p>Apreciado cliente de acuerdo a su solicitud, su {notificacion}  {Message} <span style=""font-weight:bold; color:#0c739f"">{Fecha}</span> por valor de <span style=""font-weight:bold"">{formattedValue}</span>.</p>
                <p style=""font-weight:bold"">Se adjunta certificado de operación</p>
                <p>Si presenta inquietudes puede comunicarse con la línea de servicio al cliente <span style=""font-weight:bold"">300 912 5787</span></p>          
                </div>
            </div>
            </body>
            </html>";

            // Obtener la clave API de Mailgun
            var apiKey = Environment.GetEnvironmentVariable("MAILGUN_API_KEY") ?? AppConfig.Get("MailgunApiKey"); 
            // Si la clave es un placeholder, intentar usar un archivo local seguro
            if (apiKey == "MAILGUN_API_KEY_PLACEHOLDER")
            {
                try {
                    // Intentar leer de un archivo local que no esté en el repositorio
                    string secretsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mailgun_key.txt");
                    if (File.Exists(secretsPath))
                    {
                        apiKey = File.ReadAllText(secretsPath).Trim();
                        EventLogger.SaveLog(EventType.Info, "Usando clave API de Mailgun desde archivo local.");
                    }
                    else 
                    {
                        EventLogger.SaveLog(EventType.Warning, "No se encontró archivo de clave API. Cree mailgun_key.txt en el directorio de la aplicación.");
                        // Usar una clave vacía que no funcionará pero evita errores de compilación
                        apiKey = "key-placeholder";
                    }
                } catch (Exception ex) {
                    EventLogger.SaveLog(EventType.Error, $"Error al leer clave API: {ex.Message}");
                    apiKey = "key-placeholder";
                }
            }
            var base64ApiKey = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}"));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64ApiKey);
            var content = new MultipartFormDataContent
                {
                    {new StringContent(AppConfig.Get("from")), "from"},
                    {new StringContent(userMail), "to" },
                    {new StringContent("Comprobante de compra: Detalles de tu transacción"), "subject" },
                    {new StringContent(htmlBody), "html"},
                    {new ByteArrayContent(bytesPDF), "attachment", "Comprobante.pdf"}
                };

            var response = await _client.PostAsync(Enpoint, content);
            var result = await response.Content.ReadAsStringAsync();

            EventLogger.SaveLog(EventType.Info, $"Respuesta Correo: {result} - Status: {response.StatusCode}");

            if (result == null) throw new Exception("No se logró enviar su comprobante, por favor comunicate con un asesor.");
            if (response.StatusCode != HttpStatusCode.OK) throw new Exception("No se logró enviar su comprobante, por favor comunicate con un asesor.");

        }

    }
}
