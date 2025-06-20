using Newtonsoft.Json;

namespace SuSuerteV2.Domain.ApiService.IntegrationModels
{
    public class RequestGetLotteries
    {
        public int Id { get; set; }

        public string FechaS { get; set; }

        public long transaccion { get; set; }
    }


    public class IdProducto
    {
        public int Id { get; set; }

        public string FechaS { get; set; }

        public long transaccion { get; set; }
    }




    public class RequestAwardsChance
    {
        public bool EsValorFijo { get; set; }

        public ListApuestasAwardsR LstApuestas { get; set; }

        public int Transaccion { get; set; }
    }

    public class ListApuestasAwardsR
    {
        public List<ApuestasAwardsR> apuestas { get; set; }
    }

    public class ApuestasAwardsR
    {
        public double iva { get; set; }

        public double ValorDirecto { get; set; }

        public double ValorCombinado { get; set; }
        public double ValorPata { get; set; }
        public double ValorUna { get; set; }

        public TipoChanceAwardsR tipoChance { get; set; }

    }

    public class TipoChanceAwardsR
    {
        public long Id { get; set; }
    }

 

    public class RequestValidateChance
    {
        public SubproductoValidate Subproducto { get; set; }

        public ListApuestasValidate LstApuestas { get; set; }

        public bool AsumeIva { get; set; }

        public string FechaSorteo { get; set; }

        public int Transaccion { get; set; }
    }

    public class SubproductoValidate
    {
        public int Id { get; set; }

        public long CodigoServicio { get; set; }
    }

    public class ListApuestasValidate
    {
        public List<ApuestasValidate> apuestas { get; set; }
    }

    public class ApuestasValidate
    {
        public int id { get; set; }

        public string NumeroApostado { get; set; }

        public int ValorDirecto { get; set; }

        public int ValorCombinado { get; set; }
        public int ValorPata { get; set; }
        public int ValorUna { get; set; }

        public TipoChanceValidateM tipoChance { get; set; }

        public ListLoteriasValidate ListLoteriasValidate { get; set; }
    }

    public class TipoChanceValidateM
    {
        public int Id { get; set; }
    }

    public class ListLoteriasValidate
    {
        public List<LoteriaValidate> loteria { get; set; }
    }

    public class LoteriaValidate
    {
        public int codigo { get; set; }
    }



    public class RequestNotifyChance
    {
        public SubproductoNotify Subproducto { get; set; }

        public ListApuestasNotify LstApuestas { get; set; }

        public bool AsumeIva { get; set; }

        public string FechaSorteo { get; set; }

        public int Transaccion { get; set; }

        public string codigoApostar { get; set; }

        public string idPagador { get; set; }

        public string cedula { get; set; }
    }

    public class SubproductoNotify
    {
        public int Id { get; set; }

        public long CodigoServicio { get; set; }
    }

    public class ListApuestasNotify
    {
        public List<ApuestasNotify> apuestas { get; set; }
    }

    public class ApuestasNotify
    {
        public int id { get; set; }

        public string NumeroApostado { get; set; }

        public int ValorDirecto { get; set; }

        public int ValorCombinado { get; set; }
        public int ValorPata { get; set; }
        public int ValorUna { get; set; }

        public TipoChanceNotifyM tipoChance { get; set; }

        public ListLoteriasNotify ListLoteriasValidate { get; set; }
    }

    public class TipoChanceNotifyM
    {
        public int Id { get; set; }
    }

    public class ListLoteriasNotify
    {
        public List<LoteriaNotify> loteria { get; set; }
    }

    public class LoteriaNotify
    {
        public string codigo { get; set; }
    }



    public class RequestSavePayer
    {
        public int identificacion { get; set; }
        public string NombreUser { get; set; }
        public string Correo { get; set; }
        public int transaccion { get; set; }
    }




    public class RequestSendAlert
    {
        public string TransaccionChance { get; set; }
        public int transaccion { get; set; }

        public string codigoApostar { get; set; }

        public string idPagador { get; set; }

        public string cedula { get; set; }
    }


    public class RequestGetRecaudos
    {
        public int codigo { get; set; }

        public double IdTrx { get; set; }
    }

 

    public class RequestConsultValue
    {
        public Iddepartamento idDepartamento { get; set; }
        public Recaudo Recaudo { get; set; }
        public int IdTrx { get; set; }
        public int Servicio { get; set; }
        public Listadocamposm ListadoCamposM { get; set; }
    }

    public class requestIdDepartamento
    {
        public int codigo { get; set; }
    }

    public class RecaudoData
    {
        public int codigo { get; set; }
    }

    public class Listadocamposm
    {
        public List<Camposm> camposM { get; set; }
    }

