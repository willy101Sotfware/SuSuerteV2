using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.ApiService.Models;
using SuSuerteV2.Domain.Enumerables;
using System.ComponentModel;
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
        public ETypeTramites TypeTramite { get; set; }
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

        public string Name { get; set; }

        public bool DevueltaCorrecta { get; set; }
        public ulong Transacciondistribuidorid { get; set; }

        public string NombreUsuario { get; set; }
        public string Valor { get; set; }
        public int ValorGanar { get; set; }
        public string Fecha { get; set; }

        public float Iva { get; set; }
        public int ExtraMum { get; set; }
        public string Operador { get; set; }

        public string IdProduct { get; set; }

        public string NumOperator { get; set; }
        public string StatePay { get; set; }



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
        public PAYER Payer { get; set; }
        public ResponseConsultarCRMRegistro ResponseConsultarCRMRegistro { get; set; }
        public string NombreLoteria { get; set; }
        public string CodigoLoteria { get; set; }
        public string NumeroApostadoAstro { get; set; }
        public int ValorApostadoAstro { get; set; }
        public List<Chance> ListaChances { get; set; }

        public ResponseNotificarPaquetes ResponseNotificarPaquetes { get; set; }
        public Dictionary<string, int> DicSginosSeleccionados { get; set; } = new Dictionary<string, int>();

        public class Chance
        {
            public List<LotteriesViewModel> Loterias { get; set; }
            public DateTime FechaJuego { get; set; }

            // Valores de la apuesta
            public int Directo { get; set; }
            public int Combinado { get; set; }
            public int Pata { get; set; }
            public int Una { get; set; }

            public int TipoChance { get; set; }


            //Número jugado
            public string Numero { get; set; }

            //Total
            private int _Total { get; set; }

            public int GetTotalChance()
            {
                _Total = (Directo + Combinado + Pata + Una) * Loterias.Count;
                return _Total;
            }
        }

        private UserDataChance _userData { get; set; }
        public UserDataChance UserData
        {
            get
            {
                return _userData;
            }
            set
            {
                _userData = value;
                OnPropertyRaised(nameof(UserData));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        public class LotteriesViewModel
        {

            private string _Title;
            public string Title
            {
                get { return this._Title; }
                set { this._Title = value; }
            }

            private BitmapImage _ImageData;
            public BitmapImage ImageData
            {
                get { return this._ImageData; }
                set { this._ImageData = value; }
            }

            private string _ImageTag;
            public string ImageTag
            {
                get { return this._ImageTag; }
                set { this._ImageTag = value; }
            }

            private string _CodigoCodesa;
            public string CodigoCodesa
            {
                get { return this._CodigoCodesa; }
                set { this._CodigoCodesa = value; }
            }

            public string IdCodesa { get; set; }
            public string DescripcionLoteria { get; set; }
            public string Nombre { get; set; }
            public string NombreCorto { get; set; }




            private BitmapImage _ImageS;
            public BitmapImage ImageS
            {
                get { return this._ImageS; }
                set { this._ImageS = value; }
            }

            private BitmapImage _Image;
            public BitmapImage Image
            {
                get { return this._Image; }
                set { this._Image = value; }
            }

            private bool _IsSelected;
            public bool IsSelected
            {
                get { return this._IsSelected; }
                set
                {
                    this._IsSelected = value;
                    if (_IsSelected) this.ImageData = ImageS;
                    else this.ImageData = Image;
                }
            }
        }



        private CrmCreateRegistro _CrmCreate { get; set; }
        public CrmCreateRegistro CrmCreate
        {
            get
            {
                return _CrmCreate;
            }
            set
            {
                _CrmCreate = value;
                OnPropertyRaised(nameof(CrmCreate));
            }
        }





    }




    public class CrmCreateRegistro
    {
        public string IDENTIFICACION { get; set; }
        public string NOMBRES { get; set; }
        public string APELLIDOS { get; set; }
        public string FECHA_NACIMIENTO { get; set; }
        public string CELULAR { get; set; }
        public string EMAIL { get; set; }
        public string GENERO { get; set; }
        public string TIPO_IDENTIFICACION { get; set; }
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
