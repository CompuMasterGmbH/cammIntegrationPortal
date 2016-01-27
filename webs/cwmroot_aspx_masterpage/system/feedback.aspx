<%@ Page language="VB" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<script runat="server">

Dim MyLangID as integer

	dim SessionUserID as string, SessionUser_Addresses as string, SessionUser_Name as string, SessionUser_Company as string
	dim SessionUser_email as string, SessionUser_1stLang as string, SessionUser_2ndLang as string, SessionUser_3rdLang as string
	dim SessionUser_Motivation as string

sub Page_Load (sender as object, e as eventargs)

	If Session("System_Username") <> "" Then
		SessionUserID = cammWebManager.CurrentUserID
		SessionUser_Addresses = cammWebManager.CurrentUserInfo.SalutationMrOrMs
		SessionUser_Name = cammWebManager.CurrentUserInfo.FullName
		SessionUser_Company = cammWebManager.CurrentUserInfo.Company
		SessionUser_email = cammWebManager.CurrentUserInfo.EMailAddress
		SessionUser_1stLang = cammWebManager.CurrentUserInfo.PreferredLanguage1.ID
		SessionUser_2ndLang = cammWebManager.CurrentUserInfo.PreferredLanguage2.ID
		SessionUser_3rdLang = cammWebManager.CurrentUserInfo.PreferredLanguage3.ID
		SessionUser_Motivation = cammWebManager.CurrentUserInfo.AdditionalFlags("Motivation")
	End If

	MyLangID = cammWebManager.UIMarket
	Select Case MyLangID
		Case 3,2,1: 'DoNothing - Supported languages
		Case Else
			MyLangID = cammWebManager.Internationalization.GetAlternativelySupportedLanguageID (MyLangID)
	End Select

end sub

</script>
				<html>
				<head>
				<!--#include virtual="/sysdata/includes/metalink.aspx"-->
<%

	Select Case MyLangID
		case 2: 'DEU
