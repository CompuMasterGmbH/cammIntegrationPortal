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

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     A page to reset user password
    ''' </summary>
    Public Class User_Resetpw
        Inherits Page

#Region "Variable Declaration"
        Dim displayloginname, ErrMsg As String
        Protected lblMsg As Label
        Protected txtLoginName, txtNewPassword As TextBox
        Protected WithEvents btnSubmit As Button
#End Region

#Region "Page Event"
        Private Sub User_Resetpw_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            displayloginname = Request.Form("txtLoginName")
            txtLoginName.Text = Utils.Nz(displayloginname, String.Empty)
            txtNewPassword.TextMode = TextBoxMode.Password
        End Sub
#End Region

#Region "Control Events"
        Private Sub btnSubmitClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
            If Request.Form("txtNewPassword") <> "" Then

                Dim LoginIDOfUser As Integer = Utils.Nz(cammWebManager.System_GetUserID(Request.Form("txtLoginName")), -1)
                If LoginIDOfUser = -1 Then
                    ErrMsg = "Unknown login name """ & Request.Form("txtLoginName") & """" & Utils.Nz(cammWebManager.System_GetUserID(Request.Form("txtLoginName")), String.Empty)
                Else
                    Dim MyUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = cammWebManager.System_GetUserInfo(CType(LoginIDOfUser, Int64))

                    Select Case cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).ValidatePasswordComplexity(Request.Form("txtNewPassword"), MyUserInfo)
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_HigherPasswordComplexityRequired
                            ErrMsg = cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).ErrorMessageComplexityPoints(1)
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_LengthMaximum
                            ErrMsg = "The password requires to be not bigger than " & cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).RequiredMaximumPasswordLength & " characters!"
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_LengthMinimum
                            ErrMsg = "The password requires to be not smaller than " & cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).RequiredPasswordLength & " characters!"
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_NotAllowed_PartOfProfileInformation
                            ErrMsg = "The password shouldn't contain pieces of the user account profile, especially login name, first or last name!"
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_Unspecified
                            ErrMsg = "There are some unknown errors when validating with the security policy for passwords!"
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Success
                            MyUserInfo.SetPassword(Request.Form("txtNewPassword"))
                            displayloginname = ""
                            ErrMsg = "Password successfully changed for user """ & MyUserInfo.LoginName & """ (" & MyUserInfo.FullName & ")!"
                    End Select
                End If
            ElseIf Request.Form("txtNewPassword") = "" And Request.Form("txtLoginName") <> "" Then
                ErrMsg = "Please enter a new password for the user!"
            End If

            If ErrMsg <> "" Then
                lblMsg.Text = ErrMsg
            End If
        End Sub
#End Region

    End Class

End Namespace