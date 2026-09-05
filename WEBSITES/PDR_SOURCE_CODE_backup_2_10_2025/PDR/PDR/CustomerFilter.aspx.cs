using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using PDR.BLL;
using System.Web.Script.Services;
using System.Security.Cryptography;
namespace PDR.UI
{
    public partial class CustomerFilter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

       

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string[] GetAutoCompleteData(string prefix)
        {
            DataTable dt = new DataTable();
            // List<Issues> objDept = new List<Issues>();
            List<string> customers = new List<string>();
            classDevelop_Request_BLL getcustomer = new classDevelop_Request_BLL();
            dt = getcustomer.Populate_Issue_dt(prefix);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Issues DataObj = new Issues();

                    //DataObj.poc_issue_master_id = Convert.ToInt32(dt.Rows[i]["poc_issue_master_id"]);
                    //DataObj.issue_name = dt.Rows[i]["issue_name"].ToString();
                    //objDept.Add(DataObj);
                    string chk = dt.Rows[i]["custcd"].ToString();
                    if (chk == "-1")
                    {
                        // customers.Add(string.Format("{0}- ", Convert.ToInt32(dt.Rows[i]["poc_issue_master_id"]), "' '"));
                        customers.Add(string.Format("'--1',{1} ", Convert.ToInt32(dt.Rows[i]["custcd"]), "' '"));
                    }
                    else
                    {
                        customers.Add(string.Format("{0}-{1}", Convert.ToInt32(dt.Rows[i]["custcd"]), dt.Rows[i]["name"].ToString()));
                    }

                }
                return customers.ToArray();
            }

            else
            {
                return null;
            }

        }


        [System.Web.Services.WebMethod]
        public static string GetCurrentTime()
        {
            return "Hello kk " + Environment.NewLine + "The Current Time is: "
                + DateTime.Now.ToString();
        }
    }
}