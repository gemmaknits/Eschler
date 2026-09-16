using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PDR.BLL;
using System.Data;
using System.IO;
using System.Data.SqlClient;
namespace PDR.UI
{
    public partial class PDR_Approval : System.Web.UI.Page
    {
        DataTable dsapproval;
        protected void Page_Load(object sender, EventArgs e)
        {
            string sessionID = Request.QueryString["SessionID"];
            if (sessionID != null && sessionID != "")
            {
                SqlDataReader objreader = null;
                sessionID = Request.QueryString["SessionID"].ToString();
                classValidateUserLogin_BLL userValidation = new classValidateUserLogin_BLL();
                objreader = userValidation.ValidateLoginFromEmail(sessionID);
                string session_Valid = objreader["session_valid"].ToString();
                if (session_Valid == "Y")
                {
                    string pdrno = Request.QueryString["pdrno"].ToString();// Session["pdrno"].ToString();
                    Session["usersessionid"] = sessionID;
                    Response.Redirect("~/UI/PDR_approval.aspx?pdrno=" + pdrno);
                }
            }
            if (Session["usersessionid"] == null)
            {
                string pdrno = "";
                if (Request.QueryString["pdrno"] != null)
                {
                    pdrno = Request.QueryString["pdrno"];//.ToString();
                }
                Response.Redirect("~/ui/login.aspx?pagename=PDR_Approval" + "&pdrno=" + pdrno.Trim());
            }
            if (!IsPostBack)
            {
                populate_Approval();
                classPDR_Approval_BLL objapproval = new classPDR_Approval_BLL();
                string pdrno = Request.QueryString["pdrno"];
                DataTable dtapproval = objapproval.LoadPDRApprovalDetails(pdrno);
                if (dtapproval.Rows.Count > 0)
                {
                    foreach (DataRow resultrow in dtapproval.Rows)
                    {
                        txtPDRNO.Text = resultrow["pdr_no"].ToString();
                        if (!string.IsNullOrEmpty(resultrow["pdr_date"].ToString()))
                        {
                            DateTime dt = Convert.ToDateTime(resultrow["pdr_date"].ToString());
                            txtDate.Text = String.Format("{0:dd-MM-yyyy}", dt);// dt.ToString("dd-MM-yyyy");// String.Format("M/d/yyyy", dt);// Convert.ToDateTime(row["pdr_date"]).ToString();
                        }
                        //txtDate.Text = string.Format(resultrow["pdr_date"].ToString(), "dd/MM/yyyy");
                        txtCustomer.Text = resultrow["customer_name"].ToString();
                        txtProduct.Text = resultrow["design_no"].ToString();
                        txtEndBuyer.Text = resultrow["endbuyername"].ToString();
                        lblFileName.Text = resultrow["file_location"].ToString();
                        hid_pdr_new_develop_req_id.Value = resultrow["pdr_new_develop_req_id"].ToString();
                        hid_approvalby.Value = resultrow["requested_by"].ToString();
                        txtComment.Text = resultrow["pdr_app_rej_comment"].ToString();
                        ddlApproval.SelectedValue = resultrow["pdr_app_rej_status"].ToString().Trim();
                    }
                }
            }
        }
        private void populate_Approval()
        {
            try
            {
                classPDR_Approval_BLL approval = new classPDR_Approval_BLL();
                dsapproval = approval.LoadPDRApproval();
                ddlApproval.DataSource = dsapproval;
                ddlApproval.DataTextField = "NAME";
                ddlApproval.DataValueField = "CODE";
                ddlApproval.DataBind();
            }
            catch(Exception EX)
            {
                String MSG = EX.Message.ToString();

            }
        }

