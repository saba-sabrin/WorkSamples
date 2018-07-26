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
    public class Ii_Test
    {
        bool resultFlag = false;

        [Test]
        public void testIi_D()
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

            IiList objIi = null;
            int decimalPoint = 6;

            double[] th_Values = CatRcs.Utils.CommonHelper.Sequence(-4, 4, by: 1);

            Console.WriteLine("******* TEST for Ability value Theta ********");

            for (int k = 0; k < th_Values.Length; k++)
            {
                // Sequence Generation for Theta (Ability) values
                NumericVector th_val = engineObj.CreateNumericVector(new double[] { th_Values[k] });
                engineObj.SetSymbol("th_val", th_val);

                // Calling the Ii function from R
                GenericVector result_Ii = engineObj.Evaluate("result_Ii <- Ii(th = th_val, DichoItems)").AsList();

                // Getting the function result
                NumericVector Ii = result_Ii[0].AsNumeric();
                NumericVector dIi = result_Ii[1].AsNumeric();
                NumericVector d2Ii = result_Ii[2].AsNumeric();

                Console.WriteLine("Value of Theta: " + th_Values[k]);

                objIi = CatRLib.Ii(th_Values[k], itemBank, "", 1);

                #region "Ii"

                int z = 0;
                resultFlag = true;

                Console.WriteLine("****** Ii ******");

                for (int i = 0; i < objIi.Ii.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(Ii[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objIi.Ii[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of Ii result is Passed!");

                #endregion

                #region "dIi"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** dIi ******");

                for (int i = 0; i < objIi.dIi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(dIi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objIi.dIi[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of dIi result is Passed!");

                #endregion

                #region "d2Ii"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** d2Ii ******");

                for (int i = 0; i < objIi.d2Ii.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(d2Ii[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objIi.d2Ii[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of d2Ii result is Passed!");

                #endregion
            }

            #endregion

            #region "Test block for Metric values"

            objIi = null;

            double[] D_Values = CatRcs.Utils.CommonHelper.Sequence(0.5, 1, by: 0.1);

            Console.WriteLine("******* TEST for Metric Constant ********");

            for (int k = 0; k < D_Values.Length; k++)
            {
                // Sequence Generation for Theta (Ability) values
                NumericVector D_val = engineObj.CreateNumericVector(new double[] { D_Values[k] });
                engineObj.SetSymbol("D_val", D_val);

                // Calling the Ii function from R
                GenericVector result_Ii = engineObj.Evaluate("result_Ii <- Ii(th = 0, DichoItems, D = D_val)").AsList();

                // Getting the function result
                NumericVector Ii = result_Ii[0].AsNumeric();
                NumericVector dIi = result_Ii[1].AsNumeric();
                NumericVector d2Ii = result_Ii[2].AsNumeric();

                Console.WriteLine("Value of Metric Constant: " + D_Values[k]);

                objIi = CatRLib.Ii(0, itemBank, "", D_Values[k]);

                #region "Ii"

                int z = 0;
                resultFlag = true;

                Console.WriteLine("****** Ii ******");

                for (int i = 0; i < objIi.Ii.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(Ii[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objIi.Ii[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of Ii result is Passed!");

                #endregion

                #region "dIi"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** dIi ******");

                for (int i = 0; i < objIi.dIi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(dIi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objIi.dIi[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of dIi result is Passed!");

                #endregion

                #region "d2Ii"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** d2Ii ******");

                for (int i = 0; i < objIi.d2Ii.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(d2Ii[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objIi.d2Ii[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of d2Ii result is Passed!");

                #endregion
            }

            #endregion
        }

        [Test]
        [TestCase(ModelNames.Models.PCM)]
        public void testIi_P(ModelNames.Models paramModel)
        {
            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Polytomous Items
            CharacterVector modelName = engineObj.CreateCharacterVector(new string[] { paramModel.EnumToString() });
            engineObj.SetSymbol("modelName", modelName);
            DataFrame PolyItems = engineObj.Evaluate("PolyItems <- genPolyMatrix(10, 5, model = modelName, same.nrCat = FALSE)").AsDataFrame();
            engineObj.SetSymbol("PolyItems", PolyItems);

            // Adapting with the existing "CAT-Items" type (Wrapper)

            Console.WriteLine("*******************************************");
            Console.WriteLine("Polytomous Items, Model : " + paramModel.EnumToString());
            Console.WriteLine("*******************************************");

            CATItems itemBank = new CATItems(NumOfItems: PolyItems[0].Length, model: paramModel.EnumToString(), same_nrCat:false, nrCat: 5);

            for (int i = 0; i < itemBank.colSize; i++)
            {
                itemBank.all_items_poly[i] = PolyItems[i].Select(y => (double)y).ToArray();
            }

            for (int j = 0; j < itemBank.colSize; j++)
            {
                for (int k = 0; k < itemBank.NumOfItems; k++)
                {
                    if (CatRcs.Utils.CheckNaValues.IsNaNvalue(itemBank.all_items_poly[j][k]))
                    {
                        itemBank.all_items_poly[j][k] = double.NaN;
                    }
                }
            }

            #region "Test block for Ability Values (th)"

            IiList objIi = null;
            int decimalPoint = 6;

            double[] th_Values = CatRcs.Utils.CommonHelper.Sequence(-4, 4, by: 1);

            Console.WriteLine("******* TEST for Ability value Theta ********");

            for (int k = 0; k < th_Values.Length; k++)
            {
                // Sequence Generation for Theta (Ability) values
                NumericVector th_val = engineObj.CreateNumericVector(new double[] { th_Values[k] });
                engineObj.SetSymbol("th_val", th_val);

                // Calling the Ii function from R
                GenericVector result_Ii = engineObj.Evaluate("result_Ii <- Ii(th = th_val, PolyItems, model = modelName, D = 1)").AsList();

                // Getting the function result
                NumericVector Ii = result_Ii[0].AsNumeric();
                NumericVector dIi = result_Ii[1].AsNumeric();
                NumericVector d2Ii = result_Ii[2].AsNumeric();

                Console.WriteLine("Value of Theta: " + th_Values[k]);

                objIi = CatRLib.Ii(th_Values[k], itemBank, paramModel.EnumToString(), 1);

                #region "Ii"

                int z = 0;
                resultFlag = true;

                Console.WriteLine("****** Ii ******");

                for (int i = 0; i < objIi.Ii.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(Ii[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objIi.Ii[i]), decimalPoint))
                    {
                        Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }


                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of Ii result is Passed!");

                #endregion

                #region "dIi"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** dIi ******");

                for (int i = 0; i < objIi.dIi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(dIi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objIi.dIi[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of dIi result is Passed!");

                #endregion

                #region "d2Ii"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** d2Ii ******");

                for (int i = 0; i < objIi.d2Ii.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(d2Ii[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objIi.d2Ii[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of d2Ii result is Passed!");

                #endregion
            }

            #endregion

            #region "Test block for Metric values"

            objIi = null;

            double[] D_Values = CatRcs.Utils.CommonHelper.Sequence(0.5, 1, by: 0.1);

            Console.WriteLine("******* TEST for Metric Constant ********");

            for (int k = 0; k < D_Values.Length; k++)
            {
                // Sequence Generation for Metric values
                NumericVector D_val = engineObj.CreateNumericVector(new double[] { D_Values[k] });
                engineObj.SetSymbol("D_val", D_val);

                // Calling the Ii function from R
                GenericVector result_Ii = engineObj.Evaluate("result_Ii <- Ii(th = 0, PolyItems, model = modelName, D = D_val)").AsList();

                // Getting the function result
                NumericVector Ii = result_Ii[0].AsNumeric();
                NumericVector dIi = result_Ii[1].AsNumeric();
                NumericVector d2Ii = result_Ii[2].AsNumeric();

                Console.WriteLine("Value of Metric Constant: " + D_Values[k]);

                objIi = CatRLib.Ii(0, itemBank, paramModel.EnumToString(), D_Values[k]);

                #region "Ii"

                int z = 0;
                resultFlag = true;

                Console.WriteLine("****** Ii ******");

                for (int i = 0; i < objIi.Ii.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(Ii[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objIi.Ii[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of Ii result is Passed!");

                #endregion

                #region "dIi"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** dIi ******");

                for (int i = 0; i < objIi.dIi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(dIi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objIi.dIi[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of dIi result is Passed!");

                #endregion

                #region "d2Ii"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** d2Ii ******");

                for (int i = 0; i < objIi.d2Ii.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(d2Ii[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objIi.d2Ii[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of d2Ii result is Passed!");

                #endregion
            }

            #endregion
        }
    }
}
