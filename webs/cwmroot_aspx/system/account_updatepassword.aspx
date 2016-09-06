<%@ Page ValidateRequest="false" Inherits="CompuMaster.camm.WebManager.Pages.UserAccount.ChangeUserPassword" language="VB" %>
<%@ Import NameSpace="System.Data" %>
<%@ Import NameSpace="System.Data.SqlClient" %>
<%@ Import NameSpace="CompuMaster.camm.WebManager.WMSystem" %>
<%@ Import NameSpace="CompuMaster.camm.WebManager" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" SecurityObject="@@Public" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top.aspx"-->
<%
If HideForm = False Then
%>
	<TABLE cellSpacing=10 cellPadding=0 bgColor="#ffffff" border=0>
	  <TBODY>
	  <TR>
	    <TD vAlign=top>
		<P><font face="Arial" size="3"><b><%= cammWebManager.Internationalization.UpdatePW_Descr_Title %></b></font></P>
		<% If ErrMsg <> "" Then Response.Write ("<p><font face=""Arial"" size=""2"" color=""red"">" & ErrMsg & "</font></p>") %>
		<TABLE cellSpacing=0 cellPadding=0 bgColor="#ffffff" border=0 bordercolor="#C1C1C1">
		  <TBODY>
		  <TR>
	        <TD vAlign=top><FORM METHOD="POST" ACTION="<%= Response.ApplyAppPathModifier(Request.ServerVariables("SCRIPT_NAME")) %>" id=form1 name=form1>
			  <input type="hidden" name="loginname" value="<%= Session("System_Username") %>">
		      <TABLE cellSpacing=0 cellPadding=3 width="100%" border="0" bordercolor="#FFFFFF">
		        <TBODY>
					<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size=2><b><%= cammWebManager.Internationalization.UpdatePW_Descr_PleaseSpecifyCurrendAndOldPW %></b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdatePW_Descr_CurrentPW %> <font color="#C1C1C1"> *</font> </FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><input style="width: 200px" type="password" name="oldpassword" value=""></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdatePW_Descr_NewPW %> <font color="#C1C1C1"> *</font> </FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><input style="width: 200px" type="password" name="newpassword1" value="" maxlength="64" autocomplete="new-password"></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdatePW_Descr_NewPWConfirm %> <font color="#C1C1C1"> *</font> </FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><input style="width: 200px" type="password" name="newpassword2" value="" maxlength="64" autocomplete="new-password"></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><input type="submit" name="submit" value="<%= Server.htmlencode(cammWebManager.Internationalization.UpdatePW_Descr_Submit) %>"></FONT></P></TD>
					<TD VAlign="Middle" Align="Right" Width="240"><P><font face="Arial" size="2" color="#C1C1C1"><%= cammWebManager.Internationalization.UpdatePW_Descr_RequiredFields %></font></P></TD>
					</TR>
		        </TBODY></TABLE></FORM></TD></TR>
	      </TBODY></TABLE></TD></TR>
	</TBODY></TABLE>
<%
Else
%>
<asp:Literal runat="server" id="Message" />
<%
End If
%>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom.aspx"-->