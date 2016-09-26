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

#Const DebugMode = False

Imports System.Web

Namespace CompuMaster.camm.SmartWebEditor

    Public MustInherit Class SmartWebEditorCommonBase
        Inherits SmartWebEditorBaseLevel2

#Region "Version tracking"
        Dim _LatestData As Integer
        ''' <summary>
        '''     The pages latest data (RowID)
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public Property LatestData() As Integer
            Get
                Return _LatestData
            End Get
            Set(ByVal Value As Integer)
                _LatestData = Value
            End Set
        End Property

        Dim _LatestVersion As Integer
        ''' <summary>
        '''     The pages latest version
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public Property LatestVersion() As Integer
            Get
                Return _LatestVersion
            End Get
            Set(ByVal Value As Integer)
                _LatestVersion = Value
            End Set
        End Property

        Private _CurrentVersionAvailableForReadAccess As Boolean
        Private _CurrentVersionHasChanged As Boolean
        ''' <summary>
        '''     The pages current version
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public Property CurrentVersion() As Integer
            Get
                If _CurrentVersionAvailableForReadAccess = False Then
                    Throw New InvalidOperationException("Page Load must be executed, first")
                End If
                Return CType(ViewState("WebEditorXXL.showversion"), Integer)
            End Get
            Set(ByVal Value As Integer)
                If CType(ViewState("WebEditorXXL.showversion"), Integer) <> Value Then
                    _CurrentVersionHasChanged = True
                End If
                ViewState("WebEditorXXL.showversion") = Value
            End Set
        End Property
        ''' <summary>
        '''     Only for saving the new value without a "_CurrentVersionHasChanged = True"
        ''' </summary>
        ''' <param name="value"></param>
        ''' <remarks>
        ''' </remarks>
        Private Sub CurrentVersionSetWithoutInternalChangeFlag(ByVal value As Integer)
            ViewState("WebEditorXXL.showversion") = value
        End Sub
        ''' <summary>
        '''     Indicate if the viewstate already contains an information about the version which shall be shown
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Private Function CurrentVersionNotYetDefinedByViewstate() As Boolean
            If ViewState("WebEditorXXL.showversion") Is Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
#End Region

#Region " Properties "

        ''' <summary>
        ''' Is this smart editor in one of the EditModes (editor control visible in edit or view mode)?
        ''' </summary>
        ''' <returns></returns>
        Public NotOverridable Overrides ReadOnly Property EditModeActive As Boolean
            Get
                Return Me.MainEditor.Visible
            End Get
        End Property

        ''' <summary>
        '''     Indicates the visibility of the switch to edit mode image button
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     Is needed to make this button only visible if it shall appear
        ''' </remarks>
        Public Property EditModeSwitchAvailable() As Boolean
            Get
                Return Me.ibtnSwitchToEditMode.Visible
            End Get
            Set(ByVal Value As Boolean)
                Me.ibtnSwitchToEditMode.Visible = Value
            End Set
        End Property

        Public ReadOnly Property EditorClientID As String
            Get
                Return Me.editorMain.ClientID
            End Get
        End Property

        Private _EditorID As String
        ''' <summary>
        '''     An identifier of the editor instance in the current document, by default the ClientID property value (but don't use this as ClientID for access in client scripting!)
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     This ID is required for support of multiple editor instances on the same page.
        ''' </remarks>
        Public Property EditorID() As String
            Get
                Return _EditorID
            End Get
            Set(ByVal Value As String)
                _EditorID = Value
            End Set
        End Property

        Private _NoteForEditor As String
        ''' <summary>
        ''' An optional note for the editor how to edit page content correctly (e.g. to update on which markets/servergroups)
        ''' </summary>
        ''' <returns></returns>
        Public Property NoteForEditor As String
            Get
                Return _NoteForEditor
            End Get
            Set(value As String)
                _NoteForEditor = value
            End Set
        End Property

        ''' <summary>
        '''     The edit mode as it is defined in the viewstate
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public Property EditModeAsDefinedInViewstate() As TriState
            Get
                If ViewState("WebEditorXXL.EditMode") Is Nothing Then
                    Return TriState.UseDefault
                ElseIf CType(ViewState("WebEditorXXL.EditMode"), Boolean) = False Then
                    Return TriState.False
                Else
                    Return TriState.True
                End If
            End Get
            Set(ByVal Value As TriState)
                If Value = TriState.True Then
                    ViewState("WebEditorXXL.EditMode") = True
                ElseIf Value = TriState.False Then
                    ViewState("WebEditorXXL.EditMode") = False
                Else
                    Throw New ArgumentOutOfRangeException("Value must be true or false")
                End If
            End Set
        End Property

        ''' <summary>
        '''     Is the control editable in general or not?
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public Property Enabled() As Boolean
            Get
                If ViewState Is Nothing OrElse ViewState("WebEditorXXL.Enabled") Is Nothing Then
                    Return True
                Else
                    Return CType(ViewState("WebEditorXXL.Enabled"), Boolean)
                End If
            End Get
            Set(ByVal Value As Boolean)
                ViewState("WebEditorXXL.Enabled") = Value
            End Set
        End Property

        ''' <summary>
        '''     Possible actions requested by the form front-end
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Public Enum RequestModes As Integer
            ''' <summary>
            '''     Nothing to do
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            NoActionRequested = 0
            ''' <summary>
            '''     Save changes
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            Update = 1
            ''' <summary>
            '''     Create a new version for modification
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            NewVersion = 2
            ''' <summary>
            '''     Save changes and release the whole version
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            Activation = 3
            ''' <summary>
            '''     Drop the content of the current market
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            DropCurrentMarketData = 6
            ''' <summary>
            '''     There is no version data available, but the first version shall be created now
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            CreateFirstVersion = 7
        End Enum

        Private _RequestMode As RequestModes
        ''' <summary>
        '''     Contains the requested controls mode
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public Property RequestMode() As RequestModes
            Get
                Return _RequestMode
            End Get
            Set(ByVal Value As RequestModes)
                _RequestMode = Value
            End Set
        End Property 'MyRequestMode()

        Protected ReadOnly Property IsEditVersionAvailable() As Boolean
            Get
                Return CountAvailableEditVersions() > 0
            End Get
        End Property
        ''' <summary>
        ''' Counts available edit versions
        ''' </summary>
        ''' <remarks>Based on old IsEditVersionAvailable() Code, slightly modified</remarks>
        ''' <returns></returns>
        Protected ReadOnly Property CountAvailableEditVersions() As Integer
            Get
                Dim result As Integer = 0
                Dim myDataTable As DataTable = Database.ReadAllData(Me.ContentOfServerID, DocumentID, Me.EditorID)
                If myDataTable.Rows.Count > 0 Then
                    'Find highest version
                    Dim MaxVersion As Integer = Integer.MinValue
                    For MyCounter As Integer = 0 To myDataTable.Rows.Count - 1
                        MaxVersion = System.Math.Max(MaxVersion, CType(myDataTable.Rows(MyCounter)("Version"), Integer))
                    Next
                    'If highest version is activated, this is a release version, otherwise it must be an edit version
                    Dim myRows() As DataRow
                    myRows = myDataTable.Select("version=" & MaxVersion)

                    For Each row As DataRow In myRows
                        If CType(myRows(0)("IsActive"), Boolean) = False Then
                            result += 1
                        End If
                    Next

                End If
                Return result
            End Get
        End Property

        ''' <summary>
        '''     Indicates which language shall be opened
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     <list>
        '''         <item>Value &gt;= 0: valid, selected market</item>
        '''         <item>Value = -1 (&lt;0): default initialization value to use current CWM market</item>
        '''     </list>
        ''' </remarks>
        Public Property LanguageToShow() As Integer
            Get
                If ViewState("WebEditorXXL.showlanguage") Is Nothing Then
                    If Me.MarketLookupMode = MarketLookupModes.SingleMarket Then
                        Me.LanguageToShow = 0
                    Else
                        Me.LanguageToShow = Me.cammWebManager.UI.MarketID
                    End If
                End If
                Return CType(ViewState("WebEditorXXL.showlanguage"), Integer)
            End Get
            Set(ByVal Value As Integer)
                ViewState("WebEditorXXL.showlanguage") = Value
            End Set
        End Property 'LanguageToShow()

        Private _ShowWithEditRights As Boolean = False
        ''' <summary>
        '''     Has the user authorization to change the content and does he see at least the pencil to start editing?
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public Property ShowWithEditRights() As Boolean
            Get
                Return _ShowWithEditRights
            End Get
            Set(ByVal Value As Boolean)
                _ShowWithEditRights = Value
            End Set
        End Property 'ShowInEditMode()

        Protected Enum ToolbarSettings As Integer
            None = 0
            EditEditableVersion = 1
            EditNoneEditableVersions = 2
            EditWithValidEditVersion = 3
            NoVersionAvailable = 4
        End Enum

        Private _ToolbarSetting As ToolbarSettings
        ''' <summary>
        '''     Indicates which toolbar shall be used
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Protected Property ToolbarSetting() As ToolbarSettings
            Get
                Return _ToolbarSetting
            End Get
            Set(ByVal Value As ToolbarSettings)
                _ToolbarSetting = Value
            End Set
        End Property 'ToolbarSetting()

        ''' <summary>
        '''     Creates the text value for the edit information label which gives you information
        '''     about the currently opened version and it's status
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Private ReadOnly Property HtmlCodeCurrentEditInformation() As String
            Get
                If CurrentVersion = 0 Then
                    If Me.ShowWithEditRights = False Then
                        'No version info available in this case
                        Return String.Empty
                    Else
                        If Me.MarketLookupMode = MarketLookupModes.SingleMarket Then
                            Return "Language/Market - Neutral / " & "<font color=""green"">Predefined default version</font>"
                        Else
                            If LanguageToShow < 0 Then
                                Throw New Exception("Invalid language/market " & LanguageToShow)
                            End If
                            Dim MarketName As String
                            If LanguageToShow = 0 Then
                                MarketName = "All unconfigured languages/markets"
                            Else
                                MarketName = GetWMLanguageDescriptionByLanguageID(CType(LanguageToShow, Integer))
                            End If
                            Return "Language/Market - " & MarketName & " / " & "<font color=""green"">International, predefined default version</font>"
                        End If
                    End If
                Else
                    Dim activeVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                    If activeVersion = CurrentVersion Then
                        If Me.MarketLookupMode = MarketLookupModes.SingleMarket Then
                            Return "Language/Market - Neutral / " & "<font color=""green"">Released version</font>"
                        Else
                            If LanguageToShow < 0 Then
                                Throw New Exception("Invalid language/market " & LanguageToShow)
                            End If
                            Dim MarketName As String
                            If LanguageToShow = 0 Then
                                MarketName = "All unconfigured languages/markets"
                            Else
                                MarketName = GetWMLanguageDescriptionByLanguageID(CType(LanguageToShow, Integer))
                            End If
                            Return "Language/Market - " & MarketName & " / " & "<font color=""green"">Released version</font>"
                        End If
                    ElseIf activeVersion < CurrentVersion Then
                        If Me.IsEditVersionAvailable Then
                            If Me.MarketLookupMode = MarketLookupModes.SingleMarket Then
                                Return "Language/Market - Neutral / " & "<font color=""red"">Edit version</font>"
                            Else

                                If LanguageToShow < 0 Then
                                    Throw New Exception("Invalid language/market " & LanguageToShow)
                                End If
                                Dim MarketName As String
                                If LanguageToShow = 0 Then
                                    MarketName = "All unconfigured languages/markets"
                                Else
                                    MarketName = GetWMLanguageDescriptionByLanguageID(CType(LanguageToShow, Integer))
                                End If
                                Return "Language/Market - " & MarketName & " / " & "<font color=""red"">Edit version</font>"
                            End If
                        Else
                            Throw New Exception("Error: CurrentVersion " & CurrentVersion & " is higher than ActiveVersion " & activeVersion & ", but IsEditVersionAvailable = False!")
                        End If
                    Else
                        If Me.MarketLookupMode = MarketLookupModes.SingleMarket Then
                            Return "Language/Market - Neutral / " & "<font color=""red"">Former version (V" & Me.CurrentVersion & ")</font>"
                        Else
                            If LanguageToShow < 0 Then
                                Throw New Exception("Invalid language/market " & LanguageToShow)
                            End If
                            Dim MarketName As String
                            If LanguageToShow = 0 Then
                                MarketName = "All unconfigured languages/markets"
                            Else
                                MarketName = GetWMLanguageDescriptionByLanguageID(CType(LanguageToShow, Integer))
                            End If
                            Return "Language/Market - " & MarketName & " / " & "<font color=""red"">Former version (V" & Me.CurrentVersion & ")</font>"
                        End If
                    End If
                End If
            End Get
        End Property
