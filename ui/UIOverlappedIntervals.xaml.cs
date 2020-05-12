using LORA.MAC;
using LORA.Modules;
using LORA.ui.conts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static LORA.Parameters.PublicParamerters;

namespace LORA.ui
{
    /// <summary>
    /// Interaction logic for UIOverlappedIntervals.xaml
    /// </summary>
    public partial class UIOverlappedIntervals : Window
    {
       

        

        public UIOverlappedIntervals(Sensor Node)
        {
            InitializeComponent();

            stack_intervals.MaxHeight = SystemParameters.FullPrimaryScreenHeight;
            stack_intervals.MaxWidth = SystemParameters.FullPrimaryScreenWidth;
            stack_intervals.Children.Add(new IntervalControl(Node));
            List<Sensor> Nn = Node.NeighboreNodes;
            foreach (Sensor n in Nn)
            {
                stack_intervals.Children.Add(new IntervalControl(n));
            }

            int ax = Convert.ToInt16((Periods.ActivePeriod + Periods.SleepPeriod) / 2);
            stack_intervals.Children.Add(new IntervalControl(ax, Node.DutyCycleString.Count));



        }
    }
}
