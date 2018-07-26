using System;
using System.Collections.Generic;

namespace CatRcs
{
    public static class CatRLib
    {
        public static string[] GetStringArray(string[] inputArray)
        {
            List<string> resultList = new List<string>();            

            if(inputArray.Length > 0)
            {
                foreach (string value in inputArray)
                {
                    resultList.Add("Welcome to CAT-R ... + '" + value + "'!");
                }
            }

            string[] resultArray = resultList.ToArray();

            return resultArray;
        }

        public static double[] GetNumericArray(double[] inputArray)
        {
            List<double> resultList = new List<double>();

            if (inputArray.Length > 0)
            {
                foreach (double value in inputArray)
                {
                    resultList.Add(value);
                }
            }

            double[] resultArray = resultList.ToArray();

            return resultArray;
        }
    }
}