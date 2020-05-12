using LORA.Computations;
using LORA.Modules;
using LORA.Parameters;
using LORA.Properties;
using System;
using System.Collections.Generic;
using System.Windows;

namespace LORA.Forwarding
{

    public class ForwardersSelection
    {
        public class LinkQualitySorter : IComparer<RoutingMetric>
        {

            public int Compare(RoutingMetric y, RoutingMetric x)
            {
                return x.LinkEstimation.CompareTo(y.LinkEstimation);
            }
        }

        public class LinkPrioritySorter : IComparer<RoutingMetric> 
        {

            public int Compare(RoutingMetric y, RoutingMetric x)
            {
                return x.LinkEstimationNormalized.CompareTo(y.LinkEstimationNormalized);
            }
        }

        public static int ex = 1;
        /// <summary>
        /// intialize all nodes. compute the link quality by SelectForwardersSetWithinMySubZone
        /// MyForwardersLongList= all nodes.
        /// MyForwardersShortedList= the nodes LinkEstimationNormalized > (1 / (Convert.ToDouble(MyForwardersLongList.Count)
        /// </summary>
        /// <param name="net"></param>
        public void IntializeForwarders(List<Sensor> net)
        {
            foreach (Sensor sen in net)
            {
                if (sen.ID != PublicParamerters.SinkNode.ID)
                {
                    List<RoutingMetric> longList = SelectForwardersSetWithinMySubZone(sen);
                    sen.MyForwardersLongList = longList;
                    // select the shorter list
                    foreach (RoutingMetric or in longList)
                    {
                        if (or.LinkEstimationNormalized > (1 / ((Convert.ToDouble(longList.Count)+ ex))))
                        {
                            sen.MyForwardersShortedList.Add(or);
                        }
                    }
                }
                sen.MyForwardersShortedList.Sort(new LinkQualitySorter());
            }
        }

        /// <summary>
        /// sort my forwarders according to the predis of source node.
        /// when the sender node recived the packet, the sender compute
        /// </summary>
        /// <param name="senderNode"></param>
        /// <param name="sourceNode"></param>
        public void UpdateMyForwardersAccordingtoPerpendicaureDistance(Sensor senderNode, Sensor sourceNode)
        {
            List<RoutingMetric> Metrics = senderNode.MyForwardersLongList;
            double PreDistanceSum = 0; // prependiculare distance
            foreach (RoutingMetric met in Metrics)
            {
                Sensor potentialForwarder = met.PotentialForwarder;
                double Dij = met.DistanceFromSenderToForwarder;
                double Djb = met.DistanceFromFrowarderToSink;
                double Dib = met.DistanceFromSenderToSink;
                met.SourceNode = sourceNode;
                PreDistanceSum += (1 / (Math.Exp(Math.Pow(met.NormalizePerpendicularDistance, Settings.Default.PrepDistanceDistCnt))));
            }
            //
            double LinkQEstiSum = 0;//  the sum of link quality estimations, for all forwarders.
            foreach (RoutingMetric forwarder in Metrics)
            {
                // propablity distrubution:
                forwarder.PerdPr = (1 / (Math.Exp(Math.Pow(forwarder.NormalizePerpendicularDistance, Settings.Default.PrepDistanceDistCnt)))) / PreDistanceSum;
                LinkQEstiSum += forwarder.LinkEstimation;
            }

            foreach (RoutingMetric Potentialforwarder in Metrics)
            {
                Potentialforwarder.LinkEstimationNormalized = Potentialforwarder.LinkEstimation / LinkQEstiSum;
            }

            // re-select the short list forwarders:
            senderNode.MyForwardersShortedList.Clear();
            // select the shorted List:
            foreach (RoutingMetric or in senderNode.MyForwardersLongList)
            {
                if (or.LinkEstimationNormalized > (1 / ((Convert.ToDouble(Metrics.Count) + ex))))
                {
                    senderNode.MyForwardersShortedList.Add(or);
                }
            }

            senderNode.MyForwardersShortedList.Sort(new LinkQualitySorter());
        }

        /// <summary>
        /// update the OR when the node levels is reduced 5%
        /// </summary>
        /// <param name="senderNode"></param>
        /// <param name="sourceNode"></param>
        public  void UpdateMyForwardersAccordingtoBattryPower(Sensor senderNode)
        {
            if (senderNode.ID != PublicParamerters.SinkNode.ID)
            {
                foreach (Sensor sen in senderNode.NeighboreNodes)
                {
                    if (sen.ID != PublicParamerters.SinkNode.ID)
                    {
                        sen.MyForwardersLongList.Clear();
                        sen.MyForwardersShortedList.Clear();
                        List<RoutingMetric> longList = SelectForwardersSetWithinMySubZone(sen);
                        sen.MyForwardersLongList = longList;
                        // select the shorter list
                        foreach (RoutingMetric or in longList)
                        {
                            if (or.LinkEstimationNormalized > (1 / ((Convert.ToDouble(longList.Count) + ex))))
                            {
                                sen.MyForwardersShortedList.Add(or);
                            }
                        }
                    }
                    sen.MyForwardersShortedList.Sort(new LinkQualitySorter());
                }
            }
        }

