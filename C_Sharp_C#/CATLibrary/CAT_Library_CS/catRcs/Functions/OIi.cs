using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatRcs.Models;

namespace CatRcs
{
    internal class OIi
    {
        public static OIiList OIi_Calc(double th, CATItems it, int[] x, string model, double D)
        {
            OIiList objOIi = null;

            double[] OIi = null;
            try
            {
                if (String.IsNullOrEmpty(model))
                {
                    #region "Calculation for Dichotmous Items"

                    PiList pr = Pi.Pi_Calc(th, it, model, D);

                    if (pr == null)
                    {
                        objOIi = new OIiList();
                        objOIi.Exception = "No Pi values found!";
                        return objOIi;
                    }

                    double[] P = pr.Pi; double[] dP = pr.dPi; double[] d2P = pr.d2Pi;

                    OIi = new double[P.Length];

                    objOIi = new OIiList(P.Length);

                    double[] Q = new double[P.Length];

                    for (int i = 0; i < P.Length; i++)
                    {
                        Q[i] = 1 - P[i];
                    }

                    for (int j = 0; j < OIi.Length; j++)
                    {
                        OIi[j] = (P[j] * Q[j] * Math.Pow(dP[j], 2) - (x[0] - P[j]) * (P[j] * Q[j] * d2P[j] + Math.Pow(dP[j], 2) * (P[j] - Q[j]))) / (Math.Pow(P[j], 2) * Math.Pow(Q[j], 2));
                    }

                    #endregion
                }
                else
                {
                    #region "Calculation for Polytomous Items"

                    PiListPoly pr = Pi.Pi_Poly_Calc(th, it, model, D);

                    if (pr == null)
                    {
                        objOIi = new OIiList();
                        objOIi.Exception = "No Pi values found!";
                        return objOIi;
                    }

                    double[,] P = pr.Pi; double[,] dP = pr.dPi; double[,] d2P = pr.d2Pi;

                    OIi = new double[it.NumOfItems];

                    objOIi = new OIiList(it.NumOfItems);

                    for (int i = 0; i < x.Length; i++)
                    {
                        double tempdP = dP[i, x[i]];
                        double tempP = P[i, x[i]];
                        double tempd2P = d2P[i, x[i]];

                        OIi[i] = (Math.Pow(dP[i, x[i]], 2) / Math.Pow(P[i, x[i]], 2)) - (d2P[i, x[i]] / P[i, x[i]]);
                    }

                    #endregion
                }

                objOIi.Add(OIi);

            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    if (objOIi == null)
                    {
                        objOIi = new OIiList();
                    }
                    objOIi.Exception = "No Pi value found!";
                }
                return objOIi;
            }

            return objOIi;
        }
    }
}
