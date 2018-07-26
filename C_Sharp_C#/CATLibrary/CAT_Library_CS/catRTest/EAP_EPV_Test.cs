using System;
using NUnit.Framework;
using NUnit.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using RDotNet.Devices;
using RDotNet.Dynamic;
using RDotNet.Internals;
using RDotNet.Utilities;
using RDotNet.NativeLibrary;
using CatRcs.Models;
using CatRcs;
using System.Globalization;
using System.Diagnostics;

namespace catRTest
{
    [TestFixture]
    public class EAP_EPV_Test
    {
        bool resultFlag = false;

        private int[] numberOfItems = { 100 };

        private double testEpsilon = 0.0000001;
        NumberFormatInfo nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };

        [Test]
        public void testEapEST_and_EapSEM_D()
        {
            resultFlag = true;

            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Test for item banks with different number of items

            for (int i1 = 0; i1 < numberOfItems.Length; i1++)
            {
                // Dichotomous Items
                DataFrame DichoItems = engineObj.Evaluate("DichoItems <- genDichoMatrix(items = " + numberOfItems[i1] + ")").AsDataFrame();
                engineObj.SetSymbol("DichoItems", DichoItems);

                // Adapting with the existing "CAT-Items" type (Wrapper)
                CATItems itemBank = new CATItems(DichoItems[0].Select(y => (double)y).ToArray(), DichoItems[1].Select(y => (double)y).ToArray(),
                    DichoItems[2].Select(y => (double)y).ToArray(), DichoItems[3].Select(y => (double)y).ToArray());

                // test for different theta values
                double[] th = CatRcs.Utils.CommonHelper.Sequence(-6, 6, length: 11);

                var stopwatch = new Stopwatch();
                stopwatch.Restart();

                for (int i2 = 0; i2 < th.Length; i2++)
                {
                    string temp = th[i2].ToString(nfi);

                    //Creation of a response pattern for theta value
                    NumericVector x_val = engineObj.Evaluate("x_val <- genPattern(" + th[i2].ToString(nfi) + ", DichoItems)").AsNumeric();

                    int[] x = x_val.Select(y => (int)y).ToArray();

                    // Call "EapEST" function from R
                    NumericVector r_eapEst = engineObj.Evaluate("result_Eap <- eapEst(DichoItems, x_val)").AsNumeric();

                    // Call "EapSEM" function from R
                    NumericVector r_eapSem = engineObj.Evaluate("result_EapSem <- eapSem(result_Eap, DichoItems, x_val)").AsNumeric();

                    stopwatch.Restart();

                    // Call "EapEST" function from CatRCS
                    double cs_eapEst = CatRLib.EapEST(itemBank, x, "");

                    // Call "EapSEM" function from CatRCS
                    double cs_eapSem = CatRLib.EapSEM(cs_eapEst, itemBank, x, "");

                    Console.WriteLine("Time: " + stopwatch.ElapsedMilliseconds);

                    if (r_eapEst[0] - cs_eapEst > testEpsilon ||
                        r_eapSem[0] - cs_eapSem > testEpsilon)
                    {
                        resultFlag = false;
                    }
                }
            }

            Assert.IsTrue(resultFlag, "Test Passed!");
        }

        [Test]
        [TestCase(10, ModelNames.Models.PCM)]
        public void testEapEST_and_EapSEM_P(int NumOfItems, ModelNames.Models paramModel)
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

                //Creation of a response pattern for theta value
                engineObj.Evaluate("set.seed(1)");
                NumericVector x_val = engineObj.Evaluate("x_val <- genPattern(" + "th = " + th[j].ToString(nfi) + ", PolyItems, " + "model = modelName)").AsNumeric();
                engineObj.SetSymbol("x_val", x_val);

                int[] x = x_val.Select(y => (int)y).ToArray();

                // Call "EapEST" function from R
                NumericVector r_eapEst = engineObj.Evaluate("result_Eap <- eapEst(PolyItems, x_val, model = modelName)").AsNumeric();

                // Call "EapEST" function from CatRCS
                double cs_eapEst = CatRLib.EapEST(itemBank, x, paramModel.EnumToString());


                /* EapEst function, line 111 needs to be optimized for passing the Test
                 * Check EapSEM function too !! */

