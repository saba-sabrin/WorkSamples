using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatRcs.Models;

namespace CatRcs
{
    internal class EapSEM
    {
        internal static double EapSEM_Calc(double thEst, CATItems it, int[] x, string model, double[] priorPar = null, double D = 1, string priorDist = "norm", double lower = -4, double upper = 4, double nqp = 33)
        {
            double res = 0;
            double[] X = CatRcs.Utils.CommonHelper.Sequence(lower, upper, nqp);
            double[] Y1 = null; double[] Y2 = null;

            if (priorPar == null || priorPar.Length < 2)
            {
                priorPar = new double[2];
                priorPar[0] = 0;
                priorPar[1] = 1;
            }

            if (String.IsNullOrEmpty(model))   // Dichotomous Items
            {
                #region "Function 'L' "
                Func<double, CATItems, int[], double> L = (th, items, x_in) =>
                {
                    double result = 0;

                    double[] pi = Pi.Pi_Calc(th, it, model, D).Pi;

                    double[] x_p = new double[pi.Length]; double[] x_q = new double[pi.Length]; double[] p_q = new double[pi.Length];

                    for (int ind_p = 0; ind_p < pi.Length; ind_p++)
                    {
                        x_p[ind_p] = Math.Pow(pi[ind_p], x_in[ind_p]); ;
                        x_q[ind_p] = Math.Pow(1 - pi[ind_p], 1 - x_in[ind_p]);
                        p_q[ind_p] = x_p[ind_p] * x_q[ind_p];
                    }

                    result = p_q.Aggregate((acc, val) => acc * val);

                    return result;
                };
                #endregion

                #region "Function 'g' "
                Func<double[], double[]> g = (s) =>
                {
                    double[] resList = new double[s.Length];

                    for (int i = 0; i < s.Length; i++)
                    {
                        switch (priorDist)
                        {
                            case "norm":
                                resList[i] = Math.Pow(s[i] - thEst, 2) * CatRcs.Utils.CommonHelper.Dnorm(s[i], priorPar[0], priorPar[1]) * L(s[i], it, x);
                                break;
                            case "unif":
                                resList[i] = Math.Pow(s[i] - thEst, 2) * CatRcs.Utils.CommonHelper.Dunif(s[i], priorPar[0], priorPar[1]) * L(s[i], it, x);
                                break;
                            case "Jeffreys":
                                resList[i] = Math.Pow(s[i] - thEst, 2) * Math.Sqrt(CatRcs.Utils.RowColumn.Sum(Ii.Ii_Calc(s[i], it, model, D).Ii)) * L(s[i], it, x);
                                break;
                        }
                    }

                    return resList;
                };
                #endregion

                #region "Function 'h' "
                Func<double[], double[]> h = (s) =>
                {
                    double[] resList = new double[s.Length];

                    for (int i = 0; i < s.Length; i++)
                    {
                        switch (priorDist)
                        {
                            case "norm":
                                resList[i] = CatRcs.Utils.CommonHelper.Dnorm(s[i], priorPar[0], priorPar[1]) * L(s[i], it, x);
                                break;
                            case "unif":
                                resList[i] = CatRcs.Utils.CommonHelper.Dunif(s[i], priorPar[0], priorPar[1]) * L(s[i], it, x);
                                break;
                            case "Jeffreys":
                                resList[i] = Math.Sqrt(CatRcs.Utils.RowColumn.Sum(Ii.Ii_Calc(s[i], it, model, D).Ii)) * L(s[i], it, x);
                                break;
                        }
                    }

                    return resList;
                };
                #endregion
   
                Y1 = g(X);
                Y2 = h(X);
            }
            else   // Polytomous Items
            {
                #region "Function 'LL' "
                Func<double, CATItems, int[], double> LL = (th, items, x_in) =>
                {
                    double result = 1;

                    double[,] prob = Pi.Pi_Poly_Calc(th, it, model, D).Pi;

                    for (int i = 0; i < x_in.Length; i++)
                    {
                        result = result * prob[i, x_in[i]];
                    }

                    return result;
                };
                #endregion

                #region "Function 'gg' "
                Func<double[], double[]> gg = (s) =>
                {
                    double[] resList = new double[s.Length];

                    for (int i = 0; i < s.Length; i++)
                    {
                        switch (priorDist)
                        {
                            case "norm":
                                resList[i] = Math.Pow(s[i] - thEst, 2) * CatRcs.Utils.CommonHelper.Dnorm(s[i], priorPar[0], priorPar[1]) * LL(s[i], it, x);
                                break;
                            case "unif":
                                resList[i] = Math.Pow(s[i] - thEst, 2) * CatRcs.Utils.CommonHelper.Dunif(s[i], priorPar[0], priorPar[1]) * LL(s[i], it, x);
                                break;
                            case "Jeffreys":
                                resList[i] = Math.Pow(s[i] - thEst, 2) * Math.Sqrt(CatRcs.Utils.RowColumn.Sum(Ii.Ii_Calc(s[i], it, model, D).Ii)) * LL(s[i], it, x);
                                break;
                        }
                    }

                    return resList;
                };
                #endregion

                #region "Function 'hh' "
                Func<double[], double[]> hh = (s) =>
                {
                    double[] resList = new double[s.Length];

                    for (int i = 0; i < s.Length; i++)
                    {
                        switch (priorDist)
                        {
                            case "norm":
                                resList[i] = CatRcs.Utils.CommonHelper.Dnorm(s[i], priorPar[0], priorPar[1]) * LL(s[i], it, x);
                                break;
                            case "unif":
                                resList[i] = CatRcs.Utils.CommonHelper.Dunif(s[i], priorPar[0], priorPar[1]) * LL(s[i], it, x);
                                break;
                            case "Jeffreys":
                                resList[i] = Math.Sqrt(CatRcs.Utils.RowColumn.Sum(Ii.Ii_Calc(s[i], it, model, D).Ii)) * LL(s[i], it, x);
                                break;
                        }
                    }

                    return resList;
                };
                #endregion

                Y1 = gg(X);
                Y2 = hh(X);
            }

            if ((Y1 != null) && (Y2 != null))
            {
                res = Math.Sqrt(CatRcs.Integrate_catR.Integrate_CatR_Calc(X, Y1) / CatRcs.Integrate_catR.Integrate_CatR_Calc(X, Y2));
            }

            return res;
        }
    }
}
