using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;
using Color = System.Drawing.Color;

namespace Processing
{
    public class TileInfo
    {
        public Bitmap TileBitmap { get; set; }
        public Rgb Rgb { get; set; }
        public int Id { get; set; }
    }


    public class ImageToMozaicConverter
    {
        private readonly Bitmap _image;
        private readonly int _tileSize;
        private readonly ObservableCollection<PaletteTile> _paletteTiles;
        private readonly int _desirableWidth;
        private readonly int _desirableHeight;
        private readonly AvgColorCalculator _avgColorCalculator;
        private readonly ColorFilter _colorFilter;
        private readonly int _gridHorSteps;
        private readonly int _gridVerSteps;
        private readonly int _gridCellWidth;
        private readonly int _gridCellHeight;
        private readonly Size _gridCellSize;



        public ImageToMozaicConverter(string imageFile, int tileSize, ObservableCollection<PaletteTile> paletteTiles, int desirableWidth, int desirableHeight)
        {
            _image = new Bitmap(imageFile);
            _tileSize = tileSize;
            _paletteTiles = paletteTiles;

            _desirableWidth = desirableWidth;

            _desirableHeight = desirableHeight;

            _gridHorSteps = _desirableWidth / tileSize;
            _gridVerSteps = _desirableHeight / tileSize;

            float scale = Math.Min((float)_gridHorSteps / _image.Width, (float)_gridVerSteps / _image.Height);

         

            var scaleWidth = (_image.Width *  scale);
            var scaleHeight =(_image.Height * scale);

            var bmp = new Bitmap((int)scaleWidth, (int)scaleHeight);
            var graph = Graphics.FromImage(bmp);

            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.HighQuality;

            graph.DrawImage(_image, 0, 0, scaleWidth, scaleHeight);

            bmp.Save("res1.jpg");

            _gridHorSteps = (int)scaleWidth;
            _gridVerSteps = (int)scaleHeight;

            _image = bmp;

            /*_desirableHeight = desirableHeight ;*/
            _avgColorCalculator = new AvgColorCalculator();
            _colorFilter = new ColorFilter(paletteTiles);

            _gridCellWidth = _image.Width / _gridHorSteps;
            _gridCellHeight = _image.Height / _gridVerSteps;
            _gridCellSize = new Size(_gridCellWidth, _gridCellHeight);
        }


        public ObservableCollection<ObservableCollection<PaletteTile>> Convert()
        {
            var result = new ObservableCollection<ObservableCollection<PaletteTile>>();
            //var convertedBitmap = new Bitmap(_gridCellWidth * _gridHorSteps, _gridCellHeight * _gridVerSteps);
            //var convertedImGraphics = Graphics.FromImage(convertedBitmap);
            //            convertedImGraphics.CompositingQuality = CompositingQuality.HighQuality;
            //convertedImGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //TileContainer.PaletteTiles.Clear();

            var newImage = new Bitmap(_gridHorSteps, _gridVerSteps);

            int c = 0;
            for (var y = 0; y < _gridVerSteps; y++)
            {
                //  var verticalTiles = new ObservableCollection<TileInfo>();
                // result.Add(verticalTiles);
                for (var x = 0; x < _gridHorSteps; x++)
                {
                    var bmp = new Bitmap(_gridCellWidth, _gridCellHeight);

                    var imageTileGraphics = Graphics.FromImage(bmp);
                    /*       imageTileGraphics.CompositingQuality = CompositingQuality.HighQuality;
                           imageTileGraphics.CompositingMode = CompositingMode.SourceCopy;
                           imageTileGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;*/
                    var point = new Point(_gridCellWidth * x, _gridCellHeight * y);


                    //imageTileGraphics.DrawImage(_image, 0, 0, new Rectangle(point, _gridCellSize), GraphicsUnit.Pixel);

                    //imageTileGraphics.DrawImage(_image, 0, 0, new Rectangle(point, new Size(15, 15)), GraphicsUnit.Pixel);

                    for (int i = 0; i < _gridCellHeight; i++)
                    {
                        for (int j = 0; j < _gridCellWidth; j++)
                        {
                            bmp.SetPixel(j, i, _image.GetPixel(_gridCellWidth * x + j, _gridCellHeight * y + i));
                        }
                    }

                    var avgColor = _avgColorCalculator.Calculate(bmp);
                    newImage.SetPixel(x, y, Color.FromArgb((int)avgColor.R, (int)avgColor.G, (int)avgColor.B));
                    /*
                                        var Rgb = new Lab {L = Rgb.R, A = Rgb.G, B = Rgb.B}.ToRgb();

                                        var findBestMatchTitle = new TileInfo { Id = c++, Rgb = new Rgb(Rgb.R, Rgb.G, Rgb.B), TileBitmap = CreateBitmapWithColor(Rgb, _gridCellSize)};
                                        TileContainer.PaletteTiles.Add(findBestMatchTitle.Id, findBestMatchTitle);
                    */

                    //var findBestMatchTitle = _colorFilter.FindTitle(Rgb);
                    // verticalTiles.Add(findBestMatchTitle);
                    // bmp.Save("Im\\"+ c++ + ".jpg");
                    //      convertedImGraphics.DrawImage(findBestMatchTitle.TileBitmap, new Rectangle(point, _gridCellSize), new Rectangle(0, 0, 240, 240), GraphicsUnit.Pixel);

                    imageTileGraphics.Dispose();
                }
            }

            for (var y = 0; y < newImage.Height; y++)
            {
                var verticalTiles = new ObservableCollection<PaletteTile>();
                result.Add(verticalTiles);
                for (var x = 0; x < newImage.Width; x++)
                {
                    var pixel = newImage.GetPixel(x, y);

                    var Rgb = new Rgb { R = pixel.R, G = pixel.G, B = pixel.B };

                    var findBestMatchTitle = _colorFilter.FindBestTile(new Rgb(pixel.R, pixel.G, pixel.B));

                    var newRgb = findBestMatchTitle.Color;
                    var paletteColor = new Rgb { R = newRgb.R, G = newRgb.G, B = newRgb.B };
                    SetErrorForPixel(Rgb, paletteColor, y, x, newImage.Width, newImage.Height, newImage);

                    verticalTiles.Add(findBestMatchTitle);

                    /*var newRgbAAZ = newColor.ToRgb();*/
                    /* convertedImGraphics.FillRectangle(
                        new SolidBrush(Color.FromARgb((int) newRgb.R, (int) newRgb.G, (int) newRgb.B)), x, y, 1, 1);*/

                }
            }

            //convertedBitmap.Save("result1.jpg");

            //convertedImGraphics.Dispose();

            return result;
        }