       /// <summary>
       /// a mong all neighbors.
       /// </summary>
       /// <param name="senderNode"></param>
       /// <returns></returns>
        private List<RoutingMetric> SelectForwardersSetWithinMyNeighbors(Sensor senderNode)
        {
            List<RoutingMetric> Forwarders = new List<RoutingMetric>();
            List<Sensor> N = senderNode.NeighboreNodes;
            if (N != null)
            {
                if (N.Count > 0)
                {
                    // sum:
                    double EnergySum = 0; // energy
                    double DirectionSum = 0; // direction
                    double PreDistanceSum = 0; // prependiculare distance
                    double TransDistanceSum = 0; // transmission distance
                    foreach (Sensor potentialForwarder in N)
                    {
                        if (potentialForwarder.ResidualEnergy > 0)
                        {

                            if (PublicParamerters.SinkNode.ID == potentialForwarder.ID)
                            {
                                double Dij = Operations.DistanceBetweenTwoSensors(senderNode, potentialForwarder);// i and j
                                double Djb = Operations.DistanceBetweenTwoSensors(potentialForwarder, PublicParamerters.SinkNode); // j and b
                                double Dib = Operations.DistanceBetweenTwoSensors(senderNode, PublicParamerters.SinkNode); // i and b

                                RoutingMetric metRic = new RoutingMetric();
                                metRic.PID = PublicParamerters.NumberofGeneratedPackets;
                                metRic.SenderNode = senderNode;
                                metRic.SourceNode = senderNode;
                                metRic.PotentialForwarder = potentialForwarder;
                                metRic.DistanceFromSenderToForwarder = Dij;
                                metRic.DistanceFromSenderToSink = Dib;
                                metRic.DistanceFromFrowarderToSink = Djb;
                                metRic.r = senderNode.ComunicationRangeRadius;
                                metRic.ZoneWidthControl = Settings.Default.ZoneWidth;

                                // sum's:
                                EnergySum += Math.Exp(Math.Pow(metRic.NormalizedEnergy, Settings.Default.EnergyDistCnt));
                                TransDistanceSum += (1 / (Math.Exp(Math.Pow(metRic.NormalizedTransDistance, Settings.Default.TransDistanceDistCnt))));
                                DirectionSum += (1 / (Math.Exp(Math.Pow(metRic.NormalizedDirection, Settings.Default.DirectionDistCnt))));
                                PreDistanceSum += (1 / (Math.Exp(Math.Pow(metRic.NormalizePerpendicularDistance, Settings.Default.PrepDistanceDistCnt))));

                                Forwarders.Add(metRic); // keep for each node.
                            }
                            else
                            {
                                double Dij = Operations.DistanceBetweenTwoSensors(senderNode, potentialForwarder);// i and j
                                double Djb = Operations.DistanceBetweenTwoSensors(potentialForwarder, PublicParamerters.SinkNode); // j and b
                                double Dib = Operations.DistanceBetweenTwoSensors(senderNode, PublicParamerters.SinkNode); // i and b

                                RoutingMetric metRic = new RoutingMetric();
                                metRic.PID = PublicParamerters.NumberofGeneratedPackets;
                                metRic.SenderNode = senderNode;
                                metRic.SourceNode = senderNode;
                                metRic.PotentialForwarder = potentialForwarder;
                                metRic.DistanceFromSenderToForwarder = Dij;
                                metRic.DistanceFromSenderToSink = Dib;
                                metRic.DistanceFromFrowarderToSink = Djb;
                                metRic.r = senderNode.ComunicationRangeRadius;
                                metRic.ZoneWidthControl = Settings.Default.ZoneWidth;

                                // sum's:
                                EnergySum += Math.Exp(Math.Pow(metRic.NormalizedEnergy, Settings.Default.EnergyDistCnt));
                                TransDistanceSum += (1 / (Math.Exp(Math.Pow(metRic.NormalizedTransDistance, Settings.Default.TransDistanceDistCnt))));
                                DirectionSum += (1 / (Math.Exp(Math.Pow(metRic.NormalizedDirection, Settings.Default.DirectionDistCnt))));
                                PreDistanceSum += (1 / (Math.Exp(Math.Pow(metRic.NormalizePerpendicularDistance, Settings.Default.PrepDistanceDistCnt))));

                                Forwarders.Add(metRic); // keep for each node.
                            }

                        }
                    }

                    //
                    double LinkQEstiSum = 0;//  the sum of link quality estimations, for all forwarders.
                    int k = 0;
                    foreach (RoutingMetric forwarder in Forwarders)
                    {
                        // propablity distrubution:
                        forwarder.EnPr = (Math.Exp(Math.Pow(forwarder.NormalizedEnergy, Settings.Default.EnergyDistCnt))) / EnergySum;
                        forwarder.TdPr = (1 / (Math.Exp(Math.Pow(forwarder.NormalizedTransDistance, Settings.Default.TransDistanceDistCnt)))) / TransDistanceSum;
                        forwarder.DirPr = (1 / (Math.Exp(Math.Pow(forwarder.NormalizedDirection, Settings.Default.DirectionDistCnt)))) / DirectionSum; // propablity for lemda.
                        forwarder.PerdPr = (1 / (Math.Exp(Math.Pow(forwarder.NormalizePerpendicularDistance, Settings.Default.PrepDistanceDistCnt)))) / PreDistanceSum;
                        LinkQEstiSum += forwarder.LinkEstimation;
                        k++;
                    }

                    foreach (RoutingMetric Potentialforwarder in Forwarders)
                    {
                        Potentialforwarder.LinkEstimationNormalized = Potentialforwarder.LinkEstimation / LinkQEstiSum;

                    } // e
                }
            }

            Forwarders.Sort(new LinkPrioritySorter());// sort according to Priority.
            return Forwarders;
        }


