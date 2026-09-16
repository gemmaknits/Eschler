using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PDR.BLL;
using Syncfusion.JavaScript.Models;
using Syncfusion.JavaScript.Web;
namespace PDR.UI
{
    public partial class PDRArticleList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                Response.Redirect("~/ui/login.aspx?pagename=PDR_List");
            }
            if (!IsPostBack)
            {
                btnFind_Click();
            }
        }

        private void populate_PDRRequestor()
        {
            DataSet dspdrrequestor;
            classPDR_List_BLL pdrrequestor = new classPDR_List_BLL();
            dspdrrequestor = pdrrequestor.Populate_PDR_REQUESTORS();
 
        }
 
 
        protected void btnFind_Click()
        {
            DataTable dtArticleList;
            classPDR_List_BLL pdrlist = new classPDR_List_BLL();
            dtArticleList = pdrlist.LoadArticleList();
            grdArticleList.DataSource = dtArticleList;
            grdArticleList.DataBind();

        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UI/develop_request.aspx");
        }


  
    }
}