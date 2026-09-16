<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Sample_DownloadFile.aspx.cs" Inherits="PDR.UI.Sample_DownloadFile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript">
  
        $(window).unload(function () {
            alert("Goodbye!");
        });
        function redirect_close()
     {
         alert("DD");
         Window.close()
        }
        $(window).unload(function () {
            return "Bye now!";
        })
        function a() {
            window.opener = null
            window.close();
        }
 </script>
</head>
<body  onunload="redirect_close()" >
   
    <input type="button" onclick="a();" id="closeButton" value="close" runat="server" />
   
</body>
</html>
