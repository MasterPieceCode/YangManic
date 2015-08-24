using System.Drawing;
using ColorMine.ColorSpaces;

namespace Processing
{
    public class AvgColorCalculator
    {
        public Rgb Calculate(Bitmap bitmap)
        {
            double rSum = 0;
            double gSum = 0;
            double bSum = 0;

            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    var rgb = new Rgb{ R = pixel.R, G = pixel.G, B = pixel.B };
                    //  var lab= rgb.To<Lab>();

                    rSum += rgb.R;
                    gSum += rgb.G;
                    bSum += rgb.B;
                }
            }

            var pixelCount = bitmap.Width * bitmap.Height;

            return new Rgb(rSum / pixelCount, gSum / pixelCount, bSum / pixelCount);
        }
    }
}