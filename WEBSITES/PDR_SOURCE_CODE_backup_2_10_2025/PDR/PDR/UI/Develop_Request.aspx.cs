using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PDR.BLL;
using System.IO;
using System.Web.Services;
using System.Drawing;
using System.Web.Script.Services;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Reporting.WebForms;
namespace PDR.UI
{
    public partial class Develop_Request : System.Web.UI.Page
    {
        DataTable dtpdrlist;
        DataTable dtprdlistcopy = new DataTable();
        string valuechanged = "NO";
        public DataSet dsapplication;
        public DataSet dssubapplication;
        DataSet dssplfunc;
        DataSet dscountry;
        DataSet dszone;
        DataSet dscustomer;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                string pdrno = "";
                if (Request.QueryString["pdrno"] != null)
                {
                    pdrno = Request.QueryString["pdrno"];//.ToString();
                }
                Response.Redirect("~/ui/login.aspx?pagename=develop_request" + "&pdrno=" + pdrno.Trim());
            }
            if (!IsPostBack)
            {
                lblErrorMSg.Visible = false;
                populate_ItemCategory();
                populate_ItemSubCategory();
                populate_ItemGroups();
                populate_ItemSubGroups();
                populate_ItemType();
                populate_ItemSubType();
                populate_PDRRequestor();
                populate_PDRDevType();
                populate_YarnFace();
                // populate_DesignApplication();
                populate_PDR_Priority();
                populate_Code_Item_Master();
                populate_EndBuyer();
                populate_Finishing();
                populate_UOM();
                populate_Currency();
                populate_ItemApplication();
                populate_ItemSubApplication();
                populate_SplFunc();
                populate_Zone();
                populate_Customer();
                populate_Country();
                populateShoeTestMethod();
                populateShoeSize();
//                populateShoeType();
                populateShoeGender();

                string pdrno = "";
                // txtRequestedBy.Text = Session["username"].ToString();
                if (Request.QueryString["pdrno"] != null)
                {
                    pdrno = Request.QueryString["pdrno"];//.ToString();
                }
                if (pdrno != "")
                {
                    classDevelop_Request_BLL developRequest = new classDevelop_Request_BLL();
                    dtpdrlist = developRequest.LoadPDRList(pdrno);
                    //  dtprdlistcopy = dtpdrlist;
                    ViewState["pdrlist"] = dtpdrlist;
                    string followcustomerspec = "";
                    try
                    {
                        if (dtpdrlist != null)
                        {
                            foreach (DataRow row in dtpdrlist.Rows)
                            {

                                hidpdr_new_develop_req_id.Value = row["pdr_new_develop_req_id"].ToString();
                                txtPDRNO.Text = (row["pdr_no"].ToString());
                                lblCancel.Text = (row["pdr_cancel_label"].ToString());
                                hidPDRNOCancel.Value = (row["pdr_cancel"].ToString());
                                hidavilToEmailID.Value = row["yarn_avail_alert_email"].ToString();
                                if (row["pdr_cancel"].ToString().Trim() == "Y")
                                {
                                    ImgSave.Enabled = false;
                                }
                                else
                                {
                                    ImgSave.Enabled = true;
                                }
                                txtPrepareBy.Text = (row["preparer_name"].ToString());
                                if (!string.IsNullOrEmpty(row["pdr_date"].ToString()))
                                {
                                    DateTime dt = Convert.ToDateTime(row["pdr_date"].ToString());
                                    PDRDate.Value = String.Format("{0:dd-MM-yyyy}", dt);// dt.ToString("dd-MM-yyyy");// String.Format("M/d/yyyy", dt);// Convert.ToDateTime(row["pdr_date"]).ToString();
                                }
                                if (!string.IsNullOrEmpty(row["design_no"].ToString()))
                                {
                                    txtDesignno.Text = row["design_no"].ToString();
                                }
                                if (!string.IsNullOrEmpty(row["customer_name"].ToString()))
                                {
                                    txtSearchMain.Text = row["customer_name"].ToString();
                                }
                                if (!string.IsNullOrEmpty(row["custcd"].ToString()))
                                {
                                    txtCustCode.Text = row["custcd"].ToString();
                                    hidSearchMainValue.Value = row["custcd"].ToString();
                                }

                                ddlPDRRequestor.SelectedValue = row["requested_by"].ToString();
                                ddlSubCat.SelectedValue = row["itsubcatid"].ToString();
                                ddlgroup.SelectedValue = row["itgroupid"].ToString();
                                ddlCategory.SelectedValue = row["itcatid"].ToString();

                                ddlSubGroup.SelectedValue = row["itsubid"].ToString();
                                ddlType.SelectedValue = row["ittypeid"].ToString();
                                ddlSubType.SelectedValue = row["itsubid2"].ToString();
                                txtDevelopmentTypeRemark.Text = row["development_type_remark"].ToString();
                                ddlPriority.SelectedValue = row["priority_id"].ToString();
                                txtPriorityComment.Text = row["priority_comment"].ToString();
                                ddlcombocurrency.SelectedValue = row["price_curr"].ToString().Trim();
                                ddlCode1.SelectedValue = row["itcd1"].ToString();
                                ddlCode2.SelectedValue = row["itcd2"].ToString();
                                ddlCode3.SelectedValue = row["itcd3"].ToString();
                                ddlCode4.SelectedValue = row["itcd4"].ToString();
                                ddlCode5.SelectedValue = row["itcd5"].ToString();
                                ddlCode6.SelectedValue = row["itcd6"].ToString();
                                ddlEndBuyer.SelectedValue = row["endbuyerid"].ToString();

                                if (!string.IsNullOrEmpty(row["follow_customer_spec"].ToString()))
                                {
                                    followcustomerspec = row["follow_customer_spec"].ToString();
                                    if (followcustomerspec == "N")
                                    {
                                        CHKFollow.Checked = false;

                                    }

                                    else
                                    {
                                        CHKFollow.Checked = true;
                                    }
                                }
                                else
                                {
                                    CHKFollow.Checked = false;

                                }

                                txtDevelopmentTypeRemark.Text = row["development_type_remark"].ToString();
                                txtComposition1_name.Text = row["composition1_name"].ToString();
                                txtComposition2_name.Text = row["composition2_name"].ToString();
                                txtComposition3_name.Text = row["composition3_name"].ToString();
                                txtComposition4_name.Text = row["composition4_name"].ToString();
                                txtComposition5_name.Text = row["composition5_name"].ToString();
                                txtComposition6_name.Text = row["composition6_name"].ToString();
                                txtweight.Text = row["weight_sqm"].ToString();
                                TXTWIDTH.Text = row["width"].ToString();
                                TXTElongation.Text = row["elongation"].ToString();
                                TXTMODULUS.Text = row["modulus"].ToString();
                                // txtRecovStretbility.Text = row["stretch"].ToString();
                                // txtDimStbilityShr.Text = row["shrinkage"].ToString();
                                txtremark.Text = row["remark"].ToString();
                                txtweightTolerance.Text = row["weight_sqm_tolerance"].ToString();
                                txtwidth_Tolerance.Text = row["width_tolerance"].ToString();
                                txtpilingremark.Text = row["pilling_remark"].ToString();
                                txtsnaggingremark.Text = row["snagging_remark"].ToString();
                                txtdyeincolor.Text = row["color_name"].ToString();
                                txtMachineTense.Text = row["machine_dense"].ToString();
                                // if (row["knitting_appointment"] != null || row["knitting_appointment"].ToString() != "{}")
                                if (!string.IsNullOrEmpty(row["knitting_appointment"].ToString()))
                                {

                                    DateTime dt1 = Convert.ToDateTime(row["knitting_appointment"].ToString());
                                    txtknittingappoinment.Value = String.Format("{0:dd-MM-yyyy}", dt1);// row["knitting_appointment"].ToString();
                                }
                                if (!string.IsNullOrEmpty(row["yarn_available_date"].ToString()))
                                {
                                    DateTime dt2 = Convert.ToDateTime(row["yarn_available_date"].ToString());
                                    txtyarnavailabledate.Value = String.Format("{0:dd-MM-yyyy}", dt2);// row["yarn_available_date"].ToString();
                                }
                                if (!string.IsNullOrEmpty(row["expected_finished_date"].ToString()))
                                {
                                    DateTime dt3 = Convert.ToDateTime(row["expected_finished_date"].ToString());
                                    txtExpectedFinisheddate.Value = String.Format("{0:dd-MM-yyyy}", dt3);//row["expected_finished_date"].ToString();
                                }
                                if (!string.IsNullOrEmpty(row["spec_master_date"].ToString()))
                                {
                                    DateTime dt4 = Convert.ToDateTime(row["spec_master_date"].ToString());
                                    txtspecmasterdate.Value = String.Format("{0:dd-MM-yyyy}", dt4);//row["spec_master_date"].ToString();
                                }
                                if (!string.IsNullOrEmpty(row["qa_report_date"].ToString()))
                                {
                                    DateTime dt5 = Convert.ToDateTime(row["qa_report_date"].ToString());
                                    txtqa_reportdate.Value = String.Format("{0:dd-MM-yyyy}", dt5);//= row["qa_report_date"].ToString();
                                }

                                txtStandard_Customer.Text = row["test_method_text"].ToString();
                                txtCustPriceExpectation.Text = row["expected_price"].ToString();
                                txtPercent1.Text = row["composition1_percent"].ToString();
                                txtPercent2.Text = row["composition2_percent"].ToString();
                                txtPercent3.Text = row["composition3_percent"].ToString();
                                txtPercent4.Text = row["composition4_percent"].ToString();
                                txtPercent5.Text = row["composition5_percent"].ToString();
                                txtPercent6.Text = row["composition6_percent"].ToString();

                                txtAvail1.Text = row["yarn_avail_qty1"].ToString();
                                txtAvail2.Text = row["yarn_avail_qty2"].ToString();
                                txtAvail3.Text = row["yarn_avail_qty3"].ToString();
                                txtAvail4.Text = row["yarn_avail_qty4"].ToString();
                                txtAvail5.Text = row["yarn_avail_qty5"].ToString();
                                txtAvail6.Text = row["yarn_avail_qty6"].ToString();

                                TXTGUAGE.Text = row["gauge"].ToString();
                                if (!string.IsNullOrEmpty(row["yarn_face1"].ToString()))
                                {
                                    ddlFront.SelectedValue = row["yarn_face1"].ToString().Trim();
                                }
                                if (!string.IsNullOrEmpty(row["yarn_face2"].ToString()))
                                {
                                    ddlBack.SelectedValue = row["yarn_face2"].ToString().Trim();
                                }
                                if (!string.IsNullOrEmpty(row["yarn_face3"].ToString()))
                                {
                                    ddlfront1.SelectedValue = row["yarn_face3"].ToString().Trim();
                                }
                                if (!string.IsNullOrEmpty(row["yarn_face4"].ToString()))
                                {
                                    ddlfront2.SelectedValue = row["yarn_face4"].ToString().Trim();
                                }
                                if (!string.IsNullOrEmpty(row["yarn_face5"].ToString()))
                                {
                                    ddlfront5.SelectedValue = row["yarn_face5"].ToString().Trim();
                                }
                                if (!string.IsNullOrEmpty(row["yarn_face6"].ToString()))
                                {
                                    ddlfront6.SelectedValue = row["yarn_face6"].ToString().Trim();
                                }
                                if (!string.IsNullOrEmpty(row["dye_finishing_formula_id"].ToString()))
                                {
                                    ddlFinishing.SelectedValue = row["dye_finishing_formula_id"].ToString().Trim();
                                }
                                if (!string.IsNullOrEmpty(row["require_tag"].ToString()))
                                {
                                    string gangtagrequired = row["require_tag"].ToString();
                                    if (gangtagrequired == "N" || gangtagrequired == "")
                                    {
                                        chkHangTagRequire.Checked = false;
                                    }
                                    else
                                    {
                                        chkHangTagRequire.Checked = true;
                                    }
                                }
                                TXTYARDAGE.Text = row["yardage"].ToString();
                                string snagging = row["snagging"].ToString();
                                if (snagging == "N")
                                {
                                    chksnagging.Checked = false;
                                }
                                else
                                {
                                    chksnagging.Checked = true;
                                }
                                string pilling = row["piling"].ToString();
                                if (pilling == "N")
                                {
                                    chkpilling.Checked = false;
                                }
                                else
                                {
                                    chkpilling.Checked = true;
                                }

                                string standard_aatcc = row["standard_aatcc"].ToString();
                                if (standard_aatcc == "N" || standard_aatcc == "0")
                                {
                                    CHKAATCC.Checked = false;
                                }
                                else
                                {
                                    CHKAATCC.Checked = true;
                                }
                                string standard_astm = row["standard_astm"].ToString();
                                if (standard_astm == "N" || standard_astm == "0")
                                {
                                    CHKASTM.Checked = false;
                                }
                                else
                                {
                                    CHKASTM.Checked = true;
                                }

                                string standard_iso = row["standard_iso"].ToString();
                                if (standard_iso == "N" || standard_iso == "0")
                                {
                                    CHKISO.Checked = false;
                                }
                                else
                                {
                                    CHKISO.Checked = true;
                                }

                                string standard_jis = row["standard_jis"].ToString();
                                if (standard_jis == "N" || standard_jis == "0")
                                {
                                    CHKJIS.Checked = false;
                                }
                                else
                                {
                                    CHKJIS.Checked = true;
                                }
                                string standard_ms = row["standard_ms"].ToString();
                                if (standard_ms == "N" || standard_ms == "0" || standard_ms == "F")
                                {
                                    chkMS.Checked = false;
                                }
                                else
                                {
                                    chkMS.Checked = true;
                                }
                                string standard_hm = row["standard_hm"].ToString();
                                if (standard_hm == "N" || standard_hm == "0" || standard_ms == "F")
                                {
                                    chkHM.Checked = false;
                                }
                                else
                                {
                                    chkHM.Checked = true;
                                }
                                string standard_vss = row["standard_vss"].ToString();
                                if (standard_vss == "N" || standard_vss == "0" || standard_ms == "F")
                                {
                                    chkVSS.Checked = false;
                                }
                                else
                                {
                                    chkVSS.Checked = true;
                                }

                                txtElongation_l.Text = row["elongation_l"].ToString();
                                txtmodulus_l.Text = row["modulus_l"].ToString();
                                txtstretch_l.Text = row["stretch_l"].ToString();
                                txtshrinkage_l.Text = row["shrinkage_l"].ToString();
                                txtElongation_w.Text = row["elongation_w"].ToString();
                                txtmodulus_w.Text = row["modulus_w"].ToString();
                                txtstretch_w.Text = row["stretch_w"].ToString();
                                txtshrinkage_w.Text = row["shrinkage_w"].ToString();
                                txtelongation_tolerance.Text = row["elongation_tolerance"].ToString();
                                txtmodulus_tolerance.Text = row["modulus_tolerance"].ToString();
                                txtshrinkage_tolerance.Text = row["shrinkage_tolerance"].ToString();
                                txtstretch_tolerance.Text = row["stretch_tolerance"].ToString();
                                ddlReasonForDevl.SelectedValue = row["development_type_id"].ToString();
                                // ddlPurposeofGarment.SelectedValue = row["product_application_id"].ToString();
                                if (!string.IsNullOrEmpty(row["expected_date"].ToString()))
                                {
                                    DateTime dt6 = Convert.ToDateTime(row["expected_date"].ToString());
                                    dtTo.Value = String.Format("{0:dd-MM-yyyy}", dt6);//= row["qa_report_date"].ToString();
                                                                                      //dtTo.Value = row["expected_date"].ToString();
                                }

                                txtSampleRequire.Text = row["hanger"].ToString();
                                txtStandard_Customer.Text = row["standard_customer"].ToString();
                                //txtComment.Text = 
                                string yarnavailable = row["yarn_available"].ToString();
                                if (yarnavailable == "N")
                                {
                                    chkYarnAvailable.Checked = false;
                                }
                                else
                                {
                                    chkYarnAvailable.Checked = true;
                                }

                                txtweightTolerance.Text = row["weight_sqm_tolerance"].ToString();
                                txtwidth_Tolerance.Text = row["width_tolerance"].ToString();
                                // fileField.Value= row["width_tolerance"].ToString();
                                //avatarUpload.Value= row["file_location"].ToString();
                                //lblFileName.Text = row["file_location"].ToString();
                                txtAppRej.Text = row["pdr_app_rej_status"].ToString().Trim();

                                txtBy.Text = row["pdr_approver_name"].ToString();
                                if (!string.IsNullOrEmpty(row["pdr_app_rej_date"].ToString()))
                                {
                                    DateTime dt7 = Convert.ToDateTime(row["pdr_app_rej_date"].ToString());
                                    txtAppRejDate.Text = String.Format("{0:dd-MM-yyyy}", dt7);//= row["qa_report_date"].ToString();
                                                                                              //dtTo.Value = row["expected_date"].ToString();
                                }

                                //txtAppRejDate.Text= row["pdr_app_rej_date"].ToString();
                                txtAppRejComment.Text = row["pdr_app_rej_comment"].ToString();

                                if (!string.IsNullOrEmpty(row["kg_per_finished_roll"].ToString()))
                                {
                                    txtkgperfinishedroll.Text = (row["kg_per_finished_roll"].ToString());
                                }
                                if (!string.IsNullOrEmpty(row["no_of_finished_rolls"].ToString()))
                                {
                                    txtTotalFinishedRolls.Text = (row["no_of_finished_rolls"].ToString());
                                }

                                if (!string.IsNullOrEmpty(row["yarn_shortage1"].ToString()))
                                {
                                    string yarn_shortage1 = row["yarn_shortage1"].ToString();
                                    if (yarn_shortage1 == "N" || yarn_shortage1 == "")
                                    {
                                        chkshortage1.Checked = false;
                                    }
                                    else
                                    {
                                        chkshortage1.Checked = true;
                                    }
                                }

                                if (!string.IsNullOrEmpty(row["yarn_shortage2"].ToString()))
                                {
                                    string yarn_shortage2 = row["yarn_shortage2"].ToString();
                                    if (yarn_shortage2 == "N" || yarn_shortage2 == "")
                                    {
                                        chkshortage2.Checked = false;
                                    }
                                    else
                                    {
                                        chkshortage2.Checked = true;
                                    }
                                }
                                if (!string.IsNullOrEmpty(row["yarn_shortage3"].ToString()))
                                {
                                    string yarn_shortage3 = row["yarn_shortage3"].ToString();
                                    if (yarn_shortage3 == "N" || yarn_shortage3 == "")
                                    {
                                        chkshortage3.Checked = false;
                                    }
                                    else
                                    {
                                        chkshortage3.Checked = true;
                                    }
                                }
                                if (!string.IsNullOrEmpty(row["yarn_shortage4"].ToString()))
                                {
                                    string yarn_shortage4 = row["yarn_shortage4"].ToString();
                                    if (yarn_shortage4 == "N" || yarn_shortage4 == "")
                                    {
                                        chkshortage4.Checked = false;
                                    }
                                    else
                                    {
                                        chkshortage4.Checked = true;
                                    }
                                }
                                if (!string.IsNullOrEmpty(row["yarn_shortage5"].ToString()))
                                {
                                    string yarn_shortage5 = row["yarn_shortage5"].ToString();
                                    if (yarn_shortage5 == "N" || yarn_shortage5 == "")
                                    {
                                        chkshortage5.Checked = false;
                                    }
                                    else
                                    {
                                        chkshortage5.Checked = true;
                                    }
                                }
                                if (!string.IsNullOrEmpty(row["yarn_shortage6"].ToString()))
                                {
                                    string yarn_shortage6 = row["yarn_shortage6"].ToString();
                                    if (yarn_shortage6 == "N" || yarn_shortage6 == "")
                                    {
                                        chkshortage6.Checked = false;
                                    }
                                    else
                                    {
                                        chkshortage6.Checked = true;
                                    }
                                }
                                if (!string.IsNullOrEmpty(row["new_yarn1"].ToString()))
                                {
                                    string new_yarn1 = row["new_yarn1"].ToString();
                                    if (new_yarn1 == "N" || new_yarn1 == "")
                                    {
                                        chknewyarn1.Checked = false;
                                    }
                                    else
                                    {
                                        chknewyarn1.Checked = true;
                                    }
                                }
                                if (!string.IsNullOrEmpty(row["new_yarn2"].ToString()))
                                {
                                    string new_yarn2 = row["new_yarn2"].ToString();
                                    if (new_yarn2 == "N" || new_yarn2 == "")
                                    {
                                        chknewyarn2.Checked = false;
                                    }
                                    else
                                    {
                                        chknewyarn2.Checked = true;
                                    }
                                }
                                if (!string.IsNullOrEmpty(row["new_yarn3"].ToString()))
                                {
                                    string new_yarn3 = row["new_yarn3"].ToString();
                                    if (new_yarn3 == "N" || new_yarn3 == "")
                                    {
                                        chknewyarn3.Checked = false;
                                    }
                                    else
                                    {
                                        chknewyarn3.Checked = true;
                                    }
                                }
                                if (!string.IsNullOrEmpty(row["new_yarn4"].ToString()))
                                {
                                    string new_yarn4 = row["new_yarn4"].ToString();
                                    if (new_yarn4 == "N" || new_yarn4 == "")
                                    {
                                        chknewyarn4.Checked = false;
                                    }
                                    else
                                    {
                                        chknewyarn4.Checked = true;
                                    }
                                }
                                if (!string.IsNullOrEmpty(row["new_yarn5"].ToString()))
                                {
                                    string new_yarn5 = row["new_yarn5"].ToString();
                                    if (new_yarn5 == "N" || new_yarn5 == "")
                                    {
                                        chknewyarn5.Checked = false;
                                    }
                                    else
                                    {
                                        chknewyarn5.Checked = true;
                                    }
                                }
                                if (!string.IsNullOrEmpty(row["new_yarn6"].ToString()))
                                {
                                    string new_yarn6 = row["new_yarn6"].ToString();
                                    if (new_yarn6 == "N" || new_yarn6 == "")
                                    {
                                        chknewyarn6.Checked = false;
                                    }
                                    else
                                    {
                                        chknewyarn6.Checked = true;
                                    }
                                }

                                if (!string.IsNullOrEmpty(row["standard_test_method_id"].ToString()))
                                {
                                    string test_method_id = row["standard_test_method_id"].ToString();
                                    ddlTestMethod.SelectedValue = test_method_id;
                                }


                                if (!string.IsNullOrEmpty(row["shoe_size_id"].ToString()))
                                {
                                    string shoe_size_id = row["shoe_size_id"].ToString();
                                        ddlShoeSize.SelectedValue = shoe_size_id;
                                }

                                if (!string.IsNullOrEmpty(row["shoe_Style"].ToString()))
                                {
                                    string shoe_Style = row["shoe_Style"].ToString();
                                    txtShoeStyle.Text = shoe_Style;
                                }

                                if (!string.IsNullOrEmpty(row["shoe_gender_id"].ToString()))
                                {
                                    string shoe_gender_id = row["shoe_gender_id"].ToString();
                                    ddlShoeGender.SelectedValue = shoe_gender_id;
                                }

                                txtPalletPatternNo.Text = row["pallet_pattern_no"].ToString();
                                txtProductPatternNo.Text = row["product_pattern_no"].ToString();
                                txtPairs.Text= row["shoe_pairs_required"].ToString();
                                txtShoeLength.Text= row["shoe_length_cm"].ToString();
                                txtShoeWidth.Text = row["shoe_width_cm"].ToString();

                                string requireLaserCut = row["shoe_require_laser_cut"].ToString();
                                if (requireLaserCut == "N" || requireLaserCut == "0")
                                {
                                    chkLaserCut.Checked = false;
                                }
                                else
                                {
                                    chkLaserCut.Checked = true;
                                }

                                string WithPattern = row["shoe_laser_cut_with_pattern"].ToString();
                                if (WithPattern == "N" || WithPattern == "0")
                                {
                                    chkWithPattern.Checked = false;
                                }
                                else
                                {
                                    chkWithPattern.Checked = true;
                                }
                                txtRptPerRoll.Text = row["repeat_per_roll"].ToString();

                            }


                            // load the grid
                            DataTable dtdevreq;
                            classDevelop_Request_BLL devrequest = new classDevelop_Request_BLL();
                            dtdevreq = devrequest.LoadDeveRequestApp(hidpdr_new_develop_req_id.Value.ToString().Trim());
                            gvDevelop_Request.DataSource = dtdevreq;
                            gvDevelop_Request.DataBind();
                            ViewState["pdrDevReq"] = dtdevreq;

                            if (dtdevreq.Rows.Count <= 0)
                            {
                                BindHeaderRowInGridview();
                            }
                            //
                        }
                    }
                    catch (Exception ex)
                    {
                        lblErrorMSg.Visible = true;
                        lblErrorMSg.Text = ex.Message.ToString();
                    }
                }
                else
                {
                    txtPDRNO.Text = "NEW";
                    txtPDRNO.Enabled = true;
                    DateTime dt = DateTime.Now.AddDays(-70);
                    DateTime dt1 = DateTime.Now;
                    PDRDate.Value = dt.ToString("dd/MM/yyyy").Replace('-', '/');// DateTime.Now.AddDays(-70).ToString();//.ToShortDateString();

                    PDRDate.Value = dt1.ToString("dd/MM/yyyy").Replace('-', '/');
                    ddlcombocurrency.SelectedValue = "THB";
                    ddlCategory.SelectedItem.Text = "KNITTED";
                    ddlSubCat.SelectedItem.Text = "CIRCULAR";


                }
            }
            else  // postback
            {
                if (hidSearchMainValue.Value != "")
                {
                    txtCustCode.Text = hidSearchMainValue.Value.Trim();
                }
                // populate_ItemApplication();
                //populate_ItemSubApplication();
                //populate_SplFunc();
                //populate_Zone();
                //populate_Customer();
                //populate_Country();
                // classPDR_List_BLL itemapplications = new classPDR_List_BLL();
                //  dsapplication = itemapplications.Populate_Item_Application();
            }

        }

        private void BindHeaderRowInGridview()
        {
            DataTable dt = new DataTable();
            //Add columns to datatable
            dt.Columns.Add("ddlAppl");//chkdelete
            dt.Columns.Add("ddlSubAppl");
            dt.Columns.Add("ddlsplfunc");
            dt.Columns.Add("ddlctry");
            dt.Columns.Add("ddlzone");
            dt.Columns.Add("ddlcustomer");
            dt.Columns.Add("pdr_new_develop_req_id");
            dt.Columns.Add("pdr_item_properties_id");
            // dt.Columns.Add("pdr_new_develop_req_id");
            dt.Columns.Add("appl_id");
            dt.Columns.Add("sub_appl_id");
            dt.Columns.Add("spl_func_id");
            dt.Columns.Add("ctry");
            dt.Columns.Add("market_zone_id");
            dt.Columns.Add("market_customer_id");
            //Add row to datatable
            DataRow dr = null;
            dr = dt.NewRow();

            dt.Rows.Add(dr);
            //Bind row having 5 columns to gridview as a Gridview's header row.

            gvDevelop_Request.DataSource = dt;
            gvDevelop_Request.DataBind();
            ViewState["pdrDevReq"] = dt;
        }


        private void populate_ItemCategory()
        {
            DataSet dscategory;
            classPDR_List_BLL itemcategory = new classPDR_List_BLL();
            dscategory = itemcategory.Populate_Item_Category("FABRIC");
            ddlCategory.DataSource = dscategory;
            ddlCategory.DataTextField = "itcatdesc";
            ddlCategory.DataValueField = "itcatid";
            ddlCategory.DataBind();
            ddlCategory.SelectedValue = "";
        }

        private void populateShoeTestMethod()
        {
            DataTable dtTestMethod;
            classPDR_List_BLL classPDRList = new classPDR_List_BLL();
            dtTestMethod = classPDRList.PopulateShoeTestMethod();
            ddlTestMethod.DataSource = dtTestMethod;
            ddlTestMethod.DataTextField = "lookup_value";
            ddlTestMethod.DataValueField = "lookup_value_id";
            ddlTestMethod.DataBind();
            ddlTestMethod.SelectedValue = "";
        }

        private void populateShoeSize()
        {
            DataTable dtShoeSize;
            classPDR_List_BLL classPDRList = new classPDR_List_BLL();
            dtShoeSize = classPDRList.PopulateShoeSize();
            ddlShoeSize.DataSource = dtShoeSize;
            ddlShoeSize.DataTextField = "lookup_value";
            ddlShoeSize.DataValueField = "lookup_value_id";
            ddlShoeSize.DataBind();
            ddlShoeSize.SelectedValue = "";
        }

//        private void populateShoeType()
 //       {
  //          DataTable dtShoeType;
   //         classPDR_List_BLL classPDRList = new classPDR_List_BLL();
    //        dtShoeType = classPDRList.PopulateShoeType();
