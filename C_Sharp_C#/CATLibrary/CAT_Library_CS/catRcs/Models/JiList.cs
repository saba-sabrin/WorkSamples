using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatRcs.Models
{
    public class JiList
    {
        public double[] Ji { get; set; }
        public double[] dJi { get; set; }
        public string[] Errors { get; set; }
        public string Exception { get; set; }

        public JiList()
        {
 
        }

        public JiList(int lengthOfArray)
        {
            this.Ji = new double[lengthOfArray];
            this.dJi = new double[lengthOfArray];
            this.Errors = new string[lengthOfArray];
        }

        public void Add(double[] Ji, double[] dJi)
        {
            this.Ji = Ji;
            this.dJi = dJi;
        }
    }
}
