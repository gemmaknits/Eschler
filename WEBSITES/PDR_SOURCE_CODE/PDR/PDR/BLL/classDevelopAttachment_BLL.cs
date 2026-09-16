using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace PDR.BLL
{
    class classDevelopAttachment_BLL
    {


        public DataTable LoadProductAttachmentDetails(string p_product)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_product_attachments_select]");

            try
            {
                if (p_product == null || p_product == "")
                {
                    command1.Parameters.AddWithValue("@p_product", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_product", p_product.Trim());
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

        public DataTable LoadeDevelopAttachmentDetails(string p_pdr_new_develop_req_id)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_new_develop_req_attachments_select]");

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



        public DataTable UpdateDevelopDetails(string p_doc_attachments_id ,string p_source_doc_number, string p_file_description,string p_file_location)

        {
            string connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
            SqlConnection cnn = new SqlConnection(connectionstring);
            SqlTransaction transaction;
            cnn.Open();
            transaction = cnn.BeginTransaction();
            SqlCommand command1 = new SqlCommand("[PDR].[p_pdr_new_develop_req_attachments_update]", cnn);
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
                SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_new_develop_req_attachments_update]");
                foreach (DataRow dtRow in dt.Rows)
                {
                    command1.Parameters.Clear();
                    object p_doc_attachments_id = dtRow["p_doc_attachments_id"];
                    object p_source_doc_number = dtRow["p_source_doc_number"];
                    object p_file_description = dtRow["p_file_description"];
                    if (p_doc_attachments_id == null || p_doc_attachments_id.ToString() == "")
                    {
                        command1.Parameters.AddWithValue("@p_doc_attachments_id", "-1");
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@p_doc_attachments_id",Convert.ToInt64(p_doc_attachments_id));
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
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_new_develop_req_attachments_delete]");
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
