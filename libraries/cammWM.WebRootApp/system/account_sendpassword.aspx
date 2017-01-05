<%@ Page Inherits="CompuMaster.camm.WebManager.Pages.UserAccount.SendUserPassword" language="VB" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top.aspx"-->
<% If Request.Form ("loginname") = "" and Request.Form ("email") = "" then %>
<table border="0" width="100%" cellspacing="0" cellpadding="0">
<tr><td><table border="0" cellspacing="40" cellpadding="0"><tr><td>
		<P><font face="Arial" size="4"><%= cammWebManager.Internationalization.SendPassword_Descr_Title %></font></P>
		<p><hr></p>
		<TABLE cellSpacing=0 cellPadding=0 bgColor=#ffffff border=0 bordercolor="#c1c1c1">
		  <TBODY>
		  <TR><TD vAlign=top><FORM ACTION="<%= Response.ApplyAppPathModifier(Request.ServerVariables("SCRIPT_NAME")) %>" METHOD="POST" id=form1 name=form1>
		      <TABLE cellSpacing=0 cellPadding=3 width="100%" border="0" bordercolor="#ffffff">
		        <TBODY>
					<TR>
						<TD VAlign="top" WIDTH="160"><P><FONT face="Arial" size=3><%= cammWebManager.Internationalization.SendPassword_Descr_LoginName %> <font color="#C1C1C1"> *</font> </FONT></P></TD>
						<TD VAlign="top" Width="240"><P><FONT face="Arial" size=3><input name="loginname" width="30" ></FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="top" WIDTH="160"><P><FONT face="Arial" size=3><%= cammWebManager.Internationalization.SendPassword_Descr_EMail %> <font color="#C1C1C1"> *</font> </FONT></P></TD>
						<TD VAlign="top" Width="240"><P><FONT face="Arial" size=3><input name="email" width="30" ></FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="top" WIDTH="160"><P><FONT face="Arial" size=3><input type="submit" name="submit" value="<%= Server.htmlencode(cammWebManager.Internationalization.SendPassword_Descr_Submit) %>" width="30"></FONT></P></TD>
						<TD VAlign="Middle" Align="Right" Width="240"><P><font face="Arial" size="2" color="#C1C1C1">* <%= cammWebManager.Internationalization.SendPassword_Descr_RequiredFields %></font></P></TD>
					</TR>
					<TR>
						<TD VAlign="top"><P><FONT face="Arial" size=2><br><a href="<%= cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL %>"><%= cammWebManager.Internationalization.SendPassword_Descr_BackToLogin %></a><br> &nbsp;</FONT></P></TD>
					</TR>
		        </TBODY></TABLE></FORM></TD></TR>
	      </TBODY></TABLE></TD></TR></TABLE>
<% Else %>
	<%
            'Message verschicken
            Dim SuccessMessage As String
            Try
                ValidateInputAndSendMail(Trim(Request.Form("loginname")), Trim(Request.Form("email")))
                SuccessMessage = String.Format(cammWebManager.Internationalization.SendPassword_Descr_PasswordSentTo, Request.Form("email"))
            Catch ex As Exception
                SuccessMessage = "<font face=""Arial"" size=""3"" color=""red"">" & ex.Message & "</font>"
            End Try
	%>
<table border="0" width="100%" cellspacing="0" cellpadding="0">
<tr><td><table border="0" cellspacing="40" cellpadding="0"><tr><td>
		<P><font face="Arial" size="3"><b><%= cammWebManager.Internationalization.SendPassword_Descr_Title %></b></font></P>
		<TABLE cellSpacing=0 cellPadding=0 bgColor=#ffffff border=0 bordercolor="#c1c1c1">
		  <TBODY>
		  <TR>
	        <TD vAlign=top>
		      <TABLE cellSpacing=0 cellPadding=3 width="100%" border="0" bordercolor="#ffffff">
		        <TBODY>
					<TR>
						<TD VAlign="top"><P><FONT face="Arial" size=2><%= SuccessMessage %><br> &nbsp;</FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="top"><P><FONT face="Arial" size=2><%= String.Format(cammWebManager.Internationalization.SendPassword_Descr_FurtherCommentWithContactAddress, cammWebManager.StandardEMailAccountAddress, cammWebManager.StandardEMailAccountAddress) %></FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="top"><P><FONT face="Arial" size=2><br><a href="<%= cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL %>"><%= cammWebManager.Internationalization.SendPassword_Descr_BackToLogin %></a><br> &nbsp;</FONT></P></TD>
					</TR>
		        </TBODY></TABLE></TD></TR>
	      </TBODY></TABLE></TD></TR>
	  </TABLE>
<%
End If %>
</td></tr>
</table>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom.aspx"-->