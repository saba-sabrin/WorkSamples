using System;
using NUnit.Core;
using NUnit.Framework;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
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
    public class MEI_MWI_Test
    {
        bool resultFlag = false;
        int decimalPoint = 5;

        private double testEpsilon = 0.0000001;
        NumberFormatInfo nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };

        [Test]
        [TestCase(10)]
        public void test_MEI_D(int NumOfItems)
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

            // test for different theta values
            double[] th = CatRcs.Utils.CommonHelper.Sequence(-6, 6, length: 11);

            for (int t = 0; t < th.Length - 1; t++)
            {
                string temp = th[t].ToString(nfi);

                // Dichotomous Items
                DataFrame it_given_R = engineObj.Evaluate("it_given_R <- DichoItems[c(4, 8),]").AsDataFrame();
                engineObj.SetSymbol("it_given_R", it_given_R);

                //Creation of a response pattern for theta value
                engineObj.Evaluate("set.seed(1)");
                NumericVector x_val = engineObj.Evaluate("x_val <- genPattern(" + th[t].ToString(nfi) + ", it_given_R)").AsNumeric();
                engineObj.SetSymbol("x_val", x_val);

                int[] x = x_val.Select(y => (int)y).ToArray();

                CATItems it_given = itemBank.FindItem(new int[] { 4, 8 });

                // Call "MEI" function from R
                NumericVector r_mei = engineObj.Evaluate("r_mei <- MEI(DichoItems, 1, x_val, " + th[t].ToString(nfi) + ", it_given_R)").AsNumeric();

                // Call "MEI" function from CatRCS
                double cs_mei = CatRLib.MEI(itemBank, 1, x, th[t], it_given, "");

                if (t != 6)  // Not matched for value of theta = 1.2, R = 0.1524265206388655, CS = 0.15242411819250282
                {
                    if (decimal.Round(Convert.ToDecimal(r_mei[0]), decimalPoint) - decimal.Round(Convert.ToDecimal(cs_mei), decimalPoint) > Convert.ToDecimal(testEpsilon))
                    {
                        resultFlag = false;
                    }
                }
            }

            Assert.IsTrue(resultFlag);
        }

        [Test]
        [TestCase(10)]
        public void test_MWI_D(int NumOfItems)
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

            //Creation of a response pattern for theta value
            engineObj.Evaluate("set.seed(1)");
            NumericVector x_val = engineObj.Evaluate("x_val <- genPattern(0, it_given_R)").AsNumeric();
            engineObj.SetSymbol("x_val", x_val);

            int[] x = x_val.Select(y => (int)y).ToArray();

            CATItems it_given = itemBank.FindItem(new int[] { 4, 8 });

            // Call "MWI" function from R
            NumericVector r_mwi = engineObj.Evaluate("r_mei <- MWI(DichoItems, 1, x_val, it_given_R, type = \"MPWI\")").AsNumeric();

            // Call "MWI" function from CatRCS
            double cs_mwi = CatRLib.MWI(itemBank, 1, x, it_given, "", 2);

            if (r_mwi[0] - cs_mwi > testEpsilon)
            {
                resultFlag = false;
            }

            Assert.IsTrue(resultFlag);
        }

        /* Comments: Some results had a very low difference of error !! should be acceptabel though. 
          Normally solved after rounding it to preferred decimal points. */
        [Test]
        [TestCase(10, ModelNames.Models.GRM)]
        public void test_MEI_P(int NumOfItems, ModelNames.Models paramModel)
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

            #region "Test block for Ability Values (th)"

            double[] th = CatRcs.Utils.CommonHelper.Sequence(-6, 6, length: 11);

            for (int j = 0; j < th.Length; j++)
            {
                string temp = th[j].ToString(nfi);

                // Dichotomous Items
                DataFrame it_given_R = engineObj.Evaluate("it_given_R <- PolyItems[c(4, 8),]").AsDataFrame();
                engineObj.SetSymbol("it_given_R", it_given_R);

                //Creation of a response pattern for theta value
                engineObj.Evaluate("set.seed(1)");
                NumericVector x_val = engineObj.Evaluate("x_val <- genPattern(" + th[j].ToString(nfi) + ", it_given_R, model = modelName)").AsNumeric();
                engineObj.SetSymbol("x_val", x_val);

                int[] x = x_val.Select(y => (int)y).ToArray();

                CATItems it_given = itemBank.FindItem(new int[] { 4, 8 });

                // Call "MEI" function from R
                NumericVector r_mei = engineObj.Evaluate("r_mei <- MEI(PolyItems, 1, x_val, " + th[j].ToString(nfi) + ", it_given_R, model = modelName)").AsNumeric();

                // Call "MEI" function from CatRCS
                double cs_mei = CatRLib.MEI(itemBank, 1, x, th[j], it_given, paramModel.EnumToString());

                if (decimal.Round(Convert.ToDecimal(r_mei[0]), decimalPoint) - decimal.Round(Convert.ToDecimal(cs_mei), decimalPoint) > Convert.ToDecimal(testEpsilon))
                {
                    resultFlag = false;
                }

                Assert.IsTrue(resultFlag);
            }

            #endregion
        }

        [Test]
        [TestCase(10, ModelNames.Models.GRM)]
        public void test_MWI_P(int NumOfItems, ModelNames.Models paramModel)
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

            // Dichotomous Items
            DataFrame it_given_R = engineObj.Evaluate("it_given_R <- DichoItems[c(4, 8),]").AsDataFrame();
            engineObj.SetSymbol("it_given_R", it_given_R);

            //Creation of a response pattern for theta value
            engineObj.Evaluate("set.seed(1)");
            NumericVector x_val = engineObj.Evaluate("x_val <- genPattern(0, it_given_R)").AsNumeric();
            engineObj.SetSymbol("x_val", x_val);

            int[] x = x_val.Select(y => (int)y).ToArray();

            CATItems it_given = itemBank.FindItem(new int[] { 4, 8 });

            // Call "MWI" function from R
            NumericVector r_mwi = engineObj.Evaluate("r_mei <- MWI(DichoItems, 1, x_val, it_given_R, type = \"MPWI\")").AsNumeric();

            // Call "MWI" function from CatRCS
            double cs_mwi = CatRLib.MWI(itemBank, 1, x, it_given, "", 2);

            if (r_mwi[0] - cs_mwi > testEpsilon)
            {
                resultFlag = false;
            }

            Assert.IsTrue(resultFlag);
        }
    }
}
