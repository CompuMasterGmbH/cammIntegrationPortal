'Copyright 2004,2005,2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'Alternatively, the camm Integration Portal (or camm Web-Manager) can be licensed for closed-source / commercial projects from CompuMaster GmbH, <http://www.camm.biz/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie k�nnen es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder sp�teren ver�ffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es n�tzlich sein wird, aber OHNE JEDE GEW�HRLEISTUNG, bereitgestellt; sogar ohne die implizite Gew�hrleistung der MARKTF�HIGKEIT oder EIGNUNG F�R EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License f�r weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
'Alternativ kann camm Integration Portal (oder camm Web-Manager) lizenziert werden f�r Closed-Source / kommerzielle Projekte von  CompuMaster GmbH, <http://www.camm.biz/>.

Option Explicit On
Option Strict On

Imports System.Web

Namespace CompuMaster.camm.WebManager.Pages.Checks

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