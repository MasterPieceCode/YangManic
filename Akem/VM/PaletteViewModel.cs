using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Akem.Annotations;
using Processing;

namespace Akem.VM
{
     public class MozaicStatistic
    {
         public string Id { get { return Tile.Id; } }
         public PaletteTile Tile { get; set; }
         public int Count { get; set; }
    }

    public class PaletteViewModel: INotifyPropertyChanged
    {
        private PaletteTile _selectedTile;
        private ObservableCollection<PaletteTile> _paletteTiles;
        private ObservableCollection<PaletteTile> _selectedTiles;

        public ObservableCollection<PaletteTile> PaletteTiles
        {
            get
            {
                return _paletteTiles ?? (_paletteTiles = GetPalleteTiles());
            }

            set
            {
                _paletteTiles = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PaletteTile> GetPalleteTiles()
        {
            var result = new ObservableCollection<PaletteTile>();
            foreach (var tileInfo in TileLibrary.TileBase)
            {
                result.Add(new PaletteTile(tileInfo.Value, 10));
            }

            return result;
        }

        public ObservableCollection<PaletteTile> SelectedTiles
        {
            get { return _selectedTiles; }
            set
            {
                _selectedTiles = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectTileCommand { get; set; }


        public PaletteTile SelectedTile
        {
            get { return _selectedTile; }
            set
            {
                _selectedTile = value;
                OnPropertyChanged();
            }
        }

        public PaletteViewModel()
        {
            SelectTileCommand = new SelectTileCommand(this);
            SelectedTiles = new ObservableCollection<PaletteTile>();
            SelectedTiles.CollectionChanged += SelectedTilesCollectionChanged;
            
        }

        private void SelectedTilesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("SelectedTiles");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
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
