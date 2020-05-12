using LORA.Computations;
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


    public class ExpermentResults
    {
        // patch:1 hops 
        public double NumberOfPackets { get; set; }
        public double EnergyConsumtion { get; set; } // total energy for the path.
        public double Delay { get; set; } // total deley, for the path.
        public int Hops { get; set; } // how many hops in the path.
        public double RoutingDistance { get; set; } // how many hops in the path. 

        // path 2: the energy effices
        public double AverageTransDistrancePerHop { get; set; } // average distance per hop.  
        public double TransDistanceEfficiency { get; set; } // TransDistanceEfficiency  for apath. for hop.
        public double RoutingDistanceEfficiency { get; set; } // average distance per hop. 
        public double RoutingEfficiency { get; set; } // RoutingEfficiency

        // pacth 3: balancing:
        public double RelaysCount { get; set; }
        public double HopsPerRelay { get; set; }
        public double PathsSpread { get; set; }

        

    }

    public class KeyValue
    {
        public int Hops { get; set; } // how many hops in the path.
        public double HopsCount { get; set; } // how many path has the same hopes.
        public double EnergyConsumtion { get; set; } // total energy for the path.
        public double Delay { get; set; } // total deley, for the path.

        public double AverageEnergyConsumtion { get { return EnergyConsumtion / HopsCount; } } // energy average for the paths with same count 
        public double AverageDelay { get { return Delay / HopsCount; } } 

    }



    public class Distrubtions
    {
        /// <summary>
        /// for three: hops, energy and delay.
        /// </summary>
        /// <returns></returns>
        public static List<List<KeyValuePair<int, double>>> FindDistubtions()
        {
            List<KeyValue> Values = new List<KeyValue>(); // 0 hops 
            Sensor sink = PublicParamerters.SinkNode;
            if (sink != null)
            {
                foreach (Datapacket pck in sink.PacketsList)
                {

                    // hops:
                    int keyh = pck.Hops;
                    KeyValue oldRowHops = isFound(keyh, Values);
                    if (oldRowHops.Hops == -1) // not found
                    {
                        //: hops
                        KeyValue newRowHops = new KeyValue() { Hops = keyh, HopsCount = 1, EnergyConsumtion = pck.UsedEnergy_Joule, Delay = pck.Delay };
                        Values.Add(newRowHops);
                    }
                    else // then add.
                    {
                        oldRowHops.HopsCount += 1;
                        oldRowHops.EnergyConsumtion += pck.UsedEnergy_Joule;
                        oldRowHops.Delay += pck.Delay;
                    }
                }
            }

            List<List<KeyValuePair<int, double>>> re = new List<List<KeyValuePair<int, double>>>();
            List<KeyValuePair<int, double>> hopsList = new List<KeyValuePair<int, double>>();
            List<KeyValuePair<int, double>> energList = new List<KeyValuePair<int, double>>();
            List<KeyValuePair<int, double>> delayList = new List<KeyValuePair<int, double>>(); 

            foreach(KeyValue val in Values)
            {
                hopsList.Add(new KeyValuePair<int, double>(val.Hops,val.HopsCount));
                energList.Add(new KeyValuePair<int, double>(val.Hops, val.AverageEnergyConsumtion));
                delayList.Add(new KeyValuePair<int, double>(val.Hops,val.AverageDelay));
            }

            re.Add(hopsList); //0 
            re.Add(energList); //
            re.Add(delayList);


            return re;
        }
        
        /// <summary>
        /// for hops only.
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<int, double>> FindDistrubtionsForHops()
        {
            List<KeyValue> Values = new List<KeyValue>(); // 0 hops 
            Sensor sink = PublicParamerters.SinkNode;
            if (sink != null)
            {
                foreach (Datapacket pck in sink.PacketsList)
                {

                    // hops:
                    int keyh = pck.Hops;
                    KeyValue oldRowHops = isFound(keyh, Values);
                    if (oldRowHops.Hops == -1) // not found
                    {
                        //: hops
                        KeyValue newRowHops = new KeyValue() { Hops = keyh, HopsCount = 1, EnergyConsumtion = pck.UsedEnergy_Joule, Delay = pck.Delay };
                        Values.Add(newRowHops);
                    }
                    else // then add.
                    {
                        oldRowHops.HopsCount += 1;
                        oldRowHops.EnergyConsumtion += pck.UsedEnergy_Joule;
                        oldRowHops.Delay += pck.Delay;
                    }
                }
            }

            List<KeyValuePair<int, double>> hopsList = new List<KeyValuePair<int, double>>();

            foreach (KeyValue val in Values)
            {
                hopsList.Add(new KeyValuePair<int, double>(val.Hops, val.HopsCount));
            }

            return hopsList;
        }

        /// <summary>
        /// total energy and delay for an expement: for a path.
        /// this is true only for one source node. don't send the data from many sources.
        /// </summary>
        /// <returns></returns>
        public static ExpermentResults GetEnergyDelayForAnExperment()
        {
            ExpermentResults re = new ExpermentResults();

            SegmaManager sgManager = new SegmaManager();
            List<string> Paths = new List<string>();
            Sensor sink = PublicParamerters.SinkNode;

            if (sink != null)
            {
                foreach (Datapacket pck in sink.PacketsList)
                {
                    re.Delay += pck.Delay;
                    re.EnergyConsumtion += pck.UsedEnergy_Joule;
                    re.Hops += pck.Hops;
                    re.RoutingDistance += pck.RoutingDistance;
                    re.AverageTransDistrancePerHop += pck.AverageTransDistrancePerHop;
                    re.RoutingDistanceEfficiency += pck.RoutingDistanceEfficiency;
                    re.RoutingEfficiency += pck.RoutingEfficiency;
                    re.TransDistanceEfficiency += pck.TransDistanceEfficiency;
                    Paths.Add(pck.Path);
                }

                sgManager.Filter(Paths);
                SegmaCollection collectionx = sgManager.GetCollection;
                SegmaSource sigmasourc = collectionx.GetSourcesList[0];
                sigmasourc.NumberofPacketsGeneratedByMe = sink.PacketsList.Count;

                // add:
                re.RelaysCount = sigmasourc.RelaysCount;
                re.NumberOfPackets = sink.PacketsList.Count;
                re.HopsPerRelay = sigmasourc.Mean;
                re.PathsSpread = sigmasourc.PathsSpread;
            }

            return re;
        }

        public static KeyValue isFound(int hops, List<KeyValue> ListValues)
        {
            foreach (KeyValue r in ListValues)
            {
                if (r.Hops == hops) { return r; } // found.
            }
            KeyValue xx = new KeyValue() { Hops=-1, HopsCount=-1 };
            return xx;
        }


    }
}
