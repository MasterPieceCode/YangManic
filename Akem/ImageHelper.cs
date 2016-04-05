using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Size = System.Windows.Size;

namespace Akem
{
    public class ImageHelper
    {
        public static DrawingVisual GetFileImage(string filName, double width, double height)
        {
            var bitmapImage = new BitmapImage(new Uri(filName));
            var imageBrush = new ImageBrush(bitmapImage) { Stretch = Stretch.Uniform };

            var visual = new DrawingVisual();
            var dc = visual.RenderOpen();
            dc.DrawRectangle(imageBrush, null, new Rect(new Size(width, height)));
            dc.Close();
            return visual;
        }


        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}
