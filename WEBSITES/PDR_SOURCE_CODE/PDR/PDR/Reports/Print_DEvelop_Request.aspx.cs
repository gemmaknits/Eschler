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
    public partial class Print_DEvelop_Request : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                Response.Redirect("~/ui/login.aspx?pagename=Print_DEvelop_Request");
            }
            if (!IsPostBack)
            {
                DataSet developrequest_DS;
                DataTable item_prop_DS;
                string pdrno;
                string pdr_new_develop_req_id;
                pdrno = Session["pdrno"].ToString();
                pdr_new_develop_req_id = Session["pdr_new_develop_req_id"].ToString();
                string thisConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
                classDevelop_Request_BLL obj_developrequest_BLL = new classDevelop_Request_BLL();
                developrequest_DS = obj_developrequest_BLL.Load_Print_PDRList(pdrno);
                item_prop_DS = obj_developrequest_BLL.LoadDeveRequestApp(pdr_new_develop_req_id);
                reportDevelopRequest.Visible = true;


                reportDevelopRequest.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                reportDevelopRequest.LocalReport.ReportEmbeddedResource = "RDLC/rpt_Develop_Request.rdlc";
                reportDevelopRequest.Reset();
                reportDevelopRequest.LocalReport.DataSources.Clear();


                reportDevelopRequest.LocalReport.ReportPath = Server.MapPath(@"~/RDLC/rpt_Develop_Request.rdlc");

                ReportParameter[] parameters = new ReportParameter[1];
                //  parameters[0] = new ReportParameter("fromdate", pdrno);
                // this.reportDevelopRequest.LocalReport.SetParameters(parameters);
                if (developrequest_DS.Tables[0].Rows.Count > 0)
                {
                    ReportDataSource datasource = new ReportDataSource("DS_Develop_Request", developrequest_DS.Tables[0]);
                    ReportDataSource item_prop_datasource = new ReportDataSource("DSItemProp", item_prop_DS);
                    reportDevelopRequest.LocalReport.DataSources.Clear();
                    reportDevelopRequest.LocalReport.DataSources.Add(datasource);
                    reportDevelopRequest.LocalReport.DataSources.Add(item_prop_datasource);
                    reportDevelopRequest.LocalReport.EnableExternalImages = true;
                    reportDevelopRequest.LocalReport.Refresh();

                    // to download reort
                    // Variables
                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;
                    string rptoption = Session["rptoption"].ToString();

                    // Setup the report viewer object and get the array of bytes
                    ReportViewer viewer = new ReportViewer();
                    // viewer.ProcessingMode = ProcessingMode.Local;
                    this.reportDevelopRequest.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                    viewer.LocalReport.ReportPath = "rpt_Develop_Request.rdlc";
                    viewer.LocalReport.DataSources.Add(datasource); // Add datasource here


                    byte[] bytes = reportDevelopRequest.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);


                    // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
                    Response.Buffer = true;
                    Response.Clear();
                    if (rptoption == "print")
                    {
                        Response.ContentType = mimeType;
                        Response.AddHeader("content-disposition", "attachment; filename=PDR" + pdrno + "." + extension);
                        //  Response.BinaryWrite(bytes); // create the file

                        Response.OutputStream.Write(bytes, 0, bytes.Length);
                    }
                    bool exists = System.IO.Directory.Exists(Server.MapPath("~/PDRDOC/"));

                    if (!exists)
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/PDRDOC/"));
                    //uploadfiles1.SaveAs("//172.16.3.4/pdr_files/ANALYZE/" + fileName);
                    //Response.WriteFile(Server.MapPath("//172.16.3.4/pdr_files/pdrdoc/" + pdrno + "." + extension));
                    Response.Flush(); // send it to the client to download
                                      //

                    // another method//
                    ////Response.ContentType = "Application/pdf";
                    ////Response.AppendHeader("Content-Disposition", "attachment; filename=" + pdrno + "." + extension);
                    ////Response.TransmitFile((@"c:\" + pdrno + "." + extension));
                    //Response.TransmitFile((@"c:\" + pdrno + "." + extension));
                    //Response.End();

                    byte[] Bytes = reportDevelopRequest.LocalReport.Render(format: "PDF", deviceInfo: "");
                    string savePath = @"c:\pdrdoc\";
                    FileStream fs = new FileStream(@"d:\PDR" + pdrno + "." + extension,
                                 FileMode.Create);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                    string fileName = @"d:\PDR" + pdrno + ".pdf";
                    if (fileName != "")
                    {
                        bool fileexists = System.IO.Directory.Exists(Server.MapPath("~/pdrdoc/"));

                        if (!fileexists)
                            System.IO.Directory.CreateDirectory(Server.MapPath("~/pdrdoc/"));
                        FileUpload1.SaveAs("//172.16.3.4/pdr_files/pdrdoc/" + fileName);
                        // lblMsg.Text = "File Uploaded Successfully";
                    }

                    System.IO.File.Copy(fileName, "//172.16.3.4/pdr_files/pdrdoc/PDR" + pdrno + ".pdf", true);
                    //using (FileStream stream = new FileStream("2001.pdf", FileMode.Create))
                    //{
                    //    stream.Write(Bytes, 0, Bytes.Length);
                    //}


                    //
                }

            }

        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UI/Develop_Request.aspx");
        }
    }
}