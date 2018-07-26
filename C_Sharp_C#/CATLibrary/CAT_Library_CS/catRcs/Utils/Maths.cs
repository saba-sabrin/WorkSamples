using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatRcs.Models;

namespace CatRcs.Utils
{
    public class Maths
    {
        // Function for calculating the Range of max & min value in any Array
        public static double[] Range(double[] param, bool na_rm, bool finite)
        {
            List<double> result = new List<double>(); // returns the maximum & minimum of the given array parameter

            double[] temp_array = new double[param.Length];
            double[] temp_array_2 = new double[] { 2.9, 4.0, 4.5, 8.1 };

            if(na_rm == true)
            {
                for(var j = 0; j < param.Length; j++)
                {
                    temp_array[j] = CheckNaValues.IsNaNvalue(param[j]) ? double.NaN : param[j];
                }

                temp_array_2 = CheckNaValues.GetArrayWithoutNaN(temp_array);
            }

            if (finite == true)
            {
                for (var i = 0; i < param.Length; i++)
                {
                    if (CheckNaValues.IsPosInFinityValue(param[i]))
                    {
                        temp_array[i] = double.PositiveInfinity;
                    }
                    else if (CheckNaValues.IsNegInFinityValue(param[i]))
                    {
                        temp_array[i] = double.NegativeInfinity;
                    }
                    else
                    {
                        temp_array[i] = param[i];
                    }
                }

                temp_array_2 = CheckNaValues.GetArrayWithoutNaN(temp_array);
            }

            try
            {
                Array.Sort(temp_array_2);

                result.Add(temp_array_2[0]);
                result.Add(temp_array_2[temp_array_2.Length-1]);
            }
            catch (InvalidOperationException ex)
            {
                // To be decided.. .
            }

            return result.ToArray();
        }

        // Function for calculating Sample Standard Deviation
        public static double StandardDeviation(double[] values)
        {
            double avg = values.Average();
            double[] squareDiffs = new double[values.Length];

            double sumSquareDiff = 0;

            for (var j = 0; j < values.Length; j++)
            {
                sumSquareDiff = sumSquareDiff + Math.Pow(values[j] - avg, 2);
            }

            return Math.Sqrt(sumSquareDiff / (values.Length - 1));
        }

        // Function for calculating Covariance of Number values
        public static double Covariance(double[] param1, double[] param2)
        {
            double finalCov = 0, len = 0, totalSum = 0;

            if(param1.Length > 0 && param2.Length > 0)
            {
                if(param1.Length == param2.Length)
                {
                    len = param1.Length;
                }

                for (int j = 0; j < param1.Length; j++)
                {
                    double tempResult = (param1[j] - param1.Average()) * (param2[j] - param2.Average());
                    totalSum = totalSum + tempResult;
                }

                finalCov = totalSum / (param1.Length - 1);
            }

            return finalCov;
		}

        // Function for calculating Correlation between Two Numeric Variable
        public static double Correlation(double[] param1, double[] param2)
        {
            double corResult = Covariance(param1, param2) / (StandardDeviation(param1) * StandardDeviation(param2));
			return corResult;
		}

        // Calculates Maxima
        public static double[] PMax(double[] arg1, double[] arg2)
        {
            List<double> result = new List<double>();

            if(arg1.Length > 0)
            {
                if(arg2.Length > 0)
                {
                    if(arg1.Length == arg2.Length)
                    {
                        for (int i = 0; i < arg1.Length; i++ )
                        {
                            if (arg1[i] > arg2[i])
                            {
                                result.Add(arg1[i]);
                            }
                            else if (arg2[i] > arg1[i])
                            {
                                result.Add(arg2[i]);
                            }
                            else
                            {
                                result.Add(arg1[i]);
                            }
                        }
                    }
                }
            }

            return result.ToArray();
        }

        // Calculates Minima
        public static double[] PMin(double[] arg1, double[] arg2)
        {
            List<double> result = new List<double>();

            if (arg1.Length > 0)
            {
                if (arg2.Length > 0)
                {
                    if (arg1.Length == arg2.Length)
                    {
                        for (int i = 0; i < arg1.Length; i++)
                        {
                            if (arg1[i] < arg2[i])
                            {
                                result.Add(arg1[i]);
                            }
                            else if (arg2[i] < arg1[i])
                            {
                                result.Add(arg2[i]);
                            }
                            else
                            {
                                result.Add(arg1[i]);
                            }
                        }
                    }
                }
            }

            return result.ToArray();
        }

        private static double Truncate(double x)
        {
            double result = 0;
            double Machine_double_xmax = 1.797693e+308;
            result = PMax(PMin(new double[] { x }, new double[] { Machine_double_xmax }), new double[] { -Machine_double_xmax }).First();

            return result;
        }

        private static double[] Delta(double[] u)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < u.Length; i++)
            {
                result.Add(0.01 * (PMax(new double[] { 1e-04 }, new double[] { Math.Abs(u[i]) })).First());
            }

