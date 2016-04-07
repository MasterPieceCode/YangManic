
using System.Drawing;
using System.Globalization;
using ColorMine.ColorSpaces;
using Color = System.Windows.Media.Color;

namespace Processing
{
    public class PaletteTile
    {
        public int Id { get; set; }
        public Rgb Rgb { get; set; }
        public Color Color { get; set; }
        public Bitmap Bitmap { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        public PaletteTile(TileInfo tileInfo, double widthAndHeight)
            : this(tileInfo.Id, widthAndHeight, tileInfo.Rgb)
        {
            Bitmap = tileInfo.TileBitmap;
        }

        protected PaletteTile(int id, double widthAndHeight, Rgb rgb)
        {
            Id = id;
            Width = widthAndHeight;
            Height = widthAndHeight;
            Rgb = rgb;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

}
