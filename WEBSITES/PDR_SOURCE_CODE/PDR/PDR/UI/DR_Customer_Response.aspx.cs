using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PDR.BLL;
using System.Web.Services;
using System.IO;
namespace PDR.UI
{
    public partial class DR_Customer_Response : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                Response.Redirect("~/ui/login.aspx?pagename=DR_Customer_Response");
            }
            if (!IsPostBack)
            {
                populate_APPREJ();
                //txtApproveType.Text = Request.QueryString["Type"];
                classDR_Internal_Approve_Attachment_BLL obj = new classDR_Internal_Approve_Attachment_BLL();
                DataTable dt1 = obj.LoadDRAttachmentDetails(txtDRNo.Text.Trim(), "DR");
                grdAttachmentDetails.DataSource = dt1;
                grdAttachmentDetails.DataBind();
            }
        }

        private void populate_APPREJ()
        {
            DataSet dsAPPREJ;
            classDR_Customer_Response_BLL apprej = new classDR_Customer_Response_BLL();
            dsAPPREJ = apprej.Populate_APPREJ();
            ddlAppRej.DataSource = dsAPPREJ;
            ddlAppRej.DataTextField = "lookup_value";
            ddlAppRej.DataValueField = "lookup_value_id";
            ddlAppRej.DataBind();
            ddlAppRej.SelectedValue = "";
        }


        [WebMethod]
        public static DetailsClass_GetDRNoDetails[] GetDRNODetails(string DRNo)
        {
            List<DetailsClass_GetDRNoDetails> obj_GetDRNODetails = new List<DetailsClass_GetDRNoDetails>();
            classDR_Approval_BLL obj_DRNODetails_BLL = new classDR_Approval_BLL();
            DataTable obj_dt;
            obj_dt = obj_DRNODetails_BLL.GetDRNODetails(DRNo);
            foreach (DataRow dtRow in obj_dt.Rows)
            {
                DetailsClass_GetDRNoDetails DataObj = new DetailsClass_GetDRNoDetails();
                DataObj.dr_date = dtRow["dr_date"].ToString();
                DataObj.design_no = dtRow["design_no"].ToString();
                DataObj.customer_name = dtRow["customer_name"].ToString();
                DataObj.dr_customer_app_rej_comment = dtRow["dr_customer_app_rej_comment"].ToString();
                DataObj.dr_customer_app_rej_id = dtRow["dr_customer_app_rej_id"].ToString();
                obj_GetDRNODetails.Add(DataObj);
            }
            return obj_GetDRNODetails.ToArray();
        }

        public class DetailsClass_GetDRNoDetails
        {
            public string dr_date { get; set; }
            public string design_no { get; set; }
            public string customer_name { get; set; }
            public string dr_customer_app_rej_comment { get; set; }
            public string dr_customer_app_rej_id { get; set; }
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            classDR_Approval_BLL getDRNODetails = new classDR_Approval_BLL();
            DataTable dt_DRNODetails = getDRNODetails.GetDRNODetails(txtDRNo.Text.Trim());
            if (dt_DRNODetails.Rows.Count > 0)
            {
                foreach (DataRow resultrow in dt_DRNODetails.Rows)
                {
                    DRDate.Value = resultrow["dr_date"].ToString();
                    string drdate = resultrow["dr_date"].ToString().Substring(6) + "/" + resultrow["dr_date"].ToString().Substring(4, 2) + "/" + resultrow["dr_date"].ToString().Substring(0, 4);
                    DRDate.Value = drdate;
                    txtArticle.Text = resultrow["NEW_design_no"].ToString();
                    txtCustomer.Text = resultrow["customer_name"].ToString();
                    txtComment.Text = resultrow["dr_customer_app_rej_comment"].ToString();
                    hidapprejID.Value = resultrow["dr_customer_app_rej_id"].ToString();
                    txtrefno.Text= resultrow["dr_ref_no"].ToString();

                }
            }
            classDR_Internal_Approve_Attachment_BLL obj = new classDR_Internal_Approve_Attachment_BLL();
            DataTable dt1 = obj.LoadDRAttachmentDetails(txtDRNo.Text.Trim(), "DR");
            grdAttachmentDetails.DataSource = dt1;
            grdAttachmentDetails.DataBind();
        }

        protected void btnDone_Click(object sender, EventArgs e)
        {
            string p_user_session_id = (Session["usersessionid"].ToString());
          
            classDR_Customer_Response_BLL objDRNOUpdate = new classDR_Customer_Response_BLL();
            DataTable dt_DRNODetails = objDRNOUpdate.UpdateDRNODetails(txtDRNo.Text.Trim(), ddlAppRej.SelectedItem.Value.Trim(),
                                                                        txtComment.Text.Trim(), p_user_session_id);

            lblmsg.Visible = true;
        }

        protected void ImgAttachment_Click(object sender, ImageClickEventArgs e)
        {

            Response.Redirect("~/UI/DR_Internal_Approve_Attachment.aspx?SOURCEDOCNO=" + txtDRNo.Text.Trim() + "&PRODUCT=" + txtArticle.Text.Trim() + "&SOURCEDOCTYPE=DR");
        }

        protected void ImgEmal_Click(object sender, ImageClickEventArgs e)
        {
            //sendInternalApprovalMail();
            Response.Redirect("~/UI/SendMail_Customer_Response.aspx?drno=" + txtDRNo.Text.Trim() + "&PRODUCT=" + txtArticle.Text.Trim() + "&SOURCEDOCTYPE=DR");
        }

        private void sendInternalApprovalMail()
        {
            string filename = "";
            string sessionid = Session["usersessionid"].ToString();
            foreach (GridViewRow attdetailrow in grdAttachmentDetails.Rows)
            {
                string file_description;
                string doc_attachment_id;
                CheckBox chk = attdetailrow.FindControl("chkdelete") as CheckBox;
                if (chk.Checked)
                {
                    if (filename == "")
                    {
                        string locaiton = attdetailrow.Cells[2].Text;
                        filename = locaiton;
                    }
                    else
                    {
                        string locaiton = attdetailrow.Cells[2].Text;
                        filename = filename + "," + locaiton.Trim();
                    }
                }
                TextBox col1 = attdetailrow.FindControl("FILE_DESCRIPTION") as TextBox;
                file_description = Request[col1.UniqueID] as string;
            }
            DataTable dt;
            classDR_Customer_Response_BLL devReqBLL = new classDR_Customer_Response_BLL();
            string mailResult;
            classDR_Approval_BLL getDRNODetails = new classDR_Approval_BLL();
            DataTable dt_GetDRNODetails = getDRNODetails.GetDRNODetails(txtDRNo.Text.Trim());
            //mailResult = devReqBLL.sendCustomerApproveMail(dt_GetDRNODetails, filename, sessionid);
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


        protected void OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                GridViewRow rowSelect = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                int rowindex = rowSelect.RowIndex;

                string doc_attachment_id = grdAttachmentDetails.DataKeys[rowindex].Values[0].ToString();
                if (doc_attachment_id != "")
                {
                    Int32 deleteresult;
                    if (Convert.ToInt32(doc_attachment_id) > 0)
                    {
                        string p_source_doc_type = "DR";// Request.QueryString["SOURCEDOCTYPE"];
                        classDR_Internal_Approve_Attachment_BLL attachmentid = new classDR_Internal_Approve_Attachment_BLL();
                        deleteresult = attachmentid.Delete_DocAttachmentID(doc_attachment_id.ToString().Trim());
                        //  sourcedocnumber = Request.QueryString["SOURCEDOCNO"];
                        classDR_Internal_Approve_Attachment_BLL obj = new classDR_Internal_Approve_Attachment_BLL();
                        DataTable dt1 = obj.LoadDRAttachmentDetails(txtDRNo.Text.Trim(), p_source_doc_type);
                        grdAttachmentDetails.DataSource = dt1;
                        grdAttachmentDetails.DataBind();
                    }
                }
            }
            if (e.CommandName == "Download")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string filepath = grdAttachmentDetails.DataKeys[index].Values[2].ToString();// Server.MapPath("~/Media/photo/prlq/ESTIMATE/");// lblFileName.Text;

                if (filepath != "")
                {
                    //  Session["uploadedfilename"] = lblFileName.Text.Trim();
                    string extension;
                    string filename = Path.GetFileName(filepath);
                    // filepath = filepath + filename;
                    extension = Path.GetExtension(filepath);
                    Page.ClientScript.RegisterStartupScript(
                    this.GetType(), "OpenWindow", "window.open('" + filepath + "','_newtab');", true);
                    System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    byte[] ar = new byte[(int)fs.Length];
                    fs.Read(ar, 0, (int)fs.Length);
                    fs.Close();
                    Response.AddHeader("content-disposition", "attachment;filename=" + txtDRNo.Text.Trim() + filename);
                    Response.ContentType = "application/octectstream";
                    Response.BinaryWrite(ar);
                    Response.End();
                }
            }

        }



    }
}