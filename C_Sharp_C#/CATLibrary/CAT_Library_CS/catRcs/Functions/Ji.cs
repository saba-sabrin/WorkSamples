using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatRcs.Models;
using CatRcs.Utils;

namespace CatRcs
{
    internal class Ji
    {
        public static JiList Ji_Calc(double th, CATItems it, string model, double D)
        {
            JiList objJi = null;
            double[] Ji = null; double[] dJi = null;

            try
            {
                if (String.IsNullOrEmpty(model))
                {
                    #region "Calculation for Dichotmous Items"

                    PiList pr = Pi.Pi_Calc(th, it, model, D);

                    if (pr == null)
                    {
                        objJi = new JiList();
                        objJi.Exception = "No Pi values found!";
                        return objJi;
                    }

                    double[] P = pr.Pi; double[] dP = pr.dPi; double[] d2P = pr.d2Pi; double[] d3P = pr.d3Pi;

                    Ji = new double[dP.Length];
                    dJi = new double[d2P.Length];

                    objJi = new JiList(P.Length);

                    double[] Q = new double[P.Length];

                    for (int i = 0; i < P.Length; i++)
                    {
                        Q[i] = 1 - P[i];
                    }

                    for (int j = 0; j < Ji.Length; j++)
                    {
                        Ji[j] = dP[j] * d2P[j] / (P[j] * Q[j]);
                    }

                    for (int k = 0; k < dJi.Length; k++)
                    {
                        dJi[k] = (P[k] * Q[k] * (Math.Pow(d2P[k], 2) + dP[k] * d3P[k]) - Math.Pow(dP[k], 2) * d2P[k] * (Q[k] - P[k])) / (Math.Pow(P[k], 2) * Math.Pow(Q[k], 2));
                    }

                    #endregion
                }
                else
                {
                    #region "Calculation for Polytomous Items"

                    PiListPoly pr = Pi.Pi_Poly_Calc(th, it, model, D);

                    if (pr == null)
                    {
                        objJi = new JiList();
                        objJi.Exception = "No Pi values found!";
                        return objJi;
                    }

                    double[,] P = pr.Pi; double[,] dP = pr.dPi; double[,] d2P = pr.d2Pi; double[,] d3P = pr.d3Pi;

                    int rowLength = pr.Pi.GetLength(0);
                    int columnLength = pr.Pi.GetLength(1);

                    objJi = new JiList(rowLength);

                    double[,] prov = new double[rowLength, columnLength];
                    double[,] prov1 = new double[rowLength, columnLength];

                    Ji = new double[rowLength];
                    dJi = new double[rowLength];

                    for (int i = 0; i < rowLength; i++)
                    {
                        for (int j = 0; j < columnLength; j++)
                        {
                            prov[i, j] = dP[i, j] * d2P[i, j] / P[i, j];

                            prov1[i, j] = (P[i, j] * Math.Pow(d2P[i, j], 2) + P[i, j] * dP[i, j] * d3P[i, j] - Math.Pow(dP[i, j], 2) * d2P[i, j]) / Math.Pow(P[i, j], 2);
                        }
                    }

                    Ji = RowColumn.rowSums(prov);
                    dJi = RowColumn.rowSums(prov1);

                    #endregion
                }

                objJi.Add(Ji, dJi);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    if (objJi == null)
                    {
                        objJi = new JiList();
                    }
                    objJi.Exception = ex.Message;
                }
                return objJi;
            }

            return objJi;
        }

    }
}
