using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDR.BLL;
using System.Data.SqlClient;

namespace PDR.BLL
{
    class classValidateUserLogin_BLL
    {
        public classValidateUserLogin_BLL()
        {

        }

        public SqlDataReader ValidateLogin(string username, string password)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            List<SqlParameter> paramList = new List<SqlParameter>(2);
            paramList.Add(new SqlParameter("@p_un", username));
            paramList.Add(new SqlParameter("@p_pw", password));
            SqlDataReader reader= db.ExecuteCommandAndGetDataReader("[PDR].[p_pdr_sec_verify_user]", paramList);
            if (reader != null)
            {


                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //System.Diagnostics.Debug.WriteLine("{0}\t{1}", reader.GetString(0),
                        //  reader.GetString(1));
                        return reader;
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No rows found.");
            }

            reader.Close();
            db.closeDBConnection();
            return reader;
        }


        // 09/10/2026 John - URL login: same SP/params as ValidateLogin, returns {userid, username, user_session_id, login_allowed} or null, always closes reader + connection
        public string[] ValidateLoginByUrl(string username, string password) // 09/10/2026 John
        { // 09/10/2026 John
            ConnectionDB db = ConnectionDB.getDbInstance(); // 09/10/2026 John
            SqlDataReader reader = null; // 09/10/2026 John
            try // 09/10/2026 John
            { // 09/10/2026 John
                db.ConnectToDB(); // 09/10/2026 John
                List<SqlParameter> paramList = new List<SqlParameter>(2); // 09/10/2026 John
                paramList.Add(new SqlParameter("@p_un", username)); // 09/10/2026 John
                paramList.Add(new SqlParameter("@p_pw", password)); // 09/10/2026 John
                reader = db.ExecuteCommandAndGetDataReader("[PDR].[p_pdr_sec_verify_user]", paramList); // 09/10/2026 John
                if (reader == null || !reader.Read()) // 09/10/2026 John
                { // 09/10/2026 John
                    return null; // 09/10/2026 John - no rows
                } // 09/10/2026 John
                return new string[] { // 09/10/2026 John
                    reader["userid"].ToString().Trim(), // 09/10/2026 John
                    reader["username"].ToString().Trim(), // 09/10/2026 John
                    reader["user_session_id"].ToString().Trim(), // 09/10/2026 John
                    reader["login_allowed"].ToString().Trim() }; // 09/10/2026 John
            } // 09/10/2026 John
            finally // 09/10/2026 John
            { // 09/10/2026 John
                if (reader != null && !reader.IsClosed) reader.Close(); // 09/10/2026 John
                db.closeDBConnection(); // 09/10/2026 John
            } // 09/10/2026 John
        } // 09/10/2026 John

        public SqlDataReader ValidateLoginFromEmail(string sessionID)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            List<SqlParameter> paramList = new List<SqlParameter>(1);
            paramList.Add(new SqlParameter("@p_user_session_id",Convert.ToInt64(sessionID)));
            SqlDataReader reader = db.ExecuteCommandAndGetDataReader("[dbo].[p_sec_verify_user_session_id]", paramList);
            if (reader != null)
            {


                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //System.Diagnostics.Debug.WriteLine("{0}\t{1}", reader.GetString(0),
                        //  reader.GetString(1));
                        return reader;
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No rows found.");
            }

            reader.Close();
            db.closeDBConnection();
            return reader;
        }

        public void UpdateUserSession(string userid,string usersessionid)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            List<SqlParameter> paramList = new List<SqlParameter>(2);
            paramList.Add(new SqlParameter("@p_userid", userid));
            paramList.Add(new SqlParameter("@p_user_session_id", usersessionid));
            SqlDataReader reader = db.ExecuteCommandAndGetDataReader("[PDR].[p_pdr_sec_user_logout]", paramList);
            reader.Close();
            db.closeDBConnection();
            return ;
        }
    }
}