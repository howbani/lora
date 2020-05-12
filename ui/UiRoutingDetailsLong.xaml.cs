using LORA.Logs;
using LORA.Modules;
using System;
using System.Collections.Generic;
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

namespace LORA.ui
{
    /// <summary>
    /// Interaction logic for UiRoutingDetailsLong.xaml
    /// </summary>
    public partial class UiRoutingDetailsLong : Window
    {
        public UiRoutingDetailsLong( List<Sensor> Network)
        {
            InitializeComponent();

            List<SensorRoutingLog> Logs = new List<SensorRoutingLog>(); 
            foreach(Sensor sen in Network)
            {
                Logs.AddRange(sen.Logs);
            }

            dg_routingLogs.ItemsSource = Logs;

           
        }
    }
}
