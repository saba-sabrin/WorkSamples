using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatRcs.Models;
using CatRcs.Utils;

namespace CatRcs
{
    internal class Ii
    {
        public static IiList Ii_Calc(double th, CATItems it, string model, double D)
        {
            IiList objIi = null;           
            double[] Ii = null; double[] dIi = null; double[] d2Ii = null;

            try
            {
                if (String.IsNullOrEmpty(model))
                {
                   #region "Calculation for Dichotmous Items"

                   double[] P = null; double[] dP = null; double[] d2P = null; double[] d3P = null;

                   PiList pr = Pi.Pi_Calc(th, it, model, D);  // Check return value of Pi

                   if (pr == null)
                   {
                       objIi = new IiList();
                       objIi.Exception = "No Pi values found!";
                       return objIi;
                   }

                   P = pr.Pi;
                   dP = pr.dPi;
                   d2P = pr.d2Pi;
                   d3P = pr.d3Pi;

                   Ii = new double[dP.Length];
                   dIi = new double[d2P.Length];
                   d2Ii = new double[d3P.Length];

                   objIi = new IiList(P.Length);

                   double[] Q = new double[P.Length];

                   for (int i = 0; i < P.Length; i++)
                   {
                       Q[i] = 1 - P[i];
                   }

                   for (int j = 0; j < Ii.Length; j++)
                   {
                       Ii[j] = Math.Pow(dP[j], 2) / (P[j] * Q[j]);
                   }

                   for (int k = 0; k < dIi.Length; k++)
                   {
                       dIi[k] = dP[k] * (2 * P[k] * Q[k] * d2P[k] - Math.Pow(dP[k], 2) * (Q[k] - P[k])) / (Math.Pow(P[k], 2) * Math.Pow(Q[k], 2));
                   }

                   for (int l = 0; l < d2Ii.Length; l++)
                   {
                       d2Ii[l] = (2 * P[l] * Q[l] * (Math.Pow(d2P[l], 2) + dP[l] * d3P[l]) - 2 * Math.Pow(dP[l], 2) * d2P[l] * (Q[l] - P[l])) / (Math.Pow(P[l], 2) * Math.Pow(Q[l], 2)) -
                           (3 * Math.Pow(P[l], 2) * Q[l] * Math.Pow(dP[l], 2) * d2P[l] - P[l] * Math.Pow(dP[l], 4) * (2 * Q[l] - P[l])) / (Math.Pow(P[l], 4) * Math.Pow(Q[l], 2)) +
                           (3 * P[l] * Math.Pow(Q[l], 2) * Math.Pow(dP[l], 2) * d2P[l] - Q[l] * Math.Pow(dP[l], 4) * (Q[l] - 2 * P[l])) / (Math.Pow(P[l], 2) * Math.Pow(Q[l], 4));
                   }

                   #endregion
                }
                else
                {
                   #region "Calculation for Polytomous Items"

                    double[,] P = null; double[,] dP = null; double[,] d2P = null; double[,] d3P = null;

                    PiListPoly pr = Pi.Pi_Poly_Calc(th, it, model, D);

                    if (pr == null)
                    {
                        objIi = new IiList();
                        objIi.Exception = "No Pi values found!";
                        return objIi;
                    }

                    P = pr.Pi;
                    dP = pr.dPi;
                    d2P = pr.d2Pi;
                    d3P = pr.d3Pi;

                    int rowLength = pr.Pi.GetLength(0);
                    int columnLength = pr.Pi.GetLength(1);

                    objIi = new IiList(rowLength);

                    double[,] pr0 = new double[rowLength, columnLength];
                    double[,] pr1 = new double[rowLength, columnLength];
                    double[,] pr2 = new double[rowLength, columnLength];

                    Ii = new double[rowLength];
                    dIi = new double[rowLength];
                    d2Ii = new double[rowLength];

                    for (int i = 0; i < rowLength; i++)
                    {
                        for (int j = 0; j < columnLength; j++)
                        {
                            pr0[i, j] = Math.Pow(dP[i, j], 2) / P[i, j];

                            pr1[i, j] = 2 * dP[i, j] * (d2P[i, j] / P[i, j]) - (Math.Pow(dP[i, j], 3) / Math.Pow(P[i, j], 2));

                            pr2[i, j] = (2 * Math.Pow(d2P[i, j], 2) + 2 * dP[i, j] * d3P[i, j]) / P[i, j] - 2 * Math.Pow(dP[i, j], 2) * (d2P[i, j] / -3) * dP[i, j] * (d2P[i, j] / Math.Pow(P[i, j], 2)) + 2 * (Math.Pow(dP[i, j], 4) / Math.Pow(P[i, j], 3));
                        }
                    }
                    
                    Ii = RowColumn.rowSums(pr0);
                    dIi = RowColumn.rowSums(pr1);
                    d2Ii = RowColumn.rowSums(pr2);

                    #endregion
                }

                objIi.Add(Ii, dIi, d2Ii);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    if (objIi == null)
                    {
                        objIi = new IiList();
                    }
                    objIi.Exception = ex.Message;
                }
                return objIi;
            }

            return objIi;
        }
    }
}
