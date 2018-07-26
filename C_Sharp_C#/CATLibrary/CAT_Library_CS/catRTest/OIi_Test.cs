using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Core;
using NUnit.Framework;
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

namespace catRTest
{
    [TestFixture]
    public class OIi_Test
    {
        bool resultFlag = false;

        [Test]
        public void testOIi_D()
        {
            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Dichotomous Items
            DataFrame DichoItems = engineObj.Evaluate("DichoItems <- genDichoMatrix(items = 10)").AsDataFrame();
            engineObj.SetSymbol("DichoItems", DichoItems);

            // Adapting with the existing "CAT-Items" type (Wrapper)
            CATItems itemBank = new CATItems(DichoItems[0].Length, false);

            // New Dictionary
            itemBank.SetItemParamter(CATItems.ColumnNames.a, DichoItems[0].Select(y => (double)y).ToArray());
            itemBank.SetItemParamter(CATItems.ColumnNames.b, DichoItems[1].Select(y => (double)y).ToArray());
            itemBank.SetItemParamter(CATItems.ColumnNames.c, DichoItems[2].Select(y => (double)y).ToArray());
            itemBank.SetItemParamter(CATItems.ColumnNames.d, DichoItems[3].Select(y => (double)y).ToArray());

            #region "Test block for Ability Values (th)"

            OIiList objOIi = null;
            int decimalPoint = 6;

            double[] th_Values = CatRcs.Utils.CommonHelper.Sequence(-6, 6, by: 1);

            Console.WriteLine("******* TEST for Ability value Theta ********");

            for (int k = 0; k < th_Values.Length; k++)
            {
                // Sequence Generation for Theta (Ability) values
                NumericVector th_val = engineObj.CreateNumericVector(new double[] { th_Values[k] });
                engineObj.SetSymbol("th_val", th_val);

                // Calling the Ii function from R
                GenericVector result_OIi = engineObj.Evaluate("result_OIi <- OIi(th = th_val, DichoItems, x = 1, D = 1)").AsList();

                // Getting the function result
                NumericVector OIi = result_OIi.AsNumeric();

                Console.WriteLine("Value of Theta: " + th_Values[k]);

                objOIi = CatRLib.OIi(th_Values[k], itemBank, new int[] { 1 }, "", 1);

                #region "OIi"

                int z = 0;
                resultFlag = true;

                Console.WriteLine("****** OIi ******");

                for (int i = 0; i < objOIi.OIi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(OIi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objOIi.OIi[i]), decimalPoint))
                    {
                        Console.WriteLine("Test for Item No. # " + z + " is not Matched!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of OIi result are Matched!");

                #endregion
            }

            #endregion

            #region "Test block for Metric values"

            objOIi = null;

            double[] D_Values = CatRcs.Utils.CommonHelper.Sequence(0.5, 1, by: 0.1);

            Console.WriteLine("******* TEST for Metric Constant ********");

            for (int k = 0; k < D_Values.Length; k++)
            {
                // Sequence Generation for Metric values
                NumericVector D_val = engineObj.CreateNumericVector(new double[] { D_Values[k] });
                engineObj.SetSymbol("D_val", D_val);

                // Calling the Ii function from R
                GenericVector result_OIi = engineObj.Evaluate("result_OIi <- OIi(th = 0, DichoItems, x = 1, D = D_val)").AsList();

                // Getting the function result
                NumericVector OIi = result_OIi.AsNumeric();

                Console.WriteLine("Value of Metric COnstant: " + D_Values[k]);

                objOIi = CatRLib.OIi(0, itemBank, new int[] { 1 }, "", D_Values[k]);

                #region "OIi"

                int z = 0;
                resultFlag = true;

                Console.WriteLine("****** OIi ******");

                for (int i = 0; i < objOIi.OIi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(OIi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objOIi.OIi[i]), decimalPoint))
                    {
                        Console.WriteLine("Test for Item No. # " + z + " is not Matched!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of OIi result are Matched!");

                #endregion
            }

            #endregion
        }

