using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PDR.BLL;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
namespace PDR.UI
{
    public partial class AddEditCustomer : System.Web.UI.Page
    {
        DataSet dscountry;
        private Develop_Request otherForm = new Develop_Request();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                Response.Redirect("~/ui/login.aspx");
            }
            if (!IsPostBack)
            {
                populate_Country();
                string custcode = Request.QueryString["custcode"];//.ToString();
                if (custcode != "")
                {
                    classCustomer_Details_BLL custdetails = new classCustomer_Details_BLL();
                    DataTable dt_custdetails = custdetails.LoadCustomerDetails(custcode);
                    if (dt_custdetails.Rows.Count > 0)
                    {
                        foreach (DataRow resultrow in dt_custdetails.Rows)
                        {
                            txtCode.Text = resultrow[0].ToString();
                            txtName.Text = resultrow[1].ToString();
                            txtNameThai.Text = resultrow["namet"].ToString();
                            txtAddress.Text = resultrow["addr1"].ToString();
                            txtAddress2.Text = resultrow["addr2"].ToString();
                            txtAddress3.Text = resultrow["addr3"].ToString();
                            txtAddressThai.Text = resultrow["addr1t"].ToString();
                            txtAddressThai2.Text = resultrow["addr2t"].ToString();
                            txtAddressThai3.Text = resultrow["addr3t"].ToString();
                            txtCity.Text = resultrow["city"].ToString();
                            ddlCountry.SelectedValue = resultrow["ctry"].ToString();
                            txtTel.Text = resultrow["tel"].ToString();
                            txtFax.Text = resultrow["fax"].ToString();
                            txtEmail.Text = resultrow["email"].ToString();
                            txtContact.Text = resultrow["contact"].ToString();
                        }
                    }
                }
                //Page.PreviousPage.FindControl("txtCustCode");
                // string tt = otherForm.txtCustCode.Text;
                //TextBox txt= (TextBox)Page.PreviousPage.FindControl("txtCustCode");
                //string ss= Request[txt.UniqueID] as string;
                // string tt = txt.Text;
            }
        }
        private void populate_Country()
        {

            classPDR_List_BLL country = new classPDR_List_BLL();
            dscountry = country.Populate_POC_Combo_Country();
            ddlCountry.DataSource = dscountry;
            ddlCountry.DataTextField = "ctry_name";
            ddlCountry.DataValueField = "ctry";
            ddlCountry.DataBind();
            ddlCountry.SelectedValue = "";
            ViewState["ctry"] = dscountry;

        }

        protected void ImgSave_Click(object sender, ImageClickEventArgs e)
        {
            string p_custcd;
            string p_name;
            string p_namet;
            string p_addr1;
            string p_addr2;
            string p_addr3;
            string p_addr1t;
            string p_addr2t;
            string p_addr3t;
            string p_city;
            string p_ctry;
            string p_tel;
            string p_fax;
            string p_email;
            string p_contact;
            string p_user_session_id = (Session["usersessionid"].ToString());
            p_custcd = txtCode.Text.Trim();
            p_name = txtName.Text.Trim();
            p_namet = txtNameThai.Text.Trim();
            p_addr1 = txtAddress.Text.Trim();
            p_addr2 = txtAddress2.Text.Trim();
            p_addr3 = txtAddress3.Text.Trim();
            p_addr1t = txtAddressThai.Text.Trim();
            p_addr2t = txtAddressThai2.Text.Trim();
            p_addr3t = txtAddressThai3.Text.Trim();
            p_city = txtCity.Text.Trim();
            p_ctry = ddlCountry.SelectedItem.Value;
            p_tel = txtTel.Text.Trim();
            p_fax = txtFax.Text.Trim();
            p_email = txtEmail.Text.Trim();
            p_contact = txtContact.Text.Trim();
            classCustomer_Details_BLL objCustomerDetails = new classCustomer_Details_BLL();
            DataTable dt_CustomerDetails = objCustomerDetails.UpdateCustomer(p_custcd, p_name, p_namet, p_addr1, p_addr2, p_addr3,
                                                                            p_addr1t, p_addr2t, p_addr3t,p_city, p_ctry, p_tel, p_fax, p_email,
                                                                            p_contact, p_user_session_id);
            foreach (DataRow resultrow in dt_CustomerDetails.Rows)
            {
                txtCode.Text = (resultrow[0].ToString());
                lblmsg.Visible = true;

            }
        }

        protected void ImgAdd_Click(object sender, ImageClickEventArgs e)
        {
            clearAll();
        }
        private void clearAll()
        {
            txtCode.Text = "";
            txtName.Text = "";
            txtNameThai.Text = "";
            txtAddress.Text = "";
            txtAddress2.Text = "";
            txtAddress3.Text = "";
            txtAddressThai.Text = "";
            txtAddressThai2.Text = "";
            txtAddressThai3.Text = "";
            txtCity.Text = "";
            txtFax.Text = "";
            txtTel.Text = "";
            txtContact.Text = "";
            ddlCountry.SelectedValue = "";
            txtEmail.Text = "";
            lblmsg.Visible = false;
        }

        [WebMethod]
        public static DetailsClass_GetCustDetails[] GetCustomerDetails(string custcode) //GetFacBomCode function

        {
            List<DetailsClass_GetCustDetails> obj_customerDetails = new List<DetailsClass_GetCustDetails>();
            classCustomer_Details_BLL obj_IssueDetails_BLL = new classCustomer_Details_BLL();
            DataTable obj_dt;
            obj_dt = obj_IssueDetails_BLL.LoadCustomerDetails(custcode);
            foreach (DataRow dtRow in obj_dt.Rows)
            {
                DetailsClass_GetCustDetails DataObj = new DetailsClass_GetCustDetails();
                DataObj.name = dtRow["name"].ToString();
                DataObj.namet = dtRow["namet"].ToString();
                DataObj.addr1 = dtRow["addr1"].ToString();
                DataObj.addr2 = dtRow["addr2"].ToString();
                DataObj.addr3 = dtRow["addr3"].ToString();
                DataObj.addr1t = dtRow["addr1t"].ToString();
                DataObj.addr2t = dtRow["addr2t"].ToString();
                DataObj.addr3t = dtRow["addr3t"].ToString();
                DataObj.city = dtRow["city"].ToString();
                DataObj.ctry = dtRow["ctry"].ToString();
                DataObj.tel = dtRow["tel"].ToString();
                DataObj.fax = dtRow["fax"].ToString();
                DataObj.email = dtRow["email"].ToString();
                DataObj.contact = dtRow["contact"].ToString();
                obj_customerDetails.Add(DataObj);
            }
            return obj_customerDetails.ToArray();
        }

        public class DetailsClass_GetCustDetails
        {
            public string name { get; set; }
            public string namet { get; set; }
            public string addr1 { get; set; }
            public string addr2 { get; set; }
            public string addr3 { get; set; }
            public string addr1t { get; set; }
            public string addr2t { get; set; }
            public string addr3t { get; set; }
            public string city { get; set; }
            public string ctry { get; set; }
            public string tel { get; set; }
            public string fax { get; set; }
            public string email { get; set; }
            public string contact { get; set; }

        }
        [WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string[] GetCustomers1(string prefix)
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
                        customers.Add(string.Format("{0}-{1}", (dt.Rows[i]["custcd"]), dt.Rows[i]["name"].ToString()));
                    }

                }
                return customers.ToArray();

            }

            else
            {
                return null;
            }

        }
    }
}