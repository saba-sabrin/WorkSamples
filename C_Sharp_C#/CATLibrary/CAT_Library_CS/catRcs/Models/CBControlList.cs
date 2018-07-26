using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatRcs.Models
{
    public class CBControlList
    {
        public string[] Names { get; set; }
        public double[] Props { get; set; }

        public CBControlList(string[] names, double[] props)
        {
            this.Names = names;
            this.Props = props;
        }
    }
}
