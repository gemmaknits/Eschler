<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PDR_List.aspx.cs" Inherits="PDR.UI.PDR_List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="left:72px; top:54px; width:400px;position:absolute" >
        <Table ID="Table1" runat="server">
        <tr>
             <td> <strong> <asp:Label ID="Label1" runat="server" Text="PDR LIST"></asp:Label></strong></td>
        </tr>
        </Table>
        </div>
     <div style="left:86px; top:90px; width:400px;position:absolute" >
                <table style="line-height:28px;">
                    <tr style="line-height:28px;">
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="PDR NO" CssClass="clsFromLabel"></asp:Label></td>
                        <td>
                            <asp:TextBox ID="txtPDRNO" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);"></asp:TextBox></td>
                    </tr>
                    <tr style="line-height:28px;">
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="CATEGORY" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td >
                            <asp:DropDownList ID="ddlcategory" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="FABRIC GROUP" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td>
                            
                            <asp:DropDownList ID="ddlGroup" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                            
                        </td>
                    </tr>
                     <tr style="line-height:28px;">
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="APPLICATION" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlApplication" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr style="line-height:28px;">
                        <td style="padding-right:20px">
                            <asp:Label ID="Label5" runat="server" Text="PDR DATE FROM" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td>
                            <INPUT class="datepicker TextBox" id="dtFromDate"   type="text" size="10" name="dtFromDate" runat="server" style="width:90px" >
                        </td>
                    </tr>
                     <tr style="line-height:28px;">
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="REQUESTED BY" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtRequestedBy" runat="server" CssClass="TextBox" ReadOnly="True"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>

                        </td>
                        <td>
                            <asp:Button ID="btnNew" runat="server" Text="New" CssClass="button" OnClick="btnNew_Click"/>&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnFind" runat="server" Text="FIND" CssClass="button" OnClick="btnFind_Click" />&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnClear" runat="server" Text="CLEAR" CssClass="button" />
                        </td>
                    </tr>
                </table>
        </div>
    <div style="left:492px; top:91px; width:400px;position:absolute" >
                <table style="line-height:28px;">
                   <tr>
                       <td>
                            <asp:Label ID="Label6" runat="server" Text="CUSTOMER" CssClass="clsFromLabel"></asp:Label>
                        </td>
                       <td>
                            <%--<asp:TextBox ID="txtRequestedBy" runat="server" CssClass="TextBox" ReadOnly="True"></asp:TextBox>--%>
                            <asp:TextBox ID="txtCustomer" runat="server" CssClass="TextBox"></asp:TextBox>
                        </td>
                   </tr>
                    <tr style="line-height:28px;">
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="SUB CATEGORY" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSubCategory" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-right:20px">
                            <asp:Label ID="Label8" runat="server" Text="FABRIC SUB GROUP" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td>
                            
                            <asp:DropDownList ID="ddlSubGroup" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                            
                        </td>
                    </tr>
                     <tr style="line-height:28px;">
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="SUB APPLICATION" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSubApplication" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                        </td>
                    </tr>
                   
                     <tr style="line-height:28px;">
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="PDR DATE TO" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td>
                            <INPUT class="datepicker TextBox" id="dtTo"  type="text" size="10" name="dtTo" runat="server" style="width:90px" ></td>
                    </tr>
                     <tr style="line-height:28px;">
                        <td >
                            <asp:Label ID="Label11" runat="server" Text="PREPARED BY" CssClass="clsFromLabel"></asp:Label>
                         </td>
                        <td>
                            <asp:DropDownList ID="ddlPDRprepared" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                         </td>
                    </tr>
                </table>
        </div>
     <div style="left:77px; top:310px; position:absolute;Width:1415px">
    <asp:GridView ID="grdPDRList" runat="server" AutoGenerateColumns="false"  GridLines="None"
        emptydatatext="There is No Records To Display"   Width="1415px" 
      CssClass="col"  >
        <HeaderStyle CssClass="grdheader"/>
      
                           
        <Columns>
               <asp:TemplateField HeaderText="PDR NO" HeaderStyle-CssClass="col" >
                    <ItemTemplate>
                          <a  target="_blank"  href="develop_request.aspx?pdrno=<%#Eval("pdr_no")%>">
                         <%# Eval("pdr_no") %>
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>
             <asp:BoundField DataField="pdr_date" HeaderText="PDR DATE" DataFormatString="{0:dd/MM/yyy}" HeaderStyle-CssClass="col" ItemStyle-Wrap="false">
                 <ItemStyle  CssClass="col"  width="150px" />
             </asp:BoundField>
            <asp:TemplateField HeaderText="CUSTOMER" HeaderStyle-CssClass="col" >
                    <ItemTemplate>
                          <a  target="_blank"  href="PDR_Approval.aspx?pdrno=<%#Eval("pdr_no")%>">
                         <%# Eval("customer_name") %>
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>
              <%--<asp:BoundField DataField="customer_name" HeaderText="CUSTOMER" HeaderStyle-CssClass="col">
                 <ItemStyle  CssClass="col"   />
             </asp:BoundField>--%>
             <asp:BoundField DataField="requested_by" HeaderText="REQUESTED BY" HeaderStyle-CssClass="col">
                 <ItemStyle  CssClass="col"  />
             </asp:BoundField>
             <asp:BoundField DataField="design_no" HeaderText="PRODUCT" HeaderStyle-CssClass="col">
                 <ItemStyle  CssClass="col"  />
             </asp:BoundField>
             <asp:BoundField DataField="itsubcatdesc" HeaderText="DESIGN SUB CAT" HeaderStyle-CssClass="col">
                 <ItemStyle  CssClass="col"   />
             </asp:BoundField>
             <asp:BoundField DataField="itgroupdesc" HeaderText="DESIGN GROUP" HeaderStyle-CssClass="col">
                 <ItemStyle  CssClass="col"   />
             </asp:BoundField>
             <asp:BoundField DataField="itsubdesc" HeaderText="DESIGN SUB GROUP" HeaderStyle-CssClass="col">
                 <ItemStyle  CssClass="col"  />
             </asp:BoundField>
             <asp:BoundField DataField="product_appl_name" HeaderText="APPLICATION" HeaderStyle-CssClass="col">
                 <ItemStyle  CssClass="col"   />
             </asp:BoundField>
             <asp:BoundField DataField="product_sub_appl_name" HeaderText="SUB APPLICATION" HeaderStyle-CssClass="col">
                 <ItemStyle  CssClass="col"   />
             </asp:BoundField>
        </Columns>
        </asp:GridView>
    </div>
     <script>
     
        
  $(function() {
      $(".datepicker").datepicker({
           numberOfMonths: 2,
          showOn: "button",
          buttonImage: "/images/calendar.gif",
          buttonImageOnly: true,
          buttonText: "Select date",
          dateFormat: "dd/mm/yy",
          onSelect: function () {
              this.focus();
          }
      });
    
         
  });
   
		</SCRIPT>
     <script type="text/javascript">

        function stopRKey(evt) {
            // tHIS function will prevent the page from refresh
            var evt = (evt) ? evt : ((event) ? event : null);
            var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
            if ((evt.keyCode == 13) && (node.type == "text")) { return false; }
        }

        document.onkeypress = stopRKey;

    </script>
      <body onkeydown="if(event.keyCode==13){event.keyCode=9; return event.keyCode}">
</asp:Content>
