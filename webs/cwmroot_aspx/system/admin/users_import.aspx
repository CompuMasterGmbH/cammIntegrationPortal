<%@ Page Inherits="CompuMaster.camm.WebManager.Pages.Administration.ImportUsers" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" PageTitle="Administration - User accounts" SecurityObject="System - User Administration - Users" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_wo_form.aspx"-->
<form runat="server" id="ImportWizardForm">
	<h3><font face="Arial">Administration - User accounts - Import</font></h3>
 	<TABLE cellSpacing=0 cellPadding=0 bgColor=#ffffff border=0 bordercolor="#C1C1C1">
	  <TBODY>
	  <TR>
        <TD vAlign=top>
		<asp:Panel runat="server" id="PanelStep1">
			<h4>Step 1 of 3 - Upload</h4>
			<h5>Please select a CSV file which contains the list of users</h5>
			<p><input id="CsvFile" type="file" runat="server"></p>
			<p>
			<asp:Label runat="server" id="LabelStep1Errors" forecolor="#FF0000" />
			<asp:Button runat="server" id="ButtonStep1Submit" text="Next" /> 
			</p>
		</asp:Panel>
		<asp:Panel runat="server" id="PanelStep2">
			<h4>Step 2 of 3 - Charset</h4>
			<asp:Label runat="server" id="LabelStep2Errors" forecolor="#FF0000" />

			<h5>Please choose the appropriate charset of the uploaded file</h5>
			<p>Charset: <asp:Textbox runat="server" id="TextboxStep2Charset" text="UTF-8" /> (e. g. &quot;UTF-8&quot;, &quot;ISO-8859-1&quot;, &quot;WINDOWS-1252&quot;)</p>

			<h5>Please choose the appropriate import culture settings</h5>
			<p>Column separator: <asp:Textbox runat="server" id="TextboxStep2ColumnSeparator" text="," maxlength="0" /> (e. g. &quot;,&quot; or &quot;;&quot; or &quot;TAB&quot;)</p>
			<p>Text identifier: <asp:Textbox runat="server" id="TextboxStep2TextIdentifier" text="&quot;" maxlength="0" /> (e. g. &quot; or ' )</p>

			<p><asp:Button runat="server" id="ButtonStep2PreviewData" text="Preview" /></p>
			<asp:DataGrid runat="server" id="DatagridStep2DataPreview" />
			<p>
			<asp:Button runat="server" id="ButtonStep2Submit" text="Next" /> 
			<asp:Button runat="server" id="ButtonStep2PreviousStep" text="Previous step" /> 
			</p>
		</asp:Panel>
		<asp:Panel runat="server" id="PanelStep3">
			<h4>Step 3 of 3 - Verify the list of columns and select an action</h4>
			<asp:Label runat="server" id="LabelStep3Errors" forecolor="#FF0000" />
			<p>
				Culture of import data: 
				<asp:Textbox runat="server" id="TextboxStep3Culture" text="" />
				(e. g. invariant, en-GB, de-DE; if you've got some date values in ISO style and numbers with a dot as decimal separator, you might consider to use "invariant" as your culture)
			</p>
			<h5>Please choose the desired action type for the user accounts:</h5>
			<ul>
				<asp:RadioButton runat="server" id="RadioStep3ActionInsertUpdate" groupname="Action" Text="Insert+Update" autopostback="true" /><br>
				<asp:RadioButton runat="server" id="RadioStep3ActionInsertOnly" groupname="Action" Text="Insert only" autopostback="true" /><br>
				<asp:RadioButton runat="server" id="RadioStep3ActionUpdateOnly" groupname="Action" Text="Update only" autopostback="true" /><br>
				<asp:RadioButton runat="server" id="RadioStep3ActionRemoveOnly" groupname="Action" Text="Remove only" autopostback="true" /><br>
			</ul>
			<asp:Checkbox runat="server" id="CheckboxStep3SuppressUserNotificationMails" text="Suppress all notification mails to users" checked="true" AutoPostBack="true" /><br>
			<asp:Checkbox runat="server" id="CheckboxStep3SuppressAdminNotificationMails" text="Suppress all notification mails to security administration team" checked="true" AutoPostBack="true" /><br>
			<asp:Panel runat="server" id="PanelStep3MembershipsImportType">
			<h5>Please choose the desired action type for the users' memberships:</h5>
			<ul>
				<asp:RadioButton runat="server" id="RadioStep3ActionMembershipsFitExact" groupname="ActionGroups" Text="Setup memberships exactly as defined (remove undefined ones)" /><br>
				<asp:RadioButton runat="server" id="RadioStep3ActionMembershipsInsertOnly" checked="true" groupname="ActionGroups" Text="Add memberships only" /><br>
			</ul>
			</asp:Panel>
			<asp:Panel runat="server" id="PanelStep3AuthorizationsImportType">
			<h5>Please choose the desired action type for the users' authorizations:</h5>
			<ul>
				<asp:RadioButton runat="server" id="RadioStep3ActionAuthorizationsFitExact" groupname="ActionAuthorizations" Text="Setup authorizations exactly as defined (remove undefined ones)" /><br>
				<asp:RadioButton runat="server" id="RadioStep3ActionAuthorizationsInsertOnly" checked="true" groupname="ActionAuthorizations" Text="Add authorizations only" /><br>
			</ul>
			</asp:Panel>
			<asp:Panel runat="server" id="PanelStep3AdditionalFlagsImportType">
			<h5>Please note: the action type for the users' additional flags is always:</h5>
			<ul>
				<asp:RadioButton runat="server" id="RadioStep3ActionAdditionalFlagsDefinedKeysOnly" checked="true" groupname="ActionAdditionalFlags" Text="Setup declared additional flags only (add/update/remove defined keys only)" /><br>
			</ul>
			</asp:Panel>
			<h5>Imported fields summary:</h5>
			<asp:DataGrid runat="server" id="DatagridStep3ColumnsCheck" />
			<p><em>Please note: Update actions will never update a user's password.</em></p>
			<p>
			<asp:Button runat="server" id="ButtonStep3Submit" text="Next" /> 
			<asp:Button runat="server" id="ButtonStep3PreviousStep" text="Previous step" /> 
			</p>
		</asp:Panel>
		<asp:Panel runat="server" id="PanelStep4">
			<h4>Processing...</h4>
			<asp:Label runat="server" id="LabelStep4ProcessingStatus" />
			<asp:Label runat="server" id="LabelStep4Errors" name="ErrorLog" forecolor="#FF0000" />
			<iframe runat="server" id="IFrameStep4ProcessingWindow" border="0" frameborder="0" width="400" height="200" src="users_importprocessing.aspx"></iframe>
			<p>
			<asp:Button runat="server" id="ButtonStep4Submit" text="Next" /> 
			<asp:Button runat="server" id="ButtonStep4PreviousStep" text="Previous step" /> 
			</p>
		</asp:Panel>
		<asp:Panel runat="server" id="PanelStep5">
			<h4>Finish</h4>
			<h5>The import has been processed. Please view the log carefully for any errors.</h5>
			<ul><asp:Label runat="server" id="LabelStep5Log" /></ul>
			<p>
			<asp:Button runat="server" id="ButtonStep5Submit" text="Start new import" /> 
			</p>
		</asp:Panel>

		</TD></TR>
      </TBODY></TABLE>
</form>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu id="cammWebManagerAdminMenu" runat="server"></camm:WebManagerAdminMenu>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_wo_form.aspx"-->