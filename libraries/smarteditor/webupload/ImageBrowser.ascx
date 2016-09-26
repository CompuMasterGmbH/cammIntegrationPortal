<%@ import Namespace="System.Drawing" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ Control Language="VB" Inherits=" CompuMaster.camm.SmartWebEditor.Pages.FileBrowser" %>
        <script>
	    function confirmDeletion()
		{
			var selectedFile = document.getElementById('listBoxUploadedFiles').value;
			return confirm('Are you sure you want to delete the following file: ' + selectedFile + '?')
		}
		
	    function passPathToEditor()
		{
	        var filePath = document.getElementById('<%=txtBoxFilePath.ClientID%>').value;
	        if (filePath == '')
		    {
		        alert('Please select a file');
		        return false;
	        }
            var altText = document.getElementById('<%=txtBoxAltText.ClientID%>').value;

            if (window.opener.<%=ParentWindowCallbackFunction%>)
		    {
		        window.opener.<%=ParentWindowCallbackFunction%>("<%= Me.EditorId %>", filePath, altText);
            }
	        return true;
		}
		
         function listBoxOnChange(value)
         {
             document.getElementById('<%= Me.txtBoxFilePath.ClientID%>').value = value;
                var button = document.getElementById('<%=Me.btnDeleteFile.ClientId%>');
                button.disabled = value.indexOf('<%=Me.UploadFolderPath%>') !== 0;
         }
		</script>
		<div id="imagebrowser">
		<b><asp:Literal runat="server" ID="ltrlUploadedFiles" Text="Uploaded images"/></b><br>
		<asp:ListBox runat="server" id="listBoxUploadedFiles" ClientIdMode="Static" style="width: 600px;" Rows=10  />
	
		<p><b>Image Path:</b> <asp:TextBox runat="server" ID="txtBoxFilePath" width="400px" ReadOnly="true" /></p>
           
        <p><b>Alt Text:</b> <input type="text" runat="server" id="txtBoxAltText" /></p>

		<p><asp:Label runat="server" id="lblDeletionMessage" EnableViewState="false"></asp:Label></p>

		<p><asp:Button runat="server" ID="btnPasteToEditor" Text="Insert to editor"  /> <asp:Button runat="server" id="btnDeleteFile" onClientClick="return confirmDeletion();" Text="Delete"></asp:Button></p>
		</div>
