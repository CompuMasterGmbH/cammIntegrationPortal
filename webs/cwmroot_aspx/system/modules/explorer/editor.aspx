<%@ Page Language="vb" validateRequest=false Inherits="CompuMaster.camm.WebExplorer.Standard.Pages.Editor" %>
<%@ Import Namespace="System.Web.UI.Webcontrols" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<script language="vb" runat="server">

Sub Page_Init(obj as Object, e as eventargs)
        mybase.cammWebManager = cammwebmanager
        mybase.tboxedit = tboxedit
        mybase.btnsave = btnsave
        mybase.btncancel = btncancel
        mybase.btnShowLineNo = btnShowLineNo
        mybase.TableTitleMain = TableTitleMain
        mybase.txtViewTable = txtViewTable
End Sub

</script>

<html>
<head>
<link rel="stylesheet" type="text/css" href="/system/modules/explorer/explorer.css" />
</head>
<body>
<form runat="server" enctype="multipart/form-data" ID="Form1">

	<asp:Table id="TableTitleMain" Cellspacing=3 Width="100%" runat="server" background="/system/modules/explorer/images/background_titlebar.jpg" Class="TitleBlue">
	</asp:Table>
	<asp:Table id="TableMenu" background="/system/modules/explorer/images/background_symbolbar.jpg" runat="server" height="25" Cellspacing=0 cellpadding=0 Width="100%" Class="TitleBlue" Style="Font: 14px">
		<asp:tablerow>
			<asp:TableCell background="/system/modules/explorer/images/background_symbolline.gif" width="10" height="25" >
				<asp:Image runat="server" ImageUrl="/system/modules/explorer/images/splitter_start_symbols.gif" height="25" width="10" BorderStyle="None" EnableViewState="False"></asp:Image>
			</asp:TableCell>
			<asp:TableCell background="/system/modules/explorer/images/background_symbolline.gif" height="25"  >
				<asp:ImageButton id="btnSave" ImageUrl="/system/modules/explorer/images/save_ico.gif" runat="server" onclick="SaveChanges" />
			</asp:TableCell>
			<asp:TableCell background="/system/modules/explorer/images/background_symbolline.gif" height="25px"  >
				<asp:ImageButton id="btnShowLineNo" ImageUrl="/system/modules/explorer/images/showFileWithLineNo.gif" height="20" width="20" background="/system/modules/explorer/images/background_symbolline.gif"  runat="server" onclick="ShowLineNos" />
			</asp:TableCell>
			<asp:TableCell height="25" width="10" background="/system/modules/explorer/images/background_symbolline.gif">
				<asp:Image runat="server" ImageUrl="/system/modules/explorer/images/splitter.gif" height="25" width="10" BorderStyle="None" EnableViewState="False"></asp:Image>
			</asp:TableCell>


			<asp:TableCell background="/system/modules/explorer/images/background_symbolline.gif" height="25"  >
				<asp:ImageButton id="btnCancel" ImageUrl="/system/modules/explorer/images/exit_icon.gif" runat="server"  onclick="CancelChanges" />
			</asp:TableCell>
			<asp:TableCell height="25" width="5" >
				<asp:Image runat="server" ImageUrl="/system/modules/explorer/images/splitter_end_symbols.gif" height="25" width="5" BorderStyle="None" EnableViewState="False"></asp:Image>
			</asp:TableCell>

			<asp:TableCell   width="100%">
				<asp:Image runat="server" ImageUrl="/system/modules/explorer/images/space.gif" height="25" BorderStyle="None" EnableViewState="False"></asp:Image>
			</asp:TableCell>
		</asp:tablerow>
	</asp:Table>
	<asp:Table id="TableBody" runat="server" Cellspacing=0 cellpadding=3 Width="100%" Class="TitleBlue" />
	
	
	<asp:TextBox id="tboxedit" TextMode="multiline" Columns="60" Rows="33" runat="server" BorderStyle="None" style="width: 100%" Class="cssTableBody"/>
	
	
	<asp:Table id="txtViewTable" Cellspacing=3  runat="server" Class="cssTableBody">
	</asp:Table>

	
	
</form>
</body></html>