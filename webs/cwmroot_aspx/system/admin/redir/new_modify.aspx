<%@ Page Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.Redirector.Pages.Administration.EditRedirection" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:webmanager PageTitle="Redirections - Modify Item" id="cammWebManager" runat="server"></camm:webmanager>
<!--#include virtual="/sysdata/includes/standardtemplate_top_wo_form.aspx"-->
		<asp:panel runat="server" id="ActivateJScript">
		<SCRIPT language="JavaScript">
			//close popup window and refresh main window
			function cl_window(){
				window.close();
				window.opener.document.forms[0].submit();
			}
			
			cl_window()

		</SCRIPT>
		</asp:panel>
		<form method="post" runat="server">
			<h3><font face="Arial"><asp:label id="LabelTitle" runat="server" /></font></h3>
			<P align="center">
				<TABLE id="Table1" cellSpacing="0" cellPadding="2" width="100%" border="1">
					<TR runat="server" id="RowID">
						<TD>ID</TD>
						<TD><asp:label id="LabelID" runat="server"></asp:label></TD>
					</TR>
					<TR>
						<TD>Description</TD>
						<TD><asp:textbox id="TextDesc" runat="server" Width="238px"></asp:textbox></TD>
					</TR>
					<TR>
						<TD>Redirect To</TD>
						<TD><asp:textbox id="TextRT" runat="server" Width="239px"></asp:textbox></TD>
					</TR>
					<TR>
						<TD>Number of redirections</TD>
						<TD><asp:textbox id="TextNR" runat="server" Width="68px"></asp:textbox></TD>
					</TR>
				</TABLE>
			</P>
			<asp:button id="button_send" OnClick="button_send_Click" runat="server" Text="Send" /> <asp:button id="button_reset" onclick="button_reset_Click" runat="server" Text="Reset" />
		</form>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_wo_form.aspx"-->