<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintTrackingList.aspx.cs" Inherits="PDR.Reports.PrintTrackingList" %>

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
    <form id="frmtrackingList" runat="server" class="auto-style1">
        
        <asp:Image ID="Image1" runat="server" />
        
&nbsp;<div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="true" >
        </asp:ScriptManager>
            
            <asp:Button ID="btnBack" runat="server" OnClick="btnBack_Click" Text="Back" />
            <asp:Label ID="lblmsg" runat="server" Text="There is no record to display" Visible="false"></asp:Label>     
        <rsweb:ReportViewer ID="reportTrackingSheet" runat="server"
             AsyncRendering="False"  EnableEventValidation="false" OnDataBinding="Page_Load" Font-Names="Arial,sans-serif" Font-Size="8pt" Font-Bold="False"
            WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="141%"   Height="100%"  >
            <LocalReport ReportPath="RDLC/rpt_TrackingSheet.rdlc" >
                <DataSources>
                    <rsweb:ReportDataSource  DataSourceId="DS_TrackingList" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>

    </div>
    </form>
      
    </div>
</body>
</html>

