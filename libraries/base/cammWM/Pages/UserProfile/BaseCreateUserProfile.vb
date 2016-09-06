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

    <System.Runtime.InteropServices.ComVisible(False)> Public MustInherit Class BaseCreateUserProfile
        Inherits CompuMaster.camm.WebManager.Pages.Page

        Private _SuppressSecurityAdminNotifications As Boolean = False
        Protected Overridable Property SuppressSecurityAdminNotifications() As Boolean
            Get
                Return _SuppressSecurityAdminNotifications
            End Get
            Set(ByVal value As Boolean)
                _SuppressSecurityAdminNotifications = value
            End Set
        End Property

        Private _SuppressUserNotifications As Boolean = False
        Protected Overridable Property SuppressUserNotifications() As Boolean
            Get
                Return _SuppressUserNotifications
            End Get
            Set(ByVal value As Boolean)
                _SuppressUserNotifications = value
            End Set
        End Property

        Private _AccessLevelDefault As Integer = Integer.MinValue
        Protected Overridable Function AccessLevelDefault() As Integer
            If _AccessLevelDefault = Integer.MinValue Then
                _AccessLevelDefault = cammWebManager.CurrentServerInfo.ParentServerGroup.AccessLevelDefault.ID
            End If
            Return _AccessLevelDefault
        End Function

        Protected Overridable ReadOnly Property LocalizedTextRequiredField() As String
            Get
                Return cammWebManager.Internationalization.ErrorRequiredField
            End Get
        End Property

        ''' <summary>
        ''' After a successfull account creation and login, the user lands on this URL
        ''' </summary>
        ''' <param name="user">The user account that has been created</param>
        ''' <remarks></remarks>
        Public Overridable Function UrlAfterLogin(ByVal user As IUserInformation) As String
            Return cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL & "?ref=/sysdata/userjustcreated.aspx&Lang=" & cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(user.ID)
        End Function

        ''' <summary>
        ''' The main workflow for this page is to collect provided data from user, create the account and login
        ''' </summary>
        ''' <remarks>If the creation is successful, method AfterUserCreation will be executed and the login workflow starts</remarks>
        Protected Overridable Sub CollectDataAndCreateAccountAndStartLoginWorkflow()
            Dim UserInfo As CompuMaster.camm.WebManager.IUserInformation
            UserInfo = Me.CreateUserInfo
            'Fill (with possibility for custom overridings) the user info object
            Me.FillUserAccount(UserInfo)

            'Reset user login in case there is already a user logged on (typically situtations when a user registered himself, browsed back and recreated a 2nd account (the first user is logged on in background); this would lead to a misinterpretion and wrong notification message to the security admins (it would tell them that the user has been created by another user and not by himself))
            Me.cammWebManager.ResetUserLoginName()

            'Write account
            Dim UpdateSuccessfull As Boolean
            UpdateSuccessfull = WriteUserAccount(UserInfo)

            'Login and redirect to next page if successfull - otherwise keep here with the validation error messages
            If UpdateSuccessfull = True Then
                AfterUserCreation(UserInfo)
                ExecuteLogin(UserInfo.LoginName, Me.NewUserPassword, UrlAfterLogin(UserInfo))
            End If
        End Sub

        Private _ForceLogin As Boolean
        ''' <summary>
        ''' Force the user login - also in case that the user already logged on at another terminal
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Protected Property ForceLogin() As Boolean
            Get
                Return _ForceLogin
            End Get
            Set(ByVal value As Boolean)
                _ForceLogin = True
            End Set
        End Property

        ''' <summary>
        ''' Start the login workflow
        ''' </summary>
        ''' <param name="loginName"></param>
        ''' <param name="password"></param>
        ''' <param name="pageUrlAfterLogin"></param>
        ''' <remarks></remarks>
        Public Sub ExecuteLogin(ByVal loginName As String, ByVal password As String, ByVal pageUrlAfterLogin As String)
            'Creation successfull - login with this user name at the master server now
            If Request.ApplicationPath = "/" AndAlso cammWebManager.CurrentServerInfo.ParentServerGroup.MasterServer.ID = cammWebManager.CurrentServerInfo.ID Then
                'We are the master server
                Me.cammWebManager.ExecuteLogin(loginName, NewUserPassword)
                'Memorize to welcome the user
                Session("System_Referer") = pageUrlAfterLogin
                'Login now at all servers and finish initializing of user session
                Me.cammWebManager.RedirectTo(cammWebManager.Internationalization.User_Auth_Validation_CheckLoginURL & "?User=created")
            Else
                'Forward to the login user credential validation form
                Dim PostData As New System.Collections.Specialized.NameValueCollection
                PostData("TargetUrlAfterLogin") = pageUrlAfterLogin
                PostData("Username") = loginName
                PostData("Passcode") = password
                Me.cammWebManager.RedirectWithPostDataTo(cammWebManager.Internationalization.User_Auth_Validation_CheckLoginURL & "?User=created", PostData, "Account created, login must follow on master server root application", Nothing)
            End If
        End Sub

        ''' <summary>
        ''' Create an IUserInformation object based on provided data from user
        ''' </summary>
        ''' <remarks></remarks>
        Protected MustOverride Function CreateUserInfo() As WebManager.IUserInformation
        ''' <summary>
        ''' Overridable method for customized actions after the new user account has been written
        ''' </summary>
        ''' <param name="userInfo">The created user account</param>
        Protected Overridable Sub AfterUserCreation(ByVal userInfo As WebManager.IUserInformation)
        End Sub

        ''' <summary>
        ''' Pointing to the textbox with the new password which shall be used for the new user account
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Protected MustOverride Property NewUserPassword() As String

        ''' <summary>
        ''' Finally write the user account
        ''' </summary>
        ''' <param name="userInfo"></param>
        ''' <remarks></remarks>
        Protected Overridable Function WriteUserAccount(ByVal userInfo As WebManager.IUserInformation) As Boolean

            Try
                'Validate that the loginname is not already in use
                If WebManager.PerformanceMethods.IsUserExisting(Me.cammWebManager, userInfo.LoginName) = True Then
                    Throw New Exception("User already exists: " & userInfo.LoginName)
                End If
                'Validate if the update is allowed to be made
                If CType(userInfo, WMSystem.UserInformation).IsSystemUser Then
                    Throw New Exception("Update of profiles only for real users")
                End If

                Dim NewPassword As String = NewUserPassword
                CType(userInfo, WMSystem.UserInformation).Save(NewPassword, SuppressUserNotifications, SuppressSecurityAdminNotifications)

                Return True

            Catch ex As UserInfoDataException
                ShowErrorMessage(ex.Message)
                Return False

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
        ''' Prohibited password parts for password complexity check
        ''' </summary>
        Protected MustOverride Function PasswordSeverityCheckStrings() As String()
        ''' <summary>
        '''     Fill the user profile with the new data which shall be saved
        ''' </summary>
        ''' <param name="userInfo">The current user profile which shall be created</param>
        Protected MustOverride Sub FillUserAccount(ByVal userInfo As WebManager.IUserInformation)

        ''' <summary>
        ''' The list of allowed values for the country field (or empty list in case of no limitation)
        ''' </summary>
        ''' <returns></returns>
        Protected ReadOnly Property LimitedAllowedCountries As System.Collections.Generic.List(Of String)
            Get
                Static _LimitedAllowedCountries As System.Collections.Generic.List(Of String)
                If _LimitedAllowedCountries Is Nothing Then
                    _LimitedAllowedCountries = WMSystem.UserInformation.CentralConfig_AllowedValues_FieldCountry(Me.cammWebManager)
                    If _LimitedAllowedCountries Is Nothing Then
                        _LimitedAllowedCountries = New System.Collections.Generic.List(Of String)
                    End If
                End If
                Return _LimitedAllowedCountries
            End Get
        End Property

        Protected Function ConvertStringsToListItems(values As System.Collections.Generic.List(Of String)) As List(Of System.Web.UI.WebControls.ListItem)
            Dim Result As New List(Of System.Web.UI.WebControls.ListItem)
            For MyCounter As Integer = 0 To values.Count - 1
                Result.Add(New System.Web.UI.WebControls.ListItem(values(MyCounter)))
            Next
            Return Result
        End Function

    End Class

End Namespace