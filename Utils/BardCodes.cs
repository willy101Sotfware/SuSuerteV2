using BarcodeLib.Barcode;
using System.Drawing;
using System.IO;

namespace SuSuerteV2.Utils
{
    public class BardCodes
    {
        public static string CodeBar(string Factura)
        {
            byte[] base64SingleBytes;
            string str2 = string.Empty;

            try
            {
                string str = string.Empty;
                int num3 = 0;
                int num4 = 0x3b9ac9ff;

                for (int i = 0; i < 50; i++)
                {
                    Linear barcode = new Linear
                    {
                        Type = BarcodeType.CODABAR,
                        Data = Factura,
                        UOM = UnitOfMeasure.PIXEL,
                        BarWidth = 2,
                        BarHeight = 30,
                        TextFont = new Font("Arial", 8, System.Drawing.FontStyle.Bold)
                    };

                    base64SingleBytes = barcode.drawBarcodeAsBytes();
                    int length = base64SingleBytes.Length;

                    if (length > num3) num3 = length;
                    if (length < num4) num4 = length;

                    if ((num3 != length) && (num4 < num3))
                    {
                        str = "data:image/png;base64," + Convert.ToBase64String(base64SingleBytes);
                        Directory.CreateDirectory(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Barcode\");
                        File.WriteAllBytes(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Barcode\code.png", base64SingleBytes);
                        break;
                    }
                }
                str2 = str;
            }
            catch (Exception ex)
            {
                throw;
            }

            return str2;
        }
    }
}