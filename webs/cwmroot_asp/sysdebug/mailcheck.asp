<!-- #include virtual="/system/definitions.asp" -->
<script runat="server" language="VBScript">
    Response.Write("<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">" & vbNewLine)
    Response.Write("<HTML><HEAD>" & vbNewLine)
    Response.Write("<TITLE>Server connectivity check</TITLE>" & vbNewLine)
    Response.Write("<link rel=""stylesheet"" type=""text/css"" href=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_SystemData & "style_standard.css"">" & vbNewLine)
    Response.Write("</HEAD>" & vbNewLine)
    Response.Write("<BODY vLink=""#585888"" aLink=""#000080"" link=""#000080"" leftMargin=""0"" topMargin=""0"" marginwidth=""0"" marginheight=""0"" bgcolor=""#FFFFFF"">" & vbNewLine)
    Response.Write("<table border=""0"" cellspacing=""10"" cellpadding=""0""><tr><td>" & vbNewLine)
    Response.Write("<FONT face=""Arial"" size=2>" & vbNewLine)

    Response.Write("<h4>Test environment</h4>")
    Response.Write("<p>Configured to send e-mails via camm Web-Manager mail queue</p>")

    Response.Write("<h4>Test results</h4>")
    Dim bufErrDetails 
    bufErrDetails = ""
    If System_SendEMailEx(TechnicalServiceEMailAccountName, TechnicalServiceEMailAccountAddress, "camm WebManager - Test of mail configuration", "If you receive this mail then there is no general malfunction of the e-mail system of camm Web-Manager." & vbNewLine & vbNewLine & "This mail has been sent by the ASP engine from server " & GetCurrentServerIdentString, "", DevelopmentEMailAccountAddress, DevelopmentEMailAccountAddress, "") = True Then
        Response.Write("<p><font color=""green""><strong>The server has been configured for mail sending via Mail Queue successfully.</strong></font></p>")
        Response.Write("<p>" & TechnicalServiceEMailAccountName & " (" & TechnicalServiceEMailAccountAddress & ") should have got a test e-mail. If it is missing, then you should consult your camm Web-Manager supervisor and solve any routing errors.</p>")
    Else
        bufErrDetails = err.Description
        Response.Write("<p><font color=""red""><strong>The server has detected an error while sending mail via SMTP.</strong></font></p>")
        'Response.Write("<p>Reported error of mail component: <br><em>" & Replace(Replace(Replace(bufErrDetails, chrw(10) & chrw(13), "<br>"),chrw(13), "<br>"),chrw(10), "<br>") & "</em></p>")
    End If
    bufErrDetails = ""
    If System_SendEMail_MultipleRcpts(TechnicalServiceEMailAccountAddress, DevelopmentEMailAccountAddress, "", "camm WebManager - Test of mail configuration with multiple receipients", "If you receive this mail twice (because this is a multiple receipient e-mail with TO and CC addresses) then there is no general malfunction of the e-mail system of camm Web-Manager." & vbNewLine & vbNewLine & "This mail has been sent by the ASP engine from server " & GetCurrentServerIdentString, "", DevelopmentEMailAccountAddress, DevelopmentEMailAccountAddress, "") = True Then
        Response.Write("<p><font color=""green""><strong>The server has been configured for mail sending via Mail Queue for multiple receipients successfully.</strong></font></p>")
        Response.Write("<p>" & TechnicalServiceEMailAccountName & " (" & TechnicalServiceEMailAccountAddress & ") should have got a test e-mail. If it is missing, then you should consult your camm Web-Manager supervisor and solve any routing errors.</p>")
    Else
        bufErrDetails = err.Description
        Response.Write("<p><font color=""red""><strong>The server has detected an error while sending mail via SMTP for multiple receipients.</strong></font></p>")
        'Response.Write("<p>Reported error of mail component: <br><em>" & Replace(Replace(Replace(bufErrDetails, chrw(10) & chrw(13), "<br>"),chrw(13), "<br>"),chrw(10), "<br>") & "</em></p>")
    End If
    response.write (err.Description)
    Response.Write("</FONT></TD>" & vbNewLine)
    Response.Write("</tr>" & vbNewLine)
    Response.Write("</table>" & vbNewLine)
    Response.Write("</BODY>" & vbNewLine)
    Response.Write("</HTML>" & vbNewLine)
</script>