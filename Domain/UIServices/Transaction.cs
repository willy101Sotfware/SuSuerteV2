using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.ApiService.Models;
using SuSuerteV2.Domain.Enumerables;
using System.Windows.Media.Imaging;

namespace SuSuerteV2.Domain.UIServices
{
    public class Transaction
    {
        // Patron de Diseño Singleton
        private static Transaction? _instance;
        public static Transaction Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Transaction();
                return _instance;
            }
        }

        public static void Reset()
        {
            _instance = null;
        }

        private Transaction() { }

        public TransactionDto ApiDto { get; set; }
        public int IdTransaccionApi { get; set; }

        public int IdPaypad { get; set; } = 3;
        public string? TipoRecaudo { get; set; }
        public string? TipoOperacion { get; set; }
        public TypeTransaction TipoTransaccion { get; set; }
        public TypePayment TipoPago { get; set; }
        public StateTransaction EstadoTransaccion { get; set; }

        public ETransactionType Tipo { get; set; }
        public string EstadoTransaccionVerb { get; set; }
        public string? Referencia { get; set; }
        public string? Documento { get; set; }
        public string? Descripcion { get; set; }
        public string? FechaVencimiento { get; set; }
        public decimal TotalSinRedondear { get; set; }
        public decimal Total { get; set; }
        public decimal TotalDevuelta { get; set; }
        public decimal TotalIngresado { get; set; }

        public bool DevueltaCorrecta { get; set; }
        public ulong Transacciondistribuidorid { get; set; }

        public ETypeTramites Type { get; set; }
        public ResponseNotifyPayment ResponseNotifyPayment { get; set; }
        public List<Listadotipochancefield> TypeChance { get; set; }

        public ResponseTypeChance TipoChance { get; set; }

        public ApuestaValidate ApuestaValidate { get; set; }

        public ResponseNotifyChance ResponseNotifyChance { get; set; }

        public ResponseNotifyBetPlay ResponseNotifyBetplay { get; set; }

        public ResponseConsultarAstro ResponseConsultarAstro { get; set; }
        public ResponseConsultarSignos ResponseConsultarSignos { get; set; }
        public ResponseVentaAstro ResponseVentaAstro { get; set; }

        public ResponseNotificacionRecargaCel ResponseNotificacionRecargaCel { get; set; }
        public DataCompany Company { get; set; }
        public SubProductoGeneral ProductSelected { get; set; }
        public RecaudoResponse listadorecaudosField { get; set; }
        public string tramite { get; set; }
        public ResponseTokenBetplay ResponseTokenBetplay { get; set; }

    }


    public class DataCompany
    {
        public int Codigo { get; set; }

        public int CodigoSubProducto { get; set; }

        public float Iva { get; set; }

        public string Nombre { get; set; }

        private BitmapImage _ImageData;
        public BitmapImage ImageData
        {
            get { return this._ImageData; }
            set { this._ImageData = value; }
        }
    }
}
