using System;
using NUnit.Framework;
using NUnit.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using RDotNet;
using RDotNet.Devices;
using RDotNet.Dynamic;
using RDotNet.Internals;
using RDotNet.Utilities;
using RDotNet.NativeLibrary;
using CatRcs.Models;
using CatRcs;

namespace catRTest
{
    [TestFixture]
    public class KL_Test
    {
        bool resultFlag = false;
        private double testEpsilon = 0.0000001;
        NumberFormatInfo nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };

        [Test]
        [TestCase(10)]
        public void testKL_D(int NumOfItems)
        {
            resultFlag = true;

            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Dichotomous Items
            DataFrame DichoItems = engineObj.Evaluate("DichoItems <- genDichoMatrix(items = " + NumOfItems + ")").AsDataFrame();
            engineObj.SetSymbol("DichoItems", DichoItems);

            // Adapting with the existing "CAT-Items" type (Wrapper)
            CATItems itemBank = new CATItems(DichoItems[0].Length, false);

            // New Dictionary
            itemBank.SetItemParamter(CATItems.ColumnNames.a, DichoItems[0].Select(y => (double)y).ToArray());
            itemBank.SetItemParamter(CATItems.ColumnNames.b, DichoItems[1].Select(y => (double)y).ToArray());
            itemBank.SetItemParamter(CATItems.ColumnNames.c, DichoItems[2].Select(y => (double)y).ToArray());
            itemBank.SetItemParamter(CATItems.ColumnNames.d, DichoItems[3].Select(y => (double)y).ToArray());

            // Dichotomous Items
            DataFrame it_given_R = engineObj.Evaluate("it_given_R <- DichoItems[c(4, 8),]").AsDataFrame();
            engineObj.SetSymbol("it_given_R", it_given_R);

            CATItems it_given = itemBank.FindItem(new int[] { 4, 8 });

            //Creation of a response pattern
            NumericVector x_val = engineObj.Evaluate("x_val <- c(0, 1)").AsNumeric();
            engineObj.SetSymbol("x_val", x_val);

            int[] x = new int[] { 0, 1 };

            // Call "thetaEST" function from R,   method = \"ML\"
            NumericVector Theta = engineObj.Evaluate("Theta <- thetaEst(it_given_R, x_val, method = \"ML\")").AsNumeric();

            // Call "KL" function from R
            NumericVector r_KL = engineObj.Evaluate("r_KL <- KL(DichoItems, 1, x_val, it_given_R, theta = Theta)").AsNumeric();

            // Call "thetaEST" function from CS
            double th = CatRLib.ThetaEst(it_given, x, "", method: "ML");

            // Call "KL" function from CS
            double cs_KL = CatRLib.KL(itemBank, 1, x, it_given, "", theta: th);

            if (r_KL[0] - cs_KL > testEpsilon)
            {
                resultFlag = false;
            }

            Assert.IsTrue(resultFlag);
        }

        [Test]
        [TestCase(10, ModelNames.Models.GRM)]
        public void testKL_P(int NumOfItems, ModelNames.Models paramModel)
        {
            resultFlag = true;

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

            CATItems itemBank = new CATItems(PolyItems[0].Length, paramModel.EnumToString(), 5);

            Tuple<CATItems.ColumnNames, int>[] cols = itemBank.GetKeys();

            for (int i = 0; i < cols.Length; i++)
            {
                itemBank.SetItemParamter(cols[i], PolyItems[i].Select(y => (double)y).ToArray());
            }


            DataFrame it_given_R = engineObj.Evaluate("it_given_R <- PolyItems[c(4, 8),]").AsDataFrame();
            engineObj.SetSymbol("it_given_R", it_given_R);

            CATItems it_given = itemBank.FindItem(new int[] { 4, 8 });

            //Creation of a response pattern for theta value
            engineObj.Evaluate("set.seed(1)");
            NumericVector x_val = engineObj.Evaluate("x_val <- genPattern(0, it_given_R, model = modelName)").AsNumeric();
            engineObj.SetSymbol("x_val", x_val);

            int[] x = x_val.Select(y => (int)y).ToArray();

            // Call "thetaEST" function from R,   method = \"ML\"
            NumericVector Theta = engineObj.Evaluate("Theta <- thetaEst(it_given_R, x_val, method = \"ML\", model = modelName)").AsNumeric();

            // Call "KL" function from R
            NumericVector r_KL = engineObj.Evaluate("r_KL <- KL(PolyItems, 1, x_val, it_given_R, theta = Theta, model = modelName)").AsNumeric();

            // Call "thetaEST" function from CS
            double th_est = CatRLib.ThetaEst(it_given, x, paramModel.EnumToString(), method: "ML");

            // Call "KL" function from CS
            double cs_KL = CatRLib.KL(itemBank, 1, x, it_given, paramModel.EnumToString(), theta: th_est);

            if (r_KL[0] - cs_KL > testEpsilon)
            {
                resultFlag = false;
            }

            Assert.IsTrue(resultFlag);
        }
    }
}
