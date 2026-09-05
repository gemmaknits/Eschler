<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PDRArticleListMulti2.aspx.cs" Inherits="PDR.UI.PDRArticleListMulti2" %>

<%@ Register assembly="Syncfusion.EJ" namespace="Syncfusion.JavaScript.Models" tagprefix="ej" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>

        td.locked, th.locked {
        position:relative;    
        left:expression((this.parentElement.parentElement.parentElement.parentElement.scrollLeft-2)+'px');
        }   
         
        .hide {
            display:none;
        }

        .blank_row
        {
            height: 10px !important; /* overwrites any other rules */
            /*background-color: #FFFFFF;*/
        }

        #aspGrid
        {
          overflow: scroll;  
          margin-bottom: 14px;     
          padding-left: 0px !important; 
        padding-right: 0px !important; 
        overflow-y: scroll; 
        overflow-x: visible !important; 
        height: 600px;        
        /*overflow: auto;*/
        scrollbar-base-color:#ffeaff;
        }
    
        .header-center{
            text-align:center;
        }

        .GVTextBoxBorder
        {
            border:none;
        }
     
        .GFixedHeader {
            position: absolute;
/*            font-weight: bold;*/
            font-Size: 10px;
            font-family: "Segoe UI", Arial, sans-serif;
        }     

        div#div-datagrid {
        padding-left: 0px !important; 
        padding-right: 0px !important; 
        overflow-y: scroll; 
        overflow-x: visible !important; 
        height: 600px;        
        /*overflow: auto;*/
        scrollbar-base-color:#ffeaff;
        }

        /* Locks table header */
/*        th {
        font-size: 14px;
        font-weight: bold;
        text-align: center;
        background-color: navy;
        color: white;
        border-right: 1px solid silver;
        position:relative;
        cursor: default;
        /*IE5+ only*/
