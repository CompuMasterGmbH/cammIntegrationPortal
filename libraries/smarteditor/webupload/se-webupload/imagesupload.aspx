<%@ Page Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.SmartWebEditor.Pages.ImagesUploadForm"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Register TagPrefix="SmartWebEditor" TagName="FileBrowser" Src="ImageBrowser.ascx" %>

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
			
			if ((e.type == 'file') && (e.name == 'InputFileUpload'))
			{
				if ((e.value == null) || (e.value == ""))
				{
					alert('<%=Me.NoImageChosenJavascriptMessageText%>');
					e.focus();
					return false;		
				}
				else {
					var ext = '.' + e.value.split('.').pop();
			        var allowedExtensions = ["<%=String.Join(""",""", Me.UploadParamters.AllowedFileExtensions)%>"];
					if ( allowedExtensions.indexOf(ext) > -1)
					{
						return true;
					}
					else {
						alert('<%=Me.OnlyFollowingExtensionsAreAllowed%> <%=String.Join(""",""", Me.UploadParamters.AllowedFileExtensions)%>');
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
			
			
		function setUploadButtonState()
		{
			var oneCheckBoxChecked = document.getElementById('<%=Me.CheckBoxMiniatureView.ClientId%>').checked || document.getElementById('<%=Me.CheckBoxNormalView.ClientId%>').checked || document.getElementById('<%=Me.CheckBoxOriginalView.ClientId%>').checked; 
			document.getElementById('<%=Me.ButtonUploadFile.ClientId%>').disabled = !oneCheckBoxChecked; 

		}
		</script>
		<link href="/Styles.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server"  >
		
		<div class="pagearea" id="uploadarea">
		<h1>Upload</h1>
		<p>		<asp:Label Runat="server" ID="LabelSelectFileToUpload" Font-Bold="True" /><b>:</b></p>
		<p><input runat="server" type="file" id="InputFileUpload" name="InputFileUpload"/>  </p>
		<p><asp:Label Runat=server ID="LabelUploadedImageNames" /></p>
		<div style="width: 60%">
			<p><asp:Label Runat=server ID="LabelFileUploadFolder" /> <asp:Label id="LabelFileUploadFolderValue" runat="server"></asp:Label></p>
		</div>		
		<p><asp:Label Runat="server" ID="LabelImageDimensionQuestion" /></p>
	
			<table border="0" cellpadding="0" cellspacing="0" width="100%">
										<tr valign="bottom">
											<td width="150"></td>
											<td width="150">
												<asp:Label Runat="server" ID="LabelMiniatureView" Font-Bold="True" />&nbsp;
												<asp:CheckBox Runat="server" ID="CheckBoxMiniatureView" Checked="True" onclick="setUploadButtonState()" /></td>
											<td width="50"></td>
											<td width="150">
												<asp:Label Runat="server" ID="LabelNormalView" Font-Bold="True" />&nbsp;
												<asp:CheckBox Runat="server" ID="CheckBoxNormalView" Checked="True" onclick="setUploadButtonState()" /></td>
											<td width="50"></td>
											<td>
												<asp:Label Runat="server" ID="LabelOriginalView" Font-Bold="True" Text="Original" />&nbsp;
												<asp:CheckBox Runat="server" ID="CheckBoxOriginalView" Checked="True" onclick="setUploadButtonState()" /></td>
										</tr>
										<tr valign="bottom">
											<td width="150"><img src="/system/modules/smartwcms/images/maxwidth.gif" width="20" height="20">&nbsp;
												<asp:Label Runat="server" ID="LabelMaxWidth" /></td>
											<td>
												<asp:TextBox Runat="server" ID="TextBoxMiniatureMaxWidth" Columns="9" />&nbsp;Pixel</td>
											<td width="50"></td>
											<td width="150">
												<asp:TextBox Runat="server" ID="TextBoxNormalMaxWidth" Columns="9" />&nbsp;Pixel</td>
											<td width="50"></td>
											<td>&nbsp;</td>
										</tr>
										<tr valign="bottom">
											<td><img src="/system/modules/smartwcms/images/maxheight.gif" width="20" height="20">&nbsp;
												<asp:Label Runat="server" ID="LabelMaxHeight" /></td>
											<td>
												<asp:TextBox Runat="server" ID="TextBoxMiniatureMaxHeight" Columns="9" />&nbsp;Pixel</td>
											<td width="50"></td>
											<td width="150">
												<asp:TextBox Runat="server" ID="TextBoxNormalMaxHeight" Columns="9" />&nbsp;Pixel</td>
											<td width="50"></td>
											<td>&nbsp;</td>
										</tr>
									</table>
				
		<p style="clear: both;"><asp:Label Runat="server" ID="LabelWarning" ForeColor="#ff0033" /></p>
		
										
		<p><asp:Button Runat="server" ID="ButtonUploadFile" OnClientClick="return checkImageValidity(document.forms[0]);" /></p>
			
		<p><asp:Label Runat="server" ID="LabelProcessingTips" /></p>
		
		</div>

		<hr>		
	
		<h1><asp:Literal runat="server" ID="ltrlInsertSectionHeadline" /></h1>
		<SmartWebEditor:FileBrowser  id="FileBrowserControl" runat="server" />		
			

		</form>
	</body>
</HTML>
 