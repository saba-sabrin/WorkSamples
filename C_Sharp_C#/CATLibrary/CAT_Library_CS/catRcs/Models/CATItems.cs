using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CatRcs.Models
{
    public class CATItems
    {
        #region "Properties"

        public double[] a { get; set; }
        public double[] b { get; set; }
        public double[] c { get; set; }
        public double[] d { get; set; }

        // Polytomous items
        public double[][] all_items_poly { get; set; }

        public int NumOfItems { get; set; }
        public int nrCat { get; set; }
        public bool same_nrCat { get; set; }

        public string[] Group { get; set; }
        public bool IsGroupExist { get; set; }

        private ModelNames.Models modelName;
        public ModelNames.Models model { get { return modelName; } }

        public int colSize { get; set; }

        public string ErrMsg { get; set; }

        private bool status = false;

        // Common Collection for all the Item columns (Itembank)
        private Dictionary<Tuple<ColumnNames, int>, double[]> CATItemColumns { get; set; }

        // Enumeration of Column Names
        public enum ColumnNames
        {
            a,
            b,
            c,  // Used for both Dicho & Poly
            d,
            alphaj,
            alpha,
            betaj,
            deltaj,
            delta,
            lambdaj
        }

        #endregion

        #region "Constructor Methods"

        // Constructor for Dichotomous Items with number of items
        public CATItems(int NumOfItems, bool IsGroupExist = false)
        {
            this.NumOfItems = NumOfItems;
            this.modelName = ModelNames.Models.NoModel;
            this.colSize = 4;

            this.a = new double[NumOfItems];
            this.b = new double[NumOfItems];
            this.c = new double[NumOfItems];
            this.d = new double[NumOfItems];

            if (IsGroupExist)
            {
                this.Group = new string[this.NumOfItems];
            }
        }

        // Constructor for Dichotomous Items :: 4PL ::
        public CATItems(double[] a, double[] b, double[] c, double[] d, bool IsGroupExist = false)
        {
            this.NumOfItems = a.Length;
            this.modelName = ModelNames.Models.NoModel;
            this.colSize = 4;

            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;

            if (IsGroupExist)
            {
                this.Group = new string[this.NumOfItems];
            }
        }

        // Test constructor for polytomous items
        public CATItems(int NumOfItems, string model, bool same_nrCat, int nrCat = 3, bool IsGroupExist = false)
        {
            this.NumOfItems = NumOfItems;
            this.nrCat = nrCat;
            this.modelName = ModelNames.StringToEnum(model);
            this.same_nrCat = same_nrCat;

            if (IsGroupExist)
            {
                this.Group = new string[this.NumOfItems];
            }

            switch (this.modelName)
            {
                case ModelNames.Models.GRM:
                    this.all_items_poly = new double[nrCat][];
                    this.colSize = nrCat;
                    break;

                case ModelNames.Models.MGRM:
                    this.all_items_poly = new double[nrCat+1][];
                    this.colSize = nrCat + 1;
                    break;

                case ModelNames.Models.PCM:
                    this.all_items_poly = new double[nrCat - 1][];
                    this.colSize = nrCat - 1;
                    break;

                case ModelNames.Models.GPCM:
                    this.all_items_poly = new double[nrCat][];
                    this.colSize = nrCat;
                    break;

                case ModelNames.Models.RSM:
                    this.all_items_poly = new double[nrCat][];
                    this.colSize = nrCat;
                    break;

                case ModelNames.Models.NRM:
                    this.all_items_poly = new double[2 * (nrCat - 1)][];
                    this.colSize = 2 * (nrCat - 1);
                    break;
            }
        }
        
        // Constructor for Polytomous Items
        public CATItems(int NumOfItems, string model, int nrCat = 3, bool IsGroupExist = false)
        {
            this.NumOfItems = NumOfItems;
            this.nrCat = nrCat;
            this.modelName = ModelNames.StringToEnum(model);
            this.CATItemColumns = new Dictionary<Tuple<ColumnNames, int>, double[]>();

            switch (this.modelName)
            {
                case ModelNames.Models.GRM:

                    this.CATItemColumns.Add(new Tuple<ColumnNames, int>(ColumnNames.alphaj, 0), new double[this.NumOfItems]);

                    for (int i = 1; i < this.nrCat; i++)
                    {
                        this.CATItemColumns.Add(new Tuple<ColumnNames, int>(ColumnNames.betaj, i), new double[this.NumOfItems]);
                    }

                    if (IsGroupExist)
                    {
                        this.Group = new string[this.NumOfItems];
                    }

                    this.colSize = this.CATItemColumns.Count;

                break;

                case ModelNames.Models.MGRM:

                    this.CATItemColumns.Add(new Tuple<ColumnNames, int>(ColumnNames.alphaj, 0), new double[this.NumOfItems]);
                    this.CATItemColumns.Add(new Tuple<ColumnNames, int>(ColumnNames.betaj, 0), new double[this.NumOfItems]);

                    for (int i = 1; i < this.nrCat; i++)
                    {

                        this.CATItemColumns.Add(new Tuple<ColumnNames, int>(ColumnNames.c, i), new double[this.NumOfItems]);
                    }

                    if (IsGroupExist)
                    {
                        this.Group = new string[this.NumOfItems];
                    }

                    this.colSize = this.CATItemColumns.Count;

                break;

                case ModelNames.Models.PCM:

                    for (int i = 1; i < this.nrCat; i++)
                    {
                        this.CATItemColumns.Add(new Tuple<ColumnNames, int>(ColumnNames.deltaj, i), new double[this.NumOfItems]);
                    }

                    if (IsGroupExist)
                    {
                        this.Group = new string[this.NumOfItems];
                    }

                    this.colSize = this.CATItemColumns.Count;

                break;

                case ModelNames.Models.GPCM:

                    this.CATItemColumns.Add(new Tuple<ColumnNames, int>(ColumnNames.alphaj, 0), new double[this.NumOfItems]);

                    for (int i = 1; i < this.nrCat; i++)
                    {
                        this.CATItemColumns.Add(new Tuple<ColumnNames, int>(ColumnNames.deltaj, i), new double[this.NumOfItems]);
                    }

                    if (IsGroupExist)
                    {
                        this.Group = new string[this.NumOfItems];
                    }

                    this.colSize = this.CATItemColumns.Count;

                break;

                case ModelNames.Models.RSM:

                    this.CATItemColumns.Add(new Tuple<ColumnNames, int>(ColumnNames.lambdaj, 0), new double[this.NumOfItems]);

                    for (int i = 1; i < this.nrCat; i++)
                    {
                        this.CATItemColumns.Add(new Tuple<ColumnNames, int>(ColumnNames.delta, i), new double[this.NumOfItems]);
                    }

                    if (IsGroupExist)
                    {
                        this.Group = new string[this.NumOfItems];
                    }

                    this.colSize = this.CATItemColumns.Count;

                break;

                case ModelNames.Models.NRM:

                    for (int i = 1; i < this.nrCat; i++)
                    {
                        this.CATItemColumns.Add(new Tuple<ColumnNames, int>(ColumnNames.alpha, i), new double[this.NumOfItems]);
                        this.CATItemColumns.Add(new Tuple<ColumnNames, int>(ColumnNames.c, i), new double[this.NumOfItems]);
                    }

                    if (IsGroupExist)
                    {
                        this.Group = new string[this.NumOfItems];
                    }

                    this.colSize = this.CATItemColumns.Count;

                break;
            }
        }

        #endregion

        #region "Utility Methods"

        // After adding the data to the object, this check should be performed !!
        public void NAValueHandler()
        {
            for (int j = 0; j < this.colSize; j++)
            {
                for (int k = 0; k < this.NumOfItems; k++)
                {
                    if (CatRcs.Utils.CheckNaValues.IsNaNvalue(this.all_items_poly[j][k]))
                    {
                        this.all_items_poly[j][k] = double.NaN;
                    }
                }
            }
        }

        // wrap R data to CS
        public void DataWrapper(double[] values, int index)
        {
            this.all_items_poly[index] = values;
        }

        // Set method for itembank using seperate column type and item array
        public bool SetItemParamter(ColumnNames type, double[] parColumn)
        {
            status = false;

            try
            {
                this.CATItemColumns[new Tuple<ColumnNames, int>(type, 0)] = parColumn;
                status = true;
            }
            catch(Exception ex)
            {
                this.ErrMsg = ex.Message;
            }

            return status;
        }

        // Set method for itembank using Tuple
        public bool SetItemParamter(Tuple<ColumnNames, int> itemCol, double[] parColumn)
        {
            status = false;

            try
            {
                this.CATItemColumns[itemCol] = parColumn;
                status = true;
            }
            catch (Exception ex)
            {
                this.ErrMsg = ex.Message;
            }

            return status;
        }

        // Get Column names with index
        public Tuple<ColumnNames, int>[] GetKeys()
        {
            Tuple<ColumnNames, int>[] all_Keys = this.CATItemColumns.Keys.Select(p => p).ToArray();
            return all_Keys;
        }

        // Get Column without index
        public double[] GetItemParamter(ColumnNames type)
        {
            double[] result = this.CATItemColumns[new Tuple<ColumnNames, int>(type, 0)];
            return result;
        }

        // Find single item with Item Index
        public CATItems FindItem(int item)
        {
            CATItems result = null;
            ModelNames.Models tempModel = this.model;

            if (tempModel == ModelNames.Models.NoModel)
            {
                result = new CATItems(1);
            }
            else
            {
                result = new CATItems(1, this.model.EnumToString(), this.nrCat);
            }

            Tuple<CATItems.ColumnNames, int>[] cols = this.GetKeys();

            for (int i = 0; i < cols.Length; i++)
            {
                double[] itemsTemp = this.CATItemColumns[cols[i]];

                double[] singleItemValue = new double[] { itemsTemp[item - 1] };

                result.CATItemColumns[cols[i]] = singleItemValue;
            }

            return result;
        }

        // Find multiple items with Item Index
        public CATItems FindItem(int[] items)
        {
            CATItems result = null;
            ModelNames.Models tempModel = this.model;

            if(tempModel == ModelNames.Models.NoModel)
            {
                result = new CATItems(items.Length, this.IsGroupExist);
            }
            else
            {
                result = new CATItems(items.Length, this.model.EnumToString(), this.nrCat);
            }

            Tuple<CATItems.ColumnNames, int>[] cols = this.GetKeys();

            for (int i = 0; i < cols.Length; i++)
            {
                double[] itemsTemp = this.CATItemColumns[cols[i]];

                List<double> ItemValues = new List<double>();

                for (int k = 0; k < items.Length; k++)
                {
                    ItemValues.Add(itemsTemp[items[k] - 1]);
                }

                result.CATItemColumns[cols[i]] = ItemValues.ToArray();
            }

            return result;
        }

        // Usage:: Find Single item from itembank (Single row) and Add with the invoked list
        public void AddItem(CATItems parItem, int item)
        {
            CATItems result = this.FindItem(item);

            Tuple<CATItems.ColumnNames, int>[] cols = parItem.GetKeys();

            for (int i = 0; i < cols.Length; i++)
            {
                List<double> tempValues = parItem.CATItemColumns[cols[i]].ToList();

                tempValues.Add(result.CATItemColumns[cols[i]][0]);

                parItem.CATItemColumns[cols[i]] = tempValues.ToArray();
            }

            parItem.NumOfItems++;
        }

        public CATItems FIndItem_D(int[] items)
        {
            CATItems res_items = null;

            if(items.Length > 0)
            {
                res_items = new CATItems(items.Length);

                for (int i = 0; i < items.Length; i++)
                {
                    res_items.a[i] = this.a[items[i] - 1];
                    res_items.b[i] = this.b[items[i] - 1];
                    res_items.c[i] = this.c[items[i] - 1];
                    res_items.d[i] = this.d[items[i] - 1];
                }
            }

            return res_items;
        }

        public CATItems AddItem_D(CATItems parItem, int[] items)
        {
            CATItems newItem = this.FIndItem_D(items);

            CATItems res_item = new CATItems(parItem.a.Length + 1);

            for(int i=0; i < parItem.a.Length; i++)
            {
                res_item.a[i] = parItem.a[i];
                res_item.b[i] = parItem.b[i];
                res_item.c[i] = parItem.c[i];
                res_item.d[i] = parItem.d[i];
            }

            int new_index = res_item.a.Length - items.Length;

            for(int j=0; j < newItem.a.Length; j++)
            {
                res_item.a[new_index] = newItem.a[j];
                res_item.b[new_index] = newItem.b[j];
                res_item.c[new_index] = newItem.c[j];
                res_item.d[new_index] = newItem.d[j];
                new_index++;
            }

            return res_item;
        }

        #endregion
    }
}
