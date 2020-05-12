using LORA.DataPacket;
using LORA.Modules;
using LORA.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LORA.Charts
{
   public class PathEfficiencyChart
    {
        /// <summary>
        /// y: the Efficiency: 1-100%.
        /// x: is the index of the node. that is when the ith index node is the source.
        /// this to evalute when the ith node sended a packet, we see the qulity of the path.
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<List<KeyValuePair<int, double>>> BuildPathEfficiencyAndDelayChart() 
        {
            List<List<KeyValuePair<int, double>>> overAll = new List<List<KeyValuePair<int, double>>>();
            Sensor sink = PublicParamerters.SinkNode;
            if (sink != null)
            {
                List<KeyValuePair<int, double>> ListValuesPathEfficiency = new List<KeyValuePair<int, double>>();
                List<KeyValuePair<int, double>> ListValuesDelay = new List<KeyValuePair<int, double>>(); 

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
                        Delay= pck.Delay

                    };

                    KeyValuePair<int, double> rowPath = new KeyValuePair<int, double>(pp.SID, pp.RoutingDistanceEffiecncy);
                    ListValuesPathEfficiency.Add(rowPath);

                    KeyValuePair<int, double> rowDelay = new KeyValuePair<int, double>(pp.SID, pp.Delay*1000); // ms.
                    ListValuesDelay.Add(rowDelay);

                }

                overAll.Add(ListValuesPathEfficiency);// 0;
                overAll.Add(ListValuesDelay);// 1;
            }

            return overAll;
        }

    }
}
