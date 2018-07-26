using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatRcs.Models;

namespace CatRcs
{
    internal class MWI
    {
        public static double MWI_Calc(CATItems itemBank, int item, int[] x, CATItems it_given, string model, int type = 1 /* MLWI */, double[] priorPar = null, double D = 1, string priorDist = "norm", int lower = -4, int upper = 4, int nqp = 33)
        {
            double res = 0;

            if (priorPar == null || priorPar.Length < 2)
            {
                priorPar = new double[2];
                priorPar[0] = 0;
                priorPar[1] = 1;
            }

            if (type != (int)ModelNames.MWI_Type.MLWI && type != (int)ModelNames.MWI_Type.MPWI)
            {
                return res;
            }
            else
            {
                double[] lik = null; double[] info = null; double[] crit_value = null;
                double[] X = CatRcs.Utils.CommonHelper.Sequence(lower, upper, nqp);

                if (String.IsNullOrEmpty(model))  // Dichotomous Items
                {
                    #region "Function 'L' "
                    Func<double, int[], CATItems, double> L = (th, x_in, items) =>
                    {
                        double result = 0;

                        var temp_1 = Pi.Pi_Calc(th, it_given, model, D).Pi.Select((p, i) => Math.Pow(p, x_in[i])).ToArray();

                        var temp_2 = Pi.Pi_Calc(th, it_given, model, D).Pi.Select((p, i) => Math.Pow(1 - p, 1 - x_in[i])).ToArray();

                        if (temp_1.Length == temp_2.Length)
                        {
                            var temp_3 = temp_1.Select((p, i) => p * temp_2[i]).ToArray();

                            result = temp_3.Aggregate((acc, val) => acc * val);
                        }

                        return result;
                    };
                    #endregion

                    lik = X.Select((a, i) => L(a, x, it_given)).ToArray();   // "sapply" function shortcut using LINQ

                    info = X.Select(b => Ii.Ii_Calc(b, itemBank, model, D).Ii[item - 1]).ToArray(); 

                    crit_value = lik.Select((c, j) => c * info[j]).ToArray();
                }
                else   // Polytomous Items
                {
                    #region "Function 'LL' "
                    Func<double, CATItems, int[], double> LL = (th, items, x_in) =>
                    {
                        double result = 0;
                        
                        if(it_given.colSize == 0)
                        {
                            result = 1;
                        }
                        else
                        {
                            result = 1;
                            double[,] prob = Pi.Pi_Poly_Calc(th, it_given, model, D).Pi;

                            for (int i = 0; i < x_in.Length; i++)
                            {
                                res = res * prob[i, x[i]];
                            }
                        }

                        return result;
                    };
                    #endregion

                    lik = X.Select((a, i) => LL(a, it_given, x)).ToArray();

                    CATItems temp_ItemBank = itemBank.FindItem(item);

                    info = X.Select(b => CatRcs.Utils.RowColumn.Sum(Ii.Ii_Calc(b, temp_ItemBank, model, D).Ii)).ToArray();

                    crit_value = lik.Select((c, j) => c * info[j]).ToArray();
                }

                if (lik.Length > 0 && info.Length > 0)
                {
                    if (type == (int)ModelNames.MWI_Type.MPWI)
                    {
                        double[] pd = new double[X.Length];

                        for (int i = 0; i < X.Length; i++)
                        {
                            switch (priorDist)
                            {
                                case "norm":
                                    pd[i] = CatRcs.Utils.CommonHelper.Dnorm(X[i], priorPar[0], priorPar[1]);
                                    break;
                                case "unif":
                                    pd[i] = CatRcs.Utils.CommonHelper.Dunif(X[i], priorPar[0], priorPar[1]);
                                    break;
                            }
                        }

                        crit_value = crit_value.Select((d, k) => d * pd[k]).ToArray();
                    }

                    res = CatRcs.Integrate_catR.Integrate_CatR_Calc(X, crit_value);
                }
            }
            return res;
        }
    }
}
