using System.IO.Ports;

namespace SuSuerteV2.Domain.ApiService.Models
{
    public class ControlScanner
    {
        #region Serial ports
        private SerialPort _BarcodeReader;
        #endregion

        #region Callbacks
        public Action<string> callbackScanner;
        public Action<string> callbackErrorScanner;
        public Action<DataDocument> callbackOut;//Calback para cuando sale cieerta cantidad del dinero
        #endregion

        #region Variables
        public int flagScanner = 0;
        #endregion

        public ControlScanner()
        {
            if (_BarcodeReader == null)
            {
                _BarcodeReader = new SerialPort();
            }
        }

        #region Methods
        public void Start()
        {
            try
            {
                if (_BarcodeReader != null)
                {
                    InitializePortBarcode(AppConfig.Get("PortBarcode"), 9600);
                    //InitializePortBarcode(AdminPayPlus.DataPayPlus.PayPadConfiguration.scanneR_PORT, 9600);
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        ///  Método para inciar el puerto del scanner
        /// </summary>
        public void InitializePortBarcode(string portName, int barcodeBaudRate)
        {
            try
            {
                if (!_BarcodeReader.IsOpen)
                {
                    _BarcodeReader.PortName = portName;
                    _BarcodeReader.BaudRate = barcodeBaudRate;
                    _BarcodeReader.Open();
                    _BarcodeReader.ReadTimeout = 200;
                    //_BarcodeReader.DtrEnable = true;
                    //_BarcodeReader.RtsEnable = true;
                    _BarcodeReader.DataReceived += new SerialDataReceivedEventHandler(Scanner_DataReceived);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void ClosePortScanner()
        {
            if (_BarcodeReader.IsOpen)
            {
                _BarcodeReader.Close();
            }
        }
        #endregion

        #region Listeners

        /// <summary>
        /// Método que escucha la respuesta del puerto del scanner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scanner_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (flagScanner == 0)
                {
                    flagScanner = 1;
                    var data = _BarcodeReader.ReadExisting();
                    proccessResponseScanner(data);
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Proccess Responses
        private void proccessResponseScanner(string response)
        {
            try
            {

                EventLogger.SaveLog(EventType.Info,"Data Scanner: " + response);

                if (Convert.ToBoolean(AppConfig.Get("EsMaquinaGrande")))
                {
                    ProcessResponseBarcode(response);
                    return;

                }
                callbackScanner?.Invoke(response.ToString());


            }
            catch (Exception ex)
            {
            }
        }

        private void ProcessResponseBarcode(string response)
        {
            try
            {
                if (!string.IsNullOrEmpty(response))
                {
                    var dataReader = new DataDocument();

                    response = response.Remove(0, response.IndexOf("PubDSK_1") + 6);

                    string documentData = string.Empty;
                    string fullName = string.Empty;

                    if (response.IndexOf("0M") > 0)
                    {
                        dataReader.Gender = "Masculino";
                        dataReader.Date = response.Substring(response.IndexOf("0M") + 2, 8);
                        documentData = response.Substring(0, response.IndexOf("0M"));
                    }
                    else
                    {
                        documentData = response.Substring(0, response.IndexOf("0F"));
                        dataReader.Date = response.Substring(response.IndexOf("0F") + 2, 8);
                        dataReader.Gender = "Femenino";
                    }

                    foreach (var item in documentData.ToCharArray())
                    {
                        if (char.IsLetter(item))
                        {
                            fullName += item;
                        }
                        else if (char.IsWhiteSpace(item) || item.Equals('\0'))
                        {
                            fullName += " ";
                        }
                        else if (char.IsNumber(item))
                        {
                            dataReader.Document += item;
                        }
                    }
                    fullName = (fullName.TrimStart()).TrimEnd();

                    dataReader.Document = dataReader.Document.Substring(dataReader.Document.Length - 10, 10);

                    foreach (var item in fullName.Split(' '))
                    {
                        if (!string.IsNullOrEmpty(item) && item.Length > 1)
                        {
                            dataReader.FullName += string.Concat(item, " ");
                        }
                    }

                    dataReader.FirstName = dataReader.FullName.Split(' ')[2] ?? string.Empty;
                    dataReader.SecondName = dataReader.FullName.Split(' ')[3] ?? string.Empty;
                    dataReader.LastName = dataReader.FullName.Split(' ')[0] ?? string.Empty;
                    dataReader.SecondLastName = dataReader.FullName.Split(' ')[1] ?? string.Empty;

                    if (!string.IsNullOrEmpty(dataReader.Document) && !string.IsNullOrEmpty(dataReader.FullName))
                    {
                        callbackOut?.Invoke(dataReader);
                    }

                }
                else
                {
                    //         callbackError?.Invoke("no se logro realizar la lectura");
                }
            }
            catch (Exception ex)
            {
                //         callbackError?.Invoke(ex.ToString());
            }
        }

        #endregion
    }

    public class DataDocument
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public string Date { get; set; }
        public string Gender { get; set; }
        public string Document { get; set; }
    }
}
