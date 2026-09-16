using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using PDR.BLL;
using Microsoft.Reporting.WebForms;
using System.IO;
namespace PDR.Reports
{
    public partial class PrintTrackingList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                Response.Redirect("~/ui/login.aspx?pagename=PrintTrackingList");
            }
            if (!IsPostBack)
            {
                DataSet trackingList_DS;
                string pdrfromdate = string.Empty;
                string pdrtodate = string.Empty;
                string categoryid ="";
                string subcategoryid = "";
                string groupid = string.Empty;
                string subgroupid = string.Empty;
                string applicationid = string.Empty;
                string subapplicationid = string.Empty;
                string categoryname = "";
                string subcategoryname = "";
                string groupname = string.Empty;
                string subgroupname = string.Empty;
                string applicationname = string.Empty;
                string subapplicationname = string.Empty;
                string preparedby = string.Empty;
                string requestedby = string.Empty;
                string p_customer = string.Empty;
                string pdrno = string.Empty;
                string p_show_closed_pdr;
                string p_internal_app_rej_date_from;
                string p_internal_app_rej_date_to;
                string p_customer_app_rej_date_from;
                string p_customer_app_rej_date_to;
                string p_final_app_rej_date_from;
                string p_final_app_rej_date_to;
                string p_wait_internal_app_rej;
                string p_wait_customer_app_rej;
                string p_wait_final_app_rej;
                string p_add_to_collection;
                string p_new_yarn;
                string p_yarn_shortage;
                string p_no_order;
                string p_internal_app_rej_Text;
                string p_customer_app_rej_Text;
                string p_final_app_rej_Text;
                string p_knitting_app_rej_Text;
                string p_internal_app_rej_id;
                string p_customer_app_rej_id;
                string p_final_app_rej_id;
                string p_knitting_app_rej_id;
                pdrfromdate = Session["pdrfromdate"].ToString();
                pdrtodate = Session["pdrtodate"].ToString();
                categoryid = Session["categoryid"].ToString();
                subcategoryid = Session["subcategoryid"].ToString();
                groupid = Session["groupid"].ToString();
                subgroupid = Session["subgroupid"].ToString();
                applicationid = Session["applicationid"].ToString();
                subapplicationid = Session["subapplicationid"].ToString();
                categoryname = Session["categoryname"].ToString();
                subcategoryname = Session["subcategoryname"].ToString();
                groupname = Session["groupname"].ToString();
                subgroupname = Session["subgroupname"].ToString();
                applicationname = Session["applicationname"].ToString();
                subapplicationname = Session["subapplicationname"].ToString();
                preparedby = Session["preparedby"].ToString();
                requestedby = Session["requestedby"].ToString();
                p_customer = Session["p_customer"].ToString();
                pdrno = Session["pdrno"].ToString();
                p_internal_app_rej_date_from = Session["p_internal_app_rej_date_from"].ToString();
                p_internal_app_rej_date_to = Session["p_internal_app_rej_date_to"].ToString();
                p_customer_app_rej_date_from = Session["p_customer_app_rej_date_from"].ToString();
                p_customer_app_rej_date_to = Session["p_customer_app_rej_date_to"].ToString();
                p_final_app_rej_date_from = Session["p_final_app_rej_date_from"].ToString();
                p_final_app_rej_date_to = Session["p_final_app_rej_date_to"].ToString();
                p_wait_internal_app_rej = Session["p_wait_internal_app_rej"].ToString();
                p_wait_customer_app_rej = Session["p_wait_customer_app_rej"].ToString();
                p_wait_final_app_rej = Session["p_wait_final_app_rej"].ToString();
                p_add_to_collection = Session["p_add_to_collection"].ToString();
                p_show_closed_pdr = Session["p_show_closed_pdr"].ToString();
                p_new_yarn = Session["p_new_yarn"].ToString();
                p_yarn_shortage= Session["p_yarn_shortage"].ToString();
                p_no_order = Session["p_no_order"].ToString();
                p_internal_app_rej_Text = Session["internal_app_rej_Text"].ToString();
                p_customer_app_rej_Text = Session["customer_app_rej_Text"].ToString();
                p_final_app_rej_Text = Session["final_app_rej_Text"].ToString();
                p_knitting_app_rej_Text = Session["knitting_app_rej_Text"].ToString();

                p_internal_app_rej_id = Session["internal_app_rej_id"].ToString();
                p_customer_app_rej_id = Session["customer_app_rej_id"].ToString();
                p_final_app_rej_id = Session["final_app_rej_id"].ToString();
                p_knitting_app_rej_id = Session["knitting_app_rej_id"].ToString();
                string thisConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
                classTrackingSheet_BLL obj_trackinglist_BLL = new classTrackingSheet_BLL();
                trackingList_DS = obj_trackinglist_BLL.Load_Print_PDRList(pdrfromdate, pdrtodate,categoryid.Trim(), subcategoryid.Trim(), groupid.Trim(),subgroupid, applicationid.Trim(),
                                subapplicationid.Trim(), "", "", pdrno.Trim(), requestedby.Trim(), p_customer, preparedby, p_show_closed_pdr,
                                 p_internal_app_rej_date_from, p_internal_app_rej_date_to, p_customer_app_rej_date_from, p_customer_app_rej_date_to,
                                 p_final_app_rej_date_from, p_final_app_rej_date_to, p_wait_internal_app_rej, p_wait_customer_app_rej, p_wait_final_app_rej,
                                 p_add_to_collection,p_new_yarn,p_yarn_shortage, p_no_order, p_internal_app_rej_id,
                                 p_customer_app_rej_id, p_final_app_rej_id, p_knitting_app_rej_id);
                reportTrackingSheet.Visible = true;
                reportTrackingSheet.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                reportTrackingSheet.LocalReport.ReportEmbeddedResource = "RDLC/rpt_TrackingSheet.rdlc";
                reportTrackingSheet.LocalReport.ReportPath = "RDLC/rpt_TrackingSheet.rdlc";
                reportTrackingSheet.Reset();
                reportTrackingSheet.LocalReport.DataSources.Clear();


                reportTrackingSheet.LocalReport.ReportPath = Server.MapPath(@"~/RDLC/rpt_TrackingSheet.rdlc");

                ReportParameter[] parameters = new ReportParameter[30];
                DataTable dt_Result = (DataTable)trackingList_DS.Tables[0];
                foreach (DataRow resultrow in dt_Result.Rows)
                {
                    groupid = resultrow[0].ToString();
                    parameters[0] = new ReportParameter("pdrno", pdrno.Trim());
                    if (categoryid != "")
                    { parameters[1] = new ReportParameter("cat", categoryname); }
                    else
                    {
                        parameters[1] = new ReportParameter("cat", "");
                    }
                    if (subcategoryid != "")
                    {
                        parameters[2] = new ReportParameter("subcat", subcategoryname);
                    }
                    else
                    {
                        parameters[2] = new ReportParameter("subcat", "");
                    }
                    parameters[3] = new ReportParameter("cust", p_customer);
                    parameters[4] = new ReportParameter("fabgroup", groupname.Trim());
                    parameters[5] = new ReportParameter("fabsubgroup", subgroupname);
                    parameters[6] = new ReportParameter("app", applicationname);
                    parameters[7] = new ReportParameter("subapp", subapplicationname);
                    parameters[8] = new ReportParameter("pdrdtfrom", pdrfromdate.Trim());
                    parameters[9] = new ReportParameter("pdrdtto", pdrtodate);
                    parameters[10] = new ReportParameter("intappdtfrom", p_internal_app_rej_date_from);
                    parameters[11] = new ReportParameter("intappdtto", p_internal_app_rej_date_to);
                    parameters[12] = new ReportParameter("custappdtfrom", p_customer_app_rej_date_from);
                    parameters[13] = new ReportParameter("custappdtto", p_customer_app_rej_date_to.Trim());
                    parameters[14] = new ReportParameter("finappdtfrom", p_final_app_rej_date_from);
                    parameters[15] = new ReportParameter("finappdtto", p_final_app_rej_date_to);
                    parameters[16] = new ReportParameter("reqby", requestedby);
                    parameters[17] = new ReportParameter("prepby", preparedby);
                    parameters[18] = new ReportParameter("showclosedpdr", p_show_closed_pdr.Trim());
                    parameters[19] = new ReportParameter("addtocollection", p_add_to_collection);
                    parameters[20] = new ReportParameter("waitintapp", p_wait_internal_app_rej);
                    parameters[21] = new ReportParameter("waitcustapp", p_wait_customer_app_rej);
                    parameters[22] = new ReportParameter("waitfinapp", p_wait_final_app_rej);
                    parameters[23] = new ReportParameter("newyarn", p_new_yarn);
                    parameters[24] = new ReportParameter("yarnshortage", p_yarn_shortage);
                    parameters[25] = new ReportParameter("drwaitso", p_no_order);
                    parameters[26] = new ReportParameter("internalapprej", p_internal_app_rej_Text);
                    parameters[27] = new ReportParameter("customerapprej", p_customer_app_rej_Text);
                    parameters[28] = new ReportParameter("finalapprej", p_final_app_rej_Text);
                    parameters[29] = new ReportParameter("knittingapprej", p_knitting_app_rej_Text);
                }
                //if (trackingList_DS.Tables[0].Rows.Count > 0)
                if (trackingList_DS != null && trackingList_DS.Tables.Count > 0 && trackingList_DS.Tables[0].Rows.Count > 0)
                 {
                    lblmsg.Visible = false;
                    this.reportTrackingSheet.LocalReport.SetParameters(parameters);
                    ReportDataSource datasource_trackinglist = new ReportDataSource("DS_TrackingList", trackingList_DS.Tables[0]);
                    reportTrackingSheet.LocalReport.DataSources.Clear();
                    reportTrackingSheet.LocalReport.DataSources.Add(datasource_trackinglist);
                    reportTrackingSheet.LocalReport.EnableExternalImages = true;
                    reportTrackingSheet.LocalReport.Refresh();

                    //// download the report to pdf
                    //// to download reort
                    //// Variables
                    //Warning[] warnings;
                    //string[] streamIds;
                    //string mimeType = string.Empty;
                    //string encoding = string.Empty;
                    //string extension = string.Empty;
                    ////string rptoption = Session["rptoption"].ToString();

                    //// Setup the report viewer object and get the array of bytes
                    //ReportViewer viewer = new ReportViewer();
                    //// viewer.ProcessingMode = ProcessingMode.Local;
                    //this.reportTrackingSheet.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                    //viewer.LocalReport.ReportPath = "rpt_TrackingSheet.rdlc";
                    //viewer.LocalReport.DataSources.Add(datasource_trackinglist); // Add datasource here


                    //byte[] bytes = reportTrackingSheet.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);


                    //// Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
                    //Response.Buffer = true;
                    //Response.Clear();
                    //Response.ContentType = mimeType;
                    //Response.AddHeader("content-disposition", "attachment; filename=PDR" + pdrno + "." + extension);
                    ////  Response.BinaryWrite(bytes); // create the file

                    //Response.OutputStream.Write(bytes, 0, bytes.Length);
                    //// done

                    // download to PDF
                    // to download reort
                    // Variables
                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;
                    string rptoption = "";

                    // Setup the report viewer object and get the array of bytes
                    ReportViewer viewer = new ReportViewer();
                    // viewer.ProcessingMode = ProcessingMode.Local;
                    this.reportTrackingSheet.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                    viewer.LocalReport.ReportPath = "rpt_TrackingSheet.rdlc";
                    viewer.LocalReport.DataSources.Add(datasource_trackinglist); // Add datasource here


                    byte[] bytes = reportTrackingSheet.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);


                    // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
                    Response.Buffer = true;
                    Response.Clear();
                    //if (rptoption == "print")
                    //{
                    Response.ContentType = mimeType;
                    Response.AddHeader("content-disposition", "attachment; filename=TrackingList." + extension);
                    //  Response.BinaryWrite(bytes); // create the file

                    Response.OutputStream.Write(bytes, 0, bytes.Length);

                }
                else
                {
                    reportTrackingSheet.Visible = false;
                    lblmsg.Visible = true;
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UI/pdr_list.aspx");
        }
    }
}