        private void SetErrorForPixel(Rgb originalPixelColor, Rgb transformedPixelColor, int rowInd, int colInd, int width, int height, Bitmap bitmap)
        {
            Color offsetPixel;

            int offsetX;
            int offsetY;

            var redError = (int)(originalPixelColor.R - transformedPixelColor.R);
            var blueError = (int)(originalPixelColor.G - transformedPixelColor.G);
            var greenError = (int)(originalPixelColor.B - transformedPixelColor.B);

            int R = 0;
            int G = 0;
            int B = 0;

            if (colInd + 1 < width)
            {
                // right
                offsetX = colInd + 1;
                offsetY = rowInd;
                offsetPixel = bitmap.GetPixel(offsetX, offsetY);
                R = GetTruncatedDiff(offsetPixel.R + ((redError * 7) >> 4));
                G = GetTruncatedDiff(offsetPixel.G + ((greenError * 7) >> 4));
                B = GetTruncatedDiff(offsetPixel.B + ((blueError * 7) >> 4));
                bitmap.SetPixel(offsetX, offsetY, Color.FromArgb(R, G, B));
            }

            if (rowInd + 1 < height)
            {
                if (colInd - 1 > 0)
                {
                    // left and down
                    offsetX = colInd - 1;
                    offsetY = rowInd + 1;

                    offsetPixel = bitmap.GetPixel(offsetX, offsetY);
                    R = GetTruncatedDiff(offsetPixel.R + ((redError * 3) >> 4));
                    G = GetTruncatedDiff(offsetPixel.G + ((greenError * 3) >> 4));
                    B = GetTruncatedDiff(offsetPixel.B + ((blueError * 3) >> 4));
                    bitmap.SetPixel(offsetX, offsetY, Color.FromArgb(R, G, B));
                }

                // down
                offsetX = colInd;
                offsetY = rowInd + 1;


                offsetPixel = bitmap.GetPixel(offsetX, offsetY);
                R = GetTruncatedDiff(offsetPixel.R + ((redError * 5) >> 4));
                G = GetTruncatedDiff(offsetPixel.G + ((greenError * 5) >> 4));
                B = GetTruncatedDiff(offsetPixel.B + ((blueError * 5) >> 4));
                bitmap.SetPixel(offsetX, offsetY, Color.FromArgb(R, G, B));

                if (colInd + 1 < width)
                {
                    offsetX = colInd + 1;
                    offsetY = rowInd + 1;

                    offsetPixel = bitmap.GetPixel(offsetX, offsetY);
                    R = GetTruncatedDiff(offsetPixel.R + ((redError * 1) >> 4));
                    G = GetTruncatedDiff(offsetPixel.G + ((greenError * 1) >> 4));
                    B = GetTruncatedDiff(offsetPixel.B + ((blueError * 1) >> 4));
                    bitmap.SetPixel(offsetX, offsetY, Color.FromArgb(R, G, B));
                }
            }

        }