#End Region

#Region " Control specific methods "

        ''' <summary>
        '''     Image button SwitchToEditMode click events
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        Private Sub ibtnSwitchToEditMode_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnSwitchToEditMode.Click
            If CType(ViewState("WebEditorXXL.CurrentEditorInEditMode"), String) = "" OrElse CType(ViewState("WebEditorXXL.CurrentEditorInEditMode"), String) = Me.EditorID Then
                ViewState("WebEditorXXL.CurrentEditorInEditMode") = Me.EditorID
                editorMain.Visible = True
                Me.ibtnSwitchToEditMode.Visible = False
                EditModeAsDefinedInViewstate = TriState.True
                'Switch version to edit version!
                Me.CurrentVersion = Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                editorMain.Editable = CanEditCurrentVersion()

            End If
        End Sub

        ''' <summary>
        '''     Adds the controls components to it's form
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Protected Overrides Sub CreateChildControls()

            lblCurrentEditInformation = New System.Web.UI.WebControls.Label
            lblCurrentEditInformation.Visible = False
            Controls.Add(lblCurrentEditInformation)

            ibtnSwitchToEditMode = New System.Web.UI.WebControls.ImageButton
            ibtnSwitchToEditMode.Visible = False
            ibtnSwitchToEditMode.EnableViewState = False
            ibtnSwitchToEditMode.AlternateText = "Edit"
            If System.IO.File.Exists(Me.Page.Server.MapPath("~/system/images/editor.gif")) Then
                ibtnSwitchToEditMode.ImageUrl = "~/system/images/editor.gif"
            ElseIf System.IO.File.Exists(Me.Page.Server.MapPath("/system/images/editor.gif")) Then
                ibtnSwitchToEditMode.ImageUrl = "/system/images/editor.gif"
            Else
                ibtnSwitchToEditMode.ImageUrl = "~/system/images/editor.gif"
            End If
            ibtnSwitchToEditMode.ToolTip = "Edit"
            Controls.Add(ibtnSwitchToEditMode)

            lblViewOnlyContent = New System.Web.UI.LiteralControl()
            lblViewOnlyContent.ID = "lblViewOnlyContent"
            lblViewOnlyContent.Visible = False
            lblViewOnlyContent.EnableViewState = False
            Controls.Add(lblViewOnlyContent)

            txtHiddenActiveVersion = New System.Web.UI.WebControls.TextBox
            txtHiddenActiveVersion.Visible = False
            Controls.Add(txtHiddenActiveVersion)

            txtHiddenLastVersion = New System.Web.UI.WebControls.TextBox
            txtHiddenLastVersion.Visible = False
            Controls.Add(txtHiddenLastVersion)

            txtRequestedAction = New System.Web.UI.HtmlControls.HtmlInputHidden
            txtRequestedAction.ID = "txtNewVersion"
            txtRequestedAction.Name = "txtNewVersion"
            Controls.Add(txtRequestedAction)

            txtCurrentURL = New System.Web.UI.HtmlControls.HtmlInputHidden
            txtCurrentURL.ID = "txtCurrentURL"
            txtCurrentURL.Name = "txtCurrentURL"
            Controls.Add(txtCurrentURL)

            txtActivate = New System.Web.UI.HtmlControls.HtmlInputHidden
            txtActivate.ID = "txtActivate"
            txtActivate.Name = "txtActivate"
            Controls.Add(txtActivate)

            txtBrowseToMarketVersion = New System.Web.UI.HtmlControls.HtmlInputHidden
            txtBrowseToMarketVersion.ID = "txtRedirUrl"
            txtBrowseToMarketVersion.Name = "txtRedirUrl"
            Controls.Add(txtBrowseToMarketVersion)

            txtLastVersion = New System.Web.UI.HtmlControls.HtmlInputHidden
            txtLastVersion.ID = "txtLastVersion"
            txtLastVersion.Name = "txtLastVersion"
            Controls.Add(txtLastVersion)

            txtCurrentlyLoadedVersion = New System.Web.UI.HtmlControls.HtmlInputHidden
            txtCurrentlyLoadedVersion.ID = "txtCurrentlyLoadedVersion"
            txtCurrentlyLoadedVersion.Name = "txtCurrentlyLoadedVersion"
            Controls.Add(txtCurrentlyLoadedVersion)

            txtEditModeRequested = New System.Web.UI.HtmlControls.HtmlInputHidden
            txtEditModeRequested.ID = "txtEditMode"
            txtEditModeRequested.Name = "txtEditMode"
            Controls.Add(txtEditModeRequested)

            pnlEditorToolbar = New UI.WebControls.Panel()
            pnlEditorToolbar.Visible = False
            Controls.Add(pnlEditorToolbar)

        End Sub    'CreateChildControls()

        ''' <summary>
        '''     Remove any duplicate paths and return the new array with path values with all different meaning
        ''' </summary>
        ''' <param name="paths">Array of several virtual paths</param>
        ''' <param name="singlePath">One virtual path</param>
        ''' <returns>Array without any duplicate, virtual path meanings</returns>
        ''' <remarks>
        ''' </remarks>
        Protected Function PathsWithoutDuplicateMeaning(ByVal paths As String(), ByVal singlePath As String) As String()
            Dim Result As New ArrayList
            '1st parameter
            For MyCounter As Integer = 0 To paths.Length - 1
                Dim path As String = Utils.FullyInterpretedVirtualPath(paths(MyCounter))
                If Not Result.Contains(path) Then
                    Result.Add(path)
                End If
            Next
            '2nd parameter
            If singlePath <> Nothing Then
                Dim path As String = Utils.FullyInterpretedVirtualPath(singlePath)
                If Not Result.Contains(path) Then
                    Result.Add(path)
                End If
            End If
            'Return values
            Return CType(Result.ToArray(GetType(String)), String())
        End Function

        ''' <summary>
        '''     Detects which toolbar shall appear
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Private Sub GetToolbarSetting()
            If CurrentVersion = 0 AndAlso Me.IsEmptyInnerHtml Then
                ToolbarSetting = ToolbarSettings.NoVersionAvailable
            Else
                Dim ReleasedVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                If System.Math.Max(1, CurrentVersion) <= ReleasedVersion AndAlso Not IsEditVersionAvailable Then
                    ToolbarSetting = ToolbarSettings.EditNoneEditableVersions
                ElseIf System.Math.Max(1, CurrentVersion) <= ReleasedVersion AndAlso IsEditVersionAvailable Then
                    ToolbarSetting = ToolbarSettings.EditWithValidEditVersion
                ElseIf System.Math.Max(1, CurrentVersion) > ReleasedVersion Then
                    ToolbarSetting = ToolbarSettings.EditEditableVersion
                End If
            End If
        End Sub    'GetToolbarSetting()

        ''' <summary>
        '''     Raises the standard 404 error message as defined right in the IIS
        ''' </summary>
        ''' <param name="message">An error message with some details on the missing item</param>
        ''' <remarks>
        ''' </remarks>
        Protected Overridable Sub Raise404(ByVal message As String)

            If Not cammWebManager Is Nothing AndAlso cammWebManager.DebugLevel >= CompuMaster.camm.WebManager.WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                Throw New HttpException(404, message)
            Else
                Throw New HttpException(404, "File not found!")
            End If

            'HttpContext.Current.Response.Clear()
            'HttpContext.Current.Response.StatusCode = 404
            'HttpContext.Current.Response.End()
        End Sub    'Raise404()

        ''' <summary>
        '''     If the inner html property contains spaces, tabs, CR or LF chars only, then this will be indicated as True, otherwise there is some real text/content and this method will return False
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Private Function IsEmptyInnerHtml() As Boolean
            If Me._InnerHtml Is Nothing Then
                Return True
            Else
                For MyCounter As Integer = 0 To Me._InnerHtml.Length - 1
                    Select Case Me._InnerHtml.Chars(MyCounter)
                        Case " "c, ControlChars.Lf, ControlChars.Cr, ControlChars.Tab
                        Case Else
                            Return False
                    End Select
                Next
                Return True
            End If
        End Function

        ''' <summary>
        '''     Detect the language/market for first-time-data-creation in edit-requests
        ''' </summary>
        ''' <returns>An integer ID of the market/language</returns>
        ''' <remarks>
        ''' </remarks>
        Private Function LookupDefaultMarketBasedOnMarketLookupMode() As Integer
            Dim Result As Integer
            Select Case Me.MarketLookupMode
                Case MarketLookupModes.BestMatchingLanguage
                    Result = Me.cammWebManager.UI.MarketID
                Case MarketLookupModes.Language
                    Result = Me.cammWebManager.UI.LanguageID
                Case MarketLookupModes.Market
                    Result = Me.cammWebManager.UI.MarketID
                Case MarketLookupModes.SingleMarket
                    Result = 0
                Case Else
                    Throw New Exception("Invalid market lookup mode")
            End Select
            Return Result
        End Function

        ''' <summary>
        '''     Change the LanguageToShow variable to an existing value when the current one is not valid
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Private Sub LookupMatchingMarketDataBasedOnExistingContent()
            'Switch to another language which is there - if required only
            Dim myLangs() As Integer = Database.AvailableMarketsInData(Me.ContentOfServerID, DocumentID, Me.EditorID, Me.CurrentVersion)
            Dim Found As Boolean = False
            For MyCounter As Integer = 0 To myLangs.Length - 1
                If Me.LanguageToShow = myLangs(MyCounter) Then
                    Found = True
                    Exit For
                End If
            Next
            If Not Found Then
                'Current LanguageToShow value is invalid
                If myLangs.Length = 0 Then
                    'Default element
                    Me.LanguageToShow = Me.LookupDefaultMarketBasedOnMarketLookupMode
                Else
                    'First element
                    Me.LanguageToShow = myLangs(0)
                End If
                'FillEditor must be enforced now by setting the following flag
                _CurrentVersionHasChanged = True
            End If
        End Sub


        Private _AlternativeDataMarkets As Integer() = New Integer() {1, 10000}
        ''' <summary>
        ''' Alternative markets which might contain data if the typical market doesn't contain any released data
        ''' </summary>
        ''' <returns></returns>
        <System.ComponentModel.TypeConverter(GetType(IntegerArrayConverter))> Public Property AlternativeDataMarkets As Integer()
            Get
                Return _AlternativeDataMarkets
            End Get
            Set(value As Integer())
                _AlternativeDataMarkets = value
            End Set
        End Property


        ''' <summary>
        '''     Detect the LanguageToShow for view-only requests
        ''' </summary>
        ''' <param name="returnCode404InCaseOfMissingData">True raises a 404 error when it can't be looked up</param>
        ''' <remarks>
        ''' </remarks>
        Private Sub LookupMatchingMarketDataForReleasedContent(ByVal returnCode404InCaseOfMissingData As Boolean)
            Dim myLangs() As Integer = Database.AvailableMarketsInData(Me.ContentOfServerID, DocumentID, Me.EditorID, Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID))
            Dim IsLangExisting As Boolean = False
            Dim ErrorMessageInCaseOfMissingData As String = Nothing
            If Me.MarketLookupMode = MarketLookupModes.BestMatchingLanguage Then
                ErrorMessageInCaseOfMissingData = "No matching market version found for this file"
                For Each myLang As Integer In myLangs
                    If myLang = Me.cammWebManager.UI.MarketID Then
                        IsLangExisting = True
                        LanguageToShow = myLang
                    End If
                Next
                If Not IsLangExisting Then
                    For Each myLang As Integer In myLangs
                        If myLang = Me.cammWebManager.UI.LanguageID Then
                            IsLangExisting = True
                            LanguageToShow = myLang
                        End If
                    Next
                End If
                If Not IsLangExisting Then
                    For Each myLang As Integer In Me.AlternativeDataMarkets
                        For Each alternativeLang As Integer In AlternativeDataMarkets
                            If myLang = alternativeLang Then
                                IsLangExisting = True
                                LanguageToShow = myLang
                            End If
                        Next
                    Next
                End If
                If Not IsLangExisting Then
                    For Each myLang As Integer In Me.AlternativeDataMarkets
                        If myLang = 0 Then
                            IsLangExisting = True
                            LanguageToShow = 0
                        End If
                    Next
                End If
            ElseIf Me.MarketLookupMode = MarketLookupModes.Language Then
                ErrorMessageInCaseOfMissingData = "No language version found for for this file"
                For Each myLang As Integer In myLangs
                    If myLang = Me.cammWebManager.UI.MarketID Then
                        IsLangExisting = True
                        LanguageToShow = myLang
                    End If
                Next
                If Not IsLangExisting Then
                    For Each myLang As Integer In myLangs
                        If myLang = Me.cammWebManager.UI.LanguageID Then
                            IsLangExisting = True
                            LanguageToShow = myLang
                        End If
                    Next
                End If
                If Not IsLangExisting Then
                    For Each myLang As Integer In myLangs
                        If myLang = 0 Then
                            IsLangExisting = True
                            LanguageToShow = 0
                        End If
                    Next
                End If
            ElseIf Me.MarketLookupMode = MarketLookupModes.Market Then
                'Check if MarketLookupModeLangID is contained by the document
                ErrorMessageInCaseOfMissingData = "No market version found for for this file"
                For Each myLang As Integer In myLangs
                    If myLang = Me.cammWebManager.UI.MarketID Then
                        IsLangExisting = True
                        LanguageToShow = Me.cammWebManager.UI.MarketID
                    End If
                Next
                If Not IsLangExisting Then
                    For Each myLang As Integer In myLangs
                        If myLang = 0 Then
                            IsLangExisting = True
                            LanguageToShow = 0
                        End If
                    Next
                End If
            ElseIf Me.MarketLookupMode = MarketLookupModes.SingleMarket Then
                ErrorMessageInCaseOfMissingData = "No version available for this file"
                For Each myLang As Integer In myLangs
                    If myLang = 0 Then
                        IsLangExisting = True
                        LanguageToShow = 0
                    End If
                Next
            End If
            If Not IsLangExisting AndAlso Not Me.IsEmptyInnerHtml Then
                Throw New UseInnerHtmlException
            ElseIf Not IsLangExisting Then
                If returnCode404InCaseOfMissingData Then
                    Raise404(ErrorMessageInCaseOfMissingData)
                End If
            End If
        End Sub    'GetAllowedLanguageDefinedByUserProfile()

        Protected Function LookupParentServerFormName() As String
            Dim FormID As String = Me.LookupParentServerForm.Name
            If FormID = Nothing Then
                FormID = Me.LookupParentServerForm.ClientID
            End If
            If FormID = Nothing Then
                FormID = Replace(Me.LookupParentServerForm.UniqueID, ":", "_")
            End If
            If FormID = Nothing Then
                Throw New Exception("Empty Form-Name" & vbNewLine & "UniqueID=" & Me.LookupParentServerForm.UniqueID & vbNewLine & "ID=" & Me.LookupParentServerForm.ID & vbNewLine & "FormName=" & Me.LookupParentServerForm.Name & vbNewLine & "FormNewClientID=" & Me.LookupParentServerForm.NamingContainer.ClientID)
            End If
            Return FormID
        End Function

        ''' <summary>
        '''     Generates an encrypted url conform string 
        ''' </summary>
        ''' <param name="sourceString"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Protected Function EncryptStringForUrlUsage(ByVal sourceString As String) As String
            Dim result As String = Nothing
            Try
                Dim myCryptingEngine As CompuMaster.camm.WebManager.ICrypt = New CompuMaster.camm.WebManager.TripleDesBase64Encryption
                result = myCryptingEngine.CryptString(sourceString)
            Catch ex As Exception
                Throw New Exception("Error while trying to encrypt an url parameter. '" & ex.Message & "'")
            End Try
            Return HttpUtility.UrlEncode(result)
        End Function    'EncryptAStringForUrlUsement(ByVal sourceString As String) As String

        Protected Function GenerateImageUploadFormUrl() As String
            Dim parameters As New UploadFormParameters
            parameters.RefPath = DocumentID 'I am just using the old logic for now, it used document id for ref...
            parameters.DocumentID = DocumentID
            parameters.UploadPath = Utils.FullyInterpretedVirtualPath(Me.ImagesUploadPath)
            parameters.SecurityObject = Me.SecurityObjectEditMode
            parameters.EditorClientId = Me.editorMain.ClientID
            parameters.MaxUploadSize = Me.ImagesUploadSizeMax
            parameters.ReadOnlyDirectories = Me.ImagesReadOnly
            parameters.JavaScriptCallBackFunctionName = "pasteImageToEditor"

            Dim guid As Guid = New Guid()
            Dim guidStr As String = Guid.NewGuid().ToString().Replace("-"c, "")
            Dim memStream As New System.IO.MemoryStream()
            Dim formatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            formatter.Serialize(memStream, parameters)

            Dim base64 As String = Convert.ToBase64String(memStream.ToArray())
            Dim key As String = "WebEditorUpload_" & guidStr
            Me.cammWebManager.System_SetSessionValue(key, base64)

            Return Me.ImagesUploadFormUrl & "?key=" & key
        End Function

        Protected Function GenerateDocumentsUploadUrl() As String
            Dim parameters As New UploadFormParameters
            parameters.RefPath = DocumentID 'I am just using the old logic for now, it used document id for ref...
            parameters.DocumentID = DocumentID
            parameters.UploadPath = Utils.FullyInterpretedVirtualPath(Me.DocumentsUploadPath)
            parameters.SecurityObject = Me.SecurityObjectEditMode
            parameters.EditorClientId = Me.editorMain.ClientID
            parameters.MaxUploadSize = Me.DocumentsUploadSizeMax
            parameters.ReadOnlyDirectories = Me.DocumentsReadOnly
            parameters.JavaScriptCallBackFunctionName = "pasteDocumentToEditor"

            Dim guid As Guid = New Guid()
            Dim guidStr As String = Guid.NewGuid().ToString().Replace("-"c, "")
            Dim memStream As New System.IO.MemoryStream()
            Dim formatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            formatter.Serialize(memStream, parameters)

            Dim base64 As String = Convert.ToBase64String(memStream.ToArray())
            Dim key As String = "WebEditorUpload_" & guidStr
            Me.cammWebManager.System_SetSessionValue(key, base64)

            Return Me.DocumentsUploadFormUrl & "?key=" & key
        End Function


        ''' <summary>
        '''     Update an already existing document in given language or create a new language for the document, depends on several parameters
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Private Sub DoUpdateAction()
            If CurrentVersion <> 0 Then
                'Create a new version or update an older one
                If LanguageToShow >= 0 Then
                    SaveEditorContent(ContentOfServerID, DocumentID, Me.EditorID, LanguageToShow, editorMain.Html, cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous))
                Else
                    SaveEditorContent(ContentOfServerID, DocumentID, Me.EditorID, cammWebManager.UI.MarketID, editorMain.Html, cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous))
                End If
            End If
        End Sub

        ''' <summary>
        '''     Creates a new pages version
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Private Sub CreateANewPageVersion()
            'Check for clicked savebutton with update instructions without an already existing version in the given language
            'Case new version
            Try
                Me.Page.Application.Lock()
                Database.ClearCachedDbValues()
                Dim ReleasedVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                Dim MaxVersion As Integer = Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                If ReleasedVersion <> 0 AndAlso MaxVersion <> 0 Then
                    If MaxVersion - ReleasedVersion > 0 Then
                        'Invalid request - but sometimes, IE <= 11 do submit the form 2 times for whatever reason
                        'Throw New Exception("Invalid request (HighestVersion = " & MaxVersion & "; ActiveVersion=" & ReleasedVersion & ")")
                        'HACK: just ignore and continue loading the page as usual in edit mode --> do nothing here 
                    Else
                        If LanguageToShow >= 0 Then
                            Database.CopyEditorVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, ReleasedVersion)
                        Else
                            'Invalid request
                            Throw New Exception("LanguageToShow hasn't been initialized")
                        End If
                    End If
                End If
                If CurrentVersion = 0 Then
                    'Create the first version for this document
                    SaveEditorContent(ContentOfServerID, DocumentID, Me.EditorID, LanguageToShow, Me.InnerHtml, cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous))
                End If
                Database.ClearCachedDbValues()
            Finally
                Me.Page.Application.UnLock()
            End Try

        End Sub

        ''' <summary>
        '''     Returns the languages description in english as string
        ''' </summary>
        ''' <param name="ID">The language id whose description you want to get</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Private Function GetWMLanguageDescriptionByLanguageID(ByVal ID As Integer) As String
            Dim myDataTable As DataTable = CType(HttpContext.Current.Application("WebManager.ActiveLanguages"), DataTable)
            If myDataTable Is Nothing Then
                myDataTable = Database.ActiveMarketsInWebManager()
            End If
            For Each myDataRow As DataRow In myDataTable.Rows
                If CType(myDataRow(0), Integer) = ID Then
                    Return CType(myDataRow(1), String)
                End If
            Next
            Return Nothing
            'Returns it's description in english
        End Function    'GetWMLanguageDescriptionByLanguageID(ByVal ID As Integer)