%>
				<link rel="stylesheet" type="text/css" href="<%= cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData %>style_standard.css">
				<title>Feedback</title>
				<script language="JavaScript">
				<!--
				function Check(theForm)
				{

				  if (theForm.EMail.value == "")
				  {
				  alert("Ihr Profil enthält keine e-mail-Adresse. Bitte ergänzen Sie zuerst dieses und versuchen es anschließend erneut.");
				  theForm.EMail.focus();
				  return (false);
				  }

				  if (theForm.Comment.value == "")
				  {
				  alert("Bitte geben Sie einen Wert in das Feld \"Kommentar\" ein.");
				  theForm.Comment.focus();
				  return (false);
				  }

				  return (true);

				}
				//-->
				</script>
				</head>

				<body leftmargin="20" topmargin="20" merginwidth="20" marginheight="20" background="images/feedback/background_question.gif" bgcolor="#FFFFFF">

				<table border="0" width="100%" height="100%" cellspacing="0" cellpadding="0">
				  <tr>
				    <td align="center" valign="middle">
				      <div align="center">
				        <center>
				      <table border="0" cellspacing="0" cellpadding="0">
				        <tr>
				          <td width="100%" valign="top">
				<form method="POST" action="feedback_gotit.aspx" onsubmit="return Check(this)" id=form2 name=form2><table border="0" cellspacing="10" cellpadding="0">

				<tr><td><table border="0" cellspacing="0" cellpadding="0">

				<tr><td align="left"><input type="hidden" name="message_email_sent" value="Thank you for your comments!"><input type="hidden" name="message_email_error" value="Ein Fehler trat bei der &Uuml;bermittlung auf. Bitte versuchen Sie es in K&uuml;rze nochmals!"><b><font face="Times New Roman" color="#00426b" size="5">Feedback</font></b></td></tr>

				<tr><td><table border="1" bordercolor="#C1C1C1" cellspacing="0" cellpadding="0">

				<tr><td><table border="0" cellspacing="0" cellpadding="2">

				<tr>
				  <td width="20" height="16" valign="top"><img border="0" src="images/feedback/name.gif" width="20" height="20"></td>
				  <td width="120" height="16" valign="top" align="left"><font face="Arial" color="#000000" size="3">Name:</font></td>
				  <td width="300" height="16" valign="top"><% If Session("System_Username") = "" Then %><input type="text" width="300" style="width: 300; position: relative; height: 23" value="" name="Name"><% Else %><input type="hidden" value="<%= SessionUser_Name %>" name="Name"><%= SessionUser_Name %><% End If %></td>
				</tr>
				<tr>
				  <td width="20" height="16" valign="top"><img border="0" src="images/feedback/email.gif" width="20" height="20"></td>
				  <td width="120" height="16" valign="top" align="left"><font face="Arial" color="#000000" size="3">e-mail:</font></td>
				  <td width="300" height="16" valign="top"><% If Session("System_Username") = "" Then %><input type="text" width="300" style="width: 300; position: relative; height: 23" value="" name="EMail"><% Else %><input type="hidden" value="<%= SessionUser_email %>" name="EMail"><%= SessionUser_email %><% End If %></td>
				</tr>
				<input type="hidden" name="UserID" value="<%= SessionUserID %>">
				<input type="hidden" name="Addresses" value="<%= SessionUser_Addresses %>">
				<input type="hidden" name="Company" value="<%= SessionUser_Company %>">
				<input type="hidden" name="1stPreferredLanguage" value="<%= SessionUser_1stLang %>">
				<input type="hidden" name="2ndPreferredLanguage" value="<%= SessionUser_2ndLang %>">
				<input type="hidden" name="3rdPreferredLanguage" value="<%= SessionUser_3rdLang %>">
				<input type="hidden" name="MotivationOfMembership" value="<%= SessionUser_Motivation %>">
				</table></td></tr>

				</table></td></tr>

				<tr><td>&nbsp;</td></tr>

				<tr><td><table border="1" bordercolor="#C1C1C1" cellspacing="0" cellpadding="0">

				<tr><td><table border="0" cellspacing="0" cellpadding="2">

				<tr>
				  <td width="20" height="16" valign="top"><img border="0" src="images/feedback/thema.gif" width="20" height="20"></td>
				  <td width="120" height="16" valign="top" align="left"><font face="Arial" color="#000000" size="3">Thema:</font></td>
				  <td width="300" height="16"><select style="width: 300; position: relative; height: 23" size="1" name="Theme" tabindex="0">
					<% If Request.QueryString("topicdescription") <> "" Then %>
					        <option selected><%= Server.HTMLEncode(Request.QueryString("topicdescription")) %></option>
					<% Else %>
					        <option<% If Request.QueryString("topic") = "myprofile" Then Response.Write (" selected") %>>Mein Benutzerprofil</option>
					        <option<% If Request.QueryString("topic") = "security" Then Response.Write (" selected") %>>Sicherheit</option>
					        <option<% If Request.QueryString("topic") = "newfeatures" Then Response.Write (" selected") %>>Erweiterung der Webseite</option>
					        <option<% If Request.QueryString("topic") = "content" Then Response.Write (" selected") %>>Inhalt</option>
					        <option<% If Request.QueryString("topic") = "design" Then Response.Write (" selected") %>>Layouts/Grafik</option>
				        	<option<% If Request.QueryString("topic") = "other" Then Response.Write (" selected") %>>Sonstiges</option>
					<% End If %>
				      </select></td>
					<% If Request.QueryString("email") <> "" Then %><input type="hidden" name="emailto" value="<%= Server.HTMLEncode(Request.QueryString("email")) %>"><% End If %>
				</tr>
				<tr>
				  <td width="20" height="16" valign="top"><img border="0" src="images/feedback/kommentar.gif" width="20" height="20"></td>
				  <td width="120" height="16" valign="top" align="left"><font face="Arial" color="#000000" size="3">Kommentar:</font></td>
				  <td width="300" valign="top"><textarea rows="9" width="300" type="text" style="width: 300px; position: relative" size="30" name="Comment" cols="20"><%= Request.QueryString("comment") %></textarea></td>
				</tr>

				</table></td></tr>

				</table></td></tr>

				<tr><td>&nbsp;</td></tr>

				<tr>
				  <td align="right" valign="bottom"><input type="submit" value="Abschicken" name="Submit"> <input type="reset" value="Zurücksetzen" name="Reset"></td>
				</tr>

				</table></td></tr>

				</table></form>

				<p align="right"><font size="1">V<%= cammWebManager.System_Version %></font></p>
				          </td>
				        </tr>
				        </center>
				      </table>
				      </div>
				    </td>
				  </tr>
				</table>

				</body>

				</html>
<%
		case else: 'ENG
