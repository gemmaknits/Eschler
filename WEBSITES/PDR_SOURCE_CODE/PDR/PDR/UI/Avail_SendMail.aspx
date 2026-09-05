<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Avail_SendMail.aspx.cs" Inherits="PDR.UI.Avail_SendMail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title></title>
    <link href="../PDR_StyleSheet.css" rel="stylesheet" />
</head>

<script type="text/javascript">
    function ShowProgressBar() {
        document.getElementById('dvProgressBar').style.visibility = 'visible';
    }

    function HideProgressBar() {
        document.getElementById('dvProgressBar').style.visibility = "hidden";
    }
</script>

<body>

    <form id="frmAvailSendMail" runat="server">
        <div style="left: 160px; top: 50px; position: absolute; width: 300px">
            <h4>SHORTAGE MAIL</h4>
        </div>
      
        <div style="left: 100px; top: 100px; position: absolute; width: 500px">

            <table>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="TO" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtTo" runat="server" CssClass="TextBox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text="CC" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtCC" runat="server" CssClass="TextBox" Width="250px" onkeydown="(event.keyCode!=13);"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label4" runat="server" Text="SUBJECT" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtSubject" runat="server" CssClass="TextBox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label5" runat="server" Text="BODY" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBody" runat="server" CssClass="TextBox" Width="650px" TextMode="MultiLine" Rows="10" Height="400px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="btnSendMail" runat="server" Text="Send Mail" CssClass="button" OnClick="btnSendMail_Click" OnClientClick="javascript:ShowProgressBar()" /></td>
                </tr>
            </table>
        </div>
          <div id="dvProgressBar" style="visibility: hidden; left: 100px">
            <img src="~/Images/progress_bar.gif" runat="server" style="left: 250px; position: absolute; top: 530px;" />
        </div>
        <div style="top: 611px; left: 290px; position: absolute;">
            <strong>
                <asp:Label ID="lblmsg" runat="server" Visible="False" ForeColor="#009933"></asp:Label>
            </strong>
        </div>
    </form>

</body>
</html>
