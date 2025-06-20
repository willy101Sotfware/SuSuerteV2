namespace SuSuerteV2.Domain.ApiService.IntegrationModels
{
    public class ResponseGeneric
    {
        public int codeError { get; set; }
        public string? message { get; set; }
        public object? data { get; set; }
    }

 


    public class ResponseTokenBetplay
    {
        public string Token { get; set; }
    }




    public class ResponseGetProducts
    {
        public Empresafield Empresa { get; set; }
        public bool Estado { get; set; }
        public Listadosubproducto Listadosubproductos { get; set; }
    }
    public class Listadosubproducto
    {
        public List<SubProductoGeneral> Subproducto { get; set; }
    }
    public class Empresafield
    {
        public Ciudadfield ciudadField { get; set; }
        public string direccionField { get; set; }
        public int nitField { get; set; }
        public string nombreField { get; set; }
        public int telefonoField { get; set; }
    }

    public class Ciudadfield
    {
        public int codigoField { get; set; }
        public Departamentofield departamentoField { get; set; }
        public string nombreField { get; set; }
    }

    public class Departamentofield
    {
        public int codigoField { get; set; }
        public string nombreField { get; set; }
    }

    public class SubProductoGeneral
    {
        public int Codigo { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Codigoservicio { get; set; }
        public double Iva { get; set; }
    }






    public class ResponseNotifyBetPlay
    {
        public string CodigoAsesor { get; set; }
        public string CodigoSeguridad { get; set; }
        public Empresa Empresa { get; set; }
        public ErrorNotyfyBetplay error { get; set; }
        public bool Estado { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string TransaccionId { get; set; }
        public string ClienteId { get; set; }
        public double ValorRecaudo { get; set; }
        public string SubproductoId { get; set; }
    }

    public class ErrorNotyfyBetplay
    {

        public string codigo { get; set; }
        public string mensaje { get; set; }
    }

    public class Empresa
    {
        public Ciudad Ciudad { get; set; }
        public string Direccion { get; set; }
        public string Nit { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
    }

    public class Ciudad
    {
        public string Codigo { get; set; }
        public Departamento Departamento { get; set; }
        public string Nombre { get; set; }
    }

    public class Departamento
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }


    public class Errorfield
    {
        public Codigofield codigoField { get; set; }
        public string mensajeField { get; set; }
    }

    public class Codigofield
    {
    }

 
    public class ResponseGetProductsChance
    {
        public Empresa empresaField { get; set; }
        public bool estadoField { get; set; }
        public SubProductoGeneral[] listadosubproductosField { get; set; }
    }




    public class ResponseGetLotteries
    {
        public bool estadoField { get; set; }
        public List<Loteria> listadoloteriasField { get; set; }
    }

    public class Loteria
    {
        public int codigoField { get; set; }
        public int idField { get; set; }
        public string nombreField { get; set; }
        public string nombrecortoField { get; set; }
    }




    public class ResponseTypeChance
    {
        public bool estadoField { get; set; }
        public float ivaField { get; set; }
        public List<Listadotipochancefield> listadotipochanceField { get; set; }
    }

    public class Listadotipochancefield
    {
        public int cifrasField { get; set; }
        public bool combinadoField { get; set; }
        public bool directoField { get; set; }
        public int idField { get; set; }
        public string nombreField { get; set; }
        public bool pataField { get; set; }
        public bool unaField { get; set; }
    }



    public class ResponseAwardsChance
    {
        public bool estado { get; set; }
        public ErrorAwardsChance error { get; set; }
        public EmpresaAwardsChance empresa { get; set; }
        public ChanceAwardsChance chance { get; set; }
    }

    public class ErrorAwardsChance
    {
        public string codigo { get; set; }
        public string mensaje { get; set; }
    }

    public class EmpresaAwardsChance
    {
        public string direccion { get; set; }
        public string nit { get; set; }
        public string nombre { get; set; }
        public string telefono { get; set; }
        public CiudadAwardsChance ciudad { get; set; }
    }

    public class CiudadAwardsChance
    {
        public string codigo { get; set; }
        public string nombre { get; set; }
        public DepartamentoAwardsChance departamento { get; set; }
    }

    public class DepartamentoAwardsChance
    {
        public string codigo { get; set; }
        public string nombre { get; set; }
    }

    public class ChanceAwardsChance
    {
        public bool asumeiva { get; set; }
        public bool esvalorfijo { get; set; }
        public double Valortotalpremio { get; set; }
        public double Valortotalpremiosiniva { get; set; }
    }

    public class ResponseSavePayer
    {
        public bool Estado { get; set; }
        public Tercero Tercero { get; set; }
    }

    public class Tercero
    {
        public int Id { get; set; }
    }



    public class ResponseSendAlert
    {
        public EmpresaAlert Empresa { get; set; }
        public ErrorAlert Error { get; set; }
        public bool Estado { get; set; }
    }

    public class EmpresaAlert
    {
        public CiudadAlert Ciudad { get; set; }
        public string Direccion { get; set; }
        public string Nit { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
    }

    public class CiudadAlert
    {
        public string Codigo { get; set; }
        public DepartamentoAlert Departamento { get; set; }
        public string Nombre { get; set; }
    }

    public class DepartamentoAlert
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }

    public class ErrorAlert
    {
        public int codigo { get; set; }

        public string mensaje { get; set; }
    }


 

    public class ResponseValidateChance
    {
        public EmpresaValidate Empresa { get; set; }
        public bool Estado { get; set; }
        public object Error { get; set; }
        public ChanceValidate Chance { get; set; }
    }

    public class EmpresaValidate
    {
        public CiudadValidate Ciudad { get; set; }
        public string Direccion { get; set; }
        public long Nit { get; set; }
        public string Nombre { get; set; }
        public int Telefono { get; set; }
    }

    public class CiudadValidate
    {
        public int Codigo { get; set; }
        public DepartamentoValidate Departamento { get; set; }
        public string Nombre { get; set; }
    }

    public class DepartamentoValidate
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
    }

    public class ChanceValidate
    {
        public bool Asumeiva { get; set; }
        public bool Esvalorfijo { get; set; }
        public ListadoapuestasValidate Listadoapuestas { get; set; }
    }

    public class ListadoapuestasValidate
    {
        public ApuestaValidate Apuesta { get; set; }
    }

    public class ApuestaValidate
    {
        public string Codigoerror { get; set; }
        public int Codigovalidacion { get; set; }
        public int Id { get; set; }
        public string Mensajevalidacion { get; set; }
    }




    public class ResponseNotifyChance
    {
        public double Codigoasesor { get; set; }
        public string Codigoseguridad { get; set; }
        public EmpresaNotifyR Empresa { get; set; }
        public bool Estado { get; set; }
        public object Error { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public JerarquiaNotifyR Jerarquia { get; set; }
        public double Transaccionid { get; set; }
        public ChanceNotifyR Chance { get; set; }
        public string Consecutivoapuesta { get; set; }
        public string Municipio { get; set; }
        public int Oficina { get; set; }
        public SubproductoNotifyR Subproducto { get; set; }
    }

    public class EmpresaNotifyR
    {
        public CiudadNotifyR Ciudad { get; set; }
        public string Direccion { get; set; }
        public long Nit { get; set; }
        public string Nombre { get; set; }
        public int Telefono { get; set; }
    }

    public class CiudadNotifyR
    {
        public int Codigo { get; set; }
        public DepartamentoNotifyR Departamento { get; set; }
        public string Nombre { get; set; }
    }

    public class DepartamentoNotifyR
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
    }

    public class JerarquiaNotifyR
    {
        public int Celulaid { get; set; }
        public int Ciudadid { get; set; }
        public int Departamentooid { get; set; }
        public int Empresaid { get; set; }
        public int Equipoid { get; set; }
        public int Oficinaid { get; set; }
        public int Paisid { get; set; }
        public int Puntoventaid { get; set; }
        public int Subzonaid { get; set; }
        public int Usuarioid { get; set; }
        public int Zonaid { get; set; }
    }

    public class ChanceNotifyR
    {
        public bool Asumeiva { get; set; }
        public double Encimavalorcom { get; set; }
        public double Encimavalordir { get; set; }
        public double Encimavalordir3 { get; set; }
        public double Encimavalordir4 { get; set; }
        public double Encimavalorpat { get; set; }
        public double Encimavaloruna { get; set; }
        public bool Esvalorfijo { get; set; }
        public string Fechasorteo { get; set; }
        public double Iva { get; set; }
        public ListadoapuestasNotifyR Listadoapuestas { get; set; }
        public string Serie1 { get; set; }
        public int Serie2 { get; set; }
        public double Totalapostado { get; set; }
        public double Totalpagado { get; set; }
        public double Totaliva { get; set; }
    }

    public class ListadoapuestasNotifyR
    {
        public List<ApuestaNotifyR> Apuesta { get; set; }
    }

    public class ApuestaNotifyR
    {
        public ListadoloteriasNotifyR Listadoloterias { get; set; }
        public double Numeroapostado { get; set; }
        public double Valorapostadocombinado { get; set; }
        public double Valorapostadodirecto { get; set; }
        public double Valorapostadopata { get; set; }
        public double Valorapostadototal { get; set; }
        public double Valorapostadouna { get; set; }
        public double Valoriva { get; set; }
        public double Valortotal { get; set; }
    }

    public class ListadoloteriasNotifyR
    {
        public List<LoteriaNotifyR> Loteria { get; set; }
    }

    public class LoteriaNotifyR
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public string Nombrecorto { get; set; }
    }

    public class SubproductoNotifyR
    {
        public string Nombre { get; set; }
        public ProductoNotifyR Producto { get; set; }
    }

    public class ProductoNotifyR
    {
        public string Nombre { get; set; }
    }



    /// <summary>
    /// ResponseConsultSubproductosPaquetes
    /// </summary>
    public class Listadosubproductospaquetes
    {
        public List<Paquete> Paquete { get; set; }
    }

    public class Paquete
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }

    public class ResponseConsultSubproductosPaquetes
    {
        public bool Estado { get; set; }
        public Listadosubproductospaquetes Listadosubproductospaquetes { get; set; }
    }


    /// <summary>
    ///  ResponseConsultPaquetes
    /// </summary>

    public class Listadopaquetes
    {
        public List<Paquetes> Paquetes { get; set; }
    }

    public class Paquetes
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Nombrecorto { get; set; }
        public int Valor { get; set; }
    }

    public class ResponseConsultPaquetes
    {
        public bool Estado { get; set; }
        public Listadopaquetes Listadopaquetes { get; set; }
    }


    public class ResponseGuardarPaquetes
    {
        public bool Estado { get; set; }
        public string Transaccionid { get; set; }
    }




    public class ResponseParametrosRecarga
    {
        public bool Estado { get; set; }
        public Empresa Empresa { get; set; }
        public double Topemaximo { get; set; }
        public double Topeminimo { get; set; }
    }


    public class ResponseConsultarCRMRegistro
    {
        public string CRM_ID { get; set; }
        public string IDENTIFICACION { get; set; }
        public string NOMBRES { get; set; }
        public string APELLIDOS { get; set; }
        public string FECHA_NACIMIENTO { get; set; }
        public string CELULAR { get; set; }
        public string EMAIL { get; set; }
        public string GENERO { get; set; }
        public string TIPO_IDENTIFICACION { get; set; }
        public string FECHA_CREACION { get; set; }
        public string FECHA_ACTUALIZACION { get; set; }
        public bool AUTORIZA_MANEJO_DATOS { get; set; }
        public bool ACTIVO { get; set; }
    }



    public class ResponseNotificarPaquetes
    {
        public bool Estado { get; set; }
        public string Transaccionid { get; set; }
    }




    public class Jerarquia
    {
        public string Empresaid { get; set; }
        public string Paisid { get; set; }
        public string Departamentooid { get; set; }
        public string Zonaid { get; set; }
        public string Subzonaid { get; set; }
        public string Ciudadid { get; set; }
        public string Oficinaid { get; set; }
        public string Puntoventaid { get; set; }
        public string Equipoid { get; set; }
        public string Celulaid { get; set; }
        public string Usuarioid { get; set; }
    }

    public class Lineaspierecarga
    {
        public List<string> Linea { get; set; }
    }

    public class ResponseNotificacionRecargaCel
    {
        public bool Estado { get; set; }
        public Empresa Empresa { get; set; }
        public string Transaccionid { get; set; }
        public string Numeroautorizacion { get; set; }
        public Jerarquia Jerarquia { get; set; }
        public Lineaspierecarga Lineaspierecarga { get; set; }
    }



    public class Listadosubproductos
    {
        public List<Subproducto> Subproducto { get; set; }
    }

    public class ResponseConsultarSubproductRecargas
    {
        public bool Estado { get; set; }
        public Empresa Empresa { get; set; }
        public Listadosubproductos Listadosubproductos { get; set; }
        public object Error { get; set; }
    }

    public class Subproducto
    {
        public string Codigo { get; set; }
        public string Id { get; set; }
        public string Nombre { get; set; }
    }




    public class Listadosorteos
    {
        public List<Sorteo> Sorteo { get; set; }
    }

    public class ResponseConsultarAstro
    {
        public bool Estado { get; set; }
        public Empresa Empresa { get; set; }
        public Listadosorteos Listadosorteos { get; set; }
        public string Subproducto { get; set; }
    }

    public class Sorteo
    {
        public string Codigocortoloteria { get; set; }
        public string Codigoloteria { get; set; }
        public string Descripcion { get; set; }
        public string Horacierre { get; set; }
        public string Horasorteo { get; set; }
    }




    public class Listadosignos
    {
        public List<Signo> Signo { get; set; }
    }

    public class ResponseConsultarSignos
    {
        public bool Estado { get; set; }
        public Empresa Empresa { get; set; }
        public Listadosignos Listadosignos { get; set; }
    }

    public class Signo
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }




    public class CiudadVentaAstro
    {
        public string Codigo { get; set; }
        public DepartamentoVentaAstro Departamento { get; set; }
        public string Nombre { get; set; }
    }

    public class DepartamentoVentaAstro
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }

    public class DetalleVe4ntaAstro
    {
        public bool Apuestaautomatica { get; set; }
        public string Numeroapostado { get; set; }
        public double Valorapostado { get; set; }
        public double Valoriva { get; set; }
        public double Valortotal { get; set; }
    }

    public class EmpresaVentaAstro
    {
        public CiudadVentaAstro Ciudad { get; set; }
        public string Direccion { get; set; }
        public string Nit { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
    }

    public class Listadoautorizacion
    {
        public string Autorizacion { get; set; }
    }

    public class ListadodetallesVentaAstro
    {
        public DetalleVe4ntaAstro Detalle { get; set; }
    }

    public class ListadoencabezadosVentaAstro
    {
        public List<string> Encabezado { get; set; }
    }

    public class ListadopiesVentaAstro
    {
        public List<string> Pie { get; set; }
    }

    public class ListadoplanpremiosVentaAstro
    {
        public List<string> Linea { get; set; }
    }

    public class ListadosorteosAstroVenta
    {
        public SorteoAstroVenta Sorteo { get; set; }
    }

    public class ResponseVentaAstro
    {
        public string Codigoseguridad { get; set; }
        public bool Estado { get; set; }
        public EmpresaVentaAstro Empresa { get; set; }
        public string Transaccionid { get; set; }
        public string Codigoasesor { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Equipo { get; set; }
        public Listadoautorizacion Listadoautorizacion { get; set; }
        public ListadodetallesVentaAstro Listadodetalles { get; set; }
 
        public ListadosorteosAstroVenta Listadosorteos { get; set; }
        public object Mensaje { get; set; }
        public string Municipio { get; set; }
        public string Oficina { get; set; }
        public string Puntoventa { get; set; }
        public string Serie1 { get; set; }
        public string Serie2 { get; set; }
        public double Totalapostado { get; set; }
        public double Totaliva { get; set; }
        public double Totalpagado { get; set; }
    }

    public class SorteoAstroVenta
    {
        public string Codigoloteria { get; set; }
    }






    public class ResponseGetRecaudo
    {
        public Empresafield Empresa { get; set; }
        public object Error { get; set; }
        public bool Estado { get; set; }
        public RecaudoResponse Listadorecaudos { get; set; }
    }
    public class RecaudoResponse
    {
        public List<RecaudoResponseObj> Recaudo { get; set; }
    }

    public class RecaudoResponseObj
    {
        public int Codigo { get; set; }
        public int Codigosubproducto { get; set; }
        public float Iva { get; set; }
        public string Nombre { get; set; }
    }




    public class ResponseGetParameters
    {
        public bool estadoField { get; set; }

        public Errorfield errorField { get; set; }


        public Recaudofield recaudoField { get; set; }
    }

    public class Recaudofield
    {
        public int codigoField { get; set; }
        public Listadoserviciosfield listadoserviciosField { get; set; }
    }

    public class Listadoserviciosfield
    {
        public Serviciofield servicioField { get; set; }
    }

    public class Serviciofield
    {
        public bool activoField { get; set; }
        public int diasdespuesvencimientoField { get; set; }
        public string horamodificacionrecaudoField { get; set; }
        public int idField { get; set; }
        public List<Listadocamposfield> listadocamposField { get; set; }
        public bool modificafecharecaudoField { get; set; }
        public bool validafechavencimientoField { get; set; }
        public bool validasaldoField { get; set; }
    }

    public class Listadocamposfield
    {
        public string nombreField { get; set; }
        public bool editableField { get; set; }
        public string formatosalidaField { get; set; }
        public int idField { get; set; }
        public string jsonvalidacionesField { get; set; }
        public string etiquetaField { get; set; }
        public string tipoCampoField { get; set; }
        public string tipodatoField { get; set; }
        public bool visibleField { get; set; }
    }







    public class ResponseConsultValue
    {
        public bool estadoField { get; set; }

        public Errorfield errorField { get; set; }


        public RecaudofieldData recaudoField { get; set; }
    }

    public class RecaudofieldData
    {
        public int codigoField { get; set; }
        public ListadoserviciosfieldData listadoserviciosField { get; set; }
        public string nombreField { get; set; }
        public Listadoregistrosfield listadoregistrosField { get; set; }
    }

    public class ListadoserviciosfieldData
    {
        public ServiciofieldData servicioField { get; set; }
    }

    public class ServiciofieldData
    {
        public bool activoField { get; set; }
        public int diasdespuesvencimientoField { get; set; }
        public Horamodificacionrecaudofield horamodificacionrecaudoField { get; set; }
        public int idField { get; set; }
        public List<ListadocamposfieldData> listadocamposField { get; set; }
        public bool modificafecharecaudoField { get; set; }
        public bool validafechavencimientoField { get; set; }
        public bool validasaldoField { get; set; }
    }

    public class Horamodificacionrecaudofield
    {
    }

    public class ListadocamposfieldData
    {
        public string nombreField { get; set; }
        public bool editableField { get; set; }
        public string formatosalidaField { get; set; }
        public int idField { get; set; }
        public string jsonvalidacionesField { get; set; }
        public string etiquetaField { get; set; }
        public string tipodatoField { get; set; }
        public bool visibleField { get; set; }
    }

    public class Listadoregistrosfield
    {
        public List<Registrofield> registroField { get; set; }
    }

    public class Registrofield
    {
        public string etiquetaField { get; set; }
        public string valorField { get; set; }
    }






    public class ResponseNotifyPayment
    {
        public string Codigoseguridad { get; set; }
        public EmpresaNotifyP Empresa { get; set; }
        public bool Estado { get; set; }
        public ErrorAlert Error { get; set; }
        public JerarquiaNotifyP Jerarquia { get; set; }
        public Listadomensajespublicitarios Listadomensajespublicitarios { get; set; }
        public object Periodopago { get; set; }
        public string Transaccionid { get; set; }
    }

    public class EmpresaNotifyP
    {
        public CiudadNotifyP Ciudad { get; set; }
        public string Direccion { get; set; }
        public long Nit { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
    }

    public class CiudadNotifyP
    {
        public string Codigo { get; set; }
        public DepartamentoNotifyP Departamento { get; set; }
        public string Nombre { get; set; }
    }

    public class DepartamentoNotifyP
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
    }

    public class JerarquiaNotifyP
    {
        public int Celulaid { get; set; }
        public int Ciudadid { get; set; }
        public int Departamentooid { get; set; }
        public int Empresaid { get; set; }
        public int Equipoid { get; set; }
        public int Oficinaid { get; set; }
        public int Paisid { get; set; }
        public int Puntoventaid { get; set; }
        public int Subzonaid { get; set; }
        public int Usuarioid { get; set; }
        public int Zonaid { get; set; }
    }

    public class Listadomensajespublicitarios
    {
    }

    public class Periodopago
    {

    }



    public class ResponseConsultPayer
    {
        public string code { get; set; }
        public string error { get; set; }
        public string msg { get; set; }
        public List<DatumConsult> data { get; set; }
    }

    public class DatumConsult
    {
        public string TipoIdentificacion { get; set; }
        public string idcontact { get; set; }
        public string namecontact { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string emailcontact { get; set; }
        public Telephonecontact telephonecontact { get; set; }
        public string Sexo { get; set; }
        public string FechadeNacimiento { get; set; }
        public string AutorizaManejodeDatos { get; set; }
        public string wolkvox_usuario_creacion { get; set; }
        public string wolkvox_fecha_creacion { get; set; }
        public string wolkvox_origen { get; set; }
        public string wolkvox_ip_address { get; set; }
        public Wolkvox_Owner wolkvox_owner { get; set; }
        public string Canaldeactualizacion { get; set; }
        public string CelularValidado { get; set; }
        public string Correovalidado { get; set; }
        public string FechaAutorizaManejoDatos { get; set; }
        public string FechaCreación { get; set; }
        public string HoraAutorizaManejoDatos { get; set; }
        public string IdUsuarioCreación { get; set; }
        public string NombreUsuarioAutorizaManejoDatos { get; set; }
        public string Nombresyapellidosvalidado { get; set; }
        public string Sexovalidado { get; set; }
        public string UsuarioAutorizaManejoDatos { get; set; }
        public string ValFechadeNacimiento { get; set; }
        public string wolkvox_fecha_modificacion { get; set; }
        public string wolkvox_usuario_modificacion { get; set; }
        public string wolkvox_id { get; set; }
    }

    public class Telephonecontact
    {
        public string country { get; set; }
        public string code { get; set; }
        public object value { get; set; }
    }

    public class Wolkvox_Owner
    {
        public string user { get; set; }
        public string nombre { get; set; }
        public string id { get; set; }
        public string email { get; set; }
    }


    public class ResponseInsertPayer
    {
        public int code { get; set; }
        public string error { get; set; }
        public string msg { get; set; }
        public DataInsert data { get; set; }
    }

    public class DataInsert
    {
        public string wolkvox_id { get; set; }
    }



    public class ErrorCrm
    {
        public int code { get; set; }
        public string message { get; set; }
    }

    public class ResponseErrorCrearRegistroCrm
    {
        public ErrorCrm error { get; set; }
    }

    public class Root
    {
        public string CRM_ID { get; set; }
        public string IDENTIFICACION { get; set; }
        public string NOMBRES { get; set; }
        public string APELLIDOS { get; set; }
        public DateTime FECHA_NACIMIENTO { get; set; }
        public string CELULAR { get; set; }
        public string EMAIL { get; set; }
        public string GENERO { get; set; }
        public string TIPO_IDENTIFICACION { get; set; }
        public DateTime FECHA_CREACION { get; set; }
        public DateTime FECHA_ACTUALIZACION { get; set; }
        public bool AUTORIZA_MANEJO_DATOS { get; set; }
        public bool ACTIVO { get; set; }
    }



    public class ResponseUpdatePayer
    {
        public int code { get; set; }
        public string error { get; set; }
        public string msg { get; set; }
        public DataU data { get; set; }
    }

    public class DataU
    {
        public string wolkvox_id { get; set; }
    }




    public class ResponseGetOTP
    {
        public string status { get; set; }
        public string pTracking { get; set; }
    }

 

    public class ResponseVOTP
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public string description { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public bool validOtp { get; set; }
    }


}

