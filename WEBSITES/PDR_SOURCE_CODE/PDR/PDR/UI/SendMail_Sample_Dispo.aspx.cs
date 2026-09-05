using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PDR.BLL;
using System.Data;
using System.IO;
namespace PDR.UI
{
    public partial class SendMail_Sample_Dispo : System.Web.UI.Page
    {
        string designno;
        string TOMailList;
        string CCMailList;
        string filename;
        string mailResult;
        string sessionID;
        string source_doc_no;
        string seq;
        string comment;
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dt;
            if (Session["usersessionid"] == null)
            {
                source_doc_no = Request.QueryString["SOURCEDOCNO"];
                designno = Request.QueryString["designo"];
                Response.Redirect("~/ui/login.aspx?pagename=SendMail_Sample_Dispo" + "&SOURCEDOCNO" + source_doc_no.Trim() + "&designno=" + designno.Trim());
            }
            if (!IsPostBack)
            {
                 source_doc_no = Request.QueryString["SOURCEDOCNO"];
                designno = Request.QueryString["designo"];
                seq = Request.QueryString["seq"];
                comment = Request.QueryString["comment"];
                classDR_Internal_Approve_Attachment_BLL obj = new classDR_Internal_Approve_Attachment_BLL();
                DataTable dt1;
                dt1 = obj.LoadDRAttachmentDetails(source_doc_no, "SAMPLE_DISPO");
                grdAttachmentDetails.DataSource = dt1;
                grdAttachmentDetails.DataBind();
                classKnitting_Approval_BLL getDRNODetails = new classKnitting_Approval_BLL();
                 dt = getDRNODetails.GetDRKnittingDetails(designno.Trim());
                TOMailList = dt.Rows[0]["factory_manager_email"].ToString();
                CCMailList = dt.Rows[0]["dr_staff_email"].ToString();
                CCMailList = CCMailList + "," + dt.Rows[0]["production_email"].ToString();
                CCMailList = CCMailList + "," + dt.Rows[0]["rd_manager_email"].ToString();
                txtTo.Text = TOMailList;
                txtCC.Text = CCMailList;
                txtSubject.Text = "KNIT SAMPLE";
                string sent = "\r\n" + "\r\n";// + "\r\n" + "\r\n" + "\r\n";
                sent += "KNIT SAMPLE" + "\r\n";
                sent += "DESIGN NO: " + dt.Rows[0]["design_no"].ToString().Trim() + "  " + dt.Rows[0]["CUSTOMER_NAME"].ToString().Trim() + "\r\n";
                sent += "KI: " + dt.Rows[0]["KONO"].ToString().Trim() + "\r\n"; 
                sent += "DR NO: " + dt.Rows[0]["DR_NO"].ToString().Trim() + "\r\n";
                sent += "SEQ: " + seq.Trim() + "\r\n";
                sent += "COMMENT:" + comment.Trim() + "\r\n";
                txtBody.Text = sent;
            }
           
        }

        protected void btnSendMail_Click(object sender, EventArgs e)
        {
            classKnitting_Approval_BLL getDRNODetails = new classKnitting_Approval_BLL();
            designno = Request.QueryString["designo"];
            string sessionID = Session["usersessionid"].ToString();
            string filename = "";
            string mailResult = "";
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
                //file_description = Request[col1.UniqueID] as string;
            }
            DataTable dt_GetDRNODetails = getDRNODetails.GetDRKnittingDetails(designno.Trim());
           // filename = Request.QueryString["attachmentfilename"];
            classDR_Sample_Dispo obj = new classDR_Sample_Dispo();
            seq = Request.QueryString["seq"];
            comment = Request.QueryString["comment"];
            mailResult = obj.sendSampleKnittingMail(designno.Trim(), filename, sessionID, txtTo.Text.Trim(), txtCC.Text.Trim(), txtSubject.Text.Trim(), txtBody.Text.Trim(),seq,comment);
            //(DataTable dt, string attachmentIDs, string sessionID, string tomaillist, string ccmaillist, string subject, string bodytext)
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
                        string p_source_doc_type = "SAMPLE_DISPO";// Request.QueryString["SOURCEDOCTYPE"];
                        classDR_Internal_Approve_Attachment_BLL attachmentid = new classDR_Internal_Approve_Attachment_BLL();
                        deleteresult = attachmentid.Delete_DocAttachmentID(doc_attachment_id.ToString().Trim());
                        //  sourcedocnumber = Request.QueryString["SOURCEDOCNO"];
                        classDR_Internal_Approve_Attachment_BLL obj = new classDR_Internal_Approve_Attachment_BLL();
                        DataTable dt1 = obj.LoadDRAttachmentDetails(source_doc_no.Trim(), p_source_doc_type);
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
                    source_doc_no = Request.QueryString["SOURCEDOCNO"];
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
                    Response.AddHeader("content-disposition", "attachment;filename=" + source_doc_no.Trim() + filename);
                    Response.ContentType = "application/octectstream";
                    Response.BinaryWrite(ar);
                    Response.End();
                }
            }

        }


    }
}