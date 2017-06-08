using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processing
{
    public class MozaicResult
    {
        public Dictionary<string, int> MozaicStatisitcs { get; private set; }
        public ObservableCollection<ObservableCollection<PaletteTile>> Tiles { get; private set; }

        public MozaicResult()
        {
            Tiles = new ObservableCollection<ObservableCollection<PaletteTile>>();
            MozaicStatisitcs = new Dictionary<string, int>();
        }
    }
}
