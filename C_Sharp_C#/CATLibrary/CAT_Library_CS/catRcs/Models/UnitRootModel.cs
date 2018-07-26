using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatRcs.Models
{
    public class UniRootModel
    {
        public double root { get; set; }
        public double f_root { get; set; }
        public double iter { get; set; }
        public double estim_prec { get; set; }
        public string Error { get; set; }

        public UniRootModel(double root, double f_root, double iter, double estim_prec)
        {
            this.root = root;
            this.f_root = f_root;
            this.iter = iter;
            this.estim_prec = estim_prec;
        }

        public UniRootModel(string errorMsg)
        {
            this.Error = errorMsg;
        }
    }
}
