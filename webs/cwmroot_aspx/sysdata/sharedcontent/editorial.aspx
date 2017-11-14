<%@ Page Language="vb" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<script language="vb" runat="server">
    Sub Page_Load(sender As Object, e As EventArgs)
        Select Case cammWebManager.UILanguage
            Case 2  'German
                cammWebManager.PageTitle = "Impressum"
            Case Else 'English
                cammWebManager.PageTitle = "Editorial"
        End Select
    End Sub
</script>
<!--#include virtual="/sysdata/includes/standardtemplate_top.aspx"-->
<h3>Editorial / Impressum</h3>
<p>
    <font face="Arial" size="3">
				YourCompany<br />
				Your street<br />
				Your town, your zip code<br />
				Your country<p>
				Tel.: +1 800 123456789<br />
				Fax: +1 800 123456780<br />
				e-mail: <a href="mailto:info@yourcompany.com">info@yourcompany.com</a><br />
				<a target="_blank" href="http://www.yourcompany.com">http://www.yourcompany.com</a></font>
</p>

<p>
    <font face="Arial" size="3">
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
				Contributions submitted by individual authors do not necessarily reflect the opinion of the publisher.<br />
				Namentlich gekennzeichnete Beiträge geben nicht immer die Meinung des Herausgebers wieder.<br />
				</font>
</p>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom.aspx"-->
