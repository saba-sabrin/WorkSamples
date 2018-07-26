using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatRcs.Utils
{
    public static class CheckNaValues
    {
        private static bool is_na = false;

        // Check NaN values
        public static bool IsNaNvalue(double value)
        {
            bool status = false;

            if (value.ToString() == double.NaN.ToString())
            {
                status = true;
            }

            return status;
        }

        // Check Positive Finite values
        public static bool IsPosInFinityValue(double value)
        {
            bool status = false;

            if (value != double.PositiveInfinity)
            {
                status = true;
            }

            return status;
        }

        // Check Negative Finite values
        public static bool IsNegInFinityValue(double value)
        {
            bool status = false;

            if (value != double.NegativeInfinity)
            {
                status = true;
            }

            return status;
        }

        // Removes Infinite values from a double array
        public static double[] GetArrayWithoutInfinity(double[] param)
        {
            List<double> final_Val = new List<double>();

            foreach (double tempVal in param)
            {
                if ((tempVal != double.PositiveInfinity) || (tempVal != double.NegativeInfinity))
                {
                    final_Val.Add(tempVal);
                }
            }

            return final_Val.ToArray();
        }

        public static double[] GetArrayWithoutNaN(double[] param)
        {
            List<int> index = new List<int>();
            double[] result_array = null;

            for (int i = 0; i < param.Length; i++)
            {
                if (!Double.IsNaN(param[i]))
                {
                    index.Add(i);
                }
                else
                {
                    is_na = true;
                }
            }

            if (is_na)
            {
                result_array = new double[index.Count];

                for (int j = 0; j < result_array.Length; j++)
                {
                    result_array[j] = param[index[j]];
                }
            }
            else
            {
                result_array = param;
            }

            return result_array;
        }

        // Removes NaN values from a double array
        public static double[][] GetArrayWithoutNaN(double[] param, double[] v_param)
        {
            List<int> index = new List<int>();
            double[][] result_array = new double[2][];
            double[] v_temp = null; double[] param_temp = null;

            for (int i = 0; i < param.Length; i++)
            {
                if (!Double.IsNaN(param[i]))
                {
                    index.Add(i);
                }
                else
                {
                    is_na = true;
                }
            }

            if (is_na)
            {
                param_temp = new double[index.Count];
                v_temp = new double[index.Count];

                for (int j = 0; j < param_temp.Length; j++)
                {
                    param_temp[j] = param[index[j]];
                    v_temp[j] = v_param[index[j]];
                }

                result_array[0] = param_temp;
                result_array[1] = v_temp;
            }
            else
            {
                result_array[0] = param;
                result_array[1] = v_param;
            }

            return result_array;
        }
    }
}
