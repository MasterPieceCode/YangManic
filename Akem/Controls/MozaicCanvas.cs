using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Akem.Controls
{
    public class MozaicCanvas : Canvas
    {
        public MozaicCanvas()
        {
            Visuals = new List<Visual>();
        }

        protected override int VisualChildrenCount
        {
            get { return Visuals.Count; }
        }

        protected List<Visual> Visuals { get; private set; }

        // Provide a required override for the VisualChildrenCount property. 

        // Provide a required override for the GetVisualChild method. 
        protected override Visual GetVisualChild(int index)
        {
            if (Visuals == null)
            {
                throw new ArgumentOutOfRangeException();
            }

            return Visuals[index];
        }

        public Visual GetVisual(int index)
        {
            return Visuals[index];
        }

        public void AddVisual(Visual visual)
        {
            Visuals.Add(visual);
            AddVisualChild(visual);
            AddLogicalChild(visual);
        }

        public void DeleteVisual(Visual visual)
        {
            Visuals.Remove(visual);
            RemoveVisualChild(visual);
            RemoveLogicalChild(visual);
        }

        public void Clear()
        {
            Visuals.ToList().ForEach(DeleteVisual);
        }
    }
}
