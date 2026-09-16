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
using Syncfusion.XlsIO;

namespace PDR.UI
{
    public partial class PDRArticleListMulti2 : System.Web.UI.Page
    {
        public class listAppl
        {
            public int lookup_value_id { get; set; }
            public string lookup_value { get; set; }
            public static DataTable dt;
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
                populate_SplFunc();
                populate_DesignType();
                populate_FamilyName();
//                populate_ItemType();
 //               populate_ItemSubType();
                populate_Finishing();
                //                populate_YarnDesc();
                populate_FibreType();
                populate_FibreSubType();
                populateXLSheetName();

                DateTime dt = DateTime.Now.AddDays(-7300);
                DateTime dt1 = DateTime.Now;

                dtFromDate.Value = dt.ToString("dd/MM/yyyy").Replace('-', '/');// DateTime.Now.AddDays(-70).ToString();//.ToShortDateString();

                dtTo.Value = dt1.ToString("dd/MM/yyyy").Replace('-', '/');
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Label tb; 
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == 0) { }

                //                    e.Row.Style.("height", "50px");
                /*
                tb = (TextBox)e.Row.FindControl("txtMinUSDKG");
                if (tb.Text == "0.00")
                    tb.Visible = false;
                else
                    tb.Visible = true;
                tb = (TextBox)e.Row.FindControl("txtMidUSDKG");
                if (tb.Text == "0.00")
                    tb.Visible = false;
                else
                    tb.Visible = true;
                tb = (TextBox)e.Row.FindControl("txtMaxUSDKG");
                if (tb.Text == "0.00")
                    tb.Visible = false;
                else
                    tb.Visible = true;

                tb = (TextBox)e.Row.FindControl("txtMinUSDKgWL");
                if (tb.Text == "0")
                    tb.Visible = false;
                else
                    tb.Visible = true;

                tb = (TextBox)e.Row.FindControl("txtMinUSDKgMD");
                if (tb.Text == "0")
                    tb.Visible = false;
                else
                    tb.Visible = true;

                tb = (TextBox)e.Row.FindControl("txtMidUSDKgWL");
                if (tb.Text == "0")
                    tb.Visible = false;
                else
                    tb.Visible = true;

                tb = (TextBox)e.Row.FindControl("txtMidUSDKgMD");
                if (tb.Text == "0")
                    tb.Visible = false;
                else
                    tb.Visible = true;
                tb = (TextBox)e.Row.FindControl("txtMaxUSDKgWL");
                if (tb.Text == "0")
                    tb.Visible = false;
                else
                    tb.Visible = true;

                tb = (TextBox)e.Row.FindControl("txtMaxUSDKgMD");
                if (tb.Text == "0")
                    tb.Visible = false;
                else
                    tb.Visible = true;
                    */

                //tb = (Label)e.Row.FindControl("txtMinUSDKG");
                //if (tb.Text == "0.00")
                //    tb.Visible = false;
                //else
                //    tb.Visible = true;
                //tb = (Label)e.Row.FindControl("txtMidUSDKG");
                //if (tb.Text == "0.00")
                //    tb.Visible = false;
                //else
                //    tb.Visible = true;
                //tb = (Label)e.Row.FindControl("txtMaxUSDKG");
                //if (tb.Text == "0.00")
                //    tb.Visible = false;
                //else
                //    tb.Visible = true;

                //tb = (Label)e.Row.FindControl("txtMinUSDKgWL");
                //if (tb.Text == "0")
                //    tb.Visible = false;
                //else
                //    tb.Visible = true;

                //tb = (Label)e.Row.FindControl("txtMinUSDKgMD");
                //if (tb.Text == "0")
                //    tb.Visible = false;
                //else
                //    tb.Visible = true;

                //tb = (Label)e.Row.FindControl("txtMidUSDKgWL");
                //if (tb.Text == "0")
                //    tb.Visible = false;
                //else
                //    tb.Visible = true;

                //tb = (Label)e.Row.FindControl("txtMidUSDKgMD");
                //if (tb.Text == "0")
                //    tb.Visible = false;
                //else
                //    tb.Visible = true;
                //tb = (Label)e.Row.FindControl("txtMaxUSDKgWL");
                //if (tb.Text == "0")
                //    tb.Visible = false;
                //else
                //    tb.Visible = true;

