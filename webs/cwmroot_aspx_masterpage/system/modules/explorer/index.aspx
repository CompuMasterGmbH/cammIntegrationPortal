<%@ Page Language="vb" Inherits="CompuMaster.camm.WebExplorer.Standard.Pages.Explorer" AutoEventWireup="True" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Web.UI.Webcontrols" %>
<%@ Import Namespace="System.Drawing.Color" %>
<%@ Import Namespace="System.Text" %>
<%@ Import Namespace="Microsoft.VisualBasic" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<script language="vb" runat="server">

Sub Initialize (ByVal obj As Object, ByVal e As EventArgs) Handles MyBase.Init

	mybase.cammWebmanager = cammwebmanager

End Sub

</script>

<html>
<head>
<!--#include virtual="/sysdata/includes/metalink.aspx"-->
<link rel="stylesheet" type="text/css" href="/system/modules/explorer/explorer.css" />
</head>
<body >
<form runat="server" enctype="multipart/form-data">
	<asp:Table Width="100%" height="100%" runat="server" borderwidth="1" cellspacing="0" cellpadding="0" >
	<asp:TableRow runat="server">
		<asp:TableCell height="100%" valign="top" width="100%">
			<asp:Table id="TableBody" runat="server" cellspacing=0 Cellpadding=0 Width="100%" Class="cssTableBody" />
		</asp:TableCell>
	</asp:TableRow>
	</asp:Table>
</form>
</body></html>
