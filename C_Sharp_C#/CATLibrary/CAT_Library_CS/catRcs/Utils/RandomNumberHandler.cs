using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatRcs.Utils
{
    public static class RandomNumberHandler
    {
        public static Random objRand = null;

        public static double[] Runif(int n, double min = 0, double max = 1)
        {
            double[] result = null;
            objRand = new Random();

            if(n > 0)
            {
                result = new double[n];
            }
            else
            {
                return result;
            }

            if (double.IsInfinity(min) || double.IsInfinity(min) || max < min)
            {
                return result;
            }

            if(min == max)
            {
                result = new double[1];
                result[0] = min;
                return result;
            }
            else
            {
                for (int i = 0; i < result.Length; i++ )
                {
                    double u = 0;

                    while(u <= 0 || u >= 1)
                    {
                        u = objRand.NextDouble();
                    }

                    u = min + (max - min) * u;

                    if(!result.ToList().Exists(p => p == u))
                    {
                        result[i] = u;
                    }
                }
            }

            return result;
        }

        public static void Set_seed(int seed)
        {
            objRand = new Random(seed);
        }

        // Function returns a simple random permutation of the values inside "x" (Knuth / Fisher–Yates shuffle)
        public static int[] Sample(int[] x, int size, bool replace = false, double[] prob = null)
        {
            List<int> resultList = new List<int>();

            if (objRand == null)
            {
                objRand = new Random();
            }

            
            int[] x_temp = x;
            int len = x.Length;

            // Shuffle the numbers based on index swap
            while (len > 1)
            {
                len--;
                int index = objRand.Next(len + 1);
                int temp = x_temp[index];
                x_temp[index] = x_temp[len];
                x_temp[len] = temp;
            }

            // Check for size
            if (size == 0)
            {
                size = x.Length;
            }

            // Get the desired size of numbers
            for (int i = 0; i < size; i++)
            {
                resultList.Add(x_temp[i]);
            }

            return resultList.ToArray();
        }
    }
}
