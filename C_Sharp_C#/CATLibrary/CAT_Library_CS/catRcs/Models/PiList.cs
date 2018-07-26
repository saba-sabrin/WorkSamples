using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatRcs.Models
{
    public class PiList
    {
        public double[] Pi { get; set; }
        public double[] dPi { get; set; }
        public double[] d2Pi { get; set; }
        public double[] d3Pi { get; set; }
        public string[] Errors { get; set; }
        public string Exception { get; set; }

        public PiList()
        { }

        // Exception Constructor
        public PiList(string Exp)
        {
            this.Exception = Exp;
        }

        public PiList(int lengthOfArray)
        {
            this.Pi = new double[lengthOfArray];
            this.dPi = new double[lengthOfArray];
            this.d2Pi = new double[lengthOfArray];
            this.d3Pi = new double[lengthOfArray];
            this.Errors = new string[lengthOfArray];
        }

        public void AddPiList(double Pi, double dPi, double d2Pi, double d3Pi, int index)
        {
            this.Pi.SetValue(Pi,index);
            this.dPi.SetValue(dPi, index);
            this.d2Pi.SetValue(d2Pi, index);
            this.d3Pi.SetValue(d3Pi, index);
        }
    }
}
