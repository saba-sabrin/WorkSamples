using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatRcs.Utils
{
    public class CommonHelper
    {
        // A common exception handling structure must be defined with return messages.

        /* This function represents "rep", which replicates the values in given parameter "x"
         * "times", indicates the number of times to repeat elements of "x"
         *  "len", indicates the desired length of the result
         *  "each", indicates the number of times every element in "x" is repeated.
         */
        public static double[] Replicate(double[] x, int times, int? each = null, int? len = null)
        {
            List<double> res_final = new List<double>(); // No size limit for Generic list
            List<double> res_times_each = new List<double>();

            if (x.Length != 0)
            {
                if(times > 0)
                {
                    for(int i = 0; i < times; i++)
                    {
                        res_final.AddRange(x);
                    }
                }

                if(each > 1)
                {
                    //var results = (from value in result from e in Enumerable.Range(0, each) select x).ToList();

                    int currentLength = res_final.Count;
                    
                    for (int i = 0; i < currentLength; i++)
                    {
                        int count = 0;

                        while (count < each)
                        {
                            res_times_each.Add(res_final[i]);
                            count++;
                        }
                    }

                    res_final = res_times_each;
                }

                if(len != null && len > 0)
                {
                    res_final = res_final.GetRange(0, int.Parse(len.ToString()));
                }
            }
            else
            {
                // Not Handled
            }

            return res_final.ToArray();
        }

        public static int[] Replicate(int[] x, int times, int? each = null, int? len = null)
        {
            List<int> res_final = new List<int>(); // No size limit for Generic list
            List<int> res_times_each = new List<int>();

            if (x.Length != 0)
            {
                if (times > 0)
                {
                    for (int i = 0; i < times; i++)
                    {
                        res_final.AddRange(x);
                    }
                }

                if (each > 1)
                {
                    //var results = (from value in result from e in Enumerable.Range(0, each) select x).ToList();

                    int currentLength = res_final.Count;

                    for (int i = 0; i < currentLength; i++)
                    {
                        int count = 0;

                        while (count < each)
                        {
                            res_times_each.Add(res_final[i]);
                            count++;
                        }
                    }

                    res_final = res_times_each;
                }

                if (len != null && len > 0)
                {
                    res_final = res_final.GetRange(0, int.Parse(len.ToString()));
                }
            }
            else
            {
                // Not Handled
            }

            return res_final.ToArray();
        }

        /* Represents function "seq" which generates regulaer sequences
         * "along.with" parameter not handled and has not been used */
        public static double[] Sequence(double from, double to, double length = 0, double by = 0)
        {
            List<double> result = new List<double>();

            if(to > from)
            {
                // to check for integer or decimal type result
                double lenCheck = to - from;

                if (by > 0)
                {
                    double sum = from, stepCount = 0;

                    while(sum <= to)
                    {
                        result.Add(sum);
                        sum = sum + double.Parse(by.ToString());
                        stepCount++;
                    }
                }

                if (length > 0)
                {
                    result.Add(from);
                    if (length > 1)
                    {
                        double increment = (to - from) / (length - 1);             
                        for (int i = 1; i < length - 1; i++)
                            result.Add(from + i * increment);
                        
                    }
                  
                    result.Add(to);
                }
            }

            return result.ToArray();
        }

        // Calculates density of given quantities
        // For undefined min and max, 0 & 1 is considered respectively
        public static double[] Dunif(double[] x, double min = 0, double max = 1)
        {
            List<double> resultList = new List<double>();

            for (int i = 0; i < x.Length; i++)
            {
                if ((x[i] <= max) && (x[i] >= min))
                {
                    resultList.Add(1 / (max - min));
                }
                else
                {
                    resultList.Add(0);
                }
            }

            return resultList.ToArray();
        }

        // Overloaded Function "Dunif"
        public static double Dunif(double x, double min = 0, double max = 1)
        {
            double result = 0;

            if ((x <= max) && (x >= min))
            {
                result = 1 / (max - min);
            }
            else
            {
                result = 0;
            }

            return result;
        }

        // Function for Calculating density of normal distribution
        public static double[] Dnorm(double[] x, double mean = 0, double sd = 1)
        {
            List<double> resultList = new List<double>();

            for (int i = 0; i < x.Length; i++)
            {
                resultList.Add((1 / (Math.Sqrt(2 * Math.PI) * sd)) * (Math.Exp(-(Math.Pow(x[i] - mean, 2) / 2 * Math.Pow(sd, 2)))));
            }

            return resultList.ToArray();
        }

        // Overloaded Function for single value
        public static double Dnorm(double x, double mean = 0, double sd = 1)
        {
            double result = (1 / (Math.Sqrt(2 * Math.PI) * sd)) * (Math.Exp(-(Math.Pow(x - mean, 2) / 2 * Math.Pow(sd, 2))));

            return result;
        }

        // FUnction for Calculating Unique numbers
        public static double[] Unique(double[] x)
        {
            List<double> resultList = new List<double>();

            resultList = x.Distinct().ToList();

            return resultList.ToArray();
        }

        // FUnction for Calculating Unique numbers
        public static int[] Unique(int[] x)
        {
            List<int> resultList = new List<int>();

            resultList = x.Distinct().ToList();

            return resultList.ToArray();
        }

        // Function for calculating rank of numbers
        public static double[] Rank(double[] x)
        {
            List<double> resultList = new List<double>();
            List<Ranking> rankList = new List<Ranking>();

            rankList = x.OrderBy(n => n).Select((n, i) => new Ranking(n, i + 1)).ToList();

            for(int i = 0; i < x.Length; i++)
            {
                resultList.Add(rankList.First(p => p.Number == x[i]).Rank);
            }

            return resultList.ToArray();
        }
    }

    public class Ranking
    {
        public double Number { get; set; }
        public double Rank { get; set; }

        public Ranking(double number, double rank)
        {
            Number = number;
            Rank = rank;
        }
    }
}
