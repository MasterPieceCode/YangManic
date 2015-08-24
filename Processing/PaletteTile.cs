using System.Windows.Media;
using ColorMine.ColorSpaces;

namespace Processing
{
    public class PaletteTile
    {
        public int Id { get; set; }
        public Rgb Rgb { get; set; }
        public Color Color { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        public PaletteTile(int id, Color color, double widthAndHeight)
        {
            Id = id;
            Color = color;
            Rgb = new Rgb(Color.R, Color.G, Color.B);
            Width = widthAndHeight;
            Height = widthAndHeight;
        }
    }
}
