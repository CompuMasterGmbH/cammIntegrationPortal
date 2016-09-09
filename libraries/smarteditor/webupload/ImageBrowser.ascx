<%@ import Namespace="System.Drawing" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ Control Language="VB" Inherits=" CompuMaster.camm.SmartWebEditor.Pages.ImageBrowser" %>
        <script>
		
		
		imagesUploadPath = '<%= Me.ImageUploadFolderPath %>';

		function choseImage(name, path)
		{
			document.getElementById('imagePath').innerText = path;
		}
		
		function confirmDeletion()
		{
			var selectedImage = document.getElementById('listBoxUploadedFiles').value;
			return confirm('Are you sure you want to delete the following image: ' + selectedImage + '?')
		}
		
		function getPathOfSelectedImage()
		{
		    var selectedImage = document.getElementById('listBoxUploadedFiles').value;
		    if(selectedImage == '')
		    {
		        return '';
		    }
		    var path = imagesUploadPath;
		    if (path.substr(path.length - 1) != '/') {
		        path += '/';
		    }

		    return path + selectedImage;
		}
		function passPathToEditor()
		{
		    var imagePath = getPathOfSelectedImage();
		    if (imagePath == '')
		    {
		        alert('Please select an image');
		        return;
		    }
		    if (window.opener.pasteImageToEditor)
		    {
		        window.opener.pasteImageToEditor("<%= Me.EditorId %>", imagePath);
		    }
		
			
		}
		
		</script>
		<div id="imagebrowser">
		<b><asp:Literal runat="server" ID="ltrlUploadedFiles" Text="Uploaded files"/></b><br>
		<asp:ListBox runat="server" id="listBoxUploadedFiles" ClientIdMode="Static" style="width: 600px;" Rows=10  />
	
		<p><b>Image Path:</b> <input type="text" id="imagePath" style="width: 300px;" readonly></p>
		<p><asp:Label runat="server" id="lblDeletionMessage" EnableViewState="false"></asp:Label></p>

		<p><asp:Button runat="server" ID="btnPasteToEditor" Text="Insert image to editor" OnClientClick="passPathToEditor(); return false;" /> <asp:Button runat="server" id="btnDeleteImage" onClientClick="return confirmDeletion();" Text="Delete image"></asp:Button></p>
		</div>
