using System.Collections.Generic;
using System.Linq;
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;

namespace Processing
{
    public class ColorFilter
    {
        private readonly IColorSpaceComparison _comparer;
        private readonly List<PaletteTile> _tiles;

        public ColorFilter(IEnumerable<PaletteTile> paletteTiles)
        {
            _comparer = new Cie1976Comparison();
            _tiles = paletteTiles.ToList();
        }

        public PaletteTile FindBestTile(Rgb rgb)
        {
            var minimumDeltaE = double.MaxValue;
            PaletteTile result = null;
            foreach (var tileInfo in _tiles)
            {
                var deltaE = GetDifferenceUsingCie1976Comparison(tileInfo.Rgb, rgb);
                if (deltaE < minimumDeltaE)
                {
                    minimumDeltaE = deltaE;
                    result = tileInfo;
                }
            }

            return result;
        }

        public double GetDifferenceUsingCie1976Comparison(Rgb color1, Rgb color2)
        {
            var colorRgb1 = new Rgb { R = color1.R, G = color1.G, B = color1.B };
            var colorRgb2 = new Rgb { R = color2.R, G = color2.G, B = color2.B };

            return _comparer.Compare(colorRgb1, colorRgb2);
        }
    }
}