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
                Response.Redirect("~/ui/login.aspx?pagename=PDR_List");
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
                // txtRequestedBy.Text = Session["username"].ToString();
                populate_PDRUsers();
                populate_InternalAPPREJ();
                populate_CustomerAPPREJ();
                populate_FinalAPPREJ();
                populate_KnittingSampleApproveAPPREJ();
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
        private void populate_PDRUsers()
        {
            DataSet dspdrrequestor;
            classPDR_List_BLL pdrrequestor = new classPDR_List_BLL();
            dspdrrequestor = pdrrequestor.populate_PDRUsers();
            ddlRequestedBy.DataSource = dspdrrequestor;
            ddlRequestedBy.DataTextField = "empname";
            ddlRequestedBy.DataValueField = "empcd";
            ddlRequestedBy.DataBind();
            ddlRequestedBy.SelectedValue = Session["userid"].ToString().ToUpper();
            for (int i = 0; i < ddlRequestedBy.Items.Count; i++)
            {
                if (ddlRequestedBy.Items[i].Value.Trim() == Session["username"].ToString().ToUpper())
                {
                    ddlRequestedBy.SelectedIndex = i;
                }
            }
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
        private void populate_InternalAPPREJ()
        {
            DataSet dsAPPREJ;
            classDR_Approval_BLL apprej = new classDR_Approval_BLL();
            dsAPPREJ = apprej.Populate_APPREJ();
            ddlInternalAppRej.DataSource = dsAPPREJ;
            ddlInternalAppRej.DataTextField = "lookup_value";
            ddlInternalAppRej.DataValueField = "lookup_value_id";
            ddlInternalAppRej.DataBind();
            ddlInternalAppRej.SelectedValue = "";
        }
        private void populate_CustomerAPPREJ()
        {
            DataSet dsAPPREJ;
            classDR_Customer_Response_BLL apprej = new classDR_Customer_Response_BLL();
            dsAPPREJ = apprej.Populate_APPREJ();
            ddlCustomerApprej.DataSource = dsAPPREJ;
            ddlCustomerApprej.DataTextField = "lookup_value";
            ddlCustomerApprej.DataValueField = "lookup_value_id";
            ddlCustomerApprej.DataBind();
            ddlCustomerApprej.SelectedValue = "";
        }
        private void populate_FinalAPPREJ()
        {
            DataSet dsAPPREJ;
            classDR_Final_Response_BLL apprej = new classDR_Final_Response_BLL();
            dsAPPREJ = apprej.Populate_APPREJ();
            ddlFinalAppRej.DataSource = dsAPPREJ;
            ddlFinalAppRej.DataTextField = "lookup_value";
            ddlFinalAppRej.DataValueField = "lookup_value_id";
            ddlFinalAppRej.DataBind();
            ddlFinalAppRej.SelectedValue = "";
        }
        private void populate_KnittingSampleApproveAPPREJ()
        {
            DataSet dsAPPREJ;
            classKnitting_Sample_Approve apprej = new classKnitting_Sample_Approve();
            dsAPPREJ = apprej.Populate_APPREJ();
            ddlKnittingApprej.DataSource = dsAPPREJ;
            ddlKnittingApprej.DataTextField = "lookup_value";
            ddlKnittingApprej.DataValueField = "lookup_value_id";
            ddlKnittingApprej.DataBind();
            ddlKnittingApprej.SelectedValue = "";
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
            string p_show_closed_pdr;
            string p_internal_app_rej_date_from = "";
            string p_internal_app_rej_date_to = "";
            string p_customer_app_rej_date_from = "";
            string p_customer_app_rej_date_to = "";
            string p_final_app_rej_date_from = "";
            string p_final_app_rej_date_to = "";
            string p_wait_internal_app_rej = "N";
            string p_wait_customer_app_rej = "N";
            string p_wait_final_app_rej="N";
            string p_add_to_collection = "N";
            string p_new_yarn = "N";
            string p_yarn_shortage = "N";
            string p_no_order = "";
            string p_internal_app_rej_id;
            string p_customer_app_rej_id;
            string p_final_app_rej_id;
            string p_knitting_app_rej_id;
            
            requestedby = ddlRequestedBy.SelectedValue.Trim();// Session["usersessionid"].ToString();
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
            p_show_closed_pdr = chkshowclosedpdr.Checked.ToString();            
            if (p_show_closed_pdr.Trim() =="True" || p_show_closed_pdr.Trim() == "TRUE")
            {
                p_show_closed_pdr = "Y";
            }
            else
            {
                p_show_closed_pdr = "N";
            }
            p_internal_app_rej_date_from = txtInternalApproveDTFrom.Value;
            p_internal_app_rej_date_to = txtInternalApproveDTTo.Value;
            p_customer_app_rej_date_from = txtCustomerApproveDTFrom.Value;
            p_customer_app_rej_date_to = txtCustomerApproveDTTo.Value;
            p_final_app_rej_date_from = txtFinalApproveDTFrom.Value;
            p_final_app_rej_date_to = txtFinalApproveDTTo.Value;
            p_wait_internal_app_rej = chkInterApproval.Checked.ToString();
            if (p_wait_internal_app_rej.Trim() == "True" || p_wait_internal_app_rej.Trim() == "TRUE")
            {
                p_wait_internal_app_rej = "Y";
            }
            else
            {
                p_wait_internal_app_rej = "N";
            }
            p_wait_customer_app_rej = chkCustomerApproval.Checked.ToString();
            if (p_wait_customer_app_rej.Trim() == "True" || p_wait_customer_app_rej.Trim() == "TRUE")
            {
                p_wait_customer_app_rej = "Y";
            }
            else
            {
                p_wait_customer_app_rej = "N";
            }
            p_wait_final_app_rej = chkFinalApproval.Checked.ToString();
            if (p_wait_final_app_rej.Trim() == "True" || p_wait_final_app_rej.Trim() == "TRUE")
            {
                p_wait_final_app_rej = "Y";
            }
            else
            {
                p_wait_final_app_rej = "N";
            }
            p_add_to_collection = chkaddedtocollection.Checked.ToString();
            if (p_add_to_collection.Trim() == "True" || p_add_to_collection.Trim() == "TRUE")
            {
                p_add_to_collection = "Y";
            }
            else
            {
                p_add_to_collection = "N";
            }
            p_new_yarn = chkNewYarn.Checked.ToString();
            if (p_new_yarn.Trim() == "True" || p_new_yarn.Trim() == "TRUE")
            {
                p_new_yarn = "Y";
            }
            else
            {
                p_new_yarn = "N";
            }
            p_yarn_shortage = chkshortage.Checked.ToString();
            if (p_yarn_shortage.Trim() == "True" || p_yarn_shortage.Trim() == "TRUE")
            {
                p_yarn_shortage = "Y";
            }
            else
            {
                p_yarn_shortage = "N";
            }
            p_no_order = chkdrwaitingso.Checked.ToString();
            if (p_no_order.Trim() == "True" || p_no_order.Trim() == "TRUE")
            {
                p_no_order = "Y";
            }
            else
            {
                p_no_order = "N";
            }
            p_internal_app_rej_id = ddlInternalAppRej.SelectedValue.ToString();
            p_customer_app_rej_id = ddlCustomerApprej.SelectedValue.ToString();
            p_final_app_rej_id = ddlFinalAppRej.SelectedValue.ToString();
            p_knitting_app_rej_id = ddlKnittingApprej.SelectedValue.ToString();
            DataTable dtpdrlist;
            classPDR_List_BLL pdrlist = new classPDR_List_BLL();
            dtpdrlist = pdrlist.LoadPDRList(pdrfromdate, pdrtodate,categoryid.Trim(), subcategoryid.Trim(), groupid.Trim(),subgroupid.Trim(), applicationid.Trim(), subapplicationid.Trim(), "", "",
                txtPDRNO.Text.Trim(), requestedby.Trim(), p_customer, preparedby, p_show_closed_pdr, p_internal_app_rej_date_from,p_internal_app_rej_date_to,
                p_customer_app_rej_date_from,p_customer_app_rej_date_to,p_final_app_rej_date_from,p_final_app_rej_date_to,p_wait_internal_app_rej,
                p_wait_customer_app_rej,p_wait_final_app_rej,p_add_to_collection,p_new_yarn,p_yarn_shortage, p_no_order, p_internal_app_rej_id,
                p_customer_app_rej_id, p_final_app_rej_id, p_knitting_app_rej_id);
            grdPDRList.DataSource = dtpdrlist;
            grdPDRList.DataBind();

        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UI/develop_request.aspx");
        }
        protected void grdPDRList_onRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (grdPDRList.DataKeys[e.Row.RowIndex].Values[0].ToString() == "Y" || grdPDRList.DataKeys[e.Row.RowIndex].Values[0].ToString() == null)
                {
                    e.Row.Font.Strikeout = true;
                }
            }
        }

        protected void btnTrackingReport_Click(object sender, EventArgs e)
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
            string pdrno = "";
            requestedby = ddlRequestedBy.SelectedValue.Trim();
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
            pdrno = txtPDRNO.Text.Trim();
            Session["pdrfromdate"] = pdrfromdate;
            Session["pdrtodate"] = pdrtodate;
            Session["categoryid"] = categoryid;
            Session["subcategoryid"] = subcategoryid;
            Session["groupid"] = groupid;
            Session["subgroupid"] = subgroupid;
            Session["applicationid"] = applicationid;
            Session["subapplicationid"] = subapplicationid;
            Session["categoryname"] = ddlcategory.SelectedItem.Text.Trim();
            Session["subcategoryname"] = ddlcategory.SelectedItem.Text.Trim();
            Session["groupname"] = ddlGroup.SelectedItem.Text.Trim();
            Session["subgroupname"] = ddlSubGroup.SelectedItem.Text.Trim();
            Session["applicationname"] = ddlApplication.SelectedItem.Text.Trim();
            Session["subapplicationname"] = ddlSubApplication.SelectedItem.Text.Trim();
            Session["preparedby"] = preparedby;
            Session["requestedby"] = requestedby;
            Session["p_customer"] = p_customer;
            Session["pdrno"] = pdrno;
            if (chkshowclosedpdr.Checked)
            {
                Session["p_show_closed_pdr"] = "Y";
            }
            else
            {
                Session["p_show_closed_pdr"] = "";
            }
            
            Session["p_internal_app_rej_date_from"] = txtInternalApproveDTFrom.Value;
            Session["p_internal_app_rej_date_to"] = txtInternalApproveDTTo.Value;
            Session["p_customer_app_rej_date_from"] = txtCustomerApproveDTFrom.Value;
            Session["p_customer_app_rej_date_to"] = txtCustomerApproveDTTo.Value;
            Session["p_final_app_rej_date_from"] = txtFinalApproveDTFrom.Value;
            Session["p_final_app_rej_date_to"] = txtFinalApproveDTTo.Value;
            if (chkInterApproval.Checked)
            {
                Session["p_wait_internal_app_rej"] = "Y";
            }
            else
            {
                Session["p_wait_internal_app_rej"] = "";
            }
            if (chkCustomerApproval.Checked)
            {
                Session["p_wait_customer_app_rej"] = "Y";
            }
            else
            {
                Session["p_wait_customer_app_rej"] = "";
            }
            if (chkFinalApproval.Checked)
            {
                Session["p_wait_final_app_rej"] = "Y";
            }
            else
            {
                Session["p_wait_final_app_rej"] = "";
            }
            if (chkaddedtocollection.Checked)
            { 
                Session["p_add_to_collection"] = "Y";
            }
            else
            {
                Session["p_add_to_collection"] = "";            
            }
            if (chkNewYarn.Checked)
            {
                Session["p_new_yarn"] = "Y";
            }
            else
            {
                Session["p_new_yarn"] = "";
            }
            if (chkshortage.Checked)
            {
                Session["p_yarn_shortage"] = "Y";
            }
            else
            {
                Session["p_yarn_shortage"] = "";
            }
            if (chkdrwaitingso.Checked)
            {
                Session["p_no_order"] = "Y";
            }
            else
            {
                Session["p_no_order"] = "";
            }
            Session["internal_app_rej_id"] = ddlInternalAppRej.SelectedItem.Value.Trim();
            Session["customer_app_rej_id"] = ddlCustomerApprej.SelectedItem.Value.Trim();
            Session["final_app_rej_id"] = ddlFinalAppRej.SelectedItem.Value.Trim();
            Session["knitting_app_rej_id"] = ddlKnittingApprej.SelectedItem.Value.Trim();
            Session["internal_app_rej_Text"] = ddlInternalAppRej.SelectedItem.Text.Trim();
            Session["customer_app_rej_Text"] = ddlCustomerApprej.SelectedItem.Text.Trim();
            Session["final_app_rej_Text"] = ddlFinalAppRej.SelectedItem.Text.Trim();
            Session["knitting_app_rej_Text"] = ddlKnittingApprej.SelectedItem.Text.Trim();
            Response.Redirect("~/Reports/PrintTrackingList.aspx");
            //Server.Transfer("~/Reports/PrintTrackingList.aspx");
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtPDRNO.Text = "";
            txtCustomer.Text = "";
            ddlcategory.SelectedValue = "";
            ddlSubCategory.SelectedValue ="";
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
    }
}