using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Akem.VM;
using Processing;

namespace Akem.Commands
{
    public class LoadCommand : ICommand
    {
        private readonly RenderViewModel _renderViewModel;

        public LoadCommand(RenderViewModel renderViewModel)
        {
            _renderViewModel = renderViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ProjectInfo projectInfo;
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter ="Project Files|*.ymp";
                fileDialog.InitialDirectory = "Projects";
                
                if (fileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                };

                try
                {
                    projectInfo = ProjectInfo.Load(fileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Unable to load project \r\n. {0}", ex.Message));
                    return;
                }
            }

            _renderViewModel.FileName = projectInfo.FileName;
            _renderViewModel.KeepRatio = projectInfo.KeepRatio;
            _renderViewModel.Width = projectInfo.Width;
            _renderViewModel.Height = projectInfo.Height;
            _renderViewModel.Tiles.SelectedTiles = projectInfo.SelectedTiles;
            _renderViewModel.Tiles.PaletteTiles = projectInfo.PaletteTiles;
        }

        public event EventHandler CanExecuteChanged;
    }
}
