<%@ Page MasterPageFile="~/portal/MasterPage.master" language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Page"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<script runat="server">

sub Page_Load (sender as object, e as eventargs)
	cammWebManager.PageTitle = cammWebManager.Internationalization.StatusLineLegalNote 
end sub

</script>
<asp:content id="Content2" contentplaceholderid="ContentPlaceHolderMain" runat="server">
<%
    Select Case cammWebManager.UI.LanguageID
        Case 2 : 'DEU
			%><!--#include virtual="/system/includes/disclaimer.deu.inc"--><%
		Case Else: 'ENG
			%><!--#include virtual="/system/includes/disclaimer.eng.inc"--><%
	End Select
%>
</asp:content>