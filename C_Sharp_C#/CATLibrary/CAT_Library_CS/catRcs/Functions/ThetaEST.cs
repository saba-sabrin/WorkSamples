using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatRcs.Models;

namespace CatRcs
{
    internal class ThetaEST
    {
        public static double ThetaEst_Calc(CATItems it, int[] x, string model = null, double[] priorPar = null, double[] range = null, int[] parInt = null, double D = 1, string method = "BM", string priorDist = "norm")
        {
            double result = 0; double[] RANGE = null;

            #region "Parameter Validation"

            if (priorPar == null || priorPar.Length < 2)
            {
                priorPar = new double[2];
                priorPar[0] = 0;
                priorPar[1] = 1;
            }

            if (range == null || range.Length < 2)
            {
                range = new double[2];
                range[0] = -4;
                range[1] = 4;
            }

            if (parInt == null || parInt.Length < 3)
            {
                parInt = new int[3];
                parInt[0] = -4;
                parInt[1] = 4;
                parInt[2] = 33;
            }

            #endregion

            if (method == ModelNames.EstimaatorMethods.EAP.EnumToString())
            {
                result = EapEST.EapEST_Calc(it, x, model, priorPar, D, priorDist, parInt[0], parInt[1], parInt[2]);
            }
            else
            {
                if (String.IsNullOrEmpty(model))   // Dichotomous Items
                {
                    #region "Function 'r0' "
                    Func<double, string, double> r0 = (th, met) =>
                    {
                        double res = 0;

                        if (met == ModelNames.EstimaatorMethods.BM.EnumToString())
                        {
                            switch (priorDist)
                            {
                                case "norm":
                                    res = (priorPar[0] - th) / Math.Pow(priorPar[1], 2);
                                    break;
                                case "unif":
                                    res = 0;
                                    break;
                                case "Jeffreys":
                                    IiList objIi = Ii.Ii_Calc(th, it, model, D);
                                    res = CatRcs.Utils.RowColumn.Sum(objIi.dIi) / (2 * CatRcs.Utils.RowColumn.Sum(objIi.Ii));
                                    break;
                            }
                        }
                        else
                        {
                            ModelNames.EstimaatorMethods md = ModelNames.StringToEnumMethods(met);

                            switch (md)
                            {
                                case ModelNames.EstimaatorMethods.ML:
                                    res = 0;
                                    break;
                                case ModelNames.EstimaatorMethods.WL:
                                    res = CatRcs.Utils.RowColumn.Sum(Ji.Ji_Calc(th, it, model, D).Ji) / (2 * CatRcs.Utils.RowColumn.Sum(Ii.Ii_Calc(th, it, model, D).Ii));
                                    break;
                            }
                        }

                        return res;
                    };
                    #endregion

                    #region "Function 'r' "
                    Func<double, double> r = (th) =>
                    {
                        double res = 0;
                        double[] Q = new double[it.NumOfItems], x_p = new double[it.NumOfItems], p_q = new double[it.NumOfItems], temp = new double[it.NumOfItems];

                        double[] pi = Pi.Pi_Calc(th, it, model, D).Pi;
                        double[] dpi = Pi.Pi_Calc(th, it, model, D).dPi;

                        for (int ind_p = 0; ind_p < pi.Length; ind_p++)
                        {
                            Q[ind_p] = 1 - pi[ind_p];
                            x_p[ind_p] = x[ind_p] - pi[ind_p];
                            p_q[ind_p] = pi[ind_p] * Q[ind_p];
                            temp[ind_p] = dpi[ind_p] * (x_p[ind_p] / p_q[ind_p]);
                        }

                        res = temp.Sum();

                        return res;
                    };
                    #endregion

                    #region "Function 'f' "
                    Func<double, double> f = (th) =>
                    {
                        double res = 0;
                        return res;
                    };
                   

                    if(method == ModelNames.EstimaatorMethods.BM.EnumToString() && priorDist == "unif")
                    {
                        f = (th) =>
                        {
                            double res = 0;

                            string methodName = ModelNames.EstimaatorMethods.ML.EnumToString();

                            res = r0(th, methodName) + r(th);

                            return res;
                        };
                    }
                    else
                    {
                        f = (th) =>
                        {
                            double res = 0;

                            res = r0(th, method) + r(th);

                            return res;
                        };
                    }

                    #endregion

                    if (method == ModelNames.EstimaatorMethods.BM.EnumToString() && priorDist == "unif")
                    {
                        RANGE = priorPar;
                    }
                    else
                    {
                        RANGE = range;
                    }

                    if(f(RANGE[0]) < 0 && f(RANGE[1]) < 0)
                    {
                        result = RANGE[0];
                    }
                    else
                    {
                        if (f(RANGE[0]) > 0 && f(RANGE[1]) > 0)
                        {
                            result = RANGE[1];
                        }
                        else
                        {
                            result = CatRcs.Utils.Maths.UniRoot(f, RANGE).root;
                        }
                    }
                }
                else   // Polytomous Items
                {
                    int met = 0, pd = 0;
                    ModelNames.EstimaatorMethods md = ModelNames.StringToEnumMethods(method);

                    #region "Conditional Block"

                    switch (md)
                    {
                        case ModelNames.EstimaatorMethods.ML:
                            met = 1;
                            break;
                        case ModelNames.EstimaatorMethods.BM:
                            met = 2;
                            break;
                        case ModelNames.EstimaatorMethods.WL:
                            met = 3;
                            break;
                        case ModelNames.EstimaatorMethods.EAP:
                            met = 4;
                            break;
                    }

                    switch (priorDist)
                    {
                        case "norm":
                            pd = 1;
                            break;
                        case "unif":
                            pd = 2;
                            break;
                        case "Jeffreys":
                            pd = 3;
                            break;
                    }

                    if (met == 2 && pd == 2)
                    {
                        RANGE = new double[priorPar.Length];
                        RANGE[0] = Math.Max(priorPar[0], range[0]);
                        RANGE[1] = Math.Min(priorPar[1], range[1]);
                    }
                    else
                    {
                        RANGE = range;
                    }

                    #endregion

                    #region "Function 'dl' "
                    Func<double, double> dl = (th) =>
                    {
                        double res = 0;

                        PiListPoly p = Pi.Pi_Poly_Calc(th, it, model, D);
                        double[,] pr = p.Pi;
                        double[,] dpr = p.dPi;

                        for (int i = 0; i < x.Length; i++ )
                        {
                            res = res + dpr[i, x[i]] / pr[i, x[i]];
                        }

                        return res;
                    };
                    #endregion

                    #region "Function 'f0' "
                    Func<double, double> f0 = (th) =>
                    {
                        double res = 0;

                         if (met == 2)
                         {
                             switch (pd.ToString())
                             {
                                 case "1":
                                     res = (priorPar[0] -th) / Math.Pow(priorPar[1],2);
                                     break;
                                 case "2":
                                     res = 0;
                                     break;
                                 case "3":
                                     IiList objIi = CatRcs.Ii.Ii_Calc(th, it, model, D);
                                     res = CatRcs.Utils.RowColumn.Sum(objIi.dIi) / (2 * CatRcs.Utils.RowColumn.Sum(objIi.Ii));
                                     break;
                             }
                         }
                        else
                         {
                             switch (met.ToString())
                             {
                                 case "1":
                                     res = 0;
                                     break;
                                 case "2":
                                     res = 0;
                                     break;
                                 case "3":
                                     res = CatRcs.Utils.RowColumn.Sum(CatRcs.Ji.Ji_Calc(th, it, model, D).Ji) / (2 * CatRcs.Utils.RowColumn.Sum(CatRcs.Ii.Ii_Calc(th, it, model, D).Ii));
                                     break;
                             }
                         }

                        return res;
                    };
                    #endregion

                    #region "Function 'optF' "
                    Func<double, double> optF = (th) =>
                    {
                        double res = 0;
                        res = dl(th) + f0(th);
                        return res;
                    };
                    #endregion

                    if (optF(RANGE[0]) < 0 && optF(RANGE[1]) < 0)
                    {
                        result = RANGE[0];
                    }
                    else
                    {
                        if (optF(RANGE[0]) > 0 && optF(RANGE[1]) > 0)
                        {
                            result = RANGE[1];
                        }
                        else
                        {
                            result = CatRcs.Utils.Maths.UniRoot(optF, RANGE).root;
                        }
                    }
                }
            }

            return result;
        }
    }
}