        protected void BTNDONE_Click(object sender, EventArgs e)
        {
            string pdr_new_develop_req_id = hid_pdr_new_develop_req_id.Value;
            string pdr_app_rej_status = "";
            string pdr_app_rej_date = "";
            string pdr_app_rej_by = "";
            string pdr_app_rej_comment = "";
            pdr_app_rej_status = ddlApproval.SelectedValue.ToString().Trim();
            pdr_app_rej_date = txtDate.Text;
            pdr_app_rej_by = hid_approvalby.Value.Trim();
            pdr_app_rej_comment = txtComment.Text.Trim();
            classPDR_Approval_BLL objupdate = new classPDR_Approval_BLL();
            DataTable intresult = objupdate.UpdatePDRApprovalDetails(pdr_new_develop_req_id, pdr_app_rej_status, pdr_app_rej_date,
                                                                    pdr_app_rej_by, pdr_app_rej_comment);
            if (ddlApproval.SelectedValue == "APP")
            {
                lblmsg.Visible = true;
                lblmsg.Text = "APPROVAL DONE";
                lblmsg.ForeColor = System.Drawing.Color.Green;
                sendApproveApprovalMail();
            }
            if (ddlApproval.SelectedValue == "REJ")
            {
                lblmsg.Visible = true;
                lblmsg.Text = "REJECTED DONE";
                lblmsg.ForeColor = System.Drawing.Color.Red;
                sendApproveApprovalMail();
            }
        }
        private void sendApproveApprovalMail()
        {
            string filename = "";
            string sessionid = Session["usersessionid"].ToString();
           
            DataTable dt;
            //classDevelop_Request_BLL devReqBLL = new classDevelop_Request_BLL();
            string mailResult;
            classPDR_Approval_BLL getDRNODetails = new classPDR_Approval_BLL();
            
            string pdrno = Request.QueryString["pdrno"];
            DataTable dtapproval = getDRNODetails.LoadPDRApprovalDetails(pdrno);
            string toEmail_Sample_room_Email = "";
            string ccEmail_Requestor_Email = "";
            string ccEmail_Prepare_Email = "";
            foreach (DataRow row in dtapproval.Rows)
            {

                toEmail_Sample_room_Email = row["sample_room_email"].ToString();
                ccEmail_Requestor_Email = row["requester_email"].ToString();
                ccEmail_Prepare_Email= row["preparer_email"].ToString();
            }
            if (ccEmail_Requestor_Email !="")
            {
                ccEmail_Requestor_Email = ccEmail_Requestor_Email + "," + ccEmail_Prepare_Email;
            }
         
                mailResult = getDRNODetails.sendApprovalMail(dtapproval );
            if (mailResult == "S")
            {
                lblmsg.Visible = true;
                lblmsg.Text = "Mail Send";
            }
            else
            {
                lblmsg.Visible = true;
                lblmsg.Text = mailResult;// "Mail Not Send";
            }
        }
        protected void btndownload_Click(object sender, EventArgs e)
        {
            string filepath = lblFileName.Text;

            if (filepath != "")
            {

                ////
                //byteImageData = ReadImage(lblFileName.Text.Trim(), new string[] { ".gif", ".jpg", ".bmp" });
                //byteArrayToImage(byteImageData);
                Session["uploadedfilename"] = lblFileName.Text.Trim();
                //// Response.Write("window.open("fileopen.aspx")");
                // Response.Write("<script>");
                // Response.Write("window.open('" + filepath + ",'_blank', ' fullscreen=no')");
                //// //Response.Write("window.open(" + path + ",'_blank')");
                // Response.Write("</script>");
                //Response.Redirect("fileopen.aspx");
                ////
                string extension;
                string filename = Path.GetFileName(filepath);
                extension = Path.GetExtension(filepath);
                Page.ClientScript.RegisterStartupScript(
              this.GetType(), "OpenWindow", "window.open('" + filepath + "','_newtab');", true);
                System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                byte[] ar = new byte[(int)fs.Length];
                fs.Read(ar, 0, (int)fs.Length);
                fs.Close();

                Response.AddHeader("content-disposition", "attachment;filename=" + filename);
                Response.ContentType = "application/octectstream";
                Response.BinaryWrite(ar);
                Response.End();
                System.Diagnostics.Process.Start(filepath);
                Session["uploadedfilename"] = lblFileName.Text.Trim();
                
            }
            else
            {
                Response.Write("<script>");
                Response.Write("window.open('" + lblFileName.Text.Trim() + "','_blank', ' fullscreen=yes')");
                //Response.Write("window.open(" + path + ",'_blank')");
                Response.Write("</script>");
                
            }
        }


      


    }
}