    public class Camposm
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string valor { get; set; }
    }




    public class RequestGetParameters
    {
        public Iddepartamento idDepartamento { get; set; }
        public Recaudo recaudo { get; set; }
        public int IdTrx { get; set; }
    }

    public class Iddepartamento
    {
        public int codigo { get; set; }
    }

    public class Recaudo
    {
        public int codigo { get; set; }
    }





    public class RequestConsultSubproductBetplay
    {
        public string Transacciondistribuidorid { get; set; }
        public string Token { get; set; }
    }



    public class RequesttokenBetplay
    {
        public string Transacciondistribuidorid { get; set; }
        public string Transaccionclienteid { get; set; }
    }




    public class RequestNotifyRecaudo
    {
        public Iddepartamento IdDepartamento { get; set; }
        public RecaudoNotify Recaudo { get; set; }
        public int IdTrx { get; set; }
        public Servicionotify ServicioNotify { get; set; }
    }

    //Request NotifyPayment

    public class RequestNotifyBetplay
    {
        public string Token { get; set; }
        public int ClienteId { get; set; }
        public ulong transaccionId { get; set; }
        public int valor { get; set; }
        public subproducto subproducto { get; set; }
    }

    public class subproducto
    {
        public int Id { get; set; }
    }

    public class RecaudoNotify
    {
        public int codigo { get; set; }
        public int valor { get; set; }
    }

    public class Servicionotify
    {
        public int id { get; set; }
        public Listadocamponotify listadoCampoNotify { get; set; }
    }

    public class Listadocamponotify
    {
        public List<Camposm> camposM { get; set; }
    }

 

    public class RequestInsertPayerM
    {
        [JsonProperty("Tipo Identificacion")]
        public string TipoIdentificacion { get; set; }
        public string idcontact { get; set; }
        public string namecontact { get; set; }

        [JsonProperty("Segundo Nombre")]
        public string SegundoNombre { get; set; }

        [JsonProperty("Primer Apellido")]
        public string PrimerApellido { get; set; }

        [JsonProperty("Segundo Apellido")]
        public string SegundoApellido { get; set; }
        public string emailcontact { get; set; }
        public string telephonecontact { get; set; }
        public string Sexo { get; set; }

        [JsonProperty("Fecha de Nacimiento")]
        public string FechadeNacimiento { get; set; }

        [JsonProperty("Autoriza Manejo de Datos")]
        public string AutorizaManejodeDatos { get; set; }
    }

  

    public class RequestUpdatePayerM
    {
        public string IdToken { get; set; }
        public string idcontact { get; set; }
        public string emailcontact { get; set; }
        public string telephonecontact { get; set; }
        public string TipoIdentificacion { get; set; }
        public string Sexo { get; set; }
    }


    public class RequestConsultPayerM
    {
        public string Document { get; set; }
    }


    public class RequestGetOTP
    {
        public string FlowID { get; set; }

        public List<DataOTP> dataToReplace { get; set; }
    }

    public class DataOTP
    {
        public string Nombre { get; set; }

        public string Number { get; set; }
    }



    public class RequestVTOP
    {
        public string otp { get; set; }
        public string recipient { get; set; }
    }

  


    //Request Recargas Paquetes



    public class RequestNotifyRecargasCel
    {
        public string Numero { get; set; }
        public string Codigosubproducto { get; set; }
        public double Valor { get; set; }
        public int Transacciondistribuidorid { get; set; }
    }



    public class RequestNotifyPaquetes
    {
        public string Codigosubproducto { get; set; }
        public int Valor { get; set; }
        public string Numero { get; set; }
        public string Id { get; set; }
        public int Transacciondistribuidorid { get; set; }
    }




    public class RequestConsultSubproductosPaquetes
    {
        public int Transacciondistribuidorid { get; set; }
    }


    public class RequestConsultPaquetes
    {
        public int Transacciondistribuidorid { get; set; }
        public string Codigosubproducto { get; set; }
    }


    public class RequestGuardarPaquete
    {
        public string Codigosubproducto { get; set; }
        public int Valor { get; set; }
        public string Numero { get; set; }
        public string Id { get; set; }
        public int Transacciondistribuidorid { get; set; }
    }


    public class RequestConsultCrmRegistro
    {
        public string DocumentType { get; set; }
        public string Document { get; set; }
    }

    public class RequestUpdateCrmRegistro
    {
        public string IDENTIFICACION { get; set; }
        public string TIPO_IDENTIFICACION { get; set; }
        public string NOMBRES { get; set; }
        public string APELLIDOS { get; set; }
        public string CELULAR { get; set; }
        public string EMAIL { get; set; }
    }



    //Astro

    public class RequestConsultarSorteo
    {
        public string Transacciondistribuidorid { get; set; }
    }



    public class RequestRealizarAstro
    {
        public string Transacciondistribuidorid { get; set; }
        public ListadosorteosRequest Listadosorteos { get; set; }
        public Listadodetalles Listadodetalles { get; set; }
        public Sistemaautorizado Sistemaautorizado { get; set; }
    }



    public class Detalle
    {
        public string Numeroapostado { get; set; }
        public double Valorapostado { get; set; }
        public List<SignoRequest> Signo { get; set; }
        public bool Apuestaautomatica { get; set; }
    }

    public class Listadodetalles
    {
        public Detalle Detalle { get; set; }
    }

    public class ListadosorteosRequest
    {
        public SorteoRequest Sorteo { get; set; }
    }


    public class SignoRequest
    {
        public string Codigo { get; set; }
    }

    public class Sistemaautorizado
    {
        public string Codigo { get; set; }
    }

    public class SorteoRequest
    {
        public string Codigoloteria { get; set; }
    }




}
