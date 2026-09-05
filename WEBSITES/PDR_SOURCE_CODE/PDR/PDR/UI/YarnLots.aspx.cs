using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PDR.BLL;
using System.Data;
namespace PDR.UI
{
    public partial class YarnLots : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                string kono = Request.QueryString["kikono"];
               
                Response.Redirect("~/ui/login.aspx?pagename=YARNLOTS" );
            }
            if (!IsPostBack)
            {
                string kono = Request.QueryString["kikono"];
                LBLKIKO.Text = kono;
                string designno = Request.QueryString["designno"];
                LBLDESIGNNO.Text = designno;
                string SESSIONID = Session["usersessionid"].ToString();
                classYarnLot_BLL obj = new classYarnLot_BLL();
                DataTable dt1 = obj.LoadeYarnLotDetails(kono,SESSIONID);
                grdYarnLo.DataSource = dt1;
                grdYarnLo.DataBind();
            }
        }
    }
}