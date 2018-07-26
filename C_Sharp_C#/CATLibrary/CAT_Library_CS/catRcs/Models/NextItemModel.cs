using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatRcs.Models
{
    public class NextItemModel
    {
        public int item { get; set; }
        public CATItems par { get; set; }
        public double info { get; set; }
        public int criterion { get; set; }
        public int randomesque { get; set; }
        public double[] prior_prop { get; set; }
        public double[] post_prop { get; set; }
        public double[] cb_prop { get; set; }

        private bool hasErr = false;
        public bool HasError { get { return hasErr; } set { value = hasErr; } }
        private string errmsg = "";
        public string ErrorMsg { get { return errmsg; } set { value = errmsg; } }

        public NextItemModel(bool hasError, string errMsg)
        {
            hasErr = hasError;
            errmsg = errMsg;
        }

        public NextItemModel(int item, CATItems par, double info, int criterion, int randomesque)
        {
            this.item = item;
            this.par = par;
            this.info = info;
            this.criterion = criterion;
            this.randomesque = randomesque;
        }

        public NextItemModel(int item, CATItems par, double info, int criterion, int randomesque, double[] prior_prop, double[] post_prop, double[] cb_prop)
        {
            this.item = item;
            this.par = par;
            this.info = info;
            this.criterion = criterion;
            this.randomesque = randomesque;
            this.prior_prop = prior_prop;
            this.post_prop = post_prop;
            this.cb_prop = cb_prop;
        }  
    }
}
