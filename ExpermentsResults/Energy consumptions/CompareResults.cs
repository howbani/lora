using LORA.Computations;
using LORA.DataPacket;
using LORA.Modules;
using LORA.Parameters;
using System.Collections.Generic;

namespace LORA.ExpermentsResults 
{

    public class CompareResult
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

    public class CompareResultsClass 
    {

        /// <summary>
        /// total energy and delay for an expement: for a path.
        /// this is true only for one source node. don't send the data from many sources.
        /// </summary>
        /// <returns></returns>
        public static CompareResult GetEnergyDelayForAnExperment()
        {
            CompareResult re = new CompareResult();

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
    }
}
