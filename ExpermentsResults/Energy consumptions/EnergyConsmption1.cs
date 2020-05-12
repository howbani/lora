using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LORA.Modules;
using LORA.Parameters;
using LORA.Properties;
using LORA.Computations;
using LORA.Forwarding;
using LORA.Properties;

namespace LORA.ExpermentsResults
{
    public class EnergyConsmption1
    {
        public int Distance { get; set; }
        public int Number_of_Packets_tobe_Sent { get; set; }
        public double EnergyConsumption { get; set; }
        public double SumRoutingEfficiency { get; set; }
        public double AvaerageRoutingEfficiency { get { return SumRoutingEfficiency / Number_of_RecievedPackets; } }
        public int Number_of_RecievedPackets { get; set; }
        public List<Sensor> Nodes = new List<Sensor>();
    }

    public class EnergyConsmption2 
    {
        public double Distance { get; set; }
        public int  NOS { get; set; }
        public int NOP { get; set; }
        public double EnergyConsumption { get; set; }
        public double SumRoutingEfficiency { get; set; }
        public double AvaerageRoutingEfficiency { get { return SumRoutingEfficiency / Number_of_RecievedPackets; } }
        public int Number_of_RecievedPackets { get; set; }
        public List<Sensor> Nodes = new List<Sensor>();
    }


    public class DoEnergyConsumptionExpermentRandomWithDistance
    {
        private List<Sensor> NET;
        public DoEnergyConsumptionExpermentRandomWithDistance(List<Sensor> Network)
        {
            NET = Network;
        }

        /// <summary>
        /// SLEECT THE NODES WITHIN A DISTANCE
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        private List<Sensor> SelectNodesWithinDistance(double distance)
        {
            List<Sensor> NodesWidDistance = new List<Sensor>();
            if (distance > (PublicParamerters.SensingRangeRadius * 1.2))
            {
                double maxDi = distance + (PublicParamerters.SensingRangeRadius / 2);
                double MinDi = distance - (PublicParamerters.SensingRangeRadius / 2);

                foreach(Sensor s in NET)
                {
                    double senDis = Operations.DistanceBetweenTwoSensors(PublicParamerters.SinkNode, s);
                    if (senDis >= MinDi && senDis <= maxDi)
                    {
                        NodesWidDistance.Add(s);
                    }
                }
                return NodesWidDistance;
            }
            else
            {
                
                return null;
            }
        }

        /// <summary>
        /// perform the experment.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="NOS"></param>
        /// <param name="NOP"></param>
        public EnergyConsmption2 DoExperment(double distance, int NOS, int NOP)
        {
           
            List<Sensor> SelectNodesWithinDistanceN = SelectNodesWithinDistance(distance);
            if (SelectNodesWithinDistanceN != null)
            {
                // selecte The Nodes:
                List<Sensor> SelectedSn = new List<Modules.Sensor>(NOS);
                for (int i = 0; i < NOS; i++)
                {
                    int ran = Convert.ToInt16(UnformRandomNumberGenerator.GetUniform(SelectNodesWithinDistanceN.Count - 1));
                    if (ran > 0)
                    {
                        SelectedSn.Add(SelectNodesWithinDistanceN[ran]);
                    }
                    else
                    {
                        SelectedSn.Add(SelectNodesWithinDistanceN[1]);
                    }
                }




                // each packet sendt NOP:
                for (int i = 0; i < NOP; i++)
                {
                    foreach (Sensor sen in SelectedSn)
                    {
                        sen.GeneratePacketAndSent();
                      //  sen.GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                    }
                }


                EnergyConsmption2 re = new ExpermentsResults.EnergyConsmption2();
                re.Distance = distance;
                re.NOP = NOP;
                re.Nodes = SelectedSn;
                re.NOS = NOS;
                re.Number_of_RecievedPackets = PublicParamerters.SinkNode.PacketsList.Count;

                foreach (DataPacket.Datapacket pa in PublicParamerters.SinkNode.PacketsList)
                {
                    re.EnergyConsumption += pa.UsedEnergy_Joule;
                    re.SumRoutingEfficiency += pa.RoutingEfficiency;
                }

                PublicParamerters.SinkNode.PacketsList.Clear();// clear.

                return re;
            }
            else
            {
                System.Windows.MessageBox.Show("Distance is too short it should be 2*sensang range");
                return null;
            }
        }
    }



    public class DoEnergyConsmption1Experment
    {
        private int NumPackets = 0;
        private string d50Str = "50-10,592,575,46,236";
        private string d100Str = "100-91,511,151,538,530";
        private string d150Str = "150-28,418,99,310,115";
        private string d200Str = "200-369,505,422,591,326";
        private string d250Str = "250-222,569,395,344,588";
        List<string> Distances = new List<string>();
        List<EnergyConsmption1> Results = new List<EnergyConsmption1>();
        private List<Sensor> NETWORK;
        public DoEnergyConsmption1Experment(List<Sensor> nET, int _NumPackets)
        {
            NETWORK = nET;
            NumPackets = _NumPackets;
            Distances.Add(d50Str);
            Distances.Add(d100Str);
            Distances.Add(d150Str);
            Distances.Add(d200Str);
            Distances.Add(d250Str);


            foreach (string distance in Distances)
            {
                EnergyConsmption1 x = new EnergyConsmption1();
                x.Number_of_Packets_tobe_Sent = _NumPackets;
                x.Distance = Convert.ToInt32(distance.Split('-')[0]); // distance.
                string nodesStr = distance.Split('-')[1];
                string[] nodes = nodesStr.Split(',');
                foreach (string node in nodes)
                {
                    if (node.ToString() != "")
                    {
                        int id = Convert.ToInt16(node);
                        x.Nodes.Add(NETWORK[id]);
                    }
                }
                Results.Add(x);
            }

        }


        private void SumEffeciancyAndConsumption(EnergyConsmption1 exp)
        {

            foreach (LORA.DataPacket.Datapacket pa in PublicParamerters.SinkNode.PacketsList)
            {
                exp.EnergyConsumption += pa.UsedEnergy_Joule;
                exp.SumRoutingEfficiency += pa.RoutingEfficiency;
            }

        }


        public List<EnergyConsmption1> Perform()
        {
            foreach (EnergyConsmption1 exp in Results)
            {
                for (int i = 1; i <= NumPackets; i++)
                {
                    foreach (Sensor node in exp.Nodes)
                    {
                        node.GeneratePacketAndSent();
                      //  node.GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                    }
                }
                // collect:
                exp.Number_of_RecievedPackets = PublicParamerters.SinkNode.PacketsList.Count;
                SumEffeciancyAndConsumption(exp);
                PublicParamerters.SinkNode.PacketsList.Clear();// clear.
            }
            return Results;
        }
    }
}
