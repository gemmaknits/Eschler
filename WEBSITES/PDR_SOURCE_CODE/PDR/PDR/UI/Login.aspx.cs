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
            // 09/10/2026 John - URL login: UI/Login.aspx?user_id=<username>&pwd=<password>
            if (!IsPostBack) // 09/10/2026 John
            { // 09/10/2026 John
                if (Session["LoginMsg"] != null) // 09/10/2026 John - show + clear message from a failed URL login
                { // 09/10/2026 John
                    lblLoginMsg.Text = Session["LoginMsg"].ToString(); // 09/10/2026 John
                    Session["LoginMsg"] = null; // 09/10/2026 John
                } // 09/10/2026 John

                string urlUserID = Request.QueryString["user_id"]; // 09/10/2026 John
                if (!string.IsNullOrEmpty(urlUserID)) // 09/10/2026 John
                { // 09/10/2026 John
                    string urlPwd = Request.QueryString["pwd"] ?? ""; // 09/10/2026 John
                    bool loginOK = false; // 09/10/2026 John
                    try // 09/10/2026 John
                    { // 09/10/2026 John
                        classValidateUserLogin_BLL urlValidation = new classValidateUserLogin_BLL(); // 09/10/2026 John
                        string[] urlUser = urlValidation.ValidateLoginByUrl(urlUserID.Trim(), urlPwd.Trim()); // 09/10/2026 John
                        if (urlUser != null && urlUser[3] == "Y") // 09/10/2026 John - login_allowed
                        { // 09/10/2026 John
                            Session["userid"] = urlUser[0]; // 09/10/2026 John
                            Session["username"] = urlUser[1]; // 09/10/2026 John
                            Session["usersessionid"] = urlUser[2]; // 09/10/2026 John
                            loginOK = true; // 09/10/2026 John
                        } // 09/10/2026 John
                    } // 09/10/2026 John
                    catch (Exception) // 09/10/2026 John - DB error etc. treated as failed login
                    { // 09/10/2026 John
                        loginOK = false; // 09/10/2026 John
                    } // 09/10/2026 John

                    // 09/10/2026 John - redirects kept outside try/catch (ThreadAbortException)
                    if (loginOK) // 09/10/2026 John
                    { // 09/10/2026 John
                        Response.Redirect("~/ui/MAIN.aspx"); // 09/10/2026 John
                        return; // 09/10/2026 John
                    } // 09/10/2026 John
                    Session["LoginMsg"] = "Invalid username or password, please login."; // 09/10/2026 John
                    Response.Redirect("~/UI/Login.aspx"); // 09/10/2026 John - clean URL, no query string
                    return; // 09/10/2026 John
                } // 09/10/2026 John
            } // 09/10/2026 John

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