<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Print_DEvelop_Request.aspx.cs" Inherits="PDR.Reports.Print_DEvelop_Request" %>
<%@ Register Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" 
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb"  %>

<html >
<head runat="server">
     <style>html,body,form {height:100%;width:80%}
         .auto-style1 {
             width: 80%;
             height: 100%;
         }
     </style>
    <title></title>
</head>
<body>
    <div style="Width:auto;"> 
    <form id="form1" runat="server" class="auto-style1">
        <asp:Button ID="btnBack" runat="server" Text="Back " OnClick="btnBack_Click" Visible="false" />
        <input id="Button1" type="button" value="Print Report" onclick="PrintReport();" hidden="hidden"/><asp:FileUpload ID="FileUpload1" runat="server" Visible="true" />
&nbsp;<div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="true" >
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="reportDevelopRequest" runat="server"
             AsyncRendering="False"  EnableEventValidation="false" OnDataBinding="Page_Load" Font-Names="Arial,sans-serif" Font-Size="8pt" Font-Bold="False"
            WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="141%"   Height="100%"  >
            <LocalReport ReportPath="RDLC/rpt_Develop_Request.rdlc" >
                <DataSources>
                    <rsweb:ReportDataSource  DataSourceId="DS_Develop_Request" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>

    </div>
    </form>
        <script language="javascript"> 
         function PrintReport() { 
             var viewerReference = $find("reportDevelopRequest");

             var stillonLoadState = viewerReference.get_isLoading();

             if (!stillonLoadState ) { 
                 var reportArea = viewerReference .get_reportAreaContentType(); 
                 if (reportArea == Microsoft.Reporting.WebFormsClient.ReportAreaContent.ReportPage) { 
                     $find("reportDevelopRequest").invokePrintDialog(); 
                 } 
             } 
         } 
     </script>
    </div>
</body>
</html>
