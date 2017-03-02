<%@ import Namespace="System.Drawing" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ Control Language="VB" Inherits=" CompuMaster.camm.SmartWebEditor.Pages.ImageBrowser" %>
        <script>
	    function confirmDeletion()
		{
			var selectedFile = document.getElementById('listBoxUploadedFiles').value;
			return confirm('<%=Me.ConfirmDeletionText%> ' + selectedFile + '?')
		}
		
	    function passPathToEditor()
		{
	        var filePath = document.getElementById('<%=txtBoxFilePath.ClientID%>').value;
	        if (filePath == '')
		    {
		        alert('<%=Me.PleaseSelectAFile%>');
		        return false;
	        }
            var altText = document.getElementById('<%=txtBoxAltText.ClientID%>').value;

            if (window.opener.<%=ParentWindowCallbackFunction%>)
		    {
                window.opener.<%=ParentWindowCallbackFunction%>("<%= Me.EditorId %>", encodeURI(filePath), altText);
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
		<b><asp:Literal runat="server" ID="ltrlUploadedFiles"/></b><br>
		<asp:ListBox runat="server" id="listBoxUploadedFiles" ClientIdMode="Static" style="width: 600px;" Rows=10  />
	
		<p><b><asp:Literal runat="server" ID="ltrlImagePath"></asp:Literal></b> <asp:TextBox runat="server" ID="txtBoxFilePath" width="400px" ReadOnly="true" /></p>
           
        <p><b><asp:Literal runat="server" ID="ltrlAltText"></asp:Literal></b> <input type="text" runat="server" id="txtBoxAltText" /></p>

		<p><asp:Label runat="server" id="lblDeletionMessage" EnableViewState="false"></asp:Label></p>

		<p><asp:Button runat="server" ID="btnPasteToEditor"  /> <asp:Button runat="server" id="btnDeleteFile" onClientClick="return confirmDeletion();"></asp:Button></p>
		</div>
