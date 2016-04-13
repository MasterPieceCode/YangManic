using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Akem.Annotations;

namespace Akem.VM
{
   
    public class OperationViewModel : INotifyPropertyChanged
    {
        public OperationViewModel()
        {
            _isOriginalImageDisplayed = true;
        }

        private bool _isOriginalImageDisplayed;

        public bool IsOriginalImageDisplayed
        {
            get { return _isOriginalImageDisplayed; }
            set
            {
                _isOriginalImageDisplayed = value;
                OnPropertyChanged();
                OnPropertyChanged("IsMozaicDisplayed");
            }
        }

        public bool IsMozaicDisplayed
        {
            get { return !_isOriginalImageDisplayed; }
            set { IsOriginalImageDisplayed = !value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
