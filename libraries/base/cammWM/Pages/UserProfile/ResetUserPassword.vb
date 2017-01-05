'Copyright 2001-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager.Pages.UserAccount

    <System.Runtime.InteropServices.ComVisible(False)> Public Class ResetUserPassword
        Inherits CompuMaster.camm.WebManager.Pages.Page

        Protected NewPassword As Web.UI.WebControls.TextBox
        Protected NewPasswordConfirm As Web.UI.WebControls.TextBox
        Protected Message As Web.UI.WebControls.Literal
        Protected HideForm As Boolean = False
        Protected ErrMsg As String = Nothing

        Private Function IsComplexEnoughPasswordForUser(ByVal ui As WMSystem.UserInformation, ByVal password As String) As Boolean
            Return cammWebManager.PasswordSecurity.InspectionSeverities(ui.AccessLevel.ID).ValidatePasswordComplexity(password, ui) = CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Success
        End Function


        Protected Sub UpdateUserPassword(ByVal username As String, ByVal password As String)
            Dim cryptor As New DefaultAlgoCryptor(Me.cammWebManager)
            Dim transformResult As CryptoTransformationResult = cryptor.TransformPlaintext(password)
            cammWebManager.System_UpdateUserTransformationResult(username, transformResult)
        End Sub

        Private Function UserIDExists(ByVal id As Long) As Boolean
            Dim MyCmd As New SqlClient.SqlCommand
            MyCmd.Connection = New SqlClient.SqlConnection(Me.cammWebManager.ConnectionString)
            MyCmd.CommandText = "SELECT 1 FROM [dbo].Benutzer WHERE ID = @id"
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@id", SqlDbType.Int).Value = id
            Return CType(CompuMaster.camm.WebManager.Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), False), Boolean)
        End Function

        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.cammWebManager.PageTitle = cammWebManager.Internationalization.UpdatePW_Descr_Title
            Dim UserId As Long
            Dim token As String
            Try
                UserId = CType(Request("user"), Long)
                token = Request("token")
            Catch ex As Exception
                Message.Text = "Invalid input."
                HideForm = True
                Return
            End Try

            If token Is Nothing OrElse token = String.Empty Then
                Message.Text = "Missing token."
                HideForm = True
                Return
            End If

            If UserIDExists(UserId) Then
                Dim userinfo As New WMSystem.UserInformation(UserId, Me.cammWebManager)
                Dim passwordReset As New PassswordReset(Me.cammWebManager, userinfo)
                If passwordReset.TokenIsValid(token) Then
                    If Me.IsPostBack Then
                        Dim password As String = NewPassword.Text
                        Dim passwordConfirmed As String = NewPasswordConfirm.Text

                        Dim passwordsAreEqual As Boolean = (password = passwordConfirmed)
                        If Not passwordsAreEqual Then
                            ErrMsg = cammWebManager.Internationalization.UpdatePW_ErrMsg_ConfirmationFailed
                            Return
                        End If

                        If Not IsComplexEnoughPasswordForUser(userinfo, password) Then
                            ErrMsg = cammWebManager.Internationalization.UpdatePW_Error_PasswordComplexityPolicy
                            Return
                        End If

                        UpdateUserPassword(userinfo.LoginName, password)
                        passwordReset.DeleteStoredToken()
                        Message.Text = cammWebManager.Internationalization.UpdatePW_ErrMsg_Success & "<br><a href=""" & Me.cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL & """>" & Me.cammWebManager.Internationalization.NavAreaNameLogin & "</a>"
                        HideForm = True
                    End If
                Else
                    Message.Text = "Invalid token. Has your token expired? You can request the e-mail again using this link: " & "<a href=""" & Me.cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx"">Try again</a>"
                    HideForm = True
                End If
            Else
                Message.Text = "Invalid user"
                HideForm = True
            End If
        End Sub
    End Class

End Namespace