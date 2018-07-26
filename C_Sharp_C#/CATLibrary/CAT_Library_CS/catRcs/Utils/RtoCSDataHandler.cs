using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDotNet;
using RDotNet.NativeLibrary;
using CatRcs.Models;

namespace CatRcs.Utils
{
    public static class RtoCSDataHandler
    {
        public static string DataConverterRtoCS(CATItems itemBank, Double[] items)
        {
            int index = 0;

            try
            {
                // Populate data to the object array property "all_items_poly"
                for (int i = 0; i < itemBank.colSize; i++)
                {
                    double[] array = new double[itemBank.NumOfItems];

                    for (int j = 0; j < itemBank.NumOfItems; j++)
                    {
                        array[j] = items[index];
                        index++;
                    }

                    itemBank.all_items_poly[i] = array;
                }

                // Convert "NA" values to "NaN"
                itemBank.NAValueHandler();
            }
            catch(System.IndexOutOfRangeException ex)
            {
                //throw ex;
            }

            return "Index value: " + index.ToString();

            //return itemBank;
        }

        public static CATItems DataConverterRtoCS(CATItems itemBank, DataFrame items)
        {
            // Populate data to the object array property "all_items_poly"
            for (int i = 0; i < itemBank.colSize; i++)
            {
                itemBank.all_items_poly[i] = items[i].Select(y => (double)y).ToArray();
            }

            // Convert "NA" values to "NaN"
            itemBank.NAValueHandler();

            return itemBank;
        }

        // Convert NA values to NaN
        public static double[] NAValueHandler(double[] param)
        {
            for (int k = 0; k < param.Length; k++)
            {
                if (Double.IsNaN(param[k]))
                {
                    param[k] = double.NaN;
                }
            }

            return param;
        }
    }
}
