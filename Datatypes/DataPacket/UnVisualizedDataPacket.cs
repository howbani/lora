using LORA.Parameters;
using System;

namespace LORA.DataPacket
{

    class UnVisualizedDataPacket
    {
        public long PID { get; set; } // PACKET SEQUENCTIAL ID.
        public int SID { set; get; } // source where the packets generated
        public int Hops { set; get; } // the real number of hops to the sink from the source
        public bool IsDelivered { get; set; }
        public int PacketWaitingTimes { set; get; }
        public int MaxHops { get; set; }
        public string Path { set; get; } // path
      
        
        public double Distance { set; get; } // the distance from source to the sink, or cluster head.
        public double RoutingDistance { set; get; }// The Routing Distance, denoted by〖 d〗_j^i (p_k ), of a data packet p_k traveled in the path P_j^i (p_k ) is the sum of distances between any two consecutive nodes in〖 P〗_j^i (p_k ) as modeled in (1). 
        /// <summary>
        /// RoutingDistanceEfficiency
        /// </summary>
        public double RoutingDistanceEffiecncy
        {
            get; set;
        }

        /// <summary>
        /// AverageTransDisPerHop
        /// </summary>
        public double AverageTransDistrancePerHop
        {
            get; set;
        }
        /// <summary>
        /// TransmisionDistanceEfficency
        /// </summary>
        public double TransDistanceEfficiency
        {
            get; set;
        }


        /// <summary>
        /// RoutingEfficiency
        /// </summary>
        public double RoutingEfficiency
        {
            get; set;
        }


        public double RoutingProbabilityForPath { get; set; }


        public double UsedEnergy_Joule { set; get; } // the real consumned energy for this packet in the path. the sum
        public double Delay { get; set; }

        

        // for the packets:
        public double PrepDistanceDistCnt { set; get; } 
        public double TransDistanceDistCnt { set; get; }
        public double EnergyDistCnt { set; get; }
        public double DirectionDistCnt { set; get; }
        public double RoutingZoneWidthCnt { get; set; }
        // nano jol:
        public double MaximumAcceptableEnergy
        {
            get
            {
                double E_elec = PublicParamerters.E_elec;
                double Efs = PublicParamerters.Efs;
                double Emp = PublicParamerters.Emp;
                double K = PublicParamerters.RoutingDataLength;

                return K * ((2 * Hops * E_elec) + (Efs * Distance * Distance));
            }
        }



    }
}
