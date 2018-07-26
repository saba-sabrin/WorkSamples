using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Core;
using CatRcs.Models;

namespace catRTest
{
    [TestFixture]
    public class GeneralTests
    {
        bool resultFlag = false;

        [Test]
        public void testModelEnums()
        {
            resultFlag = false;
            //string result = "";
            //result = ModelNames.EnumToString(ModelNames.Models.GRM);

            ModelNames.Models result = ModelNames.StringToEnum("MGRM");
            resultFlag = true;

            /*if (!String.IsNullOrEmpty(result))
            {
                resultFlag = true;
            }*/

            Assert.IsTrue(resultFlag, "Test Passed!");
        }

        [Test]
        public void testRandomNumber()
        {
            resultFlag = false;
            int size = 10;
            double[] result = new double[size];

            result = CatRcs.Utils.RandomNumberHandler.Runif(size);

            if(result != null)
            {
                resultFlag = true;
            }

            Assert.IsTrue(resultFlag, "Test Passed!");
        }
    }
}
