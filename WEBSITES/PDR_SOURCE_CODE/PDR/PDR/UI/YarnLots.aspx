<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="YarnLots.aspx.cs" Inherits="PDR.UI.YarnLots" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../PDR_StyleSheet.css" rel="stylesheet" />
</head>
<body>
    <form id="frmYarnLot" runat="server">
        <div>
            <h5>YARN LOTS</h5>
        </div>
        <div>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="LBLKIKO" runat="server" Text=""></asp:Label></td>
                    <td>
                        <asp:Label ID="LBLDESIGNNO" runat="server" Text=""></asp:Label></td>
                </tr>
            </table>
        </div>
        <div class="col-sm-10" style="margin-left: 7px; padding-left: 7px; left: 7px">
            <asp:GridView ID="grdYarnLo" runat="server" AutoGenerateColumns="false"
                EmptyDataText="There is No Records To Display" Width="57%"
                CssClass="table table-striped table-bordered table-hover">

                <HeaderStyle CssClass="grdheader" />
                <Columns>

                    <asp:BoundField DataField="ITCD" HeaderText="YARN CODE" ReadOnly="True" HeaderStyle-CssClass="col">
                        <ItemStyle CssClass="col" />
                    </asp:BoundField>

                    <asp:BoundField DataField="ITDESC2" HeaderText="YARN NAME" ReadOnly="True" HeaderStyle-CssClass="col">
                        <ItemStyle CssClass="col" />
                    </asp:BoundField>
                    <asp:BoundField DataField="SUPNAME" HeaderText="SUPPLIER" ReadOnly="True" HeaderStyle-CssClass="col">
                        <ItemStyle CssClass="col" />
                    </asp:BoundField>
                    <asp:BoundField DataField="SUPPLIER_LOT_NUMBER" HeaderText="SUPPLIER LOT" ReadOnly="True" HeaderStyle-CssClass="col">
                        <ItemStyle CssClass="col" />
                    </asp:BoundField>
                    <asp:BoundField DataField="LOT_NUMBER" HeaderText="INTERNAL LOT NO" ReadOnly="True" HeaderStyle-CssClass="col">
                        <ItemStyle CssClass="col" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ISSUED_QTY_KG" HeaderText="ISSUED QTY (KG)" ReadOnly="True" HeaderStyle-CssClass="numericcol">
                        <ItemStyle CssClass="numericcol" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ISSUED_QTY_TUBES" HeaderText="ISSUED QTY (SPOOLS)" ReadOnly="True" HeaderStyle-CssClass="numericcol">
                        <ItemStyle CssClass="numericcol" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