/*        top: expression(document.getElementById("div-datagrid").scrollTop-2);
        z-index: 10;
        }
*/

        .checkbox label
        {
        padding-left:5px;
        }


        .StaticColumn
          {    
            font-family: "Segoe UI", Arial, sans-serif;
            font-size: 14px;
            font-weight: normal;   
              border: none;
              position: relative;
          }

        /* Keeps the header as the top most item. Important for top left item*/
        th.locked {z-index: 99;}

        .blank_row
        {
            height: 8px !important; /* overwrites any other rules */
            background-color: #f5f5f5;
        }
        .container {
            margin-left: 5px !important;
            margin-right: 5px !important;
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
            position:relative;top: 2px;
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

        .btn {
            font-family: "Segoe UI", Arial, sans-serif;
            color: #f7f8f9;
            text-decoration: none;
        }

        .btnfont {
            font-family: "Segoe UI", Arial, sans-serif;
            font-size: 10px;
            font-weight: normal;
        }

        .panel-primary > .panel-heading {
            background-color: #5cb85c !important;
            border-color: #4cae4c !important;
            height:30px;
        }

        .panel-primary {
            border-color: #4cae4c !important;
        }

        .gridcol {
            padding-right: 3px;
            padding-left: 3px;
            padding-top: 5px;
            padding-bottom: 5px;
            font-family: "Segoe UI", Arial, sans-serif;
            font-size: 11px;
            font-weight: normal !important;
        }

        .numericcol {
            text-align: right;
            font-family: "Segoe UI", Arial, sans-serif;
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
            font-family: "Segoe UI", Arial, sans-serif;
            font-size: 11px;
            font-weight: bold;
        }

        .panel-title {
            color: #FFFFFF;
            font-family: "Segoe UI", Arial, sans-serif;
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


        .checkboxListHeader {
            background-color:lightgreen;
            align-self:center;
        }

        @media only screen and (min-width: 0px) and (max-width: 768px) {
            .divider-middle {
                border-top: 0px solid #f5f5f5 !important;
                border-right: 0px solid #99CC99 !important;
            }
        }

        .clsFromLabel {
            font-family: "Segoe UI", Arial, sans-serif;
            font-size: 10px !important;
            font-weight: normal;
            width:200px;
        }

        .clsFoundLabel {
            text-align:right;
            font-family: "Segoe UI", Arial, sans-serif;
            font-size: 10px !important;
            font-weight: normal;
            width:200px;
        }

        table {
            height: 100%;
            width: 80%;
        }


        tr {
            /*line-height:5px !important;*/
        }

        appltd {
            /*max-width: 10% !important;
         height:25px !important;
          max-height:25px !important;*/
            padding-left: 5px;
            width:100px;
        }
 

        table td {
            /*max-width: 10% !important;
         height:25px !important;
          max-height:25px !important;*/
            padding-left: 5px;
            padding-right: 5px;
            width: 150px;
  
        }

        

        .bordercol {
            border-bottom: solid 1px #4cae4c;
        }

        .borderright {
            border-right: solid 1px #4cae4c;
        }
        .borderleft {
            border-left: solid 1px #4cae4c;
        }

        .bordertop {
            border-top: solid 1px #4cae4c;
        }
        .borderall {
            border: solid 1px #4cae4c;
        }
        .combo {
            border: 1px solid #456879;
            border-radius: 3px;
            height: 23px;
            font-family: "Segoe UI", Arial, sans-serif;
            font-size: 10px;
            font-weight: normal;
        }

        .LabelWide {
            font-family: "Segoe UI", Arial, sans-serif;
            font-size: 10px !important;
            font-weight: normal;
            width:120px;
/*            display: block;*/
        }

        .TextBox{
            font-family: "Segoe UI", Arial, sans-serif;
            font-size: 10px !important;
            font-weight: normal;
            width:50px;
        }
        
        .col {
                padding-right: 3px;
                padding-left: 3px;
                padding-top: 5px;
                padding-bottom: 5px;
                font-family: "Segoe UI", Arial, sans-serif;
                font-size: 10px !important;
                font-weight: normal;
             }

        .checkboxlistformat {
            padding-left: 5px;
            font-family: "Segoe UI", Arial, sans-serif;
            font-size: 12px !important;
            font-weight:100;
            margin-left:5px;
            margin-right: 10px;
            padding-right: 10px;
        }

     .checkboxlist {
            padding-left: 5px;
            font-family: "Segoe UI", Arial, sans-serif;
            font-size: 10px !important;
            font-weight:100;
            margin-left:5px;
            margin-right: 10px;
            padding-right: 10px;
    }

     .checkboxAll {
    padding-left: 3px;
            font-family: "Segoe UI", Arial, sans-serif;
    font-size: 10px !important;
    font-weight: normal;
    }

    </style>

    <div class="container" style="top: 0px;">

        <div class="row">
            <div class="col-xs-12" style="padding-left: 0px !important; padding-right: 0px !important; overflow: hidden !important; height: 100%;">
                <div class="page-header">
                    <h3 style="margin-top: -10px !important;"><small>
                        <asp:Label ID="Label1" runat="server" Text="ARTICLE / DESIGN LIST"></asp:Label></small></h3>
                </div>

                <div class="panelnew panel-primary">

                    <div class="panel-heading">
                        <span class="panel-title">Article Search.</span>
                    </div>

                    <div class="panel-body">

                        <div class="col-xs-12">

                            <table >

                                <tr style="line-height: 5px !important;">
                                    <td>
                                        <asp:Label ID="Label12"  runat="server" Text="COMPOSITION" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label16" runat="server" Text="ARTICLE" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label5" runat="server" Text="DESIGN&nbsp;DATE&nbsp;FROM" CssClass="clsFromLabel"></asp:Label>
                                    </td>

                                    <td>
                                        <asp:Label ID="Label10" runat="server" Text="DESIGN&nbsp;DATE&nbsp;TO" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td  >
                                        <asp:Label ID="Label11" runat="server" Text="WIDTH(CM)&nbsp;FROM" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label13"  runat="server" Text="WIDTH(CM) TO " CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td  >
                                        <asp:Label ID="Label6" runat="server" Text="Guage" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                </tr>
                                <tr class="blank_row">
                                    <td colspan="3"></td>
                                </tr>
                                <tr style=""line-height: 5px !important;">
                                    <td >
                                        <asp:TextBox ID="txtCompos" Style="width: 200px" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);"></asp:TextBox>
                                    </td>
                                    <td >
                                        <asp:Textbox ID="txtArticle" runat="server" Style="width: 200px" CssClass="combo"></asp:Textbox>
                                    </td>
                                    <td >
                                        <input class="datepicker TextBox" id="dtFromDate" type="text" size="10" name="dtFromDate" runat="server" style="width: 90px">
                                    </td>
                                    <td >
                                        <input class="datepicker TextBox" id="dtTo" type="text" size="10" name="dtTo" runat="server" style="width: 90px">
                                    </td>
                                    <td >
                                        <asp:TextBox ID="txtWidthFrom" Style="width: 80px" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);"></asp:TextBox>
                                    </td>
                                    <td >
                                        <asp:TextBox ID="txtWidthTo" Style="width: 80px" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);"></asp:TextBox>
                                    </td>
                                    <td >
                                        <asp:TextBox ID="txtGauge" Style="width: 80px" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);"></asp:TextBox>
                                    </td>
                                </tr>

                                <tr class="blank_row">
                                    <td colspan="3"></td>
                                </tr>

                                <tr style="line-height: 5px !important;">
                                    <td>
                                        <asp:Label ID="Label3" runat="server" Text="CATEGORY" CssClass="clsFromLabel"></asp:Label>
                                    </td>

                                    <td>
                                        <asp:Label ID="Label7" runat="server" Text="SUB&nbsp;CATEGORY" CssClass="clsFromLabel"></asp:Label>
                                    </td>

                                    <td>
                                        <asp:Label ID="Label4" runat="server" Text="FABRIC&nbsp;GROUP" CssClass="clsFromLabel"></asp:Label>
                                    </td>

                                    <td>
                                        <asp:Label ID="Label8" runat="server" Text="FABRIC&nbsp;SUB&nbsp;GROUP" CssClass="clsFromLabel"></asp:Label>
                                    </td>

                                    <td>
                                        <asp:Label ID="Label14"  runat="server" Text="WEIGHT(GM)&nbsp;FROM " CssClass="clsFromLabel"></asp:Label>
                                    </td>

                                    <td>
                                        <asp:Label ID="Label15"  runat="server" Text="WEIGHT(GM) TO " CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label17"  runat="server" Text="Sheet " CssClass="clsFromLabel"></asp:Label>
                                    </td>

                                </tr>

                                <tr class="blank_row">
                                    <td colspan="3"></td>
                                </tr>

                                <tr style="line-height: 5px !important;">
                                    <td >
                                        <asp:DropDownList ID="ddlcategory" runat="server" Style="width: 200px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td >
                                        <asp:DropDownList ID="ddlSubCategory" runat="server" Width="200px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td >
                                        <asp:DropDownList ID="ddlGroup" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td >
                                        <asp:DropDownList ID="ddlSubGroup" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtWeightFrom" Style="width: 80px" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);"></asp:TextBox>
                                    </td>
                                    <td >
                                        <asp:TextBox ID="txtWeightTo" Style="width: 80px" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);"></asp:TextBox>
                                    </td>
                                    <td >
                                        <asp:DropDownList ID="ddlSheetName" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>

                                </tr>
                            </table>
                               </div>
                        <div>
                        </div>
                    </div>
                    <div class="panel-heading">
                        <span class="panel-title">Article Filter</span>
                    </div>

                    <div class="panel-body" style="width:3000px; overflow-y: scroll; overflow-x: scroll">
                        <div class="col-md-6">
                            <Table>

                                <tr>
                                    <th class="borderall checkboxListHeader"><asp:Button ID="btnClearAppl" runat="server" Text="Clear" CssClass="btn btn-success btnfont" OnClick="btnClearAppl_Click"></asp:Button></th>
                                    <th class="borderall checkboxListHeader"><asp:Button ID="btnClearSubAppl" runat="server" Text="Clear" CssClass="btn btn-success btnfont" OnClick="btnClearSubAppl_Click"></asp:Button></th>
                                    <th class="borderall checkboxListHeader"><asp:Button ID="btnClearSplFunc" runat="server" Text="Clear" CssClass="btn btn-success btnfont" OnClick="btnClearSplFunc_Click"></asp:Button></th>
                                    <th class="borderall checkboxListHeader"><asp:Button ID="btnClearDesignType" runat="server" Text="Clear" CssClass="btn btn-success btnfont" OnClick="btnClearDesignType_Click"></asp:Button></th>
                                    <th class="borderall checkboxListHeader"><asp:Button ID="btnClearFamily" runat="server" Text="Clear" CssClass="btn btn-success btnfont" OnClick="btnClearFamily_Click"></asp:Button></th>
                                    <th class="borderall checkboxListHeader"><asp:Button ID="btnClearFinishing" runat="server" Text="Clear" CssClass="btn btn-success btnfont" OnClick="btnClearFinishing_Click"></asp:Button></th>
                                    <th class="borderall checkboxListHeader"><asp:Button ID="btnClearFibre" runat="server" Text="Clear" CssClass="btn btn-success btnfont" OnClick="btnClearFinishing_Click"></asp:Button></th>
                                    <th class="borderall checkboxListHeader"><asp:Button ID="btnClearFibreSub" runat="server" Text="Clear" CssClass="btn btn-success btnfont" OnClick="btnClearFinishing_Click"></asp:Button></th>
                                </tr>
                                <tr>
                                    <th class="borderall checkboxListHeader"><asp:Label ID="Application" runat="server" Text="APPLICATION" CssClass="clsFromLabel"></asp:Label></th>
                                    <th class="borderall checkboxListHeader"><asp:Label ID="Label21" runat="server" Text="SUB APPLICATION" CssClass="clsFromLabel"></asp:Label></th>
                                    <th class="borderall checkboxListHeader"><asp:Label ID="Label22" runat="server" Text="SPL. FUNC" CssClass="clsFromLabel"></asp:Label></th>
                                    <th class="borderall checkboxListHeader"><asp:Label ID="Label23" runat="server" Text="DESIGN TYPE" CssClass="clsFromLabel"></asp:Label></th>
                                    <th class="borderall checkboxListHeader"><asp:Label ID="Label24" runat="server" Text="FAMILY" CssClass="clsFromLabel"></asp:Label></th>
                                    <th class="borderall checkboxListHeader"><asp:Label ID="Label25" runat="server" Text="FINISHING" CssClass="clsFromLabel"></asp:Label></th>
                                    <th class="borderall checkboxListHeader"><asp:Label ID="Label9" runat="server" Text="FIBRE" CssClass="clsFromLabel"></asp:Label></th>
                                    <th class="borderall checkboxListHeader"><asp:Label ID="Label2" runat="server" Text="KIND OF YARN" CssClass="clsFromLabel"></asp:Label></th>
                                </tr>
                                <!--
                                <tr>
                                    <td class="borderall checkboxAll "><asp:checkbox ID="AllAppl" runat="server" Text="ALL" ></asp:checkbox></td>
                                    <td class="borderall checkboxAll"><asp:checkbox ID="AllSubAppl" runat="server" Text="ALL"></asp:checkbox></td>
                                    <td class="borderall checkboxAll"><asp:checkbox ID="AllSplFunc" runat="server" Text="ALL"></asp:checkbox></td>
                                    <td class="borderall checkboxAll"><asp:checkbox ID="AllDesignType" runat="server" Text="ALL"></asp:checkbox></td>
                                    <td class="borderall checkboxAll"><asp:checkbox ID="AllFamily" runat="server" Text="ALL"></asp:checkbox></td>
                                    <td class="borderall checkboxAll"><asp:checkbox ID="AllFinish" runat="server" Text="ALL"></asp:checkbox></td>
                                </tr>
                                -->
                                <tr>
                                    <td class="borderall"><div style="height:500px; width:200px; overflow-y: scroll; overflow-x:hidden"> 
                                        <asp:CheckBoxList class="checkboxlistformat" ID="cblAppl" runat="server" DataTextField="lookup_value" DataValueField="lookup_value_id" ></asp:CheckBoxList>
                                        </div>
                                    </td>
                                    <td class="borderall"><div style="height:500px; width:200px; overflow-y: scroll; overflow-x:hidden">
                                        <asp:CheckBoxList class="checkboxlistformat" ID="cblSubAppl" runat="server" DataTextField="lookup_value" DataValueField="lookup_value_id" ></asp:CheckBoxList>
                                        </div>
                                    </td>
                                    <td class="borderall"><div style="height:500px; width:200px; overflow-y: scroll; overflow-x:hidden">
                                        <asp:CheckBoxList class="checkboxlistformat" ID="cblSplFunc" runat="server" DataTextField="lookup_value" DataValueField="lookup_value_id" ></asp:CheckBoxList>
                                        </div>
                                    </td>
                                    <td class="borderall"><div style="height:500px; width:200px; overflow-y: scroll; overflow-x:hidden">
                                        <asp:CheckBoxList class="checkboxlistformat" ID="cblDesignType" runat="server" DataTextField="lookup_value" DataValueField="lookup_value_id" ></asp:CheckBoxList>
                                        </div>
                                    </td>
                                    <td class="borderall"><div style="height:500px; width:200px; overflow-y: scroll; overflow-x:hidden">
                                        <asp:CheckBoxList class="checkboxlistformat" ID="cblFamilyName" runat="server" DataTextField="lookup_value" DataValueField="lookup_value_id" ></asp:CheckBoxList>
                                        </div>
                                    </td>
                                    <td class="borderall"><div style="height:500px; width:300px; overflow-y: scroll; overflow-x:hidden">
                                        <asp:CheckBoxList class="checkboxlistformat" ID="cblFinishing" runat="server" DataTextField="fin_name" DataValueField="id" ></asp:CheckBoxList>
                                        </div>
                                    </td>
                                    <td class="borderall"><div style="height:500px; width:200px; overflow-y: scroll; overflow-x:hidden">
                                        <asp:CheckBoxList class="checkboxlistformat" ID="cblFibreType" runat="server" DataTextField="ittypedesc" DataValueField="ittypecd" ></asp:CheckBoxList>
                                        </div>
                                    </td>
                                    <td class="borderall"><div style="height:500px; width:200px; overflow-y: scroll; overflow-x:hidden">
                                        <asp:CheckBoxList class="checkboxlistformat" ID="cblFibreSubType" runat="server" DataTextField="itsubdesc2" DataValueField="itsubcd2" ></asp:CheckBoxList>
                                        </div>
                                    </td>
                                </tr>

                                <tr><td><div>&nbsp;</div></td></tr>
                                <tr>
                                    <td class="clsFoundLabel">
                                        <asp:Label ID="lblCount" runat="server" Text="Found:" CssClass="clsFoundLabel"></asp:Label>
                                    </td>
                                    <td >
                                        <asp:Textbox ID="txtRecordCount"  runat="server" Style="width: 200px" CssClass="combo" ReadOnly="True"></asp:Textbox>
                                    </td>

                                    <td colspan="10">
                                        <div class="pull-right" style="margin-right: 150px;">
                                            <asp:Button ID="btnFind" runat="server" Text="Find" CssClass="btn btn-success btnfont" OnClick="btnFind_Click" />&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnClear" runat="server" Text="Reset All" CssClass="btn btn-success btnfont" OnClick="btnClear_Click" />
                                            <asp:Button ID="btnExcel" runat="server" Text="Excel" CssClass="btn btn-success btnfont" OnClick="btnExcel_Click" />
                                            &nbsp;&nbsp;
                                        </div>
                                    </td>
                                </tr>
                            </Table>
                        </div>
                    </div>
                </div>

