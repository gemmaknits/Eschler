<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditCustomer.aspx.cs" Inherits="PDR.UI.AddEditCustomer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../PDR_StyleSheet.css" rel="stylesheet" />
</head>
<body>
    <form id="frmAddEditCustomer" runat="server">
        <div style="left: 100px; top: 40px; position: absolute; width: 500px">
            <asp:Label ID="Label1" runat="server" Text="CUSTOMER INFORMATION"></asp:Label>
            <asp:ImageButton ID="ImgAdd" runat="server" Width="25px" Height="25px" ImageUrl="~/Images/add-icon.png" OnClick="ImgAdd_Click" />
            <asp:ImageButton ID="ImgSave" runat="server" Width="25px" Height="25px" ImageUrl="~/Images/Save-as-icon.png" OnClick="ImgSave_Click" OnClientClick="return checkCountry();" />
        </div>
        <div style="left: 100px; top: 80px; position: absolute; width: 500px">
             <strong>
             <asp:Label ID="lblmsg" runat="server" Text="Successfully Saved" Visible="False" ForeColor="#009933"></asp:Label>
             </strong>
        </div>
        <div style="left: 100px; top: 100px; position: absolute; width: 500px">

            <table>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="CODE" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtCode" runat="server" CssClass="TextBox" ReadOnly="True"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text="NAME" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtName" runat="server" CssClass="TextBox" Width="250px" onkeydown = "(event.keyCode!=13);" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label4" runat="server" Text="ADDRESS" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtAddress" runat="server" CssClass="TextBox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label5" runat="server" Text="ADDRESS2" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtAddress2" runat="server" CssClass="TextBox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label6" runat="server" Text="ADDRESS3" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtAddress3" runat="server" CssClass="TextBox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label7" runat="server" Text="NAME(thai)" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtNameThai" runat="server" CssClass="TextBox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label8" runat="server" Text="ADDRESS(thai)" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtAddressThai" runat="server" CssClass="TextBox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:TextBox ID="txtAddressThai2" runat="server" CssClass="TextBox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:TextBox ID="txtAddressThai3" runat="server" CssClass="TextBox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label14" runat="server" Text="CITY" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtCity" runat="server" CssClass="TextBox" Width="250px"></asp:TextBox>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label9" runat="server" Text="COUNTRY" CssClass="clsFromLabel"></asp:Label>

                    </td>
                    <td>
                        <asp:DropDownList ID="ddlCountry" runat="server" Width="200px"></asp:DropDownList>
                        
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label10" runat="server" Text="TEL" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtTel" runat="server" CssClass="TextBox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label11" runat="server" Text="FAX" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtFax" runat="server" CssClass="TextBox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label12" runat="server" Text="EMail" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="TextBox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label13" runat="server" Text="CONTACT" CssClass="clsFromLabel"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtContact" runat="server" CssClass="TextBox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
    
    <script>
        function checkCountry()
        {
          
            var ctry = document.getElementById("ddlCountry").value;
            if (ctry == '')
            {
                alert("Select Country");
                return false;   
            }
            return true;

        }
    </script>
         <script src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.0.min.js" type="text/javascript"></script>
<script src="//ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/jquery-ui.min.js" type="text/javascript"></script>
<link href="//ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/themes/blitzer/jquery-ui.css"
    rel="Stylesheet" type="text/css" />
      <script type="text/javascript">
            $(document).ready(function () {
                // search main
                $("[id*=txtName]").autocomplete({
                    source: function (request, response) {
                        $.ajax({

                            url: '<%=ResolveUrl("Develop_Request.aspx/GetCustomers1") %>',
                            data: "{ 'prefix': '" + request.term + "'}",
                            dataType: "json",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            success: function (data) {
                                response($.map(data.d, function (item) {
                                   // alert(item) ;
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
                      //  $("[id*=hidSearchMainValue]").val(i.item.val);// "-" + $("[id*=hfCustomerId]").val(i.item.label);
                        //alert(document.getElementById("ContentPlaceHolder1_hidSearchMainValue").value);
                        document.getElementById("txtCode").value = i.item.val;
                        var custcode = document.getElementById("txtCode").value;
                        alert(custcode);
                        /// load other values when custcode is assigned
                        $.ajax({
                            type: "POST",
                            url: "AddEditCustomer.aspx/GetCustomerDetails",
                            data: '{custcode:' + JSON.stringify(custcode) + '}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                //  console.log(data);
                                for (var i = 0; i < eval(data.d.length) ; i++) {
                                    var name = (data.d[i].name);
                                    var ctry = data.d[i].ctry;
                                    var addr1 = data.d[i].addr1;
                                    var addr2 = data.d[i].addr2;
                                    var addr3 = data.d[i].addr3;
                                    var addr1t = data.d[i].addr1t;
                                    var addr2t = data.d[i].addr2t;
                                    var addr3t = data.d[i].addr3t;
                                    var city = data.d[i].city;
                                    var tel = data.d[i].tel;
                                    var fax = data.d[i].fax;
                                    var email = data.d[i].email;
                                    var contact = data.d[i].contact;
                                    document.getElementById("txtName").value = name;
                                    document.getElementById("ddlCountry").value = ctry;
                                    document.getElementById("txtAddress").value =addr1;
                                    document.getElementById("txtAddress2").value =addr2;
                                    document.getElementById("txtAddress3").value = addr3;
                                    document.getElementById("txtAddressThai").value = addr1t;
                                    document.getElementById("txtAddressThai2").value = addr2t;
                                    document.getElementById("txtAddressThai3").value = addr3t;
                                    document.getElementById("txtCity").value = city;
                                    document.getElementById("txtTel").value = tel;
                                    document.getElementById("txtFax").value = fax;
                                    document.getElementById("txtEmail").value = email;
                                    document.getElementById("txtContact").value = contact;
                                }
                            }
                        });
                        ///
                    }
                });
            })
</script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js"></script>
<script src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.9/jquery-ui.js" type="text/javascript"></script>
<link href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.9/themes/start/jquery-ui.css"
    rel="stylesheet" type="text/css" />
       <%--<script type="text/javascript">
            $(document).ready(function () {
                // search main
                $("[id*=txtName]").autocomplete({
                    source: function (request, response) {
                    
                        $.ajax({
                             url: '<%=ResolveUrl("Develop_Request.aspx/GetCustomers1") %>',
                            data: "{ 'prefix': '" + request.term + "'}",
                            dataType: "json",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            success: function (data) {
                                response($.map(data.d, function (item) {
                                //   alert(item) ;
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
                        //$("[id*=hidSearchMainValue]").val(i.item.val);// "-" + $("[id*=hfCustomerId]").val(i.item.label);
                        //alert(document.getElementById("ContentPlaceHolder1_hidSearchMainValue").value);
                        document.getElementById("txtCode").value = i.item.val;
                        var custcode =i.item.val;
                        alert(custcode);
                        /// load other values when custcode is assigned
                        $.ajax({
                            type: "POST",
                            url: "AddEditCustomer.aspx/GetCustomerDetails",
                            data: "'{custcode:'" + custcode + "'}'",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                //  console.log(data);
                                for (var i = 0; i < eval(data.d.length) ; i++) {
                                    var name = (data.d[i].name);
                                    var ctry=data.d[i].ctry;
                                    document.getElementById("txtName").value = name;
                                   
                                }
                            }
                        });
                        ///
                    }
                });
            })
</script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js"></script>
<script src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.9/jquery-ui.js" type="text/javascript"></script>
<link href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.9/themes/start/jquery-ui.css"
    rel="stylesheet" type="text/css" />--%>
        </form>
</body>
</html>

