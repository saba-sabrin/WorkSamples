using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatRcs.Models;

namespace CatRcs
{
    internal class StartItems
    {
        // Function for calculating the Start Items
        public static StartItemsModel StartItems_Calc(CATItems itemBank, string model = null, int[] fixItems = null, int? seed = null, int nrItems = 1, double theta = 0, double D = 1, int halfRange = 2, int startSelect = 2, /* MFI */ int[] nAvailable = null)
        {
            int[] items = null;
            CATItems par = null;
            double[] thStart = null;
            StartItemsModel result = null;

            #region "Parameter Validation"

            if(nAvailable != null)
            {
                if(nAvailable.Length != itemBank.NumOfItems)
                {
                    return result;
                }
                if(CatRcs.Utils.RowColumn.Sum(nAvailable) < nrItems)
                {
                    return result;
                }
            }

            if (nrItems > 0)
            {
                if ((startSelect == (int)ModelNames.StartSelectTypes.Progressive) || (startSelect == (int)ModelNames.StartSelectTypes.Proportional))
                {
                    fixItems = null;
                    nrItems = 1;
                    seed = null;
                }
            }

            #endregion

            if (fixItems != null)
            {
                items = fixItems;
                par = itemBank.FindItem(fixItems);
                startSelect = 0;  // NA value

                result = new StartItemsModel(items, par, null, 0);
            }
            else
            {
                if (nrItems > 0)
                {
                    if(seed != null)
                    {
                        #region "Seed Value Checking"

                        CatRcs.Utils.RandomNumberHandler.Set_seed((int)seed);

                        if (nAvailable == null)
                        {
                            int[] sample_items = new int[itemBank.NumOfItems];
                            sample_items = sample_items.Select((p, i) => i + 1).ToArray(); // creating item numbers

                            items = CatRcs.Utils.RandomNumberHandler.Sample(sample_items, nrItems, false);
                        }
                        else
                        {
                            int j = 0;
                            List<int> temp_items = new List<int>();
                            foreach(int x in nAvailable)
                            {
                                if(x == 1)
                                {
                                    temp_items.Add(j);
                                }
                                j++;
                            }

                            items = CatRcs.Utils.RandomNumberHandler.Sample(temp_items.ToArray(), nrItems, false);
                        }

                        par = itemBank.FindItem(items);
                        thStart = null;
                        startSelect = 0;  // NA value

                        #endregion
                    }
                    else
                    { 
                        if(nrItems == 1)
                        {
                            thStart = new double[1];
                            thStart.ToList().Add(theta);
                        }
                        else
                        {
                            thStart = CatRcs.Utils.CommonHelper.Sequence(theta - halfRange, theta + halfRange, nrItems);
                        }

                        if ((startSelect != (int)ModelNames.StartSelectTypes.BOpt) && (startSelect != (int)ModelNames.StartSelectTypes.ThOpt)
                            && (startSelect != (int)ModelNames.StartSelectTypes.MFI) && (startSelect != (int)ModelNames.StartSelectTypes.Progressive)
                            && (startSelect != (int)ModelNames.StartSelectTypes.Proportional))
                        {
                            result = new StartItemsModel(true, "'startSelect' must be either 'bOpt', 'thOpt', 'progressive', 'proportional' or 'MFI'");
                            return result;
                        }

                        if (!String.IsNullOrEmpty(model) && startSelect != 2)
                        {
                            result = new StartItemsModel(true, "'startSelect' can only be 'MFI' with polytomous items!");
                            return result;
                        }

                        items = new int[thStart.Length];
                        int nr_items = itemBank.NumOfItems;
                        int[] selected = CatRcs.Utils.CommonHelper.Replicate(new double[] { 0 }, nr_items).ToList().Select(p => (int)p).ToArray();
                        int[] pos_adm = null;

                        #region "Selection Type 'BOpt'"

                        if (startSelect == (int)ModelNames.StartSelectTypes.BOpt)
                        {
                            for(int i = 0; i < thStart.Length; i++)
                            {
                                pos_adm = new int[nr_items];
                                double[] item_dist = itemBank.GetItemParamter(CATItems.ColumnNames.b).Select(p => Math.Abs(thStart[i] - p)).ToArray();

                                if (nAvailable.Length == selected.Length)  // number of elements should be same
                                {
                                    if (nAvailable != null)
                                    {
                                        pos_adm = selected.Select((a, b) => 1 - a * nAvailable[b]).ToArray();
                                    }
                                    else
                                    {
                                        pos_adm = selected.Select((a, b) => 1 - a * nAvailable[b]).ToArray();
                                    }
                                }

                                int k = 0;
                                List<double> temp_items = new List<double>();
                                List<int> prov = new List<int>();

                                foreach (int x in pos_adm)
                                {
                                    if (x == 1)
                                    {
                                        temp_items.Add(item_dist[k]);
                                    }
                                    k++;
                                }

                                int n = 0;
                                foreach(double item in temp_items)
                                {
                                    if(item == temp_items.Min())
                                    {
                                        if(pos_adm[n] == 1)
                                        {
                                            prov.Add(n);
                                        }
                                    }
                                    n++;
                                }

                                if(prov.Count == 1)
                                {
                                    items[i] = prov.First();
                                }
                                else
                                {
                                    items[i] = CatRcs.Utils.RandomNumberHandler.Sample(prov.ToArray(), 1, false).First();
                                }

                                selected[items[i]] = 1;
                            }
                        }

                        #endregion

                        #region "Selection Type 'ThOpt'"

                        if (startSelect == (int)ModelNames.StartSelectTypes.ThOpt)
                        {
                            double[] u = itemBank.GetItemParamter(CATItems.ColumnNames.c).Select((c, j) => -3 / 4 + (c + itemBank.GetItemParamter(CATItems.ColumnNames.d)[j] + (-2) * c * itemBank.GetItemParamter(CATItems.ColumnNames.d)[j]) / 2).ToArray();
                            double[] v = itemBank.GetItemParamter(CATItems.ColumnNames.c).Select((c, j) => (c + itemBank.GetItemParamter(CATItems.ColumnNames.d)[j] - 1) / 4).ToArray();
                            double[] xstar = u.Select((c, j) => 2 * Math.Sqrt(-c / 3) * Math.Cos(Math.Acos(-v[j] * Math.Sqrt(-27 / Math.Pow(c,3)) / 2) / 3 + 4 * Math.PI / 3) + 0.5).ToArray();
                            double[] thMax = itemBank.GetItemParamter(CATItems.ColumnNames.b).Select((a, k) => a + Math.Log10((xstar[k] - itemBank.GetItemParamter(CATItems.ColumnNames.c)[k]) / (itemBank.GetItemParamter(CATItems.ColumnNames.d)[k] - xstar[k])) / (D * itemBank.GetItemParamter(CATItems.ColumnNames.a)[k])).ToArray();
                            double[] item_dist = thMax.Select(th => Math.Abs(th - theta)).ToArray();

                            for (int i = 0; i < thStart.Length; i++)
                            {
                                pos_adm = new int[nr_items];

                                if (nAvailable.Length == selected.Length)  // number of elements should be same
                                {
                                    if (nAvailable != null)
                                    {
                                        pos_adm = selected.Select((a, b) => 1 - a * nAvailable[b]).ToArray();
                                    }
                                    else
                                    {
                                        pos_adm = selected.Select((a, b) => 1 - a * nAvailable[b]).ToArray();
                                    }
                                }

                                int k = 0;
                                List<double> temp_items = new List<double>();
                                List<int> prov = new List<int>();

                                foreach (int x in pos_adm)
                                {
                                    if (x == 1)
                                    {
                                        temp_items.Add(item_dist[k]);
                                    }
                                    k++;
                                }

                                int n = 0;
                                foreach (double item in temp_items)
                                {
                                    if (item == temp_items.Min())
                                    {
                                        if (pos_adm[n] == 1)
                                        {
                                            prov.Add(n);
                                        }
                                    }
                                    n++;
                                }

                                if (prov.Count == 1)
                                {
                                    items[i] = prov.First();
                                }
                                else
                                {
                                    items[i] = CatRcs.Utils.RandomNumberHandler.Sample(prov.ToArray(), 1, false).First();
                                }

                                selected[items[i]] = 1;
                            }
                        }

                        #endregion

                        #region "Selection Type 'MFI'"

                        if (startSelect == (int)ModelNames.StartSelectTypes.MFI)
                        {
                            for (int i = 0; i < thStart.Length; i++)
                            {
                                pos_adm = new int[nr_items];

                                double[] item_info = CatRcs.Ii.Ii_Calc(thStart[i], itemBank, model, D).Ii;

                                if (nAvailable != null)  // number of elements should be same
                                {
                                    if (nAvailable.Length == selected.Length)
                                    {
                                        pos_adm = selected.Select((a, b) => 1 - a * nAvailable[b]).ToArray();
                                    }
                                }
                                else
                                {
                                    pos_adm = selected.Select((a, b) => 1 - a).ToArray();
                                }

                                int k = 0;
                                List<int> prov = new List<int>();

                                int n = 1;

                                double maxItemVal = item_info.Where((a, m) => pos_adm[m] == 1).ToArray().Max();

                                foreach (int x in pos_adm)
                                {
                                    if (x == 1) 
                                    {
                                        if (item_info[k] == maxItemVal)
                                        {
                                            if (pos_adm[n - 1] == 1)
                                            {
                                                prov.Add(n);
                                            }
                                        }
                                    }
                                    k++; n++;
                                }

                                if (prov.Count == 1)
                                {
                                    items[i] = prov.ToArray()[0];
                                }
                                else
                                {
                                    items[i] = CatRcs.Utils.RandomNumberHandler.Sample(prov.ToArray(), 1, false).First();
                                }

                                selected[items[i] - 1] = 1;  // As of R index starts with 1 & C# index starts with 0, 
                                                             //So item no. "1" will eventually point to the "0" indexed item in the items
                            }
                        }

                        #endregion

                        par = itemBank.FindItem(items);
                    }

                    result = new StartItemsModel(items, par, thStart, startSelect);
                }
                else
                {
                    result = new StartItemsModel(null, null, null, 0);  // startselect is "NA"
                }
            }

            return result;
        }
    }
}
