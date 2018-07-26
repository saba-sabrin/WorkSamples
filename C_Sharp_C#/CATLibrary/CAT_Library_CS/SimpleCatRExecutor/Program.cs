using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using RDotNet.NativeLibrary;
using CatRcs.Models;
using CatRcs.Utils;
using CatRcs;
using System.Diagnostics;

namespace SimpleCatRExecutor
{
    class Program
    {
        public delegate TOutput f<TInput, TOutput>(TInput from);
        public static bool resultFlag = false;
        public static double testEpsilon = 0.0000001;
        public static int decimalPoint = 4;

        public static double Method2(double[] param1)
        {
            param1 = new double[] {9, 1, 3};
            return (param1[0]);
        }

        private void CommectedFunc()
        {
            REngine engineObj = REngine.GetInstance();

            //NativeUtility.SetEnvironmentVariables("D:/Office_Projects/neps-catirt/CS/R/bin/x64/", "D:/Office_Projects/neps-catirt/CS/");

            //var logInfo = NativeUtility.FindRPaths(ref rPath, ref rHome);

            //REngine.SetEnvironmentVariables("D:/Office_Projects/neps-catirt/CS/R/bin/x64/", "D:/Office_Projects/neps-catirt/CS"); // office

            //Console.WriteLine(logInfo);

            // Data Frame
            //DataFrame tcals = engineObj.Evaluate("tcals <- read.delim(\"D:/Office_Projects/neps-catirt/catR/data/tcals.txt\")").AsDataFrame();  // Office

            //DataFrame tcals = engineObj.Evaluate("tcals <- read.delim(\"D:/PROJECT/Office_Projects/LATEST/neps-catirt/catR/data/tcals.txt\")").AsDataFrame();  // Home

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Calling functions from catR library
            GenericVector cbList = engineObj.Evaluate("cbList <- list(names = c(\"Group1\", \"Group2\", \"Group3\", \"Group4\"), props = c(0.2,0.4,0.3,0.1))").AsList();
            engineObj.SetSymbol("cbList", cbList);

            // Dichotomous Items
            DataFrame DichoItems = engineObj.Evaluate("DichoItems <- genPolyMatrix(10, 4, model = \"GRM\")").AsDataFrame();
            //DataFrame DichoItems = engineObj.Evaluate("DichoItems <- genDichoMatrix(items = 10, cbControl = cbList)").AsDataFrame();
            engineObj.SetSymbol("DichoItems", DichoItems);

            // Adapting with the existing "CAT-Items" type (Wrapper)
            //CATItems itemBank = new CATItems();
            double[] aj = DichoItems[0].Select(y => (double)y).ToArray();
            double[] bj1 = DichoItems[1].Select(y => (double)y).ToArray();
            double[] bj2 = DichoItems[2].Select(y => (double)y).ToArray();
            double[] bj3 = DichoItems[3].Select(y => (double)y).ToArray();

            string[] grp_All = DichoItems[4].AsCharacter().Select(y => (string)y).ToArray();
            int[] grp = DichoItems[4].AsNumeric().Select(y => (int)y).ToArray();

            // Conversion of Group Levels to character
            CharacterVector groupLevels = engineObj.Evaluate("groupLevels <- levels(DichoItems$Group)").AsCharacter();
            //itemBank.Levels = groupLevels.Select(m => (string)m).ToArray();

            // Working with factors (Example)
            NumericVector cbGroup = engineObj.CreateNumericVector(new double[] { 1, 3, 1, 2, 2, 3 });
            engineObj.SetSymbol("cbGroup", cbGroup);

            Factor groupValues = engineObj.Evaluate("factor(cbGroup, labels = c(\"Audio\", \"Video\", \"Music\"))").AsFactor();

            var arrayVals = groupValues.Select(x => Convert.ToInt16(x.ToString())).ToArray();

            // .NET Framework Array to R vector.
            NumericVector group1 = engineObj.CreateNumericVector(new double[] { 30.02, 29.99, 30.11, 29.97, 30.01, 29.99 });
            engineObj.SetSymbol("group1", group1);

            // Direct parsing from R script.
            NumericVector group2 = engineObj.Evaluate("group2 <- c(29.89, 29.93, 29.72, 29.98, 30.02, 29.98)").AsNumeric();

            // Test difference of mean and get the P-value.
            GenericVector testResult = engineObj.Evaluate("t.test(group1, group2)").AsList();
            double p = testResult["p.value"].AsNumeric().First();

            Console.WriteLine("Group1: [{0}]", string.Join(", ", group1));
            Console.WriteLine("Group2: [{0}]", string.Join(", ", group2));
            Console.WriteLine("P-value = {0:0.000}", p);

            // Dispose of the REngine object
            engineObj.Dispose();

            Console.ReadKey();
        }