        private byte GetTruncatedDiff(int diff)
        {
            if (diff < 0)
            {
                return 0;
            }

            if (diff > 255)
            {
                return 255;
            }

            return (byte)diff;
        }
    }


    public class ColorFilter
    {
        public TileContainer TileContainer;
        private readonly IColorSpaceComparison _comparer;
        private readonly List<int> _usedTiles;

        public ColorFilter(IEnumerable<PaletteTile> paletteTiles)
        {
            _comparer = new Cie1976Comparison();
            _usedTiles = new List<int>();
            TileContainer = new TileContainer(paletteTiles.ToList());
        }

        public PaletteTile FindBestTile(Rgb Rgb)
        {
            double minimumDeltaE = double.MaxValue;
            PaletteTile result = null;
            foreach (var tileInfo in TileContainer.PaletteTiles)
            {
                var deltaE = GetDifferenceUsingCie1976Comparison(tileInfo.Rgb, Rgb);
                if (deltaE < minimumDeltaE)
                {
                    minimumDeltaE = deltaE;
                    result = tileInfo;
                }
            }

            return result;
        }


        public PaletteTile FindTitle(Rgb Rgb)
        {
            var threshold = 1;
            PaletteTile result = null;
            var hitMatch = false;

            while (result == null)
            {
                hitMatch = false;
                foreach (var title in TileContainer.PaletteTiles)
                {
                    if (GetDifferenceUsingCie1976Comparison(title.Rgb, Rgb) <= threshold)
                    {
                        // if we use title, then go next
                        /*  if (_usedTiles.Contains(title.Key))
                          {
                              hitMatch = true;
                              continue;
                          }*/

                        result = title;
                        _usedTiles.Add(title.Id);
                        break;
                    }
                }

                /*  var oldThreshold = threshold;

                  if (result == null)
                  {
                      if (hitMatch)
                      {
                          foreach (var title in TileContainer.PaletteTiles)
                          {
                              if (!_usedTiles.Contains(title.Key) && GetDifferenceUsingCie1976Comparison(title.Value.Rgb, Rgb) <= threshold + 5)
                              {
                                  result = title.Value;
                                  _usedTiles.Add(title.Key);
                                  break;
                              }
                          }
                      }

                      if (result == null)
                      {
                          threshold = oldThreshold;
                          var _usedTitlesCopy = _usedTiles.ToList();

                          foreach (var usedTile in _usedTitlesCopy)
                          {
                              var title = TileContainer.PaletteTiles[usedTile];
                              if (GetDifferenceUsingCie1976Comparison(title.Rgb, Rgb) <= threshold)
                              {
                                  result = title;
                                  break;
                              }
                          }
                      }
                  }*/

                threshold += 1;
            }

            return result;
        }

        public double GetDifference(Rgb color1, Rgb color2)
        {
            var redMass = (color1.R + color2.R) / 2;
            var deltaRed = color1.R - color2.R;
            var deltaGreen = color1.G - color2.G;
            var deltaBlue = color1.B - color2.B;

            /*return Math.Sqrt(Math.Pow(deltaRed, 2) + Math.Pow(deltaGreen, 2) + Math.Pow(deltaBlue, 2));*/


            return Math.Sqrt((2 + redMass / 256) * Math.Pow(deltaRed, 2) + 4 * Math.Pow(deltaGreen, 2) +
                          (2 + (255 - redMass) / 256) * Math.Pow(deltaBlue, 2));

        }

        public double GetDifferenceUsingCie1976Comparison(Rgb color1, Rgb color2)
        {

            /*            var colorLab1 = new Lab() { L = color1.R, A = color1.G, B = color1.B };
                        var colorLab2 = new Lab() { L = color2.R, A = color2.G, B = color2.B };*/



            var colorRgb1 = new Rgb { R = color1.R, G = color1.G, B = color1.B };
            var colorRgb2 = new Rgb { R = color2.R, G = color2.G, B = color2.B };

            return _comparer.Compare(colorRgb1, colorRgb2);
        }
    }

}
