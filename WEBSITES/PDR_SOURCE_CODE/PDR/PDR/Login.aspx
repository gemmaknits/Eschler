<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PDR.UI.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="../POC_StyleSheet.css" type="text/css" rel="stylesheet"/>
</head>

<body>
      
    <form id="login" runat="server">
    <div>
         <table runat="server" style="position:absolute;left:295px; top:184px;background-color:#99CC99;line-height:15px; height: 146px;">
        <tr>
            <td>
                <asp:Label runat="server" ID="username" Text="Username:" Width="100px"  CssClass="col"></asp:Label>
                                 </td>
            <td>
             <asp:TextBox ID="txtUserName" runat="server" CssClass="TextBox col" style="height:20px" Text="SURESH"></asp:TextBox>
             <asp:RequiredFieldValidator ID="rfvUser" ErrorMessage="Please enter Username" ControlToValidate="txtUserName" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" ID="password" Text="Password:"   CssClass="col"></asp:Label>
                                 </td>
            
            <td style="margin-left: 40px">
                <asp:TextBox ID="txtPWD" runat="server" TextMode="Password" CssClass="TextBox col" style="height:20px" Text="1234"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPWD" runat="server" ControlToValidate="txtPWD" ErrorMessage="Please enter Password"/>
            </td>
        </tr>
              <tr>
            <td>
            </td>
            <td>
                &nbsp;</td>
            </tr>
        <tr>
            <td>
            </td>
            <td>
                 <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" style="width: 70px;height:25px" CssClass="button" />
            </td>
        </tr>
</table>
    
    </div>
     <div style ="left:224px; top:76px; position:absolute">
         <asp:Label ID="Label2" runat="server" Text="PRODUCT DEVELOPMENT REQUIREMENT"></asp:Label>
     </div>
       <div   style ="left:293px; top:158px; position:absolute">
    
        <asp:Label ID="Label1" runat="server" Text="LOGIN"></asp:Label>
       
        
        </div>
        <div style ="left:163px; top:80px; position:absolute; height: 44px; width: 50px;">
             <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/logo.png" STYLE="height:50PX;width:50PX"/>
        </div>
       
        
    </form>
    </body>
</html>
