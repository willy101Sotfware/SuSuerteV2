using BarcodeLib.Barcode;
using System.IO;

namespace SuSuerteV2.Utils
{
    public class BardCodes
    {
        public static string CodeBar(string Factura)
        {
            byte[] base64SingleBytes;
            string str2;
            try
            {
                string str = "";

                int num3 = 0;
                int num4 = 0x3b9ac9ff;
                for (int i = 0; i < 50; i++)
                {
                    Linear barcode = new()
                    {
                        Type = BarcodeType.CODABAR,
                        Data = string.Concat(Factura),
                        // barcode.Data = string.Concat("(415)" + Code + "(8020)" + string.Format(Factura).PadLeft(10, '0') + "(3900)" + string.Format("{0:0}", ValorFactura).PadLeft(10, '0') + "(96)" + fecha);
                        UOM = UnitOfMeasure.PIXEL,
                        BarWidth = 2,
                        BarHeight = 30,
                        TextFont = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold)
                    };
                    base64SingleBytes = barcode.drawBarcodeAsBytes();
                    int length = base64SingleBytes.GetLength(0);
                    if (length > num3) { num3 = length; }
                    if (length < num4) { num4 = length; }
                    if ((num3 != length) && (num4 < num3))
                    {
                        byte[] inArray = base64SingleBytes;
                        str = string.Format("data:image/png;base64," + Convert.ToBase64String(inArray), new object[0]);

                        File.WriteAllBytes(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Barcode\code.png", inArray);

                        break;
                    }
                }
                str2 = str;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return str2;
        }
    }
}
