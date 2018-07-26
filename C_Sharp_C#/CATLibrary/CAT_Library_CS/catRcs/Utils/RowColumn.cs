using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatRcs.Utils
{
    public class RowColumn
    {
        // Calculates Summation of each row within a Multidimensional double array
        public static double[] rowSums(double[,] param)
        {
            int rowLength = param.GetLength(0);
            int columnLength = param.GetLength(1);

            double[] result = new double[rowLength];
            double summation = 0;

            for (int i = 0; i < rowLength; i++)
            {
                summation = 0.00;

                for (int j = 0; j < columnLength; j++)
                {
                    summation = summation + (Double.IsNaN(param[i, j]) ? 0 : param[i, j]);
                }

                result[i] = summation;
            }

            return result;
        }

        public static double rowSum(double[,] param)
        {
            int rowLength = param.GetLength(0);
            int columnLength = param.GetLength(1);

            double summation = 0;

            for (int i = 0; i < rowLength; i++)
            {
                summation = 0.00;

                for (int j = 0; j < columnLength; j++)
                {
                    summation = summation + param[i, j];
                }
            }

            return summation;
        }

        // Calculates Summation of double array values
        public static double Sum(double[] param)
        {
            double summation = 0.00;

            for (int a = 0; a < param.Length; a++)
            {
                summation = summation + (Double.IsNaN(param[a]) ? 0 : param[a]);
                //summation = summation + param[a];
            }

            return summation;
        }

        // Calculates Summation of int array values
        public static double Sum(int[] param)
        {
            double summation = 0.00;

            for (int a = 0; a < param.Length; a++)
            {
                summation = summation + (Double.IsNaN(param[a]) ? 0 : param[a]);
            }

            return summation;
        }
    }
}
