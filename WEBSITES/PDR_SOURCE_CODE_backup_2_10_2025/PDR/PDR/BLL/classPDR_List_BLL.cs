using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
namespace PDR.BLL
{
    class classPDR_List_BLL
    {
        public DataSet Populate_Item_Category(string p_itnaturecd)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_items_cat]");
            try
            {
                command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataTable PopulateSheetName()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_combo_xl_sheet]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable  PopulateShoeTestMethod()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[poc].[pdr].[p_pdr_combo_test_method]");

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable PopulateShoeSize()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[poc].[pdr].[p_pdr_combo_shoe_size]");

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable PopulateShoeType()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[poc].[pdr].[p_pdr_combo_shoe_type]");

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable PopulateShoeGender()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[poc].[pdr].[p_pdr_combo_shoe_gender]");

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }


        public DataSet Populate_Item_SubCategory(string p_itnaturecd)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_items_subcat]");
            try
            {
                command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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


        public DataSet Populate_Item_Group(string p_itnaturecd)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_items_group]");
            try
            {
                command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataSet Populate_Item_SubGroup(string p_itnaturecd)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_items_sub]");
            try
            {
                command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataTable Populate_Item_Type(string p_itnaturecd)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_items_type]");
            try
            {
                command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable Populate_Item_Sub_Type(string p_itnaturecd)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_items_sub_type]");
            try
            {
                command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }


        public DataTable populateSplFuncList()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_combo_design_spl_func]");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable populateYarnDescList()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_combo_yarn_desc_list]");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable populateFibreTypeList()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_combo_fibre_type]");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }
        public DataTable populateFibreSubTypeList()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_combo_fibre_sub_type]");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable populateSplFunc()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[p_combo_design_spl_func]");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable populateDesignTypeList()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_combo_design_type]");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable populateDesignType()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[p_combo_design_pattern_type]");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable populateFinishing()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("p_combo_finishing");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable populateItemFamilyList()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_combo_design_family_name]");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataTable populateItemFamily()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[PDR].[p_pdr_combo_design_family_name]");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
               DataTable dt = new DataTable();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dt);

                db.closeDBConnection();
                return dt;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataSet populateYarnType()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[p_combo_items_type]");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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


        public DataSet populateYarnSubType()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[p_combo_items_sub2]");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataSet PopulateItemApplList()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_combo_design_appl]");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataSet Populate_Item_Application()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_design_appl]");
            try
            {
               // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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


        public DataSet PopulateItemSubApplList()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_combo_design_sub_appl]");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataSet Populate_Item_SubApplication()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_design_sub_appl]");
            try
            {
                // command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataSet Populate_PDR_REQUESTORS()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_pdr_requesters]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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
        public DataSet populate_PDRUsers()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_pdr_users]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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
        public DataSet Populate_DevelopType()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_pdr_devl_type]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataSet Populate_DesignApplication()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_design_appl]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataSet Populate_YarnFace()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_pdr_yarn_face]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataSet Populate_ItemType(string p_itnaturecd)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_items_type]");
            try
            {
                command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataSet Populate_ItemSubType(string p_itnaturecd)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_items_sub2]");
            try
            {
                 command1.Parameters.AddWithValue("@p_itnaturecd", "FABRIC");
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataSet Populate_SplFunc()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_design_spl_func]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet dsspl = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dsspl);

                db.closeDBConnection();
                return dsspl;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataSet Populate_Zone()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_market_zone]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet dszone = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dszone);

                db.closeDBConnection();
                return dszone;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataSet Populate_Customer()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_market_customer]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet dscustomer = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dscustomer);

                db.closeDBConnection();
                return dscustomer;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }

        public DataSet Populate_POC_Combo_Country()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_country]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet dscountry = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
                adapter.Fill(dscountry);

                db.closeDBConnection();
                return dscountry;
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
                return null;
            }
        }


        public DataSet Populate_Finishing()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_finishing]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataSet Populate_UOM()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_uom_fabric]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataSet Populate_Currency()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_currency]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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

        public DataSet Populate_PDR_Priority()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_pdr_priority]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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
        public DataSet Populate_EndBuyer()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_endbuyers]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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
        public DataSet Populate_Code_Item_Master()
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();

            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[dbo].[p_combo_item_master]");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command1);
                DataSet ds = new DataSet();// = db.ExecuteCommandAndGetDataSet(command1);
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
        public DataTable LoadPDRList(string p_from_date, string p_to_date, string p_itcatid,string p_itsubcatid, string p_itgroupid, string p_itsubid,string p_product_application_id,
                                    string p_product_sub_application_id, string p_dye_finishing_formula_id, string p_finishing_id, string p_pdr_no,
                                    string p_requested_by, string p_customer, string p_prepared_by,string p_show_closed_pdr, string p_internal_app_rej_date_from,
                                   string  p_internal_app_rej_date_to,string p_customer_app_rej_date_from, string p_customer_app_rej_date_to, 
                                   string p_final_app_rej_date_from, string p_final_app_rej_date_to, string  p_wait_internal_app_rej,string p_wait_customer_app_rej, 
                                   string p_wait_final_app_rej, string p_add_to_collection,string p_new_yarn,string p_yarn_shortage,string p_no_order, 
                                   string p_internal_app_rej_id, string p_customer_app_rej_id,string p_final_app_rej_id, string p_knitting_app_rej_id)
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_new_develop_req_list]");
            command1.CommandTimeout = 600;
            try
            {
                if (p_from_date == null || p_from_date == "")
                {
                    command1.Parameters.AddWithValue("@p_from_date", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_from_date",Convert.ToDateTime (p_from_date.Trim()));
                }

                if (p_to_date == null || p_to_date == "")
                {
                    command1.Parameters.AddWithValue("@p_to_date", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@p_to_date",Convert.ToDateTime(p_to_date.Trim()));
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
                    command1.Parameters.AddWithValue("@p_itsubcatid",Convert.ToInt32(p_itsubcatid.Trim()));
                }
                if (p_itgroupid == null || p_itgroupid == "")
                {
                    command1.Parameters.AddWithValue("@p_itgroupid", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_itgroupid",Convert.ToInt32(p_itgroupid.Trim()));
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
                    command1.Parameters.AddWithValue("@p_product_application_id",Convert.ToInt32(p_product_application_id.Trim()));
                }
                if (p_product_sub_application_id == null || p_product_sub_application_id == "")
                {
                    command1.Parameters.AddWithValue("@p_product_sub_application_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_product_sub_application_id",Convert.ToInt32(p_product_sub_application_id.Trim()));
                }
                if (p_dye_finishing_formula_id == null || p_dye_finishing_formula_id == "")
                {
                    command1.Parameters.AddWithValue("@p_dye_finishing_formula_id", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@p_dye_finishing_formula_id",Convert.ToInt32(p_dye_finishing_formula_id.Trim()));
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
        string DesignFromDate = "";
        string DesignToDate = "";
        string subcategoryid = "";
        string categoryid = "";
        string groupid = "";
        string subgroupid = "";
        string applicationid = "";
        string subapplicationid = "";
        string p_no_order = "";

        public DataTable LoadArticleList(String pDesignFromDate,string pDesignToDate, string pCategoryID,string pSubCategoryID, string pGroupID,string pSubGroupID,string pApplicationID,string pSubApplicationID, string pWaitSO,string pCompos, string pDesignFamilyNameID) 
        {
            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_article_list]");

            try
            {
                    if (pDesignFromDate == null || pDesignFromDate == "")
                    {
                        command1.Parameters.AddWithValue("@pDesignFromdate", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@pDesignFromdate", Convert.ToDateTime(pDesignFromDate.Trim()));
                    }

                    if (pDesignToDate == null || pDesignToDate == "")
                    {
                        command1.Parameters.AddWithValue("@pDesignToDate", DBNull.Value);
                    }
                    else
                    {

                        command1.Parameters.AddWithValue("@pDesignToDate", Convert.ToDateTime(pDesignToDate.Trim()));
                    }
                    if (pCategoryID == null || pCategoryID == "")
                    {
                        command1.Parameters.AddWithValue("@pCategoryID", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@pCategoryID", Convert.ToInt32(pCategoryID.Trim()));
                    }
                    if (pSubCategoryID == null || pSubCategoryID == "")
                    {
                        command1.Parameters.AddWithValue("@pSubCategoryID", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@pSubCategoryID", Convert.ToInt32(pSubCategoryID.Trim()));
                    }
                    if (pGroupID == null || pGroupID == "")
                    {
                        command1.Parameters.AddWithValue("@pGroupID", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@pGroupID", Convert.ToInt32(pGroupID.Trim()));
                    }
                    if (pSubGroupID == null || pSubGroupID == "")
                    {
                        command1.Parameters.AddWithValue("@pSubGroupID", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@pSubGroupID", Convert.ToInt32(pSubGroupID.Trim()));
                    }
                    if (pApplicationID == null || pApplicationID == "")
                    {
                        command1.Parameters.AddWithValue("@pApplicationID", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@pApplicationID", Convert.ToInt32(pApplicationID.Trim()));
                    }
                    if (pSubApplicationID == null || pSubApplicationID == "")
                    {
                        command1.Parameters.AddWithValue("@pSubApplicationID", DBNull.Value);
                    }
                    else
                    {
                        command1.Parameters.AddWithValue("@pSubApplicationID", Convert.ToInt32(pSubApplicationID.Trim()));
                    }

                command1.Parameters.AddWithValue("@pWaitSO", pWaitSO.Trim());

                if (pCompos == null || pCompos == "")
                {
                    command1.Parameters.AddWithValue("@pCompos", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pCompos", pCompos.Trim());
                }


                if (pDesignFamilyNameID == null || pDesignFamilyNameID == "")
                {
                    command1.Parameters.AddWithValue("@pDesignFamilyNameID", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pDesignFamilyNameID", Convert.ToInt32(pDesignFamilyNameID.Trim()));
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

        public DataTable LoadArticleListMulti(  string pCompos, string pArticleName, String pDesignFromDate, string pDesignToDate, 
                                                string pWidthFrom, string pWidthTo, string pWeightFrom, string pWeightTo,
                                                string pCategoryID, string pSubCategoryID, string pGroupID, string pSubGroupID, string pItemTypeId, string pItemSubTypeId, 
                                                string pWaitSO,  string pApplList, string pSubApplList, string pDesignTypeList, string pSplFuncList, string pFamilyList,  string pFinishList ,
                                                string pGauge, string pXLSheetName, string pKindOfYarnList)
        {

            /*
                            @pCompos varchar(500)=NULL,
                            @pArticleName nvarchar(100)=NULL,
                            @pDesignFromdate datetime='20100101',
                            @pDesignToDate datetime='20500101',
                            @pWeightFrom numeric(12,2)=0,
                            @pWeightTo numeric(12,2)=9999,
                            @pWidthFrom numeric(12,2)=0,
                            @pWidthTo numeric(12,2)=9999,
                            @pCategoryID bigint=NULL,
                            @pSubCategoryId bigint=null,
                            @pGroupID bigint=null,
                            @pSubGroupID bigint=null,
                            @pTypeID bigint=null,
                            @pSubTypeID bigint=null,
                            @pFinishingID bigint=null,

                            @pApplicationID bigint=null,
                            @pSubApplicationId bigint=null,
                            @pWaitSO CHAR(1)=NULL,

                        --	@pDesignFamilyNameID bigint=null, -- not used

                            @pApplList varchar(100)=null,
                            @pSubApplList varchar(100)=null,
                            @pSplFuncList varchar(100)=null,
                            @pDesignTypeList varchar(100)=null,
                            @pFamilyList varchar(100)=null
                            */

            ConnectionDB db = ConnectionDB.getDbInstance();
            db.ConnectToDB();
            SqlCommand command1 = (SqlCommand)db.GetCommandToSetParameters("[pdr].[p_pdr_article_list_multi]");

            try
            {
                if (pCompos == null || pCompos == "")
                {
                    command1.Parameters.AddWithValue("@pCompos", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pCompos", pCompos.Trim());
                }

                if (pArticleName == null || pArticleName == "")
                {
                    command1.Parameters.AddWithValue("@pArticleName", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pArticleName", pArticleName.Trim());
                }

                if (pWidthFrom == null || pWidthFrom == "")
                {
                    command1.Parameters.AddWithValue("@pWidthFrom", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pWidthFrom", pWidthFrom.Trim());
                }

                if (pWidthTo == null || pWidthTo == "")
                {
                    command1.Parameters.AddWithValue("@pWidthTo", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pWidthTo", pWidthTo.Trim());
                }


                if (pWeightFrom == null || pWeightFrom == "")
                {
                    command1.Parameters.AddWithValue("@pWeightFrom", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pWeightFrom", pWeightFrom.Trim());
                }

                if (pWeightTo == null || pWeightTo == "")
                {
                    command1.Parameters.AddWithValue("@pWeightTo", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pWeightTo", pWeightTo.Trim());
                }


                if (pDesignFromDate == null || pDesignFromDate == "")
                {
                    command1.Parameters.AddWithValue("@pDesignFromdate", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pDesignFromdate", Convert.ToDateTime(pDesignFromDate.Trim()));
                }

                if (pDesignToDate == null || pDesignToDate == "")
                {
                    command1.Parameters.AddWithValue("@pDesignToDate", DBNull.Value);
                }
                else
                {

                    command1.Parameters.AddWithValue("@pDesignToDate", Convert.ToDateTime(pDesignToDate.Trim()));
                }

                if (pGauge == null || pGauge == "")
                {
                    command1.Parameters.AddWithValue("@pGauge", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pGauge", pGauge.Trim());
                }

                if (pCategoryID == null || pCategoryID == "")
                {
                    command1.Parameters.AddWithValue("@pCategoryID", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pCategoryID", Convert.ToInt32(pCategoryID.Trim()));
                }

                if (pSubCategoryID == null || pSubCategoryID == "")
                {
                    command1.Parameters.AddWithValue("@pSubCategoryID", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pSubCategoryID", Convert.ToInt32(pSubCategoryID.Trim()));
                }

                if (pGroupID == null || pGroupID == "")
                {
                    command1.Parameters.AddWithValue("@pGroupID", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pGroupID", Convert.ToInt32(pGroupID.Trim()));
                }

                if (pSubGroupID == null || pSubGroupID == "")
                {
                    command1.Parameters.AddWithValue("@pSubGroupID", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pSubGroupID", Convert.ToInt32(pSubGroupID.Trim()));
                }
                command1.Parameters.AddWithValue("@pWaitSO", pWaitSO.Trim());

                if (pApplList    == null || pApplList == "")
                {
                    command1.Parameters.AddWithValue("@pApplList", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pApplList", pApplList);
                }

                if (pSubApplList == null || pSubApplList == "")
                {
                    command1.Parameters.AddWithValue("@pSubApplList", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pSubApplList",pSubApplList);
                }

                if (pSplFuncList == null || pSplFuncList == "")
                {
                    command1.Parameters.AddWithValue("@pSplFuncList", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pSplFuncList", pSplFuncList);
                }


                if (pFamilyList == null || pFamilyList == "")
                {
                    command1.Parameters.AddWithValue("@pFamilyList", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pFamilyList", pFamilyList);
                }

                if (pDesignTypeList == null || pDesignTypeList == "")
                {
                    command1.Parameters.AddWithValue("@pDesignTypeList", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pDesignTypeList", pDesignTypeList);
                }

                if (pFinishList == null || pFinishList == "")
                {
                    command1.Parameters.AddWithValue("@pFinishList", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pFinishList", pFinishList);
                }

                if (pXLSheetName == null || pXLSheetName == "")
                {
                    command1.Parameters.AddWithValue("@pXLSheetName", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pXLSheetName", pXLSheetName.Trim());
                }

                if (pKindOfYarnList== null || pKindOfYarnList == "")
                {
                    command1.Parameters.AddWithValue("@pKindOfYarnList", DBNull.Value);
                }
                else
                {
                    command1.Parameters.AddWithValue("@pKindOfYarnList", pKindOfYarnList.Trim());
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
    }
}