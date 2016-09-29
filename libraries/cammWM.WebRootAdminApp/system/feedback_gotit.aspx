<%@ Page validateRequest=false %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<html>
<head>
<!--#include virtual="/sysdata/includes/metalink.aspx"-->
<link rel="stylesheet" type="text/css" href="<%= cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData %>style_standard.css">
<title>Success!</title>
</head>

<body background="images/feedback/background_question.gif" bgcolor="#FFFFFF">
<%
Dim MailStatus As String
dim MainSubject as string
dim Receipient as string

    Receipient = cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "AreaContentManagementContactEMail")
    
    MainSubject = "Feedback von " & cammWebManager.Internationalization.OfficialServerGroup_Title

    'Message verschicken
    
    Dim bufErrorDetails As String
    If Request.Form("Theme") = Nothing And Request.Form("Comment") = Nothing And Request.Form("Name") = Nothing And Request.Form("Abteilung") = Nothing And Request.Form("EMail") = Nothing Then
        MailStatus = "<center><p>&nbsp;</p><p><img src=""/sysdata/images/feedback/smiley_sorry.gif""><br><font face=""Arial""><br><br>(Missing emaildata)</font></p></center>"
    ElseIf Not cammWebManager.System_SendEMailEx(Receipient, Receipient, MainSubject & " - " & Request.Form("Theme"), Request.Form("Comment") & vbNewLine & vbNewLine & "--------------------------" & vbNewLine & "Sent by: " & CompuMaster.camm.WebManager.Utils.LookupRealRemoteClientIPOfHttpContext & " / " & Request.ServerVariables("REMOTE_HOST") & " / " & Request.ServerVariables("REMOTE_ADDR") & " / " & Request.ServerVariables("REMOTE_USER"), "", Request.Form("Name") & IIf(Request.Form("Abteilung") <> "", " (" & Request.Form("Abteilung") & ")", ""), Request.Form("EMail"), "", bufErrorDetails) Then
        If bufErrorDetails = "" Then
            MailStatus = "<center><p>&nbsp;</p><p><img src=""/sysdata/images/feedback/smiley_sorry.gif""><br><font face=""Arial"">" & Request.Form("message_email_error") & "<br><br>(No error details found)</font></p></center>"
        Else
            MailStatus = "<center><p>&nbsp;</p><p><img src=""/sysdata/images/feedback/smiley_sorry.gif""><br><font face=""Arial"">" & Request.Form("message_email_error") & "<br><br>(Error details: " & bufErrorDetails.Replace(ControlChars.CrLf, "<br>") & ")</font></p></center>"
        End If
    Else
        MailStatus = "<center><p>&nbsp;</p><p><img src=""/sysdata/images/feedback/smiley_happy.gif""><br><font face=""Arial"">" & Request.Form("message_email_sent") & "</font></p></center><!-- sent to " & Receipient & " -->"
    End If
    Response.Write(MailStatus)
	
%>
</body>
</html>