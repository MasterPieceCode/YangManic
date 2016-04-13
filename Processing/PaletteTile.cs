
using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using ColorMine.ColorSpaces;
using Color = System.Windows.Media.Color;

namespace Processing
{
    [Serializable]
    public class PaletteTile : ISerializable
    {
        public int Id { get; set; }


        public Rgb Rgb { get; set; }
        public Color Color { get; set; }
        public Bitmap Bitmap { get; set; }
        public BitmapImage BitmapImage { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        public PaletteTile(TileInfo tileInfo, double widthAndHeight)
            : this(tileInfo.Id, widthAndHeight, tileInfo.Rgb)
        {
            Bitmap = tileInfo.TileBitmap;
            BitmapImage = ImageHelper.ToBitmapImage(Bitmap);
        }

        protected PaletteTile(int id, double widthAndHeight, Rgb rgb)
        {
            Id = id;
            Width = widthAndHeight;
            Height = widthAndHeight;
            Rgb = rgb;
        }

        public PaletteTile(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt32("Id");
            Bitmap = (Bitmap)info.GetValue("Bitmap", typeof(Bitmap));
            BitmapImage = ImageHelper.ToBitmapImage(Bitmap);
            Rgb = (Rgb)info.GetValue("Rgb", typeof(Rgb));
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Bitmap", Bitmap);
            info.AddValue("Rgb", Rgb);
        }
    }
}
