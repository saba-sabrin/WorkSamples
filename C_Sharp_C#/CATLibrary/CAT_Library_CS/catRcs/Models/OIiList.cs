using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatRcs.Models
{
    public class OIiList
    {
        public double[] OIi { get; set; }
        public string[] Errors { get; set; }
        public string Exception { get; set; }

        public OIiList() { }

        public OIiList(int lengthOfArray)
        {
            this.OIi = new double[lengthOfArray];
            this.Errors = new string[lengthOfArray];
        }

        public void Add(double[] OIi)
        {
            this.OIi = OIi;
        }
    }
}
