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
    public class Start_Next_Item_Test
    {
        bool resultFlag = false;
        private double testEpsilon = 0.0000001;
        int decimalPoint = 5;

        /* Testing is successful only for "MFI", Other options should be checked! */
        [Test]
        [TestCase(10)]
        public void test_StartItems_D(int numofItems)
        {
            resultFlag = true;

            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Dichotomous Items
            DataFrame DichoItems = engineObj.Evaluate("DichoItems <- genDichoMatrix(items = " + numofItems + ")").AsDataFrame();
            engineObj.SetSymbol("DichoItems", DichoItems);

            // Adapting with the existing "CAT-Items" type (Wrapper)
            CATItems itemBank = new CATItems(DichoItems[0].Length, false);

            // New Dictionary
            itemBank.SetItemParamter(CATItems.ColumnNames.a, DichoItems[0].Select(y => (double)y).ToArray());
            itemBank.SetItemParamter(CATItems.ColumnNames.b, DichoItems[1].Select(y => (double)y).ToArray());
            itemBank.SetItemParamter(CATItems.ColumnNames.c, DichoItems[2].Select(y => (double)y).ToArray());
            itemBank.SetItemParamter(CATItems.ColumnNames.d, DichoItems[3].Select(y => (double)y).ToArray());

            // Call "StartItems" function from R, "MFI" criterion
            GenericVector r_StartItems = engineObj.Evaluate("r_StartItems <- startItems(DichoItems, nrItems = 3, theta = 1, halfRange = 2)").AsList();

            // Resulting item numbers
            IntegerVector items = r_StartItems[0].AsInteger();

            DataFrame tempitems = r_StartItems[1].AsDataFrame();

            // Resulting Items
            CATItems r_CatItems = new CATItems(tempitems[0].Length, false);

            r_CatItems.SetItemParamter(CATItems.ColumnNames.a, tempitems[0].Select(y => (double)y).ToArray());
            r_CatItems.SetItemParamter(CATItems.ColumnNames.b, tempitems[1].Select(y => (double)y).ToArray());
            r_CatItems.SetItemParamter(CATItems.ColumnNames.c, tempitems[2].Select(y => (double)y).ToArray());
            r_CatItems.SetItemParamter(CATItems.ColumnNames.d, tempitems[3].Select(y => (double)y).ToArray());

            // Ability value
            NumericVector thStart = r_StartItems[2].AsNumeric();

            // Criterion
            CharacterVector startSelect = r_StartItems[3].AsCharacter();

            // Call "StartItems" function from CS
            StartItemsModel cs_StartItems = CatRLib.StartItems(itemBank, nrItems: 3, theta: 1, halfRange: 2);

            // Check selected items
            if(items.Length == cs_StartItems.items.Length)
            {
                for(int ind =0; ind < cs_StartItems.items.Length; ind++)
                {
                    if(items[ind] != cs_StartItems.items[ind])
                    {
                        resultFlag = false;
                    }
                }
            }

            // Check starting ability values
            if (thStart.Length == cs_StartItems.thStart.Length)
            {
                for (int ind2 = 0; ind2 < cs_StartItems.thStart.Length; ind2++)
                {
                    if (thStart[ind2] != cs_StartItems.thStart[ind2])
                    {
                        resultFlag = false;
                    }
                }
            }

            Assert.IsTrue(resultFlag);
        }

        /* Testing is successful only for "MFI", Other options should be checked! */
        [Test]
        [TestCase(10, ModelNames.Models.GRM)]
        public void test_StartItems_P(int numofItems, ModelNames.Models paramModel)
        {
            resultFlag = true;

            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Polytomous Items
            CharacterVector modelName = engineObj.CreateCharacterVector(new string[] { paramModel.EnumToString() });
            engineObj.SetSymbol("modelName", modelName);
            DataFrame PolyItems = engineObj.Evaluate("PolyItems <- genPolyMatrix(" + numofItems + ", 5, model = modelName, same.nrCat = TRUE)").AsDataFrame();
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

            // Call "StartItems" function from R, "MFI" criterion
            GenericVector r_StartItems = engineObj.Evaluate("r_StartItems <- startItems(PolyItems, model = modelName, nrItems = 3, theta = 1, halfRange = 2)").AsList();

            // Resulting item numbers
            IntegerVector items = r_StartItems[0].AsInteger();

            DataFrame tempitems = r_StartItems[1].AsDataFrame();

            // Resulting Items
            CATItems r_CatItems = new CATItems(tempitems[0].Length, paramModel.EnumToString(), 5);

            for (int i = 0; i < cols.Length; i++)
            {
                r_CatItems.SetItemParamter(cols[i], tempitems[i].Select(y => (double)y).ToArray());
            }

            // Resulting
            NumericVector thStart = r_StartItems[2].AsNumeric();
            CharacterVector startSelect = r_StartItems[3].AsCharacter();

            // Call "StartItems" function from CS
            StartItemsModel cs_StartItems = CatRLib.StartItems(itemBank, paramModel.EnumToString(), nrItems: 3, theta: 1, halfRange: 2);

            // Check items
            if (items.Length == cs_StartItems.items.Length)
            {
                for (int ind = 0; ind < cs_StartItems.items.Length; ind++)
                {
                    if (items[ind] != cs_StartItems.items[ind])
                    {
                        resultFlag = false;
                    }
                }
            }

            // Check starting ability values
            if (thStart.Length == cs_StartItems.thStart.Length)
            {
                for (int ind2 = 0; ind2 < cs_StartItems.thStart.Length; ind2++)
                {
                    if (thStart[ind2] != cs_StartItems.thStart[ind2])
                    {
                        resultFlag = false;
                    }
                }
            }

            Assert.IsTrue(resultFlag);
        }

        /* Testing is successful for "MFI", "bOpt", "thOpt", "MLWI", "MPWI", "MEI" */
        [Test]
        [TestCase(20, ModelNames.CriterionTypes.proportional)]
        public void test_NextItem_D(int numofItems, ModelNames.CriterionTypes paramCriterion)
        {
            resultFlag = true;

            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Dichotomous Items
            DataFrame DichoItems = engineObj.Evaluate("DichoItems <- genDichoMatrix(items = " + numofItems + ")").AsDataFrame();
            engineObj.SetSymbol("DichoItems", DichoItems);

            // Adapting with the existing "CAT-Items" type (Wrapper)
            CATItems itemBank = new CATItems(DichoItems[0].Length, false);

            // New Dictionary
            itemBank.SetItemParamter(CATItems.ColumnNames.a, DichoItems[0].Select(y => (double)y).ToArray());
            itemBank.SetItemParamter(CATItems.ColumnNames.b, DichoItems[1].Select(y => (double)y).ToArray());
            itemBank.SetItemParamter(CATItems.ColumnNames.c, DichoItems[2].Select(y => (double)y).ToArray());
            itemBank.SetItemParamter(CATItems.ColumnNames.d, DichoItems[3].Select(y => (double)y).ToArray());

            // Call "thetaEst" function
            //NumericVector r_th = engineObj.Evaluate("r_th <- thetaEst(rbind(DichoItems[3,], DichoItems[13,]), c(0, 1), method = \"WL\")").AsNumeric();
            //engineObj.SetSymbol("r_th", r_th);

            //double cs_th = CatRLib.ThetaEst(itemBank.FindItem(new int[] { 3, 13 }), x: new int[] { 0, 1 }, model: "", method: ModelNames.EstimaatorMethods.WL.EnumToString());

            // Call "NextItem" function from R with parameter "out"
            GenericVector r_NextItem = engineObj.Evaluate("r_NextItem <- nextItem(DichoItems, theta = 0, out = c(3, 13), criterion = \"" + paramCriterion.ToString() + "\")").AsList();

            // Call "NextItem" function from R with response pattern "x"
            //GenericVector r_NextItem = engineObj.Evaluate("r_NextItem <- nextItem(DichoItems, x = c(0, 1), out = c(3, 13), theta = r_th, criterion = \"" + paramCriterion.ToString() + "\")").AsList();

            // Selected item
            NumericVector item = r_NextItem[0].AsNumeric();

            DataFrame par = r_NextItem[1].AsDataFrame();

            // Item parameter
            CATItems parItems = new CATItems(par[0].Length, false);

            parItems.SetItemParamter(CATItems.ColumnNames.a, par[0].Select(y => (double)y).ToArray());
            parItems.SetItemParamter(CATItems.ColumnNames.b, par[1].Select(y => (double)y).ToArray());
            parItems.SetItemParamter(CATItems.ColumnNames.c, par[2].Select(y => (double)y).ToArray());
            parItems.SetItemParamter(CATItems.ColumnNames.d, par[3].Select(y => (double)y).ToArray());

            // Value of "info"
            NumericVector info = r_NextItem[2].AsNumeric();

            // Criterion
            CharacterVector startSelect = r_NextItem[3].AsCharacter();

            // Call "NextItem" function from CS with parameter "out"
            NextItemModel cs_NextItem = CatRLib.NextItem(itemBank, theta: 0, Out: new int[]{ 3, 13 }, criterion: (int)paramCriterion);

            // Call "NextItem" function from CS with response pattern "x"
            //NextItemModel cs_NextItem = CatRLib.NextItem(itemBank, theta: cs_th, Out: new int[] { 3, 13 }, x: new int[] { 0, 1 }, criterion: (int)paramCriterion);

            // Checking item & other values
            if (item[0] != cs_NextItem.item)
            {
                resultFlag = false;
            }

            if(decimal.Round((decimal)info[0], 3) != decimal.Round((decimal)cs_NextItem.info, 3))
            {
                resultFlag = false;
            }

            Assert.IsTrue(resultFlag);
        }

        /* Testing is successful only for "MFI", Other options should be checked! */
        [Test]
        [TestCase(10, ModelNames.Models.GRM, ModelNames.CriterionTypes.MFI)]
        public void test_NextItem_P(int numofItems, ModelNames.Models paramModel, ModelNames.CriterionTypes paramCriterion)
        {
            resultFlag = true;

            REngine.SetEnvironmentVariables();

            REngine engineObj = REngine.GetInstance();

            // Loading a library from R
            engineObj.Evaluate("library(catR)");

            // Polytomous Items
            CharacterVector modelName = engineObj.CreateCharacterVector(new string[] { paramModel.EnumToString() });
            engineObj.SetSymbol("modelName", modelName);
            DataFrame PolyItems = engineObj.Evaluate("PolyItems <- genPolyMatrix(" + numofItems + ", 5, model = modelName, same.nrCat = TRUE)").AsDataFrame();
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

            // Call "NextItem" function from R
            GenericVector r_NextItem = engineObj.Evaluate("r_NextItem <- nextItem(PolyItems, model = modelName, theta = 0, criterion = \"" + paramCriterion.ToString() + "\")").AsList();

            // Selected item
            NumericVector item = r_NextItem[0].AsNumeric();

            DataFrame par = r_NextItem[1].AsDataFrame();

            // Value of "info"
            NumericVector thStart = r_NextItem[2].AsNumeric();

            // Criterion
            CharacterVector startSelect = r_NextItem[3].AsCharacter();

            // Call "NextItem" function from CS
            NextItemModel cs_NextItem = CatRLib.NextItem(itemBank, model: paramModel.EnumToString(), theta: 0, criterion: (int)paramCriterion);

            //Test passed for "MFI"

            if (item[0] != cs_NextItem.item)
            {
                resultFlag = false;
            }

            Assert.IsTrue(resultFlag);
        }
    }
}
