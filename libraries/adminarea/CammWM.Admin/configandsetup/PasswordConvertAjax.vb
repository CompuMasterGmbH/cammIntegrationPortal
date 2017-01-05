'Copyright 2012-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager.Pages.Administration

    Public Class PasswordConvertAjax
        Inherits Page
#If NetFramework <> "1_1" Then
        Private sessionKey As String = "PasswordConverter"
        Private ReadOnly Property Converter As PasswordConverter
            Get
                Dim sessionKey As String = "PasswordConverter"
                If HttpContext.Current.Session(sessionKey) Is Nothing Then
                    Dim convert As PasswordConverter = New PasswordConverter(Me.cammWebManager, Me.cammWebManager.System_GetDefaultPasswordAlgorithm())
                    HttpContext.Current.Session(sessionKey) = convert
                    Return convert
                End If
                Return CType(HttpContext.Current.Session(sessionKey), PasswordConverter)
            End Get
        End Property


        Private Sub Page_Onload(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim action As String = Request("action")
            If Not action Is Nothing Then
                If action = "count" Then
                    Dim counted As Integer = Me.Converter.CountConvertable()
                    Response.Write(counted.ToString())
                End If
                If action = "convert" Then
                    Dim converted As Integer = Me.Converter.ConvertPasswords(5)
                    Response.Write(converted.ToString())
                End If
                If action = "reset" Then
                    HttpContext.Current.Session(sessionKey) = Nothing
                    Response.Write("ok")
                End If

            End If
        End Sub
#End If
    End Class


End Namespace
