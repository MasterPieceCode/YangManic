using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processing
{
    public class TileLibrary
    {
        public static Dictionary<int, TileInfo> TileBase { get; private set; }

        static TileLibrary()
        {
            TileBase = new Dictionary<int, TileInfo>();
        }


        public void LoadContainer()
        {
            var directory = new DirectoryInfo("Tiles");

            var avgColorCalculator = new AvgColorCalculator();
            var i = 0;
            foreach (var tileImageFile in directory.GetFiles("*.jpg", SearchOption.TopDirectoryOnly))
            {
                var bitmap = new Bitmap(tileImageFile.FullName);

                /*            // hard code, because tile images has white surrounding area
                            var tileBitmap = new Bitmap(240, 240);
                            var g = Graphics.FromImage(tileBitmap);
                            g.DrawImage(bitmap, 0, 0, new Rectangle(145, 145, 240, 240), GraphicsUnit.Pixel);
                            tileBitmap.Save(string.Format("{0}.jpg", i));
                            i++;
                            //
            */
                var tileInfo = new TileInfo {TileBitmap = bitmap};
                tileInfo.Rgb = avgColorCalculator.Calculate(bitmap);
                tileInfo.Id = i++;
                TileBase.Add(tileInfo.Id, tileInfo);
            }
        }

    }

}
