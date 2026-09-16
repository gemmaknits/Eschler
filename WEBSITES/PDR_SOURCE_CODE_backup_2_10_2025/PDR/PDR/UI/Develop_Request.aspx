<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Develop_Request.aspx.cs" Inherits="PDR.UI.Develop_Request" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">



    <asp:HiddenField ID="hidpdr_new_develop_req_id" runat="server" />
<asp:HiddenField ID="hidSearchMainValue" runat="server" />
    <asp:HiddenField ID="hidPDRNOCancel" runat="server" />
    <asp:HiddenField ID="hidavilToEmailID" runat="server" />
     <div style="left:50px; top:45px; width:400px;position:absolute; margin-top: 0px;" >
         <asp:Label ID="lblErrorMSg" runat="server" Text="" Visible="false"></asp:Label>
         </div>
    <div style="left:50px; top:60px; width:500px;position:absolute; margin-top: 0px;" >
        <Table ID="Table1" runat="server">
        <tr>
             <td> <strong> <asp:Label ID="Label1" runat="server" Text="DEVELOP REQUEST" class="headertext"></asp:Label></strong></td>
             <td style="padding-left:20px">
                  <a  target="_self"  href="Develop_Request.aspx">
                            <asp:ImageButton ID="ImgAdd" runat="server" Width="20px" Height="20px" ToolTip="Create new pdr" ImageUrl="~/Images/add-icon.png" href="Develop_Request.aspx?pdrno=''" OnClick="ImgAdd_Click"/>
                 </a>
             </td>
             <td style="padding-left:20px">
                  <a  target="_self"  href="Develop_Request.aspx">
                            <asp:ImageButton ID="ImgSave" runat="server" Width="20px" Height="20px"  ToolTip="Save PDR" ImageUrl="~/Images/Save-as-icon.png" href="Develop_Request.aspx" OnClick="ImgSave_Click"/>
                 </a>
             </td>
             <td style="padding-left:20px">
                  <a  target="_self"  href="Develop_Request.aspx">
                            <asp:ImageButton ID="ImgPrint" runat="server" Width="20px" Height="20px"  ToolTip="Print to PDF" ImageUrl="~/Images/pdf.jpeg" href="PDR_List.aspx" OnClick="ImgPrint_Click"/>
                 </a>
             </td>
            <td style="padding-left:20px">
                  <a  target="_self"   href="#">
                            <asp:ImageButton ID="ImageButton3" runat="server" Width="20px" Height="20px" ToolTip="send mail for approval" ImageUrl="~/Images/EMail.jpg"  OnClick="ImgEMail_Click" 
                                OnClientClick="javascript:ShowProgressBar()"/>
                 </a>
             </td>
             <td style="padding-left:20px">
                  <a  href="#" target="_self"  >
                            <asp:ImageButton ID="ImgCopy" runat="server" Width="20px" Height="20px" ToolTip="Copy to New PDR" ImageUrl="~/Images/Copy.png"  OnClick="ImgCopy_Click" OnClientClick="ConfirmCopy()"/> 
                </a>
             </td>
            <td>
                <a href="#" target="_blank">
                    <asp:ImageButton ID="ImgAttachment" Width="20px" Height="20px" ToolTip="Attachment" runat="server" ImageUrl="~/Images/Attachment.png"  
                        OnClientClick="return ValidateRange()"                         OnClick="ImgAttachment_Click" /></a>
            </td>
           <td> <asp:Label ID="lblmsg" runat="server" Text="" CssClass="clsFromLabel" Visible="false"></asp:Label></td>
        </tr>
        </Table>
        </div>
           <div style=" left: 690px;
             top: 60px;
             width: 200px;
             position: absolute;">
               <table style="line-height: 28px;">
                   <tr style="line-height: 28px;">
                       <td>
                           <asp:Label ID="lblCancel" runat="server" Text="" CssClass="clsFromLabel" Width="100px"></asp:Label>
                       </td>
                       <td>
                           <a target="_self" href="#">
                               <asp:ImageButton ID="ImgBtnCancel" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/cancel-doc.png" OnClick="ImgCancel_Click" OnClientClick="Confirm()" />
                           </a>
                       </td>
                   </tr>
               </table>
            </div>
    <div style="left: 820px; top: 60px; width: 200px; position: absolute; height: 23px;">
        <table style="line-height: 28px;">
            <tr style="line-height: 28px;">
                <td>
                    <asp:Label ID="Label3" runat="server" Text="PDR NO:" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td style="padding-left: 20px">


                    <asp:Panel ID="Panel1" runat="server" DefaultButton="myButton">
                        <asp:TextBox ID="txtPDRNO" runat="server" CssClass="TextBox" Width="85px"></asp:TextBox>
                        <asp:Button ID="myButton" runat="server" Text="Button" Style="display: none" OnClick="myButton_Click" />
                    </asp:Panel>

                </td>
                <td style="padding-right: 20px">
                    <a target="_blank" href="PDR_List.aspx">
                        <asp:ImageButton ID="ImgPdrList" runat="server" Width="15px" Height="15px" ImageUrl="~/Images/Edit.ico" href="PDR_List.aspx" OnClick="ImgPdrList_Click" />
                </td>
            </tr>
        </table>
    </div>
      <div style=" left: 820px;
             top: 90px;
             width: 200px;
             position: absolute;
             height: 24px;" >  
             <table style="line-height:28px;">
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Date" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td style="padding-left:40px">
                             <INPUT class="datepicker TextBox" id="PDRDate"  type="text" size="10" name="PDRDate" runat="server" style="width:85px" >
                        </td>
                    </tr>
                </table>
        </div>
        <div style=" left: 820px;
             top: 120px;
             width: 200px;
             position: absolute;
             height: 24px;" >  
             <table style="line-height:28px;">
                    <tr>
                        <td>
                            <asp:Label ID="Label62" runat="server" Text="Prepare by" CssClass="clsFromLabel"></asp:Label>
                        </td>
                        <td style="padding-left:12px">
                            <asp:TextBox ID="txtPrepareBy" runat="server"   CssClass="TextBox"  Width="85px"  ></asp:TextBox>
                        </td>
                    </tr>
                </table>
        </div>
     
    <div style=" left: 1070px;
             top: 60px;
             width: 304px;
             position: absolute;
             width:800px" >
         <table style="line-height:25px;">
             <tr>
                 <td>
                     <asp:Label ID="Label44" runat="server" Text="APP / REJ" CssClass="clsFromLabel"></asp:Label></td>
                 <td style="padding-left:48px">
                     <asp:TextBox ID="txtAppRej" runat="server" CssClass="TextBox"></asp:TextBox></td>
                 <td style="padding-left:20px">
                     <asp:Label ID="Label45" runat="server" Text="BY" CssClass="clsFromLabel"></asp:Label></td>
                 <td style="padding-left:20px">
                     <asp:TextBox ID="txtBy" runat="server" CssClass="TextBox"></asp:TextBox></td>

             </tr>
              <tr>
                 <td>
                     <asp:Label ID="Label60" runat="server" Text="APP/REJ DATE" CssClass="clsFromLabel"></asp:Label></td>
                 <td style="padding-left:48px">
                     <asp:TextBox ID="txtAppRejDate" runat="server" CssClass="TextBox"></asp:TextBox></td>
                 <td>
                     </td>
                 <td>
                     </td>

             </tr>
             </table>
             </DIV>
             <DIV style=" left: 1070px;
             top: 115px;
             width: 304px;
             position: absolute;
             width:800px">
                 <table>
             <tr>
                 <td >
                     <asp:Label ID="Label61" runat="server" Text="APP/REJ COMMENT" CssClass="clsFromLabel"></asp:Label></td>
                 <td style="padding-left:23px">
                     <asp:TextBox ID="txtAppRejComment" runat="server" CssClass="TextBox"></asp:TextBox></td>
                 <td>
                     </td>
                 <td>
                     </td>

             </tr>
         </table>
         </DIV>
      
    
    <div style="left:50px; top:115px; width:1428px; position:absolute">
        <table style="line-height:28px;">
             <tr >
                 <td style="width:150px">
                     <asp:Label ID="Label35" runat="server" Text="PRODUCT NAME" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td>
                     <asp:TextBox ID="txtDesignno" runat="server"  Width="243px" CssClass="TextBox"  ></asp:TextBox>
                 </td>
             </tr>
             <tr >
                 <td style="width:150px">
                     <asp:Label ID="Label5" runat="server" Text="CUSTOMER" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td>
                     <%--<asp:TextBox ID="txtCustomer" runat="server"  Width="243px" CssClass="TextBox"  ></asp:TextBox>--%>
                     <asp:TextBox ID="txtSearchMain" runat="server"  onkeydown = "(event.keyCode!=13);"  CssClass="TextBox" 
                           Width="200px" Columns="300" Text="" ></asp:TextBox>
                   
                     <%--<asp:HyperLink id="hyperlink1" ImageHeight="20px" ImageWidth="20px"
                  ImageUrl="~/Images/add-icon.png"                 
                        Target="_blank"
                  runat="server" 
                          NavigateUrl="addeditcustomer.aspx" ></asp:HyperLink>--%>
                     <%--<asp:LinkButton runat="server" id="lnkReport"
  NavigateUrl="addeditcustomer.aspx" 
  target="_blank" ></asp:LinkButton>--%>
                      <a  target="_blank"  href="addeditcustomer.aspx">
                            <asp:ImageButton ID="ImageButton1" runat="server" Width="15px" Height="15px" ImageUrl="~/Images/add-icon.png"  
                                OnClientClick="return OpenW();"  /></a>
                     <a><asp:ImageButton ID="ImageButton2" runat="server"  Width="15px" Height="15px" ImageUrl="~/Images/Edit.ico"  OnClientClick="return OpenWinEdit();" /></a>
                 </td>
                 <td style="width:100px;padding-left:20px">
                     <asp:Label ID="Label53" runat="server" Text="CUSTCODE" CssClass="clsFromLabel"></asp:Label>
                 </td>
                  <td >
                     <asp:TextBox ID="txtCustCode" runat="server"  onkeydown = "(event.keyCode!=13);"   CssClass="TextBox" Text="" ReadOnly="true"></asp:TextBox>
                 </td>
                       <td style="padding-left:20px">
                     <asp:Label ID="Label56" runat="server" Text="END BUYER" CssClass="clsFromLabel"></asp:Label>
                 </td>
                  <td >
                     <asp:DropDownList ID="ddlEndBuyer" runat="server" Width="200px" CssClass="combo"></asp:DropDownList>
                 </td>
             </tr>
            <tr>
                <td>
                    <asp:Label ID="Label6" runat="server" Text="REQUESTED BY" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlPDRRequestor" runat="server" Width="250px" CssClass="combo"></asp:DropDownList>
                </td>
                <td style="padding-left:20px;padding-left:20px">
                     <asp:Label ID="Label10" runat="server" Text="EXP.CMPL.DATE" width="120px" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td style="padding-right:40px">
                     <INPUT class="datepicker TextBox" id="dtTo"  type="text" size="10" name="dtTo" runat="server" style="width:90px" >
                </td>
                  <td style="width:190px;padding-left:20px">
                     <asp:Label ID="Label2" runat="server" Text="CUST PRICE EXPECTATION:" Width="190px" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td  >
                    <asp:TextBox ID="txtCustPriceExpectation" runat="server"  CssClass="TextBox numericcol" onkeydown="return jsDecimals(event);" Width="100px"></asp:TextBox>
                </td>
                  <td style="padding-left:20px">
                    <asp:DropDownList ID="ddluom" runat="server" Width="50px" CssClass="combo"></asp:DropDownList>
                </td>
               
                  <td style="padding-left:20px">
                    <asp:DropDownList ID="ddlcombocurrency" runat="server" Width="100px" CssClass="combo"></asp:DropDownList>
                </td>
            </tr>
            </TABLE>
        </div>
        <div style="left: 50px;
             top: 205px;
             width: 1178px;
             height: 23px;
             position: absolute;">
             <strong>
             <asp:Label ID="Label15" runat="server" Text="CUSTOMER REQUIRE" BackColor="#999999" Width="150px"></asp:Label>
             </strong>
            </div>
    <div style="left:50px; top:226px; width:1178px; position:absolute">
        <table style="line-height:28px;">
           
            <tr style="line-height:28px;">
               <td style="width:150px">
                     <asp:Label ID="Label7" runat="server" Text="REASON FOR DEVL." CssClass="clsFromLabel"></asp:Label>
                </td>
                <td >
                    <asp:DropDownList ID="ddlReasonForDevl" runat="server" Width="250px" CssClass="combo" ></asp:DropDownList>
                </td>
                <Td style="width:140px;padding-left:20px">
                    <asp:Label ID="Label12" runat="server" Text="PRIORITY" Width="120px"  CssClass="clsFromLabel"></asp:Label></Td>
                <td>
                    <asp:DropDownList ID="ddlPriority" runat="server" Width="250px" CssClass="combo"></asp:DropDownList></td>
               </tr>
            <tr>
                <td >
                    <asp:Label ID="Label11" runat="server" Text="OTHER REASON" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtDevelopmentTypeRemark" runat="server"  CssClass="TextBox"></asp:TextBox>
                </td>
                 <td style="padding-left:20px">
                    <asp:Label ID="Label55" runat="server" Text="REMARKS" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtPriorityComment" runat="server"  CssClass="TextBox"></asp:TextBox>
                </td>
            </tr>
            
            </table>
    </div>
    <div style="left:50px; top:288px; width:1178px; position:absolute">
        <strong>
        <asp:Label ID="Label16" runat="server" Text="ITEM CATEGORY" BackColor="#999999" Width="150px"></asp:Label>
        </strong>
     </DIV>
    <div style="left:50px; top:318px; width:1388px; position:absolute; height: 50px;">

        <table style="width: 1160px">
                      
            <tr style="line-height:28px;">
               <td style="width:150px">
                     <asp:Label ID="Label8" runat="server" Text="CATEGORY" Width="150px" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td style="padding-right:20px">
                    <asp:DropDownList ID="ddlCategory" runat="server" Width="250px" CssClass="combo"></asp:DropDownList>
                </td>
                <Td style="width:150px">
                    <asp:Label ID="Label9" runat="server" Text="GROUP" Width="120px" CssClass="clsFromLabel"></asp:Label></Td>
                <td style="padding-right:20px">
                    <asp:DropDownList ID="ddlgroup" runat="server" Width="250px" CssClass="combo"></asp:DropDownList></td>
                 <Td style="width:95px;padding-right:15px">
                    <asp:Label ID="Label17" runat="server" Text="TYPE" CssClass="clsFromLabel"></asp:Label></Td>
                <td>
                    <asp:DropDownList ID="ddlType" runat="server" Width="250px" CssClass="combo"></asp:DropDownList></td>
               </tr>
            <tr>
                <td style="width:200px">
                    <asp:Label ID="Label13" runat="server" Text="SUB CAT" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlSubCat" runat="server" Width="250px" CssClass="combo"></asp:DropDownList></td>
            
                 <td style="width:120px">
                    <asp:Label ID="Label14" runat="server" Text="SUB GROUP" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlSubGroup" runat="server" Width="250px" CssClass="combo"></asp:DropDownList></td>
         
                 <Td style="width:95px;">
                    <asp:Label ID="Label18" runat="server" Text="SUB TYPE" Width="95px" CssClass="clsFromLabel"></asp:Label></Td>
                <td style="padding-right:15px">
                    <asp:DropDownList ID="ddlSubType" runat="server" Width="250px" CssClass="combo"></asp:DropDownList></td>
            </tr>
        </table>
    </div>
  <div style="left:50px; top:381px; width:1395px; position:absolute">
      
      <table>
            <tr style="line-height:25px;">
               <td style="width:150px">
                       <strong>
                    <asp:Label ID="Label19" runat="server" Text="TEST METHOD:" BackColor="#999999" Width="150px"></asp:Label>
                    </strong>
                </td>

                <td style="padding-right:0px">
                    <asp:CheckBox ID="CHKAATCC" CssClass="chkcol" Width="90px" runat="server" Text="AATCC" />
                </td>
                 <td style="padding-right:0px">
                    <asp:CheckBox ID="CHKASTM" CssClass="chkcol" Width="90px" runat="server" Text="ASTM"/>
                </td>
                 <td style="padding-right:0px">
                    <asp:CheckBox ID="CHKISO" CssClass="chkcol" Width="90px" runat="server" Text="GMK"/>
                </td>
                 <td style="padding-right:0px">
                    <asp:CheckBox ID="CHKJIS" CssClass="chkcol" Width="90px" runat="server" Text="JIS" />
                </td>
                 <td style="padding-right:0px">
                    <asp:CheckBox ID="chkMS" CssClass="chkcol" Width="90px"  runat="server" Text="M & S"/>
                </td>
                 <td style="padding-right:0px">
                    <asp:CheckBox ID="chkHM" CssClass="chkcol" Width="90px" runat="server" Text="HM" />
                </td>
                 <td style="padding-right:0px">
                    <asp:CheckBox ID="chkVSS" CssClass="chkcol" Width="90px" runat="server" Text="VSS" />
                </td>
                <Td  style="padding-left:30px">
                    <asp:Label ID="lblTestMethod" runat="server" Text="FOR SHOE" CssClass="clsFromLabel"></asp:Label></Td>
                <td style="padding-right:10px">
                     <asp:DropDownList ID="ddlTestMethod" runat="server" Width="100px" CssClass="combo"> </asp:DropDownList>
                </td>
                <Td  style="padding-left:30px">
                    <asp:Label ID="Label21" runat="server" Text="OTHERS" CssClass="clsFromLabel"></asp:Label></Td>
                <td style="padding-left:50px">
                    <asp:TextBox ID="txtStandard_Customer" runat="server" CssClass="TextBox"  Width="240px"></asp:TextBox>

                </td>
                
               </tr>
      </table>
     </DIV>
    <div style=" left: 50px;
             top: 400px;
             width: 500px;
             position: absolute;height:230px">
        
        <table class="auto-style5">
            <tr>
                <td class="auto-style3">

                </td>
                <td class="auto-style4"></td>
                <td class="auto-style2"> <h4> YARN COMPOSITION</h4></td>
                <td style="padding-left:30px">
                    <h4>%</h4>
                </td>
            </tr>
             <tr >
                 <td class="auto-style3" >
                     <asp:DropDownList ID="ddlFront" runat="server" Width="100px" CssClass="combo"> </asp:DropDownList>
                 </td>
                 <td style="padding-right:5px;" class="auto-style4">
                     <asp:Label ID="Label22" runat="server" width="10px" Text="1" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td class="auto-style2">
                     <asp:TextBox ID="txtComposition1_name" runat="server" CssClass="TextBox"  Width="261px"></asp:TextBox>
                 </td>
                 <td >
                      <asp:TextBox ID="txtPercent1" runat="server" CssClass="TextBox numericcol"  Width="50px"  onkeydown="return jsDecimals(event);"></asp:TextBox>
                 </td>
                 
             </tr>
             <tr>
                 <td class="auto-style3">
                     <asp:DropDownList ID="ddlBack" runat="server" Width="100px" CssClass="combo"></asp:DropDownList>
                 </td>&nbsp;
                 <td class="auto-style4">
                     <asp:Label ID="Label24" runat="server" width="10px" Text="2" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td class="auto-style2">
                     <asp:TextBox ID="txtComposition2_name" runat="server" CssClass="TextBox"  Width="261px"></asp:TextBox>
                 </td>
                 <td >
                      <asp:TextBox ID="txtPercent2" runat="server" CssClass="TextBox numericcol"  Width="50px" onkeydown="return jsDecimals(event);"></asp:TextBox>
                 </td>
               
             </tr>
              <tr>
                 <td class="auto-style3">
                     <asp:DropDownList ID="ddlfront1" runat="server" Width="100px" CssClass="combo"></asp:DropDownList>
                 </td>&nbsp;
                 <td class="auto-style4">
                     <asp:Label ID="Label26" runat="server" width="10px" Text="3" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td class="auto-style2">
                     <asp:TextBox ID="txtComposition3_name" runat="server" CssClass="TextBox"  Width="261px"></asp:TextBox>
                 </td>
                  <td >
                      <asp:TextBox ID="txtPercent3" runat="server" CssClass="TextBox numericcol"  Width="50px" onkeydown="return jsDecimals(event);"></asp:TextBox>
                 </td>
                  
             </tr>
              <tr>
                 <td class="auto-style3">
                     <asp:DropDownList ID="ddlfront2" runat="server" Width="100px" CssClass="combo"></asp:DropDownList>
                 </td>&nbsp;
                 <td class="auto-style4">
                     <asp:Label ID="Label28" runat="server" width="10px" Text="4" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td class="auto-style2">
                     <asp:TextBox ID="txtComposition4_name" runat="server" CssClass="TextBox"  Width="261px"></asp:TextBox>
                 </td>
                  <td >
                      <asp:TextBox ID="txtPercent4" runat="server" CssClass="TextBox numericcol"  Width="50px" onkeydown="return jsDecimals(event);"></asp:TextBox>
                 </td>
                
             </tr>
           <tr>
                 <td class="auto-style3">
                     <asp:DropDownList ID="ddlfront5" runat="server" Width="100px" CssClass="combo"></asp:DropDownList>
                 </td>&nbsp;
                 <td class="auto-style4">
                     <asp:Label ID="Label41" runat="server" width="10px" Text="5" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td class="auto-style2">
                     <asp:TextBox ID="txtComposition5_name" runat="server" CssClass="TextBox"  Width="261px"></asp:TextBox>
                 </td>
                  <td >
                      <asp:TextBox ID="txtPercent5" runat="server" CssClass="TextBox numericcol"  Width="50px" onkeydown="return jsDecimals(event);"></asp:TextBox>
                 </td>
                 
             </tr>
            <tr>
                 <td class="auto-style3">
                     <asp:DropDownList ID="ddlfront6" runat="server" Width="100px" CssClass="combo"></asp:DropDownList>
                 </td>&nbsp;
                 <td class="auto-style4">
                     <asp:Label ID="Label46" runat="server" width="10px" Text="6" CssClass="clsFromLabel"></asp:Label>
                 </td>
                 <td class="auto-style2">
                     <asp:TextBox ID="txtComposition6_name" runat="server" CssClass="TextBox"  Width="261px"></asp:TextBox>
                 </td>
                  <td >
                      <asp:TextBox ID="txtPercent6" runat="server" CssClass="TextBox numericcol"  Width="50px" onkeydown="return jsDecimals(event);"></asp:TextBox>
                 </td>
                  
             </tr>
        </table>
    </div>
    <div style="left: 540px; top: 415px; width: 610px; position: absolute;height:230px">
        <table style="line-height:25px;">
            <tr >
                <td>
                    <asp:Label ID="Label57" runat="server" Text="CODE" CssClass="clsFromLabel"></asp:Label>
                     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href="#" target="_blank" class="clsFromLabel" style="padding-left:50px;"> 
                    <asp:ImageButton ID="imgAvailMail" style="padding-top:10px" Width="30px" Height="30px" ToolTip="Avial Send Mail" runat="server" ImageUrl="~/Images/mail-exclaim.jpg"  
                        OnClick="ImgAvailAlert_Click"   OnClientClick="return ValidateRange()"  />
                    </a>
                    <asp:Label ID="Label58"  runat ="server" Text="AVAIL" CssClass="clsFromLabel"></asp:Label>
                    <asp:ImageButton ID="imgAvailRefresh" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/refresh-icon.png"  OnClick="ImgAvailRefresh_Click"/>

                </td>
                <td>
                    <%--<asp:ImageButton ID="imgAvailMail" runat="server" Width="30px" Height="20px" ImageUrl="~/Images/mail-exclaim.jpg"  OnClick="ImgAvailAlert_Click"/>--%>

                </td>
                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="new1" Text="N" runat="server" CssClass="clsFromLabel" ></asp:Label></td>
                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label65" Text="S" runat="server" CssClass="clsFromLabel"></asp:Label></td>
            </tr>

            <tr style="line-height:25px;">
                <td>
                    <asp:DropDownList ID="ddlCode1" runat="server" Width="230px" CssClass="combo" AutoPostBack="True" OnSelectedIndexChanged="ddlCode1Selected"></asp:DropDownList>
                </td>
                <td style="padding-left:10px">
                    <asp:TextBox ID="txtAvail1" runat="server" Width="50px" CssClass="TextBox" ></asp:TextBox>
                </td>
                  <td><asp:CheckBox ID="chknewyarn1" runat="server" CssClass="chkcol" /></td>
                <td><asp:CheckBox ID="chkshortage1" runat="server" CssClass="chkcol"   /></td>
            </tr>
            
            <tr>
                <td>
                    <asp:DropDownList ID="ddlCode2" runat="server" Width="230px" CssClass="combo" AutoPostBack="True" OnSelectedIndexChanged="ddlCode2Selected"></asp:DropDownList>
                </td>
                <td style="padding-left:10px">
                    <asp:TextBox ID="txtAvail2" runat="server" Width="50px" CssClass="TextBox"></asp:TextBox>
                </td>
                <td><asp:CheckBox ID="chknewyarn2" runat="server" CssClass="chkcol" /></td>
                <td><asp:CheckBox ID="chkshortage2" runat="server" CssClass="chkcol" /></td>
            </tr>
            
            <tr>
                <td>
                    <asp:DropDownList ID="ddlCode3" runat="server"  Width="230px" CssClass="combo" AutoPostBack="True" OnSelectedIndexChanged="ddlCode3Selected"></asp:DropDownList>
                </td>
                <td style="padding-left:10px">
                    <asp:TextBox ID="txtAvail3" runat="server" Width="50px" CssClass="TextBox"></asp:TextBox>
                </td>
                  <td><asp:CheckBox ID="chknewyarn3" runat="server" CssClass="chkcol" /></td>
                <td><asp:CheckBox ID="chkshortage3" runat="server" CssClass="chkcol" /></td>
            </tr>
            
            <tr>
                <td>
                    <asp:DropDownList ID="ddlCode4" runat="server" Width="230px" CssClass="combo" AutoPostBack="True" OnSelectedIndexChanged="ddlCode4Selected"></asp:DropDownList>
                </td>
                <td style="padding-left:10px">
                    <asp:TextBox ID="txtAvail4" runat="server"  Width="50px" CssClass="TextBox"></asp:TextBox>
                </td>
                  <td><asp:CheckBox ID="chknewyarn4" runat="server" CssClass="chkcol" /></td>
                <td><asp:CheckBox ID="chkshortage4" runat="server" CssClass="chkcol" /></td>
            </tr>
            <tr>
                <td>
                    <asp:DropDownList ID="ddlCode5" runat="server" Width="230px" CssClass="combo" AutoPostBack="True" OnSelectedIndexChanged="ddlCode5Selected"></asp:DropDownList>
                </td>
                <td style="padding-left:10px">
                    <asp:TextBox ID="txtAvail5" runat="server" Width="50px" CssClass="TextBox"></asp:TextBox>
                </td>
                  <td><asp:CheckBox ID="chknewyarn5" runat="server" CssClass="chkcol" /></td>
                <td><asp:CheckBox ID="chkshortage5" runat="server" CssClass="chkcol" /></td>
            </tr>
            <tr>
                <td>
                    <asp:DropDownList ID="ddlCode6" runat="server" Width="230px" CssClass="combo" AutoPostBack="True" OnSelectedIndexChanged="ddlCode6Selected"></asp:DropDownList>
                </td>
                <td style="padding-left:10px">
                    <asp:TextBox ID="txtAvail6" runat="server" Width="50px" CssClass="TextBox"></asp:TextBox>
                </td>
                  <td><asp:CheckBox ID="chknewyarn6" runat="server" CssClass="chkcol" /></td>
                <td><asp:CheckBox ID="chkshortage6" runat="server" CssClass="chkcol" /></td>
            </tr>
        </table>
    </div>
    <div style=" left: 950px;
             top: 430px;
             width: 675px;
             position: absolute;
             height: 25px;">
         <table>
             <tr>
                 <td><strong><asp:Label ID="Label54" runat="server" Text="COMMERICAL PROPERTIES" CssClass="clsFromLabel"></asp:Label> </strong> </td>
                 <td style="padding-left:20PX">
                     <asp:Button ID="btnAdd" runat="server" Text="Add" CssClass="btn btn-success btnfont btn-sm" OnClick="btnAdd_Click" />

                 </td>
             </tr>
         </table>
        </div>
    <div style=" left: 930px;
             top: 465px;
             width: 810px;
             position: absolute;
             margin-right: 0px;">
        <asp:GridView ID="gvDevelop_Request" runat="server" AutoGenerateColumns="false" emptydatatext="There is No Records To Display"
            OnRowDataBound="gvDevelop_Request_OnRowDataBound" DataKeyNames="pdr_new_develop_req_id,pdr_item_properties_id, 
            appl_id,sub_appl_id,spl_func_id,ctry,market_zone_id,market_customer_id" Width="809px" OnRowDeleting="OnRowDeleting"  BorderColor="#4CAF50" BorderStyle="Solid" BorderWidth="1px">
            <RowStyle Height="25" />
            <PagerStyle Height="25" />
            <HeaderStyle CssClass="grdheader" />
            <Columns>
                <asp:TemplateField ItemStyle-Width="120px" HeaderText="APPL" HeaderStyle-CssClass="gridcol" ItemStyle-CssClass="gridcol TextBox" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlAppl" runat="server" Width="120px" DataTextField="lookup_value_id" DataValueField="lookup_value" style="border:none" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-Width="120px" HeaderText="SUB APPL" HeaderStyle-CssClass="gridcol" ItemStyle-CssClass="gridcol TextBox" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlSubAppl" runat="server" Width="120px" DataTextField="lookup_value_id" DataValueField="lookup_value" style="border:none" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-Width="120px" HeaderText="SPL.FUNC" HeaderStyle-CssClass="gridcol" ItemStyle-CssClass="gridcol TextBox" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlsplfunc" runat="server" Width="120px" DataTextField="lookup_value_id" DataValueField="lookup_value" style="border:none"/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-Width="120px" HeaderText="CTRY" HeaderStyle-CssClass="gridcol" ItemStyle-CssClass="gridcol TextBox" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlctry" runat="server" Width="120px" DataTextField="ctry_name" DataValueField="ctry" style="border:none"/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-Width="120px" HeaderText="ZONE" HeaderStyle-CssClass="gridcol" ItemStyle-CssClass="gridcol TextBox" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlzone" runat="server" Width="120px" DataTextField="lookup_value_id" DataValueField="lookup_value" style="border:none"/>
                    </ItemTemplate>
                </asp:TemplateField>
                 <asp:TemplateField ItemStyle-Width="120px" HeaderText="CUSTOMER" HeaderStyle-CssClass="gridcol" ItemStyle-CssClass="gridcol TextBox" HeaderStyle-HorizontalAlign="Right" HeaderStyle-Font-Bold="false">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlcustomer" runat="server" Width="120px" DataTextField="lookup_value_id" DataValueField="lookup_value" style="border:none" />
                    </ItemTemplate>
                </asp:TemplateField>
                 <asp:CommandField ShowDeleteButton="True" ButtonType="Button" ControlStyle-CssClass="btn btn-danger btn-sm" />
            </Columns>
        </asp:GridView>
    </div>

      <div  style="left:313px; top:620px; width:450px;position:absolute">
        <asp:CheckBox ID="CHKFollow" runat="server" CssClass="chkcol" Text="FOLLOW CUSTOMER SPEC./STANDARD"/>
    </div>
     <div  style="left:50px; top:633px; width:450px;position:absolute">
         <strong>
        <asp:Label ID="ddlspec" runat="server" Text="SPEC REQUIRED:" BackColor="#999999" Width="200px" style="margin-top: 0px"></asp:Label>
         </strong>
    </div>
    <div style="left:50px; top:662px; width:295px; position:absolute; height: 226px; ">
        <table style="line-height:25px;">
            <tr>
                <td style="width:150px">
                    <asp:Label ID="Label25" runat="server" Text="WEIGHT(G/Sq.M)" Width="150px" CssClass="clsFromLabel"></asp:Label>

                </td>
                <td class="auto-style6">
                    <asp:TextBox ID="txtweight" runat="server" CssClass="TextBox numericcol" Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label20" runat="server" Text="WIDTH(CM)" CssClass="clsFromLabel"></asp:Label>

                </td>
                <td class="auto-style6">
                    <asp:TextBox ID="TXTWIDTH" runat="server" CssClass="TextBox numericcol" Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label27" runat="server" Text="GUAGE" CssClass="clsFromLabel"></asp:Label>

                </td>
                <td class="auto-style6">
                    <asp:TextBox ID="TXTGUAGE" runat="server" CssClass="TextBox numericcol" Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
                 <td style="padding-left:5px">
                    <asp:Label ID="Label59" runat="server" Text="DIA" CssClass="clsFromLabel"></asp:Label>

                </td>
                <td class="auto-style6" style="padding-left:5px">
                    <asp:TextBox ID="txtMachineTense" runat="server" Text="" CssClass="TextBox"  Width="60px"></asp:TextBox></td>
            </tr>
            <tr>
                &nbsp&nbsp;
            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label23" runat="server" Text="% EXTN AT LOAD" CssClass="clsFromLabel"></asp:Label>

                </td>
                <td class="auto-style6">
                    <asp:TextBox ID="TXTElongation" runat="server" CssClass="TextBox numericcol" Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label29" runat="server" Text="MODULUS @" CssClass="clsFromLabel"></asp:Label>

                </td>
                <td class="auto-style6">
                    <asp:TextBox ID="TXTMODULUS" runat="server" CssClass="TextBox numericcol" Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
                 <td>%</td>
            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label30" runat="server" Text="RECOV/STRETBILITY" CssClass="clsFromLabel"></asp:Label>

                </td>
                <td class="auto-style6">
                    <asp:TextBox ID="txtRecovStretbility" runat="server" CssClass="TextBox numericcol" Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
                 <td>%</td>
            </tr>
              <tr>
                <td style="width:160px;padding-right:5px">
                    <asp:Label ID="Label31" runat="server" Text="DIM.STBILITY/SHRNKGE"  CssClass="clsFromLabel"></asp:Label>

                </td>
                <td class="auto-style6">
                    <asp:TextBox ID="txtDimStbilityShr" runat="server" CssClass="TextBox numericcol" Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
                 <td>%</td>
            </tr>
        </table>
    </div>
    <div style="left: 390px;
             top: 725px;
             width: 97px;
             position: absolute;
             height: 170px;
             margin-top: 0px;">
        <table style="line-height:25px;">
            <tr>
                <td><asp:Label ID="lbllength" runat="server" Text="LENGTH" CssClass="clsFromLabel"></asp:Label></td>
            </tr>
             <tr>
                <td><asp:TextBox ID="txtElongation_l" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:TextBox ID="txtmodulus_l" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:TextBox ID="txtstretch_l" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
             <tr>
                <td><asp:TextBox ID="txtshrinkage_l" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
        </table>
    </div>
     <div style="margin-top: 0px ; left: 460px;
             top: 725px;
             width: 97px;
             position: absolute;
             height: 170px;">
        <table style="line-height:25px;">
            <tr>
                <td><asp:Label ID="Label32" runat="server" Text="WIDTH" CssClass="clsFromLabel"></asp:Label></td>
            </tr>
             <tr>
                <td><asp:TextBox ID="txtElongation_w" runat="server" Text="" CssClass="TextBox numericcol" Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:TextBox ID="txtmodulus_w" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:TextBox ID="txtstretch_w" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
              <tr>
                <td><asp:TextBox ID="txtshrinkage_w" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
        </table>
    </div>
    <div style="left:500px; top:655px; width:151px; position:absolute; ">
        <table style="line-height:15px;">
            <tr>
                <td><asp:Label ID="Label51" runat="server" Text="Tol" CssClass="clsFromLabel"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                 <td>
                     <asp:TextBox ID="txtweightTolerance" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox>

                 </td>
                 <td style="padding-left:10px">%</td>
                 <td  style="padding-left:10px">
                     <asp:Label ID="Label67" runat="server" Width="80px" Text="Shoe Size:" CssClass="clsFromLabel"></asp:Label>&nbsp;&nbsp;&nbsp;
                 </td>
                 <td>
                        <asp:DropDownList ID="ddlShoeSize" runat="server" width="200px" CssClass="combo"></asp:DropDownList>               
                 </td>
                 <td  style="padding-left:10px"><asp:Label ID="Label68" runat="server" Width="80px" Text="Shoe Style:" CssClass="clsFromLabel"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                 <td>
                     <asp:TextBox ID="txtShoeStyle" runat="server" Text=""   Width="60px"></asp:TextBox>
                 </td>
                 <td  style="padding-left:10px"><asp:Label ID="Label69" runat="server" Width="80px" Text="Gender:" CssClass="clsFromLabel"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                 <td>
                        <asp:DropDownList ID="ddlShoeGender" runat="server" width="200px" CssClass="combo"></asp:DropDownList>               
                 </td>
                 <td style="padding-left:10px"> 
                       <asp:CheckBox ID="chkLaserCut" CssClass="chkcol" Width="120px" runat="server" Text="Laser cut" /> 

                 </td>
                 <td style="padding-left:10px">
                        <asp:CheckBox ID="chkWithPattern" CssClass="chkcol" Width="120px" runat="server" Text="With Pattern" /> 

                 </td>

            </tr>
              <tr>
                <td><asp:Label ID="Label52" runat="server" Text="Tol" CssClass="clsFromLabel"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                 <td><asp:TextBox ID="txtwidth_Tolerance" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
                 <td style="padding-left:10px">%</td>
                 <td  style="padding-left:10px"><asp:Label ID="Label70" runat="server" Width="80px" Text="Shoe Length(cm):" CssClass="clsFromLabel"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                 <td><asp:TextBox ID="txtShoeLength" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
                 <td  style="padding-left:10px"><asp:Label ID="Label71" runat="server" Width="80px" Text="Shoe Width(cm):" CssClass="clsFromLabel"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                 <td><asp:TextBox ID="txtShoeWidth" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
                 <td  style="padding-left:10px"><asp:Label ID="Label73" runat="server" Width="80px" Text="Pallet Pattern No:" CssClass="clsFromLabel"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                 <td><asp:TextBox ID="txtPalletPatternNo" runat="server" Text="" CssClass="TextBox"  Width="60px" ></asp:TextBox></td>
                 <td  style="padding-left:10px"><asp:Label ID="Label75" runat="server" Width="80px" Text="rpt/Roll:" CssClass="clsFromLabel"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                 <td><asp:TextBox ID="txtRptPerRoll" runat="server" Text="" CssClass="TextBox"  Width="60px" ></asp:TextBox></td>
                 <td  style="padding-left:10px"><asp:Label ID="Label74" runat="server" Width="80px" Text="Product Pattern No:" CssClass="clsFromLabel"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                 <td><asp:TextBox ID="txtProductPatternNo" runat="server" Text="" CssClass="TextBox"  Width="60px" ></asp:TextBox></td>
            </tr>
            </table>
    </div>
    <div style="top: 725px;
             left: 530px;
             width: 97px;
             position: absolute;
             height: 165px;">
        <table style="line-height:25px; width: 94px;">
            <tr>
                <td><asp:Label ID="Label33" runat="server" Text="% Tol" CssClass="clsFromLabel"></asp:Label></td>
            </tr>
             <tr>
                <td><asp:TextBox ID="txtelongation_tolerance" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
            <tr>
                <td ><asp:TextBox ID="txtmodulus_tolerance" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:TextBox ID="txtstretch_tolerance" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
              <tr>
                <td><asp:TextBox ID="txtshrinkage_tolerance" runat="server" Text="" CssClass="TextBox numericcol"  Width="60px" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
        </table>
    </div>
    <div style="left:677px; top:725px; width:328px; position:absolute; height: 120px;">
        <table style="line-height:25px;">
             <tr>
                <td><asp:Label ID="Label34" runat="server" Text="NOTE" CssClass="clsFromLabel"></asp:Label></td>
            </tr>
            <tr>
                <td><asp:TextBox ID="txtremark" runat="server"  Text="" TextMode="MultiLine" CssClass="TextBox" Width="317px" Height="150px" ></asp:TextBox></td>
            </tr>
            
        </table>
    </div>
    <div style="left:45px; top:890px; width:178px; position:absolute; height: 76px;" >
        <table cellspacing="10px" style="width: 150px; height: 53px">
            <tr>
               
                <td><asp:CheckBox ID="chkpilling" runat="server"  CssClass="chkcol" Text="PILLING"/></td>
            </tr>
            <tr>
              
                <td><asp:CheckBox ID="chksnagging" runat="server"  CssClass="chkcol" Text="SNAGGING"/></td>
            </tr>
        </table>
    </div>
     <div STYLE=" left:250px;
             top: 890px;
             width: 301px;
             position: absolute;
             height: 87px;
             margin-top: 0px;"  >
        <table cellspacing="10px">
            <tr>
                <td >
                    <asp:Label ID="Label36" runat="server" Text="ORIGINAL WASH" Width="127px" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td><asp:TextBox ID="txtpilingremark" runat="server" Text="" CssClass="TextBox"  ></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label37" runat="server" Text="ORIGINAL WASH" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td><asp:TextBox ID="txtsnaggingremark" runat="server" Text="" CssClass="TextBox"  ></asp:TextBox></td>
            </tr>
        </table>
    </div>
      <div style="left: 767px;
             top: 975px;
             width: 296px;
             position: absolute;
             height: 47px;" >
        <table cellspacing="10px" style="width: 292px">
            <tr>
                <td >
                    <asp:Label ID="Label38" runat="server" Text="FINISHING" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td><asp:DropDownList ID="ddlFinishing" runat="server" width="200px" CssClass="combo"></asp:DropDownList></td>
            </tr>
            
        </table>
    </div>
    <div style="left:50px; top:952px; width:296px; position:absolute; height: 18px;" >
        <strong>
        <asp:Label ID="lbldyeing" runat="server" Text="DYEING" BackColor="#999999" Width="200px"></asp:Label>
        </strong>
    </div>
      <div style="left:50px; top:976px; width:296px; position:absolute; height: 46px;" >
        <table cellspacing="10px" style="width: 290px">
            <tr>
                <td >
                    <asp:Label ID="Label39" runat="server" Text="DYEING COLOR / COLOR WAY:" Width="200px" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td><asp:TextBox ID="txtdyeincolor" runat="server" Text="" CssClass="TextBox"></asp:TextBox></td>
            </tr>
            
        </table>
    </div>
     <div style="left:480px; top:976px; width:240px; position:absolute; height: 49px;" >
        <table cellspacing="10px" style="width: 235px; ">
            <tr>
                <td style="padding-left:5px">
                    <asp:Label ID="Label40" runat="server" Text="COMMENT" Width="115px" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td style="padding-right:20px"><asp:TextBox ID="txtComment" runat="server" Text="" CssClass="TextBox"></asp:TextBox></td>
            </tr>
            
        </table>
    </div>
     <div  STYLE="
             left: 950px;
             top: 1030px;
             width: 330px;
             position: absolute;
             height: 49px;"  >
        <table cellspacing="10px" style="width: 320px;">
            <tr>
                <td style="padding-left=10px">
                    <asp:CheckBox ID="chkHangTagRequire" runat="server"   CssClass="chkcol" Text="HANG TAG REQUIRE"/></td>
            </tr>
            
        </table>
    </div>
     <div style="left:50px; top:1030px; width:400px; position:absolute;" >
        <table cellspacing="10px" style="width: 356px;">
            <tr>
                <td >
                    <strong>
                    <asp:Label ID="Label42" runat="server" Text="SAMPLE REQUIRE" Width="200px" BackColor="#999999"></asp:Label>
                    </strong>
                </td
           
                <td style="width:85px;"></td>
                <td><asp:TextBox ID="txtSampleRequire" runat="server" Text="" CssClass="TextBox numericcol" Width="80px" onkeydown="return jsDecimals(event);" ></asp:TextBox>PCS</td>
            </tr>
            <tr>
                <td>
                     <asp:Label ID="Label63" runat="server" Text="KG / ROLL" Width="150px" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td><asp:TextBox ID="txtkgperfinishedroll" runat="server" Text="" CssClass="TextBox numericcol" Width="80px" onkeydown="return jsDecimals(event);" ></asp:TextBox></td>
            </tr>
            
        </table>
    </div>
     <div style="left:450px; top:1030px; width:296px; position:absolute; " >
        <table style="width: 292px; ">
            <tr>
                <td >
                    <asp:Label ID="Label43" runat="server" Text="YARDAGE (MTS)" Width="150px" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td  >
                   <asp:TextBox ID="TXTYARDAGE" CssClass="TextBox numericcol" runat="server" onkeydown="return jsDecimals(event);"></asp:TextBox>

                </td>
                <td style="padding-left:10px">
                    <asp:Label ID="Label72" runat="server" Text="PAIRS" Width="50px" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td  >
                   <asp:TextBox ID="txtPairs" CssClass="TextBox numericcol" runat="server" onkeydown="return jsDecimals(event);"></asp:TextBox>
                </td>

            </tr>
            <tr>
                <td >
                    <asp:Label ID="Label64" runat="server" Text="TOTAL ROLLS" Width="150px" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td  >
                   <asp:TextBox ID="txtTotalFinishedRolls" CssClass="TextBox numericcol" runat="server" onkeydown="return jsDecimals(event);"></asp:TextBox></td>
            </tr>
        </table>
    </div>
    <%--<div style="left:50px; top:1089px; width:162px; position:absolute; height: 30px;" >
        <table >
            <tr>
                <td  >
                    <strong>
                    <asp:Label ID="Label44" runat="server" Text="R & D ONLY" Width="150px" BackColor="#999999"></asp:Label>
                    </strong>
                </td>
         </tr>
     </table>
        </div>--%>
    <%--<div style="left:50px; top:1106px; width:382px; position:absolute; height: 26px;" >
        <table>
            <tr>        <td style="width:150px">
                    <asp:Label ID="Label45" runat="server" Text="MACHINE DENSE" Width="200px" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td><asp:TextBox ID="txtMachineTense" runat="server" Text="" CssClass="TextBox" ></asp:TextBox></td>
            </tr>
            
        </table>
    </div>--%>
     <div style="left:50px; top:1080px; width:420px; position:absolute;" >
        <table >
            <tr>        <td style="width:190px" >
                </td>
                <td ><asp:CheckBox ID="chkYarnAvailable" runat="server" Text="YARN AVAILABLE"  Width="190px" CssClass="clsFromLabel" /></td>
            </tr>
             <tr>        <td style="width:200px">
                    <asp:Label ID="Label50" runat="server" Text="YARN AVAILABLE DATE" Width="200px" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td >
                     <INPUT class="datepicker TextBox" id="txtknittingappoinment"  type="text" size="10" name="txtknittingappoinment" runat="server"  >

                </td>
            </tr>
        </table>
    </div>
    <div style="left: 460px;
             top: 1080px;
             width: 267px;
             position: absolute;
             " >
        <table>
            <tr>        <td style="width:150px">
                    <asp:Label ID="Label47" runat="server" Text="READY DATE" CssClass="clsFromLabel" ></asp:Label>
                </td>
                <td> <INPUT class="datepicker TextBox" id="txtyarnavailabledate"  type="text" size="10" name="txtyarnavailabledate" runat="server" style="width:90px" ></td>
            </tr>
            <tr>        <td style="width:150px">
                    <asp:Label ID="Label48" runat="server" Text="FINISH DATE" CssClass="clsFromLabel" ></asp:Label>
                </td>
                <td> <INPUT class="datepicker TextBox" id="txtExpectedFinisheddate"  type="text" size="10" name="txtExpectedFinisheddate" runat="server" style="width:90px" >

                </td>
            </tr>
        </table>
    </div>
    <%-- <div style="left:661px; top:1113px; width:80px; position:absolute; height: 39px;" >
        <table style="width: 75px; height: 29px;">
            <tr>        <td style="width:150px">
                    <asp:CheckBox runat="server" ID="CHKSPEC" Text="SPEC." CssClass="chkcol"/>
                </td>
               
            </tr>
             <tr>        <td style="width:150px">
                    <asp:CheckBox runat="server" ID="chkQA" Text="QA." CssClass="chkcol"/>
                </td>
               
            </tr>
        </table>
    </div>--%>
      <div style="left:776px; top:1080px; width:293px; position:absolute;" >
        <table >
            <tr>
                 <td>
                    <asp:Label ID="LBLMASTERDATE" runat="server" Text="MASTER DATE" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td style="padding-left:20px"> <INPUT class="datepicker TextBox" id="txtspecmasterdate"  type="text" size="10" name="txtspecmasterdate" runat="server" style="width:90px" ></td>
            </tr>
            <tr>
                 <td>
                    <asp:Label ID="Label49" runat="server" Text="QA REPORT DATE" CssClass="clsFromLabel"></asp:Label>
                </td>
                <td style="padding-left:20px"> <INPUT class="datepicker TextBox" id="txtqa_reportdate"  type="text" size="10" name="txtqa_reportdate" runat="server" style="width:90px" ></td>
            </tr>
            </table>
          </div>
     <div id="dvProgressBar" style="visibility: hidden; left: 100px">
            <img id="Img1" src="~/Images/progress_bar.gif" runat="server" style="left: 200px; position: absolute; top: 130px;" />
        </div>
        <div style="top: 611px; left: 211px; position: absolute;">
            <strong>
                <asp:Label ID="Label66" runat="server" Visible="False" ForeColor="#009933"></asp:Label>
            </strong>
        </div>
    <%-- <div style=" left: 50px;
             top: 1150px;
             width: 897px;
             position: absolute;
             height: 26px;" >
        <table >
            <tr>
                <td><asp:Label ID="LBLATTACH" runat="server" Text="ATTACHED SUPPORT DOC" CssClass="clsFromLabel"></asp:Label></td>
                 <td style="width:400px">
                     <asp:FileUpload ID="uploadfiles1" runat="server" />
                     <asp:Label ID="lblFileName" runat="server" Text="" ></asp:Label>
                     </td>
                <td>
                      <asp:Button ID="btnView" runat="server" Text="View" CssClass="button"  OnClientClick="previewFile()" OnClick="btnView_Click"   /></a>
                    <asp:Image ID="img" runat="server" />
                </td>                
               
            </tr>
            </table>
            </div>--%>
    
    
