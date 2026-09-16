using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace PDR.BLL
{
    class classDR_Internal_Approve_Attachment_BLL
    {


        public DataTable LoadDRAttachmentDetails(string p_source_doc_number,string p_source_doc_type)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_dr_attachments_select]");

            try
            {
                if (p_source_doc_number == null || p_source_doc_number == "")
                {
                    command1.Parameters.AddWithValue("@p_source_doc_number", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_source_doc_number", (p_source_doc_number.Trim()));
                }
                if (p_source_doc_type == null || p_source_doc_type == "")
                {
                    command1.Parameters.AddWithValue("@p_source_doc_type", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_source_doc_type", (p_source_doc_type.Trim()));
                }
                DataTable attachmentdetails = db.ExecuteCommandAndGetDataTable(command1);
                db.closeDBConnection();
                return attachmentdetails;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataSet LoadDRAttachmentDetails_DS(string p_source_doc_number, string p_source_doc_type)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_dr_attachments_select]");

             try
                {
                    if (p_source_doc_number == null || p_source_doc_number == "")
                    {
                        command1.Parameters.AddWithValue("@p_source_doc_number", DBNull.Value);
                    }
                    else
                    {

                        command1.Parameters.AddWithValue("@p_source_doc_number", (p_source_doc_number.Trim()));
                    }
                    if (p_source_doc_type == null || p_source_doc_type == "")
                    {
                        command1.Parameters.AddWithValue("@p_source_doc_type", DBNull.Value);
                    }
                    else
                    {

                        command1.Parameters.AddWithValue("@p_source_doc_type", (p_source_doc_type.Trim()));
                    }
                    DataSet loadCheckAvailabilityGreigeStock = db.ExecuteCommandAndGetDataSet(command1);

                    db.closeDBConnection();
                    return loadCheckAvailabilityGreigeStock;
                }
                catch (Exception ex)
                {
                    return null;
                }
            
        }

        public DataTable UpdateDevelopDetails(string p_doc_attachments_id, string p_source_doc_number, string p_file_description, string p_file_location, string p_source_doc_type)

        {
            string connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
            SqlConnection cnn = new SqlConnection(connectionstring);
            SqlTransaction transaction;
            cnn.Open();
            transaction = cnn.BeginTransaction();
            SqlCommand command1 = new SqlCommand("[PDR].[p_pdr_dr_attachments_update]", cnn);
            command1.CommandType = CommandType.StoredProcedure; // new
            command1.Parameters.Clear(); // new
            try
            {
                if (p_doc_attachments_id == null || p_doc_attachments_id == "-1" || p_doc_attachments_id == "")
                {
                    command1.Parameters.AddWithValue("@p_doc_attachments_id", "-1");
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_doc_attachments_id", Convert.ToInt64(p_doc_attachments_id.Trim()));
                }
                if (p_source_doc_number == null || p_source_doc_number == "")
                {
                    command1.Parameters.AddWithValue("@p_source_doc_number", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_source_doc_number", (p_source_doc_number.Trim()));
                }
                if (p_file_description == null || p_file_description == "")
                {
                    command1.Parameters.AddWithValue("@p_file_description", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_file_description", (p_file_description.Trim()));
                }
                if (p_file_location == null || p_file_location == "")
                {
                    command1.Parameters.AddWithValue("@p_file_location", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_file_location", (p_file_location.Trim()));
                }
                if (p_source_doc_type == null || p_source_doc_type == "")
                {
                    command1.Parameters.AddWithValue("@p_source_doc_type", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_source_doc_type", (p_source_doc_type.Trim()));
                }
                command1.Transaction = transaction;
                SqlDataReader dr1 = command1.ExecuteReader();
                DataTable dt2 = new DataTable();
                dt2.Load(dr1);

                //save first grid


                ///
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


        public DataTable UpdateAttachmentDescription(DataTable dt)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            DataTable dtresult;
            try
            {
                SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_dr_attachments_update]");
                foreach (DataRow dtRow in dt.Rows)
                {
                    command1.Parameters.Clear();
                    object p_doc_attachments_id = dtRow["p_doc_attachments_id"];
                    object p_source_doc_number = dtRow["p_source_doc_number"];
                    object p_file_description = dtRow["p_file_description"];
                    object p_source_doc_type = dtRow["p_source_doc_type"];
                    if (p_doc_attachments_id == null || p_doc_attachments_id.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_doc_attachments_id", "-1");
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_doc_attachments_id", Convert.ToInt64(p_doc_attachments_id));
                    }
                    if (p_source_doc_number == null || p_source_doc_number.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_source_doc_number", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_source_doc_number", p_source_doc_number);
                    }
                    if (p_file_description == null || p_file_description.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_file_description", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_file_description", (p_file_description));
                    }
                    if (p_source_doc_type == null || p_source_doc_type.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_source_doc_type", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_source_doc_type", (p_source_doc_type));
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




        public int Delete_DocAttachmentID(string p_doc_attachments_id)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_dr_attachments_delete]");
            try
            {
                if (p_doc_attachments_id == null || p_doc_attachments_id.ToString() == "")
                {
                    command1.Parameters.AddWithValue("@p_doc_attachments_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_doc_attachments_id", Convert.ToInt64(p_doc_attachments_id));
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


    }
}
