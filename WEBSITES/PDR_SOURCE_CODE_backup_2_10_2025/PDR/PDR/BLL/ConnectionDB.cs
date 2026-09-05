using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.Security;
using System.Data;

namespace PDR.BLL
{
    public class ConnectionDB
    {
        private readonly string connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
        private static ConnectionDB s_SingletonConnection = null;
        private SqlConnection m_dbConnection;
        public static string strLinkIPAddress="172.16.3.4:86";
        private ConnectionDB()
        {

        }
        public string LinkIPAddress()
        {
            return strLinkIPAddress;
        }
        public static ConnectionDB getDbInstance()
        {
            if (s_SingletonConnection == null)
            {
                if (s_SingletonConnection == null)
                {
                    s_SingletonConnection = new ConnectionDB();
                }
            }
            return s_SingletonConnection;
        }

        public SqlConnection ConnectToDB()
        {
            try
            {
                m_dbConnection = new SqlConnection(connectionstring);
                m_dbConnection.Open();
                System.Diagnostics.Debug.WriteLine("Connected");
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine("Not connected : " + e.ToString());
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("End..");
            }
            return m_dbConnection;
        }

        public SqlConnection closeDBConnection()
        {
            try
            {
                if (m_dbConnection.State == ConnectionState.Open)
                {
                    m_dbConnection.Close();
                    System.Diagnostics.Debug.WriteLine("Connection Closed");
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine("Error in Closing Connection : " + e.ToString());
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("End..");
            }
            return m_dbConnection;
        }

        public SqlTransaction BeginTransaction()
        {
            SqlTransaction sqlTransaction = null;
            try
            {
                if (m_dbConnection.State == ConnectionState.Open)
                {
                    sqlTransaction = m_dbConnection.BeginTransaction();
                    System.Diagnostics.Debug.WriteLine("transaction open");
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine(sqlEx.ToString());
            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("End..");
            }
            return sqlTransaction;
        }

        public void commitTransaction(SqlTransaction transaction)
        {
            try
            {
                if (m_dbConnection.State == ConnectionState.Open)
                {
                    transaction.Commit();
                    System.Diagnostics.Debug.WriteLine("Transaction commited");
                }
            }
            catch (SqlException e)
            {
                transaction.Rollback();
                System.Diagnostics.Debug.WriteLine("Error in Transaction commit : " + e.ToString());
            }
            finally
            {
                transaction.Dispose();
                System.Diagnostics.Debug.WriteLine("End..");
            }
        }

        public SqlDataReader ExecuteCommandAndGetDataReader(string commandName, List<SqlParameter> parameterList)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = m_dbConnection;
            //
            try
            {
                if (m_dbConnection != null && m_dbConnection.State == ConnectionState.Closed)
                {
                    // do something
                    // ...
                    m_dbConnection.Open();
                }

                //
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = commandName;

                if (parameterList != null)
                {
                    cmd.Parameters.AddRange(parameterList.ToArray());
                }
                SqlDataReader reader = cmd.ExecuteReader();
                return reader;
            }
            finally
            {
                //if (m_dbConnection.State != ConnectionState.Closed)
                //    m_dbConnection.Close();
            }
        }

        public System.Data.DataSet ExecuteCommandAndGetDataSet(string commandName, List<SqlParameter> parameterList)
        {
            SqlCommand cmd1 = new SqlCommand();
            cmd1.Connection = m_dbConnection;
            try
            {
                if (m_dbConnection != null && m_dbConnection.State == ConnectionState.Closed)
                {
                    // do something
                    // ...
                    m_dbConnection.Open();
                }
                cmd1.CommandType = System.Data.CommandType.StoredProcedure;
                cmd1.CommandText = commandName;

                if (parameterList != null)
                {
                    cmd1.Parameters.AddRange(parameterList.ToArray());
                }
                SqlDataAdapter da = new SqlDataAdapter(commandName, m_dbConnection);
                var dataset = new DataSet();
                da.Fill(dataset);
                return dataset;
            }
            catch (SqlException ex)
            {
                string errormessage = ex.Message.ToString();
                return null;

            }
            finally
            {
            }

        }

        public System.Data.IDbCommand GetCommandToSetParameters(String commandname)
        {
            SqlCommand command = new SqlCommand(commandname, m_dbConnection);
            command.Connection = m_dbConnection;
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = commandname;
            return command;
        }


        public DataSet ExecuteCommandAndGetDataSet(IDbCommand command)
        {
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter();

                if (m_dbConnection != null && m_dbConnection.State == ConnectionState.Closed)
                {
                    m_dbConnection.Open();
                }
                command.CommandTimeout = 600;
                dataAdapter.SelectCommand = (SqlCommand) command;
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);

                m_dbConnection.Close();
                return ds;
            }
            catch (SqlException ex)
            {
                string errormessage = ex.Message.ToString();
                return null;

            }
            finally
            {
            }
        }
        public DataTable ExecuteCommandAndGetDataTable(IDbCommand command)
        {
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter();

                if (m_dbConnection != null && m_dbConnection.State == ConnectionState.Closed)
                {
                    m_dbConnection.Open();
                }
                command.CommandTimeout = 60;
                dataAdapter.SelectCommand = (SqlCommand)command;
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);

