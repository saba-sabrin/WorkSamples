using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatRcs.Models;

namespace CatRcs
{
    internal class KL
    {
        // Optimization needed !!!
        public static double KL_Calc(CATItems itemBank, int item, int[] x, CATItems it_given, string model = null, double theta = 0, double[] priorPar = null, double[] X = null, double[] lik = null, int type = 1, double D = 1, string priorDist = "norm", double lower = -4, double upper = 4, int nqp = 33)
        {
            double res = 0;

            if((type == (int)ModelNames.KLTypes.KL) || (type == (int)ModelNames.KLTypes.KLP))  // type is either "KL" or "KLP"
            {
                if(X != null && lik != null)
                {
                    if (X.Length > 0 && lik.Length > 0)
                    {
                        if (X.Length != lik.Length)
                        {
                            return res; // Error handling
                        }
                    }
                }

                if(theta == 0)
                {
                    theta = CatRcs.ThetaEST.ThetaEst_Calc(it_given, x, D: D, model: model, method: CatRcs.Models.ModelNames.EstimaatorMethods.ML.EnumToString());
                }

                double[] KLF = new double[nqp];
                double[] crit_value = new double[nqp];

                CATItems par = itemBank.FindItem(item);

                if (X == null)
                {
                    X = CatRcs.Utils.CommonHelper.Sequence(lower, upper, nqp);
                }
                
                if(String.IsNullOrEmpty(model))   // Dichotomous Items
                {
                    if(lik == null)
                    {
                        #region "Function 'L' "
                        Func<double, int[], CATItems, double> L = (th, r, param) =>
                        {
                            double result = 0;

                            var temp_1 = Pi.Pi_Calc(th, param, model, D).Pi.Select((p, i) => Math.Pow(p, r[i])).ToArray();

                            var temp_2 = Pi.Pi_Calc(th, param, model, D).Pi.Select((p, i) => Math.Pow(1 - p, 1 - r[i])).ToArray();

                            if (temp_1.Length == temp_2.Length)
                            {
                                var temp_3 = temp_1.Select((p, i) => p * temp_2[i]).ToArray();

                                result = temp_3.Aggregate((acc, val) => acc * val);
                            }

                            return result;
                        };
                        #endregion

                        lik = X.ToList().Select(a => L(a, x, it_given)).ToArray();
                    }

                    for(int j = 0; j < KLF.Length; j++)
                    {
                        KLF[j] = Pi.Pi_Calc(theta, par, model, D).Pi[0] * Math.Log(Pi.Pi_Calc(theta, par, model, D).Pi[0] / Pi.Pi_Calc(X[j], par, model, D).Pi[0], Math.Exp(1))
                            + (1 - Pi.Pi_Calc(theta, par, model, D).Pi[0]) * Math.Log((1 - Pi.Pi_Calc(theta, par, model, D).Pi[0]) / (1 - Pi.Pi_Calc(X[j], par, model, D).Pi[0]), Math.Exp(1));
                    }

                    crit_value = KLF.ToList().Select((c, i) => c * lik[i]).ToArray();

                    if((type == (int)ModelNames.KLTypes.KLP))
                    {
                        double[] pd = null;

                        switch (priorDist)
                        {
                            case "norm":
                                pd = CatRcs.Utils.CommonHelper.Dnorm(X, priorPar[0], priorPar[1]);
                                break;
                            case "unif":
                                pd = CatRcs.Utils.CommonHelper.Dunif(X, priorPar[0], priorPar[1]);
                                break;
                        }

                        crit_value = crit_value.ToList().Select((c, i) => c * pd[i]).ToArray();
                    }
                }
                else   // Polytomous Items
                {
                    if (lik == null)
                    {
                        #region "Function 'LL' "
                        Func<double, CATItems, int[], double> LL = (th, param, r) =>
                        {
                            double result = 1;

                            double[,] prob = Pi.Pi_Poly_Calc(th, param, model, D).Pi;

                            for (int i = 0; i < r.Length; i++)
                            {
                                result = result * prob[i, r[i]];
                            }

                            return result;
                        };
                        #endregion

                        lik = X.ToList().Select(a => LL(a, it_given, x)).ToArray();
                    }

                    double[,] pi = Pi.Pi_Poly_Calc(theta, par, model, D).Pi;

                    for (int i = 0; i < X.Length; i++ )
                    {
                        double[,] pri = Pi.Pi_Poly_Calc(X[i], par, model, D).Pi;
                        double[] tempPi = new double[pi.Length];

                        for (int j = 0; j < pi.Length; j++)
                        {
                            tempPi[j] = pi[0, j] * Math.Log(pi[0, j] / pri[0, j]);
                        }

                        KLF[i] = CatRcs.Utils.RowColumn.Sum(tempPi);
                    }

                    crit_value = KLF.ToList().Select((c, i) => c * lik[i]).ToArray();

                    if ((type == (int)ModelNames.KLTypes.KLP))
                    {
                        double[] pd = null;

                        switch (priorDist)
                        {
                            case "norm":
                                pd = CatRcs.Utils.CommonHelper.Dnorm(X, priorPar[0], priorPar[1]);
                                break;
                            case "unif":
                                pd = CatRcs.Utils.CommonHelper.Dunif(X, priorPar[0], priorPar[1]);
                                break;
                        }

                        crit_value = crit_value.ToList().Select((c, i) => c * pd[i]).ToArray();
                    }
                }

                res = CatRcs.Integrate_catR.Integrate_CatR_Calc(X, crit_value);
            }
            else
            {
                return res;
            }

            return res;
        }
    }
}
