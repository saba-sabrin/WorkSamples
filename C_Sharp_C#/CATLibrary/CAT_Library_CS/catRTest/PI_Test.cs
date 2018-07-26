using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using NUnit.Core;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using RDotNet;
using RDotNet.NativeLibrary;
using CatRcs.Models;
using CatRcs;
using System.IO;
using System.Reflection;

namespace catRTest
{
    [TestFixture]
    public class PI_Test
    {
        bool resultFlag = false;

        [Test]
        public void testPi_D()
        {
            bool OSProc = System.Environment.Is64BitProcess;

            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Dichotomous Items
            DataFrame DichoItems = engineObj.Evaluate("DichoItems <- genDichoMatrix(items = 100)").AsDataFrame();
            engineObj.SetSymbol("DichoItems", DichoItems);

            // Adapting with the existing "CAT-Items" type (Wrapper)
            CATItems itemBank = new CATItems(DichoItems[0].Select(y => (double)y).ToArray(), DichoItems[1].Select(y => (double)y).ToArray(),
              DichoItems[2].Select(y => (double)y).ToArray(), DichoItems[3].Select(y => (double)y).ToArray());

            #region "Test block for Ability Values (th)"

            PiList objPI = null;
            int decimalPoint = 4;

            double[] th_Values = CatRcs.Utils.CommonHelper.Sequence(-6, 6, by: 1);

            Console.WriteLine("******* TEST for Ability value Theta ********");

            for (int k = 0; k < th_Values.Length; k++)
            {
                // Sequence Generation for Theta (Ability) values
                NumericVector th_val = engineObj.CreateNumericVector(new double[] { th_Values[k] });
                engineObj.SetSymbol("th_val", th_val);

                // Calling the "Pi" from R
                GenericVector result_Pi = engineObj.Evaluate("result_Pi <- Pi(th = th_val, DichoItems, D = 1)").AsList();

                // Getting the function result
                NumericVector Pi = result_Pi[0].AsNumeric();
                NumericVector dPi = result_Pi[1].AsNumeric();
                NumericVector d2Pi = result_Pi[2].AsNumeric();
                NumericVector d3Pi = result_Pi[3].AsNumeric();

                Console.WriteLine("Value of Theta: " + th_Values[k]);

                objPI = CatRLib.Pi_D(th_Values[k], itemBank, "", 1);

                #region "Pi"

                int z = 0;
                resultFlag = true;

                Console.WriteLine("****** Pi ******");

                for (int i = 0; i < Pi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(Pi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.Pi[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag, "Passed");
                Console.WriteLine("Values of Pi are Matched!");

                #endregion

                #region "dPi"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** dPi ******");

                for (int i = 0; i < dPi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(dPi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.dPi[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of dPi are Matched!");

                #endregion

                #region "d2Pi"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** d2Pi ******");

                for (int i = 0; i < d2Pi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(d2Pi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.d2Pi[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of d2Pi are Matched!");

                #endregion

                #region "d3Pi"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** d3Pi ******");

                for (int i = 0; i < d3Pi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(d3Pi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.d3Pi[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of d3Pi are Matched!");

                #endregion
            }

            Assert.IsTrue(resultFlag);

            #endregion

            #region "Test block for Metric values"

            objPI = null;

            double[] D_Values = CatRcs.Utils.CommonHelper.Sequence(0.5, 1, by: 0.1);

            Console.WriteLine("******* TEST for Metric Constant ********");

            for (int k = 0; k < D_Values.Length; k++)
            {
                // Sequence Generation for Theta (Ability) values
                NumericVector D_val = engineObj.CreateNumericVector(new double[] { D_Values[k] });
                engineObj.SetSymbol("D_val", D_val);

                // Calling the "Pi" from R
                GenericVector result_Pi = engineObj.Evaluate("result_Pi <- Pi(th = 0, DichoItems, D = D_val)").AsList();

                // Getting the function result
                NumericVector Pi = result_Pi[0].AsNumeric();
                NumericVector dPi = result_Pi[1].AsNumeric();
                NumericVector d2Pi = result_Pi[2].AsNumeric();
                NumericVector d3Pi = result_Pi[3].AsNumeric();

                Console.WriteLine("Value of Metric constant: " + D_Values[k]);

                objPI = CatRLib.Pi_D(0, itemBank, "", D_Values[k]);

                #region "Pi"

                int z = 0;
                resultFlag = true;

                Console.WriteLine("****** Pi ******");

                for (int i = 0; i < Pi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(Pi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.Pi[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of Pi are matched!");

                #endregion

                #region "dPi"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** dPi ******");

                for (int i = 0; i < dPi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(dPi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.dPi[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of dPi are matched!");

                #endregion

                #region "d2Pi"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** d2Pi ******");

                for (int i = 0; i < d2Pi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(d2Pi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.d2Pi[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of d2Pi are Matched!");

                #endregion

                #region "d3Pi"

                z = 0;
                resultFlag = true;

                Console.WriteLine("****** d3Pi ******");

                for (int i = 0; i < d3Pi.Length; i++)
                {
                    z = i + 1;  // item number

                    if (decimal.Round(Convert.ToDecimal(d3Pi[i]), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.d3Pi[i]), decimalPoint))
                    {
                        //Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of d3Pi are Matched!");

                #endregion
            }

            #endregion
        }

        // Test for other models... "MGRM", "PCM"...
        // Also update function logic of Pi.. "RSM".. afterwards

        [Test]
        [TestCase(ModelNames.Models.PCM)]
        public void testPi_P(ModelNames.Models paramModel)
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

            CATItems itemBank = new CATItems(NumOfItems: PolyItems[0].Length, model: paramModel.EnumToString(), same_nrCat: true, nrCat: 5);

            for (int i = 0; i < itemBank.colSize; i++)
            {
                itemBank.all_items_poly[i] = PolyItems[i].Select(y => (double)y).ToArray();
            }

            itemBank.NAValueHandler();

            #region "Test block for Ability Values (th)"

            PiListPoly objPI = null;
            int decimalPoint = 6;

            // Block for temporary testing

            double[] th_Values = CatRcs.Utils.CommonHelper.Sequence(-4, 4, by: 1);

            Console.WriteLine("******* TEST for Ability value Theta ********");

            for (int k = 0; k < th_Values.Length; k++)
            {
                // Sequence Generation for Theta (Ability) values
                NumericVector th_val = engineObj.CreateNumericVector(new double[] { th_Values[k] });
                engineObj.SetSymbol("th_val", th_val);

                // Calling the "Pi" from R
                GenericVector result_Pi = engineObj.Evaluate("result_Pi <- Pi(th = th_val, PolyItems, model = modelName, D = 1)").AsList();

                // Getting the "Pi" function result
                NumericVector Pi = result_Pi[0].AsNumeric();
                NumericVector dPi = result_Pi[1].AsNumeric();
                NumericVector d2Pi = result_Pi[2].AsNumeric();
                NumericVector d3Pi = result_Pi[3].AsNumeric();

                Console.WriteLine("Value of Theta: " + th_Values[k]);

                // Calling "Pi" function
                objPI = CatRLib.Pi_P(th_Values[k], itemBank, paramModel.EnumToString(), 1);

                #region "Pi"

                int z = 0, pi_index = 0;
                resultFlag = true;
                Console.WriteLine("****** Pi ******");

                int len = objPI.Pi.GetLength(0);
                int len2 = objPI.Pi.GetLength(1);

                for (int i = 0; i < objPI.Pi.GetLength(1); i++)  // column
                {
                    z = i + 1;  // item number

                    for (int j = 0; j < objPI.Pi.GetLength(0); j++)  // row
                    {
                        double tempPiVal = !Double.IsNaN(Pi[pi_index]) ? Pi[pi_index] : 0;

                        decimal temp2 = decimal.Round(Convert.ToDecimal(tempPiVal), decimalPoint);
                        decimal temp1 = decimal.Round(Convert.ToDecimal(objPI.Pi[j, i]), decimalPoint);

                        if (decimal.Round(Convert.ToDecimal(tempPiVal), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.Pi[j, i]), decimalPoint))
                        {
                            Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                            resultFlag = false;
                        }

                        pi_index++;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of Pi are Matched!");

                #endregion

                #region "dPi"

                z = 0; pi_index = 0;
                resultFlag = true;
                Console.WriteLine("****** dPi ******");

                for (int i = 0; i < objPI.dPi.GetLength(1); i++)  // column
                {
                    z = i + 1;  // item number

                    for (int j = 0; j < objPI.dPi.GetLength(0); j++)  // row
                    {
                        double tempPiVal = !Double.IsNaN(dPi[pi_index]) ? dPi[pi_index] : 0;

                        decimal temp2 = decimal.Round(Convert.ToDecimal(tempPiVal), decimalPoint);
                        decimal temp1 = decimal.Round(Convert.ToDecimal(objPI.Pi[j, i]), decimalPoint);

                        if (decimal.Round(Convert.ToDecimal(tempPiVal), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.dPi[j, i]), decimalPoint))
                        {
                            Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                            resultFlag = false;
                        }

                        pi_index++;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of dPi are Matched!");

                #endregion

                #region "d2Pi"

                z = 0; pi_index = 0;
                resultFlag = true;
                Console.WriteLine("****** d2Pi ******");

                for (int i = 0; i < objPI.d2Pi.GetLength(1); i++)  // column
                {
                    z = i + 1;  // item number

                    for (int j = 0; j < objPI.d2Pi.GetLength(0); j++)  // row
                    {
                        double tempPiVal = !Double.IsNaN(d2Pi[pi_index]) ? d2Pi[pi_index] : 0;

                        if (decimal.Round(Convert.ToDecimal(tempPiVal), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.d2Pi[j, i]), decimalPoint))
                        {
                            Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                            resultFlag = false;
                        }

                        pi_index++;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of d2Pi are Matched!");

                #endregion

                #region "d3Pi"

                z = 0; pi_index = 0;
                resultFlag = true;
                Console.WriteLine("****** d3Pi ******");

                for (int i = 0; i < objPI.d3Pi.GetLength(1); i++)  // column
                {
                    z = i + 1;  // item number

                    for (int j = 0; j < objPI.d3Pi.GetLength(0); j++)  // row
                    {
                        double tempPiVal = !Double.IsNaN(d3Pi[pi_index]) ? d3Pi[pi_index] : 0;

                        if (decimal.Round(Convert.ToDecimal(tempPiVal), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.d3Pi[j, i]), decimalPoint))
                        {
                            Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                            resultFlag = false;
                        }

                        pi_index++;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of d3Pi are Matched!");

                #endregion
            }

            #endregion

            #region "Test Block for Metric values"

            objPI = null;
            double[] D_Values = CatRcs.Utils.CommonHelper.Sequence(0.5, 1, by: 0.1);
            

            Console.WriteLine("******* TEST for Metric Constant ********");

            for (int k = 0; k < D_Values.Length; k++)
            {
                // Sequence Generation for Metric Constant
                NumericVector D_val = engineObj.CreateNumericVector(new double[] { D_Values[k] });
                engineObj.SetSymbol("D_val", D_val);

                // Calling the "Pi" from R
                GenericVector result_Pi = engineObj.Evaluate("result_Pi <- Pi(th = 0, PolyItems, model = modelName, D = D_val)").AsList();

                // Getting the "Pi" function result
                NumericVector Pi = result_Pi[0].AsNumeric();
                NumericVector dPi = result_Pi[1].AsNumeric();
                NumericVector d2Pi = result_Pi[2].AsNumeric();
                NumericVector d3Pi = result_Pi[3].AsNumeric();

                Console.WriteLine("Value of Metric Constant: " + D_Values[k]);

                // Calling "Pi" function
                objPI = CatRLib.Pi_P(0, itemBank, paramModel.EnumToString(), D_Values[k]);

                #region "Pi"

                int z = 0, pi_index = 0;
                resultFlag = true;
                Console.WriteLine("****** Pi ******");

                int len = objPI.Pi.GetLength(0);
                int len2 = objPI.Pi.GetLength(1);

                for (int i = 0; i < objPI.Pi.GetLength(1); i++)  // column
                {
                    z = i + 1;  // item number

                    for (int j = 0; j < objPI.Pi.GetLength(0); j++)  // row
                    {
                        double tempPiVal = !Double.IsNaN(Pi[pi_index]) ? Pi[pi_index] : 0;

                        if (decimal.Round(Convert.ToDecimal(tempPiVal), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.Pi[j, i]), decimalPoint))
                        {
                            Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                            resultFlag = false;
                        }

                        pi_index++;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of Pi are Matched!");

                #endregion

                #region "dPi"

                z = 0; pi_index = 0;
                resultFlag = true;
                Console.WriteLine("****** dPi ******");

                for (int i = 0; i < objPI.dPi.GetLength(1); i++)  // column
                {
                    z = i + 1;  // item number

                    for (int j = 0; j < objPI.dPi.GetLength(0); j++)  // row
                    {
                        double tempPiVal = !Double.IsNaN(dPi[pi_index]) ? dPi[pi_index] : 0;

                        if (decimal.Round(Convert.ToDecimal(tempPiVal), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.dPi[j, i]), decimalPoint))
                        {
                            Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                            resultFlag = false;
                        }

                        pi_index++;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of dPi are Matched!");

                #endregion

                #region "d2Pi"

                z = 0; pi_index = 0;
                resultFlag = true;
                Console.WriteLine("****** d2Pi ******");

                for (int i = 0; i < objPI.d2Pi.GetLength(1); i++)  // column
                {
                    z = i + 1;  // item number

                    for (int j = 0; j < objPI.d2Pi.GetLength(0); j++)  // row
                    {
                        double tempPiVal = !Double.IsNaN(d2Pi[pi_index]) ? d2Pi[pi_index] : 0;

                        if (decimal.Round(Convert.ToDecimal(tempPiVal), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.d2Pi[j, i]), decimalPoint))
                        {
                            Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                            resultFlag = false;
                        }

                        pi_index++;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of d2Pi are Matched!");

                #endregion

                #region "d3Pi"

                z = 0; pi_index = 0;
                resultFlag = true;
                Console.WriteLine("****** d3Pi ******");

                for (int i = 0; i < objPI.d3Pi.GetLength(1); i++)  // column
                {
                    z = i + 1;  // item number

                    for (int j = 0; j < objPI.d3Pi.GetLength(0); j++)  // row
                    {
                        double tempPiVal = !Double.IsNaN(d3Pi[pi_index]) ? d3Pi[pi_index] : 0;

                        if (decimal.Round(Convert.ToDecimal(tempPiVal), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.d3Pi[j, i]), decimalPoint))
                        {
                            Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                            resultFlag = false;
                        }

                        pi_index++;
                    }
                }

                Assert.IsTrue(resultFlag);
                Console.WriteLine("Values of d3Pi are Matched!");

                #endregion
            }

            #endregion
        }

        /* Implement for all the models !!
         * Optimize !!
         * Performance check !!
         * Test for all the models */
        [Test]
        [TestCase(ModelNames.Models.PCM)]
        public void testPi_P_New(ModelNames.Models paramModel)
        {
            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Polytomous Items
            CharacterVector modelName = engineObj.CreateCharacterVector(new string[] { paramModel.EnumToString() });
            engineObj.SetSymbol("modelName", modelName);
            DataFrame PolyItems = engineObj.Evaluate("PolyItems <- genPolyMatrix(100, 5, model = modelName, same.nrCat = FALSE)").AsDataFrame();
            engineObj.SetSymbol("PolyItems", PolyItems);

            // Adapting with the existing "CAT-Items" type (Wrapper)

            Console.WriteLine("*******************************************");
            Console.WriteLine("Polytomous Items, Model : " + paramModel.EnumToString());
            Console.WriteLine("*******************************************");

            CATItems itemBank = new CATItems(NumOfItems: PolyItems[0].Length, model: paramModel.EnumToString(), nrCat: 5);

            for (int i = 0; i < itemBank.colSize; i++)
            {
                itemBank.all_items_poly[i] = PolyItems[i].Select(y => (double)y).ToArray();
            }

            PiListPoly objPI = null;
            int decimalPoint = 4;

            double[] th_Values = CatRcs.Utils.CommonHelper.Sequence(-4, 4, by: 1);
            NumericVector th_val = engineObj.CreateNumericVector(new double[] { th_Values[0] });
            engineObj.SetSymbol("th_val", th_val);

            // Calling the "Pi" from R
            GenericVector result_Pi = engineObj.Evaluate("result_Pi <- Pi(th = th_val, PolyItems, model = modelName, D = 1)").AsList();

            // Getting the "Pi" function result
            NumericVector Pi = result_Pi[0].AsNumeric();
            NumericVector dPi = result_Pi[1].AsNumeric();
            NumericVector d2Pi = result_Pi[2].AsNumeric();
            NumericVector d3Pi = result_Pi[3].AsNumeric();

            Console.WriteLine("Value of Theta: " + th_Values[0]);

            // Calling "Pi" function
            objPI = CatRLib.Pi_P(th_Values[0], itemBank, paramModel.EnumToString(), 1);

            #region "Pi"

            int z = 0, pi_index = 0;
            resultFlag = true;
            Console.WriteLine("****** Pi ******");

            int len = objPI.Pi.GetLength(0);
            int len2 = objPI.Pi.GetLength(1);

            for (int i = 0; i < objPI.Pi.GetLength(1); i++)  // column
            {
                z = i + 1;  // item number

                for (int j = 0; j < objPI.Pi.GetLength(0); j++)  // row
                {
                    double tempPiVal = !CatRcs.Utils.CheckNaValues.IsNaNvalue(Pi[pi_index]) ? Pi[pi_index] : 0;

                    if (decimal.Round(Convert.ToDecimal(tempPiVal), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.Pi[j, i]), decimalPoint))
                    {
                        Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }

                    pi_index++;
                }
            }

            Assert.IsTrue(resultFlag);
            Console.WriteLine("Values of Pi are Matched!");

            #endregion

            #region "dPi"

            z = 0; pi_index = 0;
            resultFlag = true;
            Console.WriteLine("****** dPi ******");

            for (int i = 0; i < objPI.dPi.GetLength(1); i++)  // column
            {
                z = i + 1;  // item number

                for (int j = 0; j < objPI.dPi.GetLength(0); j++)  // row
                {
                    double tempPiVal = !CatRcs.Utils.CheckNaValues.IsNaNvalue(dPi[pi_index]) ? dPi[pi_index] : 0;

                    if (decimal.Round(Convert.ToDecimal(tempPiVal), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.dPi[j, i]), decimalPoint))
                    {
                        Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }

                    pi_index++;
                }
            }

            Assert.IsTrue(resultFlag);
            Console.WriteLine("Values of dPi are Matched!");

            #endregion

            #region "d2Pi"

            z = 0; pi_index = 0;
            resultFlag = true;
            Console.WriteLine("****** d2Pi ******");

            for (int i = 0; i < objPI.d2Pi.GetLength(1); i++)  // column
            {
                z = i + 1;  // item number

                for (int j = 0; j < objPI.d2Pi.GetLength(0); j++)  // row
                {
                    double tempPiVal = !CatRcs.Utils.CheckNaValues.IsNaNvalue(d2Pi[pi_index]) ? d2Pi[pi_index] : 0;

                    if (decimal.Round(Convert.ToDecimal(tempPiVal), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.d2Pi[j, i]), decimalPoint))
                    {
                        Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }

                    pi_index++;
                }
            }

            Assert.IsTrue(resultFlag);
            Console.WriteLine("Values of d2Pi are Matched!");

            #endregion

            #region "d3Pi"

            z = 0; pi_index = 0;
            resultFlag = true;
            Console.WriteLine("****** d3Pi ******");

            for (int i = 0; i < objPI.d3Pi.GetLength(1); i++)  // column
            {
                z = i + 1;  // item number

                for (int j = 0; j < objPI.d3Pi.GetLength(0); j++)  // row
                {
                    double tempPiVal = !CatRcs.Utils.CheckNaValues.IsNaNvalue(d3Pi[pi_index]) ? d3Pi[pi_index] : 0;

                    if (decimal.Round(Convert.ToDecimal(tempPiVal), decimalPoint) != decimal.Round(Convert.ToDecimal(objPI.d3Pi[j, i]), decimalPoint))
                    {
                        Console.WriteLine("Test for Item No. # " + z + " is not Passed!");
                        resultFlag = false;
                    }

                    pi_index++;
                }
            }

            Assert.IsTrue(resultFlag);
            Console.WriteLine("Values of d3Pi are Matched!");

            #endregion
        }
    }
}