                m_dbConnection.Close();
                return dt;
            }
            catch (SqlException ex)
            {

                string errormessage = ex.Message.ToString();
                return null;

            }
            finally
            {
            }
        }
       
        public int ExecuteScalar_POC(IDbCommand command)
        {
            try
            {
                int intresult = 0;
                //SqlDataAdapter dataAdapter = new SqlDataAdapter();

                if (m_dbConnection != null && m_dbConnection.State == ConnectionState.Closed)
                {
                    m_dbConnection.Open();
                }
                command.CommandTimeout = 600;
                
              //  intresult = (Int32)command.ExecuteScalar();
              intresult= command.ExecuteNonQuery();

                m_dbConnection.Close();
                return intresult;
            }
            catch (SqlException ex)
            {
                string errormessage = ex.Message.ToString();
                return 0;

            }
            finally
            {
            }
        }

        public int ExecuteNonQuery(IDbCommand command)
        {
            try
            {
                int intresult = 0;
                SqlDataAdapter dataAdapter = new SqlDataAdapter();

                if (m_dbConnection != null && m_dbConnection.State == ConnectionState.Closed)
                {
                    m_dbConnection.Open();
                }
                //intresult = (Int32)command.ExecuteScalar();
                intresult = command.ExecuteNonQuery();
                m_dbConnection.Close();
                return intresult;
            }
            catch (SqlException ex)
            {
                string errormessage = ex.Message.ToString();
                return 0;

            }
            finally
            {
            }
        }

        public string ExecuteNonQuery_Validate(IDbCommand command)
        {
            try
            {
                string result = "";
                SqlDataAdapter dataAdapter = new SqlDataAdapter();

                if (m_dbConnection != null && m_dbConnection.State == ConnectionState.Closed)
                {
                    m_dbConnection.Open();
                }
                //intresult = (Int32)command.ExecuteScalar();
                result =Convert.ToString(command.ExecuteScalar());
                m_dbConnection.Close();
                return result;
            }
            catch (SqlException ex)
            {
                string errormessage = ex.Message.ToString();
                return null;

            }
            finally
            {
            }
        }

        public int ExecuteNonQuery(IDbCommand command,SqlTransaction transaction)
        {
            try
            {
                int intresult = 0;
                SqlDataAdapter dataAdapter = new SqlDataAdapter();

                if (m_dbConnection != null && m_dbConnection.State == ConnectionState.Closed)
                {
                    m_dbConnection.Open();
                }
                intresult =(Int32) command.ExecuteScalar();
                m_dbConnection.Close();
                return intresult;
            }
            catch (SqlException ex)
            {
                string errormessage = ex.Message.ToString();
                return 0;

            }
            finally
            {
            }
        }




        //public System.Data.DataSet ExecuteCommandAndGetDataSet_OrderList(String commandname, string p_order_date_from, string p_order_date_to, string p_customer_code,
        //     string p_finished_design, string p_factory_design, string p_order_type, string p_order_group, string p_ki_opened, string p_pdf_opened,
        //     int p_user_session_id, string p_user_id)
        //{
        //    SqlCommand cmd1 = new SqlCommand(commandname, m_dbConnection);
        //    cmd1.Connection = m_dbConnection;
        //    try
        //    {
        //        if (m_dbConnection != null && m_dbConnection.State == ConnectionState.Closed)
        //        {
        //            // do something
        //            // ...
        //            m_dbConnection.Open();
        //        }


        //        cmd1.CommandType = System.Data.CommandType.StoredProcedure;
        //        cmd1.CommandText = commandname;// "p_his_order_hist_view_knitting";

        //        cmd1.Parameters.AddWithValue("@p_order_date_from", Convert.ToDateTime(p_order_date_from));
        //        cmd1.Parameters.AddWithValue("@p_order_date_to", Convert.ToDateTime(p_order_date_to));
        //        cmd1.Parameters.AddWithValue("@p_customer_code", DBNull.Value);
        //        cmd1.Parameters.AddWithValue("@p_finished_design", DBNull.Value);
        //        cmd1.Parameters.AddWithValue("@p_factory_design", DBNull.Value);
        //        cmd1.Parameters.AddWithValue("@p_order_type", DBNull.Value);
        //        cmd1.Parameters.AddWithValue("@p_order_group", DBNull.Value);
        //        cmd1.Parameters.AddWithValue("@p_ki_opened", p_ki_opened);
        //        cmd1.Parameters.AddWithValue("@p_pdf_opened", p_pdf_opened);
        //        cmd1.Parameters.AddWithValue("@p_user_session_id", p_user_session_id);
        //        cmd1.Parameters.AddWithValue("@p_user_id", p_user_id);
        //        SqlDataAdapter da = new SqlDataAdapter(commandname, m_dbConnection);
        //        da.SelectCommand = cmd1;
        //        DataSet ds = new DataSet();
        //        da.Fill(ds);
        //        return ds;
        //    }
        //    catch (SqlException ex)
        //    {
        //        return null;

        //    }
        //    finally
        //    {
        //    }
        //}

        private void CheckNullOrEmptyAndReturnDBNullValue(string valueToBeChecked, out DBNull returnValue)
        {
            returnValue = null;
            if (string.IsNullOrEmpty(valueToBeChecked))
            {
                returnValue = DBNull.Value;
            }
        }

        public System.Data.DataSet ExecuteCommandAndGetDataSet_OrderHistoryGrid1(String commandname, string psono, string finishedDesignNo, string designNo,
                                string bomCode, string colorCode, int usersessionid, string dataname, string refreshdata)
        {
           
            try
            {
                if (m_dbConnection != null && m_dbConnection.State == ConnectionState.Closed)
                {
                    // do something
                    // ...
                    m_dbConnection.Open();
                }

                SqlCommand cmd1 = new SqlCommand("p_his_order_hist_view_knitting", m_dbConnection);
                cmd1.Connection = m_dbConnection;
                cmd1.CommandType = System.Data.CommandType.StoredProcedure;
                cmd1.CommandText = commandname;// "p_his_order_hist_view_knitting";

                cmd1.Parameters.AddWithValue("@p_sono", psono);
                cmd1.Parameters.AddWithValue("@p_finished_design_no", finishedDesignNo);
                cmd1.Parameters.AddWithValue("@p_design_no", designNo);
                cmd1.Parameters.AddWithValue("@p_bom_code", bomCode);
                cmd1.Parameters.AddWithValue("@p_color_code", colorCode);
                cmd1.Parameters.AddWithValue("@p_user_session_id", usersessionid);
                cmd1.Parameters.AddWithValue("@p_data_name", dataname);
                cmd1.Parameters.AddWithValue("@p_refresh_data", refreshdata);
                SqlDataAdapter da = new SqlDataAdapter(commandname, m_dbConnection);
                da.SelectCommand = cmd1;
              //  da.SelectCommand.CommandTimeout = 600000;
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (SqlException ex)
            {
                string errormessage = ex.Message.ToString();
                return null;

            }
            finally
            {
            }



        }
    }
}
