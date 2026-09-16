<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendMail_InternalApproval.aspx.cs" Inherits="PDR.UI.SendMail_InternalApproval" %>

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
    <form id="frmSendMailKnittingApproval" runat="server">
        <div style="left: 50px; top: 50px; position: absolute; width: 300px">
            <h4>INTERNAL APPROVAL MAIL</h4>

        </div>
        <div style="left: 50px; top: 100px; position: absolute; width: 700px">

            <table>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="TO" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtTo" runat="server" CssClass="TextBox" Width="600px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text="CC" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtCC" runat="server" CssClass="TextBox" Width="600px" onkeydown="(event.keyCode!=13);"></asp:TextBox>
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
                        <asp:Button ID="btnSendMail" runat="server" Text="Send Mail" CssClass="button" OnClientClick="javascript:ShowProgressBar()" OnClick="btnSendMail_Click" /></td>
                </tr>
            </table>
        </div>
        <div id="dvProgressBar" style="visibility: hidden; left: 100px">
            <img src="~/Images/progress_bar.gif" runat="server" style="left: 200px; position: absolute; top: 530px;" />
        </div>
        <div style="top: 611px; left: 211px; position: absolute;">
            <strong>
                <asp:Label ID="lblmsg" runat="server" Visible="False" ForeColor="#009933"></asp:Label>
            </strong>
        </div>
           <div  style="top: 643px; left: 55px; position: absolute;">
              <asp:GridView ID="grdAttachmentDetails" runat="server" AutoGenerateColumns="false" GridLines="None"
                   DataKeyNames="doc_attachments_id,source_doc_number,file_location" OnRowCommand="OnRowCommand"
                    Width="32%">
                    <HeaderStyle CssClass="grdheader" />
                    <Columns>
                         <asp:TemplateField ItemStyle-Width="2%" HeaderText="" HeaderStyle-CssClass="col" ItemStyle-CssClass="col" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                                <ItemTemplate>
                                        <asp:CheckBox ID="chkdelete" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        <asp:BoundField ItemStyle-Width="80px" DataField="file_Description" HeaderText="DESCRIPTION" ItemStyle-Wrap="false" ItemStyle-CssClass="col" HeaderStyle-CssClass="col">
                            <ItemStyle CssClass="col" />
                        </asp:BoundField>
                        <asp:BoundField DataField="file_location_disp" HeaderText="LOCATION" ReadOnly="True" HeaderStyle-CssClass="col">
                            <ItemStyle CssClass="col" Width="20px" />
                        </asp:BoundField>
                         <asp:TemplateField>
                                <HeaderStyle Width="20" />
                                <ItemTemplate>
                                    <asp:ImageButton ID="img" ImageUrl="~/Images/downloadfile.png" ControlStyle-Width="14px" ControlStyle-Height="14px" CommandName="Download"
                                        CommandArgument='<%#((GridViewRow)Container).RowIndex%>' ToolTip="Download" runat="server" Height="14" Width="14" />
                                </ItemTemplate>
                            </asp:TemplateField>
                    </Columns>
                </asp:GridView>
        </div>
    </form>
</body>
</html>

