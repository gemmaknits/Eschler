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
                

            <div class="col-xs-12" style="padding-left: 0px !important; padding-right: 0px !important; overflow-x: visible !important; height: 100%;">
                   <ej:grid ID="grdArticleList" runat="server" AllowFiltering="true" AllowScrolling="true" AllowPaging="true">
                       <ScrollSettings Height="100%" Width="100%" ></ScrollSettings> 
                       <FilterSettings FilterType="Excel" />  
                       <Columns>
                            <ej:Column Field="design_no" HeaderText="Design No." Width="60"/>
                            <ej:Column Field="article_name" HeaderText="Article Name" Width="100"  />
                            <ej:Column Field="fabric_composition" HeaderText="Composition"  Width="100" />
                            <ej:Column Field="itptype" HeaderText="Appl Func"  Width="100" />
                            <ej:Column Field="itpvalue" HeaderText="Appl Func"  Width="100" />
                            <ej:Column Field="finished_width" HeaderText="Width"  Width="100" />
                            <ej:Column Field="grams_per_sqm" HeaderText="Weight"   Width="100"/>
                            <ej:Column Field="finished_yield" HeaderText="Mt/Kg"   Width="100"/>
                            <ej:Column Field="itcatdesc" HeaderText="Category"   Width="100"/>
                            <ej:Column Field="itsubcatdesc" HeaderText="Sub Category"   Width="100"/>
                            <ej:Column Field="itgroupdesc" HeaderText="Items Group"  Width="100" />
                            <ej:Column Field="itsubdesc" HeaderText="Sub Group"   Width="100"/>
                            <ej:Column Field="ittypedesc" HeaderText="Yarn Type"   Width="100"/>
                            <ej:Column Field="itsusbdesc2" HeaderText="Yarn Sub"    Width="100"/>
                            <ej:Column Field="finishing_code" HeaderText="Finish Code"   Width="100"/>
                            <ej:Column Field="finishing" HeaderText="Finish" Width="100"/>
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
