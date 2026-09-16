using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
namespace PDR.BLL
{
    class classPDR_Approval_BLL
    {
        public DataTable LoadPDRApproval()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_combo_apprej]");

            try
            {

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

        public DataTable LoadPDRApprovalDetails(string p_pdr_no)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_apprej_select]");

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

        public DataTable UpdatePDRApprovalDetails(string p_pdr_new_develop_req_id,string p_pdr_app_rej_status,
                                                    string p_pdr_app_rej_date,string p_pdr_app_rej_by,string p_pdr_app_rej_comment)
        {
            //ConnectionDB db = ConnectionDB.getDbInstance();
            //db.ConnectToDB();
            string connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
            SqlConnection cnn = new SqlConnection(connectionstring);
            SqlTransaction transaction;
            cnn.Open();
            transaction = cnn.BeginTransaction();
            //SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_apprej_update]");
            SqlCommand command1 = new SqlCommand("[pdr].[p_pdr_apprej_update]", cnn);
            command1.CommandType = CommandType.StoredProcedure;
            command1.Parameters.Clear();
            try
            {
                if (p_pdr_new_develop_req_id == null || p_pdr_new_develop_req_id == "")
                {
                    command1.Parameters.AddWithValue("@p_pdr_new_develop_req_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_pdr_new_develop_req_id",Convert.ToInt64(p_pdr_new_develop_req_id.Trim()));
                }
                if (p_pdr_app_rej_status == null || p_pdr_app_rej_status == "")
                {
                    command1.Parameters.AddWithValue("@p_pdr_app_rej_status", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_pdr_app_rej_status", (p_pdr_app_rej_status.Trim()));
                }
                if (p_pdr_app_rej_date == null || p_pdr_app_rej_date == "")
                {
                    command1.Parameters.AddWithValue("@p_pdr_app_rej_date", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_pdr_app_rej_date",Convert.ToDateTime(p_pdr_app_rej_date.Trim()));
                }
                if (p_pdr_app_rej_by == null || p_pdr_app_rej_by == "")
                {
                    command1.Parameters.AddWithValue("@p_pdr_app_rej_by", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_pdr_app_rej_by", (p_pdr_app_rej_by.Trim()));
                }
                if (p_pdr_app_rej_comment == null || p_pdr_app_rej_comment == "")
                {
                    command1.Parameters.AddWithValue("@p_pdr_app_rej_comment", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_pdr_app_rej_comment", (p_pdr_app_rej_comment.Trim()));
                }

                //
                command1.Transaction = transaction;
                SqlDataReader dr = command1.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                //
                // DataTable orderissuedesign = db.ExecuteCommandAndGetDataTable(command1);

                // int intresult = command1.ExecuteNonQuery();
                // db.closeDBConnection();
                transaction.Commit();
                return dt;
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



        public string sendApprovalMail(DataTable dt)
        {
            string strResult;
            try
            {
                //Create the msg object to be sent
                StringBuilder strBodyHTML = new StringBuilder();
                DataRow row;
                string toEmail_Sample_room_Email = "";
                string ccEmail_Requestor_Email = "";
                string ccEmail_Prepare_Email = "";

                row = dt.Rows[0];
                toEmail_Sample_room_Email = row["sample_room_email"].ToString();
                ccEmail_Requestor_Email = row["requester_email"].ToString();
                ccEmail_Prepare_Email = row["preparer_email"].ToString();
                if (ccEmail_Requestor_Email != "")
                {
                    if (ccEmail_Prepare_Email != "")
                    {
                        ccEmail_Requestor_Email = ccEmail_Requestor_Email + "," + ccEmail_Prepare_Email + ",suresh@geminitextile.com";
                    }
                    else
                    {
                        ccEmail_Requestor_Email = ccEmail_Requestor_Email + ",suresh@geminitextile.com";
                    }
                }
                else
                {
                    ccEmail_Requestor_Email = ccEmail_Requestor_Email +  ",suresh@geminitextile.com";
                }

                MailAddress fromMailId = new MailAddress("wf@gemmaknits.com");
                MailAddress toMailId = new MailAddress(toEmail_Sample_room_Email);
               // MailAddress ccMailID = new MailAddress(ccEmail_Requestor_Email);
                //MailAddress toMailId = new MailAddress("kalai@eplt.in");
                //MailAddress ccMailID = new MailAddress("kalai@eplt.in");
                MailMessage msg = new MailMessage(fromMailId, toMailId);
                msg.To.Add(toMailId);
                string[] CCId = ccEmail_Requestor_Email.Split(',');
                foreach (string CCEmail in CCId)
                {
                    msg.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                }
                strBodyHTML.Append("<HTML>");
                strBodyHTML.Append("<H1> PDR </h1>");
                strBodyHTML.Append("<P>");
                //                strBodyHTML.Append("<H1>");
                //              strBodyHTML.Append(Font3);
                string strIPAddress = ConnectionDB.strLinkIPAddress;
                strBodyHTML.Append("<a href=\"http://" + strIPAddress + "/ui/PDR_Approval.aspx?pdrno=" + dt.Rows[0]["pdr_no"]  + "&pagename=PDR_Approval" + "\">");
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


    }
}
