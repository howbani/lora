using LORA.Modules;
using System;
using System.Collections.Generic;
using LORA.Energy;
using LORA.ExpermentsResults.Lifetime;

namespace LORA.Parameters
{
    /// <summary>
    /// 
    /// </summary>
    public class PublicParamerters
    {


        public static double TotalWastedEnergyJoule { get; set; } // idel listening energy
        public static double TotalEnergyConsumptionJoule { get; set; }
        public static double WastedEnergyPercentage { get { return 100 * (TotalWastedEnergyJoule / TotalEnergyConsumptionJoule); } } // idel listening energy percentage  
        public static long TotalReduntantTransmission { get; set; } // how many transmission are redundant, that is to say, recived and canceled.

        public static double AverageRedundantTransmissions { get { return TotalReduntantTransmission / NumberofGeneratedPackets;} }
        public static double RoutingDistanceEffeciency { get; set; }
        public static double AverageRoutingDistanceEffeciency { get { return RoutingDistanceEffeciency / NumberofGeneratedPackets; } }

        public static double TotalNumberOfHope { get; set; }
        public static double AverageTotalNumberOfHope { get { return TotalNumberOfHope / NumberofGeneratedPackets; } }

        public static double TransmissionDistanceEff { get; set; }
        public static double AverageTransmissionDistanceEff { get { return TransmissionDistanceEff / NumberofGeneratedPackets; } }

        public static double TotalWaitingTime { get; set; } // how many times the node waitted for its node to wake up.
        public static double AverageWaitingTime { get { return TotalWaitingTime / NumberofGeneratedPackets; } }



        public static long NumberofDropedPacket { get; set; }
        public static long NumberofDeliveredPacket { get; set; } // the number of the pakctes recived in the sink node.
        public static long Rounds { get; set; } // how many rounds.
        public static List<DeadNodesRecord> DeadNodeList = new List<DeadNodesRecord>();
        public static long NumberofGeneratedPackets { get; set; }
      
       
        public static bool IsNetworkDied { get; set; }
        public static double SensingRangeRadius { get; set; }
        public static double Density { get; set; } // average number of neighbores (stander deiviation)
        public static string NetworkName { get; set; }
        public static Sensor SinkNode { get; set; }
        public static double BatteryIntialEnergy = 0.1; //J 0.5
        public static double RoutingDataLength = 1024; // bit
        public static double ControlDataLength = 521; // bit
        public static double PreamlePackets = 128;
        public static double MultiplyBy = 1; // uniform(x), here MultiplyBy=x; u can change x=100 or any.
        public static double E_elec = 50; // unit: (nJ/bit) //Energy dissipation to run the radio
        public static double Efs = 0.01;// unit( nJ/bit/m^2 ) //Free space model of transmitter amplifier
        public static double Emp = 0.0000013; // unit( nJ/bit/m^4) //Multi-path model of transmitter amplifier
        public static double CommunicationRangeRadius { get { return SensingRangeRadius * 2; } } // sensing range is R in the DB.
        public static double TransmissionRate = 2 * 1000000;////2Mbps 100 × 10^6 bit/s , //https://en.wikipedia.org/wiki/Transmission_time
        public static double SpeedOfLight = 299792458;//https://en.wikipedia.org/wiki/Speed_of_light

       

       

        public static long InQueuePackets
        {
            get
            {
                return NumberofGeneratedPackets - NumberofDeliveredPacket - NumberofDropedPacket;
            }
        }
        public static double SucessRatio
        {
            get
            {
                return 100 * (Convert.ToDouble(NumberofDeliveredPacket) / Convert.ToDouble(NumberofGeneratedPackets));
            }
        }
        public static double DropedRatio
        {
            get
            {
                return 100 * (Convert.ToDouble(NumberofDropedPacket) / Convert.ToDouble(NumberofGeneratedPackets));
            }
        }

        public static double ThresholdDistance  //Distance threshold ( unit m) 
        {
            get { return Math.Sqrt(Efs / Emp); }
        }