&nbsp;               
                <!--style="padding-left: 0px !important; padding-right: 0px !important; overflow-y: scroll; overflow-x: visible !important; height: 600px;-->
            <div id="aspGrid" class="col-xs-12 aspGrid div-datagrid" ">

                <asp:GridView ID="grvArticleList" runat="server" AutoGenerateColumns="false" RowStyle-Wrap="true" HeaderStyle-Wrap="true" OnRowDataBound="GridView1_RowDataBound" >
                    <HeaderStyle CssClass="GVFixedHeader" BackColor="#EBF3FF" Font-Size="10px" Font-Names="Arial,sans-serif"
                BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"   />
                    <Columns>

                            <asp:TemplateField HeaderText="Design" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
<!-->                                    <asp:Label class="LabelWide GVTextBoxBorder" ID="txtDesign2" runat="server"  Text='<%# Bind("design_no") %>'></asp:Label> -->
                                    <a target="_blank" href="DocAttachmentView.aspx?product=<%#Eval("design_no")%>" style="width: 20px">
                                        <%# Eval("design_no") %>
                                    </a>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Article"  HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label   class="LabelWide GVTextBoxBorder" ID="txtArticleName" runat="server"  Text='<%# Bind("article_name") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Weight (gsm)" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtWeight" runat="server"  Text='<%# Bind("grams_per_sqm") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Width (cm)" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtWidth" runat="server"  Text='<%# Bind("finished_width") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Gauge"  HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label   class="LabelWide GVTextBoxBorder" ID="txtGauge" runat="server"  Text='<%# Bind("design_guage") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="DIA"  HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label   class="LabelWide GVTextBoxBorder" ID="txtDia" runat="server"  Text='<%# Bind("dia") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        
                            <asp:TemplateField HeaderText="Material" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtMaterial" runat="server"  Text='<%# Bind("xl_material") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Description" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtDesc" runat="server"  Text='<%# Bind("xl_description") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Composition" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtComposition" runat="server"  Text='<%# Bind("xl_compo") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

 <%--                           <asp:TemplateField HeaderText="MIN Bht/KG" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtMinThbKG" runat="server"  Text='<%# Bind("min_thb_kg") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="MAX Bht/KG" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtMaxThbKG" runat="server"  Text='<%# Bind("max_thb_kg") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="FOB MIN USD/m" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtMinUSDKG" runat="server"  Text='<%# Bind("min_usd") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="FOB MID USD/m" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtMidUSDKG" runat="server"  Text='<%# Bind("mid_usd") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="FOB MAX USD/m" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtMaxUSDKG" runat="server"  Text='<%# Bind("max_usd") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="FOB MIN USD/m (W/L)" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtMinUSDKgWL" runat="server"  Text='<%# Bind("WL_MIN_USD") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="FOB MIN USD/m (M/D)" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtMinUSDKgMD" runat="server"  Text='<%# Bind("MD_MIN_USD") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        
                            <asp:TemplateField HeaderText="FOB MID USD/m (W/L)" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtMidUSDKgWL" runat="server"  Text='<%# Bind("WL_MID_USD") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="FOB MID USD/m (M/D)" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtMidUSDKgMD" runat="server"  Text='<%# Bind("MD_MID_USD") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="FOB MAX USD/m (W/L)" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtMaxUSDKgWL" runat="server"  Text='<%# Bind("WL_MAX_USD") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="FOB MID USD/m (M/D)" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide  GVTextBoxBorder" ID="txtMaxUSDKgMD" runat="server"  Text='<%# Bind("MD_MAX_USD") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>--%>


                            <asp:TemplateField HeaderText="Finishing" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide GVTextBoxBorder" ID="txtFinish" runat="server"  Text='<%# Bind("finishing") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Application" HeaderStyle-CssClass="header-center" >
                            <ItemTemplate>
                                    <asp:Label   class="LabelWide  GVTextBoxBorder" ID="txtApplList" runat="server"  Text='<%# Bind("appl_list") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Sub Application" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide  GVTextBoxBorder" ID="txtSubApplList" runat="server"  Text='<%# Bind("sub_appl_list") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Spl. Function" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide  GVTextBoxBorder" ID="txtSplFuncList" runat="server"  Text='<%# Bind("spl_func_list") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Family" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide  GVTextBoxBorder" ID="txtFamilyList" runat="server"  Text='<%# Bind("family_list") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Design Type" HeaderStyle-CssClass="header-center" >
                                <ItemTemplate>
                                    <asp:Label  class="LabelWide  GVTextBoxBorder" ID="txtDesignTypeList" runat="server"  Text='<%# Bind("design_type_list") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="itcatdesc" HeaderText="Category" HeaderStyle-CssClass="col header-center" ItemStyle-Wrap="true">
                                <ItemStyle CssClass="col" Width="50px" />
                            </asp:BoundField>

                            <asp:BoundField DataField="itsubcatdesc" HeaderText="Sub Category" HeaderStyle-CssClass="col header-center" ItemStyle-Wrap="true">
                                <ItemStyle CssClass="col" Width="80px" />
                            </asp:BoundField>

                            <asp:BoundField DataField="itgroupdesc" HeaderText="Group" HeaderStyle-CssClass="col header-center" ItemStyle-Wrap="true">
                                <ItemStyle CssClass="col" Width="80px" />
                            </asp:BoundField>

                            <asp:BoundField DataField="itsubdesc" HeaderText="Sub Group" HeaderStyle-CssClass="col header-center" ItemStyle-Wrap="true">
                                <ItemStyle CssClass="col" Width="80px" />
                            </asp:BoundField>

                            <asp:BoundField DataField="ittypedesc" HeaderText="Yarn Type" HeaderStyle-CssClass="col header-center" ItemStyle-Wrap="true">
                                <ItemStyle CssClass="col" Width="50px" />
                            </asp:BoundField>

                            <asp:BoundField DataField="itsubdesc2" HeaderText="Yarn Sub Type" HeaderStyle-CssClass="col header-center" ItemStyle-Wrap="true">
                                <ItemStyle CssClass="col" Width="50px" />
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

    <script type="text/javascript">

    (function (n) { jQuery.fn.extend({ gridviewScroll: function (t) { function yt(n, t) { pi(n, t), wi(n, t) } function pi(t, i) { t.find("input").each(function () { var t = n(this)[0].type, f, e; if (t == "checkbox" || t == "radio" || t == "text") { var r = n(this)[0].id, u = n(this)[0].name, r = r.replace("_Copy", ""), u = u.replace("_Copy", ""); n(this)[0].name = u + "_" + i, n(this)[0].id = r + "_" + i, f = n("#" + r), e = n(this); switch (t) { case "checkbox": case "radio": e.change(function () { var t = n(this).is(":checked"); f.attr("checked", t) }); break; case "text": e.change(function () { var t = n(this).val(); f.val(t) }) } } }) } function wi(t, i) { t.find("select").each(function () { var t = n(this)[0].id, r = n(this)[0].name, t = t.replace("_Copy", ""), r = r.replace("_Copy", ""), u, f; n(this)[0].name = r + "_" + i, n(this)[0].id = t + "_" + i, u = n("#" + t), f = n(this), f.change(function () { var n = this.selectedIndex; u.prop("selectedIndex", n) }) }) } function bi() { var a = l.attr("id") + "Freeze", o, y, v, h, b, c, k, e, s, d, f, t, g; for (document.getElementById(a) ? it = n("#" + a) : (it = l.clone(), it.attr("id", a), it.css({ position: "absolute", width: "", height: "100%", top: 0, left: 0, zIndex: li }), yt(it, "freezeheader"), it.appendTo(tt)), it.show(), o = it.children().eq(0), o.css({ width: "", height: "100%" }), o.find("td").each(function () { n(this)[0].style.display = "" }), y = o.children().eq(0), ki(), v = w[0].length, f = 0; f < i.headerrowcount; f++) for (t = 0; t < v; t++) (h = w[f][t], h != "RS" && h != "CS") && (b = parseInt(h.split(":")[1]), y.children().eq(f).children().eq(b)[0].style.display = t < ct ? "" : "none"); for (c = r.attr("id") + "Freeze", document.getElementById(c) ? p = n("#" + c) : (p = r.clone(), p.attr("id", c), p.css({ position: "absolute", width: "", top: 0, left: 0, zIndex: li }), yt(p, "freezeitem"), e = p.children().eq(0).children().eq(0), k = e.attr("id") + "Freeze", e.attr("id", k), p.appendTo(u)), e = p.children().eq(0).children().eq(0), e.css({ width: "" }), s = e.children().eq(0), s.find("td").each(function () { n(this)[0].style.display = "" }), d = s.children().length, f = i.headerrowcount; f < d; f++) for (t = 0; t < v; t++) t < ct ? s[0].rows[f].cells[t].style.display = "" : (g = s[0].rows[f].cells[t], g.style.display = "none"); o.width() < i.width - i.railsize ? (it.show(), p.show(), p.height(r.height())) : (it.hide(), p.hide()) } function ki() { var r = 0, n, t; for (ct = 0, n = 0; n < w[0].length; n++) { if (t = w[0][n], t == "RS" || t == "CS") { ct++; continue } if (i.freezesize == r) return ct; r++, ct++ } } function di() { for (var r = a.children().length, u = gi() + 1, t, n = i.headerrowcount; n < r; n++) t = a[0].rows[n].cells[0].children[0], t.style.height = u + "px" } function gi() { for (var e = a.children().eq(i.headerrowcount).children().length, t = 0, f, r, u, n = 0; n < e; n++) f = a.children().eq(i.headerrowcount).children().eq(n), r = f.children().eq(0), r[0].style.height = "auto", u = r[0].offsetHeight, u > t && (t = u); return t } function nr() { var it = f.attr("id") + "VerticalRail", r, ut, l, a, t, p, w, d, tt; document.getElementById(it) ? o = n("#" + it) : (o = n(nt).css({ background: i.railcolor, width: i.railsize + "px", position: "absolute", zIndex: ft }), o.attr("id", it), u.append(o), t = { right: 0 }, o.css(t), o.mousedown(function (t) { clearInterval(e); var i = n(this).offset(), r = t.clientY - i.top + n(document).scrollTop(), u = c.offset().top - i.top, f = c.height() + u; r < u && g(-1, !1, !0), r > f && g(1, !1, !0), e = window.setInterval(function () { var r = t.clientY - i.top + n(document).scrollTop(), u = c.offset().top - i.top, f = c.height() + u; r < u && g(-1, !1, !0), r > f && g(1, !1, !0) }, 200) }), o.mouseup(function () { clearInterval(e) }), o.mouseout(function () { clearInterval(e) })), r = f.attr("id") + "VerticalBar", document.getElementById(r) ? c = n("#" + r) : (c = n(nt).css({ background: i.barcolor, width: i.barsize + "px", position: "absolute", zIndex: ft }), c.attr("id", r), ut = { right: (i.railsize - i.barsize) / 2 }, c.css(ut), u.append(c), c.draggable({ axis: "y", containment: o, start: function () { n(this).css({ backgroundColor: i.barhovercolor }) }, stop: function () { n(this).css({ backgroundColor: i.barcolor }) }, drag: function () { g(0, !1) } })), l = f.attr("id") + "Vertical_TIMG", document.getElementById(l) ? v = n("#" + l) : (v = n(pt).css({ height: i.arrowsize, position: "absolute", zIndex: ft, top: 0 }), v.attr("id", l), v.attr("src", i.varrowtopimg), u.append(v), t = { right: 0 }, v.css(t), v.mousedown(function () { clearInterval(e), g(-1, !1, !0), e = window.setInterval(function () { g(-1, !1, !0) }, 200) }), v.mouseup(function () { clearInterval(e) }), v.mouseout(function () { clearInterval(e) })), a = f.attr("id") + "Vertical_BIMG", document.getElementById(a) ? y = n("#" + a) : (y = n(pt).css({ height: i.arrowsize, position: "absolute", zIndex: ft }), y.attr("id", a), y.attr("src", i.varrowbottomimg), u.append(y), t = { right: 0 }, y.css(t), y.mousedown(function () { clearInterval(e), g(1, !1, !0), e = window.setInterval(function () { g(1, !1, !0) }, 200) }), y.mouseup(function () { clearInterval(e) }), y.mouseout(function () { clearInterval(e) })), p = f.attr("id") + "HorizontalRail", document.getElementById(p) ? s = n("#" + p) : (s = n(nt).css({ background: i.railcolor, height: i.railsize + "px", position: "absolute", zIndex: ft }), s.attr("id", p), u.append(s), s.mousedown(function (t) { clearInterval(e); var i = n(this).offset(), r = t.clientX - i.left + n(document).scrollLeft(), u = h.offset().left - i.left, f = h.width() + u; r < u && rt(-1, !0), r > f && rt(1, !0), e = window.setInterval(function () { var r = t.clientX - i.left + n(document).scrollLeft(), u = h.offset().left - i.left, f = h.width() + u; r < u && rt(-1, !0), r > f && rt(1, !0) }, 200) }), s.mouseup(function () { clearInterval(e) }), s.mouseout(function () { clearInterval(e) })), w = f.attr("id") + "HorizontalBar", document.getElementById(w) ? h = n("#" + w) : (h = n(nt).css({ background: i.barcolor, height: i.barsize + "px", position: "absolute", zIndex: ft }), h.attr("id", w), u.append(h), h.draggable({ axis: "x", containment: s, start: function () { n(this).css({ backgroundColor: i.barhovercolor }) }, stop: function () { n(this).css({ backgroundColor: i.barcolor }) }, drag: function () { rt() } })), d = f.attr("id") + "Horizontal_LIMG", document.getElementById(d) ? b = n("#" + d) : (b = n(pt).css({ width: i.arrowsize, position: "absolute", zIndex: ft, top: 0 }), b.attr("id", d), b.attr("src", i.harrowleftimg), u.append(b), b.mousedown(function () { clearInterval(e), rt(-1, !0), e = window.setInterval(function () { rt(-1, !0) }, 200) }), b.mouseup(function () { clearInterval(e) }), b.mouseout(function () { clearInterval(e) })), tt = f.attr("id") + "Horizontal_RIMG", document.getElementById(tt) ? k = n("#" + tt) : (k = n(pt).css({ width: i.arrowsize, position: "absolute", zIndex: ft }), k.attr("id", tt), k.attr("src", i.harrowrightimg), u.append(k), k.mousedown(function () { clearInterval(e), rt(1, !0), e = window.setInterval(function () { rt(1, !0) }, 200) }), k.mouseup(function () { clearInterval(e) }), k.mouseout(function () { clearInterval(e) })) } function tr() { var n, t; o.css({ height: u.outerHeight() - i.railsize - i.arrowsize * 2, top: i.arrowsize }), c.css({ top: i.arrowsize }), s.css({ width: u.outerWidth() - i.railsize - i.arrowsize * 2, top: u.outerHeight() - i.railsize, left: i.arrowsize }), h.css({ top: u.outerHeight() - (i.railsize + i.barsize) / 2, left: i.arrowsize }), n = Math.max(r.outerHeight() / r[0].scrollHeight * o.outerHeight(), i.minscrollbarsize), c.css({ height: n + "px" }), t = Math.max(r.outerWidth() / r[0].scrollWidth * s.outerWidth(), i.minscrollbarsize), h.css({ width: t + "px" }), n + i.arrowsize * 2 >= r.outerHeight() ? (o.hide(), c.hide(), v.hide(), y.hide(), fi = !0) : (o.show(), c.show(), v.show(), y.show()), t + i.arrowsize * 2 >= r.outerWidth() ? (s.hide(), h.hide(), b.hide(), k.hide()) : (s.show(), h.show(), b.show(), k.show()), o.is(":hidden") && (tt.css({ width: ut }), l.css({ width: ut }), u.css({ width: ut }), r.css({ width: ut }), s.css({ width: ut - i.arrowsize * 2 }), t = Math.max(r.outerWidth() / r[0].scrollWidth * s.outerWidth(), i.minscrollbarsize), h.css({ width: t + "px" }), s.css({ top: u.height() - i.railsize }), h.css({ top: u.height() - (i.railsize + i.barsize) / 2 })), s.is(":hidden") && (o.is(":hidden") ? (u.css({ height: f.height() }), r.css({ height: f.height() })) : (u.css({ height: ot }), r.css({ height: ot })), o.css({ height: ot - i.arrowsize * 2 }), n = Math.max(r.outerHeight() / r[0].scrollHeight * o.outerHeight(), i.minscrollbarsize), c.css({ height: n + "px" })), v.css({ top: 0 }), y.css({ top: o.outerHeight() + i.arrowsize }), b.css({ top: r.outerHeight() }), k.css({ top: r.outerHeight(), left: s.outerWidth() + i.arrowsize }), i.arrowsize == 0 && (v.hide(), y.hide(), b.hide(), k.hide()) } function g(n, t, u) { var f = n, e, s, l; (t || u) && (e = 0, e = t ? n * parseInt(i.wheelstep) / 100 : n, f = parseInt(c.css("top")) + e * (r.outerHeight() / r[0].scrollHeight * o.outerHeight() - 1), s = r.outerHeight() - c.outerHeight() - i.arrowsize, f = Math.min(Math.max(f, i.arrowsize), s), c.css({ top: f + "px" })), l = (parseInt(c.css("top")) - i.arrowsize) / (o.outerHeight() - c.outerHeight()), f = l * (r[0].scrollHeight - r.outerHeight()), r.scrollTop(f), i.freezesize == 0 || h.is(":hidden") || (f + r.outerHeight() > r[0].scrollHeight && (f = r[0].scrollHeight - r.outerHeight()), p.scrollTop(f)) } function rt(n, t) { var u = n, f, e, o; t && (f = n, u = parseInt(h.css("left")) + f * (r.outerWidth() / r[0].scrollWidth * s.outerWidth() - 1), e = r.outerWidth() - h.outerWidth() - i.arrowsize, u = Math.min(Math.max(u, i.arrowsize), e), h.css({ left: u + "px" })), o = (parseInt(h.css("left")) - i.arrowsize) / (s.outerWidth() - h.outerWidth()), u = o * (r[0].scrollWidth - r.outerWidth()), u + l.outerWidth() > l[0].scrollWidth && (u = l[0].scrollWidth - l.outerWidth()), r.scrollLeft(u), l.scrollLeft(u) } function ir() { var u, r, t; if (st.show(), i.headerrowcount > 1) for (t = 1; t < i.headerrowcount; t++) a.children().eq(t).show(); if (lt.find("td").each(function (t) { n(this).children().eq(0).css("width", "auto"), ht.children().eq(t).children().eq(0).css("width", "auto"), n(this).children().eq(0).css("width", "auto") }), u = et.children().eq(0), r = [], lt.find("td").each(function (t) { var i = n(this)[0].childNodes[0].offsetWidth + 1; r[t] = i, n(this).children().eq(0).css({ width: i }) }), rr(), lt.find("td").each(function (n) { for (var s = r[n], f, e, o, t = 0; t < i.headerrowcount; t++) (f = w[t][n], f != "RS" && f != "CS") && (e = f.split(":")[0], e != "N") && (o = f.split(":")[1], u.children().eq(t).children().eq(o).children().eq(0).css({ width: s })) }), st.hide(), i.headerrowcount > 1) for (t = 1; t < i.headerrowcount; t++) a.children().eq(t).hide() } function rr() { for (var n = [], t = 0; t < i.headerrowcount; t++) w[t] = [], n[t] = 0; return lt.find("td").each(function (t) { for (var o, e, f, u, r = 0; r < i.headerrowcount; r++) if (w[r][t] != "RS" && w[r][t] != "CS") { if (o = a.children().eq(r).children().eq(n[r]), n[r]++, e = o.attr("rowspan"), f = o.attr("colspan"), e = e ? parseInt(e) : 1, f = f ? parseInt(f) : 1, e != 1) for (u = r; u < e; u++) w[u][t] = "RS"; if (f != 1) { for (u = 1; u < f; u++) w[r][u + t] = "CS"; w[r][t] = "N:" + (n[r] - 1) } f == 1 && (w[r][t] = "Y:" + (n[r] - 1)) } }), w } function ur() { a.find("td").each(function () { for (var r = n(this)[0].childNodes.length, i, t = 0; t < r; t++) i = n(this)[0].childNodes[t], i.tagName == "DIV" && alert(i.style.position); n(this)[0].innerHTML = "<div>" + n(this)[0].innerHTML + "<\/div>" }) } var i = n.extend({ width: 500, height: 300, railcolor: "#F0F0F0", barcolor: "#CDCDCD", barhovercolor: "#606060", bgcolor: "#F0F0F0", freezesize: 0, arrowsize: 0, varrowtopimg: "", varrowbottomimg: "", harrowleftimg: "", harrowrightimg: "", headerrowcount: 1, railsize: 15, barsize: 15, wheelstep: 20, minscrollbarsize: 30 }, t), e = null, hi = !0, a = null, st = null, lt = null, at = !1, fi = !1, nt = "<div><\/div>", pt = "<img />", tt = null, u = null, l = null, r = null, o = null, c = null, v = null, y = null, s = null, h = null, b = null, k = null, it = null, p = null, et = null, ht = null, f = null, wt = null, d = null, w = [], ci = 0, li = 0, ft = 0, ct = -1, f = n(this), bt, kt, dt, gt, ni, ti, ii, oi, ot, vt, si, vi, ui, yi; if (f[0] && (a = f.children().eq(0), !(a.children().length < 2))) { n.browser.msie && jQuery.browser.version == "7.0" || f.css({ position: "relative" }); var ut = i.width, fr = i.height, ei = f.attr("id") + "Wrapper"; if (document.getElementById(ei) ? wt = n("#" + ei) : (wt = n(nt), wt.attr("id", ei), f.parent().wrap(wt)), bt = f.attr("id") + "PanelHeader", document.getElementById(bt) ? tt = n("#" + bt) : (tt = n(nt), tt.attr("id", bt), f.parent().before(tt)), tt.css({ background: i.bgcolor }), kt = f.attr("id") + "PanelItem", document.getElementById(kt) ? u = n("#" + kt) : (u = n(nt), u.attr("id", kt), f.parent().before(u)), u.css({ background: i.bgcolor }), dt = f.attr("id") + "PanelHeaderContent", document.getElementById(dt) ? (l = n("#" + dt), l.scrollLeft(0), l.scrollTop(0)) : (l = n(nt).css({ background: "#FFFFFF" }), l.attr("id", dt), l.appendTo(tt)), gt = f.attr("id") + "PanelItemContent", document.getElementById(gt) ? (r = n("#" + gt), r.scrollLeft(0), r.scrollTop(0)) : (r = n(nt).css({ background: "#FFFFFF" }), r.attr("id", gt), r.appendTo(u), f.parent().appendTo(r)), st = a.children().eq(0), st.attr("id", f.attr("id") + "Header"), lt = a.children().eq(i.headerrowcount), ni = f.attr("id") + "Copy", document.getElementById(ni) ? et = n("#" + ni) : (et = f.clone().children().remove().end(), et.attr("id", ni), et.appendTo(l), ur(), hi = !1), ti = st.attr("id") + "Copy", document.getElementById(ti)) ht = n("#" + ti); else if (ht = st.clone(!1), ht.attr("id", ti), yt(ht, "Copy"), ht.appendTo(et), i.headerrowcount > 1) for (ii = 1; ii < i.headerrowcount; ii++) oi = a.children().eq(ii).clone(!1), yt(oi, "Copy"), oi.appendTo(et); if (ot = fr - tt.height(), vt = f.attr("id") + "PagerBottom", document.getElementById(vt)) d = n("#" + vt), d[0] && d.width(ut); else { var ri = a.children().eq(a.children().length - 1), er = ri.children().eq(0), ai = er.children().eq(0).children().eq(0); ai[0] != null && ai[0].tagName == "TABLE" && (document.getElementById(vt) || (d = n(nt), d.attr("id", vt), d.addClass(ri[0].className), u.after(d), ri.children().eq(0).appendTo(d), d.width(ut)), ri.remove()) } if (d && d[0] && (ot -= d.height()), u.css({ position: "relative", overflow: "hidden", width: ut, height: ot }), r.css({ overflow: "hidden", width: u.outerWidth() - i.railsize, height: ot - i.railsize, zIndex: ci }), tt.css({ position: "relative", overflow: "hidden", width: ut }), l.css({ overflow: "hidden", width: u.outerWidth() - i.railsize, zIndex: ci }), ir(), i.freezesize != 0 && di(), nr(), tr(), i.freezesize == 0 || s.is(":hidden") ? (si = l.attr("id") + "Freeze", vi = r.attr("id") + "Freeze", document.getElementById(si) && (n("#" + si).hide(), n("#" + vi).hide())) : bi(), !hi) return r.hover(function () { at = !0 }, function () { at = !1 }), i.freezesize == 0 || s.is(":hidden") || p.hover(function () { at = !0 }, function () { at = !1 }), ui = function (n) { if (at && !o.is(":hidden")) { var n = n || window.event, t = 0; n.wheelDelta && (t = -n.wheelDelta / 120), n.detail && (t = n.detail / 3), g(t, !0), n.preventDefault && !fi && n.preventDefault(), fi || (n.returnValue = !1) } }, yi = function () { window.addEventListener ? (this.addEventListener("DOMMouseScroll", ui, !1), this.addEventListener("mousewheel", ui, !1)) : document.attachEvent("onmousewheel", ui) }, yi(), this } } }), jQuery.fn.extend({ gridviewScroll: jQuery.fn.gridviewScroll }) })(jQuery)

        var windowSize;

        // Don't try this - $(window).load(function ()
        function pageLoad() {
            windowSize = $(window).width();
            gridviewScroll();
        }

        $(window).resize(function () {
            windowSize = $(window).width();
            gridviewScroll();
        });

        function gridviewScroll() {
            $('#<%=grvArticleList.ClientID%>').gridviewScroll({
                width: windowSize-44,
                height: 500,
                freezesize: 3
            });
        } 
    </script>

    <body onkeydown="if(event.keyCode==13){event.keyCode=9; return event.keyCode}">
</asp:Content>
