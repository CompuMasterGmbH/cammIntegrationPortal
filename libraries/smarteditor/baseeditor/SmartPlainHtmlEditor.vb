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

            Protected Overrides Sub CreateChildControls()
                MyBase.CreateChildControls()

                editorMain = New PlainTextEditor
                editorMain.Editable = True
                editorMain.EnableViewState = False

                Controls.Add(editorMain)
            End Sub

            Private Sub SetToolbar(ByVal toolbar As ToolbarSettings)
                Dim saveButton As System.Web.UI.WebControls.Button = New System.Web.UI.WebControls.Button()
                saveButton.OnClientClick = "document.getElementById('" & Me.txtRequestedAction.ClientID & "').value = 'update'; document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();"
                saveButton.Text = "Save"


                Dim activateButton As New System.Web.UI.WebControls.Button
                activateButton.OnClientClick = "document.getElementById('" & Me.txtActivate.ClientID & "').value = 'activate'; document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();"
                activateButton.Text = "Aktivieren"

                Dim previewButton As System.Web.UI.WebControls.Button = New System.Web.UI.WebControls.Button()
                previewButton.OnClientClick = "document.getElementById('" & Me.txtEditModeRequested.ClientID & "').value = 'false'; document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();"
                previewButton.Text = "Preview"

                Dim newVersion As New System.Web.UI.WebControls.Button
                newVersion.OnClientClick = "document.getElementById('" & Me.txtRequestedAction.ClientID & "').value = 'newversion'; document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();"
                newVersion.Text = "New Version"

                Dim versionDropDown As New System.Web.UI.WebControls.DropDownList
                Dim myDataTable As DataTable = Database.ReadAllData(Me.ContentOfServerID, DocumentID, Me.EditorID)
                Dim counter As Integer = 0

                Dim versionRows As DataRow() = myDataTable.Select(Nothing, "Version ASC")
                For Each row As DataRow In versionRows
                    If counter = 7 Then
                        Exit For
                    End If
                    Dim myDate As Date = Utils.Nz(row("ReleasedOn"), CType(Nothing, Date))
                    Dim ReleaseDate As String
                    If myDate = Nothing Then
                        ReleaseDate = "(no release)"
                    Else
                        ReleaseDate = myDate.ToShortDateString()
                    End If
                    Dim listItem As New System.Web.UI.WebControls.ListItem
                    listItem.Text = "v" & counter & " " & ReleaseDate
                    listItem.Value = CType(row("Version"), Integer) & ";" & "0"

                    versionDropDown.Items.Add(listItem)
                    counter += 1

                Next
                Dim dropDownScript As String = "document.getElementById('" & txtBrowseToMarketVersion.ClientID & "').value = this.value;"
                versionDropDown.Attributes.Add("onchange", dropDownScript & "document.forms['" & Me.LookupParentServerForm.ClientID & "'].submit();")

                versionDropDown.SelectedValue = Me.CurrentVersion & ";0"

                If toolbar = ToolbarSettings.EditEditableVersion Then
                    Me.pnlEditorToolbar.Controls.Add(saveButton)
                    Me.pnlEditorToolbar.Controls.Add(activateButton)
                    Me.pnlEditorToolbar.Controls.Add(previewButton)
                    Me.pnlEditorToolbar.Controls.Add(versionDropDown)
                ElseIf toolbar = ToolbarSettings.EditNoneEditableVersions Then
                    Me.pnlEditorToolbar.Controls.Add(newVersion)
                    Me.pnlEditorToolbar.Controls.Add(previewButton)
                    Me.pnlEditorToolbar.Controls.Add(versionDropDown)
                ElseIf toolbar = ToolbarSettings.EditWithValidEditVersion Then
                    Me.pnlEditorToolbar.Controls.Add(previewButton)
                    Me.pnlEditorToolbar.Controls.Add(versionDropDown)

                End If




            End Sub

            Protected Overrides Sub PagePreRender_InitializeToolbar()
                Dim label As System.Web.UI.WebControls.Label = New System.Web.UI.WebControls.Label()

                If Me.editorMain.Visible Then
                    SetToolbar(Me.ToolbarSetting)
                End If


                If Me.ToolbarSetting = ToolbarSettings.EditEditableVersion Then

                ElseIf Me.ToolbarSetting = ToolbarSettings.EditNoneEditableVersions Then
                    label.Text = "EditNonEditableVersion"
                    Me.pnlEditorToolbar.Controls.Add(label)
                ElseIf Me.ToolbarSetting = ToolbarSettings.EditWithValidEditVersion Then
                    Me.pnlEditorToolbar.Controls.Add(label)
                ElseIf Me.ToolbarSetting = ToolbarSettings.NoVersionAvailable Then
                    label.Text = "Non versions available"
                    Me.pnlEditorToolbar.Controls.Add(label)
                End If

                Me.pnlEditorToolbar.Visible = Me.editorMain.Visible
            End Sub

        End Class

    End Namespace

End Namespace