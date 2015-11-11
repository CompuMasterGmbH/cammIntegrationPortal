Option Explicit On 
Option Strict On

Imports System.Data
Imports System.Data.SqlClient
Imports System.Web
Imports CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs

Namespace CompuMaster.camm.WebManager.Pages.Specialized

#Region " Public Class StartPageWithoutFrameSet "
    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Specialized.StartPageWithoutFrameSet
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Handles referrer requests by session content or query string arguments
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	28.02.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Runtime.InteropServices.ComVisible(False)> Public Class StartPageWithoutFrameSet
        Inherits Page

        Protected FrameContentURL As String = ""

        Sub PageInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init

            'Referer
            If Request.QueryString("forceref") <> "" AndAlso Request.QueryString("forceref").IndexOf("/"c) = 0 Then
                FrameContentURL = Request.QueryString("forceref")
            ElseIf LCase(CType(Session("System_Referer"), String)) = LCase(Request.ServerVariables("SCRIPT_NAME")) Then
                'Falls Referer = dieses FrameSet
                FrameContentURL = Response.ApplyAppPathModifier("/sysdata/frames/frame_main.aspx?Lang=" & cammWebManager.UI.MarketID)
            ElseIf CType(Session("System_Referer"), String) <> "" Then
                'Falls anderer Referer
                FrameContentURL = CType(Session("System_Referer"), String)
            ElseIf Request.QueryString("referer") <> "" Then 'used in old defaults of account_register.aspx for launching userjustcreated.aspx
                'Falls anderer Referer
                FrameContentURL = Request.QueryString("referer")
            ElseIf Request.QueryString("ref") <> "" Then
                'Falls anderer Referer
                FrameContentURL = Request.QueryString("ref")
            Else
                'Wenn überhaupt kein Referer
                FrameContentURL = ""
            End If
            Session("System_Referer") = ""
            If cammWebManager.IsLoggedOn Then
                cammWebManager.System_SetSessionValue("System_Referer", "")
            End If

            'Redirect to destination page if required (redirect with 301 permenantly not allowed because client must redirect sometimes to another location as it already remembers by a previous request, so the client is not allowed to cache the redirection)
            If FrameContentURL <> Nothing Then
                If cammWebManager Is Nothing Then
                    Utils.RedirectTemporary(HttpContext.Current, FrameContentURL)
                Else
                    cammWebManager.RedirectTo(FrameContentURL)
                End If
            End If

        End Sub
    End Class

#End Region

#Region " Public Class StartPageForFrameSet "
    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Specialized.StartPageForFrameSet
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Handles referrer requests by session content or query string arguments
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	28.02.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Runtime.InteropServices.ComVisible(False)> Public Class StartPageForFrameSet
        Inherits Page

        Protected FrameContentURL As String = ""

        Sub PageInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init

            'Referer

            If Request.QueryString("forceref") <> "" AndAlso Request.QueryString("forceref").IndexOf("/"c) = 0 Then
                FrameContentURL = Request.QueryString("forceref")
            ElseIf LCase(CType(Session("System_Referer"), String)) = LCase(Request.ServerVariables("SCRIPT_NAME")) Then
                'Falls Referer = dieses FrameSet
                FrameContentURL = Response.ApplyAppPathModifier("/sysdata/frames/frame_main.aspx?Lang=" & cammWebManager.UI.MarketID)
            ElseIf CType(Session("System_Referer"), String) <> "" Then
                'Falls anderer Referer
                FrameContentURL = CType(Session("System_Referer"), String)
            ElseIf Request.QueryString("referer") <> "" Then 'used in old defaults of account_register.aspx for launching userjustcreated.aspx
                'Falls anderer Referer
                FrameContentURL = Request.QueryString("referer")
            ElseIf Request.QueryString("ref") <> "" Then
                'Falls anderer Referer
                FrameContentURL = Request.QueryString("ref")
            Else
                'Wenn überhaupt kein Referer
                FrameContentURL = Response.ApplyAppPathModifier("/sysdata/frames/frame_main.aspx?Lang=" & cammWebManager.UI.MarketID)
            End If
            Session("System_Referer") = ""
            If cammWebManager.IsLoggedOn Then
                cammWebManager.System_SetSessionValue("System_Referer", "")
            End If

        End Sub
    End Class

