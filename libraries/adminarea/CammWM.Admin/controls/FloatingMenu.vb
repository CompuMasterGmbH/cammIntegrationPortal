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

Option Strict On
Option Explicit On

Imports System.Web.UI.WebControls

Namespace CompuMaster.camm.WebManager.Controls.Administration

    ''' <summary>
    '''     The floating menu on the right bottom which allows navigation to top of the current page or a history go back
    ''' </summary>
    Public Class FloatingMenu
        Inherits UserControl

        Public Sub New()
            Me.AnchorText = "Overview"
        End Sub

        Public Property AnchorText() As String
            Get
                Return Utils.Nz(ViewState("AnchorText"), String.Empty)
            End Get
            Set(ByVal Value As String)
                ViewState("AnchorText") = Value
            End Set
        End Property

        Public Property AnchorTitle() As String
            Get
                Return Utils.Nz(ViewState("AnchorTitle"), String.Empty)
            End Get
            Set(ByVal Value As String)
                ViewState("AnchorTitle") = Value
            End Set
        End Property

        Public Property HRef() As String
            Get
                Return Utils.Nz(ViewState("HRef"), String.Empty)
            End Get
            Set(ByVal Value As String)
                ViewState("HRef") = Value
            End Set
        End Property

        Private Literal As Literal
        Private Sub ControlInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            Literal = New Literal
            Me.Controls.Add(Literal)
        End Sub

        Private Sub ControlPreRender(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
            Dim Script As New System.Text.StringBuilder
            If HRef = Nothing Then
                Script.Append("<div id=""RePos"" style=""float: left; width: 80px; height: 60px; line-height: 60px; position: fixed; right: 0px; bottom: 20px; z-index: 10000; background-color: #ffffff; border: solid 2px #E1E1E1; font-family: Arial; font-size: 8pt; text-align: center;"">")
                Script.Append("<a href=""#top"">Go to top</a>")
                Script.Append("</div>")
            Else
                Script.Append("<div id=""RePos"" style=""float: left; width: 80px; height: 60px; line-height: 30px; position: fixed; right: 0px; bottom: 20px; z-index: 10000; background-color: #ffffff; border: solid 2px #E1E1E1; font-family: Arial; font-size: 8pt; text-align: center;"">")
                Script.Append("<a href=""#top"">Go to top</a>")
                Script.Append("<br>")
                Script.Append("<a href=""" & HRef & """")
                If Me.AnchorTitle <> Nothing Then Script.Append(" title=""" & Server.HtmlEncode(Me.AnchorTitle) & """")
                Script.Append(">")
                Script.Append(Server.HtmlEncode(Me.AnchorText))
                Script.Append("</a>")
                Script.Append("</div>")
            End If
            Literal.Text = Script.ToString
        End Sub

    End Class

End Namespace
