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

            Protected Overrides Sub PagePreRender_InitializeToolbar()
                Dim label As System.Web.UI.WebControls.Label = New System.Web.UI.WebControls.Label()

                If Me.ToolbarSetting = ToolbarSettings.EditEditableVersion Then
                    label.Text = "Editable version"
                    Me.pnlEditorToolbar.Controls.Add(label)
                ElseIf Me.ToolbarSetting = ToolbarSettings.EditNoneEditableVersions Then
                    label.Text = "EditNonEditableVersion"
                    Me.pnlEditorToolbar.Controls.Add(label)
                ElseIf Me.ToolbarSetting = ToolbarSettings.EditWithValidEditVersion Then
                    label.Text = "EditWithValidEditVersion"
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