using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using System.Net;
namespace PDR.BLL
{
    class classDR_Approval_BLL
    {
        public DataSet Populate_APPREJ
            ()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_dr_combo_apprej_internal]");
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


        public DataTable GetDRNODetails(string p_dr_no)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_dr_apprej_select]");
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
                DataTable GetDRNODetails = db.ExecuteCommandAndGetDataTable(command1);
                db.closeDBConnection();
                return GetDRNODetails;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataTable UpdateDRNODetails(string p_dr_no, string p_dr_app_rej_id, string p_dr_app_rej_comment, string p_send_to_customer,
                                        string p_add_to_collection, string p_user_session_id)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_dr_apprej_internal_update]");
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
                    command1.Parameters.AddWithValue("@p_dr_app_rej_id",Convert.ToInt64(p_dr_app_rej_id.Trim()));
                }
                if (p_dr_app_rej_comment == null || p_dr_app_rej_comment == "")
                {
                    command1.Parameters.AddWithValue("@p_dr_app_rej_comment", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_dr_app_rej_comment", (p_dr_app_rej_comment.Trim()));
                }
                if (p_send_to_customer == null || p_send_to_customer == "")
                {
                    command1.Parameters.AddWithValue("@p_send_to_customer", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_send_to_customer", (p_send_to_customer.Trim()));
                }
                if (p_add_to_collection == null || p_add_to_collection == "")
                {
                    command1.Parameters.AddWithValue("@p_add_to_collection", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_add_to_collection", (p_add_to_collection.Trim()));
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

        //public string sendApprovalMail(DataTable dt,string filename, string sessionID)
        //{
        //    string strResult;
        //    try
        //    {
        //        //Create the msg object to be sent
        //        StringBuilder strBodyHTML = new StringBuilder();
        //        DataRow r;
        //        String RequesterEmail;
        //        string toemailID;
        //        string MDEmailID;
        //        string ccEmailID;
        //       /// String PreparerEmail;

        //        r = dt.Rows[0];
        //        RequesterEmail = Convert.ToString(r["requester_email"]);
        //        MDEmailID = Convert.ToString(r["md_email"]);
        //        toemailID= Convert.ToString(r["sample_room_email"]);
        //        ccEmailID = MDEmailID + " ," + RequesterEmail + "," + "suresh@geminitextile.com";
        //        MailAddress fromMailId = new MailAddress("wf@gemmaknits.com");
        //        MailAddress toMailId = new MailAddress(toemailID);
        //        MailAddress ccMailID = new MailAddress(ccEmailID);

        //        MailMessage msg = new MailMessage(fromMailId, toMailId);
        //        msg.CC.Add(ccMailID);
        //        //Add your email address to the recipients
        //        //Configure the address we are sending the mail from

        //        strBodyHTML.Append("<HTML>");
        //        strBodyHTML.Append("<H1> PDR </h1>");
        //        strBodyHTML.Append("<P>");
        //        //                strBodyHTML.Append("<H1>");
        //        //              strBodyHTML.Append(Font3);
        //        //strBodyHTML.Append("<a href=\"http://110.78.165.119:86/ui/PDR_Approval.aspx?pdrno=" + dt.Rows[0]["pdr_no"] + "&SessionID=" + sessionID + "\">");
        //        strBodyHTML.Append("DR No. " + dt.Rows[0]["dr_no"] + "       ");
        //        strBodyHTML.Append("</a>");
        //        strBodyHTML.Append("<BR>");
        //        strBodyHTML.Append("Date:  " + dt.Rows[0]["dr_date"] + "     ");
        //        strBodyHTML.Append("<BR>");
        //        strBodyHTML.Append("Design No: " + dt.Rows[0]["design_no"]);
        //        strBodyHTML.Append("<BR>");
        //        strBodyHTML.Append("Requested By: " + dt.Rows[0]["requested_by"]);
        //        strBodyHTML.Append("<BR>");
        //        strBodyHTML.Append("App/Rej By: " + dt.Rows[0]["dr_internal_app_rej_by"]);
        //        strBodyHTML.Append("<BR>");
        //        //strBodyHTML.Append("Sales person: " + dt.Rows[0]["requested_by"] + "</FONT>");
        //        //              strBodyHTML.Append("</H1>");

        //        strBodyHTML.Append("<TABLE CELLSPACING=3 CELLPADING=1 ");

        //        msg.Subject = "Please add DR No." + dt.Rows[0]["dr_no"] + " to collection";
        //        msg.Body = strBodyHTML.ToString();
        //        msg.IsBodyHtml = true;
        //        // string filename = "//172.16.3.4/pdr_files/pdrdoc/PDR" + dt.Rows[0]["pdr_no"].ToString().Trim() + ".pdf";
        //        //  Attachment attchfile = new Attachment((filename));
        //        //  attchfile.Name = "PDR" + dt.Rows[0]["pdr_no"].ToString().Trim() + ".pdf";
        //        //   msg.Attachments.Add(attchfile);
        //        // --------
        //        //Configure an SmtpClient to send the mail.
        //        SmtpClient client = new SmtpClient("61.19.245.242", 25);
        //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        client.EnableSsl = false;
        //        client.UseDefaultCredentials = false;

        //        //Setup credentials to login to our sender email address ("UserName", "Password")
        //        NetworkCredential credentials = new NetworkCredential("wf@gemmaknits.com", "wf@2016");
        //        client.Credentials = credentials;
        //        ////Send the msg
        //        client.Send(msg);
        //        strResult = "S";
        //        return strResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        //If the message failed at some point, let the user know
        //        strResult = "F";
        //        return strResult;
        //        //"Your message failed to send, please try again."
        //    }
        //}

        public string sendInternalApprovalMail(DataTable dt, string attachmentIDs, string sessionID, string tomaillist, string ccmaillistIN, string subject, string bodytext)
        {
            string strResult;
            try
            {
                //Create the msg object to be sent
                StringBuilder strBodyHTML = new StringBuilder();
                DataRow r;

                r = dt.Rows[0];
                string toEmailID = "";// "kalai@eplt.in";
                string ccMailList="";
                MailAddress fromMailId = new MailAddress("wf@gemmaknits.com");
                // MailAddress toMailId;
                if (tomaillist == "")
                {
                    //if (Convert.ToString(r["requester_email"]) != "")
                    //{
                    //    toEmailID = Convert.ToString(r["requester_email"]) + "," + Convert.ToString(r["PREPARER_email"]);
                    //}
                    //if (Convert.ToString(r["PREPARER_email"]) != "")
                    //{
                    //    toEmailID = toEmailID + "," + Convert.ToString(r["PREPARER_email"]);
                    //}
                    toEmailID = "wf@gemmaknits.com";

                }
                else
                {
                    toEmailID = tomaillist;
                }
                string[] TOId = toEmailID.Split(',');
                if (ccmaillistIN == "")
                {
                    //if (Convert.ToString(r["rd_staff_email"]) != "")
                    //{
                    //    ccMailList =  Convert.ToString(r["rd_staff_email"]);
                    //}
                    //if (Convert.ToString(r["factory_manager_email"]) != "")
                    //{
                    //    ccMailList = ccMailList + "," + Convert.ToString(r["factory_manager_email"]);

                    //}
                    //if (Convert.ToString(r["md_email"]) != "")
                    //{
                    //    ccMailList = ccMailList + "," + Convert.ToString(r["md_email"]);

                    //}
                    //if (Convert.ToString(r["md_email"]) != "")
                    //{
                    //    ccMailList = ccMailList + "," +  "suresh@geminitextile.com,kalai@eplt.in";
                    //}
                    ////ccMailList = Convert.ToString(r[""]) + "," + Convert.ToString(r[""]) + Convert.ToString(r[""]) +

                }
                   
                else
                {
                    ccMailList = ccmaillistIN;// + "," + "suresh@geminitextile.com,kalai@eplt.in"; ;
                }
                //ccMailList = "kalai@eplt.in" + "," + "suresh@geminitextile.com";
                string[] CCId = ccMailList.Split(',');
                //MailAddress toMailId = new MailAddress(toEmailID);
                MailMessage msg = new MailMessage(fromMailId, fromMailId);

                foreach (string ToEmail in TOId)
                {
                    if (ToEmail.ToString() != "")
                    {
                        msg.To.Add(new MailAddress(ToEmail)); //Adding Multiple CC email Id
                    }
                }
               // msg.To.Add(toMailId);
               
                foreach (string CCEmail in CCId)
                {
                    if (CCEmail != "")
                    {
                        msg.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                    }
                }
                if (bodytext != "")
                {
                    string msgbody = bodytext.ToString().Replace("\r\n", "<br>");
                    msgbody = msgbody.Replace("DR NO:  " + dt.Rows[0]["DR_NO"].ToString().Trim() + "  " + dt.Rows[0]["CUSTOMER_NAME"].ToString().Trim(), "");
                    msgbody = msgbody.Replace("DESIGN NO:  " + dt.Rows[0]["NEW_DESIGN_NO"].ToString().Trim(), "");
                    msgbody = msgbody.Replace("INTENAL APP/REJ:  " + dt.Rows[0]["DR_INTERNAL_aPP_REJ_CODE"].ToString().Trim(), "");
                    msgbody = msgbody.Replace("ADD TO COLLECTION:  " + dt.Rows[0]["ADD_tO_cOLLECTION"].ToString().Trim(), "");
                    msgbody = msgbody.Replace("SEND TO CUSTOMER:  " + dt.Rows[0]["SEND_tO_cUSTOMER"].ToString().Trim(), "");
                    msgbody = msgbody.Replace("INTERNAL COMMENT:" + dt.Rows[0]["DR_INTERNAL_aPP_REJ_COMMENT"].ToString().Trim(), "");
                    strBodyHTML.Append("<HTML>");
                    strBodyHTML.Append("DR NO:  " + dt.Rows[0]["DR_NO"].ToString().Trim() + "   " + dt.Rows[0]["CUSTOMER_NAME"].ToString().Trim());
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("DESIGN NO:  " + dt.Rows[0]["NEW_DESIGN_NO"].ToString().Trim() + "  ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("INTENAL APP/REJ:  " + dt.Rows[0]["DR_INTERNAL_aPP_REJ_CODE"].ToString().Trim() + "     ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("ADD TO COLLECTION:  " + dt.Rows[0]["ADD_tO_cOLLECTION"].ToString().Trim() + "     ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("SEND TO CUSTOMER:  " + dt.Rows[0]["SEND_tO_cUSTOMER"].ToString().Trim() + " ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("INTERNAL COMMENT:  " + dt.Rows[0]["DR_INTERNAL_aPP_REJ_COMMENT"].ToString().Trim() + "</FONT>");
                    msg.Body = msgbody + strBodyHTML.ToString();//
                    msg.Subject = dt.Rows[0]["internal_approval_subject"].ToString();
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
                    strBodyHTML.Append("INTERNAL APP/REJ:  " + dt.Rows[0]["DR_INTERNAL_aPP_REJ_CODE"].ToString().Trim() + "     ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("ADD TO COLLECTION:  " + dt.Rows[0]["ADD_tO_cOLLECTION"].ToString().Trim() + "     ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("SEND TO CUSTOMER:  " + dt.Rows[0]["SEND_tO_cUSTOMER"].ToString().Trim() + " ");
                    strBodyHTML.Append("<BR>");
                    strBodyHTML.Append("INTERNAL COMMENT:  " + dt.Rows[0]["DR_INTERNAL_aPP_REJ_COMMENT"].ToString().Trim() + "</FONT>");

                    msg.Subject = dt.Rows[0]["internal_approval_subject"].ToString();

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
                strResult = "F";// ex.Message.ToString();// "F";
                return strResult;
                //"Your message failed to send, please try again."
            }
        }
    }
}
