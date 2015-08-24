using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Akem.Controls;
using Akem.VM;
using Processing;

namespace Akem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImageScrollViewerViewer_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {
            var fileDrop = (string[])e.Data.GetData("FileName");
            if (!fileDrop.Any())
            {
                return;
            }

            FileName = fileDrop[0];

            var mozaicCanvas = (MozaicCanvas)MainGrid.Resources["MozaicCanvas"];
            mozaicCanvas.Clear();

            var visual = DrawImage(ImageScrollViewerViewer.ActualWidth, ImageScrollViewerViewer.ActualHeight);
            mozaicCanvas.AddVisual(visual);

            InitMozaicCanvas(ImageScrollViewerViewer.ActualWidth, ImageScrollViewerViewer.ActualHeight);
        }

        private DrawingVisual DrawImage(double width, double height)
        {/*
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.Red);
            colors.Add(System.Windows.Media.Colors.Blue);
            colors.Add(System.Windows.Media.Colors.Green);
            BitmapPalette myPalette = new BitmapPalette(((PaletteViewModel)PalettePanel.DataContext).PaletteTiles.Select(pt => pt.Color).ToList());

*/
            var bi = new BitmapImage(new Uri(FileName));

          /*  //var formatter = new FormatConvertedBitmap(bi, PixelFormats.Bgr24, myPalette, 0);
            FormatConvertedBitmap fcb =
                       new FormatConvertedBitmap();
            fcb.BeginInit();
            fcb.Source = bi;
            fcb.DestinationFormat = PixelFormats.Indexed4;
            fcb.DestinationPalette = myPalette;
            fcb.EndInit();

            
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            Guid photoID = System.Guid.NewGuid();
            String photolocation = "res.jpg";  //file name 


            var bitmap = new TransformedBitmap(fcb,
                new ScaleTransform(300/ fcb.Width, 150 / fcb.Height));


            var bitmapFrame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(bitmapFrame);


            using (var filestream = new FileStream(photolocation, FileMode.Create))
                encoder.Save(filestream);*/


            var imageBrush = new ImageBrush(bi) { Stretch = Stretch.Uniform };

            var visual = new DrawingVisual();
            var dc = visual.RenderOpen();
            dc.DrawRectangle(imageBrush, null,
                new Rect(new Size(width, height)));
            dc.Close();
            return visual;
        }

        protected string FileName { get; set; }

        private void InitMozaicCanvas(double width, double height)
        {
            var mozaicCanvas = (MozaicCanvas)MainGrid.Resources["MozaicCanvas"];

            ImagePanel.Children.Clear();
            ImagePanel.Children.Add(mozaicCanvas);

            mozaicCanvas.Children.Clear();
            mozaicCanvas.Width = width;
            mozaicCanvas.Height = height;
        }

        private void FillCanvasWithTiles(MozaicCanvas canvas)
        {
            var tileSize = 15;

             var clientWidth = canvas.Width;
             var clientHeight = canvas.Height;

             var tilesCountHor = clientWidth / tileSize;
             var tilesCountVer = clientHeight / tileSize;

             var gridWidth = clientWidth / tilesCountHor;
             var gridHeight = clientHeight / tilesCountVer;


             var brush1 = new SolidColorBrush(Colors.DimGray);
             var brush2 = new SolidColorBrush(Colors.DarkGray);

             var brushes = new Brush[] {brush1, brush2};

             var randomizer = new Random(0);

             for (int i = 0; i < tilesCountVer; i++)
             {
                 for (int j = 0; j < tilesCountHor; j++)
                 {
                     var positionX = gridWidth * j;
                     var positionY = gridHeight * i;
                     var visual = new DrawingVisual();
                     var dc = visual.RenderOpen();

                     var selectedBrush = brushes[randomizer.Next(2)];

                     dc.DrawRectangle(selectedBrush, new Pen(new SolidColorBrush(Colors.Black), 0.3), new Rect(positionX, positionY, gridWidth, gridHeight));

                     dc.Close();
                     canvas.AddVisual(visual);
                 }
             }
        }

        private void FillCanvasWithTiles(MozaicCanvas canvas, int panoWidth, double grouthWidth, Color groutColor, int tileWidht, int tileHeight, IEnumerable<IEnumerable<PaletteTile>> mozaicTiles)
        {
            var clientWidth = canvas.ActualWidth;
            var clientHeight = canvas.ActualHeight;

            var tilesCountHor = mozaicTiles.First().Count();
            var tilesCountVer = mozaicTiles.Count();

            var penGroutWidth = (clientWidth / panoWidth) * grouthWidth;

            var gridWidth = clientWidth / tilesCountHor - (penGroutWidth /2);

            var gridHeight = clientHeight / tilesCountVer - (penGroutWidth /2);

            if (gridWidth > gridHeight)
            {
                gridWidth = gridHeight* tileWidht/tileHeight;
            }
            else
            {
                gridHeight = gridWidth* tileHeight/tileWidht;
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
                    var positionX = (gridWidth + penGroutWidth )* colInd;
                    var positionY = (gridHeight  + penGroutWidth)* rowInd;
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

        private void GrouteColorSelected(object sender, MouseButtonEventArgs e)
        {
            var rectange = (Rectangle) sender;
            ((GroutViewModel)GroutComboBox.DataContext).SelectedColor = ((SolidColorBrush)rectange.Fill).Color;
        }

        private void TileSelectionChanged(object sender, MouseButtonEventArgs e)
        {
            var rectange = (Rectangle)sender;
            ((PaletteViewModel)PalettePanel.DataContext).SelectedTile = (PaletteTile) rectange.DataContext;
        }

        private void Render(object sender, RoutedEventArgs routedEventArgs)
        {
            var viewModel = (RenderViewModel) RenderingPanel.DataContext;
            var paletteViewModel = ((PaletteViewModel) PalettePanel.DataContext);
            var groutViewModel = ((GroutViewModel) GroutComboBox.DataContext);
            var result = new ImageToMozaicConverter(FileName,  10, paletteViewModel.PaletteTiles, viewModel.Widht, viewModel.Height).Convert();
            var mozaicCanvas = (MozaicCanvas)MainGrid.Resources["MozaicCanvas"];
            
            mozaicCanvas.Clear();
            FillCanvasWithTiles(mozaicCanvas, viewModel.Widht, groutViewModel.SelectedGrout.Thikness, groutViewModel.SelectedGrout.Color, 10, 10, result);
            var drawingVisual = DrawImage(mozaicCanvas.Width, mozaicCanvas.Height);
            drawingVisual.Opacity = 0.2;
   //         mozaicCanvas.AddVisual(drawingVisual);

        }

        private void CanvasGrid_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var panel = (UIElement)sender;

            Point p = e.MouseDevice.GetPosition(panel);

            Matrix m = panel.RenderTransform.Value;
            if (e.Delta > 0)
                m.ScaleAtPrepend(1.1, 1.1, p.X, p.Y);
            else
                m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, p.X, p.Y);

            panel.RenderTransform = new MatrixTransform(m);
        }
    }
}
