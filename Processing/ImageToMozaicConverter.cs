using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ColorMine.ColorSpaces;
using Color = System.Drawing.Color;

namespace Processing
{
    public class ImageToMozaicConverter
    {
        private readonly Bitmap _image;
        private readonly int _desirableWidth;
        private readonly int _desirableHeight;
        private readonly AvgColorCalculator _avgColorCalculator;
        private readonly ColorFilter _colorFilter;
        private readonly int _gridHorSteps;
        private readonly int _gridVerSteps;
        private readonly int _gridCellWidth;
        private readonly int _gridCellHeight;
        private readonly Size _gridCellSize;



        public ImageToMozaicConverter(Bitmap bitmap, int tileSize, IEnumerable<PaletteTile> paletteTiles, int desirableWidth, int desirableHeight)
        {
            _image = bitmap;

            _desirableWidth = desirableWidth;

            _desirableHeight = desirableHeight;

            _gridHorSteps = _desirableWidth / tileSize;
            _gridVerSteps = _desirableHeight / tileSize;

            var scale = Math.Min((float)_gridHorSteps / _image.Width, (float)_gridVerSteps / _image.Height);         

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


        public MozaicResult Convert()
        {
            var result =new MozaicResult();
            //var convertedBitmap = new Bitmap(_gridCellWidth * _gridHorSteps, _gridCellHeight * _gridVerSteps);
            //var convertedImGraphics = Graphics.FromImage(convertedBitmap);
            //            convertedImGraphics.CompositingQuality = CompositingQuality.HighQuality;
            //convertedImGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //TileContainer.PaletteTiles.Clear();

            var newImage = new Bitmap(_gridHorSteps, _gridVerSteps);

            var c = 0;
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

                newImage.Save("new image.jpg");
            }

            for (var y = 0; y < newImage.Height; y++)
            {
                var verticalTiles = new ObservableCollection<PaletteTile>();
                result.Tiles.Add(verticalTiles);
                for (var x = 0; x < newImage.Width; x++)
                {
                    var pixel = newImage.GetPixel(x, y);

                    var rgb = new Rgb { R = pixel.R, G = pixel.G, B = pixel.B };

                    var findBestMatchTitle = _colorFilter.FindBestTile(new Rgb(pixel.R, pixel.G, pixel.B));

                    var newRgb = findBestMatchTitle.Rgb;
                    var paletteColor = new Rgb { R = newRgb.R, G = newRgb.G, B = newRgb.B };
                    SetErrorForPixel(rgb, paletteColor, y, x, newImage.Width, newImage.Height, newImage);

                    verticalTiles.Add(findBestMatchTitle);
                    if (!result.MozaicStatisitcs.ContainsKey(findBestMatchTitle.Id))
                    {
                        result.MozaicStatisitcs.Add(findBestMatchTitle.Id, 0);
                    }

                    result.MozaicStatisitcs[findBestMatchTitle.Id]++;
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
}
