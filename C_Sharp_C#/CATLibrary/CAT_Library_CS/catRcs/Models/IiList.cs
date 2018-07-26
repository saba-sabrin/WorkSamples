using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatRcs.Models
{
    public class IiList
    {
        public double[] Ii { get; set; }
        public double[] dIi { get; set; }
        public double[] d2Ii { get; set; }
        public string[] Errors { get; set; }
        public string Exception { get; set; }

        public IiList()
        { }

        public IiList(int lengthOfArray)
        {
            this.Ii = new double[lengthOfArray];
            this.dIi = new double[lengthOfArray];
            this.d2Ii = new double[lengthOfArray];
            this.Errors = new string[lengthOfArray];
        }

        public void Add(double[] Ii, double[] dIi, double[] d2Ii)
        {
            this.Ii = Ii;
            this.dIi = dIi;
            this.d2Ii = d2Ii;
        }
    }
}
