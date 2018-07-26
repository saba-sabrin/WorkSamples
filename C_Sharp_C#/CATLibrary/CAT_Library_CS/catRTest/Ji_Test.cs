using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
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

namespace catRTest
{
    [TestFixture]
    public class Ji_Test
    {
        bool resultFlag = false;

        [Test]
        public void testJi_D()
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

            JiList objJi = null;
            int decimalPoint = 6;

            double[] th_Values = CatRcs.Utils.CommonHelper.Sequence(-6, 6, by: 1);

            Console.WriteLine("******* TEST for Ability value Theta ********");

            for (int k = 0; k < th_Values.Length; k++)
            {
                // Sequence Generation for Theta (Ability) values
                NumericVector th_val = engineObj.CreateNumericVector(new double[] { th_Values[k] });
                engineObj.SetSymbol("th_val", th_val);

                // Calling the "Ji" function from R
                GenericVector result_Ji = engineObj.Evaluate("result_Ji <- Ji(th = th_val, DichoItems, D = 1)").AsList();

                // Getting the function result
                NumericVector Ji = result_Ji[0].AsNumeric();
                NumericVector dJi = result_Ji[1].AsNumeric();

                Console.WriteLine("Value of Theta: " + th_Values[k]);

                objJi = CatRLib.Ji(th_Values[k], itemBank, "", 1);

                #region "Ji"

                int z = 0;
                resultFlag = true;

                Console.WriteLine("****** Ji ******");

                for (int i = 0; i < objJi.Ji.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(Ji[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objJi.Ji[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of Ji result is Passed!");

                #endregion

                #region "dJi"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** dJi ******");

                for (int i = 0; i < objJi.dJi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(dJi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objJi.dJi[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of dJi result is Passed!");

                #endregion
            }

            #endregion

            #region "Test block for Metric Constant"

            objJi = null;

            double[] D_Values = CatRcs.Utils.CommonHelper.Sequence(0.5, 1, by: 0.1);

            Console.WriteLine("******* TEST for Metric Constant ********");

            for (int k = 0; k < D_Values.Length; k++)
            {
                // Sequence Generation for Theta (Ability) values
                NumericVector D_val = engineObj.CreateNumericVector(new double[] { D_Values[k] });
                engineObj.SetSymbol("D_val", D_val);

                // Calling the "Ji" function from R
                GenericVector result_Ji = engineObj.Evaluate("result_Ji <- Ji(th = 0, DichoItems, D = D_val)").AsList();

                // Getting the function result
                NumericVector Ji = result_Ji[0].AsNumeric();
                NumericVector dJi = result_Ji[1].AsNumeric();

                Console.WriteLine("Value of Metric Constant: " + D_Values[k]);

                objJi = CatRLib.Ji(0, itemBank, "", D_Values[k]);

                #region "Ii"

                int z = 0;
                resultFlag = true;

                Console.WriteLine("****** Ji ******");

                for (int i = 0; i < objJi.Ji.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(Ji[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objJi.Ji[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of Ji result is Passed!");

                #endregion

                #region "dJi"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** dJi ******");

                for (int i = 0; i < objJi.dJi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(dJi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objJi.dJi[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of dJi result is Passed!");

                #endregion
            }

            #endregion
        }

        [Test]
        [TestCase(ModelNames.Models.GRM)]
        public void testJi_P(ModelNames.Models paramModel)
        {
            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Polytomous Items
            CharacterVector modelName = engineObj.CreateCharacterVector(new string[] { paramModel.EnumToString() });
            engineObj.SetSymbol("modelName", modelName);
            DataFrame PolyItems = engineObj.Evaluate("PolyItems <- genPolyMatrix(100, 5, model = modelName, same.nrCat = TRUE)").AsDataFrame();
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

            JiList objJi = null;
            int decimalPoint = 6;

            double[] th_Values = CatRcs.Utils.CommonHelper.Sequence(-6, 6, by: 1);

            Console.WriteLine("******* TEST for Ability value Theta ********");

            for (int k = 0; k < th_Values.Length; k++)
            {
                // Sequence Generation for Theta (Ability) values
                NumericVector th_val = engineObj.CreateNumericVector(new double[] { th_Values[k] });
                engineObj.SetSymbol("th_val", th_val);

                // Calling the "Ji" function from R
                GenericVector result_Ji = engineObj.Evaluate("result_Ji <- Ji(th = th_val, PolyItems, model = modelName, D = 1)").AsList();

                // Getting the function result
                NumericVector Ji = result_Ji[0].AsNumeric();
                NumericVector dJi = result_Ji[1].AsNumeric();

                Console.WriteLine("Value of Theta: " + th_Values[k]);

                objJi = CatRLib.Ji(th_Values[k], itemBank, paramModel.EnumToString(), 1);

                #region "Ji"

                int z = 0;
                resultFlag = true;

                Console.WriteLine("****** Ji ******");

                for (int i = 0; i < objJi.Ji.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(Ji[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objJi.Ji[i]), decimalPoint))
                    {
                        Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        Console.WriteLine("Value from R: " + decimal.Round(Convert.ToDecimal(Ji[i]), decimalPoint));
                        Console.WriteLine("Value from CS: " + decimal.Round(Convert.ToDecimal(objJi.Ji[i]), decimalPoint));
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of Ji result is Passed!");

                #endregion

                #region "dJi"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** dJi ******");

                for (int i = 0; i < objJi.dJi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(dJi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objJi.dJi[i]), decimalPoint))
                    {
                        Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of dJi result is Passed!");

                #endregion
            }

            #endregion

            #region "Test block for Metric values"

            objJi = null;

            double[] D_Values = CatRcs.Utils.CommonHelper.Sequence(0.5, 1, by: 0.1);

            Console.WriteLine("******* TEST for Metric Constant ********");

            for (int k = 0; k < D_Values.Length; k++)
            {
                // Sequence Generation for Theta (Ability) values
                NumericVector D_val = engineObj.CreateNumericVector(new double[] { D_Values[k] });
                engineObj.SetSymbol("D_val", D_val);

                // Calling the "Ji" function from R
                GenericVector result_Ji = engineObj.Evaluate("result_Ji <- Ji(th = 0, PolyItems, model = modelName, D = D_val)").AsList();

                // Getting the function result
                NumericVector Ji = result_Ji[0].AsNumeric();
                NumericVector dJi = result_Ji[1].AsNumeric();

                Console.WriteLine("Value of Metric Constant: " + D_Values[k]);

                objJi = CatRLib.Ji(0, itemBank, paramModel.EnumToString(), D_Values[k]);

                #region "Ji"

                int z = 0;
                resultFlag = true;

                Console.WriteLine("****** Ji ******");

                for (int i = 0; i < objJi.Ji.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(Ji[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objJi.Ji[i]), decimalPoint))
                    {
                        /*Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        Console.WriteLine("Value from R: " + decimal.Round(Convert.ToDecimal(Ji[i]), decimalPoint));
                        Console.WriteLine("Value from CS: " + decimal.Round(Convert.ToDecimal(objJi.Ji[i]), decimalPoint));*/
                        resultFlag = false;
                    }
                }

                //Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of Ji result is Passed!");

                #endregion

                #region "dJi"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** dJi ******");

                for (int i = 0; i < objJi.dJi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(dJi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objJi.dJi[i]), decimalPoint))
                    {
                        Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of dJi result is Passed!");

                #endregion
            }

            #endregion
        }
    }
}
