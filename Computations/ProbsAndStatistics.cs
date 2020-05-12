using LORA.Computations;
using LORA.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LORA.Computations
{
    public class ProbsAndStatistics
    {
        /// <summary>
        /// the probablity that the subzone is empty.
        /// When the sender has no forwarders in its subzone, the sender define a new Candidates Zone and acts as a source node.  Each sender node has approximately D neighbor nodes.   The probability that no node is located within the sender’s subzone is expressed in Eq. (17), however, the probability that there are E_xf≥k≥1 nodes within the sender’s subzone is shown in Eq. (18). The probability that there are exactly E_xf nodes within the sender’s subzone is shown in Eq. (19). 
        /// </summary>
        public static double PrNoNodeWithinSubZone
        {
            get
            {
                double subzoneArea = (PublicParamerters.CandidateZoneWidth * PublicParamerters.CommunicationRangeRadius);
                double sensorArea = Operations.FindNodeArea(PublicParamerters.CommunicationRangeRadius);
                double pr1 = subzoneArea / sensorArea;// prob of the node will be located within the supzone.
                double pr2 = 1 - pr1;// prob of the node will not be located within the supzone.

                double PronoNodeWithinTheSubzone = Math.Pow(pr2, PublicParamerters.Density);

                return PronoNodeWithinTheSubzone;
            }
        }
        /// <summary>
        /// The probability that there are exactly E_xf nodes within the sender’s subzone
        /// </summary>
        /// <returns></returns>
        public static double ProThereAreExfNodesWithinTheSubzone
        {
            get
            {
                double Exf = Convert.ToInt32(Math.Ceiling(PublicParamerters.ExpectedNumberOfFowarders));
                int int_Exf = Convert.ToInt16(Math.Ceiling(Exf));
                double pr = ProThatKnodesInSubzone(int_Exf);
                return pr;
            }
        }

        /// <summary>
        /// the probability that there are E_xf≥k≥1 nodes within the sender’s subzone 
        /// </summary>
        public static double ProAtLeastOneAndAtMostExf
        {
            get
            {
                double Exf = Convert.ToInt32(Math.Ceiling(PublicParamerters.ExpectedNumberOfFowarders));
                int int_Exf = Convert.ToInt16(Math.Ceiling(Exf));
                double sumPro = 0;
                for(int i=1;i<= int_Exf;i++)
                {
                    double cp= ProThatKnodesInSubzone(i);
                    sumPro += cp;
                }
                return sumPro;
            }
        }

        public static double ProThatKnodesInSubzone(int K)
        {
            double subzoneArea = (PublicParamerters.CandidateZoneWidth * PublicParamerters.CommunicationRangeRadius);
            double sensorArea = Operations.FindNodeArea(PublicParamerters.CommunicationRangeRadius);
            double pr1 = subzoneArea / sensorArea;// prob of the node will be located within the supzone.
            double pr2 = 1 - pr1;// prob of the node will not be located within the supzone.

          
            int D = Convert.ToInt32(Math.Ceiling(PublicParamerters.Density));

            double combina = Operations.Combination(D, K);
            double PowK = Math.Pow(pr1, K);
            double PowD_K = Math.Pow(pr2, D - K);

            double pr = combina * PowK * PowD_K;

            return pr;
        }
    }
}
