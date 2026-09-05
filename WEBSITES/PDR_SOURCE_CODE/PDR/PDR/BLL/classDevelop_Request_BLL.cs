using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
namespace PDR.BLL
{
    class classDevelop_Request_BLL
    {
        public decimal getYarnNetBal(string item_code)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();

            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_yarn_net_bal_select]");
            try
            {

                command1.Parameters.AddWithValue("@p_item_code", item_code);
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable ds = new DataTable();
                adapter.Fill(ds);

                db.closeDBConnection();
                return Convert.ToDecimal(ds.Rows[0]["net_bal_kg"]);
            }
            catch (Exception ex2)
            {
                string errormessage = ex2.Message.ToString();
                return 0;
            }
        }

        public string sendApprovalMailTest(DataTable dt,string sessionID)
        {
            string strResult;
            try
            {
                //Create the msg object to be sent
                StringBuilder strBodyHTML = new StringBuilder();
                DataRow r;
                String RequesterEmail;

                String PreparerEmail;

                r = dt.Rows[0];
                RequesterEmail = Convert.ToString(r["requester_email"]);
                PreparerEmail = Convert.ToString(r["preparer_email"]);

                MailAddress fromMailId = new MailAddress("wf@gemmaknits.com");
                MailAddress toMailId =  new MailAddress(RequesterEmail);
                //MailAddress toMailId = new MailAddress("suresh@geminitextile.com");
                MailAddress ccMailID = new MailAddress("suresh@geminitextile.com");


                MailMessage msg = new MailMessage(fromMailId, toMailId);
                msg.CC.Add(ccMailID);
                msg.CC.Add(PreparerEmail);
                msg.CC.Add("amorn@gemmaknits.com");
                //Add your email address to the recipients
                //Configure the address we are sending the mail from

                // -----------
                //                Font3 = "<font size=""" + "3" + """" + "face=""" + "Times" + """" + ">";
                //              Font2 = "<font size=""" + "2" + """" + "face=""" + "Times" + """" + ">";

                strBodyHTML.Append("<HTML>");
                strBodyHTML.Append("<H1> PDR </h1>");
                strBodyHTML.Append("<P>");
                //                strBodyHTML.Append("<H1>");
                //              strBodyHTML.Append(Font3);
               // strBodyHTML.Append("<a href=\"http://110.78.165.119:86/ui/PDR_Approval.aspx?pdrno=" + dt.Rows[0]["pdr_no"] + "&SessionID=" + sessionID +  "\">");
                strBodyHTML.Append("<a href=\"http://103.40.140.12:86/ui/PDR_Approval.aspx?pdrno=" + dt.Rows[0]["pdr_no"] + "&SessionID=" + sessionID + "\">");
                strBodyHTML.Append("No. " + dt.Rows[0]["pdr_no"] + "       ");
                strBodyHTML.Append("</a>");
                strBodyHTML.Append("<BR>");
                strBodyHTML.Append("Dt:  " + dt.Rows[0]["pdr_date"] + "     ");
                strBodyHTML.Append("<BR>");
                strBodyHTML.Append("For: " + dt.Rows[0]["customer_name"]);
                strBodyHTML.Append("<BR>");
                strBodyHTML.Append("Sales person: " + dt.Rows[0]["requested_by"] + "</FONT>");
                //              strBodyHTML.Append("</H1>");

                strBodyHTML.Append("<TABLE CELLSPACING=3 CELLPADING=1 ");

                msg.Subject = "PDR No." + dt.Rows[0]["pdr_no"] + " requires your approval";
                msg.Body = strBodyHTML.ToString();
                msg.IsBodyHtml = true;
                Attachment a = new Attachment((@"C:\Users\Kalaiarasi\Downloads\" + dt.Rows[0]["pdr_no"].ToString().Trim() + ".pdf"));
                // msg.Attachments.Add(a);
                // --------
                //Configure an SmtpClient to send the mail.
                SmtpClient client = new SmtpClient("61.19.245.242", 25);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = false;
                client.UseDefaultCredentials = false;

                //Setup credentials to login to our sender email address ("UserName", "Password")
                NetworkCredential credentials = new NetworkCredential("wf@gemmaknits.com", "wf@2016");
                client.Credentials = credentials;
                ////Send the msg
                client.Send(msg);
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

        public string sendApprovalMail(DataTable dt, string sessionID)
        {
            string strResult;
            try
            {
            //Create the msg object to be sent
            StringBuilder strBodyHTML = new StringBuilder();
            DataRow r;
            String RequesterEmail;

            String PreparerEmail;

            r = dt.Rows[0];
                RequesterEmail = Convert.ToString(r["requester_email"]);
                PreparerEmail = Convert.ToString(r["preparer_email"]);

                MailAddress fromMailId = new MailAddress("wf@gemmaknits.com");
                MailAddress toMailId = new MailAddress(RequesterEmail);
                MailAddress ccMailID = new MailAddress("suresh@geminitextile.com");
                //MailAddress toMailId = new MailAddress("kalai@eplt.in");
                //MailAddress ccMailID = new MailAddress("kalai@eplt.in");

                MailMessage msg = new MailMessage(fromMailId, toMailId);
                if (ccMailID.ToString() != "")
                {
                    msg.CC.Add(ccMailID);
                }
                if (PreparerEmail != "")
                {
                    msg.CC.Add(PreparerEmail);
                }
            msg.CC.Add("amorn@gemmaknits.com");
            //Add your email address to the recipients
            //Configure the address we are sending the mail from

            // -----------
            //                Font3 = "<font size=""" + "3" + """" + "face=""" + "Times" + """" + ">";
            //              Font2 = "<font size=""" + "2" + """" + "face=""" + "Times" + """" + ">";

            strBodyHTML.Append("<HTML>");
            strBodyHTML.Append("<H1> PDR </h1>");
            strBodyHTML.Append("<P>");
                //                strBodyHTML.Append("<H1>");
                //              strBodyHTML.Append(Font3);
                //strBodyHTML.Append("<a href=\"http://110.78.165.119:86/ui/PDR_Approval.aspx?pdrno=" + dt.Rows[0]["pdr_no"] + "&SessionID=" + sessionID + "&pagename=PDR_Approval"  + "\">");
                string strIPAddress = ConnectionDB.strLinkIPAddress;
                //strBodyHTML.Append("<a href=\"http://103.40.140.12:86/ui/PDR_Approval.aspx?pdrno=" + dt.Rows[0]["pdr_no"] + "&SessionID=" + sessionID + "&pagename=PDR_Approval" + "\">");
                strBodyHTML.Append("<a href=\"http://" + strIPAddress + "/ui/PDR_Approval.aspx?pdrno=" + dt.Rows[0]["pdr_no"] + "&SessionID=" + sessionID + "&pagename=PDR_Approval" + "\">");
                strBodyHTML.Append("No. " + dt.Rows[0]["pdr_no"] + "       ");
            strBodyHTML.Append("</a>");
            strBodyHTML.Append("<BR>");
            strBodyHTML.Append("Dt:  " + dt.Rows[0]["pdr_date"] + "     ");
            strBodyHTML.Append("<BR>");
            strBodyHTML.Append("For: " + dt.Rows[0]["customer_name"]);
            strBodyHTML.Append("<BR>");
            strBodyHTML.Append("Sales person: " + dt.Rows[0]["requested_by"] + "</FONT>");
                //              strBodyHTML.Append("</H1>");
              
            strBodyHTML.Append("<TABLE CELLSPACING=3 CELLPADING=1 ");

                //msg.Subject = "PDR No." + dt.Rows[0]["pdr_no"] + " requires your approval";
                msg.Subject = "PDR No." + dt.Rows[0]["pdr_no"] + " requires your approval";
                msg.Body = strBodyHTML.ToString();
            msg.IsBodyHtml = true;
                // string filename = "//172.16.3.4/pdr_files/pdrdoc/PDR" + dt.Rows[0]["pdr_no"].ToString().Trim() + ".pdf";
                //  Attachment attchfile = new Attachment((filename));
                //  attchfile.Name = "PDR" + dt.Rows[0]["pdr_no"].ToString().Trim() + ".pdf";
                //   msg.Attachments.Add(attchfile);
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
                strResult = ex.Message;// "F";
                return strResult;
                //"Your message failed to send, please try again."
            }
}


        public string sendInternalApproveMail(DataTable dt, string attachmentIDs,string sessionID)
        {
            string strResult;
            try
            {
                //Create the msg object to be sent
                StringBuilder strBodyHTML = new StringBuilder();
                DataRow r;

                r = dt.Rows[0];
                string toEmailID= "kalai@eplt.in";
                string RequesterEmail;
                string CCmailList;
                MailAddress fromMailId = new MailAddress("wf@gemmaknits.com");
                MailAddress requestormail;
                // MailAddress toMailId;               
                toEmailID = Convert.ToString(r["requester_email"]) + "," + Convert.ToString(r["preparer_email"]) + "," + Convert.ToString(r["sample_room_Email"]);
                CCmailList =  "kalai@eplt.in" + "," + "suresh@geminitextile.com";
                CCmailList = Convert.ToString(r["rd_staff"]) + "," + Convert.ToString(r["factory_mgr"]) + "," + Convert.ToString(r["md"]);
                string[] CCId = CCmailList.Split(',');
                MailAddress toMailId = new MailAddress(Convert.ToString(r["sample_room_Email"]));
                // MailAddress toMailId = new MailAddress(toEmailID);
                string[] TOID = toEmailID.Split(',');
                MailMessage msg = new MailMessage(fromMailId, toMailId);
                foreach (string toid in TOID)
                {
                    msg.To.Add(new MailAddress(toid)); //Adding Multiple CC email Id
                }
                msg.To.Add(toMailId);               
                foreach (string CCEmail in CCId)
                {
                    msg.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                }

                // msg.CC.Add("kalai@eplt.in");
                //Add your email address to the recipients
                //Configure the address we are sending the mail from

                // -----------
                //                Font3 = "<font size=""" + "3" + """" + "face=""" + "Times" + """" + ">";
                //              Font2 = "<font size=""" + "2" + """" + "face=""" + "Times" + """" + ">";

                strBodyHTML.Append("<HTML>");
                strBodyHTML.Append("<H1> PDR </h1>");
                strBodyHTML.Append("<P>");
                //                strBodyHTML.Append("<H1>");Append(Font3);
                string strIPAddress = ConnectionDB.strLinkIPAddress;
               // strBodyHTML.Append("<a href=\"http://103.40.140.12:86/ui/Develop_Request.aspx?pdrno=" + dt.Rows[0]["pdr_no"] + "&SessionID=" + sessionID + "&pagename=Develop_Request" + "\">");
                strBodyHTML.Append("<a href=\"http://" + strIPAddress  + "/ui/Develop_Request.aspx?pdrno=" + dt.Rows[0]["pdr_no"] + "&SessionID=" + sessionID + "&pagename=Develop_Request" + "\">");
                strBodyHTML.Append("DR No. " + dt.Rows[0]["dr_No"] + "       ");
                strBodyHTML.Append("</a>");
                strBodyHTML.Append("<BR>");
                strBodyHTML.Append("INTERNAL APP/REJ:  " + dt.Rows[0]["pdr_date"] + "     ");
                strBodyHTML.Append("<BR>");
                strBodyHTML.Append("ADD TO COLLECTION:  " + dt.Rows[0]["pdr_date"] + "     ");
                strBodyHTML.Append("<BR>");
                strBodyHTML.Append("SEND TO CUSTOMER:  " + dt.Rows[0]["pdr_date"] + "     ");
                strBodyHTML.Append("<BR>");
                strBodyHTML.Append("INTERNAL COMMENT:  " + dt.Rows[0]["pdr_date"] + "     ");
                strBodyHTML.Append("<BR>");
               //              strBodyHTML.Append("</H1>");

                // strBodyHTML.Append("<TABLE CELLSPACING=3 CELLPADING=1 >");
              
                    msg.Subject =  dt.Rows[0]["INTERNAL_APPROVAL_SUBJECT"].ToString() ;
                
                msg.Body = strBodyHTML.ToString();
                msg.IsBodyHtml = true;
                //string attachmentFilename = "//172.16.3.4/pdr_files/ANALYZE/" + attachmentIDs;// "//172.16.3.4/pdr_files/pdrdoc/PDR" + dt.Rows[0]["pdr_no"].ToString().Trim() + ".pdf";
                if (attachmentIDs.ToString().Trim() != null && attachmentIDs.ToString().Trim() != "")
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


        public string sendAvailMail(DataTable dt,string toEmailID,string ccEmailID,string subject,string body,string sessionID)
        {
            string strResult;
            try
            {
                //Create the msg object to be sent
                StringBuilder strBodyHTML = new StringBuilder();
                DataRow r;
                r = dt.Rows[0];

                MailAddress fromMailId = new MailAddress("wf@gemmaknits.com");                
                //MailAddress toMailId;
                MailAddress ccMailID;
                MailAddress toMailId = new MailAddress(Convert.ToString(r["yarn_avail_alert_email"]));
                if (toEmailID != "")
                {
                    toMailId = new MailAddress(toEmailID);
                }
                else
                {
                    toMailId = new MailAddress("kalai@eplt.in");
                }

                // MailAddress ccMailID = new MailAddress("suresh@geminitextile.com");
                if (ccEmailID != "")
                {
                    ccMailID = new MailAddress(ccEmailID);
                }
                else
                {
                    ccMailID = new MailAddress("kalai@eplt.in");
                }
                MailMessage msg = new MailMessage(fromMailId, toMailId);
                msg.To.Add(toMailId);
                msg.CC.Add(ccMailID);
                msg.CC.Add("suresh@geminitextile.com");
                //Add your email address to the recipients
                //Configure the address we are sending the mail from

                // -----------
                //////strBodyHTML.Append("<HTML>");
                //////strBodyHTML.Append("<H1> PDR </h1>");
                //////strBodyHTML.Append("<P>");
                ////// strBodyHTML.Append("<a href=\"http://110.78.165.119:86/ui/PDR_Approval.aspx?pdrno=" + dt.Rows[0]["pdr_no"] + "&SessionID=" + sessionID + "\">");
                //////strBodyHTML.Append("No. " + dt.Rows[0]["pdr_no"] + "       ");
                //////strBodyHTML.Append("</a>");
                //////strBodyHTML.Append("<BR>");
                //////strBodyHTML.Append("Dt:  " + dt.Rows[0]["pdr_date"] + "     ");
                //////strBodyHTML.Append("<BR>");
                //////strBodyHTML.Append("For: " + dt.Rows[0]["customer_name"]);
                //////strBodyHTML.Append("<BR>");
                //////strBodyHTML.Append("Sales person: " + dt.Rows[0]["requested_by"] + "</FONT>");

                // strBodyHTML.Append("<TABLE CELLSPACING=3 CELLPADING=1 >");
                if (subject != "")
                {
                    msg.Subject = subject;
                }
                else
                {
                    msg.Subject = "PDR No." + dt.Rows[0]["pdr_no"] + " have shortage of yarn";
                }
                if (body != "")
                {
                    string msgbody = body.ToString().Replace("\r\n", "<br>");
                    // string searchFor = "No.";
                    msgbody = msgbody.Replace("PDR", "");
                    msgbody = msgbody.Replace("No. " + dt.Rows[0]["pdr_no"].ToString().Trim(),"");
                    msgbody = msgbody.Replace("Dt:  " + dt.Rows[0]["pdr_date"].ToString().Trim(), "");
                    msgbody = msgbody.Replace("For: " + dt.Rows[0]["customer_name"], "");
                    msgbody = msgbody.Replace("Sales person: " + dt.Rows[0]["requested_by"], "");
                    //  Join the remaining lines back into a single string
                    strBodyHTML.Append("<HTML>");
                    strBodyHTML.Append("<H1> PDR </h1>");
                    string strIPAddress = ConnectionDB.strLinkIPAddress;
                    // strBodyHTML.Append("<a href=\"http://103.40.140.12:86/ui/develop_request.aspx?pdrno=" + dt.Rows[0]["pdr_no"] + "&SessionID=" + sessionID + "&pagename=develop_request" + " \">");
                    strBodyHTML.Append("<a href=\"http://" + strIPAddress + "/ui/develop_request.aspx?pdrno=" + dt.Rows[0]["pdr_no"] + "&SessionID=" + sessionID + "&pagename=develop_request" + " \">");
                    strBodyHTML.Append("No. " + dt.Rows[0]["pdr_no"] + "       ");
                    strBodyHTML.Append("</a>");
                    strBodyHTML.Append("<br>");
                    strBodyHTML.Append("dt:  " + dt.Rows[0]["pdr_date"] + "     ");
                    strBodyHTML.Append("<br>");
                    strBodyHTML.Append("for: " + dt.Rows[0]["customer_name"]);
                    strBodyHTML.Append("<br>");
                    strBodyHTML.Append("sales person: " + dt.Rows[0]["requested_by"]);
                   // strBodyHTML.Append(msgbody + "<br><HTML>");
                    // msgbody = msgbody(.Replace("PDR", strBodyHTML.Append("<a href=\"http://110.78.165.119:86/ui/PDR_Approval.aspx?pdrno=" + dt.Rows[0]["pdr_no"] + "&SessionID=" + sessionID + "\">");
                    msg.Body = msgbody +  strBodyHTML.ToString();// + msgbody;// body.ToString().Replace("\r\n","<br>") + "\r\n" + strBodyHTML.ToString();
                }
                else
                {
                    msg.Body = strBodyHTML.ToString();
                }
              
                msg.IsBodyHtml = true;
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
                strResult = ex.Message;// "F";
                return strResult;
                //"Your message failed to send, please try again."
            }
        }
        public DataTable LoadPDRList(string p_pdr_no)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_new_develop_req_select]");

            try
            {
                if (p_pdr_no == null || p_pdr_no == "")
                {
                    command1.Parameters.AddWithValue("@p_pdr_no", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_pdr_no", (p_pdr_no.Trim()));
                }
                DataTable orderissuedesign = db.ExecuteCommandAndGetDataTable(command1);
                db.closeDBConnection();
                return orderissuedesign;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable LoadPDRRec(string p_pdr_no)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_new_develop_req_select]");

            try
            {
                if (p_pdr_no == null || p_pdr_no == "")
                {
                    command1.Parameters.AddWithValue("@p_pdr_no", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_pdr_no", (p_pdr_no.Trim()));
                }
                DataTable orderissuedesign = db.ExecuteCommandAndGetDataTable(command1);
                db.closeDBConnection();
                return orderissuedesign;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }
        public DataSet Load_Print_PDRList(string p_pdr_no)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_new_develop_req_print]");

            try
            {

                if (p_pdr_no == null || p_pdr_no == "")
                {
                    command1.Parameters.AddWithValue("@p_pdr_no", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_pdr_no", (p_pdr_no.Trim()));
                }
                DataSet developrequest = db.ExecuteCommandAndGetDataSet(command1);
                db.closeDBConnection();
                return developrequest;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }
        public DataTable CancelPDRNO(string p_pdr_new_develop_req_id, string p_userid)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_cancel]");

            try
            {
                if (p_pdr_new_develop_req_id == null || p_pdr_new_develop_req_id == "")
                {
                    command1.Parameters.AddWithValue("@p_pdr_new_develop_req_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_pdr_new_develop_req_id", Convert.ToInt64(p_pdr_new_develop_req_id.Trim()));
                }
                if (p_userid == null || p_userid == "")
                {
                    command1.Parameters.AddWithValue("@p_userid", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_userid", (p_userid.Trim()));
                }
                DataTable orderissuedesign = db.ExecuteCommandAndGetDataTable(command1);
                db.closeDBConnection();
                return orderissuedesign;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable UpdateDevelopREquest(string p_pdr_new_develop_req_id, string p_pdr_no, string p_pdr_date, string p_design_no, string p_gauge, string p_custcd,
                                        string p_requested_by, string p_expected_price, string p_uom, string p_expected_date, string p_product_application_id,
                                        string p_development_type_id, string p_development_type_remark, string p_itcatid, string p_itsubcatid, string p_itgroupid,
                                        string p_itsubid, string p_ittypeid, string p_itsubid2, string p_standard_aatcc, string p_standard_astm,
                                        string p_standard_iso, string p_standard_jis, string p_standard_ms, string p_standard_hm,
                                         string p_standard_vss,
                                         string p_standard_customer, string p_follow_customer_spec, string p_yarn_face1, string p_yarn_face2,
                                        string p_yarn_face3, string p_yarn_face4, string p_yarn_face5, string p_yarn_face6, string p_composition1_percent,
                                        string p_composition2_percent, string p_composition3_percent,
                                        string p_composition4_percent, string p_composition5_percent, string p_composition6_percent,
                                        string p_composition1_name, string p_composition2_name, string p_composition3_name,
                                        string p_composition4_name, string p_composition5_name, string p_composition6_name,
                                        string p_weight_sqm, string p_weight_sqm_tolerance, string p_weight_sqm_remark,
                                        string p_width, string p_width_tolerance, string p_elongation, string p_elongation_l, string p_elongation_w, string p_elongation_tolerance,
                                        string p_modulus, string p_modulus_l, string p_modulus_w, string p_modulus_tolerance, string p_stretch_l,
                                        string p_stretch_w, string p_stretch_tolerance, string p_shrinkage_l, string p_shrinkage_w, string p_shrinkage_tolerance,
                                        string p_piling, string p_pilling_remark, string p_snagging, string p_snagging_remark, string p_color_name,
                                        string p_dye_finishing_formula_id, string p_dye_remark, string p_hanger, string p_yardage, string p_require_tag,
                                        string p_machine_dense, string @p_yarn_available, string p_yarn_available_date, string p_knitting_appointment,
                                        string p_offer_price, string p_is_consideration, string p_expected_finished_date, string p_spec_master_date,
                                        string p_qa_report_date, string p_remark, string p_user_session_id, string p_doc_attachment,
                                        string p_endbuyercd, string p_priority_id, string p_priority_comment, string p_price_curr, string p_itcd1,
                                        string p_itcd2, string p_itcd3, string p_itcd4, string p_itcd5, string p_itcd6,
                                        decimal p_avail1,
                                        decimal p_avail2,
                                        decimal p_avail3,
                                        decimal p_avail4,
                                        decimal p_avail5,
                                        decimal p_avail6,decimal p_kg_per_finished_roll,decimal p_no_of_finished_rolls,
                                        string p_yarn_shortage1, string p_yarn_shortage2, string p_yarn_shortage3, string p_yarn_shortage4, 
                                        string p_yarn_shortage5, string p_yarn_shortage6, string p_new_yarn1, string p_new_yarn2,
                                        string p_new_yarn3, string p_new_yarn4,  string p_new_yarn5, string p_new_yarn6,string p_test_method_id,
                                        string p_shoe_style,string p_shoe_size_id,string p_shoe_gender_id,string p_shoe_length,string p_shoe_width,
                                        string p_pallet_pattern_no,string p_product_pattern_no,string p_shoe_require_laser_cut,string p_laser_cut_with_pattern,
                                        string p_shoe_pairs_required,
                                        string p_repeat_per_roll)
            
        {
            //ConnectionDB db = ConnectionDB.getDbInstance();
            //db.ConnectToDB();
            
            string connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
            SqlConnection cnn = new SqlConnection(connectionstring);
            SqlTransaction transaction;
            cnn.Open();
            transaction = cnn.BeginTransaction();
            // SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_new_develop_req_update]");
            SqlCommand command1 = new SqlCommand("[pdr].[p_pdr_new_develop_req_update]", cnn);
            command1.CommandType = CommandType.StoredProcedure; // new
            command1.Parameters.Clear(); // new
            DateTime dt = Convert.ToDateTime(p_pdr_date.ToString().Trim());
            string dt1 = dt.ToString().Substring(0, 10);
            try
            {
                if (p_pdr_new_develop_req_id == null || p_pdr_new_develop_req_id == "")
                {
                    command1.Parameters.AddWithValue("@p_pdr_new_develop_req_id", "-1");
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_pdr_new_develop_req_id", Convert.ToInt32(p_pdr_new_develop_req_id.Trim()));
                }
                if (p_pdr_no == null || p_pdr_no == "" || p_pdr_no == "NEW")
                {
                    command1.Parameters.AddWithValue("@p_pdr_no", "NEW");
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_pdr_no", (p_pdr_no.Trim()));
                }
                if (p_pdr_date == null || p_pdr_date == "")
                {
                    command1.Parameters.AddWithValue("@p_pdr_date", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_pdr_date", p_pdr_date);
                }

                if (p_design_no == null || p_design_no == "")
                {
                    command1.Parameters.AddWithValue("@p_design_no", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_design_no", (p_design_no.Trim()));
                }
                if (p_gauge == null || p_gauge == "")
                {
                    command1.Parameters.AddWithValue("@p_gauge", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_gauge", (p_gauge.Trim()));
                }

                if (p_custcd == null || p_custcd == "")
                {
                    command1.Parameters.AddWithValue("@p_custcd", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_custcd", (p_custcd.Trim()));
                }

                if (p_requested_by == null || p_requested_by == "")
                {
                    command1.Parameters.AddWithValue("@p_requested_by", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_requested_by", (p_requested_by.Trim()));
                }
                if (p_expected_price == null || p_expected_price == "")
                {
                    command1.Parameters.AddWithValue("@p_expected_price", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_expected_price", Convert.ToDouble(p_expected_price.Trim()));
                }

                if (p_uom == null || p_uom == "")
                {
                    command1.Parameters.AddWithValue("@p_uom", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_uom", (p_uom.Trim()));
                }
                if (p_expected_date == null || p_expected_date == "")
                {
                    command1.Parameters.AddWithValue("@p_expected_date", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_expected_date", (p_expected_date.Trim()));
                }
                if (p_product_application_id == null || p_product_application_id == "")
                {
                    command1.Parameters.AddWithValue("@p_product_application_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_product_application_id", (p_product_application_id.Trim()));
                }
                if (p_development_type_id == null || p_development_type_id == "")
                {
                    command1.Parameters.AddWithValue("@p_development_type_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_development_type_id", Convert.ToInt32(p_development_type_id.Trim()));
                }
                if (p_development_type_remark == null || p_development_type_remark == "")
                {
                    command1.Parameters.AddWithValue("@p_development_type_remark", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_development_type_remark", (p_development_type_remark.Trim()));
                }
                if (p_itcatid == null || p_itcatid == "")
                {
                    command1.Parameters.AddWithValue("@p_itcatid", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_itcatid", Convert.ToInt32(p_itcatid.Trim()));
                }
                if (p_itsubcatid == null || p_itsubcatid == "")
                {
                    command1.Parameters.AddWithValue("@p_itsubcatid", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_itsubcatid", Convert.ToInt32(p_itsubcatid.Trim()));
                }
                if (p_itgroupid == null || p_itgroupid == "")
                {
                    command1.Parameters.AddWithValue("@p_itgroupid", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_itgroupid", Convert.ToInt32(p_itgroupid.Trim()));
                }
                if (p_itsubid == null || p_itsubid == "")
                {
                    command1.Parameters.AddWithValue("@p_itsubid", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_itsubid", Convert.ToInt32(p_itsubid.Trim()));
                }
                if (p_ittypeid == null || p_ittypeid == "")
                {
                    command1.Parameters.AddWithValue("@p_ittypeid", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_ittypeid", Convert.ToInt32(p_ittypeid.Trim()));
                }
                if (p_itsubid2 == null || p_itsubid2 == "")
                {
                    command1.Parameters.AddWithValue("@p_itsubid2", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_itsubid2", Convert.ToInt32(p_itsubid2.Trim()));
                }
                if (p_standard_iso == null || p_standard_iso == "")
                {
                    command1.Parameters.AddWithValue("@p_standard_iso", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_standard_iso", (p_standard_iso.Trim()));
                }
                if (p_standard_aatcc == null || p_standard_aatcc == "")
                {
                    command1.Parameters.AddWithValue("@p_standard_aatcc", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_standard_aatcc", (p_standard_aatcc.Trim()));
                }
                if (p_standard_astm == null || p_standard_astm == "")
                {
                    command1.Parameters.AddWithValue("@p_standard_astm", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_standard_astm", (p_standard_astm.Trim()));
                }
                if (p_standard_jis == null || p_standard_jis == "")
                {
                    command1.Parameters.AddWithValue("@p_standard_jis", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_standard_jis", (p_standard_jis.Trim()));
                }

                if (p_standard_ms == null || p_standard_ms == "")
                {
                    command1.Parameters.AddWithValue("@p_standard_ms", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_standard_ms", (p_standard_ms.Trim()));
                }
                if (p_standard_vss == null || p_standard_vss == "")
                {
                    command1.Parameters.AddWithValue("@p_standard_vss", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_standard_vss", (p_standard_vss.Trim()));
                }
                if (p_standard_hm == null || p_standard_hm == "")
                {
                    command1.Parameters.AddWithValue("@p_standard_hm", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_standard_hm", (p_standard_hm.Trim()));
                }

                if (p_standard_customer == null || p_standard_customer == "")
                {
                    command1.Parameters.AddWithValue("@p_standard_customer", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_standard_customer", (p_standard_customer.Trim()));
                }
                if (p_follow_customer_spec == null || p_follow_customer_spec == "")
                {
                    command1.Parameters.AddWithValue("@p_follow_customer_spec", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_follow_customer_spec", (p_follow_customer_spec.Trim()));
                }
                if (p_yarn_face1 == null || p_yarn_face1 == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_face1", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_face1", (p_yarn_face1.Trim()));
                }
                if (p_yarn_face2 == null || p_yarn_face2 == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_face2", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_face2", (p_yarn_face2.Trim()));
                }
                if (p_yarn_face3 == null || p_yarn_face3 == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_face3", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_face3", (p_yarn_face3.Trim()));
                }
                if (p_yarn_face4 == null || p_yarn_face4 == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_face4", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_face4", (p_yarn_face4.Trim()));
                }
                if (p_yarn_face5 == null || p_yarn_face5 == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_face5", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_face5", (p_yarn_face5.Trim()));
                }
                if (p_yarn_face6 == null || p_yarn_face6 == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_face6", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_face6", (p_yarn_face6.Trim()));
                }
                if (p_composition1_percent == null || p_composition1_percent == "")
                {
                    command1.Parameters.AddWithValue("@p_composition1_percent", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_composition1_percent", Convert.ToDouble(p_composition1_percent.Trim()));
                }
                if (p_composition2_percent == null || p_composition2_percent == "")
                {
                    command1.Parameters.AddWithValue("@p_composition2_percent", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_composition2_percent", Convert.ToDouble(p_composition2_percent.Trim()));
                }
                if (p_composition3_percent == null || p_composition3_percent == "")
                {
                    command1.Parameters.AddWithValue("@p_composition3_percent", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_composition3_percent", Convert.ToDouble(p_composition3_percent.Trim()));
                }
                if (p_composition4_percent == null || p_composition4_percent == "")
                {
                    command1.Parameters.AddWithValue("@p_composition4_percent", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_composition4_percent", Convert.ToDouble(p_composition4_percent.Trim()));
                }
                if (p_composition5_percent == null || p_composition5_percent == "")
                {
                    command1.Parameters.AddWithValue("@p_composition5_percent", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_composition5_percent", Convert.ToDouble(p_composition5_percent.Trim()));
                }
                if (p_composition6_percent == null || p_composition6_percent == "")
                {
                    command1.Parameters.AddWithValue("@p_composition6_percent", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_composition6_percent", Convert.ToDouble(p_composition6_percent.Trim()));
                }
                if (p_composition1_name == null || p_composition1_name == "")
                {
                    command1.Parameters.AddWithValue("@p_composition1_name", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_composition1_name", (p_composition1_name.Trim()));
                }
                if (p_composition2_name == null || p_composition2_name == "")
                {
                    command1.Parameters.AddWithValue("@p_composition2_name", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_composition2_name", (p_composition2_name.Trim()));
                }
                if (p_composition3_name == null || p_composition3_name == "")
                {
                    command1.Parameters.AddWithValue("@p_composition3_name", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_composition3_name", (p_composition3_name.Trim()));
                }
                if (p_composition4_name == null || p_composition4_name == "")
                {
                    command1.Parameters.AddWithValue("@p_composition4_name", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_composition4_name", (p_composition4_name.Trim()));
                }
                if (p_composition5_name == null || p_composition5_name == "")
                {
                    command1.Parameters.AddWithValue("@p_composition5_name", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_composition5_name", (p_composition5_name.Trim()));
                }
                if (p_composition6_name == null || p_composition6_name == "")
                {
                    command1.Parameters.AddWithValue("@p_composition6_name", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_composition6_name", (p_composition6_name.Trim()));
                }
                if (p_weight_sqm == null || p_weight_sqm == "")
                {
                    command1.Parameters.AddWithValue("@p_weight_sqm", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_weight_sqm", Convert.ToDouble(p_weight_sqm.Trim()));
                }
                if (p_weight_sqm_tolerance == null || p_weight_sqm_tolerance == "")
                {
                    command1.Parameters.AddWithValue("@p_weight_sqm_tolerance", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_weight_sqm_tolerance", Convert.ToDouble(p_weight_sqm_tolerance.Trim()));
                }
                if (p_weight_sqm_remark == null || p_weight_sqm_remark == "")
                {
                    command1.Parameters.AddWithValue("@p_weight_sqm_remark", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_weight_sqm_remark", (p_weight_sqm_remark.Trim()));
                }
                if (p_width == null || p_width == "")
                {
                    command1.Parameters.AddWithValue("@p_width", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_width", Convert.ToDouble(p_width.Trim()));
                }
                if (p_width_tolerance == null || p_width_tolerance == "")
                {
                    command1.Parameters.AddWithValue("@p_width_tolerance", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_width_tolerance", Convert.ToDouble(p_width_tolerance.Trim()));
                }
                if (p_elongation == null || p_elongation == "")
                {
                    command1.Parameters.AddWithValue("@p_elongation", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_elongation", Convert.ToDouble(p_elongation.Trim()));
                }
                if (p_elongation_l == null || p_elongation_l == "")
                {
                    command1.Parameters.AddWithValue("@p_elongation_l", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_elongation_l", Convert.ToDouble(p_elongation_l.Trim()));
                }
                if (p_elongation_w == null || p_elongation_w == "")
                {
                    command1.Parameters.AddWithValue("@p_elongation_w", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_elongation_w", Convert.ToDouble(p_elongation_w.Trim()));
                }
                if (p_elongation_tolerance == null || p_elongation_tolerance == "")
                {
                    command1.Parameters.AddWithValue("@p_elongation_tolerance", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_elongation_tolerance", Convert.ToDouble(p_elongation_tolerance.Trim()));
                }
                if (p_modulus == null || p_modulus == "")
                {
                    command1.Parameters.AddWithValue("@p_modulus", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_modulus", Convert.ToDouble(p_modulus.Trim()));
                }
                if (p_modulus_l == null || p_modulus_l == "")
                {
                    command1.Parameters.AddWithValue("@p_modulus_l", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_modulus_l", Convert.ToDouble(p_modulus_l.Trim()));
                }
                if (p_modulus_w == null || p_modulus_w == "")
                {
                    command1.Parameters.AddWithValue("@p_modulus_w", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_modulus_w", Convert.ToDouble(p_modulus_w.Trim()));
                }
                if (p_modulus_tolerance == null || p_modulus_tolerance == "")
                {
                    command1.Parameters.AddWithValue("@p_modulus_tolerance", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_modulus_tolerance", Convert.ToDouble(p_modulus_tolerance.Trim()));
                }
                if (p_stretch_l == null || p_stretch_l == "")
                {
                    command1.Parameters.AddWithValue("@p_stretch_l", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_stretch_l", Convert.ToDouble(p_stretch_l.Trim()));
                }
                if (p_stretch_w == null || p_stretch_w == "")
                {
                    command1.Parameters.AddWithValue("@p_stretch_w", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_stretch_w", Convert.ToDouble(p_stretch_w.Trim()));
                }
                if (p_stretch_tolerance == null || p_stretch_tolerance == "")
                {
                    command1.Parameters.AddWithValue("@p_stretch_tolerance", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_stretch_tolerance", Convert.ToDouble(p_stretch_tolerance.Trim()));
                }
                if (p_shrinkage_l == null || p_shrinkage_l == "")
                {
                    command1.Parameters.AddWithValue("@p_shrinkage_l", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_shrinkage_l", Convert.ToDouble(p_shrinkage_l.Trim()));
                }
                if (p_shrinkage_w == null || p_shrinkage_w == "")
                {
                    command1.Parameters.AddWithValue("@p_shrinkage_w", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_shrinkage_w", Convert.ToDouble(p_shrinkage_w.Trim()));
                }
                if (p_shrinkage_tolerance == null || p_shrinkage_tolerance == "")
                {
                    command1.Parameters.AddWithValue("@p_shrinkage_tolerance", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_shrinkage_tolerance", Convert.ToDouble(p_shrinkage_tolerance.Trim()));
                }
                if (p_piling == null || p_piling == "")
                {
                    command1.Parameters.AddWithValue("@p_piling", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_piling", (p_piling.Trim()));
                }
                //if (p_piling == null || p_piling == "")
                //{
                //    command1.Parameters.AddWithValue("@p_piling", DBNull.Value);
                //}
                //else
                //{
                //    command1.Parameters.AddWithValue("@p_piling", (p_piling.Trim()));
                //}

                if (p_pilling_remark == null || p_pilling_remark == "")
                {
                    command1.Parameters.AddWithValue("@p_pilling_remark", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_pilling_remark", (p_pilling_remark.Trim()));
                }
                if (p_snagging == null || p_snagging == "")
                {
                    command1.Parameters.AddWithValue("@p_snagging", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_snagging", (p_snagging.Trim()));
                }
                if (p_snagging_remark == null || p_snagging_remark == "")
                {
                    command1.Parameters.AddWithValue("@p_snagging_remark", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_snagging_remark", (p_snagging_remark.Trim()));
                }
                if (p_color_name == null || p_color_name == "")
                {
                    command1.Parameters.AddWithValue("@p_color_name", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_color_name", (p_color_name.Trim()));
                }
                if (p_dye_finishing_formula_id == null || p_dye_finishing_formula_id == "")
                {
                    command1.Parameters.AddWithValue("@p_dye_finishing_formula_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_dye_finishing_formula_id", Convert.ToInt32(p_dye_finishing_formula_id.Trim()));
                }
                if (p_dye_remark == null || p_dye_remark == "")
                {
                    command1.Parameters.AddWithValue("@p_dye_remark", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_dye_remark", (p_dye_remark.Trim()));
                }
                if (p_hanger == null || p_hanger == "")
                {
                    command1.Parameters.AddWithValue("@p_hanger", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_hanger", (p_hanger.Trim()));
                }
                if (p_yardage == null || p_yardage == "")
                {
                    command1.Parameters.AddWithValue("@p_yardage", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yardage", Convert.ToDouble(p_yardage.Trim()));
                }
                if (p_require_tag == null || p_require_tag == "")
                {
                    command1.Parameters.AddWithValue("@p_require_tag", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_require_tag", (p_require_tag.Trim()));
                }
                if (p_machine_dense == null || p_machine_dense == "")
                {
                    command1.Parameters.AddWithValue("@p_machine_dense", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_machine_dense", (p_machine_dense.Trim()));
                }
                if (p_yarn_available == null || p_yarn_available == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_available", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_available", (p_yarn_available.Trim()));
                }
                if (p_yarn_available_date == null || p_yarn_available_date == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_available_date", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_available_date", Convert.ToDateTime(p_yarn_available_date.Trim()));
                }
                if (p_knitting_appointment == null || p_knitting_appointment == "")
                {
                    command1.Parameters.AddWithValue("@p_knitting_appointment", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_knitting_appointment", Convert.ToDateTime(p_knitting_appointment.Trim()));
                }
                if (p_offer_price == null || p_offer_price == "")
                {
                    command1.Parameters.AddWithValue("@p_offer_price", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_offer_price", Convert.ToDouble(p_offer_price.Trim()));
                }

                if (p_is_consideration == null || p_is_consideration == "")
                {
                    command1.Parameters.AddWithValue("@p_is_consideration", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_is_consideration", (p_is_consideration.Trim()));
                }

                if (p_expected_finished_date == null || p_expected_finished_date == "")
                {
                    command1.Parameters.AddWithValue("@p_expected_finished_date", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_expected_finished_date", Convert.ToDateTime(p_expected_finished_date.Trim()));
                }
                if (p_spec_master_date == null || p_spec_master_date == "")
                {
                    command1.Parameters.AddWithValue("@p_spec_master_date", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_spec_master_date", Convert.ToDateTime(p_spec_master_date.Trim()));
                }
                if (p_qa_report_date == null || p_qa_report_date == "")
                {
                    command1.Parameters.AddWithValue("@p_qa_report_date", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_qa_report_date", Convert.ToDateTime(p_qa_report_date.Trim()));
                }
                if (p_remark == null || p_remark == "")
                {
                    command1.Parameters.AddWithValue("@p_remark", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_remark", (p_remark.Trim()));
                }
                if (p_user_session_id == null || p_user_session_id == "")
                {
                    command1.Parameters.AddWithValue("@p_user_session_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_user_session_id", Convert.ToInt32(p_user_session_id.Trim()));
                }
                if (p_doc_attachment == null || p_doc_attachment == "")
                {
                    command1.Parameters.AddWithValue("@p_doc_attachment", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_doc_attachment", (p_doc_attachment.Trim()));
                }

                if (p_endbuyercd == null || p_endbuyercd == "")
                {
                    command1.Parameters.AddWithValue("@p_endbuyerid", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_endbuyerid", Convert.ToInt64(p_endbuyercd.Trim()));
                }
                if (p_priority_id == null || p_priority_id == "")
                {
                    command1.Parameters.AddWithValue("@p_priority_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_priority_id", Convert.ToInt64(p_priority_id.Trim()));
                }
                if (p_priority_comment == null || p_priority_comment == "")
                {
                    command1.Parameters.AddWithValue("@p_priority_comment", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_priority_comment", (p_priority_comment.Trim()));
                }
                if (p_price_curr == null || p_price_curr == "")
                {
                    command1.Parameters.AddWithValue("@p_price_curr", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_price_curr", (p_price_curr.Trim()));
                }
                if (p_itcd1 == null || p_itcd1 == "")
                {
                    command1.Parameters.AddWithValue("@p_itcd1", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_itcd1", (p_itcd1.Trim()));
                }
                if (p_itcd2 == null || p_itcd2 == "")
                {
                    command1.Parameters.AddWithValue("@p_itcd2", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_itcd2", (p_itcd2.Trim()));
                }
                if (p_itcd3 == null || p_itcd3 == "")
                {
                    command1.Parameters.AddWithValue("@p_itcd3", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_itcd3", (p_itcd3.Trim()));
                }
                if (p_itcd4 == null || p_itcd4 == "")
                {
                    command1.Parameters.AddWithValue("@p_itcd4", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_itcd4", (p_itcd4.Trim()));
                }
                if (p_itcd5 == null || p_itcd5 == "")
                {
                    command1.Parameters.AddWithValue("@p_itcd5", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_itcd5", (p_itcd5.Trim()));
                }
                if (p_itcd6 == null || p_itcd6 == "")
                {
                    command1.Parameters.AddWithValue("@p_itcd6", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_itcd6", (p_itcd6.Trim()));
                }

                if (p_avail1 == 0)
                {
                    command1.Parameters.AddWithValue("@p_yarn_avail_qty1", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_avail_qty1", p_avail1);
                }

                if (p_avail2 == 0)
                {
                    command1.Parameters.AddWithValue("@p_yarn_avail_qty2", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_avail_qty2", p_avail2);
                }
                if (p_avail3 == 0)
                {
                    command1.Parameters.AddWithValue("@p_yarn_avail_qty3", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_avail_qty3", p_avail3);
                }
                if (p_avail4 == 0)
                {
                    command1.Parameters.AddWithValue("@p_yarn_avail_qty4", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_avail_qty4", p_avail4);
                }

                if (p_avail5 == 0)
                {
                    command1.Parameters.AddWithValue("@p_yarn_avail_qty5", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_avail_qty5", p_avail5);
                }
                if (p_avail6 == 0)
                {
                    command1.Parameters.AddWithValue("@p_yarn_avail_qty6", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_avail_qty6", p_avail6);
                }
                if (p_kg_per_finished_roll == 0)
                {
                    command1.Parameters.AddWithValue("@p_kg_per_finished_roll", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_kg_per_finished_roll", p_kg_per_finished_roll);
                }
                if (p_no_of_finished_rolls == 0)
                {
                    command1.Parameters.AddWithValue("@p_no_of_finished_rolls", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_no_of_finished_rolls", p_no_of_finished_rolls);
                }
                if (p_yarn_shortage1 == null || p_yarn_shortage1 == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_shortage1", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_shortage1", (p_yarn_shortage1.Trim()));
                }
                if (p_yarn_shortage2 == null || p_yarn_shortage2 == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_shortage2", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_shortage2", (p_yarn_shortage2.Trim()));
                }
                if (p_yarn_shortage3 == null || p_yarn_shortage3 == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_shortage3", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_shortage3", (p_yarn_shortage3.Trim()));
                }
                if (p_yarn_shortage4 == null || p_yarn_shortage4 == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_shortage4", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_shortage4", (p_yarn_shortage4.Trim()));
                }
                if (p_yarn_shortage5 == null || p_yarn_shortage5 == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_shortage5", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_shortage5", (p_yarn_shortage5.Trim()));
                }
                if (p_yarn_shortage6 == null || p_yarn_shortage6 == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_shortage6", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_shortage6", (p_yarn_shortage6.Trim()));
                }
                if (p_new_yarn1 == null || p_new_yarn1 == "")
                {
                    command1.Parameters.AddWithValue("@p_new_yarn1", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_new_yarn1", (p_new_yarn1.Trim()));
                }
                if (p_new_yarn2 == null || p_new_yarn2 == "")
                {
                    command1.Parameters.AddWithValue("@p_new_yarn2", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_new_yarn2", (p_new_yarn2.Trim()));
                }
                if (p_new_yarn3 == null || p_new_yarn3 == "")
                {
                    command1.Parameters.AddWithValue("@p_new_yarn3", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_new_yarn3", (p_new_yarn3.Trim()));
                }
                if (p_new_yarn4 == null || p_new_yarn4 == "")
                {
                    command1.Parameters.AddWithValue("@p_new_yarn4", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_new_yarn4", (p_new_yarn4.Trim()));
                }
                if (p_new_yarn5 == null || p_new_yarn5 == "")
                {
                    command1.Parameters.AddWithValue("@p_new_yarn5", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_new_yarn5", (p_new_yarn5.Trim()));
                }
                if (p_new_yarn6 == null || p_new_yarn6 == "")
                {
                    command1.Parameters.AddWithValue("@p_new_yarn6", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_new_yarn6", (p_new_yarn6.Trim()));
                }

                if (p_test_method_id == null || p_test_method_id == "")
                {
                    command1.Parameters.AddWithValue("@p_test_method_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_test_method_id", (p_test_method_id));
                }

                if (p_shoe_size_id == null || p_shoe_size_id == "")
                {
                    command1.Parameters.AddWithValue("@p_shoe_size_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_shoe_size_id", (p_shoe_size_id));
                }

                if (p_shoe_style == null || p_shoe_style == "")
                {
                    command1.Parameters.AddWithValue("@p_shoe_style", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_shoe_style", (p_shoe_style));
                }

                if (p_shoe_gender_id == null || p_shoe_gender_id == "")
                {
                    command1.Parameters.AddWithValue("@p_shoe_gender_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_shoe_gender_id", (p_shoe_gender_id));
                }

                if (p_shoe_require_laser_cut == null || p_shoe_require_laser_cut == "")
                {
                    command1.Parameters.AddWithValue("@p_shoe_require_laser_cut", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_shoe_require_laser_cut", (p_shoe_require_laser_cut));
                }

                if (p_laser_cut_with_pattern == null || p_laser_cut_with_pattern == "")
                {
                    command1.Parameters.AddWithValue("@p_laser_cut_with_pattern", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_laser_cut_with_pattern", p_laser_cut_with_pattern);
                }

                if (p_shoe_length == null || p_shoe_length == "")
                {
                    command1.Parameters.AddWithValue("@p_shoe_length", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_shoe_length", p_shoe_length);
                }


                if (p_shoe_width == null || p_shoe_width == "")
                {
                    command1.Parameters.AddWithValue("@p_shoe_width", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_shoe_width", p_shoe_width);
                }

                if (p_pallet_pattern_no == null || p_pallet_pattern_no == "")
                {
                    command1.Parameters.AddWithValue("@p_pallet_pattern_no", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_pallet_pattern_no", p_pallet_pattern_no);
                }

                if (p_product_pattern_no == null || p_product_pattern_no == "")
                {
                    command1.Parameters.AddWithValue("@p_product_pattern_no", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_product_pattern_no", p_product_pattern_no);
                }
                if (p_shoe_pairs_required == null || p_shoe_pairs_required == "")
                {
                    command1.Parameters.AddWithValue("@p_shoe_pairs_required", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_shoe_pairs_required", p_shoe_pairs_required);
                }

                if (p_repeat_per_roll == null || p_repeat_per_roll == "")
                {
                    command1.Parameters.AddWithValue("@p_repeat_per_roll", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_repeat_per_roll", p_repeat_per_roll);
                }


                //DataTable result = db.ExecuteCommandAndGetDataTable(command1);
                //db.closeDBConnection();
                //
                command1.Transaction = transaction;
                SqlDataReader dr1 = command1.ExecuteReader();
                DataTable dt2 = new DataTable();
                dt2.Load(dr1);
                transaction.Commit();
                //
                return dt2;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                string errormessage = ex.Message.ToString();
                return null;
            }
            finally
            {
                cnn.Close();
                cnn.Dispose();
                transaction.Dispose();
            }
        }

        public int Delete_ItemProperty(string p_pdr_item_properties_id)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            //SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[pdr_item_properties_delete]");
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_item_properties_delete]");
            try
            {
                if (p_pdr_item_properties_id == null || p_pdr_item_properties_id.ToString() == "")
                {
                    command1.Parameters.AddWithValue("@p_pdr_item_properties_id", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_pdr_item_properties_id", Convert.ToInt32(p_pdr_item_properties_id));
                }

                int intresult = db.ExecuteNonQuery(command1);

                db.closeDBConnection();
                return intresult;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return 0;
            }
        }

        public DataTable Populate_Issue_dt(string p_customer_name)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[p_combo_customer]");
            try
            {

                if (p_customer_name == null || p_customer_name == "")
                {
                    command1.Parameters.AddWithValue("@p_customer_name", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_customer_name", (p_customer_name.Trim()));
                }
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable ds = new DataTable();
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


        public DataTable LoadDeveRequestApp(string p_pdr_new_develop_req_id)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            //SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("pdr.pdr_item_properties_select")
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("pdr.p_pdr_item_properties_select");
            try
            {
                if (p_pdr_new_develop_req_id == null || p_pdr_new_develop_req_id == "")
                {
                    command1.Parameters.AddWithValue("@p_pdr_new_develop_req_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_pdr_new_develop_req_id", Convert.ToInt32(p_pdr_new_develop_req_id.Trim()));
                }
                DataTable devrequest = db.ExecuteCommandAndGetDataTable(command1);
                db.closeDBConnection();
                return devrequest;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable Save_DevRequestApp(DataTable dt)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            DataTable dtresult;
            try
            {
               // SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[pdr_item_properties_update]");
                SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_item_properties_update]");
                foreach (DataRow dtRow in dt.Rows)
                {
                    command1.Parameters.Clear();
                    object p_pdr_item_properties_id = dtRow["p_pdr_item_properties_id"];
                    object p_pdr_new_develop_req_id = dtRow["p_pdr_new_develop_req_id"];
                    object p_appl_id = dtRow["p_appl_id"];
                    object p_sub_appl_id = dtRow["p_sub_appl_id"];
                    object p_spl_func_id = dtRow["p_spl_func_id"];
                    object p_ctry = dtRow["p_ctry"];
                    object p_market_zone_id = dtRow["p_market_zone_id"];
                    object p_market_customer_id = dtRow["p_market_customer_id"];
                    if (p_pdr_item_properties_id == null || p_pdr_item_properties_id.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_pdr_item_properties_id", "-1");
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_pdr_item_properties_id", p_pdr_item_properties_id);
                    }
                    if (p_pdr_new_develop_req_id == null || p_pdr_new_develop_req_id.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_pdr_new_develop_req_id", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_pdr_new_develop_req_id", p_pdr_new_develop_req_id);
                    }
                    if (p_appl_id == null || p_appl_id.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_appl_id", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_appl_id", Convert.ToInt32(p_appl_id));
                    }
                    if (p_sub_appl_id == null || p_sub_appl_id.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_sub_appl_id", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_sub_appl_id", Convert.ToInt32(p_sub_appl_id));
                    }
                    if (p_spl_func_id == null || p_spl_func_id.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_spl_func_id", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_spl_func_id", Convert.ToInt32(p_spl_func_id));
                    }
                    if (p_ctry == null || p_ctry.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_ctry", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_ctry", (p_ctry));
                    }
                    if (p_market_zone_id == null || p_market_zone_id.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_market_zone_id", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_market_zone_id", Convert.ToInt32(p_market_zone_id));
                   } 
                    if (p_market_customer_id == null || p_market_customer_id.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_market_customer_id", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_market_customer_id", Convert.ToInt32(p_market_customer_id));
                    }
                    dtresult = db.ExecuteCommandAndGetDataTable(command1);
                }
                db.closeDBConnection();
                return null;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

    }
}
