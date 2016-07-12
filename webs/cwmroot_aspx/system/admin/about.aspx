<%@ Page Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.About" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify user account" id="cammWebManager"
    SecurityObject="System - User Administration - ServerSetup" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>

<script runat="server">
	
</script>

<!--#include virtual="/sysdata/includes/standardtemplate_top_wo_form.aspx"-->
<h3>
    <font face="Arial">Administration - About WebManager</font></h3>
<p>
    <font face="Arial" size="2" color="red">
        <asp:label runat="server" id="lblErrMsg" />
    </font>
</p>
<table cellspacing="0" cellpadding="0" bgcolor="#ffffff" border="0">
    <tbody>
        <tr>
            <td valign="top">
                <table cellspacing="0" cellpadding="3" width="100%" border="0" bordercolor="#FFFFFF">
                    <tbody>
                        <tr>
                            <td colspan="2" bgcolor="#C1C1C1">
                                <p>
                                    <font face="Arial" size="2"><b>Software details</b></font></p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Product</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2">
                                        <%= cammWebManager.System_ProductName %></font></p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Base-Version</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2">
                                        <%=CurrentApplicationVersion.ToString(4)%></font></p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Build</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2">
                                        <%= CurrentApplicationVersion.Build %></font></p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Admin-Version</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2">
                                        <%=CurrentAdminAreaVersion.ToString(4)%></font></p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Debug level</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2">
                                        <%
                                            Dim DebugLevel_Descr As String
                                            Select Case cammWebManager.System_DebugLevel
                                                Case 0
                                                    DebugLevel_Descr = "<p>Disabled</p>"
                                                Case 1
                                                    DebugLevel_Descr = "<p>Warning messages will be sent to the developer contact configured in your config files.</p>"
                                                Case 2
                                                    DebugLevel_Descr = "" & _
                                                     "<p>Additional warning messages will be sent to the developer contact configured in your config files.</p>"
                                                Case 3
                                                    DebugLevel_Descr = "" & _
                                                     "<p>Even more additional warning messages will be sent to the developer contact configured in your config files.</p>"
                                                Case 4
                                                    DebugLevel_Descr = "" & _
                                                     "<p>Advanced logging and all e-mail messages will be sent to the developer contact configured in your config files.</p>"
                                                Case 5
                                                    DebugLevel_Descr = "<p>All e-mails will be redirected to the developer; no e-mail will be sent to the origin receipient.</p>" & _
                                                     "<p>Warning messages will be sent to the developer contact configured in your config files.</p>" & _
                                                     "<p>Automatic redirections caused by camm Web-Manager require manual execution.</p>"
                                                Case Else
                                                    DebugLevel_Descr = "<p>Unknown/undefined debug level</p>"
                                            End Select
                                            Response.Write(DebugLevel_Descr)
                                        %></font></p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Options</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2"><a href="configuration.aspx">Advanced configuration</a></font></p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" colspan="2">
                                <p>
                                    &nbsp;</p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Updates from</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2"><a target="_blank" href="https://www.camm.biz/redir/?R=31&PrefLang=<%= cammWebManager.UIMarket %>&SN=<%= Server.URLEncode(cammWebManager.System_Licence) %>&Build=<%= cammWebManager.System_Build %>&V=<%= cammWebManager.System_Version %>">
                                        camm.biz</a> <asp:Literal runat="server" id="ltrlUpdateHint"/></font></p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Additional modules from</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2"><a target="_blank" href="https://www.camm.biz/redir/?R=32&PrefLang=<%= cammWebManager.UIMarket %>&SN=<%= Server.URLEncode(cammWebManager.System_Licence) %>&Build=<%= cammWebManager.System_Build %>&V=<%= cammWebManager.System_Version %>">
                                        camm.biz</a></font></p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" colspan="2">
                                <p>
                                    &nbsp;</p>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" bgcolor="#C1C1C1">
                                <p>
                                    <font face="Arial" size="2"><b>Database details</b></font></p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Version</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2">
                                        <%= CurrentDatabaseBuild.Major & "." & CurrentDatabaseBuild.Minor.ToString("00") %></font></p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Build</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2">
                                        <%= CurrentDatabaseBuild.Build %>
                                        <asp:literal runat="server" id="brDbUpdate" visible="false"><br></asp:literal>
                                        <a href="../../system/admin/install/update.aspx" runat="server" id="hrefDbUpdate"
                                            visible="false">Update database now</a></font></p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" colspan="2">
                                <p>
                                    &nbsp;</p>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" bgcolor="#C1C1C1">
                                <p>
                                    <font face="Arial" size="2"><b>Check for common issues</b></font></p>
                            </td>
                        </tr>
						<tr>
							<td colspan="2" valign"Top">
							<font face="Arial" size="2">
							Last WebCron executed on: <asp:Label runat="server" id="lblLastWebCronExecution" />
							</font>
							</td>
						</tr>
                        
                        <tr>                            <td colspan="2" valign="Top">                                <p>                                    <font face="Arial" size="2"><a href="check_flags_not_required.aspx">Check for additional flags in use by userprofiles not required by security objects</a></font></p>                            </td>                        </tr>                      <%'  <tr>                            <td colspan="2" valign="Top">                                <p>                                    <font face="Arial" size="2"><a href="#">Check for missing external account assignment</a></font></p>                            </td>                        </tr>                        %>
                        <tr>
                            <td colspan="2" valign="Top">
                                <p>
                                    <font face="Arial" size="2"><a href="check_file_consistency.aspx">Check for missing files in webroot</a></font></p>
                            </td>
                        </tr>
                        <%'<tr>                            <td colspan="2" valign="Top">                                <p>                                    <font face="Arial" size="2"><a href="#">Check all CWM instances/ versions</a></font></p>                            </td>                        </tr>                        <tr>                            <td colspan="2" valign="Top">                                <p>                                    <font face="Arial" size="2"><a href="#">Check versions of all installed components</a></font></p>                            </td>                        </tr> %>
                        <tr>
                            <td valign="Top" colspan="2">
                                <p>
                                    &nbsp;</p>
                            </td>
                        </tr>
                        <tr runat="server" id="trSecurity">
                            <td valign="Top" colspan="2">
                                <table cellspacing="0" cellpadding="3" width="100%" border="0">
                                    <tr>
                                        <td colspan="2" bgcolor="#C1C1C1">
                                            <p>
                                                <font face="Arial" size="2"><b>Security recommendations</b></font></p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="Top" width="160">
                                            <p>
                                                <font face="Arial" size="2">Remove temporary
                                                    <br>
                                                    install files</font></p>
                                        </td>
                                        <td valign="Top" width="240">
                                            <p>
                                                <font face="Arial" size="2">
                                                    <% Me.FindFilesToBeRemoved()%><asp:label runat="server" id="lblInstallLinks" /><br>
                                                    <form name="FORM1" runat="server">
                                                    <asp:linkbutton id="lnkBtn" runat="server" text="Delete Now" />
                                                    </form>
                                                </font>
                                            </p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="Top" colspan="2">
                                            <p>
                                                &nbsp;</p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" bgcolor="#C1C1C1">
                                <p>
                                    <font face="Arial" size="2"><b>Licence details</b> </font></p>
                            </td>
                        </tr>
						<tr>
							<td valign="Top" width="160">
							 <p>
                                    <font face="Arial" size="2"> <a href="about.aspx?forceserverrefresh=1">Refresh data from server</a></font></p>
									
							
							</td>
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Licence agreement</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2">By installing or running this software you have agreed to
                                        the following licence terms:<br>
                                        <a target="_blank" href="<%= Me.LicenseFile %>">Licence contract</a></font></p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Licence key</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2">
									<% If Me.CurrentAdminIsSupervisor Then %>
										<%= Me.cammWebManager.Environment.LicenceKey %>
									<% Else %>
										<%= Me.cammWebManager.Environment.LicenceKeyShortened & "..." %>
									<% End If %>
                                    </font></p>
                            </td>
                        </tr>
								<tr>
							 <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Licence model</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2"><asp:Literal runat="server" id="ltrlLicenceModel"/></font></p>
                            </td>
						</tr>
						
						<tr>
							 <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Licence type</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2"><asp:Literal runat="server" id="ltrlLicenceType"/></font></p>
                            </td>
						</tr>
						<tr>
							 <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Licence expiration date (UTC)</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2"><asp:Literal runat="server" id="ltrlLicenceExpirationDate"/></font></p>
                            </td>
						</tr>
					
						
						<tr>
							 <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Support and maintenance contract expiration date (UTC)</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2"><asp:Literal runat="server" id="ltrlSupportContractExpirationDate"/></font></p>
                            </td>
						</tr>
						
						<tr>
							 <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Update contract expiration date (UTC)</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2"><asp:Literal runat="server" id="ltrlUpdateContractExpirationDate"/></font></p>
                            </td>
						</tr>
						
						
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Licence description</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2"><asp:Literal runat="server" id="lblLicenceDescription"/></font></p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" colspan="2">
                                <p>
                                    &nbsp;</p>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <p>
                    <a href="http://www.camm.biz/" target="_blank" title="The Enterprise Web Manager is the software solution for managing Extranet, Intranet and Internet pages">
                        <img align="left" border="0" width="211" height="64" src="/system/admin/images/logo_camm_biz.gif"></a>
            </td>
        </tr>
    </tbody>
</table>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="users.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_wo_form.aspx"-->
