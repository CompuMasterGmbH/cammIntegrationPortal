﻿'Copyright 2015-2016 CompuMaster GmbH, http://www.compumaster.de
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
                Me.CssWidth = "100%"
                Me.CssHeight = "280px"
            End Sub

            Private Sub PlainTextEditor_Init(sender As Object, e As EventArgs) Handles Me.Init
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