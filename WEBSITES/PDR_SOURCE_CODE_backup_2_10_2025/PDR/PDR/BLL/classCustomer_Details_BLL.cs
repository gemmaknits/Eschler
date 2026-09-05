using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace PDR.BLL
{
    class classCustomer_Details_BLL
    {

        public DataTable LoadCustomerDetails(string p_custcd)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_customers_select]");

            try
            {
                if (p_custcd == null || p_custcd == "")
                {
                    command1.Parameters.AddWithValue("@p_custcd", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_custcd", (p_custcd.Trim()));
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


        public DataTable UpdateCustomer(string p_custcd, string p_name, string p_namet, string p_addr1,
                                         string p_addr2, string p_addr3, string p_addr1t, string p_addr2t,
                                        string p_addr3t, string p_city, string p_ctry, string p_tel, string p_fax,
                                        string p_email, string p_contact, string p_user_session_id)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_customers_update]");
            try
            {
                if (p_custcd == null || p_custcd == "")
                {
                    command1.Parameters.AddWithValue("@p_custcd", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_custcd", (p_custcd.Trim()));
                }
                if (p_name == null || p_name == "")
                {
                    command1.Parameters.AddWithValue("@p_name", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_name", (p_name.Trim()));
                }
                if (p_namet == null || p_namet == "")
                {
                    command1.Parameters.AddWithValue("@p_namet", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_namet", (p_namet.Trim()));
                }
                if (p_addr1 == null || p_addr1 == "")
                {
                    command1.Parameters.AddWithValue("@p_addr1", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_addr1", (p_addr1.Trim()));
                }
                if (p_addr2 == null || p_addr2 == "")
                {
                    command1.Parameters.AddWithValue("@p_addr2", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_addr2", (p_addr2.Trim()));
                }
                if (p_addr3 == null || p_addr3 == "")
                {
                    command1.Parameters.AddWithValue("@p_addr3", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_addr3", (p_addr3.Trim()));
                }
                if (p_addr1t == null || p_addr1t == "")
                {
                    command1.Parameters.AddWithValue("@p_addr1t", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_addr1t", (p_addr1t.Trim()));
                }
                if (p_addr2t == null || p_addr2t == "")
                {
                    command1.Parameters.AddWithValue("@p_addr2t", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_addr2t", (p_addr2t.Trim()));

                }
                if (p_addr3t == null || p_addr3t == "")
                {
                    command1.Parameters.AddWithValue("@p_addr3t", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_addr3t", (p_addr3t.Trim()));
                }
                if (p_city == null || p_city == "")
                {
                    command1.Parameters.AddWithValue("@p_city", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_city", (p_city.Trim()));
                }
                if (p_ctry == null || p_ctry == "")
                {
                    command1.Parameters.AddWithValue("@p_ctry", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_ctry", (p_ctry.Trim()));
                }
                if (p_tel == null || p_tel == "")
                {
                    command1.Parameters.AddWithValue("@p_tel", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_tel", (p_tel.Trim()));
                }

                if (p_fax == null || p_fax == "")
                {
                    command1.Parameters.AddWithValue("@p_fax", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_fax", (p_fax.Trim()));
                }
                if (p_email == null || p_email == "")
                {
                    command1.Parameters.AddWithValue("@p_email", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_email", (p_email.Trim()));
                }
                if (p_contact == null || p_contact == "")
                {
                    command1.Parameters.AddWithValue("@p_contact", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_contact", (p_contact.Trim()));
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
    }
}
