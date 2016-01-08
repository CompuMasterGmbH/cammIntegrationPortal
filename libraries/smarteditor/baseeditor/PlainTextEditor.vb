﻿Option Explicit On
Option Strict Off

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace CompuMaster.camm.SmartWebEditor

    Namespace Controls

        <DefaultProperty("Html"), ToolboxData("<{0}:PlainTextEditor1 runat=server></{0}:PlainTextEditor1>")>
        Friend Class PlainTextEditor
            Inherits System.Web.UI.WebControls.TextBox
            Implements IEditor

            '<Bindable(True), Category("Appearance"), DefaultValue(""), Localizable(True)> Property Text() As String
            '    Get
            '        Dim s As String = CStr(ViewState("Text"))
            '        If s Is Nothing Then
            '            Return String.Empty
            '        Else
            '            Return s
            '        End If
            '    End Get

            '    Set(ByVal Value As String)
            '        ViewState("Text") = Value
            '    End Set
            'End Property

            'Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
            '    writer.Write(Text)
            'End Sub

            Public Sub New()
                MyBase.New()
                Me.TextMode = TextBoxMode.MultiLine
            End Sub

            Public Property CssWidth As String Implements IEditor.CssWidth
                Get
                    Return Me.Style.Item("width")
                End Get
                Set(value As String)
                    Me.Style.Add("width", value)
                End Set
            End Property

            Public Property CssHeight As String Implements IEditor.CssHeight
                Get
                    Return Me.Style.Item("height")
                End Get
                Set(value As String)
                    Me.Style.Add("height", value)
                End Set
            End Property

            Public Property TextareaColumns As Integer Implements IEditor.TextareaColumns
                Get
                    Return Me.Columns
                End Get
                Set(value As Integer)
                    Me.Columns = value
                End Set
            End Property

            Public Property TextareaRows As Integer Implements IEditor.TextareaRows
                Get
                    Return Me.Rows
                End Get
                Set(value As Integer)
                    Me.Rows = value
                End Set
            End Property




            Public Property Editable As Boolean Implements IEditor.Editable
                Get
                    Return Me.Enabled
                End Get
                Set(value As Boolean)
                    Me.Enabled = value
                End Set
            End Property

            Private Property IEditor_Html As String Implements IEditor.Html
                Get
                    Return Me.Text
                End Get
                Set(value As String)
                    Me.Text = value
                End Set
            End Property

            Private Property IEditor_EnableViewState As Boolean Implements IEditor.EnableViewState
                Get
                    Return Me.EnableViewState
                End Get
                Set(value As Boolean)
                    Me.EnableViewState = value
                End Set
            End Property

            Private Property IEditor_Visible As Boolean Implements IEditor.Visible
                Get
                    Return Me.Visible
                End Get
                Set(value As Boolean)
                    Me.Visible = value
                End Set
            End Property

            Private ReadOnly Property IEditor_ClientID As String Implements IEditor.ClientID
                Get
                    Return Me.ClientID
                End Get
            End Property

        End Class

    End Namespace

End Namespace