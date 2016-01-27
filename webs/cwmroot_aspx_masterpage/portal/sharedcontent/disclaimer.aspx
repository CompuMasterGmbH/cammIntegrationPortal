<%@ Page MasterPage="/portal/MasterPage.master" language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Page"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<script runat="server">

sub Page_Load (sender as object, e as eventargs)
	cammWebManager.PageTitle = cammWebManager.Internationalization.StatusLineLegalNote 
end sub

</script>

<%
	Select case cammWebManager.UILanguage
		case 2: 'DEU
			%><!--#include virtual="/system/includes/disclaimer.deu.inc"--><%
		Case Else: 'ENG
			%><!--#include virtual="/system/includes/disclaimer.eng.inc"--><%
	End Select
%>

