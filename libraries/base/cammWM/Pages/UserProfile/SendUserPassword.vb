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

    <System.Runtime.InteropServices.ComVisible(False)> Public Class SendUserPassword
        Inherits CompuMaster.camm.WebManager.Pages.Page

        Protected Sub ValidateInputAndSendMail(ByVal userName As String, ByVal emailAddress As String)

            If userName = Nothing Then
                Throw New ArgumentNullException("userName")
            ElseIf emailAddress = Nothing Then
                Throw New ArgumentNullException("emailAddress")
            End If

            'Lookup user ID
            Dim UserID As Long
            UserID = CType(cammWebManager.System_GetUserID(userName, True), Long)
            If UserID = WMSystem.SpecialUsers.User_Anonymous Then
                cammWebManager.RedirectToErrorPage(WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorSendPWWrongLoginOrEmailAddress, Nothing, "User doesn't exist", Nothing, userName, emailAddress, True)
            End If

            'Validate input credentials
            Dim UserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = Nothing
            If WMSystem.IsSystemUser(UserID) = False Then 'Not an anonymous user
                UserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                If LCase(userName) <> LCase(UserInfo.LoginName) OrElse LCase(emailAddress) <> LCase(UserInfo.EMailAddress) Then
                    cammWebManager.RedirectToErrorPage(WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorSendPWWrongLoginOrEmailAddress, Nothing, "User credentials don't match with ID " & UserID, Nothing, userName, emailAddress, True)
                End If
            Else
                cammWebManager.RedirectToErrorPage(WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorSendPWWrongLoginOrEmailAddress, Nothing, "User ID " & UserID & " invalid (IsSystemUser = True)", Nothing, userName, emailAddress, True)
            End If

            Dim recoveryBehavior As PasswordRecoveryBehavior = cammWebManager.System_GetPasswordRecoveryBehavior()

            If recoveryBehavior = PasswordRecoveryBehavior.DecryptIfPossible Then

                Dim transformationResult As CryptoTransformationResult = cammWebManager.System_GetUserPasswordTransformationResult(userName)

                If AlgorithmInfo.CanDecrypt(transformationResult.Algorithm) Then
                    'Message verschicken
                    cammWebManager.Notifications.NotificationForUser_ForgottenPassword(UserInfo)
                End If
            Else
                Dim resetLinkGenerator As New PassswordReset(cammWebManager, UserInfo)
                cammWebManager.Notifications.NotificationForUser_PasswordResetLink(UserInfo, resetLinkGenerator.CreateResetUrl())
            End If


        End Sub

    End Class

End Namespace