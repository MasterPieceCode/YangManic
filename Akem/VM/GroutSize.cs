using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Akem.VM
{
    public class GroutSize : INotifyPropertyChanged
    {
        private Color _color;

        public GroutSize(Color color, double thiknessInPhysicalUnit)
            : this(color, thiknessInPhysicalUnit, string.Format("{0} mm", thiknessInPhysicalUnit))
        {
        }

        public GroutSize(Color color, double thiknessInPhysicalUnit, string name)
        {
            Color = color;
            Name = name;
            Thikness = thiknessInPhysicalUnit;
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Color"));
                }
            }
        }

        public double Thikness { get; set; }
        public string Name { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged  = delegate {};
    }
}
