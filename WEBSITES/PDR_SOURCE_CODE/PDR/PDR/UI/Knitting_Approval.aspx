<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Knitting_Approval.aspx.cs" Inherits="PDR.UI.Knitting_Approval" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hidapprejID" runat="server" />
    <asp:HiddenField ID="hidkono" runat="server" />
    <asp:HiddenField ID="hidpdr_new_develop_req_id" runat="server"></asp:HiddenField>
    <div style="position: absolute; top: 90px; left: 10px">
        <h4>KNITTING APPROVAL</h4>
    </div>
    <div style="position: absolute; top: 140px; left: 5px">
        <table>
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="DESIGN NO" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtDesignNo" runat="server" CssClass="TextBox" Width="90px"></asp:TextBox>
                    <asp:Button ID="btnFind" runat="server" Text="Find" CssClass="button" OnClick="btnFind_Click" /></td>
            </tr>
              <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" Text="PDR NO" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <%--<asp:TextBox ID="txtPDRNo" runat="server" CssClass="TextBox" Width="90px" ReadOnly="true"></asp:TextBox>--%>
                    <%--<a onclick="DetailsPDRINPDF_click"><asp:HyperLink ID="hyperlinkPDRNO" runat="server" CssClass="TextBox clsFromLabel" Width="90px" ></asp:HyperLink></a>--%>
                   
                    <asp:LinkButton ID="MyLnkButton" runat="server" onClick="DetailsPDRINPDF_click" Text="" CssClass="TextBox" Width="90px"></asp:LinkButton>

            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label5" runat="server" Text="DR NO" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtDRNo" runat="server" CssClass="TextBox" Width="90px" ReadOnly="true"></asp:TextBox>
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
                    <asp:Label ID="Label8" runat="server" Text="KI/KO NO" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <%--<asp:TextBox ID="txtKINO" runat="server" CssClass="TextBox" Width="90px" ReadOnly="true"></asp:TextBox>--%>
                   <%-- <asp:HyperLink ID="hyperlinkKINO" runat="server" CssClass="TextBox clsFromLabel" Width="90px"></asp:HyperLink>--%>
                   <a target="_blank" href="YarnLots.aspx">
                        <asp:LinkButton ID="kikono" OnClientClick="return ValidateRange()"   runat="server" Width="90px"  Text="" CssClass="TextBox" OnClick="kikono_Click"></asp:LinkButton></a>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label4" runat="server" Text="CUSTOMER" CssClass="clsFromLabel" ></asp:Label>
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
                    <asp:TextBox ID="txtComment" runat="server" CssClass="TextBox" Width="370px" TextMode="MultiLine" Height="99px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td></td>
                <td>
                    <asp:Button ID="btnDone" runat="server" Text="DONE" CssClass="button" OnClick="btnDone_Click" Height="35px" />
                    <a target="_blank">
                        <asp:ImageButton ID="ImgAttachment" Width="35px" Height="35px" ToolTip="ATTCHMENT" runat="server" ImageUrl="~/Images/Attachment.png"
                            OnClientClick="return ValidateRange()" OnClick="ImgAttachment_Click" ImageAlign="Top" /></a>
                     <a target="_blank">
                    <asp:ImageButton ID="ImgEmal" Width="35px" Height="35px" ToolTip="EMail" runat="server" ImageUrl="~/Images/EMail.jpg"  
                       OnClientClick="return ValidateRange()"  OnClick="ImgEmal_Click" ImageAlign="Top" /></a>
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
     <div class="col-sm-10" style="margin-left: 7px; padding-left: 7px; left: 17px;top:530px">
                    <asp:GridView ID="grdAttachmentDetails" runat="server" AutoGenerateColumns="false" GridLines="None"
                        EmptyDataText="There is No Records To Display" Width="13%"  DataKeyNames="doc_attachments_id,source_doc_number,file_location"
                        CssClass="table table-striped table-bordered table-hover" >

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
    <body onkeydown="if(event.keyCode==13){event.keyCode=9; return event.keyCode}">
</asp:Content>


