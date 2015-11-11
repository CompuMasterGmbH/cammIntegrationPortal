Option Explicit On 
Option Strict On

Namespace CompuMaster.camm.WebManager

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.WindowsClient
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' A camm Web-Manager client for windows and console applications
    ''' </summary>
    ''' <remarks>
    ''' This client is not intended for web applications and throws an exception if used in a HttpContext.
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	20.03.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class WindowsClient
        Implements IWebManager

        Private _BaseWebManager As WMSystem
        Friend Overridable Property BaseWebManager() As WMSystem
            Get
                Return _BaseWebManager
            End Get
            Set(ByVal value As WMSystem)
                _BaseWebManager = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Create an instance of camm Web-Manager client
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	20.03.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
            If Not System.Web.HttpContext.Current Is Nothing Then
                Throw New Exception("WindowsWebManager can't be used in HTTP contexts, please use it in windows or console applications only")
            End If
            BaseWebManager = New WMSystemEmbedded()
        End Sub

        Friend Sub New(ByVal type As System.Type)
            BaseWebManager = New WMSystemEmbedded()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Create an instance of camm Web-Manager client
        ''' </summary>
        ''' <param name="connectionString">The connection string to the camm Web-Manager database</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	20.03.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal connectionString As String)
            Me.New()
            Me.ConnectionString = connectionString
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The connection string to the camm Web-Manager database
        ''' </summary>
        ''' <value>A string containing all information required to successfully establish a connection to the database</value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ConnectionString() As String Implements IWebManager.ConnectionString
            Get
                Return BaseWebManager.ConnectionString
            End Get
            Set(ByVal Value As String)
                BaseWebManager.ConnectionString = Value
            End Set
        End Property

        ''' <summary>
        ''' The server identification string for the current server
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ServerIdentString() As String
            Get
                Return BaseWebManager.CurrentServerIdentString
            End Get
            Set(ByVal value As String)
                BaseWebManager.CurrentServerIdentString = value
            End Set
        End Property

        'TODO: make it public but with non-WMSystem-ServerInformation-class
        Friend ReadOnly Property ServerInfo() As WMSystem.ServerInformation
            Get
                Return BaseWebManager.CurrentServerInfo()
            End Get
        End Property

        ''' <summary>
        '''     Login with login name and password
        ''' </summary>
        ''' <param name="loginName">The login name of a user</param>
        ''' <param name="password">The password of this user</param>
        ''' <exception cref="System.Exception">If the login fails, this exception will be thrown</exception>
        ''' <remarks>
        ''' </remarks>
        Public Sub Login(ByVal loginName As String, ByVal password As String)
            If Me.ServerIdentString = Nothing Then Throw New Exception("Before a login, the server identitification string must be set")
            If Me.ConnectionString = Nothing Then Throw New Exception("Before a login, the databae connection string must be set")
            Dim LoginResult As WMSystem.ReturnValues_UserValidation
            LoginResult = Me.BaseWebManager.ExecuteLogin(loginName, password, True)
            Select Case LoginResult
                Case WMSystem.ReturnValues_UserValidation.ValidationSuccessfull
                    'okay
                Case Else
                    Throw New Exception("Login denied (" & LoginResult.ToString & ")")
            End Select
        End Sub

        ''' <summary>
        '''     Logout and perform some clean-ups
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Public Sub Logout()
            Me.BaseWebManager.ExecuteLogout()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     User interface related properties and methods
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	01.03.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property UI() As CompuMaster.camm.WebManager.UserInterface Implements IWebManager.UI
            Get
                Return BaseWebManager.UI
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Methods for fast data querying
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	23.10.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property PerformanceMethods() As PerformanceMethods Implements IWebManager.PerformanceMethods
            Get
                Static _PerformanceMethods As CompuMaster.camm.WebManager.PerformanceMethods
                If _PerformanceMethods Is Nothing Then _PerformanceMethods = New CompuMaster.camm.WebManager.PerformanceMethods(BaseWebManager)
                Return _PerformanceMethods
            End Get
        End Property

#If ImplementedSubClassesWithIWebManagerInterface Then
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The messaging methods for e-mail distribution
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	04.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property MessagingEMails() As Messaging.EMails Implements IWebManager.MessagingEMails
            Get
                Static _MessagingEMails As Messaging.EMails
                If _MessagingEMails Is Nothing Then
                    _MessagingEMails = New Messaging.EMails(Me)
                End If
                Return _MessagingEMails
            End Get
        End Property

        Private _IsSupported As WMCapabilities
        ''' <summary>
        ''' Which features are supported by the current instance of camm Web-Manager?
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Each instance is set up separately. Some ones support the sending of SMS messages because the gateways are configured, other instances haven't got prepared to send SMS messages. That's why you can and should ask the current configuration here.</remarks>
        Public ReadOnly Property IsSupported() As WMCapabilities Implements IWebManager.IsSupported
            Get
                If _IsSupported Is Nothing Then
                    _IsSupported = New WMCapabilities(Me)
                End If
                Return _IsSupported
            End Get
        End Property
#End If

    End Class

    ''' <summary>
    ''' A Web-Manager instance for non-HTTP environment (but which can be used in HTTP-environments)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class NetClient
        Inherits WindowsClient

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Create an instance of camm Web-Manager client
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	20.03.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
            MyBase.New(CType(Nothing, System.Type))
        End Sub

        ''' <summary>
        ''' Provides access to the inner WMSystem object because several instantiations require it, currently
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Obsolete("Property will be removed in a later version")> Public ReadOnly Property WebManager() As WMSystem
            Get
                Return MyBase.BaseWebManager
            End Get
        End Property

    End Class

    Friend Class WMSystemEmbedded
        Inherits WMSystem

        Public Sub New()
            MyBase.New("")
        End Sub

        Public Overrides ReadOnly Property CurrentUserLoginName() As String
            Get
                'Console/Windows application
                Return _CurrentUserLoginName
            End Get
        End Property

        Private _CurrentScriptEngineSessionID As String
        Public Overrides ReadOnly Property CurrentScriptEngineSessionID() As String
            Get
                'Windows/console applications
                If _CurrentScriptEngineSessionID Is Nothing Then
                    Dim Rnd As New System.Random
                    _CurrentScriptEngineSessionID = Rnd.Next.ToString("00000000000000000000")
                End If
                Return _CurrentScriptEngineSessionID
            End Get
        End Property

        Public Overrides Sub ResetUserLoginName()
            'Console/Windows application
            _CurrentUserLoginName = Nothing
        End Sub

        Friend Overrides Sub SetUserLoginName(ByVal loginName As String)
            'Console/Windows application
            _CurrentUserLoginName = loginName
        End Sub

    End Class

End Namespace