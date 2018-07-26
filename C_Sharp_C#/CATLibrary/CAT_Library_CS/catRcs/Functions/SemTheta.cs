using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatRcs.Models;

namespace CatRcs
{
    internal class SemTheta
    {
        public static double SemTheta_Calc(double thEst, CATItems it, int[] x = null, string model = null, double[] priorPar = null, int[] parInt = null, double D = 1, string method = "BM", string priorDist = "norm")
        {
            double result = 0;

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

            if(method == ModelNames.EstimaatorMethods.EAP.EnumToString())
            {
                result = EapSEM.EapSEM_Calc(thEst, it, x, model, priorPar, D, priorDist, parInt[0], parInt[1], parInt[2]);
            }
            else
            {
                if (String.IsNullOrEmpty(model))   // Dichotomous Items
                {
                    #region "Function 'dr0' "
                    Func<double> dr0 = () =>
                    {
                        double res = 0;

                        if(method == ModelNames.EstimaatorMethods.BM.EnumToString())
                        {
                            switch (priorDist)
                            {
                                case "norm":
                                    res = -1 / (Math.Pow(priorPar[1], 2));
                                    break;
                                case "unif":
                                    res = 0;
                                    break;
                                case "Jeffreys":
                                    IiList objIi = Ii.Ii_Calc(thEst, it, model, D);
                                    res = (CatRcs.Utils.RowColumn.Sum(objIi.d2Ii) * CatRcs.Utils.RowColumn.Sum(objIi.Ii) 
                                        - Math.Pow(CatRcs.Utils.RowColumn.Sum(objIi.dIi), 2)) / (2 * Math.Pow(CatRcs.Utils.RowColumn.Sum(objIi.Ii) , 2));
                                    break;
                            }
                        }
                        else
                        {
                            if(method == ModelNames.EstimaatorMethods.ML.EnumToString() || method == ModelNames.EstimaatorMethods.WL.EnumToString())
                            {
                                res = 0;
                            }
                        }

                        return res;
                    };
                    #endregion

                    result = 1 / Math.Sqrt(-dr0() + CatRcs.Utils.RowColumn.Sum(Ii.Ii_Calc(thEst, it, model, D).Ii));
                }
                else   // Polytomous Items
                {
                    double met = 0, pd = 0, optI = 0;
                    ModelNames.EstimaatorMethods md = ModelNames.StringToEnumMethods(method);

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

                    if(met == 1 || (met == 2 && pd == 2))
                    {
                        optI = CatRcs.Utils.RowColumn.Sum(Ii.Ii_Calc(thEst, it, model, D).Ii);
                    }

                    if (met == 2 && pd == 1)
                    {
                        optI = CatRcs.Utils.RowColumn.Sum(Ii.Ii_Calc(thEst, it, model, D).Ii) + (1 / Math.Pow(priorPar[1], 2));
                    }

                    if (met == 3 || (met == 2 && pd == 3))
                    {
                        IiList objIi = Ii.Ii_Calc(thEst, it, model, D);

                        if (met == 2)
                        {   
                           optI = CatRcs.Utils.RowColumn.Sum(objIi.Ii) + (Math.Pow(CatRcs.Utils.RowColumn.Sum(objIi.dIi), 2) - CatRcs.Utils.RowColumn.Sum(objIi.d2Ii) *
                           CatRcs.Utils.RowColumn.Sum(objIi.Ii)) / (2 * Math.Pow(CatRcs.Utils.RowColumn.Sum(objIi.Ii), 2));
                        }
                        else
                        {
                            optI = CatRcs.Utils.RowColumn.Sum(objIi.Ii);
                        }
                    }

                    result = 1 / Math.Sqrt(optI);
                }
            }

            return result;
        }
    }
}
