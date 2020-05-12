using LORA.Computations;
using LORA.Modules;
using System;
using System.Windows;

namespace LORA.Forwarding
{
    /*
     To understance this code, please refere to the manscript section 4.
         */
    public class RoutingMetric
    {


        public long PID { get; set; } // PACKET SEQUENCE ID
        public int SourceID  { get { return SourceNode.ID; } } // the sourc ID
        public int SenderID { get { return SenderNode.ID;  } } // FORWARD NODE ID. relay node: ID, at first it is the source node.
        public int ForwarderID { get { return PotentialForwarder.ID; } } // NEXT HOP ID

        public double DirPr { get; set; } // direction prob
        public double EnPr { get; set; } //EnergyProb
        public double TdPr { get; set; } //TransDistanceProb
        public double PerdPr { get; set; } //PerpendicularDistanceProb
        public double LinkEstimationNormalized { get; set; } // normalize to one

        /// <summary>
        /// MULTIPLICATIONS
        /// </summary>
        public double LinkEstimation
        {
            get
            {
                return TdPr * DirPr * PerdPr * EnPr;
            }
        }

       

        public double ZoneWidthControl { get; set; } // the width of routing zone.

      

        
       

        public double NormalizedDirection
        {
            get
            {
                Point pi = SenderNode.CenterLocation;
                Point pj =PotentialForwarder.CenterLocation;
                Point pb =Parameters.PublicParamerters.SinkNode.CenterLocation;
                double axb = (pj.X - pi.X) * (pb.X - pi.X) + (pj.Y - pi.Y) * (pb.Y - pi.Y);
                double disMul = DistanceFromSenderToForwarder * DistanceFromSenderToSink;
                double t = axb / disMul;
                double f = (1 - t) / 2;
                return f+1;
            }
        }
       
        public double NormalizedEnergy { get { return 1 + (PotentialForwarder.ResidualEnergyPercentage / 100); } }
        public double NormalizedTransDistance
        {
            get
            {
                double nj = (DistanceFromSenderToForwarder / (2 * r));
                return nj+1;
            }
        }
      
        public double NormalizePerpendicularDistance
        { // 
            get
            {
                double h = (Hj / ((2 * r) + Hi));
                return h+1;
            }
        }

      
     
     
         
        public string RoutingRangeString { get { return RoutingProbRange[0].ToString("0.000") + "-" + RoutingProbRange[1].ToString("0.000"); } }


        /// <summary>
        /// perpendicular distance:shortest distance from a pj(point) to any point on a fixed Line in Euclidean geometry.
        /// If the line passes through two points P1=(x1,y1) and P2=(x2,y2) then the distance of (x0,y0) from the line is:
        /// https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line
        /// j: the node to be recived the packet.
        /// </summary>
        public double Hj
        {
            get
            {
                Point pj = PotentialForwarder.CenterLocation;
                Point pb = Parameters.PublicParamerters.SinkNode.CenterLocation;
                Point ps = SourceNode.CenterLocation;
                double past = ((pb.Y - ps.Y) * pj.X) - ((pb.X - ps.X) * pj.Y) + (pb.X * ps.Y) - (pb.Y * ps.X);
                past = Math.Sqrt(Math.Pow(past, 2));
                double sbDis = Operations.DistanceBetweenTwoPoints(ps, pb);
                return past / sbDis;
            }
        }

        /// <summary>
        /// perpendicular distance:shortest distance from a pi(point) to any point on a fixed Line in Euclidean geometry.
        /// If the line passes through two points P1=(x1,y1) and P2=(x2,y2) then the distance of (x0,y0) from the line is:
        /// https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line
        /// the current forward node.
        /// </summary>
        public double Hi  
        {
            get
            {
                Point pi = SenderNode.CenterLocation;
                Point ps = SourceNode.CenterLocation;
                Point pb = Parameters.PublicParamerters.SinkNode.CenterLocation;
                double past = ((pb.Y - ps.Y) * pi.X) - ((pb.X - ps.X) * pi.Y) + (pb.X * ps.Y) - (pb.Y * ps.X);
                past = Math.Sqrt(Math.Pow(past, 2));
                double dis = Operations.DistanceBetweenTwoPoints(ps, pb);
                return past / dis;
            }
        }


        /// <summary>
        /// Distance from sender to potential forwarder
        /// </summary>
        public double DistanceFromSenderToForwarder { get; set; } // 
        /// <summary>
        /// from sender to sink
        /// </summary>
        public double DistanceFromSenderToSink { get; set; } // 
        /// <summary>
        /// from forwarder to the sink
        /// </summary>
        public double DistanceFromFrowarderToSink { get; set; } // 

        public double[] RoutingProbRange = new double[2];

        public double r { get; set; }
        public Sensor SourceNode { get; set; } // s // the source node of this packet 
        public Sensor SenderNode { get; set; } //i // the current senser. ( current relay).
        public Sensor PotentialForwarder { get; set; } //j //  the node to be the netx hop node

      


    }
}
