using System;
using System.Collections.Generic;
using CatRcs.Models;

namespace CatRcs
{
    public static class CatRLib
    {
        #region "Function Calls"
        // Function "Pi" for Dichotmous items
        public static PiList Pi_D(double th, CATItems it, string model, double D)
        {
            PiList objPi = CatRcs.Pi.Pi_Calc(th, it, model, D);
            return objPi;
        }

        // Function "Pi" for Polytomous items
        public static PiListPoly Pi_P(double th, CATItems it, string model, double D)
        {
            PiListPoly objPi = CatRcs.Pi.Pi_Poly_Calc(th, it, model, D);
            return objPi;
        }

        // Function "Ii"
        public static IiList Ii(double th, CATItems it, string model, double D)
        {
            IiList objIi = CatRcs.Ii.Ii_Calc(th, it, model, D);
            return objIi;
        }

        // Function "Ji"
        public static JiList Ji(double th, CATItems it, string model, double D)
        {
            JiList objJi = CatRcs.Ji.Ji_Calc(th, it, model, D);
            return objJi;
        }

        // Function OIi
        public static OIiList OIi(double th, CATItems it, int[] x, string model, double D)
        {
            OIiList objOIi = CatRcs.OIi.OIi_Calc(th, it, x, model, D);
            return objOIi;
        }

        public static double Integrate_catR(double[] x, double[] y)
        {
            double result = CatRcs.Integrate_catR.Integrate_CatR_Calc(x, y);
            return result;
        }

        // Function KL
        public static double KL(CATItems itemBank, int item, int[] x, CATItems it_given, string model = null, double theta = 0, double[] priorPar = null, double[] X = null, double[] lik = null, int type = 1, double D = 1, string priorDist = "norm", double lower = -4, double upper = 4, int nqp = 33)
        {
            double result = CatRcs.KL.KL_Calc(itemBank, item, x, it_given, model, theta, priorPar, X, lik, type, D, priorDist, lower, upper, nqp);
            return result;
        }

        public static double MEI(CATItems itemBank, int item, int[] x, double theta, CATItems it_given, string model = null, string method = "BM", double D = 1, double[] priorPar = null, string priorDist = "norm", double[] range = null, int[] parInt = null, int infoType = 2)
        {
            double result = CatRcs.MEI.MEI_Calc(itemBank, item, x, theta, it_given, model, method, D, priorPar, priorDist, range, parInt, infoType);
            return result;
        }

        public static double MWI(CATItems itemBank, int item, int[] x, CATItems it_given, string model, int type = 1 /* MLWI */, double[] priorPar = null, double D = 1, string priorDist = "norm", int lower = -4, int upper = 4, int nqp = 33)
        {
            double result = CatRcs.MWI.MWI_Calc(itemBank, item, x, it_given, model, type, priorPar, D, priorDist, lower, upper, nqp);
            return result;
        }

        public static double EapEST(CATItems it, int[] x, string model, double[] priorPar = null, double D = 1, string priorDist = "norm", double lower = -4, double upper = 4, double nqp = 33)
        {
            double result = CatRcs.EapEST.EapEST_Calc(it, x, model, priorPar, D, priorDist, lower, upper, nqp);
            return result;
        }

        public static double EapSEM(double thEst, CATItems it, int[] x, string model, double[] priorPar = null, double D = 1, string priorDist = "norm", double lower = -4, double upper = 4, double nqp = 33)
        {
            double result = CatRcs.EapSEM.EapSEM_Calc(thEst, it, x, model, priorPar, D, priorDist, lower, upper, nqp);
            return result;
        }

        public static double EPV(CATItems itemBank, int item, int[] x, double theta, CATItems it_given, string model, double[] priorPar = null, int[] parInt = null, double D = 1, string priorDist = "norm")
        {
            double result = CatRcs.EPV.EPV_Calc(itemBank, item, x, theta, it_given, model, priorPar, parInt, D, priorDist);
            return result;
        }

        public static double SemTheta(double thEst, CATItems it, int[] x = null, string model = null, double[] priorPar = null, int[] parInt = null, double D = 1, string method = "BM", string priorDist = "norm")
        {
            double result = CatRcs.SemTheta.SemTheta_Calc(thEst, it, x, model, priorPar, parInt, D, method, priorDist);
            return result;
        }

        public static double ThetaEst(CATItems it, int[] x, string model = null, double[] priorPar = null, double[] range = null, int[] parInt = null, double D = 1, string method = "BM", string priorDist = "norm")
        {
            double result = CatRcs.ThetaEST.ThetaEst_Calc(it, x, model, priorPar, range, parInt, D, method, priorDist);
            return result;
        }

        public static StartItemsModel StartItems(CATItems itemBank, string model = null, int[] fixItems = null, int? seed = null, int nrItems = 1, double theta = 0, double D = 1, int halfRange = 2, int startSelect = 2, /* MFI */ int[] nAvailable = null)
        {
            StartItemsModel result = CatRcs.StartItems.StartItems_Calc(itemBank, model, fixItems, seed, nrItems, theta, D, halfRange, startSelect, nAvailable);
            return result;
        }

        public static NextItemModel NextItem(CATItems itemBank, string model = null, double theta = 0, int[] Out = null, int[] x = null, int criterion = 5, /* MFI */ string method = "BM",
            string priorDist = "norm", double[] priorPar = null, double D = 1, double[] range = null, int[] parInt = null, int infoType = 2, /* observed */ int randomesque = 1, int rule = 1, /* Length */
            double thr = 20, double? SETH = null, double AP = 1, int[] nAvailable = null, int maxItems = 50, CBControlList cbControl = null, string[] cbGroup = null)
        {
            NextItemModel result = CatRcs.NextItem.NextItem_Calc(itemBank, model, theta, Out, x, criterion, method, priorDist, priorPar, D, range, parInt,
                                            infoType, randomesque, rule, thr, SETH, AP, nAvailable, maxItems, cbControl, cbGroup);
            return result;
        }

        #endregion

        #region Sample Functions

        private static string[] GetStringArray(string[] inputArray)
        {
            List<string> resultList = new List<string>();

            if (inputArray.Length > 0)
            {
                foreach (string value in inputArray)
                {
                    resultList.Add("Welcome to C# & R Integration ... + '" + value + "'!");
                }
            }

            string[] resultArray = resultList.ToArray();

            return resultArray;
        }

        private static double[] GetNumericArray(double[] inputArray)
        {
            List<double> resultList = new List<double>();

            if (inputArray.Length > 0)
            {
                foreach (double value in inputArray)
                {
                    resultList.Add(value);
                }
            }

            double[] resultArray = resultList.ToArray();

            return resultArray;
        }

        private static List<double> GetNumericList(List<double> inputList)
        {
            List<double> resultList = new List<double>();

            if (inputList.Count > 0)
            {
                foreach (double value in inputList)
                {
                    resultList.Add(value);
                }
            }

            return resultList;
        }

        #endregion
    }
}