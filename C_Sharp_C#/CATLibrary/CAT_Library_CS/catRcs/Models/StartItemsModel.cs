using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatRcs.Models
{
    // Model for StartItems Return Type
    public class StartItemsModel
    {
        public int[] items { get; set; }
        public CATItems par { get; set; }
        public double[] thStart { get; set; }
        public int startSelect { get; set; }

        // Internal parameters for Error Handling
        private bool hasErr = false;
        public bool HasError { get { return hasErr; } set { value = hasErr; } }
        private string errmsg = "";
        public string ErrorMsg { get { return errmsg; } set { value = errmsg; } }

        public StartItemsModel(bool hasError, string errMsg)
        {
            hasErr = hasError;
            errmsg = errMsg;
        }

        public StartItemsModel(int[] items, CATItems par, double[] thStart, int startSelect)
        {
            this.items = items;
            this.par = par;
            this.thStart = thStart;
            this.startSelect = startSelect;
        }
    }
}
