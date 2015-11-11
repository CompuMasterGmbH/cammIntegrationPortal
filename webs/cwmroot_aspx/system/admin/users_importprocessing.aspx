<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.ImportUsersProcessing" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" PageTitle="Administration - User accounts" SecurityObject="System - User Administration - Users" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_wo_form.aspx"-->
<script language="javascript" type="text/javascript">
<!--
<%
If Me.ProgressState = Me.TotalRecords Then
%>
	function __doPostBackParentWindows(eventTarget, eventArgument) 
	{
		var theform;
		if (window.navigator.appName.toLowerCase().indexOf("microsoft") > -1) {
			theform = window.parent.document.ImportWizardForm;
		}
		else {
			theform = window.parent.document.forms["ImportWizardForm"];
		}
//		theform.__EVENTTARGET.value = eventTarget.split("$").join(":");
//		theform.__EVENTARGUMENT.value = eventArgument;
		theform.submit();
	}

	__doPostBackParentWindows('','')
<%
Else
%>
	var ProgressState = <%= ProgressState %>;
	var TotalItems = <%= TotalRecords %>;
	var ProgressMessage = '<p>Processed: ' + ProgressState + '/' + TotalItems + '</p>';
	window.parent.document.getElementById('LabelStep4ProcessingStatus').innerHTML = ProgressMessage;
	window.location = window.location.reload();
<%
End If
%>
-->
</script>
<asp:Label runat="server" id="LiteralStep4ProcessingMessageLog" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_wo_form.aspx"-->