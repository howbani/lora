using LORA.Parameters;
using System;
using System.Collections.Generic;

namespace LORA.Forwarding
{
    public class  RandomControls
    {
         
        /// <summary>
        ///  Direction should be fixed:
        /// </summary>
        /// <returns></returns>
        public static List<double> Generate4Controls()
        {
            List<double> re = new List<double>();
            double max = PublicParamerters.MaxRandomContols; //(ControlsRange / 3); } 
            double DirectionDistCnt = PublicParamerters.ControlsRange / 2;
            double EnergyDistCnt = 1 + UnformRandomNumberGenerator.GetUniform(Math.Sqrt(max));
            double TransDistanceDistCnt = UnformRandomNumberGenerator.GetUniform(max);
            double PrepDistanceDistCnt = UnformRandomNumberGenerator.GetUniform(max);

            /// don't change the order of this.
            re.Add(EnergyDistCnt); //0
            re.Add(TransDistanceDistCnt); //1
            re.Add(DirectionDistCnt);//2
            re.Add(PrepDistanceDistCnt);//3
            return re;

        }
    }
}
