using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PDR.BLL;
using System.Data;
using System.Drawing;
using System.Web.Services;
using System.IO;
namespace PDR.UI
{
    public partial class DR_Sample_Dispo : System.Web.UI.Page
    {
        DataSet dsAPPREJ;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                Response.Redirect("~/ui/login.aspx?pagename=DR_Sample_Dispo");
            }
            if (!IsPostBack)
            {
                populate_APPREJ();
                hidpageloadindex.Value = "0";
                BindDummyGridrow_SampleDispo();
            }
            else
            {
                hidpageloadindex.Value = "";
                //BindDummyGridrow_YarnStock();
            }
        }

        private void populate_APPREJ()
        {

            classDR_Approval_BLL apprej = new classDR_Approval_BLL();
            dsAPPREJ = apprej.Populate_APPREJ();
            ViewState["apprej"] = dsAPPREJ;
            //ddlAppRej.DataSource = dsAPPREJ;
            //ddlAppRej.DataTextField = "lookup_value";
            //ddlAppRej.DataValueField = "lookup_value_id";
            //ddlAppRej.DataBind();
            //ddlAppRej.SelectedValue = "";
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            classKnitting_Approval_BLL getDRNODetails = new classKnitting_Approval_BLL();
            DataTable dt_DRNODetails = getDRNODetails.GetDRKnittingDetails(txtDesignNo.Text.Trim());
            if (dt_DRNODetails.Rows.Count > 0)
            {
                foreach (DataRow resultrow in dt_DRNODetails.Rows)
                {
                    DRDate.Value = resultrow["dr_date"].ToString();
                    string drdate = resultrow["dr_date"].ToString().Substring(6) + "/" + resultrow["dr_date"].ToString().Substring(4, 2) + "/" + resultrow["dr_date"].ToString().Substring(0, 4);
                    DRDate.Value = drdate;
                    txtDRNo.Text = resultrow["dr_no"].ToString();
                    txtDesignNo.Text = resultrow["design_no"].ToString();
                    txtCustomer.Text = resultrow["customer_name"].ToString();
                    //txtComment.Text = resultrow["dr_knitting_app_rej_comment"].ToString();
                    hidapprejID.Value = resultrow["dr_knitting_app_rej_id"].ToString();
                    MyLnkButton.Text = resultrow["pdr_no"].ToString();
                    kikono.Text = resultrow["kono"].ToString();
                    hidkono.Value = resultrow["kono"].ToString();
                    TXTKNITTINGREMARK.Text= resultrow["knitting_remark"].ToString();

                }
            }
            classDR_Sample_Dispo obj = new classDR_Sample_Dispo();
            string p_user_session_id = (Session["usersessionid"].ToString());
            DataTable dt1 = obj.GetDRKnittingDetails(kikono.Text.ToString().Trim(), p_user_session_id);
            grdDRSampleKnitting.DataSource = dt1;
            grdDRSampleKnitting.DataBind();
        }


        protected void DetailsPDRINPDF_click(object sender, EventArgs e)
        {
            Session["pdrno"] = MyLnkButton.Text.Trim();
            Session["pdr_new_develop_req_id"] = hidpdr_new_develop_req_id.Value.ToString().Trim();
            Session["rptoption"] = "print";
            Server.Execute("~/Reports/Print_Develop_Request.aspx");
        }

        protected void kikono_Click(object sender, EventArgs e)
        {
            string kono = kikono.Text.Trim();
            Response.Redirect("~/UI/yarnlots.aspx?kikono=" + kono.Trim() + "&designno=" + txtDesignNo.Text.Trim());
        }
        protected void gvOrderIssues_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdDRSampleKnitting, "Select$" + e.Row.RowIndex);
                e.Row.ToolTip = "Click to select this row.";
                e.Row.Attributes.Add("onclick", string.Format("ChangeRowColor('{0}','{1}');", e.Row.ClientID, e.Row.RowIndex, grdDRSampleKnitting.DataKeys[e.Row.RowIndex].Value.ToString()));
            }
        }
        protected void gvorder_issues_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onclick", string.Format("ChangeRowColor('{0}','{1}');", e.Row.ClientID, e.Row.RowIndex, grdDRSampleKnitting.DataKeys[e.Row.RowIndex].Value.ToString()));
            }
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    e.Row.Attributes.Add("onclick", string.Format("ChangeRowColor('{0}','{1}');", e.Row.ClientID, e.Row.RowIndex, grdDRSampleKnitting.DataKeys[e.Row.RowIndex].Value.ToString()));
            //   // e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdDRSampleKnitting, "Select$" + e.Row.RowIndex);
            //    e.Row.ToolTip = "Click to select this row.";
            //    // string colochex = grdDRSampleKnitting.DataKeys[e.Row.RowIndex].Values[5].ToString().Trim();
            //}
        }
        protected void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            string so_lines_ext_id = null;
            int index = grdDRSampleKnitting.SelectedRow.RowIndex;


            //Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "selectedIndexValue();", true);
            foreach (GridViewRow row in grdDRSampleKnitting.Rows)
            {
                if (grdDRSampleKnitting.SelectedIndex == -1)
                {
                    grdDRSampleKnitting.SelectedIndex = 0;
                }

                if (row.RowType == DataControlRowType.DataRow)
                {
                    if (row.RowIndex == grdDRSampleKnitting.SelectedRow.RowIndex)// gvOrderIssues.SelectedIndex)
                    {
                        row.BackColor = ColorTranslator.FromHtml("#FFF3B7");
                        classDR_Internal_Approve_Attachment_BLL obj = new classDR_Internal_Approve_Attachment_BLL();
                        string source_doc_no = grdDRSampleKnitting.DataKeys[row.RowIndex].Values[0].ToString().Trim();
                        hidsource_doc_no.Value = source_doc_no.Trim();
                        hidrowindex.Value = row.RowIndex.ToString().Trim();
                        //txtComment.Text= grdDRSampleKnitting.DataKeys[row.RowIndex].Values[1].ToString().Trim();
                        if (Convert.ToInt64(source_doc_no) != -1)
                        {
                            DataTable dt1 = obj.LoadDRAttachmentDetails(source_doc_no, "SAMPLE_DISPO");
                            grdAttachmentDetails.DataSource = dt1;
                            grdAttachmentDetails.DataBind();
                            row.BackColor = ColorTranslator.FromHtml("#FFF3B7");
                            grdDRSampleKnitting.SelectedRow.BackColor = ColorTranslator.FromHtml("#FFF3B7");
                        }
                        else
                        {
                            row.BackColor = ColorTranslator.FromHtml("#FFF3B7");
                            grdDRSampleKnitting.SelectedRow.BackColor = ColorTranslator.FromHtml("#FFF3B7");
                            DataTable dt1 = null;
                            grdAttachmentDetails.DataSource = dt1;
                            grdAttachmentDetails.DataBind();
                        }
                    }
                    else
                    {
                        row.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                    }
                }
            }
        }
        protected void btnDone_Click(object sender, EventArgs e)
        {
            string p_user_session_id = (Session["usersessionid"].ToString());

            classDR_Sample_Dispo objSamKnitting = new classDR_Sample_Dispo();


            DataTable dtsave = new DataTable();
            dtsave.Columns.AddRange(new DataColumn[7] { new DataColumn("p_mfg_sample_dispo_id", typeof(Int64)),
                new DataColumn("p_dispo_seq", typeof(int)),
                new DataColumn("p_kono",typeof(string)),
                new DataColumn("p_primary_quantity",typeof(decimal)),
                new DataColumn("p_secondary_quantity",typeof(string)),
                new DataColumn("p_sample_dispo_comment",typeof(string)),
                 new DataColumn("p_user_session_id",typeof(Int64))
                 });
            Int64 p_mfg_sample_dispo_id = -1;
            Int64 p_dispo_seq = 0;
            string p_kono = kikono.Text.Trim();
            double p_primary_quantity;
            string p_secondary_quantity;
            string p_sample_dispo_comment = "";
            DataTable dtretunvalue;
            foreach (GridViewRow samknittingrow in grdDRSampleKnitting.Rows)
            {
                if (samknittingrow.RowType == DataControlRowType.DataRow)
                {
                    p_mfg_sample_dispo_id = Convert.ToInt64(grdDRSampleKnitting.DataKeys[samknittingrow.RowIndex].Values[0].ToString().Trim());
                    if (samknittingrow.Cells[1].Text.ToString() != "" && samknittingrow.Cells[1].Text.ToString().Trim() != "&nbsp;")
                    {
                        p_dispo_seq = Convert.ToInt64(samknittingrow.Cells[1].Text.ToString());
                    }
                    else
                    {
                        p_dispo_seq = 0;
                    }
                    p_kono = kikono.Text.Trim();
                    TextBox sampledispocomment = grdDRSampleKnitting.Rows[samknittingrow.RowIndex].FindControl("SAMPLEDISPOCOMMENT") as TextBox;
                    p_sample_dispo_comment = sampledispocomment.Text;
                }

                TextBox a = samknittingrow.FindControl("QTYMTS") as TextBox;
                p_secondary_quantity = Request[a.UniqueID] as string;
                dtsave.Rows.Add(p_mfg_sample_dispo_id, p_dispo_seq, p_kono, 0, p_secondary_quantity, p_sample_dispo_comment, p_user_session_id);
            }

            dtretunvalue = objSamKnitting.UpdateKnittingSampleDetails(dtsave);
            classDR_Sample_Dispo obj = new classDR_Sample_Dispo();
            DataTable dt1 = obj.GetDRKnittingDetails(kikono.Text.ToString().Trim(), p_user_session_id);
            grdDRSampleKnitting.DataSource = dt1;
            grdDRSampleKnitting.DataBind();

            lblmsg.Visible = true;
        }

        protected void ImgAttachment_Click(object sender, ImageClickEventArgs e)
        {
            string sourcedocno;
            foreach (GridViewRow row in grdDRSampleKnitting.Rows)
            {
                if (grdDRSampleKnitting.SelectedIndex == -1)
                {
                    grdDRSampleKnitting.SelectedIndex = 0;
                }
                if (row.RowIndex == grdDRSampleKnitting.SelectedRow.RowIndex)// gvOrderIssues.SelectedIndex)
                {
                    //hidrowindex.Value = row.RowIndex.ToString();
                    hidrowindex.Value = (grdDRSampleKnitting.DataKeys[row.RowIndex].Values[0].ToString().Trim());
                }
            }
            if (hidsource_doc_no.Value.Trim() != "-1")
            {
                Response.Redirect("~/UI/DR_Internal_Approve_Attachment.aspx?SOURCEDOCNO=" + hidsource_doc_no.Value.Trim() + "&SOURCEDOCTYPE=SAMPLE_DISPO");
            }
        }

        protected void ImgEmal_Click(object sender, ImageClickEventArgs e)
        {
            //sendInternalApprovalMail();
          
            Response.Redirect("~/UI/SendMail_Sample_Dispo.aspx?SOURCEDOCNO=" + hidsource_doc_no.Value.Trim() + "&SOURCEDOCTYPE=KI" + "&designo=" + txtDesignNo.Text.Trim() + "&seq=" + hidseq.Value.Trim() + "&comment=" + hidcomment.Value.Trim() );
        }
        private void sendInternalApprovalMail()
        {
            string filename = "";
            string sessionid = Session["usersessionid"].ToString();
            foreach (GridViewRow attdetailrow in grdAttachmentDetails.Rows)
            {
                string file_description;
                string doc_attachment_id;
                //CheckBox chk = attdetailrow.FindControl("chkcheck") as CheckBox;
                
                //if (chk.Checked)
                //{
                //    if (filename == "")
                //    {
                //        string locaiton = attdetailrow.Cells[2].Text;
                //        filename = locaiton;
                //    }
                //    else
                //    {
                //        string locaiton = attdetailrow.Cells[2].Text;
                //        filename = filename + "," + locaiton.Trim();
                //    }
                //}
                //TextBox col1 = attdetailrow.FindControl("FILE_DESCRIPTION") as TextBox;
                //file_description = Request[col1.UniqueID] as string;
            }
            DataTable dt;
            classDR_Sample_Dispo sampleknitting = new classDR_Sample_Dispo();
            string mailResult;
            classKnitting_Approval_BLL getDRNODetails = new classKnitting_Approval_BLL();
            DataTable dt_GetDRNODetails = getDRNODetails.GetDRKnittingDetails(txtDesignNo.Text.Trim());
            //mailResult = sampleknitting.sendSampleKnittingMail(dt_GetDRNODetails, filename, sessionid);
            //if (mailResult == "S")
            //{
            //    lblmsg.Visible = true;
            //    lblmsg.Text = "Mail Send";
            //}
            //else
            //{
            //    lblmsg.Visible = true;
            //    lblmsg.Text = "Mail Not Send";
            //}
        }
        public void BindDummyGridrow_SampleDispo()
        {
            DataTable dt = new DataTable();
           // dt.Columns.Add("");
            dt.Columns.Add("FILE_DESCRIPTION");
            dt.Columns.Add("FILE_LOCATION_disp");
            dt.Columns.Add("");
            dt.Rows.Add();
            grdAttachmentDetails.DataSource = dt;
            grdAttachmentDetails.DataBind();
            grdAttachmentDetails.Rows[0].Visible = false;
        }



        [WebMethod]
        public static DetailsClass_ST[] GetData(string source_doc_no)

        {
            List<DetailsClass_ST> Detail_ST_Production = new List<DetailsClass_ST>();
            classDR_Internal_Approve_Attachment_BLL obj = new classDR_Internal_Approve_Attachment_BLL();
            DataTable dt;
            //Check_Availability_BLL knitting_Obj = new Check_Availability_BLL();
            dt = obj.LoadDRAttachmentDetails(source_doc_no, "SAMPLE_DISPO");
            foreach (DataRow dtRow in dt.Rows)
            {
                DetailsClass_ST DataObj = new DetailsClass_ST();
                DataObj.file_description = dtRow["file_description"].ToString();
                DataObj.file_location_disp = dtRow["file_location_disp"].ToString();
                DataObj.file_location = dtRow["file_location"].ToString();
                DataObj.doc_attachments_id = dtRow["doc_attachments_id"].ToString();
                Detail_ST_Production.Add(DataObj);
            }
            return Detail_ST_Production.ToArray();
        }
        public class DetailsClass_ST
        {
            public string file_description { get; set; }
            public string file_location_disp { get; set; }
            public string file_location { get; set; }
            public string doc_attachments_id { get; set; }
        }


        [WebMethod]
        public static string downloadAttachment(string file_location,string sourcedocno)
        {
            string filepath = file_location;//

            if (filepath != "")
            {
                string filename = Path.GetFileName(filepath);
                string FilePath = file_location;
                //// filepath = filepath + filename;
                string extension;
                extension = Path.GetExtension(filepath);
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenWindow", "window.open('" + filepath + "','_newtab');", true);
                //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //byte[] ar = new byte[(int)fs.Length];
                //fs.Read(ar, 0, (int)fs.Length);
                //fs.Close();
                //Response.AddHeader("content-disposition", "attachment;filename=" + sourcedocno.Trim() + filename);
                //Response.ContentType = "application/octectstream";
                //Response.BinaryWrite(ar);
                //Response.End();



                //HttpContext.Current.Response.ContentType = "APPLICATION/OCTET-STREAM";
                //String Header = "Attachment; Filename=" + sfilename;
                //HttpContext.Current.Response.AppendHeader("Content-Disposition", Header);
                //System.IO.FileInfo Dfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(sFilePath));
                //HttpContext.Current.Response.WriteFile(Dfile.FullName);
                //HttpContext.Current.Response.End();

                String result = "Result : " + DateTime.Now.ToString() + " - From Server";

                // System.IO.FileInfo file = new System.IO.FileInfo(System.Web.HttpContext.Current.Server.MapPath(filepath));// System.Configuration.ConfigurationManager.AppSettings["FolderPath"].ToString()) + "\\" + "Default.aspx");
                System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                Response.ClearContent();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + sourcedocno.Trim() + filename);
                byte[] ar = new byte[(int)fs.Length];
                fs.Read(ar, 0, (int)fs.Length);
                fs.Close();
                //Response.AddHeader("content-disposition", "attachment;filename=" + sourcedocno.Trim() + filename);
                Response.ContentType = "application/octectstream";
                Response.BinaryWrite(ar);
                Response.End();

                //extension = Path.GetExtension(filepath);
                //Page.ClientScript.RegisterStartupScript(
                //this.GetType(), "OpenWindow", "window.open('" + filepath + "','_newtab');", true);
                //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //byte[] ar = new byte[(int)fs.Length];
                //fs.Read(ar, 0, (int)fs.Length);
                //fs.Close();
                //Response.AddHeader("content-disposition", "attachment;filename=" + txtSourceDocNo.Text.Trim() + filename);
                //Response.ContentType = "application/octectstream";
                //Response.BinaryWrite(ar);
                //Response.End();


                return "";

            }
            return "";
            //string SQL =
            //      "select top 1 FILENAME, FILE_MIME_TYPE, ATTACHMENT" + ControlChars.CrLf
            //    + "  FROM vwATTACHMENTS_CONTENT" + ControlChars.CrLf
            //    + " where ID = @ATTACHMENT_ID" + ControlChars.CrLf
            //    + " order by DATE_ENTERED desc";
            ////Debug.Print(SQL);

            //DbProviderFactory dbf = DbProviderFactories.GetFactory();
            //using (IDbConnection con = dbf.CreateConnection())
            //using (IDbCommand cmd = con.CreateCommand())
            //{
            //    cmd.CommandText = SQL;
            //    Sql.AppendParameter(cmd, id, "ATTACHMENT_ID");

            //    using (IDataReader rdr = cmd.ExecuteReader())
            //    {
            //        if (rdr.Read())
            //        {
            //            // Send the file to the browser
            //            Response.AddHeader("Content-type", r["FILE_MIME_TYPE"].ToString());
            //            Response.AddHeader("Content-Disposition", "attachment; filename=" + r["FILENAME"].ToString());

            //            Response.BinaryWrite((Byte[])r["ATTACHMENT"]);

            //            Response.Flush();
            //            Context.ApplicationInstance.CompleteRequest();
            //        }
            //    }
            //}
        }



        [WebMethod]
        public static string GetData2(string source_doc_no)

        {
            //List<DetailsClass_YarnStock> Detail_YarnStock = new List<DetailsClass_YarnStock>();

            classDR_Internal_Approve_Attachment_BLL obj = new classDR_Internal_Approve_Attachment_BLL();
            DataSet yarn_dt;

            //string data_name = "YARN";
            //// yarn_dt = yarn_Obj.Load_ChkAvailability_GreigeStock(finisheddesignno, data_name);
            //string factorydesignno = null;
            //string greigestockgrade = null;
            //string dyedstockgrade = null;
            //string colorcode = null;
            //string finished_designno = finisheddesignno;
            //string required_qty = requiredqty;
            //string req_unit = requnit;
            yarn_dt = obj.LoadDRAttachmentDetails_DS(source_doc_no, "SAMPLE_DISPO");
            return yarn_dt.GetXml();


        }


        [WebMethod]
        public static string[] GetData1(string source_doc_no)
        {
            DataTable dt = new DataTable();
            // List<Issues> objDept = new List<Issues>();
            List<string> customers = new List<string>();
            classDR_Internal_Approve_Attachment_BLL obj = new classDR_Internal_Approve_Attachment_BLL();
            dt = obj.LoadDRAttachmentDetails(source_doc_no, "SAMPLE_DISPO");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //string chk = dt.Rows[i]["mfg_sample_dispo_id"].ToString();
                    //if (chk == "-1")
                    //{
                    //    // customers.Add(string.Format("{0}- ", Convert.ToInt32(dt.Rows[i]["poc_issue_master_id"]), "' '"));
                    //    customers.Add(string.Format("'--1',{1} ", (dt.Rows[i]["FILE_DESCRIPTION"]), "' '"));
                    //}
                    //else
                    //{
                        customers.Add(string.Format("{0}-{1}", (dt.Rows[i]["FILE_DESCRIPTION"]), dt.Rows[i]["FILE_LOCATION_disp"].ToString()));
                   // }

                }
                return customers.ToArray();
            }
            else
            {
                return null;
            }

        }



    }
}