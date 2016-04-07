using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Akem.Views;
using Akem.VM;

namespace Akem.Commands
{
    public class ShowStatisticsCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var mozaicStatisitics = (ObservableCollection<MozaicStatistic>) parameter;

            var statWindow = new StatisticsWindow(mozaicStatisitics.OrderByDescending(s => s.Count).ToList());
            statWindow.Show();
        }

        public event EventHandler CanExecuteChanged;
    }
}
