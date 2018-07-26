using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using LPMathSolver;

namespace CatRcsWeb
{
    public partial class LPWeb : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                string[] modelFile = System.IO.Directory.GetFiles(HttpContext.Current.Server.MapPath("~/App_Data/")).Where(p => p.Contains("mod")).Select(q => q).ToArray();

                FileStream dataStream = new FileStream(modelFile[0], FileMode.Open);

                byte[] data = new byte[dataStream.Length];
                int read_count = dataStream.Read(data, 0, data.Length);
                string all_data = new string(Encoding.UTF8.GetChars(data), 0, read_count);
                dataStream.Close();

                Dictionary<string, string> Result_GLPK = new Dictionary<string, string>();
                Dictionary<string, string> Result_LP = new Dictionary<string, string>();

                // Call LP Libraries
                // Change the parameter "SolverType", values: "GLPK" or "LPSolve"
                
                if(chkGlpk.Checked || chkLP.Checked)
                {
                    divDownloadFiles.Visible = true;

                    if (chkGlpk.Checked)
                    {
                        // GLPK
                        SolverLib.func_Solver(all_data, Utility.SolverType.GLPK, out Result_GLPK, HttpContext.Current.Server.MapPath("~/App_Data/"));
                        btnGLPK.Visible = true;
                    }

                    if (chkLP.Checked)
                    {
                        // LPSolve
                        SolverLib.func_Solver(all_data, Utility.SolverType.LPSolve, out Result_LP, HttpContext.Current.Server.MapPath("~/App_Data/"));
                        btnLP.Visible = true;
                    }

                    lblTest.ForeColor = System.Drawing.Color.Green;
                    lblTest.Text = "LP problem has processed successfully.";
                }
                else
                {
                    lblTest.Text = "Please select LP types properly !!";
                }
                
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
            }
        }

        private void SaveFile(string fileName)
        {
            try
            {

                System.IO.FileInfo Dfile = new System.IO.FileInfo(Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), fileName));

                if (Dfile.Exists)
                {
                    Response.ContentType = "text/plain";
                    String Header = "Attachment; Filename=" + fileName;
                    Response.AppendHeader("Content-Disposition", Header);
                    Response.WriteFile(Dfile.FullName);
                    Response.End();
                }

            }
            catch (Exception ex)
            {
                lblTest.Text = ex.Message;
            }
        }

        protected void btnGLPK_Click(object sender, EventArgs e)
        {
            SaveFile("result_glpk.txt");
        }

        protected void btnLP_Click(object sender, EventArgs e)
        {
            SaveFile("result_lpSolve.txt");
        }
    }
}