#End Region

#Region " Public Class ErrorPage "
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
                        .CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM Benutzer WHERE LoginName = @Username"
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
                        ErrorDetailDataFromServer4eMailAttachment = ErrorDetailDataFromServer4eMailAttachment & "<br>" & "Current login from: " & Utils.Nz(MyDebugRecSet("CurrentLoginViaRemoteIP"), "")
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
            Select Case ErrID
                Case ErrorWrongNetwork
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorWrongNetwork & "</font></p>"
                    DisplayLoginDenied = cammWebManager.Internationalization.AccessError_Descr_LoginDenied
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress, _
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorWrongNetwork & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorWrongNetwork & ErrorDetailDataFromServer4eMailAttachment & "</font>", _
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                    cammWebManager.ResetUserLoginName()
                Case ErrorCookiesMustNotBeDisabled
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorCookiesMustNotBeDisabled & "</font></p>"
                    DisplayLoginDenied = cammWebManager.Internationalization.AccessError_Descr_LoginDenied
                    'Do not send unnecessary e-mail warnings when the client is a crawler like "Yahoo! Slurp" or others
                    If cammWebManager.System_DebugLevel >= 2 OrElse (Utils.IsRequestFromCrawlerAgent(Me.Request) = False AndAlso cammWebManager.System_DebugLevel >= 1) Then 'Debug level for crawlers >=2 or normal browsers >=1
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress, _
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorCookiesMustNotBeDisabled & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorCookiesMustNotBeDisabled & ErrorDetailDataFromServer4eMailAttachment & "</font>", _
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                    cammWebManager.ResetUserLoginName()
                Case ErrorServerConfigurationError
                    Dim CustomErrorMessage As String
                    CustomErrorMessage = cammWebManager.System_DebugServerConnectivity()
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorServerConfigurationError & CustomErrorMessage & "</font></p>"
                    DisplayLoginDenied = cammWebManager.Internationalization.AccessError_Descr_LoginDenied
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress, _
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorServerConfigurationError & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorServerConfigurationError & CustomErrorMessage & ErrorDetailDataFromServer4eMailAttachment & "</font>", _
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                    cammWebManager.ResetUserLoginName()
                Case ErrorNoAuthorization
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorNoAuthorization & "</font></p>"
                    DisplayLoginDenied = cammWebManager.Internationalization.AccessError_Descr_LoginDenied
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress, _
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorNoAuthorization & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorNoAuthorization & ErrorDetailDataFromServer4eMailAttachment & "</font>", _
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                Case ErrorAlreadyLoggedOn
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.ErrorAlreadyLoggedOn, cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString(), "AreaSecurityContactEMail"), cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString(), "AreaSecurityContactEMail"), "", "", "", "", "", "", "", "", "", "") & "</font></p>"
                    DisplayLoginDenied = cammWebManager.Internationalization.AccessError_Descr_LoginDenied
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress, _
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorAlreadyLoggedOn & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.ErrorAlreadyLoggedOn, cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString(), "AreaSecurityContactEMail"), cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString(), "AreaSecurityContactEMail"), "", "", "", "", "", "", "", "", "", "") & ErrorDetailDataFromServer4eMailAttachment & "</font>", _
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                    cammWebManager.ResetUserLoginName()
                Case ErrorLoggedOutBecauseLoggedOnAtAnotherMachine
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorLoggedOutBecauseLoggedOnAtAnotherMachine & "</font></p>"
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress, _
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorLoggedOutBecauseLoggedOnAtAnotherMachine & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorLoggedOutBecauseLoggedOnAtAnotherMachine & vbNewLine & "Userlogin: " & cammWebManager.CurrentUserLoginName & ErrorDetailDataFromServer4eMailAttachment & "</font>", _
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                    cammWebManager.ResetUserLoginName()
                Case ErrorLogonFailedTooOften
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorLogonFailedTooOften & "</font></p>"
                    DisplayLoginDenied = cammWebManager.Internationalization.AccessError_Descr_LoginDenied
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress, _
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorLogonFailedTooOften & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorLogonFailedTooOften & ErrorDetailDataFromServer4eMailAttachment & "</font>", _
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                    cammWebManager.ResetUserLoginName()
                Case ErrorSendPWWrongLoginOrEmailAddress
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorSendPWWrongLoginOrEmailAddress & "</font></p>"
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress, _
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorSendPWWrongLoginOrEmailAddress & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorSendPWWrongLoginOrEmailAddress & ErrorDetailDataFromServer4eMailAttachment & "</font>", _
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                    cammWebManager.ResetUserLoginName()
                Case ErrorApplicationConfigurationIsEmpty
                    DisplayMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorApplicationConfigurationIsEmpty & "</font></p>"
                    If cammWebManager.System_DebugLevel >= 1 Then
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress, _
                            "camm WebManager - Error or warning detected", cammWebManager.Internationalization.ErrorApplicationConfigurationIsEmpty & ErrorDetailDataFromServer4eMailTextAttachment, "<font face=""Arial"">" & cammWebManager.Internationalization.ErrorApplicationConfigurationIsEmpty & ErrorDetailDataFromServer4eMailAttachment & "</font>", _
                            cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                    End If
                Case ErrorUndefined
                    If cammWebManager.System_DebugLevel >= 1 Then
                        DisplayMessage = "Undefined error found - the webmaster has been informed."
                        cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress, _
                            "camm WebManager - Error or warning detected", ErrorDetailDataFromServer4eMailAttachment, "<font face=""Arial"">" & ErrorDetailDataFromServer4eMailAttachment & "</font>", _
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
#End Region

