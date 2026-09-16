using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace PDR.BLL
{
    class classYarnLot_BLL
    {
        public DataTable LoadeYarnLotDetails(string p_kono ,string p_user_session_id)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_dr_knitting_yarn_lot]");

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

                    command1.Parameters.AddWithValue("@p_user_session_id",Convert.ToInt64 (p_user_session_id.Trim()));
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
    }
}
