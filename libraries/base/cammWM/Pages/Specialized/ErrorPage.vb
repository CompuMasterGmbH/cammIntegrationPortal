'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'Alternatively, the camm Integration Portal (or camm Web-Manager) can be licensed for closed-source / commercial projects from CompuMaster GmbH, <http://www.camm.biz/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie können es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren veröffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es nützlich sein wird, aber OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License für weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
'Alternativ kann camm Integration Portal (oder camm Web-Manager) lizenziert werden für Closed-Source / kommerzielle Projekte von  CompuMaster GmbH, <http://www.camm.biz/>.

Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs

Namespace CompuMaster.camm.WebManager.Pages.Specialized

    <System.Runtime.InteropServices.ComVisible(False)> Public Class ErrorPage
        Inherits Page

        Private Function GetMailContent(ByVal DisplayMessage As String) As String
            Dim ErrorDetailDataFromServer4eMailAttachment As String = Nothing
            Dim MyDBConn As New SqlConnection
            Dim MyDebugRecSet As SqlDataReader = Nothing
            Dim MyCmd As New SqlCommand
            Try
                MyDBConn.ConnectionString = cammWebManager.ConnectionString
                MyDBConn.Open()
                Try
                    With MyCmd
                        .CommandText = "SELECT * FROM Benutzer WHERE LoginName = @Username"
                        .CommandType = CommandType.Text
                    End With
                    MyCmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = cammWebManager.CurrentUserLoginName
                    MyCmd.Connection = MyDBConn
                    MyDebugRecSet = MyCmd.ExecuteReader()
                    Dim UserDataRead As Boolean = MyDebugRecSet.Read()

                    'Implements
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "<br>" & "<hl>"
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "<b>Error description:</b>"
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & DisplayMessage
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "<br>" & "<hl>"
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "<b>" & "Environmental information:</b>"
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Session.SessionID: " & Session.SessionID
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Session.Cwm.SessionID: " & cammWebManager.CurrentScriptEngineSessionID
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Session.Username: " & cammWebManager.CurrentUserLoginName
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Request.QueryString.Username: " & HttpContext.Current.Request.QueryString("User")
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Session.LogonBufferUsername: " & CType(Session("System_Logon_Buffer_Username"), String)
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Referer: " & HttpContext.Current.Request.ServerVariables("HTTP_REFERER")
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Browser: " & HttpContext.Current.Request.UserAgent
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Cookies: " & HttpContext.Current.Request.ServerVariables("HTTP_COOKIE")
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "AcceptLanguage: " & HttpContext.Current.Request.ServerVariables("HTTP_ACCEPT_LANGUAGE")
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Via (Proxy): " & HttpContext.Current.Request.ServerVariables("HTTP_VIA")
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "SSL: " & HttpContext.Current.Request.ServerVariables("HTTPS")
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "RemoteAddress: " & Utils.LookupRealRemoteClientIPOfHttpContext(HttpContext.Current)
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "RemoteHost: " & HttpContext.Current.Request.UserHostName
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "RemoteUser: " & HttpContext.Current.Request.ServerVariables("REMOTE_USER")
                    If UserDataRead Then
                        ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Last login from: " & Utils.Nz(MyDebugRecSet("LastLoginViaRemoteIP"), "")
                        ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Last login on: " & Utils.Nz(MyDebugRecSet("LastLoginOn"), "")
                    End If
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Current server date/time: " & Now()
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "ServerAddress: " & cammWebManager.CurrentServerIdentString
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "ServerName: " & HttpContext.Current.Request.ServerVariables("SERVER_NAME")
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "ServerPort: " & HttpContext.Current.Request.ServerVariables("SERVER_PORT")
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "ScriptName: " & HttpContext.Current.Request.ServerVariables("SCRIPT_NAME")
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "QueryString: <ul>" & Utils.JoinNameValueCollectionToString(HttpContext.Current.Request.QueryString, "<li>", "=", "</li>") & "</ul>"
                    If UserDataRead Then
                        ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Account Accessability: " & Utils.Nz(MyDebugRecSet("AccountAccessability"), "")
                        ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Login locked till: " & Utils.Nz(MyDebugRecSet("LoginLockedTill"), "")
                        ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Login disabled: " & Utils.Nz(MyDebugRecSet("LoginDisabled"), "")
                        ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Login failures: " & Utils.Nz(MyDebugRecSet("LoginFailures"), "")
                        ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "User e-mail address: " & Utils.Nz(MyDebugRecSet("E-MAIL"), "")
                        ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "User first name: " & Utils.Nz(MyDebugRecSet("Vorname"), "")
                        ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "User family name addition: " & Utils.Nz(MyDebugRecSet("Namenszusatz"), "")
                        ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "User family name: " & Utils.Nz(MyDebugRecSet("Nachname"), "")
                        ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "User company: " & Utils.Nz(MyDebugRecSet("Company"), "")
                    End If

                Catch ex As Exception
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "<br>" & "<hl>"
                    ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Debug recordset access error: " & CType(IIf(cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation, ex.ToString, ex.Message), String)
                End Try
            Catch ex As Exception
                ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "<br>" & "<hl>"
                ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Debug connection access error: " & CType(IIf(cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation, ex.ToString, ex.Message), String)
            Finally
                If Not MyDebugRecSet Is Nothing AndAlso MyDebugRecSet.IsClosed Then
                    MyDebugRecSet.Close()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyDBConn Is Nothing Then
                    If MyDBConn.State <> ConnectionState.Closed Then
                        MyDBConn.Close()
                    End If
                    MyDBConn.Dispose()
                End If
            End Try
            ErrorDetailDataFromServer4eMailAttachment &= "<p><small><small>camm Web-Manager V" & Me.cammWebManager.System_Version_Ex.ToString & "</small></small></p>"
            ErrorDetailDataFromServer4eMailAttachment &= "</font>"

            Return ErrorDetailDataFromServer4eMailAttachment

        End Function

        Protected DisplayLoginDenied As String, DisplayMessage As String, HideLogonAnchor As Boolean

        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

            Me.cammWebManager.SafeMode = False

            Server.ScriptTimeout = 180

            DisplayMessage = Request.QueryString("ErrCode")
            Dim ErrorDetailDataFromServer4eMailAttachment As String = GetMailContent(DisplayMessage)
            Dim ErrorDetailDataFromServer4eMailTextAttachment As String = CompuMaster.camm.WebManager.Utils.ConvertHTMLToText(ErrorDetailDataFromServer4eMailAttachment)
            Dim ErrID As CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs

            ErrID = CType(Request.QueryString("ErrID"), CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs)
