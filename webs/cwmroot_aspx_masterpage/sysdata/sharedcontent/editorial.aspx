<%@ Page Language="vb" trace="false" TraceMode="SortByTime" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammwebmanager" runat="server" />
<%@ Register TagPrefix="cammWebEdit" Namespace="CompuMaster.camm.WebManager.Modules.WebEdit.Controls" Assembly="cammWM" %>
<script language="vb" runat="server">
sub page_load (sender as object, e as eventargs)
	
	Select case cammWebManager.UILanguage
		case 2: 'German
			cammWebManager.PageTitle = "Impressum"
		Case Else 'English
			cammWebManager.PageTitle = "Editorial"
	End Select

end sub
</script>
<html>
<head>
<!--#include virtual="/sysdata/includes/metalink.aspx"-->
<link rel="stylesheet" type="text/css" href="<%= cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData %>style_standard.css">
<title><%= cammWebManager.PageTitle %></title>
</head>
<body>
    <form runat="server">
        <cammWebEdit:SmartWcms ID="SmartWcms" SecurityObjectEditMode="@@Supervisor" MarketLookupMode="Language" runat="server">
            <TABLE cellSpacing=0 cellPadding=0 width="100%" border=0>
				  <TBODY>
				  <TR>
				    <TD vAlign=top>
				      <table border="0" width="100%" cellspacing="30" cellpadding="0">
				        <tr>
				          <td width="100%">
				      <TABLE cellSpacing=0 cellPadding=0 width="100%" border=0>

				<tr><td valign="top"><p><FONT face=Arial SIZE=4><B>Impressum</b></font></P>
				<p><FONT face=Arial size=3>
				YourCompany<BR>
				Your street<BR>
				Your town, your zip code<br>
				Your country<p>
				Tel.: +1 800 123456789<BR>
				Fax: +1 800 123456780<BR>
				e-mail: <a href="mailto:info@yourcompany.com">info@yourcompany.com</a><BR>
				<a target="_blank" href="http://www.yourcompany.com">http://www.yourcompany.com</a></font></p>

				<p><FONT face=Arial size=3>
				Handelsregister Musterstadt<br>
				HRB 1234<br>
				<br>
				UST-ID: DE123456789<br>
				<br>
				Geschäftsführung:<br>
				Max Mustermann<br>
				<br>
				Verantwortlich für den Inhalt:<br>
				Susanne Muster<br>
				<br>
				Namentlich gekennzeichnete Beiträge geben nicht immer die Meinung des Herausgebers wieder.<br>
				</font></p>

				</td>
				</TR></TABLE></TD></TR></TABLE></TD></TR></TBODY></TABLE>

        </cammWebEdit:SmartWcms>
    </form>
</body>
</html>
