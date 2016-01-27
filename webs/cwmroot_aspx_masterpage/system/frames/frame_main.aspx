<%@ Page language="VB" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" SecurityObject="@@Public" />
<script language="vb" runat="server">

	sub Page_Init (sender as object, e as eventargs)
		cammWebManager.PageTitle = cammWebManager.Internationalization.WelcomeTextWelcomeMessage
	end sub

</script>
<!--#include virtual="/sysdata/includes/standardtemplate_top.aspx"-->
      <TABLE cellSpacing=0 cellPadding=0 width="100%" border=0>
        <TBODY>
        <TR>
          <TD vAlign=top><FONT face=arial><IMG src="<%= cammWebManager.System_GetServerGroupImageBigAddr(cammWebManager.CurrentServerIdentString) %>" align=right border=0></FONT><STRONG><font face="Arial" size="4"><%= cammWebManager.Internationalization.WelcomeTextWelcomeMessage %></font></STRONG><FONT face=arial>
          <%= IIf(cammWebManager.Internationalization.WelcomeTextIntro<>"" or cammWebManager.Internationalization.HighlightTextIntro<>"","<p>","") & cammWebManager.Internationalization.WelcomeTextIntro & IIf(cammWebManager.Internationalization.WelcomeTextIntro<>"" and cammWebManager.Internationalization.HighlightTextIntro<>""," ","") & cammWebManager.Internationalization.HighlightTextIntro & IIf(cammWebManager.Internationalization.WelcomeTextIntro<>"" or cammWebManager.Internationalization.HighlightTextIntro<>"","</p>","") %>
          <%= IIf(cammWebManager.Internationalization.HighlightTextTechnicalSupport<>"","<p>","") & Replace(cammWebManager.Internationalization.HighlightTextTechnicalSupport,vbnewline,"</p><p>") & IIf(cammWebManager.Internationalization.HighlightTextTechnicalSupport<>"","</p>","") %>
          <%= IIf(cammWebManager.Internationalization.HighlightTexteMedia<>"","<p>","") & Replace(cammWebManager.Internationalization.HighlightTexteMedia,vbnewline,"</p><p>") & IIf(cammWebManager.Internationalization.HighlightTexteMedia<>"","</p>","") %>
          <%= IIf(cammWebManager.Internationalization.HighlightTextExtro<>"","<p>","") & cammWebManager.Internationalization.HighlightTextExtro & IIf(cammWebManager.Internationalization.HighlightTextExtro<>"","</p>","") %>
          <%= "<p>" & cammWebManager.sprintf(cammWebManager.Internationalization.WelcomeTextFeedbackToContact, cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString(), "AreaContentManagementContactEMail"), cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString(), "AreaContentManagementContactTitle")) & "</p>" %>
            </font></TD></TR></TBODY></TABLE>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom.aspx"-->
