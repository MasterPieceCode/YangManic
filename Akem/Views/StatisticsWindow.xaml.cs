﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Akem.VM;

namespace Akem.Views
{
    /// <summary>
    /// Interaction logic for StatisticsWindow.xaml
    /// </summary>
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow(IReadOnlyCollection<MozaicStatistic> mozaicStatisitics)
        {
            InitializeComponent();
            MozaicStatisticsList.DataContext = mozaicStatisitics;
            CountText.DataContext = mozaicStatisitics.Sum(x => x.Count);
        }
    }
}
