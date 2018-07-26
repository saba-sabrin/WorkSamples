using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatRcs
{
    internal class Integrate_catR
    {
        public static double Integrate_CatR_Calc(double[] x, double[] y)
        {
            double result = 0, sum = 0;

            if(x.Length == y.Length)
            {
                double[] hauteur = new double[x.Length - 1];
                double[] base_List = new double[x.Length - 1];

                for (int i = 0; i < x.Length - 1; i++)
                {
                    hauteur[i] = x[i+1] - x[i];
                }

                for (int row = 0; row < y.Length - 1; row++)
                {
                    sum = 0;

                    for (int col = 0; col < 2; col++)
                    {
                        if(col == 0)
                        {
                            sum = sum + y[row];
                        }
                        if (col == 1)
                        {
                            sum = sum + y[row + 1];
                        }
                    }

                    base_List[row] = (sum / 2);  // Calculate mean, column wise
                }

                double[] temp_res = new double[base_List.Length];

                for(int p =0; p < base_List.Length; p++)
                {
                    temp_res[p] = hauteur[p] * base_List[p];
                }

                result = CatRcs.Utils.RowColumn.Sum(temp_res);
            }

            return result;
        }
    }
}