#End Region

#Region " MultipleEditorPerPage Communications "
        ''' <summary>
        '''     Is there at least one SmartWebEditor control in edit mode?
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Private Function FindSWcmsControlsInEditModeOnThisPage() As Boolean
            Dim Result As Boolean
#If NetFrameWork = "1_1" Then
            Dim EditorControls As New ArrayList
#Else
            Dim EditorControls As New List(Of System.Web.UI.Control)
#End If
            FindSWcmsControlsOnThisPage(EditorControls, Me.Page.Controls)
            For MyCounter As Integer = 0 To EditorControls.Count - 1
                If EditorControls(MyCounter) Is Me Then
                    'do nothing
                ElseIf Me.EditModeActive = True Then
                    Result = True
                End If
            Next
            Return Result
        End Function

        ''' <summary>
        '''     Walk recursive through all children controls and collect all SmartWebEditor items
        ''' </summary>
        ''' <param name="results">An arraylist which will contain all positive matches</param>
        ''' <param name="controlsCollection">The control collection which shall be browsed</param>
        ''' <remarks>
        ''' </remarks>
#If NetFrameWork = "1_1" Then
        Private Sub FindSWcmsControlsOnThisPage(ByVal results As ArrayList, ByVal controlsCollection As System.Web.UI.ControlCollection)
#Else
        Protected Sub FindSWcmsControlsOnThisPage(ByVal results As List(Of System.Web.UI.Control), ByVal controlsCollection As System.Web.UI.ControlCollection)
#End If
            For MyCounter As Integer = 0 To controlsCollection.Count - 1
                If Me.GetType.IsInstanceOfType(controlsCollection(MyCounter)) Then
                    results.Add(controlsCollection(MyCounter))
                ElseIf controlsCollection(MyCounter).Controls.Count > 0 Then
                    FindSWcmsControlsOnThisPage(results, controlsCollection(MyCounter).Controls)
                End If
            Next
        End Sub
#End Region


        Private _RemoveServerNameFromLinks As Boolean = True
        ''' <summary>
        ''' Remove all occurances in links of the current server name, e. g. http://www.yourcompany.com
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public Property RemoveServerNameFromLinks() As Boolean
            Get
                Return _RemoveServerNameFromLinks
            End Get
            Set(ByVal Value As Boolean)
                _RemoveServerNameFromLinks = Value
            End Set
        End Property

        Private Function RemoveCurrentServerNameFromLinks(ByVal html As String) As String
            If html = Nothing Then
                Return html
            ElseIf RemoveServerNameFromLinks Then
                'Find occurances of ="http://www.yourcompany.com 
                'or =http://www.yourcompany.com 
                'and replace them by =" or = _
                Return html.Replace("=""" & Me.cammWebManager.CurrentServerInfo.ServerURL(), "=""").Replace("=" & Me.cammWebManager.CurrentServerInfo.ServerURL(), "=")
            Else
                Return html
            End If
        End Function

        Protected Friend Sub SaveEditorContent(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal marketID As Integer, ByVal content As String, ByVal modifiedBy As Long)
            Me.Database.SaveEditorContent(serverID, url, editorID, marketID, RemoveCurrentServerNameFromLinks(content), modifiedBy)
        End Sub

        Private _EnableCache As Boolean = True
        ''' <summary>
        '''     Use HttpCache to boost the performance by decreasing the number of required database queries
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public Property EnableCache() As Boolean
            Get
                Return _EnableCache
            End Get
            Set(ByVal Value As Boolean)
                _EnableCache = Value
            End Set
        End Property

        Private _InnerHtml As String
        ''' <summary>
        '''     The static inner HTML code is the default HTML when the database doesn't contain any released content
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     If there is no released content from database and this inner HTML contains whitespaces only, you'll run into a 404 HTTP error page (file not found).
        ''' </remarks>
        Public ReadOnly Property InnerHtml() As String
            Get
                Return _InnerHtml
            End Get
        End Property

        Private ReadOnly Property editorMain As IEditor
            Get
                Return Me.MainEditor
            End Get
        End Property
        Protected txtHiddenActiveVersion As System.Web.UI.WebControls.TextBox
        Protected txtHiddenLastVersion As System.Web.UI.WebControls.TextBox
        Protected txtRequestedAction As System.Web.UI.HtmlControls.HtmlInputHidden
        Protected txtCurrentURL As System.Web.UI.HtmlControls.HtmlInputHidden
        Protected txtActivate As System.Web.UI.HtmlControls.HtmlInputHidden
        Protected txtBrowseToMarketVersion As System.Web.UI.HtmlControls.HtmlInputHidden
        Protected txtLastVersion As System.Web.UI.HtmlControls.HtmlInputHidden
        Protected txtCurrentlyLoadedVersion As System.Web.UI.HtmlControls.HtmlInputHidden
        Protected txtEditModeRequested As System.Web.UI.HtmlControls.HtmlInputHidden
        Protected lblCurrentEditInformation As System.Web.UI.WebControls.Label
        Protected lblViewOnlyContent As System.Web.UI.LiteralControl
        Protected pnlEditorToolbar As System.Web.UI.WebControls.Panel
        Protected WithEvents ibtnSwitchToEditMode As System.Web.UI.WebControls.ImageButton

        ''' <summary>
        '''     The requested edit mode for the processing of this page request
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     Typically, this value is predefined to a default or will be changed by user via form
        ''' </remarks>
        Protected Property RequestedEditMode() As TriState
            Get
                If txtEditModeRequested.Value = "false" Then
                    Return TriState.False
                ElseIf txtEditModeRequested.Value = "true" Then
                    Return TriState.True
                Else
                    Return TriState.UseDefault
                End If
            End Get
            Set(ByVal Value As TriState)
                If Value = TriState.False Then
                    txtEditModeRequested.Value = "false"
                ElseIf Value = TriState.True Then
                    txtEditModeRequested.Value = "true"
                Else
                    Throw New ArgumentOutOfRangeException("Value must be true or false")
                End If
            End Set
        End Property

#Region "Page events"

        Private Sub PageOnInit(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            'Prevent a second execution
            Static AlreadyRun As Boolean
            If AlreadyRun = True Then
                Exit Sub
            Else
                AlreadyRun = True
            End If

            ' LanguageToShow = 0

            'Initialze the editors id for internal usage right now
            If Me.ID = "" Then
                Me.ID = Me.ClientID
                If Me.EditorID = "" Then
                    Me.EditorID = "Standard.Editor"
                End If
            Else
                Me.EditorID = Me.ID
            End If

            'Check for needed SecurityObjectEditMode
            RaiseEvent BeforeSecurityCheck()

            Dim IsUserAuthorized As Boolean = False
            If Me.SecurityObjectEditMode <> "" Then
                If Me.cammWebManager.IsUserAuthorized(Me.SecurityObjectEditMode, True, True) Then
                    'open the currently released version in ui language in edit mode
                    IsUserAuthorized = True
                End If
            End If
            Me.ShowWithEditRights = IsUserAuthorized

            'Checks if the control has child elements
            'if none available execute CreateChildControls()
            EnsureChildControls()

            'Enable/disable the viewstate dependent on the rights
            If Me.ShowWithEditRights Then
                'View state for controls
                Me.editorMain.EnableViewState = True
                Me.lblViewOnlyContent.EnableViewState = True
            Else
                'View state for controls
                Me.editorMain.EnableViewState = False
                Me.lblViewOnlyContent.EnableViewState = False
            End If

            'Check for sub controls - allowed subcontrols are either none or 1 literal control
            Dim ForeignControls As New ArrayList
            For MyCounter As Integer = 0 To Me.Controls.Count - 1
                Dim compareControl As UI.Control = Me.Controls(MyCounter)
                If compareControl Is editorMain OrElse
                            compareControl Is txtHiddenActiveVersion OrElse
                            compareControl Is txtHiddenLastVersion OrElse
                            compareControl Is txtRequestedAction OrElse
                            compareControl Is txtCurrentURL OrElse
                            compareControl Is txtActivate OrElse
                            compareControl Is txtBrowseToMarketVersion OrElse
                            compareControl Is txtLastVersion OrElse
                            compareControl Is txtCurrentlyLoadedVersion OrElse
                            compareControl Is txtEditModeRequested OrElse
                            compareControl Is lblCurrentEditInformation OrElse
                            compareControl Is lblViewOnlyContent OrElse
                            compareControl Is ibtnSwitchToEditMode OrElse
                            compareControl Is pnlEditorToolbar Then

                    'All is fine - this is an internally managed control
                Else
                    'This is a foreign control added by the user
                    ForeignControls.Add(compareControl)
                End If
            Next
            If ForeignControls.Count = 1 Then
                For MyCounter As Integer = 0 To ForeignControls.Count - 1
                    If Not GetType(UI.LiteralControl).IsInstanceOfType(ForeignControls(MyCounter)) Then
                        Throw New Exception("This control """ & ForeignControls(MyCounter).GetType.ToString & """ only supports literal text and no active element (runat=server)")
                    Else
                        Me._InnerHtml = CType(ForeignControls(MyCounter), UI.LiteralControl).Text
                        Me.Controls.Remove(CType(ForeignControls(MyCounter), UI.Control))
                    End If
                Next
            ElseIf ForeignControls.Count > 1 Then
                Dim strBuilder As New System.Text.StringBuilder
                For MyCounter As Integer = 0 To ForeignControls.Count - 1
                    If Not MyCounter = 0 Then strBuilder.Append(","c)
                    Dim MyControl As UI.Control = CType(ForeignControls(MyCounter), UI.Control)
                    strBuilder.Append(MyControl.GetType.ToString)
                Next
                Throw New Exception("Too many subcontrols (" & ForeignControls.Count & " elements: " & strBuilder.ToString & "), there is only allowed 1 literal control")
            Else 'ForeignControls.Count = 0
                'No controls are fine - do nothing here
            End If

            'Get the language which shall be opened if this is a view only request
            If Not ShowWithEditRights OrElse (ShowWithEditRights AndAlso Page.IsPostBack = False) Then
                'Not authorized or authorized but first page access
                If Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) <> 0 Then
                    'Check workflow for the documents opening language
                    Try
                        LookupMatchingMarketDataForReleasedContent(Not ShowWithEditRights)
                    Catch ex As UseInnerHtmlException
                        'Use inner html 
                        lblViewOnlyContent.Text = Me.InnerHtml
                        If ShowWithEditRights Then
                            Me.editorMain.Html = Me.InnerHtml
                        End If
                    Catch
                        Throw
                    End Try
                    If Me.LanguageToShow < 0 Then 'ShowWithEditRights=True is the only possibility where the LanguageToShow has kept at value -1
                        Me.LanguageToShow = Me.LookupDefaultMarketBasedOnMarketLookupMode
                    End If
                ElseIf Not Me.IsEmptyInnerHtml Then
                    'Use inner html 
                    lblViewOnlyContent.Text = Me.InnerHtml
                    If ShowWithEditRights Then
                        Me.editorMain.Html = Me.InnerHtml
                    End If
                ElseIf ShowWithEditRights AndAlso Page.IsPostBack = False Then
                    'User is authorized to edit an empty document, too
                    CurrentVersionSetWithoutInternalChangeFlag(0)
                Else
                    'In this case there is no view version available
                    Raise404("No file version available for unauthorized acces")
                End If
            End If
        End Sub

        ''' <summary>
        '''     This event will happen while Page_Init and allows application developers to define the security object name just-in-time
        ''' </summary>
        ''' <remarks>
        '''     This early execution of the security check is required to decide about required viewstate of this control's data controls. (After Page_Init, the viewstate wouldn't be loaded any more.)
        ''' </remarks>
        Public Event BeforeSecurityCheck()

        Private Sub PageOnLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'First, reset visibilities after viewstate-loading
            Me.lblViewOnlyContent.Visible = False
            Me.editorMain.Visible = False

            'CurrentVersion must be initialized before the JavaScriptRegistration method gets fired
            If Me.ShowWithEditRights Then

                'ViewMode first implementation
                If Not Me.Page.IsPostBack Then
                    'First page access
                    If EditModeAsDefinedInViewstate = TriState.UseDefault Then
                        Me.editorMain.Visible = False
                        Me.lblCurrentEditInformation.Visible = False
                        Me.lblViewOnlyContent.Visible = True
                        Me.lblViewOnlyContent.Text = "<br>" & Me.editorMain.Html
                        EditModeAsDefinedInViewstate = TriState.False
                        Me.RequestedEditMode = TriState.False
                    End If
                    If EditModeAsDefinedInViewstate = TriState.False AndAlso Me.RequestedEditMode = TriState.False Then
                        Me.editorMain.Editable = False
                        Me.ibtnSwitchToEditMode.Visible = True
                        Me.lblCurrentEditInformation.Visible = False
                        Me.lblViewOnlyContent.Visible = True
                        Me.lblViewOnlyContent.Text = "<br>" & Me.editorMain.Html
                        Me.RequestedEditMode = TriState.True
                    End If
                Else 'Me.Page.IsPostBack Then
                    'If page has been refreshed (gets fired in nearly every javascript event), set the editmode to true
                    If EditModeAsDefinedInViewstate = TriState.False AndAlso Me.RequestedEditMode = TriState.True Then
                        Me.ibtnSwitchToEditMode.Visible = True
                        editorMain.Visible = False
                        editorMain.Editable = False
                    ElseIf EditModeAsDefinedInViewstate = TriState.False AndAlso Me.RequestedEditMode = TriState.False Then
                        Me.editorMain.Editable = False
                        If CType(ViewState("WebEditorXXL.CurrentEditorInEditMode"), String) = Me.EditorID Then
                            Me.ibtnSwitchToEditMode.Visible = True
                            Me.RequestedEditMode = TriState.True
                            EditModeAsDefinedInViewstate = TriState.True
                        End If
                    ElseIf EditModeAsDefinedInViewstate = TriState.True AndAlso Me.RequestedEditMode = TriState.True Then
                        editorMain.Visible = True
                        editorMain.Editable = True
                        ibtnSwitchToEditMode.Visible = False
                    ElseIf EditModeAsDefinedInViewstate = TriState.True AndAlso Me.RequestedEditMode = TriState.False Then
                        editorMain.Visible = False
                        editorMain.Editable = False
                        If CType(ViewState("WebEditorXXL.CurrentEditorInEditMode"), String) = Me.EditorID Then
                            ibtnSwitchToEditMode.Visible = True
                        End If
                    End If
                End If

                'Version/language browsing filled the txtRedirUrl field
                If Me.Page.IsPostBack AndAlso txtBrowseToMarketVersion.Value <> "" Then
                    'Example: txtBrowseToMarketVersion.Value = "1;0"
                    Dim myDocInfo() As String = Split(txtBrowseToMarketVersion.Value, ";")
                    Dim RequestedVersion As Integer = Integer.Parse(myDocInfo(0))
                    Dim RequestedMarket As Integer = Integer.Parse(myDocInfo(1))
                    Dim ReleasedVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                    If RequestedVersion > ReleasedVersion Then
                        'Yes, it's sure that we are in an edit version, currently
                        Dim PreviouslyShownContent As String = Me.editorMain.Html
                        'Deactivated by JW until this feature is requested (but this code might be already stable) - 20060218
                        ''Save the old market data when that was the edit version and it has changed
                        'If Me.CurrentVersion > Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) AndAlso PreviouslyShownContent <> Me.Database.ReadContent(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.LanguageToShow, Me.CurrentVersion) Then
                        '    Me.Database.SaveEditorContent(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.LanguageToShow, PreviouslyShownContent, Me.cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))
                        'End If
                        editorMain.Editable = True
                    Else
                        editorMain.Editable = IsEditableWhenBrowsingVersions()
                    End If
                    CurrentVersion = RequestedVersion
                    If Not LanguageToShow = RequestedMarket Then
                        'Only when it has changed, apply the change 
                        LanguageToShow = RequestedMarket
                        'FillEditor must be enforced now by setting the following flag
                        _CurrentVersionHasChanged = True
                    End If
                    txtBrowseToMarketVersion.Value = "" 'Reset the value
                    editorMain.Visible = True
                    ibtnSwitchToEditMode.Visible = False
                End If

                If CurrentVersionNotYetDefinedByViewstate() Then
                    'View-mode release version
                    Dim ActiveVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                    If Not Me.IsEmptyInnerHtml AndAlso ActiveVersion = 0 Then
                        CurrentVersionSetWithoutInternalChangeFlag(0)
                    ElseIf ActiveVersion > 0 Then
                        Me.CurrentVersionSetWithoutInternalChangeFlag(ActiveVersion)
                    Else
                        'ActiveVersion = 0, but InnerHtml is empty
                        Throw New Exception("Unexpected decision workflow of operation")
                        'Me.CurrentVersionSetWithoutInternalChangeFlag(GetHighestAvailableEditorControlVersion(DocumentID))
                    End If
                End If
                If Not Page.IsPostBack Then
                    Me._CurrentVersionHasChanged = True
                End If
                _CurrentVersionAvailableForReadAccess = True

                'Logical data verification
                Dim myIsValidVersionRequest As Boolean = False
                If Not Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) < CurrentVersion Then
                    myIsValidVersionRequest = True
                End If
                If myIsValidVersionRequest = False Then
                    Me.cammWebManager.Log.Write("SmartWebEditor<br>There was an invalid request blocked by the security.")
                    Raise404("Invalid data found: the highest document version is lesser than the current version of this file")
                End If

                'Init of the image upload folder to get all the files who 
                'are already uploaded, the global and control dependent ones

                If Me.Page.IsPostBack Then
                    'Detects the controls internal request mode
                    CheckForRequestMode()

                    If RequestMode = RequestModes.Activation Then
                        'Activation actions = Save + Release
                        'first save
                        DoUpdateAction()
                        'then release
                        Database.ReleaseLatestVersion(Me.ContentOfServerID, DocumentID, Me.EditorID, Me.cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous))
                        'The cache data has to be refreshed
                        ClearCache()

                        Me.editorMain.Editable = CanEditCurrentVersion()
                        'This redirect is necessary because of the javascript for versionbrowsing used by the RadEditor
                    ElseIf RequestMode = RequestModes.Update Then
                        'Do update or create document in new language action will be triggered here
                        DoUpdateAction()
                        'The cache data doesn't need to be refreshed since released data hasn't changed
                    ElseIf RequestMode = RequestModes.NewVersion Then
                        ClearCache() 'The cache data has to be refreshed
                        CreateANewPageVersion()
                        ClearCache() 'The cache data has to be refreshed
                        CurrentVersion = Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                        'The cache data doesn't need to be refreshed since released data hasn't changed
                    ElseIf RequestMode = RequestModes.CreateFirstVersion Then
                        Dim firstContent As String = ""
                        If Me.InnerHtml <> Nothing Then
                            'Create 1st version from inner html
                            firstContent = Me.InnerHtml
                        End If
                        If LanguageToShow < 0 Then
                            LanguageToShow = Me.LookupDefaultMarketBasedOnMarketLookupMode
                        End If
                        If LanguageToShow >= 0 Then
                            Me.SaveEditorContent(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.LanguageToShow, firstContent, Me.cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous))
                        Else
                            Me.SaveEditorContent(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.LookupDefaultMarketBasedOnMarketLookupMode, firstContent, Me.cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous))
                        End If
                        CurrentVersion = 1
                        'The cache data doesn't need to be refreshed since released data hasn't changed
                    ElseIf RequestMode = RequestModes.DropCurrentMarketData Then
                        'Drop the current market's data
                        Me.Database.RemoveMarketFromEditVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.LanguageToShow)
                        'The cache data doesn't need to be refreshed since released data hasn't changed
                    End If

                End If
            Else
                'View mode
                _CurrentVersionAvailableForReadAccess = True
            End If

        End Sub

        Private Sub PagePreRender(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.PreRender

            'Proceeds the standard content loading for the editor control
            StandardLoading()

#If DebugMode Then
            CreateDebugOutput("PreRender Start")
#End If

            'Make the invalid edit buttons invisible
            If Me.ShowWithEditRights Then

                'Update visibility of main controls
                If Me.Enabled = False OrElse FindSWcmsControlsInEditModeOnThisPage() Then
                    Me.EditModeSwitchAvailable = False
                    Me.editorMain.Visible = False
                    Me.ibtnSwitchToEditMode.Visible = False
                    Me.lblViewOnlyContent.Visible = True
                    Me.lblViewOnlyContent.Text = "<br>" & Me.editorMain.Html
                    Me.lblCurrentEditInformation.Visible = False
                ElseIf Me.editorMain.Visible = True Then
                    Me.ibtnSwitchToEditMode.Visible = False
                    Me.lblViewOnlyContent.Visible = False
                ElseIf Me.editorMain.Visible = False Then
                    Me.ibtnSwitchToEditMode.Visible = True
                    Me.lblCurrentEditInformation.Visible = False
                    Me.lblViewOnlyContent.Visible = True
                    Me.lblViewOnlyContent.Text = "<br>" & editorMain.Html
                End If

                'Shows which version and language is currently opened
                lblCurrentEditInformation.Visible = Me.editorMain.Visible
                Me.lblCurrentEditInformation.ForeColor = System.Drawing.Color.Black
                Me.lblCurrentEditInformation.Font.Bold = True

                Dim currentEditInformation As String = HtmlCodeCurrentEditInformation

                If Trim(currentEditInformation) = Nothing Then
                    Me.lblCurrentEditInformation.Text = Nothing
                Else
                    Me.lblCurrentEditInformation.Text = currentEditInformation & "<br>"
                End If
                If Trim(Me.NoteForEditor) <> Nothing Then
                    Me.lblCurrentEditInformation.Text = Me.NoteForEditor & "<br>" & Me.lblCurrentEditInformation.Text
                End If

                'Initialize ToolbarSetting who indicates which toolbar shall be used
                GetToolbarSetting()
                PagePreRender_InitializeToolbar()

                'Prepare JavaScripts and register them if necessary
                If Me.editorMain.Editable = True Then
                    'define this temporary helper var with the version history HTML String
                    'Registers some JavaScripts needed for the editors toolbar
                    PagePreRender_JavaScriptRegistration()
                End If

            Else
                Me.editorMain.Html = Me.lblViewOnlyContent.Text 'required - lblViewOnlyContent is not shown for some reason, but EditorMain.Html is visible as textoutput !?!
#If DebugMode Then
                CreateDebugOutput("PreRender ViewMode")
                'Me.lblViewOnlyContent.TagName = "test"
                'Me.lblViewOnlyContent.ID = "lblViewOnlyContent"
                'Me.lblViewOnlyContent.Text = "<h1>innertext</h1>"

#End If
            End If

#If DebugMode Then
            CreateDebugOutput("PreRender End")
            WriteDebugOutput()
#End If

        End Sub

        Protected MustOverride Sub PagePreRender_JavaScriptRegistration()
        Protected MustOverride Sub PagePreRender_InitializeToolbar()

        Protected MustOverride Function IsEditableWhenBrowsingVersions() As Boolean

        Protected MustOverride Function CanEditCurrentVersion() As Boolean



#End Region

#Region "Debug output methods"
#If DebugMode Then
        Private _DebugOutput As New System.Text.StringBuilder
        Private Sub WriteDebugOutput()
            If Me.cammWebManager.DebugLevel < CompuMaster.camm.WebManager.WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                Exit Sub
            End If
            If _DebugOutput.Length > 0 Then
                HttpContext.Current.Response.Write("<p>RawUrl=" & HttpContext.Current.Server.HtmlEncode(HttpContext.Current.Request.RawUrl))
                HttpContext.Current.Response.Write("<table><tr>")
                HttpContext.Current.Response.Write(_DebugOutput.ToString)
                HttpContext.Current.Response.Write("</tr></table>")
            End If
        End Sub

        Private Sub WriteDebugOutputDirectly(ByVal text As String)
            If Me.cammWebManager.DebugLevel < CompuMaster.camm.WebManager.WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                Exit Sub
            End If
            HttpContext.Current.Response.Write(text)
        End Sub

        Private Sub CreateDebugOutput(ByVal stepLocation As String)
            If Me.cammWebManager.DebugLevel < CompuMaster.camm.WebManager.WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                Exit Sub
            End If
            _DebugOutput.Append("<td>")
            _DebugOutput.Append("<h4>" & stepLocation & "</h4>")
            _DebugOutput.Append("ShowInEditMode=" & Me.ShowWithEditRights & "<br>")
            _DebugOutput.Append("editorMain.Visible=" & Me.editorMain.Visible & "<br>")
            _DebugOutput.Append("txtEditModeRequested.Value=" & Me.txtEditModeRequested.Value & "<br>")
            _DebugOutput.Append("txtRequestedAction.Value=" & Me.txtRequestedAction.Value & "<br>")
            _DebugOutput.Append("RequestMode=" & Me.RequestMode.ToString & "<br>")
            _DebugOutput.Append("EditModeSwitchAvailable=" & Me.EditModeSwitchAvailable & "<br>")
            _DebugOutput.Append("EditModeActive=" & Me.EditModeActive & "<br>")
            _DebugOutput.Append("CurrentMarket=" & Me.LanguageToShow & "<br>")
            _DebugOutput.Append("CurrentVersion=" & Me.CurrentVersion & "<br>")
            _DebugOutput.Append("txtCurrentlyLoadedVersion=" & Me.txtCurrentlyLoadedVersion.Value & "<br>")
            _DebugOutput.Append("txtHiddenActiveVersion=" & Me.txtHiddenActiveVersion.Text & "<br>")
            _DebugOutput.Append("_ToolbarSetting=" & Me._ToolbarSetting & "<br>")
            _DebugOutput.Append("lblViewOnlyContent.Visible=" & Me.lblViewOnlyContent.Visible & "<br>")
            _DebugOutput.Append("lblViewOnlyContent.InnerHtmlStart=" & System.Web.HttpUtility.HtmlEncode(Mid(Me.lblViewOnlyContent.Text, 1, 20)) & "<br>")
            _DebugOutput.Append("editorMain.Visible=" & Me.editorMain.Visible & "<br>")
            _DebugOutput.Append("MarketLookupMode=" & Me.MarketLookupMode.ToString & "<br>")
            _DebugOutput.Append("</td>")
        End Sub
#End If
#End Region

#Region "Anything else "

        Public Function GetAvailableLanguagesDataTable() As DataTable
            Dim myLanguages As DataTable = Nothing
            Select Case Me.MarketLookupMode
                Case MarketLookupModes.BestMatchingLanguage, MarketLookupModes.Market
                    'Show languages and markets
                    myLanguages = Data.DataTables.GetDataTableClone(Database.ActiveMarketsInWebManager())
                    'Remove languages not available when in old or released version
                    If CurrentVersion <= Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) Then
                        RemoveLanguageItemsNotInAllowedList(myLanguages, Me.Database.AvailableMarketsInData(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.CurrentVersion))
                    End If
                Case MarketLookupModes.Language
                    'Only show languages, no markets
                    myLanguages = Data.DataTables.GetDataTableClone(Database.ActiveMarketsInWebManager(), "AlternativeLanguage IS NULL")
                    'Remove languages not available when in old or released version
                    If CurrentVersion <= Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) Then
                        RemoveLanguageItemsNotInAllowedList(myLanguages, Me.Database.AvailableMarketsInData(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.CurrentVersion))
                    End If
                Case MarketLookupModes.SingleMarket
                    'Do nothing special
                Case Else
                    Throw New Exception("Invalid market lookup mode " & Me.MarketLookupMode)
            End Select
            Return myLanguages
        End Function

        Public Structure AvailableLanguage
            Public id As Integer
            Public languageDescriptionEnglish As String
            Public available As Boolean
        End Structure

        ''' <summary>
        ''' Returns an array of available languages
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Based on code by Swiercz, extracted into this function to not mix it with HTML code</remarks>
        Public Function GetAvailableLanguages() As AvailableLanguage()
            Dim result As AvailableLanguage() = Nothing
            Dim myLanguages As DataTable = GetAvailableLanguagesDataTable()
            If Not myLanguages Is Nothing Then
                Dim myDataRows() As DataRow = Database.ReadAllData(Me.ContentOfServerID, DocumentID, Me.EditorID).Select("Version=" & CurrentVersion)
                Dim myDocumentsGivenLanguages() As Integer = Nothing
                Dim myIndex As Integer = 0

                For Each myDataRow As DataRow In myDataRows
                    ReDim Preserve myDocumentsGivenLanguages(myIndex)
                    myDocumentsGivenLanguages(myIndex) = CType(myDataRow("LanguageID"), Integer)
                    myIndex = myIndex + 1
                Next
                ReDim result(myLanguages.Rows.Count - 1)

                Dim index As Integer = 0
                For Each myDataRow As DataRow In myLanguages.Rows
                    'Detect availability of activated market in current editor data
                    Dim myIsAvailable As Boolean = False
                    If Not myDocumentsGivenLanguages Is Nothing Then
                        myIsAvailable = Array.IndexOf(myDocumentsGivenLanguages, myDataRow("ID")) > -1
                    End If

                    result(index).id = CType(myDataRow("ID"), Integer)
                    result(index).languageDescriptionEnglish = CType(myDataRow("Description_English"), String)
                    result(index).available = myIsAvailable
                    index += 1
                Next

            End If
            Return result
        End Function


        ''' <summary>
        '''     Remove all elements which are not in the allow-list and return all remaining items
        ''' </summary>
        ''' <param name="list">An array containing some elements</param>
        ''' <param name="allowedItems">Allowed values</param>
        ''' <remarks>
        ''' </remarks>
        Private Sub RemoveLanguageItemsNotInAllowedList(ByVal list As DataTable, ByVal allowedItems As Integer())
            Dim allowedLanguages As New ArrayList(allowedItems)
            For MyCounter As Integer = list.Rows.Count - 1 To 0 Step -1
                If Not allowedLanguages.Contains(list.Rows(MyCounter)("ID")) Then
                    list.Rows.RemoveAt(MyCounter)
                End If
            Next
        End Sub


        ''' <summary>
        '''     Set the editor by a given id
        ''' </summary>
        ''' <param name="serverID"></param>
        ''' <remarks>
        ''' </remarks>
        Private Sub FillEditorContent(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal marketID As Integer, ByVal version As Integer)
            Try
                Dim myContent As String = Database.ReadContent(serverID, url, editorID, marketID, version)
                editorMain.Html = myContent
            Catch ex As Exception
                Me.cammWebManager.Log.Write("SmartWebEditor<br>" & ex.ToString)
            End Try
        End Sub 'SetEditorContent(byval ID as integer)

        ''' <summary>
        '''     Gets information to decide which request mode shall be shown
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Private Sub CheckForRequestMode()

            'Lookup requested action
            If txtRequestedAction.Value = Nothing AndAlso Database.ReadAllData(Me.ContentOfServerID, DocumentID, Me.EditorID).Rows.Count = 0 Then
                RequestMode = RequestModes.CreateFirstVersion
            ElseIf txtRequestedAction.Value.ToLower = "newversion" AndAlso Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) = 0 AndAlso Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) > 0 Then
                'Special case when new version requested and editversion = 1 (=not yet released)
                RequestMode = Nothing
            ElseIf txtRequestedAction.Value.ToLower = "update" Then
                RequestMode = RequestModes.Update
            ElseIf txtRequestedAction.Value.ToLower = "dropcurrentmarket" Then
                RequestMode = RequestModes.DropCurrentMarketData
            ElseIf txtRequestedAction.Value.ToLower = "newversion" Then
                RequestMode = RequestModes.NewVersion
            ElseIf txtActivate.Value.ToLower = "activate" Then
                RequestMode = RequestModes.Activation
            ElseIf txtBrowseToMarketVersion.Value <> "" Then 'AndAlso Page.IsPostBack Then
                Throw New Exception("Unexpected workflow")
            End If

            'Reset form action fields
            txtRequestedAction.Value = Nothing
            txtActivate.Value = Nothing
            txtBrowseToMarketVersion.Value = Nothing

        End Sub

        ''' <summary>
        '''     The standard loading functionallity for this document
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Private Sub StandardLoading()

