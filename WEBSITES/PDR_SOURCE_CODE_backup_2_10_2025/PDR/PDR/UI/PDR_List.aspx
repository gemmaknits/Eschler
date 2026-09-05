<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PDR_List.aspx.cs" Inherits="PDR.UI.PDR_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .container {
            margin-left: 25px !important;
            margin-right: 25px !important;
            width: auto !important;
            max-width: none;
        }



        .panelnew {
            margin-bottom: 20px;
            background-color: #f5f5f5;
            border: 1px solid transparent;
            border-radius: 4px;
            -webkit-box-shadow: 0 1px 1px rgba(0,0,0,.05);
            box-shadow: 0 1px 1px rgba(0,0,0,.05);
        }

        .ui-datepicker-trigger {
            margin-left: 93px;
            margin-bottom: -25px;
            margin-top: -56px;
        }

        .dateboots {
            padding: 6px 12px;
            height: 30px;
            background-color: #fff;
            background-image: none;
            border: 1px solid #ccc;
            border-radius: 4px;
            font-size: 11px;
        }

        .btna {
            font-family: Arial,sans-serif;
            color: #337ab7;
            text-decoration: none;
        }

        .btnfont {
            font-family: Arial,sans-serif !important;
            font-size: 10px;
            font-weight: normal;
        }

        .panel-primary > .panel-heading {
            background-color: #5cb85c !important;
            border-color: #4cae4c !important;
        }

        .panel-primary {
            border-color: #4cae4c !important;
        }

        .gridcol {
            padding-right: 3px;
            padding-left: 3px;
            padding-top: 5px;
            padding-bottom: 5px;
            font-family: Arial,sans-serif;
            font-size: 11px;
            font-weight: normal !important;
        }

        .numericcol {
            text-align: right;
            font-family: Arial,sans-serif;
            font-size: 11px !important;
            font-weight: normal;
            padding-right: 3px;
            padding-left: 3px;
            padding-top: 5px;
            padding-bottom: 5px;
        }

        .GridDock {
            overflow-x: auto;
            overflow-y: hidden;
            width: 200px;
            padding: 0 0 17px 0;
        }

        .page-header {
            padding-bottom: 0px !important;
            margin: 10px 0 10px !important;
            border-bottom: 0px solid #eeeeee !important;
        }

        .form-control, .control-label {
            height: 30px;
            font-size: 11px !important;
        }

        .grdheader {
            background-color: #4CAF50; /* #D8F781;*/
            color: #FFFFFF;
            font-family: Arial,sans-serif;
            font-size: 11px;
            font-weight: bold;
        }

        .panel-title {
            color: #FFFFFF;
            font-family: Arial,sans-serif;
            font-size: 14px;
            font-weight: bold;
        }

        .form-group {
            margin-bottom: 5px;
        }

        .glyphicon {
            font-weight: normal !important;
        }

        .form-control-feedback {
            line-height: 30px !important;
        }

        .issuecolumntext {
            color: #FFFFFF !important;
        }


        @media only screen and (min-width: 0px) and (max-width: 768px) {
            .divider-middle {
                border-top: 0px solid #f5f5f5 !important;
                border-right: 0px solid #99CC99 !important;
            }
        }

        .clsFromLabel {
            font-family: Arial,sans-serif;
            font-size: 10px !important;
            font-weight: normal;
        }

        table {
            height: 100%;
        }

        tr {
            /*line-height:5px !important;*/
        }

        table td {
            /*max-width: 10% !important;
         height:25px !important;
          max-height:25px !important;*/
            padding-left: 5px;
            padding-right: 5px;
        }


        .bordercol {
            border-right: solid 1px #4cae4c;
        }

        .combo {
            border: 1px solid #456879;
            border-radius: 3px;
            height: 23px;
            font-family: Arial,sans-serif;
            font-size: 10px;
            font-weight: normal;
        }

        .TextBox {
            font-family: Arial,sans-serif;
            font-size: 10px !important;
            font-weight: normal;
        }
        .col {
    padding-right: 3px;
    padding-left: 3px;
    padding-top: 5px;
    padding-bottom: 5px;
    font-family: Arial,sans-serif;
    font-size: 10px !important;
    font-weight: normal;
}
    </style>

    <div class="container" style="top: 0px;">

        <div class="row">
            <div class="col-xs-12" style="padding-left: 0px !important; padding-right: 0px !important; overflow: hidden !important; height: 100%;">
                <div class="page-header">
                    <h3 style="margin-top: -10px !important;"><small>
                        <asp:Label ID="Label1" runat="server" Text="PDR LIST"></asp:Label></small></h3>
                </div>
                <div class="panelnew panel-primary">
                    <div class="panel-heading">
                        <span class="panel-title">Details</span>
                    </div>
                    <div class="panel-body">







                        <div class="col-xs-12">


                            <table class="" style="width: 80% !important;">
                                <tr style="line-height: 5px !important;">
                                    <td>
                                        <asp:Label ID="Label12" runat="server" Text="PDR NO" CssClass="clsFromLabel"></asp:Label></td>
                                    <td class="bordercol">
                                        <asp:TextBox ID="txtPDRNO" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);"></asp:TextBox></td>
                                    <td>
                                        <asp:Label ID="Label3" runat="server" Text="CATEGORY" CssClass="clsFromLabel"></asp:Label></td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlcategory" runat="server" Style="width: 170px" CssClass="combo"></asp:DropDownList></td>
                                    <td>
                                        <asp:Label ID="Label15" runat="server" Text="INTERNAL APP DATE FROM" CssClass="clsFromLabel"></asp:Label></td>
                                    <td class="bordercol">
                                        <input class="datepicker TextBox" id="txtInternalApproveDTFrom" type="text" size="10" name="txtInternalApproveDTFrom" runat="server" style="width: 90px"></td>
                                    <td>
                                        <asp:Label ID="Label28" runat="server" Text="INTERNAL APP/REJ " CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlInternalAppRej" runat="server" Width="170px" CssClass="combo"></asp:DropDownList></td>
                                    <td>
                                        <asp:Label ID="Label14" runat="server" Text="SHOW&nbsp;CLOSED&nbsp;PDR" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkshowclosedpdr" runat="server" Width="170px" CssClass="checkbox"></asp:CheckBox>
                                    </td>
                                </tr>
                                <tr style="line-height: 5px !important;">
                                    <td>
                                        <asp:Label ID="Label6" runat="server" Text="CUSTOMER" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <%--<asp:TextBox ID="txtRequestedBy" runat="server" CssClass="TextBox" ReadOnly="True"></asp:TextBox>--%>
                                        <asp:TextBox ID="txtCustomer" runat="server" CssClass="TextBox"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label7" runat="server" Text="SUB&nbsp;CATEGORY" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlSubCategory" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label21" runat="server" Text="INTERNAL&nbsp;APP&nbsp;DATE&nbsp;TO" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <input class="datepicker TextBox" id="txtInternalApproveDTTo" type="text" size="10" name="txtInternalApproveDTTo" runat="server" style="width: 90px"></td>

                                    <td>
                                        <asp:Label ID="Label30" runat="server" Text="CUSTOMER APP/REJ " CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlCustomerApprej" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label24" runat="server" Text="ADDED&nbsp;TO&nbsp;COLLECTION" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkaddedtocollection" runat="server" Width="170px" CssClass="checkbox"></asp:CheckBox>
                                    </td>
                                </tr>
                                <tr style="line-height: 5px !important;">
                                    <td>
                                        <asp:Label ID="Label5" runat="server" Text="PDR&nbsp;DATE&nbsp;FROM" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <input class="datepicker TextBox" id="dtFromDate" type="text" size="10" name="dtFromDate" runat="server" style="width: 90px">
                                    </td>
                                    <td>
                                        <asp:Label ID="Label4" runat="server" Text="FABRIC&nbsp;GROUP" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">

                                        <asp:DropDownList ID="ddlGroup" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>

                                    </td>
                                    <td>
                                        <asp:Label ID="Label17" runat="server" Text="CUSTOMER&nbsp;APP&nbsp;DATE&nbsp;FROM" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <input class="datepicker TextBox" id="txtCustomerApproveDTFrom" type="text" size="10" name="txtCustomerApproveDTFrom" runat="server" style="width: 90px">
                                    </td>
                                    <td>
                                        <asp:Label ID="Label29" runat="server" Text="FINAL&nbsp;APP/REJ " CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlFinalAppRej" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label25" runat="server" Text="SHORTAGE" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkshortage" runat="server" Width="170px" CssClass="checkbox"></asp:CheckBox>
                                    </td>
                                </tr>
                                <tr style="line-height: 5px !important;">
                                    <td>
                                        <asp:Label ID="Label10" runat="server" Text="PDR&nbsp;DATE&nbsp;TO" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <input class="datepicker TextBox" id="dtTo" type="text" size="10" name="dtTo" runat="server" style="width: 90px"></td>
                                    <td>
                                        <asp:Label ID="Label8" runat="server" Text="FABRIC&nbsp;SUB&nbsp;GROUP" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">

                                        <asp:DropDownList ID="ddlSubGroup" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>

                                    </td>
                                    <td>
                                        <asp:Label ID="Label22" runat="server" Text="CUSTOMER&nbsp;APP&nbsp;DATE&nbsp;TO" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <input class="datepicker TextBox" id="txtCustomerApproveDTTo" type="text" size="10" name="txtCustomerApproveDTTo" runat="server" style="width: 90px">
                                    </td>
                                    <td>
                                        <asp:Label ID="Label31" runat="server" Text="KNITTING&nbsp;APP/REJ " CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlKnittingApprej" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label26" runat="server" Text="NEW&nbsp;YARN" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkNewYarn" runat="server" Width="170px" CssClass="checkbox"></asp:CheckBox>
                                    </td>
                                </tr>
                                <tr style="line-height: 5px !important;">
                                    <td>
                                        <asp:Label ID="Label13" runat="server" Text="REQUESTED&nbsp;BY" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlRequestedBy" runat="server" Width="140px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Text="APPLICATION" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlApplication" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label19" runat="server" Text="FINAL APP DATE FROM" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <input class="datepicker TextBox" id="txtFinalApproveDTFrom" type="text" size="10" name="txtFinalApproveDTFrom" runat="server" style="width: 90px">
                                    </td>
                                    <td>
                                        <asp:Label ID="Label16" runat="server" Text="WAIT&nbsp;INTERNAL&nbsp;APP" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:CheckBox ID="chkInterApproval" runat="server" Width="170px" CssClass="checkbox"></asp:CheckBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label27" runat="server" Text="DR WAITING SO" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkdrwaitingso" runat="server" Width="170px" CssClass="checkbox"></asp:CheckBox>
                                    </td>
                                </tr>
                                <tr style="line-height: 5px !important;">
                                    <td>
                                        <asp:Label ID="Label11" runat="server" Text="PREPARED&nbsp;BY" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlPDRprepared" runat="server" Width="145px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label9" runat="server" Text="SUB&nbsp;APPLICATION" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlSubApplication" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label23" runat="server" Text="FINAL&nbsp;APP&nbsp;DATE&nbsp;TO" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <input class="datepicker TextBox" id="txtFinalApproveDTTo" type="text" size="10" name="txtFinalApproveDTTo" runat="server" style="width: 90px">
                                    </td>
                                    <td>
                                        <asp:Label ID="Label18" runat="server" Text="WAIT&nbsp;CUSTOMER&nbsp;APP" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:CheckBox ID="chkCustomerApproval" runat="server" Width="170px" CssClass="checkbox"></asp:CheckBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label20" runat="server" Text="WAIT&nbsp;FINAL&nbsp;APP" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkFinalApproval" runat="server" Width="170px" CssClass="checkbox"></asp:CheckBox>
                                    </td>
                                </tr>
                                <tr>

                                    <td colspan="10">
                                        <div class="pull-right" style="margin-right: 150px;">
                                            <asp:Button ID="btnNew" runat="server" Text="New" CssClass="btn btn-success btnfont" OnClick="btnNew_Click" />&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnFind" runat="server" Text="Find" CssClass="btn btn-success btnfont" OnClick="btnFind_Click" />&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-success btnfont" OnClick="btnClear_Click" />
                                            &nbsp;&nbsp;
                            <a target="_blank" href="~/Reports/PrintTrackingList.aspx">
                                <asp:Button ID="btnTrackingReport" runat="server" Text="Tracking Report" CssClass="btn btn-success btnfont" Height="28px" OnClientClick="SetTarget();" OnClick="btnTrackingReport_Click" Width="130px" /></a>
                                        </div>
                                    </td>

                                </tr>
                            </table>

                        </div>

                    </div>
                </div>

            <div class="col-xs-12" style="padding-left: 0px !important; padding-right: 0px !important; overflow-x: visible !important; height: 100%;">
                    <asp:GridView ID="grdPDRList" runat="server" AutoGenerateColumns="false" GridLines="Horizontal" DataKeyNames="pdr_cancel" BorderColor="#4CAF50" BorderStyle="Solid" BorderWidth="1px"
                        EmptyDataText="There is No Records To Display" OnRowDataBound="grdPDRList_onRowDataBound"
                        CssClass="gridcol">
                        <HeaderStyle CssClass="grdheader" />

                        <Columns>
                            <asp:TemplateField HeaderText="PDR NO" HeaderStyle-CssClass="col" ItemStyle-Width="20px" HeaderStyle-Width="20px">
                                <ItemTemplate>
                                    <a target="_blank" href="develop_request.aspx?pdrno=<%#Eval("pdr_no")%>" style="width: 20px">
                                        <%# Eval("pdr_no") %>
                                    </a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="pdr_date" HeaderText="PDR DATE" DataFormatString="{0:dd/MM/yyy}" HeaderStyle-CssClass="col" ItemStyle-Wrap="false">
                                <ItemStyle CssClass="col" Width="80px" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="CUSTOMER" HeaderStyle-CssClass="col" ItemStyle-Width="200px" HeaderStyle-Width="200px">
                                <ItemTemplate>
                                    <a target="_blank" href="PDR_Approval.aspx?pdrno=<%#Eval("pdr_no")%>">
                                        <%# Eval("customer_name") %>
                                    </a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--<asp:BoundField DataField="customer_name" HeaderText="CUSTOMER" HeaderStyle-CssClass="col">
                 <ItemStyle  CssClass="col"   />
             </asp:BoundField>--%>
                            <asp:BoundField DataField="requested_by" HeaderText="REQUESTED BY" HeaderStyle-CssClass="col">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="product_name" HeaderText="PRODUCT" HeaderStyle-CssClass="col">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="design_no" HeaderText="DESIGN NO" HeaderStyle-CssClass="col">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="color_code" HeaderText="COLOR" HeaderStyle-CssClass="col">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="DEVELOP_TYPE_NAME" HeaderText="DEV TYPE" HeaderStyle-CssClass="col">
                                <ItemStyle CssClass="col" Width="100px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="itgroupdesc" HeaderText="GROUP" HeaderStyle-CssClass="col">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="itsubdesc" HeaderText="SUB GROUP" HeaderStyle-CssClass="col">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="product_appl_name" HeaderText="APPLICATION" HeaderStyle-CssClass="col" Visible="false">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="product_sub_appl_name" HeaderText="SUB APPLICATION" HeaderStyle-CssClass="col" Visible="false">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="pdr_app_rej_status" HeaderText="APP/REJ STATUS" HeaderStyle-CssClass="col" ItemStyle-Width="50px" HeaderStyle-Width="50px">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="pdr_app_rej_comment" HeaderText="APP/REJ COMMENT" HeaderStyle-CssClass="col" ItemStyle-Width="50px" HeaderStyle-Width="50px">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>

                            <%--  <asp:BoundField DataField="KO_STATUS" HeaderText="KO" HeaderStyle-CssClass="col"  ItemStyle-Width="25px" HeaderStyle-Width="25px" >
                 <ItemStyle  CssClass="col"   />
             </asp:BoundField>
             <asp:BoundField DataField="df_status" HeaderText="DF" HeaderStyle-CssClass="col"  ItemStyle-Width="25px" HeaderStyle-Width="25px" >
                 <ItemStyle  CssClass="col"   />
             </asp:BoundField>
              <asp:BoundField DataField="din_status" HeaderText="DIN" HeaderStyle-CssClass="col"  ItemStyle-Width="25px" HeaderStyle-Width="25px" >
                 <ItemStyle  CssClass="col"   />
             </asp:BoundField>--%>
                            <asp:BoundField DataField="sonoid" HeaderText="DR<br> NO" HeaderStyle-CssClass="col" ItemStyle-Width="80px" HeaderStyle-Width="80px" HtmlEncode="false">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="dr_date" HeaderText="DR DT" HeaderStyle-CssClass="col" DataFormatString="{0:dd/MM/yyy}" ItemStyle-Width="80px" HeaderStyle-Width="80px">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="sonoid" HeaderText="DR<br> NO" HeaderStyle-CssClass="col" ItemStyle-Width="80px" HeaderStyle-Width="80px" HtmlEncode="false">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="knit_start_date" HeaderText="knit start" HeaderStyle-CssClass="col" DataFormatString="{0:dd/MM/yyy}" ItemStyle-Width="80px" HeaderStyle-Width="80px">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="knit_end_date" HeaderText="knit end" HeaderStyle-CssClass="col" DataFormatString="{0:dd/MM/yyy}" ItemStyle-Width="80px" HeaderStyle-Width="80px">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="dfno" HeaderText="DF<br> NO" HeaderStyle-CssClass="col" ItemStyle-Width="50px" HeaderStyle-Width="50px" HtmlEncode="false">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="dinno" HeaderText="DIN<br> NO" HeaderStyle-CssClass="col" ItemStyle-Width="80px" HeaderStyle-Width="80px" HtmlEncode="false">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="dindt" HeaderText="DIN DT" HeaderStyle-CssClass="col" DataFormatString="{0:dd/MM/yyy}" ItemStyle-Width="80px" HeaderStyle-Width="80px">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="dr_closed" HeaderText="DR CLOSED" HeaderStyle-CssClass="col" ItemStyle-Width="20px" HeaderStyle-Width="20px">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="pdr_closed" HeaderText="PDR CLOSED" HeaderStyle-CssClass="col" ItemStyle-Width="20px" HeaderStyle-Width="20px">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="WAIT_INTERNAL_APP_rEJ" HeaderText="WAIT INT APP" HeaderStyle-CssClass="col" ItemStyle-Width="50px" HeaderStyle-Width="50px">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="WAIT_cUSTOMER_APP_rEJ" HeaderText="WAIT CUST APP" HeaderStyle-CssClass="col" ItemStyle-Width="50px" HeaderStyle-Width="50px">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="WAIT_FINAL_APP_rEJ" HeaderText="WAIT FINAL APP" HeaderStyle-CssClass="col" ItemStyle-Width="50px" HeaderStyle-Width="50px">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="NEW_YARN" HeaderText="NEW YARN" HeaderStyle-CssClass="col" ItemStyle-Width="50px" HeaderStyle-Width="50px">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                            <asp:BoundField DataField="YARN_SHORTAGE" HeaderText="YARN SHORT" HeaderStyle-CssClass="col" ItemStyle-Width="50px" HeaderStyle-Width="50px">
                                <ItemStyle CssClass="col" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function SetTarget() {
            document.forms[0].target = "_blank";
        }
    </script>


    <script>


        $(function () {
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
    <body onkeydown="if(event.keyCode==13){event.keyCode=9; return event.keyCode}">
</asp:Content>
