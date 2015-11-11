<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.UpdateServer"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify server" id="cammWebManager" SecurityObject="System - User Administration - Applications" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

<script language="Javascript">
<!--
function MyCheckURL()
{
		var MyURL = '';
		MyURL = document.getElementById("txtProtocal").value + '://' + document.getElementById("txtServerName").value;
		MyURL = MyURL + document.getElementById("ServerTestURL").value + '?Host=' + escape(document.getElementById("txtHostHeader").value);
		window.open(MyURL,'CheckServer','toolbar=no,menubar=no,scrollbars=yes,width=450,height=350,location=yes');
}
//-->
</script>
		<h3><font face="Arial">Administration - Modify server</font></h3>
		<p><font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErrMsg" /></font></p>
		<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
			<TBODY>
				<TR>
					<TD vAlign="top">
		
							<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
								<TBODY>
									<TR>
										<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Server details</b></FONT></P></TD>
									</TR>
									<TR>
										<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">ID</FONT></P></TD>
										<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
											<asp:label id="lblGroupID" runat="Server" />
											</FONT></P>
										</TD>
									</TR>
									<TR>
										<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">IP / Host Header</FONT><br><FONT face="Arial" size=1><em>Edit the config files on the server to <strong>switch between IP and Host Header mode</strong>.</em></FONT></P></TD>
										<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
											<asp:Textbox runat="server" id="txtHostHeader" />
											</FONT><br><FONT face="Arial" size=1><em>The config files are located in the folder /sysdata/ of your webserver.</em>
											</font></P>
										</TD>
									</TR>
									<TR>
										<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Description (for internal use only)</FONT></P></TD>
										<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
											<asp:Textbox runat="server" id="txtDescription" />
											</FONT></P>
										</TD>
									</TR>

									<TR id="trMasterServer" runat="server">
										<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Group</FONT></P></TD>
										<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
											<asp:DropDownList runat="server" id="cmbServerGroup" style="width: 200px" size="1" />
										</FONT></P></TD>
									</TR>

									<TR id="trNotMasterServer" runat="server" >
											<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Group</FONT></P></TD>
											<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><em>This server is a master server and cannot move to another server group. &nbsp;</em></FONT></P>
												</TD>
									</TR>


									<TR id="trEnabledMsg" runat="server">
											<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Enabled</FONT></P></TD>
											<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><em>If you want to disable this server you have to use another administration server than the one you're currently working on. &nbsp;</em></FONT></P>
											<input type="hidden"  name="enabled" value="1"></TD>
									</TR>

									<TR id="trEnableCombo" runat="server"> 
									<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Enabled &nbsp;</FONT></P></TD>
									<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
										<asp:DropDownList runat="server" id="cmbEnabled" style="width: 200px" size="1" />
										</font></p></td>
									</TR>
									<TR>
										<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
									</TR>
									<TR>
										<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Server address</b></FONT></P></TD>
									</TR>
									<TR>
											<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Protocol</FONT></P></TD>
											<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
												<asp:Textbox runat="server" id="txtProtocal" />
											</FONT></P></TD>
									</TR>
									<TR>
											<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Domain name</FONT></P></TD>
											<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
												<asp:Textbox runat="server" id="txtServerName" />
											</FONT></P></TD>
									</TR>
									<TR>
											<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Port number (optional)</FONT></P></TD>
											<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
												<asp:Textbox runat="server" id="txtPortNumber" />
											</FONT></P></TD>
									</TR>
									<TR>
										<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
									</TR>
									<TR>
										<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Script engines</b></FONT></P></TD>
									</TR>
									<TR>
										<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
									</TR>
									
									
									
									<TR>
									<TD VAlign="Top">
									<asp:Repeater Id="rptEngine" runat="server" EnableViewState="True">
										<ItemTemplate>
										<TR>
												<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2>
												<%#DataBinder.Eval(Container.DataItem,"EngineName")%></FONT></P></TD>
												<TD VAlign="Top" Width="240">
												<P><FONT face="Arial" size=2>
												<asp:DropDownList runat="server" Id="cmbEngine"  style="width: 200px" size="1" />
												</FONT></P></TD>
												<TD> <input type="hidden" runat="server" id="EngineId" value='<%#DataBinder.Eval(Container.DataItem,"ScriptEngineID")%>' /></TD>
												</TR>
										</ItemTemplate>
									</asp:Repeater></TD>
									</TR>
									
									
																		

									
									<TR>
										<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Server check</b></FONT></P></TD>
									</TR>
									<TR>
									<TD VAlign="Top" width="400" colspan="2"><FONT face="Arial" size="2"><p>To ensure that the server is running without problems, a test connection to the given URL will be created. If the server is behind a firewall you might not get a connection.<p>If you get a positive response or if you are sure that the server will be accessible, you may update the server settings.</FONT></TD>
									</TR>
									<TR>
										<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><input style="width: 155px" type="text" id="ServerTestURL" value="/sysdata/servercheck.aspx"></FONT></P></TD>
										<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
											<input type="button" name="servercheck" value="Create test connection" onClick="MyCheckURL();">
										</FONT></P></TD>
									</TR>
									<TR>
										<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
									</TR>
					
									<TR>
										<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Server event log</b></FONT></P></TD>
									</TR>
									<TR>
										<TD VAlign="Top" width="400" colspan="2">
											<FONT face="Arial" size="2">
												<p>
													If you encounter any trouble on the server, it might make sense to take a look into the 
													<asp:hyperlink id="hypServerURl" runat="server" />
													 (only for windows servers and only for servers you're currently logged on to).</p><p>Please note:  If the server is behind a firewall you might not get a connection.
												</p>
											</FONT>
										</TD>
									</TR>
									<TR>
										<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
									</TR>
									<TR>
										<TD VAlign="Top" WIDTH="160"><P>
											<asp:Button id="btnSubmit" runat="server" Text="Update server" /></FONT></P></TD>
										<TD VAlign="Top" Width="240"><P> &nbsp;</P></TD>
									</TR>
					</TBODY>
		        </TABLE>

					</TD>
				</TR>
			</TBODY>
		</TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="servers.aspx" id="cammWebManagerAdminMenu" runat="server" /> 
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
