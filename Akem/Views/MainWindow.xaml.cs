using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Akem.Controls;
using Akem.VM;
using Processing;

namespace Akem.Views
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

        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {
            var fileDrop = (string[])e.Data.GetData("FileName");
            if (!fileDrop.Any())
            {
                return;
            }

            var fileName = fileDrop[0];
            SetFileName(fileName);

            var mozaicCanvas = (MozaicCanvas)Resources["MozaicCanvas"];
            mozaicCanvas.Clear();

            var visual = ImageHelper.GetFileImage(fileName, ImageScrollViewerViewer.ActualWidth, ImageScrollViewerViewer.ActualHeight);
            mozaicCanvas.AddVisual(visual);

            InitMozaicCanvas(mozaicCanvas, ImageScrollViewerViewer.ActualWidth, ImageScrollViewerViewer.ActualHeight);
        }

        private void SetFileName(string fileName)
        {
            ((RenderViewModel) Resources["RenderViewModel"]).FileName = fileName;
        }


        private void InitMozaicCanvas(MozaicCanvas mozaicCanvas, double width, double height)
        {
            ImagePanel.Children.Clear();
            ImagePanel.Children.Add(mozaicCanvas);

            mozaicCanvas.Children.Clear();
            mozaicCanvas.Width = width;
            mozaicCanvas.Height = height;
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