#Region " Public Class DownloadFileByDownloadHandler "
    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Specialized.DownloadFileByDownloadHandler
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Send a binary file from download handler to the browser
    ''' </summary>
    ''' <remarks>
    '''     Per default, all size limits will be ignored since the CPU time or file system space has already been involved and can't be saved any more. So, since the file has already been created, it should be send now without any hassles.
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	27.01.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Runtime.InteropServices.ComVisible(False)> Public Class DownloadFileByDownloadHandler
        Inherits Page

        Private _IgnoreSizeLimits As Boolean = True
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Ignore file size limits
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	15.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IgnoreSizeLimits() As Boolean
            Get
                Return _IgnoreSizeLimits
            End Get
            Set(ByVal Value As Boolean)
                _IgnoreSizeLimits = Value
            End Set
        End Property

        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

            If _IgnoreSizeLimits Then
                cammWebManager.DownloadHandler.MaxDownloadCollectionSize = Long.MaxValue
                cammWebManager.DownloadHandler.MaxDownloadSize = Long.MaxValue
            End If

            If HttpContext.Current.Request.QueryString("fid") <> "" Then
                'Identify required file by file ID (requires a lookup in download handler's database)
                Log.WriteEventLogTrace("CWM DH: DownloadFileByID")
                cammWebManager.DownloadHandler.DownloadFileByID(HttpContext.Current.Request.QueryString("fid"))
            ElseIf HttpContext.Current.Request.QueryString("fpath") <> "" Then
                'Identify required file by its path (only available for files in cache (because for the cache are no security demands)
                Log.WriteEventLogTrace("CWM DH: DownloadFileByPath")
                cammWebManager.DownloadHandler.DownloadFileByPath(HttpContext.Current.Request.QueryString("fpath"))
            ElseIf HttpContext.Current.Request.QueryString("cat") <> "" AndAlso HttpContext.Current.Request.QueryString("dataid") <> "" Then
                Log.WriteEventLogTrace("CWM DH: DownloadFileFromCache")
                cammWebManager.DownloadHandler.DownloadFileFromCache(HttpContext.Current.Request.QueryString("cat"), HttpContext.Current.Request.QueryString("dataid"))
            End If

        End Sub

    End Class

#End Region

End Namespace