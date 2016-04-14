using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Akem.Annotations;
using Akem.Controls;
using Akem.VM;
using Processing;

namespace Akem.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ImageScrollViewerViewer.SizeChanged += ImageScrollViewerViewerSizeChanged;
        }

        private void ImageScrollViewerViewerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var rvm = ((RenderViewModel)Resources["RenderViewModel"]);
            rvm.CanvasWidth = ImageScrollViewerViewer.ActualWidth;
            rvm.CanvasHeight = ImageScrollViewerViewer.ActualHeight;
        }

        private void ImagePanelDrop(object sender, DragEventArgs e)
        {
            var fileDrop = (string[])e.Data.GetData("FileName");
            if (!fileDrop.Any())
            {
                return;
            }

            var fileName = fileDrop[0];
            SetFileName(fileName);
        }

        private void SetFileName(string fileName)
        {
            ((RenderViewModel) Resources["RenderViewModel"]).FileName = fileName;
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
         var formatter = new BinaryFormatter();

/*
         var fs = new FileStream("project.dat", FileMode.OpenOrCreate);
         formatter.Serialize(fs, projectInfo);
*/

        var fs = new FileStream("project.dat", FileMode.Open);
        var res = (ProjectInfo)formatter.Deserialize(fs);

           
        }
    }
}