        public static double SensingFeildArea
        {
            get; set;
        }
        public static int NumberofNodes
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public static int ControlsRange
        {
            get { return Convert.ToInt16(Density * SensingRangeRadius); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static double MaxRandomContols
        {
            get { return (ControlsRange / 3); } // we have three parts:
        }
        
       /*
        /// <summary>
        /// node that the comunication range = radius *2
        /// </summary>
        public static double CandidateZoneWidth
        {
            get
            {
                if (CommunicationRangeRadius * 2 > Density)
                {
                    return (CommunicationRangeRadius * Math.Sqrt(2 * CommunicationRangeRadius)) / Density;
                }
                else
                {
                    System.Windows.MessageBox.Show("DENSITY>COM RANGE!!!");
                    return 1;
                }
            }
        }*/

        public static double CandidateZoneWidth
        {
            get
            {
                if (CommunicationRangeRadius * 2 > Density)
                {
                    return (2 * CommunicationRangeRadius) / Math.Sqrt(Density);
                }
                else
                {
                    System.Windows.MessageBox.Show("DENSITY>COM RANGE!!!");
                    return 1;
                }
            }
        }


        /// <summary>
        /// Each time when the node loses 5% of its energy, it shares new energy percentage with its neighbors. The neighbor nodes update their energy distributions according to the new percentage immediately as explained by Algorithm 2. 
        /// </summary>
        public static int BatteryLosePerUpdate
        {
            get
            {
                //
                return 5;
            }
            set
            {

            }
        }

        /// <summary>
        /// how many nodes in the H length area. the width is fixed.
        /// </summary>
        /// <param name="length_of_zone"></param>
        /// <returns></returns>
        public static double ExpectedNumberoNodesInZone(double length_of_zone)
        {
            double exp = (NumberofNodes * length_of_zone * CandidateZoneWidth) / SensingFeildArea;
            return exp;
        }

        /// <summary>
        /// expected number of forwarders
        /// </summary>
        public static double ExpectedNumberOfFowarders
        {
            get
            {
                // tringulure.
              //  double a = Math.Pow(CandidateZoneWidth / 2, 2);
              //  double b = Math.Pow(CommunicationRangeRadius, 2);
              //  double c = Math.Sqrt(a + b);

               double ExF = (CandidateZoneWidth * CommunicationRangeRadius * Density) / (Math.PI * Math.Pow(CommunicationRangeRadius, 2)); //=Math.Sqrt(2 * CommunicationRangeRadius) / Math.PI;
              // double CExF = Math.Ceiling(ExF);

               // double ExF = (CandidateZoneWidth * c * Density) / (Math.PI * Math.Pow(c, 2)); //=Math.Sqrt(2 * CommunicationRangeRadius) / Math.PI;
               // double CExF = Math.Ceiling(ExF);
                return ExF;
            }
        }

    

        // lifetime paramerts:
        public static int NOS { get; set; } // NUMBER OF RANDOM SELECTED SOURCES
        public static int NOP { get; set; } // NUMBER OF PACKETS TO BE SEND.




        /// <summary>
        /// in sec.
        /// </summary>
        public static class Periods
        {
            public static double ActivePeriod = 1; //  the node trun on and check for CheckPeriod seconds.// +1
            public static double SleepPeriod = 2; // the node trun off and sleep for SleepPeriod seconds.
        }



        /// <summary>
        /// When all forwarders are sleep, 
        /// the sender try agian until its formwarder is wake up. the sender try agian each 500 ms.
        /// when the sensor retry to send the back is it's forwarders are in sleep mode.
        /// </summary>
        public static TimeSpan SenderWaitingTime
        {
            get
            {
                return TimeSpan.FromSeconds(1);
            }
        }

        /// <summary>
        /// the timer interval between 1 and 5 sec.
        /// </summary>
        public static int MacStartUp
        {
            get
            {
                return 10;
            }
            set { }
        }

        /// <summary>
        /// the runnunin time of simulator. in SEC
        /// </summary>
        public static int SimulationTime
        {
            get;set;
        }


        public static List<BatRange> getRanges()
        {
            List<BatRange> re = new List<BatRange>();

            int x = 100 / BatteryLosePerUpdate;
            for (int i = 1; i <= x; i++)
            {
                BatRange r = new Energy.BatRange();
                r.isUpdated = false;
                r.Rang[0] = (i - 1) * BatteryLosePerUpdate;
                r.Rang[1] = i * BatteryLosePerUpdate;
                r.ID = i;
                re.Add(r);
            }

            re[re.Count - 1].isUpdated = true;

            return re;
        }
    }
}
