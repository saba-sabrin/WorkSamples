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

namespace catRTest
{
    [TestFixture]
    public class Integrate_catR_Test
    {
        [Test]
        public void testIntegrateCatR()
        {
            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            //Sequence generation
            NumericVector x = engineObj.Evaluate("x <- seq(from = -4, to = 4, length = 33)").AsNumeric();
            engineObj.SetSymbol("x", x);

            double[] x_val = x.Select(p => (double)p).ToArray();

            NumericVector y = engineObj.Evaluate("y <- exp(x)").AsNumeric();
            engineObj.SetSymbol("y", y);

            double[] y_val = y.Select(p => (double)p).ToArray();

            // Call function from R
            NumericVector result_Integrate = engineObj.Evaluate("result_Integrate <- integrate.catR(x, y)").AsNumeric();
            engineObj.SetSymbol("result_Integrate", result_Integrate);

            // Call function from inside
            double result = CatRLib.Integrate_catR(x_val, y_val);

            decimal temp1 = decimal.Round(Convert.ToDecimal(result_Integrate[0]), 4);
            decimal temp2 = decimal.Round(Convert.ToDecimal(result), 4);

            decimal errFraction = 0;

            if (temp1 > temp2)
            {
                errFraction = temp1 - temp2;
            }
            else if (temp2 > temp1)
            {
                errFraction = temp2 - temp1;
            }
            else
            {
                Assert.AreEqual(temp1, temp2);
            }
        }
    }
}
