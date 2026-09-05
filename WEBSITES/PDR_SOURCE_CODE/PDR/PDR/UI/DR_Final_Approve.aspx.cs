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
    public partial class DR_Final_Approve : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                Response.Redirect("~/ui/login.aspx?page=DR_Final_Approve");
            }
            if (!IsPostBack)
            {
                populate_APPREJ();
                //txtApproveType.Text = Request.QueryString["Type"];
            }
        }

        private void populate_APPREJ()
        {
            DataSet dsAPPREJ;
            classDR_Final_Response_BLL apprej = new classDR_Final_Response_BLL();
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
                DataObj.dr_final_app_rej_comment = dtRow["dr_final_app_rej_comment"].ToString();
                DataObj.dr_final_app_rej_id = dtRow["dr_final_app_rej_id"].ToString();
                obj_GetDRNODetails.Add(DataObj);
            }
            return obj_GetDRNODetails.ToArray();
        }

        public class DetailsClass_GetDRNoDetails
        {
            public string dr_date { get; set; }
            public string design_no { get; set; }
            public string customer_name { get; set; }
            public string dr_final_app_rej_comment { get; set; }
            public string dr_final_app_rej_id { get; set; }
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
                    txtArticle.Text = resultrow["design_no"].ToString();
                    txtCustomer.Text = resultrow["customer_name"].ToString();
                    txtComment.Text = resultrow["dr_final_app_rej_comment"].ToString();
                    hidapprejID.Value = resultrow["dr_final_app_rej_id"].ToString();
                    if (!string.IsNullOrEmpty(resultrow["dr_final_app_rej_id"].ToString()))
                    {
                        ddlAppRej.SelectedValue = resultrow["dr_final_app_rej_id"].ToString().Trim();
                    }
                }
            }
        }

        protected void btnDone_Click(object sender, EventArgs e)
        {
            string p_user_session_id = (Session["usersessionid"].ToString());

            classDR_Final_Response_BLL objDRNOUpdate = new classDR_Final_Response_BLL();
            DataTable dt_DRNODetails = objDRNOUpdate.UpdateDRNODetails(txtDRNo.Text.Trim(), hidapprejID.Value.Trim(),
                                                                        txtComment.Text.Trim(), p_user_session_id);

            lblmsg.Visible = true;
        }

        protected void ImgAttachment_Click(object sender, ImageClickEventArgs e)
        {

            Response.Redirect("~/UI/DR_Internal_Approve_Attachment.aspx?SOURCEDOCNO=" + txtDRNo.Text.Trim() + "&PRODUCT=" + txtArticle.Text.Trim() + "&SOURCEDOCTYPE=DR");
        }

        protected void ImgEmal_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/UI/SendMail_FinalApproval.aspx?drno=" + txtDRNo.Text.Trim() + "&PRODUCT=" + txtArticle.Text.Trim() + "&SOURCEDOCTYPE=DR");
        }
    }
}