

using SuSuerteV2.Domain;
using SuSuerteV2.Domain.Enumerables;
using SuSuerteV2.Domain.Peripherals;
using SuSuerteV2.Domain.UIServices;

namespace SuSuerteV2.Utils
{

    public class PrintBaucher
    {
       
        private static PrintService _printService;
        public static PrintService PrintService
        {
            get { return _printService; }
        }

        public static void PrintVoucherBetPlay(Transaction Transaction)
        {
            try
            {
                if (Transaction != null)
                {
                    PrintService.StartBuilder();

                    // Imprimir encabezado
                    PrintService.PrintInCenter("BetPlay-Apuestas Deportivas");
                    PrintService.PrintInCenter("Autoriza COLJUEGOS Contrato C1444");
                    PrintService.PrintInCenter("AV CLL 26 #69091 OF.802A PBX(1)5190395");

                    // Imprimir fecha
                    PrintService.PrintInCenter($"Fecha: {DateTime.Now.Date:yyyy-MM-dd HH:mm:ss}");

                    // Imprimir título de datos de recarga
                    PrintService.PrintInCenter("DATOS RECARGA - BETPLAY");

                    // Número de Cédula
                    PrintService.PrintInLeft("Número de Cedula");
                    PrintService.PrintInRight(Transaction.Documento ?? string.Empty);

                    // Valor a Pagar
                    PrintService.PrintInLeft("Valor recarga");
                    PrintService.PrintInRight($"${Transaction.Total}");

                    // Mensaje final
                    PrintService.PrintInCenter("REALIZA TU APUESTA EN: BETPLAY.COM.CO");

                    // Información adicional
                    PrintService.PrintInCenter($"OF: 144388");
                    PrintService.PrintInCenter($"AS: {AppConfig.Get("Terminal")}");
                    PrintService.PrintInCenter($"EQ: 000000052441");

                    PrintService.ExecuteBuilder();
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
            }
        }


        public static void PrintVoucherSuperChance(Transaction Transaction)
        {
            try
            {
                PrintService.StartBuilder();

                // Imprimir encabezado
                PrintService.PrintInCenter("SUPERCHANCE");
                PrintService.PrintInCenter("EL CHANCE MANUAL ES ILEGAL");

                // Imprimir loterías y números
                PrintService.PrintInCenter("Loterías"); // Asegúrate de definir Loterias
                PrintService.PrintInCenter("NÚMERO. DERE. CUNA. COMB. CIF.");
                PrintService.PrintInCenter("Numero1"); // Asegúrate de definir Numero1
                PrintService.PrintInCenter("Numero2"); // Asegúrate de definir Numero2
                PrintService.PrintInCenter("Numero3"); // Asegúrate de definir Numero3
                PrintService.PrintInCenter("Numero4"); // Asegúrate de definir Numero4
                PrintService.PrintInCenter("Numero5"); // Asegúrate de definir Numero5
                PrintService.PrintInCenter("Numero6"); // Asegúrate de definir Numero6
                PrintService.PrintInCenter("Numero7"); // Asegúrate de definir Numero7

                // Imprimir información adicional
                PrintService.PrintInCenter($"COL - {AppConfig.Get("Terminal")} CLI - {Transaction.Documento}");
                PrintService.PrintInCenter($"FV: {DateTime.Now:dd/MM/yyyy HH:mm:ss} FS: {Convert.ToDateTime(Transaction.Fecha):dd/MM/yyyy}");
                PrintService.PrintInCenter($"Vlr: {string.Format("{0:C2}", $"${Transaction.Valor}")} IVA: {string.Format("{0:C2}", $"${Transaction.Iva}")} TOTAL: {string.Format("{0:C2}", $"${Transaction.Total}")}");

                PrintService.PrintInCenter("OF:");
                PrintService.PrintInCenter("SV: " + AppConfig.Get("Sitio"));
                PrintService.PrintInCenter("E:");
                PrintService.PrintInCenter("MEDELLIN");

                // Imprimir imagen de código de barras
                // Nota: Asegúrate de que la ruta de la imagen sea accesible y válida
                PrintService.PrintBitmap($@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Barcode\code.png");

                PrintService.PrintInCenter("Contrato Vigente No. 32 de 2021");

                PrintService.ExecuteBuilder();
            }
            catch (Exception ex)
            {
                // Manejo de errores
            }
        }

        public static void PrintVoucherRecargas(Transaction Transaction)
        {
          
            try
            {
                if (Transaction != null)
                {
                    PrintService.StartBuilder();

                    // Imprimir fecha y hora
                    PrintService.PrintInLeft("Fecha: " + DateTime.Now.Date.ToString("yyyy-MM-dd"));
                    PrintService.PrintInRight("Hora: " + DateTime.Now.ToString("HH:mm:ss"));

                    // Imprimir código de sitio
                    PrintService.PrintInLeft("Cód.Sitio: " + AppConfig.Get("CodSitio"));

                    // Determinar si es Paquetes Celular o Recargas Celular
                    string info = Transaction.TypeTramite == ETypeTramites.PaquetesCel ? "Paquetes Celular" : "Recargas Celular";
                    string TypePack = "";
                    PrintService.PrintInLeft(info);

                    // Imprimir línea divisoria
                    PrintService.PrintInCenter("------------------------------------------------------------");

                    // Número de Celular
                    PrintService.PrintInLeft("Número de Celular: " + Transaction.NumOperator.ToString());

                    // Descripción
                    PrintService.PrintInLeft("Descripción: " + TypePack);

                    // Valor recarga
                    PrintService.PrintInLeft("Valor recarga: " + string.Format("{0:C2}", "$" + Transaction.Total.ToString()));

                    // Estado de la transacción
                    PrintService.PrintInLeft("Estado de transacción: " + Transaction.StatePay.ToString());

                    // Valor ingresado
                    PrintService.PrintInLeft("Valor ingresado: " + string.Format("{0:C2}", "$" + Transaction.TotalIngresado.ToString()));

                    // Total Devuelto
                    PrintService.PrintInLeft("Total Devuelto: " + string.Format("{0:C2}", "$" + Transaction.TotalDevuelta.ToString()));

                    // Imprimir línea divisoria
                    PrintService.PrintInCenter("------------------------------------------------------------");

                    // Mensaje final
                    PrintService.PrintInCenter("Toda Transacción está sujeta");
                    PrintService.PrintInCenter("a verificación y aprobación");

                    PrintService.ExecuteBuilder();
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
            }
        }

    }
}
