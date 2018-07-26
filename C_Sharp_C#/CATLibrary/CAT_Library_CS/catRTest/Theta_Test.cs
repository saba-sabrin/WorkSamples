using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using NUnit.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using RDotNet;
using RDotNet.Devices;
using RDotNet.Dynamic;
using RDotNet.Internals;
using RDotNet.Utilities;
using RDotNet.NativeLibrary;
using CatRcs.Models;
using CatRcs;
using CatRcs.Utils;

namespace catRTest
{
    [TestFixture]
    public class Theta_Test
    {
        bool resultFlag = false;
        private double testEpsilon = 0.0000001;
        int decimalPoint = 4;

        [Test]
        [TestCase(100)]
        public void test_ThetaEST_SemTheta_D(int numberOfItems)
        {
            resultFlag = true;

            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            //DataFrame tcals = engineObj.Evaluate("tcals <- read.delim(\"D:/PROJECT/DIPF_Project/catR/data/tcals.txt\")").AsDataFrame();
            //engineObj.SetSymbol("tcals", tcals);

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Dichotomous Items
            //DataFrame DichoItems = engineObj.Evaluate("DichoItems <- tcals[1:80,1:4]").AsDataFrame();
            DataFrame DichoItems = engineObj.Evaluate("DichoItems <- genDichoMatrix(items = " + numberOfItems + ")").AsDataFrame();
            engineObj.SetSymbol("DichoItems", DichoItems);

            //Creation of a response pattern
            engineObj.Evaluate("set.seed(1)");
            NumericVector x_val = engineObj.Evaluate("x_val <- genPattern(-1, DichoItems)").AsNumeric();
            engineObj.SetSymbol("x_val", x_val);

            Console.WriteLine("Start of R Processing: " + DateTime.Now.TimeOfDay.ToString());

            // Call "thetaEST" function from R
            NumericVector r_ThetaEst = engineObj.Evaluate("r_Theta <- thetaEst(DichoItems, x_val, method=\"EAP\")").AsNumeric();
            engineObj.SetSymbol("r_ThetaEst", r_ThetaEst);

            Console.WriteLine("R Theta Calculation Finished on: " + DateTime.Now.TimeOfDay.ToString());

            // TESTING PURPOSE
            CATItems itemBank = new CATItems(DichoItems[0].Select(y => (double)y).ToArray(), DichoItems[1].Select(y => (double)y).ToArray(),
              DichoItems[2].Select(y => (double)y).ToArray(), DichoItems[3].Select(y => (double)y).ToArray());

            int[] x = x_val.Select(y => (int)y).ToArray();

            // Call "SemTheta" function from R
            //NumericVector r_ThetaSem = engineObj.Evaluate("r_ThetaSem <- semTheta(r_ThetaEst, DichoItems)").AsNumeric();

            Console.WriteLine("Start of CS Processing: " + DateTime.Now.TimeOfDay.ToString());

            var stopwatch = new Stopwatch();
            stopwatch.Restart();

            // Call "thetaEST" function from CS
            double cs_ThetaEst = CatRLib.ThetaEst(itemBank, x, "", new double[] { 0, 1 }, new double[] { -4, 4 }, new int[] { -4, 4, 33 }, 1, method: "EAP");

            Console.WriteLine("Time taken: " + stopwatch.ElapsedMilliseconds);

            Console.WriteLine("CS Theta Calculation Finished on: " + DateTime.Now.TimeOfDay.ToString());

            // Call "SemTheta" function from CS
            //double cs_ThetaSem = CatRLib.SemTheta(cs_ThetaEst, itemBank);

            // Compare result of function "ThetaEst"
            if (decimal.Round(Convert.ToDecimal(r_ThetaEst[0]), decimalPoint) - decimal.Round(Convert.ToDecimal(cs_ThetaEst), decimalPoint) > decimal.Round(Convert.ToDecimal(testEpsilon), decimalPoint))
            {
                resultFlag = false;
            }

            // Compare result of function "SemTheta"
            /*if (decimal.Round(Convert.ToDecimal(r_ThetaSem[0]), decimalPoint) - decimal.Round(Convert.ToDecimal(cs_ThetaSem), decimalPoint) > decimal.Round(Convert.ToDecimal(testEpsilon), decimalPoint))
            {
                resultFlag = false;
            }*/

            Console.WriteLine("Theta Values: " + decimal.Round(Convert.ToDecimal(r_ThetaEst[0]), decimalPoint) + " , " + decimal.Round(Convert.ToDecimal(cs_ThetaEst), decimalPoint));

            Assert.IsTrue(resultFlag);
        }

