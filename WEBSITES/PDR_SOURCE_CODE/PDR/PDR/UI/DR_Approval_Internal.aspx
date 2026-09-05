<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DR_Approval_Internal.aspx.cs" Inherits="PDR.UI.DR_Approval_Internal" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hidapprejID" runat="server" />
     <asp:HiddenField ID="hidsampleremail" runat="server" />
     <asp:HiddenField ID="hidmdemail" runat="server" />
    <asp:HiddenField ID="hidrequestoremail" runat="server" />
    <asp:HiddenField ID="hidaddtocollection" runat="server" />
    <div style="position: absolute; top: 90px; left: 10px">
        <h4>DR INTERNAL APPROVAL</h4>
    </div>
    <div style="position: absolute; top: 140px; left: 5px">
        <table>
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="DR NO / DYE JOB LOT" CssClass="clsFromLabel"></asp:Label>
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
                    <asp:DropDownList ID="ddlAppRej" runat="server" CssClass="combo" onchange="SetAddToCollection();"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label7" runat="server" Text="COMMENT" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtComment" runat="server" CssClass="TextBox" Width="370px" TextMode="MultiLine" Height="179px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:CheckBox ID="chkSendToCustomer" runat="server" Text="Send To Customer" CssClass="chkcol" />
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:CheckBox ID="chkAddToCollection" runat="server" Text="Add To Collection" CssClass="chkcol" onchange="CheckValue();"/>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Button ID="btnDone" runat="server" Text="DONE" CssClass="button" OnClick="btnDone_Click" />&nbsp;
                <a target="_blank">
                    <asp:ImageButton ID="ImgAttachment" Width="35px" Height="35px" ToolTip="ATTCHMENT" runat="server" ImageUrl="~/Images/Attachment.png"  
                        OnClientClick="return ValidateRange()"         ImageAlign="Middle"                OnClick="ImgAttachment_Click" /></a>&nbsp;&nbsp;&nbsp;            
                <a target="SendMail_InternalApproval.aspx">
                    <asp:ImageButton ID="ImgEmal" Width="35px" Height="35px" ToolTip="EMail" runat="server" ImageUrl="~/Images/EMail.jpg"  
                        OnClick="ImgEmal_Click" ImageAlign="Middle"  OnClientClick="return ValidateRange()" /></a>
            </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Label ID="lblmsg" runat="server" Text="APPROVAL DONE" Visible="False" ForeColor="#009933"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
      <div class="col-sm-10" style="margin-left: 7px; padding-left: 7px; left: 5px;top:530px">
                    <asp:GridView ID="grdAttachmentDetails" runat="server" AutoGenerateColumns="false" GridLines="None"
                        EmptyDataText="There is No Records To Display" Width="13%"  DataKeyNames="doc_attachments_id,source_doc_number,file_location"
                        CssClass="table table-striped table-bordered table-hover" OnRowCommand="OnRowCommand">

                        <HeaderStyle CssClass="grdheader" />
                        <Columns>
                            <asp:TemplateField ItemStyle-Width="2%" HeaderText="" HeaderStyle-CssClass="col col-xs-2" ItemStyle-CssClass="col col-xs-2" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                                <ItemTemplate>
                                        <asp:CheckBox ID="chkdelete" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>              
                            <asp:TemplateField ItemStyle-Width="2%" HeaderText="DESCRIPTION" HeaderStyle-CssClass="col col-xs-2" ItemStyle-CssClass="col col-xs-2" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="FILE_DESCRIPTION" runat="server" onkeydown="(event.keyCode!=13);" Text='<%# Eval("FILE_DESCRIPTION") %>' CssClass="TextBox " Width="300px" BorderStyle="None" ReadOnly="true"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--                            <asp:TemplateField ItemStyle-Width="2%" HeaderText="LOCATION" HeaderStyle-CssClass="col col-xs-2 " ItemStyle-CssClass="col col-xs-2 numericcol other_cost_per_kg" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="FILE_LOCATION" runat="server" onkeydown="(event.keyCode!=13);" Text='<%# Eval("FILE_LOCATION_disp") %>' CssClass="TextBox " Width="180px" ReadOnly="true" />
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                            <asp:BoundField DataField="FILE_LOCATION_disp" HeaderText="LOCATION" ReadOnly="True" HeaderStyle-CssClass="col">
                                <ItemStyle CssClass="col" Width="20px" />
                            </asp:BoundField>
                            <asp:TemplateField>
                                <HeaderStyle Width="20" />
                                <ItemTemplate>
                                    <asp:ImageButton ImageUrl="~/Images/downloadfile.png" ControlStyle-Width="14px" ControlStyle-Height="14px" CommandName="Download"
                                        CommandArgument='<%#((GridViewRow)Container).RowIndex%>' ToolTip="Download" runat="server" Height="14" Width="14" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
      <div id="dvProgressBar" style="visibility: hidden; left: 50px">
            <img src="~/Images/progress_bar.gif" runat="server" style="left: 150px; position: absolute; top: 530px;" />
        </div>
        <div style="top: 611px; left: 160px; position: absolute;">
            <strong>
                <asp:Label ID="Label5" runat="server" Visible="False" ForeColor="#009933"></asp:Label>
            </strong>
        </div>
    <script>
        function SetAddToCollection()
        {
            var value = $('#ContentPlaceHolder1_ddlAppRej').val();
            
            var apprejtext = $('#ContentPlaceHolder1_ddlAppRej :selected').text();
            if ($.trim(apprejtext) == "APPROVE")
            {
                $("#ContentPlaceHolder1_chkAddToCollection").prop("checked", true);
            }
            else
            {
                $("#ContentPlaceHolder1_chkAddToCollection").prop("checked", false);
            }
        }
    </script>
    <script>
        function CheckValue()
        {
           
            var apprejtext = $('#ContentPlaceHolder1_ddlAppRej :selected').text();
            if ($.trim(apprejtext) == "APPROVE") {
                var addtocollvalue = $('#ContentPlaceHolder1_chkAddToCollection').is(':checked');
                //alert(addtocollvalue);
                if ((!addtocollvalue)) {
                    alert("Selected as APPROVE ,So Cannot Uncheck");
                }
                $("#ContentPlaceHolder1_chkAddToCollection").prop("checked", true);
            }

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
                            var dr_internal_app_rej_comment = data.d[i].dr_internal_app_rej_comment;
                            var dr_internal_app_rej_id = data.d[i].dr_internal_app_rej_id;
                            // alert(dr_date.substring(0, 4));
                            //alert(dr_date.substring(4, 6))
                            var drdate = (dr_date.substring(6)) + "/" + (dr_date.substring(4, 6)) + "/" + (dr_date.substring(0, 4));
                            //alert(drdate);
                            document.getElementById("ContentPlaceHolder1_DRDate").value = drdate;
                            document.getElementById("ContentPlaceHolder1_txtCustomer").value = customer_name;
                            document.getElementById("ContentPlaceHolder1_txtArticle").value = article;
                            document.getElementById("ContentPlaceHolder1_txtComment").value = dr_internal_app_rej_comment;
                            document.getElementById("ContentPlaceHolder1_apprejID").value = dr_internal_app_rej_id;
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
      function ValidateRange() {
 
          document.forms[0].target = "_blank";
          return true;
      }
  </script>
    <script type="text/javascript">
    function ShowProgressBar() {
        document.getElementById('dvProgressBar').style.visibility = 'visible';
    }

    function HideProgressBar() {
        document.getElementById('dvProgressBar').style.visibility = "hidden";
    }
</script>
    <body onkeydown="if(event.keyCode==13){event.keyCode=9; return event.keyCode}">
    </div>
</asp:Content>
