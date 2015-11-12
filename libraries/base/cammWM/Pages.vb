Option Explicit On 
Option Strict On

Imports System.Web

Namespace CompuMaster.camm.WebManager.Pages

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
    Public Interface IPage
        Property cammWebManager() As CompuMaster.camm.WebManager.Controls.cammWebManager
        ReadOnly Property Page() As System.Web.UI.Page
    End Interface

#If NetFramework <> "1_1" Then
    Public Class MasterPage
        Inherits System.Web.UI.MasterPage

        Private _WebManager As CompuMaster.camm.WebManager.Controls.cammWebManager
        Private AlreadyTryedToLookUpCammWebManager As Boolean
        ''' <summary>
        ''' The cammWebManager instance created by a cammWebManager control on this master page or one of its parent master pages
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property cammWebManager As CompuMaster.camm.WebManager.Controls.cammWebManager
            Get
                If Not _WebManager Is Nothing Then 'Save a few checks in following code block
                    Return _WebManager
                End If
                'Look in parent master pages
                If _WebManager Is Nothing AndAlso Me.Master IsNot Nothing AndAlso GetType(MasterPage).IsInstanceOfType(Me.Master) AndAlso CType(Me.Master, MasterPage).cammWebManager IsNot Nothing Then
                    _WebManager = CType(CType(Me.Master, MasterPage).cammWebManager, CompuMaster.camm.WebManager.Controls.cammWebManager)
                End If
                'Look in parent master pages with shadowed cammWebManager property (VS2010 designer automatically inserts protected property cammWebManager even if already existing by inheriting from this master page
                If _WebManager Is Nothing AndAlso Me.Master IsNot Nothing AndAlso AlreadyTryedToLookUpCammWebManager = False Then
                    Try
                        _WebManager = CType(CompuMaster.camm.WebManager.Controls.cammWebManager.GetField(Me.Master, "cammWebManager", Nothing), Controls.cammWebManager)
                        If _WebManager Is Nothing Then
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl (MasterPage)", "Auto lookup of camm WebManager failed: no CWM instance found")
                        Else
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl (MasterPage)", "Auto lookup of camm WebManager successfull")
                        End If
                    Catch ex As Exception
                        HttpContext.Current.Trace.Warn("camm WebManager BaseControl (MasterPage)", "Auto lookup of camm WebManager failed: " & ex.Message & ex.StackTrace)
                    Finally
                        AlreadyTryedToLookUpCammWebManager = True
                    End Try
                End If
                'Look in main page - but without JIT creation there and without lookup again in master page
                If _WebManager Is Nothing AndAlso Me.Page IsNot Nothing AndAlso GetType(Page).IsInstanceOfType(Me.Page) AndAlso CType(Me.Page, Page)._WebManager IsNot Nothing Then
                    _WebManager = CType(Me.Page, Page)._WebManager
                End If
                Return _WebManager
            End Get
            Set(value As CompuMaster.camm.WebManager.Controls.cammWebManager)
                _WebManager = value
            End Set
        End Property

    End Class
#End If

#Region " Public Class Page"
    <System.Runtime.InteropServices.ComVisible(False)> Public MustInherit Class Page
        Inherits System.Web.UI.Page
        Implements IPage

#Region "ProtectedPage logic"
        Private _IsProtectedPage As Boolean
        ''' <summary>
        ''' If IsProtectedPage is enabled, the page request must be authorized before page prerender event, otherwise an exception is thrown.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsProtectedPage As Boolean
            Get
                Return _IsProtectedPage
            End Get
        End Property

        ''' <summary>
        ''' Activate the ProtectedPage logic as indicated by IsProtectedPage
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub EnableProtectedPageRequirement()
            _IsProtectedPage = True
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Throws an error when no security object has been set up
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.12.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CheckForExecutedPageAuthValidationOnPreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            If _IsProtectedPage Then
                If Me.cammWebManager Is Nothing Then
                    Throw New Exception("Page hasn't got a reference to a camm Web-Manager instance, but it is required")
                ElseIf Me.cammWebManager.SecurityObjectSuccessfullyTested = Nothing Then
                    Throw New Exception("This page must be protected by a defined security object")
                End If
            End If
        End Sub
