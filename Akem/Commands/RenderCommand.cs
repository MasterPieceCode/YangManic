using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Akem.Controls;
using Akem.VM;
using Processing;

namespace Akem.Commands
{
    public class RenderCommand : ICommand
    {
        private const int TileSize = 10;

        private readonly RenderViewModel _renderViewModel;
        private MozaicResult _mozaicResult;

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
/*
            var drawingVisual = ImageHelper.GetFileImage(_renderViewModel.FileName, mozaicCanvas.Width, mozaicCanvas.Height);
            drawingVisual.Opacity = 0.2;
*/
        }

        private void FillMozaicStatisitcs(MozaicResult mozaicResult)
        {
            _mozaicResult = mozaicResult;
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
            var clientWidth = canvas.ActualWidth;
            var clientHeight = canvas.ActualHeight;

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

        public event EventHandler CanExecuteChanged;
    }
}