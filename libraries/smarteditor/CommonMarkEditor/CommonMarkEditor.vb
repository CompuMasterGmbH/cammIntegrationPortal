﻿Option Explicit On
Option Strict Off

Imports System.ComponentModel
Imports System.Web.UI

Namespace CompuMaster.camm.SmartWebEditor.Controls

    ' Involves CommonMark.Net from https://github.com/Knagis/CommonMark.NET, see license at https://github.com/Knagis/CommonMark.NET/blob/master/LICENSE.md or in following:
    '
    ' Copyright (c) 2014, Kārlis Gaņģis
    ' All rights reserved.
    ' 
    ' Redistribution and use in source and binary forms, with or without
    ' modification, are permitted provided that the following conditions are met:
    ' 
    '    * Redistributions of source code must retain the above copyright
    '      notice, this list of conditions and the following disclaimer.
    ' 
    '    * Redistributions in binary form must reproduce the above copyright
    '      notice, this list of conditions and the following disclaimer in the
    '      documentation and/or other materials provided with the distribution.
    ' 
    '    * Neither the name of Kārlis Gaņģis nor the names of other contributors 
    '      may be used to endorse or promote products derived from this software 
    '      without specific prior written permission.
    ' 
    ' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
    ' ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
    ' WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    ' DISCLAIMED. IN NO EVENT SHALL COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY
    ' DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
    ' (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
    ' LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
    ' ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
    ' (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    ' SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

    ''' <summary>
    ''' A plain text editor control which renders using CommonMark specification
    ''' </summary>
    ''' <remarks>
    ''' Involves CommonMark.Net from https://github.com/Knagis/CommonMark.NET, see license at https://github.com/Knagis/CommonMark.NET/blob/master/LICENSE.md 
    ''' </remarks>
    <DefaultProperty("Html"), ToolboxData("<{0}:CommonMarkEditor1 runat=server></{0}:CommonMarkEditor1>")>
    Public Class CommonMarkEditor
        Inherits Global.CompuMaster.camm.SmartWebEditor.Controls.SmartPlainHtmlEditor


        Protected Overrides Sub PagePreRender_InitializeToolbar()
            MyBase.PagePreRender_InitializeToolbar()

            If Me.MainEditor.Editable Then
                Dim linkButtonToHelp As New System.Web.UI.WebControls.Button
                linkButtonToHelp.EnableViewState = False
                linkButtonToHelp.OnClientClick = "window.open('http://commonmark.org/help/', '_blank'); return false;"
                linkButtonToHelp.Text = "Help"

                Dim addAtIndex = 0
                'perhaps proper solution woudl be to create a  "buttonToolbar"...
                Dim counter = 0
                For Each control As Control In Me.pnlEditorToolbar.Controls
                    If control Is Me.PreviewButton Then
                        addAtIndex = counter + 1
                        Exit For
                    End If
                    counter += 1
                Next

                Me.pnlEditorToolbar.Controls.AddAt(addAtIndex, linkButtonToHelp)

            End If
        End Sub

        Private Sub CommonMarkEditor_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

            Me.lblViewOnlyContent.InnerHtml = CommonMark.CommonMarkConverter.Convert(Me.MainEditor.Html)



        End Sub

    End Class

End Namespace