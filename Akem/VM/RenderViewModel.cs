using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Akem.Annotations;
using Akem.Commands;
using Akem.Controls;
using Processing;

namespace Akem.VM
{
    public class RenderViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ObservableCollection<PaletteTile>> _mozaicTiles;
        private int _width;
        private int _height;
        private string _fileName;
        private PaletteViewModel _tiles;
        private ObservableCollection<MozaicStatistic> _mozaicStatistics;

        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;

                if (!_isAdjusting && KeepRatio)
                {
                    AdjsutHeight();
                }
                
                OnPropertyChanged();
            }
        }

        private bool _isAdjusting;

        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;

                if (!_isAdjusting && KeepRatio)
                {
                    AdjustWidth();
                }
                OnPropertyChanged();
            }
        }


        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                Image = new Bitmap(_fileName);

                _isAdjusting = true;

                Width = Image.Width;
                Height = Image.Height;

                _isAdjusting = false;

                OnPropertyChanged();
            }
        }

        public Bitmap Image { get; private set; }

        public bool KeepRatio { get; set; }

        public ICommand RenderCommand { get; set; }
        public MozaicCanvas Canvas { get; set; }

        public PaletteViewModel Tiles
        {
            get { return _tiles; }
            set { _tiles = value; _tiles.PropertyChanged += TilesPropertyChanged; }
        }

        private void TilesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SelectedTiles"))
            {
                OnPropertyChanged("Tiles");
            }
        }

        public GroutViewModel Grout { get; set; }

        public ObservableCollection<ObservableCollection<PaletteTile>> MozaicTiles
        {
            get { return _mozaicTiles; }
            set
            {
                _mozaicTiles = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MozaicStatistic> MozaicStatistics
        {
            get { return _mozaicStatistics; }
            set { _mozaicStatistics = value; OnPropertyChanged(); }
        }

        public RenderViewModel()
        {
            RenderCommand = new RenderCommand(this);
            KeepRatio = true;
        }

        private int GetRatio(Func<Size, double> ratio, int current, int oppositeDimension)
        {
            if (Image == null)
            {
                return current;

            }
            return (int)(oppositeDimension * ratio(Image.Size));
        }

        private void AdjsutHeight()
        {
            _isAdjusting = true;

            Height =  GetRatio(size => ((double)size.Height / size.Width), Height, Width);

            _isAdjusting = false;
        }

        private void AdjustWidth()
        {
            _isAdjusting = true;

            Width = GetRatio(size => (double)size.Width / size.Height, Width, Height);

            _isAdjusting = false;
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
}
