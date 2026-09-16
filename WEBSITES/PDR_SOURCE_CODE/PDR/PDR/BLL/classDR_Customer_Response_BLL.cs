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
    class classDR_Customer_Response_BLL
    {

        public DataSet Populate_APPREJ()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_dr_combo_apprej_customer]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(ds);

                db.closeDBConnection();
                return ds;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }


        public DataTable UpdateDRNODetails(string p_dr_no, string p_dr_app_rej_id, string p_dr_app_rej_comment,string p_user_session_id)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_dr_apprej_customer_update]");
            try
            {
                if (p_dr_no == null || p_dr_no == "")
                {
                    command1.Parameters.AddWithValue("@p_dr_no", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_dr_no", (p_dr_no.Trim()));
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



        //public string sendCustomerApproveMail(DataTable dt, string attachmentIDs, string sessionID)
        public string sendCustomerApproveMail(DataTable dt, string attachmentIDs, string sessionID, string tomaillist, string ccmaillist, string subject, string bodytext)
        {
            string strResult;
            try
            {
                //Create the msg object to be sent
                StringBuilder strBodyHTML = new StringBuilder();
                DataRow r;

                r = dt.Rows[0];
                string toEmailID = "";// "kalai@eplt.in";
                string RequesterEmail;
                string CCmailList="";
                MailAddress fromMailId = new MailAddress("wf@gemmaknits.com");
                MailAddress requestormail;
                // MailAddress toMailId;

                if (ccmaillist != "")
                {
                    CCmailList = ccmaillist;// + ",kalai@eplt.in" + "," + "suresh@geminitextile.com";
                }
                else
                {
                    //CCmailList = Convert.ToString(r["requester_email"]) + "," + Convert.ToString(r["preparer_email"]) + "," + Convert.ToString(r["sample_room_Email"]) + "," + Convert.ToString(r["factory_manager_email"]) + "," + Convert.ToString(r["md_email"]) + ",kalai@eplt.in" + "," + "suresh@geminitextile.com";
                }
                if (tomaillist != "")
                {
                    toEmailID = tomaillist;//
                }
                else
                {
                    toEmailID = "wf@gemmaknits.com";
                }
                //string[] TOID = toEmailID.Split(',');
                //foreach (string TOEmail in TOID)
                //{
                //    if (TOEmail != "")
                //    {
                //    }
                //}
                //MailAddress toMailId = new MailAddress(toEmailID);
                MailMessage msg = new MailMessage(fromMailId, fromMailId);
                string[] TOId = toEmailID.Split(',');
                foreach (string ToEmail in TOId)
                {
                    if (ToEmail.ToString() != "")
                    {
                        msg.To.Add(new MailAddress(ToEmail)); //Adding Multiple CC email Id
                    }
                }
                //  msg.To.Add(toMailId);
                string[] CCId = CCmailList.Split(',');
                foreach (string CCEmail in CCId)
                {
                    if (CCEmail != "")
                    {
                        msg.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                    }
                }
                // msg.CC.Add("kalai@eplt.in");
                //Add your email address to the recipients
                //Configure the address we are sending the mail from

                // -----------
                //                Font3 = "<font size=""" + "3" + """" + "face=""" + "Times" + """" + ">";
                //              Font2 = "<font size=""" + "2" + """" + "face=""" + "Times" + """" + ">";

                if (bodytext != "")
                {
                    string msgbody = bodytext.ToString().Replace("\r\n", "<br>");
                    msgbody = msgbody.Replace("DR NO:  " + dt.Rows[0]["DR_NO"].ToString().Trim() + "  " + dt.Rows[0]["CUSTOMER_NAME"].ToString().Trim(), "");
                    msgbody = msgbody.Replace("DESIGN NO:  " + dt.Rows[0]["NEW_DESIGN_NO"].ToString().Trim(), "");
                    msgbody = msgbody.Replace("CUSTOMER APP/REJ:  " + dt.Rows[0]["DR_CUSTOMER_aPP_REJ_CODE"].ToString().Trim(), "");
                    msgbody = msgbody.Replace("ADD TO COLLECTION:  " + dt.Rows[0]["ADD_tO_cOLLECTION"].ToString().Trim(), "");
                    msgbody = msgbody.Replace("CUSTOMER COMMENT:" + dt.Rows[0]["DR_CUSTOMER_aPP_REJ_COMMENT"].ToString().Trim(), "");
                    strBodyHTML.Append("<HTML>");
                    strBodyHTML.Append("DR NO:  " + dt.Rows[0]["DR_NO"].ToString().Trim() + "  " + dt.Rows[0]["CUSTOMER_NAME"].ToString().Trim());
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("DESIGN NO:  " + dt.Rows[0]["NEW_DESIGN_NO"].ToString().Trim() + "  ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("CUSTOMER APP/REJ:  " + dt.Rows[0]["DR_CUSTOMER_aPP_REJ_CODE"].ToString().Trim() + "     ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("ADD TO COLLECTION:  " + dt.Rows[0]["ADD_tO_cOLLECTION"].ToString().Trim() + "     ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("CUSTOMER COMMENT:  " + dt.Rows[0]["DR_CUSTOMER_aPP_REJ_COMMENT"].ToString().Trim() + "</FONT>");
                    msg.Body = msgbody + strBodyHTML.ToString();//
                    msg.Subject = dt.Rows[0]["CUSTOMER_RESPONSE_subject"].ToString();
                }
                else
                {
                    strBodyHTML.Append("<HTML>");
                    strBodyHTML.Append("<H1><Td color='blue'>INTERNAL APPROVAL</td></h1>");
                    strBodyHTML.Append("<P>");
                    //                strBodyHTML.Append("<H1>");Append(Font3);

                    // strBodyHTML.Append("<a href=\"http://110.78.165.119:86/ui/Develop_Request.aspx?pdrno=" + dt.Rows[0]["design_no"] + "&SessionID=" + sessionID + "&pagename=Develop_Request" + "\">");
                    strBodyHTML.Append("<HTML>");
                    strBodyHTML.Append("DR NO:  " + dt.Rows[0]["DR_NO"].ToString().Trim() + "  ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("DESIGN NO:  " + dt.Rows[0]["NEW_DESIGN_NO"].ToString().Trim() + "  ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("INTENAL APP/REJ:  " + dt.Rows[0]["DR_CUSTOMER_aPP_REJ_CODE"].ToString().Trim() + "     ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("ADD TO COLLECTION:  " + dt.Rows[0]["ADD_tO_cOLLECTION"].ToString().Trim() + "     ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("INTERNAL COMMENT:  " + dt.Rows[0]["DR_CUSTOMER_aPP_REJ_COMMENT"].ToString().Trim() + "</FONT>");

                    msg.Subject = dt.Rows[0]["CUSTOMER_RESPONSE_subject"].ToString();

                    msg.Body = strBodyHTML.ToString();
                }
                if (subject != "")
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
                // --------
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
