<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CustomerFilter.aspx.cs" Inherits="PDR.UI.CustomerFilter" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <input type="hidden" id="hfCustomerId" value="" runat="server" />
    <div style="top: 100px; position: absolute; left: 100px">
        <asp:TextBox ID="txtSearch" runat="server" onkeydown="(event.keyCode!=13);" MaxLength="500" Width="700px" TextMode="MultiLine" Columns="300"></asp:TextBox>
        <td>
            <asp:Button ID="btnInsert" runat="server" Text="Insert" MaxLength="500" />
    </div>
    <%--   <script src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="//ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/jquery-ui.min.js" type="text/javascript"></script>
    <link href="//ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/themes/blitzer/jquery-ui.css"  rel="Stylesheet" type="text/css" />--%>
    <script src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.0.min.js" type="text/javascript"></script>
    <script src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/jquery-ui.min.js" type="text/javascript"></script>
    <link href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/themes/blitzer/jquery-ui.css"
        rel="Stylesheet" type="text/css" />
    <script type="text/javascript">
        $(document).ready(function () {
            // alert("SDFd");
            // $("[id$=txtSearch]").autocomplete({
            $("[id*=txtSearch]").autocomplete({
                source: function (request, response) {
                    $.ajax({

                        url: '<%=ResolveUrl("CustomerFilter.aspx/GetAutoCompleteData") %>',
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        response($.map(data.d, function (item) {
                            //alert(item) ;
                            return {

                                label: item.split('-')[1],
                                val: item.split('-')[0]

                            }
                        }))
                    },
                    error: function (response) {
                        //alert(response.responseText);
                    },
                    failure: function (response) {
                        //SSalert(response.responseText);
                    }
                });
            },
                select: function (e, i) {
                    // alert(i.item.val);
                    $("[id*=hfCustomerId]").val(i.item.val);// "-" + $("[id*=hfCustomerId]").val(i.item.label);
                },
                minLength: 1
            });
    });
    </script>
</asp:Content>
