using LORA.DataPacket;
using LORA.Modules;
using LORA.Parameters;
using System.Collections.Generic;
using System.Windows;

namespace LORA.ui
{
    /// <summary>
    /// Interaction logic for UiRecievedPackertsBySink.xaml
    /// </summary>
    public partial class UiRecievedPackertsBySink : Window
    {
        public UiRecievedPackertsBySink()
        {
            InitializeComponent();
            Sensor sink= PublicParamerters.SinkNode;
            if (sink != null)
            {
                List<UnVisualizedDataPacket> recivedpackets = new List<DataPacket.UnVisualizedDataPacket>();
                foreach (Datapacket pck in sink.PacketsList)
                {
                    UnVisualizedDataPacket pp = new UnVisualizedDataPacket()
                    {
                        Distance = pck.DistanceFromSourceToSink,
                        Path = pck.Path,
                        RoutingDistance = pck.RoutingDistance,
                        SID = pck.SourceNodeID,
                        Hops = pck.Hops,
                        UsedEnergy_Joule = pck.UsedEnergy_Joule,
                        Delay = pck.Delay,
                        PrepDistanceDistCnt = pck.PrepDistanceDistCnt,
                        TransDistanceDistCnt = pck.TransDistanceDistCnt,
                        EnergyDistCnt = pck.EnergyDistCnt,
                        DirectionDistCnt = pck.DirectionDistCnt,
                        AverageTransDistrancePerHop = pck.AverageTransDistrancePerHop,
                        RoutingDistanceEffiecncy = pck.RoutingDistanceEfficiency,
                        RoutingEfficiency = pck.RoutingEfficiency,
                        TransDistanceEfficiency = pck.TransDistanceEfficiency,
                        RoutingZoneWidthCnt = pck.RoutingZoneWidthCnt,
                        RoutingProbabilityForPath = pck.PathLinksQualityEstimator,
                        PID = pck.PacektSequentID,
                        IsDelivered= pck.IsDelivered,
                        MaxHops= pck.MaxHops,
                        PacketWaitingTimes= pck.PacketWaitingTimes
                    };
                    recivedpackets.Add(pp);
                }

                
               

                dg_packets.ItemsSource = recivedpackets;
            }
        }
    }
}
