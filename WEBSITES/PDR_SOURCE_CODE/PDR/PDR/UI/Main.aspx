<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="PDR.UI.Main" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css" rel="stylesheet"/>
    <%--<script src="//maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>--%>
    <%--<script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>--%>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    </head>
<body>
    <form id="frmMain" runat="server">
        
        <div style="max-width: 600px;height:600px"   >
            
            <div class="panel panel-default ">
                <div class="panel-body" style="height:600px">
                    <div class="container text-primary text-center row" style="width: 400px; height: 30px;left:50px">
            <strong>PDR SYSTEM
            </strong>
            </div>
                    <div class="form-horizontal" role="form">
                        <div class="form-group" >
                            <div class="col-sm-offset-2 col-sm-10" style="height: 36px; width: 222px">
                                <a target="_blank" href="pdr_list.aspx">
                                    <input type="button" class="btn btn-success" value="CREATE / EDIT / TRACK PDR" style="width: 327px" />
                                </a>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-sm-offset-2 col-sm-10" style="height: 36px; width: 222px">
                                <a target="_blank" href="DR_Sample_Dispo.aspx">
                                    <input type="button" class="btn btn-success" value="KNITTING SAMPLE DISPO" style="width: 327px" />
                                </a>
                            </div>
                        </div>
                                 <div class="form-group">
                            <div class="col-sm-offset-2 col-sm-10" style="height: 36px; width: 222px">
                                <a target="_blank" href="DR_Knitting_Sample_Approve.aspx">
                                    <input type="button" class="btn btn-success" value="KNITTING APPROVE" style="width: 327px" />
                                </a>
                            </div>
                        </div>
                     <%--   <div class="form-group">
                            <div class="col-sm-offset-2 col-sm-10" style="height: 36px; width: 222px">
                                <a target="_blank" href="DR_Knitting_Sample_Approve.aspx">
                                    <input type="button" class="btn btn-success" value="KNITTING APPROVE" style="width: 222px" /></a></div>
                        </div
                          </div>--%>
                        <div class="form-group">
                            <div class="col-sm-offset-2 col-sm-10" style="height: 36px; width: 222px">
                                <a target="_blank" href="DR_Approval_Internal.aspx?Type=INTERNAL">
                                    <input type="button"  class="btn btn-success " value="INTERNAL APPROVE & SEND TO CUSTOMER" style="width: 327px; height: 36px;" >
                                </a>
                            </div>
                        </div>
                            <div class="form-group">
                                <div class="col-sm-offset-2 col-sm-10" style="height: 36px; width: 222px">
                                    <a target="_blank" href="DR_Customer_Response.aspx?Type=CUSTOMER">
                                        <input type="button" class="btn btn-success" value="CUSTOMER APPROVE" style="width: 327px" />
                                    </a>
                                </div>
                            </div>
                         
                      <div class="form-group">
                            <div  class="col-sm-offset-2 col-sm-10" style="height: 36px; width: 222px">
                                <a target="_blank" href="dr_final_approve.aspx?type=final">
                                    <input type="button" class="btn btn-success" value="FINAL APPROVE" style="width: 327px" />
                                </a>                              
                            </div>
                        </div>

                            <div class="form-group">
                                <div class="col-sm-offset-2 col-sm-10" style="height: 36px; width: 222px">
                                    <a target="_blank" href="PDRArticleListMulti2.aspx">
                                        <input type="button" class="btn btn-success" value="ARTICLE & PRICE LIST" style="width: 327px" />
                                    </a>
                                </div>
                            </div>

                          <div class="form-group">
                            <div class="col-sm-offset-2 col-sm-10" style="height: 36px; width: 222px">                               
                                <asp:button id="btnlogout"  class="btn btn-success" style="width: 327px" runat="server" text="LOGOUT" onclick="BTNLOGOUT_Click1" />
                            </div>
                        </div>
                        <%--    <div class="form-group">
                            <div class="col-sm-offset-2 col-sm-10" style="height: 36px; width: 190px">
                                <a target="_blank" href="knitting_approval.aspx">
                                    <input type="button" class="btn btn-success" value="KNITTING APPROVE" style="width: 190px" />
                                </a>
                            </div>
                        </div>--%>
                         
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
