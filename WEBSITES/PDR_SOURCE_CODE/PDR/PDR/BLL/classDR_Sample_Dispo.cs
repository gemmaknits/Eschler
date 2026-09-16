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
    class classDR_Sample_Dispo
    {
        public DataTable GetDRKnittingDetails(string p_kono, string p_user_session_id)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_mfg_sample_dispo_select]");
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
                if (p_user_session_id == null || p_user_session_id == "")
                {
                    command1.Parameters.AddWithValue("@p_user_session_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_user_session_id", Convert.ToInt64(p_user_session_id.Trim()));
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


        public DataTable UpdateKnittingSampleDetails(DataTable dt)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            DataTable dtresult;
            try
            {
                SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_mfg_sample_dispo_update]");
                foreach (DataRow dtRow in dt.Rows)
                {
                    command1.Parameters.Clear();
                    object p_mfg_sample_dispo_id = dtRow["p_mfg_sample_dispo_id"];
                    object p_dispo_seq = dtRow["p_dispo_seq"];
                    object p_kono = dtRow["p_kono"];
                    object p_primary_quantity = dtRow["p_primary_quantity"];
                    object p_secondary_quantity = dtRow["p_secondary_quantity"];
                    object p_sample_dispo_comment = dtRow["p_sample_dispo_comment"];
                    object p_user_session_id = dtRow["p_user_session_id"];
                    if (p_mfg_sample_dispo_id == null || p_mfg_sample_dispo_id.ToString() == "" || p_mfg_sample_dispo_id.ToString() == "-1")
                    {
                        command1.Parameters.AddWithValue("@p_mfg_sample_dispo_id", "-1");
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_mfg_sample_dispo_id", p_mfg_sample_dispo_id);
                    }
                    if (p_dispo_seq == null || p_dispo_seq.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_dispo_seq", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_dispo_seq", Convert.ToInt64(p_dispo_seq));
                    }
                    if (p_kono == null || p_kono.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_kono", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_kono", (p_kono));
                    }
                    if (p_primary_quantity == null || p_primary_quantity.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_primary_quantity", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_primary_quantity", Convert.ToDouble(p_primary_quantity));
                    }
                    if (p_secondary_quantity == null || p_secondary_quantity.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_secondary_quantity", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_secondary_quantity", Convert.ToDouble(p_secondary_quantity));
                    }
                    if (p_sample_dispo_comment == null || p_sample_dispo_comment.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_sample_dispo_comment", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_sample_dispo_comment", (p_sample_dispo_comment));
                    }
                    if (p_user_session_id == null || p_user_session_id.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_user_session_id", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_user_session_id", Convert.ToInt64(p_user_session_id));
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

        public DataTable UpdateKnittingSampleDetails(string p_mfg_sample_dispo_id, string p_dispo_seq, string p_kono, string p_primary_quantity, string p_secondary_quantity, string p_user_session_id)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_mfg_sample_dispo_update]");
            try
            {
                if (p_mfg_sample_dispo_id == null || p_mfg_sample_dispo_id == "")
                {
                    command1.Parameters.AddWithValue("@p_mfg_sample_dispo_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_mfg_sample_dispo_id", Convert.ToInt64(p_mfg_sample_dispo_id.Trim()));
                }
                if (p_dispo_seq == null || p_dispo_seq == "")
                {
                    command1.Parameters.AddWithValue("@p_dispo_seq", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_dispo_seq", Convert.ToInt64(p_dispo_seq.Trim()));
                }
                if (p_kono == null || p_kono == "")
                {
                    command1.Parameters.AddWithValue("@p_kono", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_kono", (p_kono.Trim()));
                }
                if (p_primary_quantity == null || p_primary_quantity == "")
                {
                    command1.Parameters.AddWithValue("@p_primary_quantity", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_primary_quantity", Convert.ToDouble(p_primary_quantity.Trim()));
                }

                if (p_secondary_quantity == null || p_secondary_quantity == "")
                {
                    command1.Parameters.AddWithValue("@p_secondary_quantity", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_secondary_quantity", Convert.ToDouble(p_secondary_quantity.Trim()));
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
                // DataTable errormessage = new DataTable();// = ex.Message.ToString();
                //errormessage = ex.Message.ToString();
                return null;
            }

        }



        // public string sendSampleKnittingMail(DataTable dt, string attachmentIDs, string sessionID)
        public string sendSampleKnittingMail(string desginno, string attachmentIDs, string sessionID, string tomaillist, string ccmaillist, string subject, string bodytext, string seq, string comment)
        {
            string strResult = "F";
            try
            {
                //Create the msg object to be sent
                StringBuilder strBodyHTML = new StringBuilder();
                DataRow r;
                //r = dt.Rows[0];
                string toEmailID="";
                string ccMails="";
                string PreparerEmail;
                MailAddress fromMailId = new MailAddress("wf@gemmaknits.com");
                MailAddress requestormail;
                // MailAddress toMailId;
                classKnitting_Approval_BLL getDRNODetails = new classKnitting_Approval_BLL();
                DataTable dt = getDRNODetails.GetDRKnittingDetails(desginno.Trim());
                foreach (DataRow row in dt.Rows)
                {
                    if (tomaillist != "")
                    {
                        toEmailID = tomaillist;
                    }
                    else
                    {
                        //toEmailID = row["factory_manager_email"].ToString();
                    }
                    if (ccmaillist != "")
                    {
                        ccMails = ccmaillist;
                    }
                    else
                    {
                        //ccMails = row["dr_staff_email"].ToString();
                        //ccMails = ccMails + "," + row["production_email"].ToString();
                        //ccMails = ccMails + "," + row["rd_manager_email"].ToString();
                    }

                    // }
                    string[] CCId = ccMails.Split(',');
                   // MailAddress toMailId = new MailAddress(toEmailID);

                    MailMessage msg = new MailMessage(fromMailId, fromMailId);
                    string[] TOId = toEmailID.Split(',');
                    foreach (string ToEmail in TOId)
                    {
                        if (ToEmail.ToString() != "")
                        {
                            msg.To.Add(new MailAddress(ToEmail)); //Adding Multiple CC email Id
                        }
                    }
                    //msg.To.Add(toMailId);
                    foreach (string CCEmail in CCId)
                    {
                        if (CCEmail != "")
                        {
                            msg.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                        }
                    }
                    if (subject != "")
                    {
                        msg.Subject = subject.ToString();
                    }
                    else
                    {
                        msg.Subject = "KNIT SAMPLE";
                    }
                    if (bodytext != "")
                    {
                        string msgbody = bodytext.ToString().Replace("\r\n", "<br>");
                        msgbody = msgbody.Replace("KNIT SAMPLE", "");
                        //strBodyHTML.Append("<a href=\"http://103.40.140.12:86/ui/DR_Knitting_Sample_Approve.aspx?pagename=DR_Knitting_Sample_Approve&designno=" + dt.Rows[0]["design_no"] + "&SessionID=" + sessionID + "\">");
                        // strBodyHTML.Append("No. " + dt.Rows[0]["pdr_no"] + "       ");
                        msgbody = msgbody.Replace("DESIGN NO: " + dt.Rows[0]["design_no"].ToString().Trim() + "  " + dt.Rows[0]["CUSTOMER_NAME"].ToString().Trim(), "");
                        // strBodyHTML.Append("</a>");
                        msgbody = msgbody.Replace("DESIGN NO: " + dt.Rows[0]["design_no"].ToString().Trim(), "");
                        msgbody = msgbody.Replace("KI: " + dt.Rows[0]["KONO"].ToString().Trim(), "");
                        msgbody = msgbody.Replace("DR NO: " + dt.Rows[0]["DR_NO"].ToString().Trim(), "");
                        msgbody = msgbody.Replace("SEQ: " + seq.Trim(), "");
                        msgbody = msgbody.Replace("COMMENT:" + comment.Trim(), "");
                        strBodyHTML.Append("<HTML>");

                        strBodyHTML.Append("DESIGN NO: ");
                        string strIPAddress = ConnectionDB.strLinkIPAddress;

                        //strBodyHTML.Append("<a href=\"http://103.40.140.12:86/ui/DR_Knitting_Sample_Approve.aspx?pagename=DR_Knitting_Sample_Approve&designno=" + dt.Rows[0]["design_no"].ToString().Trim() + "&SessionID=" + sessionID + "\">");
                        strBodyHTML.Append("<a href=\"http://" + strIPAddress  + "/ui/DR_Knitting_Sample_Approve.aspx?pagename=DR_Knitting_Sample_Approve&designno=" + dt.Rows[0]["design_no"].ToString().Trim() + "&SessionID=" + sessionID + "\">");
                        strBodyHTML.Append(dt.Rows[0]["design_no"].ToString().Trim() + "    ");
                        strBodyHTML.Append("</a>");
                        strBodyHTML.Append(dt.Rows[0]["CUSTOMER_NAME"].ToString().Trim());
                        strBodyHTML.Append("<BR>");

                        strBodyHTML.Append("KI: " + dt.Rows[0]["KONO"].ToString().Trim() + "     ");
                        strBodyHTML.Append("<BR>");
                        strBodyHTML.Append("DR NO: " + dt.Rows[0]["DR_NO"].ToString().Trim() + "     ");
                        strBodyHTML.Append("<BR>");
                        strBodyHTML.Append("SEQ: " + seq.Trim());
                        strBodyHTML.Append("<BR>");
                        strBodyHTML.Append("COMMENT: " + comment.Trim() + "</FONT>");
                        msg.Body = msgbody + strBodyHTML.ToString();//
                    }
                    else
                    {
                        strBodyHTML.Append("<HTML>");
                        strBodyHTML.Append("<H1><Td color='blue'>DR KNITTING</td></h1>");
                        strBodyHTML.Append("<P>");
                        //                strBodyHTML.Append("<H1>");Append(Font3);
                        strBodyHTML.Append("DESIGN NO: " + dt.Rows[0]["design_no"].ToString().Trim() + "     ");
                        strBodyHTML.Append("<BR>");
                        strBodyHTML.Append("KI: " + dt.Rows[0]["KONO"].ToString().Trim() + "     ");
                        strBodyHTML.Append("<BR>");
                        strBodyHTML.Append("DR NO: " + dt.Rows[0]["DR_NO"].ToString().Trim() + "     ");
                        strBodyHTML.Append("<BR>");
                        strBodyHTML.Append("SEQ: " + seq.Trim());
                        strBodyHTML.Append("<BR>");
                        strBodyHTML.Append("COMMENT: " + comment.Trim() + "</FONT>");
                        //              strBodyHTML.Append("</H1>");

                        // strBodyHTML.Append("<TABLE CELLSPACING=3 CELLPADING=1 >");

                        msg.Body = strBodyHTML.ToString();
                    }

                    msg.Body = strBodyHTML.ToString();
                    msg.IsBodyHtml = true;

                    if (attachmentIDs.ToString().Trim() != null && attachmentIDs.ToString().Trim() != "")
                    {
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
