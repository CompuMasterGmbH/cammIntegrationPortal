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

    <System.Runtime.InteropServices.ComVisible(False)> Public Class ChangeUserPassword
        Inherits CompuMaster.camm.WebManager.Pages.ProtectedPage

        Protected Message As Web.UI.WebControls.Literal
        Protected HideForm As Boolean = False
        Protected ErrMsg As String = Nothing



        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If Request.Form("loginname") <> "" And Request.Form("oldpassword") <> "" And Request.Form("newpassword1") <> "" And Request.Form("newpassword2") <> "" Then

                Dim MyCurUserID As Long = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                Dim MyCurUserInfo As New CompuMaster.camm.WebManager.WMSystem.UserInformation(MyCurUserID, CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                If Request.Form("newpassword1") <> Request.Form("newpassword2") Then
                    ErrMsg = cammWebManager.Internationalization.UpdatePW_ErrMsg_ConfirmationFailed
                ElseIf Request.Form("oldpassword") = Request.Form("newpassword1") Then
                    ErrMsg = cammWebManager.Internationalization.UpdatePW_ErrMsg_InsertAllRequiredPWFields
                ElseIf cammWebManager.PasswordSecurity.InspectionSeverities(MyCurUserInfo.AccessLevel.ID).ValidatePasswordComplexity(Request.Form("newpassword1"), MyCurUserInfo) <> CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Success Then
                    ErrMsg = cammWebManager.Internationalization.UpdatePW_Error_PasswordComplexityPolicy
                Else
                    Dim MyDBConn As New System.Data.SqlClient.SqlConnection
                    Dim MyRecSet As System.Data.SqlClient.SqlDataReader
                    Dim MyCmd As New System.Data.SqlClient.SqlCommand

                    Dim username As String = Trim(CType(Session("System_Username"), String))
                    Dim oldPassword_plain As String = CStr(Request.Form("oldpassword"))
                    Dim newPassword_plain As String = CStr(Request.Form("newpassword1"))

                    Dim oldPassword_transformationResult As CryptoTransformationResult = Me.cammWebManager.System_GetUserPasswordTransformationResult(username)
                    Dim oldPassword_Transformer As IWMPasswordTransformation = PasswordTransformerFactory.ProduceCryptographicTransformer(oldPassword_transformationResult.Algorithm)

                    Dim defaultAlgoTransformer As New CompuMaster.camm.WebManager.DefaultAlgoCryptor(Me.cammWebManager)
                    Dim newPassword_transformationResult As CryptoTransformationResult = defaultAlgoTransformer.TransformPlaintext(newPassword_plain)

                    Dim oldPasssword_crypted As String = oldPassword_Transformer.TransformString(oldPassword_plain, oldPassword_transformationResult.Noncevalue)
                    Dim newPassword_crypted As String = newPassword_transformationResult.TransformedText

                    MyDBConn.ConnectionString = cammWebManager.ConnectionString
                    MyDBConn.Open()

                    ' Open command object
                    With MyCmd

                        .CommandText = "Public_UpdateUserPW"
                        .CommandType = CommandType.StoredProcedure

                        ' Get parameter value and append parameter.
                        .Parameters.Add("@Username", SqlDbType.NVarChar).Value = username
                        .Parameters.Add("@OldPasscode", SqlDbType.VarChar).Value = oldPasssword_crypted
                        .Parameters.Add("@NewPasscode", SqlDbType.VarChar).Value = newPassword_crypted
                        .Parameters.Add("@ServerIP", SqlDbType.NVarChar).Value = cammWebManager.CurrentServerIdentString
                        .Parameters.Add("@RemoteIP", SqlDbType.NVarChar).Value = Utils.LookupRealRemoteClientIPOfHttpContext(Me.Context)
                        .Parameters.Add("@WebApplication", SqlDbType.VarChar, 1024).Value = "Public"
                        If Me.cammWebManager.System_SupportsMultiplePasswordAlgorithms() Then
                            .Parameters.Add("@LoginPWAlgorithm", SqlDbType.Int).Value = newPassword_transformationResult.Algorithm
                            .Parameters.Add("@LoginPWNonceValue", SqlDbType.VarBinary, 4096).Value = newPassword_transformationResult.Noncevalue
                        End If
                    End With

                    'Create recordset by executing the command
                    MyCmd.Connection = MyDBConn
                    MyRecSet = MyCmd.ExecuteReader()

                    If Err.Number <> 0 Then
                        ErrMsg = cammWebManager.Internationalization.UpdatePW_ErrMsg_Undefined
                    ElseIf Not MyRecSet.Read Then
                        ErrMsg = cammWebManager.Internationalization.UpdatePW_ErrMsg_Undefined
                    ElseIf IsDBNull(MyRecSet(0)) = True Then
                        ErrMsg = cammWebManager.Internationalization.UpdateProfile_ErrMsg_Undefined
                    ElseIf Utils.Nz(MyRecSet(0), 0) = -1 Then
                        ErrMsg = cammWebManager.Internationalization.UpdatePW_ErrMsg_Success
                        Message.Text = "<TABLE cellSpacing=10 cellPadding=0 border=0><TBODY><TR>"
                        Message.Text &= "<TD vAlign=top><P><font face=""Arial"" size=""3""><b>" & cammWebManager.Internationalization.UpdatePW_Descr_Title & "</b></font></P>"
                        If ErrMsg <> "" Then
                            Message.Text &= "<p><font face=""Arial"" size=""2"" color=""red"">" & ErrMsg & "</font></p>"
                        End If
                        Message.Text &= "</TD></TR></TBODY></TABLE>"

                        HideForm = True
                    Else
                        ErrMsg = cammWebManager.Internationalization.UpdatePW_ErrMsg_WrongOldPW
                    End If
                End If

            ElseIf Request.Form("oldpassword") <> "" Or Request.Form("newpassword1") <> "" Or Request.Form("newpassword2") <> "" Then
                ErrMsg = cammWebManager.Internationalization.UpdatePW_ErrMsg_InsertAllRequiredFields
            End If

        End Sub

    End Class

End Namespace