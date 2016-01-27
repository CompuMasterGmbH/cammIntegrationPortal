<%@ Page Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Modules.WebEdit.Controls.UploadForm"%>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebmanager" runat="server" />
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>Bilddaten - Management</title>
		<script language="javascript" >
	function checkImageValidity(thisForm)
	{

		for (var i = 0; i < thisForm.length; i++)
		{
			var e = thisForm.elements[i];
			
			if ((e.type == 'file') && (e.name = 'InputFileUploadImage'))
			{
				if ((e.value == null) || (e.value == ""))
				{
					alert('Bitte wählen Sie ein Bilddatei zum hochladen.');
					e.focus();
					return false;		
				}
				else {
					var ext = e.value;
					ext = ext.substr((ext.length - 4), 4).toLowerCase();
					if ( (ext == '.jpg') || (ext == '.gif') || (ext == '.png') || (ext == '.bmp') || (ext == 'jpeg'))
					{
						return true;
					}
					else {
						alert('Es können nur folgende Datenformate auf den Server geladen werden: JPEGs, GIFs, PNGs und BMPs.');
						e.value = "";
						thisForm.reset();
						e.focus();
						return false;		
					}
				}
			}
		}
		return true;
	}
		
		</script>
		<link href="/Styles.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server" onsubmit="return checkImageValidity(this)" >
<table cellSpacing="2" cellPadding="0" border="0">
	<tr>
		<td><COMPONENTART:TABSTRIP id="TabStrip" runat="server" MultiPageId="MultiPage">
				<Tabs>
					<componentart:TabStripTab Text="Upload" ID="TabStripUpload" PageViewId="PageViewUpload" />
					<componentart:TabStripTab Text="Optionen" ID="TabStripOptions" PageViewId="PageViewOptions" />
				</Tabs>
			</COMPONENTART:TABSTRIP></td>
	</tr>
	<tr>
		<td><COMPONENTART:MULTIPAGE id="MultiPage" runat="server" width="740" CssClass="MultiPage">
				<ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageViewUpload">
						<table border="0" cellpadding="5" cellspacing="0" width="100%">
							<tr>
								<td colspan="3">
									<asp:Label Runat="server" ID="LabelSelectImageToUpload" Font-Bold="True" /><b>:</b></td>
							</tr>
							<tr>
								<td colspan="3"><input runat="server" type="file" id="InputFileUploadImage" NAME="InputFileUploadImage"/>&nbsp;
									<asp:Button Runat="server" ID="ButtonUploadImage" /></td>
							</tr>
							<tr>
								<td colspan="3"><asp:Label Runat=server ID="LabelUploadedImageNames" /></td>
							</tr>
							
							<tr>
								<td>
									<asp:CheckBox Runat="server" ID="CheckBoxImageReduction" /></asp:CheckBox></td>
								<td align="right"><asp:Label Runat=server ID="LabelImageUploadFolder" /></td>
								<td align="left">
									<asp:label id="LabelImageUploadFolderValue" runat="server"></asp:label>
								</td>
							</tr>
							<tr>
								<td colspan="3">
									<asp:Label Runat="server" ID="LabelWarning" ForeColor="#ff0033" /></td>
							</tr>
							<tr>
								<td colspan="3"></td>
							</tr>
							<tr>
								<td height="20" colspan="3"></td>
							</tr>
							<tr>
								<td colspan="3">
									<asp:Label Runat="server" ID="LabelProcessingTips" /></td>
							</tr>
						</table>
				</ComponentArt:PageView>
				<ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageViewOptions">
			<asp:Table Runat="server" BorderStyle="None" BorderWidth="0" CellPadding="0" CellSpacing="0"
				Width="100%" id="TableOptions">
				<asp:TableRow Runat="server" ID="AreaGeneralAdjustments">
					<asp:TableCell Runat="server" ID="Tablecell1" NAME="Tablecell1">
						<table border="0" cellpadding="5" cellspacing="0" width="100%">
							<tr>
								<td>
									<asp:Label Runat="server" ID="LabelImageDataGeneralProcessing" Font-Bold="True" /></td>
							</tr>
							<tr>
								<td>
									<asp:Image Runat="server" ID="ImageSampleFile" /></td>
							</tr>
							<tr>
								<td>
									<asp:Label Runat="server" ID="LabelImageDimensionQuestion" /></td>
							</tr>
							<tr>
								<td>
									<table border="0" cellpadding="0" cellspacing="0" width="100%">
										<tr valign="bottom">
											<td width="150"></td>
											<td width="150">
												<asp:Label Runat="server" ID="LabelMiniatureView" Font-Bold="True" />&nbsp;
												<asp:CheckBox Runat="server" ID="CheckBoxMiniatureView" Checked="True" /></td>
											<td width="50"></td>
											<td>
												<asp:Label Runat="server" ID="LabelNormalView" Font-Bold="True" />&nbsp;
												<asp:CheckBox Runat="server" ID="CheckBoxNormalView" Checked="True" /></td>
										</tr>
										<tr valign="bottom">
											<td width="150"><img src="/system/modules/smartwcms/images/maxwidth.gif" width="20" height="20">&nbsp;
												<asp:Label Runat="server" ID="LabelMaxWidth" /></td>
											<td>
												<asp:TextBox Runat="server" ID="TextBoxMiniatureMaxWidth" Columns="9" />&nbsp;Pixel</td>
											<td width="50"></td>
											<td>
												<asp:TextBox Runat="server" ID="TextBoxNormalMaxWidth" Columns="9" />&nbsp;Pixel</td>
										</tr>
										<tr valign="bottom">
											<td><img src="/system/modules/smartwcms/images/maxheight.gif" width="20" height="20">&nbsp;
												<asp:Label Runat="server" ID="LabelMaxHeight" /></td>
											<td>
												<asp:TextBox Runat="server" ID="TextBoxMiniatureMaxHeight" Columns="9" />&nbsp;Pixel</td>
											<td width="50"></td>
											<td>
												<asp:TextBox Runat="server" ID="TextBoxNormalMaxHeight" Columns="9" />&nbsp;Pixel</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</asp:TableCell>
				</asp:TableRow>
				<asp:TableRow Runat="server" ID="AreaFrameType" Visible="False">
					<asp:TableCell Runat="server" ID="Tablecell2" NAME="Tablecell2">
						<table border="0" cellpadding="5" cellspacing="0" width="100%">
							<tr>
								<td colspan="3">
									<asp:Label Runat="server" ID="LabelImageBorderType" Font-Bold="True" /></td>
							</tr>
							<tr>
								<td></td>
								<td></td>
								<td></td>
							</tr>
							<tr>
								<td></td>
								<td></td>
								<td></td>
							</tr>
							<tr>
								<td></td>
								<td></td>
								<td></td>
							</tr>
							<tr>
								<td></td>
								<td></td>
								<td></td>
							</tr>
						</table>
					</asp:TableCell>
				</asp:TableRow>
			</asp:Table>
				</ComponentArt:PageView>
			</COMPONENTART:MULTIPAGE></td>
	</tr>
</table>
		</form>
	</body>
</HTML>
