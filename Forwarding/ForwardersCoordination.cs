using LORA.Parameters;
using LORA.Forwarding;
using System;
using System.Collections.Generic;
using LORA.Modules;
using LORA.Properties;

namespace LORA.Forwarding
{
    public class ForwardersCoordination
    {
        
        private static double RdmGenerator(double max)
        {
            return UnformRandomNumberGenerator.GetUniform(max);
        }

        /// <summary>
        /// the senderNode seletes the next hop from the nodes which are active or ON now.
        /// select the higest priority.
        /// we assume that the active nodes will recive the date and send ACK. we assume also that the active nodes recived the pck and the sender node send CTS packet, to let the selected forwarder node to send its packet.
        /// 
        /// </summary>
        /// <param name="candidates"></param>
        /// <returns></returns>
        public RoutingMetric SelectedForwarder( Sensor senderNode, Sensor source)
        {
            RoutingMetric Max = null;
            List<RoutingMetric> wakUps = new List<Forwarding.RoutingMetric>();

            // check the sink is there. one hop to the sink.
            foreach (RoutingMetric candidate in senderNode.MyForwardersShortedList)
            {
                if (candidate.PotentialForwarder.ID == PublicParamerters.SinkNode.ID)
                {
                    Max = candidate;
                }
            }


            if (Max == null)
            {
                bool isMaxSelected = false;
                if (senderNode.ID != source.ID)
                {
                    // sort the nodes agian prependicual distance
                    ForwardersSelection xx = new Forwarding.ForwardersSelection();
                    xx.UpdateMyForwardersAccordingtoPerpendicaureDistance(senderNode, source);
                }

                // copy.
                List<RoutingMetric> coordernaters = new List<Forwarding.RoutingMetric>();
                coordernaters.AddRange(senderNode.MyForwardersShortedList);

                // candidates are sorted according to heigh pri, then we need to select the first active node " radio ON".
                foreach (RoutingMetric candidate in coordernaters)
                {

                    // check it one of candidate is the sink node.
                    // we asume that the currently Active node, can lsiten and will be the recieve the data.
                    // the heighest active node.
                    if (candidate.PotentialForwarder.CurrentSensorState == Datatypes.SensorStatus.Active)
                    {
                        if (!isMaxSelected)
                        {
                            isMaxSelected = true;
                            Max = candidate;
                        }
                        else
                        {
                            Logs.Logs.RedundantPackets(senderNode, candidate.PotentialForwarder);
                            PublicParamerters.TotalReduntantTransmission += 1;
                        }
                    }
                    
                }
            }

            return Max;
        }

       

    }
}
