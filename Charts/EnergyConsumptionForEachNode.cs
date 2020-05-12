using LORA.Logs;
using LORA.Modules;
using LORA.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LORA.Charts
{
    public class EnergyConsumptionForEachNode
    {
        /// <summary>
        /// y: the energy in UsedEnergy_Joule.
        /// x: is the node.
        /// to see how much energy is consumed.
        /// </summary>
        /// <param name="Network"></param>
        /// <returns></returns>
        public static List<KeyValuePair<int, double>> BuildEnergyConsumptionForEachNodeChart(List<Sensor> Network)
        {
            List<KeyValuePair<int, double>> ListValues = new List<KeyValuePair<int, double>>();

            Sensor sink = PublicParamerters.SinkNode;
            if (sink != null)
            {

                foreach (Sensor sen in Network)
                {
                    if (sen.ID != sink.ID)
                    {
                        double en = 0;
                        foreach (SensorRoutingLog log in sen.Logs)
                        {
                            en += log.UsedEnergy_Joule;
                        }
                        KeyValuePair<int, double> row = new KeyValuePair<int, double>(sen.ID,en);
                        en = 0;
                        ListValues.Add(row);
                    }
                }
            }

            return ListValues;
        }
    }
}
