<%@ Page Language="vb" Inherits="CompuMaster.camm.WebExplorer.Standard.Pages.DownloadFile" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<script language="vb" runat="server">

Sub Initialize (ByVal obj As Object, ByVal e As EventArgs) Handles MyBase.Init

	mybase.cammWebmanager = cammwebmanager

End Sub

</script>
