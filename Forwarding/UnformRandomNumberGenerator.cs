using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LORA.Computations;
using System.Threading;

namespace LORA.Forwarding
{
    /// <summary>
    /// generate anumber between 0- max:
    /// </summary>
    public static class UnformRandomNumberGenerator
    {
        public static double GetUniform(double max) 
        {
            return max * RandomeNumberGenerator.GetUniform();
        }

        public static double GetUniformSleepSec(double max)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(1));
            return max * RandomeNumberGenerator.GetUniform();
        }
    }
}
