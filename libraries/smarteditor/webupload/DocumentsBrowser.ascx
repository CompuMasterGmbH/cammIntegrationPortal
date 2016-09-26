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
            var descriptionText = document.getElementById('<%=txtBoxDescription.ClientID%>').value;
	        var target = document.getElementById('<%= dropDownTarget.ClientID%>').value;
            if (window.opener.<%=ParentWindowCallbackFunction%>)
            {
                var linktext = document.getElementById('<%=Me.txtBoxLinkText.ClientID%>').value;
                if(linktext === "" )
                    linktext = filePath.split('/').pop();
                window.opener.<%=ParentWindowCallbackFunction%>("<%= Me.EditorId %>", filePath, descriptionText, target, linktext);
            }
	        return true;
		}
		
         function listBoxOnChange(value)
         {
             document.getElementById('<%=Me.txtBoxFilePath.ClientID%>').value = value;
                var button = document.getElementById('<%=Me.btnDeleteFile.ClientId%>');
                button.disabled = value.indexOf('<%=Me.UploadFolderPath%>') !== 0;
         }
		</script>
		<div id="imagebrowser">
		<b><asp:Literal runat="server" ID="ltrlUploadedFiles" Text="Uploaded documents"/></b><br>
		<asp:ListBox runat="server" id="listBoxUploadedFiles" ClientIdMode="Static" style="width: 600px;" Rows=10  />
	
		<p><b>Document Path:</b> <asp:TextBox runat="server" ID="txtBoxFilePath" width="400px" ReadOnly="true" /></p>
           
        <p><b>Description Text:</b> <input type="text" runat="server" id="txtBoxDescription" /></p>

        <p><b>Link text:</b> <input type="text" runat="server" id="txtBoxLinkText" /></p>

        <p><b>Target</b> <asp:DropDownList runat="server" ID ="dropDownTarget">
            <asp:ListItem>_blank</asp:ListItem>
            <asp:ListItem>_parent</asp:ListItem>
            <asp:ListItem>_self</asp:ListItem>
            <asp:ListItem>_top</asp:ListItem>

                         </asp:DropDownList>

        </p>
		<p><asp:Label runat="server" id="lblDeletionMessage" EnableViewState="false"></asp:Label></p>

		<p><asp:Button runat="server" ID="btnPasteToEditor" Text="Insert to editor"  /> <asp:Button runat="server" id="btnDeleteFile" onClientClick="return confirmDeletion();" Text="Delete"></asp:Button></p>
		</div>
