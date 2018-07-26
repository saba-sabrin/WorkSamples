using System;
using System.Collections.Generic;
using CatRcs.Models;
using CatRcs.Utils;

namespace CatRcs
{
    internal static class Pi
    {
        // Pi method for Dichotomous items
        public static PiList Pi_Calc(double th, CATItems it, string model, double D)
        {
            PiList objPi = null;

            if (String.IsNullOrEmpty(model))
            {
                #region "Functional Logic"
                try
                {
                    double[] e = new double[it.NumOfItems];
                    objPi = new PiList(it.NumOfItems);

                    for (int i = 0; i < it.NumOfItems; i++)
                    {
                        e[i] = Math.Exp(D * it.a[i] * (th - it.b[i])); // round test

                        double final_pi = 0;

                        double temp_pi = it.c[i] + (it.d[i] - it.c[i]) * e[i] / (1 + e[i]);

                        final_pi = (temp_pi == 0) ? 1e-10 : temp_pi;

                        final_pi = (temp_pi == 1) ? 1 - 1e-10 : temp_pi;

                        double temp_dPi = D * it.a[i] * e[i] * (it.d[i] - it.c[i]) / Math.Pow(1 + e[i], 2); // round test

                        double temp_d2Pi = Math.Pow(D, 2) * Math.Pow(it.a[i], 2) * e[i] * (1 - e[i]) * (it.d[i] - it.c[i]) / Math.Pow(1 + e[i], 3);  // round test

                        double temp_d3Pi = Math.Pow(D, 3) * Math.Pow(it.a[i], 3) * e[i] * (it.d[i] - it.c[i]) * (Math.Pow(e[i], 2) - 4 * e[i] + 1) / Math.Pow(1 + e[i], 4); // round test

                        objPi.AddPiList(final_pi, temp_dPi, temp_d2Pi, temp_d3Pi, i);
                    }
                }
                catch (Exception ex)
                {
                    if (ex != null)
                    {
                        if (objPi == null)
                        {
                            objPi = new PiList(ex.Message);
                        }
                    }
                    return objPi;
                }
                #endregion
            }
            else
            {
                objPi = new PiList();
                objPi.Errors.SetValue("Model is not empty!!", 0);
            }

            return objPi;
        }

