using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using PDR.BLL;
namespace PDR.UI
{
    public partial class SendMail_KnittingApproval : System.Web.UI.Page
    {
        string designno;
        string TOMailList;
        string filename;
        string mailResult;
        string sessionID;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                Response.Redirect("~/ui/login.aspx?pagename=Avail_SendMail");
            }
            if (!IsPostBack)
            {

                sessionID = Session["usersessionid"].ToString();
                StringBuilder strBodyHTML = new StringBuilder();
                designno = Request.QueryString["SOURCEDOCNO"];
                filename = Request.QueryString["attachmentfilename"];
                //txtTo.Text = Session["factory_manager_email"].ToString();
                //txtSubject.Text = "PDR No." + pdrno + " have shortage of yarn";
                txtBody.Text = "";
                classKnitting_Approval_BLL getDRNODetails = new classKnitting_Approval_BLL();
                DataTable dt = getDRNODetails.GetDRKnittingDetails(designno.Trim());
                TOMailList = dt.Rows[0]["factory_manager_email"].ToString();
                TOMailList=TOMailList + "," + dt.Rows[0]["dr_staff_email"].ToString();
                TOMailList = TOMailList + "," + dt.Rows[0]["production_email"].ToString();
                txtTo.Text = TOMailList;
                txtSubject.Text = "DESIGN " + dt.Rows[0]["design_no"] + " KNITTING APPROVED";
                strBodyHTML.Append("<HTML>");
                strBodyHTML.Append("Design:" + dt.Rows[0]["design_no"] + "       </TR>");
                strBodyHTML.Append("<BR>");
                strBodyHTML.Append("DR NO:" + dt.Rows[0]["DR_NO"] + "     ,KI: " + dt.Rows[0]["KONO"] + "     ");
                strBodyHTML.Append("<BR>");
                strBodyHTML.Append("DR Dt:" + dt.Rows[0]["pdr_date"] + "     ");
                strBodyHTML.Append("<BR>");
                strBodyHTML.Append("For:" + dt.Rows[0]["customer_name"]);
                strBodyHTML.Append("<BR>");
                strBodyHTML.Append("Sales person:" + dt.Rows[0]["requested_by"] + "");
                //              strBodyHTML.Append("</H1>");

                string sent = "\r\n" + "\r\n";// + "\r\n" + "\r\n" + "\r\n" + "\r\n";
                sent += "DR KNITTING" + "\r\n";
                sent += "Design:" + dt.Rows[0]["design_no"].ToString().Trim() + "  " + dt.Rows[0]["CUSTOMER_NAME"].ToString().Trim() + "\r\n";
                sent += "DR NO:" + dt.Rows[0]["DR_NO"].ToString().Trim() + "     ,KI: " + dt.Rows[0]["KONO"].ToString().Trim() + "\r\n";
                sent += "DR Dt:" + dt.Rows[0]["pdr_date"].ToString().Trim() + "\r\n";
                sent += "For:" + dt.Rows[0]["customer_name"].ToString().Trim() + "\r\n";
                sent += "Sales person:" + dt.Rows[0]["requested_by"].ToString().Trim();
             
                txtBody.Text =  sent;               
            }


        }

        protected void btnSendMail_Click(object sender, EventArgs e)
        {
            classKnitting_Approval_BLL getDRNODetails = new classKnitting_Approval_BLL();
            designno = Request.QueryString["SOURCEDOCNO"];
            DataTable dt_GetDRNODetails = getDRNODetails.GetDRKnittingDetails(designno.Trim());
            filename = Request.QueryString["attachmentfilename"];
            mailResult = getDRNODetails.sendKnittingApproveMail(dt_GetDRNODetails, filename, sessionID , txtTo.Text.Trim(), txtCC.Text.Trim(), txtSubject.Text.Trim(), txtBody.Text.Trim());
            if (mailResult =="S")
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
    }
}