%>
				<link rel="stylesheet" type="text/css" href="<%= cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData %>style_standard.css">
				<title>Feedback</title>

				<script language="JavaScript">
				<!--
				function Check(theForm)
				{

				  if (theForm.Name.value == "")
				  {
				  alert("Your profile is incomplete. Please update it with your name and try again.");
				  theForm.Name.focus();
				  return (false);
				  }

				  if (theForm.EMail.value == "")
				  {
				  alert("Your profile is incomplete. Please update it with your e-mail address and try again.");
				  theForm.EMail.focus();
				  return (false);
				  }

				  if (theForm.Comment.value == "")
				  {
				  alert("Please insert a value into the field \"Comment\".");
				  theForm.Comment.focus();
				  return (false);
				  }

				  return (true);

				}
				//-->
				</script>
				</head>

				<body leftmargin="20" topmargin="20" merginwidth="20" marginheight="20" background="images/feedback/background_question.gif" bgcolor="#FFFFFF">

				<table border="0" width="100%" height="100%" cellspacing="0" cellpadding="0">
				  <tr>
				    <td align="center" valign="middle">
				      <div align="center">
				        <center>
				      <table border="0" cellspacing="0" cellpadding="0">
				        <tr>
				          <td width="100%" valign="top">
				<form method="POST" action="feedback_gotit.aspx" onsubmit="return Check(this)" id=form1 name=form1><table border="0" cellspacing="10" cellpadding="0">

				<tr><td><table border="0" cellspacing="0" cellpadding="0">

				<tr><td align="left"><input type="hidden" name="message_email_sent" value="Thank you for your comments!"><input type="hidden" name="message_email_error" value="Ein Fehler trat bei der &Uuml;bermittlung auf. Bitte versuchen Sie es in K&uuml;rze nochmals!"><b><font face="Times New Roman" color="#00426b" size="5">Feedback on the
				    Extranet-Site:</font></b></td></tr>

				<tr><td><table border="1" bordercolor="#C1C1C1" cellspacing="0" cellpadding="0">

				<tr><td><table border="0" cellspacing="0" cellpadding="2">

				<tr>
				  <td width="20" height="16" valign="top"><img border="0" src="images/feedback/name.gif" width="20" height="20"></td>
				  <td width="120" height="16" valign="top" align="left"><font face="Arial" color="#000000" size="3">Name:</font></td>
				  <td width="300" height="16" valign="top"><input type="text" width="300" style="width: 300; position: relative; height: 23" value="" name="Name"></td>
				</tr>
				<tr>
				  <td width="20" height="16" valign="top"><img border="0" src="images/feedback/email.gif" width="20" height="20"></td>
				  <td width="120" height="16" valign="top" align="left"><font face="Arial" color="#000000" size="3">e-mail:</font></td>
				  <td width="300" height="16" valign="top"><input type="text" width="300" style="width: 300; position: relative; height: 23" value="" name="EMail"></td>
				</tr>
				<input type="hidden" name="UserID" value="<%= SessionUserID %>">
				<input type="hidden" name="Addresses" value="<%= SessionUser_Addresses %>">
				<input type="hidden" name="Company" value="<%= SessionUser_Company %>">
				<input type="hidden" name="1stPreferredLanguage" value="<%= SessionUser_1stLang %>">
				<input type="hidden" name="2ndPreferredLanguage" value="<%= SessionUser_2ndLang %>">
				<input type="hidden" name="3rdPreferredLanguage" value="<%= SessionUser_3rdLang %>">
				<input type="hidden" name="MotivationOfMembership" value="<%= SessionUser_Motivation %>">
				</table></td></tr>

				</table></td></tr>

				<tr><td>&nbsp;</td></tr>

				<tr><td><table border="1" bordercolor="#C1C1C1" cellspacing="0" cellpadding="0">

				<tr><td><table border="0" cellspacing="0" cellpadding="2">

				<tr>
				  <td width="20" height="16" valign="top"><img border="0" src="images/feedback/thema.gif" width="20" height="20"></td>
				  <td width="120" height="16" valign="top" align="left"><font face="Arial" color="#000000" size="3">Subject:</font></td>
				  <td width="300" height="16"><select style="width: 300; position: relative; height: 23" size="1" name="Theme" tabindex="0">
					<% If Request.QueryString("topicdescription") <> "" Then %>
					        <option selected><%= Server.HTMLEncode(Request.QueryString("topicdescription")) %></option>
					<% Else %>
				        	<option<% If Request.QueryString("topic") = "myprofile" Then Response.Write (" selected") %>>My user profile</option>
					        <option<% If Request.QueryString("topic") = "security" Then Response.Write (" selected") %>>Security</option>
					        <option<% If Request.QueryString("topic") = "newfeatures" Then Response.Write (" selected") %>>Improve the website</option>
					        <option<% If Request.QueryString("topic") = "content" Then Response.Write (" selected") %>>Content</option>
					        <option<% If Request.QueryString("topic") = "design" Then Response.Write (" selected") %>>Layout/Graphics</option>
					        <option<% If Request.QueryString("topic") = "other" Then Response.Write (" selected") %>>Other</option>
					<% End If %>
				      </select></td>
					<% If Request.QueryString("email") <> "" Then %><input type="hidden" name="emailto" value="<%= Server.HTMLEncode(Request.QueryString("email")) %>"><% End If %>
				      </select></td>
				</tr>
				<tr>
				  <td width="20" height="16" valign="top"><img border="0" src="images/feedback/kommentar.gif" width="20" height="20"></td>
				  <td width="120" height="16" valign="top" align="left"><font face="Arial" color="#000000" size="3">Comment:</font></td>
				  <td width="300" valign="top"><textarea rows="9" width="300" type="text" style="width: 300px; position: relative" size="30" name="Comment" cols="20"><%= Request.QueryString("comment") %></textarea></td>
				</tr>

				</table></td></tr>

				</table></td></tr>

				<tr><td>&nbsp;</td></tr>

				<tr>
				  <td align="right" valign="bottom"><input type="submit" value="Submit" name="Submit"> <input type="reset" value="Reset" name="Reset"></td>
				</tr>

				</table></td></tr>

				</table></form>

				<p align="right"><font size="1">V<%= cammWebManager.System_Version %></font></p>
				          </td>
				        </tr>
				        </center>
				      </table>
				      </div>
				    </td>
				  </tr>
				</table>

				</body>

				</html>
<%
	End Select
%>