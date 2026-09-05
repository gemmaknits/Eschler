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
                Response.Redirect("~/ui/pdr_list.aspx");
                //Response.Redirect("~ui/OrderList.aspx");
            }
        }
    }
}