                //tb = (Label)e.Row.FindControl("txtMaxUSDKgMD");
                //if (tb.Text == "0")
                //    tb.Visible = false;
                //else
                //    tb.Visible = true;

            }
        }

        private void populateXLSheetName()
        {
            DataTable dt;
            classPDR_List_BLL sheet = new classPDR_List_BLL();
            dt = sheet.PopulateSheetName();
            ddlSheetName.DataSource = dt;
            ddlSheetName.DataTextField = "sheet_name";
            ddlSheetName.DataValueField = "sheet_code";
            ddlSheetName.DataBind();

        }

        private void populate_SplFunc()
        {
            DataTable dt;
            classPDR_List_BLL SplFunc= new classPDR_List_BLL();
            dt = SplFunc.populateSplFuncList();
            cblSplFunc.DataSource = dt;
            cblSplFunc.DataBind();

        }

        private void populate_DesignType()
        {
            DataTable dt;
            classPDR_List_BLL DesignType = new classPDR_List_BLL();
            dt = DesignType.populateDesignTypeList();
            cblDesignType.DataSource = dt;
            cblDesignType.DataBind();
        }

        private void populate_FamilyName()
        {
            DataTable dt;
            classPDR_List_BLL FamilyName = new classPDR_List_BLL();
            dt = FamilyName.populateItemFamilyList();
            cblFamilyName.DataSource = dt;
            cblFamilyName.DataBind();
        }



        /*        private void populate_ItemType()
                {
                    DataSet dsItemType;
                    classPDR_List_BLL ItemType = new classPDR_List_BLL();
                    dsItemType = ItemType.populateYarnType();
                    ddlYarnType.DataSource = dsItemType;
                    ddlYarnType.DataTextField = "ittypedesc";
                    ddlYarnType.DataValueField = "ittypeid";
                    ddlYarnType.DataBind();
                    ddlYarnType.SelectedValue = "";
                    ddlYarnType.DataSource = dsItemType.Tables[0];
                    ddlYarnType.DataBind();
                }
        */
        private void populate_Finishing()
        {
            DataTable dt;
            classPDR_List_BLL cls= new classPDR_List_BLL();
            dt = cls.populateFinishing();
            cblFinishing.DataSource = dt;
            cblFinishing.DataBind();
        }

        private void populate_YarnDesc()
        {
            DataTable dt;
            classPDR_List_BLL YarnDesc = new classPDR_List_BLL();
            dt = YarnDesc.populateYarnDescList();
//            cblYarn.DataSource = dt;
//            cblYarn.DataBind();
        }

        private void populate_FibreType()
        {
            DataTable dt;
            classPDR_List_BLL FibreType = new classPDR_List_BLL();
            dt = FibreType.populateFibreTypeList();
            cblFibreType.DataSource = dt;
            cblFibreType.DataBind();
        }

        private void populate_FibreSubType()
        {
            DataTable dt;
            classPDR_List_BLL FibreType = new classPDR_List_BLL();
            dt = FibreType.populateFibreSubTypeList();
            cblFibreSubType.DataSource = dt;
            cblFibreSubType.DataBind();
        }

        /*       private void populate_ItemSubType()
               {
                   DataSet dsItemSubType;
                   classPDR_List_BLL ItemSubType= new classPDR_List_BLL();
                   dsItemSubType = ItemSubType.populateYarnSubType();
                   ddlYarnSubType.DataSource = dsItemSubType;
                   ddlYarnSubType.DataTextField = "itsubdesc2";
                   ddlYarnSubType.DataValueField = "itsubid2";
                   ddlYarnSubType.DataBind();
                   ddlYarnSubType.SelectedValue = "";
                   ddlYarnSubType.DataSource = dsItemSubType.Tables[0];
                   ddlYarnSubType.DataBind();
               }
       */

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
            dsapplication = itemapplications.PopulateItemApplList();
            
            List<listAppl> al = new List<listAppl>();
           // al = (from DataRow dr in dsapplication.Tables[0].Rows
             //     select new listAppl()
               //            {
                 //              lookup_value_id = Convert.ToInt32(dr["lookup_value_id"]),
                   //            lookup_value = dr["lookup_value"].ToString(),
                     //      }).ToList();

            //            ddlAppl.DataSource = al;
            cblAppl.DataSource = dsapplication.Tables[0];
            cblAppl.DataBind();
        }

        private void populate_ItemSubApplication()
        {
            DataSet dssubapplication;
            classPDR_List_BLL itemsubapplications = new classPDR_List_BLL();
            dssubapplication = itemsubapplications.PopulateItemSubApplList();
            cblSubAppl.DataSource = dssubapplication.Tables[0];
            cblSubAppl.DataBind();

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

        protected void btnClearAppl_Click(object sender, EventArgs e)
        {
            clearCheckBoxList(cblAppl);
        }

        protected void btnClearSubAppl_Click(object sender, EventArgs e)
        {
            clearCheckBoxList(cblSubAppl);
        }

        protected void btnClearSplFunc_Click(object sender, EventArgs e)
        {
            clearCheckBoxList(cblSplFunc);
        }

        protected void btnClearDesignType_Click(object sender, EventArgs e)
        {
            clearCheckBoxList(cblDesignType);
        }

        protected void btnClearFamily_Click(object sender, EventArgs e)
        {
            clearCheckBoxList(cblFamilyName);
        }

        protected void btnClearFinishing_Click(object sender, EventArgs e)
        {
            clearCheckBoxList(cblFinishing);
        }


        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlcategory.SelectedValue = "";
            ddlSubCategory.SelectedValue = "";
            ddlGroup.SelectedValue = "";
            ddlSubGroup.SelectedValue = "";
//            ddlYarnType.SelectedIndex = -1;
//            ddlYarnSubType.SelectedIndex = -1;
            txtCompos.Text = "";
            txtWeightFrom.Text = "";
            txtWeightTo.Text = "";
            txtWidthFrom.Text = "";
            txtRecordCount.Text = "";
            txtArticle.Text = "";
            DateTime dt = DateTime.Now.AddDays(-90);
            DateTime dt1 = DateTime.Now;

            //chkdrwaitingso.Checked = false;
            clearCheckBoxList(cblAppl);
            clearCheckBoxList(cblSubAppl);
            clearCheckBoxList(cblSplFunc);
            clearCheckBoxList(cblDesignType);
            clearCheckBoxList(cblFamilyName);
            clearCheckBoxList(cblFinishing);

        }

        private void clearCheckBoxList(CheckBoxList cbl)
        {
            foreach (ListItem item in cbl.Items)
            {
                if (item.Selected)
                {
                    item.Selected = false;
                }
            }
        }

        private String buildApplIDList(CheckBoxList cbl)
        {
            string IDList;
            IDList = "";
            foreach (ListItem item in cbl.Items)
            {
                if (item.Selected)
                {
                    IDList = IDList + item.Value.ToString() + ",";
                }
            }
            if (IDList == "") { IDList = null; return IDList; }

            if (IDList.Substring(IDList.Length-1)==",") {
                IDList = IDList.Substring(0, IDList.Length - 1);
            }
                return IDList;
        }

        private String buildKindOfYarnList(CheckBoxList cbl)
        {
            string IDList;
            IDList = "";
            foreach (ListItem item in cbl.Items)
            {
                if (item.Selected)
                {
                    IDList = IDList + item.Value.ToString() + ",";
                }
            }
            if (IDList == "") { IDList = null; return IDList; }

            if (IDList.Substring(IDList.Length - 1) == ",")
            {
                IDList = IDList.Substring(0, IDList.Length - 1);
            }
            return IDList;
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {

            string DesignFromDate = "";
            string DesignToDate = "";
            string subcategoryid = "";
            string categoryid = "";
            string groupid = "";
            string subgroupid = "";
            string p_no_order = "";
            string compos = "";
            string ArticleName = "";
            string ItemTypeID = "";
            string ItemSubTypeID="";
            string ApplIDList = "";
            string SubApplIDList = "";
            string SplFuncIDList = "";
            string DesignTypeIDList = "";
            string FamilyIDList = "";
            string KindOfYarnList = "";
            string WidthFrom = "";
            string WidthTo = "";
            string WeightFrom = "";
            string WeightTo = "";
            string FinishIDList = "";
            string gauge = "";
            string XLSheetName = "";


            ApplIDList=buildApplIDList(cblAppl);
            SubApplIDList= buildApplIDList(cblSubAppl);
            SplFuncIDList= buildApplIDList(cblSplFunc);
            DesignTypeIDList= buildApplIDList(cblDesignType);
            FamilyIDList= buildApplIDList(cblFamilyName);
            FinishIDList= buildApplIDList(cblFinishing);
            KindOfYarnList = buildKindOfYarnList(cblFibreSubType);
                       
            compos = txtCompos.Text;
            ArticleName = txtArticle.Text;
            DesignFromDate = dtFromDate.Value;
            DesignToDate = dtTo.Value;
            WeightFrom = txtWeightFrom.Text;
            WeightTo = txtWeightTo.Text;
            WidthFrom = txtWidthFrom.Text;
            WidthTo = txtWidthTo.Text;
            categoryid = ddlcategory.SelectedValue.ToString();
            subcategoryid = ddlSubCategory.SelectedValue.ToString();
            groupid = ddlGroup.SelectedValue.ToString();
            subgroupid = ddlSubGroup.SelectedValue.ToString();
//            ItemTypeID = ddlYarnType.SelectedValue.ToString();
            
//            ItemSubTypeID = ddlYarnSubType.SelectedValue.ToString();

            gauge = txtGauge.Text.ToString();

            p_no_order = "N";
            /*            if (chkdrwaitingso.Checked)
                        {
                            p_no_order = "Y";
                        }
                        else
                        {
                            p_no_order = "N";
                        }
            */
                        XLSheetName = ddlSheetName.SelectedValue.ToString();

            

            DataTable dtArticleList;
            classPDR_List_BLL pdrlist = new classPDR_List_BLL();
            dtArticleList = pdrlist.LoadArticleListMulti(pCompos: compos, pArticleName: ArticleName,pWidthFrom:WidthFrom, pWidthTo:WidthTo,pWeightFrom:WeightFrom,pWeightTo:WeightTo, pDesignFromDate:DesignFromDate, pDesignToDate:DesignToDate,
                pCategoryID:categoryid,pSubCategoryID:subcategoryid,pGroupID:groupid,pSubGroupID:subgroupid,pItemTypeId:ItemTypeID,pItemSubTypeId:ItemSubTypeID,
                pApplList:ApplIDList,pSubApplList:SubApplIDList,pWaitSO:p_no_order, pSplFuncList:SplFuncIDList, pDesignTypeList: DesignTypeIDList,pFamilyList:FamilyIDList,pFinishList:FinishIDList,pGauge:gauge,pXLSheetName:XLSheetName, pKindOfYarnList:KindOfYarnList );
            int FoundCount = 0;
            listAppl.dt = dtArticleList;
            FoundCount = dtArticleList.Rows.Count;
            txtRecordCount.Text = FoundCount.ToString();
            grvArticleList.DataSource = dtArticleList;
            grvArticleList.DataBind();

        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            exportToExcel();
        }
        protected void exportToExcel()
        {
            //Create an instance of ExcelEngine
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                //Initialize application object
                IApplication application = excelEngine.Excel;

                //Set the default application version as Excel 2016
                application.DefaultVersion = ExcelVersion.Excel2016;

                //Create a new workbook
                IWorkbook workbook = application.Workbooks.Create(1);

                //Access first worksheet from the workbook instance
                IWorksheet worksheet = workbook.Worksheets[0];

                //Exporting DataTable to worksheet
                DataTable dataTable = listAppl.dt;
                worksheet.ImportDataTable(dataTable, true, 1, 1);
                worksheet.UsedRange.AutofitColumns();

                //SaveFileDialog savedialog = new SaveFileDialog();
                //savedialog.AddExtension = true;
                //savedialog.FileName = "Sample";
                //savedialog.DefaultExt = "xlsx";
                //savedialog.Filter = @"Excel file (.xlsx)|*.xlsx";

                //if (savedialog.ShowDialog() == DialogResult.OK)
                //{
                    //Save the workbook to disk in xlsx format
                    workbook.SaveAs("D://DOTNET//DEMO//output.xlsx");
                string filename;
                string filepath;

                filename = "D://DOTNET//DEMO//output.xlsx";
                filepath = filename;
                System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                Response.ClearContent();
                Response.AddHeader("Content-Disposition", "attachment; filename=" +  filename);
                byte[] ar = new byte[(int)fs.Length];
                fs.Read(ar, 0, (int)fs.Length);
                fs.Close();
                // Response.AddHeader("content-disposition", "attachment;filename=" +  filename);
                Response.ContentType = "application/octectstream";
                Response.BinaryWrite(ar);
                Response.End();

                //}
            }
        }
    }
}