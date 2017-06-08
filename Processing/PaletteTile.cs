
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using ColorMine.ColorSpaces;
using Color = System.Windows.Media.Color;

namespace Processing
{
    [Serializable]
    public class PaletteTile : ISerializable
    {
        public string Id { get; set; }


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
            SetBytes();
        }

        private void SetBytes()
        {
            var bitmapData = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            var length = bitmapData.Stride*bitmapData.Height;
            Bytes = new byte[length];

            // Copy bitmap to byte[]
            Marshal.Copy(bitmapData.Scan0, Bytes, 0, length);
            Bitmap.UnlockBits(bitmapData);
        }

        public byte[] Bytes { get; set; }

        protected PaletteTile(string id, double widthAndHeight, Rgb rgb)
        {
            Id = id;
            Width = widthAndHeight;
            Height = widthAndHeight;
            Rgb = rgb;
        }

        public PaletteTile(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetString("Id");
            Bitmap = (Bitmap)info.GetValue("Bitmap", typeof(Bitmap));
            BitmapImage = ImageHelper.ToBitmapImage(Bitmap);
            Rgb = (Rgb)info.GetValue("Rgb", typeof(Rgb));
            SetBytes();
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
