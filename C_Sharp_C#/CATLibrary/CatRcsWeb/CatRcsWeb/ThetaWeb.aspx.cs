using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CatRcs.Models;
using System.Globalization;

namespace CatRcsWeb
{
    public partial class ThetaWeb : System.Web.UI.Page
    {
        Dictionary<string, double[]> cat_items = null;
        bool IsDicho = false, IsPoly = false;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void ClearData()
        {
            txtItemID.Text = "";
            txtResponse.Text = "";
            lblTest.Text = "";
            chkDicho.Enabled = true;
            chkPoly.Enabled = true;
            chkDicho.Checked = false;
            chkPoly.Checked = false;
            Session["IsDicho"] = false;
            Session["IsPoly"] = false;
            cat_items = new Dictionary<string, double[]>();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtItemID.Text) || string.IsNullOrEmpty(txtResponse.Text))
                {
                    lblTest.Text = "Please enter Item IDs and Response categories!";
                }
                else if (chkDicho.Enabled == true && chkDicho.Checked == false)
                {
                    lblTest.Text = "Please select at least one model type.";
                }
                else if (chkPoly.Enabled == true && chkPoly.Checked == false)
                {
                    lblTest.Text = "Please select at least model type.";
                }
                else
                {
                    cat_items = new Dictionary<string, double[]>();

                    // Handling user item data
                    string item_data = txtItemID.Text;
                    string[] item_ids = item_data.Split(new char[] { ',', ' ' });
                    int[] user_items = item_ids.Select(p => Convert.ToInt32(p)).ToArray();

                    // Handling user item data
                    string response_data = txtResponse.Text;
                    string[] responses = response_data.Split(new char[] { ',', ' ' });
                    int[] user_responses = responses.Select(p => Convert.ToInt32(p)).ToArray();

                    // Load Items from Item pool
                    LoadItems(user_items);

                    // Create CAT Item parameter
                    ProcessTheta(user_items, user_responses);

                    // Clear all control data
                    ClearData();

                    lblTest.ForeColor = System.Drawing.Color.Green;
                    lblTest.Text = "Theta estimation is successful !!";
                }
            }
            catch(Exception ex)
            {
                lblTest.Text = ex.Message;
            }
        }

        protected void chkDicho_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDicho.Checked)
            {
                chkPoly.Enabled = false;
                IsDicho = true;
            }
            else
            {
                chkPoly.Enabled = true;
                IsDicho = false;
            }

            Session["IsDicho"] = IsDicho;
            Session["IsPoly"] = IsPoly;
            lblTest.Text = "";
        }

        protected void chkPoly_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPoly.Checked)
            {
                chkDicho.Enabled = false;
                IsPoly = true;
            }
            else
            {
                chkDicho.Enabled = true;
                IsPoly = false;
            }

            Session["IsDicho"] = IsDicho;
            Session["IsPoly"] = IsPoly;
            lblTest.Text = "";
        }

        private void LoadItems(int[] items)
        {
            double[] new_items = null;
            string all_data = null;
            string[] temp_items = null;

            if ((bool)Session["IsDicho"])
            {
                all_data = UtilityFunc.GetItemData(HttpContext.Current.Server.MapPath("~/App_Data/tcals.txt"));

                if (!string.IsNullOrEmpty(all_data))
                {
                    temp_items = all_data.Split(new char[] { '\n' });

                    // Get column names
                    string[] colNames = temp_items[0].Split(new char[] { '\t' });

                    // Get values from Column wise
                    for (int col = 0; col < colNames.Length - 1; col++)
                    {
                        new_items = new double[items.Length];

                        // Get values row wise
                        for (int index = 0; index < items.Length; index++)
                        {
                            string[] item = temp_items[items[index]].Split(new char[] { '\t' });

                            new_items[index] = double.Parse(item[col], System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));
                        }

                        // Add columns to the dictionary
                        cat_items.Add(colNames[col] + "0", new_items);
                    }
                }
            }

            if ((bool)Session["IsPoly"])
            {
                all_data = UtilityFunc.GetItemData(HttpContext.Current.Server.MapPath("~/App_Data/Poly_GRM.txt"));
               
                if (!string.IsNullOrEmpty(all_data))
                {
                    temp_items = all_data.Split(new char[] { '\n', '\r' });
                    temp_items = temp_items.Where(p => p != "").Select(q => q).ToArray();

                    // Get column names
                    string[] colNames = temp_items[0].Split(new char[] { '\t' });

                    // Get values Column wise
                    for (int col = 0; col < colNames.Length; col++)
                    {
                        new_items = new double[items.Length];

                        // Get values row wise
                        for (int index = 0; index < items.Length; index++)
                        {
                            string[] item = temp_items[items[index]].Split(new char[] { '\t' });

                            new_items[index] = double.Parse(item[col], System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));
                        }

                        // Add columns to the dictionary
                        if(col == 0)
                        {
                            cat_items.Add(colNames[col] + col.ToString(), new_items);
                        }
                        else
                        {
                            cat_items.Add(colNames[col], new_items);
                        }
                    }
                }
            }
        }

        private void ProcessTheta(int[] items, int[] responses)
        {
            CATItems itemBank = null;

            if ((bool)Session["IsDicho"])
            {
                itemBank = new CATItems(items.Length);
                Tuple<CATItems.ColumnNames, int>[] cols = itemBank.GetKeys();

                for (int i = 0; i < cols.Length; i++)
                {
                    string tempKey = cols[i].Item1.ToString() + cols[i].Item2.ToString();
                    itemBank.SetItemParamter(cols[i], cat_items[tempKey].Select(y => (double)y).ToArray());
                }

                // Calculate theta
                CalculateTheta(itemBank, responses);
            }

            if ((bool)Session["IsPoly"])
            {
                ModelNames.Models paramModel = ModelNames.Models.GRM;
                itemBank = new CATItems(items.Length, paramModel.EnumToString(), nrCat: 5);
                Tuple<CATItems.ColumnNames, int>[] cols = itemBank.GetKeys();

                for (int i = 0; i < cols.Length; i++)
                {
                    string tempKey = cols[i].Item1.ToString() + cols[i].Item2.ToString();
                    itemBank.SetItemParamter(cols[i], cat_items[tempKey].Select(y => (double)y).ToArray());
                }

                // Calculate theta
                CalculateTheta(itemBank, responses, model: paramModel.EnumToString());
            }
        }

        private void CalculateTheta(CATItems itemBank, int[] responses, string model = "")
        {
            if (string.IsNullOrEmpty(model))
            {
                // Calling "thetaEST" function
                double cs_ThetaEst = CatRcs.CatRLib.ThetaEst(itemBank, responses, "");
                Session["Theta_Value"] = cs_ThetaEst;
            }
            else
            {
                double[] priorPar = new double[2]; priorPar[0] = -2; priorPar[1] = 2;

                // Calling "thetaEST" function
                double cs_ThetaEst = CatRcs.CatRLib.ThetaEst(it: itemBank, x: responses, method: "BM", model: model, priorPar: priorPar, priorDist: "Jeffreys");
                Session["Theta_Value"] = cs_ThetaEst;
            }
        }
    }
}