                // Call "EapSEM" function from R
                NumericVector r_eapSem = engineObj.Evaluate("result_EapSem <- eapSem(result_Eap, PolyItems, x_val, model = modelName)").AsNumeric();

                // Call "EapSEM" function from CatRCS
                double cs_eapSem = CatRLib.EapSEM(cs_eapEst, itemBank, x, paramModel.EnumToString());

                if (r_eapEst[0] - cs_eapEst > testEpsilon ||
                    r_eapSem[0] - cs_eapSem > testEpsilon)
                {
                    resultFlag = false;
                }
            }

            Assert.IsTrue(resultFlag);

            #endregion
        }

        [Test]
        public void testEPV_D()
        {
            resultFlag = true;

            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Dichotomous Items
            DataFrame DichoItems = engineObj.Evaluate("DichoItems <- genDichoMatrix(items = 200)").AsDataFrame();
            engineObj.SetSymbol("DichoItems", DichoItems);

            // Adapting with the existing "CAT-Items" type (Wrapper)
            CATItems itemBank = new CATItems(DichoItems[0].Select(y => (double)y).ToArray(), DichoItems[1].Select(y => (double)y).ToArray(),
               DichoItems[2].Select(y => (double)y).ToArray(), DichoItems[3].Select(y => (double)y).ToArray());

            // test for different theta values
            double[] th = CatRcs.Utils.CommonHelper.Sequence(-6, 6, length: 11);

            var stopwatch = new Stopwatch();
            stopwatch.Restart();

            for (int t = 0; t < th.Length; t++)
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

                int[] index = new int[] { 4, 8 };
                CATItems it_given = new CATItems(index.Length);  // itemBank.FindItem(new int[] { 4, 8 });

                for (int i=0; i < index.Length; i++)
                {
                    it_given.a[i] = itemBank.a[index[i]-1];
                    it_given.b[i] = itemBank.b[index[i]-1];
                    it_given.c[i] = itemBank.c[index[i]-1];
                    it_given.d[i] = itemBank.d[index[i]-1];
                }

                // Call "EPV" function from R
                NumericVector r_epv = engineObj.Evaluate("r_epv <- EPV(DichoItems, 1, x_val, " + th[t].ToString(nfi) + ", it_given_R)").AsNumeric();

                stopwatch.Restart();

                // Call "EPV" function from CatRCS
                double cs_epv = CatRLib.EPV(itemBank, 1, x, th[t], it_given, "");

                Console.WriteLine("Time taken: " + stopwatch.ElapsedMilliseconds);

                if (r_epv[0] - cs_epv > testEpsilon)
                {
                    resultFlag = false;
                }

                Console.WriteLine(r_epv[0] + " " + cs_epv);
            }

            Assert.IsTrue(resultFlag);
        }

        /* Bug: Have to see the function for internal Pi return.. column based & row based data must be handled */
        [Test]
        [TestCase(10, ModelNames.Models.GRM)]
        public void testEPV_P(int NumOfItems, ModelNames.Models paramModel)
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

                // Polytomous Items
                DataFrame it_given_R = engineObj.Evaluate("it_given_R <- PolyItems[c(4, 8),]").AsDataFrame();
                engineObj.SetSymbol("it_given_R", it_given_R);

                //Creation of a response pattern for theta value
                engineObj.Evaluate("set.seed(1)");
                NumericVector x_val = engineObj.Evaluate("x_val <- genPattern(" + th[j].ToString(nfi) + ", it_given_R, model = modelName)").AsNumeric();
                engineObj.SetSymbol("x_val", x_val);

                int[] x = x_val.Select(y => (int)y).ToArray();

                CATItems it_given = itemBank.FindItem(new int[] { 4, 8 });

                // Call "EPV" function from R
                NumericVector r_epv = engineObj.Evaluate("r_epv <- EPV(PolyItems, 1, x_val, " + th[j].ToString(nfi) + ", it_given_R, model = modelName)").AsNumeric();

                // Call "EPV" function from CatRCS
                double cs_epv = CatRLib.EPV(itemBank, 1, x, th[j], it_given, paramModel.EnumToString());

                if (r_epv[0] - cs_epv > testEpsilon)
                {
                    resultFlag = false;
                }
            }

            Assert.IsTrue(resultFlag);

            #endregion
        }
    }
}
