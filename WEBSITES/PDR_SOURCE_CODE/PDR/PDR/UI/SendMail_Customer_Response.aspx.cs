using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using PDR.BLL;
using System.IO;
namespace PDR.UI
{
    public partial class SendMail_Customer_Response : System.Web.UI.Page
    {
        string drno;
        string TOMailList;
        string filename;
        string mailResult;
        string sessionID;
        string ccMailList;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                drno = Request.QueryString["drno"];
                Response.Redirect("~/ui/login.aspx?pagename=SendMail_Customer_Response" + "&drno=" + drno.Trim());
            }
            if (!IsPostBack)
            {
                sessionID = Session["usersessionid"].ToString();
                StringBuilder strBodyHTML = new StringBuilder();
                drno = Request.QueryString["drno"];
                filename = Request.QueryString["attachmentfilename"];
                //txtTo.Text = Session["factory_manager_email"].ToString();
                //txtSubject.Text = "PDR No." + pdrno + " have shortage of yarn";
                txtBody.Text = "";
                classDR_Approval_BLL getDRNODetails = new classDR_Approval_BLL();
                DataTable dt = getDRNODetails.GetDRNODetails(drno.Trim());
                TOMailList = dt.Rows[0]["rd_staff_email"].ToString();//
                txtTo.Text = TOMailList;
                if  (dt.Rows[0]["sample_room_email"].ToString() !="")
                {
                    ccMailList = dt.Rows[0]["sample_room_email"].ToString();
                }
                if (dt.Rows[0]["requester_email"].ToString() != "")
                {
                    ccMailList = ccMailList + "," +  dt.Rows[0]["requester_email"].ToString();
                }
                if (dt.Rows[0]["requester_email"].ToString().Trim().ToUpper() != dt.Rows[0]["PREPARER_email"].ToString().Trim().ToUpper())
                {
                    if (dt.Rows[0]["PREPARER_email"].ToString() != "")
                    {
                        ccMailList = ccMailList + "," + dt.Rows[0]["PREPARER_email"].ToString();
                    }
                }
                if (dt.Rows[0]["factory_manager_email"].ToString() != "")
                {
                    ccMailList = ccMailList + "," + dt.Rows[0]["factory_manager_email"].ToString();
                }
                if (dt.Rows[0]["md_email"].ToString() != "")
                {
                    ccMailList = ccMailList + "," + dt.Rows[0]["md_email"].ToString();
                }

                txtCC.Text = ccMailList;
                txtSubject.Text = dt.Rows[0]["CUSTOMER_RESPONSE_subject"].ToString();// "INTERNAL APPROVAL SUBJECT";
                strBodyHTML.Append("<HTML>");

                string sent = "\r\n" + "\r\n";// + "\r\n" + "\r\n" + "\r\n" + "\r\n";
                sent += "DR NO:  " + dt.Rows[0]["DR_NO"].ToString().Trim() + "  " + dt.Rows[0]["CUSTOMER_NAME"].ToString().Trim() + "\r\n";
                sent += "DESIGN NO:  " + dt.Rows[0]["NEW_DESIGN_NO"].ToString().Trim() + "\r\n";
                sent += "CUSTOMER APP/REJ:  " + dt.Rows[0]["DR_CUSTOMER_aPP_REJ_CODE"].ToString().Trim() + "\r\n";
                sent += "ADD TO COLLECTION:  " + dt.Rows[0]["ADD_tO_cOLLECTION"].ToString().Trim() + "\r\n";
              //  sent += "CUSTOMER COMMENT:  " + dt.Rows[0]["DR_CUSTOMER_aPP_REJ_COMMENT"].ToString().Trim() + "\r\n";
                if (dt.Rows[0]["DR_CUSTOMER_aPP_REJ_COMMENT"].ToString().Trim() != "")
                {
                    sent += "CUSTOMER COMMENT:" + dt.Rows[0]["DR_CUSTOMER_aPP_REJ_COMMENT"].ToString().Trim() + "\r\n";
                }
                else
                {
                    sent += "CUSTOMER COMMENT:\r\n";
                }
                txtBody.Text = sent;

                classDR_Internal_Approve_Attachment_BLL obj = new classDR_Internal_Approve_Attachment_BLL();
                DataTable dt1 = obj.LoadDRAttachmentDetails(drno.Trim(), "DR");
                grdAttachmentDetails.DataSource = dt1;
                grdAttachmentDetails.DataBind();

            }
        }

        protected void btnSendMail_Click(object sender, EventArgs e)
        {
            sendCustomerApprovalMail();
        }

        private void sendCustomerApprovalMail()
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
               // TextBox col1 = attdetailrow.FindControl("FILE_DESCRIPTION") as TextBox;
               // file_description = Request[col1.UniqueID] as string;
            }
            drno = Request.QueryString["drno"];
            DataTable dt;
            classDR_Approval_BLL getDRNODetails = new classDR_Approval_BLL();
            DataTable dt_DRNODetails = getDRNODetails.GetDRNODetails(drno.Trim());
            string mailResult;
            classDR_Customer_Response_BLL sendmail = new classDR_Customer_Response_BLL();
            drno = Request.QueryString["drno"];
            //DataTable dt_GetDRNODetails = getDRNODetails1.GetDRNODetails(drno.Trim());
            //mailResult = getDRNODetails.sendApprovalMail(dt_GetDRNODetails, filename, sessionid);
            mailResult = sendmail.sendCustomerApproveMail(dt_DRNODetails, filename, sessionID, txtTo.Text.Trim(), txtCC.Text.Trim(), txtSubject.Text.Trim(), txtBody.Text.Trim());
            if (mailResult == "S")
            {
                lblmsg.Visible = true;
                lblmsg.Text = "Mail Send";
            }
            else
            {
                lblmsg.Visible = true;
                lblmsg.Text = "Mail Not Send";
            }
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
                        drno = Request.QueryString["drno"];
                        string p_source_doc_type = "DR";// Request.QueryString["SOURCEDOCTYPE"];
                        classDR_Internal_Approve_Attachment_BLL attachmentid = new classDR_Internal_Approve_Attachment_BLL();
                        deleteresult = attachmentid.Delete_DocAttachmentID(doc_attachment_id.ToString().Trim());
                        //  sourcedocnumber = Request.QueryString["SOURCEDOCNO"];
                        classDR_Internal_Approve_Attachment_BLL obj = new classDR_Internal_Approve_Attachment_BLL();
                        DataTable dt1 = obj.LoadDRAttachmentDetails(drno.Trim(), p_source_doc_type);
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
                    drno = Request.QueryString["drno"];
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
                    Response.AddHeader("content-disposition", "attachment;filename=" + drno.Trim() + filename);
                    Response.ContentType = "application/octectstream";
                    Response.BinaryWrite(ar);
                    Response.End();
                }
            }

        }


    }
}