#If DebugMode Then
            CreateDebugOutput("StandardLoading Start")
#End If
            'Contains every data available for this document
            'Dim myDataTable As DataTable = myDataTable = GetAllDataForAGivenEditorControl(DocumentID, False)

            'Access Allowed by security 
            Try
                If Me.ShowWithEditRights Then
                    'Load edit mode
                    'Ensure that LanguageToShow contains a valid value also when this page runs with edit rights
                    'Define some needed vars
                    Dim myCurrentLanguage As Integer = 0
                    Dim myCurrentVersion As Integer = 0
                    Dim MyActiveVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                    'Check for parameters passed by url
                    If CurrentVersion <> 0 Then
                        myCurrentVersion = CurrentVersion
                    ElseIf editorMain.Visible Then
                        myCurrentVersion = Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                        CurrentVersion = myCurrentVersion
                    End If
                    If LanguageToShow >= 0 Then
                        myCurrentLanguage = LanguageToShow
                    End If
                    'Check if formular was redirected to itself
                    If myCurrentVersion = 0 AndAlso Me.IsEmptyInnerHtml = False Then
                        'No active/released version available, but inner html present
                        Me.editorMain.Html = Me.InnerHtml
                    ElseIf myCurrentLanguage = 0 AndAlso myCurrentVersion = 0 AndAlso MyActiveVersion <> 0 Then
                        'Formular wasn't redirected and shall be opened in it's currently released version in UIMarket Language
                        If _CurrentVersionHasChanged Then
                            FillEditorContent(Me.ContentOfServerID, DocumentID, Me.EditorID, cammWebManager.UI.MarketID, MyActiveVersion)
                        End If
                    Else
                        'Formular was redirected and shall be opened by the given parameters
                        'Use the informations passed by querystring to open the correct version
                        If _CurrentVersionHasChanged Then
                            FillEditorContent(Me.ContentOfServerID, DocumentID, Me.EditorID, myCurrentLanguage, myCurrentVersion)
                        End If
                    End If
                Else 'Without EditRights
                    'Load view only mode
                    lblViewOnlyContent.Visible = True
                    editorMain.Visible = False
                    ibtnSwitchToEditMode.Visible = False
                    If Me.EnableCache AndAlso Not CachedItemContent Is Nothing Then
                        lblViewOnlyContent.Text = CachedItemContent
