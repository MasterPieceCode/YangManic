using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Processing
{
    public class TileContainer
    {
        public static List<PaletteTile> PaletteTiles { get; private set; }

        static TileContainer()
        {
            PaletteTiles = new List<PaletteTile>();
        }

        public TileContainer(IEnumerable<PaletteTile> paletteTiles)
        {
            PaletteTiles.AddRange(paletteTiles);
        }

        public void AddTile(PaletteTile tile)
        {
            PaletteTiles.Add(tile);
        }
    }
}