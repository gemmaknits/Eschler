using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using PDR.BLL;

namespace PDR.UI
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sessionID = Request.QueryString["SessionID"];
            if (sessionID != null && sessionID != "")
            {
                SqlDataReader objreader = null;
                 sessionID = Request.QueryString["SessionID"].ToString();
                classValidateUserLogin_BLL userValidation = new classValidateUserLogin_BLL();
                objreader = userValidation.ValidateLoginFromEmail(sessionID);
               string session_Valid=objreader["session_valid"].ToString();
                if (session_Valid == "Y")
                {
                    string pdrno = Session["pdrno"].ToString();
                    Session["usersessionid"] = sessionID;
                    string pagename = Request.QueryString["pagename"];
                    string designno = Request.QueryString["designno"];
                    if (pagename.Trim().ToUpper() == "PDR_APPROVAL")
                    {
                        Response.Redirect("~/UI/PDR_approval.aspx?pagename=PDR_approval&pdrno=" + pdrno);
                    }
                    if (pagename.Trim().ToUpper() == "DR_KNITTING_SAMPLE_APPROVE")
                    {
                        Response.Redirect("~/UI/DR_Knitting_Sample_Approve.aspx?pagename=DR_Knitting_Sample_ApproveE&designno=" + designno);
                    }
                }

            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SqlDataReader objreader = null;
            string strUserID;
            string strUserName = null;
            string strUserSessionID = null;
            string strLoginAllowed = null;
            classValidateUserLogin_BLL userValidation = new classValidateUserLogin_BLL();
            objreader = userValidation.ValidateLogin(txtUserName.Text.Trim(), txtPWD.Text.Trim());

            strUserID = objreader["userid"].ToString();
            strUserName = objreader["username"].ToString();
            strUserSessionID = objreader["user_session_id"].ToString();
            strLoginAllowed = objreader["login_allowed"].ToString();
            objreader.Close();
            if (strLoginAllowed == "Y")
            {
                Session["userid"] = strUserID.Trim();
                Session["username"] = strUserName.Trim();
                Session["usersessionid"] = strUserSessionID.Trim();
                string pagename = Request.QueryString["pagename"];
                string custcode = Request.QueryString["custcode"];
               string sourcedocnumber = Request.QueryString["SOURCEDOCNO"];
                string product = Request.QueryString["PRODUCT"];
                string pdrno = Request.QueryString["pdrno"];
                string DRNO = Request.QueryString["drno"];
                string designno = Request.QueryString["designno"];
                if (pagename != null && pagename !="")
                {
                    
                    pagename = pagename + ".aspx";
                   // Response.Redirect("~/ui/" + pagename + "?pdrno=" + pdrno);
                    Response.Redirect("~/ui/" + pagename + "?pdrno=" + pdrno + "&custcode=" + custcode + "&SOURCEDOCNO=" + sourcedocnumber + "&PRODUCT=" + product + "&DRNO=" + DRNO + "&designno=" + designno);

                }
                else
                {
                    Response.Redirect("~/ui/MAIN.aspx");
                }
                //Response.Redirect("~ui/OrderList.aspx");
            }
        }
    }
}