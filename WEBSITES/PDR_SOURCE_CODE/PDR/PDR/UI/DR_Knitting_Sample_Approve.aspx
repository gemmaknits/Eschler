<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DR_Knitting_Sample_Approve.aspx.cs" Inherits="PDR.UI.DR_Knitting_Sample_Approve" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hidapprejID" runat="server" />
    <asp:HiddenField ID="hidkono" runat="server" />
    <asp:HiddenField ID="hidpdr_new_develop_req_id" runat="server"></asp:HiddenField>
    <input type="hidden" id="hidrowindex" value="" runat="server" />
    <input type="hidden" id="hidpageloadindex" value="" runat="server" />
    <input type="hidden" id="hidsource_doc_no" runat="server" value="" />
     <input type="hidden" id="hidseq" runat="server" value="" />
    <input type="hidden" id="hidcomment" runat="server" value="" />
    <div style="position: absolute; top: 90px; left: 10px">
        <h4> DR KNITTING SAMPLE APPROVAL</h4>
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
                    <asp:LinkButton ID="MyLnkButton" runat="server" OnClick="DetailsPDRINPDF_click" Text="" CssClass="TextBox" Width="90px"></asp:LinkButton>
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
                    <a target="_blank" href="YarnLots.aspx">
                        <asp:LinkButton ID="kikono" OnClientClick="return ValidateRange()" runat="server" Width="90px" Text="" CssClass="TextBox" OnClick="kikono_Click"></asp:LinkButton></a>
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
               <tr>
                <td>
                    <asp:Label ID="Label6" runat="server" Text="KNITTING REMARK" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TXTKNITTINGREMARK" runat="server" CssClass="TextBox" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
        </table>
     </div>
    <div style="position: absolute; top: 320px; left: 5px">
        <table>
            <tr>
                <td>
                    <asp:GridView ID="grdSampleKnittingApproval" runat="server" AutoGenerateColumns="false" GridLines="None"
                        EmptyDataText="There is No Records To Display" Width="76px" DataKeyNames="mfg_sample_dispo_id,app_rej_comment,app_rej_id"
                        OnRowDataBound="gvorder_issues_OnRowDataBound" OnSelectedIndexChanged="OnSelectedIndexChanged"
                        CssClass="table">
                        <HeaderStyle CssClass="grdheader" />
                        <Columns>
                            <asp:BoundField DataField="mfg_sample_dispo_id" HeaderText="" HeaderStyle-Width="2px" ItemStyle-CssClass="hidden" HeaderStyle-CssClass="hidden"></asp:BoundField>
                            <asp:BoundField DataField="dispo_Seq" HeaderStyle-Width="2px" HeaderText="#" HeaderStyle-CssClass="col" ItemStyle-CssClass="col" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false" ReadOnly="True">
                                <ItemStyle CssClass="col" Width="2px" />
                            </asp:BoundField>
                             <asp:BoundField DataField="sample_dispo_comment" HeaderText="" HeaderStyle-Width="2px" ItemStyle-CssClass="hidden" HeaderStyle-CssClass="hidden"></asp:BoundField>
                            <asp:BoundField DataField="CREATION_DATE" ItemStyle-Wrap="false" HeaderStyle-Width="120px" ItemStyle-Width="120px" HeaderText="DATE" DataFormatString="{0:d}" HeaderStyle-CssClass="co" ItemStyle-CssClass="col" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false" ReadOnly="True"></asp:BoundField>
                            <asp:TemplateField HeaderStyle-Width="40px" HeaderText="QTY<br /> (MTS)" HeaderStyle-CssClass="numericcol" ItemStyle-CssClass="numericcol " HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="QTYMTS" runat="server" AutoPostBack="false" onkeydown="(event.keyCode!=13);" Text='<%# Eval("secondary_quantity") %>' CssClass="TextBox numericcol old" Width="40px" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-Width="130px" HeaderText="SAMPLE COMMENT" HeaderStyle-CssClass="col" ItemStyle-CssClass="col" HeaderStyle-Font-Bold="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="SAMPLEDISPOCOMMENT" runat="server" TextMode="MultiLine" Height="40px" onkeydown="(event.keyCode!=13);" ReadOnly="TRUE" onkeypress="(event.keyCode!=13);" Text='<%# Eval("sample_dispo_comment") %>' CssClass="TextBox" Width="130px" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="80px" HeaderText="APP/REJ" HeaderStyle-CssClass="col" ItemStyle-CssClass="col" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlapprej" runat="server" Height="25px" Width="80px" DataTextField="lookup_value_id" DataValueField="lookup_value" Style="border: none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-Width="140px" HeaderText="APP/REJ COMMENT" HeaderStyle-CssClass="col" ItemStyle-CssClass="col" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="apprejcomment" runat="server" TextMode="MultiLine" Height="40px" AutoPostBack="false" onkeydown="(event.keyCode!=13);" Text='<%# Eval("APP_REJ_COMMENT") %>' CssClass="col" Width="140px" />
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td>&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<asp:Button ID="btnDone" runat="server" Text="DONE" CssClass="button" OnClick="btnDone_Click" Height="35px" />
                    <a target="_blank">
                        <asp:ImageButton ID="ImgAttachment" Width="35px" Height="35px" ToolTip="ATTCHMENT" runat="server" ImageUrl="~/Images/Attachment.png"
                            OnClientClick="return checkSampleDispoID ()" OnClick="ImgAttachment_Click" ImageAlign="Top" /></a>
                    <a target="_blank">
                        <asp:ImageButton ID="ImgEmal" Width="35px" Height="35px" ToolTip="EMail" runat="server" ImageUrl="~/Images/EMail.jpg"
                            OnClientClick="return ValidateRange();" OnClick="ImgEmal_Click" ImageAlign="Top" /></a>
                </td>
            </tr>
            <tr>
                &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Label ID="lblmsg" runat="server" Text="SAMPLE CREATED" Visible="False" ForeColor="#009933"></asp:Label>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <asp:GridView ID="grdAttachmentDetails" runat="server" AutoGenerateColumns="false" GridLines="None"
                    Width="132%">
                    <HeaderStyle CssClass="grdheader" />
                    <Columns>
                        <asp:BoundField ItemStyle-Width="80px" DataField="file_Description" HeaderText="DESCRIPTION" ItemStyle-Wrap="false" ItemStyle-CssClass="col" HeaderStyle-CssClass="col">
                            <ItemStyle CssClass="col" />
                        </asp:BoundField>
                        <asp:BoundField DataField="file_location_disp" HeaderText="LOCATION" ReadOnly="True" HeaderStyle-CssClass="col">
                            <ItemStyle CssClass="col" Width="20px" />
                        </asp:BoundField>
                        <asp:TemplateField>
                            <HeaderStyle Width="20" />
                            <ItemTemplate>
                                <asp:ImageButton ID="img" ImageUrl="~/Images/downloadfile.png" ControlStyle-Width="14px" ControlStyle-Height="14px" CommandName="Download"
                                    CommandArgument='<%#((GridViewRow)Container).RowIndex%>' ToolTip="Download" runat="server" Height="14" Width="14" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </tr>
        </table>
    </div>
    <style type="text/css">
        .hiddencol {
            display: none;
        }
    </style>
    <style>
        .hidden {
            display: none;
        }
    </style>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $("[id*=grdSampleKnittingApproval] td").click(function () {
                DisplayDetails($(this).closest("tr"));
            });
        });
        function DisplayDetails(row) {
            var message = "";
            var pdfno = "";
            var seq;
            var comment;
            message += "Id: " + $("td", row).eq(0).html();
            message += "\nName: " + $("td", row).eq(1).html();
            //pdfno = "\nDescription: " + $("td", row).eq(2).html();
            sample_dispo_id = $("td", row).eq(0).html();
            seq = $("td", row).eq(1).html();
            comment = $("td", row).eq(2).html();
            document.getElementById("ContentPlaceHolder1_hidsource_doc_no").value = $.trim(sample_dispo_id);
            document.getElementById("ContentPlaceHolder1_hidseq").value = $.trim(seq);
            document.getElementById("ContentPlaceHolder1_hidcomment").value = $.trim(comment);
           // alert(sample_dispo_id);
            $('#grdAttachmentDetails').empty();
            load_Data(sample_dispo_id);
        }
        $("input.old").live('focus', function () { $(this).select(); });
    </script>
    <script>
        function load_Data(sampledispoID) {
            $.ajax({
                type: "POST",
                url: "DR_Knitting_Sample_Approve.aspx/GetData",
                contentType: "application/json;charset=utf-8",
                data: "{'source_doc_no':'" + sampledispoID + "'}",
                dataType: "json",
                success: function (data) {
                    $("#ContentPlaceHolder1_grdAttachmentDetails").empty();
                    //alert(data.d.length);
                    //if (!data) {
                    if ((data.d.length) > 0) {
                        // alert(data.d.length);
                        $("#ContentPlaceHolder1_grdAttachmentDetails").append("<tr class='grdheader'><th class='col'>DESCRIPTION</th> <th class='col'>LOCATION</th><th></th> </tr>");
                        for (var i = 0; i < (data.d.length) ; i++) {
                            $("#ContentPlaceHolder1_grdAttachmentDetails").append("<tr><td class='col'>" +
                            data.d[i].file_description + "</td> <td class='col'>" +
                            data.d[i].file_location_disp + "</td> <td class='col'>" +
                             "<input type='image'  title='Download' src='../Images/downloadfile.png' style='height:14px;width:14px;'  onclick='smaoledownload(this);return false;'  data-id1='" + data.d[i].file_location + "' data-id2='" + data.d[i].doc_attachments_id + "' />" + "</td> <td class='col'>" +
                            "</td></tr>");
                            // alert($("#ContentPlaceHolder1_grdAttachmentDetails"));
                        }
                    }
                    // }
                },
                error: function (result) {
                    alert("Error login");

                }
            });
        }
    </script>
    <script>
        function checkSampleDispoID() {
            var sampledispoID;
            sampledispoID = document.getElementById("ContentPlaceHolder1_hidsource_doc_no").value;
            if (eval(sampledispoID) != -1) {
                ValidateRange();
            }
            else {
                alert("Sample DIspo ID is -1 , So cann't do attachment");
                return false;
            }

        }
    </script>
    <script type="text/javascript">
        function smaoledownload(grdattachment) {
            // alert("DD");
            var row = grdattachment.parentNode.parentNode;
            var rowIndex = row.rowIndex - 1;
            //  alert(rowIndex);
            var file_location = $(grdattachment).attr('data-id1');
            var attachmentid = $(grdattachment).attr('data-id2');

          //  alert(file_location);
           // alert(attachmentid);
            var h = 200;
            //var myWindow = window.open("ViewYarnLot.aspx?kino=" + kino, "MsgWindow", "width=400, height=500");
            // return window.open("ViewYarnLot.aspx?kino=" + kino, "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
            var left = "200px";// (screen.width / 2) - (popupWidth / 2);
            var top = "200px";//(screen.height / 2) - (popupHeight / 2);
            var pageURL = "Sample_DownloadFile.aspx?file_location=" + file_location;
            var popupWidth = "50%";
            var popupHeight = "50%";
            //alert("FF");
            var targetPop = window.open(pageURL, '', 'toolbar=no, location=no, directories=no,status=no, menubar=no, scrollbars=YES, resizable=YES, copyhistory=no,width=1200, height=300, top=150, left=50');


            $.ajax({
                type: "POST",
                url: "DR_Sample_Knitting.aspx/downloadAttachment",
                data: "{'file_location':'" + file_location + "','sourcedocno':'" + attachmentid + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccess,
                failure: function (response) {
                    // alert(response.d);
                },
                error: function (response) {
                    //alert(response.d);
                }
            });

            $.ajax({
                type: "post",
                url: "dr_sample_knitting.aspx/downloadattachment",
                contenttype: "application/json;charset=utf-8",
                data: "{'file_location':'" + file_location + "','sourcedocno':'" + attachmentid + "'}",
                datatype: "json",
                success: function (response) {
                    alert("sucess");
                },
                failure: function (response) {
                    alert(response.d);
                }
            });
        }
    </script>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        var prevRowIndex;
        function ChangeRowColor(row, rowIndex, poc_header_id) {
            var parent = document.getElementById(row);
            var currentRowIndex = parseInt(rowIndex) + 1;
            // alert(poc_header_id);
            if (prevRowIndex == currentRowIndex) {
                return;
            }
            else if (prevRowIndex != null) {
                parent.rows[prevRowIndex].style.backgroundColor = "#FFFFFF";
            }
            parent.rows[currentRowIndex].style.backgroundColor = "#ffff00"; //selected row color
            prevRowIndex = currentRowIndex;
            //document.getElementById("ContentPlaceHolder1_hidrowpdf").value = currentRowIndex;
            //document.getElementById("ContentPlaceHolder1_hidparentrow").value = parent;
            //alert(document.getElementById("ContentPlaceHolder1_hidrowpdf").value);
            $('#<%= grdSampleKnittingApproval.ClientID %>').val(row);
            $('#<%= grdSampleKnittingApproval.ClientID %>').val(rowIndex);
        }

        $(function () {
            RetainSelectedRow();
        });

        function RetainSelectedRow() {
            var parent = $('#<%= grdSampleKnittingApproval.ClientID %>').val();
            var currentIndex = $('#<%= grdSampleKnittingApproval.ClientID %>').val();
            if (parent != null) {
                ChangeRowColor(parent, currentIndex);
            }
        }

    </script>

    <script src="../Scripts/1.8.3.jquery.min.js"></script>
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
        <link href="../PDR_StyleSheet.css" rel="stylesheet" />
</asp:Content>
