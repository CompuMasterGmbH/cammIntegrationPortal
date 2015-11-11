Option Explicit On 
Option Strict On

Imports System.Web

Namespace CompuMaster.camm.WebManager.Pages.Checks

    <System.Runtime.InteropServices.ComVisible(False)> Public Class ServerConfiguration
        Inherits CompuMaster.camm.WebManager.Pages.Page

        Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

            If HttpContext.Current.Request.QueryString("Host") = "" Then
                cammWebManager.Log.RuntimeWarning(New Exception("Unexpected request call for server configuration check, might be a first try of a hacker before attacking your servers"))
                cammWebManager.RedirectToErrorPage(Nothing, "Invalid request for start of server configuration check.", Nothing, Nothing, False)

            Else
                cammWebManager.PageTitle = "Server connectivity check"

                HttpContext.Current.Response.Write("<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">")
                HttpContext.Current.Response.Write("<HTML><HEAD>")
                HttpContext.Current.Response.Write("<TITLE>Server connectivity check</TITLE>")
                HttpContext.Current.Response.Write("<link rel=""stylesheet"" type=""text/css"" href=""" & cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData & "style_standard.css"">")
                HttpContext.Current.Response.Write("</HEAD>")
                HttpContext.Current.Response.Write("<BODY vLink=""#585888"" aLink=""#000080"" link=""#000080"" leftMargin=""0"" topMargin=""0"" marginwidth=""0"" marginheight=""0"" bgcolor=""#FFFFFF"">")
                HttpContext.Current.Response.Write("<table border=""0"" cellspacing=""10"" cellpadding=""0""><tr><td>")
                HttpContext.Current.Response.Write("<FONT face=""Arial"" size=2>")

                HttpContext.Current.Response.Write("<h4>Test environment</h4>")
                HttpContext.Current.Response.Write("<p>Test for host """ & System.Web.HttpUtility.HtmlEncode(HttpContext.Current.Request.QueryString("Host")) & """</p>")
                HttpContext.Current.Response.Write("<p>Current server IP: """ & HttpContext.Current.Request.ServerVariables("LOCAL_ADDR") & """<br>")
                HttpContext.Current.Response.Write("Current server host name: """ & HttpContext.Current.Request.ServerVariables("SERVER_NAME") & """</p>")
                HttpContext.Current.Response.Write("<p>Configured IP / Host Header: """ & cammWebManager.CurrentServerIdentString & """</p>")

                Dim ErrorFound As Boolean = False

                HttpContext.Current.Response.Write("<p>Database connection test and its version: ")
                Try
                    HttpContext.Current.Response.Write(Setup.DatabaseUtils.Version(cammWebManager, False).ToString)
                Catch ex As Exception
                    HttpContext.Current.Response.Write("N/A")
                    HttpContext.Current.Response.Write(" <em><font color=""red"">[" & ex.Message & "]</em></font>")
                    ErrorFound = True
                End Try
                Response.Write("</p>")

                HttpContext.Current.Response.Write("<p>Database connection successfully established: ")
                Try
                    If cammWebManager.System_GetServerInfo.ID <> 0 Then
                        HttpContext.Current.Response.Write("True")
                    End If
                Catch ex As Exception
                    HttpContext.Current.Response.Write("False")
                    HttpContext.Current.Response.Write(" <em><font color=""red"">[" & ex.Message & "]</em></font>")
                    ErrorFound = True
                End Try
                HttpContext.Current.Response.Write("</p>")

                HttpContext.Current.Response.Write("<h4>Test results</h4>")
                If Not ErrorFound AndAlso HttpContext.Current.Request.QueryString("Host") = cammWebManager.CurrentServerIdentString Then
                    HttpContext.Current.Response.Write("<p><font color=""green""><strong>The server is configured correctly.</strong></font> Before you break out the champagne...</p>")
                    HttpContext.Current.Response.Write("<p>...ensure that this server is connectable from your visitors. If your server is running behind a firewall it might use another IP than it does from the other side you are currently testing.</p>")
                Else
                    HttpContext.Current.Response.Write("<p><font color=""red""><strong>The server configuration doesn't match with the values configured on the remote server.</strong> If your server is running behind a firewall it might use another IP than it does from the other side you are currently testing. So, it might work fine from the other side of the firewall.</font></p>")
                    HttpContext.Current.Response.Write("<p>To solve this issue please set up the files /sysdata/config.* on the remote server and try again.</font></p>")
                End If
                HttpContext.Current.Response.Write("</FONT></TD>")
                HttpContext.Current.Response.Write("</tr>")
                HttpContext.Current.Response.Write("</table>")
                HttpContext.Current.Response.Write("</BODY>")
                HttpContext.Current.Response.Write("</HTML>")
            End If
        End Sub

    End Class

    <System.Runtime.InteropServices.ComVisible(False)> Public Class MailConfiguration
        Inherits CompuMaster.camm.WebManager.Pages.Page

        Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

            cammWebManager.PageTitle = "Server mail system check"

            HttpContext.Current.Response.Write("<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">" & vbNewLine)
            HttpContext.Current.Response.Write("<HTML><HEAD>" & vbNewLine)
            HttpContext.Current.Response.Write("<TITLE>Server connectivity check</TITLE>" & vbNewLine)
            HttpContext.Current.Response.Write("<link rel=""stylesheet"" type=""text/css"" href=""" & cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData & "style_standard.css"">" & vbNewLine)
            HttpContext.Current.Response.Write("</HEAD>" & vbNewLine)
            HttpContext.Current.Response.Write("<BODY vLink=""#585888"" aLink=""#000080"" link=""#000080"" leftMargin=""0"" topMargin=""0"" marginwidth=""0"" marginheight=""0"" bgcolor=""#FFFFFF"">" & vbNewLine)
            HttpContext.Current.Response.Write("<table border=""0"" cellspacing=""10"" cellpadding=""0""><tr><td>" & vbNewLine)
            HttpContext.Current.Response.Write("<FONT face=""Arial"" size=2>" & vbNewLine)

            HttpContext.Current.Response.Write("<h4>Test environment</h4>")
            HttpContext.Current.Response.Write("<p>Configured SMTP server: """ & cammWebManager.SMTPServerName & """</p>")
            HttpContext.Current.Response.Write("<p>Configured mail delivery system: """ & cammWebManager.MessagingEMails.MailSystem.ToString & """</p>")

            HttpContext.Current.Response.Write("<h4>Test results</h4>")
            If cammWebManager.SMTPServerName = "" Then
                HttpContext.Current.Response.Write("<p><font color=""red""><strong>The server hasn't been configured for mail sending via SMTP.</strong></font></p>")
                HttpContext.Current.Response.Write("<p>Please configure an SMTP server in the config files.</p>")
            ElseIf cammWebManager.IsSupported.Messaging.eMail Then
                If CType(HttpContext.Current.Cache.Item("WebManager.MailTest.JustDone"), Boolean) = True Then
                    cammWebManager.Log.RuntimeInformation("e-mail system check has already been performed. Please try again later.", WMSystem.DebugLevels.NoDebug)
                    HttpContext.Current.Response.Write("<p><font color=""#444444""><strong>The e-mail system check has already been performed. Please try again later.</strong></font></p>")
                Else
                    cammWebManager.Log.RuntimeInformation("e-mail system check has been performed. Receipient should be " & cammWebManager.TechnicalServiceEMailAccountName & " (" & cammWebManager.TechnicalServiceEMailAccountAddress & ")", WMSystem.DebugLevels.NoDebug)
                    Dim bufErrDetails As String = String.Empty
                    If cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress, "camm WebManager - Test of mail configuration", "If you receive this mail then there is no general malfunction of the e-mail system of camm Web-Manager." & vbNewLine & vbNewLine & "This mail has been sent by the ASP.NET engine from server " & cammWebManager.CurrentServerIdentString & " and the mailing system """ & cammWebManager.MessagingEMails.MailSystem.ToString & """", Nothing, cammWebManager.DevelopmentEMailAccountAddress, cammWebManager.DevelopmentEMailAccountAddress, CType(Nothing, Messaging.EMailAttachment()), CType(Nothing, Messaging.EMails.Priority), CType(Nothing, Messaging.EMails.Sensitivity), False, False, Nothing, Nothing, bufErrDetails) = True Then
                        HttpContext.Current.Response.Write("<p><font color=""green""><strong>The server has been configured for mail sending via SMTP successfully.</strong></font></p>")
                        HttpContext.Current.Response.Write("<p>" & cammWebManager.TechnicalServiceEMailAccountName & " (" & cammWebManager.TechnicalServiceEMailAccountAddress & ") should have got a test e-mail. If it is missing, then you should consult your SMTP server administrator and solve any routing errors.</p>")
                    Else
                        HttpContext.Current.Response.Write("<p><font color=""red""><strong>The server has detected an error while sending mail via SMTP.</strong></font></p>")
                        HttpContext.Current.Response.Write("<p>Reported error of mail component: <br><em>" & bufErrDetails.Replace(ControlChars.CrLf, "<br>").Replace(ControlChars.Cr, "<br>").Replace(ControlChars.Lf, "<br>") & "</em></p>")
                    End If
                    HttpContext.Current.Response.Write("<p>Used mail delivery system: """ & cammWebManager.MessagingEMails.MailSystem.ToString & """</p>")
                End If
            Else
                HttpContext.Current.Response.Write("<p><font color=""red""><strong>The server capabilities don't match the requirements to send e-mails.</font></p>")
                HttpContext.Current.Response.Write("<p>To solve this issue please consult the documentation.</font></p>")
            End If
            HttpContext.Current.Response.Write("</FONT></TD>" & vbNewLine)
            HttpContext.Current.Response.Write("</tr>" & vbNewLine)
            HttpContext.Current.Response.Write("</table>" & vbNewLine)
            HttpContext.Current.Response.Write("</BODY>" & vbNewLine)
            HttpContext.Current.Response.Write("</HTML>" & vbNewLine)

            HttpContext.Current.Cache.Add("WebManager.MailTest.JustDone", True, Nothing, System.Web.Caching.Cache.NoAbsoluteExpiration, New TimeSpan(0, 1, 0), Caching.CacheItemPriority.NotRemovable, Nothing)

        End Sub

    End Class

End Namespace