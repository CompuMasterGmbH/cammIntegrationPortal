<%@ Page Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.SmartWebEditor.Pages.DocsUploadForm"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Register TagPrefix="SmartWebEditor" TagName="ImageBrowser" Src="ImageBrowser.ascx" %>

<camm:WebManager id="cammWebmanager" runat="server" />
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>Document - Management</title>
		<script language="javascript" >

	function checkFileValidity(thisForm)
	{

		for (var i = 0; i < thisForm.length; i++)
		{
			var e = thisForm.elements[i];
			
			if ((e.type == 'file') && (e.name = 'InputFileUploadImage'))
			{
				if ((e.value == null) || (e.value == ""))
				{
					alert('Bitte wählen Sie eine Bilddatei zum hochladen aus.');
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
		<form id="Form1" method="post" runat="server"  >
		
		<div class="pagearea" id="uploadarea">
		<h1>Upload</h1>
		<p>		<asp:Label Runat="server" ID="LabelSelectImageToUpload" Font-Bold="True" /><b>:</b></p>
		<p><input runat="server" type="file" id="InputFileUploadImage" NAME="InputFileUploadImage"/>  </p>
		<p><asp:Label Runat=server ID="LabelUploadedImageNames" /></p>
		<div style="width: 60%">
			<p><asp:Label Runat=server ID="LabelImageUploadFolder" /> <asp:Label id="LabelImageUploadFolderValue" runat="server"></asp:Label></p>
		</div>		
						
		<p style="clear: both;"><asp:Label Runat="server" ID="LabelWarning" ForeColor="#ff0033" /></p>
		
										
		<p><asp:Button Runat="server" ID="ButtonUploadImage" OnClientClick="return checkFileValidity(document.forms[0]);" /></p>
			
		<p><asp:Label Runat="server" ID="LabelProcessingTips" /></p>
		
		</div>
	<hr>		
	
		<SmartWebEditor:ImageBrowser  id="ImageBrowserControl" runat="server" />		
			

		</form>
	</body>
</HTML>
 