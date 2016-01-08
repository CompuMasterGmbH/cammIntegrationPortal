Option Explicit On
Option Strict Off

Imports System.Web

Namespace CompuMaster.camm.SmartWebEditor

    Namespace Controls

        ''' <summary>
        '''     The smart and built-in content management system of camm Web-Manager
        ''' </summary>
        ''' <remarks>
        '''     This page contains a web editor which saves/load the content to/from the CWM database. The editor will only be visible for those people with appropriate authorization. All other people will only see the content, but nothing to modify it.
        '''     The content may be different for languages or markets. In all cases, there will be a version history.
        '''     When there is no content for an URL in the database - or it hasn't been released - the page request will lead to an HTTP 404 error code.
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	29.11.2005	Created
        ''' </history>
        Public Class SmartPlainHtmlEditor
            Inherits SmartWcmsEditorCommonBase

            Protected Overrides Sub PagePreRender_JavaScriptRegistration()
                Return
            End Sub

            Private editorMain As IEditor

            Protected Overrides ReadOnly Property MainEditor As IEditor
                Get
                    Return Me.editorMain
                End Get
            End Property

            Public Property CssWidth As String
                Get
                    Return Me.editorMain.CssWidth
                End Get
                Set(value As String)
                    Me.editorMain.CssWidth = value
                End Set
            End Property

            Public Property CssHeight As String
                Get
                    Return Me.editorMain.CssHeight
                End Get
                Set(value As String)
                    Me.editorMain.CssHeight = value
                End Set
            End Property

            Public Property Columns As Integer
                Get
                    Return Me.editorMain.TextareaColumns
                End Get
                Set(value As Integer)
                    Me.editorMain.TextareaColumns = value
                End Set
            End Property

            Public Property Rows As Integer
                Get
                    Return Me.editorMain.TextareaRows
                End Get
                Set(value As Integer)
                    Me.editorMain.TextareaRows = value
                End Set
            End Property


            Public Sub New()
                editorMain = New PlainTextEditor
                editorMain.Editable = True
                editorMain.EnableViewState = False

            End Sub
            Protected Overrides Sub CreateChildControls()
                MyBase.CreateChildControls()

                Controls.Add(editorMain)
            End Sub

            Private SaveButton As System.Web.UI.WebControls.Button = Nothing
            Private ActivateButton As System.Web.UI.WebControls.Button = Nothing
            Private PreviewButton As System.Web.UI.WebControls.Button = Nothing
            Private NewVersionButton As System.Web.UI.WebControls.Button = Nothing
            Private DeleteLanguageButton As System.Web.UI.WebControls.Button = Nothing

            Private VersionDropDownBox As New System.Web.UI.WebControls.DropDownList
            Private LanguagesDropDownBox As System.Web.UI.WebControls.DropDownList

            Private Sub CreateToolBarButtons()
                Me.SaveButton = New System.Web.UI.WebControls.Button()
                Me.SaveButton.OnClientClick = "document.getElementById('" & Me.txtRequestedAction.ClientID & "').value = 'update'; document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();"
                Me.SaveButton.Text = "Save"


                Me.ActivateButton = New System.Web.UI.WebControls.Button
                Me.ActivateButton.OnClientClick = "document.getElementById('" & Me.txtActivate.ClientID & "').value = 'activate'; document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();"
                Me.ActivateButton.Text = "Activate"

                Me.PreviewButton = New System.Web.UI.WebControls.Button()
                Me.PreviewButton.OnClientClick = "document.getElementById('" & Me.txtEditModeRequested.ClientID & "').value = 'false'; document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();"
                Me.PreviewButton.Text = "Preview"

                Me.NewVersionButton = New System.Web.UI.WebControls.Button()
                Me.NewVersionButton.OnClientClick = "document.getElementById('" & Me.txtRequestedAction.ClientID & "').value = 'newversion'; document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();"
                Me.NewVersionButton.Text = "New Version"
            End Sub

            ''' <summary>
            ''' Fil the verison dropdown box with the versions available for this document.
            ''' <remarks>I have for now overall copied the code from the original smartwcms editor. Needs refactoring </remarks>
            ''' </summary>
            Private Sub InitializeVersionDropDownBox()
                Dim myDataTable As DataTable = Database.ReadAllData(Me.ContentOfServerID, DocumentID, Me.EditorID)
                Dim counter As Integer = 1
                Dim versionRows As DataRow() = myDataTable.Select("LanguageID = " & Me.LanguageToShow.ToString(), "Version DESC")
                For Each row As DataRow In versionRows
                    If counter = 7 Then
                        Exit For
                    End If
                    Dim version As Integer = CType(row("Version"), Integer)
                    Dim myDate As Date = Utils.Nz(row("ReleasedOn"), CType(Nothing, Date))
                    Dim ReleaseDate As String
                    If myDate = Nothing Then
                        ReleaseDate = "(no release)"
                    Else
                        ReleaseDate = myDate.ToShortDateString()
                    End If
                    Dim listItem As New System.Web.UI.WebControls.ListItem
                    listItem.Text = "v" & version & " " & ReleaseDate
                    listItem.Value = version.ToString() & ";" & Me.LanguageToShow.ToString()
                    Me.VersionDropDownBox.Items.Insert(0, listItem)
                    counter += 1

                Next
                Dim dropDownScript As String = "document.getElementById('" & txtBrowseToMarketVersion.ClientID & "').value = this.value;"
                Me.VersionDropDownBox.Attributes.Add("onchange", dropDownScript & "document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();")

                Me.VersionDropDownBox.SelectedValue = Me.CurrentVersion & ";" & LanguageToShow.ToString()
            End Sub

            ''' <summary>
            ''' Fill the languages/market drop down list...
            ''' </summary>
            Private Sub InitializeLanguageDropDownList()
                Me.LanguagesDropDownBox = New System.Web.UI.WebControls.DropDownList
                Dim neutralItem As New System.Web.UI.WebControls.ListItem
                neutralItem.Text = "0 / Neutral / All"
                neutralItem.Value = CurrentVersion.ToString() & ";0"
                LanguagesDropDownBox.Items.Add(neutralItem)

                Dim languages As AvailableLanguage() = GetAvailableLanguages()
                If Not languages Is Nothing Then
                    For Each language As AvailableLanguage In languages
                        Dim text As String = language.id.ToString() & " / " & language.languageDescriptionEnglish
                        Dim value As String = CurrentVersion.ToString() & ";" & language.id.ToString()
                        Dim item As New System.Web.UI.WebControls.ListItem
                        If Not language.available Then
                            text &= " (inactive)"
                        End If
                        item.Text = text
                        item.Value = value
                        LanguagesDropDownBox.Items.Add(item)
                    Next
                    LanguagesDropDownBox.SelectedValue = CurrentVersion.ToString() & ";" & LanguageToShow.ToString()
                End If

                Dim script As String = "document.getElementById('" & txtBrowseToMarketVersion.ClientID & "').value = this.value;"
                LanguagesDropDownBox.Attributes.Add("onchange", script & "document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();")
            End Sub
            ''' <summary>
            ''' Shows the buttons that we need
            ''' </summary>
            ''' <param name="toolbartype"></param>
            Private Sub AssembleToolbar(ByVal toolbartype As ToolbarSettings)
                If toolbartype = ToolbarSettings.EditEditableVersion Then
                    Me.pnlEditorToolbar.Controls.Add(SaveButton)
                    Me.pnlEditorToolbar.Controls.Add(ActivateButton)
                    Me.pnlEditorToolbar.Controls.Add(PreviewButton)
                    Me.pnlEditorToolbar.Controls.Add(VersionDropDownBox)
                ElseIf toolbartype = ToolbarSettings.EditNoneEditableVersions Then
                    Me.pnlEditorToolbar.Controls.Add(NewVersionButton)
                    Me.pnlEditorToolbar.Controls.Add(PreviewButton)
                    Me.pnlEditorToolbar.Controls.Add(VersionDropDownBox)
                ElseIf toolbartype = ToolbarSettings.EditWithValidEditVersion Then
                    Me.pnlEditorToolbar.Controls.Add(PreviewButton)
                    Me.pnlEditorToolbar.Controls.Add(VersionDropDownBox)
                End If
                If Me.MarketLookupMode <> MarketLookupModes.SingleMarket Then
                    Me.DeleteLanguageButton = New UI.WebControls.Button()
                    Me.DeleteLanguageButton.Text = "Delete unsaved data for this market"
                    Me.DeleteLanguageButton.OnClientClick = "document.getElementById('" & Me.txtRequestedAction.ClientID & "').value = 'dropcurrentmarket'; document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();"
                    InitializeLanguageDropDownList()
                    Me.pnlEditorToolbar.Controls.Add(Me.LanguagesDropDownBox)
                    If Me.CountAvailableEditVersions() > 1 AndAlso Me.pnlEditorToolbar.Controls.Contains(SaveButton) Then
                        Me.pnlEditorToolbar.Controls.Add(Me.DeleteLanguageButton)
                    End If
                End If
            End Sub
            Private Sub SetToolbar(ByVal toolbar As ToolbarSettings)
                CreateToolBarButtons()
                InitializeVersionDropDownBox()

                AssembleToolbar(toolbar)




            End Sub

            Protected Overrides Sub PagePreRender_InitializeToolbar()
                Dim label As System.Web.UI.WebControls.Label = New System.Web.UI.WebControls.Label()

                If Me.editorMain.Visible Then
                    SetToolbar(Me.ToolbarSetting)
                End If


                If Me.ToolbarSetting = ToolbarSettings.NoVersionAvailable Then
                    label.Text = "Non versions available"
                    Me.pnlEditorToolbar.Controls.Add(label)
                End If

                Me.pnlEditorToolbar.Visible = Me.editorMain.Visible
            End Sub

        End Class

    End Namespace

End Namespace