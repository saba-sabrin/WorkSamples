using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatRcs.Models;

namespace CatRcs
{
    internal class EPV
    {
        // Optimized CAT Items
        public static double EPV_Calc(CATItems itemBank, int item, int[] x, double theta, CATItems it_given, string model, double[] priorPar = null, int[] parInt = null, double D = 1, string priorDist = "norm")
        {
            double res = 0;

            double th = theta;

            if (priorPar == null || priorPar.Length < 2)
            {
                priorPar = new double[2];
                priorPar[0] = 0;
                priorPar[1] = 1;
            }

            if (parInt == null || parInt.Length < 3)
            {
                parInt = new int[3];
                parInt[0] = -4;
                parInt[1] = 4;
                parInt[2] = 33;
            }

            if (String.IsNullOrEmpty(model))  // Dichotomous Items
            {
                CATItems itj = itemBank.AddItem_D(it_given, new int[] { item });

                double p1 = Pi.Pi_Calc(th, itemBank, model, D).Pi[item-1];  // item is matched with the array index

                double p0 = 1 - p1;

                int[] temp_x = new int[x.Length + 1];
                for(int i= 0; i < x.Length; i++)
                {
                    temp_x[i] = x[i];
                }
                temp_x[x.Length -1] = 0;

                double th0 = EapEST.EapEST_Calc(itj, temp_x, "", priorPar, D, priorDist, parInt[0], parInt[1], parInt[2]);

                double var0 = Math.Pow(EapSEM.EapSEM_Calc(th0, itj, temp_x, "", priorPar, D, priorDist, parInt[0], parInt[1], parInt[2]), 2);

                temp_x = new int[x.Length + 1];
                for (int i = 0; i < x.Length; i++)
                {
                    temp_x[i] = x[i];
                }
                temp_x[x.Length - 1] = 1;

                double th1 = EapEST.EapEST_Calc(itj, temp_x, "", priorPar, D, priorDist, parInt[0], parInt[1], parInt[2]);

                double var1 = Math.Pow(EapSEM.EapSEM_Calc(th1, itj, temp_x, "", priorPar, D, priorDist, parInt[0], parInt[1], parInt[2]), 2);

                res = (p0 * var0) + (p1 * var1);
            }
            else   // Polytomous Items  !!!! Optimization needed !!!!
            {
                double[,] temp_Pi = Pi.Pi_Poly_Calc(th, itemBank, model, D).Pi;  // Already returns a NA free list of values.

                // !! Optimize !!
                List<double> probs = new List<double>();

                for(int i = 0; i < temp_Pi.GetLength(1); i++)
                {
                    probs.Add(temp_Pi[0,i]);
                }

                // !!!! Add new method for polytomous items !!!!
                CATItems it_new = it_given;

                double[] th_new = new double[probs.Count];
                double[] se_new = new double[probs.Count];
                List<int> temp_x = new List<int>();

                for(int j = 0; j < probs.Count; j++)
                {
                    temp_x = x.ToList();
                    temp_x.Add(j);

                    th_new[j] = EapEST.EapEST_Calc(it_new, temp_x.ToArray(), "", priorPar, D, priorDist, parInt[0], parInt[1], parInt[2]);

                    se_new[j] = Math.Pow(EapSEM.EapSEM_Calc(th_new[j], it_new, temp_x.ToArray(), "", priorPar, D, priorDist, parInt[0], parInt[1], parInt[2]), 2);
                }

                res = CatRcs.Utils.RowColumn.Sum(probs.Select((p, s) => p * se_new[s]).ToArray());
            }

            return res;
        }
    }
}
