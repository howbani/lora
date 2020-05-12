using LORA.Forwarding;
using LORA.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LORA.ui
{
    public class SensoreBasicsDetails
    {
        public string NodeID { get; set; }
        public string RealLocation { get; set; } // real location
        public string CenterLocation { get; set; } // center location
        public string NeighborsNodes { get; set; }
        public int NeighborsCount { get; set; }
        public string ForwardersStrings { get; set; }
        public int ForwardersCount { get; set; }
    }

    public partial class UIshowSensorsLocations : Window
    {

        private string FindOverlappingNodesString(Sensor node)
        {
            string str = "";
            if (node.NeighboreNodes != null)
            {
                foreach (Sensor _node in node.NeighboreNodes)
                {
                    str += _node.ID.ToString() + ",";
                }
            }
            return str;
        }

        private string FindForwarders(Sensor node)
        {
            string str = "";
            if (node.MyForwardersShortedList != null)
            {
                foreach (RoutingMetric _node in node.MyForwardersShortedList)
                {
                    str += _node.PotentialForwarder.ID.ToString() + ",";
                }
            }
            return str;
        }


        public UIshowSensorsLocations(List<Sensor> SensorsNodes)
        {
            InitializeComponent();

            List<SensoreBasicsDetails> NodesLocationsList = new List<SensoreBasicsDetails>();
            foreach (Sensor node in SensorsNodes)
            {

                if (node.MyForwardersShortedList != null && node.NeighboreNodes != null)
                {
                    SensoreBasicsDetails Sensorinfo = new SensoreBasicsDetails();
                    Sensorinfo.NodeID = node.lbl_Sensing_ID.Content.ToString();
                    Sensorinfo.RealLocation = node.Position.X + "," + node.Position.Y;
                    Sensorinfo.CenterLocation = (node.Position.X + node.ComunicationRangeRadius) + "," + (node.Position.Y + node.ComunicationRangeRadius);
                    Sensorinfo.NeighborsNodes = FindOverlappingNodesString(node);
                    Sensorinfo.ForwardersStrings = FindForwarders(node);
                    Sensorinfo.NeighborsCount = node.NeighboreNodes.Count;
                    Sensorinfo.ForwardersCount = node.MyForwardersShortedList.Count;
                    NodesLocationsList.Add(Sensorinfo);
                }
            }
            dg_locations.ItemsSource = NodesLocationsList;
        }
    }
}

