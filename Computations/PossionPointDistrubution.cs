using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LORA.Computations
{
    public class PossionPointDistrubution
    {
        decimal lambda;

        public PossionPointDistrubution(decimal lambda = 1.0M)
        {
            this.lambda = lambda;
        }

        public decimal ProbabilityMassFunction(int k)
        {
            //(l^k / k! ) * e^-l
            //l = lamda
            int kFactorial = Factorial(k);
            double numerator = Math.Pow(Math.E, -(double)lambda) * Math.Pow((double)lambda, (double)k);

            decimal p = (decimal)numerator / kFactorial;
            return p;
        }

        public decimal CummulitiveDistributionFunction(int k)
        {
            double e = Math.Pow(Math.E, (double)-lambda);
            int i = 0;
            double sum = 0.0;
            while (i <= k)
            {
                double n = Math.Pow((double)lambda, i) / Factorial(i);
                sum += n;
                i++;
            }
            decimal cdf = (decimal)e * (decimal)sum;
            return cdf;
        }

        private int Factorial(int k)
        {
            int count = k;
            int factorial = 1;
            while (count >= 1)
            {
                factorial = factorial * count;
                count--;
            }
            return factorial;
        }
    }

}
