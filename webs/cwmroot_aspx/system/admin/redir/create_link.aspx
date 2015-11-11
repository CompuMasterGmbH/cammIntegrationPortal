<%@ Page Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.Redirector.Pages.Administration.ShowRedirectorLinks" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:webmanager id="cammWebManager" runat="server"></camm:webmanager>
<!--#include virtual="/sysdata/includes/standardtemplate_top_wo_form.aspx"-->
		<form id="Form1" method="post" runat="server">
			<asp:Table id="Table" runat="server" Width="100%" cellpadding="3" cellspacing="0" border="1">
				<asp:TableRow>
					<asp:TableCell Font-Bold="True" Text="Server group"></asp:TableCell>
					<asp:TableCell Font-Bold="True" Text="URL"></asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		</form>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_wo_form.aspx"-->