            return result.ToArray();
        }

        // Zeroin function
        public static double Zeroin(Func<double, double> f, double ax, double bx, double fa, double fb, double Tol, int Maxit)
        {
            double a, b, c, fc;			/* Abscissae, descr. see above,  f(c) */
            double tol;
            int maxit;

            a = ax; b = bx;
            c = a; fc = fa;
            maxit = Maxit + 1;
            tol = Tol;

            /* First test if we have found a root at an endpoint */
            if (fa == 0.0)
            {
                Tol = 0.0;
                Maxit = 0;
                return a;
            }
            if (fb == 0.0)
            {
                Tol = 0.0;
                Maxit = 0;
                return b;
            }

            for (int ind = maxit; ind > 0; ind--)
            {
                double prev_step = b - a;       /* Distance from the last but one to the last approximation	*/
                double tol_act;         /* Actual tolerance		*/
                double p;           /* Interpolation step is calcu- */
                double q;           /* lated in the form p/q; divi-
					                * sion operations is delayed
					                * until the last moment	*/
                double new_step;    /* Step at this iteration	*/

                if (Math.Abs(fc) < Math.Abs(fb))
                {
                    /* Swap data for b to be the	*/
                    a = b; b = c; c = a;    /* best approximation */
                    fa = fb; fb = fc; fc = fa;
                }
                tol_act = 2 * double.Epsilon * Math.Abs(b) + tol / 2;
                new_step = (c - b) / 2;

                if ((Math.Abs(new_step) <= tol_act) || fb == 0)
                {
                    Maxit -= maxit;
                    Tol = Math.Abs(c - b);
                    return b;    /* Acceptable approx. is found	*/
                }

                /* Decide if the interpolation can be tried	*/
                if ((Math.Abs(prev_step) >= tol_act) && (Math.Abs(fa) > Math.Abs(fb)))
                {
                    /* If prev_step was large enough and was in true direction, Interpolation may be tried	*/
                    double t1, cb, t2;
                    cb = c - b;

                    if (a == c)
                    {       /* If we have only two distinct	*/
                            /* points linear interpolation	*/
                        t1 = fb / fa;       /* can only be applied		*/
                        p = cb * t1;
                        q = 1.0 - t1;
                    }
                    else
                    {
                        /* Quadric inverse interpolation*/
                        q = fa / fc; t1 = fb / fc; t2 = fb / fa;
                        p = t2 * (cb * q * (q - t1) - (b - a) * (t1 - 1.0));
                        q = (q - 1.0) * (t1 - 1.0) * (t2 - 1.0);
                    }

                    if (p > 0)      /* p was calculated with the */
                        q = -q;         /* opposite sign; make p positive */
                    else            /* and assign possible minus to	*/
                        p = -p;			/* q				*/

                    if (p < (0.75 * cb * q - Math.Abs(tol_act * q) / 2) && p < Math.Abs(prev_step * q / 2)) /* If b+p/q falls in [b,c] and isn't too large	*/
                        new_step = p / q;

                    /* it is accepted
                    * If p/q is too large then the
                    * bisection procedure can
                    * reduce [b,c] range to more
                    * extent */
                }

                if (Math.Abs(new_step) < tol_act)
                {   /* Adjust the step to be not less*/
                    if (new_step > 0)   /* than tolerance		*/
                        new_step = tol_act;
                    else
                        new_step = -tol_act;
                }
                a = b; fa = fb;         /* Save the previous approx. */
                b += new_step;
                fb = f(b);  /* Do step to a new approxim. */

                if ((fb > 0 && fc > 0) || (fb < 0 && fc < 0))
                {
                    /* Adjust c for it to have a sign opposite to that of b */
                    c = a; fc = fa;
                }
            }

            /* failed! */
            Tol = Math.Abs(c - b);
            Maxit = -1;

            return b;
        }

        // Function for calculating One Dimensional Root of function (f)
        public static UniRootModel UniRoot(Func<double, double> f, double[] interval)
        {
            double lower = 0, upper = 0, f_lower = 0, f_upper = 0;
            double tol = Math.Pow(double.Epsilon, 0.25);
            int maxiter = 1000;
            UniRootModel objUniRoot = null;
            bool doX = false;
            double[] delta = null;

            if (interval.Length == 2)  // array of length 2 whose maximum and minimum values specify the interval
            {
                lower = interval.Min();
                upper = interval.Max();

                f_lower = Truncate(f(lower));
                f_upper = Truncate(f(upper));

                if(f_lower * f_upper > 0)
                {
                    doX = true;
                }

                if(doX)
                {
                    delta = Delta(new double[] { lower, upper });

                    while ((f_lower * f_upper > 0) && ((double.IsInfinity(lower) == false) || (double.IsInfinity(upper) == false)))
                    {
                        // "maxiter" and "trace" parameters are not considered

                        if (double.IsInfinity(lower) == false)
                        {
                            double ol = lower;
                            double of = f_lower;

                            lower = lower - delta[0];
                            f_lower = f(lower);

                            if(CatRcs.Utils.CheckNaValues.IsNaNvalue(f_lower))
                            {
                                lower = ol;
                                f_lower = of;
                                delta[0] = delta[0] / 4;
                            }
                        }

                        if (double.IsInfinity(upper) == false)
                        {
                            double ol = upper;
                            double of = f_upper;

                            upper = upper - delta[1];
                            f_upper = f(upper);

                            if (CatRcs.Utils.CheckNaValues.IsNaNvalue(f_upper))
                            {
                                upper = ol;
                                f_upper = of;
                                delta[1] = delta[1] / 4;
                            }
                        }
                    }
                }

                // Call Zeroin function
                double temp_root = Zeroin(f, lower, upper, f_lower, f_upper, tol, maxiter);
                double temp_f_root = f(temp_root);
                objUniRoot = new UniRootModel(temp_root, temp_f_root, maxiter, tol);
            }
            else
            {
                objUniRoot = new UniRootModel("Interval should be of length 2");
            }

            return objUniRoot;
        }
    }
}
