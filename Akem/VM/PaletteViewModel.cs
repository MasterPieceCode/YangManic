using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Processing;

namespace Akem.VM
{
    public class PaletteViewModel: INotifyPropertyChanged
    {
        private PaletteTile _selectedTile;
        public ObservableCollection<PaletteTile> PaletteTiles { get; set; }

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
            PaletteTiles = new ObservableCollection<PaletteTile>
            {
                                new PaletteTile(0, Colors.White, 10),
                new PaletteTile(1, Colors.Black, 10),
                new PaletteTile(2, Colors.Gray, 10),
                new PaletteTile(3, Colors.DarkGray, 10),
                new PaletteTile(3, Colors.LightSlateGray, 10),
                new PaletteTile(3, Colors.LightGray, 10),
                new PaletteTile(3, Colors.DarkSlateGray, 10),
                //new PaletteTile(3, Colors.Coral, 10),
                //new PaletteTile(3, Colors.Yellow, 10)

/*
                new PaletteTile(0, Colors.Indigo, 10),
                new PaletteTile(1, Colors.ForestGreen, 10),
                new PaletteTile(2, Colors.Firebrick, 10),
                new PaletteTile(3, Colors.DarkOliveGreen, 10),
                new PaletteTile(4, Colors.DarkCyan, 10),
                new PaletteTile(5, Colors.Coral, 10),
                new PaletteTile(6, Colors.Brown, 10),
                new PaletteTile(7, Colors.RoyalBlue, 10),
                new PaletteTile(8, Colors.SeaGreen, 10),
                new PaletteTile(9, Colors.Yellow, 10),
                new PaletteTile(10, Colors.Teal, 10)
*/
            };
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

    }
}
