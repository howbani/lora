using LORA.DataPacket;
using LORA.Logs;
using LORA.Modules;
using LORA.Parameters;
using LORA.ui;
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

namespace LORA.ExpermentsResults.Energy_consumptions
{
   

    public class ValParPair
    {
        public string Par { get; set; }
        public string Val { get; set; }
    }

    /// <summary>
    /// Interaction logic for ExpReport.xaml
    /// </summary>
    public partial class ExpReport : Window
    {


        public ExpReport(MainWindow _mianWind)
        {
            InitializeComponent();

            List<ValParPair> List = new List<ValParPair>();

            List.Add(new ValParPair() {Par="Number of Nodes", Val= _mianWind.myNetWork.Count.ToString() } );
            List.Add(new ValParPair() { Par = "Communication Range Radius", Val = PublicParamerters.CommunicationRangeRadius.ToString()+" m"});
            List.Add(new ValParPair() { Par = "Density", Val = PublicParamerters.Density.ToString()});
            List.Add(new ValParPair() { Par = "Packet Rate", Val = _mianWind.PacketRate });
            List.Add(new ValParPair() { Par = "Simulation Time", Val = _mianWind.stopSimlationWhen.ToString()+" s" });
            List.Add(new ValParPair() { Par = "Start up time", Val = PublicParamerters.MacStartUp.ToString() + " s" });
            List.Add(new ValParPair() { Par = "Active Time", Val = PublicParamerters.Periods.ActivePeriod.ToString() + " s" });
            List.Add(new ValParPair() { Par = "Sleep Time", Val = PublicParamerters.Periods.SleepPeriod.ToString() + " s" });
            List.Add(new ValParPair() { Par = "Total Energy Consumption", Val = PublicParamerters.TotalEnergyConsumptionJoule.ToString() });
            List.Add(new ValParPair() { Par = "Wasted Energy (%) ", Val = PublicParamerters.WastedEnergyPercentage.ToString() });
            List.Add(new ValParPair() { Par = "Average Hops/path", Val = PublicParamerters.AverageTotalNumberOfHope.ToString() });
            List.Add(new ValParPair() { Par = "Average Redundant Transmissions/path", Val = PublicParamerters.AverageRedundantTransmissions.ToString() });
            List.Add(new ValParPair() { Par = "Average Routing Distanc eff/path (%)", Val = PublicParamerters.AverageRoutingDistanceEffeciency.ToString() });
            List.Add(new ValParPair() { Par = "Average Transmission Distance (%)", Val = PublicParamerters.AverageTransmissionDistanceEff.ToString() });
            List.Add(new ValParPair() { Par = "Average Waiting Time/path", Val = PublicParamerters.AverageWaitingTime.ToString() });
            List.Add(new ValParPair() { Par = "# gen pck", Val = PublicParamerters.NumberofGeneratedPackets.ToString() });
            List.Add(new ValParPair() { Par = "# del pck", Val = PublicParamerters.NumberofDeliveredPacket.ToString() });
            List.Add(new ValParPair() { Par = "# in Queue pck", Val = PublicParamerters.InQueuePackets.ToString() });
            List.Add(new ValParPair() { Par = "# droped pck", Val = PublicParamerters.NumberofDropedPacket.ToString() });
            List.Add(new ValParPair() { Par = "Success Rate %", Val = PublicParamerters.SucessRatio.ToString() });
            List.Add(new ValParPair() { Par = "Droped Rate %", Val = PublicParamerters.DropedRatio.ToString() });


            List.Add(new ValParPair() { Par = "En Par", Val = Properties.Settings.Default.EnergyDistCnt .ToString() });
            List.Add(new ValParPair() { Par = "Dirc Par", Val = Properties.Settings.Default.DirectionDistCnt.ToString() });
            List.Add(new ValParPair() { Par = "Trs Dis Par", Val = Properties.Settings.Default.TransDistanceDistCnt.ToString() });
            List.Add(new ValParPair() { Par = "Pre Dis Par of Packets", Val = Properties.Settings.Default.PrepDistanceDistCnt.ToString() });
           
            List.Add(new ValParPair() { Par = "Update Energy", Val = PublicParamerters.BatteryLosePerUpdate.ToString() });
            List.Add(new ValParPair() { Par = "Protocol", Val = "LORA" });
            dg_data.ItemsSource = List;
        }
    }
}
