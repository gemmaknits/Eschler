<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="PDR.UI.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:pocConnectionString %>" SelectCommand="p_combo_design_appl" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
    
        <asp:CheckBoxList ID="CheckBoxList1" runat="server" DataSourceID="SqlDataSource1" DataTextField="lookup_value" DataValueField="lookup_value_id">
        </asp:CheckBoxList>
    
    </div>
    </form>
</body>
</html>
