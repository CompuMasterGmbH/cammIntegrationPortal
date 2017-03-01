<%@ Page Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.SmartWebEditor.Pages.DocumentsUploadForm"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Register TagPrefix="SmartWebEditor" TagName="FileBrowser" Src="DocumentsBrowser.ascx" %>

<camm:WebManager id="cammWebmanager" runat="server" />
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>Documents - Management</title>
		<script language="javascript" >
	function checkFileValidity(thisForm)
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
							alert('<%=Me.OnlyFollowingExtensionsAreAllowed%> <%=String.Join(", ", Me.UploadParamters.AllowedFileExtensions)%>');
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
		<p>		<asp:Label Runat="server" ID="LabelSelectFileToUpload" Font-Bold="True" /><b>:</b></p>
		<p><input runat="server" type="file" id="InputFileUpload" name="InputFileUpload"/>  </p>
		<p><asp:Label Runat=server ID="LabelUploadedImageNames" /></p>
		<div style="width: 60%">
			<p><asp:Label Runat=server ID="LabelFileUploadFolder" /> <asp:Label id="LabelFileUploadFolderValue" runat="server"></asp:Label></p>
		</div>		
		
	
				
		<p style="clear: both;"><asp:Label Runat="server" ID="LabelWarning" ForeColor="#ff0033" /></p>
		
										
		<p><asp:Button Runat="server" ID="ButtonUploadFile" OnClientClick="return checkFileValidity(document.forms[0]);" /></p>
			
		<p><asp:Label Runat="server" ID="LabelProcessingTips" /></p>
		
		</div>

		<hr>		
	
		<h1><asp:Literal runat="server" ID="ltrlInsertSectionHeadline" /></h1>
        <SmartWebEditor:FileBrowser  id="FileBrowserControl" runat="server" />		
			

		</form>
	</body>
</HTML>
 