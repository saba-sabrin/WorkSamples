using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatRcs.Models
{
    public class PiListPoly
    {
        public double[,] Pi { get; set; }
        public double[,] dPi { get; set; }
        public double[,] d2Pi { get; set; }
        public double[,] d3Pi { get; set; }
        public string[] Errors { get; set; }
        public string Exception { get; set; }
        public Dictionary<string, Dictionary<string, catList>> catDictionaryList { get; set; }

        public PiListPoly(){
        }

        public PiListPoly(int rowLength, int columnLength)
        {
            this.Pi = new double[rowLength,columnLength];
            this.dPi = new double[rowLength, columnLength];
            this.d2Pi = new double[rowLength, columnLength];
            this.d3Pi = new double[rowLength, columnLength];
            this.Errors = new string[rowLength];
        }

        public void Add(double[,] Pi, double[,] dPi, double[,] d2Pi, double[,] d3Pi)
        {
            this.Pi = Pi;
            this.dPi = dPi;
            this.d2Pi = d2Pi;
            this.d3Pi = d3Pi;
        }
    }

    public class catList
    {
        public double cat0 { get; set; }
        public double cat1 { get; set; }
        public double cat2 { get; set; }
        public double cat3 { get; set; }

        public catList()
        { }

        public void Add(double cat0, double cat1, double cat2, double cat3)
        {
            this.cat0 = cat0;
            this.cat1 = cat1;
            this.cat2 = cat2;
            this.cat3 = cat3;
        }
    }
}