#End Region

        ''' <summary>
        '''     The current URL as the client sees it - this also works for Cookieless and UrlRewriting scenarios
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' Beginning with ASP.NET 4, the RawUrl might not contain the script name
        ''' <list>
        ''' <item>.NET &gt;= 4: request to /test/ returns /test/</item>
        ''' <item>.NET &lt;= 2: request to /test/ returns /test/default.aspx</item>
        ''' </list>
        ''' </remarks>
        Public ReadOnly Property RawClientUrl() As String
            Get
                Return Response.ApplyAppPathModifier(Request.RawUrl)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Lookup the server form which resides on the page
        ''' </summary>
        ''' <returns>The control of the server form if it's existant</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function LookupServerForm() As System.Web.UI.HtmlControls.HtmlForm
            Static Result As System.Web.UI.HtmlControls.HtmlForm
            If Result Is Nothing Then
                Result = Utils.LookupServerForm(Me)
            End If
            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fires all page evens manually if the camm Web-Manager object has been created just on the fly because it won't do automatically since this is not a regular involved control
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.10.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private _FirePageEventsToCammWebManagerManually As Boolean

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     True to allow it (makes only sense for error pages) or False to ask the configuration setting
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	09.06.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Overridable ReadOnly Property CreationOnTheFlyAllowed() As Boolean
            Get
                Return False
            End Get
        End Property

        Private ReadOnly Property WebPage() As System.Web.UI.Page Implements IPage.Page
            Get
                Return CType(Me, System.Web.UI.Page)
            End Get
        End Property

        Friend _WebManager As CompuMaster.camm.WebManager.Controls.cammWebManager

        Friend ReadOnly Property cammWebManagerWithMasterPageLookupButWithoutJitCreation As CompuMaster.camm.WebManager.Controls.cammWebManager
            Get
#If NetFramework <> "1_1" Then
                If Not _WebManager Is Nothing Then 'Save a few checks in following code block
                    Return _WebManager
                End If
                If _WebManager Is Nothing AndAlso Me.Master IsNot Nothing AndAlso GetType(MasterPage).IsInstanceOfType(Me.Master) AndAlso CType(Me.Master, MasterPage).cammWebManager IsNot Nothing Then
                    _WebManager = CType(CType(Me.Master, MasterPage).cammWebManager, CompuMaster.camm.WebManager.Controls.cammWebManager)
                End If
                'Look in parent master pages with shadowed cammWebManager property (VS2010 designer automatically inserts protected property cammWebManager even if already existing by inheriting from this master page
                If _WebManager Is Nothing AndAlso Me.Master IsNot Nothing AndAlso AlreadyTryedToLookUpCammWebManager = False Then
                    Try
                        _WebManager = CType(CompuMaster.camm.WebManager.Controls.cammWebManager.GetField(Me.Master, "cammWebManager", Nothing), Controls.cammWebManager)
                        If _WebManager Is Nothing Then
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl (PageWithMasterPageLookupButWithoutJitCreation)", "Auto lookup of camm WebManager failed: no CWM instance found")
                        Else
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl (PageWithMasterPageLookupButWithoutJitCreation)", "Auto lookup of camm WebManager successfull")
                        End If
                    Catch ex As Exception
                        HttpContext.Current.Trace.Warn("camm WebManager BaseControl (PageWithMasterPageLookupButWithoutJitCreation)", "Auto lookup of camm WebManager failed: " & ex.Message & ex.StackTrace)
                    Finally
                        AlreadyTryedToLookUpCammWebManager = True
                    End Try
                End If
#End If
                Return _WebManager
            End Get
        End Property

        Private AlreadyTryedToLookUpCammWebManager As Boolean = False
        Private AlreadyTryedToLookUpCammWebManagerInMasterPageSubControls As Boolean = False
        'ToDo: Change in next major release into Public Property cammWebManager() As CompuMaster.camm.WebManager.controls.cammWebManager or better IWebManager
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The current instance of camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	20.11.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property cammWebManager() As CompuMaster.camm.WebManager.WMSystem
            Get
#If NetFramework <> "1_1" Then
                If Not _WebManager Is Nothing Then 'Save a few checks in following code block
                    Return _WebManager
                End If
                If _WebManager Is Nothing AndAlso Me.Master IsNot Nothing AndAlso GetType(MasterPage).IsInstanceOfType(Me.Master) AndAlso CType(Me.Master, MasterPage).cammWebManager IsNot Nothing Then
                    _WebManager = CType(CType(Me.Master, MasterPage).cammWebManager, CompuMaster.camm.WebManager.Controls.cammWebManager)
                End If
                'Look in parent master pages with shadowed cammWebManager property (VS2010 designer automatically inserts protected property cammWebManager even if already existing by inheriting from this master page
                If _WebManager Is Nothing AndAlso Me.Master IsNot Nothing AndAlso AlreadyTryedToLookUpCammWebManager = False Then
                    Try
                        _WebManager = CType(CompuMaster.camm.WebManager.Controls.cammWebManager.GetField(Me.Master, "cammWebManager", Nothing), Controls.cammWebManager)
                        If _WebManager Is Nothing Then
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl (Page)", "Auto lookup of camm WebManager failed: no CWM instance found")
                        Else
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl (Page)", "Auto lookup of camm WebManager successfull")
                        End If
                    Catch ex As Exception
                        HttpContext.Current.Trace.Warn("camm WebManager BaseControl (Page)", "Auto lookup of camm WebManager failed: " & ex.Message & ex.StackTrace)
                    Finally
                        AlreadyTryedToLookUpCammWebManager = True
                    End Try
                End If
                'Look in parent master pages in all registered controls for cammWebManager instance
                If _WebManager Is Nothing AndAlso Me.Master IsNot Nothing AndAlso AlreadyTryedToLookUpCammWebManagerInMasterPageSubControls = False Then
                    Try
                        Dim CwmFoundInSubControlChildren As CompuMaster.camm.WebManager.Controls.cammWebManager = Nothing
                        For Each control As System.Web.UI.Control In Me.Master.Controls
                            If CwmFoundInSubControlChildren Is Nothing AndAlso GetType(CompuMaster.camm.WebManager.Controls.cammWebManager).IsInstanceOfType(control) Then
                                CwmFoundInSubControlChildren = CType(control, CompuMaster.camm.WebManager.Controls.cammWebManager)
                            End If
                        Next
                        For Each control As System.Web.UI.Control In Me.Master.Controls
                            If CwmFoundInSubControlChildren Is Nothing Then
                                CwmFoundInSubControlChildren = SearchForCwmInstance(control)
                            End If
                        Next
                        If CwmFoundInSubControlChildren Is Nothing Then
                            HttpContext.Current.Trace.Warn("camm WebManager BaseControl", "Auto lookup of camm WebManager in all MasterPage controls failed: no instance of any CWM control found in control hierarchy")
                        Else
                            _WebManager = CwmFoundInSubControlChildren
                            If GetType(MasterPage).IsInstanceOfType(Me.Master) Then
                                'Update the control in masterpage to point to the right CWM instance in case that another control again has to search for the CWM instance - so the next CWM lookup in this request saves time
                                CType(Me.Master, MasterPage).cammWebManager = CwmFoundInSubControlChildren
                            End If
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager in all MasterPage controls successfull")
                        End If
                    Catch ex As Exception
                        HttpContext.Current.Trace.Warn("camm WebManager BaseControl", "Auto lookup of camm WebManager in all MasterPage controls failed: " & ex.Message & ex.StackTrace)
                    Finally
                        AlreadyTryedToLookUpCammWebManagerInMasterPageSubControls = True
                    End Try
                End If
#End If
                If _WebManager Is Nothing Then
                    If CreationOnTheFlyAllowed OrElse CompuMaster.camm.WebManager.WMSystem.Configuration.CreationOnTheFlyAllowed Then
                        'Create an instance on the fly
                        _WebManager = OnWebManagerJustInTimeCreation()
                        If Not _WebManager Is Nothing Then
                            _FirePageEventsToCammWebManagerManually = True
                            _WebManager.SetBasePage(Me)
                        End If
                    Else
                        Throw New ArgumentNullException("cammWebManager", "camm Web-Manager wasn't allowed to create its runtime engine on the fly")
                    End If
                End If
                Return _WebManager
            End Get
            Set(ByVal Value As CompuMaster.camm.WebManager.WMSystem)
                If Value Is Nothing Then
                    'keep probably existing instance and do not reset it to nothing
                    Trace.Warn("Property cammWebManager has NOT been reset to Null/Nothing as requested")
                ElseIf GetType(CompuMaster.camm.WebManager.Controls.cammWebManager).IsInstanceOfType(Value) Then
                    _WebManager = CType(Value, CompuMaster.camm.WebManager.Controls.cammWebManager)
                Else
                    Throw New ArgumentException("The camm Web-Manager instance must be an instance of type CompuMaster.camm.WebManager.Controls.cammWebManager")
                End If
            End Set
        End Property

        Private Shared Function SearchForCwmInstance(subControl As System.Web.UI.Control) As CompuMaster.camm.WebManager.Controls.cammWebManager
            For Each control As System.Web.UI.Control In subControl.Controls
                If GetType(CompuMaster.camm.WebManager.Controls.cammWebManager).IsInstanceOfType(control) Then
                    Return CType(control, CompuMaster.camm.WebManager.Controls.cammWebManager)
                End If
                Dim CwmFoundInSubControlChildren As CompuMaster.camm.WebManager.Controls.cammWebManager = Nothing
                CwmFoundInSubControlChildren = SearchForCwmInstance(control)
                If Not CwmFoundInSubControlChildren Is Nothing Then Return CwmFoundInSubControlChildren
            Next
            Return Nothing
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The property which implements the interface for IPage.cammWebManager
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	27.06.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Property WebManager() As CompuMaster.camm.WebManager.Controls.cammWebManager Implements IPage.cammWebManager
            Get
                Return _WebManager
            End Get
            Set(ByVal Value As CompuMaster.camm.WebManager.Controls.cammWebManager)
                _WebManager = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Create a camm Web-Manager instance on the fly
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.10.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function OnWebManagerJustInTimeCreation() As CompuMaster.camm.WebManager.Controls.cammWebManager
            Return New CompuMaster.camm.WebManager.Controls.cammWebManagerJIT
        End Function

        Private _SecurityObject As String
        Public Property SecurityObject() As String
            Get
                Return _WebManager.SecurityObject
            End Get
            Set(ByVal Value As String)
                _WebManager.SecurityObject = Value
            End Set
        End Property

        Private _META As Controls.META
        Public Property META() As Controls.META
            Get
                Return _META
            End Get
            Set(ByVal Value As Controls.META)
                _META = Value
            End Set
        End Property

        Private _Body As UI.HtmlControls.HtmlGenericControl
        Public Property BODY() As UI.HtmlControls.HtmlGenericControl
            Get
                Return _Body
            End Get
            Set(ByVal Value As UI.HtmlControls.HtmlGenericControl)
                _Body = Value
            End Set
        End Property

        Private _Head As UI.HtmlControls.HtmlGenericControl
        Public Property HEAD() As UI.HtmlControls.HtmlGenericControl
            Get
                Return _Head
            End Get
            Set(ByVal Value As UI.HtmlControls.HtmlGenericControl)
                _Head = Value
            End Set
        End Property

        Private Sub PageOnInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Init
            If cammWebManager Is Nothing Then 'on same time of this question, cammWebManager will be created just on the fly if possible
                Throw New ArgumentNullException("cammWebManager", "camm Web-Manager wasn't able to create its runtime engine")
            End If
            If _FirePageEventsToCammWebManagerManually Then
                cammWebManager.PageOnInit(sender, e)
            End If
            'Auto-Distribution of the camm Web-Manager instance to all controls
            InheritCwmInstance(Me.Controls)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Recursive method to assign the camm Web-Manager instance to all sub controls
        ''' </summary>
        ''' <param name="controls">The control collection which shall be tested for the CompuMaster.camm.WebManager.Controls.IControl interface</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	28.06.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub InheritCwmInstance(ByVal controls As System.Web.UI.ControlCollection)
            Dim CwmIControlType As System.Type = GetType(CompuMaster.camm.WebManager.Controls.IControl)
            For MyCounter As Integer = 0 To controls.Count - 1
                If CwmIControlType.IsInstanceOfType(controls(MyCounter)) Then
                    'Assign the CWM instance
                    CType(controls(MyCounter), CompuMaster.camm.WebManager.Controls.IControl).cammWebManager = Me.WebManager
                    'TODO IF REQUESTED: Following line should execute for every child control for serviving all IControl in normal controls, too
                    'TODO won't be applied as long not requested because it would slow down doing a full recursion - 2009-07-10 JW
                    'Call this method recursively
                    InheritCwmInstance(controls(MyCounter).Controls)
                End If
            Next
        End Sub

        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If _FirePageEventsToCammWebManagerManually Then
                cammWebManager.PageOnLoad(sender, e)
            End If
            'Body tag
            If Not BODY Is Nothing Then
                For Each MyKey As String In Me.cammWebManager.PageAdditionalBodyAttributes
                    BODY.Attributes.Add(MyKey, cammWebManager.PageAdditionalBodyAttributes.Item(MyKey))
                Next
            End If
            'Head area
            If Not HEAD Is Nothing Then
                If cammWebManager.PageAdditionalHeaders <> "" Then
                    HEAD.Controls.Add(New UI.LiteralControl(cammWebManager.PageAdditionalHeaders))
                End If
                If cammWebManager.PageTitle <> "" Then
                    HEAD.Controls.Add(Nothing)  'New UI.LiteralControl("kkkk")) '<title>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.PageTitle) & "</title>"))
                End If
                'Place controls into HEAD area
                If META Is Nothing Then
                    META = New Controls.META
                End If
                HEAD.Controls.Add(META)
            End If

        End Sub

        Private Sub PageOnPreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            If _FirePageEventsToCammWebManagerManually Then
                cammWebManager.PageOnPreRender(sender, e)
            End If
        End Sub

        ''' <summary>
        ''' Redirect a browser with an HTTP 301 return code permanently to the new address
        ''' </summary>
        ''' <param name="url">The new destination address</param>
        ''' <remarks>A regular Response.Redirect does a HTTP 302 temporary redirection which has a decrease in search engine optimization efforts regarding the link priority. You'll get a more strong link priority if you redirect e. g. from your root URL to your start page with a permanent link.</remarks>
        Friend Sub RedirectPermanently(ByVal url As String)
            Utils.RedirectPermanently(Me.Context, url)
        End Sub

        ''' <summary>
        ''' Redirect a browser with an HTTP 302 status code temporary to a new address while redirecting a crawler with a 301 return code permanently to another new address
        ''' </summary>
        ''' <param name="temporaryUrlForRegularBrowsers">The new destination address for the regular browsers which can be influenced by e. g. Session Variables and which can change from time to time, from request to request. This redirection will use the HTTP 302 status code.</param>
        ''' <param name="permanentCrawlerUrl">An url for all crawlers/robots/spiders which permanently points to this target url.  </param>
        ''' <remarks>
        ''' <para>A regular Response.Redirect does a HTTP 302 temporary redirection which has a decrease in search engine optimization efforts regarding the link priority. You'll get a more strong link priority if you redirect e. g. from your root URL to your start page with a permanent link.</para>
        ''' <para>Because sometimes its required for special workflows to redirect a browser to differnent locations based on some session data, it's often possible to redirect crawlers always to the same permanent address. In case you redirect a client permanently, it will remember the address in its local cache and not re-ask the server for this address. So you must ensure that you really can redirect the client to the same address when it has to request an url.</para>
        ''' <para>A possible solution for this problem can be to redirect a normal client-browser to several URLs depending on your own internal logic while redirecting permanently all crawlers to a fixed URL. The effect is that your link priority within the search engines' logic can be moved to the destination address.</para>
        ''' </remarks>
        Public Sub RedirectSemiPermanently(ByVal temporaryUrlForRegularBrowsers As String, ByVal permanentCrawlerUrl As String)
            If Utils.IsRequestFromCrawlerAgent(Me.Request) Then
                'Crawler redirection with 301 (permanantly)
                Utils.RedirectPermanently(Me.Context, permanentCrawlerUrl)
            Else
                'Browser redirection with 302 (temporary)
                If cammWebManager Is Nothing Then
                    Utils.RedirectTemporary(Me.Context, temporaryUrlForRegularBrowsers)
                Else
                    'With possibility to activize redirection debugging (see CWM debug levels)
                    cammWebManager.RedirectTo(temporaryUrlForRegularBrowsers)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Redirect a browser with an HTTP 302 return code (temporary) to the new address
        ''' </summary>
        ''' <param name="url">The new destination address</param>
        ''' <remarks>
        ''' <para>A regular Response.Redirect does a HTTP 302 temporary redirection which is not cached by browsers. This allows a dynamic url destination e. g. based on language properties.</para>
        ''' <para>ASP.NET 2.x as well as later versions have got a bug (confirmed by Microsoft) which incorrectly encodes a redirection URL in the redirection body. Typically it's not considered because the response header is used which contains the correctly encoded URL. But some search engines might find the wrong URL of the response body and try to lookup this wrong URL. This may lead to wrong dead links or 404 errors at your website. For this reason, the redirection has been reimplemented to correct the behaviour here.</para>
        ''' </remarks>
        Public Sub RedirectTemporary(ByVal url As String)
            Utils.RedirectTemporary(Me.Context, url)
        End Sub

        ''' <summary>
        ''' Redirect to another address by using a client-form which posts data
        ''' </summary>
        ''' <param name="url">The new destination address</param>
        ''' <param name="data">A collection of key/value pairs for the destination page</param>
        ''' <remarks>
        ''' <para>This method creates a client HTML form with hidden data fields and a JavaScript auto-post-back to send the data with POST method to a destination address.</para>
        ''' <para>Use this redirection method if the recieving page only understands form data and doesn't understand query parameters. Also use this method if you have to post a huge amount of data so that it extends the critical browser-limits of the request GET parameters.</para>
        ''' </remarks>
        Public Sub RedirectWithPostData(ByVal url As String, ByVal data As System.Collections.Specialized.NameValueCollection)
            Utils.RedirectWithPostData(Me.Context, url, data)
        End Sub

    End Class

     ''' <summary>
    '''     A protected page which requires an access validation
    ''' </summary>
    ''' <remarks>
    '''     If the security object name hasn't been set via the property cammWebManager.SecurityObject and it also hasn't been validated by performing an access check, this page will throw an exception in Render event.
    '''     The security objects "Anonymous" or "@@Anonymous" would be accepted also when they allow access to everybody. This is okay since it is ensured by the non-empty string that the developer hasn't forgotten to add a security object check to his page.
    ''' </remarks>
    <System.Runtime.InteropServices.ComVisible(False)> Public MustInherit Class ProtectedPage
        Inherits Page

        Protected Overrides Sub OnInit(e As EventArgs)
            Me.EnableProtectedPageRequirement()
            MyBase.OnInit(e)
        End Sub

    End Class
