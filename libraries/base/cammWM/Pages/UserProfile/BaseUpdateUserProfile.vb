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

    <System.Runtime.InteropServices.ComVisible(False)> Public MustInherit Class BaseUpdateUserProfile
        Inherits CompuMaster.camm.WebManager.Pages.ProtectedPage

        Protected Overrides Sub OnLoad(e As EventArgs)
            Me.cammWebManager.SecurityObject = "@@Public"
            MyBase.OnLoad(e)
        End Sub

        Private _SuppressUserNotifications As Boolean = False
        Protected Overridable Property SuppressUserNotifications() As Boolean
            Get
                Return _SuppressUserNotifications
            End Get
            Set(ByVal value As Boolean)
                _SuppressUserNotifications = value
            End Set
        End Property

        ''' <summary>
        ''' Pointing to the textbox with the confirmation password which shall be used for verification of the user
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Protected MustOverride Property ConfirmationUserPassword() As String

        Protected Overridable ReadOnly Property LocalizedTextRequiredField() As String
            Get
                Return cammWebManager.Internationalization.ErrorRequiredField
            End Get
        End Property

        ''' <summary>
        ''' The main workflow for this page is to collect provided data from user, update the account 
        ''' </summary>
        ''' <remarks>If the creation is successful, method AfterUserUpdate will be executed</remarks>
        Protected Overridable Sub CollectDataAndUpdateAccount()
            Dim UserInfo As CompuMaster.camm.WebManager.IUserInformation
            UserInfo = Me.UpdateBasicUserInfoOfCurrentUser
            'Fill (with possibility for custom overridings) the user info object
            Me.FillUserAccount(UserInfo)

            'Write account
            Dim UpdateSuccessfull As Boolean
            UpdateSuccessfull = WriteUserAccount(UserInfo)

            'Login and redirect to next page if successfull - otherwise keep here with the validation error messages
            If UpdateSuccessfull = True Then
                AfterUserUpdate(UserInfo)
            End If
        End Sub

        ''' <summary>
        ''' Fill an IUserInformation object based on current user info and provided basic data from user
        ''' </summary>
        ''' <remarks></remarks>
        Protected MustOverride Function UpdateBasicUserInfoOfCurrentUser() As WebManager.IUserInformation

        ''' <summary>
        ''' Overridable method for customized actions after the new user account has been written
        ''' </summary>
        ''' <param name="userInfo">The updated user account</param>
        Protected Overridable Sub AfterUserUpdate(ByVal userInfo As WebManager.IUserInformation)
        End Sub

        ''' <summary>
        ''' Finally write the user account
        ''' </summary>
        ''' <param name="userInfo"></param>
        ''' <remarks></remarks>
        Protected Overridable Function WriteUserAccount(ByVal userInfo As WebManager.IUserInformation) As Boolean
            Try
                'Validate if the update is allowed to be made
                If CType(userInfo, WMSystem.UserInformation).IsSystemUser Then
                    Throw New Exception("Update of profiles only for real users")
                End If

                CType(userInfo, WMSystem.UserInformation).Save(SuppressUserNotifications)

                Return True

            Catch ex As Exception
                If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                    ShowErrorMessage("Internal error: " & ex.ToString)
                Else
                    ShowErrorMessage("Internal error: " & ex.Message)
                End If
                cammWebManager.Log.RuntimeException(ex, False, False, WMSystem.DebugLevels.NoDebug)

                Return False
            End Try
        End Function

        ''' <summary>
        ''' Show an error message on the GUI (e.g. in validator summary)
        ''' </summary>
        ''' <param name="message"></param>
        ''' <remarks></remarks>
        Protected MustOverride Sub ShowErrorMessage(ByVal message As String)

        ''' <summary>
        '''     Fill the user profile with the new data which shall be saved
        ''' </summary>
        ''' <param name="userInfo">The current user profile which shall be updated</param>
        Protected MustOverride Sub FillUserAccount(ByVal userInfo As WebManager.IUserInformation)

    End Class

End Namespace