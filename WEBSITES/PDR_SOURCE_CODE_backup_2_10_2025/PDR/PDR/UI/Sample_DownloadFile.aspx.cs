using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace PDR.UI
{
    public partial class Sample_DownloadFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usersessionid"] == null)
            {
                Response.Redirect("~/ui/login.aspx?pagename=Sample_DownloadFile");
            }
            if (!IsPostBack)
            {
                string file_lcoation = Request.QueryString["file_location"];
                string attachmentid = Request.QueryString["attachmentid"];
                string filepath = file_lcoation;//

                if (filepath != "")
                {
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
                    Response.AddHeader("content-disposition", "attachment;filename=" + attachmentid  + filename);
                    Response.ContentType = "application/octectstream";
                    Response.BinaryWrite(ar);
                    Response.Write("<script language='javascript'> { window.close(); }</script>");
                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "close", "window.close();", true);
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myCloseScript", "window.close()", true);
                    //string script = "window.opener = 'Self';window.open('','_parent',''); window.close();";
                    //ScriptManager.RegisterStartupScript(Page, typeof(string), "Close Window", script, true);
                    Response.End();
                    System.Web.HttpContext.Current.Response.Write("<script>self.close();</script>");
                    closeButton.Attributes.Add("onclick", "JavaScript:window.close(); return false;");
                    string script1 = "window.opener = 'Self';window.open('','_parent',''); window.close();";
                    ScriptManager.RegisterStartupScript(Page, typeof(string), "Close Window", script1, true);
                    ClientScript.RegisterStartupScript(typeof(Page), "closePage", "window.close();", true);
                }
            }
        }
    }
}