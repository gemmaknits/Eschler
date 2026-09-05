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
        public class listAppl
        {
            public int lookup_value_id { get; set; }
            public string lookup_value { get; set; }
        }
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
                populate_ItemFamily();
                

                DateTime dt = DateTime.Now.AddDays(-90);
                DateTime dt1 = DateTime.Now;
                dtFromDate.Value = dt.ToString("dd/MM/yyyy").Replace('-', '/');// DateTime.Now.AddDays(-70).ToString();//.ToShortDateString();

                dtTo.Value = dt1.ToString("dd/MM/yyyy").Replace('-', '/');
            }


        }

        private void populate_ItemFamily()
        {
            DataTable dtItemFamily;
            classPDR_List_BLL ItemFamily= new classPDR_List_BLL();
            dtItemFamily = ItemFamily.populateItemFamily();
            ddlFamilyName.DataSource = dtItemFamily;
            ddlFamilyName.DataTextField = "lookup_value";
            ddlFamilyName.DataValueField = "lookup_value_id";
            ddlFamilyName.DataBind();
            ddlFamilyName.SelectedValue = "";
        }
        private void populate_Finishing()
        {
            DataTable dt;
            classPDR_List_BLL cls = new classPDR_List_BLL();
            dt = cls.populateFinishing();
            ddlFamilyName.DataSource = dt;
            ddlFamilyName.DataTextField = "id";
            ddlFamilyName.DataValueField = "name_en";
            ddlFamilyName.DataBind();
            ddlFamilyName.SelectedValue = "";
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
            
            List<listAppl> al = new List<listAppl>();
            al = (from DataRow dr in dsapplication.Tables[0].Rows
                  select new listAppl()
                           {
                               lookup_value_id = Convert.ToInt32(dr["lookup_value_id"]),
                               lookup_value = dr["lookup_value"].ToString(),
                           }).ToList();

            ddlAppl.DataSource = al;
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

        private void populate_PDRRequestor()
        {
            DataSet dspdrrequestor;
            classPDR_List_BLL pdrrequestor = new classPDR_List_BLL();
            dspdrrequestor = pdrrequestor.Populate_PDR_REQUESTORS();

        }


        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UI/develop_request.aspx");
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlcategory.SelectedValue = "";
            ddlSubCategory.SelectedValue = "";
            ddlGroup.SelectedValue = "";
            ddlSubGroup.SelectedValue = "";
            ddlFamilyName.SelectedIndex = -1;
            ddlSubApplication.SelectedIndex = -1;
            DateTime dt = DateTime.Now.AddDays(-90);
            DateTime dt1 = DateTime.Now;

            chkdrwaitingso.Checked = false;

        }

        protected void btnFind_Click(object sender, EventArgs e)
        {

            string DesignFromDate = "";
            string DesignToDate = "";
            string subcategoryid = "";
            string categoryid = "";
            string groupid = "";
            string subgroupid = "";
            string applicationid = "";
            string subapplicationid = "";
            string p_no_order = "";
            string compos = "";
            string DesignFamilyNameID;

            DesignFromDate = dtFromDate.Value;
            DesignToDate = dtTo.Value;
            categoryid = ddlcategory.SelectedValue.ToString();
            subcategoryid = ddlSubCategory.SelectedValue.ToString();
            groupid = ddlGroup.SelectedValue.ToString();
            subgroupid = ddlSubGroup.SelectedValue.ToString();
            applicationid = ddlFamilyName.SelectedValue.ToString();
            subapplicationid = ddlSubApplication.SelectedValue.ToString();
            compos = txtCompos.Text;
            DesignFamilyNameID = ddlFamilyName.SelectedValue.ToString();
            
            if (chkdrwaitingso.Checked)
            {
                p_no_order = "Y";
            }
            else
            {
                p_no_order = "N";
            }

            DataTable dtArticleList;
            classPDR_List_BLL pdrlist = new classPDR_List_BLL();
            dtArticleList = pdrlist.LoadArticleList(pDesignFromDate:DesignFromDate, pDesignToDate:DesignToDate,pCategoryID:categoryid,
                pSubCategoryID:subcategoryid,pGroupID:groupid,pSubGroupID:subgroupid,pApplicationID:applicationid, pSubApplicationID:subapplicationid,pWaitSO:p_no_order,pCompos:compos,pDesignFamilyNameID:DesignFamilyNameID);
            grdArticleList.DataSource = dtArticleList;
            grdArticleList.DataBind();
        }
    }
}