        [Test]
        [TestCase(ModelNames.Models.GRM)]
        public void testOIi_P(ModelNames.Models paramModel)
        {
            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Polytomous Items
            CharacterVector modelName = engineObj.CreateCharacterVector(new string[] { paramModel.EnumToString() });
            engineObj.SetSymbol("modelName", modelName);
            DataFrame PolyItems = engineObj.Evaluate("PolyItems <- genPolyMatrix(10, 5, model = modelName, same.nrCat = TRUE)").AsDataFrame();
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

            OIiList objOIi = null;
            int decimalPoint = 6;

            double[] th_Values = CatRcs.Utils.CommonHelper.Sequence(-6, 6, by: 1);

            Console.WriteLine("******* TEST for Ability value Theta ********");

            for (int k = 0; k < th_Values.Length; k++)
            {
                // Sequence Generation for Theta (Ability) values
                NumericVector th_val = engineObj.CreateNumericVector(new double[] { th_Values[k] });
                engineObj.SetSymbol("th_val", th_val);

                //Creation of a response pattern
                engineObj.Evaluate("set.seed(1)");
                NumericVector x_val = engineObj.Evaluate("x_val <- genPattern(th = th_val, PolyItems, model = modelName, D = 1)").AsNumeric();
                engineObj.SetSymbol("x_val", x_val);

                // Calling the Ii function from R
                GenericVector result_OIi = engineObj.Evaluate("result_OIi <- OIi(th = th_val, PolyItems, x = x_val, model = modelName, D = 1)").AsList();

                // Getting the function result
                NumericVector OIi = result_OIi.AsNumeric();

                Console.WriteLine("Value of Theta: " + th_Values[k]);

                objOIi = CatRLib.OIi(th_Values[k], itemBank, x_val.Select(y => (int)y).ToArray(), paramModel.EnumToString(), 1);

                #region "OIi"

                int z = 0;
                resultFlag = true;

                Console.WriteLine("****** OIi ******");

                for (int i = 0; i < objOIi.OIi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(OIi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objOIi.OIi[i]), decimalPoint))
                    {
                        Console.WriteLine("Test for Item No. # " + z + " is not Matched!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of OIi result are Matched!");

                #endregion
            }

            #endregion

            #region "Test block for Metric values"

            objOIi = null;

            double[] D_Values = CatRcs.Utils.CommonHelper.Sequence(0.5, 1, by: 0.1);

            Console.WriteLine("******* TEST for Metric Constant ********");

            for (int k = 0; k < D_Values.Length; k++)
            {
                // Sequence Generation for Metric values
                NumericVector D_val = engineObj.CreateNumericVector(new double[] { D_Values[k] });
                engineObj.SetSymbol("D_val", D_val);

                //Creation of a response pattern
                engineObj.Evaluate("set.seed(1)");
                NumericVector x_val = engineObj.Evaluate("x_val <- genPattern(th = 0, PolyItems, model = modelName, D = D_val)").AsNumeric();
                engineObj.SetSymbol("x_val", x_val);

                // Calling the Ii function from R
                GenericVector result_OIi = engineObj.Evaluate("result_OIi <- OIi(th = 0, PolyItems, x = x_val, model = modelName, D = D_val)").AsList();

                // Getting the function result
                NumericVector OIi = result_OIi.AsNumeric();

                Console.WriteLine("Value of Metric COnstant: " + D_Values[k]);

                objOIi = CatRLib.OIi(0, itemBank, x_val.Select(y => (int)y).ToArray(), paramModel.EnumToString(), D_Values[k]);

                #region "OIi"

                int z = 0;
                resultFlag = true;

                Console.WriteLine("****** OIi ******");

                for (int i = 0; i < objOIi.OIi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(OIi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objOIi.OIi[i]), decimalPoint))
                    {
                        Console.WriteLine("Test for Item No. # " + z + " is not Matched!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of OIi result are Matched!");

                #endregion
            }

            #endregion
        }
    }
}
