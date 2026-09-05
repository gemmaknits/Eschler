using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PDR.UI
{
    public partial class Main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnNewQuote_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UI/QuoteV2.aspx");
        }

        protected void btnViewQuotations_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UI/ESTIMATElIST.aspx");
        }

        //protected void btnlOGOUT_Click(object sender, EventArgs e)
        //{
        //    Session["usersessionid"] = null;
        //    Session.Abandon();
        //    Response.Redirect("~/ui/login.aspx");
        //}

        protected void BTNLOGOUT_Click1(object sender, EventArgs e)
        {

            Session["usersessionid"] = null;
            Session.Abandon();
            Response.Redirect("~/ui/login.aspx");
        }
    }
}