using System.Windows.Media.Imaging;

namespace SuSuerteV2.Utils
{
    public class LoadImages
    {


        public static BitmapImage LoadImageFromFile(Uri path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = path;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.DecodePixelWidth = 900;
            bitmap.EndInit();
            bitmap.Freeze(); //This is the magic line that releases/unlocks the file.
            return bitmap;
        }

    }
}