        public static void Main(string[] args)
        {
            //f<double[], double> delObj = new f<double[], double>(Method2);
            //Console.WriteLine(delObj(new double[] {1, 4, 5}).ToString());

            ModelNames.Models paramModel = ModelNames.Models.GRM;
            int NumOfItems = 50;
            resultFlag = true;
            var stopwatch = new Stopwatch();

            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Polytomous Items
            CharacterVector modelName = engineObj.CreateCharacterVector(new string[] { paramModel.EnumToString() });
            engineObj.SetSymbol("modelName", modelName);
            DataFrame PolyItems = engineObj.Evaluate("PolyItems <- genPolyMatrix(" + NumOfItems + ", 5, model = modelName, same.nrCat = TRUE)").AsDataFrame();
            engineObj.SetSymbol("PolyItems", PolyItems);

            // Adapting with the existing "CAT-Items" type (Wrapper)

            Console.WriteLine("*******************************************");
            Console.WriteLine("Polytomous Items, Model : " + paramModel.EnumToString());
            Console.WriteLine("*******************************************");

            CATItems itemBank = new CATItems(NumOfItems: NumOfItems, model: paramModel.EnumToString(), nrCat: 5);

            for (int i = 0; i < itemBank.colSize; i++)
            {
                itemBank.all_items_poly[i] = PolyItems[i].Select(y => (double)y).ToArray();
            }

            //Creation of a response pattern
            engineObj.Evaluate("set.seed(1)");
            NumericVector x_val = engineObj.Evaluate("x_val <- genPattern(0, PolyItems, model = modelName)").AsNumeric();
            engineObj.SetSymbol("x_val", x_val);

            int[] x = x_val.Select(y => (int)y).ToArray();

            Console.WriteLine("Start of R Processing: " + DateTime.Now.TimeOfDay.ToString());
            stopwatch.Restart();

            // Call "ThetaEST" function from R,   method = \"ML\"
            NumericVector r_ThetaEst = engineObj.Evaluate("r_ThetaEst  <- thetaEst(PolyItems, x_val, model = modelName, method = \"BM\", priorDist = \"norm\", priorPar = c(-2, 2))").AsNumeric();
            engineObj.SetSymbol("r_ThetaEst", r_ThetaEst);

            Console.WriteLine("R Time taken: " + stopwatch.ElapsedMilliseconds);
            Console.WriteLine("R Theta Calculation Finished on: " + DateTime.Now.TimeOfDay.ToString());

            double[] priorPar = new double[2]; priorPar[0] = -2; priorPar[1] = 2;

            Console.WriteLine("Start of CS Processing: " + DateTime.Now.TimeOfDay.ToString());
            stopwatch.Restart();

            // Call "ThetaEST" function from CS
            double cs_ThetaEst = CatRLib.ThetaEst(it: itemBank, x: x, method: "BM", model: paramModel.EnumToString(), priorPar: priorPar, priorDist: "norm");

            Console.WriteLine("CS Time taken: " + stopwatch.ElapsedMilliseconds);
            Console.WriteLine("CS Theta Calculation Finished on: " + DateTime.Now.TimeOfDay.ToString());

            // Compare result of function "ThetaEst"
            if (decimal.Round(Convert.ToDecimal(r_ThetaEst[0]), decimalPoint) - decimal.Round(Convert.ToDecimal(cs_ThetaEst), decimalPoint) > decimal.Round(Convert.ToDecimal(testEpsilon), decimalPoint))
            {
                resultFlag = false;
            }

            if(resultFlag)
            {
                Console.WriteLine("Values matched !!");
            }
            else
            {
                Console.WriteLine("Values are not matched !!");
            }
        }
    }
}