#If DebugMode Then
                        WriteDebugOutputDirectly("cached-item-content:non-queried ")
#End If
                    Else
#If DebugMode Then
                        WriteDebugOutputDirectly("non-cached-item-content:queried ")
#End If
                        'ToDo 4 JW: optimize code to just read released content and no version number, innerhtml only when db-content has no released data
                        Try
                            Dim HighestAvailableVersion As Integer = Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                            If HighestAvailableVersion = 0 OrElse (HighestAvailableVersion = 1 AndAlso Not Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) = 1) Then
                                lblViewOnlyContent.Text = Me.InnerHtml
                            Else
                                If Me.MarketLookupMode = MarketLookupModes.SingleMarket Then
                                    lblViewOnlyContent.Text = Database.ReadReleasedContent(Me.ContentOfServerID, DocumentID, Me.EditorID, 0)
                                Else
                                    Try
                                        LookupMatchingMarketDataForReleasedContent(True)
                                    Catch ex As UseInnerHtmlException
                                        'Use inner html 
                                        lblViewOnlyContent.Text = Me.InnerHtml
                                    Catch
                                        Throw
                                    End Try
                                    lblViewOnlyContent.Text = Database.ReadReleasedContent(Me.ContentOfServerID, DocumentID, Me.EditorID, Me.LanguageToShow)
                                End If
                            End If
                        Catch ex As UseInnerHtmlException
