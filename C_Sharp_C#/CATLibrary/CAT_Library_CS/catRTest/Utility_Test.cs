using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Core;
using CatRcs.Utils;
using RDotNet;

namespace catRTest
{
    [TestFixture]
    public class Utility_Test
    {
        private bool resultFlag = false;

        [Test]
        [TestCase(new double[] { 4.00, 1.90, 2.10, double.NaN, 0.15, 8.9, 45.54, 23.95, 45.37 })]
        public void testRange(double[] paramValue)
        {
            resultFlag = false;

            double[] result = CatRcs.Utils.Maths.Range(paramValue, true, false);

            if (result != null)
            {
                resultFlag = true;
            }

            Assert.IsTrue(resultFlag, "Range is valid & Passed!");
        }

        [Test]
        public void testReplicate()
        {
            resultFlag = false;
            double[] paramValue = new double[] { 4.00, 1.90, 2.10 };

            double[] result = CatRcs.Utils.CommonHelper.Replicate(paramValue, 2, 2, 5);

            if (result != null)
            {
                resultFlag = true;
            }

            Assert.IsTrue(resultFlag, "Test Passed!");
        }

        [Test]
        public void testSequence()
        {
            resultFlag = true;

            REngine.SetEnvironmentVariables();
            REngine engineObj = REngine.GetInstance();
             
            // TODO: add more tests with different values for from, to and length and using the by parameter

            double[] result = CatRcs.Utils.CommonHelper.Sequence(-4, 4, length: 33);
            NumericVector x = engineObj.Evaluate("x <- seq(from = -4, to = 4, length = 33)").AsNumeric();

            if (result == null)
            {
                resultFlag = false;
            }
            else
            {
                // For arrays always compare all elements(!)

                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i] != x[i])
                    {
                        resultFlag = false;
                        break;
                    }
                        
                }

            }
             
            Assert.IsTrue(resultFlag, "Test Passed!");
        }

        [Test]
        public void testSample()
        {
            resultFlag = true;
            int[] paramArray = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            int[] result1 = CatRcs.Utils.RandomNumberHandler.Sample(paramArray, 10);

            int[] result2 = CatRcs.Utils.RandomNumberHandler.Sample(paramArray, 10);

            int[] result3 = CatRcs.Utils.RandomNumberHandler.Sample(paramArray, 2);

            if (result1 == null)
            {
                resultFlag = false;
            }

            Assert.IsTrue(resultFlag, "Test Passed!");
        }

        [Test]
        public void testDnorm()
        {
            resultFlag = false;

            double result = CatRcs.Utils.CommonHelper.Dnorm(0);

            if (result != 0)
            {
                resultFlag = true;
            }

            Assert.IsTrue(resultFlag, "Test Passed!");
        }

        [Test]
        public void testDunif()
        {
            resultFlag = false;
            //double[] paramArray = new double[] { 4.00, 1.90, 2.10 };
            double[] paramArray = new double[] { 0.6966087, 0.8011419, 0.1377303, 0.1597848, 0.9888336, 0.3986457, 0.1879118, 0.4777282, 0.5442340, 0.3281580 };

            double[] result = CatRcs.Utils.CommonHelper.Dunif(paramArray, 0, 0.5);

            if (result != null)
            {
                resultFlag = true;
            }

            Assert.IsTrue(resultFlag, "Test Passed!");
        }

        [Test]
        public void testRank()
        {
            resultFlag = false;
            //double[] paramArray = new double[] { 4.00, 1.90, 2.10 };
            double[] paramArray = new double[] { 3, 1, 4, 7, 2 };

            double[] result = CatRcs.Utils.CommonHelper.Rank(paramArray);

            if (result != null)
            {
                resultFlag = true;
            }

            Assert.IsTrue(resultFlag, "Test Passed!");
        }

        [Test]
        public void testUnique()
        {
            resultFlag = false;
            //double[] paramArray = new double[] { 4.00, 1.90, 2.10 };
            double[] paramArray = new double[] { 3, 1, 4, 7, 2, 1, 7, 4};

            double[] result = CatRcs.Utils.CommonHelper.Unique(paramArray);

            if (result != null)
            {
                resultFlag = true;
            }

            Assert.IsTrue(resultFlag, "Test Passed!");
        }

        [Test]
        public void testMaximaMinima()
        {
            resultFlag = false;
            double[] paramArray1 = new double[] { 5, 4, 3, 2, 1 };
            double[] paramArray2 = new double[] { 1, 2, 3, 4, 5 };

            double[] result = CatRcs.Utils.Maths.PMax(paramArray1, paramArray2);

            result = CatRcs.Utils.Maths.PMin(paramArray1, paramArray2);

            if (result != null)
            {
                resultFlag = true;
            }

            Assert.IsTrue(resultFlag, "Test Passed!");
        }

        [Test]
        public void testUniroot()
        {

        }
    }
}
