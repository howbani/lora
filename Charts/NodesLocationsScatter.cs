using LORA.Modules;
using LORA.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LORA.Charts
{
    public class NodesLocationsScatter
    {
        public static List<KeyValuePair<double, double>> BuildNodesLocationsScatterNodeChart(List<Sensor> Network)
        {
            List<KeyValuePair<double, double>> ListValues = new List<KeyValuePair<double, double>>();

            Sensor sink = PublicParamerters.SinkNode;
            if (sink != null)
            {

                foreach (Sensor sen in Network)
                {
                    KeyValuePair<double, double> row = new KeyValuePair<double, double>(sen.CenterLocation.X,sen.CenterLocation.Y);
                    ListValues.Add(row);
                }
            }
            return ListValues;
        }

        public static List<KeyValuePair<int, int>> GetNieborsDist(List<Sensor> Network)
        {
            List<KeyValuePair<int, int>> ListValues = new List<KeyValuePair<int, int>>();
            Sensor sink = PublicParamerters.SinkNode;
            if (sink != null)
            {
                foreach (Sensor sen in Network)
                {
                    KeyValuePair<int, int> row = new KeyValuePair<int, int>(sen.ID, sen.NeighboreNodes.Count);
                    ListValues.Add(row);
                }
            }
            return ListValues;
        }
    }
}