        [Test]
        /* Parameter 1: Number of items
           Parameter 2: Model */
        [TestCase(10, ModelNames.Models.GRM)]
        public void test_ThetaEST_SemTheta_P(int NumOfItems, ModelNames.Models paramModel)
        {
            resultFlag = true;
            var stopwatch = new Stopwatch();

            REngine.SetEnvironmentVariables();
            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Polytomous Items
            CharacterVector modelName = engineObj.CreateCharacterVector(new string[] { paramModel.EnumToString() });
            engineObj.SetSymbol("modelName", modelName);
            DataFrame PolyItems = engineObj.Evaluate("PolyItems <- genPolyMatrix(" + NumOfItems + ", 4, model = modelName, same.nrCat = TRUE)").AsDataFrame();
            engineObj.SetSymbol("PolyItems", PolyItems);

            // Adapting with the existing "CAT-Items" type (Wrapper)
            Console.WriteLine("*******************************************");
            Console.WriteLine("Polytomous Items, Model : " + paramModel.EnumToString());
            Console.WriteLine("*******************************************");

            // Create item object
            CATItems itemBank = new CATItems(NumOfItems: NumOfItems, model: paramModel.EnumToString(), nrCat: 4, same_nrCat: false);

            //DataFrame iii = engineObj.Evaluate("item_new <- as.matrix(PolyItems)").AsDataFrame();

            for (int k = 0; k < itemBank.colSize; k++)
            {
                itemBank.DataWrapper(PolyItems[k].Select(y => (double)y).ToArray(), k);
            }

            //itemBank = RtoCSDataHandler.DataConverterRtoCS(itemBank, iii.Select(y => (double)y).ToArray());

            //itemBank = RtoCSDataHandler.DataConverterRtoCS(itemBank, PolyItems);

            //Creation of a response pattern
            engineObj.Evaluate("set.seed(1)");
            NumericVector x_val = engineObj.Evaluate("x_val <- genPattern(1, PolyItems, model = modelName)").AsNumeric();
            engineObj.SetSymbol("x_val", x_val);

            int[] x = x_val.Select(y => (int)y).ToArray();

            Console.WriteLine("Start of R Processing: " + DateTime.Now.TimeOfDay.ToString());
            stopwatch.Restart();

            // Call "ThetaEST" function from R,   method = \"ML\"
            //NumericVector r_ThetaEst = engineObj.Evaluate("r_ThetaEst  <- thetaEst(PolyItems, x_val, model = modelName, method = \"BM\", priorDist = \"norm\", priorPar = c(-2, 2))").AsNumeric();
            NumericVector r_ThetaEst = engineObj.Evaluate("r_ThetaEst  <- thetaEst(PolyItems, x_val, modelName)").AsNumeric();
            engineObj.SetSymbol("r_ThetaEst", r_ThetaEst);

            Console.WriteLine("R Time taken: " + stopwatch.ElapsedMilliseconds + " ms");
            Console.WriteLine("R Theta Calculation Finished on: " + DateTime.Now.TimeOfDay.ToString());

            double[] priorPar = new double[2]; priorPar[0] = -2; priorPar[1] = 2;

            Console.WriteLine("Start of CS Processing: " + DateTime.Now.TimeOfDay.ToString());
            stopwatch.Restart();

            double cs_ThetaEst = CatRLib.ThetaEst(itemBank, x, paramModel.EnumToString());

            // Call "ThetaEST" function from CS
            //double cs_ThetaEst = CatRLib.ThetaEst(it: itemBank, x: x, method:"BM", model: paramModel.EnumToString(), priorPar: priorPar, priorDist: "norm");

            Console.WriteLine("CS Time taken: " + stopwatch.ElapsedMilliseconds + " ms");
            Console.WriteLine("CS Theta Calculation Finished on: " + DateTime.Now.TimeOfDay.ToString());

            // Compare result of function "ThetaEst"
            if (decimal.Round(Convert.ToDecimal(r_ThetaEst[0]), decimalPoint) - decimal.Round(Convert.ToDecimal(cs_ThetaEst), decimalPoint) > decimal.Round(Convert.ToDecimal(testEpsilon), decimalPoint))
            {
                resultFlag = false;
            }

            // Call "SemTheta" function from R
            NumericVector r_ThetaSem = engineObj.Evaluate("r_ThetaSem <- semTheta(r_ThetaEst, PolyItems, model = modelName, method = \"BM\", priorDist = \"norm\", priorPar = c(-2, 2))").AsNumeric();

            // Call "SemTheta" function from CS
            double cs_ThetaSem = CatRLib.SemTheta(thEst: cs_ThetaEst, it: itemBank, method: "BM", model: paramModel.EnumToString(), priorPar: priorPar, priorDist: "norm");

            // Compare result of function "SemTheta"
            if (decimal.Round(Convert.ToDecimal(r_ThetaSem[0]), decimalPoint) - decimal.Round(Convert.ToDecimal(cs_ThetaSem), decimalPoint) > decimal.Round(Convert.ToDecimal(testEpsilon), decimalPoint))
            {
                resultFlag = false;
            }

            Assert.IsTrue(resultFlag);
        }
    }
}
