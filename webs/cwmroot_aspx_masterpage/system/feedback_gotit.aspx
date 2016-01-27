<%@ Page Title="Success!" MasterPageFile="~/portal/MasterPage.master" validateRequest="False" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<asp:content id="Content2" contentplaceholderid="ContentPlaceHolderMain" runat="server">
<%
    Dim MailStatus As String
    Dim MainSubject As String
    Dim Receipient As String

    Receipient = cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "AreaContentManagementContactEMail")

    MainSubject = "Feedback von " & cammWebManager.Internationalization.OfficialServerGroup_Title

    'Message verschicken

    Dim bufErrorDetails As String = Nothing
    If Request.Form("Theme") = Nothing And Request.Form("Comment") = Nothing And Request.Form("Name") = Nothing And Request.Form("Abteilung") = Nothing And Request.Form("EMail") = Nothing Then
        MailStatus = "<center><p>&nbsp;</p><p><img src=""/portal/images/feedback/smiley_sorry.gif""><br /><font face=""Arial""><br /><br />(Missing emaildata)</font></p></center>"
    ElseIf Not cammWebManager.System_SendEMailEx(Receipient, Receipient, MainSubject & " - " & Request.Form("Theme"), Request.Form("Comment") & vbNewLine & vbNewLine & "--------------------------" & vbNewLine & "Sent by: " & CompuMaster.camm.WebManager.Utils.LookupRealRemoteClientIPOfHttpContext & " / " & Request.ServerVariables("REMOTE_HOST") & " / " & Request.ServerVariables("REMOTE_ADDR") & " / " & Request.ServerVariables("REMOTE_USER"), "", Request.Form("Name") & IIf(Request.Form("Abteilung") <> "", " (" & Request.Form("Abteilung") & ")", ""), Request.Form("EMail"), "", bufErrorDetails) Then
        If bufErrorDetails = "" Then
            MailStatus = "<center><p>&nbsp;</p><p><img src=""/portal/images/feedback/smiley_sorry.gif""><br /><font face=""Arial"">" & Request.Form("message_email_error") & "<br /><br />(No error details found)</font></p></center>"
        Else
            MailStatus = "<center><p>&nbsp;</p><p><img src=""/portal/images/feedback/smiley_sorry.gif""><br /><font face=""Arial"">" & Request.Form("message_email_error") & "<br /><br />(Error details: " & bufErrorDetails.Replace(ControlChars.CrLf, "<br />") & ")</font></p></center>"
        End If
    Else
        MailStatus = "<center><p>&nbsp;</p><p><img src=""/portal/images/feedback/smiley_happy.gif""><br /><font face=""Arial"">" & Request.Form("message_email_sent") & "</font></p></center><!-- sent to " & Receipient & " -->"
    End If
    Response.Write(MailStatus)

%>
</asp:content>