#If VS2015OrHigher = True Then
#Disable Warning BC40000 'disable obsolete warnings because this code must be compatible to .NET 1.1
#End If
            Select Case ErrID
                Case ErrorWrongNetwork
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorWrongNetwork & "</font></p>"
                    DisplayLoginDenied = cammWebManager.Internationalization.AccessError_Descr_LoginDenied
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress,
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorWrongNetwork & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorWrongNetwork & ErrorDetailDataFromServer4eMailAttachment & "</font>",
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                    cammWebManager.ResetUserLoginName()
                Case ErrorCookiesMustNotBeDisabled
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorCookiesMustNotBeDisabled & "</font></p>"
                    DisplayLoginDenied = cammWebManager.Internationalization.AccessError_Descr_LoginDenied
                    'Do not send unnecessary e-mail warnings when the client is a crawler like "Yahoo! Slurp" or others
                    If cammWebManager.System_DebugLevel >= 2 OrElse (Utils.IsRequestFromCrawlerAgent(Me.Request) = False AndAlso cammWebManager.System_DebugLevel >= 1) Then 'Debug level for crawlers >=2 or normal browsers >=1
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress,
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorCookiesMustNotBeDisabled & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorCookiesMustNotBeDisabled & ErrorDetailDataFromServer4eMailAttachment & "</font>",
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                    cammWebManager.ResetUserLoginName()
                Case ErrorServerConfigurationError
                    Dim CustomErrorMessage As String
                    CustomErrorMessage = cammWebManager.System_DebugServerConnectivity()
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorServerConfigurationError & CustomErrorMessage & "</font></p>"
                    DisplayLoginDenied = cammWebManager.Internationalization.AccessError_Descr_LoginDenied
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress,
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorServerConfigurationError & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorServerConfigurationError & CustomErrorMessage & ErrorDetailDataFromServer4eMailAttachment & "</font>",
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                    cammWebManager.ResetUserLoginName()
                Case ErrorNoAuthorization
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorNoAuthorization & "</font></p>"
                    DisplayLoginDenied = cammWebManager.Internationalization.AccessError_Descr_LoginDenied
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress,
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorNoAuthorization & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorNoAuthorization & ErrorDetailDataFromServer4eMailAttachment & "</font>",
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                Case ErrorAlreadyLoggedOn
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.ErrorAlreadyLoggedOn, cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString(), "AreaSecurityContactEMail"), cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString(), "AreaSecurityContactEMail"), "", "", "", "", "", "", "", "", "", "") & "</font></p>"
                    DisplayLoginDenied = cammWebManager.Internationalization.AccessError_Descr_LoginDenied
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress,
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorAlreadyLoggedOn & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.ErrorAlreadyLoggedOn, cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString(), "AreaSecurityContactEMail"), cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString(), "AreaSecurityContactEMail"), "", "", "", "", "", "", "", "", "", "") & ErrorDetailDataFromServer4eMailAttachment & "</font>",
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                    cammWebManager.ResetUserLoginName()
                Case ErrorLoggedOutBecauseLoggedOnAtAnotherMachine
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorLoggedOutBecauseLoggedOnAtAnotherMachine & "</font></p>"
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress,
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorLoggedOutBecauseLoggedOnAtAnotherMachine & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorLoggedOutBecauseLoggedOnAtAnotherMachine & vbNewLine & "Userlogin: " & cammWebManager.CurrentUserLoginName & ErrorDetailDataFromServer4eMailAttachment & "</font>",
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                    cammWebManager.ResetUserLoginName()
                Case ErrorLogonFailedTooOften
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorLogonFailedTooOften & "</font></p>"
                    DisplayLoginDenied = cammWebManager.Internationalization.AccessError_Descr_LoginDenied
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress,
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorLogonFailedTooOften & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorLogonFailedTooOften & ErrorDetailDataFromServer4eMailAttachment & "</font>",
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                    cammWebManager.ResetUserLoginName()
                Case ErrorSendPWWrongLoginOrEmailAddress
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorSendPWWrongLoginOrEmailAddress & "</font></p>"
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress,
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorSendPWWrongLoginOrEmailAddress & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorSendPWWrongLoginOrEmailAddress & ErrorDetailDataFromServer4eMailAttachment & "</font>",
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                    cammWebManager.ResetUserLoginName()
                Case ErrorApplicationConfigurationIsEmpty
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorApplicationConfigurationIsEmpty & "</font></p>"
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress,
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorApplicationConfigurationIsEmpty & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorApplicationConfigurationIsEmpty & ErrorDetailDataFromServer4eMailAttachment & "</font>",
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                Case ErrorUndefined
                    If cammWebManager.System_DebugLevel >= 1 Then
                        DisplayMessage = "Undefined error found - the webmaster has been informed."
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress,
                            "camm WebManager - Error or warning detected", ErrorDetailDataFromServer4eMailAttachment, "<font face=""Arial"">" & ErrorDetailDataFromServer4eMailAttachment & "</font>",
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    Else
                        DisplayMessage = "Undefined error found. Please notify our webmaster."
                    End If
                Case Else 'Unknown error or login required
                    If DisplayMessage = "" Then
                        cammWebManager.RedirectTo(cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL)
                    Else
                        HideLogonAnchor = True
                    End If
            End Select
#If VS2015OrHigher = True Then
#Enable Warning BC40000 'disable obsolete warnings because this code must be compatible to .NET 1.1
#End If

            cammWebManager.PageTitle = "Error"

            ''Alternative error output
            'Response.Write("<html><head><title>" & cammWebManager.PageTitle & "</title></head><body><font face=""Arial""><h2>Error</h2><code>")
            'Response.Write(CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(DisplayMessage))
            'Response.Write("</code></font></body></html>")
            'Response.End()

        End Sub

        Protected Overrides Sub OnError(ByVal e As System.EventArgs)
            Response.Write("Error found while processing error page")
        End Sub

    End Class

End Namespace