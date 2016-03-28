'Copyright 2004-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Reflection
Imports System.Web.UI
Imports System.Web

#Region "WebManager base controls"
Namespace CompuMaster.camm.WebManager.Controls

#If NetFramework <> "1_1" Then
    <System.Runtime.InteropServices.ComVisible(False), ToolboxData("<{0}:WebManager ID=""cammWebManager"" runat=""server"" SecurityObject=""""></{0}:WebManager>"), System.Web.UI.NonVisualControl(), System.Web.UI.Themeable(False), System.Web.UI.PersistChildren(False), System.Web.UI.ParseChildren(False)> Public MustInherit Class cammWebManager
#Else
    <System.Runtime.InteropServices.ComVisible(False), ToolboxData("<{0}:WebManager ID=""cammWebManager"" runat=""server"" SecurityObject=""""></{0}:WebManager>")> Public MustInherit Class cammWebManager
#End If
        Inherits CompuMaster.camm.WebManager.WMSystem

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creates a new instance of the camm Web-Manager
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("You should not create a camm Web-Manager instance by yourself in web applications, use the cammWebManager property or cammWebManager object created in the aspx page itself")> _
        Public Sub New()
            MyBase.New("")
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creates a new instance of the camm Web-Manager
        ''' </summary>
        ''' <param name="databaseConnectionString">The connection string to the database</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	24.02.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal databaseConnectionString As String)
            MyBase.New(databaseConnectionString)
        End Sub

        Friend Sub New(ByVal databaseConnectionString As String, ignoreCheckCompatibilityToDatabaseByBuildNumber As Boolean)
            MyBase.New(databaseConnectionString, ignoreCheckCompatibilityToDatabaseByBuildNumber)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return the value of the public field with the specified name, defined inside the obj object
        ''' </summary>
        ''' <param name="obj">An object which shall be analyzed for existing children objects</param>
        ''' <param name="fieldName">The name (case in-sensitive) of the searched field or property</param>
        ''' <param name="defaultValue">The return value if the field can't be found</param>
        ''' <returns>The object/value of the found child</returns>
        ''' <remarks>
        '''     This method enumerates 1st the fields and 2nd the properties for a searched object
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	09.10.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function GetField(ByVal obj As Object, ByVal fieldName As String, ByVal defaultValue As Object) As Object

            '1st step: search a field with requested fieldName
            Dim fi As FieldInfo = Nothing
            Dim fiinf As FieldInfo() = obj.GetType().GetFields(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.IgnoreCase)
            For myfieldcount As Integer = 0 To fiinf.Length - 1
                If fiinf(myfieldcount).Name.ToLower = fieldName.ToLower Then
                    fi = obj.GetType().GetField(fiinf(myfieldcount).Name, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.IgnoreCase)
                End If
            Next

            If Not fi Is Nothing Then
                'Finish search here and return the found field value
                Return fi.GetValue(obj)
            Else
                '2nd step: search for a property with the searched name
                Dim pi As PropertyInfo = Nothing
                Dim propinf As PropertyInfo() = obj.GetType.GetProperties(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
                For mypropcount As Integer = 0 To propinf.Length - 1
                    If propinf(mypropcount).Name.ToLower = fieldName.ToLower Then
                        pi = obj.GetType().GetProperty(propinf(mypropcount).Name, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
                    End If
                Next

                If Not pi Is Nothing Then
                    'return found property value
                    Return pi.GetValue(obj, Nothing)
                Else
                    '3rd step: no field or property has been found; we return the default value now
                    Return defaultValue
                End If
            End If
        End Function

        Friend IsAlreadyInitialized As Boolean = False
        Friend IsLanguageDataLoaded As Boolean = False

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load configuration (for internal purposes only)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	17.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Overridable Sub _LoadConfiguration()
            LoadConfiguration()
            Me.IsConfigurationLoaded = True
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load the configuration, mostly in /sysdata/config.vb
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''         ''' 	[adminsupport]	23.11.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected MustOverride Sub LoadConfiguration()

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load customized language/market strings (for internal purposes only)
        ''' </summary>
        ''' <param name="LanguageID">The market which shall be used</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	17.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub _LoadLanguageData(ByVal LanguageID As Integer) Handles MyBase.LanguageDataLoaded
            LoadLanguageData(LanguageID)
            Me.IsLanguageDataLoaded = True
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load customized language/market strings
        ''' </summary>
        ''' <param name="LanguageID">The market which shall be used</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	23.11.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub LoadLanguageData(ByVal LanguageID As Integer)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Initializes the webmanager inclusive loading of configuration and customized strings for the markets
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	23.11.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub System_Init()

            If IsAlreadyInitialized = False Then

                'Load configuration and language dependent data
                If SafeMode Then
                    Try
                        _LoadConfiguration()
                    Catch
                        'on error page, ignore errors raised because of configuration loading 
                    End Try
                Else
                    _LoadConfiguration()
                End If
                Response.ContentEncoding = System.Text.Encoding.UTF8
                Internationalization.LoadLanguageStrings(UI.MarketID)

                'Validate/update sessiondata
                If HttpContext.Current.Session Is Nothing OrElse CType(HttpContext.Current.Session("WebManager.ApplicationIsInitialized"), Boolean) = Nothing OrElse Not CType(HttpContext.Current.Session("WebManager.ApplicationIsInitialized"), Boolean) = True Then
                    If Not GetType(CompuMaster.camm.WebManager.Pages.Application.BaseErrorPage).IsInstanceOfType(Me.Page) AndAlso Not GetType(CompuMaster.camm.WebManager.Pages.Application.BaseWarningPage).IsInstanceOfType(Me.Page) AndAlso Not Me.IsLoggedOn Then
                        'Try to get session data from webmanager's database
                        Me.TryToRetrieveUserNameFromScriptEngineSessionID()
                    End If
                    If Not HttpContext.Current.Session Is Nothing Then
                        HttpContext.Current.Session("WebManager.ApplicationIsInitialized") = True
                    End If
                End If

                'Enable the app key to allow the customized ComponentArtWebUI component to run in redistributable mode
                If HttpContext.Current.Application("ComponentArtWebUI_AppKey") Is Nothing Then
                    Application("ComponentArtWebUI_AppKey") = "This edition of ComponentArt Web.UI is licensed for CompuMaster/camm application only."
                End If
                HttpContext.Current.Application("WebManager.Application.Version") = Me.System_Version_Ex.ToString

                'Clean up
                IsAlreadyInitialized = True

                'Fire event
                RaiseEvent InitializedWebManager()

            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fires when the camm Web-Manager has been initialized
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	02.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Event InitializedWebManager()

        Private Sub OnWebManagerInit() Handles MyBase.InitLoadConfiguration
            System_Init()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This OnLoad method executes an automatic databinding if not disabled
        ''' </summary>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	02.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As EventArgs)
            MyBase.OnLoad(e)
            If DataBindAutomaticallyWhilePageOnLoad = True Then
                Try
                    Me.Page.DataBind()
                Catch ex As Exception
                    Me.Log.RuntimeWarning("Automatic databind has been enabled, but wasn't able to bind without errors on page " & HttpContext.Current.Request.ServerVariables("SCRIPT_NAME"), ex.ToString, WMSystem.DebugLevels.NoDebug, False, True)
                End Try
                MyBase.System_DebugTraceWrite("DataBind() executed on page to ensure all controls have got a valid camm WebManager instance to work with", WMSystem.DebugLevels.Medium_LoggingOfDebugInformation)
            End If
        End Sub

        Private _DataBindAutomaticallyWhilePageOnLoad As TriState = TriState.UseDefault
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Automatically bind data to bind cammWebManager instances to sub controls 
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	19.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DataBindAutomaticallyWhilePageOnLoad() As Boolean
            Get
                If _DataBindAutomaticallyWhilePageOnLoad = TriState.UseDefault Then
                    _DataBindAutomaticallyWhilePageOnLoad = Utils.BooleanToTristate(Configuration.DataBindAutomaticallyWhilePageOnLoad)
                End If
                Return Utils.TristateToBoolean(_DataBindAutomaticallyWhilePageOnLoad)
            End Get
            Set(ByVal Value As Boolean)
                _DataBindAutomaticallyWhilePageOnLoad = Utils.BooleanToTristate(Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Some debug output for trace
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.07.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cammWebManager_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            If Me.DebugLevel >= WMSystem.DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails Then
                Me.Trace.Write("camm WebManager", "DebugLevel: " & CType(Me.DebugLevel, Byte).ToString & " (" & Me.DebugLevel.ToString & ")")
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Controls.cammWebManagerJIT
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A cammWebManager class which can be used to create a webmanager just on the fly
    ''' </summary>
    ''' <remarks>
    '''     No configuration information or localization data will be applied automatically.
    '''     This is only intended to be used 
    '''     <list>
    '''         <item>by the HttpApplication to access an unconfigured but running instance of camm Web-Manager</item>
    '''         <item>by the page's JIT creator when it is allowed (attention: only default values - no customization is available!)</item>
    '''     </list>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	09.07.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class cammWebManagerJIT
        Inherits cammWebManager

        Public Sub New()
            MyBase.New("")
        End Sub

        Friend Sub New(ignoreCheckCompatibilityToDatabaseByBuildNumber As Boolean)
            MyBase.New("", ignoreCheckCompatibilityToDatabaseByBuildNumber)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load configuration (for internal purposes only)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	17.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Overrides Sub _LoadConfiguration()
            Me.IsConfigurationLoaded = True
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Do not load any configuration data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	17.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub LoadConfiguration()
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Interface	 : camm.WebManager.Controls.IControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     An interface for all controls which are implementing the cammWebManager property
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	27.06.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Interface IControl
        Property cammWebManager() As cammWebManager
        ReadOnly Property Control() As System.Web.UI.Control
    End Interface

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Controls.HtmlContainerControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A System.Web.UI.HtmlControl.HtmlContainerControl with a property to access the camm WebManager
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[swiercz]	21.12.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Runtime.InteropServices.ComVisible(False)> Public MustInherit Class HtmlContainerControl
        Inherits System.Web.UI.HtmlControls.HtmlContainerControl
        Implements IControl

        Private _webmanager As cammWebManager
        Private AlreadyTryedToLookUpCammWebManager As Boolean
        Public Property cammWebManager() As cammWebManager Implements IControl.cammWebManager
            Get
                If Not _webmanager Is Nothing Then 'Save a few checks in following code block
                    Return _webmanager
                End If
                If _webmanager Is Nothing AndAlso Not Me.Page Is Nothing AndAlso GetType(Pages.Page).IsInstanceOfType(Me.Page) AndAlso Not CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation Is Nothing Then
                    _webmanager = CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation
                End If
                If _webmanager Is Nothing AndAlso AlreadyTryedToLookUpCammWebManager = False Then
                    Try
                        _webmanager = CType(CompuMaster.camm.WebManager.Controls.cammWebManager.GetField(Page, "cammWebManager", Nothing), cammWebManager)
                        If _webmanager Is Nothing Then
                            HttpContext.Current.Trace.Warn("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: GetField without result")
                        Else
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager successfull")
                        End If
                    Catch ex As Exception
                        HttpContext.Current.Trace.Warn("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: " & ex.Message & ex.StackTrace)
                    Finally
                        AlreadyTryedToLookUpCammWebManager = True
                    End Try
                End If
                Return _webmanager
            End Get
            Set(ByVal Value As cammWebManager)
                _webmanager = Value
            End Set
        End Property

        Public ReadOnly Property Control() As System.Web.UI.Control Implements IControl.Control
            Get
                Return CType(Me, System.Web.UI.Control)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Search for the server form in the list of parent controls
        ''' </summary>
        ''' <returns>The control of the server form if it's existant</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function LookupParentServerForm() As System.Web.UI.HtmlControls.HtmlForm
            Static Result As System.Web.UI.HtmlControls.HtmlForm
            If Result Is Nothing Then
                Result = Utils.LookupParentServerForm(Me)
            End If
            Return Result
        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Controls.LiteralControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A System.Web.UI.LiteralControl with a property to access the camm WebManager
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[swiercz]	21.12.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Runtime.InteropServices.ComVisible(False)> Public MustInherit Class LiteralControl
        Inherits System.Web.UI.LiteralControl
        Implements IControl

        Private _webmanager As cammWebManager
        Private AlreadyTryedToLookUpCammWebManager As Boolean
        Public Property cammWebManager() As cammWebManager Implements IControl.cammWebManager
            Get
                If Not _webmanager Is Nothing Then 'Save a few checks in following code block
                    Return _webmanager
                End If
                If _webmanager Is Nothing AndAlso Not Me.Page Is Nothing AndAlso GetType(Pages.Page).IsInstanceOfType(Me.Page) AndAlso Not CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation Is Nothing Then
                    _webmanager = CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation
                End If
                If _webmanager Is Nothing AndAlso AlreadyTryedToLookUpCammWebManager = False Then
                    Try
                        _webmanager = CType(CompuMaster.camm.WebManager.Controls.cammWebManager.GetField(Page, "cammWebManager", Nothing), cammWebManager)
                        If _webmanager Is Nothing Then
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: no CWM instance found")
                        Else
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager successfull")
                        End If
                    Catch ex As Exception
                        HttpContext.Current.Trace.Warn("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: " & ex.Message & ex.StackTrace)
                    Finally
                        AlreadyTryedToLookUpCammWebManager = True
                    End Try
                End If
                Return _webmanager
            End Get
            Set(ByVal Value As cammWebManager)
                _webmanager = Value
            End Set
        End Property

        Public ReadOnly Property Control() As System.Web.UI.Control Implements IControl.Control
            Get
                Return CType(Me, System.Web.UI.Control)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Search for the server form in the list of parent controls
        ''' </summary>
        ''' <returns>The control of the server form if it's existant</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function LookupParentServerForm() As System.Web.UI.HtmlControls.HtmlForm
            Static Result As System.Web.UI.HtmlControls.HtmlForm
            If Result Is Nothing Then
                Result = Utils.LookupParentServerForm(Me)
            End If
            Return Result
        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Controls.WebControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A System.Web.UI.WebControls.WebControl with a property to access the camm WebManager
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	09.10.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Runtime.InteropServices.ComVisible(False)> Public MustInherit Class WebControl
        Inherits System.Web.UI.WebControls.WebControl
        Implements IControl

        Private _webmanager As cammWebManager
        Private AlreadyTryedToLookUpCammWebManager As Boolean
        Public Property cammWebManager() As cammWebManager Implements IControl.cammWebManager
            Get
                If Not _webmanager Is Nothing Then 'Save a few checks in following code block
                    Return _webmanager
                End If
                If _webmanager Is Nothing AndAlso Not Me.Page Is Nothing AndAlso GetType(Pages.Page).IsInstanceOfType(Me.Page) AndAlso Not CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation Is Nothing Then
                    _webmanager = CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation
                End If
                If _webmanager Is Nothing AndAlso AlreadyTryedToLookUpCammWebManager = False Then
                    Try
                        _webmanager = CType(CompuMaster.camm.WebManager.Controls.cammWebManager.GetField(Page, "cammWebManager", Nothing), cammWebManager)
                        If _webmanager Is Nothing Then
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: no CWM instance found")
                        Else
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager successfull")
                        End If
                    Catch ex As Exception
                        HttpContext.Current.Trace.Warn("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: " & ex.Message & ex.StackTrace)
                    Finally
                        AlreadyTryedToLookUpCammWebManager = True
                    End Try
                End If
                Return _webmanager
            End Get
            Set(ByVal Value As cammWebManager)
                _webmanager = Value
            End Set
        End Property

        Private ReadOnly Property Control() As System.Web.UI.Control Implements IControl.Control
            Get
                Return CType(Me, System.Web.UI.Control)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Search for the server form in the list of parent controls
        ''' </summary>
        ''' <returns>The control of the server form if it's existant</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function LookupParentServerForm() As System.Web.UI.HtmlControls.HtmlForm
            Static Result As System.Web.UI.HtmlControls.HtmlForm
            If Result Is Nothing Then
                Result = Utils.LookupParentServerForm(Me)
            End If
            Return Result
        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Controls.Control
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A System.Web.UI.Control with a property to access the camm WebManager
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	09.10.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Runtime.InteropServices.ComVisible(False)> Public MustInherit Class Control
        Inherits System.Web.UI.Control
        Implements IControl

        Private _webmanager As cammWebManager
        Private AlreadyTryedToLookUpCammWebManager As Boolean
        Public Property cammWebManager() As cammWebManager Implements IControl.cammWebManager
            Get
                If Not _webmanager Is Nothing Then 'Save a few checks in following code block
                    Return _webmanager
                End If
                If _webmanager Is Nothing AndAlso Not Me.Page Is Nothing AndAlso GetType(Pages.Page).IsInstanceOfType(Me.Page) AndAlso Not CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation Is Nothing Then
                    _webmanager = CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation
                End If
                If _webmanager Is Nothing AndAlso AlreadyTryedToLookUpCammWebManager = False Then
                    Try
                        _webmanager = CType(CompuMaster.camm.WebManager.Controls.cammWebManager.GetField(Page, "cammWebManager", Nothing), cammWebManager)
                        If _webmanager Is Nothing Then
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: no CWM instance found")
                        Else
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager successfull")
                        End If
                    Catch ex As Exception
                        HttpContext.Current.Trace.Warn("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: " & ex.Message & ex.StackTrace)
                    Finally
                        AlreadyTryedToLookUpCammWebManager = True
                    End Try
                End If
                Return _webmanager
            End Get
            Set(ByVal Value As cammWebManager)
                _webmanager = Value
            End Set
        End Property

        Private ReadOnly Property Control() As System.Web.UI.Control Implements IControl.Control
            Get
                Return CType(Me, System.Web.UI.Control)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Search for the server form in the list of parent controls
        ''' </summary>
        ''' <returns>The control of the server form if it's existant</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function LookupParentServerForm() As System.Web.UI.HtmlControls.HtmlForm
            Static Result As System.Web.UI.HtmlControls.HtmlForm
            If Result Is Nothing Then
                Result = Utils.LookupParentServerForm(Me)
            End If
            Return Result
        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Controls.Control
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A System.Web.UI.TemplateControl with a property to access the camm WebManager
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	09.10.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Runtime.InteropServices.ComVisible(False)> Public MustInherit Class TemplateControl
        Inherits System.Web.UI.TemplateControl
        Implements IControl

        Private _webmanager As cammWebManager
        Private AlreadyTryedToLookUpCammWebManager As Boolean
        Public Property cammWebManager() As cammWebManager Implements IControl.cammWebManager
            Get
                If Not _webmanager Is Nothing Then 'Save a few checks in following code block
                    Return _webmanager
                End If
                If _webmanager Is Nothing AndAlso Not Me.Page Is Nothing AndAlso GetType(Pages.Page).IsInstanceOfType(Me.Page) AndAlso Not CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation Is Nothing Then
                    _webmanager = CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation
                End If
                If _webmanager Is Nothing AndAlso AlreadyTryedToLookUpCammWebManager = False Then
                    Try
                        _webmanager = CType(CompuMaster.camm.WebManager.Controls.cammWebManager.GetField(Page, "cammWebManager", Nothing), cammWebManager)
                        If _webmanager Is Nothing Then
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: no CWM instance found")
                        Else
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager successfull")
                        End If
                    Catch ex As Exception
                        HttpContext.Current.Trace.Warn("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: " & ex.Message & ex.StackTrace)
                    Finally
                        AlreadyTryedToLookUpCammWebManager = True
                    End Try
                End If
                Return _webmanager
            End Get
            Set(ByVal Value As cammWebManager)
                _webmanager = Value
            End Set
        End Property

        Private ReadOnly Property Control() As System.Web.UI.Control Implements IControl.Control
            Get
                Return CType(Me, System.Web.UI.Control)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Search for the server form in the list of parent controls
        ''' </summary>
        ''' <returns>The control of the server form if it's existant</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function LookupParentServerForm() As System.Web.UI.HtmlControls.HtmlForm
            Static Result As System.Web.UI.HtmlControls.HtmlForm
            If Result Is Nothing Then
                Result = Utils.LookupParentServerForm(Me)
            End If
            Return Result
        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Controls.BaseControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A System.Web.UI.UserControl with a property to access the camm WebManager
    ''' </summary>
    ''' <remarks>
    '''     Obsolete, to be removed in one of the next versions
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	09.10.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Obsolete("Use UserControl instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public MustInherit Class BaseControl
        Inherits UserControl

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Controls.BaseControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A System.Web.UI.UserControl with a property to access the camm WebManager
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	09.10.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Runtime.InteropServices.ComVisible(False)> Public MustInherit Class UserControl
        Inherits System.Web.UI.UserControl
        Implements IControl

        Private _webmanager As cammWebManager
        Private AlreadyTryedToLookUpCammWebManager As Boolean
        Public Property cammWebManager() As cammWebManager Implements IControl.cammWebManager
            Get
                If Not _webmanager Is Nothing Then 'Save a few checks in following code block
                    Return _webmanager
                End If
                If _webmanager Is Nothing AndAlso Not Me.Page Is Nothing AndAlso GetType(Pages.Page).IsInstanceOfType(Me.Page) AndAlso Not CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation Is Nothing Then
                    _webmanager = CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation
                End If
                If _webmanager Is Nothing AndAlso AlreadyTryedToLookUpCammWebManager = False Then
                    Try
                        _webmanager = CType(CompuMaster.camm.WebManager.Controls.cammWebManager.GetField(Page, "cammWebManager", Nothing), cammWebManager)
                        If _webmanager Is Nothing Then
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: no CWM instance found")
                        Else
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager successfull")
                        End If
                    Catch ex As Exception
                        HttpContext.Current.Trace.Warn("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: " & ex.Message & ex.StackTrace)
                    Finally
                        AlreadyTryedToLookUpCammWebManager = True
                    End Try
                End If
                Return _webmanager
            End Get
            Set(ByVal Value As cammWebManager)
                _webmanager = Value
            End Set
        End Property

        Private ReadOnly Property Control() As System.Web.UI.Control Implements IControl.Control
            Get
                Return CType(Me, System.Web.UI.Control)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Search for the server form in the list of parent controls
        ''' </summary>
        ''' <returns>The control of the server form if it's existant</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function LookupParentServerForm() As System.Web.UI.HtmlControls.HtmlForm
            Static Result As System.Web.UI.HtmlControls.HtmlForm
            If Result Is Nothing Then
                Result = Utils.LookupParentServerForm(Me)
            End If
            Return Result
        End Function

    End Class

    <System.Runtime.InteropServices.ComVisible(False)> Public Class META
        Inherits UserControl

        Sub Control_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            If Controls.Count = 0 AndAlso Not cammWebManager Is Nothing Then
                Controls.Add(New System.Web.UI.LiteralControl(cammWebManager.System_GetHtmlMetaBlock))
            ElseIf Controls.Count = 0 AndAlso cammWebManager Is Nothing Then
                Me.Page.Trace.Warn("camm WebManager Control ""META""", "cammWebManager hasn't been assigned yet")
            End If
        End Sub

    End Class

    Namespace WebControls

        <System.Runtime.InteropServices.ComVisible(False)> Public Class Image
            Inherits System.Web.UI.WebControls.Image
            Implements IControl

            Private _webmanager As cammWebManager
            Private AlreadyTryedToLookUpCammWebManager As Boolean
            Public Property cammWebManager() As cammWebManager Implements IControl.cammWebManager
                Get
                    If Not _webmanager Is Nothing Then 'Save a few checks in following code block
                        Return _webmanager
                    End If
                    If _webmanager Is Nothing AndAlso Not Me.Page Is Nothing AndAlso GetType(Pages.Page).IsInstanceOfType(Me.Page) AndAlso Not CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation Is Nothing Then
                        _webmanager = CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation
                    End If
                    If _webmanager Is Nothing AndAlso AlreadyTryedToLookUpCammWebManager = False Then
                        Try
                            _webmanager = CType(CompuMaster.camm.WebManager.Controls.cammWebManager.GetField(Page, "cammWebManager", Nothing), cammWebManager)
                            If _webmanager Is Nothing Then
                                HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: no CWM instance found")
                            Else
                                HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager successfull")
                            End If
                        Catch ex As Exception
                            HttpContext.Current.Trace.Warn("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: " & ex.Message & ex.StackTrace)
                        Finally
                            AlreadyTryedToLookUpCammWebManager = True
                        End Try
                    End If
                    Return _webmanager
                End Get
                Set(ByVal Value As cammWebManager)
                    _webmanager = Value
                End Set
            End Property

            Private ReadOnly Property Control() As System.Web.UI.Control Implements IControl.Control
                Get
                    Return CType(Me, System.Web.UI.Control)
                End Get
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Search for the server form in the list of parent controls
            ''' </summary>
            ''' <returns>The control of the server form if it's existant</returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	22.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function LookupParentServerForm() As System.Web.UI.HtmlControls.HtmlForm
                Static Result As System.Web.UI.HtmlControls.HtmlForm
                If Result Is Nothing Then
                    Result = Utils.LookupParentServerForm(Me)
                End If
                Return Result
            End Function

        End Class

        <System.Runtime.InteropServices.ComVisible(False)> Public Class ImageButton
            Inherits System.Web.UI.WebControls.ImageButton
            Implements IControl


            Private _webmanager As cammWebManager
            Private AlreadyTryedToLookUpCammWebManager As Boolean
            Public Property cammWebManager() As cammWebManager Implements IControl.cammWebManager
                Get
                    If Not _webmanager Is Nothing Then 'Save a few checks in following code block
                        Return _webmanager
                    End If
                    If _webmanager Is Nothing AndAlso Not Me.Page Is Nothing AndAlso GetType(Pages.Page).IsInstanceOfType(Me.Page) AndAlso Not CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation Is Nothing Then
                        _webmanager = CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation
                    End If
                    If _webmanager Is Nothing AndAlso AlreadyTryedToLookUpCammWebManager = False Then
                        Try
                            _webmanager = CType(CompuMaster.camm.WebManager.Controls.cammWebManager.GetField(Page, "cammWebManager", Nothing), cammWebManager)
                            If _webmanager Is Nothing Then
                                HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: no CWM instance found")
                            Else
                                HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager successfull")
                            End If
                        Catch ex As Exception
                            HttpContext.Current.Trace.Warn("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: " & ex.Message & ex.StackTrace)
                        Finally
                            AlreadyTryedToLookUpCammWebManager = True
                        End Try
                    End If
                    Return _webmanager
                End Get
                Set(ByVal Value As cammWebManager)
                    _webmanager = Value
                End Set
            End Property

            Private ReadOnly Property Control() As System.Web.UI.Control Implements IControl.Control
                Get
                    Return CType(Me, System.Web.UI.Control)
                End Get
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Search for the server form in the list of parent controls
            ''' </summary>
            ''' <returns>The control of the server form if it's existant</returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	22.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function LookupParentServerForm() As System.Web.UI.HtmlControls.HtmlForm
                Static Result As System.Web.UI.HtmlControls.HtmlForm
                If Result Is Nothing Then
                    Result = Utils.LookupParentServerForm(Me)
                End If
                Return Result
            End Function

        End Class

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.Controls.WebControls.Literal
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A System.Web.UI.WebControls.Literal with a property to access the camm WebManager
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[swiercz]	21.12.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <System.Runtime.InteropServices.ComVisible(False)> Public Class Literal
            Inherits System.Web.UI.WebControls.Literal
            Implements IControl

            Private _webmanager As cammWebManager
            Private AlreadyTryedToLookUpCammWebManager As Boolean
            Public Property cammWebManager() As cammWebManager Implements IControl.cammWebManager
                Get
                    If _webmanager Is Nothing AndAlso AlreadyTryedToLookUpCammWebManager = False Then
                        Try
                            _webmanager = CType(CompuMaster.camm.WebManager.Controls.cammWebManager.GetField(Page, "cammWebManager", Nothing), cammWebManager)
                            If _webmanager Is Nothing Then
                                HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: no CWM instance found")
                            Else
                                HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager successfull")
                            End If
                        Catch ex As Exception
                            HttpContext.Current.Trace.Warn("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: " & ex.Message & ex.StackTrace)
                        Finally
                            AlreadyTryedToLookUpCammWebManager = True
                        End Try
                    End If
                    Return _webmanager
                End Get
                Set(ByVal Value As cammWebManager)
                    _webmanager = Value
                End Set
            End Property

            Public ReadOnly Property Control() As System.Web.UI.Control Implements IControl.Control
                Get
                    Return CType(Me, System.Web.UI.Control)
                End Get
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Search for the server form in the list of parent controls
            ''' </summary>
            ''' <returns>The control of the server form if it's existant</returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	22.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function LookupParentServerForm() As System.Web.UI.HtmlControls.HtmlForm
                Static Result As System.Web.UI.HtmlControls.HtmlForm
                If Result Is Nothing Then
                    Result = Utils.LookupParentServerForm(Me)
                End If
                Return Result
            End Function

        End Class

        ''' <summary>
        ''' Show content based on existing or missing user authorizations
        ''' </summary>
#If NetFramework <> "1_1" Then
        <System.Runtime.InteropServices.ComVisible(False), ToolboxData("<{0}:ConditionalContent ID=""ConditionalContent"" runat=""server"" SecurityObject="""" NotSecurityObject=""""></{0}:WebManager>"), System.Web.UI.Themeable(False)> Public Class ConditionalContent
#Else
    <System.Runtime.InteropServices.ComVisible(False), ToolboxData("<{0}:ConditionalContent ID=""ConditionalContent"" runat=""server"" SecurityObject="""" NotSecurityObject=""""></{0}:WebManager>")> Public Class ConditionalContent
#End If

            Inherits Controls.Control

            Public Property SecurityObject As String
            Public Property NotSecurityObject As String

            Protected Overrides Sub OnPreRender(e As EventArgs)
                MyBase.OnPreRender(e)
                If Me.cammWebManager Is Nothing Then
                    Throw New NullReferenceException("ConditionalContent control hasn't got a reference to a camm Web-Manager instance, but it is required")
                ElseIf Trim(Me.SecurityObject) = Nothing AndAlso Trim(Me.SecurityObject) = Nothing Then
                    'both check values empty --> not allowed situation --> critical exception
                    Throw New NullReferenceException("SecurityObject or NotSecurityObject property must be not empty")
                Else
                    'either positive or negative rule must match - or both in case parameters for both rules are present
                    Dim ShowContentByPermissionRule As Boolean = False
                    If Trim(Me.SecurityObject) = Nothing OrElse (Trim(Me.SecurityObject) <> Nothing AndAlso cammWebManager.IsUserAuthorized(Me.SecurityObject)) Then
                        ShowContentByPermissionRule = True
                    End If
                    Dim ShowContentByMissingPermissionRule As Boolean = False
                    If Trim(Me.NotSecurityObject) = Nothing OrElse (Trim(Me.NotSecurityObject) <> Nothing AndAlso cammWebManager.IsUserAuthorized(Me.NotSecurityObject) = False) Then
                        ShowContentByMissingPermissionRule = True
                    End If
                    Me.Visible = ShowContentByPermissionRule And ShowContentByMissingPermissionRule
                End If
            End Sub

        End Class
    End Namespace

End Namespace
#End Region

#Region "Inactive controls"
#If Implemented Then
Namespace CompuMaster.camm.Controls

#Region "WebPropertyGrid (Reflect object data)"
#If Implemented Then
    Public Class WebPropertyGrid
        Inherits Web.UI.WebControls.DataGrid

        Public Function GetBindableObject(ByVal source As Object) As DataView
            Dim dt As New DataTable
            dt.Columns.Add("name", GetType(String))
            dt.Columns.Add("value", GetType(String))

            Dim t As Type = source.GetType
            For Each fi As Reflection.FieldInfo In t.GetFields(BindingFlags.NonPublic Or BindingFlags.Public Or BindingFlags.Instance)
                dt.Rows.Add(New Object() {fi.Name, fi.GetValue(source)})
            Next
            Return dt.DefaultView
        End Function

        Public Sub GetDataFromBindableObject(ByVal target As Object, ByVal bindableObject As DataView)
            Dim t As Type = target.GetType

            For Each r As DataRowView In bindableObject
                Dim fi As Reflection.FieldInfo = t.GetField(r("name"), BindingFlags.NonPublic Or BindingFlags.Public Or BindingFlags.Instance)
                If fi.FieldType Is GetType(String) Then
                    fi.SetValue(target, r("value"))
                ElseIf fi.FieldType Is GetType(DateTime) Then
                    fi.SetValue(target, DateTime.Parse(r("value")))
                End If
            Next
        End Sub

        Protected Overrides Sub CreateControlHierarchy(ByVal useDataSource As Boolean)

        End Sub

        Protected Overrides Sub PrepareControlHierarchy()

        End Sub
    End Class
#End If
#End Region

#Region "PageHeader"
#If Implemented Then
    Public Class PageHeader
        Inherits System.Web.UI.UserControl

#Region "Control logic"

        Protected Overrides Sub Render(ByVal Output As HtmlTextWriter)

        End Sub

#End Region

    End Class
#End If
#End Region

End Namespace
#End If
#End Region