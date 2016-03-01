'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie können es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren veröffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es nützlich sein wird, aber OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License für weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.

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


            Protected Overrides Function IsEditableWhenBrowsingVersions() As Boolean
                Return False
            End Function

            Protected Overrides Function CanEditCurrentVersion() As Boolean
                Dim releasedVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                Return Me.CurrentVersion > releasedVersion
            End Function

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
                editorMain.EnableViewState = False


            End Sub
            Protected Overrides Sub CreateChildControls()
                MyBase.CreateChildControls()

                Controls.Add(editorMain)
                Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "IsDirty", "function isDirty() {  var editor = document.getElementById('" & editorMain.ClientID & "'); if(editor &&  editor.defaultValue != editor.value) return true; else return false;  }", True)
                Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "UnbindCloseCheck", "function unbindCloseCheck() { if(window.removeEventListener) window.removeEventListener('beforeunload', closeCheck); } var documentForm = document.forms['" & Me.LookupParentServerForm.ClientID & "']; if(documentForm.addEventListener) documentForm.addEventListener('submit', function(e) { if(window.removeEventListener) window.removeEventListener('beforeunload', closeCheck); });", True)
                Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "confirmPageClose", "function confirmPageClose() {var result = false; if(isDirty()) result =  confirm('Do you want to leave? All your changes will be lost'); else { result = true } if(result) unbindCloseCheck(); return result; }" & System.Environment.NewLine, True)
                Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "ResetSelectBox", "function resetSelectBox(box){ for(var i = 0; i < box.options.length; i++) {	if(box.options[i].defaultSelected) {box.options[i].selected = true; return;	} }}" & System.Environment.NewLine, True)
                Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "CloseCheck", "function closeCheck(e) { if(!isDirty())  return; var confirmationMessage = 'Do you want to close this site without saving your changes?';  e.returnValue = confirmationMessage; return confirmationMessage;} " & vbNewLine & " if(window.addEventListener) window.addEventListener(""beforeunload"", closeCheck);", True)


            End Sub

            Protected SaveButton As System.Web.UI.WebControls.Button = Nothing
            Protected ActivateButton As System.Web.UI.WebControls.Button = Nothing
            Protected PreviewButton As System.Web.UI.WebControls.Button = Nothing
            Protected NewVersionButton As System.Web.UI.WebControls.Button = Nothing
            Protected DeleteLanguageButton As System.Web.UI.WebControls.Button = Nothing


            Protected VersionDifferenceLabel As System.Web.UI.WebControls.Label = Nothing


            Protected VersionDropDownBox As New System.Web.UI.WebControls.DropDownList
            Protected LanguagesDropDownBox As System.Web.UI.WebControls.DropDownList

            Private Sub CreateToolBarButtons()
                Me.SaveButton = New System.Web.UI.WebControls.Button()
                Me.SaveButton.OnClientClick = "document.getElementById('" & Me.txtRequestedAction.ClientID & "').value = 'update'; unbindCloseCheck(); document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();"
                Me.SaveButton.Text = "Save"


                Me.ActivateButton = New System.Web.UI.WebControls.Button
                Me.ActivateButton.OnClientClick = "document.getElementById('" & Me.txtActivate.ClientID & "').value = 'activate'; unbindCloseCheck(); document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();"
                Me.ActivateButton.Text = "Save & Activate"
                Me.ActivateButton.ToolTip = "Save & Activate all languages/markets of this version"

                Me.PreviewButton = New System.Web.UI.WebControls.Button()
                Me.PreviewButton.OnClientClick = "document.getElementById('" & Me.txtEditModeRequested.ClientID & "').value = 'false'; unbindCloseCheck(); document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();"
                Me.PreviewButton.Text = "Preview"

                Me.NewVersionButton = New System.Web.UI.WebControls.Button()
                Me.NewVersionButton.OnClientClick = "document.getElementById('" & Me.txtRequestedAction.ClientID & "').value = 'newversion'; document.getElementById('" & Me.txtEditModeRequested.ClientID & "').value = 'true';  unbindCloseCheck(); document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();"
                Me.NewVersionButton.Text = "New Version"

                Me.VersionDifferenceLabel = New System.Web.UI.WebControls.Label()
                Me.VersionDifferenceLabel.EnableViewState = False


            End Sub

            ''' <summary>
            ''' Fil the verison dropdown box with the versions available for this document.
            ''' <remarks>I have for now overall copied the code from the original smartwcms editor. Needs refactoring </remarks>
            ''' </summary>
            Private Sub InitializeVersionDropDownBox()
                Dim myDataTable As DataTable = Database.ReadAllData(Me.ContentOfServerID, DocumentID, Me.EditorID)
                Dim counter As Integer = 1
                Dim versionRows As DataRow() = myDataTable.Select("LanguageID = " & Me.LanguageToShow.ToString(), "Version DESC")
                Dim currentVersionExists As Boolean = False
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
                    listItem.Text = "v" & version.ToString() & " " & ReleaseDate
                    listItem.Value = version.ToString() & ";" & Me.LanguageToShow.ToString()
                    Me.VersionDropDownBox.Items.Insert(0, listItem)
                    counter += 1

                    If version = CurrentVersion Then
                        currentVersionExists = True
                    End If
                Next
                Dim dropDownScript As String = "document.getElementById('" & txtBrowseToMarketVersion.ClientID & "').value = this.value; "
                Me.VersionDropDownBox.Attributes.Add("onchange", "if(confirmPageClose()){ " & dropDownScript & " this.selectedIndex = -1; unbindCloseCheck(); document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();  } else resetSelectBox(this); ")

                Dim selectedValue As String = Me.CurrentVersion & ";" & LanguageToShow.ToString()

                If currentVersionExists Then
                    Me.VersionDropDownBox.SelectedValue = Me.CurrentVersion & ";" & LanguageToShow.ToString()
                Else
                    Me.VersionDropDownBox.Items.Add("(new)")
                    Me.VersionDropDownBox.SelectedValue = "(new)"
                End If

            End Sub

            ''' <summary>
            ''' Fill the languages/market drop down list...
            ''' </summary>
            Private Sub InitializeLanguageDropDownList()
                Dim languages As AvailableLanguage() = GetAvailableLanguages()
                Me.LanguagesDropDownBox = New System.Web.UI.WebControls.DropDownList
                Dim neutralItem As New System.Web.UI.WebControls.ListItem
                neutralItem.Text = "0 / Neutral / All"
                neutralItem.Value = CurrentVersion.ToString() & ";0"
                LanguagesDropDownBox.Items.Add(neutralItem)


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
                LanguagesDropDownBox.Attributes.Add("onchange", "if(confirmPageClose()){" & script & "; this.selectedIndex = -1; document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit(); } else resetSelectBox(this); ")
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
                    If Me.MarketLookupMode <> MarketLookupModes.SingleMarket Then
                        InitializeLanguageDropDownList()
                        Me.pnlEditorToolbar.Controls.Add(Me.LanguagesDropDownBox)
                    End If

                    Me.pnlEditorToolbar.Controls.Add(VersionDropDownBox)
                ElseIf toolbartype = ToolbarSettings.EditNoneEditableVersions Then
                    Me.pnlEditorToolbar.Controls.Add(NewVersionButton)
                    Me.pnlEditorToolbar.Controls.Add(PreviewButton)


                    If Me.MarketLookupMode <> MarketLookupModes.SingleMarket Then
                        InitializeLanguageDropDownList()
                        Me.pnlEditorToolbar.Controls.Add(Me.LanguagesDropDownBox)
                    End If

                    Me.pnlEditorToolbar.Controls.Add(VersionDropDownBox)


                ElseIf toolbartype = ToolbarSettings.EditWithValidEditVersion Then
                    Me.pnlEditorToolbar.Controls.Add(PreviewButton)
                    If Me.MarketLookupMode <> MarketLookupModes.SingleMarket Then
                        InitializeLanguageDropDownList()
                        Me.pnlEditorToolbar.Controls.Add(Me.LanguagesDropDownBox)
                    End If

                    Me.pnlEditorToolbar.Controls.Add(VersionDropDownBox)


                End If
                If Me.MarketLookupMode <> MarketLookupModes.SingleMarket Then
                    Me.DeleteLanguageButton = New UI.WebControls.Button()
                    Me.DeleteLanguageButton.Text = "Delete/Deactivate this market"
                    Me.DeleteLanguageButton.OnClientClick = "if(confirmPageClose()) { document.getElementById('" & Me.txtRequestedAction.ClientID & "').value = 'dropcurrentmarket'; document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit(); } return false; "


                    If Me.LanguageToShow <> 0 AndAlso Me.CountAvailableEditVersions() > 1 AndAlso Me.pnlEditorToolbar.Controls.Contains(SaveButton) Then
                        Me.pnlEditorToolbar.Controls.Add(Me.DeleteLanguageButton)
                    End If

                End If

                If toolbartype = ToolbarSettings.EditWithValidEditVersion OrElse toolbartype = ToolbarSettings.EditNoneEditableVersions Then
                    Me.pnlEditorToolbar.Controls.Add(VersionDifferenceLabel)
                End If


            End Sub
            Private Sub SetToolbar(ByVal toolbar As ToolbarSettings)
                CreateToolBarButtons()
                InitializeVersionDropDownBox()

                AssembleToolbar(toolbar)




            End Sub

            Private Sub SetVersionDifferenceLabelText()
                Dim currentVersion As Integer = Me.CurrentVersion
                Dim currentMarket As Integer = Me.LanguageToShow


                If currentVersion > 1 Then
                    Dim differentVersion As Integer = Database.GetFirstPreviousVersionThatDiffers(Me.ContentOfServerID, DocumentID, Me.EditorID, currentMarket, currentVersion)

                    'No version differs, so all are the same since the beginning
                    If differentVersion = 0 Then
                        If Me.Database.IsMarketAvailable(Me.ContentOfServerID, DocumentID, Me.EditorID, currentMarket, 1) Then
                            Me.VersionDifferenceLabel.Text = "Without changes since version v1"
                        End If

                    ElseIf differentVersion = currentVersion - 1 Then
                            Me.VersionDifferenceLabel.Text = "With changes"
                        Else
                            Dim firstSameVersion As Integer = differentVersion + 1
                        'It's possible the market was deactivated before and the version doesn't actually exist
                        If Me.Database.IsMarketAvailable(Me.ContentOfServerID, DocumentID, Me.EditorID, currentMarket, firstSameVersion) Then
                            Me.VersionDifferenceLabel.Text = "Without changes since version v" & (differentVersion + 1).ToString()
                        Else
                            Me.VersionDifferenceLabel.Text = "With changes"
                        End If

                    End If
                End If


            End Sub


            Protected Overrides Sub PagePreRender_InitializeToolbar()
                Dim label As System.Web.UI.WebControls.Label = New System.Web.UI.WebControls.Label()

                If Me.editorMain.Visible Then
                    SetToolbar(Me.ToolbarSetting)
                    SetVersionDifferenceLabelText()
                End If


                If Me.ToolbarSetting = ToolbarSettings.NoVersionAvailable Then
                    label.Text = "No versions available"
                    Me.pnlEditorToolbar.Controls.Add(label)
                End If

                Me.pnlEditorToolbar.Visible = Me.editorMain.Visible



            End Sub

        End Class

    End Namespace

End Namespace