        /// <summary>
        /// within my subzone ONLY
        /// the source and sender node are the same.
        /// </summary>
        /// <param name="senderNode"></param>
        /// <returns></returns>
        public List<RoutingMetric> SelectForwardersSetWithinMySubZone(Sensor senderNode)
        {

            // the routing zone.
            List<Point> FourCorners = senderNode.MyRoutingZone;
            Parallelogram Zone = new Computations.Parallelogram();
            if (FourCorners != null)
            {
                Zone.P1 = FourCorners[0];
                Zone.P2 = FourCorners[1];
                Zone.P3 = FourCorners[2];
                Zone.P4 = FourCorners[3];
            }

            Geomtric Geo = new Computations.Geomtric();
            List<RoutingMetric> Forwarders = new List<RoutingMetric>();
            if (PublicParamerters.SinkNode == null) { return null; }
            else
            if (PublicParamerters.SinkNode.ID != senderNode.ID) // for all node but sink node
            {
                List<Sensor> N = senderNode.NeighboreNodes;
                if (N != null)
                {
                    if (N.Count > 0)
                    {
                        // sum:
                        double EnergySum = 0; // energy
                        double DirectionSum = 0; // direction
                        double PreDistanceSum = 0; // prependiculare distance
                        double TransDistanceSum = 0; // transmission distance
                        foreach (Sensor potentialForwarder in N)
                        {
                            if (potentialForwarder.ResidualEnergy > 0)
                            {
                                // if next hop is wthin the zone of the source node then:
                                Point point = potentialForwarder.CenterLocation;
                                if (Geo.PointTestParallelogram(Zone, point))
                                {
                                    double Dij = Operations.DistanceBetweenTwoSensors(senderNode, potentialForwarder);// i and j
                                    double Djb = Operations.DistanceBetweenTwoSensors(potentialForwarder, PublicParamerters.SinkNode); // j and b
                                    double Dib = Operations.DistanceBetweenTwoSensors(senderNode, PublicParamerters.SinkNode); // i and b

                                    RoutingMetric metRic = new RoutingMetric();
                                    metRic.PID = PublicParamerters.NumberofGeneratedPackets;
                                    metRic.SenderNode = senderNode;
                                    metRic.SourceNode = senderNode;
                                    metRic.PotentialForwarder = potentialForwarder;
                                    metRic.DistanceFromSenderToForwarder = Dij;
                                    metRic.DistanceFromSenderToSink = Dib;
                                    metRic.DistanceFromFrowarderToSink = Djb;
                                    metRic.r = senderNode.ComunicationRangeRadius;
                                    metRic.ZoneWidthControl = Settings.Default.ZoneWidth;

                                    // sum's:
                                    EnergySum += Math.Exp(Math.Pow(metRic.NormalizedEnergy, Settings.Default.EnergyDistCnt));
                                    TransDistanceSum += (1 / (Math.Exp(Math.Pow(metRic.NormalizedTransDistance, Settings.Default.TransDistanceDistCnt))));
                                    DirectionSum += (1 / (Math.Exp(Math.Pow(metRic.NormalizedDirection, Settings.Default.DirectionDistCnt))));
                                    PreDistanceSum += (1 / (Math.Exp(Math.Pow(metRic.NormalizePerpendicularDistance, Settings.Default.PrepDistanceDistCnt))));

                                    Forwarders.Add(metRic); // keep for each node.
                                }
                                else
                                { // if the sink is a direct neigbore with me
                                    if (PublicParamerters.SinkNode.ID == potentialForwarder.ID)
                                    {
                                        double Dij = Operations.DistanceBetweenTwoSensors(senderNode, potentialForwarder);// i and j
                                        double Djb = Operations.DistanceBetweenTwoSensors(potentialForwarder, PublicParamerters.SinkNode); // j and b
                                        double Dib = Operations.DistanceBetweenTwoSensors(senderNode, PublicParamerters.SinkNode); // i and b

                                        RoutingMetric metRic = new RoutingMetric();
                                        metRic.PID = PublicParamerters.NumberofGeneratedPackets;
                                        metRic.SenderNode = senderNode;
                                        metRic.SourceNode = senderNode;
                                        metRic.PotentialForwarder = potentialForwarder;
                                        metRic.DistanceFromSenderToForwarder = Dij;
                                        metRic.DistanceFromSenderToSink = Dib;
                                        metRic.DistanceFromFrowarderToSink = Djb;
                                        metRic.r = senderNode.ComunicationRangeRadius;
                                        metRic.ZoneWidthControl = Settings.Default.ZoneWidth;

                                        // sum's:
                                        EnergySum += Math.Exp(Math.Pow(metRic.NormalizedEnergy, Settings.Default.EnergyDistCnt));
                                        TransDistanceSum += (1 / (Math.Exp(Math.Pow(metRic.NormalizedTransDistance, Settings.Default.TransDistanceDistCnt))));
                                        DirectionSum += (1 / (Math.Exp(Math.Pow(metRic.NormalizedDirection, Settings.Default.DirectionDistCnt))));
                                        PreDistanceSum += (1 / (Math.Exp(Math.Pow(metRic.NormalizePerpendicularDistance, Settings.Default.PrepDistanceDistCnt))));


                                        Forwarders.Add(metRic); // keep for each node.
                                    }
                                }
                            }
                        }


                        double LinkQEstiSum = 0;//  the sum of link quality estimations, for all forwarders.
                        int k = 0;
                        foreach (RoutingMetric forwarder in Forwarders)
                        {
                            // propablity distrubution:
                            forwarder.EnPr = (Math.Exp(Math.Pow(forwarder.NormalizedEnergy, Settings.Default.EnergyDistCnt))) / EnergySum;
                            forwarder.TdPr = (1 / (Math.Exp(Math.Pow(forwarder.NormalizedTransDistance, Settings.Default.TransDistanceDistCnt)))) / TransDistanceSum;
                            forwarder.DirPr = (1 / (Math.Exp(Math.Pow(forwarder.NormalizedDirection, Settings.Default.DirectionDistCnt)))) / DirectionSum; // propablity for lemda.
                            forwarder.PerdPr = (1 / (Math.Exp(Math.Pow(forwarder.NormalizePerpendicularDistance, Settings.Default.PrepDistanceDistCnt)))) / PreDistanceSum;
                            LinkQEstiSum += forwarder.LinkEstimation;
                            k++;
                        }

                        foreach (RoutingMetric Potentialforwarder in Forwarders)
                        {
                            Potentialforwarder.LinkEstimationNormalized = Potentialforwarder.LinkEstimation / LinkQEstiSum;

                        } // end lik hoode 

                        /*
                       
                        //
                        List<RoutingMetric> selected = new List<Forwarding.RoutingMetric>();
                        foreach (RoutingMetric or in Forwarders)
                        {
                            if(or.LinkEstimationNormalized>(1/(Convert.ToDouble(Forwarders.Count))))
                            {
                                selected.Add(or);
                            }
                        }

                        if(selected.Count>=1)
                        {
                            Forwarders.Clear();
                            Forwarders = selected;
                        }

                        LinkQEstiSum = 0;
                        foreach (RoutingMetric forwarder in Forwarders)
                        {
                            LinkQEstiSum += forwarder.LinkEstimation;
                        }

                        foreach (RoutingMetric Potentialforwarder in Forwarders)
                        {
                            Potentialforwarder.LinkEstimationNormalized = Potentialforwarder.LinkEstimation / LinkQEstiSum;

                        } 
                        */

                    }


                }
            }

            Forwarders.Sort(new LinkPrioritySorter());// sort according to Priority.

            return Forwarders;
        }
    }
}



   

