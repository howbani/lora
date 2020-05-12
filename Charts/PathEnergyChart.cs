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
   public class PathEnergyChart
    { 
        public static List<List<KeyValuePair<int, double>>> BuildChartPackets()
        {
            List<List<KeyValuePair<int, double>>> overAll = new List<List<KeyValuePair<int, double>>>();
            Sensor sink = PublicParamerters.SinkNode;
            if (sink != null)
            {
                List<KeyValuePair<int, double>> ListHops = new List<KeyValuePair<int, double>>();  
                List<KeyValuePair<int, double>> ListEnergy = new List<KeyValuePair<int, double>>(); 

                List<UnVisualizedDataPacket> recivedpackets = new List<DataPacket.UnVisualizedDataPacket>();
                int x = 0;
                foreach (Datapacket pck in sink.PacketsList)
                {

                    ListHops.Add(new KeyValuePair<int, double>(x, pck.Hops));
                   // ListEnergy.Add(new KeyValuePair<int, double>(x, pck.UsedEnergy00001));
                    x++;
                }

                overAll.Add(ListHops);// 0;
                overAll.Add(ListEnergy);// 1;
            }

            return overAll;
        }

    }
}

