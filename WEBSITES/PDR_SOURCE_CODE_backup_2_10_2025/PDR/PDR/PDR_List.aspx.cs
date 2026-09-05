using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PDR.BLL;
namespace PDR.UI
{
    public partial class PDR_List : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                Response.Redirect("~/ui/login.aspx");
            }
            if (!IsPostBack)
            {
                populate_ItemCategory();
                populate_ItemSubCategory();
                populate_ItemGroups();
                populate_ItemSubGroups();
                populate_ItemApplication();
                populate_ItemSubApplication();
                populate_PDRRequestor();
                txtRequestedBy.Text = Session["username"].ToString();
                DateTime dt = DateTime.Now.AddDays(-90);
                DateTime dt1 = DateTime.Now;
                dtFromDate.Value = dt.ToString("dd/MM/yyyy").Replace('-', '/');// DateTime.Now.AddDays(-70).ToString();//.ToShortDateString();

                dtTo.Value = dt1.ToString("dd/MM/yyyy").Replace('-', '/');
            }
        }

        private void populate_PDRRequestor()
        {
            DataSet dspdrrequestor;
            classPDR_List_BLL pdrrequestor = new classPDR_List_BLL();
            dspdrrequestor = pdrrequestor.Populate_PDR_REQUESTORS();
            ddlPDRprepared.DataSource = dspdrrequestor;
            ddlPDRprepared.DataTextField = "empname";
            ddlPDRprepared.DataValueField = "empcd";
            ddlPDRprepared.DataBind();
            ddlPDRprepared.SelectedValue = "";
        }
        private void populate_ItemCategory()
        {
            DataSet dscategory;
            classPDR_List_BLL itemcategory = new classPDR_List_BLL();
            dscategory = itemcategory.Populate_Item_Category("FABRIC");
            ddlcategory.DataSource = dscategory;
            ddlcategory.DataTextField = "itcatdesc";
            ddlcategory.DataValueField = "itcatid";
            ddlcategory.DataBind();
            ddlcategory.SelectedValue = "";
        }
        private void populate_ItemSubCategory()
        {
            DataSet dssubcategory;
            classPDR_List_BLL itemsubcategory = new classPDR_List_BLL();
            dssubcategory = itemsubcategory.Populate_Item_SubCategory("FABRIC");
            ddlSubCategory.DataSource = dssubcategory;
            ddlSubCategory.DataTextField = "itsubcatdesc";
            ddlSubCategory.DataValueField = "itsubcatid";
            ddlSubCategory.DataBind();
            ddlSubCategory.SelectedValue = "";
        }

        private void populate_ItemGroups()
        {
            DataSet dsgroup;
            classPDR_List_BLL itemcategory = new classPDR_List_BLL();
            dsgroup = itemcategory.Populate_Item_Group("FABRIC");
            ddlGroup.DataSource = dsgroup;
            ddlGroup.DataTextField = "itgroupdesc";
            ddlGroup.DataValueField = "itgroupid";
            ddlGroup.DataBind();
            ddlGroup.SelectedValue = "";
        }

        private void populate_ItemSubGroups()
        {
            DataSet dssubgroup;
            classPDR_List_BLL itemcategory = new classPDR_List_BLL();
            dssubgroup = itemcategory.Populate_Item_SubGroup("FABRIC");
            ddlSubGroup.DataSource = dssubgroup;
            ddlSubGroup.DataTextField = "itsubdesc";
            ddlSubGroup.DataValueField = "itsubid";
            ddlSubGroup.DataBind();
            ddlSubGroup.SelectedValue = "";
        }
       
        private void populate_ItemApplication()
        {
            DataSet dsapplication;
            classPDR_List_BLL itemapplications = new classPDR_List_BLL();
            dsapplication = itemapplications.Populate_Item_Application();
            ddlApplication.DataSource = dsapplication;
            ddlApplication.DataTextField = "lookup_value";
            ddlApplication.DataValueField = "lookup_value_id";
            ddlApplication.DataBind();
            ddlApplication.SelectedValue = "";
        }

        private void populate_ItemSubApplication()
        {
            DataSet dssubapplication;
            classPDR_List_BLL itemsubapplications = new classPDR_List_BLL();
            dssubapplication = itemsubapplications.Populate_Item_SubApplication();
            ddlSubApplication.DataSource = dssubapplication;
            ddlSubApplication.DataTextField = "lookup_value";
            ddlSubApplication.DataValueField = "lookup_value_id";
            ddlSubApplication.DataBind();
            ddlSubApplication.SelectedValue = "";
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            string pdrfromdate = "";
            string pdrtodate = "";
            string subcategoryid = "";
            string categoryid = "";
            string groupid = "";
            string subgroupid = "";
            string applicationid = "";
            string subapplicationid = "";
            string preparedby = "";
            string requestedby = "";
            string p_customer = "";
            requestedby = Session["usersessionid"].ToString();
            pdrfromdate = dtFromDate.Value;
            pdrtodate = dtTo.Value;
            subcategoryid = ddlSubCategory.SelectedValue.ToString();
            categoryid = ddlcategory.SelectedValue.ToString();
            groupid = ddlGroup.SelectedValue.ToString();
            subgroupid = ddlSubGroup.SelectedValue.ToString();
            applicationid = ddlApplication.SelectedValue.ToString();
            subapplicationid = ddlSubApplication.SelectedValue.ToString();
            preparedby = ddlPDRprepared.SelectedItem.Value;
            p_customer = txtCustomer.Text.ToString().Trim();
            DataTable dtpdrlist;
            classPDR_List_BLL pdrlist = new classPDR_List_BLL();
            dtpdrlist = pdrlist.LoadPDRList(pdrfromdate, pdrtodate, subcategoryid.Trim(), groupid.Trim(), applicationid.Trim(), subapplicationid.Trim(), "", "", 
                txtPDRNO.Text.Trim(), requestedby.Trim(), p_customer, preparedby);
            grdPDRList.DataSource = dtpdrlist;
            grdPDRList.DataBind();

        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UI/develop_request.aspx");
        }
    }
}