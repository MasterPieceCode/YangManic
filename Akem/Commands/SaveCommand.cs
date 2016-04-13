using System;
using System.Collections.Generic;
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
            var fileName = string.Format("{0}_{1}x{2}.ymp", Path.GetFileNameWithoutExtension(_renderViewModel.FileName), _renderViewModel.Width, _renderViewModel.Height);
            using (var fileDialog = new SaveFileDialog())
            {
                fileDialog.InitialDirectory = "Projects";
                fileDialog.Filter = "Project Files|*.ymp";
                fileDialog.FileName = fileName;
                if (fileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                };

                fileName = fileDialog.FileName;
            }

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

        public event EventHandler CanExecuteChanged;
    }
}
