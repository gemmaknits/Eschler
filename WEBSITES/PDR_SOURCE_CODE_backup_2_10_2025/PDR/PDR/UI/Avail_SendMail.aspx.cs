using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PDR.BLL;
using System.Data;
using System.Text;
namespace PDR.UI
{
    public partial class Avail_SendMail : System.Web.UI.Page
    {
        string pdrno;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                string pdrno= Request.QueryString["pdrno"];
                Response.Redirect("~/ui/login.aspx?pagename=Avail_SendMail" + "&pdrno=" + pdrno.Trim());
            }
            if (!IsPostBack)
            {
                string sessionID = Session["usersessionid"].ToString();
                StringBuilder strBodyHTML = new StringBuilder();
                pdrno = Request.QueryString["pdrno"];
                txtTo.Text = Session["avilToEmailID"].ToString();
                txtSubject.Text= "PDR No." + pdrno + " have shortage of yarn";
                txtBody.Text = "";
                classDevelop_Request_BLL developRequest = new classDevelop_Request_BLL();
                DataTable dt = developRequest.LoadPDRList(pdrno);
                strBodyHTML.Append("<HTML>");
                strBodyHTML.Append("<H1> PDR </h1>");
                strBodyHTML.Append("<P>");
                //                strBodyHTML.Append("<H1>");
                //              strBodyHTML.Append(Font3);
               // strBodyHTML.Append("<a href=\"http://110.78.165.119:86/ui/PDR_Approval.aspx?pdrno=" + dt.Rows[0]["pdr_no"] + "&SessionID=" + sessionID + "\">");
                strBodyHTML.Append("No. " + dt.Rows[0]["pdr_no"] + "       ");
                strBodyHTML.Append("</a>");
                strBodyHTML.Append("<BR>");
                strBodyHTML.Append("Dt:  " + dt.Rows[0]["pdr_date"] + "     ");
                strBodyHTML.Append("<BR>");
                strBodyHTML.Append("For: " + dt.Rows[0]["customer_name"]);
                strBodyHTML.Append("<BR>");
                strBodyHTML.Append("Sales person: " + dt.Rows[0]["requested_by"] + "</FONT>");
                //              strBodyHTML.Append("</H1>");
               
                string sent = "\r\n" + "\r\n" + "\r\n" + "\r\n" + "\r\n" + "\r\n"; 
                sent += "PDR" + "\r\n";
               // sent +=@"< a href =http://110.78.165.119:86/ui/PDR_Approval.aspx?pdrno=" + dt.Rows[0]["pdr_no"] + "&SessionID=" + sessionID + "\">";

                sent +=  "\r\n";
                sent +=  "No. " + dt.Rows[0]["pdr_no"] + "       " +  "\r\n";
                sent += "Dt:  " + dt.Rows[0]["pdr_date"] + "       " + "\r\n";
                sent += "For: " + dt.Rows[0]["customer_name"] + "       " + "\r\n";
                sent += "Sales person: " + dt.Rows[0]["requested_by"];

                txtBody.Text = sent;// strBodyHTML.ToString();
                //msg.Subject = "PDR No." + dt.Rows[0]["pdr_no"] + " requires your approval";
                //msg.Body = strBodyHTML.ToString();
                //msg.IsBodyHtml = true;

            }
        }
        protected void btnSendMail_Click(object sender, EventArgs e)
        {
            pdrno = Request.QueryString["pdrno"];
            string sessionID = Session["usersessionid"].ToString();
            DataTable dt;
            classDevelop_Request_BLL devReqBLL = new classDevelop_Request_BLL();
            string mailResult;
            classDevelop_Request_BLL developRequest = new classDevelop_Request_BLL();
            dt = developRequest.LoadPDRList(pdrno);
            mailResult = devReqBLL.sendAvailMail(dt,txtTo.Text.Trim(),txtCC.Text.Trim(),txtSubject.Text.Trim(),txtBody.Text.Trim(),sessionID);
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
    }
}