using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Akem.VM;
using Processing;

namespace Akem.Commands
{
    public class SaveCommand : ICommand
    {
        private readonly RenderViewModel _renderViewModel;

        public SaveCommand(RenderViewModel renderViewModel)
        {
            _renderViewModel = renderViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var fileName = string.Format("{0}_{1}x{2}", Path.GetFileNameWithoutExtension(_renderViewModel.FileName), _renderViewModel.Width, _renderViewModel.Height);
            SaveFileDialog fileDialog;
            using (fileDialog = new SaveFileDialog())
            {
                InitFileDialog(fileDialog, fileName);
                if (fileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                };
            }

            switch (fileDialog.FilterIndex)
            {
                case 1:
                    SaveProject(fileDialog.FileName);
                    break;
                case 2:
                    SaveImage(fileDialog.FileName);
                    break;
            }
        }

        private static void InitFileDialog(SaveFileDialog fileDialog, string fileName)
        {
            fileDialog.InitialDirectory = "Projects";
            fileDialog.Filter = "Project Files|*.ymp|Image Files|*.jpg";
            fileDialog.FileName = fileName;
        }

        private void SaveProject(string fileName)
        {
            var projectInfo = new ProjectInfo
            {
                FileName = _renderViewModel.FileName,
                Width = _renderViewModel.Width,
                KeepRatio = _renderViewModel.KeepRatio,
                Height = _renderViewModel.Height,
                SelectedTiles = _renderViewModel.Tiles.SelectedTiles,
                PaletteTiles = _renderViewModel.Tiles.PaletteTiles
            };

            projectInfo.Save(fileName);
        }

        private void SaveImage(string fileName)
        {
            if (_renderViewModel.MozaicTiles == null || !_renderViewModel.MozaicTiles.Any())
            {
                MessageBox.Show("Please render first", "Unable to save", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var height = _renderViewModel.MozaicTiles.Sum(t => _renderViewModel.Tiles.SelectedTiles.Single(ot => ot.Id == t.First().Id).Bitmap.Height);
            var width = _renderViewModel.MozaicTiles.First().Sum(t => _renderViewModel.Tiles.SelectedTiles.Single(ot => ot.Id == t.Id).Bitmap.Width);

            const int maxSize = 10000;
            float tileRatio = 1;

            if (width > maxSize && width > height)
            {
                var heightToWidthRatio = (float)height / width;
                tileRatio = (float)maxSize / width;
                width = maxSize;
                height = (int)(width * heightToWidthRatio);
            }
            else if (height > maxSize)
            {
                var widthToHeightRation = (float)width / height;
                tileRatio = (float)maxSize / height;
                height = maxSize;
                width = (int)(height * widthToHeightRation);
            }

            GC.Collect();

            using (var bitmap = new Bitmap(width, height))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    var rowInd = 0;

                    foreach (var mozaicRow in _renderViewModel.MozaicTiles)
                    {
                        var colInd = 0;
                        foreach (var mozaicColumn in mozaicRow)
                        {
                            var originalTile = _renderViewModel.Tiles.SelectedTiles.Single(t => t.Id == mozaicColumn.Id);

                            var tileWidth = originalTile.Bitmap.Width*tileRatio;
                            var tileHeight = originalTile.Bitmap.Height*tileRatio;
                            graphics.DrawImage(originalTile.Bitmap,
                                new Rectangle((int) tileWidth*colInd, (int) tileHeight*rowInd, (int) tileWidth,
                                    (int) tileHeight),
                                new Rectangle(0, 0, originalTile.Bitmap.Width, originalTile.Bitmap.Height),
                                GraphicsUnit.Pixel);
                            colInd++;
                        }

                        rowInd++;
                    }

                    bitmap.Save(fileName);
                }
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
