<%@ Page MasterPageFile="~/portal/MasterPage.master" language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Page"%>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammwebmanager" runat="server" />
<%@ Register TagPrefix="cammWebEdit" Namespace="CompuMaster.camm.WebManager.Modules.WebEdit.Controls" Assembly="cammWM" %>
<script lang="vb" runat="server">
    Sub page_load(sender As Object, e As eventargs)

        Select Case cammWebManager.UI.LanguageID
            Case 2  'German
                cammWebManager.PageTitle = "Impressum"
            Case Else 'English
                cammWebManager.PageTitle = "Editorial"
        End Select

    End Sub
</script>
<asp:content id="Content2" contentplaceholderid="ContentPlaceHolderMain" runat="server">

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
				YourCompany<br />
				Your street<br />
				Your town, your zip code<br />
				Your country<p>
				Tel.: +1 800 123456789<br />
				Fax: +1 800 123456780<br />
				e-mail: <a href="mailto:info@yourcompany.com">info@yourcompany.com</a><br />
				<a target="_blank" href="http://www.yourcompany.com">http://www.yourcompany.com</a></font></p>

				<p><FONT face=Arial size=3>
				Handelsregister Musterstadt<br />
				HRB 1234<br />
				<br />
				UST-ID: DE123456789<br />
				<br />
				Geschäftsführung:<br />
				Max Mustermann<br />
				<br />
				Verantwortlich für den Inhalt:<br />
				Susanne Muster<br />
				<br />
				Namentlich gekennzeichnete Beiträge geben nicht immer die Meinung des Herausgebers wieder.<br />
				</font></p>

				</td>
				</TR></TABLE></TD></TR></TABLE></TD></TR></TBODY></TABLE>

        </cammWebEdit:SmartWcms>

</asp:content>