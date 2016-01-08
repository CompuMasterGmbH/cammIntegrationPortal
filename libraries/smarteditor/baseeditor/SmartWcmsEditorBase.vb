Option Explicit On 
Option Strict On

Imports System.Web

Namespace CompuMaster.camm.SmartWebEditor

    ''' <summary>
    '''     A base implementation of a smart wcms editor control providing access to the database acces layer
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	18.02.2006	Created
    ''' </history>
    Public MustInherit Class SmartWcmsEditorBase
        Inherits CompuMaster.camm.WebManager.Controls.Control
        Implements UI.INamingContainer, ISmartWcmsEditor

        Public ReadOnly Property Configuration() As Configuration
            Get
                Static _Configuration As Configuration
                If _Configuration Is Nothing Then _Configuration = New Configuration
                Return _Configuration
            End Get
        End Property

        ''' <summary>
        ''' The editor control to display or edit the content
        ''' </summary>
        ''' <returns></returns>
        Protected MustOverride ReadOnly Property MainEditor As IEditor

#Region " Database methods "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Database access layer
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	17.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property Database() As SmartWcmsDatabaseAccessLayer
            Get
                Static _Database As SmartWcmsDatabaseAccessLayer
                If _Database Is Nothing Then
                    _Database = New SmartWcmsDatabaseAccessLayer(Me)
                End If
                Return _Database
            End Get
        End Property

#End Region

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The interface implementation required for the database access layer
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private ReadOnly Property _cammWebManager() As CompuMaster.camm.WebManager.IWebManager Implements ISmartWcmsEditor.cammWebManager
            Get
                Return Me.cammWebManager
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Is this editor in edit mode?
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[swiercz]	23.12.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public MustOverride ReadOnly Property EditModeActive() As Boolean

#Region "Properties"

        Private _DocumentID As String
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     An identifier of the current document, by default its URL
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	23.12.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DocumentID() As String
            Get
                If _DocumentID Is Nothing Then
                    'Needs to be initialized before the preperation of the VersionHistory HTML String gets started
                    Dim rawUrlWithScriptName As String
                    If System.Environment.Version.Major >= 4 AndAlso HttpContext.Current.Request.RawUrl.EndsWith("/") Then
                        'Beginning with .NET 4, RawUrl contains the URL as requested by the client, so the script name after a folder might be missing; e.g. /test/ is given, but required is /test/default.aspx later on
                        rawUrlWithScriptName = HttpContext.Current.Request.Url.AbsolutePath
                    Else
                        '.NET 1 + 2: RawUrl contains the URL as requested by the client + the request script name, so the script name after a folder is present; e.g. /test/ is given, RawUrl returns the expected /test/default.aspx
                        rawUrlWithScriptName = HttpContext.Current.Request.RawUrl
                    End If
                    If rawUrlWithScriptName.IndexOf("?") >= 0 Then
                        _DocumentID = rawUrlWithScriptName.ToLower.Substring(0, rawUrlWithScriptName.IndexOf("?"))
                    Else
                        _DocumentID = rawUrlWithScriptName.ToLower
                    End If
                End If
                Return _DocumentID
            End Get
            Set(ByVal Value As String)
                _DocumentID = Value
            End Set
        End Property

        Private _ServerID As Integer
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Regulary, content is always related to the current server, only. In some special cases, you might want to override this to show content from another server.
        ''' </summary>
        ''' <value>The ID value of the server to whome the content is related</value>
        ''' <remarks>
        '''     By default, the address (e. g.) "/content.aspx" provides different content on different servers. So, the intranet and the extranet are able to show independent content.
        '''     In some cases, you might want to override this behaviour and you want to show on the same URL the same content in the extranet as well as in the intranet. In this case, you would setup this property on the extranet server's scripts to show the content of the intranet server.
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	07.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ContentOfServerID() As Integer
            Get
                If _ServerID = Nothing Then
                    _ServerID = Me.Configuration.ContentOfServerID()
                    If _ServerID = Nothing Then
                        _ServerID = Me.cammWebManager.CurrentServerInfo.ID
                    End If
                End If
                Return _ServerID
            End Get
            Set(ByVal Value As Integer)
                _ServerID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Contains informations about how to handle the viewonly mode in different market, langs
        ''' </summary>
        ''' <remarks>2
        ''' </remarks>
        ''' <history>
        ''' 	[Swiercz]	31.10.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Enum MarketLookupModes As Integer
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Data is only available in an international version and this is valid for all languages/markets
            ''' </summary>
            ''' <remarks>
            '''     This value is the same as None, just the name is more explainable
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	02.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            SingleMarket = 0
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Data is only available in an international version and this is valid for all languages/markets
            ''' </summary>
            ''' <remarks>
            '''     This value is the same as SingleMarket, just the name is more simplified
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	02.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            None = 0
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Data is maintained for every market separately, the language markets (e. g. "English", "French", etc. are handled as a separate market)
            ''' </summary>
            ''' <remarks>
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	02.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Market = 1
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Data is maintained for every language/market separately; when there is no value for a market it will be searched for some compatible language data
            ''' </summary>
            ''' <remarks>
            '''     Example: When the visitor is in market "German/Austria" but there is only some content available for market "German", the German data will be used.
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	02.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Language = 2
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Data is maintained for every language/market separately; when there is no value for the current market, the sWCMS control tries to lookup a best matching content
            ''' </summary>
            ''' <remarks>
            '''     When the user requests a page in e. g. market 559 ("French/France"), there will be the following order for the lookup process:
            '''     <list>
            '''         <item>Current market, in ex. ID 559 / French/France</item>
            '''         <item>Current language of market, in ex. ID 3 / French</item>
            '''         <item>English universal, ID 1</item>
            '''         <item>Worldwide market, ID 10000</item>
            '''         <item>International, ID 0</item>
            '''     </list>
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	02.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            BestMatchingLanguage = 3
        End Enum

        Private _MarketLookupMode As MarketLookupModes
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Represents the current MarketLookupMode, passed as parameter by the ctrl
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Swiercz]	31.10.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property MarketLookupMode() As MarketLookupModes
            Get
                Return _MarketLookupMode
            End Get
            Set(ByVal Value As MarketLookupModes)
                _MarketLookupMode = Value
            End Set
        End Property 'MarketLookupMode()

        Private _SecurityObjectEditMode As String
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Indicates which application is needed to edit the formular
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Swiercz]	31.10.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property SecurityObjectEditMode() As String
            Get
                Return _SecurityObjectEditMode
            End Get
            Set(ByVal Value As String)
                _SecurityObjectEditMode = Value
            End Set
        End Property 'SecurityObjectEditMode()

#End Region
    End Class

End Namespace