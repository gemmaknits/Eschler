<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DR_Final_Approve.aspx.cs" Inherits="PDR.UI.DR_Final_Approve" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hidapprejID" runat="server" />
    <div style="position: absolute; top: 90px; left: 10px;">
        <h5><strong>DR FINAL APPROVAL</strong></h5>
    </div>
  
    <div style="position: absolute; top: 140px; left: 5px">
        <table>
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="DR NO" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtDRNo" runat="server" CssClass="TextBox" Width="90px"></asp:TextBox>
                    <asp:Button ID="btnFind" runat="server" Text="Find" CssClass="button" OnClick="btnFind_Click" /></td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="DATE" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <input class="datepicker TextBox" id="DRDate" type="text" size="10" name="DRDate" runat="server" style="width: 90px" readonly="readonly">
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" Text="ARTICLE" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtArticle" runat="server" CssClass="TextBox" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label4" runat="server" Text="CUSTOMER" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtCustomer" runat="server" CssClass="TextBox" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
           <%-- <tr>
                <td>
                    <asp:Label ID="Label5" runat="server" Text="APPROVE TYPE" CssClass="clsFromLabel" Width="100px"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtApproveType" runat="server" CssClass="TextBox" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>--%>
            <tr>
                <td>
                    <asp:Label ID="Label6" runat="server" Text="APP/REJ" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlAppRej" runat="server" CssClass="combo"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label7" runat="server" Text="COMMENT" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtComment" runat="server" CssClass="TextBox" Width="370px" Height="99px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td></td>

                 <td>
                    <asp:Button ID="btnDone" runat="server" Text="DONE" CssClass="button" OnClick="btnDone_Click"  style="margin-bottom:5px"/>
                    <a target="_blank">
                        <asp:ImageButton ID="ImgAttachment" Width="35px" Height="35px" ToolTip="ATTCHMENT" runat="server" ImageUrl="~/Images/Attachment.png"
                            OnClientClick="return ValidateRange()" OnClick="ImgAttachment_Click" ImageAlign="Middle" /></a>
                     <a target="SendMail_FinalApproval.aspx">
                    <asp:ImageButton ID="ImgEmal" Width="35px" Height="35px" ToolTip="EMail" runat="server" ImageUrl="~/Images/EMail.jpg"  
                        OnClick="ImgEmal_Click" ImageAlign="Middle" OnClientClick="ValidateRange();"/></a>
                </td>
              <%--  <td>
                    <asp:Button ID="btnDone" runat="server" Text="DONE" CssClass="button" OnClick="btnDone_Click" style="margin-bottom:5px" />
                    <a target="_blank">
                        <asp:ImageButton ID="ImgAttachment" Width="35px" Height="35px" ToolTip="ATTCHMENT" runat="server" ImageUrl="~/Images/Attachment.png"
                            OnClientClick="return ValidateRange()" OnClick="ImgAttachment_Click" /></a>
                      <a target="SendMail_FinalApproval.aspx">
                    <asp:ImageButton ID="ImgEmal" Width="35px" Height="35px" ToolTip="EMail" runat="server" ImageUrl="~/Images/EMail.jpg"  
                        OnClick="ImgEmal_Click"  OnClientClick="return ValidateRange()"/></a>
                </td>--%>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Label ID="lblmsg" runat="server" Text="APPROVAL DONE" Visible="False" ForeColor="#009933"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
       <div id="dvProgressBar" style="visibility: hidden; left: 50px">
            <img src="~/Images/progress_bar.gif" runat="server" style="left: 150px; position: absolute; top: 530px;" />
        </div>
        <div style="top: 611px; left: 160px; position: absolute;">
            <strong>
                <asp:Label ID="Label5" runat="server" Visible="False" ForeColor="#009933"></asp:Label>
            </strong>
        </div>


    <script type="text/javascript">
    function ShowProgressBar() {
        document.getElementById('dvProgressBar').style.visibility = 'visible';
    }

    function HideProgressBar() {
        document.getElementById('dvProgressBar').style.visibility = "hidden";
    }
</script>
    <script>
        function GetDRNODetails() {

            var DRNo = $.trim($('#ContentPlaceHolder1_txtDRNo').val());
            if (DRNo.length > 0) {
                // alert(DRNo);
                $.ajax({
                    type: "POST",
                    url: "DR_Approve_Internal.aspx/GetDRNODetails",
                    data: "{'DRNo':'" + DRNo + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        //  console.log(data);
                        for (var i = 0; i < eval(data.d.length) ; i++) {
                            var dr_date = (data.d[i].dr_date);
                            var customer_name = data.d[i].customer_name;
                            var article = data.d[i].design_no;
                            var dr_final_app_rej_comment = data.d[i].dr_final_app_rej_comment;
                            var dr_final_app_rej_id = data.d[i].dr_final_app_rej_id;
                            // alert(dr_date.substring(0, 4));
                            //alert(dr_date.substring(4, 6))
                            var drdate = (dr_date.substring(6)) + "/" + (dr_date.substring(4, 6)) + "/" + (dr_date.substring(0, 4));
                            //alert(drdate);
                            document.getElementById("ContentPlaceHolder1_DRDate").value = drdate;
                            document.getElementById("ContentPlaceHolder1_txtCustomer").value = customer_name;
                            document.getElementById("ContentPlaceHolder1_txtArticle").value = article;
                            document.getElementById("ContentPlaceHolder1_txtComment").value = dr_final_app_rej_comment;
                            document.getElementById("ContentPlaceHolder1_apprejID").value = dr_final_app_rej_id;
                        }
                    }
                });

            }
            //    })
            //})
        }
    </script>
    <script type="text/javascript">

        function stopRKey(evt) {
            // tHIS function will prevent the page from refresh
            var evt = (evt) ? evt : ((event) ? event : null);
            var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
            if ((evt.keyCode == 13) && (node.type == "text")) { return false; }
        }

        document.onkeypress = stopRKey;

    </script>
    <script type="text/javascript">
    function ShowProgressBar() {
        document.getElementById('dvProgressBar').style.visibility = 'visible';
    }

    function HideProgressBar() {
        document.getElementById('dvProgressBar').style.visibility = "hidden";
    }
</script>
    <script type="text/javascript">
        function ValidateRange() {

            document.forms[0].target = "_blank";
            return true;
        }
    </script>
    <body onkeydown="if(event.keyCode==13){event.keyCode=9; return event.keyCode}">
</asp:Content>


