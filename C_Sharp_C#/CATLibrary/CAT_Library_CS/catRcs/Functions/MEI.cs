using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatRcs.Models;

namespace CatRcs
{
    internal class MEI
    {
        public static double MEI_Calc(CATItems itemBank, int item, int[] x, double theta, CATItems it_given, string model = null, string method = "BM", double D = 1, double[] priorPar = null, string priorDist = "norm", double[] range = null, int[] parInt = null, int infoType = 2 /* Observed */)
        {
            double res = 0;

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

            if (range == null || range.Length < 2)
            {
                range = new double[2];
                range[0] = -4;
                range[1] = 4;
            }

            if ((infoType == (int)ModelNames.InfoType.Observed) || (infoType == (int)ModelNames.InfoType.Fisher))  // type is either "Observed" or "Fisher"
            {
                double th = theta;

                // Finding desired "item" from itembank & Added to given items "it_given".
                // Instead of "itj", "it_given" will be used.
                itemBank.AddItem(it_given, item);

                if(String.IsNullOrEmpty(model))   // Dichotomous Items
                {
                    List<int> temp_x = x.ToList();
                    temp_x.Add(0);

                    double th0 = CatRcs.ThetaEST.ThetaEst_Calc(it_given, temp_x.ToArray(), D: D, model: model, method: method, priorPar: priorPar, priorDist: priorDist, parInt: parInt, range: range);

                    temp_x = x.ToList();
                    temp_x.Add(1);

                    double th1 = CatRcs.ThetaEST.ThetaEst_Calc(it_given, temp_x.ToArray(), D: D, model: model, method: method, priorPar: priorPar, priorDist: priorDist, parInt: parInt, range: range);

                    double p1 = CatRcs.Pi.Pi_Calc(th, itemBank.FindItem(item), model, D).Pi.First();
                    double p0 = 1 - p1;
                    double Ij0 = 0, Ij1 = 0;

                    if ((infoType == (int)ModelNames.InfoType.Fisher))
                    {
                       Ij0 = CatRcs.Ii.Ii_Calc(th0, itemBank.FindItem(item), model, D).Ii.First();
                       Ij1 = CatRcs.Ii.Ii_Calc(th1, itemBank.FindItem(item), model, D).Ii.First();
                    }
                    else
                    {
                       Ij0 = CatRcs.OIi.OIi_Calc(th0, itemBank.FindItem(item), new int[]{ 0 }, model, D).OIi.First();
                       Ij1 = CatRcs.OIi.OIi_Calc(th1, itemBank.FindItem(item), new int[]{ 1 }, model, D).OIi.First();
                    }

                    res = (p0 * Ij0) + (p1 * Ij1);
                }
                else   // Polytomous Items
                {
                    double[,] probs = Pi.Pi_Poly_Calc(theta, itemBank.FindItem(item), model, D).Pi;
                    double[] Ii_new = new double[probs.GetLength(1)];

                    for(int j = 0; j < probs.GetLength(1); j++)  // column iteration
                    {
                        List<int> temp_x = x.ToList();
                        temp_x.Add(j);

                        double th_new = CatRcs.ThetaEST.ThetaEst_Calc(it_given, temp_x.ToArray(), D: D, model: model, method: method, priorPar: priorPar, priorDist: priorDist, parInt: parInt, range: range);

                        if ((infoType == (int)ModelNames.InfoType.Fisher))
                        {
                            Ii_new[j] = CatRcs.Ii.Ii_Calc(th_new, itemBank.FindItem(item), model, D).Ii.First();
                        }
                        else
                        {
                            Ii_new[j] = CatRcs.OIi.OIi_Calc(th_new, itemBank.FindItem(item), new int[]{ j }, model, D).OIi.First();
                        }
                    }

                     res = CatRcs.Utils.RowColumn.Sum(Ii_new.ToList().Select((p, i) => p * probs[0, i]).ToArray());
                }
            }
            else
            {
                return res;
            }

            return res;
        }
    }
}
