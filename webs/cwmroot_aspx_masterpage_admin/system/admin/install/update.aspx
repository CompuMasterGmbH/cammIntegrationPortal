<%@ Page Language="VB" Inherits="CompuMaster.camm.WebManager.Setup.Pages.Update" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<html>
<head>
<title><%= cammWebManager.PageTitle %></title>
</head>
<body marginwidth="20" marginheight="20" leftmargin="20" topmargin="20">
    <form runat="server">
        <p>
            <table width="600">
                <tbody>
                    <tr>
                        <td colspan="2">
                            <font face="Verdana"><h2><asp:Label id="Label1" runat="server">camm WebManager Update DB</asp:Label></h2></font>
			</td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <font face="Verdana" size="2"><asp:Label id="LabelTitleAppVersion" runat="server">Current camm Web-Manager application version</asp:Label></font>
                        </td>
                        <td valign="top">
                            <font face="Verdana" size="2"><asp:Label id="LabelShowAppVersion" runat="server"></asp:Label></font>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <font face="Verdana" size="2"><asp:Label id="LabelTitleDBVersion" runat="server">Current camm Web-Manager database version</asp:Label></font>
                        </td>
                        <td valign="top">
                            <font face="Verdana" size="2"></font>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" align="right">
                            <font face="Verdana" size="2"><em>build number reported by patch manager:</em></font>
                        </td>
                        <td valign="top">
                            <font face="Verdana" size="2"><asp:Label visible="true" id="LabelShowDBVersionPatchManager" runat="server"></asp:Label></font>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" align="right">
                            <font face="Verdana" size="2"><em>version reported by camm Web-Manager:</em></font>
                        </td>
                        <td valign="top">
                            <font face="Verdana" size="2"><asp:Label visible="true" id="LabelShowDBVersionWMSystem" runat="server"></asp:Label></font>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" align="right">
                            <font face="Verdana" size="2"><em>Update to database version:</em></font>
                        </td>
                        <td valign="top">
                            <font face="Verdana" size="2"><asp:TextBox visible="true" id="TextboxUpdateToDBVersion" runat="server"></asp:TextBox></font>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" colspan="2">
                            <h3>Reported instances (since last update)</h3>
							<asp:DataGrid runat="server" ID="GridInstanceBuildNos" ItemStyle-BackColor="LightGrey" AlternatingItemStyle-BackColor="White" HeaderStyle-Font-Bold="" BorderWidth="1" BorderColor="Black" AutoGenerateColumns="false">
								<Columns>
									<asp:TemplateColumn HeaderText="Instance location">
										<ItemTemplate>
										 <%#Replace(Container.DataItem("Instance location"), vbCrLf, "<br>")%>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:BoundColumn HeaderText="Assembly Build No" DataField="Assembly Build No" />
									<asp:BoundColumn HeaderText="Application compatible with build no" DataField="Application compatible with build no" />
									<asp:BoundColumn HeaderText="Reported on" DataField="Reported on" />
								</Columns>
							</asp:DataGrid>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" colspan="2">
                            <font color="#FF0000" face="Verdana" size="2"><asp:Label id="LabelUpdateWarning" runat="server">&nbsp;<br>Before you update the database, you should ensure that all servers and all their web applications have been updated with the current assembly files of camm Web-Manager. This is the cammWM.dll file in the /bin folders.<br><br>It is recommend to create a backup of your sql server database, first, before you continue.</asp:Label></font>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <font face="Verdana" size="2"><b><asp:Label id="LabelUpdateNow" runat="server">Do you really want to update, now?</asp:Label></b></font>
                        </td>
                        <td valign="top">
                            <font face="Verdana" size="2"><asp:Button id="btnUpdateDB" onclick="btnUpdateDB_Click" runat="server" Text="Update DB"></asp:Button></font>
                        </td>
                    </tr>
                </tbody>
            </table>
        </p>
    </form>
</body>
</html>
