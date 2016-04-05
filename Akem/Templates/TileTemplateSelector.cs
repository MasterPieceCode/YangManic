using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Processing;

namespace Akem.Templates
{
    public class TileTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BitmapTemplate { get; set; }
        public DataTemplate ColorTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var palleteTile = (PaletteTile) item;

            if (palleteTile.Bitmap != null)
            {
                return BitmapTemplate;
            }
            return ColorTemplate;
        }
    }
}
