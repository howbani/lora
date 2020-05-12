using System;
using System.Collections.Generic;
using LORA.Modules;
using LORA.Properties;
using LORA.Forwarding;
using System.Threading;

namespace LORA.ExpermentsResults.Lifetime
{
    public class NetworkLifeTime
    {

        /// <summary>
        /// how many sensor to be slected.
        /// how many packet each sensor will sent.
        /// </summary>
        /// <param name="NOS"></param>
        /// <param name="NOP"></param>
        public void RandimSelect(List<Sensor> Network, int NOS, int NOP)  
        {
            // selecte The Nodes:
            List<Sensor> SelectedSn = new List<Modules.Sensor>(NOS);
            for (int i = 0; i < NOS; i++)
            {
                int ran = 1 + Convert.ToInt16(UnformRandomNumberGenerator.GetUniform(Network.Count - 2));
                SelectedSn.Add(Network[ran]);
            }

            // each packet sendt NOP:
            for (int i = 0; i < NOP; i++)
            {
                foreach (Sensor sen in SelectedSn)
                {
                    sen.GeneratePacketAndSent();
                }
            }

        } // end class random generated.



        public void FromAllNodes(List<Sensor> Network)
        {
            foreach (Sensor sen in Network)
            {
                sen.GeneratePacketAndSent();
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
            }
        }


    }
}