//            ddlShoeType.DataSource = dtShoeType;
//            ddlShoeType.DataTextField = "lookup_value";
//            ddlShoeType.DataValueField = "lookup_value_id";
//            ddlShoeType.DataBind();
//            ddlShoeType.SelectedValue = "";
//        }

        private void populateShoeGender()
        {
            DataTable dtShoeGender;
            classPDR_List_BLL classPDRList = new classPDR_List_BLL();
            dtShoeGender = classPDRList.PopulateShoeGender();
            ddlShoeGender.DataSource = dtShoeGender;
            ddlShoeGender.DataTextField = "lookup_value";
            ddlShoeGender.DataValueField = "lookup_value_id";
            ddlShoeGender.DataBind();
            ddlShoeGender.SelectedValue = "";
        }


        private void populate_ItemSubCategory()
        {
            DataSet dssubcategory;
            classPDR_List_BLL itemsubcategory = new classPDR_List_BLL();
            dssubcategory = itemsubcategory.Populate_Item_SubCategory("FABRIC");
            ddlSubCat.DataSource = dssubcategory;
            ddlSubCat.DataTextField = "itsubcatdesc";
            ddlSubCat.DataValueField = "itsubcatid";
            ddlSubCat.DataBind();
            ddlSubCat.SelectedValue = "";
        }

        private void populate_ItemGroups()
        {
            DataSet dsgroup;
            classPDR_List_BLL itemcategory = new classPDR_List_BLL();
            dsgroup = itemcategory.Populate_Item_Group("FABRIC");
            ddlgroup.DataSource = dsgroup;
            ddlgroup.DataTextField = "itgroupdesc";
            ddlgroup.DataValueField = "itgroupid";
            ddlgroup.DataBind();
            ddlgroup.SelectedValue = "";
        }

        private void populate_ItemSubGroups()
        {
            DataSet dssubgroup;
            classPDR_List_BLL itemcategory = new classPDR_List_BLL();
            dssubgroup = itemcategory.Populate_Item_SubGroup("FABRIC");
            ddlSubGroup.DataSource = dssubgroup;
            ddlSubGroup.DataTextField = "itsubdesc";
            ddlSubGroup.DataValueField = "itsubid";
            ddlSubGroup.DataBind();
            ddlSubGroup.SelectedValue = "";
        }

        private void populate_ItemType()
        {
            DataSet dsitemtype;
            classPDR_List_BLL itemtype = new classPDR_List_BLL();
            string itnaturecd = "FABRIC";
            dsitemtype = itemtype.Populate_ItemType(itnaturecd);
            ddlType.DataSource = dsitemtype;
            ddlType.DataTextField = "ittypedesc";
            ddlType.DataValueField = "ittypeid";
            ddlType.DataBind();
            ddlType.SelectedValue = "";
        }

        private void populate_ItemSubType()
        {
            DataSet dsitemsubtype;
            string itnaturecd = "FABRIC";
            classPDR_List_BLL itemsubtype = new classPDR_List_BLL();
            dsitemsubtype = itemsubtype.Populate_ItemSubType(itnaturecd);
            ddlSubType.DataSource = dsitemsubtype;
            ddlSubType.DataTextField = "itsubdesc2";
            ddlSubType.DataValueField = "itsubid2";
            ddlSubType.DataBind();
            ddlSubType.SelectedValue = "";
        }

        private void populate_PDRRequestor()
        {
            DataSet dspdrrequestor;
            classPDR_List_BLL pdrrequestor = new classPDR_List_BLL();
            dspdrrequestor = pdrrequestor.Populate_PDR_REQUESTORS();
            ddlPDRRequestor.DataSource = dspdrrequestor;
            ddlPDRRequestor.DataTextField = "empname";
            ddlPDRRequestor.DataValueField = "empcd";
            ddlPDRRequestor.DataBind();
            ddlPDRRequestor.SelectedValue = "";
        }
        private void populate_PDRDevType()
        {
            DataSet dspdrdevtype;
            classPDR_List_BLL pdrDevType = new classPDR_List_BLL();
            dspdrdevtype = pdrDevType.Populate_DevelopType();
            ddlReasonForDevl.DataSource = dspdrdevtype;
            ddlReasonForDevl.DataTextField = "lookup_value";
            ddlReasonForDevl.DataValueField = "lookup_value_id";
            ddlReasonForDevl.DataBind();
            ddlReasonForDevl.SelectedValue = Session["username"].ToString().ToUpper(); // "";
        }
        //private void populate_DesignApplication()
        //{
        //    DataSet dsdesignapplication;
        //    classPDR_List_BLL designApplication = new classPDR_List_BLL();
        //    dsdesignapplication = designApplication.Populate_DesignApplication();
        //    ddlPurposeofGarment.DataSource = dsdesignapplication;
        //    ddlPurposeofGarment.DataTextField = "lookup_value";
        //    ddlPurposeofGarment.DataValueField = "lookup_value_id";
        //    ddlPurposeofGarment.DataBind();
        //    ddlPurposeofGarment.SelectedValue = "";
        //}
        private void populate_Finishing()
        {
            DataSet dsfinishing;
            classPDR_List_BLL finishing = new classPDR_List_BLL();
            dsfinishing = finishing.Populate_Finishing();
            ddlFinishing.DataSource = dsfinishing;
            ddlFinishing.DataTextField = "name_en";
            ddlFinishing.DataValueField = "id";
            ddlFinishing.DataBind();
            ddlFinishing.SelectedValue = "";
        }
        private void populate_UOM()
        {
            DataSet dsuom;
            classPDR_List_BLL uom = new classPDR_List_BLL();
            dsuom = uom.Populate_UOM();
            ddluom.DataSource = dsuom;
            ddluom.DataTextField = "uom";
            ddluom.DataValueField = "uom";
            ddluom.DataBind();
            //ddluom.SelectedValue = "";
        }
        private void populate_Currency()
        {
            DataSet dscurrency;
            classPDR_List_BLL currency = new classPDR_List_BLL();
            dscurrency = currency.Populate_Currency();
            ddlcombocurrency.DataSource = dscurrency;
            ddlcombocurrency.DataTextField = "currname";
            ddlcombocurrency.DataValueField = "curr";
            ddlcombocurrency.DataBind();
            //ddluom.SelectedValue = "";
        }
        private void populate_PDR_Priority()
        {
            DataSet dspriority;
            classPDR_List_BLL priority = new classPDR_List_BLL();
            dspriority = priority.Populate_PDR_Priority();
            ddlPriority.DataSource = dspriority;
            ddlPriority.DataTextField = "lookup_value";
            ddlPriority.DataValueField = "lookup_value_id";
            ddlPriority.DataBind();
            //ddluom.SelectedValue = "";
        }
        private void populate_Code_Item_Master()
        {
            DataSet dscode;
            classPDR_List_BLL code = new classPDR_List_BLL();
            dscode = code.Populate_Code_Item_Master();
            ddlCode1.DataSource = dscode;
            ddlCode1.DataTextField = "item_desc";
            ddlCode1.DataValueField = "item_code";
            ddlCode1.DataBind();
            ddlCode1.SelectedValue = "";
            ddlCode2.DataSource = dscode;
            ddlCode2.DataTextField = "item_desc";
            ddlCode2.DataValueField = "item_code";
            ddlCode2.DataBind();
            ddlCode2.SelectedValue = "";

            ddlCode3.DataSource = dscode;
            ddlCode3.DataTextField = "item_desc";
            ddlCode3.DataValueField = "item_code";
            ddlCode3.DataBind();
            ddlCode3.SelectedValue = "";

            ddlCode4.DataSource = dscode;
            ddlCode4.DataTextField = "item_desc";
            ddlCode4.DataValueField = "item_code";
            ddlCode4.DataBind();
            ddlCode4.SelectedValue = "";

            ddlCode5.DataSource = dscode;
            ddlCode5.DataTextField = "item_desc";
            ddlCode5.DataValueField = "item_code";
            ddlCode5.DataBind();
            ddlCode5.SelectedValue = "";

            ddlCode6.DataSource = dscode;
            ddlCode6.DataTextField = "item_desc";
            ddlCode6.DataValueField = "item_code";
            ddlCode6.DataBind();
            ddlCode6.SelectedValue = "";
            //ddluom.SelectedValue = "";
        }

        private void populate_EndBuyer()
        {
            DataSet dsendbuyer;
            classPDR_List_BLL endbuyer = new classPDR_List_BLL();
            dsendbuyer = endbuyer.Populate_EndBuyer();
            ddlEndBuyer.DataSource = dsendbuyer;
            ddlEndBuyer.DataTextField = "endbuyername";
            ddlEndBuyer.DataValueField = "endbuyerid";
            ddlEndBuyer.DataBind();
            ddlEndBuyer.SelectedValue = "";
        }

        public void populate_ItemApplication()
        {

            classPDR_List_BLL itemapplications = new classPDR_List_BLL();
            dsapplication = itemapplications.Populate_Item_Application();
            ViewState["appl"] = dsapplication;
            //ddlApplication.DataSource = dsapplication;
            //ddlApplication.DataTextField = "lookup_value";
            //ddlApplication.DataValueField = "lookup_value_id";
            //ddlApplication.DataBind();
            //ddlApplication.SelectedValue = "";
        }

        public void populate_ItemSubApplication()
        {

            classPDR_List_BLL itemsubapplications = new classPDR_List_BLL();
            dssubapplication = itemsubapplications.Populate_Item_SubApplication();
            ViewState["subappl"] = dssubapplication;
            //ddlSubApplication.DataSource = dssubapplication;
            //ddlSubApplication.DataTextField = "lookup_value";
            //ddlSubApplication.DataValueField = "lookup_value_id";
            //ddlSubApplication.DataBind();
            //ddlSubApplication.SelectedValue = "";
        }
        private void populate_SplFunc()
        {

            classPDR_List_BLL splfunc = new classPDR_List_BLL();
            dssplfunc = splfunc.Populate_SplFunc();
            ViewState["splfunc"] = dssplfunc;
            //ddlSubApplication.DataSource = dssubapplication;
            //ddlSubApplication.DataTextField = "lookup_value";
            //ddlSubApplication.DataValueField = "lookup_value_id";
            //ddlSubApplication.DataBind();
            //ddlSubApplication.SelectedValue = "";
        }
        private void populate_Country()
        {

            classPDR_List_BLL country = new classPDR_List_BLL();
            dscountry = country.Populate_POC_Combo_Country();
            ViewState["ctry"] = dscountry;
            //ddlSubApplication.DataSource = dssubapplication;
            //ddlSubApplication.DataTextField = "lookup_value";
            //ddlSubApplication.DataValueField = "lookup_value_id";
            //ddlSubApplication.DataBind();
            //ddlSubApplication.SelectedValue = "";
        }
        private void populate_Zone()
        {

            classPDR_List_BLL zone = new classPDR_List_BLL();
            dszone = zone.Populate_Zone();
            ViewState["zone"] = dszone;
            //ddlSubApplication.DataSource = dssubapplication;
            //ddlSubApplication.DataTextField = "lookup_value";
            //ddlSubApplication.DataValueField = "lookup_value_id";
            //ddlSubApplication.DataBind();
            //ddlSubApplication.SelectedValue = "";
        }
        private void populate_Customer()
        {

            classPDR_List_BLL customer = new classPDR_List_BLL();
            dscustomer = customer.Populate_Customer();
            ViewState["customer"] = dscustomer;
            //ddlSubApplication.DataSource = dssubapplication;
            //ddlSubApplication.DataTextField = "lookup_value";
            //ddlSubApplication.DataValueField = ookup_value_id";
            //ddlSubApplication.DataBind();
            //ddlSubApplication.SelectedValue = "";
        }
        private void populate_YarnFace()
        {
            DataSet dsyarnface;
            classPDR_List_BLL yarnface = new classPDR_List_BLL();
            dsyarnface = yarnface.Populate_YarnFace();
            ddlBack.DataSource = dsyarnface;
            ddlBack.DataTextField = "yarn_face";
            ddlBack.DataValueField = "yarn_face";
            ddlBack.DataBind();
            ddlBack.SelectedValue = "";
            ddlfront1.DataSource = dsyarnface;
            ddlfront1.DataTextField = "yarn_face";
            ddlfront1.DataValueField = "yarn_face";
            ddlfront1.DataBind();
            ddlfront1.SelectedValue = "";
            ddlfront2.DataSource = dsyarnface;
            ddlfront2.DataTextField = "yarn_face";
            ddlfront2.DataValueField = "yarn_face";
            ddlfront2.DataBind();
            ddlfront2.SelectedValue = "";
            ddlFront.DataSource = dsyarnface;
            ddlFront.DataTextField = "yarn_face";
            ddlFront.DataValueField = "yarn_face";
            ddlFront.DataBind();
            ddlFront.SelectedValue = "";
            ddlfront5.DataSource = dsyarnface;
            ddlfront5.DataTextField = "yarn_face";
            ddlfront5.DataValueField = "yarn_face";
            ddlfront5.DataBind();
            ddlfront5.SelectedValue = "";
            ddlfront6.DataSource = dsyarnface;
            ddlfront6.DataTextField = "yarn_face";
            ddlfront6.DataValueField = "yarn_face";
            ddlfront6.DataBind();
            ddlfront6.SelectedValue = "";
        }
        protected void gvDevelop_Request_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var ddlAppl = (DropDownList)e.Row.FindControl("ddlAppl");
                dsapplication = (DataSet)ViewState["appl"];
                ddlAppl.DataSource = dsapplication;
                ddlAppl.DataTextField = "lookup_value";
                ddlAppl.DataValueField = "lookup_value_id";
                ddlAppl.DataBind();

                if (ddlAppl.Items.Count > 0)
                {
                    if (gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[2].ToString() != string.Empty || gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[2].ToString() == null)
                    {
                        string app_id = gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[2].ToString();//.Trim();
                        if (app_id != "")
                        {
                            ddlAppl.ClearSelection();
                            for (int i = 0; i < ddlAppl.Items.Count; i++)
                            {
                                if (ddlAppl.Items[i].Value.Trim() == app_id.Trim())
                                {
                                    ddlAppl.SelectedIndex = i;
                                }
                            }
                            //ddl1.Items.FindByValue(person_responsible).Selected = true;
                        }
                    }
                }

                var ddlSubAppl = (DropDownList)e.Row.FindControl("ddlSubAppl");
                dssubapplication = (DataSet)ViewState["subappl"];
                ddlSubAppl.DataSource = dssubapplication;
                ddlSubAppl.DataTextField = "lookup_value";
                ddlSubAppl.DataValueField = "lookup_value_id";
                ddlSubAppl.DataBind();
                if (ddlSubAppl.Items.Count > 0)
                {
                    if (gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[3].ToString() != string.Empty || gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[3].ToString() == null)
                    {
                        string sub_app_id = gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[3].ToString();//.Trim();
                        if (sub_app_id != "")
                        {
                            ddlSubAppl.ClearSelection();
                            for (int i = 0; i < ddlSubAppl.Items.Count; i++)
                            {
                                if (ddlSubAppl.Items[i].Value.Trim() == sub_app_id.Trim())
                                {
                                    ddlSubAppl.SelectedIndex = i;
                                    break;
                                }
                            }
                            //ddl1.Items.FindByValue(person_responsible).Selected = true;
                        }
                    }
                }
                var ddlsplfunc = (DropDownList)e.Row.FindControl("ddlsplfunc");
                dssplfunc = (DataSet)ViewState["splfunc"];
                ddlsplfunc.DataSource = dssplfunc;
                ddlsplfunc.DataTextField = "lookup_value";
                ddlsplfunc.DataValueField = "lookup_value_id";
                ddlsplfunc.DataBind();
                if (ddlsplfunc.Items.Count > 0)
                {
                    if (gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[4].ToString() != string.Empty || gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[4].ToString() == null)
                    {
                        string splfunc = gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[4].ToString();//.Trim();
                        if (splfunc != "")
                        {
                            ddlsplfunc.ClearSelection();
                            for (int i = 0; i < ddlsplfunc.Items.Count; i++)
                            {
                                if (ddlsplfunc.Items[i].Value.Trim() == splfunc.Trim())
                                {
                                    ddlsplfunc.SelectedIndex = i;
                                }
                            }
                            //ddl1.Items.FindByValue(person_responsible).Selected = true;
                        }
                    }
                }
                var ddlctry = (DropDownList)e.Row.FindControl("ddlctry");
                dscountry = (DataSet)ViewState["ctry"];
                ddlctry.DataSource = dscountry;
                ddlctry.DataTextField = "ctry_name";
                ddlctry.DataValueField = "ctry";
                ddlctry.DataBind();
                if (ddlctry.Items.Count > 0)
                {
                    if (gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[5].ToString() != string.Empty || gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[5].ToString() == null)
                    {
                        string ctry = gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[5].ToString();//.Trim();
                        if (ctry != "")
                        {
                            ddlctry.ClearSelection();
                            for (int i = 0; i < ddlctry.Items.Count; i++)
                            {
                                if (ddlctry.Items[i].Value.Trim() == ctry.Trim())
                                {
                                    ddlctry.SelectedIndex = i;
                                }
                            }
                            //ddl1.Items.FindByValue(person_responsible).Selected = true;
                        }
                    }
                }
                var ddlzone = (DropDownList)e.Row.FindControl("ddlzone");
                dszone = (DataSet)ViewState["zone"];
                ddlzone.DataSource = dszone;
                ddlzone.DataTextField = "lookup_value";
                ddlzone.DataValueField = "lookup_value_id";
                ddlzone.DataBind();

                if (ddlzone.Items.Count > 0)
                {
                    if (gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[6].ToString() != string.Empty || gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[6].ToString() == null)
                    {
                        string zone = gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[6].ToString();//.Trim();
                        if (zone != "")
                        {
                            ddlzone.ClearSelection();
                            for (int i = 0; i < ddlzone.Items.Count; i++)
                            {
                                if (ddlzone.Items[i].Value.Trim() == zone.Trim())
                                {
                                    ddlzone.SelectedIndex = i;
                                }
                            }
                            //ddl1.Items.FindByValue(person_responsible).Selected = true;
                        }
                    }
                }

                var ddlcustomer = (DropDownList)e.Row.FindControl("ddlcustomer");
                dscustomer = (DataSet)ViewState["customer"];
                ddlcustomer.DataSource = dscustomer;
                ddlcustomer.DataTextField = "lookup_value";
                ddlcustomer.DataValueField = "lookup_value_id";
                ddlcustomer.DataBind();

                if (ddlcustomer.Items.Count > 0)
                {
                    if (gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[7].ToString() != string.Empty || gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[7].ToString() == null)
                    {
                        string customer = gvDevelop_Request.DataKeys[e.Row.RowIndex].Values[7].ToString();//.Trim();
                        if (customer != "")
                        {
                            ddlcustomer.ClearSelection();
                            for (int i = 0; i < ddlcustomer.Items.Count; i++)
                            {
                                if (ddlcustomer.Items[i].Value.Trim() == customer.Trim())
                                {
                                    ddlcustomer.SelectedIndex = i;
                                }
                            }
                            //ddl1.Items.FindByValue(person_responsible).Selected = true;
                        }
                    }
                }
                // Delete confirmation
                //foreach (Button button in e.Row.Cells[6].Controls.OfType<Button>())
                //{
                //    if (button.CommandName == "Delete")
                //    {
                //        button.Attributes["onclick"] = "if(!confirm('Do you want to delete the row " + e.Row.RowIndex + "?')){ return false; };";
                //    }
                //}
                //
            }
        }
        protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = Convert.ToInt32(e.RowIndex);
            //DataTable dt = ViewState["dt"] as DataTable;
            //dt.Rows[index].Delete();
            //ViewState["dt"] = dt;
            string item_properties = gvDevelop_Request.DataKeys[e.RowIndex].Values[1].ToString();
            Int32 deleteresult;
            if (item_properties != "")
            {
                if (Convert.ToInt32(item_properties) > 0)
                {
                    classDevelop_Request_BLL itemprepertydelete = new classDevelop_Request_BLL();
                    deleteresult = itemprepertydelete.Delete_ItemProperty(item_properties.ToString().Trim());
                }
            }
            //     gvDevelop_Request.DataSource = dtdevreq;
            //  gvDevelop_Request.DataBind();
            //   ViewState["pdrDevReq"] = dtdevreq;

            DataTable dt = ViewState["pdrDevReq"] as DataTable;
            dt.Rows[index].Delete();
            dt.EndInit();
            dt.AcceptChanges();
            ViewState["pdrDevReq"] = dt;
            gvDevelop_Request.DataSource = dt;
            gvDevelop_Request.DataBind();
            //ViewState["pdrDevReq"] = dt;

            //DataTable dtdevreq;
            //classDevelop_Request_BLL devrequest = new classDevelop_Request_BLL();
            //dtdevreq = devrequest.LoadDeveRequestApp(hidpdr_new_develop_req_id.Value.ToString().Trim());
            //gvDevelop_Request.DataSource = dtdevreq;
            //gvDevelop_Request.DataBind();
            //ViewState["pdrDevReq"] = dtdevreq;

        }
        protected void ImgAdd_Click(object sender, ImageClickEventArgs e)
        {

            checkValues();
            if (valuechanged == "YES")
            {
                ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "ClientScript", "alert('Modified Data is not updated, Click the Update icon and save the Data')", true);
                return;
            }
            else
            {
                hidpdr_new_develop_req_id.Value = "";
                ImgSave.Enabled = true;
                ClearALL();
                txtPDRNO.Text = "NEW";
                valuechanged = "NO";
                lblCancel.Text = "";
                DateTime dt = DateTime.Now.AddDays(-70);
                DateTime dt1 = DateTime.Now;
                PDRDate.Value = dt.ToString("dd/MM/yyyy").Replace('-', '/');// DateTime.Now.AddDays(-70).ToString();//.ToShortDateString();

                PDRDate.Value = dt1.ToString("dd/MM/yyyy").Replace('-', '/');
                //lblFileName.Text = "";
                DataTable ddt = null;
                gvDevelop_Request.DataSource = ddt;
                gvDevelop_Request.DataBind();
                ViewState["pdrlist"] = null;
                ViewState["pdrDevReq"] = null;
                ddlcombocurrency.SelectedValue = "THB";
                ddlCategory.SelectedItem.Text = "KNITTED";
                ddlSubCat.SelectedItem.Text = "CIRCULAR";

            }

        }

        protected void ImgAvailAlert_Click(object sender, ImageClickEventArgs e)
        {
            DataTable dt;
            string pdr_no = txtPDRNO.Text;
            //classDevelop_Request_BLL devReqBLL = new classDevelop_Request_BLL();
            //string mailResult;
            //classDevelop_Request_BLL developRequest = new classDevelop_Request_BLL();
            //dt = developRequest.LoadPDRList(pdr_no);
            //mailResult = devReqBLL.sendAvailMail(dt);
            Session["avilToEmailID"] = hidavilToEmailID.Value.Trim();
            Response.Redirect("~/UI/Avail_SendMail.aspx?pdrno=" + pdr_no.Trim());
        }

        protected void ImgAvailRefresh_Click(object sender, ImageClickEventArgs e)
        {

            checkValues();
            if (valuechanged == "YES")
            {
                ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "ClientScript", "alert('Modified Data is not updated, Click the Update icon and save the Data')", true);
                return;
            }
            else
            {
                //hidpdr_new_develop_req_id.Value = "";

                //ClearALL();
                //txtPDRNO.Text = "NEW";
                //valuechanged = "NO";
                //DateTime dt = DateTime.Now.AddDays(-70);
                //DateTime dt1 = DateTime.Now;
                //PDRDate.Value = dt.ToString("dd/MM/yyyy").Replace('-', '/');// DateTime.Now.AddDays(-70).ToString();//.ToShortDateString();

                //PDRDate.Value = dt1.ToString("dd/MM/yyyy").Replace('-', '/');
                //lblFileName.Text = "";
                //DataTable ddt = null;
                //gvDevelop_Request.DataSource = ddt;
                //gvDevelop_Request.DataBind();
                //ViewState["pdrlist"] = null;
                //ViewState["pdrDevReq"] = null;
                //ddlcombocurrency.SelectedValue = "THB";
                //ddlCategory.SelectedItem.Text = "KNITTED";
                //ddlSubCat.SelectedItem.Text = "CIRCULAR";
                String item_code1 = ddlCode1.SelectedValue;
                Decimal yarnNetBalKg1;
                classDevelop_Request_BLL classDevReq = new classDevelop_Request_BLL();
                yarnNetBalKg1 = classDevReq.getYarnNetBal(item_code1);
                txtAvail1.Text = Convert.ToString(yarnNetBalKg1);

                String item_code2 = ddlCode2.SelectedValue;
                Decimal yarnNetBalKg2;
                 classDevReq = new classDevelop_Request_BLL();
                yarnNetBalKg2 = classDevReq.getYarnNetBal(item_code2);
                txtAvail2.Text = Convert.ToString(yarnNetBalKg2);

                String item_code3 = ddlCode3.SelectedValue;
                Decimal yarnNetBalKg3;
                classDevReq = new classDevelop_Request_BLL();
                yarnNetBalKg3 = classDevReq.getYarnNetBal(item_code3);
                txtAvail3.Text = Convert.ToString(yarnNetBalKg3);

                String item_code4 = ddlCode4.SelectedValue;
                Decimal yarnNetBalKg4;
                classDevReq = new classDevelop_Request_BLL();
                yarnNetBalKg4 = classDevReq.getYarnNetBal(item_code4);
                txtAvail4.Text = Convert.ToString(yarnNetBalKg4);

                String item_code5 = ddlCode5.SelectedValue;
                Decimal yarnNetBalKg5;
                classDevReq = new classDevelop_Request_BLL();
                yarnNetBalKg5 = classDevReq.getYarnNetBal(item_code5);
                txtAvail5.Text = Convert.ToString(yarnNetBalKg5);

                String item_code6 = ddlCode5.SelectedValue;
                Decimal yarnNetBalKg6;
                classDevReq = new classDevelop_Request_BLL();
                yarnNetBalKg6 = classDevReq.getYarnNetBal(item_code6);
                txtAvail6.Text = Convert.ToString(yarnNetBalKg6);

            }

        }

        private void checkValues()
        {
            dtprdlistcopy = (DataTable)ViewState["pdrlist"];
            if (dtprdlistcopy != null)
            {
                foreach (DataRow rowcopy in dtprdlistcopy.Rows)
                {
                    string pdrno = (rowcopy["pdr_no"].ToString().Trim());
                    string pdrnoexit = txtPDRNO.Text.Trim();
                    if (pdrno.Trim() != txtPDRNO.Text.Trim())
                    {
                        valuechanged = "YES";
                    }
                    if (!string.IsNullOrEmpty(rowcopy["pdr_date"].ToString()))
                    {
                        DateTime dt = Convert.ToDateTime(rowcopy["pdr_date"].ToString());
                        string dtone = String.Format("{0:dd-MM-yyyy}", dt);
                        if (dtone != PDRDate.Value)
                        {
                            valuechanged = "YES";
                        }

                    }
                    if (!string.IsNullOrEmpty(rowcopy["design_no"].ToString()))
                    {
                        if (txtDesignno.Text != rowcopy["design_no"].ToString())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (txtDesignno.Text != "")
                        {
                            valuechanged = "YES";
                        }
                    }
                    if (ddlPDRRequestor.SelectedValue != rowcopy["requested_by"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    if (ddlSubCat.SelectedValue != rowcopy["itsubcatid"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    if (ddlgroup.SelectedValue != rowcopy["itgroupid"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    if (ddlCategory.SelectedValue != rowcopy["itcatid"].ToString())
                    {
                        valuechanged = "YES";
                    }

                    if (ddlSubGroup.SelectedValue != rowcopy["itsubid"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    if (ddlType.SelectedValue != rowcopy["ittypeid"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    if (ddlSubType.SelectedValue != rowcopy["itsubid2"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    if (txtDevelopmentTypeRemark.Text != rowcopy["development_type_remark"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    string followcustomerspec = rowcopy["follow_customer_spec"].ToString();
                    string chkfollow = CHKFollow.Checked.ToString();
                    if (followcustomerspec == "Y")
                    {
                        followcustomerspec = "True";

                    }

                    else
                    {
                        followcustomerspec = "False";
                    }
                    if (chkfollow != followcustomerspec)
                    {
                        valuechanged = "YES";
                    }
                    if (txtDevelopmentTypeRemark.Text != rowcopy["development_type_remark"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtDevelopmentTypeRemark.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtComposition1_name.Text != rowcopy["composition1_name"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtComposition1_name.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtComposition2_name.Text != rowcopy["composition2_name"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtComposition2_name.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtComposition3_name.Text != rowcopy["composition3_name"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtComposition3_name.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtComposition4_name.Text != rowcopy["composition4_name"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtComposition4_name.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtComposition5_name.Text != rowcopy["composition5_name"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtComposition5_name.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtComposition6_name.Text != rowcopy["composition6_name"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtComposition6_name.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtweight.Text != rowcopy["weight_sqm"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtweight.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (TXTWIDTH.Text != rowcopy["width"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (TXTWIDTH.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (TXTElongation.Text != rowcopy["elongation"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (TXTElongation.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (TXTMODULUS.Text != rowcopy["modulus"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (TXTMODULUS.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    txtRecovStretbility.Text = "";
                    txtDimStbilityShr.Text = "";
                    if (txtremark.Text != rowcopy["remark"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtremark.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtweightTolerance.Text != rowcopy["weight_sqm_tolerance"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtweightTolerance.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtwidth_Tolerance.Text != rowcopy["width_tolerance"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtwidth_Tolerance.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtpilingremark.Text != rowcopy["pilling_remark"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtpilingremark.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtsnaggingremark.Text != rowcopy["snagging_remark"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtsnaggingremark.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtdyeincolor.Text != rowcopy["color_name"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtdyeincolor.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtMachineTense.Text != rowcopy["machine_dense"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtMachineTense.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    // if (row["knitting_appointment"] != null || row["knitting_appointment"].ToString() != "{}")
                    if (!string.IsNullOrEmpty(rowcopy["knitting_appointment"].ToString()))
                    {

                        DateTime dt1 = Convert.ToDateTime(rowcopy["knitting_appointment"].ToString());
                        string dt1exit = String.Format("{0:dd-MM-yyyy}", dt1);// row["knitting_appointment"].ToString();
                        if (dt1exit != txtknittingappoinment.Value)
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (txtknittingappoinment.Value != "")
                        {
                            valuechanged = "YES";
                        }
                    }
                    if (!string.IsNullOrEmpty(rowcopy["yarn_available_date"].ToString()))
                    {
                        DateTime dt2 = Convert.ToDateTime(rowcopy["yarn_available_date"].ToString());
                        string dt2exit = String.Format("{0:dd-MM-yyyy}", dt2);// row["yarn_available_date"].ToString();
                        if (txtyarnavailabledate.Value != dt2exit)
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (txtyarnavailabledate.Value != "")
                        {
                            valuechanged = "YES";
                        }
                    }
                    if (!string.IsNullOrEmpty(rowcopy["expected_finished_date"].ToString()))
                    {
                        DateTime dt3 = Convert.ToDateTime(rowcopy["expected_finished_date"].ToString());
                        string dt3exit = String.Format("{0:dd-MM-yyyy}", dt3);//row["expected_finished_date"].ToString();
                        if (txtExpectedFinisheddate.Value != dt3exit)
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (txtExpectedFinisheddate.Value != "")
                        {
                            valuechanged = "YES";
                        }
                    }
                    if (!string.IsNullOrEmpty(rowcopy["spec_master_date"].ToString()))
                    {
                        DateTime dt4 = Convert.ToDateTime(rowcopy["spec_master_date"].ToString());
                        string dt4exit = String.Format("{0:dd-MM-yyyy}", dt4);//row["spec_master_date"].ToString();
                        if (txtspecmasterdate.Value != dt4exit)
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (txtspecmasterdate.Value != "")
                        {
                            valuechanged = "YES";
                        }
                    }
                    if (!string.IsNullOrEmpty(rowcopy["qa_report_date"].ToString()))
                    {
                        DateTime dt5 = Convert.ToDateTime(rowcopy["qa_report_date"].ToString());
                        string dt5exit = String.Format("{0:dd-MM-yyyy}", dt5);//= row["qa_report_date"].ToString();
                        if (txtqa_reportdate.Value != dt5exit)
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (txtqa_reportdate.Value != "")
                        {
                            valuechanged = "YES";
                        }
                    }
                    if (txtStandard_Customer.Text != rowcopy["standard_customer"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtStandard_Customer.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (!string.IsNullOrEmpty(rowcopy["expected_price"].ToString()))
                    {
                        if (txtCustPriceExpectation.Text != rowcopy["expected_price"].ToString())
                        {
                            valuechanged = "YES";
                        }
                    }
                    //else
                    //{
                    //    if (txtCustPriceExpectation.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (!string.IsNullOrEmpty(rowcopy["composition1_percent"].ToString()))
                    {
                        string p1 = txtPercent1.Text;
                        txtPercent1.Text = string.Format("{0:0.##}", p1);
                        if (txtPercent1.Text != rowcopy["composition1_percent"].ToString())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (txtPercent1.Text != "")
                        {
                            valuechanged = "YES";
                        }
                    }
                    if (!string.IsNullOrEmpty(rowcopy["composition2_percent"].ToString()))
                    {
                        string p2 = txtPercent2.Text;
                        txtPercent2.Text = string.Format("{0:0.##}", p2);
                        if (txtPercent2.Text != rowcopy["composition2_percent"].ToString())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (txtPercent2.Text != "")
                        {
                            valuechanged = "YES";
                        }
                    }
                    if (!string.IsNullOrEmpty(rowcopy["composition3_percent"].ToString()))
                    {
                        string p3 = txtPercent3.Text;
                        txtPercent3.Text = string.Format("{0:0.##}", p3);
                        if (txtPercent3.Text != rowcopy["composition3_percent"].ToString())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (txtPercent3.Text != "")
                        {
                            valuechanged = "YES";
                        }
                    }
                    if (!string.IsNullOrEmpty(rowcopy["composition4_percent"].ToString()))
                    {
                        string p4 = txtPercent4.Text;
                        txtPercent4.Text = string.Format("{0:0.##}", p4);
                        if (txtPercent4.Text != rowcopy["composition4_percent"].ToString())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (txtPercent4.Text != "")
                        {
                            valuechanged = "YES";
                        }
                    }
                    if (!string.IsNullOrEmpty(rowcopy["composition5_percent"].ToString()))
                    {
                        string p5 = txtPercent5.Text;

                        txtPercent5.Text = string.Format("{0:0.00}", p5);
                        if (txtPercent5.Text != rowcopy["composition5_percent"].ToString())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (txtPercent5.Text != "")
                        {
                            valuechanged = "YES";
                        }
                    }
                    if (!string.IsNullOrEmpty(rowcopy["composition6_percent"].ToString()))
                    {
                        string p6 = txtPercent6.Text;
                        txtPercent6.Text = string.Format("{0:0.##}", p6);
                        if (txtPercent6.Text != rowcopy["composition6_percent"].ToString())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (txtPercent6.Text != "")
                        {
                            valuechanged = "YES";
                        }
                    }
                    if (!string.IsNullOrEmpty(rowcopy["gauge"].ToString()))
                    {
                        if (TXTGUAGE.Text != rowcopy["gauge"].ToString())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (TXTGUAGE.Text != "")
                        {
                            valuechanged = "YES";
                        }
                    }
                    if (!string.IsNullOrEmpty(rowcopy["yarn_face1"].ToString()))
                    {
                        if (ddlFront.SelectedValue != rowcopy["yarn_face1"].ToString().Trim())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (ddlFront.SelectedIndex > 0)
                        {
                            valuechanged = "YES";
                        }

                    }
                    if (!string.IsNullOrEmpty(rowcopy["yarn_face2"].ToString()))
                    {
                        if (ddlBack.SelectedValue != rowcopy["yarn_face2"].ToString().Trim())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (ddlBack.SelectedIndex > 0)
                        {
                            valuechanged = "YES";
                        }

                    }
                    if (!string.IsNullOrEmpty(rowcopy["yarn_face3"].ToString().Trim()))
                    {
                        if (ddlfront1.SelectedValue != rowcopy["yarn_face3"].ToString().Trim())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (ddlfront1.SelectedIndex > 0)
                        {
                            valuechanged = "YES";
                        }

                    }
                    if (!string.IsNullOrEmpty(rowcopy["yarn_face4"].ToString().Trim()))
                    {
                        if (ddlfront2.SelectedValue != rowcopy["yarn_face4"].ToString().Trim())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (ddlfront2.SelectedIndex > 0)
                        {
                            valuechanged = "YES";
                        }

                    }
                    if (!string.IsNullOrEmpty(rowcopy["yarn_face5"].ToString().Trim()))
                    {
                        if (ddlfront5.SelectedValue != rowcopy["yarn_face5"].ToString().Trim())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (ddlfront5.SelectedIndex > 0)
                        {
                            valuechanged = "YES";
                        }

                    }
                    if (!string.IsNullOrEmpty(rowcopy["yarn_face6"].ToString().Trim()))
                    {
                        if (ddlfront6.SelectedValue != rowcopy["yarn_face6"].ToString().Trim())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (ddlfront6.SelectedIndex > 0)
                        {
                            valuechanged = "YES";
                        }

                    }
                    if (!string.IsNullOrEmpty(rowcopy["require_tag"].ToString().Trim()))
                    {
                        string gangtagrequired = rowcopy["require_tag"].ToString();
                        string gangexit = chkHangTagRequire.Checked.ToString();
                        if (gangtagrequired == "N")
                        {
                            gangtagrequired = "False";
                        }
                        else
                        {
                            gangtagrequired = "True";
                        }
                        if (gangtagrequired != gangexit)
                        {
                            valuechanged = "YES";
                        }
                    }

                    if (TXTYARDAGE.Text != rowcopy["yardage"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (TXTYARDAGE.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    string snagging = rowcopy["snagging"].ToString();
                    string snagexit = chksnagging.Checked.ToString();
                    if (snagging == "N")
                    {
                        snagging = "False";
                    }
                    else
                    {
                        snagging = "True";
                    }
                    if (snagging != snagexit)
                    {
                        valuechanged = "YES";
                    }
                    string pilling = rowcopy["piling"].ToString();
                    string pillexit = chkpilling.Checked.ToString();
                    if (pilling == "N")
                    {
                        pilling = "False";
                    }
                    else
                    {
                        pilling = "True";
                    }
                    if (pillexit != pilling)
                    {
                        valuechanged = "YES";
                    }
                    string standard_aatcc = rowcopy["standard_aatcc"].ToString();
                    string aatccexit = CHKAATCC.Checked.ToString();
                    if (standard_aatcc == "N" || standard_aatcc == "0")
                    {
                        standard_aatcc = "False";
                    }
                    else
                    {
                        standard_aatcc = "True";
                    }
                    if (standard_aatcc != aatccexit)
                    {
                        valuechanged = "YES";
                    }
                    string standard_astm = rowcopy["standard_astm"].ToString();
                    string astmexit = CHKASTM.Checked.ToString();
                    if (standard_astm == "N" || standard_astm == "0")
                    {
                        standard_astm = "False";
                    }
                    else
                    {
                        standard_astm = "True";
                    }
                    if (standard_astm != astmexit)
                    {
                        valuechanged = "YES";
                    }
                    string standard_iso = rowcopy["standard_iso"].ToString();
                    string isoexit = CHKISO.Checked.ToString();

                    if (standard_iso == "N" || standard_iso == "0")
                    {
                        standard_iso = "False";
                    }
                    else
                    {
                        standard_iso = "True";
                    }
                    if (standard_iso != isoexit)
                    {
                        valuechanged = "YES";
                    }
                    string standard_jis = rowcopy["standard_jis"].ToString();
                    string jisexit = CHKJIS.Checked.ToString();

                    if (standard_jis == "N" || standard_jis == "0")
                    {
                        standard_jis = "False";
                    }
                    else
                    {
                        standard_jis = "True";
                    }
                    if (jisexit != standard_jis)
                    {
                        valuechanged = "YES";
                    }

                    if (txtElongation_l.Text != rowcopy["elongation_l"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtElongation_l.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtmodulus_l.Text != rowcopy["modulus_l"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtmodulus_l.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtstretch_l.Text != rowcopy["stretch_l"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtstretch_l.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtshrinkage_l.Text != rowcopy["shrinkage_l"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtshrinkage_l.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtElongation_w.Text != rowcopy["elongation_w"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtElongation_w.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtmodulus_w.Text != rowcopy["modulus_w"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtmodulus_w.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtstretch_w.Text != rowcopy["stretch_w"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtstretch_w.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtshrinkage_w.Text != rowcopy["shrinkage_w"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtshrinkage_w.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtelongation_tolerance.Text != rowcopy["elongation_tolerance"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtelongation_tolerance.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtmodulus_tolerance.Text != rowcopy["modulus_tolerance"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtmodulus_tolerance.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtshrinkage_tolerance.Text != rowcopy["shrinkage_tolerance"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtshrinkage_tolerance.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtstretch_tolerance.Text != rowcopy["stretch_tolerance"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtstretch_tolerance.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (!string.IsNullOrEmpty(rowcopy["development_type_id"].ToString()))
                    {
                        if (ddlReasonForDevl.SelectedValue != rowcopy["development_type_id"].ToString())
                        {
                            valuechanged = "YES";
                        }
                    }
                    else
                    {
                        if (ddlReasonForDevl.SelectedIndex > 0)
                        {
                            valuechanged = "YES";
                        }
                    }

                    //if (ddlPurposeofGarment.SelectedValue.ToString() != rowcopy["product_application_id"].ToString())
                    //{
                    //    valuechanged = "YES";
                    //}
                    //else
                    //{
                    //    if (ddlPurposeofGarment.SelectedIndex > 0)
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (!string.IsNullOrEmpty(rowcopy["expected_date"].ToString()))
                    {
                        DateTime dtt = Convert.ToDateTime(rowcopy["expected_date"].ToString());
                        string dtone = String.Format("{0:dd-MM-yyyy}", dtt);
                        if (dtone != dtTo.Value.Substring(0, 10))
                        {
                            valuechanged = "YES";
                        }

                    }
                    //if (dtTo.Value != rowcopy["expected_date"].ToString())
                    //{
                    //    valuechanged = "YES";
                    //}

                    //if (!string.IsNullOrEmpty(rowcopy["require_tag"].ToString()))
                    //{

                    //    if (txtSampleRequire.Text != rowcopy["require_tag"].ToString())
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    //else
                    //{
                    //    if (txtSampleRequire.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtStandard_Customer.Text != rowcopy["standard_customer"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtStandard_Customer.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    //txtComment.Text = 
                    string yarnavailable = rowcopy["yarn_available"].ToString();
                    string yarnexit = chkYarnAvailable.Checked.ToString();

                    if (yarnavailable == "N")
                    {
                        yarnavailable = "False";
                    }
                    else
                    {
                        yarnavailable = "True";
                    }
                    if (yarnavailable != yarnexit)
                    {
                        valuechanged = "YES";
                    }

                    if (txtweightTolerance.Text != rowcopy["weight_sqm_tolerance"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtweightTolerance.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                    if (txtwidth_Tolerance.Text != rowcopy["width_tolerance"].ToString())
                    {
                        valuechanged = "YES";
                    }
                    //else
                    //{
                    //    if (txtwidth_Tolerance.Text != "")
                    //    {
                    //        valuechanged = "YES";
                    //    }
                    //}
                }
            }
        }
        private void ClearALL()
        {
            txtPDRNO.Text = "";
            PDRDate.Value = "";// dt.ToString("dd-MM-yyyy");// String.Format("M/d/yyyy", dt);// Convert.ToDateTime(row["pdr_date"]).ToString();
            txtDesignno.Text = "";
            txtCustCode.Text = "";
            ddlSubCat.SelectedValue = "";
            ddlgroup.SelectedValue = "";
            ddlCategory.SelectedValue = "";
            ddlSubGroup.SelectedValue = "";
            ddlType.SelectedValue = "";
            ddlSubType.SelectedValue = "";
            ddlEndBuyer.SelectedValue = "";
            CHKFollow.Checked = false;
            txtDevelopmentTypeRemark.Text = "";
            txtSearchMain.Text = "";
            txtDevelopmentTypeRemark.Text = "";
            txtComposition1_name.Text = "";
            txtComposition2_name.Text = "";
            txtComposition3_name.Text = "";
            txtComposition4_name.Text = "";
            txtComposition5_name.Text = "";
            txtComposition6_name.Text = "";
            txtweight.Text = "";
            TXTWIDTH.Text = "";
            TXTGUAGE.Text = "";
            TXTElongation.Text = "";
            TXTMODULUS.Text = "";
            txtRecovStretbility.Text = "";
            txtDimStbilityShr.Text = "";
            txtremark.Text = "";
            txtweightTolerance.Text = "";
            txtwidth_Tolerance.Text = "";
            txtpilingremark.Text = "";
            txtsnaggingremark.Text = "";
            txtdyeincolor.Text = "";
            txtMachineTense.Text = "";
            //ddlFinishing.SelectedValue = "";
            //  DateTime dt1 = "";
            txtknittingappoinment.Value = "";// row["knitting_appointment"].ToString();
                                             //  DateTime dt2 = Convert.ToDateTime(row["yarn_available_date"].ToString());
            txtyarnavailabledate.Value = "";// row["yarn_available_date"].ToString();
                                            // DateTime dt3 = Convert.ToDateTime(row["expected_finished_date"].ToString());
            txtExpectedFinisheddate.Value = "";//row["expected_finished_date"].ToString();
                                               // DateTime dt4 = Convert.ToDateTime(row["spec_master_date"].ToString());
            txtspecmasterdate.Value = "";//row["spec_master_date"].ToString();
                                         // DateTime dt5 = Convert.ToDateTime(row["qa_report_date"].ToString());
            txtqa_reportdate.Value = "";//= row["qa_report_date"].ToString();
            txtStandard_Customer.Text = "";
            txtCustPriceExpectation.Text = "";
            txtPercent1.Text = "";
            txtPercent2.Text = "";
            txtPercent3.Text = "";
            txtPercent4.Text = "";
            txtPercent5.Text = "";
            txtPercent6.Text = "";
            TXTGUAGE.Text = "";
            chkHangTagRequire.Checked = false;

            TXTYARDAGE.Text = "";

            chksnagging.Checked = false;

            chkpilling.Checked = false;

            CHKAATCC.Checked = false;

            CHKASTM.Checked = false;

            CHKISO.Checked = false;

            CHKJIS.Checked = false;
            ddlFront.SelectedValue = "";
            ddlBack.SelectedValue = "";
            ddlfront1.SelectedValue = "";
            ddlfront2.SelectedValue = "";
            ddlfront5.SelectedValue = "";
            ddlfront6.SelectedValue = "";
            ddlCode1.SelectedValue = "";
            ddlCode2.SelectedValue = "";
            ddlCode3.SelectedValue = "";
            ddlCode4.SelectedValue = "";
            ddlCode5.SelectedValue = "";
            ddlCode6.SelectedValue = "";
            txtElongation_l.Text = "";
            txtweight.Text = "";
            txtmodulus_l.Text = "";
            txtstretch_l.Text = "";
            txtshrinkage_l.Text = "";
            txtElongation_w.Text = "";
            txtmodulus_w.Text = "";
            txtstretch_w.Text = "";
            txtshrinkage_w.Text = "";
            txtelongation_tolerance.Text = "";
            txtmodulus_tolerance.Text = "";
            txtshrinkage_tolerance.Text = "";
            txtstretch_tolerance.Text = "";
            ddlReasonForDevl.SelectedValue = "";
            // ddlPurposeofGarment.SelectedValue = "";
            dtTo.Value = "";
            txtSampleRequire.Text = "";
            txtComment.Text = "";
            chkYarnAvailable.Checked = false;
            txtweightTolerance.Text = "";
            txtwidth_Tolerance.Text = "";
            hidpdr_new_develop_req_id.Value = "";
            txtAppRej.Text = "";
            txtAppRejComment.Text = "";
            txtBy.Text = "";
            txtAppRejDate.Text = "";
        }

        protected void ImgSave_Click(object sender, ImageClickEventArgs e)
        {
            if (hidPDRNOCancel.Value == "Y")
            {
                ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "ClientScript", "alert('Cannot Save Cancelled PDR')", true);
                return;
            }
            else
            { 
            string p_pdr_new_develop_req_id = "";
            string p_pdr_no = "";
            string p_pdr_date = "";
            string p_design_no = "";
            string p_gauge = "";
            string p_custcd = "";
            string p_requested_by = "";
            string p_expected_price = "";
            string p_uom = "";
            string p_expected_date = "";
            string p_product_application_id = "";
            string p_development_type_id = "";
            string p_development_type_remark = "";
            string p_itcatid = "";
            string p_itsubcatid = "";
            string p_itgroupid = "";
            string p_itsubid = "";
            string p_ittypeid = "";
            string p_itsubid2 = "";
            string p_standard_iso = "";
            string p_standard_aatcc = "";
            string p_standard_astm = "";
            string p_standard_jis = "";
            string p_standard_customer = "";
            string p_follow_customer_spec = "";
            string p_yarn_face1 = "";
            string p_yarn_face2 = "";
            string p_yarn_face3 = "";
            string p_yarn_face4 = "";
            string p_yarn_face5 = "";
            string p_yarn_face6 = "";
            string p_composition1_percent = "";
            string p_composition2_percent = "";
            string p_composition3_percent = "";
            string p_composition4_percent = "";
            string p_composition5_percent = "";
            string p_composition6_percent = "";
            string p_composition1_name = "";
            string p_composition2_name = "";
            string p_composition3_name = "";
            string p_composition4_name = "";
            string p_composition5_name = "";
            string p_composition6_name = "";
            string p_weight_sqm = "";
            string p_weight_sqm_tolerance = "";
            string p_weight_sqm_remark = "";
            string p_width = "";
            string p_width_tolerance = "";
            string p_elongation = "";
            string p_elongation_l = "";
            string p_elongation_w = "";
            string p_elongation_tolerance = "";
            string p_modulus = "";
            string p_modulus_l = "";
            string p_modulus_w = "";
            string p_modulus_tolerance = "";
            string p_stretch_l = "";
            string p_stretch_w = "";
            string p_stretch_tolerance = "";
            string p_shrinkage_l = "";
            string p_shrinkage_w = "";
            string p_shrinkage_tolerance = "";
            string p_piling = "";
            string p_pilling_remark = "";
            string p_snagging = "";
            string p_snagging_remark = "";
            string p_color_name = "";
            string p_dye_finishing_formula_id = "";
            string p_dye_remark = "";
            string p_hanger = "";
            string p_yardage = "";
            string p_require_tag = "";
            string p_machine_dense = "";
            string p_yarn_available = "";
            string p_yarn_available_date = "";
            string p_knitting_appointment = "";
            string p_offer_price = "";
            string p_is_consideration = "";
            string p_expected_finished_date = "";
            string p_spec_master_date = "";
            string p_qa_report_date = "";
            string p_remark = "";
                string p_user_session_id = Session["usersessionid"].ToString();
            valuechanged = "NO";
            string fileName = "";
            string p_standard_ms = "";
            string p_standard_vss = "";
            string p_standard_hm = "";
            string p_endbuyercd = "";
            string p_priority_id = "";
            string p_priority_comment = "";
            string p_price_curr = "";
            string p_itcd1 = "";
            string p_itcd2 = "";
            string p_itcd3 = "";
            string p_itcd4 = "";
            string p_itcd5 = "";
            string p_itcd6 = "";
            Decimal p_avail1 = 0;
            Decimal p_avail2 = 0;
            Decimal p_avail3 = 0;
            Decimal p_avail4 = 0;
            Decimal p_avail5 = 0;
            Decimal p_avail6 = 0;
            Decimal p_kg_per_finished_roll = 0;
            Decimal p_no_of_finished_rolls = 0;
            string p_yarn_shortage1 = "N";
            string p_yarn_shortage2 = "N";
            string p_yarn_shortage3 = "N";
            string p_yarn_shortage4  = "N";
            string p_yarn_shortage5 = "N";
            string p_yarn_shortage6 = "N";
            string p_new_yarn1 = "N";
            string p_new_yarn2 = "N";
            string p_new_yarn3 = "N";
            string p_new_yarn4 = "N";
            string p_new_yarn5 = "N";
            string p_new_yarn6 = "N";
            string p_test_method_id = "";
            string p_shoe_size_id = "";
            string p_shoe_style = "";
            string p_shoe_gender_id = "";
                string p_pallet_pattern_no = "";
                string p_product_pattern_no = "";

                string p_pattern_width = "";
                string p_pattern_length = "";
                string p_shoe_require_laser_cut = "";
                string p_laser_cut_with_pattern = "";
                string p_shoe_pairs_required = "";
                string p_shoe_length = "";
                string p_shoe_width = "";
                string p_repeat_per_roll = "";
                try
                {
                p_pdr_new_develop_req_id = hidpdr_new_develop_req_id.Value;
                p_pdr_no = txtPDRNO.Text;
                p_pdr_date = PDRDate.Value;
                string day = p_pdr_date.Substring(0, 2);
                string mon = p_pdr_date.Substring(3, 2);
                string yr = p_pdr_date.Substring(6, 4);
                p_pdr_date = yr + "-" + mon + "-" + day;// + " 00:00:00";
                p_design_no = txtDesignno.Text;
                p_custcd = hidSearchMainValue.Value;// hidSearchMainValue.Value.Trim();
                p_gauge = TXTGUAGE.Text;
                //p_custcd = "";
                p_requested_by = ddlPDRRequestor.SelectedValue;
                p_expected_price = txtCustPriceExpectation.Text;
                p_uom = ddluom.SelectedValue;

                p_expected_date = dtTo.Value;
                if (p_expected_date.ToString().Trim() != "")
                {
                    string day1 = p_expected_date.Substring(0, 2);
                    string mon1 = p_expected_date.Substring(3, 2);
                    string yr1 = p_expected_date.Substring(6, 4);
                    p_expected_date = yr1 + "-" + mon1 + "-" + day1;
                }

                p_product_application_id = null;// ddlPurposeofGarment.SelectedValue;
                p_development_type_id = ddlReasonForDevl.SelectedValue;
                p_development_type_remark = txtDevelopmentTypeRemark.Text;
                p_itcatid = ddlCategory.SelectedValue;
                p_itsubcatid = ddlSubCat.SelectedValue;
                p_itgroupid = ddlgroup.SelectedValue;
                p_itsubid = ddlSubGroup.SelectedValue;
                p_ittypeid = ddlType.SelectedValue;
                p_itsubid2 = ddlSubType.SelectedValue;
                if (CHKISO.Checked)
                {
                    p_standard_iso = "Y";
                }
                else
                {
                    p_standard_iso = "N";
                }
                if (CHKAATCC.Checked)
                {
                    p_standard_aatcc = "Y";
                }
                else
                {
                    p_standard_aatcc = "N";
                }
                if (CHKASTM.Checked)
                {
                    p_standard_astm = "Y";
                }
                else
                {
                    p_standard_astm = "N";
                }
                if (CHKJIS.Checked)
                {
                    p_standard_jis = "Y";
                }
                else
                {
                    p_standard_jis = "N";
                }
                if (chkMS.Checked)
                {
                    p_standard_ms = "Y";
                }
                else
                {
                    p_standard_ms = "N";
                }
                if (chkVSS.Checked)
                {
                    p_standard_vss = "Y";
                }
                else
                {
                    p_standard_vss = "N";
                }
                if (chkHM.Checked)
                {
                    p_standard_hm = "Y";
                }
                else
                {
                    p_standard_hm = "N";
                }

                p_standard_customer = txtStandard_Customer.Text;

                if (CHKFollow.Checked)
                {
                    p_follow_customer_spec = "Y";
                }
                else
                {
                    p_follow_customer_spec = "N";
                }
                p_yarn_face1 = ddlFront.SelectedValue;
                p_yarn_face2 = ddlBack.SelectedValue;
                p_yarn_face3 = ddlfront1.SelectedValue;
                p_yarn_face4 = ddlfront2.SelectedValue;
                p_yarn_face5 = ddlfront5.SelectedValue;
                p_yarn_face6 = ddlfront6.SelectedValue;
                p_composition1_percent = txtPercent1.Text;
                p_composition2_percent = txtPercent2.Text;
                p_composition3_percent = txtPercent3.Text;
                p_composition4_percent = txtPercent4.Text;
                p_composition5_percent = txtPercent5.Text;
                p_composition6_percent = txtPercent6.Text;
                p_composition1_name = txtComposition1_name.Text;
                p_composition2_name = txtComposition2_name.Text;
                p_composition3_name = txtComposition3_name.Text;
                p_composition4_name = txtComposition4_name.Text;
                p_composition5_name = txtComposition5_name.Text;
                p_composition6_name = txtComposition6_name.Text;
                p_weight_sqm = txtweight.Text;
                p_weight_sqm_tolerance = txtweightTolerance.Text;
                p_weight_sqm_remark = "";
                p_width = TXTWIDTH.Text;
                p_width_tolerance = txtwidth_Tolerance.Text;
                p_elongation = TXTElongation.Text;
                p_elongation_l = txtElongation_l.Text;
                p_elongation_w = txtElongation_w.Text;

                p_elongation_tolerance = txtelongation_tolerance.Text;
                p_modulus = TXTMODULUS.Text;
                p_modulus_l = txtmodulus_l.Text;
                p_modulus_w = txtmodulus_w.Text;
                p_modulus_tolerance = txtmodulus_tolerance.Text;
                p_stretch_l = txtstretch_l.Text;
                p_stretch_w = txtstretch_w.Text;
                p_stretch_tolerance = txtstretch_tolerance.Text;
                p_shrinkage_l = txtshrinkage_l.Text;
                p_shrinkage_w = txtshrinkage_w.Text;
                p_shrinkage_tolerance = txtshrinkage_tolerance.Text;
                if (chkpilling.Checked)
                {
                    p_piling = "Y";// chkpilling.Checked.ToString();
                }
                else
                {
                    p_piling = "N";
                }
                p_pilling_remark = txtpilingremark.Text;
                if (chksnagging.Checked)
                {
                    p_snagging = "Y";// chksnagging.Checked.ToString();
                }
                else
                {
                    p_snagging = "N";
                }
                p_snagging_remark = txtsnaggingremark.Text.ToString();
                p_color_name = txtdyeincolor.Text;
                p_dye_finishing_formula_id = ddlFinishing.SelectedValue;
                p_dye_remark = "";
                if (txtSampleRequire.Text != "")
                {
                    p_hanger = txtSampleRequire.Text.ToString().Trim();
                }


                p_yardage = TXTYARDAGE.Text;
                if (chkHangTagRequire.Checked)
                {
                    p_require_tag = "Y";
                }
                else
                {
                    p_require_tag = "N";
                }
                p_machine_dense = txtMachineTense.Text;
                if (chkYarnAvailable.Checked)
                {
                    p_yarn_available = "Y";// chkYarnAvailable.Checked.ToString();
                }
                else
                {
                    p_yarn_available = "N";
                }
                p_yarn_available_date = txtyarnavailabledate.Value;
                p_knitting_appointment = txtknittingappoinment.Value;
                p_offer_price = "";
                p_is_consideration = "";
                p_expected_finished_date = txtExpectedFinisheddate.Value;
                p_spec_master_date = txtspecmasterdate.Value;
                p_qa_report_date = txtqa_reportdate.Value;
                p_remark = txtremark.Text;
                //if (uploadfiles1.PostedFile.FileName.Length > 0)
                //{
                //    fileName = Path.GetFileName(uploadfiles1.PostedFile.FileName);
                //    string filename1 = Path.GetDirectoryName(uploadfiles1.PostedFile.FileName);
                //    HttpPostedFile file = Request.Files["browserHidden"];
                //    FileInfo fileInfo = new FileInfo(fileName);
                //    string directoryFullPath = fileInfo.DirectoryName;
                //    fileName = "//172.16.3.4/pdr_files/ANALYZE/" + fileName;
                //}
                //else
                //{
                //    fileName = lblFileName.Text.ToString().Trim();
                //}
                p_endbuyercd = ddlEndBuyer.SelectedValue;
                p_priority_id = ddlPriority.SelectedValue;
                p_priority_comment = txtPriorityComment.Text;
                p_price_curr = ddlcombocurrency.SelectedValue.ToString().Trim();
                p_itcd1 = ddlCode1.SelectedValue;
                p_itcd2 = ddlCode2.SelectedValue;
                p_itcd3 = ddlCode3.SelectedValue;
                p_itcd4 = ddlCode4.SelectedValue;
                p_itcd5 = ddlCode5.SelectedValue;
                p_itcd6 = ddlCode6.SelectedValue;
                if (txtAvail1.Text == "")
                {
                    txtAvail1.Text = "0";

                }
                if (txtAvail2.Text == "")
                {
                    txtAvail2.Text = "0";

                }
                if (txtAvail3.Text == "")
                {
                    txtAvail3.Text = "0";

                }
                if (txtAvail4.Text == "")
                {
                    txtAvail4.Text = "0";

                }
                if (txtAvail5.Text == "")
                {
                    txtAvail5.Text = "0";

                }
                if (txtAvail6.Text == "")
                {
                    txtAvail6.Text = "0";

                }

                p_avail1 = Convert.ToDecimal(txtAvail1.Text);
                p_avail2 = Convert.ToDecimal(txtAvail2.Text);
                p_avail3 = Convert.ToDecimal(txtAvail3.Text);
                p_avail4 = Convert.ToDecimal(txtAvail4.Text);
                p_avail5 = Convert.ToDecimal(txtAvail5.Text);
                p_avail6 = Convert.ToDecimal(txtAvail6.Text);
                    if (txtkgperfinishedroll.Text =="")
                    {
                        p_kg_per_finished_roll = 0;// Convert.ToDecimal(txtkgperfinishedroll.Text);
                    }
                    else
                    {
                        p_kg_per_finished_roll = Convert.ToDecimal(txtkgperfinishedroll.Text);
                    }
                if (txtTotalFinishedRolls.Text =="")
                    {
                        p_no_of_finished_rolls = 0;// Convert.ToDecimal(txtTotalFinishedRolls.Text);
                    }
                else
                    {
                        p_no_of_finished_rolls = Convert.ToDecimal(txtTotalFinishedRolls.Text);
                    }
                    if (chkshortage1.Checked)
                    {
                        p_yarn_shortage1 = "Y";
                    }
                    else
                    {
                        p_yarn_shortage1 = "N";
                    }
                    if (chkshortage2.Checked)
                    {
                        p_yarn_shortage2 = "Y";
                    }
                    else
                    {
                        p_yarn_shortage2 = "N";
                    }
                    if (chkshortage3.Checked)
                    {
                        p_yarn_shortage3 = "Y";
                    }
                    else
                    {
                        p_yarn_shortage3 = "N";
                    }
                    if (chkshortage4.Checked)
                    {
                        p_yarn_shortage4 = "Y";
                    }
                    else
                    {
                        p_yarn_shortage4 = "N";
                    }
                    if (chkshortage5.Checked)
                    {
                        p_yarn_shortage5 = "Y";
                    }
                    else
                    {
                        p_yarn_shortage5 = "N";
                    }
                    if (chkshortage6.Checked)
                    {
                        p_yarn_shortage6 = "Y";
                    }
                    else
                    {
                        p_yarn_shortage6 = "N";
                    }
                    if (chknewyarn1.Checked)
                    {
                        p_new_yarn1 = "Y";
                    }
                    else
                    {
                        p_new_yarn1 = "N";
                    }
                    if (chknewyarn2.Checked)
                    {
                        p_new_yarn2 = "Y";
                    }
                    else
                    {
                        p_new_yarn2 = "N";
                    }
                    if (chknewyarn3.Checked)
                    {
                        p_new_yarn3 = "Y";
                    }
                    else
                    {
                        p_new_yarn3 = "N";
                    }
                    if (chknewyarn4.Checked)
                    {
                        p_new_yarn4 = "Y";
                    }
                    else
                    {
                        p_new_yarn4 = "N";
                    }
                    if (chknewyarn5.Checked)
                    {
                        p_new_yarn5 = "Y";
                    }
                    else
                    {
                        p_new_yarn5 = "N";
                    }
                    if (chknewyarn6.Checked)
                    {
                        p_new_yarn6 = "Y";
                    }
                    else
                    {
                        p_new_yarn6 = "N";
                    }
                    p_test_method_id = ddlTestMethod.SelectedValue;
                    p_shoe_size_id = ddlShoeSize.SelectedValue;
                    p_shoe_style = txtShoeStyle.Text;
                    p_shoe_gender_id = ddlShoeGender.SelectedValue;
                    p_shoe_length = txtShoeLength.Text;
                    p_shoe_width = txtShoeWidth.Text;
                    p_pallet_pattern_no = txtPalletPatternNo.Text;
                    p_product_pattern_no = txtProductPatternNo.Text;
                    p_shoe_pairs_required= txtPairs.Text;

                    if (chkLaserCut.Checked)
                    {
                        p_shoe_require_laser_cut = "Y";
                    }
                    else
                    {
                        p_shoe_require_laser_cut = "N";
                    }

                    if (chkWithPattern .Checked)
                    {
                        p_laser_cut_with_pattern = "Y";
                    }
                    else
                    {
                        p_laser_cut_with_pattern = "N";
                    }
                    p_repeat_per_roll = txtRptPerRoll.Text.Trim();

                    classDevelop_Request_BLL obj = new classDevelop_Request_BLL();
                DataTable dtresult = obj.UpdateDevelopREquest(p_pdr_new_develop_req_id, p_pdr_no, p_pdr_date, p_design_no, p_gauge,
                                                        p_custcd, p_requested_by, p_expected_price, p_uom, p_expected_date,
                                                        p_product_application_id, p_development_type_id, p_development_type_remark,
                                                        p_itcatid, p_itsubcatid, p_itgroupid, p_itsubid, p_ittypeid, p_itsubid2,
                                                         p_standard_aatcc, p_standard_astm, p_standard_iso, p_standard_jis, p_standard_ms,
                                                       p_standard_hm, p_standard_vss, p_standard_customer,
                                                        p_follow_customer_spec, p_yarn_face1, p_yarn_face2, p_yarn_face3, p_yarn_face4,
                                                        p_yarn_face5, p_yarn_face6,
                                                         p_composition1_percent, p_composition2_percent, p_composition3_percent, p_composition4_percent,
                                                         p_composition5_percent, p_composition6_percent,
                                                        p_composition1_name, p_composition2_name, p_composition3_name, p_composition4_name,
                                                        p_composition5_name, p_composition6_name, p_weight_sqm,
                                                         p_weight_sqm_tolerance, p_weight_sqm_remark, p_width, p_width_tolerance, p_elongation,
                                                        p_elongation_l, p_elongation_w, p_elongation_tolerance, p_modulus, p_modulus_l, p_modulus_w,
                                                        p_modulus_tolerance, p_stretch_l, p_stretch_w, p_stretch_tolerance, p_shrinkage_l, p_shrinkage_w,
                                                        p_shrinkage_tolerance, p_piling, p_pilling_remark, p_snagging, p_snagging_remark, p_color_name,
                                                        p_dye_finishing_formula_id, p_dye_remark, p_hanger, p_yardage, p_require_tag, p_machine_dense,
                                                        p_yarn_available, p_yarn_available_date, p_knitting_appointment, p_offer_price, p_is_consideration,
                                                        p_expected_finished_date, p_spec_master_date, p_qa_report_date, p_remark, p_user_session_id,
                                                        fileName, p_endbuyercd, p_priority_id, p_priority_comment, p_price_curr, p_itcd1, p_itcd2,
                                                        p_itcd3, p_itcd4, p_itcd5, p_itcd6,
                                                        p_avail1, p_avail2, p_avail3, p_avail4, p_avail5, p_avail6, p_kg_per_finished_roll, p_no_of_finished_rolls,
                                                        p_yarn_shortage1, p_yarn_shortage2, p_yarn_shortage3, p_yarn_shortage4, p_yarn_shortage5, p_yarn_shortage6,
                                                        p_new_yarn1, p_new_yarn2, p_new_yarn3, p_new_yarn4, p_new_yarn5, p_new_yarn6,p_test_method_id,
                                                        p_shoe_style,p_shoe_size_id,p_shoe_gender_id,p_shoe_length,p_shoe_width,p_pallet_pattern_no,p_product_pattern_no,
                                                        p_shoe_require_laser_cut, p_laser_cut_with_pattern,p_shoe_pairs_required,p_repeat_per_roll);
                valuechanged = "NO";

                foreach (DataRow resultrow in dtresult.Rows)
                {
                    hidpdr_new_develop_req_id.Value = resultrow[0].ToString();
                    txtPDRNO.Text = (resultrow[1].ToString());
                }
                //fileName = Path.GetFileName(uploadfiles1.FileName);
                //if (fileName != "")
                //{
                //    bool exists = System.IO.Directory.Exists(Server.MapPath("~/ANALYZE/"));

                //    if (!exists)
                //        System.IO.Directory.CreateDirectory(Server.MapPath("~/ANALYZE/"));
                //    uploadfiles1.SaveAs("//172.16.3.4/pdr_files/ANALYZE/" + fileName);
                //    // lblMsg.Text = "File Uploaded Successfully";
                //}
                // Save the Grid Value
                DataTable dtsave = new DataTable();
                dtsave.Columns.AddRange(new DataColumn[8] { new DataColumn("p_pdr_item_properties_id", typeof(int)),
                new DataColumn("p_pdr_new_develop_req_id", typeof(int)),
                new DataColumn("p_appl_id",typeof(string)),
                new DataColumn("p_sub_appl_id",typeof(string)),
                new DataColumn("p_spl_func_id",typeof(string)),
                 new DataColumn("p_ctry",typeof(string)),
                new DataColumn("p_market_zone_id",typeof(string)),
                new DataColumn("p_market_customer_id",typeof(string))
                 });
                Int64 pdr_item_properties_id;
                string pdr_new_develop_req_id = hidpdr_new_develop_req_id.Value.ToString().Trim();
                string appl_id;
                string sub_appl_id;
                string spl_func_id;
                string ctry;
                string market_zone_id;
                string market_customer_id;
                DataTable dtretunvalue;
                // newly added to check 
                DataTable dtCurrentTable = (DataTable)ViewState["pdrDevReq"];
                // Response.Write(dtCurrentTable.Rows.Count);
                if (dtCurrentTable !=null)
                    {
                        DataRow drCurrentRow = dtCurrentTable.NewRow();// null;
                    }
                
                Int32 counti = 0;
                Int32 editrowcount = 0;
                //
                foreach (GridViewRow devreqrow in gvDevelop_Request.Rows)
                {
                    DropDownList ddlApp = devreqrow.FindControl("ddlAppl") as DropDownList;
                    //  DropDownList dt_ddlApp = gvDevelop_Request.Rows[editrowcount].FindControl("ddlAppl") as DropDownList;

                    //string appid= dt_ddlApp.SelectedItem.Value.ToString().Trim(); //ddlApp.SelectedItem.Value.ToString().Trim();
                    appl_id = Request[ddlApp.UniqueID] as string;
                    DropDownList ddlsubApp = devreqrow.FindControl("ddlSubAppl") as DropDownList;
                    sub_appl_id = Request[ddlsubApp.UniqueID] as string;
                    DropDownList ddlsplfunc = devreqrow.FindControl("ddlsplfunc") as DropDownList;
                    spl_func_id = Request[ddlsplfunc.UniqueID] as string;
                    DropDownList ddlcountry = devreqrow.FindControl("ddlctry") as DropDownList;
                    ctry = Request[ddlcountry.UniqueID] as string;
                    DropDownList ddlZone = devreqrow.FindControl("ddlzone") as DropDownList;
                    market_zone_id = Request[ddlZone.UniqueID] as string;
                    DropDownList ddlcustomer = devreqrow.FindControl("ddlcustomer") as DropDownList;
                    market_customer_id = Request[ddlcustomer.UniqueID] as string;
                    if (gvDevelop_Request.DataKeys[devreqrow.RowIndex].Values[1].ToString() != string.Empty)
                    {
                        pdr_item_properties_id = Convert.ToInt64(gvDevelop_Request.DataKeys[devreqrow.RowIndex].Values[1].ToString().Trim());
                    }
                    else
                    {
                        pdr_item_properties_id = -1;
                    }
                    //dtsave.Rows.Add(pdr_item_properties_id, pdr_new_develop_req_id, appl_id, sub_appl_id, spl_func_id, ctry, market_zone_id, market_customer_id);
                    //  editrowcount = editrowcount + 1;
                }
                //DataTable dtCurrentTable = (DataTable)ViewState["pdrDevReq"];
                //// Response.Write(dtCurrentTable.Rows.Count);
                //DataRow drCurrentRow = dtCurrentTable.NewRow();// null;
                //Int32 counti = 0;
                //Int32 
                editrowcount = 0;
                    if (dtCurrentTable != null)
                    {
                        foreach (DataRow editrow in dtCurrentTable.Rows)
                        {
                            DropDownList ddlApp = gvDevelop_Request.Rows[editrowcount].FindControl("ddlAppl") as DropDownList;
                            appl_id = ddlApp.SelectedItem.Value.ToString().Trim();// appl_name;

                            DropDownList ddlSubAppl = gvDevelop_Request.Rows[editrowcount].FindControl("ddlSubAppl") as DropDownList;
                            sub_appl_id = ddlSubAppl.SelectedItem.Value.ToString().Trim();// sub_appl_name;

                            DropDownList ddlsplfunc = gvDevelop_Request.Rows[editrowcount].FindControl("ddlsplfunc") as DropDownList;
                            spl_func_id = ddlsplfunc.SelectedItem.Value.ToString().Trim();// sub_appl_name;

                            DropDownList ddlctry = gvDevelop_Request.Rows[editrowcount].FindControl("ddlctry") as DropDownList;
                            ctry = ddlctry.SelectedItem.Value.ToString().Trim();// sub_appl_name;

                            DropDownList ddlzone = gvDevelop_Request.Rows[editrowcount].FindControl("ddlzone") as DropDownList;
                            market_zone_id = ddlzone.SelectedItem.Value.ToString().Trim();// sub_appl_name;

                            DropDownList ddlcustomer = gvDevelop_Request.Rows[editrowcount].FindControl("ddlcustomer") as DropDownList;
                            market_customer_id = ddlcustomer.SelectedItem.Value.ToString().Trim();// sub_appl_name;
                            if (editrow["pdr_item_properties_id"].ToString() != string.Empty)
                            {
                                pdr_item_properties_id = Convert.ToInt64(editrow["pdr_item_properties_id"].ToString().Trim());// Convert.ToInt64(gvDevelop_Request.Rows[editrowcount].Cells[0].Text.ToString().Trim());
                            }
                            else
                            {
                                pdr_item_properties_id = -1;
                            }
                            dtsave.Rows.Add(pdr_item_properties_id, pdr_new_develop_req_id, appl_id, sub_appl_id, spl_func_id, ctry, market_zone_id, market_customer_id);
                            editrowcount = editrowcount + 1;
                            // editrow.EndEdit();
                            // dtCurrentTable.AcceptChanges();
                            // ddlApp.SelectedItem.Value = ddlApp.SelectedItem.Value.ToString().Trim();
                        }
                    }
                dtretunvalue = obj.Save_DevRequestApp(dtsave);
                DataTable dtdevreq;
                classDevelop_Request_BLL devrequest = new classDevelop_Request_BLL();
                dtdevreq = devrequest.LoadDeveRequestApp(hidpdr_new_develop_req_id.Value.ToString().Trim());
                gvDevelop_Request.DataSource = dtdevreq;
                gvDevelop_Request.DataBind();
                ViewState["pdrDevReq"] = dtdevreq;
                if (dtdevreq.Rows.Count <= 0)
                {
                    BindHeaderRowInGridview();
                }
                //
                classDevelop_Request_BLL developRequestnew = new classDevelop_Request_BLL();
                dtpdrlist = developRequestnew.LoadPDRList(txtPDRNO.Text.Trim());
                ViewState["pdrlist"] = dtpdrlist;
                string followcustomerspec = "";
                if (dtpdrlist != null)
                {
                    foreach (DataRow newrow in dtpdrlist.Rows)
                    {

                        hidpdr_new_develop_req_id.Value = newrow["pdr_new_develop_req_id"].ToString();
                        txtPDRNO.Text = (newrow["pdr_no"].ToString());
                        if (!string.IsNullOrEmpty(newrow["pdr_date"].ToString()))
                        {
                            DateTime dt = Convert.ToDateTime(newrow["pdr_date"].ToString());
                            PDRDate.Value = String.Format("{0:dd-MM-yyyy}", dt);// dt.ToString("dd-MM-yyyy");// String.Format("M/d/yyyy", dt);// Convert.ToDateTime(row["pdr_date"]).ToString();
                        }
                        if (!string.IsNullOrEmpty(newrow["design_no"].ToString()))
                        {
                            txtDesignno.Text = newrow["design_no"].ToString();
                        }
                        ddlSubCat.SelectedValue = newrow["itsubcatid"].ToString();
                        ddlgroup.SelectedValue = newrow["itgroupid"].ToString();
                        ddlCategory.SelectedValue = newrow["itcatid"].ToString();

                        ddlSubGroup.SelectedValue = newrow["itsubid"].ToString();
                        ddlType.SelectedValue = newrow["ittypeid"].ToString();
                        ddlSubType.SelectedValue = newrow["itsubid2"].ToString();
                        txtDevelopmentTypeRemark.Text = newrow["development_type_remark"].ToString();

                        ddlPriority.SelectedValue = newrow["priority_id"].ToString();
                        txtPriorityComment.Text = newrow["priority_comment"].ToString();
                        ddlcombocurrency.SelectedValue = newrow["price_curr"].ToString().Trim();
                        ddlCode1.SelectedValue = newrow["itcd1"].ToString();
                        ddlCode2.SelectedValue = newrow["itcd2"].ToString();
                        ddlCode3.SelectedValue = newrow["itcd3"].ToString();
                        ddlCode4.SelectedValue = newrow["itcd4"].ToString();
                        ddlCode5.SelectedValue = newrow["itcd5"].ToString();
                        ddlCode6.SelectedValue = newrow["itcd6"].ToString();
                        ddlEndBuyer.SelectedValue = newrow["endbuyerid"].ToString();
                            if (!string.IsNullOrEmpty(newrow["preparer_name"].ToString()))
                            {
                                txtPrepareBy.Text = newrow["preparer_name"].ToString();
                            }
                        if (!string.IsNullOrEmpty(newrow["follow_customer_spec"].ToString()))
                        {
                            followcustomerspec = newrow["follow_customer_spec"].ToString();
                            if (followcustomerspec == "N")
                            {
                                CHKFollow.Checked = false;

                            }

                            else
                            {
                                CHKFollow.Checked = true;
                            }
                        }
                        else
                        {
                            CHKFollow.Checked = false;

                        }

                        txtDevelopmentTypeRemark.Text = newrow["development_type_remark"].ToString();
                        txtComposition1_name.Text = newrow["composition1_name"].ToString();
                        txtComposition2_name.Text = newrow["composition2_name"].ToString();
                        txtComposition3_name.Text = newrow["composition3_name"].ToString();
                        txtComposition4_name.Text = newrow["composition4_name"].ToString();
                        txtweight.Text = newrow["weight_sqm"].ToString();
                        TXTWIDTH.Text = newrow["width"].ToString();
                        TXTElongation.Text = newrow["elongation"].ToString();
                        TXTMODULUS.Text = newrow["modulus"].ToString();
                        txtRecovStretbility.Text = "";
                        txtDimStbilityShr.Text = "";
                        txtremark.Text = newrow["remark"].ToString();
                        txtweightTolerance.Text = newrow["weight_sqm_tolerance"].ToString();
                        txtwidth_Tolerance.Text = newrow["width_tolerance"].ToString();
                        txtpilingremark.Text = newrow["pilling_remark"].ToString();
                        txtsnaggingremark.Text = newrow["snagging_remark"].ToString();
                        txtdyeincolor.Text = newrow["color_name"].ToString();
                        txtMachineTense.Text = newrow["machine_dense"].ToString();
                        // if (row["knitting_appointment"] != null || row["knitting_appointment"].ToString() != "{}")
                        if (!string.IsNullOrEmpty(newrow["knitting_appointment"].ToString()))
                        {

                            DateTime dt1 = Convert.ToDateTime(newrow["knitting_appointment"].ToString());
                            txtknittingappoinment.Value = String.Format("{0:dd-MM-yyyy}", dt1);// row["knitting_appointment"].ToString();
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_available_date"].ToString()))
                        {
                            DateTime dt2 = Convert.ToDateTime(newrow["yarn_available_date"].ToString());
                            txtyarnavailabledate.Value = String.Format("{0:dd-MM-yyyy}", dt2);// row["yarn_available_date"].ToString();
                        }
                        if (!string.IsNullOrEmpty(newrow["expected_finished_date"].ToString()))
                        {
                            DateTime dt3 = Convert.ToDateTime(newrow["expected_finished_date"].ToString());
                            txtExpectedFinisheddate.Value = String.Format("{0:dd-MM-yyyy}", dt3);//row["expected_finished_date"].ToString();
                        }
                        if (!string.IsNullOrEmpty(newrow["spec_master_date"].ToString()))
                        {
                            DateTime dt4 = Convert.ToDateTime(newrow["spec_master_date"].ToString());
                            txtspecmasterdate.Value = String.Format("{0:dd-MM-yyyy}", dt4);//row["spec_master_date"].ToString();
                        }
                        if (!string.IsNullOrEmpty(newrow["qa_report_date"].ToString()))
                        {
                            DateTime dt5 = Convert.ToDateTime(newrow["qa_report_date"].ToString());
                            txtqa_reportdate.Value = String.Format("{0:dd-MM-yyyy}", dt5);//= row["qa_report_date"].ToString();
                        }

                        txtStandard_Customer.Text = newrow["test_method_text"].ToString();
                        txtCustPriceExpectation.Text = newrow["expected_price"].ToString();
                        txtPercent1.Text = newrow["composition1_percent"].ToString();
                        txtPercent2.Text = newrow["composition2_percent"].ToString();
                        txtPercent3.Text = newrow["composition3_percent"].ToString();
                        txtPercent4.Text = newrow["composition4_percent"].ToString();
                        txtPercent5.Text = newrow["composition5_percent"].ToString();
                        txtPercent6.Text = newrow["composition6_percent"].ToString();

                        TXTGUAGE.Text = newrow["gauge"].ToString();
                        if (!string.IsNullOrEmpty(newrow["yarn_face1"].ToString()))
                        {
                            ddlFront.SelectedValue = newrow["yarn_face1"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_face2"].ToString()))
                        {
                            ddlBack.SelectedValue = newrow["yarn_face2"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_face3"].ToString()))
                        {


                            ddlfront1.SelectedValue = newrow["yarn_face3"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_face4"].ToString()))
                        {
                            ddlfront2.SelectedValue = newrow["yarn_face4"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_face5"].ToString()))
                        {
                            ddlfront5.SelectedValue = newrow["yarn_face5"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_face6"].ToString()))
                        {
                            ddlfront6.SelectedValue = newrow["yarn_face6"].ToString().Trim();
                        }

                        txtAvail1.Text = newrow["yarn_avail_qty1"].ToString();
                        txtAvail2.Text = newrow["yarn_avail_qty2"].ToString();
                        txtAvail3.Text = newrow["yarn_avail_qty3"].ToString();
                        txtAvail4.Text = newrow["yarn_avail_qty4"].ToString();
                        txtAvail5.Text = newrow["yarn_avail_qty5"].ToString();
                        txtAvail6.Text = newrow["yarn_avail_qty6"].ToString();

                        if (!string.IsNullOrEmpty(newrow["dye_finishing_formula_id"].ToString()))
                        {
                            ddlFinishing.SelectedValue = newrow["dye_finishing_formula_id"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(newrow["require_tag"].ToString()))
                        {
                            string gangtagrequired = newrow["require_tag"].ToString();
                            if (gangtagrequired == "N" || gangtagrequired == "")
                            {
                                chkHangTagRequire.Checked = false;
                            }
                            else
                            {
                                chkHangTagRequire.Checked = true;
                            }
                        }



                        TXTYARDAGE.Text = newrow["yardage"].ToString();
                        string snagging = newrow["snagging"].ToString();
                        if (snagging == "N")
                        {
                            chksnagging.Checked = false;
                        }
                        else
                        {
                            chksnagging.Checked = true;
                        }
                        string pilling = newrow["piling"].ToString();
                        if (pilling == "N")
                        {
                            chkpilling.Checked = false;
                        }
                        else
                        {
                            chkpilling.Checked = true;
                        }

                        string standard_aatcc = newrow["standard_aatcc"].ToString();
                        if (standard_aatcc == "N" || standard_aatcc == "0")
                        {
                            CHKAATCC.Checked = false;
                        }
                        else
                        {
                            CHKAATCC.Checked = true;
                        }
                        string standard_astm = newrow["standard_astm"].ToString();
                        if (standard_astm == "N" || standard_astm == "0")
                        {
                            CHKASTM.Checked = false;
                        }
                        else
                        {
                            CHKASTM.Checked = true;
                        }

                        string standard_iso = newrow["standard_iso"].ToString();
                        if (standard_iso == "N" || standard_iso == "0")
                        {
                            CHKISO.Checked = false;
                        }
                        else
                        {
                            CHKISO.Checked = true;
                        }
                        string standard_jis = newrow["standard_jis"].ToString();
                        if (standard_jis == "N" || standard_jis == "0")
                        {
                            CHKJIS.Checked = false;
                        }
                        else
                        {
                            CHKJIS.Checked = true;
                        }
                        string standard_ms = newrow["standard_ms"].ToString();
                        if (standard_ms == "N" || standard_ms == "0")
                        {
                            chkMS.Checked = false;
                        }
                        else
                        {
                            chkMS.Checked = true;
                        }
                        string standard_hm = newrow["standard_hm"].ToString();
                        if (standard_hm == "N" || standard_hm == "0")
                        {
                            chkHM.Checked = false;
                        }
                        else
                        {
                            chkHM.Checked = true;
                        }
                        string standard_vss = newrow["standard_vss"].ToString();
                        if (standard_vss == "N" || standard_vss == "0")
                        {
                            chkVSS.Checked = false;
                        }
                        else
                        {
                            chkVSS.Checked = true;
                        }

                        txtElongation_l.Text = newrow["elongation_l"].ToString();
                        txtmodulus_l.Text = newrow["modulus_l"].ToString();
                        txtstretch_l.Text = newrow["stretch_l"].ToString();
                        txtshrinkage_l.Text = newrow["shrinkage_l"].ToString();
                        txtElongation_w.Text = newrow["elongation_w"].ToString();
                        txtmodulus_w.Text = newrow["modulus_w"].ToString();
                        txtstretch_w.Text = newrow["stretch_w"].ToString();
                        txtshrinkage_w.Text = newrow["shrinkage_w"].ToString();
                        txtelongation_tolerance.Text = newrow["elongation_tolerance"].ToString();
                        txtmodulus_tolerance.Text = newrow["modulus_tolerance"].ToString();
                        txtshrinkage_tolerance.Text = newrow["shrinkage_tolerance"].ToString();
                        txtstretch_tolerance.Text = newrow["stretch_tolerance"].ToString();
                        ddlReasonForDevl.SelectedValue = newrow["development_type_id"].ToString();
                        //  ddlPurposeofGarment.SelectedValue = newrow["product_application_id"].ToString();
                        if (!string.IsNullOrEmpty(newrow["expected_date"].ToString()))
                        {
                            DateTime dt6 = Convert.ToDateTime(newrow["expected_date"].ToString());
                            dtTo.Value = String.Format("{0:dd-MM-yyyy}", dt6);//= row["qa_report_date"].ToString();
                                                                              //dtTo.Value = row["expected_date"].ToString();
                        }

                        txtSampleRequire.Text = newrow["hanger"].ToString();
                        txtStandard_Customer.Text = newrow["standard_customer"].ToString();
                        //txtComment.Text = 
                        string yarnavailable = newrow["yarn_available"].ToString();
                        if (yarnavailable == "N")
                        {
                            chkYarnAvailable.Checked = false;
                        }
                        else
                        {
                            chkYarnAvailable.Checked = true;
                        }

                        txtweightTolerance.Text = newrow["weight_sqm_tolerance"].ToString();
                        txtwidth_Tolerance.Text = newrow["width_tolerance"].ToString();
                            txtkgperfinishedroll.Text = newrow["kg_per_finished_roll"].ToString();
                            txtTotalFinishedRolls.Text = newrow["no_of_finished_rolls"].ToString();
                            if (!string.IsNullOrEmpty(newrow["yarn_shortage1"].ToString()))
                            {
                                string yarn_shortage1 = newrow["yarn_shortage1"].ToString();
                                if (yarn_shortage1 == "N" || yarn_shortage1 == "")
                                {
                                    chkshortage1.Checked = false;
                                }
                                else
                                {
                                    chkshortage1.Checked = true;
                                }
                            }

                            if (!string.IsNullOrEmpty(newrow["yarn_shortage2"].ToString()))
                            {
                                string yarn_shortage2 = newrow["yarn_shortage2"].ToString();
                                if (yarn_shortage2 == "N" || yarn_shortage2 == "")
                                {
                                    chkshortage2.Checked = false;
                                }
                                else
                                {
                                    chkshortage2.Checked = true;
                                }
                            }
                            if (!string.IsNullOrEmpty(newrow["yarn_shortage3"].ToString()))
                            {
                                string yarn_shortage3 = newrow["yarn_shortage3"].ToString();
                                if (yarn_shortage3 == "N" || yarn_shortage3 == "")
                                {
                                    chkshortage3.Checked = false;
                                }
                                else
                                {
                                    chkshortage3.Checked = true;
                                }
                            }
                            if (!string.IsNullOrEmpty(newrow["yarn_shortage4"].ToString()))
                            {
                                string yarn_shortage4 = newrow["yarn_shortage4"].ToString();
                                if (yarn_shortage4 == "N" || yarn_shortage4 == "")
                                {
                                    chkshortage4.Checked = false;
                                }
                                else
                                {
                                    chkshortage4.Checked = true;
                                }
                            }
                            if (!string.IsNullOrEmpty(newrow["yarn_shortage5"].ToString()))
                            {
                                string yarn_shortage5 = newrow["yarn_shortage5"].ToString();
                                if (yarn_shortage5 == "N" || yarn_shortage5 == "")
                                {
                                    chkshortage5.Checked = false;
                                }
                                else
                                {
                                    chkshortage5.Checked = true;
                                }
                            }
                            if (!string.IsNullOrEmpty(newrow["yarn_shortage6"].ToString()))
                            {
                                string yarn_shortage6 = newrow["yarn_shortage6"].ToString();
                                if (yarn_shortage6 == "N" || yarn_shortage6 == "")
                                {
                                    chkshortage6.Checked = false;
                                }
                                else
                                {
                                    chkshortage6.Checked = true;
                                }
                            }
                            if (!string.IsNullOrEmpty(newrow["new_yarn1"].ToString()))
                            {
                                string new_yarn1 = newrow["new_yarn1"].ToString();
                                if (new_yarn1 == "N" || new_yarn1 == "")
                                {
                                    chknewyarn1.Checked = false;
                                }
                                else
                                {
                                    chknewyarn1.Checked = true;
                                }
                            }
                            if (!string.IsNullOrEmpty(newrow["new_yarn2"].ToString()))
                            {
                                string new_yarn2 = newrow["new_yarn2"].ToString();
                                if (new_yarn2 == "N" || new_yarn2 == "")
                                {
                                    chknewyarn2.Checked = false;
                                }
                                else
                                {
                                    chknewyarn2.Checked = true;
                                }
                            }
                            if (!string.IsNullOrEmpty(newrow["new_yarn3"].ToString()))
                            {
                                string new_yarn3 = newrow["new_yarn3"].ToString();
                                if (new_yarn3 == "N" || new_yarn3 == "")
                                {
                                    chknewyarn3.Checked = false;
                                }
                                else
                                {
                                    chknewyarn3.Checked = true;
                                }
                            }
                            if (!string.IsNullOrEmpty(newrow["new_yarn4"].ToString()))
                            {
                                string new_yarn4 = newrow["new_yarn4"].ToString();
                                if (new_yarn4 == "N" || new_yarn4 == "")
                                {
                                    chknewyarn4.Checked = false;
                                }
                                else
                                {
                                    chknewyarn4.Checked = true;
                                }
                            }
                            if (!string.IsNullOrEmpty(newrow["new_yarn5"].ToString()))
                            {
                                string new_yarn5 = newrow["new_yarn5"].ToString();
                                if (new_yarn5 == "N" || new_yarn5 == "")
                                {
                                    chknewyarn5.Checked = false;
                                }
                                else
                                {
                                    chknewyarn5.Checked = true;
                                }
                            }
                            if (!string.IsNullOrEmpty(newrow["new_yarn6"].ToString()))
                            {
                                string new_yarn6 = newrow["new_yarn6"].ToString();
                                if (new_yarn6 == "N" || new_yarn6 == "")
                                {
                                    chknewyarn6.Checked = false;
                                }
                                else
                                {
                                    chknewyarn6.Checked = true;
                                }
                            }
                            //lblFileName.Text = newrow["file_location"].ToString();
                        }
                }
               
                // DataTable dtCurrentTable = (DataTable)ViewState["pdrlist"];
                // DataRow drCurrentRow = dtCurrentTable.NewRow();
                // drCurrentRow["pdr_new_develop_req_id"] = p_pdr_new_develop_req_id;
                // drCurrentRow["pdr_no"] = p_pdr_no;
                // drCurrentRow["pdr_date"] = PDRDate.Value;
                //  drCurrentRow["itsubcatid"] = ddlSubCat.SelectedValue;
                // drCurrentRow["itgroupid"] = ddlgroup.SelectedValue;
                // if (ddlCategory.SelectedValue != "")
                // {
                //     drCurrentRow["itcatid"] = (ddlCategory.SelectedValue.ToString());
                // }
                // else
                // {
                //     drCurrentRow["itcatid"] = DBNull.Value;
                // }
                //     if (ddlSubGroup.SelectedValue != "")
                // {
                //     drCurrentRow["itsubid"] = ddlSubGroup.SelectedValue;
                // }
                // else
                // {
                //     drCurrentRow["itsubid"] = DBNull.Value;
                // }
                // if (ddlType.SelectedValue != "")
                // {
                //     drCurrentRow["ittypeid"] = ddlType.SelectedValue;
                // }
                // else
                // {
                //     drCurrentRow["ittypeid"] = DBNull.Value;
                // }
                // if (ddlSubType.SelectedValue != "")
                // {
                //     drCurrentRow["itsubid2"] = ddlSubType.SelectedValue;
                // }
                // else
                // {
                //     drCurrentRow["itsubid2"]= DBNull.Value;
                // }
                // drCurrentRow["development_type_remark"] = txtDevelopmentTypeRemark.Text;
                // drCurrentRow["follow_customer_spec"] = CHKFollow.Checked;
                // drCurrentRow["development_type_remark"].ToString();
                // drCurrentRow["composition1_name"] = txtComposition1_name.Text.ToString();
                // drCurrentRow["composition2_name"]=txtComposition2_name.Text;
                // drCurrentRow["composition3_name"] = txtComposition3_name.Text;
                // drCurrentRow["composition4_name"] = txtComposition4_name.Text ;
                // drCurrentRow["weight_sqm"] = txtweight.Text;
                // drCurrentRow["width"] = TXTWIDTH.Text;
                // drCurrentRow["elongation"] = TXTElongation.Text;
                // drCurrentRow["modulus"] = TXTMODULUS.Text;
                // // txtRecovStretbility.Text = "";
                // //txtDimStbilityShr.Text = "";
                // drCurrentRow["remark"] = txtremark.Text;
                // drCurrentRow["weight_sqm_tolerance"] = txtweightTolerance.Text;
                // drCurrentRow["width_tolerance"] = txtwidth_Tolerance.Text;
                // drCurrentRow["pilling_remark"]=txtpilingremark.Text;
                // drCurrentRow["snagging_remark"]=txtsnaggingremark.Text;
                // drCurrentRow["color_name"]=txtdyeincolor.Text;
                // drCurrentRow["machine_dense"]=txtMachineTense.Text;
                // drCurrentRow["knitting_appointment"] = txtknittingappoinment.Value;
                // drCurrentRow["yarn_available_date"] = txtyarnavailabledate.Value;
                // drCurrentRow["expected_finished_date"] = txtExpectedFinisheddate.Value;
                // drCurrentRow["spec_master_date"] = txtspecmasterdate.Value;
                // drCurrentRow["qa_report_date"] = txtqa_reportdate.Value;
                // drCurrentRow["test_method_text"]=txtStandard_Customer.Text;
                // drCurrentRow["expected_price"]=txtCustPriceExpectation.Text;
                // drCurrentRow["composition1_percent"]=txtPercent1.Text;
                // drCurrentRow["composition2_percent"]=txtPercent2.Text;
                // drCurrentRow["composition3_percent"]=txtPercent3.Text;
                // drCurrentRow["composition4_percent"]=txtPercent4.Text;
                // drCurrentRow["gauge"]=TXTGUAGE.Text;
                // drCurrentRow["yarn_face1"] = ddlFront.SelectedValue.Trim();
                // drCurrentRow["yarn_face2"]=ddlBack.SelectedValue.Trim();
                // drCurrentRow["yarn_face3"]= ddlfront1.SelectedValue.Trim();
                // drCurrentRow["yarn_face4"]=ddlfront2.SelectedValue.Trim();
                // drCurrentRow["require_tag"] = chkHangTagRequire.Checked;
                // drCurrentRow["yardage"]=TXTYARDAGE.Text;
                // drCurrentRow["snagging"]= chksnagging.Checked;
                // drCurrentRow["piling"]= chkpilling.Checked;
                //drCurrentRow["standard_aatcc"]= CHKAATCC.Checked;
                //   drCurrentRow["standard_astm"]= CHKASTM.Checked;
                //drCurrentRow["standard_iso"]=CHKISO.Checked;
                // drCurrentRow["standard_jis"] = CHKJIS.Checked;
                // drCurrentRow["elongation_l"] =txtElongation_l.Text;
                // drCurrentRow["modulus_l"] =txtmodulus_l.Text;
                //  drCurrentRow["stretch_l"] = txtstretch_l.Text;
                //  drCurrentRow["shrinkage_l"] =txtshrinkage_l.Text;
                // drCurrentRow["elongation_w"]= txtElongation_w.Text;
                //   drCurrentRow["modulus_w"]=  txtmodulus_w.Text;
                //  drCurrentRow["stretch_w"]= txtstretch_w.Text;
                //   drCurrentRow["shrinkage_w"] =txtshrinkage_w.Text;
                //   drCurrentRow["elongation_tolerance"]= txtelongation_tolerance.Text;
                //  drCurrentRow["modulus_tolerance"]= txtmodulus_tolerance.Text;
                //  drCurrentRow["shrinkage_tolerance"]= txtshrinkage_tolerance.Text;
                //  drCurrentRow["stretch_tolerance"]= txtstretch_tolerance.Text;
                //   drCurrentRow["development_type_id"]= ddlReasonForDevl.SelectedValue;
                //   drCurrentRow["product_application_id"]= ddlPurposeofGarment.SelectedValue;
                //   drCurrentRow["expected_date"] =dtTo.Value;
                //   drCurrentRow["development_type_id"] =txtSampleRequire.Text;
                //   drCurrentRow["standard_customer"] =txtStandard_Customer.Text;
                // drCurrentRow["yarn_available"] = chkYarnAvailable.Checked;
                //   drCurrentRow["weight_sqm_tolerance"] =txtweightTolerance.Text;
                //   drCurrentRow["width_tolerance"] =txtwidth_Tolerance.Text;


            }
            catch (Exception ex)
            {
                lblErrorMSg.Visible = true;
                lblErrorMSg.Text = ex.Message.ToString();
            }
                }

        }


        protected void myButton_Click(object sender, EventArgs e)
        {
            string pdrno = txtPDRNO.Text;

            classDevelop_Request_BLL developRequest = new classDevelop_Request_BLL();
            dtpdrlist = developRequest.LoadPDRList(pdrno);
            string followcustomerspec = "";
            ViewState["pdrlist"] = dtpdrlist;
            DataTable dtCurrentTable = (DataTable)ViewState["pdrlist"];
            try
            {
                if (dtCurrentTable != null)
                {
                    foreach (DataRow dtrow in dtCurrentTable.Rows)
                    {
                        DateTime dt = Convert.ToDateTime(dtrow["pdr_date"].ToString());
                        hidpdr_new_develop_req_id.Value = dtrow["pdr_new_develop_req_id"].ToString();
                        txtPDRNO.Text = (dtrow["pdr_no"].ToString());
                        lblCancel.Text = (dtrow["pdr_cancel_label"].ToString());
                        hidPDRNOCancel.Value = (dtrow["pdr_cancel"].ToString());
                        if (dtrow["pdr_cancel"].ToString().Trim() == "Y")
                        {
                            ImgSave.Enabled = false;
                        }
                        else
                        {
                            ImgSave.Enabled = true;
                        }
                        txtSearchMain.Text = dtrow["customer_name"].ToString().Trim();
                        hidSearchMainValue.Value = dtrow["custcd"].ToString().Trim();
                        txtCustCode.Text = dtrow["custcd"].ToString().Trim();
                        PDRDate.Value = String.Format("{0:dd-MM-yyyy}", dt);// dt.ToString("dd-MM-yyyy");// String.Format("M/d/yyyy", dt);// Convert.ToDateTime(row["pdr_date"]).ToString();
                        ddlSubCat.SelectedValue = dtrow["itsubcatid"].ToString();
                        ddlgroup.SelectedValue = dtrow["itgroupid"].ToString();
                        ddlCategory.SelectedValue = dtrow["itcatid"].ToString();

                        ddlSubGroup.SelectedValue = dtrow["itsubid"].ToString();
                        ddlType.SelectedValue = dtrow["ittypeid"].ToString();
                        ddlSubType.SelectedValue = dtrow["itsubid2"].ToString();
                        txtDevelopmentTypeRemark.Text = dtrow["development_type_remark"].ToString();
                        followcustomerspec = dtrow["follow_customer_spec"].ToString();

                        ddlPriority.SelectedValue = dtrow["priority_id"].ToString();
                        txtPriorityComment.Text = dtrow["priority_comment"].ToString();
                        ddlcombocurrency.SelectedValue = dtrow["price_curr"].ToString().Trim();
                        ddlCode1.SelectedValue = dtrow["itcd1"].ToString();
                        ddlCode2.SelectedValue = dtrow["itcd2"].ToString();
                        ddlCode3.SelectedValue = dtrow["itcd3"].ToString();
                        ddlCode4.SelectedValue = dtrow["itcd4"].ToString();
                        ddlCode5.SelectedValue = dtrow["itcd5"].ToString();
                        ddlCode6.SelectedValue = dtrow["itcd6"].ToString();
                        ddlEndBuyer.SelectedValue = dtrow["endbuyerid"].ToString();
                        if (!string.IsNullOrEmpty(dtrow["design_no"].ToString()))
                        {
                            txtDesignno.Text = dtrow["design_no"].ToString();
                        }
                        if (followcustomerspec == "N")
                        {
                            CHKFollow.Checked = false;

                        }

                        else
                        {
                            CHKFollow.Checked = true;
                        }
                        txtDevelopmentTypeRemark.Text = dtrow["development_type_remark"].ToString();
                        txtComposition1_name.Text = dtrow["composition1_name"].ToString();
                        txtComposition2_name.Text = dtrow["composition2_name"].ToString();
                        txtComposition3_name.Text = dtrow["composition3_name"].ToString();
                        txtComposition4_name.Text = dtrow["composition4_name"].ToString();
                        txtComposition5_name.Text = dtrow["composition5_name"].ToString();
                        txtComposition6_name.Text = dtrow["composition6_name"].ToString();
                        txtweight.Text = dtrow["weight_sqm"].ToString();
                        TXTWIDTH.Text = dtrow["width"].ToString();
                        TXTElongation.Text = dtrow["elongation"].ToString();
                        TXTMODULUS.Text = dtrow["modulus"].ToString();
                        txtRecovStretbility.Text = "";
                        txtDimStbilityShr.Text = "";
                        txtremark.Text = dtrow["remark"].ToString();
                        txtweightTolerance.Text = dtrow["weight_sqm_tolerance"].ToString();
                        txtwidth_Tolerance.Text = dtrow["width_tolerance"].ToString();
                        txtpilingremark.Text = dtrow["pilling_remark"].ToString();
                        txtsnaggingremark.Text = dtrow["snagging_remark"].ToString();
                        txtdyeincolor.Text = dtrow["color_name"].ToString();
                        txtMachineTense.Text = dtrow["machine_dense"].ToString();
                        if (!string.IsNullOrEmpty(dtrow["knitting_appointment"].ToString()))
                        {

                            DateTime dt1 = Convert.ToDateTime(dtrow["knitting_appointment"].ToString());
                            txtknittingappoinment.Value = String.Format("{0:dd-MM-yyyy}", dt1);// row["knitting_appointment"].ToString();
                        }
                        if (!string.IsNullOrEmpty(dtrow["yarn_available_date"].ToString()))
                        {
                            DateTime dt2 = Convert.ToDateTime(dtrow["yarn_available_date"].ToString());
                            txtyarnavailabledate.Value = String.Format("{0:dd-MM-yyyy}", dt2);// row["yarn_available_date"].ToString();
                        }
                        if (!string.IsNullOrEmpty(dtrow["expected_finished_date"].ToString()))
                        {
                            DateTime dt3 = Convert.ToDateTime(dtrow["expected_finished_date"].ToString());
                            txtExpectedFinisheddate.Value = String.Format("{0:dd-MM-yyyy}", dt3);//row["expected_finished_date"].ToString();
                        }
                        if (!string.IsNullOrEmpty(dtrow["spec_master_date"].ToString()))
                        {
                            DateTime dt4 = Convert.ToDateTime(dtrow["spec_master_date"].ToString());
                            txtspecmasterdate.Value = String.Format("{0:dd-MM-yyyy}", dt4);//row["spec_master_date"].ToString();
                        }
                        if (!string.IsNullOrEmpty(dtrow["qa_report_date"].ToString()))
                        {
                            DateTime dt5 = Convert.ToDateTime(dtrow["qa_report_date"].ToString());
                            txtqa_reportdate.Value = String.Format("{0:dd-MM-yyyy}", dt5);//= row["qa_report_date"].ToString();
                        }

                        txtStandard_Customer.Text = dtrow["test_method_text"].ToString();
                        txtCustPriceExpectation.Text = dtrow["expected_price"].ToString();
                        txtPercent1.Text = dtrow["composition1_percent"].ToString();
                        txtPercent2.Text = dtrow["composition2_percent"].ToString();
                        txtPercent3.Text = dtrow["composition3_percent"].ToString();
                        txtPercent4.Text = dtrow["composition4_percent"].ToString();
                        txtPercent5.Text = dtrow["composition5_percent"].ToString();
                        txtPercent6.Text = dtrow["composition6_percent"].ToString();
                        TXTGUAGE.Text = dtrow["gauge"].ToString();
                        ddlFront.SelectedValue = dtrow["yarn_face1"].ToString().Trim();
                        ddlBack.SelectedValue = dtrow["yarn_face2"].ToString().Trim();
                        ddlfront1.SelectedValue = dtrow["yarn_face3"].ToString().Trim();
                        ddlfront2.SelectedValue = dtrow["yarn_face4"].ToString().Trim();
                        if (!string.IsNullOrEmpty(dtrow["yarn_face5"].ToString()))
                        {
                            ddlfront5.SelectedValue = dtrow["yarn_face5"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(dtrow["yarn_face6"].ToString()))
                        {
                            ddlfront6.SelectedValue = dtrow["yarn_face6"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(dtrow["dye_finishing_formula_id"].ToString()))
                        {
                            ddlFinishing.SelectedValue = dtrow["dye_finishing_formula_id"].ToString().Trim();
                        }

                        string gangtagrequired = dtrow["require_tag"].ToString();

                        if (gangtagrequired == "N")
                        {
                            chkHangTagRequire.Checked = false;
                        }
                        else
                        {
                            chkHangTagRequire.Checked = true;
                        }
                        TXTYARDAGE.Text = dtrow["yardage"].ToString();
                        string snagging = dtrow["snagging"].ToString();
                        if (snagging == "N")
                        {
                            chksnagging.Checked = false;
                        }
                        else
                        {
                            chksnagging.Checked = true;
                        }
                        string pilling = dtrow["piling"].ToString();
                        if (pilling == "N")
                        {
                            chkpilling.Checked = false;
                        }
                        else
                        {
                            chkpilling.Checked = true;
                        }

                        string standard_aatcc = dtrow["standard_aatcc"].ToString();
                        if (standard_aatcc == "N")
                        {
                            CHKAATCC.Checked = false;
                        }
                        else
                        {
                            CHKAATCC.Checked = true;
                        }
                        string standard_astm = dtrow["standard_astm"].ToString();
                        if (standard_astm == "N")
                        {
                            CHKASTM.Checked = false;
                        }
                        else
                        {
                            CHKASTM.Checked = true;
                        }

                        string standard_iso = dtrow["standard_iso"].ToString();
                        if (standard_iso == "N")
                        {
                            CHKISO.Checked = false;
                        }
                        else
                        {
                            CHKISO.Checked = true;
                        }
                        string standard_jis = dtrow["standard_jis"].ToString();
                        if (standard_jis == "N")
                        {
                            CHKJIS.Checked = false;
                        }
                        else
                        {
                            CHKJIS.Checked = true;
                        }
                        string standard_ms = dtrow["standard_ms"].ToString();
                        if (standard_ms == "N" || standard_ms == "0" || standard_ms == "F")
                        {
                            chkMS.Checked = false;
                        }
                        else
                        {
                            chkMS.Checked = true;
                        }
                        string standard_hm = dtrow["standard_hm"].ToString();
                        if (standard_hm == "N" || standard_hm == "0" || standard_ms == "F")
                        {
                            chkHM.Checked = false;
                        }
                        else
                        {
                            chkHM.Checked = true;
                        }
                        string standard_vss = dtrow["standard_vss"].ToString();
                        if (standard_vss == "N" || standard_vss == "0" || standard_ms == "F")
                        {
                            chkVSS.Checked = false;
                        }
                        else
                        {
                            chkVSS.Checked = true;
                        }

                        txtElongation_l.Text = dtrow["elongation_l"].ToString();
                        txtmodulus_l.Text = dtrow["modulus_l"].ToString();
                        txtstretch_l.Text = dtrow["stretch_l"].ToString();
                        txtshrinkage_l.Text = dtrow["shrinkage_l"].ToString();
                        txtElongation_w.Text = dtrow["elongation_w"].ToString();
                        txtmodulus_w.Text = dtrow["modulus_w"].ToString();
                        txtstretch_w.Text = dtrow["stretch_w"].ToString();
                        txtshrinkage_w.Text = dtrow["shrinkage_w"].ToString();
                        txtelongation_tolerance.Text = dtrow["elongation_tolerance"].ToString();
                        txtmodulus_tolerance.Text = dtrow["modulus_tolerance"].ToString();
                        txtshrinkage_tolerance.Text = dtrow["shrinkage_tolerance"].ToString();
                        txtstretch_tolerance.Text = dtrow["stretch_tolerance"].ToString();
                        ddlReasonForDevl.ClearSelection();
                        if (!string.IsNullOrEmpty(dtrow["development_type_id"].ToString()))
                        {

                            string development_type_id = dtrow["development_type_id"].ToString();
                            ListItem selectedListItem = ddlReasonForDevl.Items.FindByValue(development_type_id);
                            // ddlReasonForDevl.Items.FindByValue(development_type_id).Selected = true;
                            if (selectedListItem != null)
                            {
                                selectedListItem.Selected = true;
                            };

                        }
                        else
                        {
                            ddlReasonForDevl.Items.FindByValue("").Selected = true;
                        }

                        //ddlReasonForDevl.SelectedValue = dtrow["development_type_id"].ToString().Trim();
                        //ddlPurposeofGarment.ClearSelection();
                        // ddlPurposeofGarment.SelectedValue = dtrow["product_application_id"].ToString();
                        dtTo.Value = dtrow["expected_date"].ToString();
                        txtSampleRequire.Text = dtrow["hanger"].ToString();
                        txtStandard_Customer.Text = dtrow["standard_customer"].ToString();
                        //txtComment.Text = 
                        string yarnavailable = dtrow["yarn_available"].ToString();
                        if (yarnavailable == "N")
                        {
                            chkYarnAvailable.Checked = false;
                        }
                        else
                        {
                            chkYarnAvailable.Checked = true;
                        }

                        txtweightTolerance.Text = dtrow["weight_sqm_tolerance"].ToString();
                        txtwidth_Tolerance.Text = dtrow["width_tolerance"].ToString();
                        //lblFileName.Text = dtrow["file_location"].ToString();
                        txtAppRej.Text = dtrow["pdr_app_rej_status"].ToString().Trim();
                        //if (dtrow["pdr_app_rej_status"].ToString() == "APP")
                        //{
                        //    txtAppRej.Text = "APPROVED";// row["pdr_app_rej_status"].ToString();
                        //}
                        //else
                        //{
                        //    txtAppRej.Text = "REJECTED";//
                        //}
                        txtBy.Text = dtrow["pdr_app_rej_by"].ToString();
                        if (!string.IsNullOrEmpty(dtrow["pdr_app_rej_date"].ToString()))
                        {
                            DateTime dt7 = Convert.ToDateTime(dtrow["pdr_app_rej_date"].ToString());
                            txtAppRejDate.Text = String.Format("{0:dd-MM-yyyy}", dt7);//= row["qa_report_date"].ToString();
                                                                                      //dtTo.Value = row["expected_date"].ToString();
                        }

                        //txtAppRejDate.Text= row["pdr_app_rej_date"].ToString();
                        txtAppRejComment.Text = dtrow["pdr_app_rej_comment"].ToString();

                        if (!string.IsNullOrEmpty(dtrow["kg_per_finished_roll"].ToString()))
                        {
                            txtkgperfinishedroll.Text = (dtrow["kg_per_finished_roll"].ToString());
                        }
                        if (!string.IsNullOrEmpty(dtrow["no_of_finished_rolls"].ToString()))
                        {
                            txtTotalFinishedRolls.Text = (dtrow["no_of_finished_rolls"].ToString());
                        }
                        if (!string.IsNullOrEmpty(dtrow["yarn_shortage1"].ToString()))
                        {
                            string yarn_shortage1 = dtrow["yarn_shortage1"].ToString();
                            if (yarn_shortage1 == "N" || yarn_shortage1 == "")
                            {
                                chkshortage1.Checked = false;
                            }
                            else
                            {
                                chkshortage1.Checked = true;
                            }
                        }

                        if (!string.IsNullOrEmpty(dtrow["yarn_shortage2"].ToString()))
                        {
                            string yarn_shortage2 = dtrow["yarn_shortage2"].ToString();
                            if (yarn_shortage2 == "N" || yarn_shortage2 == "")
                            {
                                chkshortage2.Checked = false;
                            }
                            else
                            {
                                chkshortage2.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(dtrow["yarn_shortage3"].ToString()))
                        {
                            string yarn_shortage3 = dtrow["yarn_shortage3"].ToString();
                            if (yarn_shortage3 == "N" || yarn_shortage3 == "")
                            {
                                chkshortage3.Checked = false;
                            }
                            else
                            {
                                chkshortage3.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(dtrow["yarn_shortage4"].ToString()))
                        {
                            string yarn_shortage4 = dtrow["yarn_shortage4"].ToString();
                            if (yarn_shortage4 == "N" || yarn_shortage4 == "")
                            {
                                chkshortage4.Checked = false;
                            }
                            else
                            {
                                chkshortage4.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(dtrow["yarn_shortage5"].ToString()))
                        {
                            string yarn_shortage5 = dtrow["yarn_shortage5"].ToString();
                            if (yarn_shortage5 == "N" || yarn_shortage5 == "")
                            {
                                chkshortage5.Checked = false;
                            }
                            else
                            {
                                chkshortage5.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(dtrow["yarn_shortage6"].ToString()))
                        {
                            string yarn_shortage6 = dtrow["yarn_shortage6"].ToString();
                            if (yarn_shortage6 == "N" || yarn_shortage6 == "")
                            {
                                chkshortage6.Checked = false;
                            }
                            else
                            {
                                chkshortage6.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(dtrow["new_yarn1"].ToString()))
                        {
                            string new_yarn1 = dtrow["new_yarn1"].ToString();
                            if (new_yarn1 == "N" || new_yarn1 == "")
                            {
                                chknewyarn1.Checked = false;
                            }
                            else
                            {
                                chknewyarn1.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(dtrow["new_yarn2"].ToString()))
                        {
                            string new_yarn2 = dtrow["new_yarn2"].ToString();
                            if (new_yarn2 == "N" || new_yarn2 == "")
                            {
                                chknewyarn2.Checked = false;
                            }
                            else
                            {
                                chknewyarn2.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(dtrow["new_yarn3"].ToString()))
                        {
                            string new_yarn3 = dtrow["new_yarn3"].ToString();
                            if (new_yarn3 == "N" || new_yarn3 == "")
                            {
                                chknewyarn3.Checked = false;
                            }
                            else
                            {
                                chknewyarn3.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(dtrow["new_yarn4"].ToString()))
                        {
                            string new_yarn4 = dtrow["new_yarn4"].ToString();
                            if (new_yarn4 == "N" || new_yarn4 == "")
                            {
                                chknewyarn4.Checked = false;
                            }
                            else
                            {
                                chknewyarn4.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(dtrow["new_yarn5"].ToString()))
                        {
                            string new_yarn5 = dtrow["new_yarn5"].ToString();
                            if (new_yarn5 == "N" || new_yarn5 == "")
                            {
                                chknewyarn5.Checked = false;
                            }
                            else
                            {
                                chknewyarn5.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(dtrow["new_yarn6"].ToString()))
                        {
                            string new_yarn6 = dtrow["new_yarn6"].ToString();
                            if (new_yarn6 == "N" || new_yarn6 == "")
                            {
                                chknewyarn6.Checked = false;
                            }
                            else
                            {
                                chknewyarn6.Checked = true;
                            }
                        }
                    }
                    // load the grid
                    DataTable dtdevreq;
                    classDevelop_Request_BLL devrequest = new classDevelop_Request_BLL();
                    dtdevreq = devrequest.LoadDeveRequestApp(hidpdr_new_develop_req_id.Value.ToString().Trim());
                    gvDevelop_Request.DataSource = dtdevreq;
                    gvDevelop_Request.DataBind();
                    ViewState["pdrDevReq"] = dtdevreq;
                    if (dtdevreq.Rows.Count <= 0)
                    {
                        BindHeaderRowInGridview();
                    }
                    //
                }
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message.ToString();
            }
        }

        protected void ImgPdrList_Click(object sender, ImageClickEventArgs e)
        {
            checkValues();
            if (valuechanged == "YES")
            {

                ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "ClientScript", "alert('Modified Data is not updated, Click the Update icon and save the Data')", true);
                return;
            }
            else
            {
                Response.Redirect("~/UI/PDR_List.aspx");
            }
        }

        protected void ImgPrint_Click(object sender, ImageClickEventArgs e)
        {
            setPDRDevelopRequestToSession();
            Session["rptoption"] = "print";
            //Response.Redirect("~/Reports/Print_Develop_Request.aspx");
            Server.Transfer("~/Reports/Print_Develop_Request.aspx");

            //string fileName = @"d:\" + txtPDRNO.Text.Trim() + ".pdf";
            //if (fileName != "")
            //{
            //    bool exists = System.IO.Directory.Exists(Server.MapPath("~/pdrdoc/"));

            //    if (!exists)
            //        System.IO.Directory.CreateDirectory(Server.MapPath("~/pdrdoc/"));
            //    uploadfiles1.SaveAs("//172.16.3.4/pdr_files/pdrdoc/" + fileName);
            //    // lblMsg.Text = "File Uploaded Successfully";
            //}

        }
        private void setPDRDevelopRequestToSession()
        {
            Session["pdrno"] = txtPDRNO.Text.Trim();
            
            Session["pdr_new_develop_req_id"] = hidpdr_new_develop_req_id.Value.ToString().Trim();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {


        }

        [WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string[] GetCustomers1(string prefix)
        {
            DataTable dt = new DataTable();
            // List<Issues> objDept = new List<Issues>();
            List<string> customers = new List<string>();
            classDevelop_Request_BLL getcustomer = new classDevelop_Request_BLL();
            dt = getcustomer.Populate_Issue_dt(prefix);

            if (dt.Rows.Count > 0)
            {

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Issues DataObj = new Issues();

                    //DataObj.poc_issue_master_id = Convert.ToInt32(dt.Rows[i]["poc_issue_master_id"]);
                    //DataObj.issue_name = dt.Rows[i]["issue_name"].ToString();
                    //objDept.Add(DataObj);
                    string chk = dt.Rows[i]["custcd"].ToString();
                    if (chk == "-1")
                    {
                        // customers.Add(string.Format("{0}- ", Convert.ToInt32(dt.Rows[i]["poc_issue_master_id"]), "' '"));
                        customers.Add(string.Format("'--1',{1} ", Convert.ToInt32(dt.Rows[i]["custcd"]), "' '"));
                    }
                    else
                    {
                        customers.Add(string.Format("{0}-{1}", (dt.Rows[i]["custcd"]), dt.Rows[i]["name"].ToString()));
                    }

                }
                return customers.ToArray();

            }

            else
            {
                return null;
            }

        }

        //protected void btnView_Click(object sender, EventArgs e)
        //{
        //    //Response.Redirect("D:\UploadedAttachment\AT\MRD\AT0520130008_15-05-13-03-57-12.pdf");
        //    //string path = "G:/test.txt";

        //    string filepath = lblFileName.Text;

        //    if (filepath != "")
        //    {

        //        ////
        //        //byteImageData = ReadImage(lblFileName.Text.Trim(), new string[] { ".gif", ".jpg", ".bmp" });
        //        //byteArrayToImage(byteImageData);
        //        Session["uploadedfilename"] = lblFileName.Text.Trim();
        //        //// Response.Write("window.open("fileopen.aspx")");
        //        // Response.Write("<script>");
        //        // Response.Write("window.open('" + filepath + ",'_blank', ' fullscreen=no')");
        //        //// //Response.Write("window.open(" + path + ",'_blank')");
        //        // Response.Write("</script>");
        //        //Response.Redirect("fileopen.aspx");
        //        ////
        //        string extension;
        //        string filename = Path.GetFileName(filepath);
        //        extension = Path.GetExtension(filepath);
        //        Page.ClientScript.RegisterStartupScript(
        //      this.GetType(), "OpenWindow", "window.open('" + filepath + "','_newtab');", true);
        //        System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //        byte[] ar = new byte[(int)fs.Length];
        //        fs.Read(ar, 0, (int)fs.Length);
        //        fs.Close();

        //        Response.AddHeader("content-disposition", "attachment;filename=" + filename);
        //        Response.ContentType = "application/octectstream";
        //        Response.BinaryWrite(ar);
        //        Response.End();
        //        System.Diagnostics.Process.Start(filepath);

        //        Session["uploadedfilename"] = lblFileName.Text.Trim();
        //        //Response.Redirect("fileopen.aspx");
        //        // Response.Write("<script>");
        //        // Response.Write("window.open('" + lblFileName.Text.Trim() + "','_blank', ' fullscreen=yes')");
        //        //// Response.Write("window.open(" + filepath + ",'_blank')");
        //        // Response.Write("</script>");


        //        // string url = lblFileName.Text;
        //        // ClientScript.RegisterStartupScript(this.GetType(), "OpenWin", "<script>openNewWin('" + url + "')</script>");
        //        //byteImageData = ReadImage(filename, new string[] { ".gif", ".jpg", ".bmp" });
        //        //byteArrayToImage(byteImageData);

        //    }
        //    else
        //    {
        //        Response.Write("<script>");
        //        Response.Write("window.open('" + lblFileName.Text.Trim() + "','_blank', ' fullscreen=yes')");
        //        //Response.Write("window.open(" + path + ",'_blank')");
        //        Response.Write("</script>");
        //        //string uploadedfilepath = lblFileName.Text.ToString().Trim();
        //        //WebClient client = new WebClient();
        //        //Byte[] buffer = client.OpenReadCompleted(uploadedfilepath);
        //        //if (buffer != null)
        //        //{
        //        //    Response.ContentType = "application/txt";
        //        //    Response.AddHeader("content-length", buffer.Length.ToString());
        //        //    Response.BinaryWrite(buffer);
        //        //}
        //        //  string extension;
        //        //  string filename = Path.GetFileName(uploadfiles1.FileName);
        //        //  extension = Path.GetExtension(filepath);
        //        //  string filepath1;
        //        //  filepath1 = Path.GetPathRoot(uploadfiles1.FileName);
        //        //  Page.ClientScript.RegisterStartupScript(
        //        //this.GetType(), "OpenWindow", "window.open('" + filename + "','_newtab');", true);
        //        //  System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //        //  byte[] ar = new byte[(int)fs.Length];
        //        //  fs.Read(ar, 0, (int)fs.Length);
        //        //  fs.Close();

        //        //  Response.AddHeader("content-disposition", "attachment;filename=" + filename);
        //        //  Response.ContentType = "application/octectstream";
        //        //  Response.BinaryWrite(ar);
        //        //  Response.End();
        //        //  System.Diagnostics.Process.Start(filepath1);
        //    }
        //}


        //private void byteArrayToImage(byte[] byteArrayIn)

        //{

        //    System.Drawing.Image newImage;



        //    string strFileName = lblFileName.Text.Trim();

        //    if (byteArrayIn != null)

        //    {

        //        using (MemoryStream stream = new MemoryStream(byteArrayIn))

        //        {

        //            newImage = System.Drawing.Image.FromStream(stream);



        //            newImage.Save(strFileName);



        //add            img.Attributes.Add("src", strFileName);

        //        }



        //        //  lblMessage.Text = “The image conversion was successful.”;

        //    }

        //    else

        //    {

        //        // Response.Write(“No image data found!”);

        //    }

        //}



        //private static byte[] ReadImage(string p_postedImageFileName, string[] p_fileType)

        //{

        //    bool isValidFileType = false;

        //    try

        //    {

        //        FileInfo file = new FileInfo(p_postedImageFileName);



        //        foreach (string strExtensionType in p_fileType)

        //        {

        //            if (strExtensionType == file.Extension)

        //            {

        //                isValidFileType = true;

        //                break;

        //            }

        //        }

        //        if (isValidFileType)

        //        {

        //            FileStream fs = new FileStream(p_postedImageFileName, FileMode.Open, FileAccess.Read);



        //            BinaryReader br = new BinaryReader(fs);



        //            byte[] image = br.ReadBytes((int)fs.Length);



        //            br.Close();



        //            fs.Close();



        //            return image;

        //        }

        //        return null;

        //    }

        //    catch (Exception ex)

        //    {

        //        throw ex;

        //    }

        //}


        //private static byte[] byteImageData;

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            //populate_ItemApplication();
            //populate_ItemSubApplication();
            //populate_SplFunc();
            //populate_Zone();
            //populate_Customer();
            //populate_Country();
            AddNewRow();
        }

        private void AddNewRow()
        {
            int rowIndex = 0;
            // DataRow GVr = null;

            if (ViewState["pdrDevReq"] != null)
            {
                DataTable dtCurrentTable = (DataTable)ViewState["pdrDevReq"];
                // Response.Write(dtCurrentTable.Rows.Count);
                DataRow drCurrentRow = dtCurrentTable.NewRow();// null;
                Int32 counti = 0;
                Int32 editrowcount = 0;
                DataColumnCollection columns = dtCurrentTable.Columns;

                foreach (DataRow editrow in dtCurrentTable.Rows)
                {
                    DropDownList ddlApp = gvDevelop_Request.Rows[editrowcount].FindControl("ddlAppl") as DropDownList;

                    if (dtCurrentTable.Columns.Contains("appl_name"))
                    {
                        editrow["appl_name"] = ddlApp.SelectedItem.Text.ToString().Trim();// appl_name;
                    }

                    for (int i = 0; i < ddlApp.Items.Count; i++)
                    {
                        if (ddlApp.Items[i].Text.Trim() == ddlApp.SelectedItem.Text.ToString().Trim())
                        {
                            if (ddlApp.Items[i].Value != "")
                            {
                                editrow["appl_id"] = (ddlApp.Items[i].Value);
                            }
                        }
                    }
                    //editrow["appl_id"]=Convert.ToInt64(ddlApp.SelectedItem.Value.ToString().Trim());
                    DropDownList ddlSubAppl = gvDevelop_Request.Rows[editrowcount].FindControl("ddlSubAppl") as DropDownList;

                    if (dtCurrentTable.Columns.Contains("sub_appl_name"))
                    {
                        editrow["sub_appl_name"] = ddlSubAppl.SelectedItem.Text.ToString().Trim();// sub_appl_name;
                    }
                    for (int i = 0; i < ddlSubAppl.Items.Count; i++)
                    {
                        if (ddlSubAppl.Items[i].Text.Trim() == ddlSubAppl.SelectedItem.Text.ToString().Trim())
                        {
                            if (ddlSubAppl.Items[i].Value != "")
                            {
                                editrow["sub_appl_id"] = (ddlSubAppl.Items[i].Value);
                            }
                        }
                    }
                    DropDownList ddlsplfunc = gvDevelop_Request.Rows[editrowcount].FindControl("ddlsplfunc") as DropDownList;
                    if (dtCurrentTable.Columns.Contains("spl_func"))
                    {
                        editrow["spl_func"] = ddlsplfunc.SelectedItem.Text.ToString().Trim();// sub_appl_name;
                    }
                    for (int i = 0; i < ddlsplfunc.Items.Count; i++)
                    {
                        if (ddlsplfunc.Items[i].Text.Trim() == ddlsplfunc.SelectedItem.Text.ToString().Trim())
                        {
                            if (ddlsplfunc.Items[i].Value != "")
                            {
                                editrow["spl_func_id"] = Convert.ToInt64(ddlsplfunc.Items[i].Value);
                            }
                        }
                    }
                    DropDownList ddlctry = gvDevelop_Request.Rows[editrowcount].FindControl("ddlctry") as DropDownList;
                    if (dtCurrentTable.Columns.Contains("ctry_name"))
                    {
                        editrow["ctry_name"] = ddlctry.SelectedItem.Text.ToString().Trim();// sub_appl_name;
                    }
                    for (int i = 0; i < ddlctry.Items.Count; i++)
                    {
                        if (ddlctry.Items[i].Text.Trim() == ddlctry.SelectedItem.Text.ToString().Trim())
                        {
                            editrow["ctry"] = (ddlctry.Items[i].Value);
                        }
                    }
                    DropDownList ddlzone = gvDevelop_Request.Rows[editrowcount].FindControl("ddlzone") as DropDownList;
                    if (dtCurrentTable.Columns.Contains("market_zone"))
                    {
                        editrow["market_zone"] = ddlzone.SelectedItem.Text.ToString().Trim();// sub_appl_name;
                    }
                    for (int i = 0; i < ddlzone.Items.Count; i++)
                    {
                        if (ddlzone.Items[i].Text.Trim() == ddlzone.SelectedItem.Text.ToString().Trim())
                        {
                            if (ddlzone.Items[i].Value != "")
                            {
                                editrow["market_zone_id"] = (ddlzone.Items[i].Value);
                            }
                        }
                    }
                    DropDownList ddlcustomer = gvDevelop_Request.Rows[editrowcount].FindControl("ddlcustomer") as DropDownList;
                    if (dtCurrentTable.Columns.Contains("market_customer"))
                    {
                        editrow["market_customer"] = ddlcustomer.SelectedItem.Text.ToString().Trim();// sub_appl_name;
                    }
                    for (int i = 0; i < ddlcustomer.Items.Count; i++)
                    {
                        if (ddlcustomer.Items[i].Text.Trim() == ddlcustomer.SelectedItem.Text.ToString().Trim())
                        {
                            if (ddlcustomer.Items[i].Value != "")
                            {
                                editrow["market_customer_id"] = (ddlcustomer.Items[i].Value);
                            }
                        }
                    }
                    editrowcount = editrowcount + 1;
                    editrow.EndEdit();
                    dtCurrentTable.AcceptChanges();
                    ddlApp.SelectedItem.Value = ddlApp.SelectedItem.Value.ToString().Trim();
                }
                if (dtCurrentTable.Rows.Count > 0)
                {
                    foreach (GridViewRow row in gvDevelop_Request.Rows)
                    {
                        counti = counti + 1;
                        if (counti == 1)
                        {
                            for (int i = 1; i <= 1; i++)
                            {
                                drCurrentRow = dtCurrentTable.NewRow();
                                drCurrentRow[0] = "0";
                                //drCurrentRow[4] = "";

                                drCurrentRow["pdr_new_develop_req_id"] = hidpdr_new_develop_req_id.Value;
                                rowIndex++;
                                dtCurrentTable.Rows.Add(drCurrentRow);
                            }
                            ViewState["pdrDevReq"] = dtCurrentTable;////
                            //DataSet ds;
                            gvDevelop_Request.DataSource = dtCurrentTable;
                            gvDevelop_Request.DataBind();
                            ViewState["pdrDevReq"] = dtCurrentTable;
                            gvDevelop_Request.Visible = true;

                        }
                        //else
                        //{

                        //    BindHeaderRowInGridview();
                        //}
                        //SetPreviousData();
                    }
                } // row count
                else // when there is no record to add newly
                {
                    //Response.Write("ViewState is null");
                    BindHeaderRowInGridview();
                }
            }
            else // when there is no record to add newly
            {
                //Response.Write("ViewState is null");
                BindHeaderRowInGridview();
            }
            DataTable dtCurrentTable1 = (DataTable)ViewState["pdrDevReq"];
            Int32 neweditcount = 0;
            if (dtCurrentTable1.Rows.Count > 1)
            {
                foreach (GridViewRow neweditrow in gvDevelop_Request.Rows)
                {
                    DropDownList ddlApp1 = neweditrow.FindControl("ddlAppl") as DropDownList;
                    //appl_id = Request[ddlApp.UniqueID] as string;
                    if (dtCurrentTable1.Columns.Contains("appl_name"))
                    {
                        ddlApp1.SelectedItem.Text = dtCurrentTable1.Rows[neweditcount]["appl_name"].ToString().Trim();
                    }
                    DropDownList ddlsubApp1 = neweditrow.FindControl("ddlSubAppl") as DropDownList;
                    //sub_appl_id = Request[ddlsubApp.UniqueID] as string;
                    if (dtCurrentTable1.Columns.Contains("sub_appl_name"))
                    {
                        ddlsubApp1.SelectedItem.Text = dtCurrentTable1.Rows[neweditcount]["sub_appl_name"].ToString().Trim();
                    }
                    DropDownList ddlsplfunc1 = neweditrow.FindControl("ddlsplfunc") as DropDownList;
                    // spl_func_id = Request[ddlsplfunc.UniqueID] as string;
                    if (dtCurrentTable1.Columns.Contains("spl_func"))
                    {
                        ddlsplfunc1.SelectedItem.Text = dtCurrentTable1.Rows[neweditcount]["spl_func"].ToString().Trim();
                    }
                    DropDownList ddlcountry1 = neweditrow.FindControl("ddlctry") as DropDownList;
                    //ctry = Request[ddlcountry.UniqueID] as string;
                    if (dtCurrentTable1.Columns.Contains("ctry_name"))
                    {
                        ddlcountry1.SelectedItem.Text = dtCurrentTable1.Rows[neweditcount]["ctry_name"].ToString().Trim();
                    }
                    DropDownList ddlZone1 = neweditrow.FindControl("ddlzone") as DropDownList;
                    //market_zone_id = Request[ddlZone.UniqueID] as string;
                    if (dtCurrentTable1.Columns.Contains("market_zone"))
                    {
                        ddlZone1.SelectedItem.Text = dtCurrentTable1.Rows[neweditcount]["market_zone"].ToString().Trim();
                    }
                    DropDownList ddlcustomer1 = neweditrow.FindControl("ddlcustomer") as DropDownList;
                    // market_customer_id = Request[ddlcustomer.UniqueID] as string;
                    if (dtCurrentTable1.Columns.Contains("market_customer"))
                    {
                        ddlcustomer1.SelectedItem.Text = dtCurrentTable1.Rows[neweditcount]["market_customer"].ToString().Trim();
                    }
                    neweditcount = neweditcount + 1;
                }
            }
        }


        protected void ImgAddCustomer_Click(object sender, EventArgs e)
        {
            // string message = "Message from server side";
            // Response.Redirect("AddEditCustomer.aspx");
            // ClientScript.RegisterStartupScript(this.GetType(), "Popup", "ShowPopup('" + message + "');", true);
        }

        protected void ImgEMail_Click(object sender, ImageClickEventArgs e)
        {
            DataTable dt;
            string pdr_no = txtPDRNO.Text;
            classDevelop_Request_BLL devReqBLL = new classDevelop_Request_BLL();
            string mailResult;
            classDevelop_Request_BLL developRequest = new classDevelop_Request_BLL();
            dt = developRequest.LoadPDRList(pdr_no);
            //saveReportasPDF();
            string sessionid = Session["usersessionid"].ToString();
            mailResult = devReqBLL.sendApprovalMail(dt, sessionid);

            //string fileName = @"d:\" + txtPDRNO.Text.Trim() + ".pdf";
            //if (fileName != "")
            //{
            //    bool exists = System.IO.Directory.Exists(Server.MapPath("~/pdrdoc/"));

            //    if (!exists)
            //        System.IO.Directory.CreateDirectory(Server.MapPath("~/pdrdoc/"));
            //    uploadfiles1.SaveAs("//172.16.3.4/pdr_files/pdrdoc/" + fileName);
            //    // lblMsg.Text = "File Uploaded Successfully";
            //}
        }
        protected void saveReportasPDF()
        {
            setPDRDevelopRequestToSession();
            Session["rptoption"] = "email";
            Server.Execute("~/Reports/Print_Develop_Request.aspx");
        }
        protected void ddlCode1Selected(object sender, EventArgs e)
        {
            String item_code = ddlCode1.SelectedValue;
            Decimal yarnNetBalKg;
            classDevelop_Request_BLL classDevReq = new classDevelop_Request_BLL();
            yarnNetBalKg = classDevReq.getYarnNetBal(item_code);
            txtAvail1.Text = Convert.ToString(yarnNetBalKg);
            if ( txtComposition1_name.Text == "")
            {
                txtComposition1_name.Text= ddlCode1.SelectedItem.Text.Trim();
            }

        }

        protected void ddlCode2Selected(object sender, EventArgs e)
        {
            String item_code = ddlCode2.SelectedValue;
            Decimal yarnNetBalKg;
            classDevelop_Request_BLL classDevReq = new classDevelop_Request_BLL();
            yarnNetBalKg = classDevReq.getYarnNetBal(item_code);
            txtAvail2.Text = Convert.ToString(yarnNetBalKg);
            if (txtComposition2_name.Text == "")
            {
                txtComposition2_name.Text = ddlCode2.SelectedItem.Text.Trim();
            }
        }

        protected void ddlCode3Selected(object sender, EventArgs e)
        {
            String item_code = ddlCode3.SelectedValue;
            Decimal yarnNetBalKg;
            classDevelop_Request_BLL classDevReq = new classDevelop_Request_BLL();
            yarnNetBalKg = classDevReq.getYarnNetBal(item_code);
            txtAvail3.Text = Convert.ToString(yarnNetBalKg);
            if (txtComposition3_name.Text == "")
            {
                txtComposition3_name.Text = ddlCode3.SelectedItem.Text.Trim();
            }
        }
        protected void ddlCode4Selected(object sender, EventArgs e)
        {
            String item_code = ddlCode4.SelectedValue;
            Decimal yarnNetBalKg;
            classDevelop_Request_BLL classDevReq = new classDevelop_Request_BLL();
            yarnNetBalKg = classDevReq.getYarnNetBal(item_code);
            txtAvail4.Text = Convert.ToString(yarnNetBalKg);
            if (txtComposition4_name.Text == "")
            {
                txtComposition4_name.Text =  ddlCode4.SelectedItem.Text.Trim();
            }
        }
        protected void ddlCode5Selected(object sender, EventArgs e)
        {
            String item_code = ddlCode5.SelectedValue;
            Decimal yarnNetBalKg;
            classDevelop_Request_BLL classDevReq = new classDevelop_Request_BLL();
            yarnNetBalKg = classDevReq.getYarnNetBal(item_code);
            txtAvail5.Text = Convert.ToString(yarnNetBalKg);
            if (txtComposition5_name.Text == "")
            {
                txtComposition5_name.Text =  ddlCode5.SelectedItem.Text.Trim();
            }
        }
        protected void ddlCode6Selected(object sender, EventArgs e)
        {
            String item_code = ddlCode6.SelectedValue;
            Decimal yarnNetBalKg;
            classDevelop_Request_BLL classDevReq = new classDevelop_Request_BLL();
            yarnNetBalKg = classDevReq.getYarnNetBal(item_code);
            txtAvail6.Text = Convert.ToString(yarnNetBalKg);
            if (txtComposition6_name.Text.Trim() == "")
            {
                txtComposition6_name.Text = ddlCode6.SelectedItem.Text.Trim();
            }
        }

        protected void ImgCancel_Click(object sender, ImageClickEventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];
            if (confirmValue == "Yes")
            {
                // this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('You clicked YES!')", true);
                classDevelop_Request_BLL objCancelPDRNO = new classDevelop_Request_BLL();
                string pdr_new_develop_req_id = hidpdr_new_develop_req_id.Value.Trim();
                string userid = Session["usersessionid"].ToString().Trim();
               DataTable dt_Cancel= objCancelPDRNO.CancelPDRNO(pdr_new_develop_req_id, userid);
                lblCancel.Text = "CANCELLED";
            }
            else
            {
               // this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('You clicked NO!')", true);
            }
        }

        protected void ImgCopy_Click(object sender, ImageClickEventArgs e)
        {
            string copyValue = Request.Form["copy_value"];
            if (copyValue == "Yes")
            {
                CopyPDRNO();
            }
            else
            {

            }
        }

        private void CopyPDRNO()
        {
            string p_pdr_new_develop_req_id = "";
            string p_pdr_no = "";
            string p_pdr_date = "";
            string p_design_no = "";
            string p_gauge = "";
            string p_custcd = "";
            string p_requested_by = "";
            string p_expected_price = "";
            string p_uom = "";
            string p_expected_date = "";
            string p_product_application_id = "";
            string p_development_type_id = "";
            string p_development_type_remark = "";
            string p_itcatid = "";
            string p_itsubcatid = "";
            string p_itgroupid = "";
            string p_itsubid = "";
            string p_ittypeid = "";
            string p_itsubid2 = "";
            string p_standard_iso = "";
            string p_standard_aatcc = "";
            string p_standard_astm = "";
            string p_standard_jis = "";
            string p_standard_customer = "";
            string p_follow_customer_spec = "";
            string p_yarn_face1 = "";
            string p_yarn_face2 = "";
            string p_yarn_face3 = "";
            string p_yarn_face4 = "";
            string p_yarn_face5 = "";
            string p_yarn_face6 = "";
            string p_composition1_percent = "";
            string p_composition2_percent = "";
            string p_composition3_percent = "";
            string p_composition4_percent = "";
            string p_composition5_percent = "";
            string p_composition6_percent = "";
            string p_composition1_name = "";
            string p_composition2_name = "";
            string p_composition3_name = "";
            string p_composition4_name = "";
            string p_composition5_name = "";
            string p_composition6_name = "";
            string p_weight_sqm = "";
            string p_weight_sqm_tolerance = "";
            string p_weight_sqm_remark = "";
            string p_width = "";
            string p_width_tolerance = "";
            string p_elongation = "";
            string p_elongation_l = "";
            string p_elongation_w = "";
            string p_elongation_tolerance = "";
            string p_modulus = "";
            string p_modulus_l = "";
            string p_modulus_w = "";
            string p_modulus_tolerance = "";
            string p_stretch_l = "";
            string p_stretch_w = "";
            string p_stretch_tolerance = "";
            string p_shrinkage_l = "";
            string p_shrinkage_w = "";
            string p_shrinkage_tolerance = "";
            string p_piling = "";
            string p_pilling_remark = "";
            string p_snagging = "";
            string p_snagging_remark = "";
            string p_color_name = "";
            string p_dye_finishing_formula_id = "";
            string p_dye_remark = "";
            string p_hanger = "";
            string p_yardage = "";
            string p_require_tag = "";
            string p_machine_dense = "";
            string p_yarn_available = "";
            string p_yarn_available_date = "";
            string p_knitting_appointment = "";
            string p_offer_price = "";
            string p_is_consideration = "";
            string p_expected_finished_date = "";
            string p_spec_master_date = "";
            string p_qa_report_date = "";
            string p_remark = "";
            string p_user_session_id = Session["usersessionid"].ToString();
            valuechanged = "NO";
            string fileName = "";
            string p_standard_ms = "";
            string p_standard_vss = "";
            string p_standard_hm = "";
            string p_endbuyercd = "";
            string p_priority_id = "";
            string p_priority_comment = "";
            string p_price_curr = "";
            string p_itcd1 = "";
            string p_itcd2 = "";
            string p_itcd3 = "";
            string p_itcd4 = "";
            string p_itcd5 = "";
            string p_itcd6 = "";
            Decimal p_avail1 = 0;
            Decimal p_avail2 = 0;
            Decimal p_avail3 = 0;
            Decimal p_avail4 = 0;
            Decimal p_avail5 = 0;
            Decimal p_avail6 = 0;
            Decimal p_kg_per_finished_roll = 0;
            Decimal p_no_of_finished_rolls = 0;
            string p_yarn_shortage1 = "N";
            string p_yarn_shortage2 = "N";
            string p_yarn_shortage3 = "N";
            string p_yarn_shortage4 = "N";
            string p_yarn_shortage5 = "N";
            string p_yarn_shortage6 = "N";
            string p_new_yarn1 = "N";
            string p_new_yarn2 = "N";
            string p_new_yarn3 = "N";
            string p_new_yarn4 = "N";
            string p_new_yarn5 = "N";
            string p_new_yarn6 = "N";
            string p_test_method_id = "";
            string p_shoe_size_id = "";
            string p_shoe_style = "";
            string p_shoe_gender_id = "";
            string p_pallet_pattern_no = "";
            string p_product_pattern_no = "";

            string p_shoe_width = "";
            string p_shoe_length = "";
            string p_shoe_require_laser_cut = "";
            string p_laser_cut_with_pattern = "";
            string p_shoe_pairs_required = "";
            string p_repeat_per_roll = "";
            try
            {
                p_pdr_new_develop_req_id = hidpdr_new_develop_req_id.Value;
                p_pdr_no = "NEW";// txtPDRNO.Text;
                p_pdr_date = PDRDate.Value;
                string day = p_pdr_date.Substring(0, 2);
                string mon = p_pdr_date.Substring(3, 2);
                string yr = p_pdr_date.Substring(6, 4);
                p_pdr_date = yr + "-" + mon + "-" + day;// + " 00:00:00";
                p_design_no = txtDesignno.Text;
                p_custcd = hidSearchMainValue.Value;// hidSearchMainValue.Value.Trim();
                p_gauge = TXTGUAGE.Text;
                //p_custcd = "";
                p_requested_by = ddlPDRRequestor.SelectedValue;
                p_expected_price = txtCustPriceExpectation.Text;
                p_uom = ddluom.SelectedValue;
                p_expected_date = dtTo.Value;
                if (p_expected_date.ToString().Trim() != "")
                {
                    string day1 = p_expected_date.Substring(0, 2);
                    string mon1 = p_expected_date.Substring(3, 2);
                    string yr1 = p_expected_date.Substring(6, 4);
                    p_expected_date = yr1 + "-" + mon1 + "-" + day1;
                }
                p_product_application_id = null;// ddlPurposeofGarment.SelectedValue;
                p_development_type_id = ddlReasonForDevl.SelectedValue;
                p_development_type_remark = txtDevelopmentTypeRemark.Text;
                p_itcatid = ddlCategory.SelectedValue;
                p_itsubcatid = ddlSubCat.SelectedValue;
                p_itgroupid = ddlgroup.SelectedValue;
                p_itsubid = ddlSubGroup.SelectedValue;
                p_ittypeid = ddlType.SelectedValue;
                p_itsubid2 = ddlSubType.SelectedValue;
                if (CHKISO.Checked)
                {
                    p_standard_iso = "Y";
                }
                else
                {
                    p_standard_iso = "N";
                }
                if (CHKAATCC.Checked)
                {
                    p_standard_aatcc = "Y";
                }
                else
                {
                    p_standard_aatcc = "N";
                }
                if (CHKASTM.Checked)
                {
                    p_standard_astm = "Y";
                }
                else
                {
                    p_standard_astm = "N";
                }
                if (CHKJIS.Checked)
                {
                    p_standard_jis = "Y";
                }
                else
                {
                    p_standard_jis = "N";
                }
                if (chkMS.Checked)
                {
                    p_standard_ms = "Y";
                }
                else
                {
                    p_standard_ms = "N";
                }
                if (chkVSS.Checked)
                {
                    p_standard_vss = "Y";
                }
                else
                {
                    p_standard_vss = "N";
                }
                if (chkHM.Checked)
                {
                    p_standard_hm = "Y";
                }
                else
                {
                    p_standard_hm = "N";
                }

                p_standard_customer = txtStandard_Customer.Text;

                if (CHKFollow.Checked)
                {
                    p_follow_customer_spec = "Y";
                }
                else
                {
                    p_follow_customer_spec = "N";
                }
                p_yarn_face1 = ddlFront.SelectedValue;
                p_yarn_face2 = ddlBack.SelectedValue;
                p_yarn_face3 = ddlfront1.SelectedValue;
                p_yarn_face4 = ddlfront2.SelectedValue;
                p_yarn_face5 = ddlfront5.SelectedValue;
                p_yarn_face6 = ddlfront6.SelectedValue;
                p_composition1_percent = txtPercent1.Text;
                p_composition2_percent = txtPercent2.Text;
                p_composition3_percent = txtPercent3.Text;
                p_composition4_percent = txtPercent4.Text;
                p_composition5_percent = txtPercent5.Text;
                p_composition6_percent = txtPercent6.Text;
                p_composition1_name = txtComposition1_name.Text;
                p_composition2_name = txtComposition2_name.Text;
                p_composition3_name = txtComposition3_name.Text;
                p_composition4_name = txtComposition4_name.Text;
                p_composition5_name = txtComposition5_name.Text;
                p_composition6_name = txtComposition6_name.Text;
                p_weight_sqm = txtweight.Text;
                p_weight_sqm_tolerance = txtweightTolerance.Text;
                p_weight_sqm_remark = "";
                p_width = TXTWIDTH.Text;
                p_width_tolerance = txtwidth_Tolerance.Text;
                p_elongation = TXTElongation.Text;
                p_elongation_l = txtElongation_l.Text;
                p_elongation_w = txtElongation_w.Text;
                p_elongation_tolerance = txtelongation_tolerance.Text;
                p_modulus = TXTMODULUS.Text;
                p_modulus_l = txtmodulus_l.Text;
                p_modulus_w = txtmodulus_w.Text;
                p_modulus_tolerance = txtmodulus_tolerance.Text;
                p_stretch_l = txtstretch_l.Text;
                p_stretch_w = txtstretch_w.Text;
                p_stretch_tolerance = txtstretch_tolerance.Text;
                p_shrinkage_l = txtshrinkage_l.Text;
                p_shrinkage_w = txtshrinkage_w.Text;
                p_shrinkage_tolerance = txtshrinkage_tolerance.Text;
                if (chkpilling.Checked)
                {
                    p_piling = "Y";// chkpilling.Checked.ToString();
                }
                else
                {
                    p_piling = "N";
                }
                p_pilling_remark = txtpilingremark.Text;
                if (chksnagging.Checked)
                {
                    p_snagging = "Y";// chksnagging.Checked.ToString();
                }
                else
                {
                    p_snagging = "N";
                }
                p_snagging_remark = txtsnaggingremark.Text.ToString();
                p_color_name = txtdyeincolor.Text;
                p_dye_finishing_formula_id = ddlFinishing.SelectedValue;
                p_dye_remark = "";
                if (txtSampleRequire.Text != "")
                {
                    p_hanger = txtSampleRequire.Text.ToString().Trim();
                }
                p_yardage = TXTYARDAGE.Text;
                if (chkHangTagRequire.Checked)
                {
                    p_require_tag = "Y";
                }
                else
                {
                    p_require_tag = "N";
                }
                p_machine_dense = txtMachineTense.Text;
                if (chkYarnAvailable.Checked)
                {
                    p_yarn_available = "Y";// chkYarnAvailable.Checked.ToString();
                }
                else
                {
                    p_yarn_available = "N";
                }
                p_yarn_available_date = txtyarnavailabledate.Value;
                p_knitting_appointment = txtknittingappoinment.Value;
                p_offer_price = "";
                p_is_consideration = "";
                p_expected_finished_date = txtExpectedFinisheddate.Value;
                p_spec_master_date = txtspecmasterdate.Value;
                p_qa_report_date = txtqa_reportdate.Value;
                p_remark = txtremark.Text;
                //if (uploadfiles1.PostedFile.FileName.Length > 0)
                //{
                //    fileName = Path.GetFileName(uploadfiles1.PostedFile.FileName);
                //    string filename1 = Path.GetDirectoryName(uploadfiles1.PostedFile.FileName);
                //    HttpPostedFile file = Request.Files["browserHidden"];
                //    FileInfo fileInfo = new FileInfo(fileName);
                //    string directoryFullPath = fileInfo.DirectoryName;
                //    fileName = "//172.16.3.4/pdr_files/ANALYZE/" + fileName;
                //}
                //else
                //{
                //    fileName = lblFileName.Text.ToString().Trim();
                //}
                p_endbuyercd = ddlEndBuyer.SelectedValue;
                p_priority_id = ddlPriority.SelectedValue;
                p_priority_comment = txtPriorityComment.Text;
                p_price_curr = ddlcombocurrency.SelectedValue.ToString().Trim();
                p_itcd1 = ddlCode1.SelectedValue;
                p_itcd2 = ddlCode2.SelectedValue;
                p_itcd3 = ddlCode3.SelectedValue;
                p_itcd4 = ddlCode4.SelectedValue;
                p_itcd5 = ddlCode5.SelectedValue;
                p_itcd6 = ddlCode6.SelectedValue;
                if (txtAvail1.Text == "")
                {
                    txtAvail1.Text = "0";

                }
                if (txtAvail2.Text == "")
                {
                    txtAvail2.Text = "0";

                }
                if (txtAvail3.Text == "")
                {
                    txtAvail3.Text = "0";

                }
                if (txtAvail4.Text == "")
                {
                    txtAvail4.Text = "0";

                }
                if (txtAvail5.Text == "")
                {
                    txtAvail5.Text = "0";

                }
                if (txtAvail6.Text == "")
                {
                    txtAvail6.Text = "0";

                }

                p_avail1 = Convert.ToDecimal(txtAvail1.Text);
                p_avail2 = Convert.ToDecimal(txtAvail2.Text);
                p_avail3 = Convert.ToDecimal(txtAvail3.Text);
                p_avail4 = Convert.ToDecimal(txtAvail4.Text);
                p_avail5 = Convert.ToDecimal(txtAvail5.Text);
                p_avail6 = Convert.ToDecimal(txtAvail6.Text);

                if (txtkgperfinishedroll.Text == "")
                {
                    txtkgperfinishedroll.Text = "0";
                }

                p_kg_per_finished_roll = Convert.ToDecimal(txtkgperfinishedroll.Text);

                if (txtTotalFinishedRolls.Text == "")
                {
                    txtTotalFinishedRolls.Text = "0";
                }

                p_no_of_finished_rolls = Convert.ToDecimal(txtTotalFinishedRolls.Text);

                if (chkshortage1.Checked)
                {
                    p_yarn_shortage1 = "Y";
                }
                else
                {
                    p_yarn_shortage1 = "N";
                }
                if (chkshortage2.Checked)
                {
                    p_yarn_shortage2 = "Y";
                }
                else
                {
                    p_yarn_shortage2 = "N";
                }
                if (chkshortage3.Checked)
                {
                    p_yarn_shortage3 = "Y";
                }
                else
                {
                    p_yarn_shortage3 = "N";
                }
                if (chkshortage4.Checked)
                {
                    p_yarn_shortage4 = "Y";
                }
                else
                {
                    p_yarn_shortage4 = "N";
                }
                if (chkshortage5.Checked)
                {
                    p_yarn_shortage5 = "Y";
                }
                else
                {
                    p_yarn_shortage5 = "N";
                }
                if (chkshortage6.Checked)
                {
                    p_yarn_shortage6 = "Y";
                }
                else
                {
                    p_yarn_shortage6 = "N";
                }
                if (chknewyarn1.Checked)
                {
                    p_new_yarn1 = "Y";
                }
                else
                {
                    p_new_yarn1 = "N";
                }
                if (chknewyarn2.Checked)
                {
                    p_new_yarn2 = "Y";
                }
                else
                {
                    p_new_yarn2 = "N";
                }
                if (chknewyarn3.Checked)
                {
                    p_new_yarn3 = "Y";
                }
                else
                {
                    p_new_yarn3 = "N";
                }
                if (chknewyarn4.Checked)
                {
                    p_new_yarn4 = "Y";
                }
                else
                {
                    p_new_yarn4 = "N";
                }
                if (chknewyarn5.Checked)
                {
                    p_new_yarn5 = "Y";
                }
                else
                {
                    p_new_yarn5 = "N";
                }
                if (chknewyarn6.Checked)
                {
                    p_new_yarn6 = "Y";
                }
                else
                {
                    p_new_yarn6 = "N";
                }
                p_test_method_id = ddlTestMethod.SelectedValue;
                p_shoe_size_id = ddlShoeSize.SelectedValue;
                p_shoe_style = txtShoeStyle.Text;
                p_shoe_length = txtShoeLength.Text;
                p_shoe_width = txtShoeWidth.Text;
                p_pallet_pattern_no = txtPalletPatternNo.Text;
                p_product_pattern_no = txtProductPatternNo.Text;
                p_shoe_pairs_required = txtPairs.Text;
                if (chkLaserCut.Checked)
                {
                    p_shoe_require_laser_cut = "Y";
                }
                else
                {
                    p_shoe_require_laser_cut = "N";
                }

                if (chkWithPattern.Checked)
                {
                    p_laser_cut_with_pattern = "Y";
                }
                else
                {
                    p_laser_cut_with_pattern = "N";
                }
                p_repeat_per_roll = txtRptPerRoll.Text.Trim();

                classDevelop_Request_BLL obj = new classDevelop_Request_BLL();
                DataTable dtresult = obj.UpdateDevelopREquest(p_pdr_new_develop_req_id, p_pdr_no, p_pdr_date, p_design_no, p_gauge,
                                                        p_custcd, p_requested_by, p_expected_price, p_uom, p_expected_date,
                                                        p_product_application_id, p_development_type_id, p_development_type_remark,
                                                        p_itcatid, p_itsubcatid, p_itgroupid, p_itsubid, p_ittypeid, p_itsubid2,
                                                        p_standard_aatcc, p_standard_astm, p_standard_iso, p_standard_jis, p_standard_ms,
                                                        p_standard_hm, p_standard_vss, p_standard_customer,
                                                        p_follow_customer_spec, p_yarn_face1, p_yarn_face2, p_yarn_face3, p_yarn_face4,
                                                        p_yarn_face5, p_yarn_face6,
                                                        p_composition1_percent, p_composition2_percent, p_composition3_percent, p_composition4_percent,
                                                        p_composition5_percent, p_composition6_percent,
                                                        p_composition1_name, p_composition2_name, p_composition3_name, p_composition4_name,
                                                        p_composition5_name, p_composition6_name, p_weight_sqm,
                                                        p_weight_sqm_tolerance, p_weight_sqm_remark, p_width, p_width_tolerance, p_elongation,
                                                        p_elongation_l, p_elongation_w, p_elongation_tolerance, p_modulus, p_modulus_l, p_modulus_w,
                                                        p_modulus_tolerance, p_stretch_l, p_stretch_w, p_stretch_tolerance, p_shrinkage_l, p_shrinkage_w,
                                                        p_shrinkage_tolerance, p_piling, p_pilling_remark, p_snagging, p_snagging_remark, p_color_name,
                                                        p_dye_finishing_formula_id, p_dye_remark, p_hanger, p_yardage, p_require_tag, p_machine_dense,
                                                        p_yarn_available, p_yarn_available_date, p_knitting_appointment, p_offer_price, p_is_consideration,
                                                        p_expected_finished_date, p_spec_master_date, p_qa_report_date, p_remark, p_user_session_id,
                                                        fileName, p_endbuyercd, p_priority_id, p_priority_comment, p_price_curr, p_itcd1, p_itcd2,
                                                        p_itcd3, p_itcd4, p_itcd5, p_itcd6,
                                                        p_avail1, p_avail2, p_avail3, p_avail4, p_avail5, p_avail6, p_kg_per_finished_roll, p_no_of_finished_rolls,
                                                        p_yarn_shortage1, p_yarn_shortage2, p_yarn_shortage3, p_yarn_shortage4, p_yarn_shortage5, p_yarn_shortage6,
                                                        p_new_yarn1, p_new_yarn2, p_new_yarn3, p_new_yarn4, p_new_yarn5, p_new_yarn6,p_test_method_id,
                                                        p_shoe_style, p_shoe_size_id, p_shoe_gender_id, p_shoe_length, p_shoe_width, p_pallet_pattern_no, p_product_pattern_no,
                                                        p_shoe_require_laser_cut, p_laser_cut_with_pattern, p_shoe_pairs_required,p_repeat_per_roll);
                valuechanged = "NO";

                foreach (DataRow resultrow in dtresult.Rows)
                {
                    hidpdr_new_develop_req_id.Value = resultrow[0].ToString();
                    txtPDRNO.Text = (resultrow[1].ToString());
                }
                //fileName = Path.GetFileName(uploadfiles1.FileName);
                //if (fileName != "")
                //{
                //    bool exists = System.IO.Directory.Exists(Server.MapPath("~/ANALYZE/"));

                //    if (!exists)
                //        System.IO.Directory.CreateDirectory(Server.MapPath("~/ANALYZE/"));
                //    uploadfiles1.SaveAs("//172.16.3.4/pdr_files/ANALYZE/" + fileName);
                //    // lblMsg.Text = "File Uploaded Successfully";
                //}
                // Save the Grid Value
                DataTable dtsave = new DataTable();
                dtsave.Columns.AddRange(new DataColumn[8] { new DataColumn("p_pdr_item_properties_id", typeof(int)),
                new DataColumn("p_pdr_new_develop_req_id", typeof(int)),
                new DataColumn("p_appl_id",typeof(string)),
                new DataColumn("p_sub_appl_id",typeof(string)),
                new DataColumn("p_spl_func_id",typeof(string)),
                 new DataColumn("p_ctry",typeof(string)),
                new DataColumn("p_market_zone_id",typeof(string)),
                new DataColumn("p_market_customer_id",typeof(string))
                 });
                Int64 pdr_item_properties_id;
                string pdr_new_develop_req_id = hidpdr_new_develop_req_id.Value.ToString().Trim();
                string appl_id;
                string sub_appl_id;
                string spl_func_id;
                string ctry;
                string market_zone_id;
                string market_customer_id;
                DataTable dtretunvalue;
                // newly added to check 
                DataTable dtCurrentTable = (DataTable)ViewState["pdrDevReq"];
                // Response.Write(dtCurrentTable.Rows.Count);
                DataRow drCurrentRow = dtCurrentTable.NewRow();// null;
                Int32 counti = 0;
                Int32 editrowcount = 0;
                //
                foreach (GridViewRow devreqrow in gvDevelop_Request.Rows)
                {
                    DropDownList ddlApp = devreqrow.FindControl("ddlAppl") as DropDownList;
                    //  DropDownList dt_ddlApp = gvDevelop_Request.Rows[editrowcount].FindControl("ddlAppl") as DropDownList;

                    //string appid= dt_ddlApp.SelectedItem.Value.ToString().Trim(); //ddlApp.SelectedItem.Value.ToString().Trim();
                    appl_id = Request[ddlApp.UniqueID] as string;
                    DropDownList ddlsubApp = devreqrow.FindControl("ddlSubAppl") as DropDownList;
                    sub_appl_id = Request[ddlsubApp.UniqueID] as string;
                    DropDownList ddlsplfunc = devreqrow.FindControl("ddlsplfunc") as DropDownList;
                    spl_func_id = Request[ddlsplfunc.UniqueID] as string;
                    DropDownList ddlcountry = devreqrow.FindControl("ddlctry") as DropDownList;
                    ctry = Request[ddlcountry.UniqueID] as string;
                    DropDownList ddlZone = devreqrow.FindControl("ddlzone") as DropDownList;
                    market_zone_id = Request[ddlZone.UniqueID] as string;
                    DropDownList ddlcustomer = devreqrow.FindControl("ddlcustomer") as DropDownList;
                    market_customer_id = Request[ddlcustomer.UniqueID] as string;
                    if (gvDevelop_Request.DataKeys[devreqrow.RowIndex].Values[1].ToString() != string.Empty)
                    {
                        pdr_item_properties_id = Convert.ToInt64(gvDevelop_Request.DataKeys[devreqrow.RowIndex].Values[1].ToString().Trim());
                    }
                    else
                    {
                        pdr_item_properties_id = -1;
                    }
                    //dtsave.Rows.Add(pdr_item_properties_id, pdr_new_develop_req_id, appl_id, sub_appl_id, spl_func_id, ctry, market_zone_id, market_customer_id);
                    //  editrowcount = editrowcount + 1;
                }
               
                //DataTable dtCurrentTable = (DataTable)ViewState["pdrDevReq"];
                //// Response.Write(dtCurrentTable.Rows.Count);
                //DataRow drCurrentRow = dtCurrentTable.NewRow();// null;
                //Int32 counti = 0;
                //Int32 
                editrowcount = 0;
                foreach (DataRow editrow in dtCurrentTable.Rows)
                {
                    DropDownList ddlApp = gvDevelop_Request.Rows[editrowcount].FindControl("ddlAppl") as DropDownList;
                    appl_id = ddlApp.SelectedItem.Value.ToString().Trim();// appl_name;

                    DropDownList ddlSubAppl = gvDevelop_Request.Rows[editrowcount].FindControl("ddlSubAppl") as DropDownList;
                    sub_appl_id = ddlSubAppl.SelectedItem.Value.ToString().Trim();// sub_appl_name;

                    DropDownList ddlsplfunc = gvDevelop_Request.Rows[editrowcount].FindControl("ddlsplfunc") as DropDownList;
                    spl_func_id = ddlsplfunc.SelectedItem.Value.ToString().Trim();// sub_appl_name;

                    DropDownList ddlctry = gvDevelop_Request.Rows[editrowcount].FindControl("ddlctry") as DropDownList;
                    ctry = ddlctry.SelectedItem.Value.ToString().Trim();// sub_appl_name;

                    DropDownList ddlzone = gvDevelop_Request.Rows[editrowcount].FindControl("ddlzone") as DropDownList;
                    market_zone_id = ddlzone.SelectedItem.Value.ToString().Trim();// sub_appl_name;

                    DropDownList ddlcustomer = gvDevelop_Request.Rows[editrowcount].FindControl("ddlcustomer") as DropDownList;
                    market_customer_id = ddlcustomer.SelectedItem.Value.ToString().Trim();// sub_appl_name;
                    if (editrow["pdr_item_properties_id"].ToString() != string.Empty)
                    {
                        pdr_item_properties_id = Convert.ToInt64(editrow["pdr_item_properties_id"].ToString().Trim());// Convert.ToInt64(gvDevelop_Request.Rows[editrowcount].Cells[0].Text.ToString().Trim());
                    }
                    else
                    {
                        pdr_item_properties_id = -1;
                    }
                    dtsave.Rows.Add(pdr_item_properties_id, pdr_new_develop_req_id, appl_id, sub_appl_id, spl_func_id, ctry, market_zone_id, market_customer_id);
                    editrowcount = editrowcount + 1;
                    // editrow.EndEdit();
                    // dtCurrentTable.AcceptChanges();
                    // ddlApp.SelectedItem.Value = ddlApp.SelectedItem.Value.ToString().Trim();
                }

                dtretunvalue = obj.Save_DevRequestApp(dtsave);
                DataTable dtdevreq;
                classDevelop_Request_BLL devrequest = new classDevelop_Request_BLL();
                dtdevreq = devrequest.LoadDeveRequestApp(hidpdr_new_develop_req_id.Value.ToString().Trim());
                gvDevelop_Request.DataSource = dtdevreq;
                gvDevelop_Request.DataBind();
                ViewState["pdrDevReq"] = dtdevreq;
                if (dtdevreq.Rows.Count <= 0)
                {
                    BindHeaderRowInGridview();
                }
                //
                classDevelop_Request_BLL developRequestnew = new classDevelop_Request_BLL();
                dtpdrlist = developRequestnew.LoadPDRList(txtPDRNO.Text.Trim());
                ViewState["pdrlist"] = dtpdrlist;
                string followcustomerspec = "";
                if (dtpdrlist != null)
                {
                    foreach (DataRow newrow in dtpdrlist.Rows)
                    {

                        hidpdr_new_develop_req_id.Value = newrow["pdr_new_develop_req_id"].ToString();
                        txtPDRNO.Text = (newrow["pdr_no"].ToString());
                        if (!string.IsNullOrEmpty(newrow["pdr_date"].ToString()))
                        {
                            DateTime dt = Convert.ToDateTime(newrow["pdr_date"].ToString());
                            PDRDate.Value = String.Format("{0:dd-MM-yyyy}", dt);// dt.ToString("dd-MM-yyyy");// String.Format("M/d/yyyy", dt);// Convert.ToDateTime(row["pdr_date"]).ToString();
                        }
                        if (!string.IsNullOrEmpty(newrow["design_no"].ToString()))
                        {
                            txtDesignno.Text = newrow["design_no"].ToString();
                        }
                        ddlSubCat.SelectedValue = newrow["itsubcatid"].ToString();
                        ddlgroup.SelectedValue = newrow["itgroupid"].ToString();
                        ddlCategory.SelectedValue = newrow["itcatid"].ToString();

                        ddlSubGroup.SelectedValue = newrow["itsubid"].ToString();
                        ddlType.SelectedValue = newrow["ittypeid"].ToString();
                        ddlSubType.SelectedValue = newrow["itsubid2"].ToString();
                        txtDevelopmentTypeRemark.Text = newrow["development_type_remark"].ToString();

                        ddlPriority.SelectedValue = newrow["priority_id"].ToString();
                        txtPriorityComment.Text = newrow["priority_comment"].ToString();
                        ddlcombocurrency.SelectedValue = newrow["price_curr"].ToString().Trim();
                        ddlCode1.SelectedValue = newrow["itcd1"].ToString();
                        ddlCode2.SelectedValue = newrow["itcd2"].ToString();
                        ddlCode3.SelectedValue = newrow["itcd3"].ToString();
                        ddlCode4.SelectedValue = newrow["itcd4"].ToString();
                        ddlCode5.SelectedValue = newrow["itcd5"].ToString();
                        ddlCode6.SelectedValue = newrow["itcd6"].ToString();
                        ddlEndBuyer.SelectedValue = newrow["endbuyerid"].ToString();
                        if (!string.IsNullOrEmpty(newrow["follow_customer_spec"].ToString()))
                        {
                            followcustomerspec = newrow["follow_customer_spec"].ToString();
                            if (followcustomerspec == "N")
                            {
                                CHKFollow.Checked = false;

                            }

                            else
                            {
                                CHKFollow.Checked = true;
                            }
                        }
                        else
                        {
                            CHKFollow.Checked = false;

                        }

                        txtDevelopmentTypeRemark.Text = newrow["development_type_remark"].ToString();
                        txtComposition1_name.Text = newrow["composition1_name"].ToString();
                        txtComposition2_name.Text = newrow["composition2_name"].ToString();
                        txtComposition3_name.Text = newrow["composition3_name"].ToString();
                        txtComposition4_name.Text = newrow["composition4_name"].ToString();
                        txtweight.Text = newrow["weight_sqm"].ToString();
                        TXTWIDTH.Text = newrow["width"].ToString();
                        TXTElongation.Text = newrow["elongation"].ToString();
                        TXTMODULUS.Text = newrow["modulus"].ToString();
                        txtRecovStretbility.Text = "";
                        txtDimStbilityShr.Text = "";
                        txtremark.Text = newrow["remark"].ToString();
                        txtweightTolerance.Text = newrow["weight_sqm_tolerance"].ToString();
                        txtwidth_Tolerance.Text = newrow["width_tolerance"].ToString();
                        txtpilingremark.Text = newrow["pilling_remark"].ToString();
                        txtsnaggingremark.Text = newrow["snagging_remark"].ToString();
                        txtdyeincolor.Text = newrow["color_name"].ToString();
                        txtMachineTense.Text = newrow["machine_dense"].ToString();
                        // if (row["knitting_appointment"] != null || row["knitting_appointment"].ToString() != "{}")
                        if (!string.IsNullOrEmpty(newrow["knitting_appointment"].ToString()))
                        {

                            DateTime dt1 = Convert.ToDateTime(newrow["knitting_appointment"].ToString());
                            txtknittingappoinment.Value = String.Format("{0:dd-MM-yyyy}", dt1);// row["knitting_appointment"].ToString();
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_available_date"].ToString()))
                        {
                            DateTime dt2 = Convert.ToDateTime(newrow["yarn_available_date"].ToString());
                            txtyarnavailabledate.Value = String.Format("{0:dd-MM-yyyy}", dt2);// row["yarn_available_date"].ToString();
                        }
                        if (!string.IsNullOrEmpty(newrow["expected_finished_date"].ToString()))
                        {
                            DateTime dt3 = Convert.ToDateTime(newrow["expected_finished_date"].ToString());
                            txtExpectedFinisheddate.Value = String.Format("{0:dd-MM-yyyy}", dt3);//row["expected_finished_date"].ToString();
                        }
                        if (!string.IsNullOrEmpty(newrow["spec_master_date"].ToString()))
                        {
                            DateTime dt4 = Convert.ToDateTime(newrow["spec_master_date"].ToString());
                            txtspecmasterdate.Value = String.Format("{0:dd-MM-yyyy}", dt4);//row["spec_master_date"].ToString();
                        }
                        if (!string.IsNullOrEmpty(newrow["qa_report_date"].ToString()))
                        {
                            DateTime dt5 = Convert.ToDateTime(newrow["qa_report_date"].ToString());
                            txtqa_reportdate.Value = String.Format("{0:dd-MM-yyyy}", dt5);//= row["qa_report_date"].ToString();
                        }

                        txtStandard_Customer.Text = newrow["test_method_text"].ToString();
                        txtCustPriceExpectation.Text = newrow["expected_price"].ToString();
                        txtPercent1.Text = newrow["composition1_percent"].ToString();
                        txtPercent2.Text = newrow["composition2_percent"].ToString();
                        txtPercent3.Text = newrow["composition3_percent"].ToString();
                        txtPercent4.Text = newrow["composition4_percent"].ToString();
                        txtPercent5.Text = newrow["composition5_percent"].ToString();
                        txtPercent6.Text = newrow["composition6_percent"].ToString();

                        TXTGUAGE.Text = newrow["gauge"].ToString();
                        if (!string.IsNullOrEmpty(newrow["yarn_face1"].ToString()))
                        {
                            ddlFront.SelectedValue = newrow["yarn_face1"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_face2"].ToString()))
                        {
                            ddlBack.SelectedValue = newrow["yarn_face2"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_face3"].ToString()))
                        {


                            ddlfront1.SelectedValue = newrow["yarn_face3"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_face4"].ToString()))
                        {
                            ddlfront2.SelectedValue = newrow["yarn_face4"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_face5"].ToString()))
                        {
                            ddlfront5.SelectedValue = newrow["yarn_face5"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_face6"].ToString()))
                        {
                            ddlfront6.SelectedValue = newrow["yarn_face6"].ToString().Trim();
                        }

                        txtAvail1.Text = newrow["yarn_avail_qty1"].ToString();
                        txtAvail2.Text = newrow["yarn_avail_qty2"].ToString();
                        txtAvail3.Text = newrow["yarn_avail_qty3"].ToString();
                        txtAvail4.Text = newrow["yarn_avail_qty4"].ToString();
                        txtAvail5.Text = newrow["yarn_avail_qty5"].ToString();
                        txtAvail6.Text = newrow["yarn_avail_qty6"].ToString();

                        if (!string.IsNullOrEmpty(newrow["dye_finishing_formula_id"].ToString()))
                        {
                            ddlFinishing.SelectedValue = newrow["dye_finishing_formula_id"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(newrow["require_tag"].ToString()))
                        {
                            string gangtagrequired = newrow["require_tag"].ToString();
                            if (gangtagrequired == "N" || gangtagrequired == "")
                            {
                                chkHangTagRequire.Checked = false;
                            }
                            else
                            {
                                chkHangTagRequire.Checked = true;
                            }
                        }



                        TXTYARDAGE.Text = newrow["yardage"].ToString();
                        string snagging = newrow["snagging"].ToString();
                        if (snagging == "N")
                        {
                            chksnagging.Checked = false;
                        }
                        else
                        {
                            chksnagging.Checked = true;
                        }
                        string pilling = newrow["piling"].ToString();
                        if (pilling == "N")
                        {
                            chkpilling.Checked = false;
                        }
                        else
                        {
                            chkpilling.Checked = true;
                        }

                        string standard_aatcc = newrow["standard_aatcc"].ToString();
                        if (standard_aatcc == "N" || standard_aatcc == "0")
                        {
                            CHKAATCC.Checked = false;
                        }
                        else
                        {
                            CHKAATCC.Checked = true;
                        }
                        string standard_astm = newrow["standard_astm"].ToString();
                        if (standard_astm == "N" || standard_astm == "0")
                        {
                            CHKASTM.Checked = false;
                        }
                        else
                        {
                            CHKASTM.Checked = true;
                        }

                        string standard_iso = newrow["standard_iso"].ToString();
                        if (standard_iso == "N" || standard_iso == "0")
                        {
                            CHKISO.Checked = false;
                        }
                        else
                        {
                            CHKISO.Checked = true;
                        }
                        string standard_jis = newrow["standard_jis"].ToString();
                        if (standard_jis == "N" || standard_jis == "0")
                        {
                            CHKJIS.Checked = false;
                        }
                        else
                        {
                            CHKJIS.Checked = true;
                        }
                        string standard_ms = newrow["standard_ms"].ToString();
                        if (standard_ms == "N" || standard_ms == "0")
                        {
                            chkMS.Checked = false;
                        }
                        else
                        {
                            chkMS.Checked = true;
                        }
                        string standard_hm = newrow["standard_hm"].ToString();
                        if (standard_hm == "N" || standard_hm == "0")
                        {
                            chkHM.Checked = false;
                        }
                        else
                        {
                            chkHM.Checked = true;
                        }
                        string standard_vss = newrow["standard_vss"].ToString();
                        if (standard_vss == "N" || standard_vss == "0")
                        {
                            chkVSS.Checked = false;
                        }
                        else
                        {
                            chkVSS.Checked = true;
                        }

                        txtElongation_l.Text = newrow["elongation_l"].ToString();
                        txtmodulus_l.Text = newrow["modulus_l"].ToString();
                        txtstretch_l.Text = newrow["stretch_l"].ToString();
                        txtshrinkage_l.Text = newrow["shrinkage_l"].ToString();
                        txtElongation_w.Text = newrow["elongation_w"].ToString();
                        txtmodulus_w.Text = newrow["modulus_w"].ToString();
                        txtstretch_w.Text = newrow["stretch_w"].ToString();
                        txtshrinkage_w.Text = newrow["shrinkage_w"].ToString();
                        txtelongation_tolerance.Text = newrow["elongation_tolerance"].ToString();
                        txtmodulus_tolerance.Text = newrow["modulus_tolerance"].ToString();
                        txtshrinkage_tolerance.Text = newrow["shrinkage_tolerance"].ToString();
                        txtstretch_tolerance.Text = newrow["stretch_tolerance"].ToString();
                        ddlReasonForDevl.SelectedValue = newrow["development_type_id"].ToString();
                        //  ddlPurposeofGarment.SelectedValue = newrow["product_application_id"].ToString();
                        if (!string.IsNullOrEmpty(newrow["expected_date"].ToString()))
                        {
                            DateTime dt6 = Convert.ToDateTime(newrow["expected_date"].ToString());
                            dtTo.Value = String.Format("{0:dd-MM-yyyy}", dt6);//= row["qa_report_date"].ToString();
                                                                              //dtTo.Value = row["expected_date"].ToString();
                        }

                        txtSampleRequire.Text = newrow["hanger"].ToString();
                        txtStandard_Customer.Text = newrow["standard_customer"].ToString();
                        //txtComment.Text = 
                        string yarnavailable = newrow["yarn_available"].ToString();
                        if (yarnavailable == "N")
                        {
                            chkYarnAvailable.Checked = false;
                        }
                        else
                        {
                            chkYarnAvailable.Checked = true;
                        }

                        txtweightTolerance.Text = newrow["weight_sqm_tolerance"].ToString();
                        txtwidth_Tolerance.Text = newrow["width_tolerance"].ToString();
                         txtkgperfinishedroll.Text = (newrow["kg_per_finished_roll"].ToString());
                         txtTotalFinishedRolls.Text = (newrow["no_of_finished_rolls"].ToString());
                        if (!string.IsNullOrEmpty(newrow["yarn_shortage1"].ToString()))
                        {
                            string yarn_shortage1 = newrow["yarn_shortage1"].ToString();
                            if (yarn_shortage1 == "N" || yarn_shortage1 == "")
                            {
                                chkshortage1.Checked = false;
                            }
                            else
                            {
                                chkshortage1.Checked = true;
                            }
                        }

                        if (!string.IsNullOrEmpty(newrow["yarn_shortage2"].ToString()))
                        {
                            string yarn_shortage2 = newrow["yarn_shortage2"].ToString();
                            if (yarn_shortage2 == "N" || yarn_shortage2 == "")
                            {
                                chkshortage2.Checked = false;
                            }
                            else
                            {
                                chkshortage2.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_shortage3"].ToString()))
                        {
                            string yarn_shortage3 = newrow["yarn_shortage3"].ToString();
                            if (yarn_shortage3 == "N" || yarn_shortage3 == "")
                            {
                                chkshortage3.Checked = false;
                            }
                            else
                            {
                                chkshortage3.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_shortage4"].ToString()))
                        {
                            string yarn_shortage4 = newrow["yarn_shortage4"].ToString();
                            if (yarn_shortage4 == "N" || yarn_shortage4 == "")
                            {
                                chkshortage4.Checked = false;
                            }
                            else
                            {
                                chkshortage4.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_shortage5"].ToString()))
                        {
                            string yarn_shortage5 = newrow["yarn_shortage5"].ToString();
                            if (yarn_shortage5 == "N" || yarn_shortage5 == "")
                            {
                                chkshortage5.Checked = false;
                            }
                            else
                            {
                                chkshortage5.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(newrow["yarn_shortage6"].ToString()))
                        {
                            string yarn_shortage6 = newrow["yarn_shortage6"].ToString();
                            if (yarn_shortage6 == "N" || yarn_shortage6 == "")
                            {
                                chkshortage6.Checked = false;
                            }
                            else
                            {
                                chkshortage6.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(newrow["new_yarn1"].ToString()))
                        {
                            string new_yarn1 = newrow["new_yarn1"].ToString();
                            if (new_yarn1 == "N" || new_yarn1 == "")
                            {
                                chknewyarn1.Checked = false;
                            }
                            else
                            {
                                chknewyarn1.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(newrow["new_yarn2"].ToString()))
                        {
                            string new_yarn2 = newrow["new_yarn2"].ToString();
                            if (new_yarn2 == "N" || new_yarn2 == "")
                            {
                                chknewyarn2.Checked = false;
                            }
                            else
                            {
                                chknewyarn2.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(newrow["new_yarn3"].ToString()))
                        {
                            string new_yarn3 = newrow["new_yarn3"].ToString();
                            if (new_yarn3 == "N" || new_yarn3 == "")
                            {
                                chknewyarn3.Checked = false;
                            }
                            else
                            {
                                chknewyarn3.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(newrow["new_yarn4"].ToString()))
                        {
                            string new_yarn4 = newrow["new_yarn4"].ToString();
                            if (new_yarn4 == "N" || new_yarn4 == "")
                            {
                                chknewyarn4.Checked = false;
                            }
                            else
                            {
                                chknewyarn4.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(newrow["new_yarn5"].ToString()))
                        {
                            string new_yarn5 = newrow["new_yarn5"].ToString();
                            if (new_yarn5 == "N" || new_yarn5 == "")
                            {
                                chknewyarn5.Checked = false;
                            }
                            else
                            {
                                chknewyarn5.Checked = true;
                            }
                        }
                        if (!string.IsNullOrEmpty(newrow["new_yarn6"].ToString()))
                        {
                            string new_yarn6 = newrow["new_yarn6"].ToString();
                            if (new_yarn6 == "N" || new_yarn6 == "")
                            {
                                chknewyarn6.Checked = false;
                            }
                            else
                            {
                                chknewyarn6.Checked = true;
                            }
                        }

                        //lblFileName.Text = newrow["file_location"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message.ToString());
            }
        }

        protected void ImgAttachment_Click(object sender, ImageClickEventArgs e)
        {

            Response.Redirect("~/UI/DevelopAttachment.aspx?devReqID=" + hidpdr_new_develop_req_id.Value.Trim() + "&product=" + txtDesignno.Text.Trim() + "&pdrno=" + txtPDRNO.Text.Trim());
        }

        //private void CopyPDRNO()
        //{
        //    string p_pdr_new_develop_req_id = "";
        //    string p_pdr_no = "";
        //    string p_pdr_date = "";
        //    string p_design_no = "";
        //    string p_gauge = "";
        //    string p_custcd = "";
        //    string p_requested_by = "";
        //    string p_expected_price = "";
        //    string p_uom = "";
        //    string p_expected_date = "";
        //    string p_product_application_id = "";
        //    string p_development_type_id = "";
        //    string p_development_type_remark = "";
        //    string p_itcatid = "";
        //    string p_itsubcatid = "";
        //    string p_itgroupid = "";
        //    string p_itsubid = "";
        //    string p_ittypeid = "";
        //    string p_itsubid2 = "";
        //    string p_standard_iso = "";
        //    string p_standard_aatcc = "";
        //    string p_standard_astm = "";
        //    string p_standard_jis = "";
        //    string p_standard_customer = "";
        //    string p_follow_customer_spec = "";
        //    string p_yarn_face1 = "";
        //    string p_yarn_face2 = "";
        //    string p_yarn_face3 = "";
        //    string p_yarn_face4 = "";
        //    string p_yarn_face5 = "";
        //    string p_yarn_face6 = "";
        //    string p_composition1_percent = "";
        //    string p_composition2_percent = "";
        //    string p_composition3_percent = "";
        //    string p_composition4_percent = "";
        //    string p_composition5_percent = "";
        //    string p_composition6_percent = "";
        //    string p_composition1_name = "";
        //    string p_composition2_name = "";
        //    string p_composition3_name = "";
        //    string p_composition4_name = "";
        //    string p_composition5_name = "";
        //    string p_composition6_name = "";
        //    string p_weight_sqm = "";
        //    string p_weight_sqm_tolerance = "";
        //    string p_weight_sqm_remark = "";
        //    string p_width = "";
        //    string p_width_tolerance = "";
        //    string p_elongation = "";
        //    string p_elongation_l = "";
        //    string p_elongation_w = "";
        //    string p_elongation_tolerance = "";
        //    string p_modulus = "";
        //    string p_modulus_l = "";
        //    string p_modulus_w = "";
        //    string p_modulus_tolerance = "";
        //    string p_stretch_l = "";
        //    string p_stretch_w = "";
        //    string p_stretch_tolerance = "";
        //    string p_shrinkage_l = "";
        //    string p_shrinkage_w = "";
        //    string p_shrinkage_tolerance = "";
        //    string p_piling = "";
        //    string p_pilling_remark = "";
        //    string p_snagging = "";
        //    string p_snagging_remark = "";
        //    string p_color_name = "";
        //    string p_dye_finishing_formula_id = "";
        //    string p_dye_remark = "";
        //    string p_hanger = "";
        //    string p_yardage = "";
        //    string p_require_tag = "";
        //    string p_machine_dense = "";
        //    string p_yarn_available = "";
        //    string p_yarn_available_date = "";
        //    string p_knitting_appointment = "";
        //    string p_offer_price = "";
        //    string p_is_consideration = "";
        //    string p_expected_finished_date = "";
        //    string p_spec_master_date = "";
        //    string p_qa_report_date = "";
        //    string p_remark = "";
        //    string p_user_session_id = Session["usersessionid"].ToString();
        //    valuechanged = "NO";
        //    string fileName = "";
        //    string p_standard_ms = "";
        //    string p_standard_vss = "";
        //    string p_standard_hm = "";
        //    string p_endbuyercd = "";
        //    string p_priority_id = "";
        //    string p_priority_comment = "";
        //    string p_price_curr = "";
        //    string p_itcd1 = "";
        //    string p_itcd2 = "";
        //    string p_itcd3 = "";
        //    string p_itcd4 = "";
        //    string p_itcd5 = "";
        //    string p_itcd6 = "";
        //    Decimal p_avail1 = 0;
        //    Decimal p_avail2 = 0;
        //    Decimal p_avail3 = 0;
        //    Decimal p_avail4 = 0;
        //    Decimal p_avail5 = 0;
        //    Decimal p_avail6 = 0;
        //    try
        //    {
        //        p_pdr_new_develop_req_id = hidpdr_new_develop_req_id.Value;
        //        p_pdr_no = "NEW";// txtPDRNO.Text;
        //        p_pdr_date = PDRDate.Value;
        //        string day = p_pdr_date.Substring(0, 2);
        //        string mon = p_pdr_date.Substring(3, 2);
        //        string yr = p_pdr_date.Substring(6, 4);
        //        p_pdr_date = yr + "-" + mon + "-" + day;// + " 00:00:00";
        //        p_design_no = txtDesignno.Text;
        //        p_custcd = hidSearchMainValue.Value;// hidSearchMainValue.Value.Trim();
        //        p_gauge = TXTGUAGE.Text;
        //        //p_custcd = "";
        //        p_requested_by = ddlPDRRequestor.SelectedValue;
        //        p_expected_price = txtCustPriceExpectation.Text;
        //        p_uom = ddluom.SelectedValue;
        //        p_expected_date = dtTo.Value;
        //        if (p_expected_date.ToString().Trim() != "")
        //        {
        //            string day1 = p_expected_date.Substring(0, 2);
        //            string mon1 = p_expected_date.Substring(3, 2);
        //            string yr1 = p_expected_date.Substring(6, 4);
        //            p_expected_date = yr1 + "-" + mon1 + "-" + day1;
        //        }
        //        p_product_application_id = null;// ddlPurposeofGarment.SelectedValue;
        //        p_development_type_id = ddlReasonForDevl.SelectedValue;
        //        p_development_type_remark = txtDevelopmentTypeRemark.Text;
        //        p_itcatid = ddlCategory.SelectedValue;
        //        p_itsubcatid = ddlSubCat.SelectedValue;
        //        p_itgroupid = ddlgroup.SelectedValue;
        //        p_itsubid = ddlSubGroup.SelectedValue;
        //        p_ittypeid = ddlType.SelectedValue;
        //        p_itsubid2 = ddlSubType.SelectedValue;
        //        if (CHKISO.Checked)
        //        {
        //            p_standard_iso = "Y";
        //        }
        //        else
        //        {
        //            p_standard_iso = "N";
        //        }
        //        if (CHKAATCC.Checked)
        //        {
        //            p_standard_aatcc = "Y";
        //        }
        //        else
        //        {
        //            p_standard_aatcc = "N";
        //        }
        //        if (CHKASTM.Checked)
        //        {
        //            p_standard_astm = "Y";
        //        }
        //        else
        //        {
        //            p_standard_astm = "N";
        //        }
        //        if (CHKJIS.Checked)
        //        {
        //            p_standard_jis = "Y";
        //        }
        //        else
        //        {
        //            p_standard_jis = "N";
        //        }
        //        if (chkMS.Checked)
        //        {
        //            p_standard_ms = "Y";
        //        }
        //        else
        //        {
        //            p_standard_ms = "N";
        //        }
        //        if (chkVSS.Checked)
        //        {
        //            p_standard_vss = "Y";
        //        }
        //        else
        //        {
        //            p_standard_vss = "N";
        //        }
        //        if (chkHM.Checked)
        //        {
        //            p_standard_hm = "Y";
        //        }
        //        else
        //        {
        //            p_standard_hm = "N";
        //        }

        //        p_standard_customer = txtStandard_Customer.Text;

        //        if (CHKFollow.Checked)
        //        {
        //            p_follow_customer_spec = "Y";
        //        }
        //        else
        //        {
        //            p_follow_customer_spec = "N";
        //        }
        //        p_yarn_face1 = ddlFront.SelectedValue;
        //        p_yarn_face2 = ddlBack.SelectedValue;
        //        p_yarn_face3 = ddlfront1.SelectedValue;
        //        p_yarn_face4 = ddlfront2.SelectedValue;
        //        p_yarn_face5 = ddlfront5.SelectedValue;
        //        p_yarn_face6 = ddlfront6.SelectedValue;
        //        p_composition1_percent = txtPercent1.Text;
        //        p_composition2_percent = txtPercent2.Text;
        //        p_composition3_percent = txtPercent3.Text;
        //        p_composition4_percent = txtPercent4.Text;
        //        p_composition5_percent = txtPercent5.Text;
        //        p_composition6_percent = txtPercent6.Text;
        //        p_composition1_name = txtComposition1_name.Text;
        //        p_composition2_name = txtComposition2_name.Text;
        //        p_composition3_name = txtComposition3_name.Text;
        //        p_composition4_name = txtComposition4_name.Text;
        //        p_composition5_name = txtComposition5_name.Text;
        //        p_composition6_name = txtComposition6_name.Text;
        //        p_weight_sqm = txtweight.Text;
        //        p_weight_sqm_tolerance = txtweightTolerance.Text;
        //        p_weight_sqm_remark = "";
        //        p_width = TXTWIDTH.Text;
        //        p_width_tolerance = txtwidth_Tolerance.Text;
        //        p_elongation = TXTElongation.Text;
        //        p_elongation_l = txtElongation_l.Text;
        //        p_elongation_w = txtElongation_w.Text;
        //        p_elongation_tolerance = txtelongation_tolerance.Text;
        //        p_modulus = TXTMODULUS.Text;
        //        p_modulus_l = txtmodulus_l.Text;
        //        p_modulus_w = txtmodulus_w.Text;
        //        p_modulus_tolerance = txtmodulus_tolerance.Text;
        //        p_stretch_l = txtstretch_l.Text;
        //        p_stretch_w = txtstretch_w.Text;
        //        p_stretch_tolerance = txtstretch_tolerance.Text;
        //        p_shrinkage_l = txtshrinkage_l.Text;
        //        p_shrinkage_w = txtshrinkage_w.Text;
        //        p_shrinkage_tolerance = txtshrinkage_tolerance.Text;
        //        if (chkpilling.Checked)
        //        {
        //            p_piling = "Y";// chkpilling.Checked.ToString();
        //        }
        //        else
        //        {
        //            p_piling = "N";
        //        }
        //        p_pilling_remark = txtpilingremark.Text;
        //        if (chksnagging.Checked)
        //        {
        //            p_snagging = "Y";// chksnagging.Checked.ToString();
        //        }
        //        else
        //        {
        //            p_snagging = "N";
        //        }
        //        p_snagging_remark = txtsnaggingremark.Text.ToString();
        //        p_color_name = txtdyeincolor.Text;
        //        p_dye_finishing_formula_id = ddlFinishing.SelectedValue;
        //        p_dye_remark = "";
        //        if (txtSampleRequire.Text != "")
        //        {
        //            p_hanger = txtSampleRequire.Text.ToString().Trim();
        //        }
        //        p_yardage = TXTYARDAGE.Text;
        //        if (chkHangTagRequire.Checked)
        //        {
        //            p_require_tag = "Y";
        //        }
        //        else
        //        {
        //            p_require_tag = "N";
        //        }
        //        p_machine_dense = txtMachineTense.Text;
        //        if (chkYarnAvailable.Checked)
        //        {
        //            p_yarn_available = "Y";// chkYarnAvailable.Checked.ToString();
        //        }
        //        else
        //        {
        //            p_yarn_available = "N";
        //        }
        //        p_yarn_available_date = txtyarnavailabledate.Value;
        //        p_knitting_appointment = txtknittingappoinment.Value;
        //        p_offer_price = "";
        //        p_is_consideration = "";
        //        p_expected_finished_date = txtExpectedFinisheddate.Value;
        //        p_spec_master_date = txtspecmasterdate.Value;
        //        p_qa_report_date = txtqa_reportdate.Value;
        //        p_remark = txtremark.Text;
        //        if (uploadfiles1.PostedFile.FileName.Length > 0)
        //        {
        //            fileName = Path.GetFileName(uploadfiles1.PostedFile.FileName);
        //            string filename1 = Path.GetDirectoryName(uploadfiles1.PostedFile.FileName);
        //            HttpPostedFile file = Request.Files["browserHidden"];
        //            FileInfo fileInfo = new FileInfo(fileName);
        //            string directoryFullPath = fileInfo.DirectoryName;
        //            fileName = "//172.16.3.4/pdr_files/ANALYZE/" + fileName;
        //        }
        //        else
        //        {
        //            fileName = lblFileName.Text.ToString().Trim();
        //        }
        //        p_endbuyercd = ddlEndBuyer.SelectedValue;
        //        p_priority_id = ddlPriority.SelectedValue;
        //        p_priority_comment = txtPriorityComment.Text;
        //        p_price_curr = ddlcombocurrency.SelectedValue.ToString().Trim();
        //        p_itcd1 = ddlCode1.SelectedValue;
        //        p_itcd2 = ddlCode2.SelectedValue;
        //        p_itcd3 = ddlCode3.SelectedValue;
        //        p_itcd4 = ddlCode4.SelectedValue;
        //        p_itcd5 = ddlCode5.SelectedValue;
        //        p_itcd6 = ddlCode6.SelectedValue;
        //        if (txtAvail1.Text == "")
        //        {
        //            txtAvail1.Text = "0";

        //        }
        //        if (txtAvail2.Text == "")
        //        {
        //            txtAvail2.Text = "0";

        //        }
        //        if (txtAvail3.Text == "")
        //        {
        //            txtAvail3.Text = "0";

        //        }
        //        if (txtAvail4.Text == "")
        //        {
        //            txtAvail4.Text = "0";

        //        }
        //        if (txtAvail5.Text == "")
        //        {
        //            txtAvail5.Text = "0";

        //        }
        //        if (txtAvail6.Text == "")
        //        {
        //            txtAvail6.Text = "0";

        //        }

        //        p_avail1 = Convert.ToDecimal(txtAvail1.Text);
        //        p_avail2 = Convert.ToDecimal(txtAvail2.Text);
        //        p_avail3 = Convert.ToDecimal(txtAvail3.Text);
        //        p_avail4 = Convert.ToDecimal(txtAvail4.Text);
        //        p_avail5 = Convert.ToDecimal(txtAvail5.Text);
        //        p_avail6 = Convert.ToDecimal(txtAvail6.Text);

        //        classDevelop_Request_BLL obj = new classDevelop_Request_BLL();
        //        DataTable dtresult = obj.UpdateDevelopREquest(p_pdr_new_develop_req_id, p_pdr_no, p_pdr_date, p_design_no, p_gauge,
        //                                                p_custcd, p_requested_by, p_expected_price, p_uom, p_expected_date,
        //                                                p_product_application_id, p_development_type_id, p_development_type_remark,
        //                                                p_itcatid, p_itsubcatid, p_itgroupid, p_itsubid, p_ittypeid, p_itsubid2,
        //                                                 p_standard_aatcc, p_standard_astm, p_standard_iso, p_standard_jis, p_standard_ms,
        //                                               p_standard_hm, p_standard_vss, p_standard_customer,
        //                                                p_follow_customer_spec, p_yarn_face1, p_yarn_face2, p_yarn_face3, p_yarn_face4,
        //                                                p_yarn_face5, p_yarn_face6,
        //                                                 p_composition1_percent, p_composition2_percent, p_composition3_percent, p_composition4_percent,
        //                                                 p_composition5_percent, p_composition6_percent,
        //                                                p_composition1_name, p_composition2_name, p_composition3_name, p_composition4_name,
        //                                                p_composition5_name, p_composition6_name, p_weight_sqm,
        //                                                 p_weight_sqm_tolerance, p_weight_sqm_remark, p_width, p_width_tolerance, p_elongation,
        //                                                p_elongation_l, p_elongation_w, p_elongation_tolerance, p_modulus, p_modulus_l, p_modulus_w,
        //                                                p_modulus_tolerance, p_stretch_l, p_stretch_w, p_stretch_tolerance, p_shrinkage_l, p_shrinkage_w,
        //                                                p_shrinkage_tolerance, p_piling, p_pilling_remark, p_snagging, p_snagging_remark, p_color_name,
        //                                                p_dye_finishing_formula_id, p_dye_remark, p_hanger, p_yardage, p_require_tag, p_machine_dense,
        //                                                p_yarn_available, p_yarn_available_date, p_knitting_appointment, p_offer_price, p_is_consideration,
        //                                                p_expected_finished_date, p_spec_master_date, p_qa_report_date, p_remark, p_user_session_id,
        //                                                fileName, p_endbuyercd, p_priority_id, p_priority_comment, p_price_curr, p_itcd1, p_itcd2,
        //                                                p_itcd3, p_itcd4, p_itcd5, p_itcd6,
        //                                                p_avail1, p_avail2, p_avail3, p_avail4, p_avail5, p_avail6);
        //        valuechanged = "NO";

        //        foreach (DataRow resultrow in dtresult.Rows)
        //        {
        //            hidpdr_new_develop_req_id.Value = resultrow[0].ToString();
        //            txtPDRNO.Text = (resultrow[1].ToString());
        //        }
        //        fileName = Path.GetFileName(uploadfiles1.FileName);
        //        if (fileName != "")
        //        {
        //            bool exists = System.IO.Directory.Exists(Server.MapPath("~/ANALYZE/"));

        //            if (!exists)
        //                System.IO.Directory.CreateDirectory(Server.MapPath("~/ANALYZE/"));
        //            uploadfiles1.SaveAs("//172.16.3.4/pdr_files/ANALYZE/" + fileName);
        //            // lblMsg.Text = "File Uploaded Successfully";
        //        }
        //        // Save the Grid Value
        //        DataTable dtsave = new DataTable();
        //        dtsave.Columns.AddRange(new DataColumn[8] { new DataColumn("p_pdr_item_properties_id", typeof(int)),
        //        new DataColumn("p_pdr_new_develop_req_id", typeof(int)),
        //        new DataColumn("p_appl_id",typeof(string)),
        //        new DataColumn("p_sub_appl_id",typeof(string)),
        //        new DataColumn("p_spl_func_id",typeof(string)),
        //         new DataColumn("p_ctry",typeof(string)),
        //        new DataColumn("p_market_zone_id",typeof(string)),
        //        new DataColumn("p_market_customer_id",typeof(string))
        //         });
        //        Int64 pdr_item_properties_id;
        //        string pdr_new_develop_req_id = hidpdr_new_develop_req_id.Value.ToString().Trim();
        //        string appl_id;
        //        string sub_appl_id;
        //        string spl_func_id;
        //        string ctry;
        //        string market_zone_id;
        //        string market_customer_id;
        //        DataTable dtretunvalue;
        //        // newly added to check 
        //        DataTable dtCurrentTable = (DataTable)ViewState["pdrDevReq"];
        //        // Response.Write(dtCurrentTable.Rows.Count);
        //        DataRow drCurrentRow = dtCurrentTable.NewRow();// null;
        //        Int32 counti = 0;
        //        Int32 editrowcount = 0;
        //        //
        //        foreach (GridViewRow devreqrow in gvDevelop_Request.Rows)
        //        {
        //            TextBox col1 = devreqrow.FindControl("txtItemCode") as TextBox;
        //            itemcode = Request[col1.UniqueID] as string;
        //            if (dtCurrentTable1.Columns.Contains("ITEM_CODE"))
        //            {
        //                col1.Text = itemcode;// dtCurrentTable1.Rows[neweditcount]["appl_name"].ToString().Trim();
        //            }
        //            TextBox col2 = devreqrow.FindControl("txtSearch") as TextBox;
        //            searchtext = Request[col2.UniqueID] as string;
        //            if (dtCurrentTable1.Columns.Contains("ITEM_CODE"))
        //            {
        //                col2.Text = searchtext;// dtCurrentTable1.Rows[neweditcount]["appl_name"].ToString().Trim();
        //            }
        //            TextBox col3 = neweditrow.FindControl("ITEM_PERC") as TextBox;
        //            itemperc = Request[col3.UniqueID] as string;
        //            if (dtCurrentTable1.Columns.Contains("ITEM_PERC"))
        //            {
        //                col3.Text = itemperc;// dtCurrentTable1.Rows[neweditcount]["appl_name"].ToString().Trim();
        //            }
        //            TextBox col4 = neweditrow.FindControl("SUPPLIER_NAME") as TextBox;
        //            supplier = Request[col4.UniqueID] as string;
        //            if (dtCurrentTable1.Columns.Contains("SUPPLIER_NAME"))
        //            {
        //                col4.Text = supplier;// dtCurrentTable1.Rows[neweditcount]["appl_name"].ToString().Trim();
        //            }
        //            TextBox col5 = neweditrow.FindControl("STD_COST") as TextBox;
        //            stdcost = Request[col5.UniqueID] as string;
        //            if (dtCurrentTable1.Columns.Contains("STD_COST"))
        //            {
        //                col5.Text = stdcost;// dtCurrentTable1.Rows[neweditcount]["appl_name"].ToString().Trim();
        //            }
        //            TextBox col6 = neweditrow.FindControl("KNIT_LOSS") as TextBox;
        //            knitloss = Request[col6.UniqueID] as string;
        //            if (dtCurrentTable1.Columns.Contains("KNIT_LOSS"))
        //            {
        //                col6.Text = knitloss;// dtCurrentTable1.Rows[neweditcount]["appl_name"].ToString().Trim();
        //            }
        //            TextBox col7 = neweditrow.FindControl("NET_COST") as TextBox;
        //            netcost = Request[col7.UniqueID] as string;
        //            if (dtCurrentTable1.Columns.Contains("NET_COST"))
        //            {
        //                col7.Text = netcost;// dtCurrentTable1.Rows[neweditcount]["appl_name"].ToString().Trim();
        //            }
        //            if (gvDevelop_Request.DataKeys[devreqrow.RowIndex].Values[1].ToString() != string.Empty)
        //            {
        //                pdr_item_properties_id = Convert.ToInt64(gvDevelop_Request.DataKeys[devreqrow.RowIndex].Values[1].ToString().Trim());
        //            }
        //            else
        //            {
        //                pdr_item_properties_id = -1;
        //            }
        //            //dtsave.Rows.Add(pdr_item_properties_id, pdr_new_develop_req_id, appl_id, sub_appl_id, spl_func_id, ctry, market_zone_id, market_customer_id);
        //            //  editrowcount = editrowcount + 1;
        //        }
        //        //DataTable dtCurrentTable = (DataTable)ViewState["pdrDevReq"];
        //        //// Response.Write(dtCurrentTable.Rows.Count);
        //        //DataRow drCurrentRow = dtCurrentTable.NewRow();// null;
        //        //Int32 counti = 0;
        //        //Int32 
        //        editrowcount = 0;
        //        foreach (DataRow editrow in dtCurrentTable.Rows)
        //        {
        //            DropDownList ddlApp = gvDevelop_Request.Rows[editrowcount].FindControl("ddlAppl") as DropDownList;
        //            appl_id = ddlApp.SelectedItem.Value.ToString().Trim();// appl_name;

        //            DropDownList ddlSubAppl = gvDevelop_Request.Rows[editrowcount].FindControl("ddlSubAppl") as DropDownList;
        //            sub_appl_id = ddlSubAppl.SelectedItem.Value.ToString().Trim();// sub_appl_name;

        //            DropDownList ddlsplfunc = gvDevelop_Request.Rows[editrowcount].FindControl("ddlsplfunc") as DropDownList;
        //            spl_func_id = ddlsplfunc.SelectedItem.Value.ToString().Trim();// sub_appl_name;

        //            DropDownList ddlctry = gvDevelop_Request.Rows[editrowcount].FindControl("ddlctry") as DropDownList;
        //            ctry = ddlctry.SelectedItem.Value.ToString().Trim();// sub_appl_name;

        //            DropDownList ddlzone = gvDevelop_Request.Rows[editrowcount].FindControl("ddlzone") as DropDownList;
        //            market_zone_id = ddlzone.SelectedItem.Value.ToString().Trim();// sub_appl_name;

        //            DropDownList ddlcustomer = gvDevelop_Request.Rows[editrowcount].FindControl("ddlcustomer") as DropDownList;
        //            market_customer_id = ddlcustomer.SelectedItem.Value.ToString().Trim();// sub_appl_name;
        //            if (editrow["pdr_item_properties_id"].ToString() != string.Empty)
        //            {
        //                pdr_item_properties_id = Convert.ToInt64(editrow["pdr_item_properties_id"].ToString().Trim());// Convert.ToInt64(gvDevelop_Request.Rows[editrowcount].Cells[0].Text.ToString().Trim());
        //            }
        //            else
        //            {
        //                pdr_item_properties_id = -1;
        //            }
        //            dtsave.Rows.Add(pdr_item_properties_id, pdr_new_develop_req_id, appl_id, sub_appl_id, spl_func_id, ctry, market_zone_id, market_customer_id);
        //            editrowcount = editrowcount + 1;
        //            // editrow.EndEdit();
        //            // dtCurrentTable.AcceptChanges();
        //            // ddlApp.SelectedItem.Value = ddlApp.SelectedItem.Value.ToString().Trim();
        //        }

        //        dtretunvalue = obj.Save_DevRequestApp(dtsave);
        //        DataTable dtdevreq;
        //        classDevelop_Request_BLL devrequest = new classDevelop_Request_BLL();
        //        dtdevreq = devrequest.LoadDeveRequestApp(hidpdr_new_develop_req_id.Value.ToString().Trim());
        //        gvDevelop_Request.DataSource = dtdevreq;
        //        gvDevelop_Request.DataBind();
        //        ViewState["pdrDevReq"] = dtdevreq;
        //        if (dtdevreq.Rows.Count <= 0)
        //        {
        //            BindHeaderRowInGridview();
        //        }
        //        //
        //        classDevelop_Request_BLL developRequestnew = new classDevelop_Request_BLL();
        //        dtpdrlist = developRequestnew.LoadPDRList(txtPDRNO.Text.Trim());
        //        ViewState["pdrlist"] = dtpdrlist;
        //        string followcustomerspec = "";
        //        if (dtpdrlist != null)
        //        {
        //            foreach (DataRow newrow in dtpdrlist.Rows)
        //            {

        //                hidpdr_new_develop_req_id.Value = newrow["pdr_new_develop_req_id"].ToString();
        //                txtPDRNO.Text = (newrow["pdr_no"].ToString());
        //                if (!string.IsNullOrEmpty(newrow["pdr_date"].ToString()))
        //                {
        //                    DateTime dt = Convert.ToDateTime(newrow["pdr_date"].ToString());
        //                    PDRDate.Value = String.Format("{0:dd-MM-yyyy}", dt);// dt.ToString("dd-MM-yyyy");// String.Format("M/d/yyyy", dt);// Convert.ToDateTime(row["pdr_date"]).ToString();
        //                }
        //                if (!string.IsNullOrEmpty(newrow["design_no"].ToString()))
        //                {
        //                    txtDesignno.Text = newrow["design_no"].ToString();
        //                }
        //                ddlSubCat.SelectedValue = newrow["itsubcatid"].ToString();
        //                ddlgroup.SelectedValue = newrow["itgroupid"].ToString();
        //                ddlCategory.SelectedValue = newrow["itcatid"].ToString();

        //                ddlSubGroup.SelectedValue = newrow["itsubid"].ToString();
        //                ddlType.SelectedValue = newrow["ittypeid"].ToString();
        //                ddlSubType.SelectedValue = newrow["itsubid2"].ToString();
        //                txtDevelopmentTypeRemark.Text = newrow["development_type_remark"].ToString();

        //                ddlPriority.SelectedValue = newrow["priority_id"].ToString();
        //                txtPriorityComment.Text = newrow["priority_comment"].ToString();
        //                ddlcombocurrency.SelectedValue = newrow["price_curr"].ToString().Trim();
        //                ddlCode1.SelectedValue = newrow["itcd1"].ToString();
        //                ddlCode2.SelectedValue = newrow["itcd2"].ToString();
        //                ddlCode3.SelectedValue = newrow["itcd3"].ToString();
        //                ddlCode4.SelectedValue = newrow["itcd4"].ToString();
        //                ddlCode5.SelectedValue = newrow["itcd5"].ToString();
        //                ddlCode6.SelectedValue = newrow["itcd6"].ToString();
        //                ddlEndBuyer.SelectedValue = newrow["endbuyerid"].ToString();
        //                if (!string.IsNullOrEmpty(newrow["follow_customer_spec"].ToString()))
        //                {
        //                    followcustomerspec = newrow["follow_customer_spec"].ToString();
        //                    if (followcustomerspec == "N")
        //                    {
        //                        CHKFollow.Checked = false;

        //                    }

        //                    else
        //                    {
        //                        CHKFollow.Checked = true;
        //                    }
        //                }
        //                else
        //                {
        //                    CHKFollow.Checked = false;

        //                }

        //                txtDevelopmentTypeRemark.Text = newrow["development_type_remark"].ToString();
        //                txtComposition1_name.Text = newrow["composition1_name"].ToString();
        //                txtComposition2_name.Text = newrow["composition2_name"].ToString();
        //                txtComposition3_name.Text = newrow["composition3_name"].ToString();
        //                txtComposition4_name.Text = newrow["composition4_name"].ToString();
        //                txtweight.Text = newrow["weight_sqm"].ToString();
        //                TXTWIDTH.Text = newrow["width"].ToString();
        //                TXTElongation.Text = newrow["elongation"].ToString();
        //                TXTMODULUS.Text = newrow["modulus"].ToString();
        //                txtRecovStretbility.Text = "";
        //                txtDimStbilityShr.Text = "";
        //                txtremark.Text = newrow["remark"].ToString();
        //                txtweightTolerance.Text = newrow["weight_sqm_tolerance"].ToString();
        //                txtwidth_Tolerance.Text = newrow["width_tolerance"].ToString();
        //                txtpilingremark.Text = newrow["pilling_remark"].ToString();
        //                txtsnaggingremark.Text = newrow["snagging_remark"].ToString();
        //                txtdyeincolor.Text = newrow["color_name"].ToString();
        //                txtMachineTense.Text = newrow["machine_dense"].ToString();
        //                // if (row["knitting_appointment"] != null || row["knitting_appointment"].ToString() != "{}")
        //                if (!string.IsNullOrEmpty(newrow["knitting_appointment"].ToString()))
        //                {

        //                    DateTime dt1 = Convert.ToDateTime(newrow["knitting_appointment"].ToString());
        //                    txtknittingappoinment.Value = String.Format("{0:dd-MM-yyyy}", dt1);// row["knitting_appointment"].ToString();
        //                }
        //                if (!string.IsNullOrEmpty(newrow["yarn_available_date"].ToString()))
        //                {
        //                    DateTime dt2 = Convert.ToDateTime(newrow["yarn_available_date"].ToString());
        //                    txtyarnavailabledate.Value = String.Format("{0:dd-MM-yyyy}", dt2);// row["yarn_available_date"].ToString();
        //                }
        //                if (!string.IsNullOrEmpty(newrow["expected_finished_date"].ToString()))
        //                {
        //                    DateTime dt3 = Convert.ToDateTime(newrow["expected_finished_date"].ToString());
        //                    txtExpectedFinisheddate.Value = String.Format("{0:dd-MM-yyyy}", dt3);//row["expected_finished_date"].ToString();
        //                }
        //                if (!string.IsNullOrEmpty(newrow["spec_master_date"].ToString()))
        //                {
        //                    DateTime dt4 = Convert.ToDateTime(newrow["spec_master_date"].ToString());
        //                    txtspecmasterdate.Value = String.Format("{0:dd-MM-yyyy}", dt4);//row["spec_master_date"].ToString();
        //                }
        //                if (!string.IsNullOrEmpty(newrow["qa_report_date"].ToString()))
        //                {
        //                    DateTime dt5 = Convert.ToDateTime(newrow["qa_report_date"].ToString());
        //                    txtqa_reportdate.Value = String.Format("{0:dd-MM-yyyy}", dt5);//= row["qa_report_date"].ToString();
        //                }

        //                txtStandard_Customer.Text = newrow["test_method_text"].ToString();
        //                txtCustPriceExpectation.Text = newrow["expected_price"].ToString();
        //                txtPercent1.Text = newrow["composition1_percent"].ToString();
        //                txtPercent2.Text = newrow["composition2_percent"].ToString();
        //                txtPercent3.Text = newrow["composition3_percent"].ToString();
        //                txtPercent4.Text = newrow["composition4_percent"].ToString();
        //                txtPercent5.Text = newrow["composition5_percent"].ToString();
        //                txtPercent6.Text = newrow["composition6_percent"].ToString();

        //                TXTGUAGE.Text = newrow["gauge"].ToString();
        //                if (!string.IsNullOrEmpty(newrow["yarn_face1"].ToString()))
        //                {
        //                    ddlFront.SelectedValue = newrow["yarn_face1"].ToString().Trim();
        //                }
        //                if (!string.IsNullOrEmpty(newrow["yarn_face2"].ToString()))
        //                {
        //                    ddlBack.SelectedValue = newrow["yarn_face2"].ToString().Trim();
        //                }
        //                if (!string.IsNullOrEmpty(newrow["yarn_face3"].ToString()))
        //                {


        //                    ddlfront1.SelectedValue = newrow["yarn_face3"].ToString().Trim();
        //                }
        //                if (!string.IsNullOrEmpty(newrow["yarn_face4"].ToString()))
        //                {
        //                    ddlfront2.SelectedValue = newrow["yarn_face4"].ToString().Trim();
        //                }
        //                if (!string.IsNullOrEmpty(newrow["yarn_face5"].ToString()))
        //                {
        //                    ddlfront5.SelectedValue = newrow["yarn_face5"].ToString().Trim();
        //                }
        //                if (!string.IsNullOrEmpty(newrow["yarn_face6"].ToString()))
        //                {
        //                    ddlfront6.SelectedValue = newrow["yarn_face6"].ToString().Trim();
        //                }

        //                txtAvail1.Text = newrow["yarn_avail_qty1"].ToString();
        //                txtAvail2.Text = newrow["yarn_avail_qty2"].ToString();
        //                txtAvail3.Text = newrow["yarn_avail_qty3"].ToString();
        //                txtAvail4.Text = newrow["yarn_avail_qty4"].ToString();
        //                txtAvail5.Text = newrow["yarn_avail_qty5"].ToString();
        //                txtAvail6.Text = newrow["yarn_avail_qty6"].ToString();

        //                if (!string.IsNullOrEmpty(newrow["dye_finishing_formula_id"].ToString()))
        //                {
        //                    ddlFinishing.SelectedValue = newrow["dye_finishing_formula_id"].ToString().Trim();
        //                }
        //                if (!string.IsNullOrEmpty(newrow["require_tag"].ToString()))
        //                {
        //                    string gangtagrequired = newrow["require_tag"].ToString();
        //                    if (gangtagrequired == "N" || gangtagrequired == "")
        //                    {
        //                        chkHangTagRequire.Checked = false;
        //                    }
        //                    else
        //                    {
        //                        chkHangTagRequire.Checked = true;
        //                    }
        //                }



        //                TXTYARDAGE.Text = newrow["yardage"].ToString();
        //                string snagging = newrow["snagging"].ToString();
        //                if (snagging == "N")
        //                {
        //                    chksnagging.Checked = false;
        //                }
        //                else
        //                {
        //                    chksnagging.Checked = true;
        //                }
        //                string pilling = newrow["piling"].ToString();
        //                if (pilling == "N")
        //                {
        //                    chkpilling.Checked = false;
        //                }
        //                else
        //                {
        //                    chkpilling.Checked = true;
        //                }

        //                string standard_aatcc = newrow["standard_aatcc"].ToString();
        //                if (standard_aatcc == "N" || standard_aatcc == "0")
        //                {
        //                    CHKAATCC.Checked = false;
        //                }
        //                else
        //                {
        //                    CHKAATCC.Checked = true;
        //                }
        //                string standard_astm = newrow["standard_astm"].ToString();
        //                if (standard_astm == "N" || standard_astm == "0")
        //                {
        //                    CHKASTM.Checked = false;
        //                }
        //                else
        //                {
        //                    CHKASTM.Checked = true;
        //                }

        //                string standard_iso = newrow["standard_iso"].ToString();
        //                if (standard_iso == "N" || standard_iso == "0")
        //                {
        //                    CHKISO.Checked = false;
        //                }
        //                else
        //                {
        //                    CHKISO.Checked = true;
        //                }
        //                string standard_jis = newrow["standard_jis"].ToString();
        //                if (standard_jis == "N" || standard_jis == "0")
        //                {
        //                    CHKJIS.Checked = false;
        //                }
        //                else
        //                {
        //                    CHKJIS.Checked = true;
        //                }
        //                string standard_ms = newrow["standard_ms"].ToString();
        //                if (standard_ms == "N" || standard_ms == "0")
        //                {
        //                    chkMS.Checked = false;
        //                }
        //                else
        //                {
        //                    chkMS.Checked = true;
        //                }
        //                string standard_hm = newrow["standard_hm"].ToString();
        //                if (standard_hm == "N" || standard_hm == "0")
        //                {
        //                    chkHM.Checked = false;
        //                }
        //                else
        //                {
        //                    chkHM.Checked = true;
        //                }
        //                string standard_vss = newrow["standard_vss"].ToString();
        //                if (standard_vss == "N" || standard_vss == "0")
        //                {
        //                    chkVSS.Checked = false;
        //                }
        //                else
        //                {
        //                    chkVSS.Checked = true;
        //                }

        //                txtElongation_l.Text = newrow["elongation_l"].ToString();
        //                txtmodulus_l.Text = newrow["modulus_l"].ToString();
        //                txtstretch_l.Text = newrow["stretch_l"].ToString();
        //                txtshrinkage_l.Text = newrow["shrinkage_l"].ToString();
        //                txtElongation_w.Text = newrow["elongation_w"].ToString();
        //                txtmodulus_w.Text = newrow["modulus_w"].ToString();
        //                txtstretch_w.Text = newrow["stretch_w"].ToString();
        //                txtshrinkage_w.Text = newrow["shrinkage_w"].ToString();
        //                txtelongation_tolerance.Text = newrow["elongation_tolerance"].ToString();
        //                txtmodulus_tolerance.Text = newrow["modulus_tolerance"].ToString();
        //                txtshrinkage_tolerance.Text = newrow["shrinkage_tolerance"].ToString();
        //                txtstretch_tolerance.Text = newrow["stretch_tolerance"].ToString();
        //                ddlReasonForDevl.SelectedValue = newrow["development_type_id"].ToString();
        //                //  ddlPurposeofGarment.SelectedValue = newrow["product_application_id"].ToString();
        //                if (!string.IsNullOrEmpty(newrow["expected_date"].ToString()))
        //                {
        //                    DateTime dt6 = Convert.ToDateTime(newrow["expected_date"].ToString());
        //                    dtTo.Value = String.Format("{0:dd-MM-yyyy}", dt6);//= row["qa_report_date"].ToString();
        //                                                                      //dtTo.Value = row["expected_date"].ToString();
        //                }

        //                txtSampleRequire.Text = newrow["hanger"].ToString();
        //                txtStandard_Customer.Text = newrow["standard_customer"].ToString();
        //                //txtComment.Text = 
        //                string yarnavailable = newrow["yarn_available"].ToString();
        //                if (yarnavailable == "N")
        //                {
        //                    chkYarnAvailable.Checked = false;
        //                }
        //                else
        //                {
        //                    chkYarnAvailable.Checked = true;
        //                }

        //                txtweightTolerance.Text = newrow["weight_sqm_tolerance"].ToString();
        //                txtwidth_Tolerance.Text = newrow["width_tolerance"].ToString();
        //                lblFileName.Text = newrow["file_location"].ToString();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write(ex.Message.ToString());
        //    }
        //}
    }
}