#End Region

End Namespace

#If NoCompilation Then
Namespace CompuMaster.camm.WebManager.Pages.Test
   Public Class NavigationTableContents
        Inherits Page

        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim navItems As String

            '--System_GetNavItems--
            'Cached values
            Response.Write("<h1>Auto-Cached values: System_GetNavItems</h1>")
            navItems = CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertToHtmlTable(cammWebManager.System_GetNavItems(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), Nothing, Nothing)
            Response.Write(navItems)

            'Non-cached values
            Response.Write("<h1>Non-cached values: System_GetNavItems</h1>")
            navItems = CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertToHtmlTable(cammWebManager.System_GetNavItems(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), , , , False, False), Nothing, Nothing)
            Response.Write(navItems)

            '--System_GetUserNavigationElements--
            'Cached values
            Response.Write("<h1>Auto-Cached values: System_GetUserNavigationElements</h1>")
            navItems = CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertToHtmlTable(cammWebManager.System_GetUserNavigationElements(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), Nothing, Nothing)
            Response.Write(navItems)

            'Non-cached values
            Response.Write("<h1>Non-cached values: System_GetUserNavigationElements</h1>")
            navItems = CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertToHtmlTable(cammWebManager.System_GetUserNavigationElements(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), , , , False, False), Nothing, Nothing)
            Response.Write(navItems)

        End Sub

    End Class

End Namespace
#End If