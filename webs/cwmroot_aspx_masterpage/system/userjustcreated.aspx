<%@ Page language="VB" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" SecurityObject="@@Public" />
<%
    dim AreaImage
    AreaImage = cammWebManager.System_GetServerGroupImageBigAddr(cammWebManager.CurrentServerIdentString)

	cammWebManager.PageTitle = Server.htmlencode(cammWebManager.Internationalization.UserJustCreated_Descr_Title)
%>
<!--#include virtual="/sysdata/includes/standardtemplate_top.aspx"-->
      <TABLE cellSpacing=0 cellPadding=0 width="100%" border=0>
        <TBODY><tr>
	    <TD vAlign=top>
		<h2><IMG src="<%= AreaImage %>" align=right border=0><%= cammWebManager.Internationalization.UserJustCreated_Descr_AccountCreated %></h2>
		<em><font face="Arial" size="3"><%= cammWebManager.Internationalization.UserJustCreated_Descr_LookAroundNow %></font></em><br></p>
		<p><font face="Arial" size="3"><%= cammWebManager.Internationalization.UserJustCreated_Descr_PleaseNote %></font></p>
		</TD></TR>
	  </TBODY></TABLE>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom.aspx"-->
