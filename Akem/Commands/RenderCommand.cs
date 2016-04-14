using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Akem.Controls;
using Akem.VM;
using Processing;
using Color = System.Windows.Media.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Akem.Commands
{
    public class RenderCommand : ICommand
    {
        private const int TileSize = 10;

        private readonly RenderViewModel _renderViewModel;

        public event EventHandler<EventArgs> ExecuteCompleted = delegate {};

        public RenderCommand(RenderViewModel renderViewModel)
        {
            _renderViewModel = renderViewModel;
            _renderViewModel.PropertyChanged += RenderViewModelPropertyChanged;
        }

        private void RenderViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Width") || e.PropertyName.Equals("Height") || e.PropertyName.Equals("FileName") || e.PropertyName.Equals("Tiles"))
            {
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return _renderViewModel.Width >= TileSize
                   && _renderViewModel.Height >= TileSize
                   && !string.IsNullOrEmpty(_renderViewModel.FileName)
                   && _renderViewModel.Tiles != null
                   && _renderViewModel.Tiles.SelectedTiles.Count > 0;
        }

        public void Execute(object parameter)
        {
            var mozaicResult = new ImageToMozaicConverter(_renderViewModel.Image, TileSize, _renderViewModel.Tiles.SelectedTiles, _renderViewModel.Width, _renderViewModel.Height).Convert();
            _renderViewModel.MozaicTiles = mozaicResult.Tiles;
            FillMozaicStatisitcs(mozaicResult);
            
            var mozaicCanvas = _renderViewModel.Canvas;

            mozaicCanvas.Clear();
            FillCanvasWithTiles(mozaicCanvas, _renderViewModel.Width, _renderViewModel.Grout.SelectedGrout.Thikness, _renderViewModel.Grout.SelectedGrout.Color, TileSize, TileSize, _renderViewModel.MozaicTiles);

            ExecuteCompleted(this, EventArgs.Empty);
        }

        private void FillMozaicStatisitcs(MozaicResult mozaicResult)
        {
            _renderViewModel.MozaicStatistics = new ObservableCollection<MozaicStatistic>();

            foreach (var mozaicStatistic in mozaicResult.MozaicStatisitcs.Select(statistic => new MozaicStatistic
            {
                Count = statistic.Value,
                Tile = _renderViewModel.Tiles.SelectedTiles.First(t => t.Id == statistic.Key)
            }))
            {
                _renderViewModel.MozaicStatistics.Add(mozaicStatistic);
            }
        }

        private void FillCanvasWithTiles(MozaicCanvas canvas, int panoWidth, double grouthWidth, Color groutColor, int tileWidht, int tileHeight, IEnumerable<IEnumerable<PaletteTile>> mozaicTiles)
        {
            var clientWidth = canvas.Width;
            var clientHeight = canvas.Height;
            var tilesCountHor = mozaicTiles.First().Count();
            var tilesCountVer = mozaicTiles.Count();

            var penGroutWidth = (clientWidth / panoWidth) * grouthWidth;

            var gridWidth = clientWidth / tilesCountHor - (penGroutWidth / 2);

            var gridHeight = clientHeight / tilesCountVer - (penGroutWidth / 2);

            if (gridWidth > gridHeight)
            {
                gridWidth = gridHeight * tileWidht / tileHeight;
            }
            else
            {
                gridHeight = gridWidth * tileHeight / tileWidht;
            }

            canvas.Background = new SolidColorBrush(groutColor);
            canvas.Width = gridWidth * tilesCountHor;
            canvas.Height = gridHeight * tilesCountVer;
         //   canvas.CacheMode = new BitmapCache { EnableClearType = false, RenderAtScale = 1, SnapsToDevicePixels = false };

            //   var pen = new Pen(new SolidColorBrush(groutColor), groutWidth);
            //            var gridHeight = gridWidth *  ((double)tileHeight / tileWidht);

            var colInd = 0;
            var rowInd = 0;

             foreach (var mozaicTileRow in mozaicTiles)
            {
                colInd = 0;
                foreach (var mozaicTileColumn in mozaicTileRow)
                {
                    var positionX = (gridWidth + penGroutWidth) * colInd;
                    var positionY = (gridHeight + penGroutWidth) * rowInd;
                    var visual = new DrawingVisual();
                    var dc = visual.RenderOpen();

                    var tile = _renderViewModel.Tiles.SelectedTiles.Single(t => t.Id == mozaicTileColumn.Id);

                    dc.DrawImage(tile.BitmapImage, new Rect(positionX, positionY, gridWidth, gridHeight));

                    dc.Close();

                    canvas.AddVisual(visual);

                    colInd++;
                }

                rowInd++;
            }
        }


        // using multi tasks to add tiles to canvas
        private void AddUsingMultiTasks(MozaicCanvas canvas, IEnumerable<IEnumerable<PaletteTile>> mozaicTiles, int tilesCountVer, int tilesCountHor,
            double gridWidth, double gridHeight, double penGroutWidth)
        {
            var tileList = mozaicTiles.ToList();

            var tasks = new List<Task>();
            for (var i = 0; i < tilesCountVer; i++)
            {
                var tiles = new PaletteTile[1, tilesCountHor];

                var paletteTiles = tileList[i].ToArray();
                for (var j = 0; j < paletteTiles.Length; j++)
                {
                    tiles[0, j] = paletteTiles[j];
                }

                var renderContext = new RenderContext(i, 0, tiles);
                DrawRenderContext(renderContext, canvas, gridWidth, gridHeight, penGroutWidth);
                tasks.Add(
                    Task.Factory.StartNew(() => DrawRenderContext(renderContext, canvas, gridWidth, gridHeight, penGroutWidth)));
            }

            Task.WaitAll(tasks.ToArray());
        }


        // writing tiles on one bitmap and add it to canvas
        private void AddAsWritableBitmap(MozaicCanvas canvas, IEnumerable<IEnumerable<PaletteTile>> mozaicTiles, double gridWidth, double penGroutWidth,
            double gridHeight, int rowInd)
        {
            int colInd;
            var writeableBitmap = new WriteableBitmap((int) canvas.Width, (int) canvas.Height, 96, 96, PixelFormats.Bgra32, null);

            foreach (var mozaicTileRow in mozaicTiles)
            {
                colInd = 0;
                foreach (var mozaicTileColumn in mozaicTileRow)
                {
                    var positionX = (gridWidth + penGroutWidth)*colInd;
                    var positionY = (gridHeight + penGroutWidth)*rowInd;

                    var tile = _renderViewModel.Tiles.SelectedTiles.Single(t => t.Id == mozaicTileColumn.Id);

                    var newPosition = new Int32Rect(0, 0, tile.Bitmap.Width, tile.Bitmap.Height);

                    writeableBitmap.WritePixels(newPosition, tile.Bytes, 944, (int) positionX, (int) positionY);

                    colInd++;
                }

                rowInd++;
            }

            var visual = new DrawingVisual();
            var dc = visual.RenderOpen();
            dc.DrawImage(writeableBitmap, new Rect(0, 0, writeableBitmap.Width, writeableBitmap.Height));

            dc.Close();

            canvas.AddVisual(visual);
        }


        private void DrawRenderContext(RenderContext renderContext, MozaicCanvas canvas, double gridWidth, double gridHeight, double penGroutWidth)
        {
            var stopWatch = new Stopwatch();
            var rowInd = renderContext.StartRowInd;

            for (var i = 0; i < renderContext.Tiles.GetLength(0); i++)
            {
                var colInd = renderContext.StartColInd;

                for (var j = 0; j < renderContext.Tiles.GetLength(1); j++)
                {
                    var mozaicTile = renderContext.Tiles[i, j];
                    var positionX = (gridWidth + penGroutWidth)*(colInd + j);
                    var positionY = (gridHeight + penGroutWidth)*(rowInd + i);

                    var tile = _renderViewModel.Tiles.SelectedTiles.Single(t => t.Id == mozaicTile.Id);


                    canvas.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var visual = new DrawingVisual();

                        var dc = visual.RenderOpen();
                        dc.DrawImage(tile.BitmapImage, new Rect(positionX, positionY, gridWidth, gridHeight));
//                        stopWatch.Start();
                        dc.Close();


                     //   stopWatch.Stop();

                    ///    Debug.WriteLine(stopWatch.ElapsedMilliseconds);

                        canvas.AddVisual(visual);

                    }
                    ), DispatcherPriority.Normal);
                }
            }
        }

        public event EventHandler CanExecuteChanged;
    }

    public class RenderContext
    {
        public RenderContext(int startRowInd, int startColInd, PaletteTile[,] tiles)
        {
            StartColInd = startColInd;
            StartRowInd = startRowInd;
            Tiles = tiles;
        }

        public int StartColInd { get; private set; }
        public int StartRowInd { get; private set; }
        public PaletteTile[,] Tiles { get; private set; }
    }
}