<%--<script type="text/javascript">
        function previewFile() {
            <%--var preview = document.querySelector('#<%=uploadfiles1.ClientID %>').files[0];--%>
         <%--  var file = document.querySelector('#<%=uploadfiles1.ClientID %>').files[0];
          //  alert(file);
            var reader = new FileReader();

            reader.onloadend = function () {
                preview.src = reader.result;
            }

            if (file) {
                reader.readAsDataURL(file);
            } else {
                preview.src = "";
            }
        }
    </script>--%>

    <script src="http://code.jquery.com/jquery-1.9.1.js"></script>
    <script type="text/javascript">
        //$(function previewFile() {
        //    $("#uploadfiles1").change(function () {
        <%-- function previewFile() {
            
                var file = document.getElementById("<%=uploadfiles1.ClientID%>");

                var path = file.value;

                alert(path);
                if (path != '' && path != null) {
                    var q = path.substring(path.lastIndexOf('\\') + 1);
                    $("#lblFileName").html(q);
                }
           
        }--%>
    </script>
    
<script language="javascript" type="text/javascript">


    $("#uploadfiles1").change(function () {
        $(this).removeClass("bar");
    })
    function openNewWin(url) {
        alert(url);
        var x = window.open(url, 'mynewwin', 'width=600,height=600,toolbar=1');

        x.focus();

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

		</SCRIPT>
     <style>
         /* This is the style for the trigger icon. The margin-bottom value causes the icon to shift down to center it. */
         .ui-datepicker-trigger {
             margin-left: 5px;
             margin-bottom: -3px;
             margin-top: -11px;
         }
         /*.auto-style3 {
             left: 1070px;
             top: 48px;
             width: 304px;
             position: absolute;
             height: 23px;
         }*/
         /*.auto-style4 {
             left: 489px;
             top: 717px;
             width: 97px;
             position: absolute;
             height: 170px;
         }*/
         /*.auto-style5 {
             left: 380px;
             top: 717px;
             width: 97px;
             position: absolute;
             height: 170px;
             margin-top: 0px;
         }*/
         /*.auto-style6 {
             left: 767px;
             top: 975px;
             width: 296px;
             position: absolute;
             height: 47px;
         }*/
         /*.auto-style7 {
             top: 717px;
             left: 600px;
             width: 97px;
             position: absolute;
             height: 165px;
         }*/
         /*.auto-style9 {
             left: 460px;
             top: 1145px;
             width: 267px;
             position: absolute;
             height: 56px;
         }*/
         /*.auto-style10 {
             left: 1070px;
             top: 74px;
             width: 304px;
             position: absolute;
             height: 24px;
         }*/
         /*.auto-style11 {
             left: 50px;
             top: 192px;
             width: 1178px;
             height: 23px;
             position: absolute;
         }*/
         /*.auto-style12 {
             left: 50px;
             top: 1215px;
             width: 897px;
             position: absolute;
             height: 26px;
         }*/
         /*.auto-style13 {
             left: 765px;
             top: 1040px;
             width: 330px;
             position: absolute;
             height: 49px;
         }*/
         /*.auto-style14 {
             height: 44px;
         }*/
         /*.auto-style15 {
             left: 50px;
             top: 400px;
             width: 858px;
             position: absolute;
         }*/
         /*.auto-style16 {
             left: 866px;
             top: 432px;
             width: 675px;
             position: absolute;
             height: 25px;
         }*/
         /*.auto-style17 {
             left: 864px;
             top: 465px;
             width: 810px;
             position: absolute;
             margin-right: 0px;
         }*/
         /*.auto-style18 {
             height: 22px;
             font-family: Arial,sans-serif;
             font-size: 12px;
         }*/
         .auto-style2 {
             width: 282px;
         }

         .auto-style3 {
             width: 117px;
         }

         .auto-style4 {
             width: 22px;
         }

         .auto-style5 {
             width: 503px;
         }

         .auto-style6 {
             width: 75px;
         }

         .headertext {
             font-family: "Helvetica Neue",Helvetica,Arial,sans-serif;
             font-size: 12px;
             line-height: 1.428571429;
             font-weight: normal;
             color: #999;
         }
         .contenttext {
             font-family: "Helvetica Neue",Helvetica,Arial,sans-serif;
             font-size: 10px !important;
             line-height: 1.428571429;
         }
         .clsFromLabel {
    height: 22px;
    font-family: Arial,sans-serif;
    font-size: 10px !important;
    font-weight: normal;
}
    .TextBox {
    border: 1px solid #456879;
    border-radius: 3px;
    height: 25px;
    font-family: Arial,sans-serif;
  font-size: 10px !important;
    font-weight: normal;
}
 .combo {
    border: 1px solid #456879;
    border-radius: 3px;
    height: 23px;
    font-family: Arial,sans-serif;
      font-size: 10px !important;
    font-weight: normal;
}   
   .gridcol {
            padding-right: 3px;
            padding-left: 3px;
            padding-top: 5px;
            padding-bottom: 5px;
            font-family: Arial,sans-serif;
            font-size: 10px;
            font-weight: normal !important;
        }  
    .grdheader {
            background-color: #4CAF50; /* #D8F781;*/
            color: #FFFFFF;
            font-family: Arial,sans-serif;
            font-size: 11px;
            font-weight: bold;
        }
          </style>

   <%-- <script src="../Scripts/jquery-1.4.1.min.js"></script>--%>
    <script type="text/javascript">

        //        $('#txtPDRNO').keypress(function (event) {

        //	var keycode = (event.keyCode ? event.keyCode : event.which);
        //	if(keycode == '13'){
        //		alert('You pressed a "enter" key in textbox');	
        //	}
        //	event.stopPropagation();
        //});

        //$(document).keypress(function(event){

        //	var keycode = (event.keyCode ? event.keyCode : event.which);
        //	if (keycode == '13') {
        //	    event.stopPropagation();
        //		alert('You pressed a "enter" key in somewhere');	
        //	}

        //});
        function EnterEvent(e) {
            if (e.keyCode == 13) {
                __doPostBack('<%=myButton.UniqueID%>', "");
            }
        }

</script>
     <script type="text/javascript">
         function checkNumeric(event) {
             var kCode = event.keyCode || event.charCode; // for cross browser check

             //FF and Safari use e.charCode, while IE use e.keyCode that returns the ASCII value 
             if ((kCode > 57 || kCode < 48) && (kCode != 46 && kCode != 45)) {
                 //code for IE
                 if (window.ActiveXObject) {
                     event.keyCode = 0
                     return false;
                 }
                 else {
                     event.charCode = 0
                 }
             }
         }

         function jsDecimals(e) {

             var evt = (e) ? e : window.event;
             var key = (evt.keyCode) ? evt.keyCode : evt.which;
             if (key != null) {
                 key = parseInt(key, 10);
                 if ((key < 48 || key > 57) && (key < 96 || key > 105)) {
                     if (!jsIsUserFriendlyChar(key, "Decimals")) {
                         return false;
                     }
                 }
                 else {
                     if (evt.shiftKey) {
                         return false;
                     }
                 }
             }
             return true;
         }

         // Function to check for user friendly keys  
         //------------------------------------------
         function jsIsUserFriendlyChar(val, step) {
             // Backspace, Tab, Enter, Insert, and Delete  
             if (val == 8 || val == 9 || val == 13 || val == 45 || val == 46) {
                 return true;
             }
             // Ctrl, Alt, CapsLock, Home, End, and Arrows  
             if ((val > 16 && val < 21) || (val > 34 && val < 41)) {
                 return true;
             }
             if (step == "Decimals") {
                 if (val == 190 || val == 110) {  //Check dot key code should be allowed
                     return true;
                 }
             }
             // The rest  
             return false;
         }
         </script>
   <%-- <script type="text/javascript">

        function stopRKey(evt) {
            // tHIS function will prevent the page from refresh
            var evt = (evt) ? evt : ((event) ? event : null);
            var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
            if ((evt.keyCode == 13) && (node.type == "text")) { return false; }
        }

        document.onkeypress = stopRKey;

    </script>--%>
    <script src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.0.min.js" type="text/javascript"></script>
<script src="//ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/jquery-ui.min.js" type="text/javascript"></script>
<link href="//ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/themes/blitzer/jquery-ui.css"
    rel="Stylesheet" type="text/css" />

<script type="text/javascript">
    //$(function () {
    //    $("[id$=txtSearch]").autocomplete({
    //        source: function (request, response) {
    //            $.ajax({
    //                url: "Develop_Request.aspx/GetCustomers1",
    //                data: "{'prefix':'" + request.term + "'}",
    //                dataType: "JSON",
    //                type: "POST",
    //                contentType: "application/json; charset=utf-8",
    //                success: function (data) {
    //                    response($.map(data.d, function (item) {
    //                        return {
    //                            label: item.split('-')[0],
    //                            val: item.split('-')[1]
    //                        }
    //                    }))
    //                },
    //                error: function (response) {
    //                    alert(response.responseText);
    //                },
    //                failure: function (response) {
    //                    alert(response.responseText);
    //                }
    //            });
    //        },
    //        select: function (e, hfCustomerIdhfCustomerIdi) {
    //            $("[id$=hfCustomerId]").val(i.item.val);
    //        },
    //        minLength: 2
    //    });
    //});  
</script>
    
    <%--<script src="../Scripts/jquery.1.8.4.js"></script>--%>
        <script type="text/javascript">

            $(document).ready(function () {


                // search main
                $("[id*=txtSearchMain]").autocomplete({
                    source: function (request, response) {
                        debugger;
                        $.ajax({

                            url: '<%=ResolveUrl("Develop_Request.aspx/GetCustomers1") %>',
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
                        $("[id*=hidSearchMainValue]").val(i.item.val);// "-" + $("[id*=hfCustomerId]").val(i.item.label);
                        //alert(document.getElementById("ContentPlaceHolder1_hidSearchMainValue").value);
                        document.getElementById("ContentPlaceHolder1_txtCustCode").value = i.item.val;

                    }
                });
            })
</script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js"></script>
<script src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.9/jquery-ui.js" type="text/javascript"></script>
<link href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.9/themes/start/jquery-ui.css"
    rel="stylesheet" type="text/css" />
<script type="text/javascript">
    function OpenW() {
        //alert(document.getElementById("ContentPlaceHolder1_txtCustCode").value);
        var ccode = document.getElementById("ContentPlaceHolder1_txtCustCode").value;
        var pageURL = "AddEditCustomer.aspx";
        // window.open(pageURL , '', "height=500,width=700");
        var popupWidth = "50%";
        var popupHeight = "100%";
        //  return true;
        var targetPop = window.open(pageURL, '', 'toolbar=no, location=no, directories=no,status=no, menubar=no, scrollbars=YES, resizable=YES, copyhistory=no,width=600, height=500, top=150, left=150');
        return false;
    }
</script>
    <script type="text/javascript">
        function OpenWinEdit() {
            //alert(document.getElementById("ContentPlaceHolder1_txtCustCode").value);
            var ccode = document.getElementById("ContentPlaceHolder1_txtCustCode").value;
            if (ccode == '') {
                alert("Select the customer");
                return false;
            }
            var pageURL = "AddEditCustomer.aspx?custcode=" + ccode;
            // window.open(pageURL , '', "height=500,width=700");
            var popupWidth = "50%";
            var popupHeight = "100%";
            //  return true;
            var targetPop = window.open(pageURL, '', 'toolbar=no, location=no, directories=no,status=no, menubar=no, scrollbars=YES, resizable=YES, copyhistory=no,width=600, height=500, top=150, left=150');
            return false;
        }
</script>
        <script type = "text/javascript">
            function Confirm() {
                var confirm_value = document.createElement("INPUT");
                confirm_value.type = "hidden";
                confirm_value.name = "confirm_value";
                if (confirm("Are you sure to Cancel this PDR?")) {
                    confirm_value.value = "Yes";
                } else {
                    confirm_value.value = "No";
                }
                document.forms[0].appendChild(confirm_value);
            }
    </script>
     <script type = "text/javascript">
         function ConfirmCopy() {
             var copy_value = document.createElement("INPUT");
             copy_value.type = "hidden";
             copy_value.name = "copy_value";
             if (confirm("Are you sure to Copy this PDR?")) {
                 copy_value.value = "Yes";
             } else {
                 copy_value.value = "No";
             }
             document.forms[0].appendChild(copy_value);
         }
    </script>
      <script type="text/javascript">
          function ValidateRange() {

              document.forms[0].target = "_blank";
              return true;
          }
  </script>
    <script type="text/javascript">
        function ShowProgressBar() {
            document.getElementById('dvProgressBar').style.visibility = 'visible';
        }

        function HideProgressBar() {
            document.getElementById('dvProgressBar').style.visibility = "hidden";
        }
</script>
  <body onkeydown="if(event.keyCode==13){event.keyCode=9; return event.keyCode}">

    </div>
    </div>
</asp:Content>

