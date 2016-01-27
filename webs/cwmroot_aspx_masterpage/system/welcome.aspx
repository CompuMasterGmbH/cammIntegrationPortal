<%@ Page MasterPageFile="~/portal/MasterPage.master" Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Page" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<asp:content id="Content1" contentplaceholderid="head" runat="server">
<camm:WebManager id="cammWebManager" runat="server" SecurityObject="@@Public" />
</asp:content>
<script lang="vb" runat="server">

    Sub Page_Init(sender As Object, e As EventArgs)
        cammWebManager.PageTitle = cammWebManager.Internationalization.WelcomeTextWelcomeMessage
    End Sub

</script>
<asp:content id="Content2" contentplaceholderid="ContentPlaceHolderMain" runat="server">
      <table cellSpacing=0 cellPadding=0 width="100%" border=0>
        <tbody>
        <tr>
          <td valign="top"><font face="arial"><img src="<%= cammWebManager.System_GetServerGroupImageBigAddr(cammWebManager.CurrentServerIdentString) %>" align=right border=0></font><STRONG><font face="Arial" size="4"><%= cammWebManager.Internationalization.WelcomeTextWelcomeMessage %></font></STRONG><FONT face=arial>
          <%= IIf(cammWebManager.Internationalization.WelcomeTextIntro<>"" or cammWebManager.Internationalization.HighlightTextIntro<>"","<p>","") & cammWebManager.Internationalization.WelcomeTextIntro & IIf(cammWebManager.Internationalization.WelcomeTextIntro<>"" and cammWebManager.Internationalization.HighlightTextIntro<>""," ","") & cammWebManager.Internationalization.HighlightTextIntro & IIf(cammWebManager.Internationalization.WelcomeTextIntro<>"" or cammWebManager.Internationalization.HighlightTextIntro<>"","</p>","") %>
          <%= IIf(cammWebManager.Internationalization.HighlightTextTechnicalSupport<>"","<p>","") & Replace(cammWebManager.Internationalization.HighlightTextTechnicalSupport,vbnewline,"</p><p>") & IIf(cammWebManager.Internationalization.HighlightTextTechnicalSupport<>"","</p>","") %>
          <%= IIf(cammWebManager.Internationalization.HighlightTextExtro <> "", "<p>", "") & cammWebManager.Internationalization.HighlightTextExtro & IIf(cammWebManager.Internationalization.HighlightTextExtro <> "", "</p>", "") %>
          <%= "<p>" & CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.WelcomeTextFeedbackToContact, cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString(), "AreaContentManagementContactEMail"), cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString(), "AreaContentManagementContactTitle")) & "</p>" %>
            </font></td></tr></tbody></table>
</asp:content>
