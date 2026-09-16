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

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtPDRNO.Text = "";
            txtCustomer.Text = "";
            ddlcategory.SelectedValue = "";
            ddlSubCategory.SelectedValue = "";
            ddlGroup.SelectedValue = "";
            ddlSubGroup.SelectedValue = "";
            ddlApplication.SelectedIndex = -1;
            ddlSubApplication.SelectedIndex = -1;
            DateTime dt = DateTime.Now.AddDays(-90);
            DateTime dt1 = DateTime.Now;
            dtFromDate.Value = dt.ToString("dd/MM/yyyy").Replace('-', '/');// DateTime.Now.AddDays(-70).ToString();//.ToShortDateString();

            dtTo.Value = dt1.ToString("dd/MM/yyyy").Replace('-', '/');
            txtInternalApproveDTFrom.Value = "";
            txtInternalApproveDTTo.Value = "";
            txtCustomerApproveDTFrom.Value = "";
            txtCustomerApproveDTTo.Value = "";
            txtFinalApproveDTFrom.Value = "";
            txtFinalApproveDTTo.Value = "";
            ddlRequestedBy.SelectedIndex = -1;
            ddlPDRprepared.SelectedIndex = -1;
            chkshowclosedpdr.Checked = false;
            chkaddedtocollection.Checked = false;
            chkInterApproval.Checked = false;
            chkCustomerApproval.Checked = false;
            chkFinalApproval.Checked = false;
            chkshortage.Checked = false;
            chkNewYarn.Checked = false;
            chkdrwaitingso.Checked = false;

        }

        protected void btnFind_Click(object sender, EventArgs e)
        {

            DataTable dtArticlelist;
            classPDR_List_BLL pdrlist = new classPDR_List_BLL();
            dtArticleList = pdrlist.LoadArticleList();
            grdArticleList.DataSource = dtArticleList;
            grdArticleList.DataBind();
        }
    }
}