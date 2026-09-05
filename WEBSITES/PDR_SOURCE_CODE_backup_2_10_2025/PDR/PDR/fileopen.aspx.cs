using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;

namespace PDR.UI
{
    public partial class fileopen : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //System.IO.FileStream fs = new System.IO.FileStream("//172.16.3.4/pdr_files/analyze/test.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read);
            //byte[] ar = new byte[(int)fs.Length];
            //fs.Read(ar, 0, (int)fs.Length);
            //fs.Close();

            //Response.AddHeader("content-disposition", "attachment;filename=" + "test.txt");
            //Response.ContentType = "application/octectstream";
            //Response.BinaryWrite(ar);
            //Response.End();
            //System.Diagnostics.Process.Start(@"//172.16.3.4/pdr_files/analyze/test.txt");
            string uploadedfilepath = Session["uploadedfilename"].ToString();
            string path = Server.MapPath(uploadedfilepath);
            WebClient client = new WebClient();
            Byte[] buffer = client.DownloadData(uploadedfilepath);
            if (buffer != null)
            {
                //Response.Write("<script>");
                //Response.Write("ContentType='image/jpg");
                //Response.Write("window.open('" + uploadedfilepath + "','_blank', ' fullscreen=yes')");
                ////Response.Write("window.open(" + path + ",'_blank')");
                //Response.Write("</script>");
                Response.ContentType = "image/jpg";
                Response.AddHeader("content-length", buffer.Length.ToString());
                Response.BinaryWrite(buffer);
                Response.Flush();



                Response.End();
            }
        }
    }
}