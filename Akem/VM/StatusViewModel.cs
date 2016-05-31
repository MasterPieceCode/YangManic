using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Akem.Annotations;

namespace Akem.VM
{
    public class StatusViewModel : INotifyPropertyChanged
    {
        public StatusViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Maximum = 100;
        }

        public Dispatcher Dispatcher { get; set; }

        private int _percent;
        private string _status;
        private int _minimum;
        private int _maximum;
        private TaskScheduler _scheduler;

        public string Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged(); }
        }

        public int Percent
        {
            get { return _percent; }
        }

        public int Minimum
        {
            get { return _minimum; }
            set { _minimum = value; OnPropertyChanged(); }
        }

        public int Maximum
        {
            get { return _maximum; }
            set { _maximum = value; OnPropertyChanged(); }
        }

        public void ReportProgress()
        {
            _percent = Interlocked.Increment(ref _percent);

            if (_percent == _maximum)
            {
                _percent = 0;
            }

            OnPropertyChanged("Percent");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
                //Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() => handler(this, new PropertyChangedEventArgs(propertyName))));                
            }
        }
    }
}
