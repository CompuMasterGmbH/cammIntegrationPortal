Option Explicit On 
Option Strict On

Namespace CompuMaster.camm.WebManager
#Region " Public Class UI "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.UserInterface
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Provide the cammWebManager.UI elements
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	01.03.2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UserInterface

        Private _webManager As CompuMaster.camm.WebManager.WMSystem
        Friend Sub New(ByVal webManager As CompuMaster.camm.WebManager.WMSystem)
            _webManager = webManager
        End Sub

        Public Function MarketID() As Integer
            Return _webManager.UIMarket(0)
        End Function

        Public Function LanguageID() As Integer
            Return _webManager.Internationalization.GetAlternativelySupportedLanguageID(MarketID)
        End Function

        Private ReadOnly Property TextModules() As CompuMaster.camm.WebManager.Modules.Text.TextModules
            Get
                Static _TextModules As CompuMaster.camm.WebManager.Modules.Text.TextModules
                If _TextModules Is Nothing Then
                    _TextModules = New CompuMaster.camm.WebManager.Modules.Text.TextModules(_webManager)
                End If
                Return _TextModules
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load a text module
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <returns>A plain text string or HTML code (defined by the variable type) with the complete value of the text</returns>
        ''' <remarks>
        '''     By default, the requested websitAreaID is empty.
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	10.01.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function TextModule(ByVal key As String) As String
            Return TextModules.Load(key)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load a text module
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <param name="websitAreaID">An array of strings with unique IDs of the website area, e. g. {"Shop", "default", ""} - the order defines the priority</param>
        ''' <returns>A plain text string or HTML code (defined by the variable type) with the complete value of the text</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	10.01.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function TextModule(ByVal key As String, ByVal websitAreaID() As String) As String
            Return TextModules.Load(key, websitAreaID)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load a text module
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <param name="websitAreaID">An array of strings with unique IDs of the website area, e. g. {"Shop", "default", ""} - the order defines the priority</param>
        ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
        ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
        ''' <returns>A plain text string or HTML code (defined by the variable type) with the complete value of the text</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	10.01.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function TextModule(ByVal key As String, ByVal websitAreaID() As String, ByVal marketID As Integer, ByVal serverGroupID As Integer) As String
            Return TextModules.Load(key, websitAreaID, marketID, serverGroupID)
        End Function

    End Class
#End Region
End Namespace