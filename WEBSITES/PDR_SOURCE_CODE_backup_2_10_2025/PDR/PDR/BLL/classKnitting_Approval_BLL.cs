using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Net.Mail;
namespace PDR.BLL
{
    class classKnitting_Approval_BLL
    {

        public DataTable GetDRKnittingDetails(string p_design_no)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_dr_knitting_apprej_select]");
            try
            {
                if (p_design_no == null || p_design_no == "")
                {
                    command1.Parameters.AddWithValue("@p_design_no", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_design_no", (p_design_no.Trim()));
                }
                DataTable GetDRNODetails = db.ExecuteCommandAndGetDataTable(command1);
                db.closeDBConnection();
                return GetDRNODetails;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public DataTable UpdateKnittingApprovalDetails(string p_kono, string p_dr_app_rej_id, string p_dr_app_rej_comment, string p_user_session_id)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_dr_apprej_knitting_update]");
            try
            {
                if (p_kono == null || p_kono == "")
                {
                    command1.Parameters.AddWithValue("@p_kono", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_kono", (p_kono.Trim()));
                }
                if (p_dr_app_rej_id == null || p_dr_app_rej_id == "")
                {
                    command1.Parameters.AddWithValue("@p_dr_app_rej_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_dr_app_rej_id", Convert.ToInt64(p_dr_app_rej_id.Trim()));
                }
                if (p_dr_app_rej_comment == null || p_dr_app_rej_comment == "")
                {
                    command1.Parameters.AddWithValue("@p_dr_app_rej_comment", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_dr_app_rej_comment", (p_dr_app_rej_comment.Trim()));
                }

                if (p_user_session_id == null || p_user_session_id == "")
                {
                    command1.Parameters.AddWithValue("@p_user_session_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_user_session_id", Convert.ToInt64(p_user_session_id.Trim()));
                }
                DataTable result = db.ExecuteCommandAndGetDataTable(command1);
                db.closeDBConnection();
                return result;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }

        }


        public string sendKnittingApproveMail(DataTable dt, string attachmentIDs, string sessionID, string tomaillist,string ccmaillist,string subject,string bodytext)
        {
            string strResult;
            try
            {
                //Create the msg object to be sent
                StringBuilder strBodyHTML = new StringBuilder();
                DataRow r;

                r = dt.Rows[0];
                string toEmailID = "kalai@eplt.in";
                string ccMailList;
                MailAddress fromMailId = new MailAddress("wf@gemmaknits.com");
                // MailAddress toMailId;
                // toEmailID = Convert.ToString(r["factory_manager_email"]) + "," + Convert.ToString(r["dr_staff_email"]) + "," + Convert.ToString(r["production_email"]);
                //string[] TOId = toEmailID.Split(',');
                ccMailList = "kalai@eplt.in" + "," + "suresh@geminitextile.com";
                string[] CCId = ccMailList.Split(',');
                MailAddress toMailId = new MailAddress(toEmailID);               
                MailMessage msg = new MailMessage(fromMailId, toMailId);

                //foreach (string ToEmail in TOId)
                //{
                //    msg.To.Add(new MailAddress(ToEmail)); //Adding Multiple CC email Id
                //}
                msg.To.Add(toMailId);   
                           
                foreach (string CCEmail in CCId)
                {
                    msg.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                }
                if (bodytext != "")
                {
                    string msgbody = bodytext.ToString().Replace("\r\n", "<br>");
                    msgbody = msgbody.Replace("DR KNITTING", "");
                    msgbody = msgbody.Replace("Design:" + dt.Rows[0]["design_no"].ToString().Trim() + "  " + dt.Rows[0]["CUSTOMER_NAME"].ToString().Trim(), "");
                    //msgbody = msgbody.Replace("DR NO:" + dt.Rows[0]["DR_NO"].ToString().Trim(), "").Replace(", KI: " + dt.Rows[0]["KONO"] ,"");
                    msgbody = msgbody.Replace("DR NO:" + dt.Rows[0]["DR_NO"] + "     ,KI: " + dt.Rows[0]["KONO"].ToString().Trim(), "");
                    msgbody = msgbody.Replace("DR Dt:" + dt.Rows[0]["pdr_date"].ToString().Trim(), "");
                    msgbody = msgbody.Replace("For:" + dt.Rows[0]["customer_name"].ToString().Trim(), "");
                    msgbody = msgbody.Replace("Sales person:" + dt.Rows[0]["requested_by"].ToString().Trim(), "");
                    strBodyHTML.Append("<HTML>");
                    strBodyHTML.Append("Design:" + dt.Rows[0]["design_no"] + "       </TR>");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("DR NO:" + dt.Rows[0]["DR_NO"] + "     ,KI: " + dt.Rows[0]["KONO"] + "     ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("DR Dt:" + dt.Rows[0]["pdr_date"] + "     ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("For:" + dt.Rows[0]["customer_name"]);
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("Sales person:" + dt.Rows[0]["requested_by"] + "</FONT>");
                    msg.Body = msgbody + strBodyHTML.ToString();//
                }
                else
                {
                    strBodyHTML.Append("<HTML>");
                    strBodyHTML.Append("<H1><Td color='blue'>DR KNITTING</td></h1>");
                    strBodyHTML.Append("<P>");
                    //                strBodyHTML.Append("<H1>");Append(Font3);

                    // strBodyHTML.Append("<a href=\"http://110.78.165.119:86/ui/Develop_Request.aspx?pdrno=" + dt.Rows[0]["design_no"] + "&SessionID=" + sessionID + "&pagename=Develop_Request" + "\">");
                    string strIPAddress = ConnectionDB.strLinkIPAddress;

                   //strBodyHTML.Append("<a href=\"http://103.40.140.12:86/ui/DR_Knitting_Sample_Approve.aspx?pagename=DR_Knitting_Sample_Approve&designno=" + dt.Rows[0]["design_no"] + "&SessionID=" + sessionID + "\">");
                    strBodyHTML.Append("<a href=\"http://" + strIPAddress + "/ui/DR_Knitting_Sample_Approve.aspx?pagename=DR_Knitting_Sample_Approve&designno=" + dt.Rows[0]["design_no"] + "&SessionID=" + sessionID + "\">");
                    strBodyHTML.Append("Design: <TR >" + dt.Rows[0]["design_no"] + "       </TR>");
                    strBodyHTML.Append("</a>");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("DR NO:  " + dt.Rows[0]["DR_NO"] + "     ,KI: " + dt.Rows[0]["KONO"] + "     ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("DR Dt:  " + dt.Rows[0]["pdr_date"] + "     ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("For: " + dt.Rows[0]["customer_name"]);
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("Sales person: " + dt.Rows[0]["requested_by"] + "</FONT>");
                    //              strBodyHTML.Append("</H1>");

                    // strBodyHTML.Append("<TABLE CELLSPACING=3 CELLPADING=1 >");

                    msg.Subject = "DESIGN " + dt.Rows[0]["design_no"] + " KNITTING APPROVED";

                    msg.Body = strBodyHTML.ToString();
                }
                if (subject !="")
                {
                    msg.Subject = subject;// "DESIGN " + dt.Rows[0]["design_no"] + " KNITTING APPROVED";
                }
                msg.IsBodyHtml = true;
                //string attachmentFilename = "//172.16.3.4/pdr_files/ANALYZE/" + attachmentIDs;// "//172.16.3.4/pdr_files/pdrdoc/PDR" + dt.Rows[0]["pdr_no"].ToString().Trim() + ".pdf";
                if (attachmentIDs != null && attachmentIDs.ToString().Trim() != "")
                {
                    // for attachment
                    Char delimiter = ',';
                    string[] words = attachmentIDs.Split(delimiter);
                    foreach (var word in words)
                    {
                        string attachmentFilename = "//172.16.3.4/pdr_files/ANALYZE/" + word.Trim();
                        msg.Attachments.Add(new Attachment(attachmentFilename.ToString().Trim()));
                    }
                }
                //
                //// --------
                ////Configure an SmtpClient to send the mail.
                //SmtpClient client = new SmtpClient("mail.gemmaknits.com", 25);
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.EnableSsl = false;
                //client.UseDefaultCredentials = false;

                ////Setup credentials to login to our sender email address ("UserName", "Password")
                //NetworkCredential credentials = new NetworkCredential("wf@gemmaknits.com", "wf@2016");
                //client.Credentials = credentials;
                //////Send the msg
                //client.Send(msg);
                var smtp = new System.Net.Mail.SmtpClient();
                //Cast the newtwork credentials in to the NetworkCredential class and use it .
                var credential = (System.Net.NetworkCredential)smtp.Credentials;
                string strHost = smtp.Host;
                int port = smtp.Port;
                smtp.Host = strHost;
                smtp.Port = port;
                string strUserName = credential.UserName;
                string strFromPass = credential.Password;
                smtp.Credentials = credential;
                smtp.Send(msg);

                strResult = "S";
                return strResult;
            }
            catch (Exception ex)
            {
                //If the message failed at some point, let the user know
                strResult = "F";
                return strResult;
                //"Your message failed to send, please try again."
            }
        }


    }
}
