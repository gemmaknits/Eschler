<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PDR_Approval.aspx.cs" Inherits="PDR.UI.PDR_Approval" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hid_pdr_new_develop_req_id" runat="server" />
    <asp:HiddenField ID="hid_approvalby" runat="server" />
     <div style="left:72px; top:54px; width:400px;position:absolute" >
        <Table ID="Table1" runat="server">
        <tr>
             <td> <strong> <asp:Label ID="Label1" runat="server" Text="PDR APPROVAL"></asp:Label></strong></td>
        </tr>
        </Table>
        </div>
     <div style="left:86px; top:90px; width:500px;position:absolute" >
                <table style="line-height:28px;">
                    <tr style="line-height:28px;">
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="PDR NO" CssClass="clsFromLabel"></asp:Label></td>
                        <td>
                            <asp:TextBox ID="txtPDRNO" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);" ReadOnly="True"></asp:TextBox></td>
                    </tr>
                    <tr style="line-height:28px;">
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="DATE" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td >
                            
                            <asp:TextBox ID="txtDate" runat="server" CssClass="TextBox" ReadOnly="True"></asp:TextBox>
                            
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="PRODUCT" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td>
                            
                            <asp:TextBox ID="txtProduct" runat="server" CssClass="TextBox" ReadOnly="True" Width="350px"></asp:TextBox>
                        </td>
                    </tr>
                     <tr style="line-height:28px;">
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="CUSTOMER" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCustomer" runat="server" CssClass="TextBox"  Width="350px" ReadOnly="True"></asp:TextBox>
                         </td>
                    </tr>
                    <tr style="line-height:28px;">
                        <td style="padding-right:20px; height: 28px;">
                            <asp:Label ID="Label5" runat="server" Text="END BUYER" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td style="height: 28px">
                            <asp:TextBox ID="txtEndBuyer" runat="server" CssClass="TextBox" ReadOnly="True"></asp:TextBox>
                        </td>
                    </tr>
                   
                    <tr style="line-height:28px;">
                        <td style="padding-right:20px; height: 28px;">
                            <asp:Label ID="Label8" runat="server" Text="COMMENT" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td style="height: 28px">
                            <asp:TextBox ID="txtComment" runat="server" CssClass="TextBox" ></asp:TextBox>
                        </td>
                    </tr>
                     <tr style="line-height:28px;">
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="APP / REJ" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlApproval" runat="server" CssClass="combo" Width="100px"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>

                        </td>
                        <td>
                            <asp:Button ID="BTNDONE" runat="server" Text="DONE" CssClass="button" OnClick="BTNDONE_Click" />
                        </td>
                    </tr>
                       
                </table>
        </div>
        <div style="top:350px;left:186px;position:absolute">
            <asp:Label ID="lblmsg" runat="server" Text=""></asp:Label>
        </div>
        <div style="top:400px;left:86px;position:absolute">
            <asp:Label ID="Label6" runat="server" Text="FILE :"></asp:Label>&nbsp;&nbsp;
            <asp:Label ID="lblFileName" runat="server" Text="" ></asp:Label>&nbsp;&nbsp;
            <asp:Button ID="btndownload" runat="server" Text="DOWNLOAD" OnClick="btndownload_Click" CssClass="button" Width="100px" />
        </div>
</asp:Content>
