using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Processing;

namespace Akem.VM
{
    public class PaletteViewModel: INotifyPropertyChanged
    {
        private PaletteTile _selectedTile;

        public ObservableCollection<PaletteTile> PaletteTiles { get; set; }
        public ObservableCollection<PaletteTile> SelectedTiles { get; set; }
        public ICommand SelectTileCommand { get; set; }


        public PaletteTile SelectedTile
        {
            get { return _selectedTile; }
            set
            {
                _selectedTile = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedTile"));
            }
        }

        public PaletteViewModel()
        {
            SelectTileCommand = new SelectTileCommand(this);
            SelectedTiles = new ObservableCollection<PaletteTile>();
            PaletteTiles = new ObservableCollection<PaletteTile>();
            foreach (var tileInfo in TileLibrary.TileBase)
            {
                PaletteTiles.Add(new PaletteTile(tileInfo.Value, 10));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }

    public class SelectTileCommand : ICommand
    {
        private readonly PaletteViewModel _viewModel;

        public SelectTileCommand(PaletteViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return _viewModel.PaletteTiles.Any() || _viewModel.SelectedTiles.Any();
        }

        public void Execute(object parameter)
        {
            var palleteTile = (PaletteTile) parameter;
            if (_viewModel.PaletteTiles.SingleOrDefault(i => i.Id == palleteTile.Id) != null)
            {
                _viewModel.PaletteTiles.Remove(palleteTile);
                _viewModel.SelectedTiles.Add(palleteTile);
            }
            else
            {
                _viewModel.SelectedTiles.Remove(palleteTile);
                _viewModel.PaletteTiles.Add(palleteTile);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
