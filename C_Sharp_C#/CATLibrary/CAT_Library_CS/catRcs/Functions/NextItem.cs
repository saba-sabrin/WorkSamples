using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatRcs.Models;
using CatRcs.Utils;

namespace CatRcs
{
    internal class NextItem
    {
        // Function for calculating Next Item
        public static NextItemModel NextItem_Calc(CATItems itemBank, string model = null, double theta = 0, int[] Out = null, int[] x = null, int criterion = 5, /* MFI */ string method = "BM",
            string priorDist = "norm", double[] priorPar = null, double D = 1, double[] range = null, int[] parInt = null, int infoType = 2, /* observed */ int randomesque = 1, int rule = 1, /* Length */
            double thr = 20, double? SETH = null, double AP = 1, int[] nAvailable = null, int maxItems = 50, CBControlList cbControl = null, string[] cbGroup = null)
        {
            CATItems par = null;
            NextItemModel result = null;

            #region "Parameter Validation"

            if (priorPar == null || priorPar.Length < 2)
            {
                priorPar = new double[2];
                priorPar[0] = 0;
                priorPar[1] = 1;
            }

            if (range == null || range.Length < 2)
            {
                range = new double[2];
                range[0] = -4;
                range[1] = 4;
            }

            if (parInt == null || parInt.Length < 3)
            {
                parInt = new int[3];
                parInt[0] = -4;
                parInt[1] = 4;
                parInt[2] = 33;
            }

            ModelNames.CriterionTypes? crit = null;

            switch(criterion)
            {
                case (int)ModelNames.CriterionTypes.bOpt:
                    crit = ModelNames.CriterionTypes.bOpt;
                break;
                case (int)ModelNames.CriterionTypes.thOpt:
                    crit = ModelNames.CriterionTypes.thOpt;
                break;
                case (int)ModelNames.CriterionTypes.KL:
                    crit = ModelNames.CriterionTypes.KL;
                break;
                case (int)ModelNames.CriterionTypes.KLP:
                    crit = ModelNames.CriterionTypes.KLP;
                break;
                case (int)ModelNames.CriterionTypes.MEI:
                    crit = ModelNames.CriterionTypes.MEI;
                break;
                case (int)ModelNames.CriterionTypes.MEPV:
                    crit = ModelNames.CriterionTypes.MEPV;
                break;
                case (int)ModelNames.CriterionTypes.MFI:
                    crit = ModelNames.CriterionTypes.MFI;
                break;
                case (int)ModelNames.CriterionTypes.MLWI:
                    crit = ModelNames.CriterionTypes.MLWI;
                break;
                case (int)ModelNames.CriterionTypes.MPWI:
                    crit = ModelNames.CriterionTypes.MPWI;
                break;
                case (int)ModelNames.CriterionTypes.progressive:
                    crit = ModelNames.CriterionTypes.progressive;
                break;
                case (int)ModelNames.CriterionTypes.proportional:
                    crit = ModelNames.CriterionTypes.proportional;
                break;
                case (int)ModelNames.CriterionTypes.random:
                    crit = ModelNames.CriterionTypes.random;
                break;
            }

            if(crit == null)
            {
                result = new NextItemModel(true, "Invalid 'criterion' name!");
                return result;
            }

            int mod = 0;
            ModelNames.Models modelEnum = ModelNames.StringToEnum(model);

            if(!String.IsNullOrEmpty(model))
            {
                switch(modelEnum)
                {
                    case ModelNames.Models.GRM:
                        mod = 1;
                    break;
                    case ModelNames.Models.MGRM:
                        mod = 2;
                    break;
                    case ModelNames.Models.PCM:
                        mod = 3;
                    break;
                    case ModelNames.Models.GPCM:
                        mod = 4;
                    break;
                    case ModelNames.Models.RSM:
                        mod = 5;
                    break;
                    case ModelNames.Models.NRM:
                        mod = 6;
                    break;
                }

                if(mod == 0)
                {
                    result = new NextItemModel(true, "Invalid 'model' type!");
                    return result;
                }
            }

            #endregion

            int[] OUT = null;
            double[] empProp = null;
            int nrGroup = 0;
            double[] thProp = null;

            #region Handling "cbControl" Parameter

            if (cbControl == null)
            {
               OUT  = Out;
            }
            else
            {
                if(cbGroup == null)
                {
                    result = new NextItemModel(true, "'cbGroup' argument must be provided for content balancing!");
                    return result;
                }

                if(RowColumn.Sum(cbControl.Props) != 1)
                {
                    double temp_sum = RowColumn.Sum(cbControl.Props);

                    for(int i = 0; i < cbControl.Props.Length; i++)
                    {
                        cbControl.Props[i] = cbControl.Props[i] / temp_sum;
                    }
                }

                nrGroup = cbControl.Names.Length;

                empProp = new double[nrGroup];

                if(Out == null)
                {
                    empProp = CatRcs.Utils.CommonHelper.Replicate(new double[] { 0 }, nrGroup);
                }
                else
                {
                    string[] temp_grp = new string[Out.Length];

                    for (int i = 0; i < Out.Length; i++)
                    {
                        temp_grp[i] = cbGroup[Out[i]];
                    }

                    for(int j = 0; j < nrGroup; j++)
                    {
                        string[] values = temp_grp.Where(m => m == cbControl.Names[j]).ToArray();

                        if (values != null && values.Length > 0)
                        {
                            empProp[j] = values.Length;
                        }
                        else
                        {
                            empProp[j] = 0;
                        }
                    }

                    empProp = empProp.Select(n => n / CatRcs.Utils.RowColumn.Sum(empProp)).ToArray();  // Functional Testing needed !!
                }

                thProp = cbControl.Props;

                List<int> indGroup = new List<int>(); int selGroup = 0;

                // Array of Group Numbers ex: "1 2 3 4 5" for nrGroup = 5.
                int[] GrpIndValues = new int[nrGroup];
                GrpIndValues = GrpIndValues.Select((a, i) => i + 1).ToArray();

                if(empProp.Min() == 0)
                {
                    for(int m =0; m < empProp.Length; m++)
                    {
                        if(empProp[m] == 0)
                        {
                            indGroup.Add(GrpIndValues[m]);
                        }
                    }
                }
                else
                {
                    double[] tempProp = thProp.Select((a, i) => a - empProp[i]).ToArray();

                    for (int n = 0; n < tempProp.Length; n++)
                    {
                        if (tempProp[n] == tempProp.Max())
                        {
                            indGroup.Add(GrpIndValues[n]);
                        }
                    }
                }

                if (indGroup.Count == 1)
                {
                    selGroup = indGroup[0];
                }
                else
                {
                    selGroup = CatRcs.Utils.RandomNumberHandler.Sample(indGroup.ToArray(), 1, false)[0];
                }

                // Populating the OUT array
                string[] tempGrp = cbGroup.Where(c => c != cbControl.Names[selGroup]).ToArray();
                OUT = tempGrp.Select((a, i) => i + 1).ToArray();
                OUT = Out.Concat(OUT).ToArray();
                OUT = CatRcs.Utils.CommonHelper.Unique(OUT);
            }

            #endregion

            #region Handling "nAvalilable Parameter"

            if (nAvailable != null)
            {
                List<int> ind_temp = new List<int>();

                for(int k = 0; k < nAvailable.Length; k++)
                {
                    if(nAvailable[k] == 0)
                    {
                        ind_temp.Add(k);
                    }
                }

                OUT = OUT.Concat(ind_temp.ToArray()).ToArray();
                OUT = CatRcs.Utils.CommonHelper.Unique(OUT);
            }

            #endregion

            int select = 0;

            #region Criterion Type "MFI"

            if (crit == ModelNames.CriterionTypes.MFI)
            {
                int[] items = CatRcs.Utils.CommonHelper.Replicate(new int[] { 1 }, itemBank.NumOfItems);

                if (OUT != null)
                {
                    for (int a = 0; a < OUT.Length; a++)
                    {
                        items[OUT[a] - 1] = 0;
                    }
                }

                double[] info = Ii.Ii_Calc(theta, itemBank, model, D).Ii;
                double[] ranks = CatRcs.Utils.CommonHelper.Rank(info).Select(m => m).ToArray();
                int nrIt = new int[]{ randomesque, (int)CatRcs.Utils.RowColumn.Sum(items) }.Min();

                List<double> tempRanks = new List<double>();

                for(int j =0; j < items.Length; j++)
                {
                    if(items[j] == 1)
                    {
                        tempRanks.Add(ranks[j]);
                    }
                }

                tempRanks = tempRanks.OrderByDescending(n => n).ToList();
                double[] keepRank = tempRanks.GetRange(0, nrIt).ToArray();

                List<int> keep = new List<int>();

                if(ranks.Length == items.Length)
                {
                    for(int m = 0; m < keepRank.Length; m++)
                    {
                        for(int n = 0; n < ranks.Length; n++)
                        {
                            if((items[n] == 1) && (ranks[n] == keepRank[m]))
                            {
                                keep.Add(n+1);
                            }
                        }
                    }
                }
                
                if(keep.Count == 1)
                {
                    select = keep[0];
                }
                else
                {
                    select = CatRcs.Utils.RandomNumberHandler.Sample(keep.ToArray(), 1, false)[0];
                }

                result = new NextItemModel(select, itemBank.FindItem(select), info[select-1], criterion, randomesque);
            }

            #endregion

            #region Criterion Type "bOpt"

            if (crit == ModelNames.CriterionTypes.bOpt)
            {
                if(string.IsNullOrEmpty(model))
                {
                    int[] items = CatRcs.Utils.CommonHelper.Replicate(new int[] { 1 }, itemBank.NumOfItems);

                    if (OUT != null)
                    {
                        for (int a = 0; a < OUT.Length; a++)
                        {
                            items[OUT[a] - 1] = 0;
                        }
                    }

                    double[] distance = itemBank.GetItemParamter(CATItems.ColumnNames.b).Select(a => Math.Abs(a - theta)).ToArray();
                    double[] ranks = CatRcs.Utils.CommonHelper.Rank(distance).Select(m => m).ToArray();

                    if (OUT != null)
                    {
                        for (int a = 0; a < OUT.Length; a++)
                        {
                            ranks[OUT[a] - 1] = -1;
                        }
                    }

                    int nrIt = new int[] { randomesque, (int)CatRcs.Utils.RowColumn.Sum(items) }.Min();

                    List<double> tempRanks = new List<double>();

                    for (int j = 0; j < items.Length; j++)
                    {
                        if (items[j] == 1)
                        {
                            tempRanks.Add(ranks[j]);
                        }
                    }

                    tempRanks = tempRanks.OrderBy(n => n).ToList();
                    double[] keepRank = tempRanks.GetRange(0, nrIt).ToArray();
                    keepRank = keepRank.Distinct().ToArray();

                    List<int> keep = new List<int>();

                    if (ranks.Length == items.Length)
                    {
                        for (int m = 0; m < keepRank.Length; m++)
                        {
                            for (int n = 0; n < ranks.Length; n++)
                            {
                                if ((items[n] == 1) && (ranks[n] == keepRank[m]))
                                {
                                    keep.Add(n + 1);
                                }
                            }
                        }
                    }

                    if (keep.Count == 1)
                    {
                        select = keep[0];
                    }
                    else
                    {
                        select = CatRcs.Utils.RandomNumberHandler.Sample(keep.ToArray(), 1, false)[0];
                    }

                    result = new NextItemModel(select, itemBank.FindItem(select), distance[select - 1], criterion, randomesque);
                }
                else
                {
                    result = new NextItemModel(true, "bOpt's rule cannot be considered with polytomous items!");
                    return result;
                }
            }

            #endregion

            #region Criterion Type "MLWI" OR "MPWI"

            if (crit == ModelNames.CriterionTypes.MLWI || crit == ModelNames.CriterionTypes.MPWI)
            {
                if (Out != null)
                {
                    if (Out.Length == 1)
                    {
                        par = itemBank.FindItem(Out[0]);
                    }
                    else
                    {
                        par = itemBank.FindItem(Out);
                    }
                }
                else
                {
                    result = new NextItemModel(true, "Out parameter can't be empty!");
                    return result;
                }

                int[] items = CatRcs.Utils.CommonHelper.Replicate(new int[] { 1 }, itemBank.NumOfItems);

                if (OUT != null)
                {
                    for (int a = 0; a < OUT.Length; a++)
                    {
                        items[OUT[a] - 1] = 0;
                    }
                }

                double[] likInfo = CatRcs.Utils.CommonHelper.Replicate(new double[] { 0 }, itemBank.NumOfItems);
                
                int mwiType = 0;

                if(criterion == (int)ModelNames.CriterionTypes.MLWI)
                {
                    mwiType = (int)ModelNames.MWI_Type.MLWI;
                }
                if(criterion == (int)ModelNames.CriterionTypes.MPWI)
                {
                    mwiType = (int)ModelNames.MWI_Type.MPWI;
                }

                if (x != null)
                {
                    for(int j = 0; j < itemBank.NumOfItems; j++)
                    {
                        if(items[j] == 1)
                        {
                            likInfo[j] = MWI.MWI_Calc(itemBank, j + 1, x, par, model, mwiType, priorPar, D, priorDist, parInt[0], parInt[1], parInt[2]);
                            /* item number is always 1 greater than the index */
                        }
                    }
                }

                int nrIt = new int[] { randomesque, (int)CatRcs.Utils.RowColumn.Sum(items) }.Min();

                double likVal = likInfo.ToList().OrderByDescending(n => n).ToArray()[nrIt - 1]; // First value with index 0

                List<int> keep = new List<int>();

                for(int k = 0; k < items.Length; k++)
                {
                    if(likInfo[k] >= likVal)
                    {
                        keep.Add(k + 1);  // Converting from index to item number
                    }
                }

                if (keep.Count == 1)
                {
                    select = keep[0];
                }
                else
                {
                    select = CatRcs.Utils.RandomNumberHandler.Sample(keep.ToArray(), 1, false)[0];
                }

                result = new NextItemModel(select, itemBank.FindItem(select), likInfo[select - 1], criterion, randomesque);
            }

            #endregion

            #region Criterion Type "KL" OR "KLP"

            if (crit == ModelNames.CriterionTypes.KL || crit == ModelNames.CriterionTypes.KLP)
            {
                if (Out != null)
                {
                    if (Out.Length == 1)
                    {
                        par = itemBank.FindItem(Out[0]);
                    }
                    else
                    {
                        par = itemBank.FindItem(Out);
                    }
                }
                else
                {
                    result = new NextItemModel(true, "Out parameter can't be empty!");
                    return result;
                }

                int[] items = CatRcs.Utils.CommonHelper.Replicate(new int[] { 1 }, itemBank.NumOfItems);

                if (OUT != null)
                {
                    for (int a = 0; a < OUT.Length; a++)
                    {
                        items[OUT[a] - 1] = 0;
                    }
                }

                double[] klValue = CatRcs.Utils.CommonHelper.Replicate(new double[] { 0 }, itemBank.NumOfItems);
                double[] X = CatRcs.Utils.CommonHelper.Sequence(parInt[0], parInt[1], parInt[2]);

                #region "Function 'L' "
                Func<double, int[], CATItems, double> L = (th, r, param) =>
                {
                    double res = 0;

                    var temp_1 = Pi.Pi_Calc(th, param, model, D).Pi.Select((p, i) => Math.Pow(p, r[i])).ToArray();

                    var temp_2 = Pi.Pi_Calc(th, param, model, D).Pi.Select((p, i) => Math.Pow(1 - p, 1 - r[i])).ToArray();

                    if (temp_1.Length == temp_2.Length)
                    {
                        var temp_3 = temp_1.Select((p, i) => p * temp_2[i]).ToArray();

                        res = temp_3.Aggregate((acc, val) => acc * val);
                    }

                    return res;
                };
                #endregion

                #region "Function 'LL' "
                Func<double, int[], CATItems, double> LL = (th, r, param) =>
                {
                    double res = 0;

                    if(param.NumOfItems == 0)
                    {
                        res = 1;
                    }
                    else
                    {
                        double[,] prob = Pi.Pi_Poly_Calc(th, param, model, D).Pi;

                        for (int i = 0; i < r.Length; i++)
                        {
                            res = res * prob[i, r[i] + 1];
                        }
                    }

                    return res;
                };
                #endregion

                double[] LF = null;

                if(string.IsNullOrEmpty(model))
                {
                    LF = X.Select(p => L(p, x, par)).ToArray();
                }
                else
                {
                    LF = X.Select(p => LL(p, x, par)).ToArray();
                }

                int kType = 0;

                if (criterion == (int)ModelNames.CriterionTypes.KL)
                {
                    kType = (int)ModelNames.KLTypes.KL;
                }
                if (criterion == (int)ModelNames.CriterionTypes.KLP)
                {
                    kType = (int)ModelNames.KLTypes.KLP;
                }

                for (int j = 0; j < itemBank.NumOfItems; j++)
                {
                    if (items[j] == 1)
                    {
                        klValue[j] = KL.KL_Calc(itemBank, j + 1, x, par, model, theta, priorPar, X, LF, kType, D, priorDist, parInt[0], parInt[1], parInt[2]);
                        /* item number is always 1 greater than the index */
                    }
                }

                int nrIt = new int[] { randomesque, (int)CatRcs.Utils.RowColumn.Sum(items) }.Min();

                double klVal = klValue.ToList().OrderByDescending(n => n).ToArray()[nrIt - 1];

                List<int> keep = new List<int>();

                for (int k = 0; k < items.Length; k++)
                {
                    if (klValue[k] >= klVal)
                    {
                        keep.Add(k + 1);
                    }
                }

                if (keep.Count == 1)
                {
                    select = keep[0];
                }
                else
                {
                    select = CatRcs.Utils.RandomNumberHandler.Sample(keep.ToArray(), 1, false)[0];
                }

                result = new NextItemModel(select, itemBank.FindItem(select), klValue[select - 1], criterion, randomesque);
            }

            #endregion

            #region Criterion Type "MEI"

            if (crit == ModelNames.CriterionTypes.MEI)
            {
                int[] items = CatRcs.Utils.CommonHelper.Replicate(new int[] { 1 }, itemBank.NumOfItems);

                if (OUT != null)
                {
                    for (int a = 0; a < OUT.Length; a++)
                    {
                        items[OUT[a] - 1] = 0;
                    }
                }

                double[] infos = CatRcs.Utils.CommonHelper.Replicate(new double[] { 0 }, itemBank.NumOfItems);

                for (int j = 0; j < items.Length; j++)
                {
                    if (items[j] > 0)
                    {
                        infos[j] = MEI.MEI_Calc(itemBank, j + 1, x, theta, itemBank.FindItem(Out), model, method, D, priorPar, priorDist, range, parInt, infoType);
                    }
                }

                int nrIt = new int[] { randomesque, (int)CatRcs.Utils.RowColumn.Sum(items) }.Min();

                double infoVal = infos.ToList().OrderByDescending(n => n).ToArray()[nrIt - 1];

                List<int> keep = new List<int>();

                for (int k = 0; k < items.Length; k++)
                {
                    if (infos[k] >= infoVal)
                    {
                        keep.Add(k + 1);
                    }
                }

                if (keep.Count == 1)
                {
                    select = keep[0];
                }
                else
                {
                    select = CatRcs.Utils.RandomNumberHandler.Sample(keep.ToArray(), 1, false)[0];
                }

                result = new NextItemModel(select, itemBank.FindItem(select), infos[select - 1], criterion, randomesque);
            }

            #endregion

            #region Criterion Type "MEPV"

            if (crit == ModelNames.CriterionTypes.MEPV)
            {
                int[] items = CatRcs.Utils.CommonHelper.Replicate(new int[] { 1 }, itemBank.NumOfItems);

                if (OUT != null)
                {
                    for (int a = 0; a < OUT.Length; a++)
                    {
                        items[OUT[a] - 1] = 0;
                    }
                }

                double[] epvs = CatRcs.Utils.CommonHelper.Replicate(new double[] { 1000 }, itemBank.NumOfItems);

                for (int j = 0; j < items.Length; j++)
                {
                    if (items[j] > 0)
                    {
                        epvs[j] = EPV.EPV_Calc(itemBank, j + 1, x, theta, itemBank.FindItem(Out), model, priorPar, parInt, D, priorDist);
                    }
                }

                var tempVal = new int[] { randomesque, items.Sum() }.Min();
                double epVal = epvs.ToList().OrderBy(n => n).ToArray()[tempVal - 1];

                List<int> keep = new List<int>();

                for (int k = 0; k < itemBank.NumOfItems; k++)
                {
                    if (epvs[k] <= epVal)
                    {
                        keep.Add(k + 1);
                    }
                }

                if (keep.Count == 1)
                {
                    select = keep[0];
                }
                else
                {
                    select = CatRcs.Utils.RandomNumberHandler.Sample(keep.ToArray(), 1, false)[0];
                }

                result = new NextItemModel(select, itemBank.FindItem(select), epvs[select - 1], criterion, randomesque);
            }

            #endregion

            #region Criterion Type "random"

            if (crit == ModelNames.CriterionTypes.random)
            {
                int[] items = CatRcs.Utils.CommonHelper.Replicate(new int[] { 1 }, itemBank.NumOfItems);

                if (OUT != null)
                {
                    for (int a = 0; a < OUT.Length; a++)
                    {
                        items[OUT[a]] = 0;
                    }
                }

                int gen = Convert.ToInt32(Convert.ToInt32(CatRcs.Utils.RandomNumberHandler.Runif(1, 0, 1)[0]) * items.Sum()) + 1;

                List<int> indexs = new List<int>();

                for (int k = 0; k < itemBank.NumOfItems; k++)
                {
                    if (items[k] > 0)
                    {
                        indexs.Add(k + 1);
                    }
                }

                select = indexs.ElementAt(gen);

                result = new NextItemModel(select, itemBank.FindItem(select), double.NaN, criterion, randomesque);
            }

            #endregion

            #region Criterion Type "progressive"

            if (crit == ModelNames.CriterionTypes.progressive)
            {
                int item_administered = Out.Length;
                int[] items = CatRcs.Utils.CommonHelper.Replicate(new int[] { 1 }, itemBank.NumOfItems);

                if (OUT != null)
                {
                    for (int a = 0; a < OUT.Length; a++)
                    {
                        items[OUT[a] - 1] = 0;
                    }
                }

                double[] info = Ii.Ii_Calc(theta, itemBank, model, D).Ii;
                List<double> tempItems = new List<double>();

                for(int j =0; j < items.Length; j++)
                {
                    if(items[j] == 1)
                    {
                        tempItems.Add(info[j]);
                    }
                }

                double wq = 0;

                double itemMaxInfo = tempItems.Max();
                double[] randomValues = CatRcs.Utils.RandomNumberHandler.Runif(items.Length, 0, itemMaxInfo);

                if(rule == (int)ModelNames.RuleType.Precision)
                {
                    double infostop = Math.Pow((1 / thr), 2);
                    double cuminfo = Math.Pow(double.Parse((1 / SETH).ToString()), 2);

                    if(item_administered > 0)
                    {
                        wq = Math.Pow(new double[] { cuminfo / infostop, item_administered / (maxItems - 1) }.Max(), AP);
                    }
                }

                if (rule == (int)ModelNames.RuleType.Length)
                {
                    if (item_administered > 0)
                    {
                        List<double> tempNum = new List<double>();

                        for(int i = 1; i <= item_administered; i++)
                        {
                            tempNum.Add(Math.Pow(i, AP));
                        }

                        double numerador = tempNum.Sum();

                        List<double> tempDenom = new List<double>();

                        for (int j = 1; j <= thr - 1; j++)
                        {
                            tempDenom.Add(Math.Pow(j, AP));
                        }

                        double denominador = tempDenom.Sum();

                        wq = numerador / denominador;
                    }
                }

                double[] funcPR = info.Select((d, i) => d * wq + randomValues[i] * (1 - wq)).ToArray();

                if (OUT != null)
                {
                    for (int a = 0; a < OUT.Length; a++)
                    {
                        funcPR[OUT[a] - 1] = 0;
                    }
                }

                List<int> keep = new List<int>();

                for (int k = 0; k < funcPR.Length; k++)
                {
                    if (funcPR[k] == funcPR.Max())
                    {
                        keep.Add(k + 1);
                    }
                }

                if (keep.Count == 1)
                {
                    select = keep[0];
                }
                else
                {
                    select = CatRcs.Utils.RandomNumberHandler.Sample(keep.ToArray(), 1, false)[0];
                }

                result = new NextItemModel(select, itemBank.FindItem(select), info[select - 1], criterion, randomesque);
            }

            #endregion

            #region Criterion Type "proportional"

            if (crit == ModelNames.CriterionTypes.proportional)
            {
                int item_administered = Out.Length;
                int[] items = CatRcs.Utils.CommonHelper.Replicate(new int[] { 1 }, itemBank.NumOfItems);

                if (OUT != null)
                {
                    for (int a = 0; a < OUT.Length; a++)
                    {
                        items[OUT[a] - 1] = 0;
                    }
                }

                double wq = 0;

                if (rule == (int)ModelNames.RuleType.Precision)
                {
                    double infostop = Math.Pow((1 / thr), 2);
                    double cuminfo = Math.Pow(double.Parse((1 / SETH).ToString()), 2);

                    if (item_administered > 0)
                    {
                        wq = infostop * Math.Pow(new double[] { cuminfo / infostop, item_administered / (maxItems - 1) }.Max(), AP);
                    }
                }

                if (rule == (int)ModelNames.RuleType.Length)
                {
                    if (item_administered > 0)
                    {
                        List<double> tempNum = new List<double>();

                        for (int i = 1; i <= item_administered; i++)
                        {
                            tempNum.Add(Math.Pow(i, AP));
                        }

                        double numerador = tempNum.Sum();

                        List<double> tempDenom = new List<double>();

                        for (int j = 1; j <= thr - 1; j++)
                        {
                            tempDenom.Add(Math.Pow(j, AP));
                        }

                        double denominador = tempDenom.Sum();

                        wq = thr * numerador / denominador;
                    }
                }

                double[] info = Ii.Ii_Calc(theta, itemBank, model, D).Ii;
                double[] infoPR = info.Select(s => Math.Pow(s, wq)).ToArray();

                if (OUT != null)
                {
                    for (int a = 0; a < OUT.Length; a++)
                    {
                        infoPR[OUT[a] - 1] = 0;
                    }
                }

                List<double> tempInfoPR = new List<double>();

                for(int k = 0; k < items.Length; k++)
                {
                    if(items[k] == 1)
                    {
                        tempInfoPR.Add(infoPR[k]);
                    }
                }

                double totalInfoPR = tempInfoPR.Sum();
                double[] probSelect = infoPR.Select(m => m / totalInfoPR).ToArray();

                int[] selectItems = items.Select((n, i) => i + 1).ToArray();

                select = CatRcs.Utils.RandomNumberHandler.Sample(selectItems, 1, false)[0];  // prob parameter will be added after Sample functiom modification

                result = new NextItemModel(select, itemBank.FindItem(select), info[select], criterion, randomesque);
            }

            #endregion

            #region Criterion Type "thOpt"

            if (crit == ModelNames.CriterionTypes.thOpt)
            {
                if(string.IsNullOrEmpty(model))  // Only for Dichotomous Items
                {
                    int[] items = CatRcs.Utils.CommonHelper.Replicate(new int[] { 1 }, itemBank.NumOfItems);

                    if (OUT != null)
                    {
                        for (int a = 0; a < OUT.Length; a++)
                        {
                            items[OUT[a] - 1] = 0;
                        }
                    }

                    double[] u = itemBank.GetItemParamter(CATItems.ColumnNames.c).Select((s, i) => -0.75 + (s + itemBank.GetItemParamter(CATItems.ColumnNames.d)[i] + -2 * s * itemBank.GetItemParamter(CATItems.ColumnNames.d)[i]) / 2).ToArray();
                    double[] v = itemBank.GetItemParamter(CATItems.ColumnNames.c).Select((n, i) => (n + itemBank.GetItemParamter(CATItems.ColumnNames.d)[i] - 1) / 4).ToArray();

                    double[] xstar = u.Select((m, i) => 2 * Math.Sqrt(-m / 3) * Math.Cos(Math.Acos(-v[i] * Math.Sqrt(-27 / Math.Pow(m, 3)) / 2) / 3 + 4 * (Math.PI / 3)) + 0.5).ToArray();
                    double[] thstar = itemBank.GetItemParamter(CATItems.ColumnNames.b).Select((o, i) => o + Math.Log((xstar[i] - itemBank.GetItemParamter(CATItems.ColumnNames.c)[i]) / (itemBank.GetItemParamter(CATItems.ColumnNames.d)[i] - xstar[i])) / (D * itemBank.GetItemParamter(CATItems.ColumnNames.a)[i])).ToArray();

                    double[] distance = thstar.Select(p => Math.Abs(p - theta)).ToArray();
                    double[] ranks = CatRcs.Utils.CommonHelper.Rank(distance).Select(m => m).ToArray();

                    if (OUT != null)
                    {
                        for (int a = 0; a < OUT.Length; a++)
                        {
                            ranks[OUT[a] - 1] = -1;
                        }
                    }

                    int nrIt = new int[] { randomesque, (int)CatRcs.Utils.RowColumn.Sum(items) }.Min();

                    List<double> tempRanks = new List<double>();

                    for (int j = 0; j < items.Length; j++)
                    {
                        if (items[j] == 1)
                        {
                            tempRanks.Add(ranks[j]);
                        }
                    }

                    tempRanks = tempRanks.OrderBy(n => n).ToList();
                    double[] keepRank = tempRanks.GetRange(0, nrIt).ToArray();
                    keepRank = keepRank.Distinct().ToArray();

                    List<int> keep = new List<int>();

                    if (ranks.Length == items.Length)
                    {
                        for (int m = 0; m < keepRank.Length; m++)
                        {
                            for (int n = 0; n < ranks.Length; n++)
                            {
                                if ((items[n] == 1) && (ranks[n] == keepRank[m]))
                                {
                                    keep.Add(n + 1);
                                }
                            }
                        }
                    }

                    if (keep.Count == 1)
                    {
                        select = keep[0];
                    }
                    else
                    {
                        select = CatRcs.Utils.RandomNumberHandler.Sample(keep.ToArray(), 1, false)[0];
                    }

                    result = new NextItemModel(select, itemBank.FindItem(select), distance[select - 1], criterion, randomesque);
                }
                else
                {
                   result = new NextItemModel(true, "thOpt's rule cannot be considered with polytomous items!");
                   return result;
                }
            }

            #endregion

            #region Handling "cbControl" Parameter

            if (cbControl == null)
            {
                if(result != null)
                {
                    result.prior_prop = null;
                    result.post_prop = null;
                    result.cb_prop = null;
                }
            }
            else
            {
                result.prior_prop = empProp;

                double[] postProp = new double[nrGroup];

                string[] temp_grp = new string[Out.Length + 1];

                for (int i = 0; i < temp_grp.Length; i++)
                {
                    if(i == 0)
                    {
                        temp_grp[i] = cbGroup[result.item];
                    }

                    temp_grp[i] = cbGroup[Out[i]];
                }

                for (int j = 0; j < postProp.Length; j++)
                {
                    string[] values = temp_grp.Where(m => m == cbControl.Names[j]).ToArray();

                    if (values != null && values.Length > 0)
                    {
                        postProp[j] = values.Length;
                    }
                    else
                    {
                        postProp[j] = 0;
                    }
                }

                result.post_prop = postProp.Select(n => n / CatRcs.Utils.RowColumn.Sum(postProp)).ToArray();
                result.cb_prop = thProp;
            }

            #endregion

            return result;
        }
    }
}
