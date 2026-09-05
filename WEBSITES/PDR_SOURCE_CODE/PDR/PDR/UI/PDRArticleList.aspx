<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PDRArticleList.aspx.cs" Inherits="PDR.UI.PDRArticleList" %>

<%@ Register assembly="Syncfusion.EJ" namespace="Syncfusion.JavaScript.Models" tagprefix="ej" %>

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
                        <asp:Label ID="Label1" runat="server" Text="ARTICLE / DESIGN LIST"></asp:Label></small></h3>
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
                                        <asp:Label ID="Label12"  runat="server" Text="COMPOSITION" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:TextBox ID="txtCompos" Style="width: 280px" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label5" runat="server" Text="DESIGN&nbsp;DATE&nbsp;FROM" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <input class="datepicker TextBox" id="dtFromDate" type="text" size="10" name="dtFromDate" runat="server" style="width: 90px">
                                    </td>
                                    <td>
                                        <asp:Label ID="Label11"  runat="server" Text="WIDTH(CM) FROM" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:TextBox ID="txtWidthFrom" Style="width: 80px" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);"></asp:TextBox>
                                    </td>
                                </tr>

                                <tr style="line-height: 5px !important;">
                                    <td>
                                        <asp:Label ID="Label3" runat="server" Text="CATEGORY" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlcategory" runat="server" Style="width: 170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label10" runat="server" Text="DESIGN&nbsp;DATE&nbsp;TO" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <input class="datepicker TextBox" id="dtTo" type="text" size="10" name="dtTo" runat="server" style="width: 90px">
                                    </td>
                                    <td>
                                        <asp:Label ID="Label13"  runat="server" Text="WIDTH(CM) TO " CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:TextBox ID="txtWidthTo" Style="width: 80px" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);"></asp:TextBox>
                                    </td>
                                </tr>

                                <tr style="line-height: 5px !important;">
                                    <td>
                                        <asp:Label ID="Label7" runat="server" Text="SUB&nbsp;CATEGORY" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlSubCategory" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label4" runat="server" Text="FABRIC&nbsp;GROUP" CssClass="clsFromLabel"></asp:Label>
                                    </td>

                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlGroup" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label14"  runat="server" Text="Yield (MT/KG) FROM " CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:TextBox ID="txtWeightFrom" Style="width: 80px" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);"></asp:TextBox>
                                    </td>

                                </tr>

                                <tr style="line-height: 5px !important;">
                                    <td>
                                        <asp:Label ID="Label6" runat="server" Text="APPLICATION" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlApplication" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label8" runat="server" Text="FABRIC&nbsp;SUB&nbsp;GROUP" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlSubGroup" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label15"  runat="server" Text="Yield (MT/KG) TO " CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:TextBox ID="txtWeightTo" Style="width: 80px" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr style="line-height: 5px !important;">
                                    <td>
                                        <asp:Label ID="Label16" runat="server" Text="APPLICATION" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <ej:DropDownList ID="ddlAppl" runat="server" DataTextField="lookup_value" ShowCheckbox="true"  Width="10%" MultiSelectMode="Delimiter"></ej:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label17" runat="server" Text="FABRIC&nbsp;SUB&nbsp;GROUP" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="DropDownList3" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label18"  runat="server" Text="Yield (MT/KG) TO " CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:TextBox ID="TextBox1" Style="width: 80px" runat="server" CssClass="TextBox" onkeypress="(event.keyCode ==13);"></asp:TextBox>
                                    </td>
                                </tr>

                                <tr style="line-height: 5px !important;">
                                    <td>
                                        <asp:Label ID="Label9" runat="server" Text="SUB&nbsp;APPLICATION" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlSubApplication" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Text="FAMILY NAME" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td class="bordercol">
                                        <asp:DropDownList ID="ddlFamilyName" runat="server" Width="170px" CssClass="combo"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblDRWaitingSO" runat="server" Text="DR WAITING SO" CssClass="clsFromLabel"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkdrwaitingso" runat="server" Width="170px" CssClass="checkbox"></asp:CheckBox>
                                    </td>

                                </tr>
                                <tr>
                                    <td colspan="10">
                                        <div class="pull-right" style="margin-right: 150px;">
                                            <asp:Button ID="btnFind" runat="server" Text="Find" CssClass="btn btn-success btnfont" OnClick="btnFind_Click" />&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-success btnfont" OnClick="btnClear_Click" />
                                            &nbsp;&nbsp;
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
               

            <div class="col-xs-12" style="padding-left: 0px !important; padding-right: 0px !important; overflow-x: visible !important; height: 100%;">
                   <ej:grid ID="grdArticleList" runat="server" AllowScrolling="true" AllowPaging="false" >
                       <ScrollSettings Height="100%" Width="100%" ></ScrollSettings> 
                       <FilterSettings FilterType="Excel" />  
                       <Columns>
                            <ej:Column Field="design_no" HeaderText="Design No." Type="string" Width="150"/>
                            <ej:Column Field="article_name" HeaderText="Article Name"  Type="string" Width="150"  />
                            <ej:Column Field="appl_list" Type="string" HeaderText="Appl."  Width="300" />
                            <ej:Column Field="sub_appl_list" Type="string" HeaderText="Sub Appl."  Width="300" />
                            <ej:Column Field="spl_func_list" Type="string" HeaderText="Func"  Width="300" />
                            <ej:Column Field="family_list" Type="string" HeaderText="Family"  Width="300" />
                            <ej:Column Field="design_type_list" Type="string" HeaderText="Design Type"  Width="150" />

                            <ej:Column Field="finished_width"  Type="string" HeaderText="Width"  Width="100" />
                            <ej:Column Field="grams_per_sqm"  Type="string" HeaderText="Weight"   Width="100"/>
                            <ej:Column Field="finished_yield"  Type="number" HeaderText="Mt/Kg"   Width="100"/>
                            <ej:Column Field="finishing_code"  Type="string" HeaderText="Finish Code"   Width="100"/>
                            <ej:Column Field="finishing"  Type="string" HeaderText="Finish" Width="100"/>

                            <ej:Column Field="xl_description"  Type="string" HeaderText="Description" Width="400"/>
                            <ej:Column Field="fabric_composition"  Type="string" HeaderText="Composition"  Width="400" />
                            <ej:Column Field="MIN_THB_KG"  Type="string" HeaderText="MIN THB/KG" Width="150"/>
                            <ej:Column Field="MAX_THB_KG"  Type="string" HeaderText="MAX THB/KG" Width="150"/>

                            <ej:Column Field="MIN_USD"  Type="string" HeaderText="MIN USD" Width="150"/>
                            <ej:Column Field="MID_USD"  Type="string" HeaderText="MID USD" Width="150"/>
                            <ej:Column Field="MAX_USD"  Type="string" HeaderText="MAX USD" Width="150"/>

                            <ej:Column Field="WL_MIN_USD"  Type="string" HeaderText="W/L MIN USD" Width="150"/>
                            <ej:Column Field="MD_MIN_USD"  Type="string" HeaderText="M/D MIN USD" Width="150"/>
                            <ej:Column Field="WL_MID_USD"  Type="string" HeaderText="W/L MID USD" Width="150"/>
                            <ej:Column Field="MD_MID_USD"  Type="string" HeaderText="M/D MID USD" Width="150"/>
                            <ej:Column Field="WL_MAX_USD"  Type="string" HeaderText="W/L MAX USD" Width="150"/>
                            <ej:Column Field="MD_MAX_USD"  Type="string" HeaderText="M/D MAX USD" Width="150"/>

                            <ej:Column Field="itcatdesc"  Type="string" HeaderText="Category"   Width="150"/>
                            <ej:Column Field="itsubcatdesc"  Type="string" HeaderText="Sub Category"   Width="150"/>
                            <ej:Column Field="itgroupdesc"  Type="string" HeaderText="Items Group"  Width="150" />
                            <ej:Column Field="itsubdesc"  Type="string" HeaderText="Sub Group"   Width="150"/>
                            <ej:Column Field="ittypedesc"  Type="string" HeaderText="Yarn Type"   Width="150"/>
                            <ej:Column Field="itsusbdesc2"  Type="string" HeaderText="Yarn Sub"    Width="150"/>
                       </Columns>             
                   </ej:grid>                    
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
