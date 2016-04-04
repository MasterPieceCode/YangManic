using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
    }
}
