using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Markup;
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

        public double CanvasWidth { get; set; }
        public double CanvasHeight { get; set; }

        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;

                if (!_isAdjustingRatio && KeepRatio)
                {
                    AdjsutHeight();
                }
                
                OnPropertyChanged();
            }
        }

        private bool _isAdjustingRatio;
        private bool _keepRatio;

        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;

                if (!_isAdjustingRatio && KeepRatio)
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

                _isAdjustingRatio = true;

                Width = Image.Width;
                Height = Image.Height;

                InitMozaicCanvas();

                _isAdjustingRatio = false;

                OnPropertyChanged();

                OperationViewModel.IsOriginalImageDisplayed = true;
            }
        }

        private void InitMozaicCanvas()
        {
            SetCanvasSize(Canvas);
            SetCanvasSize(OriginalCanvas);

            var visual = ImageHelper.GetFileImage(FileName, CanvasWidth, CanvasHeight);
            OriginalCanvas.AddVisual(visual);
        }

        private void SetCanvasSize(MozaicCanvas mozaicCanvas)
        {
            mozaicCanvas.Clear();

            mozaicCanvas.Width = CanvasWidth;
            mozaicCanvas.Height = CanvasHeight;
        }

        public Bitmap Image { get; private set; }

        public bool KeepRatio
        {
            get { return _keepRatio; }
            set {
                _keepRatio = value;
                AdjsutHeight();
                OnPropertyChanged();
            }
        }

        public RenderCommand RenderCommand { get; set; }
        public LoadCommand LoadCommand { get; set; }
        public SaveCommand SaveCommand { get; set; }

        public MozaicCanvas Canvas { get; set; }
        public MozaicCanvas OriginalCanvas { get; set; }

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
        public OperationViewModel OperationViewModel { get; set; }

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
            LoadCommand = new LoadCommand(this);
            SaveCommand = new SaveCommand(this);
            RenderCommand.ExecuteCompleted += RenderCommandCompleted;
            KeepRatio = true;
        }

        private void RenderCommandCompleted(object sender, EventArgs e)
        {
            OperationViewModel.IsMozaicDisplayed = true;
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
            _isAdjustingRatio = true;

            Height =  GetRatio(size => ((double)size.Height / size.Width), Height, Width);

            _isAdjustingRatio = false;
        }

        private void AdjustWidth()
        {
            _isAdjustingRatio = true;

            Width = GetRatio(size => (double)size.Width / size.Height, Width, Height);

            _isAdjustingRatio = false;
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
