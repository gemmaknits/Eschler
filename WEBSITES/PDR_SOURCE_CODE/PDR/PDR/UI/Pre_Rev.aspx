<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Pre_Rev.aspx.cs" Inherits="PDR.UI.Pre_Rev" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="left:280px;top:200px;width:400px;position:absolute" >
        <Table ID="Table1" runat="server" class="table">
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" Text="DR NO" CssClass="clsFromLabel"></asp:Label>
            </td>
            <td>
                <asp:Label ID="Label2" runat="server" Text="DESIGN" CssClass="clsFromLabel"></asp:Label>
            </td>
        </tr>
            <tr>
                <td><asp:TextBox ID="txtDRNO" runat="server"></asp:TextBox></td>
                <td><asp:TextBox ID="txtDesign" runat="server"></asp:TextBox></td>
            </tr>
        </Table>
        </div>
         <div style="left:850px;top:200px;width:400px;position:absolute" >
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="PDR NO:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDRNO" runat="server"   CssClass="TextBox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Date"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDate" runat="server"   CssClass="TextBox"></asp:TextBox>
                        </td>
                    </tr>
                </table>
        </div>
    <div style="left:100px;top:300px;width:1000px;position:absolute">
        <table style="line-height:28px;">
             <tr>
                 <td>
                     <asp:Label ID="Label5" runat="server" Text="CUSTOMER" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td>
                     <asp:TextBox ID="txtCustomer" runat="server"  Width="387px" CssClass="TextBox"></asp:TextBox>
                 </td>
             </tr>
            <tr>
                <td>
                    <asp:Label ID="Label6" runat="server" Text="REQUESTED BY"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtRequestedBy" runat="server"  CssClass="TextBox"></asp:TextBox>
                </td>
                <td>
                     <asp:Label ID="Label10" runat="server" Text="REASON FOR DEVL."></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtReasonForDev" runat="server"  CssClass="TextBox"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label7" runat="server" Text="EXP.CMPL.DATE"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtExpCmpDt" runat="server"  CssClass="TextBox"></asp:TextBox>
                </td>
                 <td>
                    <asp:Label ID="Label11" runat="server" Text="OTHER REASON"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtOtherReason" runat="server"  CssClass="TextBox"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <Td>
                    <asp:Label ID="Label8" runat="server" Text="CUST PRICE EXPECTATION"></asp:Label></Td>
                <td>
                    <asp:TextBox ID="txtCustPriceExpectation" runat="server"  CssClass="TextBox"></asp:TextBox>&nbsp;MTS</td>
                
                <Td>
                    <asp:Label ID="Label12" runat="server" Text="PURPOSE OF GARMENT"></asp:Label></Td>
                <td>
                    <asp:TextBox ID="txtPurposeofGarment" runat="server"  CssClass="TextBox"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label9" runat="server" Text="TEST METHOD"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtTestMethod" runat="server"  CssClass="TextBox"></asp:TextBox>
                </td>
                  <td>
                    <asp:Label ID="Label13" runat="server" Text="OTHERS"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtOthers" runat="server"  CssClass="TextBox"></asp:TextBox>
                </td>
            </tr>
            </table>
    </div>
    <div style="left:100px;top:450px;width:1000px;position:absolute">
        
        <table>
            <tr>
                <td>

                </td>
                <td></td>
                <td> <h4>YARN COMPOSITION</h4></td>
            </tr>
             <tr>
                 <td>
                     <asp:Label ID="Label14" runat="server" Text="FRONT" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td>
                     <asp:Label ID="Label15" runat="server" Text="1" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td>
                     <asp:TextBox ID="txtFront" runat="server"  CssClass="TextBox" Width="500px"></asp:TextBox>
                 </td>
             </tr>
             <tr>
                 <td>
                     <asp:Label ID="Label16" runat="server" Text="BACK" CssClass="clsFromLabel"></asp:Label>
                 </td>&nbsp;&nbsp;&nbsp;
                 <td>
                     <asp:Label ID="Label17" runat="server" Text="2" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td>
                     <asp:TextBox ID="txtBack" runat="server"  CssClass="TextBox" Width="500px"></asp:TextBox>
                 </td>
             </tr>
              <tr>
                 <td>
                     <asp:Label ID="Label18" runat="server" Text="" CssClass="clsFromLabel"></asp:Label>
                 </td>&nbsp;&nbsp;&nbsp;
                 <td>
                     <asp:Label ID="Label19" runat="server" Text="3" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td>
                     <asp:TextBox ID="TextBox1" runat="server"  CssClass="TextBox" Width="500px"></asp:TextBox>
                 </td>
             </tr>
              <tr>
                 <td>
                     <asp:Label ID="Label20" runat="server" Text="" CssClass="clsFromLabel"></asp:Label>
                 </td>&nbsp;&nbsp;&nbsp;
                 <td>
                     <asp:Label ID="Label21" runat="server" Text="4" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td>
                     <asp:TextBox ID="TextBox2" runat="server"  CssClass="TextBox" Width="500px"></asp:TextBox>
                 </td>
             </tr>
             <tr>
                 <td>
                     <asp:Label ID="Label22" runat="server" Text="DYE IN COLOR" CssClass="clsFromLabel"></asp:Label>
                 </td>&nbsp;&nbsp;&nbsp;
                 <td>
                     <asp:Label ID="Label23" runat="server" Text="COMMENT" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td>
                     <asp:Label ID="Label24" runat="server" Text="FINISHED REQUIRED" CssClass="clsFromLabel"></asp:Label>
                 </td>
             </tr>
             <tr>
                 <td>
                      <asp:TextBox ID="TXTDyeincolor" runat="server"  CssClass="TextBox" Width="500px"></asp:TextBox>
                 </td>&nbsp;&nbsp;&nbsp;
                 <td>
                      <asp:TextBox ID="TXTComment" runat="server"  CssClass="TextBox" Width="500px"></asp:TextBox>
                 </td>
                 <td>
                    <asp:TextBox ID="TXTFinishedrequired" runat="server"  CssClass="TextBox" Width="500px"></asp:TextBox>
                 </td>
             </tr>
        </table>
    </div>
    <div  style="left:100px;top:700px;width:450px;position:absolute">
        <asp:CheckBox ID="CHKFollow" runat="server" />&nbsp;FOLLOW CUSTOMER SPEC./STANDARD
    </div>
    <div style="left:100px;top:750px;width:320px; position:absolute">
        <tABLE>
            <tr>
                <td>
                    <asp:Label ID="Label25" runat="server" Text="WEIGHT(G/M)"></asp:Label>

                </td>
                <td style="width: 173px">
                    <asp:TextBox ID="txtweight" runat="server"></asp:TextBox></td>
            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label26" runat="server" Text="WIDTH(CM)"></asp:Label>

                </td>
                <td style="width: 173px">
                    <asp:TextBox ID="TXTWIDTH" runat="server"></asp:TextBox></td>
            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label27" runat="server" Text="GUAGE"></asp:Label>

                </td>
                <td style="width: 173px">
                    <asp:TextBox ID="TXTGUAGE" runat="server"></asp:TextBox></td>
            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label28" runat="server" Text="% EXTN AT LOAD"></asp:Label>

                </td>
                <td style="width: 173px">
                    <asp:TextBox ID="
                        LOAD" runat="server"></asp:TextBox></td>
            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label29" runat="server" Text="MODULUS @"></asp:Label>

                </td>
                <td style="width: 173px">
                    <asp:TextBox ID="TXTMODULUS" runat="server"></asp:TextBox></td>
                 <td>%</td>
            </tr>
        </tABLE>
    </div>
    <div style="left:450px;top:750px;width:363px; position:absolute">
        <table>
            <tr>
                <td>
                    <asp:Label ID="Label30" runat="server" Text="TECHNICAL COMMENT"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TXTTECHNICALCOMMENT" runat="server"></asp:TextBox>
                </td>
            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label31" runat="server" Text="DYEING/FINISHING COMMENT"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TXTDYEFINISHCOMMENT" runat="server"></asp:TextBox>
                </td>
            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label32" runat="server" Text="OTHER COMMENT"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TXTOTHERCOMMENT" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <div style="left:100px;top:900px;width:500px; position:absolute">
        <table>
            <tr>
                <td>
                    <asp:Label ID="Label33" runat="server" Text="SAMPLE"></asp:Label>&NBSP;&NBSP;
                </td>
                <td>
                    <asp:Panel ID="Panel1" runat="server" Height="1" Width="100" BorderStyle="Inset"></asp:Panel>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
