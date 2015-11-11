<%@ Page validateRequest="false" Inherits="CompuMaster.camm.WebManager.Pages.Specialized.ErrorPage" language="VB" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top.aspx"-->
<TABLE cellSpacing=0 cellPadding=0 width="100%" border=0>
<TBODY>
	<TR>
		<TD vAlign=top>
		<P><font face="Arial" size="3"><b><%= cammWebManager.Internationalization.AccessError_Descr_FollowingError & "<br>" & DisplayLoginDenied %></b></font></P>
		<p><em><font color="red" face="Arial" size="3"><%= DisplayMessage %></font></em></p>
		</TD>
	</TR>
	<% If HideLogonAnchor = False Then %>
	<TR>
		<TD VAlign="top"><P><FONT face="Arial" size=2><br><a href="<%= cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL %>"><%= cammWebManager.Internationalization.AccessError_Descr_BackToLogin %></a><br> &nbsp;</FONT></P></TD>
	</TR>
	<% End If %>
</TBODY>
</TABLE>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom.aspx"-->