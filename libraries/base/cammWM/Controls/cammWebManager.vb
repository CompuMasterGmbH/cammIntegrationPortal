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

Namespace CompuMaster.camm.WebManager.Controls

#If NetFramework <> "1_1" Then
    <System.Runtime.InteropServices.ComVisible(False), ToolboxData("<{0}:WebManager ID=""cammWebManager"" runat=""server"" SecurityObject=""""></{0}:WebManager>"), System.Web.UI.NonVisualControl(), System.Web.UI.Themeable(False), System.Web.UI.PersistChildren(False), System.Web.UI.ParseChildren(False)> Public MustInherit Class cammWebManager
#Else
    <System.Runtime.InteropServices.ComVisible(False), ToolboxData("<{0}:WebManager ID=""cammWebManager"" runat=""server"" SecurityObject=""""></{0}:WebManager>")> Public MustInherit Class cammWebManager
#End If
        Inherits CompuMaster.camm.WebManager.WMSystem
        ''' <summary>
        '''     Creates a new instance of the camm Web-Manager
        ''' </summary>
        <Obsolete("You should not create a camm Web-Manager instance by yourself in web applications, use the cammWebManager property or cammWebManager object created in the aspx page itself")>
        Public Sub New()
            MyBase.New("")
        End Sub
        ''' <summary>
        '''     Creates a new instance of the camm Web-Manager
        ''' </summary>
        ''' <param name="databaseConnectionString">The connection string to the database</param>
        Public Sub New(ByVal databaseConnectionString As String)
            MyBase.New(databaseConnectionString)
        End Sub

        Friend Sub New(ByVal databaseConnectionString As String, ignoreCheckCompatibilityToDatabaseByBuildNumber As Boolean)
            MyBase.New(databaseConnectionString, ignoreCheckCompatibilityToDatabaseByBuildNumber)
        End Sub
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
        ''' <summary>
        '''     Load configuration (for internal purposes only)
        ''' </summary>
        Friend Overridable Sub _LoadConfiguration()
            LoadConfiguration()
            Me.IsConfigurationLoaded = True
        End Sub
        ''' <summary>
        '''     Load the configuration, mostly in /sysdata/config.vb
        ''' </summary>
        Protected MustOverride Sub LoadConfiguration()
        ''' <summary>
        '''     Load customized language/market strings (for internal purposes only)
        ''' </summary>
        ''' <param name="LanguageID">The market which shall be used</param>
        Private Sub _LoadLanguageData(ByVal LanguageID As Integer) Handles MyBase.LanguageDataLoaded
            LoadLanguageData(LanguageID)
            Me.IsLanguageDataLoaded = True
        End Sub
        ''' <summary>
        '''     Load customized language/market strings
        ''' </summary>
        ''' <param name="LanguageID">The market which shall be used</param>
        Public Overridable Sub LoadLanguageData(ByVal LanguageID As Integer)
        End Sub
        ''' <summary>
        '''     Initializes the webmanager inclusive loading of configuration and customized strings for the markets
        ''' </summary>
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
        ''' <summary>
        '''     Fires when the camm Web-Manager has been initialized
        ''' </summary>
        Public Event InitializedWebManager()

        Private Sub OnWebManagerInit() Handles MyBase.InitLoadConfiguration
            System_Init()
        End Sub
        ''' <summary>
        '''     This OnLoad method executes an automatic databinding if not disabled
        ''' </summary>
        ''' <param name="e"></param>
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
        ''' <summary>
        '''     Automatically bind data to bind cammWebManager instances to sub controls 
        ''' </summary>
        ''' <value></value>
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
        ''' <summary>
        '''     Some debug output for trace
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub cammWebManager_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            If Me.DebugLevel >= WMSystem.DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails Then
                Me.Trace.Write("camm WebManager", "DebugLevel: " & CType(Me.DebugLevel, Byte).ToString & " (" & Me.DebugLevel.ToString & ")")
            End If
        End Sub

    End Class

End Namespace