        // Pi method for Polytomous Items
        public static PiListPoly Pi_Poly_Calc(double th, CATItems it, string model, double D)
        {
            PiListPoly objPi = null;
            double[,] prov = null, prov1 = null, prov2 = null, prov3 = null;

            try
            {
                if (!String.IsNullOrEmpty(model))
                {
                    ModelNames.Models model_Name = ModelNames.StringToEnum(model);

                    if (model_Name == ModelNames.Models.GRM || model_Name == ModelNames.Models.MGRM)
                    {
                        #region "Variable Declarations"

                        if (model_Name == ModelNames.Models.GRM)
                        {
                            prov = new double[it.NumOfItems, it.colSize];
                            prov1 = new double[it.NumOfItems, it.colSize];
                            prov2 = new double[it.NumOfItems, it.colSize];
                            prov3 = new double[it.NumOfItems, it.colSize];

                            objPi = new PiListPoly(it.NumOfItems, it.colSize);
                        }

                        if (model_Name == ModelNames.Models.MGRM)
                        {
                            prov = new double[it.NumOfItems, it.colSize - 1];
                            prov1 = new double[it.NumOfItems, it.colSize - 1];
                            prov2 = new double[it.NumOfItems, it.colSize - 1];
                            prov3 = new double[it.NumOfItems, it.colSize - 1];

                            objPi = new PiListPoly(it.NumOfItems, it.colSize - 1);
                        }

                        #endregion

                        #region "Functional Logic"

                        for (int i = 0; i < it.NumOfItems; i++)  // Traversing through items
                        {
                            // "alphaj", 1st column
                            double aj = it.all_items_poly[0][i];

                            // Calculation of "betaj" vector values
                            double[] bj = null;

                            // "GRM"
                            if (model_Name == ModelNames.Models.GRM)
                            {
                                bj = new double[it.colSize - 1];  // Values starting from 2nd column
                                int b_Index = 1;

                                for (int k = 0; k < bj.Length; k++)  
                                {
                                    bj[k] = it.all_items_poly[b_Index][i]; ;

                                    b_Index++;
                                }
                            }
                            else  // "MGRM"
                            {
                                // "betaj", 2nd column
                                double bj0 = it.all_items_poly[1][i];

                                bj = new double[it.colSize - 2];  // Values starting from 3rd column
                                int c_Index = 2;

                                for (int k = 0; k < bj.Length; k++)  
                                {
                                    bj[k] = bj0 - it.all_items_poly[c_Index][i];

                                    c_Index++;
                                }
                            }

                            // Final Array must be NaN value free
                            bj = CheckNaValues.GetArrayWithoutNaN(bj);  
             
                            double[] ej = new double[bj.Length];
                            double[] Pjs = new double[bj.Length + 2];
                            double[] dPjs = new double[Pjs.Length];
                            double[] d2Pjs = new double[Pjs.Length];
                            double[] d3Pjs = new double[Pjs.Length];

                            Pjs[0] = 1.000;  // 1st Column
                            Pjs[Pjs.Length - 1] = 0.000;  // last column

                            for (int m = 0; m < bj.Length; m++)
                            {
                                // Calculation of "ej"
                                ej[m] = Math.Exp(D * aj * (th - bj[m]));

                                // Calculation of "Pjs"
                                Pjs[m+1] = ej[m] / (1 + ej[m]);
                            }

                            // Calculation of "dPj", "d2Pj", "d3Pj"
                            for (int j = 0; j < Pjs.Length; j++)
                            {
                                dPjs[j] = D * aj * Pjs[j] * (1 - Pjs[j]);
                                d2Pjs[j] = D * aj * (dPjs[j] - 2 * Pjs[j] * dPjs[j]);
                                d3Pjs[j] = D * aj * (d2Pjs[j] - 2 * Math.Pow(dPjs[j], 2) - 2 * Pjs[j] * d2Pjs[j]);
                            }

                            for (int index = 0; index < Pjs.Length - 1; index++)
                            {
                                prov[i, index] = Pjs[index] - Pjs[index + 1];
                                prov1[i, index] = dPjs[index] - dPjs[index + 1];
                                prov2[i, index] = d2Pjs[index] - d2Pjs[index + 1];
                                prov3[i, index] = d3Pjs[index] - d3Pjs[index + 1];
                            }
                        }

                        #endregion
                    }
                    else if (model_Name == ModelNames.Models.PCM || model_Name == ModelNames.Models.GPCM || model_Name == ModelNames.Models.RSM || model_Name == ModelNames.Models.NRM)
                    {
                        double[] dj = null; double[] v = null;  // Common column for the following models

                        if (model_Name == ModelNames.Models.PCM)
                        {
                            #region "Variable Declarations"

                            prov = new double[it.NumOfItems, it.colSize + 1];
                            prov1 = new double[it.NumOfItems, it.colSize + 1];
                            prov2 = new double[it.NumOfItems, it.colSize + 1];
                            prov3 = new double[it.NumOfItems, it.colSize + 1];

                            objPi = new PiListPoly(it.NumOfItems, it.colSize + 1);

                            #endregion

                            #region "Functional Logic"

                            for (int i = 0; i < it.NumOfItems; i++)
                            {
                                dj = new double[it.colSize + 1];
                                v = new double[it.colSize + 1];

                                dj[0] = 0.00; v[0] = 0.00;

                                for (int k = 1; k < dj.Length; k++)
                                {
                                    dj[k] = dj[k - 1] + D * (th - it.all_items_poly[k-1][i]);
                                    v[k] = k;
                                }

                                double[][] temp_dj_v = CheckNaValues.GetArrayWithoutNaN(dj, v);
                                dj = temp_dj_v[0];
                                v = temp_dj_v[1];

                                calculatePolyPi(dj, prov, prov1, prov2, prov3, v, i);
                            }

                            #endregion
                        }

                        if (model_Name == ModelNames.Models.GPCM)
                        {
                            #region "Variable Declarations"

                            prov = new double[it.NumOfItems, it.colSize];
                            prov1 = new double[it.NumOfItems, it.colSize];
                            prov2 = new double[it.NumOfItems, it.colSize];
                            prov3 = new double[it.NumOfItems, it.colSize];

                            objPi = new PiListPoly(it.NumOfItems, it.colSize);

                            #endregion

                            #region "Functional Logic"

                            for (int i = 0; i < it.NumOfItems; i++)
                            {
                                // "alphaj", 1st column
                                double aj = it.all_items_poly[0][i];

                                dj = new double[it.colSize];
                                v = new double[it.colSize];
                                dj[0] = 0.00; v[0] = 0.00;

                                for (int k = 1; k < dj.Length; k++)
                                {
                                    dj[k] = dj[k - 1] + aj * D * (th - it.all_items_poly[k][i]);
                                    v[k] = aj * k;
                                }

                                double[][] temp_dj_v = CheckNaValues.GetArrayWithoutNaN(dj, v);
                                dj = temp_dj_v[0];
                                v = temp_dj_v[1];

                                calculatePolyPi(dj, prov, prov1, prov2, prov3, v, i);
                            }

                            #endregion
                        }

                        if (model_Name == ModelNames.Models.RSM)
                        {
                            #region "Variable Declarations"

                            prov = new double[it.NumOfItems, it.colSize];
                            prov1 = new double[it.NumOfItems, it.colSize];
                            prov2 = new double[it.NumOfItems, it.colSize];
                            prov3 = new double[it.NumOfItems, it.colSize];

                            objPi = new PiListPoly(it.NumOfItems, it.colSize);

                            #endregion

                            #region "Functional Logic"

                            for (int i = 0; i < it.NumOfItems; i++)
                            {
                                // "lambdaj", 1st column
                                double lambdaj = it.all_items_poly[0][i];

                                dj = new double[it.colSize];
                                v = new double[it.colSize];
                                dj[0] = 0.00; v[0] = 0.00;

                                for (int k = 1; k < dj.Length; k++)
                                {
                                    dj[k] = dj[k - 1] + D * (th - (lambdaj + it.all_items_poly[k][i]));
                                    v[k] = k;
                                }

                                double[][] temp_dj_v = CheckNaValues.GetArrayWithoutNaN(dj, v);
                                dj = temp_dj_v[0];
                                v = temp_dj_v[1];

                                calculatePolyPi(dj, prov, prov1, prov2, prov3, v, i);
                            }

                            #endregion
                        }

                        if (model_Name == ModelNames.Models.NRM)
                        {
                            #region "Variable Declarations"

                            int nc = (it.colSize / 2) + 1;

                            prov = new double[it.NumOfItems, nc];
                            prov1 = new double[it.NumOfItems, nc];
                            prov2 = new double[it.NumOfItems, nc];
                            prov3 = new double[it.NumOfItems, nc];

                            objPi = new PiListPoly(it.NumOfItems, nc);

                            #endregion

                            #region "Functional Logic"

                            for (int i = 0; i < it.NumOfItems; i++)  // Row Iteration
                            {
                                dj = new double[nc];
                                v = new double[nc];
                                dj[0] = 0.00; v[0] = 0.00;

                                for (int k = 1; k < dj.Length; k++)
                                {
                                    dj[k] = it.all_items_poly[2 * (k - 2) + 2][i] * th + it.all_items_poly[2 * (k-2) + 3][i];
                                    v[k] = it.all_items_poly[2 * (k - 2) + 2][i];
                                }

                                double[][] temp_dj_v = CheckNaValues.GetArrayWithoutNaN(dj, v);
                                dj = temp_dj_v[0];
                                v = temp_dj_v[1];

                                calculatePolyPi(dj, prov, prov1, prov2, prov3, v, i);
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        objPi.Exception = "Polytomous items model not matched!";
                    }
                }
                else
                {
                    objPi.Exception = "Polytomous model not provided!";
                }

                objPi.Add(prov, prov1, prov2, prov3);

                //objPi.catDictionaryList = new Dictionary<string, Dictionary<string, catList>>();

                //GetCATList(objPi.Pi, objPi.catDictionaryList, "Pi");
                //GetCATList(objPi.dPi, objPi.catDictionaryList, "dPi");
                //GetCATList(objPi.d2Pi, objPi.catDictionaryList, "d2Pi");
                //GetCATList(objPi.d3Pi, objPi.catDictionaryList, "d3Pi");
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    if (objPi == null)
                    {
                        objPi = new PiListPoly();
                    }
                    objPi.Exception = ex.Message;
                }

                return objPi;
            }

            return objPi;
        }

        // Calculates Gamma values
        private static void calculatePolyPi(double[] dj, double[,] prov, double[,] prov1, double[,] prov2, double[,] prov3, double[] v, int i)
        {
            double[] Gammaj = new double[dj.Length];
            double[] dGammaj = new double[dj.Length];
            double[] d2Gammaj = new double[dj.Length];
            double[] d3Gammaj = new double[dj.Length];
            
            for (int c = 0; c < dj.Length; c++)
            {
                Gammaj[c] = Math.Exp(dj[c]);
                dGammaj[c] = Gammaj[c] * v[c];
                d2Gammaj[c] = Gammaj[c] * Math.Pow(v[c], 2);
                d3Gammaj[c] = Gammaj[c] * Math.Pow(v[c], 3);
            }

            double Sg = RowColumn.Sum(Gammaj);
            double Sdg = RowColumn.Sum(dGammaj);
            double Sd2g = RowColumn.Sum(d2Gammaj);
            double Sd3g = RowColumn.Sum(d3Gammaj);

            for (int index = 0; index < Gammaj.Length; index++)
            {
                prov[i, index] = Gammaj[index] / Sg;

                prov1[i, index] = dGammaj[index] / Sg - Gammaj[index] * Sdg / Math.Pow(Sg,2);

                prov2[i, index] = d2Gammaj[index] / Sg - 2 * dGammaj[index] * Sdg / Math.Pow(Sg, 2) - Gammaj[index] * 
                    Sd2g / Math.Pow(Sg, 2) + 2 * Gammaj[index] * Math.Pow(Sdg,2) / Math.Pow(Sg,3);

                prov3[i, index] = d3Gammaj[index] / Sg - (Gammaj[index] * Sd3g + 3 * dGammaj[index] * Sd2g + 3 * d2Gammaj[index] * Sdg) / Math.Pow(Sg,2) + 
                    (6 * Gammaj[index] * Sdg * Sd2g + 6 * dGammaj[index] * Math.Pow(Sdg,2)) / Math.Pow(Sg,3) - 6 * Gammaj[index] * Math.Pow(Sdg,3) / Math.Pow(Sg,4);
            }
        }

        // Get CAT named list
        private static void GetCATList(double[,] param, Dictionary<string, Dictionary<string, catList>> catPiList, string objName)
        {
            Dictionary<string, catList> tempDict = new Dictionary<string, catList>();
            catList objCat = new catList();
            int rowLength = param.GetLength(0);  // return number of rows in the array  "nrow"
            int columnLength = param.GetLength(1);
            double cat0 = 0, cat1 = 0, cat2 = 0, cat3 = 0; 
            
            for (int row = 0; row < rowLength; row++)
            {
                for (int col = 0; col < columnLength; col++)
                {
                    if (col == 0)
                    {
                        cat0 = param[row, col];
                    }
                    if (col == 1)
                    {
                        cat1 = param[row, col];
                    }
                    if (col == 2)
                    {
                        cat2 = param[row, col];
                    }
                    if (col == 3)
                    {
                        cat3 = param[row, col];
                    }
                }
                objCat.Add(cat0, cat1, cat2, cat3);
                tempDict.Add("Item" + row.ToString(), objCat);
            }
            catPiList.Add(objName, tempDict);
        }
    }
}