#If DebugMode Then
                            WriteDebugOutputDirectly("with inner html ")
#End If
                            lblViewOnlyContent.Text = Me.InnerHtml
                        End Try
                        CachedItemContent = lblViewOnlyContent.Text
#If DebugMode Then
                        WriteDebugOutputDirectly("queried ")
#End If
                    End If
                End If
#If DebugMode Then
                WriteDebugOutputDirectly("innherhtml=" & Me.InnerHtml & "<br>")
                WriteDebugOutputDirectly("currentversion=" & Me.CurrentVersion & "<br>")
#End If
            Catch ex As Exception
                Me.cammWebManager.Log.ReportErrorViaEMail(ex, "SmartWebEditor")
                Me.cammWebManager.Log.Write("SmartWebEditor:<br>" & ex.ToString)
            End Try

#If DebugMode Then
            CreateDebugOutput("StandardLoading End")
#End If

        End Sub
#End Region

#Region "UseInnerHtmlException"
        Private Class UseInnerHtmlException
            Inherits Exception

        End Class
#End Region

#Region " Caching of released data"
        ''' <summary>
        '''     This is the name of the key for the content for this editor in the HttpCache
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Private ReadOnly Property CachedItemKey() As String
            Get
                Return Me.GetType.ToString & "&" & HttpContext.Current.Server.UrlEncode(Me.DocumentID) & "&" & HttpContext.Current.Server.UrlEncode(Me.EditorID) & "&" & LanguageToShow.ToString
            End Get
        End Property

        ''' <summary>
        '''     Clear all keys related to this document (all languages/markets)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Private Sub ClearCache()
            Dim SearchCacheKeys As String = Me.GetType.ToString & "&" & HttpContext.Current.Server.UrlEncode(Me.DocumentID) & "&" & HttpContext.Current.Server.UrlEncode(Me.EditorID) & "&"
            For Each item As Collections.DictionaryEntry In HttpContext.Current.Cache
                Dim Key As String = CType(item.Key, String)
                If Key.GetType Is GetType(String) AndAlso CType(Key, String).StartsWith(SearchCacheKeys) Then
                    HttpContext.Current.Cache.Remove(Key)
                End If
            Next
        End Sub

        ''' <summary>
        '''     The value of the cached, released content or null (Nothing in VisualBasic) when it's not cached yet
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Private Property CachedItemContent() As String
            Get
                Return CType(HttpContext.Current.Cache(CachedItemKey), String)
            End Get
            Set(ByVal Value As String)
                Utils.SetHttpCacheValue(CachedItemKey, Value, Me.CachedItemLivetime, Me.CachedItemPriority)
            End Set
        End Property

        ''' <summary>
        '''     The default cache duration takes 15 minutes at maximum
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Protected Overridable ReadOnly Property CachedItemLivetime() As TimeSpan
            Get
                Return New TimeSpan(0, 15, 0) '15 minutes
            End Get
        End Property

        ''' <summary>
        '''     The default cache priority is set to low to allow remval of this cache item at first
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Protected Overridable ReadOnly Property CachedItemPriority() As Caching.CacheItemPriority
            Get
                Return Caching.CacheItemPriority.Low
            End Get
        End Property

#End Region

    End Class

End Namespace