using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Akem.Controls;
using Processing;
using Color = System.Windows.Media.Color;

namespace Akem.VM
{
    public class RenderViewModel
    {
        public int Widht { get; set; }
        public int Height { get; set; }
        public string FileName { get; set; }
        public ICommand RenderCommand { get; set; }
        public MozaicCanvas Canvas { get; set; }
        public PaletteViewModel Tiles { get; set; }
        public GroutViewModel Grout { get; set; }

        public RenderViewModel()
        {
            RenderCommand = new RenderCommand(this);
            Widht = 500;
            Height = 500;
        }
    }

    public class PrintCommand : ICommand
    {
        private readonly ObservableCollection<ObservableCollection<PaletteTile>> _tiles;
        private int _pageNumber;
        private int _verIndex;
        private int _horIndex;
        private bool _rowProcessingFinished;
        private int[] _colInd;

        public PrintCommand(ObservableCollection<ObservableCollection<PaletteTile>>  tiles)
        {
            _tiles = tiles;
            _colInd = new int[_tiles[0].Count];
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _pageNumber = 0;
            _verIndex = 0;
            _horIndex = 0;

            // Create a new PrintPreviewDialog using constructor.
            var printPreviewDialog = new PrintPreviewDialog();

            //Set the size, location, and name.
            printPreviewDialog.ClientSize = new System.Drawing.Size(400, 300);
            printPreviewDialog.Location = new System.Drawing.Point(29, 29);

            var printDocument = new PrintDocument();

            // Associate the event-handling method with the 
            // document's PrintPage event.
            printDocument.PrintPage += PrintPage;

            // Set the minimum size the dialog can be resized to.
            printPreviewDialog.MinimumSize =
                new System.Drawing.Size(375, 250);

            // Set the UseAntiAlias property to true, which will allow the 
            // operating system to smooth fonts.
            printPreviewDialog.UseAntiAlias = true;

            printPreviewDialog.Document = printDocument;
            printPreviewDialog.ShowDialog();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            var max = Math.Max(_tiles.Count, _tiles[0].Count);
            var pagesCount = max / 30;
            if ((max % 30) > 0)
            {
                ++pagesCount;
            }

            var paperSize = e.PageSettings.PaperSize;
            var stepSize = Math.Min(paperSize.Width, paperSize.Height)/30;

            var initialHorIndex = _horIndex;
            var i = 0;

            for (var rowInd = _verIndex * 30; rowInd < Math.Min((_verIndex + 1) * 30, _tiles.Count); rowInd++)
            {
                i++;
                var j = 0;

                int colInd;

                _rowProcessingFinished = false;

                for (colInd = initialHorIndex; colInd < Math.Min(initialHorIndex + 30, _tiles[rowInd].Count); colInd++)
                {
                    j++;
                    var bitmap = _tiles[rowInd][colInd].Bitmap;
                    e.Graphics.DrawImage(bitmap, new Rectangle(j * stepSize, i * stepSize, stepSize, stepSize), new Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height), GraphicsUnit.Pixel);
                }

                if (colInd == _tiles[rowInd].Count)
                {
                    _rowProcessingFinished = true;
                }
                else
                {
                    _horIndex = colInd;
                }
            }

            if (_rowProcessingFinished)
            {
                _verIndex++;
                _horIndex = 0;
            }

            if  ((_pageNumber + 1) * 30 >=  _tiles.Count && _rowProcessingFinished)
            {
                e.HasMorePages = false;
                return;
            }
            _pageNumber++;
            e.HasMorePages = true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class RenderCommand : ICommand
    {
        private const int TileSize = 10;

        private readonly RenderViewModel _renderViewModel;

        public RenderCommand(RenderViewModel renderViewModel)
        {
            _renderViewModel = renderViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
            //TODO Implement later
            //_renderViewModel.Widht > TileSize && _renderViewModel.Height > TileSize;
        }

        public void Execute(object parameter)
        {
            var result = new ImageToMozaicConverter(_renderViewModel.FileName, TileSize, _renderViewModel.Tiles.SelectedTiles, _renderViewModel.Widht, _renderViewModel.Height).Convert();
       //     new PrintCommand(result).Execute(null);

            var mozaicCanvas = _renderViewModel.Canvas;

            mozaicCanvas.Clear();
            FillCanvasWithTiles(mozaicCanvas, _renderViewModel.Widht, _renderViewModel.Grout.SelectedGrout.Thikness, _renderViewModel.Grout.SelectedGrout.Color, TileSize, TileSize, result);
            var drawingVisual = ImageHelper.GetFileImage(_renderViewModel.FileName, mozaicCanvas.Width, mozaicCanvas.Height);
            drawingVisual.Opacity = 0.2;

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

                    var selectedBrush = new SolidColorBrush(Color.FromArgb(255, (byte)mozaicTileColumn.Rgb.R, (byte)mozaicTileColumn.Rgb.G, (byte)mozaicTileColumn.Rgb.B));

                    dc.DrawRectangle(selectedBrush, null, new Rect(positionX, positionY, gridWidth, gridHeight));

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
