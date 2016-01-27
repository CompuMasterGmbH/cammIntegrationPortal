<%@ import Namespace="System.Data.SqlClient" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Page Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Pages.Info" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<camm:webmanager id="cammWebManager" runat="server"></camm:webmanager>
<HTML>
	<HEAD>
		<title>Info</title>
		<link rel="stylesheet" type="text/css" href="/system/admin/logs/styles.css">
			
	</HEAD>
	<body bgcolor="silver">
		<form id="Form1" method="post" runat="server">
			&nbsp;
			<asp:Table id="infoTable" runat="server" CellPadding="3" CellSpacing="3" Width="100%" Height="55px"></asp:Table>
		</form>
	</body>
</HTML>
