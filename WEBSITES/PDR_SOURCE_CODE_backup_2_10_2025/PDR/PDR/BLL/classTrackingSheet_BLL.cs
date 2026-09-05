using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace PDR.BLL
{
    class classTrackingSheet_BLL
    {
        public DataSet Load_Print_PDRList(string p_from_date, string p_to_date,string p_itcatid, string p_itsubcatid, string p_itgroupid,string p_itsubid, string p_product_application_id,
                                    string p_product_sub_application_id, string p_dye_finishing_formula_id, string p_finishing_id, string p_pdr_no,
                                    string p_requested_by, string p_customer, string p_prepared_by, string p_show_closed_pdr, string p_internal_app_rej_date_from,
                                   string p_internal_app_rej_date_to, string p_customer_app_rej_date_from, string p_customer_app_rej_date_to,
                                   string p_final_app_rej_date_from, string p_final_app_rej_date_to, string p_wait_internal_app_rej, string p_wait_customer_app_rej,
                                   string p_wait_final_app_rej, string p_add_to_collection,string  p_new_yarn,string p_yarn_shortage,string p_no_order,
                                   string p_internal_app_rej_id, string p_customer_app_rej_id, string p_final_app_rej_id, string p_knitting_app_rej_id)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            //SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_tracking_list]");
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_new_develop_req_list]");
            try
            {

                if (p_from_date == null || p_from_date == "")
                {
                    command1.Parameters.AddWithValue("@p_from_date", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_from_date", Convert.ToDateTime(p_from_date.Trim()));
                }

                if (p_to_date == null || p_to_date == "")
                {
                    command1.Parameters.AddWithValue("@p_to_date", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_to_date", Convert.ToDateTime(p_to_date.Trim()));
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
                if (p_product_application_id == null || p_product_application_id == "")
                {
                    command1.Parameters.AddWithValue("@p_product_application_id", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_product_application_id", Convert.ToInt32(p_product_application_id.Trim()));
                }
                if (p_product_sub_application_id == null || p_product_sub_application_id == "")
                {
                    command1.Parameters.AddWithValue("@p_product_sub_application_id", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_product_sub_application_id", Convert.ToInt32(p_product_sub_application_id.Trim()));
                }
                if (p_dye_finishing_formula_id == null || p_dye_finishing_formula_id == "")
                {
                    command1.Parameters.AddWithValue("@p_dye_finishing_formula_id", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_dye_finishing_formula_id", Convert.ToInt32(p_dye_finishing_formula_id.Trim()));
                }
                if (p_finishing_id == null || p_finishing_id == "")
                {
                    command1.Parameters.AddWithValue("@p_finishing_id", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_finishing_id", Convert.ToInt32(p_finishing_id.Trim()));
                }
                if (p_pdr_no == null || p_pdr_no == "")
                {
                    command1.Parameters.AddWithValue("@p_pdr_no", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_pdr_no", (p_pdr_no.Trim()));
                }
                if (p_requested_by == null || p_requested_by == "")
                {
                    command1.Parameters.AddWithValue("@p_requested_by", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_requested_by", (p_requested_by.Trim()));
                }
                if (p_customer == null || p_customer == "")
                {
                    command1.Parameters.AddWithValue("@p_customer", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_customer", (p_customer.Trim()));
                }
                if (p_prepared_by == null || p_prepared_by == "")
                {
                    command1.Parameters.AddWithValue("@p_prepared_by", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_prepared_by", (p_prepared_by.Trim()));
                }
                if (p_show_closed_pdr == null || p_show_closed_pdr == "")
                {
                    command1.Parameters.AddWithValue("@p_show_closed_pdr", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_show_closed_pdr", (p_show_closed_pdr.Trim()));
                }
                if (p_internal_app_rej_date_from == null || p_internal_app_rej_date_from == "")
                {
                    command1.Parameters.AddWithValue("@p_internal_app_rej_date_from", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_internal_app_rej_date_from", Convert.ToDateTime(p_internal_app_rej_date_from.Trim()));
                }
                if (p_internal_app_rej_date_to == null || p_internal_app_rej_date_to == "")
                {
                    command1.Parameters.AddWithValue("@p_internal_app_rej_date_to", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_internal_app_rej_date_to", Convert.ToDateTime(p_internal_app_rej_date_to.Trim()));
                }
                if (p_customer_app_rej_date_from == null || p_customer_app_rej_date_from == "")
                {
                    command1.Parameters.AddWithValue("@p_customer_app_rej_date_from", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_customer_app_rej_date_from", Convert.ToDateTime(p_customer_app_rej_date_from.Trim()));
                }
                if (p_customer_app_rej_date_to == null || p_customer_app_rej_date_to == "")
                {
                    command1.Parameters.AddWithValue("@p_customer_app_rej_date_to", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_customer_app_rej_date_to", Convert.ToDateTime(p_customer_app_rej_date_to.Trim()));
                }
                if (p_final_app_rej_date_from == null || p_final_app_rej_date_from == "")
                {
                    command1.Parameters.AddWithValue("@p_final_app_rej_date_from", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_final_app_rej_date_from", Convert.ToDateTime(p_final_app_rej_date_from.Trim()));
                }
                if (p_final_app_rej_date_to == null || p_final_app_rej_date_to == "")
                {
                    command1.Parameters.AddWithValue("@p_final_app_rej_date_to", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_final_app_rej_date_to", Convert.ToDateTime(p_final_app_rej_date_to.Trim()));
                }
                if (p_wait_internal_app_rej == null || p_wait_internal_app_rej == "")
                {
                    command1.Parameters.AddWithValue("@p_wait_internal_app_rej", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_wait_internal_app_rej", (p_wait_internal_app_rej.Trim()));
                }
                if (p_wait_customer_app_rej == null || p_wait_customer_app_rej == "")
                {
                    command1.Parameters.AddWithValue("@p_wait_customer_app_rej", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_wait_customer_app_rej", (p_wait_customer_app_rej.Trim()));
                }
                if (p_wait_final_app_rej == null || p_wait_final_app_rej == "")
                {
                    command1.Parameters.AddWithValue("@p_wait_final_app_rej", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_wait_final_app_rej", (p_wait_final_app_rej.Trim()));
                }
                if (p_add_to_collection == null || p_add_to_collection == "")
                {
                    command1.Parameters.AddWithValue("@p_add_to_collection", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_add_to_collection", (p_add_to_collection.Trim()));
                }
                if (p_new_yarn == null || p_new_yarn == "")
                {
                    command1.Parameters.AddWithValue("@p_new_yarn", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_new_yarn", (p_new_yarn.Trim()));
                }
                if (p_yarn_shortage == null || p_yarn_shortage == "")
                {
                    command1.Parameters.AddWithValue("@p_yarn_shortage", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_yarn_shortage", (p_yarn_shortage.Trim()));
                }
                if (p_no_order == null || p_no_order == "")
                {
                    command1.Parameters.AddWithValue("@p_no_order", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_no_order", (p_no_order.Trim()));
                }
                if (p_internal_app_rej_id == null || p_internal_app_rej_id == "")
                {
                    command1.Parameters.AddWithValue("@p_internal_app_rej_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_internal_app_rej_id", Convert.ToInt64(p_internal_app_rej_id.Trim()));
                }
                if (p_customer_app_rej_id == null || p_customer_app_rej_id == "")
                {
                    command1.Parameters.AddWithValue("@p_customer_app_rej_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_customer_app_rej_id", Convert.ToInt64(p_customer_app_rej_id.Trim()));
                }
                if (p_final_app_rej_id == null || p_final_app_rej_id == "")
                {
                    command1.Parameters.AddWithValue("@p_final_app_rej_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_final_app_rej_id", Convert.ToInt64(p_final_app_rej_id.Trim()));
                }
                if (p_knitting_app_rej_id == null || p_knitting_app_rej_id == "")
                {
                    command1.Parameters.AddWithValue("@p_knitting_app_rej_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_knitting_app_rej_id", Convert.ToInt64(p_knitting_app_rej_id.Trim()));
                }
                DataSet trackinglist = db.ExecuteCommandAndGetDataSet(command1);
                db.closeDBConnection();
                return trackinglist;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }
    }
}
