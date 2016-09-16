'Copyright 2001-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager

    ''' <summary>
    '''     Application classes, pages, controls and modules of camm Web-Manager
    ''' </summary>
    ''' <remarks>
    '''     <para>
    '''         The basic API of camm Web-Manager is implemented in <see cref="T:CompuMaster.camm.WebManager.WMSystem">CompuMaster.camm.WebManager.WMSystem</see>. Every cammWebManager object on your page will provide these interfaces. The recommendation is to use <see cref="T:CompuMaster.camm.WebManager.Pages.Page">CompuMaster.camm.WebManager.Pages.Page</see> instead of the normal System.Web.UI.Page if you like to access the cammWebManager property from your code-behind source code.
    '''     </para>
    '''     <para>For a fully functional camm Web-Manager, your webserver's user account needs write access to following directories
    '''         <list>
    '''             <item><code>/sysdata/</code> for a working web editor in your website's editorial, copyright and data protection page</item>
    '''             <item><code>/system/downloads/</code> for a fully featured download handler which provides advanced technologies to provide dynamic download files</item>
    '''         </list>
    '''     </para>
    ''' </remarks>
    Friend Class NamespaceDoc

    End Class



    ''' <summary>
    '''     camm Web-Manager base class
    ''' </summary>
    ''' <remarks>
    '''     <p>This is the base class of camm Web-Manager. Instantiate this one in your ASP.NET pages by using ~/system/cammWebManager.ascx</p>
    ''' </remarks>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class WMSystem
        Inherits System.Web.UI.UserControl
        Implements IWebManager

#Region "Standard WebManager enumerators and definitions"

        ''' <summary>
        '''     User interface related properties and methods
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property UI() As CompuMaster.camm.WebManager.UserInterface Implements IWebManager.UI
            Get
                Static _UI As CompuMaster.camm.WebManager.UserInterface
                If _UI Is Nothing Then
                    _UI = New CompuMaster.camm.WebManager.UserInterface(Me)
                End If
                Return _UI
            End Get
        End Property

        ''' <summary>
        '''     The download handler provides a secure, powerfull and resource saving possibility for sending files or data to a client
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property DownloadHandler() As DownloadHandler
            Get
                Static _DownloadHandler As DownloadHandler
                If _DownloadHandler Is Nothing Then
                    _DownloadHandler = New DownloadHandler(Me)
                End If
                Return _DownloadHandler
            End Get
        End Property

        Private _PasswordSecurity As WMPasswordSecurity
        ''' <summary>
        ''' Array of WMPasswordSecurity classes needed to set up password policies for each separate access level
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Each separate access level can get its own password policy. For example, users with internal access need at least 8 characters, but external access only users only need 3 characters in their passwords.</remarks>
        Public Property PasswordSecurity() As WMPasswordSecurity
            Get
                If _PasswordSecurity Is Nothing Then
                    _PasswordSecurity = New WMPasswordSecurity(Me)
                End If
                Return _PasswordSecurity
            End Get
            Set(ByVal Value As WMPasswordSecurity)
                _PasswordSecurity = Value
            End Set
        End Property

        ''' <summary>
        ''' Environmental information, e. g. product details, licence
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public ReadOnly Property Environment() As CompuMaster.camm.WebManager.Environment
            Get
                Static _Environment As CompuMaster.camm.WebManager.Environment
                If _Environment Is Nothing Then _Environment = New Environment(Me)
                Return _Environment
            End Get
        End Property

        ''' <summary>
        '''     Licence details of the camm Web-Manager instance
        ''' </summary>
        ''' <value></value>
        <Obsolete("Use Environment.Licence")> Public ReadOnly Property Licence() As Licence
            Get
                Return Environment.Licence
            End Get
        End Property

        ''' <summary>
        ''' Product name of camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Product name could be for example "camm Enterprise WebManager"</remarks>
        <Obsolete("Use Environment.ProductName")> Public ReadOnly Property System_ProductName() As String
            Get
                Return Environment.ProductName
            End Get
        End Property
        ''' <summary>
        ''' Licence hash code for camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        <Obsolete("Use Environment.LicenceKey")> Public ReadOnly Property System_Licence() As String
            Get
                Return Environment.LicenceKey
            End Get
        End Property

        ''' <summary>
        ''' Debug level of camm Web-Manager
        ''' </summary>
        ''' <value>The new debug level</value>
        ''' <remarks>The logging of warnings, errors can be influenced by this property as well as redirection of e-mails and the functionality level of automatic redirects. This is very helpfull on first setup of camm Web-Manager if you experience any problems or you don't want e-mails to be sent to the origin receipients except you yourself.</remarks>
        Public Property System_DebugLevel() As DebugLevels
            Get
                If _System_DebugLevel = Nothing Then
                    _System_DebugLevel = CompuMaster.camm.WebManager.WMSystem.Configuration.DebugLevel
                End If
                Return _System_DebugLevel
            End Get
            Set(ByVal Value As DebugLevels)
                _System_DebugLevel = Value
            End Set
        End Property
        ''' <summary>
        ''' Debug level of camm Web-Manager
        ''' </summary>
        ''' <value>The new debug level</value>
        ''' <remarks>The logging of warnings, errors can be influenced by this property as well as redirection of e-mails and the functionality level of automatic redirects. This is very helpfull on first setup of camm Web-Manager if you experience any problems or you don't want e-mails to be sent to the origin receipients except you yourself.</remarks>
        Public Property DebugLevel() As DebugLevels
            Get
                If _System_DebugLevel = Nothing Then
                    _System_DebugLevel = CompuMaster.camm.WebManager.WMSystem.Configuration.DebugLevel
                End If
                Return _System_DebugLevel
            End Get
            Set(ByVal Value As DebugLevels)
                _System_DebugLevel = Value
            End Set
        End Property

        ''' <summary>
        ''' The internationalization class contains all common strings and functions for proper internationalization
        ''' </summary>
        ''' <value></value>
        ''' <remarks>You are able to update the values or to extend the values in the file ~/sysdata/custom_internationalization.vb</remarks>
        Public Property Internationalization() As WMSettingsAndData
            Get
                If _Internationalization Is Nothing Then
                    _Internationalization = New WMSettingsAndData
                End If
                Return _Internationalization
            End Get
            Set(ByVal Value As WMSettingsAndData)
                _Internationalization = Value
            End Set
        End Property

        Private _Statistics As WMStatistics
        ''' <summary>
        ''' The statistics class provides methods to identify common figures
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property Statistics() As WMStatistics
            Get
                If _Statistics Is Nothing Then
                    _Statistics = New WMStatistics(Me)
                End If
                Return _Statistics
            End Get
            Set(ByVal Value As WMStatistics)
                _Statistics = Value
            End Set
        End Property

        ''' <summary>
        ''' Creates the notifications for all default events of camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        <Obsolete("Use property Notifications instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Property DefaultNotifications() As WMNotifications
            Get
                If _DefaultNotifications Is Nothing Then
                    _DefaultNotifications = New WMNotifications(Me)
                End If
                Return CType(_DefaultNotifications, WMNotifications)
            End Get
            Set(ByVal Value As WMNotifications)
                _DefaultNotifications = Value
            End Set
        End Property
        ''' <summary>
        ''' Creates the notifications for all default events of camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property Notifications() As Notifications.INotifications
            Get
                If _DefaultNotifications Is Nothing Then
                    _DefaultNotifications = New CompuMaster.camm.WebManager.Notifications.DefaultNotifications(Me)
                End If
                Return _DefaultNotifications
            End Get
            Set(ByVal Value As Notifications.INotifications)
                _DefaultNotifications = Value
            End Set
        End Property

        ''' <summary>
        ''' Which features are supported by the current instance of camm Web-Manager?
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Each instance is set up separately. Some ones support the sending of SMS messages because the gateways are configured, other instances haven't got prepared to send SMS messages. That's why you can and should ask the current configuration here.</remarks>
        Public Property IsSupported() As WMCapabilities
            Get
                If _IsSupported Is Nothing Then
                    _IsSupported = New WMCapabilities(Me)
                End If
                Return _IsSupported
            End Get
            Set(ByVal Value As WMCapabilities)
                _IsSupported = Value
            End Set
        End Property
        ''' <summary>
        ''' Is there a logon of an user or are we anynoumsly visiting the web site?
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public ReadOnly Property IsLoggedOn() As Boolean
            Get
                Return (Me.CurrentUserLoginName <> Nothing)
            End Get
        End Property
        ''' <summary>
        ''' The page title of the current page can be modified here
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property PageTitle() As String
            Get
                Return _PageTitle
            End Get
            Set(ByVal Value As String)
                _PageTitle = Value
            End Set
        End Property
        ''' <summary>
        ''' Configures additional META NAME tags
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property PageMETA_Name() As Collections.Specialized.NameValueCollection
            Get
                Return _PageMETA_Name
            End Get
            Set(ByVal Value As Collections.Specialized.NameValueCollection)
                _PageMETA_Name = Value
            End Set
        End Property
        ''' <summary>
        ''' Configures additional META HTTP-EQUIV tags
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property PageMETA_HttpEquiv() As Collections.Specialized.NameValueCollection
            Get
                Return _PageMETA_HttpEquiv
            End Get
            Set(ByVal Value As Collections.Specialized.NameValueCollection)
                _PageMETA_HttpEquiv = Value
            End Set
        End Property
        ''' <summary>
        ''' Configures additional tags inside of the HEAD tag
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property PageAdditionalHeaders() As String
            Get
                Return _PageAdditionalHeaders
            End Get
            Set(ByVal Value As String)
                _PageAdditionalHeaders = Value
            End Set
        End Property
        ''' <summary>
        ''' Configures additional attributes inside of the BODY tag
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property PageAdditionalBodyAttributes() As Collections.Specialized.NameValueCollection
            Get
                Return _PageAdditionalBodyAttributes
            End Get
            Set(ByVal Value As Collections.Specialized.NameValueCollection)
                _PageAdditionalBodyAttributes = Value
            End Set
        End Property

        Private _AdditionalBodyTags As String = ""
        Private _SMTPAuthType As String = ""
        Private _SMTPUserName As String = ""
        Private _SMTPPassword As String = ""
        Private _SMTPServer As String = ""
        Private _SMTPServerPort As Integer = Nothing
        ''' <summary>
        ''' Do not use! Subject of removal in v3.x!
        ''' </summary>
        ''' <remarks></remarks>
        <Obsolete(), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Property AdditionalBodyTags() As String
            Get
                Return _AdditionalBodyTags
            End Get
            Set(ByVal Value As String)
                _AdditionalBodyTags = Value
            End Set
        End Property
        ''' <summary>
        ''' Do not use! Subject of getting private in v3.x!
        ''' </summary>
        ''' <remarks></remarks>
        <Obsolete(), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Property User_Auth_Config_CurSMTPServer() As String
            Get
                If _SMTPServer = Nothing Then
                    _SMTPServer = CompuMaster.camm.WebManager.WMSystem.Configuration.SmtpServerName
                End If
                Return _SMTPServer
            End Get
            Set(ByVal Value As String)
                _SMTPServer = Value
            End Set
        End Property
        ''' <summary>
        ''' Do not use! Subject of getting private in v3.x!
        ''' </summary>
        ''' <remarks></remarks>
        Public Property User_Auth_Config_CurSMTPServer_Port() As Integer
            Get
                If _SMTPServerPort = Nothing Then
                    _SMTPServerPort = CompuMaster.camm.WebManager.WMSystem.Configuration.SmtpServerPort
                End If
                Return _SMTPServerPort
            End Get
            Set(ByVal Value As Integer)
                _SMTPServerPort = Value
            End Set
        End Property

        Private _UserCloneExludedAdditionalFlags As String()
        ''' <summary>
        ''' The Names (Keys) of Additional-Flags to exclude from cloned user
        ''' </summary>
        ''' <remarks></remarks>
        Public Property UserCloneExludedAdditionalFlags() As String()
            Get
                If _UserCloneExludedAdditionalFlags Is Nothing Then
                    _UserCloneExludedAdditionalFlags = CompuMaster.camm.WebManager.WMSystem.Configuration.UserCloneExludedAdditionalFlags
                End If
                Return _UserCloneExludedAdditionalFlags
            End Get
            Set(ByVal Value As String())
                _UserCloneExludedAdditionalFlags = Value
            End Set
        End Property


        ''' <summary>
        ''' The e-mail address of the developer is used when errors shall be reported
        ''' </summary>
        ''' <value>Valid e-mail address</value>
        ''' <remarks></remarks>
        Public Property DevelopmentEMailAccountAddress() As String
            Get
                If _DevelopmentEMailAccountAddress = Nothing Then
                    _DevelopmentEMailAccountAddress = CompuMaster.camm.WebManager.WMSystem.Configuration.DevelopmentEMailAccountAddress
                End If
                Return _DevelopmentEMailAccountAddress
            End Get
            Set(ByVal Value As String)
                _DevelopmentEMailAccountAddress = Value
            End Set
        End Property
        ''' <summary>
        ''' The common title of the 1st level support team
        ''' </summary>
        ''' <value>The public title for the 1st level support team contact address</value>
        ''' <remarks>The e-mail address is used for all common activities inside of the camm WebManager world, for example a general contact for feedback or support requests. Mostly, the security administrators get all those e-mail. Depending of the content, they can answer directly or redirect to a 2nd level support team.</remarks>
        Public Property StandardEMailAccountName() As String
            Get
                If _StandardEMailAccountName = Nothing Then
                    _StandardEMailAccountName = CompuMaster.camm.WebManager.WMSystem.Configuration.StandardEMailAccountName
                End If
                Return _StandardEMailAccountName
            End Get
            Set(ByVal Value As String)
                _StandardEMailAccountName = Value
            End Set
        End Property
        ''' <summary>
        ''' The common e-mail address of the 1st level support team
        ''' </summary>
        ''' <value>Valid e-mail address</value>
        ''' <remarks>The e-mail address is used for all common activities inside of the camm WebManager world, for example a general contact for feedback or support requests. Mostly, the security administrators get all those e-mail. Depending of the content, they can answer directly or redirect to a 2nd level support team.</remarks>
        Public Property StandardEMailAccountAddress() As String
            Get
                If _StandardEMailAccountAddress = Nothing Then
                    _StandardEMailAccountAddress = CompuMaster.camm.WebManager.WMSystem.Configuration.StandardEMailAccountAddress
                End If
                Return _StandardEMailAccountAddress
            End Get
            Set(ByVal Value As String)
                _StandardEMailAccountAddress = Value
            End Set
        End Property
        ''' <summary>
        ''' The common e-mail address of the technical support team
        ''' </summary>
        ''' <value>The public title for the techical support team contact address</value>
        ''' <remarks>The e-mail address is used for all technical activities.</remarks>
        Public Property TechnicalServiceEMailAccountName() As String
            Get
                If _TechnicalServiceEMailAccountName = Nothing Then
                    _TechnicalServiceEMailAccountName = CompuMaster.camm.WebManager.WMSystem.Configuration.TechnicalServiceEMailAccountName
                End If
                Return _TechnicalServiceEMailAccountName
            End Get
            Set(ByVal Value As String)
                _TechnicalServiceEMailAccountName = Value
            End Set
        End Property
        ''' <summary>
        ''' The common e-mail address of the technical support team
        ''' </summary>
        ''' <value>Valid e-mail address</value>
        ''' <remarks>The e-mail address is used for all technical activities.</remarks>
        Public Property TechnicalServiceEMailAccountAddress() As String
            Get
                If _TechnicalServiceEMailAccountAddress = Nothing Then
                    _TechnicalServiceEMailAccountAddress = CompuMaster.camm.WebManager.WMSystem.Configuration.TechnicalServiceEMailAccountAddress
                End If
                Return _TechnicalServiceEMailAccountAddress
            End Get
            Set(ByVal Value As String)
                _TechnicalServiceEMailAccountAddress = Value
            End Set
        End Property
        ''' <summary>
        ''' Sets up the duration the security cache is used without refresh from the database
        ''' </summary>
        ''' <value></value>
        ''' <remarks>The security queries can be cached for performace issues, but the cache could contain some old data.</remarks>
        Public Property System_SecurityQueryCache_MaxAgeInSeconds() As Integer
            Get
                Return _System_SecurityQueryCache_MaxAgeInSeconds
            End Get
            Set(ByVal Value As Integer)
                _System_SecurityQueryCache_MaxAgeInSeconds = Value
            End Set
        End Property
        ''' <summary>
        ''' The SMTP server name for sending e-mails
        ''' </summary>
        ''' <value></value>
        ''' <remarks>The camm Web-Manager is heavily using e-mails for information and workflow purposes.</remarks>
        Public Property SMTPServerName() As String
            Get
                If _SMTPServer = Nothing Then
                    _SMTPServer = CompuMaster.camm.WebManager.WMSystem.Configuration.SmtpServerName
                End If
                Return _SMTPServer
            End Get
            Set(ByVal Value As String)
                _SMTPServer = Value
            End Set
        End Property
        ''' <summary>
        ''' The SMTP server port for sending e-mails
        ''' </summary>
        ''' <value></value>
        ''' <remarks>The camm Web-Manager is heavily using e-mails for information and workflow purposes.</remarks>
        Public Property SMTPServerPort() As Integer
            Get
                If _SMTPServerPort = Nothing Then
                    _SMTPServerPort = CompuMaster.camm.WebManager.WMSystem.Configuration.SmtpServerPort
                End If
                Return _SMTPServerPort
            End Get
            Set(ByVal Value As Integer)
                _SMTPServerPort = Value
            End Set
        End Property

        ''' <summary>
        '''     Sets up an user name for the SMTP server
        ''' </summary>
        ''' <value>The user name</value>
        Public Property SMTPUserName() As String
            Get
                If _SMTPUserName = Nothing Then
                    _SMTPUserName = CompuMaster.camm.WebManager.WMSystem.Configuration.SmtpUsername
                End If
                Return _SMTPUserName
            End Get
            Set(ByVal Value As String)
                _SMTPUserName = Value
            End Set
        End Property

        ''' <summary>
        '''     Sets up a password for the SMTP server
        ''' </summary>
        ''' <value>The password</value>
        Public Property SMTPPassword() As String
            Get
                If _SMTPPassword = Nothing Then
                    _SMTPPassword = CompuMaster.camm.WebManager.WMSystem.Configuration.SmtpPassword
                End If
                Return _SMTPPassword
            End Get
            Set(ByVal Value As String)
                _SMTPPassword = Value
            End Set
        End Property

        ''' <summary>
        '''     Configures the authentication methods for the SMTP server
        ''' </summary>
        ''' <value>The authentification type, possible values are "" or "NONE", "LOGIN", "PLAIN", "CRAM-MD5", or "NTLM"</value>
        Public Property SMTPAuthType() As String
            Get
                If _SMTPAuthType = Nothing Then
                    _SMTPAuthType = CompuMaster.camm.WebManager.WMSystem.Configuration.SmtpAuthentificationMode
                End If
                Return _SMTPAuthType
            End Get
            Set(ByVal Value As String)
                _SMTPAuthType = Value
            End Set
        End Property

        Friend _CurrentUserLoginName As String

        ''' <summary>
        '''     Only for internal use; do not use in your regular applications! Resets the logged in username to nothing
        ''' </summary>
        ''' <remarks>
        '''     This method shouldn't be used by your application. Please use Logout() instead.
        ''' </remarks>
        Public Overridable Sub ResetUserLoginName()
            If HttpContext.Current Is Nothing Then
                'Console/Windows application
                _CurrentUserLoginName = Nothing
            ElseIf HttpContext.Current.Request.ApplicationPath = "/" Then
                'root application always uses user name in session variable
                HttpContext.Current.Session.Remove("System_Username")
            Else
                'sub application always looks up this value for every request
                'and caches the value for the current request only
                HttpContext.Current.Items.Remove("System_UserName")
            End If
        End Sub

        ''' <summary>
        '''     Set up the username in the environment/session
        ''' </summary>
        Friend Overridable Sub SetUserLoginName(ByVal loginName As String)
            If loginName = "" Then
                Throw New ArgumentException("loginName mustn't be empty")
            End If
            If HttpContext.Current Is Nothing Then
                'Console/Windows application
                _CurrentUserLoginName = loginName
            ElseIf HttpContext.Current.Request.ApplicationPath = "/" Then
                'root application always uses user name in session variable
                HttpContext.Current.Session("System_Username") = loginName
            Else
                'sub application always looks up this value for every request
                'and caches the value for the current request only
                HttpContext.Current.Items("System_UserName") = loginName
            End If
        End Sub
        ''' <summary>
        ''' The currently logged on user
        ''' </summary>
        ''' <value></value>
        Public Overridable ReadOnly Property CurrentUserLoginName() As String
            Get
                If HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing Then
                    'Console/Windows application
                    Return _CurrentUserLoginName
                Else
                    Dim serverInfo As ServerInformation = Me.CurrentServerInfo
                    If serverInfo Is Nothing Then
                        Throw New Exception("Error: server information is nothing - invalidly configured server identifier?")
                    ElseIf Configuration.ImpersonationLoginName <> "" AndAlso serverInfo.ParentServerGroup.AllowImpersonation Then
                        Return Configuration.ImpersonationLoginName
                    End If
                    If HttpContext.Current.Request.ApplicationPath = "/" Then
                        'root application always uses user name in session variable
                        '(or tries to lookup this value once to store it in session)
                        If HttpContext.Current.Session("System_UserName") Is Nothing OrElse CType(HttpContext.Current.Session("System_UserName"), String) = Chr(0) Then
                            HttpContext.Current.Session("System_UserName") = ""
                        End If
                        If CType(HttpContext.Current.Session("System_UserName"), String) = "" Then
                            HttpContext.Current.Session("System_UserName") = LookupUserNameByScriptEngineSessionID(serverInfo.ID, CompuMaster.camm.WebManager.WMSystem.ScriptEngines.ASPNet, Me.CurrentScriptEngineSessionID)
                        End If
                        Return Trim(CType(HttpContext.Current.Session("System_Username"), String))
                    Else
                        'sub application always looks up this value for every request
                        If CType(HttpContext.Current.Items("System_UserName"), String) = "" Then
                            HttpContext.Current.Items("System_UserName") = LookupUserNameByScriptEngineSessionID(serverInfo.ID, CompuMaster.camm.WebManager.WMSystem.ScriptEngines.ASPNet, Me.CurrentScriptEngineSessionID)
                        End If
                        Return CType(HttpContext.Current.Items("System_UserName"), String)
                    End If
                End If
            End Get
        End Property

        ''' <summary>
        '''     Lookup the username in the list of active/current user sessions which is registered for the current browser session on the specified server
        ''' </summary>
        ''' <param name="serverID">The server ID which is running the requested browser session</param>
        ''' <param name="scriptEngineID">The ID of the script engine which is related to the SessionID</param>
        ''' <param name="scriptEngineSessionID">A SessionID string of the script engine which is registered for a special user session</param>
        ''' <returns>The loginname of the registered user or Nothing if no user is registered for the specified browser session</returns>
        Friend Function LookupUserNameByScriptEngineSessionID(ByVal serverID As Integer, ByVal scriptEngineID As CompuMaster.camm.WebManager.WMSystem.ScriptEngines, ByVal scriptEngineSessionID As String) As String
            Dim MyCmd As SqlCommand
            Dim _DBBuildNo As Integer = Setup.DatabaseUtils.Version(Me, True).Build
            If _DBBuildNo >= 144 Then 'SP has been created in build 144
                MyCmd = New SqlCommand("LookupUserNameByScriptEngineSessionID", New SqlConnection(Me.ConnectionString))
                MyCmd.CommandType = CommandType.StoredProcedure
            Else 'full SQL command text
                Const sql As String = "SELECT Benutzer.LoginName" & vbNewLine & _
                    "FROM [System_WebAreasAuthorizedForSession] As SSID" & vbNewLine & _
                    "	LEFT JOIN System_UserSessions As USID On SSID.SessionID = USID.ID_Session" & vbNewLine & _
                    "	LEFT JOIN Benutzer On USID.ID_User = Benutzer.ID" & vbNewLine & _
                    "WHERE SSID.Server = @ServerID" & vbNewLine & _
                    "	And SSID.ScriptEngine_ID = @ScriptEngineID" & vbNewLine & _
                    "	And SSID.ScriptEngine_SessionID = @ScriptEngineSessionID"
                MyCmd = New SqlCommand(sql, New SqlConnection(Me.ConnectionString))
                MyCmd.CommandType = CommandType.Text
            End If
            MyCmd.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
            MyCmd.Parameters.Add("@ScriptEngineID", SqlDbType.Int).Value = scriptEngineID
            MyCmd.Parameters.Add("@ScriptEngineSessionID", SqlDbType.NVarChar).Value = scriptEngineSessionID
            Return Utils.StringNotNothingOrEmpty(Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), ""))
        End Function

        ''' <summary>
        '''     Lookup the username in the list of active/current user sessions which is registered for the current browser session on the specified server
        ''' </summary>
        ''' <param name="scriptEngineID">The ID of the script engine which is related to the SessionID</param>
        ''' <param name="scriptEngineSessionID">A SessionID string of the script engine which is registered for a special user session</param>
        ''' <returns>The loginname of the registered user or Nothing if no user is registered for the specified browser session</returns>
        Friend Function LookupUserNameByScriptEngineSessionID(ByVal scriptEngineID As CompuMaster.camm.WebManager.WMSystem.ScriptEngines, ByVal scriptEngineSessionID As String) As String
            Dim MyCmd As SqlCommand
            Const sql As String = "SELECT Benutzer.LoginName" & vbNewLine & _
                    "FROM [System_WebAreasAuthorizedForSession] As SSID" & vbNewLine & _
                    "	LEFT JOIN System_UserSessions As USID On SSID.SessionID = USID.ID_Session" & vbNewLine & _
                    "	LEFT JOIN Benutzer On USID.ID_User = Benutzer.ID" & vbNewLine & _
                    "WHERE SSID.ScriptEngine_ID = @ScriptEngineID" & vbNewLine & _
                    "	And SSID.ScriptEngine_SessionID = @ScriptEngineSessionID"
            MyCmd = New SqlCommand(sql, New SqlConnection(Me.ConnectionString))
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@ScriptEngineID", SqlDbType.Int).Value = scriptEngineID
            MyCmd.Parameters.Add("@ScriptEngineSessionID", SqlDbType.NVarChar).Value = scriptEngineSessionID
            Return Utils.StringNotNothingOrEmpty(Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), ""))
        End Function

        ''' <summary>
        '''     Lookup the internal session ID of a user in the list of active/current user sessions which is registered for the current browser session on the specified server
        ''' </summary>
        ''' <param name="serverID">The server ID which is running the requested browser session</param>
        ''' <param name="scriptEngineID">The ID of the script engine which is related to the SessionID</param>
        ''' <param name="scriptEngineSessionID">A SessionID string of the script engine which is registered for a special user session</param>
        ''' <returns>The session ID in camm Web-Manager for the specified browser session or zero value (0) if no one can be found</returns>
        Private Function LookupUserSessionIDByScriptEngineSessionID(ByVal serverID As Integer, ByVal scriptEngineID As CompuMaster.camm.WebManager.WMSystem.ScriptEngines, ByVal scriptEngineSessionID As String) As Long
            Const sql As String = "SELECT SSID.SessionID" & vbNewLine & _
                "FROM [System_WebAreasAuthorizedForSession] As SSID" & vbNewLine & _
                "WHERE SSID.Server = @ServerID" & vbNewLine & _
                "	And SSID.ScriptEngine_ID = @ScriptEngineID" & vbNewLine & _
                "	And SSID.ScriptEngine_SessionID = @ScriptEngineSessionID"
            Dim MyCmd As New SqlCommand(sql, New SqlConnection(Me.ConnectionString))
            MyCmd.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
            MyCmd.Parameters.Add("@ScriptEngineID", SqlDbType.Int).Value = scriptEngineID
            MyCmd.Parameters.Add("@ScriptEngineSessionID", SqlDbType.NVarChar).Value = scriptEngineSessionID
            Return Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), 0)

        End Function

        Private _CurrentScriptEngineSessionID As String
        ''' <summary>
        '''     Provides the current script engine session ID or a randomized session ID for non-http applications
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' May throw an exception in web environments if no session ID can be looked up from cookie or ASP.NET session handler
        ''' In windows or console applications, there will be returned a self-created session ID
        ''' </remarks>
        Public Overridable ReadOnly Property CurrentScriptEngineSessionID() As String
            Get
                If HttpContext.Current Is Nothing Then
                    'Windows/console applications
                    If _CurrentScriptEngineSessionID Is Nothing Then
                        _CurrentScriptEngineSessionID = "l:" & Guid.NewGuid.ToString("n")
                    End If
                    Return _CurrentScriptEngineSessionID
                Else
                    'Web applications
                    'Code for IIS-Inter-Application-SessionID-Sharing
                    If Configuration.CookieLess = True Then
                        If HttpContext.Current.Session Is Nothing Then
                            Throw New Exception("HttpContext.Current.Session Is Nothing")
                        End If
                        Return HttpContext.Current.Session.SessionID
                    Else
                        'Cookies activated
                        '1. Try to read SessionID from cookie (inclusive some validation)
                        If Not HttpContext.Current.Request.Cookies("CwmAuthNet") Is Nothing Then
                            Dim possibleValue As String = HttpContext.Current.Request.Cookies("CwmAuthNet").Value
                            If possibleValue Is Nothing OrElse possibleValue.Length < 10 Then
                                'looks like a faulty session ID --> better ignore it!
                            Else
                                'looks like a valid session ID --> cryptography security is high enough
                                Return possibleValue
                            End If
                        End If
                        '2. Otherwise use normal SessionID, but save this SessionID in cookie for readability by other independent IIS-applications
                        If HttpContext.Current.Session Is Nothing Then
                            Throw New Exception("HttpContext.Current.Session Is Nothing")
                        End If
                        Dim AspNetSessionID As String
                        AspNetSessionID = HttpContext.Current.Session.SessionID
                        HttpContext.Current.Response.Cookies("CwmAuthNet").Value = AspNetSessionID
                        HttpContext.Current.Response.Cookies("CwmAuthNet").Path = "/"
                        Return AspNetSessionID
                    End If
                End If
            End Get
        End Property

        ''' <summary>
        ''' The identification string of the current web server instance
        ''' </summary>
        ''' <remarks><para>The current webserver has to be identifiable by camm Web-Manager to be a known server of a known server group. Each server has got its own identification string. In the easiest case, the identification string contains the IP address of the web server.</para>
        ''' <para>ATTENTION: The maximum length of this value is 32 characters.</para></remarks>
        Private _CurrentServerIdentString As String
        ''' <summary>
        ''' The connection string to the database
        ''' </summary>
        ''' <remarks></remarks>
        Private _ConnectionString As String
        Private _DevelopmentEMailAccountAddress As String
        Private _StandardEMailAccountName As String
        Private _StandardEMailAccountAddress As String
        Private _TechnicalServiceEMailAccountName As String
        Private _TechnicalServiceEMailAccountAddress As String
        Private _System_SecurityQueryCache_MaxAgeInSeconds As Integer = 300
        Private _System_DebugLevel As DebugLevels = DebugLevels.NoDebug
        Private WithEvents _Internationalization As WMSettingsAndData
        Private _DefaultNotifications As CompuMaster.camm.WebManager.Notifications.INotifications
        Private _IsSupported As WMCapabilities
        Private _IsLoggedOn As Boolean = False
        Private _PageTitle As String = ""
        Private _PageMETA_Name As New Collections.Specialized.NameValueCollection
        Private _PageMETA_HttpEquiv As New Collections.Specialized.NameValueCollection
        Private _PageAdditionalHeaders As String
        Private _PageAdditionalBodyAttributes As New Collections.Specialized.NameValueCollection

        ''' <summary>
        ''' Existing debug levels in camm Web-Manager
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum DebugLevels As Byte
            ''' <summary>
            '''     No debugging
            ''' </summary>
            ''' <remarks>
            '''     This feature is set to disabled.
            ''' </remarks>
            NoDebug = 0 'kein Debug
            ''' <summary>
            '''     Access error warnings only
            ''' </summary>
            ''' <remarks>
            '''     Warning messages will be sent to the developer contact configured in your config files.
            ''' </remarks>
            Low_WarningMessagesOnAccessError = 1 'Warn-Messages über Access Errors an Developer Contact
            ''' <summary>
            '''     More access error warnings
            ''' </summary>
            ''' <remarks>
            '''     Additional warning messages will be sent to the developer contact configured in your config files.
            ''' </remarks>
            Low_WarningMessagesOnAccessError_AdditionalDetails = 2 'Protokollierung von zusätzlichen Informationen
            ''' <summary>
            '''     Actively collect data for debugging
            ''' </summary>
            ''' <remarks>
            '''     Even more additional warning messages will be sent to the developer contact configured in your config files.
            ''' </remarks>
            Medium_LoggingOfDebugInformation = 3 'Protokollierung von zusätzlichen Debug-Informationen
            ''' <summary>
            '''     Send all e-mails to developer account - never use in production environments!
            ''' </summary>
            ''' <remarks>
            '''     <para>ATTENTION: Never use this mode in production environments!</para>
            '''     <para>All e-mails will be redirected to the developer; no e-mail will be sent to the origin receipient. This is optimal for project development and testing environments.</para>
            '''		<para>The messages will be sent to the developer contact configured in your config files.</para>
            ''' </remarks>
            <Obsolete("Use Medium_LoggingAndRedirectAllEMailsToDeveloper instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Medium_LoggingOfDebugInformation_AdditionalDetails = 4 'e-mail-Umleitung an Developer Contact
            ''' <summary>
            '''     Send all e-mails to developer account - never use in production environments!
            ''' </summary>
            ''' <remarks>
            '''     <para>ATTENTION: Never use this mode in production environments!</para>
            '''     <para>All e-mails will be redirected to the developer; no e-mail will be sent to the origin receipient. This is optimal for project development and testing environments.</para>
            '''		<para>The messages will be sent to the developer contact configured in your config files.</para>
            ''' </remarks>
            Medium_LoggingAndRedirectAllEMailsToDeveloper = 4 'e-mail-Umleitung an Developer Contact
            ''' <summary>
            '''     Maximum debug level - never use in production environments!
            ''' </summary>
            ''' <remarks>
            '''     <para>ATTENTION: Never use this mode in production environments!</para>
            '''     <para>All e-mails will be redirected to the developer; no e-mail will be sent to the origin receipient. This is optimal for setting up the project.</para>
            '''		<para>The messages will be sent to the developer contact configured in your config files.</para>
            '''     <para>Automatic redirects have to be manually executed. This is ideally for solving redirection bugs or when loosing session paths in cookieless scenarios.</para>
            ''' </remarks>
            Max_RedirectPageRequestsManually = 5 'e-mail-Umleitung an Developer Contact + manual page redirections
            <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never), Obsolete("Use Max_RedirectPageRequestsManually instead")> Max_RedirectAllEMailsToDeveloper = 5 'e-mail-Umleitung an Developer Contact + manual page redirections
        End Enum

        ''' <summary>
        ''' Special pre-defined groups in camm Web-Manager
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum SpecialGroups As Integer
            ''' <summary>
            ''' Supervisors
            ''' </summary>
            ''' <remarks>A supervisor has got all authorizations, sees everthing, can do everything.</remarks>
            Group_Supervisors = 6
            ''' <summary>
            ''' Security administrators
            ''' </summary>
            ''' <remarks>
            ''' <para>Security administrators manage the incoming account creation notifications, assign authorizations, trouble shoot users with their problems.</para>
            ''' <para>The possibilities to manage authorizations, etc. are defined by the delegation of security authorizations.</para>
            ''' <para>The security administrator gets a notification when a user account has been created. So, he's able to authorize the new user for all applications, the security administrator is responsible for assignments.</para>
            ''' <list>
            ''' <listheader>Following security authorizations exist:</listheader>
            ''' <item>Owner</item>
            ''' <item>New</item>
            ''' <item>Delete</item>
            ''' <item>Update</item>
            ''' <item>Update relations</item>
            ''' <item>Security Master (available only in applications overview or users overview)</item>
            ''' </list>
            ''' </remarks>
            Group_SecurityAdministrators = 7
            ''' <summary>
            ''' Users who may access security administration area for review purposes (e.g. log access)
            ''' </summary>
            Group_SecurityRelatedContacts = -7
            ''' <summary>
            ''' Users who are responsible for data protection compliance and rules
            ''' </summary>
            Group_DataProtectionCoordinators = -5
            ''' <summary>
            ''' Users who are allowed to access all applications/security objects, but without supervisor priviledges for administration
            ''' </summary>
            Group_SecurityAccessEverything = -6
        End Enum

        ''' <summary>
        ''' Special users/groups pre-defined by camm Web-Manager
        ''' </summary>
        ''' <remarks>
        '''     Values &gt; 0 are really existing users;
        '''     Values &lt; 0 are pseudonyms
        '''     Values = 0 are invalid
        ''' </remarks>
        Public Enum SpecialUsers As Integer
            User_Anonymous = -1
            User_UpdateProcessor = -43
            User_Public = -2
            User_Code = -33
            User_Invalid = 0
        End Enum

        ''' <summary>
        ''' Supported script engines of camm Web-Manager
        ''' </summary>
        ''' <remarks></remarks>
        Friend Enum ScriptEngines As Integer
            NetClient = -1
            ASP = 1
            ASPNet = 2
            'SAPWebStudio = 3
            'PHP = 4
        End Enum

        Friend Shared MilestoneDBBuildNumber_MailQueue As Integer = 122
        <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Shared MilestoneAssembly_Build193 As Integer = 193
        <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Shared MilestoneDBBuildNumber_Build147 As Integer = 147
        <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Shared MilestoneDBBuildNumber_Build164 As Integer = 164
        <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Shared MilestoneDBBuildNumber_Build173 As Integer = 173
        <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Shared MilestoneDBBuildNumber_AdminSecurityDelegates As Integer = 185
        <Obsolete("ATTENTION INCOMPATIBILITY CWM-SecObj Milestone"), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Shared MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects As New Version(4, 20)
        'Friend Shared MilestoneVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects As New Version(4, 20)
        <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Shared MilestoneDBVersion_MembershipsWithSupportForSystemAndCloneRule As New Version(4, 10, 203)
        <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Shared MilestoneDBVersion_AuthsAdminViewWithCompanyField As New Version(4, 10, 204)
        <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Shared MilestoneDBVersion_AuthsWithSupportForDenyRule As New Version(4, 12, 2000) 'also with this build: distributed deletion of foreign keys by triggers

        ''' <summary>
        ''' Values for the different events in the protocol
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum Logging_ConflictTypes As Integer
            Debug = -99
            UnsuccessfullLogin = -98
            LoginLockedTemporary = -95 'PW check failed too often
            UserDeleted = -31
            UserAccountTemporaryLockedOnDocumentValidation = -29
            UserAccountDisabledOnDocumentValidation = -28
            MissingAuthorizationOnDocumentValidation = -27
            NoValidLoginData = -26 'e. g. PW
            MissingLoginOnDocumentValidation = -25
            UserProfileAttributesChangedByAdminOrTheUserItself = -9
            AuthorizationsOfGroupModified = -8
            AuthorizationsOfUserModifiedIndirectlyViaGroupMembershipment = -7
            AuthorizationsOfUserModified = -6
            GroupMembershipAdded = -11
            GroupMembershipRemoved = -12
            AccountLockHasBeenResettedByAdmin = -5
            UserProfileModifiedByTheUserItself = -4
            PageHit = 0
            UserCreatedByTheUserItself = 1
            PasswordRequestedForSendingViaEMail = 2
            UserCreatedByAdmin = 3
            UserProfileModifiedByAdmin = 4
            UserPasswordModifiedByAdmin = 5
            UserPasswordModifiedByTheUserItself = 6
            PreparationForGUIDLogin = 97
            Login = 98
            Logout = 99
            RuntimeInformation = -70
            RuntimeWarning = -71
            RuntimeException = -72
            ApplicationInformation = -80
            ApplicationWarning = -81
            ApplicationException = -82
            UpdateWarning = 44
            UpdateException = 45
            UpdateInformation = 46
            SsoMissingAssignmentOfExternalUserAccount = 47
            AddedSubSecurityDelegationGroups = 35
            RemovedSubSecurityDelegationGroups = 36
            AddedSubSecurityDelegationSecurityObjects = 37
            RemovedSubSecurityDelegationGroupsSecurityObjects = 38
        End Enum

        ''' <summary>
        '''     Return values of the validation method for users
        ''' </summary>
        ''' <remarks>
        '''     kein Record    -->	    nicht definiert! --> Fehler!
        '''     DBNull	        -->     nicht definiert! --> Fehler!
        ''' </remarks>
        Public Enum ReturnValues_UserValidation As Integer
            ''' <summary>
            '''     Unknown error without any detailed message
            ''' </summary>
            UnknownError = 1
            ''' <summary>
            '''     Angegebener Server nicht gefunden oder deaktiviert
            ''' </summary>
            ServerNotFound = -10
            ''' <summary>
            '''     Benutzer hat (keine) Anmeldeerlaubnis auf diesem Server (oder unvollständige Parameterliste)
            ''' </summary>
            NoLoginRightForThisServer = -9
            ''' <summary>
            '''     Keine Berechtigung für angeforderte Anwendung, Login jedoch erfolgreich
            ''' </summary>
            ValidationSuccessfull_ButNoAuthorizationForRequiredSecurityObject = -5
            ''' <summary>
            '''     Bereits angemeldet
            ''' </summary>
            AlreadyLoggedIn = -4
            ''' <summary>
            '''     Access denied (Benutzer fehlt Recht)
            ''' </summary>
            AccessDenied = -3
            ''' <summary>
            '''     Login zu oft fehlgeschlagen! Konto gesperrt!
            ''' </summary>
            TooManyLoginFailures = -2
            ''' <summary>
            '''     Validierung erfolgreich
            ''' </summary>
            ValidationSuccessfull = -1
            ''' <summary>
            '''     User oder Passwort oder beides konnten nicht authentifiziert werden oder Konto gesperrt oder Anmeldung auf Server verweigert
            ''' </summary>
            UserOrPasswortMisstyped_OR_AccountLocked_OR_LoginDeniedAtThisServerGroup = 0
            ''' <summary>
            '''     CurUserAccountAccessability Is Null --> User nicht gefunden
            ''' </summary>
            UserNotFound_BecauseOf_UserAccountAccessability = 43
            ''' <summary>
            '''     Benutzerkonto gesperrt
            ''' </summary>
            UserAccountLocked = 44
            ''' <summary>
            '''     Reauthentifizierung fehlgeschlagen (Login von einer anderen Station)
            ''' </summary>
            LoginFromAnotherWorkstation = 57
            ''' <summary>
            '''     Login ausstehend und erforderlich
            ''' </summary>
            LoginRequired = 58
            ''' <summary>
            '''     The lookup process for an external account name hasn't found a matching user
            ''' </summary>
            UserForDemandedExternalAccountNotFound = -67
        End Enum

        ''' <summary>
        '''     System application types
        ''' </summary>
        ''' <remarks>
        '''     Attached servers like SAP, DocWare are using SystemAppTypes IDs &lt; 0 (smaller than zero)
        ''' </remarks>
        Private Enum SystemAppTypes As Integer
            ''' <summary>
            '''     Default item
            ''' </summary>
            UserDefined = 0
            ''' <summary>
            '''     Master server items are subject of changes while reconfiguration of master server setup
            ''' </summary>
            LoginServer = 1
            ''' <summary>
            '''     Admin server items are subject of changes while reconfiguration of admin server setup
            ''' </summary>
            AdminServer = 2
            ''' <summary>
            '''     Admin server items are subject of changes while reconfiguration of admin server setup
            ''' </summary>
            ''' <remarks>
            ''' Status per 2016-07-29 JW: exact intention is unknown why to use ID values 2 and 3 - in SQL maybe a difference in how/when deleting auths, but at the end not the real matter?!?
            ''' </remarks>
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> AdminServerCategory3 = 3
        End Enum
#End Region

#Region "Standard WebManager methods"

        Private _SecurityObjectSuccessfullyTested As String
        ''' <summary>
        '''     The name of a security object that has been successfully checked for authorization
        ''' </summary>
        ''' <value>The name of the security object</value>
        Friend ReadOnly Property SecurityObjectSuccessfullyTested() As String
            Get
                Return _SecurityObjectSuccessfullyTested
            End Get
        End Property

        Private _SecurityObject As String
        ''' <summary>
        '''     The name of a security object to be automatically checked for authorization when the page loads
        ''' </summary>
        ''' <value>The name of the security object</value>
        <System.ComponentModel.DefaultValue("")> Public Property SecurityObject() As String
            Get
                Return _SecurityObject
            End Get
            Set(ByVal Value As String)
                _SecurityObject = Value

                'Automatically/directly check for the given security object if the connection and serveridentification is already available
                If Me.CurrentServerIdentString <> Nothing AndAlso Me.ConnectionString <> Nothing AndAlso _SecurityObject <> "" AndAlso Me._SecurityObject <> Me.SecurityObjectSuccessfullyTested AndAlso IsConfigurationLoaded Then
                    AuthorizeDocumentAccess()
                End If

            End Set
        End Property

        ''' <summary>
        ''' Indicates if the configuration has been loaded and authorization checks may successfully complete
        ''' </summary>
        ''' <remarks></remarks>
        Friend IsConfigurationLoaded As Boolean = False

        Friend ReadOnly Property GlobalConfiguration As GlobalConfiguration
            Get
                Static _Result As GlobalConfiguration
                If _Result Is Nothing Then _Result = New GlobalConfiguration(Me)
                Return _Result
            End Get
        End Property

        Private Sub AuthorizeDocumentAccess()
            Dim logPageHit As Boolean
            Select Case AutoSecurityCheckLogsPageAccess
                Case AutoSecurityCheckLogsPageHit.Never
                    logPageHit = False
                Case AutoSecurityCheckLogsPageHit.NotOnPostBack
                    If Me.IsPostBack Then
                        logPageHit = False
                    Else
                        logPageHit = True
                    End If
                Case AutoSecurityCheckLogsPageHit.OnEveryRequest
                    logPageHit = True
                Case Else
                    Throw New NotSupportedException("Invalid value detected For AutoSecurityCheckLogsPageAccess")
            End Select
            If logPageHit = True Then
                'Authentication caching is not allowed to be used (page hit log entry is required always)
                Me.AuthorizeDocumentAccess(_SecurityObject, Nothing, False, True, logPageHit)
            Else
                'Authentication caching is allowed to be used (won't create a page hit log entry if validated successfully)
                Me.AuthorizeDocumentAccess(_SecurityObject, Nothing, True, True, logPageHit)
            End If
        End Sub

        Private Sub System_PreFill_MetaInfo()

            'Setup the defaults
            If Not HttpContext.Current Is Nothing Then
                If Me.PageMETA_HttpEquiv("Content-Type") Is Nothing Then Me.PageMETA_HttpEquiv("Content-Type") = "text/html; charset=" & HttpContext.Current.Response.Charset
            End If
            If Me.PageMETA_HttpEquiv("Content-Language") Is Nothing Then Me.PageMETA_HttpEquiv("Content-Language") = System.Threading.Thread.CurrentThread.CurrentCulture.Name
            If Me.PageMETA_HttpEquiv("Cache-Control") Is Nothing Then Me.PageMETA_HttpEquiv("Cache-Control") = "no-cache"
            If Me.PageMETA_HttpEquiv("Pragma") Is Nothing Then Me.PageMETA_HttpEquiv("Pragma") = "no-cache"
            If Me.PageMETA_HttpEquiv("Expires") Is Nothing Then Me.PageMETA_HttpEquiv("Expires") = "0"
            If Me.PageMETA_Name("Language") Is Nothing Then Me.PageMETA_Name("Language") = System.Threading.Thread.CurrentThread.CurrentCulture.Name
            If Me.PageMETA_Name("Publisher") Is Nothing Then Me.PageMETA_Name("Publisher") = Internationalization.OfficialServerGroup_Company_FormerTitle
            If Me.PageMETA_Name("Author") Is Nothing Then Me.PageMETA_Name("Author") = Internationalization.OfficialServerGroup_Company_FormerTitle
            If Me.PageMETA_Name("Copyright") Is Nothing Then Me.PageMETA_Name("Copyright") = Internationalization.OfficialServerGroup_Company_FormerTitle
            If Me.PageMETA_Name("Revisit-After") Is Nothing Then Me.PageMETA_Name("Revisit-After") = "1 months"
            If Me.PageMETA_Name("Robots") Is Nothing Then Me.PageMETA_Name("Robots") = "INDEX, FOLLOW"

            'Abstract and Description should be the same if one of them is empty
            If Me.PageMETA_Name("Abstract") Is Nothing AndAlso Not Me.PageMETA_Name("Description") Is Nothing Then Me.PageMETA_Name("Abstract") = Me.PageMETA_Name("Description")
            If Me.PageMETA_Name("Description") Is Nothing AndAlso Not Me.PageMETA_Name("Abstract") Is Nothing Then Me.PageMETA_Name("Description") = Me.PageMETA_Name("Abstract")

            'Page-Topic defaults to the PageTitle
            If Me.PageMETA_Name("Page-Topic") Is Nothing AndAlso Me.PageTitle <> "" Then Me.PageMETA_Name("Page-Topic") = Me.PageTitle
            If Me.PageMETA_Name("Title") Is Nothing AndAlso Me.PageTitle <> "" Then Me.PageMETA_Name("Title") = Me.PageTitle

            'Ensure that the Generator is our camm WebManager
            Me.PageMETA_Name("Generator") = Environment.ProductName & " V" & Me.System_Version & " Build " & Me.System_Build & " - " & Environment.LicenceKey

        End Sub

        ''' <summary>
        '''     Get the META information for placing it into the HTML head area.
        ''' </summary>
        ''' <returns>A string with some META information for the current page</returns>
        Public Function System_GetHtmlMetaBlock() As String

            System_PreFill_MetaInfo() 'Load defaults if not yet loaded / overwrite Generator tag to ensure the correct content

            Dim Result As New System.Text.StringBuilder
            For Each MyMetaName As String In Me.PageMETA_HttpEquiv 'do not HTMLEncode this
                Result.Append("<meta http-equiv=""" & System.Web.HttpUtility.HtmlAttributeEncode(MyMetaName) & """ content=""" & Me.PageMETA_HttpEquiv(MyMetaName) & """>" & vbNewLine)
            Next
            For Each MyMetaName As String In Me.PageMETA_Name 'HTMLEncode this
                Result.Append("<meta name=""" & System.Web.HttpUtility.HtmlAttributeEncode(MyMetaName) & """ content=""" & System.Web.HttpUtility.HtmlAttributeEncode(Me.PageMETA_Name(MyMetaName)) & """>" & vbNewLine)
            Next
            Return Result.ToString

        End Function


        ''' <summary>
        '''     Recover the password of a user
        ''' </summary>
        ''' <param name="MyLoginName">Login name of the user</param>
        ''' <returns>The demanded password. If decryption isn't possible, an empty string</returns>
        Public Function System_GetUserPassword(ByVal MyLoginName As String) As String
            Dim result As String = Nothing
            Try
                Dim transformationResult As CryptoTransformationResult = System_GetUserPasswordTransformationResult(MyLoginName)
                Dim transformer As IWMPasswordTransformation = PasswordTransformerFactory.ProduceCryptographicTransformer(transformationResult.Algorithm)
                If TypeOf transformer Is IWMPasswordTransformationBackwards Then
                    Dim reverseTransformer As IWMPasswordTransformationBackwards = CType(transformer, IWMPasswordTransformationBackwards)
                    result = reverseTransformer.TransformStringBack(transformationResult.TransformedText, transformationResult.Noncevalue)
                End If
            Catch ex As Exception
                result = Nothing
            End Try
            Return result
        End Function

        ''' <summary>
        '''     Recover the password of a user
        ''' </summary>
        ''' <param name="MyLoginName">Login name of the user</param>
        ''' <param name="MyEMailAddress">The user's e-mail address</param>
        ''' <returns>The demanded password. If decryption isn't possible, an empty string</returns>
        ''' <remarks>Keeping this around for now as I don't know whether there is code out there using this function and passing the email address which is of course unnecessary when you have unique usernames... </remarks>
        Public Function System_GetUserPassword(ByVal MyLoginName As String, ByVal MyEMailAddress As String) As String
            Return System_GetUserPassword(MyLoginName)
        End Function

        Private _DatabaseIsNewerBuildThanWebManagerApplication As TripleState
        ''' <summary>
        '''     Is the Web-Manager database newer than this application? If yes, we're incompatible to the new database!
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     Used from property ConnectionString only
        ''' </remarks>
        Private Property DatabaseIsNewerBuildThanWebManagerApplication() As TripleState
            Get
                If HttpContext.Current Is Nothing Then
                    Return _DatabaseIsNewerBuildThanWebManagerApplication
                Else
                    Return CType(HttpContext.Current.Application("WebManager.DatabaseIsNewerBuildThanWebManagerApplication"), TripleState)
                End If
            End Get
            Set(ByVal Value As TripleState)
                If HttpContext.Current Is Nothing Then
                    _DatabaseIsNewerBuildThanWebManagerApplication = Value
                Else
                    HttpContext.Current.Application("WebManager.DatabaseIsNewerBuildThanWebManagerApplication") = Value
                End If
            End Set
        End Property

        Private _URLReplacements As Collections.Specialized.NameValueCollection
        ''' <summary>
        '''     Collection of strings to be replaced in hyperlinks
        ''' </summary>
        ''' <value>The new collection</value>
        Public Property URLReplacements() As Collections.Specialized.NameValueCollection
            Get
                Return _URLReplacements
            End Get
            Set(ByVal Value As Collections.Specialized.NameValueCollection)
                _URLReplacements = Value
            End Set
        End Property

        ''' <summary>
        '''     The server identification string of the current server
        ''' </summary>
        ''' <value>A server identification string</value>
        Friend ReadOnly Property CurrentServerIdentStringNoAutoLookup() As String
            Get
                Return _CurrentServerIdentString
            End Get
        End Property

        ''' <summary>
        '''     The server identification string of the current server
        ''' </summary>
        ''' <value>A server identification string</value>
        Public Property CurrentServerIdentString() As String
            Get
                If _CurrentServerIdentString = Nothing Then
                    WebManager.Log.WriteEventLogTrace("CurrentServerIdentString:Get:BeginLookup")
                    _CurrentServerIdentString = Configuration.CurrentServerIdentification
                    WebManager.Log.WriteEventLogTrace("CurrentServerIdentString:Get:EndLookup")
                End If
                Return _CurrentServerIdentString
            End Get
            Set(ByVal Value As String)
                WebManager.Log.WriteEventLogTrace("CurrentServerIdentString:Set:Begin")
                If _CurrentServerIdentString <> Value Then
                    Initialized = False
                End If
                _CurrentServerIdentString = Value
                InitializeEnvironment()
                WebManager.Log.WriteEventLogTrace("CurrentServerIdentString:Set:End")
            End Set
        End Property

        ''' <summary>
        '''     The server information object of the current server
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property CurrentServerInfo() As ServerInformation
            Get
                Static _CurrentServerInfo As ServerInformation
                If _CurrentServerInfo Is Nothing OrElse _CurrentServerInfo.IPAddressOrHostHeader <> Me.CurrentServerIdentString Then
                    _CurrentServerInfo = Me.System_GetServerInfo(Me.CurrentServerIdentString)
                End If
                Return _CurrentServerInfo
            End Get
        End Property

        ''' <summary>
        '''     The address of the client accessing the camm Web-Manager system
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property CurrentRemoteClientAddress() As String
            Get
                If HttpContext.Current Is Nothing Then
                    Return Utils.GetWorkstationID()
                Else
                    Return Utils.LookupRealRemoteClientIPOfHttpContext
                End If
            End Get
        End Property

        Private _SafeMode As Boolean
        ''' <summary>
        '''     Safe mode is for error page to allow ignoring of exceptions while loading the custom configuration
        ''' </summary>
        ''' <value></value>
        Friend Property SafeMode() As Boolean
            Get
                Return _SafeMode OrElse (Not HttpContext.Current Is Nothing AndAlso HttpContext.Current.Request.ServerVariables("SCRIPT_NAME").ToLower(System.Globalization.CultureInfo.InvariantCulture) = "/sysdata/access_error.aspx")
            End Get
            Set(ByVal Value As Boolean)
                _SafeMode = Value
            End Set
        End Property

        Private Sub InitializeEnvironment()
            If ConnectionString <> "" AndAlso _CurrentServerIdentString <> "" Then WebManager.Log.WriteEventLogTrace("InitializeEnvironment:Begin")
            If ConnectionString <> "" AndAlso _CurrentServerIdentString <> "" AndAlso Not HttpContext.Current Is Nothing AndAlso (HttpContext.Current.Session Is Nothing OrElse CType(HttpContext.Current.Session("CWM_SessionTimeoutInitialized"), Boolean) <> True) Then
                'Check the server configuration
                Dim MyCurServerID As Integer
                Try
                    MyCurServerID = System_GetServerID(CurrentServerIdentString)
                Catch
                    If Me.SafeMode Then
                        Try
                            Me.Log.RuntimeException("Current server identification value (e. g. IP, host header name) can't be found in database. There is a critical configuration problem." & vbNewLine & "Tip: While installation of the database you should configure CurrentServerIdentString as an empty value.", True, False)
                        Catch
                            'ignore errors
                        End Try
                    Else
                        Me.Log.RuntimeException("Current server identification value (e. g. IP, host header name) can't be found in database. There is a critical configuration problem." & vbNewLine & "Tip: While installation of the database you should configure CurrentServerIdentString as an empty value.", True, True)
                    End If
                End Try

                'Setup session life time
                Dim SessionTimeout As Integer
                If Me.SafeMode Then
                    Try
                        SessionTimeout = New ServerInformation(System_GetServerID(CurrentServerIdentString), Me).ServerSessionTimeout * 3
                    Catch
                        'Ignore errors in safe mode
                    End Try
                Else
                    SessionTimeout = New ServerInformation(System_GetServerID(CurrentServerIdentString), Me).ServerSessionTimeout * 3
                End If
                If SessionTimeout <> Nothing AndAlso Not HttpContext.Current.Session Is Nothing Then
                    HttpContext.Current.Session.Timeout = SessionTimeout
                    HttpContext.Current.Session("CWM_SessionTimeoutInitialized") = True
                End If
                System_PreFill_MetaInfo() 'Preload default META tags
            End If
            If ConnectionString <> "" AndAlso _CurrentServerIdentString <> "" AndAlso Initialized = False Then
                'Setup standard values
                Try
                    If Configuration.CookieLess = False Then
                        Me.Internationalization.User_Auth_Config_CurServerURL = Me.System_GetServerURL(CurrentServerIdentString)
                        Me.Internationalization.User_Auth_Config_UserAuthMasterServer = Me.System_GetMasterServerURL(CurrentServerIdentString)
                    Else
                        Me.Internationalization.User_Auth_Config_CurServerURL = Me.CalculateUrl(Me.CurrentServerInfo.ID, ScriptEngines.ASPNet, "") ' Me.System_GetServerURL(CurrentServerIdentString)
                        Me.Internationalization.User_Auth_Config_UserAuthMasterServer = Me.CalculateUrl(Me.CurrentServerInfo.ParentServerGroup.MasterServer.ID, ScriptEngines.ASPNet, "") 'Me.System_GetMasterServerURL(CurrentServerIdentString)
                    End If
                Catch ex As Exception
                    Dim ErrMessage As String
                    ErrMessage = "Server ID for """ & CurrentServerIdentString & """ not registered in camm Web-Manager database with connection string """ & Utils.ConnectionStringWithoutPasswords(ConnectionString) & """"
                    If Me.DebugLevel >= DebugLevels.Medium_LoggingAndRedirectAllEMailsToDeveloper Then
                        ErrMessage &= vbNewLine & "Exact error message=""" & ex.Message & ex.StackTrace & """"
                    ElseIf Me.DebugLevel >= DebugLevels.Medium_LoggingOfDebugInformation Then
                        ErrMessage &= vbNewLine & "Exact error message=""" & ex.Message & """"
                    End If
                    If Not SafeMode Then
                        If Me.DebugLevel >= DebugLevels.Medium_LoggingOfDebugInformation Then
                            Throw New Exception(ErrMessage, ex)
                        Else
                            Me.Log.RuntimeException(New Exception(ErrMessage, ex), False, True)
                        End If
                    End If
                End Try
                Try
                    Me.Internationalization.User_Auth_Validation_NoRefererURL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_UserAuthSystem & "index.aspx"
                    Me.Internationalization.User_Auth_Validation_LogonScriptURL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_SystemData & "logon.aspx"
                    Me.Internationalization.User_Auth_Validation_AfterLogoutURL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_SystemData & "logon.aspx?ErrID=44"
                    Me.Internationalization.User_Auth_Validation_AccessErrorScriptURL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_SystemData & "access_error.aspx"
                    Me.Internationalization.User_Auth_Validation_CreateUserAccountInternalURL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_SystemData & "account_register.aspx"
                    Me.Internationalization.User_Auth_Validation_TerminateOldSessionScriptURL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_Login & "forcelogin.aspx"
                    Me.Internationalization.User_Auth_Validation_CheckLoginURL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_Login & "checklogin.aspx"
                    Me.Internationalization.OfficialServerGroup_URL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_UserAuthSystem
                    Me.Internationalization.OfficialServerGroup_AdminURL = Me.System_GetUserAdminServer_SystemURL(CurrentServerIdentString)
                    Me.Internationalization.OfficialServerGroup_AdminURL_SecurityAdminNotifications = Me.Internationalization.OfficialServerGroup_AdminURL
                    Me.Internationalization.OfficialServerGroup_Title = Me.System_GetServerGroupTitle(CurrentServerIdentString)
                    Me.Internationalization.OfficialServerGroup_Company_FormerTitle = CType(Me.System_GetServerConfig(CurrentServerIdentString, "AreaCompanyFormerTitle"), String)
                Catch ex As Exception
                    Dim ErrMessage As String
                    ErrMessage = "Failed initializing environment"
                    If Me.DebugLevel >= DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails Then
                        ErrMessage &= vbNewLine & "Exact error message=""" & ex.Message & ex.StackTrace & """"
                    ElseIf Me.DebugLevel >= DebugLevels.Low_WarningMessagesOnAccessError Then
                        ErrMessage &= vbNewLine & "Exact error message=""" & ex.Message & """"
                    End If
                    If Not SafeMode Then
                        If Me.DebugLevel >= DebugLevels.Medium_LoggingOfDebugInformation Then
                            Throw New Exception(ErrMessage, ex)
                        Else
                            Me.Log.RuntimeException(New Exception(ErrMessage, ex), False, True)
                        End If
                    End If
                End Try
                Initialized = True
            End If
            If ConnectionString <> "" AndAlso _CurrentServerIdentString <> "" Then WebManager.Log.WriteEventLogTrace("InitializeEnvironment:End")
        End Sub

        ''' <summary>
        '''     Track initialization state because a repeated initalization would lead to an override of LogonScriptUrls and others which might be customized (e. g. in cookieless scenarios)
        ''' </summary>
        Private Initialized As Boolean = False

        ''' <summary>
        '''     The connection string to the camm Web-Manager database
        ''' </summary>
        ''' <value>A string containing all information required to successfully establish a connection to the database</value>
        Public Property ConnectionString() As String Implements IWebManager.ConnectionString
            Get
                If _ConnectionString = Nothing Then
                    WebManager.Log.WriteEventLogTrace("ConnectionString:Get:BeginLookup")
                    _ConnectionString = Configuration.ConnectionString
                    If _ConnectionString <> Nothing Then
                        LogAssemblyVersionToDatabaseOnce()
                        If _ignoreCheckCompatibilityToDatabaseByBuildNumber = False Then CheckCompatibilityToDatabaseByBuildNumber()
                    End If
                    WebManager.Log.WriteEventLogTrace("ConnectionString:Get:EndLookup")
                End If
                Return _ConnectionString
            End Get
            Set(ByVal Value As String)
                WebManager.Log.WriteEventLogTrace("ConnectionString:Set:Begin")
                _ConnectionString = Value
                LogAssemblyVersionToDatabaseOnce()
                If _ignoreCheckCompatibilityToDatabaseByBuildNumber = False Then CheckCompatibilityToDatabaseByBuildNumber()
                Initialized = False
                InitializeEnvironment()
                WebManager.Log.WriteEventLogTrace("ConnectionString:Set:End")
            End Set
        End Property

        ''' <summary>
        '''     Represents a state with 3 possible values: True, False and Undefined (default)
        ''' </summary>
        Friend Enum TripleState As Byte
            Undefined = 0
            [True] = 1
            [False] = 2
        End Enum

        ''' <summary>
        '''     Check the database build number to be smaller or equal than this camm Web-Manager version (the DLL).
        ''' </summary>
        ''' <remarks>
        '''     Throws an exception if the database is newer than this application. This is to prevent data corruption by old methods.
        ''' </remarks>
        Private Sub CheckCompatibilityToDatabaseByBuildNumber()
            WebManager.Log.WriteEventLogTrace("CheckCompatibilityToDatabaseByBuildNumber:Begin")
            If Me.DatabaseIsNewerBuildThanWebManagerApplication = TripleState.Undefined Then
                Dim MyDBVersion As System.Version = Setup.DatabaseUtils.Version(Me, False)
                If Not MyDBVersion Is Nothing AndAlso MyDBVersion.Build > System.Math.Max(Me.System_Version_Ex.Build, Configuration.CompatibilityWithDatabaseBuild) Then
                    DatabaseIsNewerBuildThanWebManagerApplication = TripleState.True
                Else
                    DatabaseIsNewerBuildThanWebManagerApplication = TripleState.False
                End If
            End If
            If Me.DatabaseIsNewerBuildThanWebManagerApplication = TripleState.True Then
                Throw New Exception("Database has a newer build no. than this application. Access denied to prevent data corruption.")
            End If
            WebManager.Log.WriteEventLogTrace("CheckCompatibilityToDatabaseByBuildNumber:End")
        End Sub

        ''' <summary>
        ''' A unique assembly location for each logical (IIS) web application
        ''' </summary>
        Private Function CurrentWebAppAssemblyLocation() As String
            Dim AssemblyLocation As String
            Dim AssemblyBuildNo As Integer = Setup.ApplicationUtils.Version.Build
            If HttpContext.Current Is Nothing Then
                Try
                    AssemblyLocation = "MachineName=" & System.Environment.MachineName & ControlChars.NewLine
                Catch
                    AssemblyLocation = "MachineName={Error while looking up machine name}" & ControlChars.NewLine
                End Try
                Try
                    AssemblyLocation &= "Path=" & System.Environment.GetCommandLineArgs(0) & ControlChars.NewLine
                Catch
                    AssemblyLocation &= "Path={Error while looking up local path}" & ControlChars.NewLine
                End Try
            Else
                Dim CurrentRequest As System.Web.HttpRequest = Nothing
                Try
                    CurrentRequest = HttpContext.Current.Request
                Catch
                    'above statement might fail in certain circumstances (e.g. build failures?!?) with
                    'Anforderung steht in diesem Kontext nicht zur Verfügung bei System.Web.HttpContext.get_Request()
                End Try
                If Me.CurrentServerIdentString <> Nothing Then
                    AssemblyLocation = "ServerIdentString=" & Me.CurrentServerIdentString & ControlChars.NewLine
                    If Not CurrentRequest Is Nothing Then
                        AssemblyLocation &= "RequestUrl=" & CurrentRequest.Url.Scheme & Uri.SchemeDelimiter & CurrentRequest.Url.Host & ":" & CurrentRequest.Url.Port & CurrentRequest.ApplicationPath
                    Else
                        AssemblyLocation &= "Request={Nothing}"
                    End If
                    Try
                        AssemblyLocation &= ControlChars.NewLine & "ServerUrl=" & Me.CurrentServerInfo.ServerURL & CurrentRequest.ApplicationPath
                    Catch
                    End Try
                Else
                    AssemblyLocation = "ServerIdentString={not yet configured at execution time}" & ControlChars.NewLine
                    If Not CurrentRequest Is Nothing Then
                        AssemblyLocation &= "RequestUrl=" & CurrentRequest.Url.Scheme & Uri.SchemeDelimiter & CurrentRequest.Url.Host & ":" & CurrentRequest.Url.Port & CurrentRequest.ApplicationPath
                    Else
                        AssemblyLocation &= "Request={Nothing}"
                    End If
                End If
            End If
            Return AssemblyLocation
        End Function

        ''' <summary>
        ''' A unique hash ID for each logical (IIS) web application
        ''' </summary>
        Friend Function CurrentWebAppInstanceID() As String
            Return Utils.ComputeHash(CurrentWebAppAssemblyLocation)
        End Function

        ''' <summary>
        ''' Log the current assembly version and its execution location to the database
        ''' </summary>
        ''' <remarks>
        ''' This feature assists the website administrator to review potential breakdowns before he updates the database build no. A newer database version than the CWM assembly version which would lead to an application break-down.
        ''' </remarks>
        Private Sub LogAssemblyVersionToDatabaseOnce()
            WebManager.Log.WriteEventLogTrace("LogAssemblyVersionToDatabaseOnce:Begin")
            Dim DoLogging As Boolean = False
            Static _AssemblyVersionAlreadyLoggedToDatabase As Boolean
            If HttpContext.Current Is Nothing Then
                If _AssemblyVersionAlreadyLoggedToDatabase = False Then DoLogging = True
            Else
                If CType(HttpContext.Current.Application("WebManager.AssemblyVersionAlreadyLoggedToDatabase"), Boolean) = False Then DoLogging = True
            End If
            'Only execute following block if it hasn't been executed in this application instance
            If DoLogging = True Then
                Dim AssemblyLocation As String
                Dim AssemblyBuildNo As Integer = Setup.ApplicationUtils.Version.Build
                Dim AssemblyLocationHash As String
                Try
                    AssemblyLocation = CurrentWebAppAssemblyLocation()
                    AssemblyLocationHash = Utils.ComputeHash(AssemblyLocation)
                Catch ex As Exception
                    AssemblyLocation = "Error while looking up current assembly location"
                    AssemblyLocation &= ": " & ControlChars.NewLine & ex.ToString
                    AssemblyLocationHash = "{ERROR detecting AssemblyLocation: " & ex.Message & "}"
                End Try
                Try
                    Dim sql As String = "DECLARE @RowNumber int" & vbNewLine & _
                        "SELECT @RowNumber = COUNT(*)" & vbNewLine & _
                        "FROM [dbo].[System_GlobalProperties]" & vbNewLine & _
                        "WHERE VALUENVarChar = N'camm WebManager' AND PropertyName = @PropertyName" & vbNewLine & _
                        "SELECT @RowNumber" & vbNewLine & _
                        "IF @RowNumber = 0 " & vbNewLine & _
                        "	INSERT INTO [dbo].[System_GlobalProperties]" & vbNewLine & _
                        "		(ValueNVarChar, PropertyName, ValueDateTime, ValueInt, ValueDecimal, ValueNText)" & vbNewLine & _
                        "	VALUES (N'camm WebManager', @PropertyName, GetDate(), @BuildNoAsIs, @BuildNoAsCompatible, @DetailsText)" & vbNewLine & _
                        "ELSE" & vbNewLine & _
                        "	UPDATE [dbo].[System_GlobalProperties]" & vbNewLine & _
                        "	SET ValueDateTime = GetDate(), ValueInt = @BuildNoAsIs, ValueDecimal = @BuildNoAsCompatible, ValueNText = @DetailsText" & vbNewLine & _
                        "	WHERE ValueNVarChar = N'camm WebManager' AND PropertyName = @PropertyName"

                    'Set flag that logging has been done for this application
                    Dim MyCmd As New SqlCommand(sql, New SqlClient.SqlConnection(Me.ConnectionString))
                    MyCmd.CommandType = CommandType.Text
                    MyCmd.Parameters.Add("@BuildNoAsIs", SqlDbType.Int).Value = AssemblyBuildNo
                    MyCmd.Parameters.Add("@BuildNoAsCompatible", SqlDbType.Decimal).Value = CompuMaster.camm.WebManager.WMSystem.Configuration.CompatibilityWithDatabaseBuild
                    MyCmd.Parameters.Add("@DetailsText", SqlDbType.NText).Value = AssemblyLocation
                    MyCmd.Parameters.Add("@PropertyName", SqlDbType.NVarChar).Value = "AppInstance_" & AssemblyLocationHash
                    CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                Catch
                    'Ignore any errors here - we might not be ready to log or throw anything yet
                End Try
                If HttpContext.Current Is Nothing Then
                    _AssemblyVersionAlreadyLoggedToDatabase = True
                Else
                    HttpContext.Current.Application("WebManager.AssemblyVersionAlreadyLoggedToDatabase") = True
                End If
            End If
            WebManager.Log.WriteEventLogTrace("LogAssemblyVersionToDatabaseOnce:End")
        End Sub

        Friend Sub SetBasePage(ByVal Page As System.Web.UI.Page)
            MyBase.Page = Page
        End Sub

        Friend Event InitLoadConfiguration()

        Friend Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            RaiseEvent InitLoadConfiguration()
            InitializeEnvironment() 'Ensure that this webmanager instance has been initialized even if ConnectionString or CurrentServerIdentString have never been assigned by the customized configuration load
        End Sub



        Friend Sub PageOnUnLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
            If Configuration.SuppressProductRegistrationServiceConnection = False Then
                Try
                    Dim registration As New Registration.ProductRegistration(Me)
                    If registration.IsRefreshFromRemoteLicenseServerRequired(48) Then
                        registration.CheckRegistration(False)
                    End If
                Catch ex As Exception
                    Me.Log.Exception(ex, False)
                End Try
            End If
        End Sub

        Friend Sub PageOnLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            For Each MyAttr As String In Me.Attributes.Keys
                'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                Dim WorkaroundEx As New Exception("")
                Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                Try
                    WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                Catch
                End Try
                Me.Log.RuntimeWarning("Invalid attribute """ & MyAttr & """ for camm WebManager Control in page """ & HttpContext.Current.Request.ServerVariables("SCRIPT_NAME") & """", WorkaroundStackTrace, DebugLevels.NoDebug, False, False)
            Next

            'Ensure that only supported attributes are assigned, otherwise there must be an exception because of security reasons
            If Me.Attributes.Count > 0 Then
                Dim ControlAttributes_WarningsIfNotInDebugMode As String = Nothing
                Dim ControlAttributes_UpgradeWarnings As String = Nothing
                Dim ControlAttributes_Errors As String = Nothing
                For Each MyAttribute As String In Me.Attributes.Keys

                    Select Case MyAttribute.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                        Case "securityobject", "pageadditionalheaders", _
                            "pagetitle", "pagep3pcompactpolicy"
                            'OK
                        Case "connectionstring", "currentserveridentstring", _
                            "smtpserverport", "smtpservername", _
                            "technicalserviceemailaccountaddress", "technicalserviceemailaccountname", _
                            "standardemailaccountaddress", "standardemailaccountname", _
                            "developmentemailaccountaddress"
                            'Shouldn't be configured here, but okay for debugging and development
                            'you should use config.vb instead
                            ControlAttributes_WarningsIfNotInDebugMode &= ", " & MyAttribute
                        Case "user_auth_config_cursmtpserver_port", _
                            "user_auth_config_cursmtpserver", "additionalbodytags"
                            'Upgrade warning
                            ControlAttributes_UpgradeWarnings &= ", " & MyAttribute
                        Case "system_debuglevel", "debuglevel"
                            'OK
                        Case "validateaccess"
                            'Enable compatibility, but also warn
                            If _SecurityObject = "" AndAlso Me.Attributes.Item(MyAttribute) <> "" Then
                                _SecurityObject = Me.Attributes.Item(MyAttribute)
                            End If
                            ControlAttributes_UpgradeWarnings &= ", " & MyAttribute
                        Case Else
                            ControlAttributes_Errors &= ", " & MyAttribute
                    End Select
                    'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                    Dim WorkaroundEx As New Exception("")
                    Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                    Try
                        WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                    Catch
                    End Try
                    If ControlAttributes_WarningsIfNotInDebugMode <> "" AndAlso Me.System_DebugLevel = DebugLevels.NoDebug Then
                        Me.Log.RuntimeWarning("Should not be used in non debug levels: " & Mid(ControlAttributes_WarningsIfNotInDebugMode, 3), WorkaroundStackTrace, DebugLevels.NoDebug, False, False)
                    End If
                    If ControlAttributes_UpgradeWarnings <> "" Then
                        Me.Log.RuntimeWarning("Upgrade warning: these property might not be available in a newer camm Web-Manager version: " & Mid(ControlAttributes_UpgradeWarnings, 3), WorkaroundStackTrace, DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails, False, False)
                    End If
                    If ControlAttributes_Errors <> "" Then
                        Dim Message As String = "Not allowed or unknown attribut for camm Web-Manager user control: " & Mid(ControlAttributes_Errors, 3)
                        Me.Log.RuntimeException(Message)
                    End If
                Next
            End If

            If GetType(CompuMaster.camm.WebManager.Pages.Specialized.ErrorPage).IsInstanceOfType(Me.Page) = False _
                AndAlso GetType(CompuMaster.camm.WebManager.Pages.Application.BaseErrorPage).IsInstanceOfType(Me.Page) = False _
                AndAlso GetType(CompuMaster.camm.WebManager.Pages.Application.BaseWarningPage).IsInstanceOfType(Me.Page) = False _
                AndAlso _SecurityObject <> "" AndAlso Me._SecurityObject <> Me.SecurityObjectSuccessfullyTested Then
                AuthorizeDocumentAccess()
            End If


        End Sub



        ''' <summary>
        ''' Enumeration of available behaviours for property AutoSecurityCheckLogsPageAccess
        ''' </summary>
        Public Enum AutoSecurityCheckLogsPageHit As Byte
            OnEveryRequest = 0
            NotOnPostBack = 1
            Never = 2
        End Enum

        Private _AutoSecurityCheckLogsPageAccess As AutoSecurityCheckLogsPageHit
        ''' <summary>
        ''' If the authorization of the current user to the current security object is checked by camm Web-Manager automatisms, how shall the page access be logged?
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' As soon as you set up a value for property SecurityObject, camm Web-Manager automatically checks if the current user is really authorized to access this security object.
        ''' By default, all security validations will be handled as a page access in the log analysis. In some cases, it may be useful to not log every access as a hit. For this, you can setup the required behaviour regarding the logging of hits.
        ''' </remarks>
        Public Property AutoSecurityCheckLogsPageAccess() As AutoSecurityCheckLogsPageHit
            Get
                Return _AutoSecurityCheckLogsPageAccess
            End Get
            Set(ByVal Value As AutoSecurityCheckLogsPageHit)
                _AutoSecurityCheckLogsPageAccess = Value
            End Set
        End Property

        Friend Sub PageOnPreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'Writes the compact policy to the page headers
            If _CompactPolicyHeader <> "" Then
                Page.Response.AddHeader("CP", _CompactPolicyHeader)
            End If
        End Sub

        ''' <summary>
        '''     Creates a new instance of the camm Web-Manager
        ''' </summary>
        ''' <param name="databaseConnectionString">The connection string to the database</param>
        Public Sub New(ByVal databaseConnectionString As String)
            Me.New(databaseConnectionString, False)
        End Sub

        ''' <summary>
        ''' Compatibility check library vs. database version: standard behaviour must be to ensure data integrity starting with checks for too old CWM libs not being trusted to cooperate correctly in all situations with newer CWM database version
        ''' </summary>
        ''' <remarks></remarks>
        Friend _ignoreCheckCompatibilityToDatabaseByBuildNumber As Boolean = False
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub New(ByVal databaseConnectionString As String, ignoreCheckCompatibilityToDatabaseByBuildNumber As Boolean)
            MyBase.New()
            _ignoreCheckCompatibilityToDatabaseByBuildNumber = ignoreCheckCompatibilityToDatabaseByBuildNumber
            _ConnectionString = databaseConnectionString
            If _ConnectionString <> Nothing Then
                LogAssemblyVersionToDatabaseOnce()
                If _ignoreCheckCompatibilityToDatabaseByBuildNumber = False Then CheckCompatibilityToDatabaseByBuildNumber()
            End If
            If HttpContext.Current Is Nothing Then
                'Non-web application should initialize localization for English universal (equals the first element of property ActiveMarkets)
                Me.UIMarket(Me.ActiveMarkets(0))
            End If
            'Security object defined in web.config should be used as default
            Me.SecurityObject = CompuMaster.camm.WebManager.Configuration.SecurityObject
        End Sub

        Private Shared _AdditionalConfiguration As New System.Collections.Specialized.NameValueCollection
        ''' <summary>
        ''' Additional configuration values setup from e.g. global.asax in MS Azure cloud service environments
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Config values are considered if the configuration value hasn't been found in web.config (missing or empty value)</remarks>
        Public Shared ReadOnly Property AdditionalConfiguration As System.Collections.Specialized.NameValueCollection
            Get
                Return _AdditionalConfiguration
            End Get
        End Property

        ''' <summary>
        '''     Creates a new instance of the camm Web-Manager
        ''' </summary>
        <Obsolete("You should not create a camm Web-Manager instance by yourself in web applications, use the cammWebManager property or cammWebManager object created in the aspx page itself")> _
        Public Sub New()
            MyBase.New()
            If HttpContext.Current Is Nothing Then
                'Non-web application should initialize localization for English universal (equals the first element of property ActiveMarkets)
                Me.UIMarket(Me.ActiveMarkets(0))
            End If
            'Security object defined in app/web.config should be used as default
            Me.SecurityObject = CompuMaster.camm.WebManager.Configuration.SecurityObject
        End Sub

        ''' <summary>
        '''     Creates a new instance of the camm Web-Manager - internal version to prevent the obsoletion warning
        ''' </summary>
        Friend Sub New(ByVal dummy As System.Type)
            MyBase.New()
            If HttpContext.Current Is Nothing Then
                'Non-web application should initialize localization for English universal (equals the first element of property ActiveMarkets)
                Throw New Exception("Web application required for internal creator")
            End If
            'Security object defined in web.config should be used as default
            Me.SecurityObject = CompuMaster.camm.WebManager.Configuration.SecurityObject
        End Sub

        'TODO: change parameter scriptEngineID to type WMSystem.ScriptEngines
        ''' <summary>
        '''     Create an absolute URL with protocol, server name and port, the path and if required a session ID modifier
        ''' </summary>
        ''' <param name="serverID">The server this URL shall target to</param>
        ''' <param name="scriptEngineID">The script engine ID in charge</param>
        ''' <param name="pathFromRootDirectory">The path to the requested file on the server</param>
        ''' <history>
        ''' 	[adminsupport]	23.04.2005	Created
        '''
        ''' </history>
        Public Overridable Function CalculateUrl(ByVal serverID As Integer, ByVal scriptEngineID As Integer, ByVal pathFromRootDirectory As String) As String
            Dim server As ServerInformation
            Dim ServerOfAnotherServerGroup As Boolean = False
            'Retrieve the required server info object
            If serverID = Me.CurrentServerInfo.ID Then
                server = Me.CurrentServerInfo
            Else
                server = New ServerInformation(serverID, Me)
                If server.ParentServerGroupID <> Me.CurrentServerInfo.ParentServerGroupID Then
                    'We never can add the correct session ID since there is no data available on that server
                    ServerOfAnotherServerGroup = True
                End If
            End If
            'Retrieve the appropriate session ID
            Dim ServerSessionID As String = Nothing
            If ServerOfAnotherServerGroup = False Then
                ServerSessionID = LookupScriptEngineSessionID(serverID, scriptEngineID)
            End If
            Dim UrlPartForSession As String = Nothing
            If ServerSessionID <> Nothing Then
                Select Case scriptEngineID
                    Case 1, 2 'ASP, ASP.NET
                        UrlPartForSession = "/(" & ServerSessionID & ")"
                    Case Else 'PHP and others
                        UrlPartForSession = "/(" & ServerSessionID & ")" 'ToDo: use correct syntax, handle 
                End Select
            End If
            'Construct the new URL
            Return CalculateUrlConstructor(server.ServerURL, scriptEngineID, UrlPartForSession, pathFromRootDirectory)
        End Function

        'TODO: change parameter scriptEngineID to type WMSystem.ScriptEngines
        ''' <summary>
        '''     Create the new URL by inserting the given pieces of the new URL at the correct position
        ''' </summary>
        ''' <param name="serverUrlWithoutTrailingSlash">Protocol and server name and port</param>
        ''' <param name="scriptEngineID">The script engine ID in charge</param>
        ''' <param name="urlPartForSessionIDOfScriptEngine">The session ID string as it can be inserted into the URL inclusive a leading path separator</param>
        ''' <param name="pathFromRootDirectory">The path to the requested file on the server</param>
        Protected Overridable Function CalculateUrlConstructor(ByVal serverUrlWithoutTrailingSlash As String, ByVal scriptEngineID As Integer, ByVal urlPartForSessionIDOfScriptEngine As String, ByVal pathFromRootDirectory As String) As String
            If Configuration.CookieLess AndAlso urlPartForSessionIDOfScriptEngine <> "" Then
                Return serverUrlWithoutTrailingSlash & urlPartForSessionIDOfScriptEngine & pathFromRootDirectory
            Else
                Return serverUrlWithoutTrailingSlash & pathFromRootDirectory
            End If
        End Function

        ''' <summary>
        ''' Methods for fast data querying
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property PerformanceMethods() As PerformanceMethods Implements IWebManager.PerformanceMethods
            Get
                Static _PerformanceMethods As CompuMaster.camm.WebManager.PerformanceMethods
                If _PerformanceMethods Is Nothing Then _PerformanceMethods = New CompuMaster.camm.WebManager.PerformanceMethods(Me)
                Return _PerformanceMethods
            End Get
        End Property

        ''' <summary>
        '''     Get the next relogon address to make the roundtrip to all web servers and their script engines and to enable/refresh the session state
        ''' </summary>
        ''' <param name="LoginNameOfUser">Login name of the user</param>
        ''' <returns>A URL where to redirect to</returns>
        Public Function System_GetNextLogonURI(ByVal LoginNameOfUser As String) As String
            Dim MyBuffer As String
            Dim MyDBConn As New SqlConnection
            Dim MyRecSet As SqlDataReader = Nothing
            Dim MyCmd As New SqlCommand

            If LoginNameOfUser = "" Then Return Nothing

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "Public_GetToDoLogonList"
                    .CommandType = CommandType.StoredProcedure

                    .Parameters.Add("@Username", SqlDbType.NVarChar).Value = LoginNameOfUser
                    .Parameters.Add("@ScriptEngine_SessionID", SqlDbType.NVarChar).Value = CurrentScriptEngineSessionID
                    If HttpContext.Current Is Nothing Then
                        .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = ScriptEngines.NetClient
                    Else
                        .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = ScriptEngines.ASPNet
                    End If

                    'Since DB Build 110, there is an additional parameter
                    If Not HttpContext.Current Is Nothing AndAlso Utils.TryCInt(HttpContext.Current.Application("WebManager.CurrentDBBuildAtLeast")) >= 110 Then
                        .Parameters.Add("@ServerID", SqlDbType.Int).Value = Me.System_GetServerID()
                    Else
                        If Setup.DatabaseUtils.Version(Me, True).Build >= 110 Then
                            .Parameters.Add("@ServerID", SqlDbType.Int).Value = Me.System_GetServerID()
                            If Not HttpContext.Current Is Nothing Then
                                HttpContext.Current.Application("WebManager.CurrentDBBuildAtLeast") = Setup.DatabaseUtils.Version(Me, True).Build
                            End If
                        End If
                    End If

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                MyRecSet = MyCmd.ExecuteReader()

                MyBuffer = ""
                If MyRecSet.Read Then
                    If Configuration.CookieLess = False OrElse Utils.TryCInt(HttpContext.Current.Application("WebManager.CurrentDBBuildAtLeast")) < 121 Then
                        'Fast URL construction
                        MyBuffer = CType(MyRecSet("ServerProtocol"), String) & "://" & CType(MyRecSet("ServerName"), String)
                        If Not IsDBNull(MyRecSet("ServerPort")) Then
                            MyBuffer = MyBuffer & ":" & CType(MyRecSet("ServerPort"), String)
                        End If
                        MyBuffer = MyBuffer & Internationalization.User_Auth_Config_Paths_Login & CType(MyRecSet("FileName_EngineLogin"), String) & "?GUID=" & System.Web.HttpUtility.UrlEncode(CType(MyRecSet("ScriptEngine_LogonGUID"), String)) & "&User=" & System.Web.HttpUtility.UrlEncode(LoginNameOfUser) & "&Dat=" & Hour(Now) & Minute(Now) & Second(Now)
                    Else
                        'Correct URL construction
                        MyBuffer = CalculateUrl(Me.CurrentServerInfo.ID, CType(MyRecSet("ScriptEngine_ID"), Integer), Internationalization.User_Auth_Config_Paths_Login & CType(MyRecSet("FileName_EngineLogin"), String) & "?GUID=" & System.Web.HttpUtility.UrlEncode(CType(MyRecSet("ScriptEngine_LogonGUID"), String)) & "&User=" & System.Web.HttpUtility.UrlEncode(LoginNameOfUser) & "&Dat=" & Hour(Now) & Minute(Now) & Second(Now))
                    End If
                End If

                System_GetNextLogonURI = MyBuffer

            Finally
                If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then
                    MyRecSet.Close()
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

        End Function

        ''' <summary>
        ''' Checks whether database schema supports multiple password algorithms. Older versions don't.
        ''' </summary>
        ''' <remarks></remarks>
        Public Function System_SupportsMultiplePasswordAlgorithms() As Boolean
            Return Setup.DatabaseUtils.Version(Me, True).Build >= 195
        End Function

        ''' <summary>
        ''' Fetches the default algorithm selected by the admin
        ''' </summary>
        ''' <remarks></remarks>
        Public Function System_GetDefaultPasswordAlgorithm() As PasswordAlgorithm
            If Not System_SupportsMultiplePasswordAlgorithms() Then
                Return PasswordAlgorithm.EncDecModSimple
            End If

            Dim MyCmd As New SqlClient.SqlCommand
            MyCmd.Connection = New SqlClient.SqlConnection(ConnectionString)
            MyCmd.CommandText = "SELECT ValueInt FROM [dbo].System_GlobalProperties WHERE PropertyName = 'LoginPWAlgorithm' AND ValueNVarChar = 'camm WebManager'"
            MyCmd.CommandType = CommandType.Text

            Dim fallbackAlgorithm As PasswordAlgorithm
#If NetFrameWork = "1_1" Then
            fallbackAlgorithm = PasswordAlgorithm.EncDecModSimple
#Else
            fallbackAlgorithm = PasswordAlgorithm.AES256
#End If

            Dim algo As Integer = CType(CompuMaster.camm.WebManager.Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), fallbackAlgorithm), Integer)
            Return CType(algo, PasswordAlgorithm)

        End Function

        ''' <summary>
        ''' Sets the default algorithm 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub System_SetDefaultPasswordAlgorithm(ByVal algo As PasswordAlgorithm)
            If System_SupportsMultiplePasswordAlgorithms() Then
                Dim MyCmd As New SqlClient.SqlCommand
                MyCmd.Connection = New SqlClient.SqlConnection(ConnectionString)
                MyCmd.CommandText = "UPDATE [dbo].System_GlobalProperties SET ValueInt = @algo WHERE PropertyName = 'LoginPWAlgorithm' AND ValueNVarChar = 'camm WebManager'"
                MyCmd.CommandType = CommandType.Text
                MyCmd.Parameters.Add("@algo", SqlDbType.Int).Value = algo

                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            End If
        End Sub


        ''' <summary>
        ''' Returns the behavior chosen for password recovery by the administrator
        ''' </summary>
        ''' <remarks></remarks>
        Public Function System_GetPasswordRecoveryBehavior() As PasswordRecoveryBehavior
            If Not System_SupportsMultiplePasswordAlgorithms() Then
                Return PasswordRecoveryBehavior.DecryptIfPossible
            End If

            Dim MyCmd As New SqlClient.SqlCommand
            MyCmd.Connection = New SqlClient.SqlConnection(ConnectionString)
            MyCmd.CommandText = "SELECT ValueInt FROM [dbo].System_GlobalProperties WHERE PropertyName = 'PasswordResetBehavior' AND ValueNVarChar = 'camm WebManager'"
            MyCmd.CommandType = CommandType.Text

            Dim behavior As Integer = CType(CompuMaster.camm.WebManager.Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), PasswordRecoveryBehavior.DecryptIfPossible), Integer)
            Return CType(behavior, PasswordRecoveryBehavior)
        End Function


        ''' <summary>
        ''' Sets the password recovery behavior
        ''' </summary>
        ''' <param name="behavior"></param>
        ''' <remarks></remarks>
        Public Sub System_SetPasswordRecoveryBehavior(ByVal behavior As PasswordRecoveryBehavior)
            If System_SupportsMultiplePasswordAlgorithms() Then
                Dim MyCmd As New SqlClient.SqlCommand
                MyCmd.Connection = New SqlClient.SqlConnection(ConnectionString)
                MyCmd.CommandText = "UPDATE [dbo].System_GlobalProperties SET ValueInt = @behavior WHERE PropertyName = 'PasswordResetBehavior' AND ValueNVarChar = 'camm WebManager'"
                MyCmd.CommandType = CommandType.Text
                MyCmd.Parameters.Add("@behavior", SqlDbType.Int).Value = behavior

                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            End If
        End Sub

        ''' <summary>
        ''' Returns the crypted password of a user
        ''' </summary>
        ''' <param name="username">user who's crypted password should be returned.</param>
        ''' <remarks></remarks>
        Public Function System_GetCryptedUserPassword(ByVal username As String) As String
            Dim MyCmd As New SqlClient.SqlCommand
            MyCmd.Connection = New SqlClient.SqlConnection(Me.ConnectionString)
            MyCmd.CommandText = "SELECT LoginPW FROM [dbo].Benutzer WHERE LoginName = @username"
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username

            Dim password As String = CType(CompuMaster.camm.WebManager.Utils.Nz(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)), String)
            Return password
        End Function


        ''' <summary>
        ''' Returns the transformation result structure, containing the crypted password, algorithm and IV/salt used. 
        ''' </summary>
        ''' <param name="userName">u</param>
        ''' <remarks></remarks>
        Public Function System_GetUserPasswordTransformationResult(ByVal username As String) As CryptoTransformationResult
            Dim result As New CryptoTransformationResult
            If Not Me.System_SupportsMultiplePasswordAlgorithms() Then
                result.Algorithm = PasswordAlgorithm.EncDecModSimple
                result.Noncevalue = Nothing
                result.TransformedText = Me.System_GetCryptedUserPassword(username)
            Else
                Dim MyCmd As New SqlClient.SqlCommand
                MyCmd.Connection = New SqlClient.SqlConnection(Me.ConnectionString)
                MyCmd.CommandText = "SELECT LoginPWAlgorithm, LoginPWNonceValue, LoginPW FROM [dbo].Benutzer WHERE LoginName = @username"
                MyCmd.CommandType = CommandType.Text
                MyCmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username

                Dim reader As System.Data.IDataReader = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReader(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                If reader.Read() Then
                    Dim record As System.Data.IDataRecord = CType(reader, System.Data.IDataRecord)
                    result.Algorithm = CType(record(0), PasswordAlgorithm)
                    result.Noncevalue = CType(CompuMaster.camm.WebManager.Utils.Nz(record(1)), Byte())
                    result.TransformedText = CType(CompuMaster.camm.WebManager.Utils.Nz(record(2)), String)
                End If
                reader.Close()
            End If
            Return result
        End Function


        Public Sub System_UpdateUserTransformationResult(ByVal loginname As String, ByVal transformationResult As CryptoTransformationResult)
            If System_SupportsMultiplePasswordAlgorithms() Then
                Dim MyCmd As New SqlClient.SqlCommand
                MyCmd.Connection = New SqlClient.SqlConnection(ConnectionString)
                MyCmd.CommandText = "UPDATE [dbo].Benutzer SET LoginPW = @password, LoginPWAlgorithm = @Algo, LoginPWNonceValue = @Param WHERE Loginname = @User"
                MyCmd.CommandType = CommandType.Text
                MyCmd.Parameters.Add("@password", SqlDbType.VarChar).Value = transformationResult.TransformedText
                MyCmd.Parameters.Add("@Algo", SqlDbType.Int).Value = transformationResult.Algorithm
                MyCmd.Parameters.Add("@Param", SqlDbType.VarBinary).Value = IIf(transformationResult.Noncevalue Is Nothing, New Byte() {0}, transformationResult.Noncevalue)
                MyCmd.Parameters.Add("@User", SqlDbType.VarChar).Value = loginname
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            Else
                Dim MyCmd As New SqlClient.SqlCommand
                MyCmd.Connection = New SqlClient.SqlConnection(ConnectionString)
                MyCmd.CommandText = "UPDATE [dbo].Benutzer SET LoginPW = @password WHERE Loginname = @User"
                MyCmd.CommandType = CommandType.Text
                MyCmd.Parameters.Add("@password", SqlDbType.VarChar).Value = transformationResult.TransformedText
                MyCmd.Parameters.Add("@User", SqlDbType.VarChar).Value = loginname
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            End If
        End Sub

        ''' <summary>
        '''     Is the user's camm Web-Manager session terminated?
        ''' </summary>
        ''' <param name="LoginNameOfUser">The login name to be checked</param>
        ''' <returns>True when the session has ended</returns>
        Public Function System_IsSessionTerminated(ByVal LoginNameOfUser As String) As Boolean
            Dim MyDBConn As New SqlConnection
            Dim MyRecSet As SqlDataReader = Nothing
            Dim MyCmd As New SqlCommand

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "Public_GetLogonList"
                    .CommandType = CommandType.StoredProcedure

                    .Parameters.Add("@Username", SqlDbType.NVarChar).Value = LoginNameOfUser

                    'Since DB Build 110, there is an additional parameter
                    If Not HttpContext.Current Is Nothing AndAlso Utils.TryCInt(HttpContext.Current.Application("WebManager.CurrentDBBuildAtLeast")) >= 110 Then
                        .Parameters.Add("@ScriptEngine_SessionID", SqlDbType.NVarChar).Value = CurrentScriptEngineSessionID
                        .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = ScriptEngines.ASPNet
                        .Parameters.Add("@ServerID", SqlDbType.Int).Value = Me.System_GetServerID()
                    Else
                        If Setup.DatabaseUtils.Version(Me, True).Build >= 110 Then
                            .Parameters.Add("@ScriptEngine_SessionID", SqlDbType.NVarChar).Value = CurrentScriptEngineSessionID
                            .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = ScriptEngines.NetClient
                            .Parameters.Add("@ServerID", SqlDbType.Int).Value = Me.System_GetServerID()
                            If Not HttpContext.Current Is Nothing Then
                                HttpContext.Current.Application("WebManager.CurrentDBBuildAtLeast") = Setup.DatabaseUtils.Version(Me, True).Build
                            End If
                        End If
                    End If

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                MyRecSet = MyCmd.ExecuteReader()

                If MyRecSet.Read Then
                    System_IsSessionTerminated = False
                Else
                    System_IsSessionTerminated = True
                End If

            Finally
                If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then
                    MyRecSet.Close()
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

        End Function

        ''' <summary>
        '''     Try to get an earlier unclosed Web-Manager session with our webserver session ID
        ''' </summary>
        ''' <remarks>
        '''     Only for internal use; this method is not intended for execution by your applications
        ''' </remarks>
        Friend Sub TryToRetrieveUserNameFromScriptEngineSessionID()
            Try
                Dim FoundUsername As String = System_GetUserNameByScriptEngineSessionID()
                If FoundUsername <> "" Then
                    SetUserLoginName(FoundUsername)
                End If
            Catch
                'Ignore errors
            End Try
        End Sub

        ''' <summary>
        '''     Get the next relogon address to make the roundtrip to all web servers and their script engines and to enable/refresh the session state
        ''' </summary>
        ''' <returns>A URL where to redirect to</returns>
        Public Function System_GetNextLogonURIOfUserAnonymous() As String
            Dim MyBuffer As String
            Dim MyDBConn As New SqlConnection
            Dim MyRecSet As SqlDataReader = Nothing
            Dim MyCmd As New SqlCommand
            Dim MyCounter As Integer
            Dim Jump2RecordNo As Integer

            If System.Web.HttpContext.Current.Request.QueryString("LogonID") <> "" Then
                Jump2RecordNo = CType(System.Web.HttpContext.Current.Request.QueryString("LogonID"), Integer)
            Else
                Jump2RecordNo = 1
            End If

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "Public_GetCurServerLogonList"
                    .CommandType = CommandType.StoredProcedure

                    .Parameters.Add("@ServerIP", SqlDbType.NVarChar).Value = CurrentServerIdentString

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                MyRecSet = MyCmd.ExecuteReader()

                MyBuffer = ""
                Do While MyRecSet.Read And Not MyCounter >= Jump2RecordNo
                    MyCounter = MyCounter + 1
                    If MyCounter >= Jump2RecordNo Then
                        If Configuration.CookieLess = False OrElse Utils.TryCInt(HttpContext.Current.Application("WebManager.CurrentDBBuildAtLeast")) < 121 Then
                            'Fast URL construction
                            MyBuffer = CType(MyRecSet("ServerProtocol"), String) & "://" & CType(MyRecSet("ServerName"), String)
                            If Not IsDBNull(MyRecSet("ServerPort")) Then
                                MyBuffer = MyBuffer & ":" & CType(MyRecSet("ServerPort"), String)
                            End If
                            MyBuffer = MyBuffer & Internationalization.User_Auth_Config_Paths_Login & CType(MyRecSet("FileName_EngineLogin"), String) & "?LogonID=" & (MyCounter + 1)
                        Else
                            'Correct URL construction
                            'ToDo: read the correct session ID from database, but this requires that also anonymous user sessions can be tracked
                            'MyBuffer = CalculateUrl(Me.CurrentServerInfo.ID, CType(MyRecSet("ScriptEngine_ID"), Integer), Internationalization.User_Auth_Config_Paths_Login & CType(MyRecSet("FileName_EngineLogin"), String) & "?LogonID=" & (MyCounter + 1))
                            MyBuffer = CalculateUrl(Me.CurrentServerInfo.ID, Nothing, Internationalization.User_Auth_Config_Paths_Login & CType(MyRecSet("FileName_EngineLogin"), String) & "?LogonID=" & (MyCounter + 1))
                        End If
                        Exit Do
                    End If
                Loop

                System_GetNextLogonURIOfUserAnonymous = MyBuffer

            Finally
                If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then
                    MyRecSet.Close()
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

        End Function

        ''' <summary>
        '''     Get the URL of the requested server
        ''' </summary>
        ''' <param name="ServerIP">The server identification string to get the URL from</param>
        ''' <returns>A URL like https://www.yourcompany.com without a trailing slash</returns>
        Public Function System_GetServerURL(ByVal ServerIP As String) As String
            Dim ServerURLs As System.Collections.Specialized.NameValueCollection
            If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Application("WebManager.ServerURLs") Is Nothing Then
                ServerURLs = CType(HttpContext.Current.Application("WebManager.ServerURLs"), System.Collections.Specialized.NameValueCollection)
                If ServerURLs(ServerIP) <> "" Then
                    Return ServerURLs(ServerIP)
                End If
            Else
                ServerURLs = New System.Collections.Specialized.NameValueCollection
            End If

            Dim MyBuffer As String
            Dim MyDBConn As New SqlConnection
            Dim MyRecSet As SqlDataReader = Nothing
            Dim MyCmd As New SqlCommand

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "Public_GetServerConfig"
                    .CommandType = CommandType.StoredProcedure

                    .Parameters.Add("@ServerIP", SqlDbType.NVarChar).Value = ServerIP

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                MyRecSet = MyCmd.ExecuteReader()

                MyBuffer = ""
                If MyRecSet.Read Then
                    MyBuffer = CType(MyRecSet("ServerProtocol"), String) & "://" & CType(MyRecSet("ServerName"), String)
                    If Not IsDBNull(MyRecSet("ServerPort")) Then
                        If LCase(CType(MyRecSet("ServerProtocol"), String)) = "http" AndAlso CType(MyRecSet("ServerPort"), Integer) = 80 Then
                            'do nothing, it's a default port
                        ElseIf LCase(CType(MyRecSet("ServerProtocol"), String)) = "https" AndAlso CType(MyRecSet("ServerPort"), Integer) = 443 Then
                            'do nothing, it's a default port
                        Else
                            MyBuffer &= ":" & CType(MyRecSet("ServerPort"), String)
                        End If
                    End If
                End If

                System_GetServerURL = MyBuffer

                ServerURLs(ServerIP) = MyBuffer
                If Not HttpContext.Current Is Nothing Then
                    HttpContext.Current.Application("WebManager.ServerURLs") = ServerURLs
                End If

            Finally
                If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then
                    MyRecSet.Close()
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

        End Function

        ''' <summary>
        '''     Get the URL of the master server of the requested server
        ''' </summary>
        ''' <param name="ServerIP">The server identification string to get the master server URL from</param>
        ''' <returns>A URL like https://www.yourcompany.com without a trailing slash</returns>
        Public Function System_GetMasterServerURL(ByVal ServerIP As String) As String
            Dim MasterServerProtocol As String = CType(System_GetServerConfig(ServerIP, "MasterServerProtocol"), String)
            Dim MasterServerName As String = CType(System_GetServerConfig(ServerIP, "MasterServerName"), String)
            Dim MasterServerPort As Integer = CType(System_GetServerConfig(ServerIP, "MasterServerPort"), Integer)
            If MasterServerPort = Nothing Then
                Return Utils.RemoveTrailingSlash(New UriBuilder(MasterServerProtocol, MasterServerName).ToString)
            ElseIf LCase(MasterServerProtocol) = "http" AndAlso MasterServerPort = 80 Then
                Return Utils.RemoveTrailingSlash(New UriBuilder(MasterServerProtocol, MasterServerName).ToString)
            ElseIf LCase(MasterServerProtocol) = "https" AndAlso MasterServerPort = 443 Then
                Return Utils.RemoveTrailingSlash(New UriBuilder(MasterServerProtocol, MasterServerName).ToString)
            Else
                Return Utils.RemoveTrailingSlash(New UriBuilder(MasterServerProtocol, MasterServerName, MasterServerPort).ToString)
            End If
        End Function

        ''' <summary>
        '''     Get the URL of the administration server of the requested server
        ''' </summary>
        ''' <param name="ServerIP">The server identification string to get the administration server URL from</param>
        ''' <returns>A URL like https://www.yourcompany.com/sysdata/admin/ with a trailing slash</returns>
        ''' <remarks>
        '''     The returned URL also contains the session ID in cookie-less environments
        ''' </remarks>
        Public Function System_GetUserAdminServer_SystemURL(ByVal ServerIP As String) As String
            If Configuration.CookieLess = False AndAlso Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Application("WebManager.AdminServerURL") Is Nothing AndAlso Not HttpContext.Current.Application("WebManager.AdminServerURL_ServerIP") Is Nothing AndAlso CType(HttpContext.Current.Application("WebManager.AdminServerURL_ServerIP"), String) = ServerIP Then
                Return CType(HttpContext.Current.Application("WebManager.AdminServerURL"), String)
            ElseIf Configuration.CookieLess = True AndAlso Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Session Is Nothing AndAlso Not HttpContext.Current.Session("CWM_AdminServerURL") Is Nothing AndAlso Not HttpContext.Current.Session("CWM_AdminServerURL_ServerIP") Is Nothing AndAlso CType(HttpContext.Current.Session("CWM_AdminServerURL_ServerIP"), String) = ServerIP Then
                Return CType(HttpContext.Current.Session("CWM_AdminServerURL"), String)
            End If

            Dim SI As New ServerInformation(ServerIP, Me)
            Dim Result As String

            Result = CalculateUrl(SI.ParentServerGroup.AdminServer.ID, ScriptEngines.ASPNet, Internationalization.User_Auth_Config_Paths_Administration)
            If Not HttpContext.Current Is Nothing Then
                If Configuration.CookieLess = False Then
                    HttpContext.Current.Application("WebManager.AdminServerURL") = Result
                    HttpContext.Current.Application("WebManager.AdminServerURL_ServerIP") = ServerIP
                ElseIf Configuration.CookieLess = True AndAlso Not HttpContext.Current.Session Is Nothing Then
                    HttpContext.Current.Session("CWM_AdminServerURL") = Result
                    HttpContext.Current.Session("CWM_AdminServerURL_ServerIP") = ServerIP
                End If
            End If

            Return Result

        End Function

        ''' <summary>
        '''     Get the server group title of the requested server
        ''' </summary>
        ''' <param name="ServerIP">A server identification string</param>
        ''' <returns>The requested server group title</returns>
        Public Function System_GetServerGroupTitle(ByVal ServerIP As String) As String
            Return CType(System_GetServerConfig(ServerIP, "ServerGroupDescription"), String)
        End Function

        ''' <summary>
        '''     Get the address to the small server group image of the requested server
        ''' </summary>
        ''' <param name="ServerIP">A server identification string</param>
        ''' <returns>An URL to the requested server group logo</returns>
        Public Function System_GetServerGroupImageSmallAddr(ByVal ServerIP As String) As String
            Return CType(System_GetServerConfig(ServerIP, "ServerGroupImageSmall"), String)
        End Function

        ''' <summary>
        '''     Get the address to the big server group image of the requested server
        ''' </summary>
        ''' <param name="ServerIP">A server identification string</param>
        ''' <returns>An URL to the requested server group logo</returns>
        Public Function System_GetServerGroupImageBigAddr(ByVal ServerIP As String) As String
            Return CType(System_GetServerConfig(ServerIP, "ServerGroupImageBig"), String)
        End Function

        ''' <summary>
        '''     Get a server ID value by the server identification string
        ''' </summary>
        ''' <param name="ServerIP">A server identification string, if missing this will be the current server</param>
        ''' <returns>A server ID</returns>
        Public Function System_GetServerID(Optional ByVal ServerIP As String = Nothing) As Integer

            If ServerIP Is Nothing Then
                ServerIP = Me.CurrentServerIdentString
            End If

            'Ask for cached servers list
            Dim MyServerInfos As ServerInformation()
            If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Application("WebManager.ServerInfos") Is Nothing Then
                MyServerInfos = CType(HttpContext.Current.Application("WebManager.ServerInfos"), ServerInformation())
            Else
                'Read and cache server list
                MyServerInfos = System_GetServersInfo()
                If Not HttpContext.Current Is Nothing Then
                    'cache only if HTTPApplication is running
                    HttpContext.Current.Application("WebManager.ServerInfos") = MyServerInfos
                End If
            End If
            'search for the ServerIP and return the ServerID
            For Each MyServerInfo As ServerInformation In MyServerInfos
                If MyServerInfo.IPAddressOrHostHeader = ServerIP Then
                    Return MyServerInfo.ID
                End If
            Next

            'The following code only executes if the server hasn't been found
            '================================================================ _

            'Reload and cache server list
            MyServerInfos = System_GetServersInfo()
            If Not HttpContext.Current Is Nothing Then
                'cache only if HTTPApplication is running
                HttpContext.Current.Application("WebManager.ServerInfos") = MyServerInfos
            End If
            'search for the ServerIP and return the ServerID
            For Each MyServerInfo As ServerInformation In MyServerInfos
                If MyServerInfo.IPAddressOrHostHeader = ServerIP Then
                    Return MyServerInfo.ID
                End If
            Next

        End Function
        ''' <summary>
        '''     Get detailled information on the current or another server
        ''' </summary>
        ''' <param name="ServerIP">A server identification string, if missing this is the current server</param>
        ''' <returns>A ServerInformation object with detailled data</returns>
        Public Function System_GetServerInfo(Optional ByVal ServerIP As String = Nothing) As ServerInformation

            If ServerIP Is Nothing Then
                ServerIP = CurrentServerIdentString
            End If

            'Ask for cached servers list
            Dim MyServerInfos As ServerInformation()
            If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Application("WebManager.ServerInfos") Is Nothing Then
                MyServerInfos = CType(HttpContext.Current.Application("WebManager.ServerInfos"), ServerInformation())
            Else
                'Read and cache server list
                MyServerInfos = System_GetServersInfo()
                If Not HttpContext.Current Is Nothing Then
                    'cache only if HTTPApplication is running
                    HttpContext.Current.Application("WebManager.ServerInfos") = MyServerInfos
                End If
            End If
            'search for the ServerIP and return the ServerID
            For Each MyServerInfo As ServerInformation In MyServerInfos
                If MyServerInfo.IPAddressOrHostHeader = ServerIP Then
                    Return MyServerInfo
                End If
            Next

            'The following code only executes if the server hasn't been found
            '================================================================ _

            'Reload and cache server list
            MyServerInfos = System_GetServersInfo()
            If Not HttpContext.Current Is Nothing Then
                'cache only if HTTPApplication is running
                HttpContext.Current.Application("WebManager.ServerInfos") = MyServerInfos
            End If
            'search for the ServerIP and return the ServerID
            For Each MyServerInfo As ServerInformation In MyServerInfos
                If MyServerInfo.IPAddressOrHostHeader = ServerIP Then
                    Return MyServerInfo
                End If
            Next

            'Otherwise return nothing
            Return Nothing

        End Function
        ''' <summary>
        '''     Query for a special property of a server configuration
        ''' </summary>
        ''' <param name="ServerIP">A server identification string</param>
        ''' <param name="PropertyName">The name of the required property</param>
        ''' <returns>The requested property value, may be DBNull</returns>
        ''' <remarks>
        '''     Once the server information has been queried, it will be cached in the application context for the next request
        ''' </remarks>
        Public Function System_GetServerConfig(ByVal ServerIP As String, ByVal PropertyName As String) As Object

            Dim MyCurServerInfo As ServerInformation = Nothing

            'Ask for cached servers list
            Dim MyServerInfos As ServerInformation()
            If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Application("WebManager.ServerInfos") Is Nothing Then
                MyServerInfos = CType(HttpContext.Current.Application("WebManager.ServerInfos"), ServerInformation())
            Else
                'Read and cache server list
                MyServerInfos = System_GetServersInfo()
                If Not HttpContext.Current Is Nothing Then
                    'demand a parent server and a masterserver property first 
                    'to read the data out of the database and to cache it, too
                    Try
                        For Each CachePrep_ServerInfo As ServerInformation In MyServerInfos
                            Dim dummy As Integer = CachePrep_ServerInfo.ParentServerGroup.MasterServer.ID
                        Next
                    Catch
                        Me.Log.RuntimeException("The database content looks to be invalid (maybe empty?)", , True)
                    End Try
                    'cache only if HTTPApplication is running
                    HttpContext.Current.Application("WebManager.ServerInfos") = MyServerInfos
                End If
            End If
            'search for the ServerIP and return the ServerID
            For Each MyServerInfo As ServerInformation In MyServerInfos
                If MyServerInfo.IPAddressOrHostHeader = ServerIP Then
                    MyCurServerInfo = MyServerInfo
                End If
            Next

            'Handle configuration errors
            If MyCurServerInfo Is Nothing Then
                Throw New ArgumentException("Server """ & ServerIP & """ not found (configuration error?)")
            ElseIf Me.DebugLevel > DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails Then
                If MyCurServerInfo.ParentServerGroup Is Nothing Then
                    Throw New InvalidProgramException("Parent server group not found")
                ElseIf MyCurServerInfo.ParentServerGroup.MasterServer Is Nothing Then
                    Throw New InvalidProgramException("Parent master server not found")
                ElseIf MyCurServerInfo.ParentServerGroup.AdminServer Is Nothing Then
                    Throw New InvalidProgramException("Parent admin server not found")
                End If
            End If

            'Return the demanded property
            Select Case PropertyName.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                Case "servergroupdescription"
                    Return MyCurServerInfo.ParentServerGroup.Title
                Case "id_group_public"
                    Return MyCurServerInfo.ParentServerGroup.GroupPublic.ID
                Case "id_group_anonymous"
                    Return MyCurServerInfo.ParentServerGroup.GroupAnonymous.ID
                Case "masterserverprotocol"
                    Return MyCurServerInfo.ParentServerGroup.MasterServer.URL_Protocol()
                Case "masterservername"
                    Return MyCurServerInfo.ParentServerGroup.MasterServer.URL_DomainName()
                Case "masterserver"
                    Return MyCurServerInfo.ParentServerGroup.MasterServer.ID
                Case "masterserverport"
                    Return MyCurServerInfo.ParentServerGroup.MasterServer.URL_Port()
                Case "useradminserverprotocol"
                    Return MyCurServerInfo.ParentServerGroup.AdminServer.URL_Protocol()
                Case "useradminservername"
                    Return MyCurServerInfo.ParentServerGroup.AdminServer.URL_DomainName()
                Case "useradminserverport"
                    Return MyCurServerInfo.ParentServerGroup.AdminServer.URL_Port()
                Case "useradminserver"
                    Return MyCurServerInfo.ParentServerGroup.AdminServer.ID()
                Case "id"
                    Return MyCurServerInfo.ID
                Case "enabled"
                    Return MyCurServerInfo.Enabled
                Case "ip"
                    Return MyCurServerInfo.IPAddressOrHostHeader
                Case "serverdescription"
                    Return MyCurServerInfo.Description
                Case "servergroup"
                    Return MyCurServerInfo.ParentServerGroup.ID
                Case "serverprotocol"
                    Return MyCurServerInfo.URL_Protocol
                Case "servername"
                    Return MyCurServerInfo.URL_DomainName
                Case "serverport"
                    Return MyCurServerInfo.URL_Port
                Case "reauthenticatebyteip"
                Case "websessiontimeout"
                Case "locktimeout"
                Case "servergroupimagebig"
                Case "servergroupimagesmall"
                Case "servergrouptitle_navigation"
                    Return MyCurServerInfo.ParentServerGroup.NavTitle
                Case "areacompanyformertitle"
                    Return MyCurServerInfo.ParentServerGroup.CompanyFormerTitle
                Case "areacompanytitle"
                    Return MyCurServerInfo.ParentServerGroup.CompanyTitle
                Case "areasecuritycontactemail"
                Case "areasecuritycontacttitle"
                Case "areadevelopmentcontactemail"
                Case "areadevelopmentcontacttitle"
                Case "areacontentmanagementcontactemail"
                Case "areacontentmanagementcontacttitle"
                Case "areaunspecifiedcontactemail"
                Case "areaunspecifiedcontacttitle"
                Case "areacopyrightsinceyear"
                Case "areacompanywebsiteurl"
                    Return MyCurServerInfo.ParentServerGroup.OfficialCompanyWebSiteURL
                Case "areacompanywebsitetitle"
                    Return MyCurServerInfo.ParentServerGroup.OfficialCompanyWebSiteTitle
                Case "id_servergroup"
                    Return MyCurServerInfo.ParentServerGroup.ID
                Case "accesslevel_default"
                    Return MyCurServerInfo.ParentServerGroup.AccessLevelDefault

                Case Else
                    Return Nothing
            End Select

            'The following code only executes if the server hasn't been found
            '================================================================ _

            Dim MyBuffer As Object
            Dim MyDBConn As New SqlConnection
            Dim MyRecSet As SqlDataReader = Nothing
            Dim MyCmd As New SqlCommand

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "Public_GetServerConfig"
                    .CommandType = CommandType.StoredProcedure

                    .Parameters.Add("@ServerIP", SqlDbType.NVarChar).Value = ServerIP

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                MyRecSet = MyCmd.ExecuteReader()

                MyBuffer = ""
                If MyRecSet.Read Then
                    MyBuffer = MyRecSet(PropertyName)
                End If

                System_GetServerConfig = MyBuffer

            Finally
                If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then
                    MyRecSet.Close()
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

        End Function

        ''' <summary>
        '''     Query the authorization status of the current user for a given security object name, but doesn't create any log records
        ''' </summary>
        ''' <param name="SecurityObjectName">A name of a security object</param>
        ''' <returns>True if the current user is authorized for that security object at the current server</returns>
        Public Function IsUserAuthorized(ByVal SecurityObjectName As String) As Boolean
            Return _IsUserAuthorized(SecurityObjectName, CType(Nothing, String), True, True)
        End Function

        ''' <summary>
        '''     Query the authorization status of the current user for a given security object name, but doesn't create any log records
        ''' </summary>
        ''' <param name="SecurityObjectName">A name of a security object</param>
        ''' <param name="ServerName">The server identification string where the security object should be validated</param>
        ''' <returns>True if the current user is authorized for that security object at the required server</returns>
        Public Function IsUserAuthorized(ByVal SecurityObjectName As String, ByVal ServerName As String) As Boolean
            Return _IsUserAuthorized(SecurityObjectName, ServerName, True, True)
        End Function

        ''' <summary>
        '''     Query the authorization status of the current user for a given security object name, but doesn't create any log records
        ''' </summary>
        ''' <param name="SecurityObjectName">A name of a security object</param>
        ''' <param name="AllowSecurityCache_Read">Allow to read this information from the cache</param>
        ''' <param name="AllowSecurityCache_Write">Allow to write this information to the cache</param>
        ''' <returns>True if the current user is authorized for that security object at the required server</returns>
        Public Function IsUserAuthorized(ByVal SecurityObjectName As String, ByVal AllowSecurityCache_Read As Boolean, ByVal AllowSecurityCache_Write As Boolean) As Boolean
            Return _IsUserAuthorized(SecurityObjectName, CType(Nothing, String), AllowSecurityCache_Read, AllowSecurityCache_Write)
        End Function

        ''' <summary>
        '''     Query the authorization status of the current user for a given security object name, but doesn't create any log records
        ''' </summary>
        ''' <param name="SecurityObjectName">A name of a security object</param>
        ''' <param name="ServerName">The server identification string where the security object should be validated</param>
        ''' <param name="AllowSecurityCache_Read">Allow to read this information from the cache</param>
        ''' <param name="AllowSecurityCache_Write">Allow to write this information to the cache</param>
        ''' <returns>True if the current user is authorized for that security object at the required server</returns>
        Public Function IsUserAuthorized(ByVal SecurityObjectName As String, ByVal ServerName As String, ByVal AllowSecurityCache_Read As Boolean, ByVal AllowSecurityCache_Write As Boolean) As Boolean
            Return _IsUserAuthorized(SecurityObjectName, ServerName, AllowSecurityCache_Read, AllowSecurityCache_Write)
        End Function

        ''' <summary>
        '''     Query the authorization status of the current user for a given security object name, but doesn't create any log records
        ''' </summary>
        ''' <param name="userID">A user ID</param>
        ''' <param name="securityObjectName">A name of a security object</param>
        ''' <param name="serverName">The server identification string where the security object should be validated</param>
        ''' <returns>True if the current user is authorized for that security object at the required server</returns>
        Public Function IsUserAuthorized(userID As Long, ByVal securityObjectName As String, ByVal serverName As String) As Boolean
            Return IsUserAuthorized(userID, SecurityObject, serverName, True, True)
        End Function

        ''' <summary>
        '''     Query the authorization status of the current user for a given security object name, but doesn't create any log records
        ''' </summary>
        ''' <param name="userID">A user ID</param>
        ''' <param name="securityObjectID">An ID of a security object</param>
        ''' <param name="serverGroupInfo">The server group where the security object shall be validated</param>
        ''' <returns>True if the current user is authorized for that security object at the required server</returns>
        Public Function IsUserAuthorized(userID As Long, ByVal securityObjectID As Integer, ByVal serverGroupInfo As ServerGroupInformation) As Boolean
            Return _IsUserAuthorized(userID, securityObjectID, "", serverGroupInfo.ID, True, True)
        End Function
        ''' <summary>
        '''     Query the authorization status of the current user for a given security object name, but doesn't create any log records
        ''' </summary>
        ''' <param name="userID">A user ID</param>
        ''' <param name="securityObjectID">An ID of a security object</param>
        ''' <param name="serverGroupID">The server group ID where the security object shall be validated</param>
        ''' <returns>True if the current user is authorized for that security object at the required server</returns>
        Public Function IsUserAuthorized(userID As Long, ByVal securityObjectID As Integer, ByVal serverGroupID As Integer) As Boolean
            Return _IsUserAuthorized(userID, securityObjectID, "", serverGroupID, True, True)
        End Function
        ''' <summary>
        '''     Query the authorization status of the current user for a given security object name, but doesn't create any log records
        ''' </summary>
        ''' <param name="userID">A user ID</param>
        ''' <param name="securityObjectName">A name of a security object (which might exist under several security object IDs)</param>
        ''' <param name="serverInfo">The server where the security object shall be validated</param>
        ''' <returns>True if the current user is authorized for that security object at the required server</returns>
        Public Function IsUserAuthorized(userID As Long, ByVal securityObjectName As String, ByVal serverInfo As ServerInformation) As Boolean
            Return _IsUserAuthorized(userID, 0, securityObjectName, serverInfo.ParentServerGroupID, True, True)
        End Function
        ''' <summary>
        '''     Query the authorization status of the current user for a given security object name, but doesn't create any log records
        ''' </summary>
        ''' <param name="userID">A user ID</param>
        ''' <param name="securityObjectName">A name of a security object (which might exist under several security object IDs)</param>
        ''' <param name="serverGroupInfo">The server group where the security object shall be validated</param>
        ''' <returns>True if the current user is authorized for that security object at the required server</returns>
        Public Function IsUserAuthorized(userID As Long, ByVal securityObjectName As String, ByVal serverGroupInfo As ServerGroupInformation) As Boolean
            Return _IsUserAuthorized(userID, 0, securityObjectName, serverGroupInfo.ID, True, True)
        End Function
        ''' <summary>
        '''     Query the authorization status of the current user for a given security object name, but doesn't create any log records
        ''' </summary>
        ''' <param name="userID">A user ID</param>
        ''' <param name="securityObjectName">A name of a security object (which might exist under several security object IDs)</param>
        ''' <param name="serverGroupID">The server group ID where the security object shall be validated</param>
        ''' <returns>True if the current user is authorized for that security object at the required server</returns>
        Public Function IsUserAuthorized(userID As Long, ByVal securityObjectName As String, ByVal serverGroupID As Integer) As Boolean
            Return _IsUserAuthorized(userID, 0, securityObjectName, serverGroupID, True, True)
        End Function

        ''' <summary>
        '''     Query the authorization status of the current user for a given security object name, but doesn't create any log records
        ''' </summary>
        ''' <param name="userID">A user ID</param>
        ''' <param name="securityObjectName">A name of a security object</param>
        ''' <param name="serverName">The server identification string where the security object should be validated</param>
        ''' <param name="allowSecurityCache_Read">Allow to read this information from the cache</param>
        ''' <param name="allowSecurityCache_Write">Allow to write this information to the cache</param>
        ''' <returns>True if the current user is authorized for that security object at the required server</returns>
        Public Function IsUserAuthorized(userID As Long, ByVal securityObjectName As String, ByVal serverName As String, ByVal allowSecurityCache_Read As Boolean, ByVal allowSecurityCache_Write As Boolean) As Boolean
            Dim UserName As String
            If userID = SpecialUsers.User_Anonymous Then
                UserName = "Anonymous"
            Else
                UserName = Me.UserLoginName(userID)
            End If
            Return Me._IsUserAuthorized(UserName, securityObjectName, serverName, allowSecurityCache_Read, allowSecurityCache_Write)
        End Function

        ''' <summary>
        '''     Query the authorization status of the current user for a given security object name, but doesn't create any log records
        ''' </summary>
        ''' <param name="SecurityObjectName">A name of a security object</param>
        ''' <param name="ServerName">The server identification string where the security object should be validated</param>
        ''' <param name="AllowSecurityCache_Read">Allow to read this information from the cache</param>
        ''' <param name="AllowSecurityCache_Write">Allow to write this information to the cache</param>
        ''' <returns>True if the current user is authorized for that security object at the required server</returns>
        <Obsolete("Use IsUserAuthorized instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_IsUserAuthorizedForApp(ByVal SecurityObjectName As String, Optional ByVal ServerName As String = Nothing, Optional ByVal AllowSecurityCache_Read As Boolean = False, Optional ByVal AllowSecurityCache_Write As Boolean = False) As Boolean
            Return _IsUserAuthorized(SecurityObjectName, ServerName, AllowSecurityCache_Read, AllowSecurityCache_Write)
        End Function

        ''' <summary>
        '''     Query the authorization status of the current user for a given security object name, but doesn't create any log records
        ''' </summary>
        ''' <param name="SecurityObjectName">A name of a security object</param>
        ''' <param name="ServerName">The server identification string where the security object should be validated</param>
        ''' <param name="AllowSecurityCache_Read">Allow to read this information from the cache</param>
        ''' <param name="AllowSecurityCache_Write">Allow to write this information to the cache</param>
        ''' <returns>True if the current user is authorized for that security object at the required server</returns>
        Private Function _IsUserAuthorized(ByVal SecurityObjectName As String, ByVal ServerName As String, ByVal AllowSecurityCache_Read As Boolean, AllowSecurityCache_Write As Boolean) As Boolean
            Dim UserName As String
            If Not Me.IsLoggedOn Then
                UserName = Nothing
            Else
                UserName = Me.CurrentUserLoginName
            End If
            Return _IsUserAuthorized(UserName, SecurityObjectName, ServerName, AllowSecurityCache_Read, AllowSecurityCache_Write)
        End Function

        Private Function _IsUserAuthorized(userID As Long, ByVal securityObjectID As Integer, ByVal securityObjectName As String, serverGroupID As Integer, ByVal AllowSecurityCache_Read As Boolean, ByVal AllowSecurityCache_Write As Boolean) As Boolean
            If userID = Nothing Then Throw New ArgumentNullException("userID")
            If securityObjectID = Nothing Xor securityObjectName = Nothing Then Throw New ArgumentException("Either securityObjectName or securityObjectID must be specified")
            If serverGroupID = Nothing Then Throw New ArgumentNullException("serverGroupID")

            Dim MyBuffer As Integer
            Dim MyDBConn As New SqlConnection
            Dim MyCmd As New SqlCommand

            If LCase(securityObjectName) = "@@anonymous" OrElse LCase(securityObjectName) = "@@public" Then
                'Anonymous or Public access allowed, no page view/access logging 
                '(very fast since there is no need for a database query)
                Return True
            ElseIf (LCase(securityObjectName) = "@@supervisor" OrElse LCase(securityObjectName) = "@@supervisors") AndAlso Me.IsLoggedOn AndAlso Me.System_IsSuperVisor(userID) Then
                'Supervisor are always allowed to access
                Return True
            ElseIf Mid(securityObjectName, 1, 2) = "@@" AndAlso securityObjectName.Length > 2 AndAlso Me.IsLoggedOn AndAlso System_IsMember(userID, Mid(securityObjectName, 3)) Then
                'Check for membership resulted with true
                Return True
            End If

            'TODO: Implementation of required _LoadAuthorizationStatusFromCache overload in following code block
            'If AllowSecurityCache_Read = True Then
            '    'Read cached security status
            '    Dim AuthorizationStatus As Boolean
            '    'TODO: Implementation: AuthorizationStatus = _LoadAuthorizationStatusFromCache(userID, securityObjectID, securityObjectName, serverGroupID)
            '    If AuthorizationStatus = True Then
            '        Return True
            '    End If
            'End If

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Dim Result As Boolean
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "Public_UserIsAuthorized"
                    .CommandType = CommandType.StoredProcedure

                    .Parameters.Add("@ReturnValue", SqlDbType.Int)
                    .Parameters("@ReturnValue").Direction = ParameterDirection.ReturnValue
                    .Parameters.Add("@UserID", SqlDbType.BigInt).Value = userID
                    .Parameters.Add("@SecurityObjectID", SqlDbType.Int).Value = securityObjectID
                    .Parameters.Add("@SecurityObjectName", SqlDbType.NVarChar).Value = Utils.StringNotEmptyOrDBNull(securityObjectName)
                    .Parameters.Add("@ServerGroupID", SqlDbType.Int).Value = serverGroupID

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                MyCmd.ExecuteNonQuery()

                MyBuffer = CType(MyCmd.Parameters("@ReturnValue").Value, Integer)
                If MyBuffer <> 1 Then
                    Result = False
                    If AllowSecurityCache_Write = True Then
                        'Write status to cache
                        'TODO: Implementation: _SaveAuthorizationInCache(userID, securityObjectID, securityObjectName, serverGroupID, False)
                        System_DebugTraceWrite("_AuthCache False end", DebugLevels.Medium_LoggingAndRedirectAllEMailsToDeveloper)
                    End If
                Else
                    Result = True
                    If AllowSecurityCache_Write = True Then
                        'Write status to cache
                        'TODO: Implementation: _SaveAuthorizationInCache(userID, securityObjectID, securityObjectName, serverGroupID, True)
                        System_DebugTraceWrite("_AuthCache True end", DebugLevels.Medium_LoggingAndRedirectAllEMailsToDeveloper)
                    End If
                End If
            Finally
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
            Return Result
        End Function

        ''' <summary>
        '''     Query the authorization status of the current user for a given security object name, but doesn't create any log records
        ''' </summary>
        ''' <param name="UserName">An user name or an empty string for anonymous user (invalid user names or user names of deleted users will be treated as anonymous user)</param>
        ''' <param name="SecurityObjectName">A name of a security object</param>
        ''' <param name="ServerName">The server identification string where the security object should be validated</param>
        ''' <param name="AllowSecurityCache_Read">Allow to read this information from the cache</param>
        ''' <param name="AllowSecurityCache_Write">Allow to write this information to the cache</param>
        ''' <returns>True if the current user is authorized for that security object at the required server</returns>
        Private Function _IsUserAuthorized(UserName As String, ByVal SecurityObjectName As String, Optional ByVal ServerName As String = Nothing, Optional ByVal AllowSecurityCache_Read As Boolean = False, Optional ByVal AllowSecurityCache_Write As Boolean = False) As Boolean
            Dim MyBuffer As Integer
            Dim MyDBConn As New SqlConnection
            Dim MyCmd As New SqlCommand

            If SecurityObjectName = "" Then
                'Invalid application name
                Return False
            End If
            If LCase(SecurityObjectName) = "@@anonymous" OrElse (LCase(SecurityObjectName) = "@@public" And Me.IsLoggedOn) Then
                'Anonymous or Public access allowed, no page view/access logging 
                '(very fast since there is no need for a database query)
                Return True
            ElseIf Configuration.SecurityRecognizeCrawlersAsAnonymous = True AndAlso Not HttpContext.Current Is Nothing AndAlso Utils.IsRequestFromCrawlerAgent(HttpContext.Current.Request) = True Then
                'Crawlers are never authorized for anything except for @@anonymous - if configured for this behaviour
                Return False
            ElseIf (LCase(SecurityObjectName) = "@@supervisor" OrElse LCase(SecurityObjectName) = "@@supervisors") AndAlso Me.IsLoggedOn AndAlso Me.System_IsSuperVisor(Me.System_LookupUserID(UserName, SpecialUsers.User_Anonymous)) Then
                'Supervisor are always allowed to access
                Return True
            ElseIf Mid(SecurityObjectName, 1, 2) = "@@" AndAlso SecurityObjectName.Length > 2 AndAlso Me.IsLoggedOn AndAlso System_IsMember(Me.System_LookupUserID(UserName, SpecialUsers.User_Anonymous), Mid(SecurityObjectName, 3)) Then
                'Check for membership resulted with true
                Return True
            End If

            If ServerName = "" Then
                ServerName = Me.CurrentServerIdentString
            End If

            If AllowSecurityCache_Read = True Then
                'Read cached security status
                Dim AuthorizationStatus As Boolean
                AuthorizationStatus = _LoadAuthorizationStatusFromCache(Me.System_LookupUserID(UserName, SpecialUsers.User_Anonymous), SecurityObjectName, ServerName)
                If AuthorizationStatus = True Then
                    Return True
                End If
            End If

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Dim Result As Boolean
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "Public_UserIsAuthorizedForApp"
                    .CommandType = CommandType.StoredProcedure

                    .Parameters.Add("@ReturnValue", SqlDbType.Int)
                    .Parameters("@ReturnValue").Direction = ParameterDirection.ReturnValue
                    If UserName = Nothing Then
                        .Parameters.Add("@Username", SqlDbType.NVarChar).Value = DBNull.Value
                    Else
                        .Parameters.Add("@Username", SqlDbType.NVarChar).Value = UserName
                    End If
                    .Parameters.Add("@WebApplication", SqlDbType.NVarChar).Value = SecurityObjectName
                    .Parameters.Add("@ServerIP", SqlDbType.VarChar).Value = ServerName

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                MyCmd.ExecuteNonQuery()

                MyBuffer = CType(MyCmd.Parameters("@ReturnValue").Value, Integer)
                If MyBuffer <> 1 Then
                    Result = False
                    If AllowSecurityCache_Write = True Then
                        'Write status to cache
                        _SaveAuthorizationInCache(Me.System_LookupUserID(UserName, SpecialUsers.User_Anonymous), SecurityObjectName, ServerName, False)
                        System_DebugTraceWrite("_AuthCache False end", DebugLevels.Medium_LoggingAndRedirectAllEMailsToDeveloper)
                    End If
                Else
                    Result = True
                    If AllowSecurityCache_Write = True Then
                        'Write status to cache
                        _SaveAuthorizationInCache(Me.System_LookupUserID(UserName, SpecialUsers.User_Anonymous), SecurityObjectName, ServerName, True)
                        System_DebugTraceWrite("_AuthCache True end", DebugLevels.Medium_LoggingAndRedirectAllEMailsToDeveloper)
                    End If
                End If
            Finally
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
            Return Result

        End Function

        ''' <summary>
        '''     Validates if the given language ID is valid and active
        ''' </summary>
        ''' <param name="LanguageID">The ID to be verified</param>
        ''' <returns>True if it's activated</returns>
        Public Function System_IsValidLanguageID(ByVal LanguageID As Integer) As Boolean
            Dim MyDBConn As New SqlConnection
            Dim MyRecSet As SqlDataReader = Nothing
            Dim MyCmd As New SqlCommand

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "SELECT ID FROM Languages WHERE ID = " & LanguageID.ToString
                    .CommandType = CommandType.Text

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                MyRecSet = MyCmd.ExecuteReader()

                If MyRecSet.Read Then
                    System_IsValidLanguageID = True
                Else
                    System_IsValidLanguageID = False
                End If

            Finally
                If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then
                    MyRecSet.Close()
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

        End Function

        ''' <summary>
        '''     Get a value saved in the user's camm Web-Manager session
        ''' </summary>
        ''' <param name="SettingName">The name of the value</param>
        ''' <returns>The requested value (can be DBNull)</returns>
        Public Function System_GetSessionValue(ByVal SettingName As String) As Object
            Dim Result As Object = Nothing

            Dim MyDBConn As New SqlConnection
            Dim MyRecSet As SqlDataReader = Nothing
            Dim MyCmd As New SqlCommand

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "SELECT * FROM [System_SessionValues] WHERE SessionID IN (SELECT System_SessionID FROM Benutzer WHERE LoginName = N'" & Replace(Me.CurrentUserLoginName, "'", "''") & "') AND VarName = N'" & SettingName.Replace("'", "''") & "'"
                    .CommandType = CommandType.Text

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                MyRecSet = MyCmd.ExecuteReader()

                If Not MyRecSet.Read Then
                    Result = DBNull.Value
                ElseIf CType(MyRecSet("VarType"), Integer) = 1 Then
                    Result = MyRecSet("ValueInt")
                ElseIf CType(MyRecSet("VarType"), Integer) = 2 Then
                    Result = MyRecSet("ValueNText")
                ElseIf CType(MyRecSet("VarType"), Integer) = 3 Then
                    Result = MyRecSet("ValueFloat")
                ElseIf CType(MyRecSet("VarType"), Integer) = 4 Then
                    Result = MyRecSet("ValueDecimal")
                ElseIf CType(MyRecSet("VarType"), Integer) = 5 Then
                    Result = MyRecSet("ValueDateTime")
                ElseIf CType(MyRecSet("VarType"), Integer) = 6 Then
                    Result = MyRecSet("ValueImage")
                ElseIf CType(MyRecSet("VarType"), Integer) = 7 Then
                    Result = MyRecSet("ValueBool")
                End If

            Finally
                If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then
                    MyRecSet.Close()
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

            Return Result

        End Function

        ' TODO: change result type to boolean
        ''' <summary>
        '''     Save a value in the user's camm Web-Manager session
        ''' </summary>
        ''' <param name="SettingName">The name of the variable where to save the value</param>
        ''' <param name="SettingValue">The value to be saved</param>
        ''' <returns>A boolean value which indicates a successfull write operation</returns>
        Public Function System_SetSessionValue(ByVal SettingName As String, ByVal SettingValue As Object) As Object
            Dim Result As Boolean
            Dim MyDBConn As New SqlConnection
            Dim MyCmd As New SqlCommand
            Dim MyDataAdapter As SqlDataAdapter
            Dim MyDataSet As New DataSet
            Dim MyDataTable As DataTable
            Dim MyRow As DataRow
            Dim MySQLCommandBuilder As SqlCommandBuilder
            Dim MyRecSet As SqlDataReader = Nothing
            Dim CurUserSessionID As Integer
            Dim CurRowStatus As Byte = 0

            If Not Me.IsLoggedOn Then
                Me.Log.RuntimeWarning(New Exception("SetSessionValue: No valid logon in this user session yet."))
                Return False
            ElseIf System_IsSessionTerminated(Me.CurrentUserLoginName) Then
                ResetUserLoginName()
                Return False
            End If

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "SELECT System_SessionID FROM Benutzer WHERE LoginName = N'" & Replace(Me.CurrentUserLoginName, "'", "''") & "'"
                    .CommandType = CommandType.Text

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                MyRecSet = MyCmd.ExecuteReader()

                'Get Last System_SessionID of user
                'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                Dim WorkaroundEx As New Exception("")
                Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                Try
                    WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                Catch
                End Try
                If Not MyRecSet.Read() Then
                    Me.Log.RuntimeWarning("User '" & Me.CurrentUserLoginName & "' does not exist (any more)", WorkaroundStackTrace, DebugLevels.NoDebug, False, False)
                    Return False
                ElseIf IsDBNull(MyRecSet("System_SessionID")) Then
                    Me.Log.RuntimeWarning("Unexpected exception found: no user session available", WorkaroundStackTrace, DebugLevels.NoDebug, False, False)
                    Return False
                Else
                    CurUserSessionID = CType(MyRecSet("System_SessionID"), Integer)
                End If
                MyRecSet.Close()
                MyCmd.Dispose()

                MyDataAdapter = New SqlDataAdapter("SELECT * FROM [System_SessionValues] WHERE SessionID = " & CurUserSessionID & " AND VarName = N'" & SettingName.Replace("'", "''") & "'", MyDBConn)
                MyDataAdapter.SelectCommand.CommandType = CommandType.Text
                MyDataAdapter.AcceptChangesDuringFill = True
                MySQLCommandBuilder = New SqlCommandBuilder(MyDataAdapter)
                MyDataAdapter.Fill(MyDataSet, "CurSessionValues")
                MyDataTable = MyDataSet.Tables("CurSessionValues")

                If MyDataTable.Rows.Count = 0 Then
                    'Is it a new record?
                    MyRow = MyDataTable.NewRow
                    MyRow("VarName") = SettingName
                    MyRow("SessionID") = CurUserSessionID
                    CurRowStatus = 1
                Else
                    MyRow = MyDataTable.Rows(0)
                    MyRow.BeginEdit()
                    CurRowStatus = 2
                End If

                Select Case VarType(SettingValue).ToString.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                    Case "boolean"
                        MyRow("ValueBool") = SettingValue
                        MyRow("VarType") = 7
                    Case "string"
                        MyRow("ValueNText") = SettingValue
                        MyRow("VarType") = 2
                    Case "date"
                        MyRow("ValueDateTime") = SettingValue
                        MyRow("VarType") = 5
                    Case "decimal"
                        MyRow("ValueDecimal") = SettingValue
                        MyRow("VarType") = 4
                    Case "double"
                        MyRow("ValueFloat") = CDbl(SettingValue)
                        MyRow("VarType") = 3
                    Case "dbnull"
                        MyRow("ValueImage") = SettingValue
                        MyRow("VarType") = 6
                    Case "byte", "integer"
                        MyRow("ValueInt") = SettingValue
                        MyRow("VarType") = 1
                    Case Else 'e.g. "int8" [because of small differences between origin and saved values]
                        Try
                            If SettingValue Is Nothing Then
                                MyRow("ValueImage") = DBNull.Value
                                MyRow("VarType") = 6
                            ElseIf CType(SettingValue, String) = "" Then
                                MyRow("ValueNText") = ""
                                MyRow("VarType") = 2
                            Else
                                CurRowStatus = 0
                                Dim Message As String = "Function System_SetSessionValue: " & SettingValue.GetType.ToString & "Saving of given variant type (VarType-ID: " & VarType(SettingValue).ToString.ToLower(System.Globalization.CultureInfo.InvariantCulture) & ") not supported"
                                Me.Log.RuntimeException(Message)
                            End If
                        Catch
                            CurRowStatus = 0
                            Dim Message As String = "Function System_SetSessionValue: " & SettingValue.GetType.ToString & "Saving of given variant type (VarType-ID: " & VarType(SettingValue).ToString.ToLower(System.Globalization.CultureInfo.InvariantCulture) & ") not supported"
                            Me.Log.RuntimeException(Message)
                        End Try
                End Select

                'Save changes
                If CurRowStatus = 1 Then
                    MyDataTable.Rows.Add(MyRow)
                    Result = True
                ElseIf CurRowStatus = 2 Then
                    MyRow.EndEdit()
                    Result = True
                Else
                    MyRow.CancelEdit()
                    Result = False
                End If

                MyDataAdapter.Update(MyDataSet, "CurSessionValues")

                MyDataTable.Dispose()
                MyDataSet.Dispose()
                MyDataAdapter.Dispose()
            Finally
                If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then
                    MyRecSet.Close()
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

            Return Result

        End Function

#Region "Languages & markets"
        Dim _CurrentMarketID As Integer = 0

        Dim CookieSavedValue_Language As Integer

        Private _ActiveMarkets As Integer() = New Integer() {1}
        ''' <summary>
        '''     Available markets for activation/localization
        ''' </summary>
        ''' <value>An array of integers representing the IDs of the markets</value>
        ''' <remarks>
        '''     The browsers will be asked for its preferred language, but is that language offered by the camm Web-Manager system?
        '''     Per default, camm Web-Manager provides all activated languages, but you can also provide a custom value by the configuration file.
        ''' </remarks>
        Private Property ActiveMarkets() As Integer()
            Get
                If HttpContext.Current Is Nothing Then
                    Return _ActiveMarkets
                Else
                    Dim Result As Integer()
                    Dim CacheValues As Boolean
                    'Read the cached value
                    Result = CType(HttpContext.Current.Cache("WebManager.ActiveMarkets"), Integer())
                    'When no cached value is existant, load the value from configuration respectively the database
                    If Result Is Nothing Then
                        Result = Configuration.GlobalizationSupportedMarkets 'Always returns a valid array with at least zero length
                        CacheValues = True
                    End If
                    If Result.Length = 0 Then
                        'If not defined yet, load the general list of activated markets from database
                        Dim Markets As LanguageInformation()
                        Dim MarketIDs As New ArrayList
                        Markets = Me.System_GetLanguagesInfo
                        For MyCounter As Integer = 0 To Markets.Length - 1
                            MarketIDs.Add(Markets(MyCounter).ID)
                        Next
                        Result = CType(MarketIDs.ToArray(GetType(Integer)), Integer())
                        CacheValues = True
                    End If
                    'Cache result values for later access when value has changed
                    If CacheValues = True Then
                        ActiveMarkets = Result
                    End If
                    'Return the result values
                    Return Result
                End If
            End Get
            Set(ByVal Value As Integer())
                If HttpContext.Current Is Nothing Then
                    _ActiveMarkets = Value
                Else
                    Try
                        If HttpContext.Current.Cache("WebManager.ActiveMarkets") Is Nothing OrElse CType(HttpContext.Current.Cache("WebManager.ActiveMarkets"), Integer()).Length = 0 Then
                            Utils.SetHttpCacheValue("WebManager.ActiveMarkets", Value, New TimeSpan(0, 5, 0), Caching.CacheItemPriority.NotRemovable)
                        End If
                    Catch 'Ignore those error which might happen in multi-threading scenarios
                    End Try
                End If
            End Set
        End Property

        ''' <summary>
        '''     Is the given market one of the supported/active markets?
        ''' </summary>
        ''' <param name="marketID">A market ID to validate</param>
        ''' <returns>True when it is supported/activated, otherwise False</returns>
        ''' <remarks>
        ''' If the application defines any supported languages in the configuration file, this list of market IDs will be used for checking, otherwise the general list of activated markets will be loaded from the database.
        ''' </remarks>
        Private Function IsMarketActivated(ByVal marketID As Integer) As Boolean
            Dim availableMarkets As Integer() = ActiveMarkets
            For MyCounter As Integer = 0 To availableMarkets.Length - 1
                If availableMarkets(MyCounter) = marketID Then
                    Return True
                End If
            Next
            Return False
        End Function

        ''' <summary>
        '''     Detect the favorite browser language which is supported by the currently configured instance of camm Web-Manager
        ''' </summary>
        ''' <returns>The language ID which matches best; if nothing matches, the result is 1 (English)</returns>
        Public Function GetBrowserPreferredLanguage() As Integer

            Dim PreferedLanguage() As String = Nothing
            If Not HttpContext.Current Is Nothing Then
                PreferedLanguage = HttpContext.Current.Request.UserLanguages()
            End If

            Dim Result As Integer
            If Not PreferedLanguage Is Nothing AndAlso PreferedLanguage.Length > 0 Then
                For Each MyPreferedLanguage As String In PreferedLanguage
                    Dim MyLanguageID As Integer = Internationalization.GetLanguageIDOfBrowserSetting(Trim(MyPreferedLanguage).ToLower(System.Globalization.CultureInfo.InvariantCulture))
                    'ask for the activated language, not the supported one
                    If IsMarketActivated(MyLanguageID) Then
                        Result = MyLanguageID
                        Exit For
                    ElseIf IsMarketActivated(Internationalization.GetAlternativelySupportedLanguageID(MyLanguageID)) Then
                        Result = MyLanguageID
                        Exit For
                    End If
                Next
            End If
            If Result = Nothing Then
                Result = Configuration.GlobalizationDefaultMarket 'default
            End If

            Return Result

        End Function

        ''' <summary>
        '''     Auto-detect the best match for the current market/language
        ''' </summary>
        Private Function LookupCurrentMarket() As Integer
            Dim ResultValue As Integer = Configuration.GlobalizationDefaultMarket
            'Default-Sprache setzen
            Try
                If IsNothing(HttpContext.Current.Session("CurLanguage")) Then
                    HttpContext.Current.Session("CurLanguage") = GetBrowserPreferredLanguage()
                    System_DebugTraceWrite("CurLanguage: Get: GetBrowserPreferredLanguage = " & CType(HttpContext.Current.Session("CurLanguage"), Integer).ToString)
                End If
                If Not HttpContext.Current.Request.Cookies("CWM") Is Nothing AndAlso Not HttpContext.Current.Request.Cookies("CWM")("Lang") Is Nothing Then
                    CookieSavedValue_Language = CType(HttpContext.Current.Request.Cookies("CWM")("Lang"), Integer)
                    If CookieSavedValue_Language <> 0 Then
                        'something is defined - validate against the activated markets
                        If Not IsMarketActivated(CookieSavedValue_Language) Then
                            CookieSavedValue_Language = Internationalization.GetAlternativelySupportedLanguageID(CookieSavedValue_Language)
                        End If
                        If Not IsMarketActivated(CookieSavedValue_Language) Then
                            CookieSavedValue_Language = Configuration.GlobalizationDefaultMarket
                        End If
                    End If
                End If
                If CookieSavedValue_Language > 0 Then
                    HttpContext.Current.Session("CurLanguage") = CookieSavedValue_Language
                    System_DebugTraceWrite("CurLanguage: Get: CookieSavedValue = " & CookieSavedValue_Language)
                End If
                If HttpContext.Current.Request.QueryString("Lang") <> "" Then
                    HttpContext.Current.Session("CurLanguage") = CLng(HttpContext.Current.Request.QueryString("Lang"))
                    System_DebugTraceWrite("CurLanguage: Get: QueryString(""Lang"") = " & CType(HttpContext.Current.Session("CurLanguage"), String))
                    If Configuration.CookieLess = False Then
                        ''ISSUE: sometimes, there might be two cookies with identical name (for e.g. www.domain.com and for domain.com); in this case, writing will always update the last one, reading from the first one. So, the cookie changes haven't got any effect on the client
                        ''SOLUTION: lookup all available cookies with name "CWM"; if there exist more than 1 cookie, drop all of them
                        'Cookie cleanup in case of multiple CWM market cookies for different paths (in this situation, cookie updates have got no effect)
                        '(Background: in past, it was possible to set the cookie for the current application directory instead of the root folder)
                        If CompuMaster.camm.WebManager.Utils.CountOfOccurances(HttpContext.Current.Request.ServerVariables("HTTP_COOKIE"), "CWM=Lang=") > 1 Then
                            Dim CookieWriteUrl As String = HttpContext.Current.Request.Url.AbsolutePath
                            Do While CookieWriteUrl <> ""
                                'Remove cookie from sub folder
                                Dim co As New HttpCookie("CWM")
                                co.Expires = Now.AddDays(-1)
                                co.Path = CookieWriteUrl
                                co.Value = Nothing
                                HttpContext.Current.Response.Cookies.Add(co)
                                'Prepare URL for next sub folder
                                CookieWriteUrl = CookieWriteUrl.Substring(0, CookieWriteUrl.LastIndexOf("/"c))
                            Loop
                        End If

                        'Validate that the market ID is an activated market
                        If Not IsMarketActivated(CType(HttpContext.Current.Session("CurLanguage"), Integer)) Then
                            HttpContext.Current.Session("CurLanguage") = Internationalization.GetAlternativelySupportedLanguageID(CType(HttpContext.Current.Session("CurLanguage"), Integer))
                        End If
                        If Not IsMarketActivated(CType(HttpContext.Current.Session("CurLanguage"), Integer)) Then
                            HttpContext.Current.Session("CurLanguage") = Configuration.GlobalizationDefaultMarket
                        End If

                        'Write regular CWM cookie in root path
                        Dim co2 As New HttpCookie("CWM", "Lang")
                        co2.Path = "/"
                        co2.Expires = Now.AddDays(183) 'Cookie expires after 6 months
                        co2.Value = "Lang=" & CType(HttpContext.Current.Session("CurLanguage"), Integer).ToString
                        If UCase(HttpContext.Current.Request.ServerVariables("HTTPS")) = "ON" Then
                            co2.Secure = True
                        Else
                            co2.Secure = False
                        End If
                        HttpContext.Current.Response.Cookies.Add(co2)
                    End If
                End If
                If CType(HttpContext.Current.Session("CurLanguage"), Integer) <= 0 Then
                    HttpContext.Current.Session("CurLanguage") = Configuration.GlobalizationDefaultMarket
                    System_DebugTraceWrite("CurLanguage: Get: Return value would be <= 0 and invalid; corrected to Configuration.LanguagesDefault")
                End If
                ResultValue = CType(HttpContext.Current.Session("CurLanguage"), Integer)
                'RedirectionParams = System_GetRequestQueryStringComplete(New String() {"Lang"})
            Catch ex As Exception
                System_DebugTraceWrite("CurLanguage: Get: Error = " & ex.Message)
                If HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing Then
                    ResultValue = Configuration.GlobalizationDefaultMarket
                Else
                    HttpContext.Current.Session("CurLanguage") = Configuration.GlobalizationDefaultMarket
                    ResultValue = CType(HttpContext.Current.Session("CurLanguage"), Integer)
                End If
            End Try
            Return (ResultValue)

        End Function

        ''' <summary>
        '''     The current language
        ''' </summary>
        ''' <param name="ForceLanguageForThisPage">Force this language ID for this page only</param>
        ''' <returns>The current language ID</returns>
        <Obsolete("Use UIMarket instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Overloads Function CurLanguage(ByVal ForceLanguageForThisPage As Integer) As Integer
            Return UIMarket(ForceLanguageForThisPage)
        End Function

        ''' <summary>
        '''     The current market
        ''' </summary>
        ''' <returns>The current market ID</returns>
        <Obsolete("Use UIMarket instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Overloads Function CurLanguage() As Integer
            Return (UIMarket(0))
        End Function

        ''' <summary>
        '''     The current market
        ''' </summary>
        ''' <param name="ForceLanguageForThisPage">Force this market ID for this page only</param>
        ''' <returns>The current market ID</returns>
        <Obsolete("Use UIMarket instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Overloads Function CurLanguage(ByVal ForceLanguageForThisPage As LanguageInformation) As Integer
            Return (UIMarket(ForceLanguageForThisPage.ID))
        End Function

        ''' <summary>
        '''     The current market
        ''' </summary>
        ''' <returns>The current market information object</returns>
        ''' <remarks>
        '''     Requires a database connection
        ''' </remarks>
        <Obsolete("Use UIMarket instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Overloads Function CurLanguageInfo() As LanguageInformation
            Return UIMarketInfo(UI.MarketID)
        End Function

        ''' <summary>
        '''     The current market
        ''' </summary>
        ''' <param name="ForceLanguageForThisPage">Force this market ID for this page only</param>
        ''' <returns>The current market information object</returns>
        ''' <remarks>
        '''     Requires a database connection
        ''' </remarks>
        <Obsolete("Use UIMarket instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Overloads Function CurLanguageInfo(ByVal ForceLanguageForThisPage As LanguageInformation) As LanguageInformation
            Return UIMarketInfo(ForceLanguageForThisPage)
        End Function

        ''' <summary>
        '''     The current market
        ''' </summary>
        ''' <param name="ForceLanguageForThisPage">Force this market ID for this page only</param>
        ''' <returns>The current market information object</returns>
        ''' <remarks>
        '''     Requires a database connection
        ''' </remarks>
        <Obsolete("Use UIMarket instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Overloads Function CurLanguageInfo(ByVal ForceLanguageForThisPage As Integer) As LanguageInformation
            Return UIMarketInfo(ForceLanguageForThisPage)
        End Function

        ''' <summary>
        '''     The language ID of the current market
        ''' </summary>
        ''' <returns>An integer value with the ID of the language of the current market</returns>
        ''' <remarks>
        '''     This is the value you receive when you ask for the alternative language of a market/language.
        ''' </remarks>
        <Obsolete("Use UI.LanguageID instead")> Public Function UILanguage() As Integer
            Return Me.Internationalization.GetAlternativelySupportedLanguageID(UI.MarketID)
        End Function

        ''' <summary>
        '''     The ID value of the current market
        ''' </summary>
        ''' <returns>An integer value with the ID of the current market</returns>
        <Obsolete("Use UI.MarketID instead")> Public Function UIMarket() As Integer
            Return UIMarket(0)
        End Function

        ''' <summary>
        '''     The ID value of the current market
        ''' </summary>
        ''' <param name="forceMarketLanguageForThisPage">Force this market ID for this page only</param>
        ''' <returns>An integer value with the ID of the current market</returns>
        Public Function UIMarket(ByVal forceMarketLanguageForThisPage As Integer) As Integer
            If forceMarketLanguageForThisPage <> 0 Then
                'Use the forced market ID - but only for this single page request
                LookupCurrentMarket() 'For saving any language switches which might be done by the query string "Lang" into the cookie
                _CurrentMarketID = forceMarketLanguageForThisPage
                System_DebugTraceWrite("UIMarket: Forced language ID usage")
                Internationalization.LoadLanguageStrings(_CurrentMarketID) 'Load language strings for that newly selected language
            ElseIf Configuration.GlobalizationForcedMarket <> 0 Then
                LookupCurrentMarket() 'For saving any language switches which might be done by the query string "Lang" into the cookie
                _CurrentMarketID = Configuration.GlobalizationForcedMarket
                System_DebugTraceWrite("UIMarket: Forced language ID usage by web.config")
                Internationalization.LoadLanguageStrings(_CurrentMarketID) 'Load language strings for that newly selected language
            ElseIf _CurrentMarketID = 0 Then
                'Detect the best matching market by the data in URL, session or cookie
                System_DebugTraceWrite("UIMarket: Look up for the current language")
                _CurrentMarketID = LookupCurrentMarket()
                If Not HttpContext.Current Is Nothing AndAlso Request.QueryString("PLang") <> Nothing AndAlso CInt(Request.QueryString("PLang")) <> Nothing Then
                    'PLang parameter forces the market value for this page
                    System_DebugTraceWrite("UIMarket: PLang parameter forces the market value for this page")
                    _CurrentMarketID = Utils.TryCInt(Request.QueryString("PLang"))
                End If
                Internationalization.LoadLanguageStrings(_CurrentMarketID) 'Load language strings if not yet loaded
            Else
                'Simply return the cached language value
                System_DebugTraceWrite("UIMarket: Return of cached language ID")
            End If
            Return _CurrentMarketID
        End Function

        ''' <summary>
        '''     The ID value of the current market
        ''' </summary>
        ''' <param name="forceMarketLanguageForThisPage">Force this market for this page only</param>
        ''' <returns>An language information object with meta data on the current market</returns>
        Public Function UIMarket(ByVal forceMarketLanguageForThisPage As LanguageInformation) As Integer
            Return UIMarket(forceMarketLanguageForThisPage.ID)
        End Function

        ''' <summary>
        '''     The current market
        ''' </summary>
        ''' <returns>An language information object with meta data on the current market</returns>
        ''' <remarks>
        '''     Requires a database connection
        ''' </remarks>
        Public Overloads Function UIMarketInfo() As LanguageInformation
            Return New LanguageInformation(UI.MarketID, Me)
        End Function

        ''' <summary>
        '''     The current market
        ''' </summary>
        ''' <param name="ForceMarketForThisPage">Force this language ID for this page only</param>
        ''' <returns>An language information object with meta data on the current market</returns>
        ''' <remarks>
        '''     Requires a database connection
        ''' </remarks>
        Public Overloads Function UIMarketInfo(ByVal ForceMarketForThisPage As LanguageInformation) As LanguageInformation
            Return New LanguageInformation(UIMarket(ForceMarketForThisPage), Me)
        End Function

        ''' <summary>
        '''     The current market
        ''' </summary>
        ''' <param name="ForceMarketForThisPage">Force this language ID for this page only</param>
        ''' <returns>An language information object with meta data on the current market</returns>
        ''' <remarks>
        '''     Requires a database connection
        ''' </remarks>
        Public Overloads Function UIMarketInfo(ByVal ForceMarketForThisPage As Integer) As LanguageInformation
            Return New LanguageInformation(UIMarket(ForceMarketForThisPage), Me)
        End Function
#End Region

        ''' <summary>
        '''     Get the current user ID; requires a successfull login, first
        ''' </summary>
        ''' <returns>The ID of the current user</returns>
        ''' <remarks>
        '''     If no user has logged in, this function throws an exception
        ''' </remarks>
        <Obsolete("Use CurrentUserID instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetCurUserID() As Integer
            If Not HttpContext.Current Is Nothing AndAlso Me.IsLoggedOn = False Then
                Dim Message As String = "No user logged in - cannot get a user ID"
                Throw New Exception(Message)
            Else
                Return CInt(Me.CurrentUserID(SpecialUsers.User_Anonymous))
            End If
        End Function

        Private _CurrentUserInfo_ID As Long
        Private _CurrentUserInfo_Result As UserInformation
        ''' <summary>
        '''     Get the current user information object; requires a successfull login, first
        ''' </summary>
        ''' <returns>The user information object of the current user</returns>
        ''' <remarks>
        '''     If no user has logged in, this function aborts, throws a runtime exception and redirect the browser to an error page
        ''' </remarks>
        Public Function CurrentUserInfo() As UserInformation
            Dim LoadThisUser As Long = CurrentUserID(SpecialUsers.User_Anonymous)
            If LoadThisUser <> SpecialUsers.User_Anonymous Then
                If _CurrentUserInfo_Result Is Nothing OrElse _CurrentUserInfo_Result.IDLong <> LoadThisUser Then
                    '(re)load the user information object
                    _CurrentUserInfo_Result = New UserInformation(LoadThisUser, Me)
                    _CurrentUserInfo_ID = LoadThisUser
                End If
                Return _CurrentUserInfo_Result
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        '''     Get the current user information object
        ''' </summary>
        ''' <returns>The user information object of the current user</returns>
        ''' <remarks>
        '''     If no user has logged in, this function aborts, throws a runtime exception and redirect the browser to an error page
        ''' </remarks>
        Public Function CurrentUserInfo(ByVal alternativeUserIDIfNotDetectable As CompuMaster.camm.WebManager.WMSystem.SpecialUsers) As UserInformation
            Dim LoadThisUser As Long = CurrentUserID(alternativeUserIDIfNotDetectable)
            If _CurrentUserInfo_Result Is Nothing OrElse _CurrentUserInfo_Result.IDLong <> LoadThisUser Then
                '(re)load the user information object
                _CurrentUserInfo_Result = New UserInformation(LoadThisUser, Me)
                _CurrentUserInfo_ID = LoadThisUser
            End If
            Return _CurrentUserInfo_Result
        End Function

        ''' <summary>
        '''     Get the current user ID; requires a successfull login, first
        ''' </summary>
        ''' <returns>The ID of the current user</returns>
        ''' <remarks>
        '''     If no user has logged in, this function aborts, throws a runtime exception and redirect the browser to an error page
        ''' </remarks>
        ''' <exception cref="Exception">May throw an Exception when no user is logged on</exception>
        Friend Function InternalCurrentUserID() As Long
            If HttpContext.Current Is Nothing Then
                If Not Me.IsLoggedOn Then
                    Return WMSystem.SpecialUsers.User_Code
                Else
                    'raises error when is null
                    Return CInt(System_GetUserID(Me.CurrentUserLoginName))
                End If
            Else
                'raises error when is null
                Dim Result As Object = System_GetUserID(Me.CurrentUserLoginName)
                If IsDBNull(Result) Then
                    Dim Message As String = "No user logged in - cannot get a user ID"
                    Throw New Exception(Message)
                Else
                    Return CLng(Result)
                End If
            End If
        End Function

        ''' <summary>
        '''     Get the current user ID; requires a successfull login, first
        ''' </summary>
        ''' <returns>The ID of the current user</returns>
        ''' <remarks>
        '''     If no user has logged in, this function aborts, throws a runtime exception and redirect the browser to an error page
        ''' </remarks>
        ''' <exception cref="Exception">May throw an Exception when no user is logged on</exception>
        <Obsolete("Better use an overloaded alternative because this method may throw an Exception without a valid user logon")> Public Function CurrentUserID() As Long
            Return InternalCurrentUserID()
        End Function
        ''' <summary>
        '''     Get the current user ID or SpecialUsers.User_Anonymous when no user has logged on or an alternative ID when the current context is not in an HTTP application
        ''' </summary>
        ''' <returns>The ID of the current user</returns>
        <Obsolete("Use CurrentUserID instead")> Public Function System_GetCurUserID(ByVal AlternativeUserIDIfNotDetectable As SpecialUsers) As Integer
            Return CInt(CurrentUserID(AlternativeUserIDIfNotDetectable))
        End Function
        ''' <summary>
        '''     Get the current user ID or SpecialUsers.User_Anonymous (for HttpContexts) or the alternative ID (for non-HttpContexts)
        ''' </summary>
        ''' <param name="AlternativeUserIDIfNotDetectable"></param>
        ''' <returns>The ID of the current user</returns>
        Public Function CurrentUserID(ByVal AlternativeUserIDIfNotDetectable As SpecialUsers) As Long

            If HttpContext.Current Is Nothing Then
                Try
                    Return CLng(System_GetUserID(Me.CurrentUserLoginName)) 'raises error when is null
                Catch
                    Return AlternativeUserIDIfNotDetectable
                End Try
            Else
                Try
                    Return CLng(System_GetUserID(Me.CurrentUserLoginName)) 'raises error when is null
                Catch
                    Return SpecialUsers.User_Anonymous
                End Try
            End If

        End Function

        ''' <summary>
        ''' Search for a user with the specified external account name
        ''' </summary>
        ''' <param name="externalAccountName">The name of the external account</param>
        ''' <returns>The user's ID or Nothing if no valid data has been found</returns>
        Friend Function LookupUserIDOfExternalUserAccount(ByVal externalAccountName As String) As Long
            If externalAccountName = Nothing Then
                Throw New ArgumentNullException("externalAccountName")
            End If
            Dim MyCmd As New SqlCommand("SELECT dbo.Benutzer.ID FROM dbo.Benutzer INNER JOIN (SELECT ID_User FROM dbo.Log_Users WHERE Type = N'ExternalAccount' AND Value = @ExternalUserName) AS ListOfExternalAccounts ON dbo.Benutzer.ID = ListOfExternalAccounts.ID_User GROUP BY dbo.Benutzer.ID", New SqlConnection(Me.ConnectionString))
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@ExternalUserName", SqlDbType.NVarChar).Value = externalAccountName
            Dim ResultList As ArrayList = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(MyCmd, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If ResultList.Count = 1 Then
                Return CType(ResultList(0), Long)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        '''     Load the list of all user IDs
        ''' </summary>
        Public Function System_GetUserIDs() As Long()
            Dim Users As DataTable = Me.SearchUserData(New UserFilter() {New UserFilter("LastName", UserFilter.SearchMethods.All), New UserFilter("FirstName", UserFilter.SearchMethods.All)}, New UserSortArgument() {New UserSortArgument("LastName"), New UserSortArgument("FirstName")})
            Return CType(CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertDataTableToArrayList(Users.Columns("ID")).ToArray(GetType(Long)), Long())
        End Function

        ''' <summary>
        '''     Get a user ID by a login name
        ''' </summary>
        ''' <param name="userLoginName">The login name to get the user ID from</param>
        ''' <returns>The requested user ID or -1 for anonymous (=empty) user name or 0 in the case of an invalid (or deleted) login name</returns>
        Public Function System_LookupUserID(ByVal userLoginName As String) As Long
            Return Utils.Nz(Me.System_GetUserID(userLoginName, False), 0L)
        End Function

        ''' <summary>
        '''     Get a user ID by a login name
        ''' </summary>
        ''' <param name="userLoginName">The login name to get the user ID from</param>
        ''' <param name="reportMissingUserAsSpecialUser">In case of a missing user (not anonymous user), report this value</param>
        ''' <returns>The requested user ID or -1 for anonymous (=empty) user name or the value of reportMissingUserAsSpecialUser in the case of an invalid (or deleted) login name</returns>
        Public Function System_LookupUserID(ByVal userLoginName As String, reportMissingUserAsSpecialUser As SpecialUsers) As Long
            Return Utils.Nz(Me.System_GetUserID(userLoginName, False), reportMissingUserAsSpecialUser)
        End Function

        ''' <summary>
        '''     Get a user ID by a login name
        ''' </summary>
        ''' <param name="userLoginName">The login name to get the user ID from</param>
        ''' <param name="returnMinus1InsteadOfDBNullIfUserDoesntExist">If this user doesn't exist, return -1 instead of DBNull</param>
        ''' <returns>The requested user ID or DBNull respectively -1 in the case of an invalid login name</returns>
        Public Function System_GetUserID(ByVal userLoginName As String, Optional ByVal returnMinus1InsteadOfDBNullIfUserDoesntExist As Boolean = False) As Object
            Static _MyLoginName As String
            Static _ReturnMinus1InsteadOfDBNullIfAnonymousUser As Boolean
            Static _ResultBuffer As Object

            If userLoginName <> "" AndAlso _MyLoginName = userLoginName AndAlso _ReturnMinus1InsteadOfDBNullIfAnonymousUser = returnMinus1InsteadOfDBNullIfUserDoesntExist Then
                'use buffered result value
                Return _ResultBuffer
            ElseIf Trim(userLoginName) = "" Then
                'Anonymous user
                If returnMinus1InsteadOfDBNullIfUserDoesntExist = True Then
                    Return SpecialUsers.User_Anonymous
                Else
                    Return DBNull.Value
                End If
            End If

            Dim MyCmd As New SqlCommand("Public_GetUserID", New SqlConnection(Me.ConnectionString))
            MyCmd.CommandType = CommandType.StoredProcedure
            MyCmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = userLoginName

            'Execute the command
            Dim Result As Object = WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If Result Is Nothing Then
                Result = DBNull.Value
            End If
            If returnMinus1InsteadOfDBNullIfUserDoesntExist = True AndAlso IsDBNull(Result) Then
                Result = SpecialUsers.User_Anonymous
            End If

            'cache result
            _MyLoginName = userLoginName
            _ReturnMinus1InsteadOfDBNullIfAnonymousUser = returnMinus1InsteadOfDBNullIfUserDoesntExist
            _ResultBuffer = Result

            Return Result
        End Function

        ''' <summary>
        ''' Check if a given user is a special user account to camm Web-Manager
        ''' </summary>
        ''' <param name="UserID">An ID of an user account</param>
        ''' <remarks>
        ''' camm Web-Manager knows some special user accounts like User_Anonymous, User_Code, User_Public, User_UpdateProcessor
        ''' </remarks>
        <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Shared Function IsSystemUser(ByVal UserID As Long) As Boolean
            Select Case UserID
                Case SpecialUsers.User_Anonymous, SpecialUsers.User_Code, SpecialUsers.User_Public, SpecialUsers.User_UpdateProcessor
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        '--------------------------------------------------------------------------------------
        'SetUserDetail
        '--------------------------------------------------------------------------------------
        'Ersteller: Jochen Wezel, CompuMaster GmbH, 2001-03-23
        '
        'MyUserID   : ID des Benutzers als Long
        'MyProperty : Gewünschte Eigenschaft als String
        'MyNewValue : Zu speichernder Wert als String/Null
        '
        'Return     : Bei Erfolg True, ansonsten False
        '--------------------------------------------------------------------------------------
        <Obsolete("Use UserInformation class instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_SetUserDetail(ByVal MyUserID As Integer, ByVal MyProperty As String, ByVal MyNewValue As Object, Optional ByVal DoNotLogSuccess As Boolean = False) As Object
            Return DataLayer.Current.SetUserDetail(Me, Nothing, CLng(MyUserID), MyProperty, Utils.Nz(MyNewValue, CType(Nothing, String)), DoNotLogSuccess)
        End Function

        ''' <summary>
        '''     Set a user profile setting
        ''' </summary>
        ''' <param name="dbConnection">An open connection which shall be used or nothing if a new one shall be created independently and on the fly</param>
        ''' <param name="MyUserID"></param>
        ''' <param name="MyProperty"></param>
        ''' <param name="MyNewValue"></param>
        ''' <param name="DoNotLogSuccess"></param>
        <Obsolete("Use UserInformation class instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_SetUserDetail(ByVal dbConnection As SqlConnection, ByVal MyUserID As Long, ByVal MyProperty As String, ByVal MyNewValue As Object, Optional ByVal DoNotLogSuccess As Boolean = False) As Boolean
            Return DataLayer.Current.SetUserDetail(Me, dbConnection, MyUserID, MyProperty, Utils.Nz(MyNewValue, CType(Nothing, String)), DoNotLogSuccess)
        End Function

        ' TODO: change result type to boolean
        <Obsolete("Use UserInformation class instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_SetUserDetail(ByVal MyUserID As Long, ByVal MyProperty As String, ByVal MyNewValue As Object, Optional ByVal DoNotLogSuccess As Boolean = False) As Object
            Return DataLayer.Current.SetUserDetail(Me, Nothing, MyUserID, MyProperty, Utils.Nz(MyNewValue, CType(Nothing, String)), DoNotLogSuccess)
        End Function

        '--------------------------------------------------------------------------------------
        'GetUserDetail
        '--------------------------------------------------------------------------------------
        'Ersteller: Jochen Wezel, CompuMaster GmbH, 2001-03-23
        '
        'MyUserID   : ID des Benutzers als Long
        'MyProperty : Gewünschte Eigenschaft als String
        '
        'Return     : Liefert das Ergebnis als String/Null zurück
        '--------------------------------------------------------------------------------------
        <Obsolete("Use UserInformation class instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_GetUserDetail(ByVal MyUserID As Object, ByVal MyProperty As Object) As Object
            If IsDBNull(MyUserID) Then
                Return DBNull.Value
            End If
            Dim Result As String = CompuMaster.camm.WebManager.DataLayer.Current.GetUserDetail(Me, CLng(MyUserID), CType(MyProperty, System.String))
            If Result = Nothing Then
                Return DBNull.Value
            Else
                Return Result
            End If
        End Function

        <Obsolete("Use UserInformation class instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_GetUserDetail(ByVal MyUserID As Long, ByVal MyProperty As String) As Object
            Dim Result As String = CompuMaster.camm.WebManager.DataLayer.Current.GetUserDetail(Me, MyUserID, MyProperty)
            If Result = Nothing Then
                Return DBNull.Value
            Else
                Return Result
            End If
        End Function

        ''' <summary>
        '''     Detect the favorite language of a user which is supported by the this configured instance of camm Web-Manager 
        ''' </summary>
        ''' <param name="MyUserID">A user ID</param>
        ''' <returns>The language ID which matches best</returns>
        <Obsolete("Use overloaded method"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(ByVal MyUserID As Integer) As Integer
            Return System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(CLng(MyUserID))
        End Function
        ''' <summary>
        '''     Detect the favorite language of a user which is supported by the this configured instance of camm Web-Manager 
        ''' </summary>
        ''' <param name="MyUserID">A user ID</param>
        ''' <returns>The language ID which matches best</returns>
        Public Function System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(ByVal MyUserID As Long) As Integer
            Return System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(New CompuMaster.camm.WebManager.WMSystem.UserInformation(MyUserID, Me))
        End Function
        ''' <summary>
        '''     Detect the favorite language of a user which is supported by the this configured instance of camm Web-Manager 
        ''' </summary>
        ''' <param name="UserInfo">A user information object</param>
        ''' <returns>The language ID which matches best</returns>
        Public Function System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(ByVal UserInfo As UserInformation) As Integer
            Dim BufferedValue As Integer
            Try
                '1stPreferredLanguage
                BufferedValue = Internationalization.GetAlternativelySupportedLanguageID(UserInfo.PreferredLanguage1.ID)
                If Internationalization.IsSupportedLanguage(BufferedValue) = True Then
                    System_Get1stPreferredLanguageWhichIsSupportedByTheSystem = BufferedValue
                Else
                    '2ndPreferredLanguage
                    BufferedValue = Internationalization.GetAlternativelySupportedLanguageID(UserInfo.PreferredLanguage2.ID)
                    If Internationalization.IsSupportedLanguage(BufferedValue) = True Then
                        System_Get1stPreferredLanguageWhichIsSupportedByTheSystem = BufferedValue
                    Else
                        '3rdPreferredLanguage
                        BufferedValue = Internationalization.GetAlternativelySupportedLanguageID(UserInfo.PreferredLanguage3.ID)
                        If Internationalization.IsSupportedLanguage(BufferedValue) = True Then
                            System_Get1stPreferredLanguageWhichIsSupportedByTheSystem = BufferedValue
                        Else
                            'English
                            System_Get1stPreferredLanguageWhichIsSupportedByTheSystem = 1
                        End If
                    End If
                End If
            Catch
                System_Get1stPreferredLanguageWhichIsSupportedByTheSystem = 1
            End Try

        End Function

        ''' <summary>
        '''     Get the salutation based on the gender of the given user
        ''' </summary>
        ''' <param name="MyUserID">A user ID</param>
        ''' <returns>Returns "Mr." respectively "Ms." in the current language</returns>
        <Obsolete("Use UserInformation class instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_GetUserAddresses(ByVal MyUserID As Integer) As String
            Return System_GetUserAddresses_Internal(CLng(MyUserID))
        End Function
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Private Function System_GetUserAddresses_Internal(ByVal MyUserID As Long) As String
            Dim BufferedValue As String

            Try

                BufferedValue = CompuMaster.camm.WebManager.DataLayer.Current.GetUserDetail(Me, MyUserID, "Sex")
                If LCase(BufferedValue) = "m" Then
                    System_GetUserAddresses_Internal = Internationalization.UserManagementAddressesMr
                ElseIf LCase(BufferedValue) = "u" Then 'undefined
                    System_GetUserAddresses_Internal = ""
                Else
                    System_GetUserAddresses_Internal = Internationalization.UserManagementAddressesMs
                End If

            Catch
                System_GetUserAddresses_Internal = Internationalization.UserManagementAddressesMr
            End Try

        End Function
        ''' <summary>
        '''     Get the salutation based on the gender of the given user
        ''' </summary>
        ''' <param name="MyUserID">A user ID</param>
        ''' <returns>Returns "Mr." respectively "Ms." in the current language</returns>
        <Obsolete("Use UserInformation class instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_GetUserAddresses(ByVal MyUserID As Long) As String
            Return System_GetUserAddresses_Internal(MyUserID)
        End Function
        ''' <summary>
        '''     Get the salutation based on the gender of the given user
        ''' </summary>
        ''' <param name="UserInfo">A user information object</param>
        ''' <returns>Returns "Mr." respectively "Ms." in the current language</returns>
        <Obsolete("Use UserInformation class instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_GetUserAddresses(ByVal UserInfo As UserInformation) As String
            If UserInfo.Gender = Sex.Feminin Then
                System_GetUserAddresses = Internationalization.UserManagementAddressesMs
            ElseIf UserInfo.Gender = Sex.Masculin Then
                System_GetUserAddresses = Internationalization.UserManagementAddressesMr
            Else
                System_GetUserAddresses = ""
            End If
        End Function

        ''' <summary>
        '''     Get the letter salutation based on the gender of the given user
        ''' </summary>
        ''' <param name="MyUserID">A user ID</param>
        ''' <returns>Returns "Dear Mr." respectively "Dear Ms." in the current language</returns>
        <Obsolete("use UserInfo.Salutation* functions to retrieve the complete salutation string"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_GetUserFormOfAddress(ByVal MyUserID As Integer) As String
            Dim BufferedValue As String
            Try
                BufferedValue = CompuMaster.camm.WebManager.DataLayer.Current.GetUserDetail(Me, MyUserID, "Sex")
                If LCase(BufferedValue) = "m" Then
                    System_GetUserFormOfAddress = Internationalization.UserManagementEMailTextDearMr
                Else
                    System_GetUserFormOfAddress = Internationalization.UserManagementEMailTextDearMs
                End If

            Catch
                System_GetUserFormOfAddress = Internationalization.UserManagementEMailTextDearMr
            End Try
        End Function
        ''' <summary>
        '''     Get the letter salutation based on the gender of the given user
        ''' </summary>
        ''' <param name="MyUserID">A user ID</param>
        ''' <returns>Returns "Dear Mr." respectively "Dear Ms." in the current language</returns>
        <Obsolete("use UserInfo.Salutation* functions to retrieve the complete salutation string"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_GetUserFormOfAddress(ByVal MyUserID As Long) As String
            Dim BufferedValue As String

            Try

                BufferedValue = CompuMaster.camm.WebManager.DataLayer.Current.GetUserDetail(Me, MyUserID, "Sex")
                If LCase(BufferedValue) = "m" Then
                    System_GetUserFormOfAddress = Internationalization.UserManagementEMailTextDearMr
                Else
                    System_GetUserFormOfAddress = Internationalization.UserManagementEMailTextDearMs
                End If

            Catch
                System_GetUserFormOfAddress = Internationalization.UserManagementEMailTextDearMr
            End Try

        End Function
        ''' <summary>
        '''     Get the letter salutation based on the gender of the given user
        ''' </summary>
        ''' <param name="UserInfo">A user information object</param>
        ''' <returns>Returns "Dear Mr." respectively "Dear Ms." in the current language</returns>
        <Obsolete("Use UserInformation class instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_GetUserFormOfAddress(ByVal UserInfo As UserInformation) As String
            If UserInfo.Gender = Sex.Feminin Then
                Return Internationalization.UserManagementEMailTextDearMs
            ElseIf UserInfo.Gender = Sex.Masculin Then
                Return Internationalization.UserManagementEMailTextDearMr
            ElseIf UserInfo.Gender = Sex.Undefined Then
                Return Internationalization.UserManagementEMailTextDearUndefinedGender
            Else
                Return ""
            End If
        End Function

        ''' <summary>
        '''     Get the complete name of a user with academic title, first name, last name
        ''' </summary>
        ''' <returns>The complete name of the current user, for example "Dr. Paul van Kampen" or DBNull if the user doesn't exist</returns>
        <Obsolete("Use CurrentUserInfo.FullName instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetCurUserCompleteName() As Object
            Try
                Return Me.System_GetCurUserInfo.FullName
            Catch
                Return DBNull.Value
            End Try
        End Function

        ''' <summary>
        '''     Get a string with all logon servers for a user 
        ''' </summary>
        ''' <param name="userID">A user ID</param>
        ''' <returns>A string with all relative server groups; every server group is placed in a new line.</returns>
        ''' <remarks>
        '''     If there is only 1 server group available, the returned string contains only the simply URL of the master server of this server group.
        '''     Are there 2 or more server groups available then each URL of the corresponding master server is followed by the server group title in parenthesis.
        ''' </remarks>
        <Obsolete("Use UserInformation class instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_GetUserLogonServers(ByVal userID As Object) As String
            Return CompuMaster.camm.WebManager.DataLayer.Current.GetUserLogonServers(Me, CType(userID, Long))
        End Function

        ' TODO: remove, has moved to utils class as "Nz"
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        <Obsolete("Use Utils.Nz instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Shared Function System_Nz(ByVal CheckValueIfDBNull As Object, Optional ByVal ReplaceWithThis As Object = Nothing) As Object
            Return Utils.Nz(CheckValueIfDBNull, ReplaceWithThis)
        End Function

        ''' <summary>
        '''     Queries detailed information for a language
        ''' </summary>
        ''' <param name="LanguageID">The ID of the requested language</param>
        ''' <param name="DescriptionType">The type of the returned value: "Abbreviation", "Description_OwnLanguage", "Description_English", "Description_German", "BrowserLanguageID", "AlternativeLanguage", "IsActive"</param>
        ''' <returns>The requested value of the requested language</returns>
        <Obsolete("Use System_GetLanguagesInfo instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetLanguageTitle(ByVal LanguageID As Integer, ByVal DescriptionType As String) As Object
            Dim MyDBConn As New SqlConnection
            Dim MyRecSet As SqlDataReader = Nothing
            Dim MyCmd As New SqlCommand

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "SELECT * FROM Languages WHERE ID = " & LanguageID.ToString
                    .CommandType = CommandType.Text

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                MyRecSet = MyCmd.ExecuteReader()

                Try
                    If MyRecSet.Read Then
                        System_GetLanguageTitle = MyRecSet(DescriptionType)
                    Else
                        System_GetLanguageTitle = DBNull.Value
                    End If
                Catch
                    System_GetLanguageTitle = DBNull.Value
                End Try

            Finally
                If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then
                    MyRecSet.Close()
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

        End Function

        ''' <summary>
        '''     Set the activation status for a market/language
        ''' </summary>
        ''' <param name="marketID"></param>
        ''' <param name="activated"></param>
        Public Sub System_SetLanguageState(ByVal marketID As Integer, ByVal activated As Boolean)
            Dim MyCmd As New SqlCommand("UPDATE dbo.System_Languages SET IsActive = @Value WHERE ID = @ID", New SqlConnection(Me.ConnectionString))
            MyCmd.Parameters.Add("@Value", SqlDbType.Bit).Value = activated
            MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = marketID
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Sub

        ''' <summary>
        '''     Queries for all existing server groups
        ''' </summary>
        ''' <returns>A hashtable with ID and name of all server groups</returns>
        Public Function System_GetServerGroups() As Hashtable
            Dim MyDBConn As New SqlConnection
            Dim MyRecSet As SqlDataReader = Nothing
            Dim MyCmd As New SqlCommand
            Dim MyHashTable As New Hashtable

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "SELECT ID, ServerGroup FROM System_ServerGroups"
                    .CommandType = CommandType.Text

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                MyRecSet = MyCmd.ExecuteReader()

                While MyRecSet.Read
                    MyHashTable.Add(MyRecSet("ID"), MyRecSet("ServerGroup"))
                End While

            Finally
                If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then
                    MyRecSet.Close()
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

            Return MyHashTable

        End Function

        ''' <summary>
        '''     This method is for compatibility purposes only and subject of getting friend member in newer versions
        ''' </summary>
        ''' <returns>A user login name or nothing if unsuccessfull</returns>
        ''' <remarks>
        '''     USE ONLY FOR DEBUG/LOG OUTPUT!
        '''     Tries to retrieve the user login name by the current script engine session ID of the current web server
        ''' </remarks>
        Public Function System_GetUserNameByScriptEngineSessionID() As String
            If ConnectionString = "" OrElse Me.CurrentServerIdentString = "" Then
                'WebManager hasn't been correctly initialized; it is only a fake installation, currently
                Return Nothing
            End If

            Dim MyDBConn As New SqlConnection
            Dim MyCmd As New SqlCommand
            Dim Result As String

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "Public_GetUserNameForScriptEngineSessionID"
                    .CommandType = CommandType.StoredProcedure

                    Dim SessionID As String = CurrentScriptEngineSessionID
                    .Parameters.Add("@UserName", SqlDbType.NVarChar).Direction = ParameterDirection.Output
                    .Parameters.Add("@ScriptEngine_SessionID", SqlDbType.NVarChar).Value = SessionID
                    If HttpContext.Current Is Nothing Then
                        .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = ScriptEngines.NetClient
                    Else
                        .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = ScriptEngines.ASPNet
                    End If
                    .Parameters.Add("@ServerID", SqlDbType.Int).Value = CType(Me.System_GetServerConfig(CurrentServerIdentString, "ID"), Integer)

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                MyCmd.ExecuteNonQuery()

                Result = Utils.Nz(MyCmd.Parameters("@UserName").Value, "")

            Finally
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

            Return Result

        End Function

        Public Enum System_AccessAuthorizationChecks_DBResults
            InvalidServer = -10
            UserCannotLoginOnThisServer = -9
            AccessDeniedButLoginSuccessfull = -5
            AlreadyLoggedOn = -4
            AccessDenied = -3
            AccountLockedTemporary = -2
            AccessGranted = -1
            AccountLocked = 44
            AccountNotFound = 43
            <Obsolete("Logins on other servers are allowed, now")> LoginFromAnotherSystem = 57
            LoginRequired = 58
            LoginFailed = 0
            UnexpectedError = 2
        End Enum
        Public Enum System_AccessAuthorizationChecks_ErrorPageForwarderIDs
            ErrorWrongNetwork = 16
            ErrorCookiesMustNotBeDisabled = 17
            ErrorServerConfigurationError = 25
            ErrorNoAuthorization = 27
            ErrorAlreadyLoggedOn = 29
            <Obsolete("Logins on other servers are allowed, now")> ErrorLoggedOutBecauseLoggedOnAtAnotherMachine = 30
            ErrorLogonFailedTooOften = 34
            ErrorSendPWWrongLoginOrEmailAddress = 69
            ErrorApplicationConfigurationIsEmpty = 134
            ErrorUndefined = 24
        End Enum
        Public Enum System_AccessAuthorizationChecks_LoginPageForwarderIDs
            InfoUserLoggedOutSuccessfully = 44
            ErrorUserOrPasswordWrong = 4
            ErrorEmptyPassword = 2
        End Enum

        ''' <summary>
        '''     Checks for authorization of the current user for the given security object name and creates a log entry for a page view
        ''' </summary>
        ''' <param name="securityObjectName">The security object ID which the user should be authorized for</param>
        ''' <param name="intReserved">Reserved. Do not use.</param>
        ''' <param name="serverIP">The identification string of that server where the user should has got authorization for the given security object</param>
        ''' <param name="allowSecurityCache_Read">Allow to read the security settings from a cache, recommended for lower security issues where performance is more important</param>
        ''' <param name="allowSecurityCache_Write">Allow to write the security to a cache, recommended for lower security issues where performance is more important</param>
        ''' <returns>The value with detailled information on successfull or unsuccessfull validation</returns>
        Public Function System_CheckForAccessAuthorization_NoRedirect(ByVal securityObjectName As String, Optional ByVal intReserved As Object = Nothing, Optional ByVal serverIP As String = Nothing, Optional ByVal allowSecurityCache_Read As Boolean = False, Optional ByVal allowSecurityCache_Write As Boolean = False) As System_AccessAuthorizationChecks_DBResults
            If Not Utils.Nz(intReserved) Is Nothing AndAlso Utils.TryCInt(intReserved) <> 0 Then
                Return System_CheckForAccessAuthorization_NoRedirect(securityObjectName, Nothing, True, serverIP, allowSecurityCache_Read, allowSecurityCache_Write)
            Else
                Return System_CheckForAccessAuthorization_NoRedirect(securityObjectName, Nothing, False, serverIP, allowSecurityCache_Read, allowSecurityCache_Write)
            End If
        End Function

        ''' <summary>
        '''     Checks for authorization of the current user for the given security object name and creates a log entry for a page view
        ''' </summary>
        ''' <param name="securityObjectName">The security object ID which the user should be authorized for</param>
        ''' <param name="redirect2URL">A reference to a string variable which shall be set with the suggested URL for redirection after a successfull validation. It's recommended to check the result value, first.</param>
        ''' <param name="loggingSuccessDisabled">Disable hit logging</param>
        ''' <param name="serverIP">The identification string of that server where the user should has got authorization for the given security object</param>
        ''' <param name="allowSecurityCache_Read">Allow to read the security settings from a cache, recommended for lower security issues where performance is more important</param>
        ''' <param name="allowSecurityCache_Write">Allow to write the security to a cache, recommended for lower security issues where performance is more important</param>
        ''' <returns>The value with detailled information on successfull or unsuccessfull validation</returns>
        ''' <remarks>
        '''    Caution: if cache validation is used and if it is also successful, page hits will not be logged for log analysis
        ''' </remarks>
        Private Function System_CheckForAccessAuthorization_NoRedirect(ByVal securityObjectName As String, ByRef redirect2URL As String, ByVal loggingSuccessDisabled As Boolean, Optional ByVal serverIP As String = Nothing, Optional ByVal allowSecurityCache_Read As Boolean = False, Optional ByVal allowSecurityCache_Write As Boolean = False) As System_AccessAuthorizationChecks_DBResults
            Dim Result As System_AccessAuthorizationChecks_DBResults = _System_CheckForAccessAuthorization_NoRedirect(securityObjectName, redirect2URL, loggingSuccessDisabled, serverIP, allowSecurityCache_Read, allowSecurityCache_Write)

            'Save the state that we already successfully checked the security access
            If Result = System_AccessAuthorizationChecks_DBResults.AccessGranted Then
                Me._SecurityObjectSuccessfullyTested = securityObjectName
            End If

            Return Result
        End Function

        ''' <summary>
        '''     Checks for authorization of the current user for the given security object name and creates a log entry for a page view
        ''' </summary>
        ''' <param name="securityObjectName">The security object ID which the user should be authorized for</param>
        ''' <param name="redirect2URL">A reference to a string variable which shall be set with the suggested URL for redirection after a successfull validation. It's recommended to check the result value, first.</param>
        ''' <param name="loggingSuccessDisabled">Disable hit logging</param>
        ''' <param name="serverIP">The identification string of that server where the user should has got authorization for the given security object</param>
        ''' <param name="allowSecurityCache_Read">Allow to read the security settings from a cache, recommended for lower security issues where performance is more important</param>
        ''' <param name="allowSecurityCache_Write">Allow to write the security to a cache, recommended for lower security issues where performance is more important</param>
        ''' <returns>The value with detailled information on successfull or unsuccessfull validation</returns>
        ''' <remarks>
        '''    Caution: if cache validation is used and if it is also successful, page hits will not be logged for log analysis
        ''' </remarks>
        Private Function _System_CheckForAccessAuthorization_NoRedirect(ByVal securityObjectName As String, ByRef redirect2URL As String, ByVal loggingSuccessDisabled As Boolean, Optional ByVal serverIP As String = Nothing, Optional ByVal allowSecurityCache_Read As Boolean = False, Optional ByVal allowSecurityCache_Write As Boolean = False) As System_AccessAuthorizationChecks_DBResults
            Dim Result As System_AccessAuthorizationChecks_DBResults

            Try

                Dim strRemoteIP As String
                Dim User_Auth_Validation_DBConn As New SqlConnection
                Dim User_Auth_Validation_RecSet As SqlDataReader = Nothing
                Dim User_Auth_Validation_Cmd As New SqlCommand

                If LCase(securityObjectName) = "@@anonymous" OrElse (LCase(securityObjectName) = "@@public" AndAlso Me.IsLoggedOn) Then
                    'Anonymous or Public access allowed, no page view/access logging 
                    '(very fast since there is no need for a database query)
                    'ATTENTION: no document access will be logged on membership check!
                    Return System_AccessAuthorizationChecks_DBResults.AccessGranted
                ElseIf Configuration.SecurityRecognizeCrawlersAsAnonymous = True AndAlso Not HttpContext.Current Is Nothing AndAlso Utils.IsRequestFromCrawlerAgent(HttpContext.Current.Request) = True Then
                    'Crawlers are never authorized for anything except for @@anonymous - if configured for this behaviour
                    'ATTENTION: no document access will be logged on membership check!
                    Return System_AccessAuthorizationChecks_DBResults.LoginRequired
                ElseIf (LCase(securityObjectName) = "@@public" AndAlso Not Me.IsLoggedOn) Then
                    'Login pending - let the stored procedure do its job
                    securityObjectName = "Public"
                ElseIf (LCase(securityObjectName) = "@@supervisor" OrElse LCase(securityObjectName) = "@@supervisors") AndAlso Me.IsLoggedOn AndAlso Me.System_IsSuperVisor(Me.CurrentUserID(SpecialUsers.User_Anonymous)) Then
                    'ATTENTION: no document access will be logged on membership check!
                    Return System_AccessAuthorizationChecks_DBResults.AccessGranted
                ElseIf Mid(securityObjectName, 1, 2) = "@@" AndAlso securityObjectName.Length > 2 AndAlso Me.IsLoggedOn AndAlso System_IsMember(Me.CurrentUserID(SpecialUsers.User_Anonymous), Mid(securityObjectName, 3)) Then
                    'ATTENTION: no document access will be logged on membership check!
                    Return System_AccessAuthorizationChecks_DBResults.AccessGranted
                End If
                If Trim(securityObjectName) = "" Then
                    'Invalid application name
                    Me.RedirectToErrorPage(System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorApplicationConfigurationIsEmpty, "System_CheckForAccessAuthorization_NoRedirect shall validate with the content of SecurityObjectName, but it hasn't been set", Nothing)
                End If

                If serverIP = "" Then serverIP = CurrentServerIdentString

                If allowSecurityCache_Read = True Then
                    'Read cached security status
                    Dim AuthorizationStatus As Boolean
                    AuthorizationStatus = _LoadAuthorizationStatusFromCache(CType(System_GetUserID(Me.CurrentUserLoginName, True), Long), securityObjectName, serverIP)
                    If AuthorizationStatus = True Then
                        Return System_AccessAuthorizationChecks_DBResults.AccessGranted
                    End If
                End If

                'HttpContext.Current.Response.ExpiresAbsolute = Now.AddDays(-1)
                'HttpContext.Current.Response.AddHeader("pragma", "no-cache")
                'HttpContext.Current.Response.AddHeader("cache-control", "private, no-cache, must-revalidate")

                Dim strWebURL As String
                If HttpContext.Current Is Nothing Then
                    strWebURL = Nothing
                Else
                    If HttpContext.Current.Request.QueryString.ToString = "" Then
                        strWebURL = HttpContext.Current.Request.ServerVariables("SCRIPT_NAME")
                    Else
                        strWebURL = HttpContext.Current.Request.ServerVariables("SCRIPT_NAME") & "?" & HttpContext.Current.Request.QueryString.ToString
                    End If
                End If

                strRemoteIP = CurrentRemoteClientAddress

                'Create connection
                User_Auth_Validation_DBConn.ConnectionString = ConnectionString
                Dim RecSetIsRead As Boolean
                Try
                    User_Auth_Validation_DBConn.Open()

                    'Get parameter value and append parameter
                    With User_Auth_Validation_Cmd

                        .CommandText = "Public_ValidateDocument"
                        .CommandType = CommandType.StoredProcedure

                        If Not Me.IsLoggedOn Then
                            .Parameters.Add("@Username", SqlDbType.NVarChar).Value = DBNull.Value
                        Else
                            .Parameters.Add("@Username", SqlDbType.NVarChar).Value = Me.CurrentUserLoginName
                        End If
                        .Parameters.Add("@ServerIP", SqlDbType.NVarChar).Value = CStr(serverIP)
                        .Parameters.Add("@RemoteIP", SqlDbType.NVarChar).Value = CStr(strRemoteIP)
                        .Parameters.Add("@WebApplication", SqlDbType.NVarChar, 1024).Value = CStr(Mid(securityObjectName, 1, 1024))
                        .Parameters.Add("@WebURL", SqlDbType.NVarChar, 1024).Value = CStr(Mid(strWebURL, 1, 1024))
                        If HttpContext.Current Is Nothing Then
                            .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = ScriptEngines.NetClient
                        Else
                            .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = ScriptEngines.ASPNet
                        End If
                        .Parameters.Add("@ScriptEngine_SessionID", SqlDbType.NVarChar).Value = CurrentScriptEngineSessionID
                        'Logging success/standard hit yes/no
                        If loggingSuccessDisabled Then
                            .Parameters.Add("@Reserved", SqlDbType.Int).Value = 1
                        Else
                            .Parameters.Add("@Reserved", SqlDbType.Int).Value = 0
                        End If

                    End With

                    If System_DebugLevel >= DebugLevels.Medium_LoggingOfDebugInformation Then
                        HttpContext.Current.Response.Write("<h3>SQL Parameters</h3><ul>")
                        For Each MyParam As System.Data.SqlClient.SqlParameter In User_Auth_Validation_Cmd.Parameters
                            HttpContext.Current.Response.Write("<li>" & MyParam.ParameterName & "=" & Utils.Nz(MyParam.Value, "DBNull.Value") & "</li>")
                        Next
                        HttpContext.Current.Response.Write("</ul>")
                    End If

                    'Create recordset by executing the command
                    User_Auth_Validation_Cmd.Connection = User_Auth_Validation_DBConn
                    User_Auth_Validation_RecSet = User_Auth_Validation_Cmd.ExecuteReader()

                    RecSetIsRead = User_Auth_Validation_RecSet.Read
                    If RecSetIsRead Then
                        Result = CType(Utils.Nz(User_Auth_Validation_RecSet(0), CType(System_AccessAuthorizationChecks_DBResults.UnexpectedError, Integer)), System_AccessAuthorizationChecks_DBResults)
                    Else
                        Result = System_AccessAuthorizationChecks_DBResults.UnexpectedError 'No ReturnValue because RecordSet empty
                    End If

                    If User_Auth_Validation_RecSet.FieldCount >= 2 AndAlso Not User_Auth_Validation_RecSet(1) Is Nothing AndAlso Not LCase(Me.CurrentUserLoginName) = LCase(User_Auth_Validation_RecSet(1).ToString) Then
                        'Prepare a redirection URL for the calling method
                        redirect2URL = (Internationalization.User_Auth_Validation_LogonScriptURL & "?su=" & System.Web.HttpUtility.UrlEncode(Me.CurrentUserLoginName) & "&ru=" & System.Web.HttpUtility.UrlEncode(User_Auth_Validation_RecSet(1).ToString))
                    End If

                Finally
                    If Not User_Auth_Validation_RecSet Is Nothing AndAlso Not User_Auth_Validation_RecSet.IsClosed Then
                        User_Auth_Validation_RecSet.Close()
                    End If
                    If Not User_Auth_Validation_Cmd Is Nothing Then
                        User_Auth_Validation_Cmd.Dispose()
                    End If
                    If Not User_Auth_Validation_DBConn Is Nothing Then
                        If User_Auth_Validation_DBConn.State <> ConnectionState.Closed Then
                            User_Auth_Validation_DBConn.Close()
                        End If
                        User_Auth_Validation_DBConn.Dispose()
                    End If
                End Try

                If allowSecurityCache_Write = True AndAlso RecSetIsRead Then 'and not read from the cache
                    _SaveAuthorizationInCache(CType(System_GetUserID(Me.CurrentUserLoginName, True), Long), securityObjectName, serverIP, True)
                End If

                Return Result

            Catch ex As Exception
                Me.Log.RuntimeException(ex, False, False)
                Return System_AccessAuthorizationChecks_DBResults.UnexpectedError
            End Try

        End Function


        ''' <summary>
        '''     Redirects to the logon page
        ''' </summary>
        ''' <param name="DebugDetailsOnRedirectionCause">Additional information usefull for debugging purposes</param>
        ''' <param name="DebugDetailsOnRequest">Additional information usefull for debugging purposes</param>
        Public Sub RedirectToLogonPage(ByVal DebugDetailsOnRedirectionCause As String, ByVal DebugDetailsOnRequest As Collections.Specialized.NameValueCollection)
            Dim Redirect2URL As String
            Redirect2URL = Internationalization.User_Auth_Validation_LogonScriptURL
            'Redirect now
            RedirectTo(Redirect2URL, DebugDetailsOnRedirectionCause, DebugDetailsOnRequest)
        End Sub
        ''' <summary>
        '''     Redirects to the logon page
        ''' </summary>
        ''' <param name="ErrorType">The cause why the user gets redirected to the logon page</param>
        ''' <param name="DebugDetailsOnRedirectionCause">Additional information usefull for debugging purposes</param>
        ''' <param name="DebugDetailsOnRequest">Additional information usefull for debugging purposes</param>
        Public Sub RedirectToLogonPage(ByVal ErrorType As WMSystem.System_AccessAuthorizationChecks_LoginPageForwarderIDs, ByVal DebugDetailsOnRedirectionCause As String, ByVal DebugDetailsOnRequest As Collections.Specialized.NameValueCollection)
            Dim Redirect2URL As String
            Redirect2URL = Internationalization.User_Auth_Validation_LogonScriptURL
            If InStr(Redirect2URL, "?") > 0 Then
                Redirect2URL &= "&"
            Else
                Redirect2URL &= "?"
            End If
            Redirect2URL &= "ErrID=" & System.Web.HttpUtility.UrlEncode(CType(ErrorType, Integer).ToString)
            'Redirect now
            RedirectTo(Redirect2URL, DebugDetailsOnRedirectionCause, DebugDetailsOnRequest)
        End Sub
        ''' <summary>
        '''     Redirects to the logon page
        ''' </summary>
        ''' <param name="Redirect2ThisUrlAfterSuccessfullLogon">Redirect to this URL after a successfull login</param>
        ''' <param name="DebugDetailsOnRedirectionCause">Additional information usefull for debugging purposes</param>
        ''' <param name="DebugDetailsOnRequest">Additional information usefull for debugging purposes</param>
        Public Sub RedirectToLogonPage(ByVal Redirect2ThisUrlAfterSuccessfullLogon As String, ByVal DebugDetailsOnRedirectionCause As String, ByVal DebugDetailsOnRequest As Collections.Specialized.NameValueCollection)
            Dim Redirect2URL As String
            Redirect2URL = Internationalization.User_Auth_Validation_LogonScriptURL
            If Redirect2ThisUrlAfterSuccessfullLogon <> "" Then
                If InStr(Redirect2URL, "?") > 0 Then
                    Redirect2URL &= "&"
                Else
                    Redirect2URL &= "?"
                End If
                Redirect2URL &= "ref=" & System.Web.HttpUtility.UrlEncode(Redirect2ThisUrlAfterSuccessfullLogon)
            End If
            'Redirect now
            RedirectTo(Redirect2URL, DebugDetailsOnRedirectionCause, DebugDetailsOnRequest)
        End Sub
        ''' <summary>
        '''     Redirects to the logon page
        ''' </summary>
        ''' <param name="su"></param>
        ''' <param name="ru"></param>
        ''' <param name="DebugDetailsOnRedirectionCause">Additional information usefull for debugging purposes</param>
        ''' <param name="DebugDetailsOnRequest">Additional information usefull for debugging purposes</param>
        Public Sub RedirectToLogonPage(ByVal su As String, ByVal ru As String, ByVal DebugDetailsOnRedirectionCause As String, ByVal DebugDetailsOnRequest As Collections.Specialized.NameValueCollection)
            Dim Redirect2URL As String
            Redirect2URL = Internationalization.User_Auth_Validation_LogonScriptURL
            If InStr(Redirect2URL, "?") > 0 Then
                Redirect2URL &= "&"
            Else
                Redirect2URL &= "?"
            End If
            Redirect2URL &= "su=" & System.Web.HttpUtility.UrlEncode(su) & "&ru=" & System.Web.HttpUtility.UrlEncode(ru)
            'Redirect now
            RedirectTo(Redirect2URL, DebugDetailsOnRedirectionCause, DebugDetailsOnRequest)
        End Sub

        ''' <summary>
        '''     Redirects to the error page
        ''' </summary>
        ''' <param name="errorDetails">The cause (free details) why the user gets redirected to the error page</param>
        ''' <param name="debugDetailsOnRedirectionCause">Additional information usefull for debugging purposes</param>
        ''' <param name="debugDetailsOnRequest">Additional information usefull for debugging purposes</param>
        Public Sub RedirectToErrorPage(ByVal errorDetails As String, ByVal debugDetailsOnRedirectionCause As String, ByVal debugDetailsOnRequest As Collections.Specialized.NameValueCollection)
            RedirectToErrorPage(Nothing, errorDetails, debugDetailsOnRedirectionCause, debugDetailsOnRequest)
        End Sub
        ''' <summary>
        '''     Redirects to the error page
        ''' </summary>
        ''' <param name="errorType">The cause why the user gets redirected to the error page</param>
        ''' <param name="debugDetailsOnRedirectionCause">Additional information usefull for debugging purposes</param>
        ''' <param name="debugDetailsOnRequest">Additional information usefull for debugging purposes</param>
        Public Sub RedirectToErrorPage(ByVal errorType As WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs, ByVal debugDetailsOnRedirectionCause As String, ByVal debugDetailsOnRequest As Collections.Specialized.NameValueCollection)
            RedirectToErrorPage(errorType, Nothing, debugDetailsOnRedirectionCause, debugDetailsOnRequest)
        End Sub
        ''' <summary>
        '''     Redirects to the error page
        ''' </summary>
        ''' <param name="errorType">The cause why the user gets redirected to the error page</param>
        ''' <param name="errorDetails">Exception details to display to the user</param>
        ''' <param name="debugDetailsOnRedirectionCause">Additional information usefull for debugging purposes</param>
        ''' <param name="debugDetailsOnRequest">Additional information usefull for debugging purposes</param>
        ''' <param name="displayFrameSet"></param>
        Public Sub RedirectToErrorPage(ByVal errorType As WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs, ByVal errorDetails As String, ByVal debugDetailsOnRedirectionCause As String, ByVal debugDetailsOnRequest As Collections.Specialized.NameValueCollection, Optional ByVal displayFrameSet As Boolean = True)
            RedirectToErrorPage(errorType, errorDetails, debugDetailsOnRedirectionCause, debugDetailsOnRequest, Nothing, Nothing, displayFrameSet)
        End Sub
        ''' <summary>
        '''     Redirects to the error page
        ''' </summary>
        ''' <param name="errorType">The cause why the user gets redirected to the error page</param>
        ''' <param name="errorDetails">The cause (free text) why the user gets redirected to the error page</param>
        ''' <param name="debugDetailsOnRedirectionCause">Additional information usefull for debugging purposes</param>
        ''' <param name="debugDetailsOnRequest">Additional information usefull for debugging purposes</param>
        ''' <param name="userName">The username which is related to the current exception (depends on SendPassword page only)</param>
        ''' <param name="eMailAddress">The e-mail address related to the current exception (depends on SendPassword page only)</param>
        ''' <param name="displayFrameSet"></param>
        Public Sub RedirectToErrorPage(ByVal ErrorType As WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs, ByVal ErrorDetails As String, ByVal DebugDetailsOnRedirectionCause As String, ByVal DebugDetailsOnRequest As Collections.Specialized.NameValueCollection, ByVal userName As String, ByVal emailAddress As String, Optional ByVal DisplayFrameSet As Boolean = True)
            Dim Redirect2URL As String = Nothing
            If DisplayFrameSet = False Then
                Redirect2URL &= "&DisplayFrameSet=No"
            End If
            If ErrorType <> Nothing Then
                Redirect2URL &= "&ErrID=" & System.Web.HttpUtility.UrlEncode(CType(ErrorType, Integer).ToString)
            End If
            If ErrorDetails <> "" Then
                Redirect2URL &= "&ErrCode=" & System.Web.HttpUtility.UrlEncode(ErrorDetails)
            End If
            If userName <> "" Then
                Redirect2URL &= "&user=" & System.Web.HttpUtility.UrlEncode(userName)
            End If
            If ErrorDetails <> "" Then
                Redirect2URL &= "&email=" & System.Web.HttpUtility.UrlEncode(emailAddress)
            End If
            If Internationalization.User_Auth_Validation_AccessErrorScriptURL = "" Then
                'hasn't been configured yet (for example when CurrentServerIdentString gets set then there is normally no path information on this moment)
                Redirect2URL = "/sysdata/access_error.aspx?" & Mid(Redirect2URL, 2)
            Else
                Redirect2URL = Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?" & Mid(Redirect2URL, 2)
            End If
            'Redirect now
            RedirectTo(Redirect2URL, DebugDetailsOnRedirectionCause, DebugDetailsOnRequest)
        End Sub
        ''' <summary>
        '''     Redirects to another URL
        ''' </summary>
        ''' <param name="RedirectToURL">The address of the new URL</param>
        ''' <param name="DebugDetailsOnRedirectionCause">Additional information usefull for debugging purposes</param>
        ''' <param name="DebugDetailsOnRequest">Additional information usefull for debugging purposes</param>
        Public Sub RedirectTo(ByVal redirectToURL As String, ByVal debugDetailsOnRedirectionCause As String, ByVal debugDetailsOnRequest As Collections.Specialized.NameValueCollection)
            If System_DebugLevel >= DebugLevels.Max_RedirectPageRequestsManually Then
                If debugDetailsOnRedirectionCause = "" Then
                    debugDetailsOnRedirectionCause = "Debug level set to maximum"
                End If
                HttpContext.Current.Response.Clear()
                'HttpContext.Current.Response.ClearHeaders() 'would strip SetCookie headers (e.g. required for ASP.NET cookies) away --> ClearHeaders is not allowed!
                HttpContext.Current.Response.ContentEncoding = New System.Text.UTF8Encoding
                HttpContext.Current.Response.ContentType = "text/html"
                HttpContext.Current.Response.CacheControl = "private"
                HttpContext.Current.Response.Write("<html><body>")
                HttpContext.Current.Response.Write("<p>You will be redirected to <a href=""" & System.Web.HttpUtility.HtmlAttributeEncode(redirectToURL) & """>" & System.Web.HttpUtility.HtmlEncode(redirectToURL) & "</a>")
                HttpContext.Current.Response.Write("<p>" & debugDetailsOnRedirectionCause & "</p>")
                HttpContext.Current.Response.Write("<h3>Validation request details:</h3><p><ul>")
                Dim ServerIPValueSubmitted As Boolean = False
                If Not debugDetailsOnRequest Is Nothing AndAlso debugDetailsOnRequest.Count > 0 Then
                    For Each MyKey As String In debugDetailsOnRequest.Keys
                        HttpContext.Current.Response.Write("<li>" & System.Web.HttpUtility.HtmlEncode(MyKey) & "=" & System.Web.HttpUtility.HtmlEncode(debugDetailsOnRequest(MyKey)) & "</li>")
                        If MyKey.ToLower(System.Globalization.CultureInfo.InvariantCulture) = "serverip" Then
                            ServerIPValueSubmitted = True
                        End If
                    Next
                End If
                If ServerIPValueSubmitted = False Then
                    HttpContext.Current.Response.Write("<li>Server ID=" & Me.CurrentServerIdentString & "</li>")
                End If
                HttpContext.Current.Response.Write("<li>CWM Version=" & Me.System_Version_Ex().ToString & "</li>")
                HttpContext.Current.Response.Write("<li>SafeMode=" & Me.SafeMode.ToString & "</li>")
                HttpContext.Current.Response.Write("<li>ScriptName=" & HttpContext.Current.Request.ServerVariables("SCRIPT_NAME") & "</li>")
                If Not HttpContext.Current.Request Is Nothing Then 'because this case might not be applicable in the HttpApplication events
                    HttpContext.Current.Response.Write("<li>RawURL=" & HttpContext.Current.Request.RawUrl & "</li>")
                End If
                HttpContext.Current.Response.Write("<li>Session(""System_Username"")=" & Trim(CType(HttpContext.Current.Session("System_Username"), String)) & "</li>")
                Try
                    HttpContext.Current.Response.Write("<li>cammWebManager.CurrentUserLoginName=" & Me.CurrentUserLoginName & "</li>")
                Catch ex As Exception
                    HttpContext.Current.Response.Write("<li>cammWebManager.CurrentUserLoginName={" & ex.ToString.Replace(vbNewLine, "<br>") & "}</li>")
                End Try
                If Not Me.IsLoggedOn Then
                    Dim _UserNameBySessionID As String
                    Try 'Function might throw an exception when database is empty
                        _UserNameBySessionID = System.Web.HttpUtility.HtmlEncode(System_GetUserNameByScriptEngineSessionID())
                    Catch ex As Exception
                        If Me.DebugLevel >= DebugLevels.Medium_LoggingOfDebugInformation Then
                            _UserNameBySessionID = "<em>(" & Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(ex.ToString)) & ")</em>"
                        Else
                            _UserNameBySessionID = "<em>(" & System.Web.HttpUtility.HtmlEncode(ex.Message) & ")</em>"
                        End If
                    End Try
                    HttpContext.Current.Response.Write("<li>System_GetUserNameByScriptEngineSessionID=" & _UserNameBySessionID & "</li>")
                End If
                If Not HttpContext.Current.Session Is Nothing Then
                    HttpContext.Current.Response.Write("<li>Session.SessionID=" & HttpContext.Current.Session.SessionID & "</li>")
                    HttpContext.Current.Response.Write("<li>Session.Cwm.SessionID=" & CurrentScriptEngineSessionID & "</li>")
                End If
                If Not HttpContext.Current.Request.UrlReferrer Is Nothing Then
                    HttpContext.Current.Response.Write("<li>UrlReferrer=" & HttpContext.Current.Request.UrlReferrer.ToString & "</li>")
                End If
                HttpContext.Current.Response.Write("</ul></p>")
                'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                Dim WorkaroundEx As New Exception("")
                Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                Try
                    WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                Catch
                End Try
                HttpContext.Current.Response.Write("<h3>Current stack trace:</h3><p><ul>")
                HttpContext.Current.Response.Write(WorkaroundStackTrace.ToString.Replace(ControlChars.CrLf, "<br>").Replace(ControlChars.Lf, "<br>").Replace(ControlChars.Cr, "<br>") & "</li>")
                HttpContext.Current.Response.Write("</ul></p>")
                HttpContext.Current.Response.Write("</body></html>")
                HttpContext.Current.Response.End()
            Else
                CompuMaster.camm.WebManager.Utils.RedirectTemporary(HttpContext.Current, redirectToURL)
            End If
        End Sub
        ''' <summary>
        '''     Redirects to another URL
        ''' </summary>
        ''' <param name="RedirectToURL">The address of the new URL</param>
        Public Sub RedirectTo(ByVal redirectToURL As String)
            RedirectTo(redirectToURL, Nothing, Nothing)
        End Sub
        ''' <summary>
        '''     Redirects to another URL
        ''' </summary>
        ''' <param name="RedirectToURL">The address of the new URL</param>
        ''' <param name="DebugDetailsOnRedirectionCause">Additional information usefull for debugging purposes</param>
        Public Sub RedirectTo(ByVal redirectToURL As String, ByVal debugDetailsOnRedirectionCause As String)
            RedirectTo(redirectToURL, Nothing, Nothing)
        End Sub
        ''' <summary>
        '''     Redirects with POST data to another URL
        ''' </summary>
        ''' <param name="RedirectToURL">The address of the new URL</param>
        ''' <param name="DebugDetailsOnRedirectionCause">Additional information usefull for debugging purposes</param>
        ''' <param name="DebugDetailsOnRequest">Additional information usefull for debugging purposes</param>
        Public Sub RedirectWithPostDataTo(ByVal redirectToURL As String, ByVal postData As Collections.Specialized.NameValueCollection, ByVal debugDetailsOnRedirectionCause As String, ByVal debugDetailsOnRequest As Collections.Specialized.NameValueCollection)
            If System_DebugLevel >= DebugLevels.Max_RedirectPageRequestsManually Then
                If debugDetailsOnRedirectionCause = "" Then
                    debugDetailsOnRedirectionCause = "Debug level set to maximum"
                End If
                HttpContext.Current.Response.Clear()
                HttpContext.Current.Response.Write("<html><body>")
                HttpContext.Current.Response.Write("<p>You will be redirected to " & System.Web.HttpUtility.HtmlAttributeEncode(redirectToURL) & " with additional form data</p>")
                HttpContext.Current.Response.Write(Utils.RedirectWithPostDataCreateFormScript(redirectToURL, postData, "POST", Nothing, False, ""))
                HttpContext.Current.Response.Write("<p>" & debugDetailsOnRedirectionCause & "</p>")
                HttpContext.Current.Response.Write("<h3>Validation request details:</h3><p><ul>")
                Dim ServerIPValueSubmitted As Boolean = False
                If Not debugDetailsOnRequest Is Nothing AndAlso debugDetailsOnRequest.Count > 0 Then
                    For Each MyKey As String In debugDetailsOnRequest.Keys
                        HttpContext.Current.Response.Write("<li>" & System.Web.HttpUtility.HtmlEncode(MyKey) & "=" & System.Web.HttpUtility.HtmlEncode(debugDetailsOnRequest(MyKey)) & "</li>")
                        If MyKey.ToLower(System.Globalization.CultureInfo.InvariantCulture) = "serverip" Then
                            ServerIPValueSubmitted = True
                        End If
                    Next
                End If
                If ServerIPValueSubmitted = False Then
                    HttpContext.Current.Response.Write("<li>Server ID=" & Me.CurrentServerIdentString & "</li>")
                End If
                HttpContext.Current.Response.Write("<li>CWM Version=" & Me.System_Version_Ex().ToString & "</li>")
                Try
                    HttpContext.Current.Response.Write("<li>Session(""CurrentUserLoginName"")=" & Me.CurrentUserLoginName & "</li>")
                Catch ex As Exception
                    HttpContext.Current.Response.Write("<li>Session(""CurrentUserLoginName"")={" & ex.ToString.Replace(vbNewLine, "<br>") & "}</li>")
                End Try
                If Not Me.IsLoggedOn Then
                    Dim _UserNameBySessionID As String
                    Try 'Function might throw an exception when database is empty
                        _UserNameBySessionID = System.Web.HttpUtility.HtmlEncode(System_GetUserNameByScriptEngineSessionID())
                    Catch ex As Exception
                        If Me.DebugLevel >= DebugLevels.Medium_LoggingOfDebugInformation Then
                            _UserNameBySessionID = "<em>(" & Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(ex.ToString)) & ")</em>"
                        Else
                            _UserNameBySessionID = "<em>(" & System.Web.HttpUtility.HtmlEncode(ex.Message) & ")</em>"
                        End If
                    End Try
                    HttpContext.Current.Response.Write("<li>SafeMode=" & Me.SafeMode.ToString & "</li>")
                    HttpContext.Current.Response.Write("<li>ScriptName=" & HttpContext.Current.Request.ServerVariables("SCRIPT_NAME") & "</li>")
                    If Not HttpContext.Current.Request Is Nothing Then 'because this case might not be applicable in the HttpApplication events
                        HttpContext.Current.Response.Write("<li>RawURL=" & HttpContext.Current.Request.RawUrl & "</li>")
                    End If
                    HttpContext.Current.Response.Write("<li>System_GetUserNameByScriptEngineSessionID=" & _UserNameBySessionID & "</li>")
                    If Not HttpContext.Current.Session Is Nothing Then
                        HttpContext.Current.Response.Write("<li>Session.SessionID=" & HttpContext.Current.Session.SessionID & "</li>")
                        HttpContext.Current.Response.Write("<li>Session.Cwm.SessionID=" & CurrentScriptEngineSessionID & "</li>")
                    End If
                End If
                If Not HttpContext.Current.Request.UrlReferrer Is Nothing Then
                    HttpContext.Current.Response.Write("<li>UrlReferrer=" & HttpContext.Current.Request.UrlReferrer.ToString & "</li>")
                End If
                HttpContext.Current.Response.Write("</ul></p>")
                'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                Dim WorkaroundEx As New Exception("")
                Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                Try
                    WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                Catch
                End Try
                HttpContext.Current.Response.Write("<h3>Current stack trace:</h3><p><ul>")
                HttpContext.Current.Response.Write(WorkaroundStackTrace.ToString.Replace(ControlChars.CrLf, "<br>").Replace(ControlChars.Lf, "<br>").Replace(ControlChars.Cr, "<br>") & "</li>")
                HttpContext.Current.Response.Write("</ul></p>")
                HttpContext.Current.Response.Write("</body></html>")
                HttpContext.Current.Response.End()
            Else
                Utils.RedirectWithPostData(HttpContext.Current, redirectToURL, postData)
            End If
        End Sub

        ''' <summary>
        '''     Checks for authorization of the current user for the given security object name and creates a log entry for a page view
        ''' </summary>
        ''' <param name="securityObjectName">The security object ID which the user should be authorized for</param>
        ''' <remarks>
        '''     This method is applicable for web applications, only, otherwise it will throw an exception respectively redirect to the logon page
        '''     Authentication caching is not activated for this method
        ''' </remarks>
        Public Sub AuthorizeDocumentAccess(ByVal securityObjectName As String)
            AuthorizeDocumentAccess(securityObjectName, Nothing, False, True) 'writing cache is okay, but not reading
        End Sub

        ''' <summary>
        '''     Checks for authorization of the current user for the given security object name and creates a log entry for a page view
        ''' </summary>
        ''' <param name="securityObjectName">The security object ID which the user should be authorized for</param>
        ''' <param name="serverIdentificationString">The identification string of that server where the user should has got authorization for the given security object</param>
        ''' <remarks>
        '''    This method is applicable for web applications, only, otherwise it will throw an exception respectively redirect to the logon page
        '''    Authentication caching is not activated for this method
        ''' </remarks>
        Public Sub AuthorizeDocumentAccess(ByVal securityObjectName As String, ByVal serverIdentificationString As String)
            AuthorizeDocumentAccess(securityObjectName, serverIdentificationString, False, True) 'writing cache is okay, but not reading
        End Sub

        ''' <summary>
        '''     Checks for authorization of the current user for the given security object name and creates a log entry for a page view
        ''' </summary>
        ''' <param name="securityObjectName">The security object ID which the user should be authorized for</param>
        ''' <param name="allowSecurityCacheRead">Allow to read the security settings from a cache, recommended for lower security issues where performance is more important</param>
        ''' <param name="allowSecurityCacheWrite">Allow to write the security to a cache, recommended for lower security issues where performance is more important</param>
        ''' <remarks>
        '''    This method is applicable for web applications, only, otherwise it will throw an exception respectively redirect to the logon page
        '''    Caution: if cache validation is used and if it is also successful, page hits will not be logged for log analysis
        ''' </remarks>
        Public Sub AuthorizeDocumentAccess(ByVal securityObjectName As String, ByVal allowSecurityCacheRead As Boolean, ByVal allowSecurityCacheWrite As Boolean)
            AuthorizeDocumentAccess(securityObjectName, Nothing, allowSecurityCacheRead, allowSecurityCacheWrite)
        End Sub

        ''' <summary>
        '''     Checks for authorization of the current user for the given security object name and creates a log entry for a page view
        ''' </summary>
        ''' <param name="securityObjectName">The security object ID which the user should be authorized for</param>
        ''' <param name="serverIdentificationString">The identification string of that server where the user should has got authorization for the given security object</param>
        ''' <param name="allowSecurityCacheRead">Allow to read the security settings from a cache, recommended for lower security issues where performance is more important</param>
        ''' <param name="allowSecurityCacheWrite">Allow to write the security to a cache, recommended for lower security issues where performance is more important</param>
        ''' <remarks>
        '''    This method is applicable for web applications, only, otherwise it will throw an exception respectively redirect to the logon page
        '''    Caution: if cache validation is used and if it is also successful, page hits will not be logged for log analysis
        ''' </remarks>
        Public Sub AuthorizeDocumentAccess(ByVal securityObjectName As String, ByVal serverIdentificationString As String, ByVal allowSecurityCacheRead As Boolean, ByVal allowSecurityCacheWrite As Boolean)
            AuthorizeDocumentAccess(securityObjectName, serverIdentificationString, allowSecurityCacheRead, allowSecurityCacheWrite, True)
        End Sub

        ''' <summary>
        '''     Checks for authorization of the current user for the given security object name and creates a log entry for a page view
        ''' </summary>
        ''' <param name="securityObjectName">The security object ID which the user should be authorized for</param>
        ''' <param name="serverIdentificationString">The identification string of that server where the user should has got authorization for the given security object</param>
        ''' <param name="allowSecurityCacheRead">Allow to read the security settings from a cache, recommended for lower security issues where performance is more important</param>
        ''' <param name="allowSecurityCacheWrite">Allow to write the security to a cache, recommended for lower security issues where performance is more important</param>
        ''' <param name="logPageHit">Log the page access as a hit in the web access logs</param>
        ''' <remarks>
        '''    This method is applicable for web applications, only, otherwise it will throw an exception respectively redirect to the logon page
        '''    Caution: if cache validation is used and if it is also successful, page hits will not be logged for log analysis
        ''' </remarks>
        Public Sub AuthorizeDocumentAccess(ByVal securityObjectName As String, ByVal serverIdentificationString As String, ByVal allowSecurityCacheRead As Boolean, ByVal allowSecurityCacheWrite As Boolean, ByVal logPageHit As Boolean)
            _System_CheckForAccessAuthorization(securityObjectName, Not logPageHit, serverIdentificationString, allowSecurityCacheRead, allowSecurityCacheWrite)
        End Sub
        ''' <summary>
        '''     Redirect to the logon page when the document authorization check resulted with a login required state
        ''' </summary>
        ''' <param name="defaultLogonUrl">The default URL for the logon page as it has been suggested by the method AuthorizeDocumentAccess</param>
        ''' <param name="redirectionCause">A short description why the redirect has happened</param>
        ''' <param name="requestDetails">More information on the requested redirection</param>
        Protected Overridable Sub AuthorizeDocumentAccessRedirectToLogonPageBecauseLoginIsRequired(ByVal defaultLogonUrl As String, ByVal redirectionCause As String, ByVal requestDetails As Collections.Specialized.NameValueCollection)
            RedirectToLogonPage(defaultLogonUrl, redirectionCause, requestDetails)
        End Sub
        ''' <summary>
        '''     Checks for authorization of the current user for the given security object name and creates a log entry for a page view
        ''' </summary>
        ''' <param name="securityObjectName">The security object ID which the user should be authorized for</param>
        ''' <param name="intReserved">Reserved. No further use.</param>
        ''' <param name="serverIP">The identification string of that server where the user should has got authorization for the given security object</param>
        ''' <param name="allowSecurityCache_Read">Allow to read the security settings from a cache, recommended for lower security issues where performance is more important</param>
        ''' <param name="allowSecurityCache_Write">Allow to write the security to a cache, recommended for lower security issues where performance is more important</param>
        ''' <remarks>
        '''    This method is applicable for web applications, only, otherwise it will throw an exception respectively redirect to the logon page
        '''    Caution: if cache validation is used and if it is also successful, page hits will not be logged for log analysis
        ''' </remarks>
        <Obsolete("Use AuthorizeDocumentAccess instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub System_CheckForAccessAuthorization(ByVal securityObjectName As String, Optional ByVal intReserved As Object = Nothing, Optional ByVal serverIP As String = Nothing, Optional ByVal allowSecurityCache_Read As Boolean = False, Optional ByVal allowSecurityCache_Write As Boolean = False)
            If Not Utils.Nz(intReserved) Is Nothing AndAlso Utils.TryCInt(intReserved) <> 0 Then
                _System_CheckForAccessAuthorization(securityObjectName, True, serverIP, allowSecurityCache_Read, allowSecurityCache_Write)
            Else
                _System_CheckForAccessAuthorization(securityObjectName, False, serverIP, allowSecurityCache_Read, allowSecurityCache_Write)
            End If
        End Sub
        ''' <summary>
        '''     Checks for authorization of the current user for the given security object name and creates a log entry for a page view
        ''' </summary>
        ''' <param name="securityObjectName">The security object ID which the user should be authorized for</param>
        ''' <param name="loggingSuccessDisabled">Disable hit logging</param>
        ''' <param name="serverIP">The identification string of that server where the user should has got authorization for the given security object</param>
        ''' <param name="allowSecurityCache_Read">Allow to read the security settings from a cache, recommended for lower security issues where performance is more important</param>
        ''' <param name="allowSecurityCache_Write">Allow to write the security to a cache, recommended for lower security issues where performance is more important</param>
        ''' <remarks>
        '''    This method is applicable for web applications, only, otherwise it will throw an exception respectively redirect to the logon page
        '''    Caution: if cache validation is used and if it is also successful, page hits will not be logged for log analysis
        ''' </remarks>
        Private Sub _System_CheckForAccessAuthorization(ByVal securityObjectName As String, ByVal loggingSuccessDisabled As Boolean, Optional ByVal serverIP As String = Nothing, Optional ByVal allowSecurityCache_Read As Boolean = False, Optional ByVal allowSecurityCache_Write As Boolean = False)

            If HttpContext.Current Is Nothing Then
                Throw New NotSupportedException("This method requires a web application to work")
            End If

            HttpContext.Current.Trace.Write("camm WebManager", "SecurityCheck: Checks for authorization of the current user for the given security object name")
            HttpContext.Current.Session.Timeout = 240 '4 h

            '***************************************************
            '*** Überprüft die Berechtigung für das aktuelle ***
            '*** Dokument, welches diese Prozedur aufruft    ***
            '***************************************************

            Dim bufRedirect2URL As String = String.Empty
            Dim Result As System_AccessAuthorizationChecks_DBResults

            Try
                Result = System_CheckForAccessAuthorization_NoRedirect(securityObjectName, bufRedirect2URL, loggingSuccessDisabled, serverIP, allowSecurityCache_Read, allowSecurityCache_Write)
            Catch ex As Exception
                Dim RedirectionCause As String = "This has been because the validation result is " & Result.ToString
                Dim RequestDetails As New Collections.Specialized.NameValueCollection
                RequestDetails("SecurityObjectName") = securityObjectName
                RequestDetails("ServerIP") = CType(IIf(serverIP <> "", serverIP, "<em>(current server)</em>"), String)
                RedirectToErrorPage(ex.Message, RedirectionCause, RequestDetails)
            End Try

#If VS2015OrHigher = True Then
#Disable Warning BC40000 'disable obsolete warnings because this code must be compatible to .NET 1.1
#End If
            If Result = System_AccessAuthorizationChecks_DBResults.LoginRequired Then
                Dim strWebURL As String = Nothing
                If Not HttpContext.Current Is Nothing Then
                    strWebURL = RemoteHostUrl.ToString
                End If
                Dim RedirectionCause As String = "Login required."
                Dim RequestDetails As New Collections.Specialized.NameValueCollection
                RequestDetails("SecurityObjectName") = securityObjectName
                RequestDetails("ServerIP") = Me.CurrentServerIdentString
                Me.AuthorizeDocumentAccessRedirectToLogonPageBecauseLoginIsRequired(strWebURL, RedirectionCause, RequestDetails)
            ElseIf Result = System_AccessAuthorizationChecks_DBResults.LoginFromAnotherSystem Then
                Dim RedirectionCause As String = "This has been because the validation result is " & Result.ToString
                Dim RequestDetails As New Collections.Specialized.NameValueCollection
                RequestDetails("SecurityObjectName") = securityObjectName
                RequestDetails("ServerIP") = CType(IIf(serverIP <> "", serverIP, "<em>(current server)</em>"), String)
                RedirectToErrorPage(System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorLoggedOutBecauseLoggedOnAtAnotherMachine, RedirectionCause, RequestDetails)
            ElseIf Result = System_AccessAuthorizationChecks_DBResults.InvalidServer Then
                Dim RedirectionCause As String = "This has been because the validation result is " & Result.ToString
                Dim RequestDetails As New Collections.Specialized.NameValueCollection
                RequestDetails("SecurityObjectName") = securityObjectName
                RequestDetails("ServerIP") = CType(IIf(serverIP <> "", serverIP, "<em>(current server)</em>"), String)
                RedirectToErrorPage(System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorWrongNetwork, RedirectionCause, RequestDetails)
            ElseIf Result = System_AccessAuthorizationChecks_DBResults.UserCannotLoginOnThisServer Then
                Dim RedirectionCause As String = "This has been because the validation result is " & Result.ToString
                Dim RequestDetails As New Collections.Specialized.NameValueCollection
                RequestDetails("SecurityObjectName") = securityObjectName
                RequestDetails("ServerIP") = CType(IIf(serverIP <> "", serverIP, "<em>(current server)</em>"), String)
                RedirectToErrorPage(System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorWrongNetwork, RedirectionCause, RequestDetails)
            ElseIf Result = System_AccessAuthorizationChecks_DBResults.AccessDeniedButLoginSuccessfull Then
                Dim RedirectionCause As String = "This has been because the validation result is " & Result.ToString
                Dim RequestDetails As New Collections.Specialized.NameValueCollection
                RequestDetails("SecurityObjectName") = securityObjectName
                RequestDetails("ServerIP") = CType(IIf(serverIP <> "", serverIP, "<em>(current server)</em>"), String)
                Dim strWebURL As String = Nothing
                If Not HttpContext.Current Is Nothing Then
                    strWebURL = RemoteHostUrl.ToString
                End If
                RedirectToErrorPage(System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorNoAuthorization, "Application name=""" & securityObjectName & """; URL=""" & strWebURL & """", RedirectionCause, RequestDetails, True)
            ElseIf Result = System_AccessAuthorizationChecks_DBResults.AlreadyLoggedOn Then
                Dim RedirectionCause As String = "This has been because the validation result is " & Result.ToString
                Dim RequestDetails As New Collections.Specialized.NameValueCollection
                RequestDetails("SecurityObjectName") = securityObjectName
                RequestDetails("ServerIP") = CType(IIf(serverIP <> "", serverIP, "<em>(current server)</em>"), String)
                RedirectToErrorPage(System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorAlreadyLoggedOn, RedirectionCause, RequestDetails)
            ElseIf Result = System_AccessAuthorizationChecks_DBResults.AccessDenied Then
                Dim RedirectionCause As String = "This has been because the validation result is " & Result.ToString
                Dim RequestDetails As New Collections.Specialized.NameValueCollection
                RequestDetails("SecurityObjectName") = securityObjectName
                RequestDetails("ServerIP") = CType(IIf(serverIP <> "", serverIP, "<em>(current server)</em>"), String)
                RedirectToErrorPage(System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorNoAuthorization, RedirectionCause, RequestDetails)
            ElseIf Result = System_AccessAuthorizationChecks_DBResults.AccountLockedTemporary Then
                Dim RedirectionCause As String = "This has been because the validation result is " & Result.ToString
                Dim RequestDetails As New Collections.Specialized.NameValueCollection
                RequestDetails("SecurityObjectName") = securityObjectName
                RequestDetails("ServerIP") = CType(IIf(serverIP <> "", serverIP, "<em>(current server)</em>"), String)
                RedirectToErrorPage(System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorLogonFailedTooOften, RedirectionCause, RequestDetails)
            ElseIf Result = System_AccessAuthorizationChecks_DBResults.AccessGranted Then
                'Access granted! - Extraline because of unknown but existing values which otherwise result in problems
            ElseIf Result = System_AccessAuthorizationChecks_DBResults.LoginFailed Or Result = System_AccessAuthorizationChecks_DBResults.AccountLocked Or Result = System_AccessAuthorizationChecks_DBResults.AccountNotFound Then
                Dim RedirectionCause As String = "This has been because the validation result is " & Result.ToString
                Dim RequestDetails As New Collections.Specialized.NameValueCollection
                RequestDetails("SecurityObjectName") = securityObjectName
                RequestDetails("ServerIP") = CType(IIf(serverIP <> "", serverIP, "<em>(current server)</em>"), String)
                RedirectToLogonPage(System_AccessAuthorizationChecks_LoginPageForwarderIDs.ErrorUserOrPasswordWrong, RedirectionCause, RequestDetails)
            Else 'System_AccessAuthorizationChecks_DBResults.UnexpectedError
                Dim RedirectionCause As String = "This has been because the validation result is " & Result.ToString
                Dim RequestDetails As New Collections.Specialized.NameValueCollection
                RequestDetails("SecurityObjectName") = securityObjectName
                RequestDetails("ServerIP") = CType(IIf(serverIP <> "", serverIP, "<em>(current server)</em>"), String)
                RedirectToErrorPage(System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorUndefined, "ReturnValue = UnknownValue (" & Result & ")", RedirectionCause, RequestDetails)
            End If
#If VS2015OrHigher = True Then
#Enable Warning BC40000 'disable obsolete warnings because this code must be compatible to .NET 1.1
#End If
            'Access has been granted!
            If bufRedirect2URL <> "" Then
                Dim RedirectionCause As String = "CheckForAccessAuthorization asked for this redirect."
                Dim RequestDetails As New Collections.Specialized.NameValueCollection
                RequestDetails("SecurityObjectName") = securityObjectName
                RequestDetails("ServerIP") = CType(IIf(serverIP <> "", serverIP, "<em>(current server)</em>"), String)
                RedirectTo(bufRedirect2URL, RedirectionCause, RequestDetails)
            End If

        End Sub
        ''' <summary>
        '''     The server address as it shall be/has been used by the client
        ''' </summary>
        ''' <remarks>
        '''     In most cases, this Uri is the same as in HttpContext.Current.Request.Url, but there might be some situations, where it's different. When you use a forwarding proxy (e. g. with Apache), the proxy might be configured to the IP address instead of the origin server name because the origin server name has been resolved to your forwarding proxy server. This situation is sometimes used for Intranets, when your firewall configuration doesn't allow a direct access to the genuine webserver. So, this method replaces the host name by the value provided by the HTTP_X_FORWARDED_HOST server variable which will be added by the forwarding proxy server.
        ''' </remarks>
        Protected Overridable Function RemoteHostUrl() As System.Uri
            Try
                'Use URL path as used by client, but use server address as configured
                Dim UriFactory As New System.UriBuilder(HttpContext.Current.Request.Url)
                UriFactory.Scheme = Me.CurrentServerInfo.URL_Protocol
                UriFactory.Host = Me.CurrentServerInfo.URL_DomainName
                If Me.CurrentServerInfo.URL_Port <> Nothing Then
                    UriFactory.Port = Integer.Parse(Me.CurrentServerInfo.URL_Port)
                End If
                Return UriFactory.Uri
            Catch
                'No current server info there - let's try to get the most matching URI based on reported server variables
                If HttpContext.Current.Request.ServerVariables("HTTP_X_FORWARDED_HOST") <> Nothing Then
                    Dim UriFactory As New System.UriBuilder(HttpContext.Current.Request.Url)
                    UriFactory.Host = HttpContext.Current.Request.ServerVariables("HTTP_X_FORWARDED_HOST")
                    Return UriFactory.Uri
                Else
                    Return HttpContext.Current.Request.Url
                End If
            End Try
        End Function
        ''' <summary>
        '''     Login with login name and password
        ''' </summary>
        ''' <param name="LoginName">The login name of a user</param>
        ''' <param name="Password">The password of this user</param>
        ''' <param name="ForceLogin">Force the login even if the user has already logged on at another work station</param>
        ''' <returns>A result value telling some details on successfull or unsuccessfull login</returns>
        <Obsolete("Use Login instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_LoginAsUser(ByVal LoginName As String, ByVal Password As String, Optional ByVal ForceLogin As Boolean = False) As ReturnValues_UserValidation
            Return ExecuteLogin(LoginName, Password, ForceLogin)
        End Function
        ''' <summary>
        '''     Login with login name and password
        ''' </summary>
        ''' <param name="loginName">The login name of a user</param>
        ''' <param name="password">The password of this user</param>
        ''' <param name="forceLogin">Force the login even if the user has already logged on at another work station</param>
        ''' <returns>A result value telling some details on successfull or unsuccessfull login, but web applications return nothing</returns>
        ''' <remarks>
        '''     Web applications require to login/logout to multiple servers, that's why there will be initiated some redirections 
        ''' </remarks>
        Public Function Login(ByVal loginName As String, ByVal password As String, Optional ByVal forceLogin As Boolean = False) As ReturnValues_UserValidation

            If HttpContext.Current Is Nothing Then
                Return ExecuteLogin(loginName, password, forceLogin)
            Else
                'Web login has to use the login page
                Dim ProcessLogonUrl As String = Me.Internationalization.User_Auth_Validation_CheckLoginURL
                If InStr(ProcessLogonUrl, "?") > 0 Then
                    ProcessLogonUrl &= "&Username=" & System.Web.HttpUtility.UrlEncode(loginName) & "&PassECode=" & System.Web.HttpUtility.UrlEncode(CompuMaster.camm.WebManager.Pages.Login.Utils.CryptedPassword(password))
                Else
                    ProcessLogonUrl &= "?Username=" & System.Web.HttpUtility.UrlEncode(loginName) & "&PassECode=" & System.Web.HttpUtility.UrlEncode(CompuMaster.camm.WebManager.Pages.Login.Utils.CryptedPassword(password))
                End If
                Me.RedirectTo(ProcessLogonUrl, "Logon processing", Nothing)
            End If

        End Function
        ''' <summary>
        '''     Login with the account which corresponds to the name of the given, external account
        ''' </summary>
        ''' <param name="externalAccountName">The external account, e. g. from a MS Windows Domain (ADS)</param>
        ''' <returns>True when the user has been logged on, false when there is already another user logged on or when the user can't be found</returns>
        ''' <remarks>
        '''     Do not use this method in your web applications since this method doesn't support server groups. It's intended for offline applications only.
        ''' </remarks>
        Public Function ExecuteLoginWithExternalAccountInfo(ByVal externalAccountName As String, ByVal forceLogin As Boolean) As ReturnValues_UserValidation

            If CurrentServerIdentString = "" Then
                Dim Message As String = "Login can't be processed since this server hasn't been configured with a proper server ID"
                Me.Log.RuntimeException(Message)
            ElseIf Trim(externalAccountName) = Nothing Then
                Dim Message As String = "External account name must not be empty for login"
                Me.Log.RuntimeException(Message)
            End If
            If Me.IsLoggedOn Then
                'A user is already logged on, don't relogon 
                Return ReturnValues_UserValidation.AlreadyLoggedIn
            End If

            Dim UserFilter As WMSystem.UserFilter() = New WMSystem.UserFilter() {New WMSystem.UserFilter("ExternalAccount", WMSystem.UserFilter.SearchMethods.MatchExactly)}
            UserFilter(0).MatchExpressions = New String() {externalAccountName}
            Dim UserSortArgument As WMSystem.UserSortArgument() = Nothing

            Dim UserIDs As Long() = Me.SearchUsers(UserFilter, UserSortArgument)
            If UserIDs.Length > 1 Then
                'There are more than one account which are referred to the same external account
                Dim Message As String = "External account name must be unique"
                Me.Log.RuntimeException(Message)
            ElseIf UserIDs.Length < 1 Then
                'User account can't be found 
                Return ReturnValues_UserValidation.UserForDemandedExternalAccountNotFound
            Else
                'Logon with this user account
                Me.SetUserLoginName(New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserIDs(0), Me).LoginName)
                Return ReturnValues_UserValidation.ValidationSuccessfull
            End If
        End Function
        ''' <summary>
        '''     Login with login name and password
        ''' </summary>
        ''' <param name="loginName">The login name of a user</param>
        ''' <param name="password">The password of this user</param>
        ''' <param name="forceLogin">Force the login even if the user has already logged on at another work station</param>
        ''' <returns>A result value telling some details on successfull or unsuccessfull login</returns>
        Public Function ExecuteLogin(ByVal loginName As String, ByVal password As String, Optional ByVal forceLogin As Boolean = False) As ReturnValues_UserValidation
            If CurrentServerIdentString = "" Then
                Dim Message As String = "Login can't be processed since this server hasn't been configured with a proper server ID"
                Me.Log.RuntimeException(Message)
            ElseIf loginName = "" OrElse password = "" Then
                Dim Message As String = "Loginname and password must not be empty for login"
                Me.Log.RuntimeException(Message)
            End If
            Return _ExecuteLogin(loginName, password, forceLogin)
        End Function
        ''' <summary>
        '''     Login with user name and password
        ''' </summary>
        ''' <param name="loginName">The login name of a user</param>
        ''' <param name="password">The password of this user</param>
        ''' <param name="forceLogin">Force the login even if the user has already logged on at another work station</param>
        ''' <returns>A result value telling some details on successfull or unsuccessfull login</returns>
        Private Function _ExecuteLogin(ByVal loginName As String, ByVal password As String, ByVal forceLogin As Boolean) As ReturnValues_UserValidation

            If CurrentServerIdentString = "" Then
                Dim Message As String = "Login can't be processed since this server hasn't been configured with a proper server ID"
                Me.Log.RuntimeException(Message)
            ElseIf loginName = "" Then
                Dim Message As String = "Loginname and password must not be empty for login"
                Me.Log.RuntimeException(Message)
            ElseIf Me.PerformanceMethods.UserFlag(loginName, "IsImpersonationUser") = "1" Then
                Throw New ImpersonationUserNotAbleToStartLoginProcessException(loginName)
            End If
            If Me.IsLoggedOn Then
                If forceLogin = True Then
                    Me._ExecuteLogout(False) 'Complete web logout not really required, since the webservers will be reassigned now
                Else
                    'A user is already logged on, don't relogon 
                    Return ReturnValues_UserValidation.AlreadyLoggedIn
                End If
            End If
            Dim MyDBConn As New SqlConnection
            Dim MyCmd As New SqlCommand
            Dim Result As ReturnValues_UserValidation

            Dim transformationResult As CryptoTransformationResult = Me.System_GetUserPasswordTransformationResult(loginName)
            Dim transformer As IWMPasswordTransformation = PasswordTransformerFactory.ProduceCryptographicTransformer(transformationResult.Algorithm)
            Dim transformedPassword As String = transformer.TransformString(password, transformationResult.Noncevalue)

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "Public_ValidateUser"
                    .CommandType = CommandType.StoredProcedure

                    .Parameters.Add("@Username", SqlDbType.NVarChar).Value = loginName
                    If password Is Nothing Then
                        'Login without passwords - for single-sign-on scenarios
                        .Parameters.Add("@Passcode", SqlDbType.VarChar).Value = DBNull.Value
                    Else
                        'Login with username and password - the standard case
                        .Parameters.Add("@Passcode", SqlDbType.VarChar).Value = transformedPassword
                    End If
                    .Parameters.Add("@ServerIP", SqlDbType.NVarChar).Value = CurrentServerIdentString
                    .Parameters.Add("@RemoteIP", SqlDbType.NVarChar).Value = CurrentRemoteClientAddress
                    If HttpContext.Current Is Nothing Then
                        .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = ScriptEngines.NetClient
                    Else
                        .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = ScriptEngines.ASPNet
                    End If
                    .Parameters.Add("@ScriptEngine_SessionID", SqlDbType.NVarChar, 512).Value = CurrentScriptEngineSessionID
                    .Parameters.Add("@ForceLogin", SqlDbType.Bit).Value = forceLogin
                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                Result = CType(Utils.Nz(MyCmd.ExecuteScalar, ReturnValues_UserValidation.UnknownError), WMSystem.ReturnValues_UserValidation)

            Finally
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

            'Reset navigation cache
            If Result = ReturnValues_UserValidation.ValidationSuccessfull OrElse Result = ReturnValues_UserValidation.ValidationSuccessfull_ButNoAuthorizationForRequiredSecurityObject Then
                Me.SetUserLoginName(loginName) '_CurrentUserLoginName = loginName
                If Not HttpContext.Current Is Nothing Then
                    Dim CacheObjectName As String
                    If Configuration.CookieLess = False Then
                        CacheObjectName = "System_GetNavItems_" & Me.CurrentUserID(SpecialUsers.User_Anonymous).ToString & "_Reset"
                    Else
                        CacheObjectName = "System_GetNavItems_" & Me.CurrentUserID(SpecialUsers.User_Anonymous).ToString & "_" & Session.SessionID & "_Reset"
                    End If
                    Utils.SetHttpCacheValue(CacheObjectName, True, Caching.CacheItemPriority.NotRemovable)
                End If
            End If

            'return
            Return Result

        End Function

        ''' <summary>
        ''' Pre-validate user login credentials without loggin in
        ''' </summary>
        ''' <param name="loginName">The login name of a user</param>
        ''' <param name="password">The password of this user</param>
        ''' <param name="ignoreCurrentlyLoggedOnState">If True, a currently logged in user will successfully validate, if False it will return with a AlreadyLoggedIn value</param>
        ''' <remarks>In case of mistypings, the login failure number will be increased anyway.</remarks>
        Public Function PreValidateLoginCredentials(ByVal loginName As String, ByVal password As String, ByVal ignoreCurrentlyLoggedOnState As Boolean) As ReturnValues_UserValidation

            If CurrentServerIdentString = "" Then
                Dim Message As String = "Login can't be processed since this server hasn't been configured with a proper server ID"
                Me.Log.RuntimeException(Message)
            ElseIf loginName = "" Then
                Dim Message As String = "Loginname and password must not be empty for login"
                Me.Log.RuntimeException(Message)
            End If

            Dim MyCmd As New SqlCommand
            Dim Result As ReturnValues_UserValidation

            Dim transformationResult As CryptoTransformationResult = Me.System_GetUserPasswordTransformationResult(loginName)
            Dim transformer As IWMPasswordTransformation = PasswordTransformerFactory.ProduceCryptographicTransformer(transformationResult.Algorithm)
            Dim transformedPassword As String = transformer.TransformString(password, transformationResult.Noncevalue)

            'Create connection
            MyCmd.Connection = New SqlConnection(ConnectionString)
            Try
                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "Public_PreValidateUser"
                    .CommandType = CommandType.StoredProcedure

                    .Parameters.Add("@Username", SqlDbType.NVarChar).Value = loginName
                    If password Is Nothing Then
                        'Login without passwords - for single-sign-on scenarios
                        .Parameters.Add("@Passcode", SqlDbType.VarChar).Value = DBNull.Value
                    Else
                        'Login with username and password - the standard case
                        .Parameters.Add("@Passcode", SqlDbType.VarChar).Value = transformedPassword
                    End If
                    .Parameters.Add("@ServerIP", SqlDbType.NVarChar).Value = CurrentServerIdentString
                    .Parameters.Add("@RemoteIP", SqlDbType.NVarChar).Value = CurrentRemoteClientAddress
                    If HttpContext.Current Is Nothing Then
                        .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = ScriptEngines.NetClient
                    Else
                        .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = ScriptEngines.ASPNet
                    End If
                    .Parameters.Add("@ScriptEngine_SessionID", SqlDbType.NVarChar, 512).Value = CurrentScriptEngineSessionID
                End With

                'Create recordset by executing the command
                Dim ResultDT As DataTable
                ResultDT = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "Result")
                If ResultDT.Rows.Count <> 1 Then
                    Result = ReturnValues_UserValidation.UnknownError
                Else
                    Result = CType(Utils.Nz(ResultDT.Rows(0).Item(0), ReturnValues_UserValidation.UnknownError), WMSystem.ReturnValues_UserValidation)
                    Dim IsCurrentLoggedOn As Boolean = False
                    If ResultDT.Columns.Count >= 2 Then
                        '2nd column is for IsCurrentLoggedOn
                        IsCurrentLoggedOn = Utils.Nz(ResultDT.Rows(0).Item(1), False)
                    End If
                    If ignoreCurrentlyLoggedOnState = False AndAlso IsCurrentLoggedOn Then
                        Result = ReturnValues_UserValidation.AlreadyLoggedIn
                    End If
                End If

            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
            End Try

            'return
            Return Result

        End Function
        ''' <summary>
        ''' Validate a user name and its password
        ''' </summary>
        ''' <param name="loginName">The username</param>
        ''' <param name="password">The password to check</param>
        ''' <returns>True if successful else False if not matching</returns>
        <Obsolete("Not used yet", True)> Private Function ValidateLoginCredentials(ByVal loginName As String, ByVal password As String) As Boolean

            If loginName = "" Or password = "" Then
                Return False
            End If

            Dim MyDBConn As New SqlConnection(ConnectionString)
            Dim MyCmd As New SqlCommand("", MyDBConn)

            Dim transformationResult As CryptoTransformationResult = Me.System_GetUserPasswordTransformationResult(loginName)
            Dim transformer As IWMPasswordTransformation = PasswordTransformerFactory.ProduceCryptographicTransformer(transformationResult.Algorithm)
            Dim transformedPassword As String = transformer.TransformString(password, transformationResult.Noncevalue)

            'Get parameter value and append parameter
            With MyCmd
                .CommandText = "SELECT LoginName FROM Benutzer WHERE LoginName = @Username AND LoginPW = @Passcode COLLATE Latin1_General_CS_AS"
                .CommandType = CommandType.Text
                .Parameters.Add("@Username", SqlDbType.NVarChar).Value = loginName
                .Parameters.Add("@Passcode", SqlDbType.VarChar).Value = transformedPassword
            End With

            'Create recordset by executing the command
            MyCmd.Connection = MyDBConn
            Dim Result As Object = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If Result Is Nothing OrElse Result Is DBNull.Value Then
                'No record found --> search values without match --> no matching user
                Return False
            Else
                'User has been found with this password --> success!
                Return True
            End If

        End Function
        ''' <summary>
        '''     Login with login name and password
        ''' </summary>
        ''' <param name="loginName">The login name of a user</param>
        ''' <param name="forceLogin">Force the login even if the user has already logged on at another work station</param>
        ''' <returns>A result value telling some details on successfull or unsuccessfull login</returns>
        Friend Function ExecuteLogin(ByVal loginName As String, ByVal forceLogin As Boolean) As ReturnValues_UserValidation
            Return _ExecuteLogin(loginName, Nothing, forceLogin)
        End Function
        ''' <summary>
        '''     Logout and perform some clean ups
        ''' </summary>
        ''' <remarks>
        '''     Web applications require to login/logout to multiple servers, that's why there will be initiated some redirections 
        ''' </remarks>
        Public Sub Logout()
            If HttpContext.Current Is Nothing Then
                ExecuteLogout()
            Else
                'Web logout has to use the logout page
                Dim LogoutUrl As String = Me.Internationalization.User_Auth_Validation_LogonScriptURL
                If InStr(LogoutUrl, "?") > 0 Then
                    LogoutUrl &= "&Action=Logout"
                Else
                    LogoutUrl &= "?Action=Logout"
                End If
                Me.RedirectTo(LogoutUrl)
            End If
        End Sub
        ''' <summary>
        '''     Logout and perform some clean-ups
        ''' </summary>
        Public Sub ExecuteLogout()
            _ExecuteLogout(True)
        End Sub
        ''' <summary>
        '''     Logout and perform some clean-ups
        ''' </summary>
        Private Sub _ExecuteLogout(ByVal publishLogoutStatusToAllRelatedServers As Boolean)

            'Profile zurücksetzen
            Dim strServerIP As String, strRemoteIP As String
            strServerIP = Me.CurrentServerIdentString
            strRemoteIP = Me.CurrentRemoteClientAddress

            If CurrentUserLoginName <> Nothing Then
                Dim MyDBConn As New SqlConnection
                Dim MyRecSet As SqlDataReader = Nothing
                Dim MyCmd As New SqlCommand

                'Create connection
                MyDBConn.ConnectionString = Me.ConnectionString
                Try
                    MyDBConn.Open()

                    'Get parameter value and append parameter
                    With MyCmd

                        .CommandText = "Public_Logout"
                        .CommandType = CommandType.StoredProcedure

                        .Parameters.Add("@Username", SqlDbType.NVarChar).Value = Me.CurrentUserLoginName
                        .Parameters.Add("@ServerIP", SqlDbType.VarChar).Value = CStr(strServerIP)
                        .Parameters.Add("@RemoteIP", SqlDbType.VarChar).Value = CStr(strRemoteIP)

                        'Since DB Build 111, there is an additional parameter
                        If Not HttpContext.Current Is Nothing AndAlso Utils.TryCInt(HttpContext.Current.Application("WebManager.CurrentDBBuildAtLeast")) >= 111 Then
                            .Parameters.Add("@ScriptEngine_SessionID", SqlDbType.NVarChar).Value = CurrentScriptEngineSessionID
                            .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = ScriptEngines.ASPNet
                        Else
                            If Setup.DatabaseUtils.Version(Me, True).Build >= 111 Then
                                .Parameters.Add("@ScriptEngine_SessionID", SqlDbType.NVarChar).Value = CurrentScriptEngineSessionID
                                .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = ScriptEngines.NetClient
                                If Not HttpContext.Current Is Nothing Then
                                    HttpContext.Current.Application("WebManager.CurrentDBBuildAtLeast") = Setup.DatabaseUtils.Version(Me, True).Build
                                End If
                            End If
                        End If

                    End With

                    'Create recordset by executing the command
                    MyCmd.Connection = MyDBConn
                    MyRecSet = MyCmd.ExecuteReader()

                    'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                    Dim WorkaroundEx As New Exception("")
                    Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                    Try
                        WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                    Catch
                    End Try
                    If MyRecSet.Read Then
                        Select Case Utils.Nz(MyRecSet("Result"), 0)
                            Case -3 'Incomplete parameters
                                Me.Log.RuntimeWarning("Logout: incomplete parameters", WorkaroundStackTrace, DebugLevels.NoDebug, False, False)
                            Case -4 'Server not found
                                Me.Log.RuntimeWarning("Logout: server not found where to logout", WorkaroundStackTrace, DebugLevels.NoDebug, False, False)
                            Case -9 'User not found
                                Me.Log.RuntimeWarning("Logout: user account """ & Me.CurrentUserLoginName & """ with an active webmanager session hasn't been found to be logged out", WorkaroundStackTrace, DebugLevels.NoDebug, False, False)
                            Case -1 'Logout successfull
                                'do nothing
                            Case Else 'unknown result value
                                Me.Log.RuntimeWarning("Logout: unknown result value", WorkaroundStackTrace, DebugLevels.NoDebug, False, False)
                        End Select
                    Else
                        'no result value
                        Me.Log.RuntimeWarning("Logout: no result value", WorkaroundStackTrace, DebugLevels.NoDebug, False, False)
                    End If

                Catch ex As Exception
                    Throw
                    If System_DebugLevel >= DebugLevels.Medium_LoggingOfDebugInformation Then
                        Throw
                    Else
                        Me.Log.RuntimeWarning(ex)
                    End If
                Finally
                    If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then
                        MyRecSet.Close()
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
            End If

            'Remove old files from download handler
            Try
                If Me.DownloadHandler.IsWebApplication Then
                    Me.DownloadHandler.CleanUp()
                End If
            Catch ex As Exception
                If Me.DebugLevel >= DebugLevels.Low_WarningMessagesOnAccessError Then
                    Me.Log.RuntimeException(ex, False, False, DebugLevels.Low_WarningMessagesOnAccessError)
                    Me.Log.ReportErrorByEMail(ex, Nothing)
                End If
            End Try

            'Session-Objekte zurücksetzen
            Me.ResetUserLoginName()

            'Publish the logout status to all related servers
            If publishLogoutStatusToAllRelatedServers AndAlso Not HttpContext.Current Is Nothing Then
                Dim NextLogonUri As String = System_GetNextLogonURIOfUserAnonymous()
                If NextLogonUri = "" Then
                    RedirectTo(Internationalization.User_Auth_Validation_NoRefererURL & "?ref=" & System.Web.HttpUtility.UrlEncode(Internationalization.User_Auth_Validation_AfterLogoutURL), Nothing, Nothing)
                Else
                    RedirectTo(System_GetNextLogonURIOfUserAnonymous() & "&redirectto=" & System.Web.HttpUtility.UrlEncode(Internationalization.User_Auth_Validation_NoRefererURL & "?ref=" & System.Web.HttpUtility.UrlEncode(Internationalization.User_Auth_Validation_AfterLogoutURL)) & "&lang=" & CType(Session("CurLanguage"), String) & "&User=", Nothing, Nothing)
                End If
            End If

        End Sub
        ''' <summary>
        '''     Reset the complete authorization cache
        ''' </summary>
        Public Sub System_ResetAuthorizationStatusCache()
            HttpContext.Current.Session.Remove("CWM_Security_Cache_ExpiresOn")
            HttpContext.Current.Session.Remove("CWM_Security_Cache_Data")
        End Sub

        Private Sub _SaveAuthorizationInCache(ByRef UserID As Long, ByVal SecurityObjectName As String, ByVal ServerName As String, ByVal AuthorizationStatus_Success As Boolean)
            'TODO: input parameter validation: all parameters must be not nothing
            Dim CreateNewCacheObjects As Boolean = False
            Dim MyDataSet As New DataSet("root")
            Dim MyDataTable As DataTable = Nothing
            Dim MyDataView As DataView = Nothing
            Dim MyExpirationDateTime As DateTime

            If HttpContext.Current Is Nothing Then
                'There is no cache, simply exit
                Exit Sub
            End If

            'Get the current cache data
            Try
                If HttpContext.Current.Session("CWM_Security_Cache_ExpiresOn") Is Nothing Then
                    CreateNewCacheObjects = True
                    Exit Try
                Else
                    MyExpirationDateTime = CType(HttpContext.Current.Session("CWM_Security_Cache_ExpiresOn"), DateTime)
                    If MyExpirationDateTime.Subtract(Now).TotalSeconds < 0 Then
                        CreateNewCacheObjects = True
                        Exit Try
                    End If
                    Dim xmlStringReader As System.IO.StringReader
                    xmlStringReader = New System.IO.StringReader(CType(HttpContext.Current.Session("CWM_Security_Cache_Schema"), String))
                    MyDataSet.ReadXmlSchema(xmlStringReader)
                    xmlStringReader = New System.IO.StringReader(CType(HttpContext.Current.Session("CWM_Security_Cache_Data"), String))
                    MyDataSet.ReadXml(xmlStringReader)
                    MyDataTable = MyDataSet.Tables("CWM_Security_CacheData")
                End If
            Catch ex As Exception
                CreateNewCacheObjects = True
            End Try

            If CreateNewCacheObjects = True Then
                'Write new cache objects
                MyExpirationDateTime = Now.AddSeconds(System_SecurityQueryCache_MaxAgeInSeconds)
                MyDataTable = New DataTable("CWM_Security_CacheData")
                MyDataTable.Columns.Add("UserID", GetType(Integer)).AllowDBNull = True
                MyDataTable.Columns.Add("SecurityObjectName", GetType(String)).AllowDBNull = False
                MyDataTable.Columns.Add("Server", GetType(String)).AllowDBNull = False
                MyDataTable.Columns.Add("AuthorizationStatus", GetType(Boolean)).AllowDBNull = False
                MyDataTable.PrimaryKey = New DataColumn() {MyDataTable.Columns("SecurityAttribute")}
                MyDataSet.Tables.Add(MyDataTable)
                Dim WriteToThisRow As DataRow = Nothing
                WriteToThisRow = MyDataTable.NewRow
                If Not WriteToThisRow Is Nothing Then
                    WriteToThisRow("UserID") = UserID
                    WriteToThisRow("SecurityObjectName") = SecurityObjectName
                    WriteToThisRow("Server") = ServerName
                    WriteToThisRow("AuthorizationStatus") = AuthorizationStatus_Success
                    MyDataTable.Rows.Add(WriteToThisRow)
                End If

                MyDataSet.AcceptChanges()
                HttpContext.Current.Session("CWM_Security_Cache_ExpiresOn") = MyExpirationDateTime
                HttpContext.Current.Session("CWM_Security_Cache_Schema") = MyDataSet.GetXmlSchema
                HttpContext.Current.Session("CWM_Security_Cache_Data") = MyDataSet.GetXml
            Else
                'Add/update current cache data
                Dim WriteToThisRow As DataRow = Nothing
                Try
                    Try
                        'Detect if we shall add or update a row
                        MyDataView = New DataView(MyDataTable, "UserID = " & UserID & " AND Server = N'" & ServerName.Replace("'", "''") & "'", "", DataViewRowState.CurrentRows)
                        If MyDataView.Count = 0 Then
                            'no matching record found
                            WriteToThisRow = MyDataTable.NewRow
                            Exit Try
                        Else
                            For MyCounter As Integer = 0 To MyDataView.Count - 1
                                Dim MyRow As DataRowView = MyDataView.Item(MyCounter)
                                If CType(MyRow("SecurityObjectName"), String) = SecurityObjectName Then
                                    If CType(MyRow("AuthorizationStatus"), Boolean) <> AuthorizationStatus_Success Then
                                        MyRow.Delete()
                                        MyDataTable.AcceptChanges()
                                        WriteToThisRow = MyDataTable.NewRow
                                    End If
                                    Exit Try
                                End If
                            Next
                            WriteToThisRow = MyDataTable.NewRow
                        End If
                    Catch ex As Exception
                        WriteToThisRow = Nothing
                    End Try
                    'Write changes if row object has been returned else cancel add/update
                    If Not WriteToThisRow Is Nothing Then
                        WriteToThisRow("UserID") = UserID
                        WriteToThisRow("SecurityObjectName") = SecurityObjectName
                        WriteToThisRow("Server") = ServerName
                        WriteToThisRow("AuthorizationStatus") = AuthorizationStatus_Success
                        MyDataTable.Rows.Add(WriteToThisRow)
                        HttpContext.Current.Session("CWM_Security_Cache_ExpiresOn") = MyExpirationDateTime
                        HttpContext.Current.Session("CWM_Security_Cache_Schema") = MyDataSet.GetXmlSchema
                        HttpContext.Current.Session("CWM_Security_Cache_Data") = MyDataSet.GetXml
                    End If
                Catch ex As Exception
                    'cancel add/update 
                End Try
            End If

            'clean up
            If Not MyDataSet Is Nothing Then MyDataSet.Dispose()
            If Not MyDataTable Is Nothing Then MyDataTable.Dispose()
            If Not MyDataView Is Nothing Then MyDataView.Dispose()

        End Sub

        Private Function _LoadAuthorizationStatusFromCache(ByRef UserID As Long, ByVal SecurityObjectName As String, ByVal ServerName As String) As Boolean
            'TODO: input parameter validation: all parameters must be not nothing
            Dim MyDataSet As New DataSet("root")
            Dim MyDataView As DataView = Nothing
            Dim MyDataTable As DataTable = Nothing
            Dim MyExpirationDateTime As DateTime
            Dim Result As Boolean = Nothing

            If HttpContext.Current Is Nothing Then
                'There is no cache, simply exit
                Exit Function
            End If

            Try
                'Get security cache objects
                If HttpContext.Current.Session("CWM_Security_Cache_ExpiresOn") Is Nothing Then
                    Exit Try
                Else
                    MyExpirationDateTime = CType(HttpContext.Current.Session("CWM_Security_Cache_ExpiresOn"), DateTime)
                    If MyExpirationDateTime.Subtract(Now).TotalSeconds < 0 Then
                        Exit Try
                    End If
                    Dim xmlStringReader As System.IO.StringReader
                    xmlStringReader = New System.IO.StringReader(CType(HttpContext.Current.Session("CWM_Security_Cache_Schema"), String))
                    MyDataSet.ReadXmlSchema(xmlStringReader)
                    xmlStringReader = New System.IO.StringReader(CType(HttpContext.Current.Session("CWM_Security_Cache_Data"), String))
                    MyDataSet.ReadXml(xmlStringReader)
                    MyDataTable = MyDataSet.Tables("CWM_Security_CacheData")
                End If

                'Get needed data row result
                MyDataView = New DataView(MyDataTable, "UserID = " & UserID & " AND Server = N'" & ServerName.Replace("'", "''") & "'", "", DataViewRowState.CurrentRows)
                If MyDataView.Count = 0 Then
                    'no matching record found
                    Exit Try
                Else
                    For MyCounter As Integer = 0 To MyDataView.Count - 1
                        Dim MyRow As DataRowView = MyDataView.Item(MyCounter)
                        If CType(MyRow("SecurityObjectName"), String) = SecurityObjectName Then
                            Result = CType(MyRow("AuthorizationStatus"), Boolean)
                            Exit Try
                        End If
                    Next
                End If
            Catch
                'Conversion errors, etc. lead to non-usage of the cache
                Result = Nothing
            End Try

            'clean up
            If Not MyDataSet Is Nothing Then MyDataSet.Dispose()
            If Not MyDataTable Is Nothing Then MyDataTable.Dispose()
            If Not MyDataView Is Nothing Then MyDataView.Dispose()

            Return Result

        End Function
        ''' <summary>
        '''     Get all navigation items in a datatable (old result style)
        ''' </summary>
        ''' <param name="UserID">The user ID the navigation should be for</param>
        ''' <param name="LanguageID">The needed language of the navigation</param>
        ''' <param name="ServerIdentString">The server identification string for the navigation</param>
        ''' <param name="AutoAddLogonLinks">Automatically add navigation items for login/logout, change profile, etc.</param>
        ''' <param name="AllowCacheRead">Allow to read from a cache</param>
        ''' <param name="AllowCacheWrite">Allow to save the navigation items to a cache</param>
        ''' <returns>A datatable with all navigation items</returns>
        <Obsolete("Use System_GetUserNavigationElements instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_GetNavItems(ByVal UserID As Integer, Optional ByVal LanguageID As Integer = Nothing, Optional ByVal ServerIdentString As String = Nothing, Optional ByVal AutoAddLogonLinks As Boolean = True, Optional ByVal AllowCacheRead As Boolean = True, Optional ByVal AllowCacheWrite As Boolean = True) As DataTable
            Return _System_GetNavItems(CLng(UserID), CType(Nothing, Integer), LanguageID, ServerIdentString, AutoAddLogonLinks, AllowCacheRead, AllowCacheWrite)
        End Function
        ''' <summary>
        '''     Get all navigation items in a datatable (old result style)
        ''' </summary>
        ''' <param name="UserID">The user ID the navigation should be for</param>
        ''' <param name="LanguageID">The needed language of the navigation</param>
        ''' <param name="ServerIdentString">The server identification string for the navigation</param>
        ''' <param name="AutoAddLogonLinks">Automatically add navigation items for login/logout, change profile, etc.</param>
        ''' <param name="AllowCacheRead">Allow to read from a cache</param>
        ''' <param name="AllowCacheWrite">Allow to save the navigation items to a cache</param>
        ''' <returns>A datatable with all navigation items</returns>
        <Obsolete("Use System_GetUserNavigationElements instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_GetNavItems(ByVal UserID As Long, Optional ByVal LanguageID As Integer = Nothing, Optional ByVal ServerIdentString As String = Nothing, Optional ByVal AutoAddLogonLinks As Boolean = True, Optional ByVal AllowCacheRead As Boolean = True, Optional ByVal AllowCacheWrite As Boolean = True) As DataTable
            Return _System_GetNavItems(UserID, CType(Nothing, Integer), LanguageID, ServerIdentString, AutoAddLogonLinks, AllowCacheRead, AllowCacheWrite)
        End Function
        ''' <summary>
        '''     Get all navigation items in a datatable (old result style)
        ''' </summary>
        ''' <param name="UserID">The user ID the navigation should be for</param>
        ''' <param name="LanguageID">The needed language of the navigation</param>
        ''' <param name="ServerIdentString">The server identification string for the navigation</param>
        ''' <param name="AutoAddLogonLinks">Automatically add navigation items for login/logout, change profile, etc.</param>
        ''' <param name="AllowCacheRead">Allow to read from a cache</param>
        ''' <param name="AllowCacheWrite">Allow to save the navigation items to a cache</param>
        ''' <returns>A datatable with all navigation items</returns>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Private Function _System_GetNavItems(ByVal userID As Long, ByVal groupID As Integer, Optional ByVal LanguageID As Integer = Nothing, Optional ByVal ServerIdentString As String = Nothing, Optional ByVal AutoAddLogonLinks As Boolean = True, Optional ByVal AllowCacheRead As Boolean = True, Optional ByVal AllowCacheWrite As Boolean = True) As DataTable

            If Not (userID = Nothing Xor groupID = Nothing) Then
                Throw New ArgumentException("Either userID or groupID must be filled")
            End If

            Dim MyDatatable As New DataTable("navitems")

            'Data completion
            If ServerIdentString = "" Then
                ServerIdentString = CurrentServerIdentString
            End If
            If LanguageID = Nothing Then
                LanguageID = UI.MarketID()
            End If

            'ToDo: resolve bug which leads to invalid cache data in cookieless scenarios (e. g. sometimes there are urls like http://localhost/(session1st)/(session2nd)/home.aspx or that cache seems to not getting removed on the correct time
            If Configuration.CookieLess = True Then
                AllowCacheRead = False
                AllowCacheWrite = False
            End If

            'Caching
            Dim CacheObjectName As String
            If Not HttpContext.Current Is Nothing Then
                If Configuration.CookieLess = False Then
                    If userID <> Nothing Then
                        CacheObjectName = "System_GetNavItems_" & userID.ToString & "_Reset"
                    Else
                        CacheObjectName = "System_GetGroupNavItems_" & groupID.ToString & "_Reset"
                    End If
                Else
                    If userID <> Nothing Then
                        CacheObjectName = "System_GetNavItems_" & userID.ToString & "_" & Session.SessionID & "_Reset"
                    Else
                        CacheObjectName = "System_GetGroupNavItems_" & groupID.ToString & "_" & Session.SessionID & "_Reset"
                    End If
                End If
                Try
                    If CType(HttpContext.Current.Cache(CacheObjectName), Boolean) = True Then
                        'Remove cache elements
                        Dim Items2Remove As New ArrayList
                        Dim MyCounter As Integer = 1
                        Dim MyCacheElements As System.Collections.IDictionaryEnumerator = HttpContext.Current.Cache.GetEnumerator
                        Dim CompareString As String
                        If Configuration.CookieLess = False Then
                            If userID <> Nothing Then
                                CompareString = "System_GetNavItems_" & userID.ToString & "_"
                            Else
                                CompareString = "System_GetGroupNavItems_" & groupID.ToString & "_"
                            End If
                        Else
                            If userID <> Nothing Then
                                CompareString = "System_GetNavItems_" & userID.ToString & "_"
                            Else
                                CompareString = "System_GetGroupNavItems_" & groupID.ToString & "_" & Session.SessionID & "_"
                            End If
                        End If
                        While MyCacheElements.MoveNext
                            If MyCacheElements.Key.GetType.ToString = "System.String" Then
                                If Mid(CType(MyCacheElements.Key, String), 1, CompareString.Length) = CompareString Then
                                    Items2Remove.Add(MyCacheElements.Key)
                                    System_DebugTraceWrite("Removed cache item=" & CType(MyCacheElements.Key, String), DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails)
                                End If
                            End If
                        End While
                        For Each MyItem As String In Items2Remove
                            HttpContext.Current.Cache.Remove(MyItem)
                        Next
                    End If
                Catch ex As Exception
                    Me.Log.RuntimeWarning("Cache object """ & CacheObjectName & """ is not a valid boolean object", ex.ToString, DebugLevels.NoDebug, False, False)
                End Try
            End If
            If AllowCacheRead AndAlso Not HttpContext.Current Is Nothing Then
                If Configuration.CookieLess = False Then
                    If userID <> Nothing Then
                        CacheObjectName = "System_GetNavItems_" & userID.ToString & "_" & LanguageID.ToString & "_" & ServerIdentString.Replace("_", "-") & "_" & AutoAddLogonLinks.ToString
                    Else
                        CacheObjectName = "System_GetGroupNavItems_" & groupID.ToString & "_" & LanguageID.ToString & "_" & ServerIdentString.Replace("_", "-") & "_" & AutoAddLogonLinks.ToString
                    End If
                Else
                    If userID <> Nothing Then
                        CacheObjectName = "System_GetNavItems_" & userID.ToString & "_" & Session.SessionID & "_" & LanguageID.ToString & "_" & ServerIdentString.Replace("_", "-") & "_" & AutoAddLogonLinks.ToString
                    Else
                        CacheObjectName = "System_GetGroupNavItems_" & groupID.ToString & "_" & Session.SessionID & "_" & LanguageID.ToString & "_" & ServerIdentString.Replace("_", "-") & "_" & AutoAddLogonLinks.ToString
                    End If
                End If
                If Not HttpContext.Current.Cache(CacheObjectName) Is Nothing Then
                    Try
                        MyDatatable = CType(HttpContext.Current.Cache(CacheObjectName), DataTable)
                        Return MyDatatable
                    Catch ex As Exception
                        HttpContext.Current.Cache.Remove(CacheObjectName)
                        Me.Log.RuntimeWarning("Cache object """ & CacheObjectName & """ is not a valid DataTable object", ex.ToString, DebugLevels.NoDebug, False, False)
                    End Try
                End If
            End If

            'Data retrieval
            Dim MyConn As New SqlConnection(ConnectionString)
            Dim MyDataAdapter As Data.SqlClient.SqlDataAdapter = Nothing
            Dim MyCmd As New SqlCommand
            Try
                MyConn.Open()
                If userID <> Nothing Then
                    MyCmd.CommandText = "Public_GetNavPointsOfUser"
                Else
                    MyCmd.CommandText = "Public_GetNavPointsOfGroup"
                End If

                MyCmd.CommandType = CommandType.StoredProcedure
                MyCmd.Connection = MyConn

                If userID <> Nothing Then
                    MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
                Else
                    MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupID
                End If

                MyCmd.Parameters.Add("@ServerIP", SqlDbType.NVarChar).Value = ServerIdentString
                MyCmd.Parameters.Add("@LanguageID", SqlDbType.Int).Value = LanguageID

                If userID <> Nothing Then
                    If userID <> CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous Then
                        MyCmd.Parameters.Add("@AnonymousAccess", SqlDbType.Bit).Value = False
                    Else
                        MyCmd.Parameters.Add("@AnonymousAccess", SqlDbType.Bit).Value = True
                    End If
                Else
                    If _CurrentUserInfo_ID <> CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous Then
                        MyCmd.Parameters.Add("@AnonymousAccess", SqlDbType.Bit).Value = False
                    Else
                        MyCmd.Parameters.Add("@AnonymousAccess", SqlDbType.Bit).Value = True
                    End If
                End If

                'Optionally TODO: when needed, add that to the method
                '==================================================== _
                ', Optional ByVal GetItemsOfAlternativeLanguageToo As Boolean = True
                'If GetItemsOfAlternativeLanguageToo = False AndAlso Me.System_DBVersion_Ex.Build >= 109 Then
                '    MyCmd.Parameters.Add("@SearchForAlternativeLanguages", SqlDbType.Bit).Value = False
                'End If

                'Create recordset by executing the command
                MyDatatable = New DataTable("navitems")
                MyDataAdapter = New System.Data.SqlClient.SqlDataAdapter(MyCmd)
                MyDataAdapter.AcceptChangesDuringFill = True
                MyDataAdapter.Fill(MyDatatable)
            Finally
                If Not MyDataAdapter Is Nothing Then
                    MyDataAdapter.Dispose()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            'Add logon/logoff/update_pw/update_profile/etc.
            If AutoAddLogonLinks = True Then
                Dim NavRow As DataRow
                If userID = SpecialUsers.User_Anonymous Then
                    'Anonymous
                    NavRow = MyDatatable.NewRow()
                    NavRow("NavURL") = "[LOGONPAGE]"
                    NavRow("NavURLAutoCompleted") = NavRow("NavURL")
                    NavRow("Level1Title") = Internationalization.NavAreaNameLogin
                    NavRow("Level2Title") = Internationalization.NavLinkNameLogin
                    NavRow("IsNew") = False
                    NavRow("IsUpdated") = False
                    NavRow("NavTooltipText") = Nothing
                    NavRow("Sort") = 9000000
                    NavRow("Level1TitleIsHTMLCoded") = False
                    NavRow("Level2TitleIsHTMLCoded") = True
                    NavRow("Level3TitleIsHTMLCoded") = False
                    NavRow("Level4TitleIsHTMLCoded") = False
                    NavRow("Level5TitleIsHTMLCoded") = False
                    NavRow("Level6TitleIsHTMLCoded") = False
                    MyDatatable.Rows.Add(NavRow)
                    NavRow = MyDatatable.NewRow()
                    NavRow("NavURL") = "[MASTERSERVER]/sysdata/account_sendpassword.aspx"
                    NavRow("NavURLAutoCompleted") = NavRow("NavURL")
                    NavRow("Level1Title") = Internationalization.NavAreaNameLogin
                    NavRow("Level2Title") = Internationalization.NavLinkNamePasswordRecovery
                    NavRow("IsNew") = False
                    NavRow("IsUpdated") = False
                    NavRow("NavTooltipText") = Nothing
                    NavRow("Sort") = 9000000
                    NavRow("Level1TitleIsHTMLCoded") = False
                    NavRow("Level2TitleIsHTMLCoded") = False
                    NavRow("Level3TitleIsHTMLCoded") = False
                    NavRow("Level4TitleIsHTMLCoded") = False
                    NavRow("Level5TitleIsHTMLCoded") = False
                    NavRow("Level6TitleIsHTMLCoded") = False
                    MyDatatable.Rows.Add(NavRow)
                    NavRow = MyDatatable.NewRow()
                    NavRow("NavURL") = "[MASTERSERVER]/sysdata/account_register.aspx"
                    NavRow("NavURLAutoCompleted") = NavRow("NavURL")
                    NavRow("Level1Title") = Internationalization.NavAreaNameLogin
                    NavRow("Level2Title") = Internationalization.NavLinkNameNewUser
                    NavRow("IsNew") = False
                    NavRow("IsUpdated") = False
                    NavRow("NavTooltipText") = Nothing
                    NavRow("Sort") = 9000000
                    NavRow("Level1TitleIsHTMLCoded") = False
                    NavRow("Level2TitleIsHTMLCoded") = False
                    NavRow("Level3TitleIsHTMLCoded") = False
                    NavRow("Level4TitleIsHTMLCoded") = False
                    NavRow("Level5TitleIsHTMLCoded") = False
                    NavRow("Level6TitleIsHTMLCoded") = False
                    MyDatatable.Rows.Add(NavRow)
                Else
                    'Public
                    NavRow = MyDatatable.NewRow()
                    NavRow("NavURL") = "[MASTERSERVER]/sysdata/account_updateprofile.aspx"
                    NavRow("NavURLAutoCompleted") = NavRow("NavURL")
                    NavRow("Level1Title") = Internationalization.NavAreaNameYourProfile
                    NavRow("Level2Title") = Internationalization.NavLinkNameUpdateProfile
                    NavRow("IsNew") = False
                    NavRow("IsUpdated") = False
                    NavRow("NavTooltipText") = Nothing
                    NavRow("Sort") = 9000000
                    NavRow("Level1TitleIsHTMLCoded") = False
                    NavRow("Level2TitleIsHTMLCoded") = False
                    NavRow("Level3TitleIsHTMLCoded") = False
                    NavRow("Level4TitleIsHTMLCoded") = False
                    NavRow("Level5TitleIsHTMLCoded") = False
                    NavRow("Level6TitleIsHTMLCoded") = False
                    MyDatatable.Rows.Add(NavRow)
                    NavRow = MyDatatable.NewRow()
                    NavRow("NavURL") = "[MASTERSERVER]/sysdata/account_updatepassword.aspx"
                    NavRow("NavURLAutoCompleted") = NavRow("NavURL")
                    NavRow("Level1Title") = Internationalization.NavAreaNameYourProfile
                    NavRow("Level2Title") = Internationalization.NavLinkNameUpdatePasswort
                    NavRow("IsNew") = False
                    NavRow("IsUpdated") = False
                    NavRow("NavTooltipText") = Nothing
                    NavRow("Sort") = 9000000
                    NavRow("Level1TitleIsHTMLCoded") = False
                    NavRow("Level2TitleIsHTMLCoded") = False
                    NavRow("Level3TitleIsHTMLCoded") = False
                    NavRow("Level4TitleIsHTMLCoded") = False
                    NavRow("Level5TitleIsHTMLCoded") = False
                    NavRow("Level6TitleIsHTMLCoded") = False
                    MyDatatable.Rows.Add(NavRow)
                    NavRow = MyDatatable.NewRow()
                    NavRow("NavURL") = "[LOGOFFPAGE]"
                    NavRow("NavURLAutoCompleted") = NavRow("NavURL")
                    NavRow("Level1Title") = Internationalization.NavAreaNameYourProfile
                    NavRow("Level2Title") = Internationalization.NavLinkNameLogout
                    NavRow("IsNew") = False
                    NavRow("IsUpdated") = False
                    NavRow("NavTooltipText") = Nothing
                    NavRow("Sort") = 9000000
                    NavRow("Level1TitleIsHTMLCoded") = False
                    NavRow("Level2TitleIsHTMLCoded") = True
                    NavRow("Level3TitleIsHTMLCoded") = False
                    NavRow("Level4TitleIsHTMLCoded") = False
                    NavRow("Level5TitleIsHTMLCoded") = False
                    NavRow("Level6TitleIsHTMLCoded") = False
                    MyDatatable.Rows.Add(NavRow)
                End If
            End If

            'Auto-complete the navigation URLs
            For MyCounter As Integer = 1 To MyDatatable.Rows.Count
                'ToDo before implementing following code block: stored procedure must be changed to return NavID respectively SecurityObjectID
                'Compatibility level >= 4
                'MyDatatable.Rows(MyCounter - 1)("NavURLAutoCompleted") = System_ModifyNavLink(MyDatatable.Rows(MyCounter - 1)("ID"), MyDatatable.Rows(MyCounter - 1)("NavURLAutoCompleted"), 2)
                'Compatibility level <= 2
                MyDatatable.Rows(MyCounter - 1)("NavURLAutoCompleted") = System_ModifyNavLink(Nothing, MyDatatable.Rows(MyCounter - 1)("NavURLAutoCompleted"), 2)
            Next

            'Remove all lines without a Level1Title
            For MyCounter As Integer = MyDatatable.Rows.Count To 1 Step -1
                If Trim(Utils.Nz(MyDatatable.Rows(MyCounter - 1)("Level1Title"), "")) = "" Then
                    MyDatatable.Rows.RemoveAt(MyCounter - 1)
                End If
            Next

            'Caching
            If AllowCacheWrite AndAlso Not HttpContext.Current Is Nothing Then
                If Configuration.CookieLess = False Then
                    If userID <> Nothing Then
                        CacheObjectName = "System_GetNavItems_" & userID.ToString & "_" & LanguageID.ToString & "_" & ServerIdentString.Replace("_", "-") & "_" & AutoAddLogonLinks.ToString
                    Else
                        CacheObjectName = "System_GetGroupNavItems_" & groupID.ToString & "_" & LanguageID.ToString & "_" & ServerIdentString.Replace("_", "-") & "_" & AutoAddLogonLinks.ToString
                    End If
                Else
                    If userID <> Nothing Then
                        CacheObjectName = "System_GetNavItems_" & userID.ToString & "_" & Session.SessionID & "_" & LanguageID.ToString & "_" & ServerIdentString.Replace("_", "-") & "_" & AutoAddLogonLinks.ToString
                    Else
                        CacheObjectName = "System_GetGroupNavItems_" & groupID.ToString & "_" & Session.SessionID & "_" & LanguageID.ToString & "_" & ServerIdentString.Replace("_", "-") & "_" & AutoAddLogonLinks.ToString
                    End If
                End If
                Try
                    If userID > 0 Then
                        'real users
                        HttpContext.Current.Cache.Add(CacheObjectName, MyDatatable, Nothing, Caching.Cache.NoAbsoluteExpiration, New TimeSpan(0, 5, 0), Caching.CacheItemPriority.High, Nothing)
                    Else
                        'special users (anonymous, etc.)
                        HttpContext.Current.Cache.Add(CacheObjectName, MyDatatable, Nothing, Caching.Cache.NoAbsoluteExpiration, New TimeSpan(1, 0, 0), Caching.CacheItemPriority.NotRemovable, Nothing)
                    End If
                Catch ex As Exception
                    Me.Log.RuntimeWarning("Cache object """ & CacheObjectName & """ can't be set", ex.ToString, DebugLevels.NoDebug, False, False)
                End Try
            End If
            Return MyDatatable

        End Function

        ''' <summary>
        '''     Get all navigation items in a datatable
        ''' </summary>
        ''' <param name="groupID">The group ID the navigation should be for</param>
        ''' <param name="languageID">The needed language of the navigation</param>
        ''' <param name="serverIdentString">The server identification string for the navigation</param>
        ''' <param name="autoAddLogonLinks">Automatically add navigation items for login/logout, change profile, etc.</param>
        ''' <param name="allowCacheRead">Allow to read from a cache</param>
        ''' <param name="allowCacheWrite">Allow to save the navigation items to a cache</param>
        ''' <returns>A datatable with all navigation items</returns>
        Public Function System_GetGroupNavigationElements(ByVal groupID As Long, Optional ByVal languageId As Integer = Nothing, Optional ByVal serverIdentString As String = Nothing, Optional ByVal autoAddLogonLinks As Boolean = True, Optional ByVal allowCacheRead As Boolean = True, Optional ByVal allowCacheWrite As Boolean = True) As DataTable
            If Setup.DatabaseUtils.Version(Me, True).Build < 164 Then
                Throw New NotImplementedException("System_GetGroupNavigationElements is not implemented in build no. " & Setup.DatabaseUtils.Version(Me, True).Build & ". Please upgrade your database to build 164 or higher to use this feature.")
            Else
                Return _System_GetGroupOrUserNavigationElements(groupID, False, languageId, serverIdentString, autoAddLogonLinks, allowCacheRead, allowCacheWrite)
            End If
        End Function

        ''' <summary>
        '''     Get all navigation items in a datatable
        ''' </summary>
        ''' <param name="userID">The user ID the navigation should be for</param>
        ''' <param name="languageID">The needed language of the navigation</param>
        ''' <param name="serverIdentString">The server identification string for the navigation</param>
        ''' <param name="autoAddLogonLinks">Automatically add navigation items for login/logout, change profile, etc.</param>
        ''' <param name="allowCacheRead">Allow to read from a cache</param>
        ''' <param name="allowCacheWrite">Allow to save the navigation items to a cache</param>
        ''' <returns>A datatable with all navigation items</returns>
        Public Function System_GetUserNavigationElements(ByVal userID As Long, Optional ByVal languageId As Integer = Nothing, Optional ByVal serverIdentString As String = Nothing, Optional ByVal autoAddLogonLinks As Boolean = True, Optional ByVal allowCacheRead As Boolean = True, Optional ByVal allowCacheWrite As Boolean = True) As DataTable
            Dim _DBVersion As Version = Setup.DatabaseUtils.Version(Me, True)
            If _DBVersion.CompareTo(MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects) >= 0 Then 'Newer
                Throw New NotImplementedException("System_GetUserNavigationElements")
            Else
                Return _System_GetGroupOrUserNavigationElements(userID, True, languageId, serverIdentString, autoAddLogonLinks, allowCacheRead, allowCacheWrite)
            End If
        End Function
        ''' <summary>
        '''     Get all navigation items in a datatable
        ''' </summary>
        ''' <param name="ID">The user or group ID the navigation should be for</param>
        ''' <param name="IsUser">Get navigation items for a user or a group</param>
        ''' <param name="languageID">The needed language of the navigation</param>
        ''' <param name="serverIdentString">The server identification string for the navigation</param>
        ''' <param name="autoAddLogonLinks">Automatically add navigation items for login/logout, change profile, etc.</param>
        ''' <param name="allowCacheRead">Allow to read from a cache</param>
        ''' <param name="allowCacheWrite">Allow to save the navigation items to a cache</param>
        ''' <returns>A datatable with all navigation items</returns>
        Private Function _System_GetGroupOrUserNavigationElements(ByVal ID As Long, ByVal IsUser As Boolean, Optional ByVal languageID As Integer = Nothing, Optional ByVal serverIdentString As String = Nothing, Optional ByVal autoAddLogonLinks As Boolean = True, Optional ByVal allowCacheRead As Boolean = True, Optional ByVal allowCacheWrite As Boolean = True) As DataTable
            Dim MyNavItemsOldStyle As DataTable
            If IsUser Then
                MyNavItemsOldStyle = _System_GetNavItems(ID, CType(Nothing, Integer), languageID, serverIdentString, autoAddLogonLinks, allowCacheRead, allowCacheWrite)
            Else
                MyNavItemsOldStyle = _System_GetNavItems(CType(Nothing, Long), CType(ID, Integer), languageID, serverIdentString, autoAddLogonLinks, allowCacheRead, allowCacheWrite)
            End If
            Dim MyNavItems As DataTable = Navigation.CreateEmptyDataTable
            Dim MyNewRow As DataRow

            For MyRowCounter As Integer = 0 To MyNavItemsOldStyle.Rows.Count - 1
                If Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level1Title"), "") <> "" Then
                    MyNewRow = MyNavItems.NewRow

                    Dim Title As String
                    Dim IsHtmlEncoded As Boolean
                    Title = Navigation.ValidNavigationPath(Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level1Title"), ""))
                    IsHtmlEncoded = Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level1TitleIsHTMLCoded"), False)
                    If Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level2Title"), "") <> "" Then
                        Title &= "\" & Navigation.ValidNavigationPath(Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level2Title"), ""))
                        IsHtmlEncoded = Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level2TitleIsHTMLCoded"), False)
                    End If
                    If Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level3Title"), "") <> "" Then
                        Title &= "\" & Navigation.ValidNavigationPath(Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level3Title"), ""))
                        IsHtmlEncoded = Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level3TitleIsHTMLCoded"), False)
                    End If
                    If Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level4Title"), "") <> "" Then
                        Title &= "\" & Navigation.ValidNavigationPath(Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level4Title"), ""))
                        IsHtmlEncoded = Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level4TitleIsHTMLCoded"), False)
                    End If
                    If Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level5Title"), "") <> "" Then
                        Title &= "\" & Navigation.ValidNavigationPath(Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level5Title"), ""))
                        IsHtmlEncoded = Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level5TitleIsHTMLCoded"), False)
                    End If
                    If Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level6Title"), "") <> "" Then
                        Title &= "\" & Navigation.ValidNavigationPath(Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level6Title"), ""))
                        IsHtmlEncoded = Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Level6TitleIsHTMLCoded"), False)
                    End If
                    MyNewRow("ID") = Nothing
                    MyNewRow("Title") = Title
                    MyNewRow("Sort") = Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("Sort"), 1000000)
                    MyNewRow("Tooltip") = Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("NavTooltipText"))
                    MyNewRow("IsNew") = Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("IsNew"), False)
                    MyNewRow("IsUpdated") = Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("IsUpdated"), False)
                    MyNewRow("URLPreDefinition") = Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("NavURL"))
                    MyNewRow("URLAutoCompleted") = Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("NavURLAutoCompleted"))
                    MyNewRow("Target") = Utils.Nz(MyNavItemsOldStyle.Rows(MyRowCounter)("NavFrame"))
                    MyNewRow("IsHtmlEncoded") = IsHtmlEncoded
                    MyNewRow("IsDisabledInStandardSituations") = MyNavItemsOldStyle.Rows(MyRowCounter)("AppDisabled")

                    MyNavItems.Rows.Add(MyNewRow)
                End If
            Next
            Return MyNavItems
        End Function
        ''' <summary>
        '''     Modify a navigation link
        ''' </summary>
        ''' <param name="NavID">The ID of the navigation item</param>
        ''' <param name="TableValue">The current address of the link</param>
        ''' <param name="CompatibilityLevel">The compatiblity level for this method, &lt;=2 for old data style, &gt;=4 for new data style since MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects</param>
        ''' <returns>A string with the correct value for the link address</returns>
        ''' <remarks>
        '''     Following substrings get replaced: "[MASTERSERVER]", "[MASTERURL]", "[LOGONPAGE]", "[LOGOFFPAGE]", "[ADMINURL]", "[SESSIONID:????]", "[SAP:????]", "[NAVID]"
        ''' </remarks>
        Public Function System_ModifyNavLink(ByVal NavID As Integer, ByVal TableValue As Object, ByVal CompatibilityLevel As Integer) As String
            Dim Buffer As String = Trim(Utils.Nz(TableValue, ""))
            If Internationalization.User_Auth_Config_UserAuthMasterServer = Nothing Then Throw New Exception("Missing value for MASTERSERVER")
            Buffer = Buffer.Replace("[MASTERSERVER]", Internationalization.User_Auth_Config_UserAuthMasterServer)
            If Internationalization.OfficialServerGroup_URL = Nothing Then Throw New Exception("Missing value for MASTERURL")
            Buffer = Buffer.Replace("[MASTERURL]", Internationalization.OfficialServerGroup_URL)
            If Internationalization.User_Auth_Validation_LogonScriptURL = Nothing Then Throw New Exception("Missing value for LOGONPAGE")
            Buffer = Buffer.Replace("[LOGONPAGE]", Internationalization.User_Auth_Validation_LogonScriptURL)
            If Internationalization.User_Auth_Validation_LogonScriptURL = Nothing Then Throw New Exception("Missing value for LOGOFFPAGE")
            Buffer = Buffer.Replace("[LOGOFFPAGE]", Internationalization.User_Auth_Validation_LogonScriptURL & "?Action=Logout")
            If Internationalization.OfficialServerGroup_AdminURL = Nothing Then Throw New Exception("Missing value for ADMINURL")
            Buffer = Buffer.Replace("[ADMINURL]", Internationalization.OfficialServerGroup_AdminURL)
            Buffer = Buffer.Replace("[NAVID]", NavID.ToString)
            Buffer = Buffer.Replace("[SECURITYOBJECTID]", NavID.ToString)
            If Not HttpContext.Current Is Nothing Then
                Buffer = Buffer.Replace("[SESSIONID]", Me.CurrentScriptEngineSessionID) 'not recommended since this is dependent on the current server, but the target machine might be another server
            End If

            If InStr(Buffer, "[SESSIONID:") > 0 Then
                Try
                    Dim StartPos As Integer = InStr(Buffer, "[SESSIONID:")
                    Dim SecondStartPos As Integer = StartPos + 11
                    Dim EndPos As Integer = InStr(SecondStartPos, Buffer, "]")
                    Dim DelimiterPos As Integer = InStr(SecondStartPos, Buffer, "|")
                    If DelimiterPos > EndPos Or DelimiterPos <= 0 Or DelimiterPos < StartPos Then
                        DelimiterPos = EndPos
                    End If
                    Dim SecondParam As String
                    Dim ScriptEngineID As Integer 'ScriptEngine
                    Dim MyServerID As Integer
                    If Mid(Buffer, SecondStartPos, DelimiterPos - SecondStartPos).ToString <> "" Then
                        ScriptEngineID = CType(Mid(Buffer, SecondStartPos, DelimiterPos - SecondStartPos), Integer)
                    Else
                        ScriptEngineID = ScriptEngines.ASPNet
                    End If
                    SecondParam = Mid(Buffer, DelimiterPos + 1, System.Math.Max(EndPos - DelimiterPos - 1, 0))
                    If SecondParam <> "" Then
                        If CType(SecondParam, Integer) = 0 Then
                            'Request ServerID by NavID
                            MyServerID = System_GetServerIDOfNavID(NavID, CompatibilityLevel)
                        End If
                        'Force ServerID by parameter value
                        MyServerID = CType(SecondParam, Integer)
                    Else
                        'Request ServerID by NavID
                        MyServerID = System_GetServerIDOfNavID(NavID, CompatibilityLevel)
                    End If
                    Dim ScriptEngineSessionIDValue As String = LookupScriptEngineSessionID(MyServerID, ScriptEngineID)
                    Buffer = Buffer.Replace(Mid(Buffer, StartPos, EndPos - StartPos + 1), ScriptEngineSessionIDValue)
                Catch ex As Exception
                    If Me.System_DebugLevel >= DebugLevels.Medium_LoggingOfDebugInformation Then
                        Buffer = """ onClick=""alert('Navigation URL malformed. Please contact our webmaster: " & (vbNewLine & "NavID=" & NavID & "&TableValue=" & CType(TableValue, String) & "&CompatibilityLevel=" & CompatibilityLevel & vbNewLine & ex.Message & vbNewLine & ex.StackTrace).Replace("\", "\\").Replace("'", "\'").Replace(ControlChars.CrLf, "\r\n").Replace(ControlChars.Cr, "\r\n").Replace(ControlChars.Lf, "\r\n") & "'); return (false);"" dummy=""" 'close href, create onclick and dummy (optional query strings shouldn't be attached to the href or onclick, that would lead to JS errors)
                    Else
                        If System_DebugLevel >= DebugLevels.Medium_LoggingOfDebugInformation Then
                            Buffer = Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?ErrCode=" & System.Web.HttpUtility.UrlEncode("Navigation URL malformed. " & ex.Message)
                        Else
                            Buffer = Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?ErrCode=" & System.Web.HttpUtility.UrlEncode("Navigation URL malformed.")
                        End If
                    End If
                End Try
            End If

            If InStr(Buffer, "[SAP:") = 1 Then
                Try
                    Buffer = Internationalization.User_Auth_Config_Paths_Login & "sapredirect.aspx?ai=" & CLng(Mid(Buffer, InStr(Buffer, "|5|AI") + 5, InStr(Buffer, "|6|") - InStr(Buffer, "|5|AI") - 5))
                Catch ex As Exception
                    If Me.System_DebugLevel >= DebugLevels.Medium_LoggingOfDebugInformation Then
                        Buffer = """ onClick=""alert('Navigation URL malformed. Please contact our webmaster: " & (vbNewLine & "NavID=" & NavID & "&TableValue=" & CType(TableValue, String) & "&CompatibilityLevel=" & CompatibilityLevel & vbNewLine & ex.Message & vbNewLine & ex.StackTrace).Replace("\", "\\").Replace("'", "\'").Replace(ControlChars.CrLf, "\r\n").Replace(ControlChars.Cr, "\r\n").Replace(ControlChars.Lf, "\r\n") & "'); return (false);"" dummy=""" 'close href, create onclick and dummy (optional query strings shouldn't be attached to the href or onclick, that would lead to JS errors)
                    Else
                        If System_DebugLevel >= DebugLevels.Medium_LoggingOfDebugInformation Then
                            Buffer = Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?ErrCode=" & System.Web.HttpUtility.UrlEncode("Navigation URL malformed. (" & ex.Message & ")")
                        Else
                            Buffer = Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?ErrCode=" & System.Web.HttpUtility.UrlEncode("Navigation URL malformed.")
                        End If
                    End If
                End Try
            End If

            If Not _URLReplacements Is Nothing Then
                For Each Replacement As String In _URLReplacements
                    Buffer = Buffer.Replace(Replacement, _URLReplacements(Replacement))
                Next
            End If

            Return Buffer

        End Function

        ''' <summary>
        '''     Lookup the script engine session ID of the current user on a server/script engine
        ''' </summary>
        ''' <param name="serverID">The server that is part of the same server group</param>
        ''' <param name="scriptEngineID">An ID of a script engine which runs on that server</param>
        ''' <returns>The session ID of the script engine on the specified server</returns>
        ''' <remarks>
        ''' Nothing will be returned e.g. WebService context is always without session data, so every lookup for a session must fail
        ''' </remarks>
        ''' <return>The session ID for the specified server and scriptEngine or Nothing (null) in case that the current session can't be used to lookup the CWM session details</return>
        Private Function LookupScriptEngineSessionID(ByVal serverID As Integer, ByVal scriptEngineID As Integer) As String
            If HttpContext.Current Is Nothing Then
                'typical environment for non-web environments
                'CurrentScriptEngineSessionID will be created on-the-fly - other servers are never involved into this type of session
                If scriptEngineID = ScriptEngines.NetClient AndAlso serverID = Me.CurrentServerInfo.ID Then
                    Return Me.CurrentScriptEngineSessionID
                Else
                    Return Nothing
                End If
            ElseIf Not HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing Then
                'typical environment for being run in a webservice environment
                Return Nothing
            Else
                'typical environment for web page requests
                Static SessionData As New Hashtable 'for caching within the current request
                Dim ScriptEngineSessionIDValue As String
                If IsNothing(SessionData(serverID)) Then
                    'Add a new hashtable into the hashtable 
                    SessionData.Add(serverID, New Hashtable)
                End If
                Dim ServerSessionData As Hashtable = CType(SessionData(serverID), Hashtable)
                If ServerSessionData(scriptEngineID) Is Nothing Then
                    Dim MySessionID As String
                    If Not Me.IsLoggedOn Then
                        'Build a new valid Session ID for the anonymous user session (logic in CurrentScriptEngineSessionID)
                        MySessionID = CurrentScriptEngineSessionID 'TODO: this seems to be WRONG, but no other quick workaround for the scenario with a 2nd script engine (e.g. classic ASP) available as long as there hasn't been a full login at all servers/script engines yet
                    Else 'If Me.IsLoggedOn
                        MySessionID = LookupBrowserSessionIDForAnotherServerOrScriptEngineTarget(Me.CurrentUserLoginName, serverID, scriptEngineID, Me.CurrentServerInfo.ID, ScriptEngines.ASPNet, Me.CurrentScriptEngineSessionID)
                    End If
                    If MySessionID <> Nothing Then
                        ServerSessionData.Add(scriptEngineID, MySessionID)
                    Else
                        Dim WorkaroundStackTrace As String
                        Try
                            'following statement may fail if not running in full trust mode - 
                            WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                        Catch
                            'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                            Dim WorkaroundEx As New Exception("")
                            WorkaroundStackTrace = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                        End Try
                        Me.Log.RuntimeWarning("Session ID not availabe for server " & serverID & " and script engine " & scriptEngineID, WorkaroundStackTrace, DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails, False, False)
                    End If
                End If
                ScriptEngineSessionIDValue = CType(ServerSessionData(scriptEngineID), String)
                Return ScriptEngineSessionIDValue
            End If
        End Function

        ''' <summary>
        '''     Retrieve the session ID of the current script engine on the current server from the camm Web-Manager session
        ''' </summary>
        ''' <param name="userName">The login name of a user</param>
        ''' <param name="requiredServerID">The desired server for which to lookup the browser session ID</param>
        ''' <param name="requiredScriptEngineID">The desired script engine for which to lookup the browser session ID</param>
        ''' <param name="currentServerID">The current server with a valid/registered session</param>
        ''' <param name="currentScriptEngineID">The current script engine with a valid/registered session</param>
        ''' <param name="currentScriptEngineSessionID">The browser session ID for the current session</param>
        ''' <returns>The browser session ID on the requested target or Nothing (null) if session hasn't been found</returns>
        Private Function LookupBrowserSessionIDForAnotherServerOrScriptEngineTarget(ByVal userName As String, ByVal requiredServerID As Integer, requiredScriptEngineID As Integer, currentServerID As Integer, currentScriptEngineID As ScriptEngines, currentScriptEngineSessionID As String) As String
            Const Sql As String = "    DECLARE @CurrentSessionID int" & vbNewLine & _
                "    SELECT @CurrentSessionID = SessionID" & vbNewLine & _
                "          from benutzer " & vbNewLine & _
                "            inner join System_UserSessions on benutzer.id = System_UserSessions.id_user" & vbNewLine & _
                "            inner join System_WebAreasAuthorizedForSession on System_WebAreasAuthorizedForSession.SessionID = System_UserSessions.ID_Session" & vbNewLine & _
                "		WHERE benutzer.loginname = @Username" & vbNewLine & _
                "            and ScriptEngine_SessionID = @CurrentScriptEngineSessionID " & vbNewLine & _
                "			AND ScriptEngine_ID = @CurrentScriptEngineID" & vbNewLine & _
                "			AND Server = @CurrentServerID" & vbNewLine & _
                "    If @CurrentSessionID IS NOT NULL" & vbNewLine & _
                "		-- gewünschten Session ID Wert ausliefern" & vbNewLine & _
                "        select System_WebAreasAuthorizedForSession.ScriptEngine_SessionID" & vbNewLine & _
                "  		FROM System_WebAreasAuthorizedForSession " & vbNewLine & _
                "          where System_WebAreasAuthorizedForSession.Server = @RequiredServerID" & vbNewLine & _
                "            and System_WebAreasAuthorizedForSession.ScriptEngine_ID = @RequiredScriptEngineID" & vbNewLine & _
                "            and System_WebAreasAuthorizedForSession.SessionID = @CurrentSessionID"
            Dim MyCmd As New SqlCommand(Sql, New SqlConnection(ConnectionString))
            With MyCmd
                .CommandText = Sql
                .CommandType = CommandType.Text

                .Parameters.Add("@CurrentServerID", SqlDbType.Int).Value = currentServerID
                .Parameters.Add("@CurrentScriptEngineID", SqlDbType.Int).Value = currentScriptEngineID
                .Parameters.Add("@CurrentScriptEngineSessionID", SqlDbType.NVarChar).Value = currentScriptEngineSessionID
                .Parameters.Add("@Username", SqlDbType.NVarChar).Value = userName
                .Parameters.Add("@RequiredServerID", SqlDbType.Int).Value = requiredServerID
                .Parameters.Add("@RequiredScriptEngineID", SqlDbType.Int).Value = requiredScriptEngineID
            End With
            Return Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), "")
        End Function
        ''' <summary>
        '''     Get the server ID from a navigation item ID
        ''' </summary>
        ''' <param name="NavID">A navigation item ID</param>
        ''' <param name="CompatibilityLevel">The compatiblity level for this method, &lt;=2 for old data style, &gt;=4 for new data style since MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects</param>
        ''' <returns>A server ID</returns>
        Private Function System_GetServerIDOfNavID(ByVal NavID As Integer, ByVal CompatibilityLevel As Integer) As Integer
            Dim User_Auth_Validation_DBConn As New SqlConnection
            Dim User_Auth_Validation_RecSet As SqlDataReader = Nothing
            Dim User_Auth_Validation_Cmd As New SqlCommand
            Dim Result As Integer

            'Create connection
            User_Auth_Validation_DBConn.ConnectionString = ConnectionString
            Try
                User_Auth_Validation_DBConn.Open()

                'Get parameter value and append parameter
                With User_Auth_Validation_Cmd
                    Select Case CompatibilityLevel
                        Case 1, 2
                            Dim _DBVersion As Version = Setup.DatabaseUtils.Version(Me, True)
                            If _DBVersion.CompareTo(MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects) >= 0 Then 'Newer
                                .CommandText = "SELECT LocationID FROM Applications_NavItems WHERE ID = @NavID"
                            Else
                                .CommandText = "SELECT LocationID FROM [dbo].[Applications_CurrentAndInactiveOnes] WHERE ID = @NavID"
                            End If
                        Case Else '>=4
                            .CommandText = "SELECT ServerID FROM Applications_NavItems_HirarchyID WHERE ID = @NavID"
                    End Select
                    .CommandType = CommandType.Text

                    .Parameters.Add("@NavID", SqlDbType.Int).Value = NavID
                End With

                'Create recordset by executing the command
                User_Auth_Validation_Cmd.Connection = User_Auth_Validation_DBConn
                User_Auth_Validation_RecSet = User_Auth_Validation_Cmd.ExecuteReader()

                User_Auth_Validation_RecSet.Read()
                Result = CType(User_Auth_Validation_RecSet(0), Integer)

            Finally
                If Not User_Auth_Validation_RecSet Is Nothing AndAlso Not User_Auth_Validation_RecSet.IsClosed Then
                    User_Auth_Validation_RecSet.Close()
                End If
                If Not User_Auth_Validation_Cmd Is Nothing Then
                    User_Auth_Validation_Cmd.Dispose()
                End If
                If Not User_Auth_Validation_DBConn Is Nothing Then
                    If User_Auth_Validation_DBConn.State <> ConnectionState.Closed Then
                        User_Auth_Validation_DBConn.Close()
                    End If
                    User_Auth_Validation_DBConn.Dispose()
                End If
            End Try

            Return Result

        End Function
#End Region

#Region "Mail sending"

        ''' <summary>
        '''     An e-mail attachment
        ''' </summary>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Structure EMailAttachement
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
            Dim AttachmentData As Byte()
            ''' <summary>
            '''     The filename for the binary data
            ''' </summary>
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
            Dim AttachmentData_Filename As String
            ''' <summary>
            '''     A path to a file which shall be included
            ''' </summary>
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
            Dim AttachmentFile As String
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
            Dim AttachmentFile_Charset As String
            ''' <summary>
            '''     A placeholder string in the HTML code of the message which shall be replaced by the CID code of the attachment
            ''' </summary>
            ''' <remarks>
            '''     Define the placeholder which shall be replaced by the Content-ID for the contents of a file to the email. Emails formatted in HTML can include images with this information and internally reference the image through a "cid" hyperlink.
            ''' </remarks>
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
            Dim PlaceholderInMHTML_ToReplaceWithCID As String
            '''' -----------------------------------------------------------------------------
            '''' <summary>
            ''''     Use this Content-ID (CID) in the HTML code of the message to access this attachment
            '''' </summary>
            '''' <remarks>
            ''''     The Content-ID is a string starting with &quot;CID:&quot;, e. g. &quot:CID:MyImage&quot;. Emails formatted in HTML can include images with this information and internally reference the image through a such "CID" value in the Src attribute.
            '''' </remarks>
            '''' <history>
            '''' 	[AdminSupport]	04.05.2005	Created
            '''' </history>
            '''' -----------------------------------------------------------------------------
            'Dim ContentID As String
        End Structure

        '<Obsolete("Use Messaging.EMails.Priority instead"), _
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Enum MailImportance As Byte
            High = 1
            Normal = 3
            Low = 5
        End Enum

        '<Obsolete("Use Messaging.EMails.Sensitivity instead"), _
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Enum MailSensitivity As Byte
            Status_Normal = 1
            Status_Personal = 2
            Status_Private = 3
            Status_CompanyConfidential = 4
        End Enum

        '<Obsolete("Use Messaging.EMails.Sensitivity instead"), _
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Enum MailSendingSystem As Integer
            [Auto] = -1
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> ChilkatActiveX = 0
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> ChilkatNet = 1
            Queue = 2
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> NetFramework1 = -3
            NetFramework = 3
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> EasyMail = 4
        End Enum
        ''' <summary>
        '''     The preferred system for sending e-mails
        ''' </summary>
        ''' <value>The new favorite</value>
        ''' <remarks>
        '''     Please note: if the mail system in unavailable, camm Web-Manager tries to send the e-mail with other systems automatically.
        ''' </remarks>
        <Obsolete("Use MessagingEMails.MailSystem instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Property MailSystem() As MailSendingSystem
            Get
                Return CType(Me.MessagingEMails.MailSystem, MailSendingSystem)
            End Get
            Set(ByVal Value As MailSendingSystem)
                Me.MessagingEMails.MailSystem = CType(Value, Messaging.EMails.MailSendingSystem)
            End Set
        End Property
        ''' <summary>
        '''     The messaging methods for e-mail distribution
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property MessagingEMails() As Messaging.EMails ''<Obsolete("Use CompuMaster.camm.WebManager.Messaging instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
            Get
                Static _MessagingEMails As Messaging.EMails
                If _MessagingEMails Is Nothing Then
                    _MessagingEMails = New Messaging.EMails(Me)
                End If
                Return _MessagingEMails
            End Get
        End Property
        ''' <summary>
        '''     The queue monitoring methods for message distribution
        ''' </summary>
        ''' <value></value>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public ReadOnly Property MessagingQueueMonitoring() As Messaging.QueueMonitoring '<Obsolete("Use Messaging.QueueMonitoring instead"), _
            Get
                Static _MessagingQueueMonitoring As Messaging.QueueMonitoring
                If _MessagingQueueMonitoring Is Nothing Then
                    _MessagingQueueMonitoring = New Messaging.QueueMonitoring(Me)
                End If
                Return _MessagingQueueMonitoring
            End Get
        End Property
        ''' <summary>
        '''     Sends an e-mail
        ''' </summary>
        ''' <param name="RcptName">The name of the receipient</param>
        ''' <param name="RcptAddress">The e-mail address of the receipient</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="MsgBody">The message text</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        <Obsolete("Use MessagingEMails.SendEMail() instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_SendEMail(ByVal RcptName As String, ByVal RcptAddress As String, ByVal MsgSubject As String, ByVal MsgBody As String, ByVal SenderName As String, ByVal SenderAddress As String, Optional ByVal Priority As MailImportance = Nothing) As Boolean
            Return Me.MessagingEMails.SendEMail(RcptName, RcptAddress, MsgSubject, MsgBody, "", SenderName, SenderAddress, CType(Nothing, Messaging.EMailAttachment()), CType(Priority, Messaging.EMails.Priority), Nothing, False, False, Nothing, Nothing, Nothing)
        End Function
        ''' <summary>
        '''     Sends an e-mail
        ''' </summary>
        ''' <param name="RcptName">The name of the receipient</param>
        ''' <param name="RcptAddress">The e-mail address of the receipient</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="MsgTextBody">The message text as plain text</param>
        ''' <param name="MsgHTMLBody">The message text as HTML code</param>
        ''' <param name="MsgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <param name="Attachments">An array of optional attachments</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="RequestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="AdditionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        <Obsolete("Use MessagingEMails.SendEMail() instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_SendEMailEx(ByVal RcptName As String, ByVal RcptAddress As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, Optional ByVal MsgCharset As Object = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal Attachments As EMailAttachement() = Nothing, Optional ByVal Priority As MailImportance = Nothing, Optional ByVal Sensitivity As MailSensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing) As Boolean
            Return MessagingEMails.SendEMail(RcptName, RcptAddress, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, Messaging.EMails.ConvertWMSystemEMailAttachmentToMessagingEMailAttachment(Attachments), CType(Priority, Messaging.EMails.Priority), CType(Sensitivity, Messaging.EMails.Sensitivity), RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders, CType(MsgCharset, String), bufErrorDetails)
        End Function
        ''' <summary>
        '''     Sends an e-mail to multiple receipients
        ''' </summary>
        ''' <param name="RcptAddresses_To">The receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="RcptAddresses_CC">The copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="RcptAddresses_BCC">The blind copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="MsgTextBody">The message text as plain text</param>
        ''' <param name="MsgHTMLBody">The message text as HTML code</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="MsgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <param name="Attachments">An array of optional attachments</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="RequestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="AdditionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        <Obsolete("Use MessagingEMails.SendEMail() instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_SendEMail_MultipleRcpts(ByVal RcptAddresses_To As String, ByVal RcptAddresses_CC As String, ByVal RcptAddresses_BCC As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal Attachments As EMailAttachement() = Nothing, Optional ByVal Priority As MailImportance = Nothing, Optional ByVal Sensitivity As MailSensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing) As Boolean
            Return MessagingEMails.SendEMail(RcptAddresses_To, RcptAddresses_CC, RcptAddresses_BCC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, Messaging.EMails.ConvertWMSystemEMailAttachmentToMessagingEMailAttachment(Attachments), CType(Priority, Messaging.EMails.Priority), CType(Sensitivity, Messaging.EMails.Sensitivity), RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders, MsgCharset, bufErrorDetails)
        End Function

#End Region

#Region "Logs and Statistics"
        ''' <summary>
        '''     Event log of camm Web-Manager
        ''' </summary>
        Public Log As New WMLog(Me)

        ''' <summary>
        '''     Event log methods of camm Web-Manager
        ''' </summary>
        Public Class WMLog
            Inherits CompuMaster.camm.WebManager.Log

            Sub New(ByVal webManager As WMSystem)
                MyBase.New(webManager)
            End Sub

        End Class

        Public Class WMStatistics
            Inherits CompuMaster.camm.WebManager.Statistics

            Sub New(ByVal webManager As WMSystem)
                MyBase.New(webManager)
            End Sub

        End Class
#End Region

#Region "Debugging tools"

        ''' <summary>
        '''     Add a trace item into the current HTTP context
        ''' </summary>
        ''' <param name="message">The trace message to identify the location or the intended purpose</param>
        ''' <param name="RequiredDebugLevel">The required debug level of camm Web-Manager</param>
        Friend Sub System_DebugTraceWrite(ByVal message As String, Optional ByVal RequiredDebugLevel As DebugLevels = DebugLevels.Medium_LoggingOfDebugInformation)
            If System_DebugLevel >= RequiredDebugLevel Then
                If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Trace Is Nothing Then
                    HttpContext.Current.Trace.Write("camm WebManager", message)
                End If
            End If
        End Sub
        ''' <summary>
        '''     Add a trace warning item into the current HTTP context
        ''' </summary>
        ''' <param name="message">The trace message to identify the location or the intended purpose</param>
        ''' <param name="RequiredDebugLevel">The required debug level of camm Web-Manager</param>
        Friend Sub System_DebugTraceWarn(ByVal message As String, Optional ByVal RequiredDebugLevel As DebugLevels = DebugLevels.NoDebug)
            If System_DebugLevel >= RequiredDebugLevel Then
                If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Trace Is Nothing Then
                    HttpContext.Current.Trace.Warn("camm WebManager", message)
                End If
                If Me.TechnicalServiceEMailAccountAddress <> "" AndAlso Me.SMTPServerName <> "" Then
                    Try
                        Me.MessagingEMails.SendEMail(Me.TechnicalServiceEMailAccountName, Me.TechnicalServiceEMailAccountAddress, "camm WebManager Warning", message & vbNewLine & vbNewLine & "Reported on " & Now.ToUniversalTime.ToString("yyyy-MM-dd HH:mm:ss") & " UTC at " & Me.CurrentServerIdentString, Nothing, Me.StandardEMailAccountName, Me.StandardEMailAccountAddress, CType(Nothing, Messaging.EMailAttachment()), Messaging.EMails.Priority.High)
                    Catch
                    End Try
                End If
            End If
        End Sub

        ''' <summary>
        '''     Checks the connectivity and minimal configuration of a server
        ''' </summary>
        ''' <returns>An HTML string containing detailed information on the status of the current server</returns>
        Public Function System_DebugServerConnectivity() As String
            Dim ErrLog As String = Nothing
            Dim ApplicationName As String
            Dim ApplicationSubTitle4LogOnly As Object
            Dim strServerIP As String
            Dim strRemoteIP As String
            Dim User_Auth_Validation_DBConn As New SqlConnection
            Dim User_Auth_Validation_RecSet As SqlDataReader = Nothing
            Dim User_Auth_Validation_Cmd As New SqlCommand
            Dim strWebURL As String

            ApplicationName = "Public"
            ApplicationSubTitle4LogOnly = DBNull.Value

            If HttpContext.Current.Request.QueryString.Count = 0 Then
                strWebURL = HttpContext.Current.Request.ServerVariables("SCRIPT_NAME")
            Else
                strWebURL = HttpContext.Current.Request.ServerVariables("SCRIPT_NAME") & "?" & Utils.QueryStringWithoutSpecifiedParameters(Nothing)
            End If

            strServerIP = CurrentServerIdentString
            strRemoteIP = CurrentRemoteClientAddress

            If IsDBNull(ApplicationSubTitle4LogOnly) Then ApplicationSubTitle4LogOnly = ""

            Try
                'Create connection
                User_Auth_Validation_DBConn.ConnectionString = ConnectionString
                User_Auth_Validation_DBConn.Open()

                'Get parameter value and append parameter
                With User_Auth_Validation_Cmd
                    .CommandText = "Public_ServerDebug"
                    .CommandType = CommandType.StoredProcedure

                    .Parameters.Add("@ServerIP", SqlDbType.NVarChar).Value = CStr(strServerIP)
                    .Parameters.Add("@RemoteIP", SqlDbType.NVarChar).Value = CStr(strRemoteIP)
                End With

                'Create recordset by executing the command
                User_Auth_Validation_Cmd.Connection = User_Auth_Validation_DBConn
                User_Auth_Validation_RecSet = User_Auth_Validation_Cmd.ExecuteReader()

                If Not User_Auth_Validation_RecSet.Read Then
                    ErrLog = ErrLog & "<p>Error description:<br>No validation records found."
                ElseIf IsDBNull(User_Auth_Validation_RecSet(0)) = True Then
                    ErrLog = ErrLog & "<p>Error description:<br>Validation record exists but is empty."
                ElseIf CType(User_Auth_Validation_RecSet(0), Integer) = -10 Then
                    Dim RedirectionCause As String = "This has been because the validation result requires it."
                    Dim RequestDetails As New Collections.Specialized.NameValueCollection
                    RequestDetails("ServerIP") = Me.CurrentServerIdentString
                    RedirectToErrorPage(System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorWrongNetwork, RedirectionCause, RequestDetails)
                ElseIf CType(User_Auth_Validation_RecSet(0), Integer) = -9 Then
                    Dim RedirectionCause As String = "This has been because the validation result requires it."
                    Dim RequestDetails As New Collections.Specialized.NameValueCollection
                    RequestDetails("ServerIP") = Me.CurrentServerIdentString
                    RedirectToErrorPage(System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorWrongNetwork, RedirectionCause, RequestDetails)
                ElseIf CType(User_Auth_Validation_RecSet(0), Integer) = -5 Then
                    ErrLog = ErrLog & "<p>Error description:<br>No authorisation for current application."
                ElseIf CType(User_Auth_Validation_RecSet(0), Integer) = -4 Then
                    ErrLog = ErrLog & "<p>Error description:<br>Already logged on."
                ElseIf CType(User_Auth_Validation_RecSet(0), Integer) = -3 Then
                    ErrLog = ErrLog & "<p>Error description:<br>Access denied."
                ElseIf CType(User_Auth_Validation_RecSet(0), Integer) = -2 Then
                    ErrLog = ErrLog & "<p>Error description:<br>Login temporarily locked."
                ElseIf CType(User_Auth_Validation_RecSet(0), Integer) = -1 OrElse CType(User_Auth_Validation_RecSet(0), Integer) = 58 Then
                    ErrLog = ErrLog & "<p>Database connection successfull!<br>"
                    'Access granted! - Extraline because of unknown but existing values which otherwise result in problems
                ElseIf CType(User_Auth_Validation_RecSet(0), Integer) = 0 Then
                    ErrLog = ErrLog & "<p>Error description:<br>Wrong username or password."
                Else
                    ErrLog = ErrLog & "<p>Error description:<br>Unknown error (" & CType(User_Auth_Validation_RecSet(0), Object).ToString & ")."
                End If
                If User_Auth_Validation_RecSet.FieldCount >= 2 Then
                    If Not LCase(Me.CurrentUserLoginName) = LCase(CType(User_Auth_Validation_RecSet(1), String)) Then
                        ErrLog = ErrLog & "<p>Error description:<br>Given username by form and username from database don't match."
                    End If
                End If
            Catch ex As Exception
                ErrLog = ErrLog & "<p>Error description:<br>" & Err.Description
            Finally
                If Not User_Auth_Validation_RecSet Is Nothing AndAlso Not User_Auth_Validation_RecSet.IsClosed Then
                    User_Auth_Validation_RecSet.Close()
                End If
                If Not User_Auth_Validation_Cmd Is Nothing Then
                    User_Auth_Validation_Cmd.Dispose()
                End If
                If Not User_Auth_Validation_DBConn Is Nothing Then
                    If User_Auth_Validation_DBConn.State <> ConnectionState.Closed Then
                        User_Auth_Validation_DBConn.Close()
                    End If
                    User_Auth_Validation_DBConn.Dispose()
                End If
            End Try

            System_DebugServerConnectivity = ErrLog

        End Function

        ''' <summary>
        '''     Get the complete query string of the current request in a form usable for recreating this query string for a following request
        ''' </summary>
        ''' <param name="removeParameters">Remove all values with this name form the query string</param>
        ''' <returns>A new string with all query string information without the starting questionmark character</returns>
        <Obsolete("Use Utils.QueryStringWithoutSpecifiedParameters instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetRequestQueryStringComplete(Optional ByVal removeParameters As String() = Nothing) As String
            Return Utils.QueryStringWithoutSpecifiedParameters(removeParameters)
        End Function

        ''' <summary>
        '''     Get the ID of the public group
        ''' </summary>
        ''' <returns>A group ID for the public user group</returns>
        <Obsolete("Use CurrentServerInfo object instead"), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetPublicGroupIDOfCurServer() As Integer
            Return CType(Me.System_GetServerConfig(Me.CurrentServerIdentString, "ID_Group_Public"), Integer)
        End Function

        ''' <summary>
        '''     Get the ID of the anonymous group
        ''' </summary>
        ''' <returns>A group ID for the anonymous user group</returns>
        <Obsolete("Use CurrentServerInfo object instead"), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetAnonymousGroupIDOfCurServer() As Integer
            Return CType(Me.System_GetServerConfig(Me.CurrentServerIdentString, "ID_Group_Anonymous"), Integer)
        End Function
#End Region

#Region "Version information"

        ''' <summary>
        '''     Get the version information from the current camm Web-Manager library (cammWM.dll)
        ''' </summary>
        ''' <returns>The version of the executing camm Web-Manager library</returns>
        Public Function System_Version_Ex() As Version Implements IWebManager.VersionAssembly
            Return Setup.ApplicationUtils.Version
        End Function

        ''' <summary>
        '''     Get the version information from the current camm Web-Manager library (cammWM.dll)
        ''' </summary>
        ''' <returns>A string with the major and minor version of the executing camm Web-Manager library</returns>
        Public Function System_Version() As String
            Dim MyVersion As Version = System_Version_Ex()
            Return MyVersion.Major & "." & MyVersion.Minor.ToString("00")
        End Function

        ''' <summary>
        '''     Get the version information from the current camm Web-Manager library (cammWM.dll)
        ''' </summary>
        ''' <returns>A string with the build version of the executing camm Web-Manager library</returns>
        Public Function System_Build() As String
            Dim MyVersion As Version = System_Version_Ex()
            Return MyVersion.Build.ToString
        End Function

        ''' <summary>
        '''     Get the version information from the current camm Web-Manager database
        ''' </summary>
        ''' <returns>The version of the connected camm Web-Manager database</returns>
        Public Function System_DBVersion_Ex() As Version Implements IWebManager.VersionDatabase
            Return Setup.DatabaseUtils.Version(Me, False)
        End Function

        ''' <summary>
        '''     Get the version information from the current camm Web-Manager database
        ''' </summary>
        ''' <param name="allowCaching">True to allow reading from cache</param>
        ''' <returns>The version of the connected camm Web-Manager database</returns>
        Public Function System_DBVersion_Ex(allowCaching As Boolean) As Version Implements IWebManager.VersionDatabase
            Return Setup.DatabaseUtils.Version(Me, allowCaching)
        End Function
#End Region

#Region "Extended WebManager methods"

        ''' <summary>
        '''     camm Web-Manager initialization states
        ''' </summary>
        Friend Enum InitializationStates
            ''' <summary>
            '''     camm Web-Manager instance can't provide any services; the database connection is not available
            ''' </summary>
            None = 0
            ''' <summary>
            '''     Database access is available, basic logging is available
            ''' </summary>
            DatabaseAccessAvailable = 10
            ''' <summary>
            '''     All features are available, but all settings are with manufacturer's default
            ''' </summary>
            ''' <remarks>
            '''     Loading and saving of user profiles, server environment, etc. is available
            ''' </remarks>
            ServerCommunicationAvailable = 20
        End Enum
        ''' <summary>
        '''     Is the camm Web-Manager initialization complete minimally to allow server communication?
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     There
        ''' </remarks>
        Friend ReadOnly Property InitializationState() As InitializationStates
            Get
                If Me.ConnectionString <> Nothing AndAlso Me.CurrentServerIdentString <> Nothing Then
                    Return InitializationStates.ServerCommunicationAvailable
                ElseIf Me.ConnectionString <> Nothing Then
                    Return InitializationStates.DatabaseAccessAvailable
                Else
                    Return InitializationStates.None
                End If
            End Get
        End Property

        ''' <summary>
        '''     Has the HttpApplication been initialized by a camm Web-Manager HttpApplication?
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     This property will be used typically to detect if the automatic cleanup procedures in HttpApplication can work
        ''' </remarks>
        Friend ReadOnly Property IsInitializedInHttpApplication() As Boolean
            Get
                Return CType(HttpContext.Current.Application("WebManager.Application.InitiatedByCwmHttpApplication"), Boolean)
                'Alternatively could be used (allows removal of related init part in HttpApplication init method):
                'Return GetType(CompuMaster.camm.WebManager.Application.HttpApplication).IsInstanceOfType(HttpContext.Current.ApplicationInstance)
            End Get
        End Property

#If NotYetImplemented Then
        Public Function System_GetAvailableServersForSpecifiedUserForSpecifiedSecurityObject(ByVal UserID As long, ByVal SecurityObjectName As String()) As ServerInformation()

        End Function
#End If

#End Region

#Region "in-memory-mirror of WebManager objects"

        ''' <summary>
        '''     Get all data of the available languages
        ''' </summary>
        ''' <param name="alsoFindInactiveLanguages">Find inactive languages, too</param>
        ''' <returns>An array of language information</returns>
        Public Function System_GetLanguagesInfo(Optional ByVal alsoFindInactiveLanguages As Boolean = False) As LanguageInformation()
            Static cachedResult As LanguageInformation()
            If cachedResult Is Nothing Then
                cachedResult = System_GetLanguagesInfo(Nothing, alsoFindInactiveLanguages)
            End If
            Return cachedResult
        End Function

        ''' <summary>
        '''     Load some language information objects
        ''' </summary>
        ''' <param name="marketIDs">An array of some market/language IDs</param>
        ''' <param name="alsoFindInactiveLanguages">Find inactive languages, too</param>
        ''' <returns>An array of language information</returns>
        Public Function System_GetLanguagesInfo(ByVal marketIDs As Integer(), ByVal alsoFindInactiveLanguages As Boolean) As LanguageInformation()
            Return System_GetLanguagesInfo(marketIDs, alsoFindInactiveLanguages, True)
        End Function

        ''' <summary>
        '''     Load some language information objects
        ''' </summary>
        ''' <param name="marketIDs">An array of some market/language IDs</param>
        ''' <param name="alsoFindInactiveLanguages">Find inactive languages, too</param>
        ''' <param name="allowReadCache">Find inactive languages, too</param>
        ''' <returns>An array of language information</returns>
        <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetLanguagesInfo(ByVal marketIDs As Integer(), ByVal alsoFindInactiveLanguages As Boolean, ByVal allowReadCache As Boolean) As LanguageInformation()
            Dim cacheItemKey As String
            Dim cacheItemSubKeyMarketIDs As String
            If marketIDs Is Nothing OrElse marketIDs.Length = 0 Then
                cacheItemSubKeyMarketIDs = "All"
            Else
                cacheItemSubKeyMarketIDs = Utils.JoinArrayToString(marketIDs, "_")
            End If
            If alsoFindInactiveLanguages = False Then
                cacheItemKey = "WebManager.Languages.AllActives.Markets_" & cacheItemSubKeyMarketIDs
            Else
                cacheItemKey = "WebManager.Languages.ActivesAndInactives.Markets_" & cacheItemSubKeyMarketIDs
            End If
            'Try to use a cached value
            If allowReadCache AndAlso Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Cache(cacheItemKey) Is Nothing Then
                Return CType(HttpContext.Current.Cache(cacheItemKey), LanguageInformation())
            End If
            'Load languages list from database
            Dim _WebManager As WMSystem = Me
            Dim _Languages As LanguageInformation()
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand
            If alsoFindInactiveLanguages = True Then
                'return all languages
                MyCmd = New SqlCommand("select * from languages", MyConn)
            Else
                'only search for activated languages
                MyCmd = New SqlCommand("select * from languages Where [isactive] = @IsActive", MyConn)
                MyCmd.Parameters.Add("@IsActive", SqlDbType.Bit, 4).Value = True
            End If
            If Not marketIDs Is Nothing Then
                'Filter for selected IDs
                If marketIDs.Length = 0 Then
                    Return New LanguageInformation() {}
                End If
                Dim Filter As New System.Text.StringBuilder
                For MyCounter As Integer = 0 To marketIDs.Length - 1
                    If MyCounter > 0 Then Filter.Append(","c)
                    Filter.Append(marketIDs(MyCounter))
                Next
                MyCmd.CommandText &= " AND ID IN (" & Filter.ToString & ")"
            End If
            MyCmd.CommandText &= " ORDER BY IsActive DESC, case when id = 10000 then 0 else 1 end, Description ASC"
            Dim MyDataSet As DataSet = New DataSet
            Dim MyDA As SqlDataAdapter = New SqlDataAdapter(MyCmd)
            Try
                MyConn.Open()
                MyDA.Fill(MyDataSet, "Languages")
            Finally
                If Not MyDA Is Nothing Then
                    MyDA.Dispose()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try
            If MyDataSet.Tables("Languages").Rows.Count > 0 Then
                ReDim Preserve _Languages(MyDataSet.Tables("Languages").Rows.Count - 1)
                Dim MyCounter As Integer = 0
                If MyDataSet.Tables("Languages").Columns.Contains("DirectionOfLetters") Then
                    For Each MyDataRow As DataRow In MyDataSet.Tables("Languages").Rows
                        _Languages(MyCounter) = New LanguageInformation(CType(MyDataRow("ID"), Integer), _
                        Utils.Nz(MyDataRow("Description"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("Description_OwnLang"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("IsActive"), False), _
                        Utils.Nz(MyDataRow("BrowserLanguageID"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("Abbreviation"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("DirectionOfLetters"), CType(Nothing, String)), Me)
                        MyCounter += 1
                    Next
                Else
                    'The additional column exist beginning with db build 171
                    For Each MyDataRow As DataRow In MyDataSet.Tables("Languages").Rows
                        _Languages(MyCounter) = New LanguageInformation(CType(MyDataRow("ID"), Integer), _
                        Utils.Nz(MyDataRow("Description"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("Description_OwnLang"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("IsActive"), False), _
                        Utils.Nz(MyDataRow("BrowserLanguageID"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("Abbreviation"), CType(Nothing, String)), _
                        "ltr", Me)
                        MyCounter += 1
                    Next
                End If
            Else
                _Languages = Nothing
            End If
            'Cache table
            If Not HttpContext.Current Is Nothing Then
                Utils.SetHttpCacheValue(cacheItemKey, _Languages, Caching.CacheItemPriority.NotRemovable)
            End If
            Return _Languages
        End Function

        ''' <summary>
        '''     Get all server group information
        ''' </summary>
        ''' <returns>An array of server group information</returns>
        Public Function System_GetServerGroupsInfo() As ServerGroupInformation()
            Return System_GetServerGroupsInfo(-1)
        End Function

        ''' <summary>
        '''     Get all server group information
        ''' </summary>
        ''' <param name="AccessLevelID">Only retrieve servergroups available for this access level ID; use -1 if you need information on all available server groups</param>
        ''' <returns>An array of server group information</returns>
        Public Function System_GetServerGroupsInfo(ByVal AccessLevelID As Integer) As ServerGroupInformation()
            'TODO in next major release: convert result type to non-array-value
            Dim _WebManager As WMSystem = Me
            Dim _ServerGroups As ServerGroupInformation()
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand

            If AccessLevelID < 0 Then
                'return all server groups
                MyCmd = New SqlCommand("select system_servergroups.* from system_servergroups", MyConn)
            Else
                'only search for server groups allowed for the current user
                MyCmd = New SqlCommand("select system_servergroups.* from system_servergroups inner join [System_ServerGroupsAndTheirUserAccessLevels] on system_servergroups.id = [System_ServerGroupsAndTheirUserAccessLevels].[ID_ServerGroup] Where [ID_AccessLevel] = @ID", MyConn)
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = AccessLevelID
            End If
            Dim MyDataSet As DataSet = New DataSet
            Dim MyDA As SqlDataAdapter = New SqlDataAdapter(MyCmd)
            Try
                MyConn.Open()
                MyDA.Fill(MyDataSet, "ServerGroups")
            Finally
                If Not MyDA Is Nothing Then
                    MyDA.Dispose()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try
            If MyDataSet.Tables("ServerGroups").Rows.Count > 0 Then
                'JIT-add required column if missing
                If MyDataSet.Tables("ServerGroups").Columns.Contains("AllowImpersonation") = False Then
                    MyDataSet.Tables("ServerGroups").Columns.Add("AllowImpersonation", GetType(Boolean))
                End If
                ReDim Preserve _ServerGroups(MyDataSet.Tables("ServerGroups").Rows.Count - 1)
                Dim MyCounter As Integer = 0
                For Each MyDataRow As DataRow In MyDataSet.Tables("ServerGroups").Rows
                    _ServerGroups(MyCounter) = New ServerGroupInformation(CType(MyDataRow("ID"), Integer), _
                        Utils.Nz(MyDataRow("ServerGroup"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("AreaNavTitle"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("AreaCompanyWebSiteTitle"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("AreaCompanyWebSiteURL"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("AreaCompanyWebSiteURL"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("AreaCompanyWebSiteURL"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("AccessLevel_Default"), 0), _
                        Utils.Nz(MyDataRow("MasterServer"), 0), _
                        Utils.Nz(MyDataRow("UserAdminServer"), 0), _
                        Utils.Nz(MyDataRow("ID_Group_Anonymous"), 0), _
                        Utils.Nz(MyDataRow("ID_Group_Public"), 0), Utils.Nz(MyDataRow("AreaSecurityContactEMail"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("AreaSecurityContactTitle"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("AreaDevelopmentContactTitle"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("AreaDevelopmentContactEMail"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("AreaContentManagementContactTitle"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("AreaContentManagementContactEMail"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("AreaUnspecifiedContactTitle"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("AreaUnspecifiedContactEMail"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("AllowImpersonation"), False), _
                        CType(MyDataRow("AreaCopyRightSinceYear"), Integer), _
                        Utils.Nz(MyDataRow("AreaImage"), ""), _
                        Utils.Nz(MyDataRow("AreaButton"), ""), _
                                                                          _WebManager)
                    MyCounter += 1
                Next
            Else
                _ServerGroups = Nothing
            End If
            Return _ServerGroups
        End Function

        ''' <summary>
        '''     Get all access level information 
        ''' </summary>
        ''' <returns>An array of access level information</returns>
        Friend Function System_GetAccessLevelInfos(ByVal serverGroupID As Integer) As AccessLevelInformation()
            Dim MyConn As New SqlConnection(ConnectionString)
            Dim MyCmd As New SqlCommand("select system_accesslevels.* from system_accesslevels inner join System_ServerGroupsAndTheirUserAccessLevels on system_accesslevels.id = System_ServerGroupsAndTheirUserAccessLevels.id_accesslevel where System_ServerGroupsAndTheirUserAccessLevels.id_servergroup = @ID ORDER BY Title", MyConn)
            MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = serverGroupID
            Dim MyAccessLevels As DataTable
            MyAccessLevels = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "AccessLevels")

            Dim Result As New ArrayList
            If MyAccessLevels.Rows.Count > 0 Then
                ReDim Preserve _AllAccessLevelInfos(MyAccessLevels.Rows.Count - 1)
                Dim MyCounter As Integer = 0
                For Each MyDataRow As DataRow In MyAccessLevels.Rows
                    Result.Add(New AccessLevelInformation(CType(MyDataRow("ID"), Integer), Me))
                Next
                Return CType(Result.ToArray(GetType(AccessLevelInformation)), AccessLevelInformation())
            Else
                Return New AccessLevelInformation() {}
            End If
        End Function

        Private _AllAccessLevelInfos As AccessLevelInformation()

        ''' <summary>
        '''     Get all access level information 
        ''' </summary>
        ''' <returns>An array of access level information</returns>
        Public Function System_GetAccessLevelInfos() As AccessLevelInformation()
            If _AllAccessLevelInfos Is Nothing Then
                Dim MyConn As New SqlConnection(ConnectionString)
                Dim MyCmd As New SqlCommand("select * from system_accesslevels ORDER BY Title", MyConn)
                Dim MyAccessLevels As DataTable
                MyAccessLevels = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "AccessLevels")

                If MyAccessLevels.Rows.Count > 0 Then
                    ReDim Preserve _AllAccessLevelInfos(MyAccessLevels.Rows.Count - 1)
                    Dim MyCounter As Integer = 0
                    For Each MyDataRow As DataRow In MyAccessLevels.Rows
                        _AllAccessLevelInfos(MyCounter) = New AccessLevelInformation(CType(MyDataRow("ID"), Integer), Me)
                        MyCounter += 1
                    Next
                Else
                    _AllAccessLevelInfos = New AccessLevelInformation() {}
                End If
            End If
            Return _AllAccessLevelInfos
        End Function

        ''' <summary>
        '''     Get all server information
        ''' </summary>
        ''' <param name="ServerGroupID">Only retrieve servers of a special server group ID</param>
        ''' <returns>An array of server information</returns>
        ''' <remarks>
        '''     No caching of any data
        ''' </remarks>
        Public Function System_GetServersInfo(Optional ByVal ServerGroupID As Integer = Nothing) As ServerInformation()
            Dim _WebManager As WMSystem = Me
            Dim _Servers As ServerInformation()
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand
            If ServerGroupID = Nothing Then
                'return all servers
                MyCmd = New SqlCommand("select * from system_servers", MyConn)
            Else
                'only search for server groups allowed for the current user
                MyCmd = New SqlCommand("select * from system_servers where servergroup = @ID", MyConn)
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = ServerGroupID
            End If
            Dim ServerData As DataTable = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "Servers")
            If ServerData.Rows.Count > 0 Then
                ReDim Preserve _Servers(ServerData.Rows.Count - 1)
                Dim MyCounter As Integer = 0
                For Each MyDataRow As DataRow In ServerData.Rows
                    _Servers(MyCounter) = New ServerInformation(CType(MyDataRow("ID"), Integer), _
                        CType(MyDataRow("IP"), String), _
                        Utils.Nz(MyDataRow("ServerDescription"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("ServerProtocol"), CType(Nothing, String)), _
                        Utils.Nz(MyDataRow("ServerName"), CType(MyDataRow("IP"), String)), _
                        Utils.Nz(MyDataRow("ServerPort"), CType(Nothing, String)), _
                        CType(MyDataRow("Enabled"), Boolean), _
                        Utils.Nz(MyDataRow("ServerGroup"), 0), _
                        _WebManager)
                    MyCounter += 1
                Next
            Else
                _Servers = Nothing
            End If
            Return _Servers

        End Function

        ''' <summary>
        '''     Get the current user information
        ''' </summary>
        ''' <returns>A user information</returns>
        ''' <remarks>
        '''     Throws an exception if there is no user logged on 
        ''' </remarks>
        Public Function System_GetCurUserInfo() As UserInformation
            Dim UserID As Long = CurrentUserID(SpecialUsers.User_Anonymous)
            If UserID = SpecialUsers.User_Anonymous Then
                Throw New Exception("User not logged in")
            Else
                Dim Result As UserInformation()
                Result = System_GetUserInfos(New Long() {UserID})
                If Not Result Is Nothing AndAlso Result.Length = 1 Then
                    Return Result(0)
                Else
                    Throw New Exception("User not logged in")
                End If
            End If
        End Function

        ''' <summary>
        '''     Create a new user group
        ''' </summary>
        ''' <param name="groupName">The new group name</param>
        ''' <param name="changedByUserID">Who shall be tracked as modifier?</param>
        ''' <param name="groupDescription"></param>
        Public Sub System_CreateGroup(ByVal groupName As String, ByVal changedByUserID As Long, Optional ByVal groupDescription As String = Nothing)
            Me.System_CreateNewGroup(groupName, changedByUserID, groupDescription)
        End Sub

        Friend Function System_CreateNewGroup(ByVal groupName As String, ByVal changedByUserID As Long, Optional ByVal groupDescription As String = Nothing) As Integer

            If groupName = Nothing Then
                Throw New ArgumentNullException("groupName")
            End If

            ' Open command object with one parameter.
            Dim MyCmd As New System.Data.SqlClient.SqlCommand
            MyCmd.CommandText = "AdminPrivate_CreateGroup"
            MyCmd.CommandType = CommandType.StoredProcedure
            MyCmd.Connection = New System.Data.SqlClient.SqlConnection(Me.ConnectionString)

            ' Get parameter value and append parameter.
            MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = changedByUserID
            MyCmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = groupName
            MyCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Utils.StringNotEmptyOrDBNull(groupDescription)

            ' Create recordset by executing the command.
            Dim RetValue As Integer = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), 0)
            If RetValue = 0 Then
                Throw New Exception("Group creation failed")
            Else
                Return RetValue
            End If

        End Function

        ''' <summary>
        '''     Save changes of the group information (without memberships)
        ''' </summary>
        ''' <param name="groupInfo">The group information object which shall be saved</param>
        ''' <param name="changedByUserID">Who shall be tracked as modifier?</param>
        Public Sub System_UpdateGroup(ByVal groupInfo As GroupInformation, ByVal changedByUserID As Long)

            ' Open command object with one parameter.
            Dim MyCmd As New System.Data.SqlClient.SqlCommand
            MyCmd.CommandType = CommandType.Text
            MyCmd.CommandText = "UPDATE dbo.Gruppen SET Name = @Name, Description = @Description, ModifiedOn = GetDate(), ModifiedBy = @ChangedByUserID WHERE ID=@ID"
            MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = groupInfo.ID
            MyCmd.Parameters.Add("@ChangedByUserID", SqlDbType.Int).Value = changedByUserID
            MyCmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = groupInfo.Name
            MyCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Utils.StringNotEmptyOrDBNull(groupInfo.Description)
            MyCmd.Parameters.Add("@IsSystemGroup", SqlDbType.Bit).Value = groupInfo.IsSystemGroup

            ' Create recordset by executing the command.
            MyCmd.Connection = New System.Data.SqlClient.SqlConnection(Me.ConnectionString)

            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

        End Sub

        ''' <summary>
        '''     Drop the group with the given ID
        ''' </summary>
        ''' <param name="groupID">The ID of a group</param>
        Public Sub System_RemoveGroup(ByVal groupID As Integer)
            'TODO: Use GroupInformation object instead + verify for SecOperator-Auths

            'Delete desired group
            Dim MyCmd As New System.Data.SqlClient.SqlCommand
            MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupID
            MyCmd.CommandType = CommandType.Text
            MyCmd.CommandText = "BEGIN TRANSACTION" & vbNewLine & _
                "DELETE FROM dbo.Gruppen WHERE ID=@GroupID" & vbNewLine & _
                "DELETE FROM dbo.Memberships WHERE ID_Group=@GroupID" & vbNewLine & _
                "DELETE FROM dbo.ApplicationsRightsByGroup WHERE ID_GroupOrPerson=@GroupID" & vbNewLine & _
                "COMMIT"
            MyCmd.Connection = New System.Data.SqlClient.SqlConnection(Me.ConnectionString)
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            MyCmd.Dispose()

        End Sub

        ''' <summary>
        '''     Save changes to a user information object 
        ''' </summary>
        ''' <param name="UserInfo">The user information object</param>
        ''' <param name="NewPassword">A new password</param>
        ''' <param name="Notifications">The notifications for sending appropriate information to the user</param>
        ''' <returns>The ID of that user profile that has been saved</returns>
        ''' <remarks>
        ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
        ''' </remarks>
        <Obsolete("Use userInfo.Save instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_SetUserInfo(ByRef UserInfo As UserInformation, Optional ByRef NewPassword As String = Nothing, Optional ByVal Notifications As WMNotifications = Nothing) As Integer
            Return System_SetUserInfo(UserInfo, NewPassword, Notifications, False)
        End Function

        ''' <summary>
        '''     Save changes to a user information object 
        ''' </summary>
        ''' <param name="userInfo">The user information object</param>
        ''' <param name="newPassword">A new password</param>
        ''' <param name="notifications">The notifications for sending appropriate information to the user</param>
        ''' <param name="suppressNotifications">False sends e-mails regulary, true disables all user notifications</param>
        ''' <returns>The ID of that user profile that has been saved</returns>
        ''' <remarks>
        ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
        ''' </remarks>
        <Obsolete("Use userInfo.Save instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_SetUserInfo(ByRef userInfo As UserInformation, ByRef newPassword As String, ByVal notifications As WMNotifications, ByVal suppressNotifications As Boolean) As Integer
            Return System_SetUserInfo(userInfo, newPassword, CType(notifications, Notifications.INotifications), suppressNotifications)
        End Function

        'ToDo: Change to Long
        ''' <summary>
        '''     Save changes to a user information object 
        ''' </summary>
        ''' <param name="userInfo">The user information object</param>
        ''' <param name="newPassword">A new password</param>
        ''' <param name="notifications">The notifications for sending appropriate information to the user</param>
        ''' <param name="suppressAllNotifications">False sends e-mails regulary, true disables all user and admin notifications</param>
        ''' <returns>The ID of that user profile that has been saved</returns>
        ''' <remarks>
        ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
        ''' </remarks>
        <Obsolete("Use userInfo.Save instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_SetUserInfo(ByRef userInfo As UserInformation, ByRef newPassword As String, ByVal notifications As Notifications.INotifications, ByVal suppressAllNotifications As Boolean) As Integer
            Return CType(System_SetUserInfo(userInfo, newPassword, notifications, suppressAllNotifications, suppressAllNotifications), Integer)
        End Function

        ''' <summary>
        '''     Save changes to a user information object 
        ''' </summary>
        ''' <param name="userInfo">The user information object</param>
        ''' <param name="newPassword">A new password</param>
        ''' <param name="notifications">The notifications for sending appropriate information to the user</param>
        ''' <param name="suppressUserNotifications">False sends e-mails regulary, true disables all user notifications</param>
        ''' <param name="suppressSecurityAdminNotifications">False sends e-mails regulary, true disables all admin notifications</param>
        ''' <returns>The ID of that user profile that has been saved</returns>
        ''' <remarks>
        ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
        ''' </remarks>
        <Obsolete("Use userInfo.Save instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_SetUserInfo(ByRef userInfo As UserInformation, ByRef newPassword As String, ByVal notifications As Notifications.INotifications, ByVal suppressUserNotifications As Boolean, ByVal suppressSecurityAdminNotifications As Boolean) As Long
            Return userInfo.Save_Internal(newPassword, notifications, suppressUserNotifications, suppressSecurityAdminNotifications)
        End Function

        ''' <summary>
        '''     Get a user information object
        ''' </summary>
        ''' <param name="UserID">A user ID</param>
        ''' <returns>A user information object</returns>
        <Obsolete("UserID should be of type Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetUserInfo(ByVal UserID As Integer) As UserInformation
            Dim UserInfos As UserInformation() = System_GetUserInfos(New Int64() {UserID})
            If UserInfos Is Nothing OrElse UserInfos.Length = 0 Then
                Return Nothing
            Else
                Return UserInfos(0)
            End If
        End Function

        ''' <summary>
        '''     Get a user information object
        ''' </summary>
        ''' <param name="UserID">A user ID</param>
        ''' <returns>A user information object</returns>
        Public Function System_GetUserInfo(ByVal UserID As Long) As UserInformation
            Dim UserInfos As UserInformation() = System_GetUserInfos(New Int64() {UserID})
            If UserInfos Is Nothing OrElse UserInfos.Length = 0 Then
                Return Nothing
            Else
                Return UserInfos(0)
            End If
        End Function

        ''' <summary>
        '''     Get some user information objects
        ''' </summary>
        ''' <param name="UserIDs">An arraylist of user IDs</param>
        ''' <returns>An array of user information</returns>
        <Obsolete("use instead: System_GetUserInfos(ByVal UserIDs As Long()) As UserInformation())"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_GetUserInfos(ByVal UserIDs As ArrayList) As UserInformation()
            If UserIDs Is Nothing OrElse UserIDs.Count = 0 Then
                'Where nothing is, there can only be returned nothing ;-)
                Return Nothing
            Else
                Dim Users As Long()
                ReDim Users(UserIDs.Count - 1)
                For MyCounter As Integer = 0 To UserIDs.Count - 1
                    Users(MyCounter) = CType(UserIDs(MyCounter), Long)
                Next
                Return System_GetUserInfos(Users)
            End If
        End Function

        ''' <summary>
        '''     Get some user information objects
        ''' </summary>
        ''' <param name="UserIDs">An array of user IDs</param>
        ''' <returns>An array of user information</returns>
        <Obsolete("use instead: System_GetUserInfos(ByVal UserIDs As Long()) As UserInformation())"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function System_GetUserInfos(ByVal UserIDs As Integer()) As UserInformation()
            Dim UserLongIDs As Long() = Nothing
            If Not UserIDs Is Nothing Then
                ReDim UserLongIDs(UserIDs.Length - 1)
                For MyCounter As Integer = 0 To UserIDs.Length - 1
                    UserLongIDs(MyCounter) = UserIDs(MyCounter)
                Next
            End If
            Return System_GetUserInfos(UserLongIDs)
        End Function

        ''' <summary>
        '''     Get some user information objects
        ''' </summary>
        ''' <param name="UserIDs">An array of user IDs (special user IDs won't be processed)</param>
        ''' <returns>An array of user information</returns>
        Public Function System_GetUserInfos(ByVal UserIDs As Long()) As UserInformation()
            If UserIDs Is Nothing Then
                Return Nothing
            ElseIf UserIDs.Length = 0 Then
                Return New UserInformation() {} 'Return at least an array with zero values, but not nothing
            Else
                'Prepare the sql statement
                Dim sqlCommand As New System.Text.StringBuilder
                sqlCommand.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine)
                sqlCommand.Append("create table #userids (ID int)")
                sqlCommand.Append(vbNewLine)
                For Each UserID As Long In UserIDs
                    sqlCommand.Append("insert into #userids (id) values (")
                    sqlCommand.Append(UserID.ToString)
                    sqlCommand.Append(")")
                    sqlCommand.Append(vbNewLine)
                Next
                sqlCommand.Append("select * from benutzer inner join #userids on benutzer.id = #userids.id")
                sqlCommand.Append(vbNewLine)
                sqlCommand.Append("drop table #userids")
                'Now execute the created command
                Dim MyConn As New SqlConnection(Me.ConnectionString)
                Dim MyCmd As IDbCommand
                MyCmd = New SqlCommand(sqlCommand.ToString, MyConn)
                Return System_GetUserInfos(MyCmd)
            End If

        End Function

        ''' <summary>
        '''     Get all user information objects
        ''' </summary>
        ''' <returns>An array of user information</returns>
        Public Function System_GetUserInfos() As UserInformation()
            'Prepare the sql statement
            Dim SqlCommand As String = "select * from benutzer"
            'Now execute the created command
            Dim MyConn As New SqlConnection(Me.ConnectionString)
            Dim MyCmd As IDbCommand
            MyCmd = New SqlCommand(SqlCommand, MyConn)
            Return System_GetUserInfos(MyCmd)
        End Function

        ''' <summary>
        '''     Get some user information objects
        ''' </summary>
        ''' <param name="dbCommand">A database command which returns some lines from table "Benutzer"</param>
        ''' <returns>An array of user information</returns>
        Private Function System_GetUserInfos(ByVal dbCommand As IDbCommand) As UserInformation()

            Dim _WebManager As WMSystem = Me
            Dim Result As UserInformation()
            Dim MyUsers As DataTable = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(dbCommand, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "Users")
            If MyUsers.Rows.Count > 0 Then
                ReDim Preserve Result(MyUsers.Rows.Count - 1)
                Dim MyCounter As Integer = 0
                For Each MyDataRow As DataRow In MyUsers.Rows
                    Dim Gender As Sex
                    Select Case LCase(Utils.Nz(MyDataRow("Anrede"), ""))
                        Case "mr."
                            Gender = Sex.Masculine
                        Case "ms."
                            Gender = Sex.Feminine
                        Case Else
                            Gender = Sex.Undefined
                    End Select
                    Result(MyCounter) = New CompuMaster.camm.WebManager.WMSystem.UserInformation(CType(MyDataRow("ID"), Long), _
                                    CType(MyDataRow("LoginName"), String), _
                                    CType(MyDataRow("E-Mail"), String), _
                                    False, _
                                    Utils.Nz(MyDataRow("Company"), CType(Nothing, String)), _
                                    Gender, _
                                    Utils.Nz(MyDataRow("Namenszusatz"), CType(Nothing, String)), _
                                    Utils.Nz(MyDataRow("Vorname"), CType(Nothing, String)), _
                                    Utils.Nz(MyDataRow("Nachname"), CType(Nothing, String)), _
                                    Utils.Nz(MyDataRow("Titel"), CType(Nothing, String)), _
                                    Utils.Nz(MyDataRow("Strasse"), CType(Nothing, String)), _
                                    Utils.Nz(MyDataRow("PLZ"), CType(Nothing, String)), _
                                    Utils.Nz(MyDataRow("Ort"), CType(Nothing, String)), _
                                    Utils.Nz(MyDataRow("State"), CType(Nothing, String)), _
                                    Utils.Nz(MyDataRow("Land"), CType(Nothing, String)), _
                                    Utils.Nz(MyDataRow("1stPreferredLanguage"), 0), _
                                    Utils.Nz(MyDataRow("2ndPreferredLanguage"), 0), _
                                    Utils.Nz(MyDataRow("3rdPreferredLanguage"), 0), _
                                    Utils.Nz(MyDataRow("LoginDisabled"), False), _
                                    Not IsDBNull(MyDataRow("LoginLockedTill")), _
                                    False, _
                                    CType(MyDataRow("AccountAccessability"), Integer), _
                                    _WebManager, _
                                    Nothing)
                    If Result(MyCounter).Gender = Sex.Undefined AndAlso (Result(MyCounter).FirstName = Nothing OrElse Result(MyCounter).LastName = Nothing) Then
                        'Regard it as a group of persons without a specific name
                        Result(MyCounter).Gender = Sex.MissingNameOrGroupOfPersons
                    End If
                    MyCounter += 1
                Next
            Else
                Result = Nothing
            End If
            Return Result

        End Function

        ''' <summary>
        '''     Collect all groups with given SQL statement
        ''' </summary>
        ''' <param name="SQLStatement">An SQL statement</param>
        ''' <returns>An array of group information</returns>
        Private Function _System_GetGroupInfos(ByVal SQLStatement As String) As GroupInformation()
            Dim _WebManager As WMSystem = Me
            Dim _GroupInfos As GroupInformation()
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand
            MyCmd = New SqlCommand(SQLStatement, MyConn)
            Dim MyDA As SqlDataAdapter = New SqlDataAdapter(MyCmd)
            Dim MyDataSet As DataSet = New DataSet
            Try
                MyConn.Open()
                MyDA.Fill(MyDataSet, "Groups")
            Finally
                If Not MyDA Is Nothing Then
                    MyDA.Dispose()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try
            If MyDataSet.Tables("Groups").Rows.Count > 0 Then
                ReDim Preserve _GroupInfos(MyDataSet.Tables("Groups").Rows.Count - 1)
                Dim MyCounter As Integer = 0
                For Each MyDataRow As DataRow In MyDataSet.Tables("Groups").Rows
                    _GroupInfos(MyCounter) = New CompuMaster.camm.WebManager.WMSystem.GroupInformation(MyDataRow, _WebManager)
                    MyCounter += 1
                Next
            Else
                _GroupInfos = Nothing
            End If
            Return _GroupInfos
        End Function

        ''' <summary>
        '''     Return all groups
        ''' </summary>
        ''' <returns>An array of group information</returns>
        Public Function System_GetGroupInfos() As GroupInformation()
            Return _System_GetGroupInfos("select * from gruppen")
        End Function

        ''' <summary>
        '''     Return selected groups
        ''' </summary>
        ''' <param name="GroupIDs">An arraylist of group IDs</param>
        ''' <returns>An array of group information</returns>
        Public Function System_GetGroupInfos(ByVal GroupIDs As ArrayList) As GroupInformation()
            If GroupIDs Is Nothing OrElse GroupIDs.Count = 0 Then
                'Where nothing is, there can only be returned nothing ;-)
                Return Nothing
            Else
                Dim Groups As Integer()
                ReDim Groups(GroupIDs.Count - 1)
                For MyCounter As Integer = 0 To GroupIDs.Count - 1
                    Groups(MyCounter) = CType(GroupIDs(MyCounter), Integer)
                Next
                Return System_GetGroupInfos(Groups)
            End If
        End Function

        ''' <summary>
        '''     Return selected groups
        ''' </summary>
        ''' <param name="GroupIDs">An array of group IDs</param>
        ''' <returns>An array of group information</returns>
        Public Function System_GetGroupInfos(ByVal GroupIDs As Integer()) As GroupInformation()
            'only search for server groups allowed for the current user
            Dim ListOfGroups As New System.Text.StringBuilder
            For Each GroupID As Integer In GroupIDs
                If ListOfGroups.Length = 0 Then
                    ListOfGroups.Append(GroupID)
                Else
                    ListOfGroups.Append(", ")
                    ListOfGroups.Append(GroupID)
                End If
            Next
            Return _System_GetGroupInfos("select * from gruppen where id in (" & ListOfGroups.ToString & ")")
        End Function

        ''' <summary>
        '''     Load group information data
        ''' </summary>
        ''' <param name="groupName">A name of a group (case insensitive)</param>
        ''' <returns>A group information object or Nothing if the group can't be found</returns>
        Public Function System_GetGroupInfo(ByVal groupName As String) As GroupInformation
            If groupName = Nothing Then
                Throw New ArgumentNullException("groupName", "Value must contain a valid group name")
            End If
            Dim Result As GroupInformation() = _System_GetGroupInfos("select top 1 * from gruppen where name = N'" & groupName.Replace("'", "''") & "'")
            If Result Is Nothing OrElse Result.Length = 0 Then
                Return Nothing
            Else
                Return Result(0)
            End If
        End Function

        ''' <summary>
        '''     The gender of a user
        ''' </summary>
        ''' <remarks>Additional enumeration items like m, f, male, female are available just for improved value lookup on user import from text files</remarks>
        Public Enum Sex As Integer
            Undefined = 0
            Masculine = 1
            Feminine = 2
            <Obsolete("Use value Masculine instead", False), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Masculin = 1
            <Obsolete("Use value Feminine instead", False), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Feminin = 2
            MissingNameOrGroupOfPersons = 3
            <Obsolete("Use value Undefined instead", True), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> u = 0
            <Obsolete("Use value Masculine instead", True), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> m = 1
            <Obsolete("Use value Feminine instead", True), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> f = 2
            <Obsolete("Use value MissingNameOrGroupOfPersons instead", True), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> g = 3
            <Obsolete("Use value Undefined instead", True), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Unknown = 0
            <Obsolete("Use value Masculine instead", True), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Male = 1
            <Obsolete("Use value Feminine instead", True), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Female = 2
            <Obsolete("Use value MissingNameOrGroupOfPersons instead", True), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Group = 3
        End Enum

        ''' <summary>
        '''     Get all security object information
        ''' </summary>
        ''' <returns>An array of security object information</returns>
        Public Function System_GetSecurityObjectInformations() As SecurityObjectInformation()
            Dim MyCmd As SqlCommand
            MyCmd = New SqlCommand("SELECT * FROM [dbo].[Applications_CurrentAndInactiveOnes] WHERE AppDeleted = 0")
            Return System_GetSecurityObjectInformations_Internal(MyCmd)
        End Function

        ''' <summary>
        '''     Get a security object information
        ''' </summary>
        ''' <param name="SecurityObjectName">A name of a security object</param>
        ''' <returns>The requested security object information</returns>
        Public Function System_GetSecurityObjectInformations(ByVal SecurityObjectName As String) As SecurityObjectInformation()
            Dim MyCmd As SqlCommand
            If SecurityObjectName <> Nothing Then
                MyCmd = New SqlCommand("SELECT * FROM [dbo].[Applications_CurrentAndInactiveOnes] WHERE Title = @SecurityObjectName AND AppDeleted = 0")
                MyCmd.Parameters.Add("@SecurityObjectName", SqlDbType.NVarChar).Value = SecurityObjectName
            Else
                MyCmd = New SqlCommand("SELECT * FROM [dbo].[Applications_CurrentAndInactiveOnes] WHERE AppDeleted = 0")
            End If
            Return System_GetSecurityObjectInformations_Internal(MyCmd)
        End Function

        ''' <summary>
        '''     Get selected security object information
        ''' </summary>
        ''' <returns>An array of security object information</returns>
        Public Function System_GetSecurityObjectInformations(ByVal IDs As Integer()) As SecurityObjectInformation()
            Dim MyCmd As SqlCommand
            MyCmd = New SqlCommand("SELECT * FROM [dbo].[Applications_CurrentAndInactiveOnes] WHERE AppDeleted = 0 AND ID IN (" & Utils.JoinArrayToString(IDs, ",") & ")")
            Return System_GetSecurityObjectInformations_Internal(MyCmd)
        End Function

        ''' <summary>
        '''     Get selected security object information
        ''' </summary>
        ''' <returns>An array of security object information</returns>
        Private Function System_GetSecurityObjectInformations_Internal(MyCmd As SqlCommand) As SecurityObjectInformation()

            'Collect the security objects from database
            MyCmd.Connection = New SqlConnection(ConnectionString)
            MyCmd.CommandType = CommandType.Text
            Dim MyReader As IDataReader = Nothing

            Dim MyTempCollection As New Collection
            Try

                MyReader = Tools.Data.DataQuery.AnyIDataProvider.ExecuteReader(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                Dim RequiredFlagsSupported As Boolean = False
                If Me.System_DBVersion_Ex(True).Build >= 185 Then
                    RequiredFlagsSupported = True
                End If
                While MyReader.Read
                    Dim secObj As SecurityObjectInformation
                    Dim navItem As New Security.NavigationInformation( _
                            0, _
                            Nothing, _
                            Utils.Nz(MyReader("Level1Title"), String.Empty), _
                            Utils.Nz(MyReader("Level2Title"), String.Empty), _
                            Utils.Nz(MyReader("Level3Title"), String.Empty), _
                            Utils.Nz(MyReader("Level4Title"), String.Empty), _
                            Utils.Nz(MyReader("Level5Title"), String.Empty), _
                            Utils.Nz(MyReader("Level6Title"), String.Empty), _
                            Utils.Nz(MyReader("Level1TitleIsHtmlCoded"), False), _
                            Utils.Nz(MyReader("Level2TitleIsHtmlCoded"), False), _
                            Utils.Nz(MyReader("Level3TitleIsHtmlCoded"), False), _
                            Utils.Nz(MyReader("Level4TitleIsHtmlCoded"), False), _
                            Utils.Nz(MyReader("Level5TitleIsHtmlCoded"), False), _
                            Utils.Nz(MyReader("Level6TitleIsHtmlCoded"), False), _
                            Utils.Nz(MyReader("NavURL"), String.Empty), _
                            Utils.Nz(MyReader("NavFrame"), String.Empty), _
                            Utils.Nz(MyReader("NavTooltipText"), String.Empty), _
                            Utils.Nz(MyReader("AddLanguageID2URL"), False), _
                            Utils.Nz(MyReader("LanguageID"), 0), _
                            Utils.Nz(MyReader("LocationID"), 0), _
                            Utils.Nz(MyReader("Sort"), 0), _
                            Utils.Nz(MyReader("IsNew"), False), _
                            Utils.Nz(MyReader("IsUpdated"), False), _
                            Utils.Nz(MyReader("ResetIsNewUpdatedStatusOn"), DateTime.MinValue), _
                            Utils.Nz(MyReader("OnMouseOver"), String.Empty), _
                            Utils.Nz(MyReader("OnMouseOut"), String.Empty), _
                            Utils.Nz(MyReader("OnClick"), String.Empty))
                    If RequiredFlagsSupported Then
                        secObj = New SecurityObjectInformation( _
                                    Utils.Nz(MyReader("ID"), 0), _
                                    Utils.Nz(MyReader("Title"), CType(Nothing, String)), _
                                    Utils.Nz(MyReader("TitleAdminArea"), CType(Nothing, String)), _
                                    Utils.Nz(MyReader("Remarks"), CType(Nothing, String)), _
                                    Utils.Nz(MyReader("ModifiedBy"), 0&), _
                                    Utils.Nz(MyReader("ModifiedOn"), CType(Nothing, DateTime)), _
                                    Utils.Nz(MyReader("ReleasedBy"), 0&), _
                                    Utils.Nz(MyReader("ReleasedOn"), CType(Nothing, DateTime)), _
                                    Utils.Nz(MyReader("AppDisabled"), False), _
                                    Utils.Nz(MyReader("AppDeleted"), False), _
                                    Utils.Nz(MyReader("AuthsAsAppID"), 0), _
                                    Utils.Nz(MyReader("SystemAppType"), 0), _
                                    Utils.Nz(MyReader("RequiredUserProfileFlags"), ""), _
                                    Utils.Nz(MyReader("RequiredUserProfileFlagsRemarks"), ""), _
                                    navItem, _
                                    Me)
                    Else
                        secObj = New SecurityObjectInformation( _
                                    Utils.Nz(MyReader("ID"), 0), _
                                    Utils.Nz(MyReader("Title"), CType(Nothing, String)), _
                                    Utils.Nz(MyReader("TitleAdminArea"), CType(Nothing, String)), _
                                    Utils.Nz(MyReader("Remarks"), CType(Nothing, String)), _
                                    Utils.Nz(MyReader("ModifiedBy"), 0&), _
                                    Utils.Nz(MyReader("ModifiedOn"), CType(Nothing, DateTime)), _
                                    Utils.Nz(MyReader("ReleasedBy"), 0&), _
                                    Utils.Nz(MyReader("ReleasedOn"), CType(Nothing, DateTime)), _
                                    Utils.Nz(MyReader("AppDisabled"), False), _
                                    Utils.Nz(MyReader("AppDeleted"), False), _
                                    Utils.Nz(MyReader("AuthsAsAppID"), 0), _
                                    Utils.Nz(MyReader("SystemAppType"), 0), _
                                    Utils.Nz(MyReader("RequiredUserProfileFlags"), ""), _
                                    "", _
                                    navItem, _
                                    Me)
                    End If
                    MyTempCollection.Add(secObj)
                End While
            Finally
                If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                    MyReader.Close()
                End If
                If Not MyCmd Is Nothing Then
                    Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeCommandAndConnection(MyCmd)
                End If
            End Try

            'return the results as an array
            If MyTempCollection.Count = 0 Then
                Return Nothing
            Else
                Dim Result As SecurityObjectInformation()
                ReDim Result(MyTempCollection.Count - 1)
                For MyCounter As Integer = 1 To MyTempCollection.Count
                    Result(MyCounter - 1) = CType(MyTempCollection(MyCounter), SecurityObjectInformation)
                Next
                Return Result
            End If

        End Function

#End Region


#Region "Events & EventHandling"
        ''' <summary>
        '''     Fires when a language change has been forced
        ''' </summary>
        ''' <remarks>
        '''     To ensure the correct localization of the user interface, all important text switches should perform when this event gets fired.
        ''' </remarks>
        Public Event LanguageDataLoaded(ByVal LanguageID As Integer)
        Private Sub _LanguageDataLoaded(ByVal LanguageID As Integer) Handles _Internationalization.LanguageDataLoaded
            RaiseEvent LanguageDataLoaded(LanguageID)
        End Sub
#End Region

#Region "Configuration settings from web.config/app.config"
        ''' <summary>
        '''     Configuration settings read from web.config/app.config
        ''' </summary>
        Friend Class Configuration
            Inherits CompuMaster.camm.WebManager.Configuration
        End Class
#End Region

#Region "Compact Policies"
        ''' <summary>
        '''     A prepared policy with a full set of rights allowing (nearly) everything
        ''' </summary>
        ''' <remarks>
        '''     Without warranty or promised purpose for anything - use at your own risk!
        ''' </remarks>
        Public Const CompactPolicyFullRights As String = "ALL CUR ADMa DEVa TAIa PSAa PSDa IVAa IVDa CONa HISa TELa OTPa OUR DELa SAMa UNRa PUBa OTRa IND PHY ONL UNI PUR FIN COM NAV INT DEM CNT STA POL HEA PRE GOV LOC"
        Private _CompactPolicyHeader As String

        ''' <summary>
        '''     A compact policy header to be sent to the client
        ''' </summary>
        ''' <value></value>
        Public Property CompactPolicyHeader() As String
            Get
                If _CompactPolicyHeader Is Nothing Then
                    _CompactPolicyHeader = Configuration.CompactPolicyHeader
                End If
                Return _CompactPolicyHeader
            End Get
            Set(ByVal Value As String)
                _CompactPolicyHeader = Value
            End Set
        End Property

#End Region

    End Class

End Namespace