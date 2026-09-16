using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PDR.BLL;
using System.Web.Services;
namespace PDR.UI
{
    public partial class Knitting_Approval : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                Response.Redirect("~/ui/login.aspx?pagename=Knitting_Approval");
            }
            if (!IsPostBack)
            {
                populate_APPREJ();
                //txtApproveType.Text = Request.QueryString["Type"];
                classDR_Internal_Approve_Attachment_BLL obj = new classDR_Internal_Approve_Attachment_BLL();
                DataTable dt1 = obj.LoadDRAttachmentDetails(txtDRNo.Text.Trim(),"KI");
                grdAttachmentDetails.DataSource = dt1;
                grdAttachmentDetails.DataBind();
            }
        }

        private void populate_APPREJ()
        {
            DataSet dsAPPREJ;
            classDR_Approval_BLL apprej = new classDR_Approval_BLL();
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
                    txtComment.Text = resultrow["dr_knitting_app_rej_comment"].ToString();
                    hidapprejID.Value = resultrow["dr_knitting_app_rej_id"].ToString();
                    MyLnkButton.Text= resultrow["pdr_no"].ToString();
                    kikono.Text= resultrow["kono"].ToString();
                    if (hidapprejID.Value  != "" || hidapprejID.Value != null)
                    {
                        ddlAppRej.SelectedValue = hidapprejID.Value.Trim();
                    }
                    hidkono.Value= resultrow["kono"].ToString();

                }
            }
            classDR_Internal_Approve_Attachment_BLL obj = new classDR_Internal_Approve_Attachment_BLL();
            DataTable dt1 = obj.LoadDRAttachmentDetails(txtDesignNo.Text.Trim(), "KI");
            grdAttachmentDetails.DataSource = dt1;
            grdAttachmentDetails.DataBind();
        }

        protected void btnDone_Click(object sender, EventArgs e)
        {
            string p_user_session_id = (Session["usersessionid"].ToString());

            classKnitting_Approval_BLL objDRNOUpdate = new classKnitting_Approval_BLL();
            DataTable dt_DRNODetails = objDRNOUpdate.UpdateKnittingApprovalDetails(hidkono.Value.Trim(),ddlAppRej.SelectedItem.Value.Trim(),
                                                                        txtComment.Text.Trim(), p_user_session_id);

            lblmsg.Visible = true;
        }

        protected void ImgAttachment_Click(object sender, ImageClickEventArgs e)
        {

            Response.Redirect("~/UI/DR_Internal_Approve_Attachment.aspx?SOURCEDOCNO=" + txtDesignNo.Text.Trim() + "&SOURCEDOCTYPE=KI");
           // Response.Redirect("~/UI/DR_Internal_Approve_Attachment.aspx?SOURCEDOCNO=KONO" + "&SOURCEDOCTYPE=KI");
        }

        protected void ImgEmal_Click(object sender, ImageClickEventArgs e)
        {
            string filename = "";
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
            }
            Session["attachmentfilename"] = filename;
                Response.Redirect("~/UI/SendMail_KnittingApproval.aspx?SOURCEDOCNO=" + txtDesignNo.Text.Trim() + "&SOURCEDOCTYPE=KI");
          //  sendInternalApprovalMail();
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
            classDevelop_Request_BLL devReqBLL = new classDevelop_Request_BLL();
            string mailResult;
            classKnitting_Approval_BLL getDRNODetails = new classKnitting_Approval_BLL();
            DataTable dt_GetDRNODetails = getDRNODetails.GetDRKnittingDetails(txtDesignNo.Text.Trim());
            mailResult = getDRNODetails.sendKnittingApproveMail(dt_GetDRNODetails, filename, sessionid,"","","","");
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
            string kono= kikono.Text.Trim();
            Response.Redirect("~/UI/yarnlots.aspx?kikono=" + kono.Trim() + "&designno=" + txtDesignNo.Text.Trim());
        }


        //private void DetailsPDRINPDF_click(object sender, EventArgs e)
        //{
        //    Session["pdrno"] = MyLnkButton.Text.Trim();
        //    Session["rptoption"] = "email";
        //    Server.Execute("~/Reports/Print_Develop_Request.aspx");

        //}
    }
}