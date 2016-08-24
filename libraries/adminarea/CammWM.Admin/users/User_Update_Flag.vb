'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'Alternatively, the camm Integration Portal (or camm Web-Manager) can be licensed for closed-source / commercial projects from CompuMaster GmbH, <http://www.camm.biz/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie k�nnen es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder sp�teren ver�ffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es n�tzlich sein wird, aber OHNE JEDE GEW�HRLEISTUNG, bereitgestellt; sogar ohne die implizite Gew�hrleistung der MARKTF�HIGKEIT oder EIGNUNG F�R EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License f�r weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
'Alternativ kann camm Integration Portal (oder camm Web-Manager) lizenziert werden f�r Closed-Source / kommerzielle Projekte von  CompuMaster GmbH, <http://www.camm.biz/>.

Option Strict On
Option Explicit On

Imports System.Web.UI.WebControls

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     A page to update user flags
    ''' </summary>
    Public Class User_Update_Flag
        Inherits Page

#Region "Variable Declaration"
        Dim dt As New DataTable
        Dim Fieldtype, FieldValue, Field_ID, ErrMsg As String
        Protected lblMsg, lblFlagUser As Label
        Protected txtType, txtValue As TextBox
        Protected WithEvents btnSubmit As Button
#End Region

#Region "Page Events"
        Private Sub ButtonSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
            If Me.IsPostBack Then
                SaveChanges()
            End If
        End Sub
        ''' <summary>
        ''' This method saves the changes
        ''' </summary>
        ''' <remarks>This save method is called by the form's submit button</remarks>
        Protected Overridable Sub SaveChanges()
            Dim type As String = txtType.Text.Trim()

            If Request.QueryString("ID") <> "" And type <> "" Then
                'Update record
                Dim value As String = txtValue.Text.Trim()

                Dim validator As New CompuMaster.camm.WebManager.FlagValidation(type)
                If validator.IsCorrectValueForType(value) Then
                    Dim userinfo As New camm.WebManager.WMSystem.UserInformation(CLng(Request.QueryString("ID")), cammWebManager)
                    userinfo.AdditionalFlags.Set(type, value)
                    userinfo.Save()
                    lblMsg.Text = "Flag saved successfully"
                Else
                    lblMsg.Text = "Incorrect value for this type. Flag wasn't saved."
                End If
            End If
        End Sub

        Private Sub User_Update_Flag_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            FillFields()
        End Sub

        Protected Overridable Sub FillFields()
            Dim userinfo As New camm.WebManager.WMSystem.UserInformation(CLng(Request.QueryString("ID")), cammWebManager) 'Reload user info object to present the latest data
            lblFlagUser.Text = Server.HtmlEncode("Flag of user " & userinfo.FullName & "  (" & userinfo.LoginName & ") ")
            If Not Me.IsPostBack Then
                txtType.Text = Request.QueryString("Type")
                txtValue.Text = userinfo.AdditionalFlags(txtType.Text)
            End If
        End Sub

#End Region

    End Class

End Namespace