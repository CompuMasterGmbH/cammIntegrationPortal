<%@ Page Language="vb" AutoEventWireup="false" ValidateRequest="false" Inherits="CompuMaster.camm.WebManager.Modules.Text.Administration.Pages.ProcessCopyOfMarketID" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebmanager" runat="server" SecurityObject="System - TextModules" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<a name="Top"></a>
<table cellpadding="3" cellspacing="0" width="100%">
    <tr>
        <td>
            <h1>
                Textmodule Administration</h1>
            <h2>
                Copy TextModuleItens from market to market or create a new market</h2>
            <a href="textmodules_overview.aspx">back to overview</a>
        </td>
    </tr>
    <tr>
        <td>
            <asp:panel runat="server" id="pnlInput" />
            <table style="width: 100%;">
                <tr>
                    <td>
                        <table style="font-size: 11px">
                            <tr>
                                <td style="font-weight: bold">
                                    SourceWebsiteAreaID:</td>
                                <td>
                                    <%=SourceWebsiteAreaID%>
                                </td>
                                <td style="font-weight: bold">
                                    SourceMarketID:</td>
                                <td>
                                    <%=SourceMarketID%>
                                </td>
                                <td style="font-weight: bold">
                                    SourceServerGroupID:</td>
                                <td>
                                    <%=SourceServerGroupID%>
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold">
                                    TargetWebsiteAreaID:</td>
                                <td>
                                    <%=TargetWebsiteAreaID%>
                                </td>
                                <td style="font-weight: bold">
                                    TargetMarketID:</td>
                                <td>
                                    <%=TargetMarketID%>
                                </td>
                                <td style="font-weight: bold">
                                    TargetServerGroupID:</td>
                                <td>
                                    <%=TargetServerGroupID%>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <a href="#TextModules">TextModules</a> <a href="#Textblocks">Textblocks</a> <a href="#Variables">Variables</a>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100%;">
                        <div style="border: solid 1px #000000; margin-bottom: 10px;">
                            <a name="TextModules"></a>
                            <table cellspacing="0" cellpadding="0" style="width: 100%">
                                <tr>
                                    <td>
                                        <h2 style="padding: 0; margin: 0; font-size: 20px">
                                            TextModules</h2>
                                    </td>
                                    <td align="right">
                                        <a href="#Top">Top</a></td>
                                </tr>
                            </table>
                            <table cellpadding="0" cellspacing="0" style="width: 100%; font-size: 12px;">
                                <thead style="font-weight: bold">
                                    <tr>
                                        <td>
                                            Key</td>
                                        <td>
                                            Source value</td>
                                        <td>
                                            Current value</td>
                                        <td>
                                            New value</td>
                                        <td align="center">
                                            Copy
                                            <input type="button" onclick="SelectAllCheckBoxex('chkcopyvalueTM')" value="a" title="Select all items." /><input type="button" onclick="SelectNoneCheckBoxex('chkcopyvalueTM')" value="n" title="Select none items." /><input type="button" onclick="SelectInvertCheckBoxex('chkcopyvalueTM')" value="i" title="Invert the current selection." /></td>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%  

                                        
                                        Dim AlternatingBgColor As String = "#ffffff"
                                        Try
                                            If MergedItems.Length = 0 Then
                                                Throw New Exception
                                            End If
                                            For Each item As CompuMaster.camm.WebManager.Modules.Text.TextModules.ModuleItem In MergedItems
                                                
                                                If item.TypeID = CompuMaster.camm.WebManager.Modules.Text.TextModules.TextModuleType.HtmlTemplate Then
                                                    If AlternatingBgColor = "#ffffff" Then
                                                        AlternatingBgColor = "#efefef"
                                                    Else
                                                        AlternatingBgColor = "#ffffff"
                                                    End If
                                                
                                    %>
                                    <tr>
                                        <td style="background-color: <%=AlternatingBgColor %>; width: 150px;">
                                            <div style="width: 150px; height: 200px; overflow: auto;">
                                                <%=server.htmlencode(item.Key)%>
                                            </div>
                                        </td>
                                        <td style="background-color: <%=AlternatingBgColor %>; width: 300px;">
                                            <div style="width: 300px; height: 200px; overflow: scroll; border: solid 1px #cecece; padding: 3px;">
                                                <%=GetModuleItemByKey(item.Key, SourceItems).Value%>
                                            </div>
                                        </td>
                                        <td style="background-color: <%=AlternatingBgColor %>; width: 300px">
                                            <div style="width: 300px; height: 200px; overflow: scroll; border: solid 1px #cecece; padding: 3px;">
                                                <%=GetModuleItemByKey(item.Key, TargetItems).Value%>
                                            </div>
                                        </td>
                                        <td style="background-color: <%=AlternatingBgColor %>; width: auto">
                                            <%
                                                Dim InputValue As String = Nothing
                                                If Page.IsPostBack Then
                                                    'get the form value. to prevent overwriting of content that is changed by user.
                                                    InputValue = Request.Form("txtnewvalueTM#" & item.Key)
                                                Else
                                                    'first time load. fill input field with source value.
                                                    InputValue = Server.HtmlEncode(GetModuleItemByKey(item.Key, SourceItems).Value)
                                                    
                                                End If
                                            %>
                                            <textarea rows="5" name="txtnewvalueTM#<%=item.key %>" onfocus="this.select()" style="width: 100%; height: 200px"><%=inputValue%></textarea>
                                        </td>
                                        <td style="background-color: <%=AlternatingBgColor %>; width: 100px" align="center">
                                            <input type="checkbox" name="chkcopyvalueTM#<%=item.key %>" <%=iif(GetModuleItemByKey(item.Key, SourceItems).Value <> GetModuleItemByKey(item.Key, TargetItems).Value, "checked=""checked""", "") %> />
                                        </td>
                                    </tr>
                                    <%           
                                    End If
                                Next
                            Catch ex As Exception%>
                                    <%=ex.Message%>
                                    <%
                                    End Try
                                    %>
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td colspan="2">
                                            <a href="#Top">Top</a></td>
                                        <td align="right" colspan="3">
                                            <asp:button runat="server" id="btnSaveMarketIDTextModules" text="Save TextModule" />
                                        </td>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>
                        <a name="TextBlocks"></a>
                        <div style="border: solid 1px #000000; margin-bottom: 10px;">
                            <a name="Textblocks"></a>
                            <table cellspacing="0" cellpadding="0" style="width: 100%">
                                <tr>
                                    <td>
                                        <h2 style="padding: 0; margin: 0; font-size: 20px">
                                            Textblocks</h2>
                                    </td>
                                    <td align="right">
                                        <a href="#Top">Top</a></td>
                                </tr>
                            </table>
                            <table cellpadding="0" cellspacing="0" style="width: 100%; font-size: 12px;">
                                <thead style="font-weight: bold">
                                    <tr>
                                        <td>
                                            Key</td>
                                        <td>
                                            Source value</td>
                                        <td>
                                            Current value</td>
                                        <td>
                                            New value</td>
                                        <td align="center">
                                            Copy
                                            <input type="button" onclick="SelectAllCheckBoxex('chkcopyvalueTB')" value="a" title="Select all items." /><input type="button" onclick="SelectNoneCheckBoxex('chkcopyvalueTB')" value="n" title="Select none items." /><input type="button" onclick="SelectInvertCheckBoxex('chkcopyvalueTB')" value="i" title="Invert the current selection." /></td>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%  

                                        
                                        AlternatingBgColor = "#ffffff"
                                        Try
                                            If MergedItems.Length = 0 Then
                                                Throw New Exception
                                            End If
                                            For Each item As CompuMaster.camm.WebManager.Modules.Text.TextModules.ModuleItem In MergedItems
                                                If item.TypeID = CompuMaster.camm.WebManager.Modules.Text.TextModules.TextModuleType.PlainTextBlock Or item.TypeID = CompuMaster.camm.WebManager.Modules.Text.TextModules.TextModuleType.HtmlTextBlock Then
                                                    If AlternatingBgColor = "#ffffff" Then
                                                        AlternatingBgColor = "#efefef"
                                                    Else
                                                        AlternatingBgColor = "#ffffff"
                                                    End If
                                    %>
                                    <tr>
                                        <td style="background-color: <%=AlternatingBgColor %>; width: 150px;">
                                            <div style="width: 150px; height: 200px; overflow: auto;">
                                                <%=server.htmlencode(item.Key)%>
                                            </div>
                                        </td>
                                        <td style="background-color: <%=AlternatingBgColor %>; width: 300px;">
                                            <div style="width: 300px; height: 200px; overflow: scroll; border: solid 1px #cecece; padding: 3px;">
                                                <%=GetModuleItemByKey(item.Key, SourceItems).Value%>
                                            </div>
                                        </td>
                                        <td style="background-color: <%=AlternatingBgColor %>; width: 300px">
                                            <div style="width: 300px; height: 200px; overflow: scroll; border: solid 1px #cecece; padding: 3px;">
                                                <%=GetModuleItemByKey(item.Key, TargetItems).Value%>
                                            </div>
                                        </td>
                                        <td style="background-color: <%=AlternatingBgColor %>; width: auto">
                                            <%
                                                Dim InputValue As String = Nothing
                                                If Page.IsPostBack Then
                                                    InputValue = Request.Form("txtnewvalueTB#" & item.Key)
                                                Else
                                                    InputValue = Server.HtmlEncode(GetModuleItemByKey(item.Key, SourceItems).Value)
                                                End If
                                            %>
                                            <textarea rows="5" name="txtnewvalueTB#<%=item.key %>" onfocus="this.select()" style="width: 100%; height: 200px"><%=inputValue%></textarea>
                                        </td>
                                        <td style="background-color: <%=AlternatingBgColor %>; width: 100px" align="center">
                                            <input type="checkbox" name="chkcopyvalueTB#<%=item.key %>" <%=iif(GetModuleItemByKey(item.Key, SourceItems).Value <> GetModuleItemByKey(item.Key, TargetItems).Value, "checked=""checked""", "") %> />
                                        </td>
                                    </tr>
                                    <%           
                                    End If
                                Next
                            Catch ex As Exception%>
                                    <%=ex.Message%>
                                    <%
                                    End Try
                                    %>
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td colspan="2">
                                            <a href="#Top">Top</a></td>
                                        <td align="right" colspan="3">
                                            <asp:button runat="server" id="btnSaveMarketIDTextBlocks" text="Save Texblocks" />
                                        </td>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>
                        <a name="Variables"></a>
                        <div style="border: solid 1px #000000; margin-bottom: 10px;">
                            <a name="Variables"></a>
                            <table cellspacing="0" cellpadding="0" style="width: 100%">
                                <tr>
                                    <td>
                                        <h2 style="padding: 0; margin: 0; font-size: 20px">
                                            Variables</h2>
                                    </td>
                                    <td align="right">
                                        <a href="#Top">Top</a></td>
                                </tr>
                            </table>
                            <table cellpadding="0" cellspacing="0" style="width: 100%; font-size: 12px;">
                                <thead style="font-weight: bold">
                                    <tr>
                                        <td>
                                            Key</td>
                                        <td>
                                            Source value</td>
                                        <td>
                                            Current value</td>
                                        <td>
                                            New value</td>
                                        <td align="center">
                                            Copy
                                            <input type="button" onclick="SelectAllCheckBoxex('chkcopyvalueVA')" value="a" title="Select all items." /><input type="button" onclick="SelectNoneCheckBoxex('chkcopyvalueVA')" value="n" title="Select none items." /><input type="button" onclick="SelectInvertCheckBoxex('chkcopyvalueVA')" value="i" title="Invert the current selection." /></td>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%  

                                        AlternatingBgColor = "#ffffff"
                                        Try
                                            If MergedItems.Length = 0 Then
                                                Throw New Exception
                                            End If
                                            For Each item As CompuMaster.camm.WebManager.Modules.Text.TextModules.ModuleItem In MergedItems
                                                If item.TypeID = CompuMaster.camm.WebManager.Modules.Text.TextModules.TextModuleType.PlainTextString Then
                                                    If AlternatingBgColor = "#ffffff" Then
                                                        AlternatingBgColor = "#efefef"
                                                    Else
                                                        AlternatingBgColor = "#ffffff"
                                                    End If
                                    %>
                                    <tr>
                                        <td style="background-color: <%=AlternatingBgColor %>;">
                                            <div style="width: 150px; height: 40px; overflow: auto;">
                                                <%=server.htmlencode(item.Key)%>
                                            </div>
                                        </td>
                                        <td style="background-color: <%=AlternatingBgColor %>;">
                                            <%=server.htmlencode(GetModuleItemByKey(item.Key, SourceItems).Value)%>
                                        </td>
                                        <td style="background-color: <%=AlternatingBgColor %>;">
                                            <%=server.htmlencode(GetModuleItemByKey(item.Key, TargetItems).Value)%>
                                        </td>
                                        <td style="background-color: <%=AlternatingBgColor %>;">
                                            <%
                                                Dim InputValue As String = Nothing
                                                If Page.IsPostBack Then
                                                    InputValue = Request.Form("txtnewvalueVA#" & item.Key)
                                                Else
                                                    InputValue = Server.HtmlEncode(GetModuleItemByKey(item.Key, SourceItems).Value)
                                                End If
                                            %>
                                            <input type="text" name="txtnewvalueVA#<%=item.key %>" value="<%=inputValue%>" onfocus="this.select()" style="width: auto" />
                                        </td>
                                        <td style="background-color: <%=AlternatingBgColor %>; width: 100px" align="center">
                                            <input type="checkbox" name="chkcopyvalueVA#<%=item.key %>" <%=iif(GetModuleItemByKey(item.Key, SourceItems).Value <> GetModuleItemByKey(item.Key, TargetItems).Value, "checked=""checked""", "") %> />
                                        </td>
                                    </tr>
                                    <%           
                                    End If
                                Next
                            Catch ex As Exception%>
                                    <%=ex.Message%>
                                    <%
                                    End Try
                                    %>
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td colspan="2">
                                            <a href="#Top">Top</a></td>
                                        <td align="right" colspan="3">
                                            <asp:button runat="server" id="btnSaveMarketIDVariables" text="Save Variables" />
                                        </td>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>
                        <div style="text-align: right">
                            <asp:button runat="server" id="btnSaveMarketIDAll" text="Save all" />
                        </div>
                    </td>
                </tr>
            </table>
            </asp:panel>
            <asp:literal runat="server" id="ltrCopyMsgs" />
        </td>
    </tr>
</table>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
