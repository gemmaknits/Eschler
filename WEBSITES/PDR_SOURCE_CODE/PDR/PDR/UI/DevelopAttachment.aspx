<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DevelopAttachment.aspx.cs" Inherits="PDR.UI.DevelopAttachment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <div class="row ">
            <table>
                <tr>
                    <td>
                        <label class="col-sm-2  control-label text-nowrap clsFromLabel label-default" for="txtresult" style="width: 190px; background-color: lightblue; left: 0px; top: 1px;">DOCUMENT ATTACHMENTS</label>
                    </td>

                </tr>
            </table>
        </div>
        <div class="row ">
            <div class="form-group form-inline ">

                 <div class="row " style="margin-left: 5px; padding-left: 5px; left: 5px">
                    <div class="form-group form-inline " style="margin-left: 5px; padding-left: 5px; left: 5px">
                        <table>

                            <tr>
                                <td>
                                    <label class="clsFromLabel" for="txtEstimateNo" style="width: 130px">DOCUMENT NO</label></td>
                                <td class="auto-style2">
                                    <asp:TextBox class="form-control input-sm TextBox" ID="txtSourceDocNo" runat="server" Width="200px"></asp:TextBox>
                                </td>

                            </tr>
                            <tr>
                                <td>
                                    <label class="clsFromLabel" for="txtProduct" style="width: 130px">PRODUCT</label></td>
                                <td class="auto-style2">
                                    <asp:TextBox class="form-control input-sm TextBox" ID="txtProduct" runat="server" Width="200px"></asp:TextBox>
                                </td>

                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="LBLATTACH" runat="server" Text="FILE LOCATION" CssClass="clsFromLabel"></asp:Label></td>
                                <td class="auto-style2">
                                    <asp:FileUpload ID="uploadfiles1" runat="server" />
                                    <asp:Label ID="lblFileName" runat="server" Text=""></asp:Label>
                                </td>
                                <td>
                                    <a target="_self" href="DevelopAttachment.aspx">
                                        <asp:ImageButton ID="ImgSave" ToolTip="Save" runat="server" Width="25px" Height="25px" ImageUrl="~/Images/Save.png" 
                                            href="DevelopAttachment.aspx" OnClick="ImgSave_Click" /></a>
                                </td>

                            </tr>
                            <tr>
                                <td>
                                    <label class="clsFromLabel" for="txtDescription" style="width: 130px">DESCRIPTION</label></td>
                                <td class="auto-style2">
                                    <asp:TextBox class="form-control input-sm TextBox" ID="txtDescription" runat="server" Width="200px"></asp:TextBox>
                                </td>

                            </tr>
                        </table>
                    </div>
                </div>
                <div class="col-sm-10" style="margin-left: 7px; padding-left: 7px; left: 7px">
                    <asp:GridView ID="grdAttachmentDetails" runat="server" AutoGenerateColumns="false" GridLines="None"
                        EmptyDataText="There is No Records To Display" Width="17%"  DataKeyNames="doc_attachments_id,source_doc_number,file_location"
                        CssClass="table table-striped table-bordered table-hover" OnRowCommand="OnRowDeleting">

                        <HeaderStyle CssClass="grdheader" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderStyle Width="20" />
                                <ItemTemplate>
                                    <asp:ImageButton ImageUrl="~/Images/close.png" ControlStyle-Width="14px" ControlStyle-Height="14px" CommandName="Select"
                                        CommandArgument="Select" ToolTip="Delete" runat="server" Height="14" Width="14" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="2%" HeaderText="DESCRIPTION" HeaderStyle-CssClass="col col-xs-2" ItemStyle-CssClass="col col-xs-2" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="FILE_DESCRIPTION" runat="server" onkeydown="(event.keyCode!=13);" Text='<%# Eval("FILE_DESCRIPTION") %>' CssClass="TextBox " Width="300px"  />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--                            <asp:TemplateField ItemStyle-Width="2%" HeaderText="LOCATION" HeaderStyle-CssClass="col col-xs-2 " ItemStyle-CssClass="col col-xs-2 numericcol other_cost_per_kg" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="FILE_LOCATION" runat="server" onkeydown="(event.keyCode!=13);" Text='<%# Eval("FILE_LOCATION_disp") %>' CssClass="TextBox " Width="180px" ReadOnly="true" />
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                            <asp:BoundField DataField="FILE_LOCATION_disp" HeaderText="LOCATION" ReadOnly="True" HeaderStyle-CssClass="col">
                                <ItemStyle CssClass="col" Width="20px" />
                            </asp:BoundField>
                            <asp:TemplateField>
                                <HeaderStyle Width="20" />
                                <ItemTemplate>
                                    <asp:ImageButton ImageUrl="~/Images/downloadfile.png" ControlStyle-Width="14px" ControlStyle-Height="14px" CommandName="Download"
                                        CommandArgument='<%#((GridViewRow)Container).RowIndex%>' ToolTip="Download" runat="server" Height="14" Width="14" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

</asp:Content>
