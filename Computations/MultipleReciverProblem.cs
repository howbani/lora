using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LORA.Computations
{
    public class MultipleReciverProblem
    { 
        private double v { get; set; } // EXPECTED NUMBER OF FORWARDERS
        private double t { get; set; } // ACTIVE PERIED
        public double T { get; set; }
        public MultipleReciverProblem(double _ExF,double _T, double _t)  
        {
            v = _ExF;
            t = _t;
            T = _T;
        }

        /// <summary>
        /// T^x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double PowT(double x)
        {
            return Math.Pow(T, x);
        }
        /// <summary>
        /// (T-2t)^x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double PowT_2t(double x)
        {
            return Math.Pow(T - (2 * t), x);
        }
        /// <summary>
        /// (T-t)^x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double PowT_t(double x)
        {
            return Math.Pow(T - t, x);
        }
        /// <summary>
        /// (T-v)^x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double PowT_v(double x)
        {
            return Math.Pow(T - v, x);
        }


        /// <summary>
        /// all are sleep
        /// </summary>
        public double Pr0
        {
            get
            {
                double _1dT = 1 / (PowT(v - 1));
                double T_2t = PowT_2t(v);
                double vd2 = 2 / v;
                double T_t = PowT_t(v);
                double Inner = vd2 * (T_t - T_2t);
                double outer = T_2t;

                double re = _1dT * (outer + Inner);
                return re;
            }
        }
        /// <summary>
        /// one is active.
        /// </summary>
        public double Pr1
        {
            get
            {
                // v-1 are active:
                double actPr = (1 - Pr0) / v;
                // all are sleep:
                double sleepPro = Pr0;
                // one is active:
                double com = Operations.Combination(Convert.ToInt16(v), 1);
               double actPrx = Math.Pow(actPr, 1);
               double sleepProx = Math.Pow(sleepPro, v - 1);
                //  return (1 - Pr0) / (v);
                return com * actPrx * sleepProx;
            }
        }
        /// <summary>
        /// at least more than 2 are active.
        /// </summary>
        public double Prg2
        {
            get
            {
                return 1 - (Pr0 + Pr1);
            }
        }
        private double I1
        {
            get
            {
                double bK = 1 / (v * PowT(v - 1));
                double Fir = PowT_2t(v - 1) * (T + (2 * t * (v - 1)));
                double sec = PowT_t(v - 1) * (T + (t * (v - 1)));
                double Kaw = Fir - sec;
                double all = bK * Kaw;
                return all;
            }
        }

        private double I2 
        {
            get
            {
                double bK = 1 / PowT(v);
                double Kaw = 2 * t * (v - 1) * PowT_2t(v);
                double all = bK * Kaw;
                return all;
            }
        }

        private double I3
        {
            get
            {
                double bK = (v - 1) / (v * (v + 1) * PowT(v));
                double fir = PowT_v(v) * ((t * v) + T);
                double sec = PowT_2t(v) * ((2 * t * v) + T);
                double kaw = fir - sec;
                double all = bK * kaw;
                return all;
            }
        }


    }
}
