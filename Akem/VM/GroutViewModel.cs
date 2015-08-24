using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Akem.VM
{
    public class GroutViewModel
    {
        public GroutSize SelectedGrout { get; set; }

        public Color SelectedColor
        {
            get { return SelectedGrout.Color; }
            set
            {
                if (SelectedGrout != GroutSizes.First())
                {
                    GroutSizes.Skip(1).ToList().ForEach(gs => gs.Color = value);
                }
            }
        }

        public List<GroutSize> GroutSizes { get; private set; }

        public GroutViewModel()
        {
            GroutSizes = new List<GroutSize>
            {
                new GroutSize(Colors.Transparent, 0, "None"),
                new GroutSize(Colors.Gray, 1),
                new GroutSize(Colors.Gray, 2),
                new GroutSize(Colors.Gray, 3),
                new GroutSize(Colors.Gray, 4),
                new GroutSize(Colors.Gray, 5)
            };

            SelectedGrout = GroutSizes.First();
        }
    }
}
