using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PDR.BLL;
using System.Data;
using System.IO;
namespace PDR.UI
{


    public partial class DocAttachmentView : System.Web.UI.Page
    {

        string new_develop_req_id = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                string product = Request.QueryString["product"];
                Response.Redirect("~/ui/login.aspx?pagename=DocAttachmentView" + "&product=" + product);
            }
            if (!IsPostBack)
            {
                string product = Request.QueryString["product"];
                txtSourceDocNo.Text = product;
                txtProduct.Text = product;
                classDevelopAttachment_BLL obj = new classDevelopAttachment_BLL();
                DataTable dt1 = obj.LoadProductAttachmentDetails(product);
                grdAttachmentDetails.DataSource = dt1;
                grdAttachmentDetails.DataBind();
            }
        }

        protected void ImgSave_Click(object sender, ImageClickEventArgs e)
        {
            classDevelopAttachment_BLL obj = new classDevelopAttachment_BLL();
            string p_source_doc_number = txtSourceDocNo.Text.Trim();
            string p_doc_attachments_id = string.Empty;
            string p_file_description = txtDescription.Text;
            string p_file_location = string.Empty;
            string fileName = string.Empty;
            if (uploadfiles1.PostedFile.FileName.Length > 0)
            {
                fileName = Path.GetFileName(uploadfiles1.PostedFile.FileName);
                string filename1 = Path.GetDirectoryName(uploadfiles1.PostedFile.FileName);
                HttpPostedFile file = Request.Files["browserHidden"];
                FileInfo fileInfo = new FileInfo(fileName);
                string directoryFullPath = fileInfo.DirectoryName;
                fileName = "//172.16.3.4/pdr_files/ANALYZE/" + fileName;
                p_file_location = fileName;
            }
            if (p_file_location != "")
            {
                DataTable dt = obj.UpdateDevelopDetails(p_doc_attachments_id, p_source_doc_number, p_file_description, p_file_location);
                fileName = Path.GetFileName(uploadfiles1.FileName);
                if (fileName != "")
                {
                    bool exists = System.IO.Directory.Exists(Server.MapPath("~/ANALYZE/"));

                    if (!exists)
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/ANALYZE/"));
                    uploadfiles1.SaveAs("//172.16.3.4/pdr_files/ANALYZE/" + fileName);
                    // lblMsg.Text = "File Uploaded Successfully";
                }
            }
            DataTable dtsave = new DataTable();
            dtsave.Columns.AddRange(new DataColumn[3] { new DataColumn("p_doc_attachments_id", typeof(string)),
                new DataColumn("p_source_doc_number", typeof(string)),
                new DataColumn("p_file_description",typeof(string))
                 });
            string source_doc_number = txtSourceDocNo.Text.Trim();
            foreach (GridViewRow attdetailrow in grdAttachmentDetails.Rows)
            {
                string file_description;
                string doc_attachment_id;
                TextBox col1 = attdetailrow.FindControl("FILE_DESCRIPTION") as TextBox;
                file_description = Request[col1.UniqueID] as string;
                doc_attachment_id = (grdAttachmentDetails.DataKeys[attdetailrow.RowIndex].Values[0].ToString().Trim());
                dtsave.Rows.Add(doc_attachment_id, source_doc_number, file_description);
            }
            DataTable dt_update = obj.UpdateAttachmentDescription(dtsave);
            string devid = Request.QueryString["devReqID"];
            DataTable dt1 = obj.LoadeDevelopAttachmentDetails(devid);
            grdAttachmentDetails.DataSource = dt1;
            grdAttachmentDetails.DataBind();
        }


        protected void OnRowDeleting(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                GridViewRow rowSelect = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                int rowindex = rowSelect.RowIndex;
                string doc_attachment_id = grdAttachmentDetails.DataKeys[rowindex].Values[0].ToString();
                if (doc_attachment_id != "")
                {
                    Int32 deleteresult;
                    if (Convert.ToInt32(doc_attachment_id) > 0)
                    {
                        classDevelopAttachment_BLL attachmentid = new classDevelopAttachment_BLL();
                        deleteresult = attachmentid.Delete_DocAttachmentID(doc_attachment_id.ToString().Trim());
                        new_develop_req_id = Request.QueryString["devReqID"];
                        classDevelopAttachment_BLL obj = new classDevelopAttachment_BLL();
                        DataTable dt1 = obj.LoadeDevelopAttachmentDetails(new_develop_req_id);
                        grdAttachmentDetails.DataSource = dt1;
                        grdAttachmentDetails.DataBind();
                    }
                }
            }
            if (e.CommandName == "Download")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string filepath = grdAttachmentDetails.DataKeys[index].Values[2].ToString();// Server.MapPath("~/Media/photo/prlq/ESTIMATE/");// lblFileName.Text;

                if (filepath != "")
                {


                    Session["uploadedfilename"] = lblFileName.Text.Trim();

                    string extension;
                    string filename = Path.GetFileName(filepath);
                    // filepath = filepath + filename;
                    extension = Path.GetExtension(filepath);
                    Page.ClientScript.RegisterStartupScript(
                  this.GetType(), "OpenWindow", "window.open('" + filepath + "','_newtab');", true);
                    System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    byte[] ar = new byte[(int)fs.Length];
                    fs.Read(ar, 0, (int)fs.Length);
                    fs.Close();

                    Response.AddHeader("content-disposition", "attachment;filename=" + txtSourceDocNo.Text.Trim() + filename);
                    Response.ContentType = "application/octectstream";
                    Response.BinaryWrite(ar);
                    Response.End();
                }
            }

        }


    }
}