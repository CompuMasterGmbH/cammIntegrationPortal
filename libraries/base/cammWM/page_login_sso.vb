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

Option Explicit On
Option Strict On

Namespace CompuMaster.camm.WebManager.Pages.Login

    ''' <summary>
    '''     A page which performs a seamless or automatic logon procedure
    ''' </summary>
    ''' <remarks>
    '''     This page provides login methods for external account data.
    '''     To prevent endless roundtrips from one page to this page and back and again to this page, the referring page should be another one than the page address after logon. This means for example, that the referring page is located in the web's root folder ("/"), but the cammwebmanager.Internationalization.User_Auth_Config_Paths_UserAuthSystem targets another subfolder, e. g. "/welcome/".
    ''' </remarks>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class LoginWithExternalAccount
        Inherits CheckLogin

        Private Sub OnPageInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Init
            If Request.QueryString("redirurl") <> Nothing Then
                Session("System_Referer") = Request.QueryString("redirurl")
            End If

            'The force-login page must post the data back to our page here, not the regular checklogin-page
            cammWebManager.Internationalization.User_Auth_Validation_TerminateOldSessionScriptURL &= "?CheckLoginUrl=" & Server.UrlEncode(Request.Url.AbsolutePath)
        End Sub
        ''' <summary>
        '''     Are any login credentials available? In single-sign-on scenarios, the user might be logged on with an external user account or anonymously.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     This property helps to find out why the LoginCredentials property was empty: either the external login information hasbn't been there or else the login information had been there but without an assigned, valid webmanager account
        ''' </remarks>
        Protected Overrides ReadOnly Property IsAuthenticated() As Boolean
            Get
                Return Page.User.Identity.IsAuthenticated
            End Get
        End Property

        Protected Overrides ReadOnly Property LoginCredentials() As Utils.LogonCredentials
            Get
                Dim Result As New LogonCredentials
                If Page.User.Identity.IsAuthenticated Then
                    Dim UserID As Long = cammWebManager.LookupUserIDOfExternalUserAccount(Page.User.Identity.Name)
                    If UserID <> Nothing Then
                        'Logon with this user account
                        Result = New LogonCredentials
                        Result.Username = New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, Me.cammWebManager).LoginName
                        Result.ForceLogin = ForceLogin OrElse (Request("ForceLogin") = "1")
                    End If
                End If
                Return Result
            End Get
        End Property

        Protected Overridable ReadOnly Property ForceLogin() As Boolean
            Get
                Return (RedirectUrlWhenLoginOkayButAlreadyLoggedOn = Nothing)
            End Get
        End Property

        Protected Overridable ReadOnly Property RedirectUrlWhenLoginOkayButAlreadyLoggedOn() As String
            Get
                Return Nothing
            End Get
        End Property

        Protected Overrides ReadOnly Property RedirectionTargets() As System.Collections.Specialized.NameValueCollection
            Get
                Dim Result As New System.Collections.Specialized.NameValueCollection
                Result.Add(CType(WMSystem.ReturnValues_UserValidation.AlreadyLoggedIn, Integer).ToString, RedirectUrlWhenLoginOkayButAlreadyLoggedOn)
                Return Result
            End Get
        End Property
        ''' <summary>
        '''     The actions which shall be made if an external login has been detected but there is not user account in CWM for it
        ''' </summary>
        Protected Overrides Sub OnMissingAuthentication()
            Me.cammWebManager.RedirectTo(Me.cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL, "OnMissingAuthentication", Nothing)
        End Sub
        ''' <summary>
        ''' Create appropriate log entries for external, not-yet-assigned user account 
        ''' </summary>
        ''' <param name="nameOfExternalAccountSystem"></param>
        ''' <param name="externalUserName"></param>
        ''' <history>
        ''' 	[wezel]	    12.09.2008	Created
        ''' 	[zeutzheim]	09.07.2009	Modified
        ''' </history>
        Protected Sub LogMissingAssignmentOfExternalUserAccount(ByVal nameOfExternalAccountSystem As String, ByVal externalUserName As String, ByVal fullUserName As String, ByVal emailAddress As String, ByVal errorDetails As String)
            CompuMaster.camm.WebManager.DataLayer.Current.AddMissingExternalAccountAssignment(Me.cammWebManager, nameOfExternalAccountSystem, externalUserName, fullUserName, emailAddress, errorDetails)
        End Sub
        ''' <summary>
        ''' Mark an external user account (which has been marked as with missing assignment) as assigned successfully
        ''' </summary>
        ''' <remarks>
        ''' In previous requests and steps, an external user account is marked by method LogMissingAssignmentOfExternalUserAccount to be an unassigned account.
        ''' To provide a most current list of user accounts which haven't been assigned (e. g. the user hasn't completed the SSO-create-account form), the user account shall be removed from the list of unassigned users.
        ''' In the camm Web-Manager administration area, there will be a statistic of those unassigned user accounts.
        ''' </remarks>
        Protected Sub MarkExternalUserAccountAsAssigned(ByVal nameOfExternalAccountSystem As String, ByVal externalUserName As String)
            CompuMaster.camm.WebManager.DataLayer.Current.RemoveMissingExternalAccountAssignment(Me.cammWebManager, "MS ADS", externalUserName)
        End Sub

    End Class

    ''' <summary>
    '''     A page which performs an automatic logon procedure with the Active Directory
    ''' </summary>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class LoginWithActiveDirectoryUser
        Inherits LoginWithExternalAccount

        Protected WithEvents IdentifiedUserName As System.Web.UI.WebControls.Label
        Protected WithEvents Messages As System.Web.UI.WebControls.Label
        Protected WithEvents PanelRegisterNew As System.Web.UI.WebControls.Panel
        Protected WithEvents PanelRegisterExisting As System.Web.UI.WebControls.Panel
        Protected WithEvents LoginNameRegisterExisting As System.Web.UI.WebControls.TextBox
        Protected WithEvents LoginPasswordRegisterExisting As System.Web.UI.WebControls.TextBox
        Protected WithEvents LoginNameRegisterNew As System.Web.UI.WebControls.TextBox
        Protected WithEvents LoginPasswordRegisterNew As System.Web.UI.WebControls.TextBox
        Protected WithEvents LoginPassword2RegisterNew As System.Web.UI.WebControls.TextBox
        Protected WithEvents EMailAddressRegisterNew As System.Web.UI.WebControls.TextBox
        Protected WithEvents RadioRegisterExisting As System.Web.UI.WebControls.RadioButton
        Protected WithEvents RadioRegisterNew As System.Web.UI.WebControls.RadioButton
        Protected WithEvents RadioDoNothing As System.Web.UI.WebControls.RadioButton
        Protected WithEvents ButtonNext As System.Web.UI.WebControls.Button
        Protected RegisterUserDocument As System.Web.UI.WebControls.PlaceHolder

        Protected PageTitle As System.Web.UI.WebControls.Literal
        Protected FormTitle As System.Web.UI.WebControls.Label
        Protected FormSubTitle As System.Web.UI.WebControls.Label
        Protected LabelTakeAnAction As System.Web.UI.WebControls.Label
        Protected LabelRegisterExistingLoginName As System.Web.UI.WebControls.Label
        Protected LabelRegisterExistingPassword As System.Web.UI.WebControls.Label
        Protected LabelRegisterNewLoginName As System.Web.UI.WebControls.Label
        Protected LabelRegisterNewPassword As System.Web.UI.WebControls.Label
        Protected LabelRegisterNewPassword2 As System.Web.UI.WebControls.Label
        Protected LabelRegisterNewEMail As System.Web.UI.WebControls.Label
        Protected ContactUs As System.Web.UI.WebControls.Label
        ''' <summary>
        '''     Prepare the page in the case that there is no external account assigned to the CWM user account, yet
        ''' </summary>
        Protected Overrides Sub OnMissingAssignmentOfExternalAccount()
            RegisterUserDocument.Visible = True
            Try
                'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                Dim WorkaroundEx As New Exception("")
                Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                Try
                    WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                Catch
                End Try
                If Not Me.AdsUser Is Nothing Then Me.LogMissingAssignmentOfExternalUserAccount("MS ADS", Me.AdsUser.ExternalAccount, Me.AdsUser.FullName, Me.AdsUser.EMailAddress, "Current StackTrace:" & vbNewLine & WorkaroundStackTrace)
                LoadPageData()
                Localize()
            Catch ex As System.Threading.ThreadAbortException
                Throw
            Catch ex As Exception
                Dim errorData As New System.Collections.Specialized.NameValueCollection
                errorData("exception details") = ex.ToString
                cammWebManager.RedirectTo(RedirectUrlWhenAdsUserCantBeAssignedToCWMUser, "LoginWithActiveDirectoryUser", errorData)
            End Try
        End Sub
        ''' <summary>
        '''     Localize this page
        ''' </summary>
        Protected Overridable Sub Localize()

            Me.cammWebManager.PageTitle = Me.cammWebManager.Internationalization.Logon_SSO_ADS_PageTitle
            Me.PageTitle.Text = Me.cammWebManager.PageTitle
            Me.FormTitle.Text = Me.cammWebManager.PageTitle
            If Not AdsUser Is Nothing Then
                Me.IdentifiedUserName.Text = String.Format(Me.cammWebManager.Internationalization.Logon_SSO_ADS_IdentifiedUserNameWithAdsUserInfo, Page.User.Identity.Name, AdsUser.FullName)
            Else
                Me.IdentifiedUserName.Text = String.Format(Me.cammWebManager.Internationalization.Logon_SSO_ADS_IdentifiedUserName, Page.User.Identity.Name)
            End If
            Me.LabelTakeAnAction.Text = Me.cammWebManager.Internationalization.Logon_SSO_ADS_LabelTakeAnAction
            Me.RadioRegisterExisting.Text = Me.cammWebManager.Internationalization.Logon_SSO_ADS_RadioRegisterExisting
            Me.RadioRegisterNew.Text = Me.cammWebManager.Internationalization.Logon_SSO_ADS_RadioRegisterNew
            Me.RadioDoNothing.Text = Me.cammWebManager.Internationalization.Logon_SSO_ADS_RadioDoNothing
            Me.ContactUs.Text = String.Format(Me.cammWebManager.Internationalization.Logon_SSO_ADS_ContactUs, Me.cammWebManager.StandardEMailAccountAddress)
            Me.ButtonNext.Text = Me.cammWebManager.Internationalization.Logon_SSO_ADS_ButtonNext
            Me.LabelRegisterExistingLoginName.Text = Me.cammWebManager.Internationalization.Logon_SSO_ADS_LabelRegisterExistingLoginName
            Me.LabelRegisterExistingPassword.Text = Me.cammWebManager.Internationalization.Logon_SSO_ADS_LabelRegisterExistingPassword
            Me.LabelRegisterNewPassword2.Text = Me.cammWebManager.Internationalization.Logon_SSO_ADS_LabelRegisterNewPassword2
            Me.LabelRegisterNewEMail.Text = Me.cammWebManager.Internationalization.Logon_SSO_ADS_LabelRegisterNewEMail
            Me.LabelRegisterNewLoginName.Text = Me.LabelRegisterExistingLoginName.Text
            Me.LabelRegisterNewPassword.Text = Me.LabelRegisterExistingPassword.Text
            Me.FormSubTitle.Text = Me.cammWebManager.CurrentServerInfo.ParentServerGroup.Title

        End Sub
        ''' <summary>
        '''     When there is an error or the user cancels the assignment process/form, the page redirects to this URL which is the logon page per default
        ''' </summary>
        ''' <value></value>
        Protected ReadOnly Property RedirectUrlWhenAdsUserCantBeAssignedToCWMUser() As String
            Get
                Return cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL
            End Get
        End Property

        Private AdsUser As CompuMaster.camm.WebManager.WMSystem.UserInformation
        ''' <summary>
        '''     When a user exists in ADS but hasn't got an e-mail address or other must fields are missing, don't ask the user for assigning/creating a new account
        ''' </summary>
        ''' <value></value>
        Protected Overridable ReadOnly Property EnsureMustFieldsInAdsBeforeAsking() As Boolean
            Get
                Return True
            End Get
        End Property
        ''' <summary>
        '''     Show the register page or automatically redirect to the start page
        ''' </summary>
        Protected Overridable Sub LoadPageData()

            AdsUser = SuggestedUserInfoByExternalAccountData
            If Not AdsUser Is Nothing Then
                Me.FillFieldsRequiredAsMinimum(AdsUser)
            End If
            If AdsUser.EMailAddress = Nothing Then
                'Fill with default if defined
                AdsUser.EMailAddress = WMSystem.Configuration.SingleSignOnDefaultEMailAddress
            End If

            If Not Page.IsPostBack Then
                If Not AdsUser Is Nothing Then

                    If Me.AutomateExternalAccountAssignment = AutomationMethod.CreateAccount Then
                        Try
                            'Try to create the account with the existing ADS user information
                            Dim suggestedLoginNames As String() = AdsUser.SuggestedFreeLoginNames()
                            If Not suggestedLoginNames Is Nothing AndAlso suggestedLoginNames.Length > 0 Then
                                AdsUser.LoginName = AdsUser.SuggestedFreeLoginNames()(0)  'Take the first suggestion
                            End If
                            AdsUser.ExternalAccount = Page.User.Identity.Name
                            AdsUser.Save(WMSystem.Configuration.SingleSignOnSuppressUserNotification)
                            RegisterUserDocument.Visible = False
                            Me.ValidateUserCredentialsAndLogon()
                            If Me.cammWebManager.DebugLevel >= WMSystem.DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails Then
                                Dim LogData As String = "The Single-Sign-On module created a new camm Web-Manager user account """ & AdsUser.LoginName & """ (" & AdsUser.FullName() & ") from the external ADS user account """ & AdsUser.ExternalAccount & """"
                                Me.cammWebManager.Log.Write(LogData)
                                Me.cammWebManager.MessagingEMails.SendEMail(Me.cammWebManager.TechnicalServiceEMailAccountName, Me.cammWebManager.TechnicalServiceEMailAccountAddress, "SSO successfully created a new user account", LogData, Nothing, Me.cammWebManager.StandardEMailAccountName, Me.cammWebManager.StandardEMailAccountAddress)
                            End If
                            'Me.ButtonNext_Click(Nothing, Nothing)
                        Catch ex As Exception
                            'In case of errors (e. g. unique loginname can't be provided), do nothing
                            If Not Me.AdsUser Is Nothing Then Me.LogMissingAssignmentOfExternalUserAccount("MS ADS", Me.AdsUser.ExternalAccount, Me.AdsUser.FullName, Me.AdsUser.EMailAddress, ex.ToString)
                            Dim err As New Collections.Specialized.NameValueCollection
                            err("exception") = ex.ToString
                            If Me.cammWebManager.DebugLevel >= WMSystem.DebugLevels.Low_WarningMessagesOnAccessError Then
                                Dim LogData As String = "The Single-Sign-On module failed creating a new camm Web-Manager account for """ & AdsUser.LoginName & """ (" & AdsUser.FullName() & ") from the external ADS user account """ & AdsUser.ExternalAccount & """" & vbNewLine & vbNewLine & "This may be caused by unsufficient data in the ADS account. Please check the details for further analysis!" & vbNewLine & vbNewLine & "There was following error:" & vbNewLine & ex.ToString
                                Me.cammWebManager.Log.Warn(ex)
                                Me.cammWebManager.MessagingEMails.SendEMail(Me.cammWebManager.TechnicalServiceEMailAccountName, Me.cammWebManager.TechnicalServiceEMailAccountAddress, "SSO Synchronization Warning", LogData, Nothing, Me.cammWebManager.StandardEMailAccountName, Me.cammWebManager.StandardEMailAccountAddress)
                            End If
                            cammWebManager.RedirectTo(RedirectUrlWhenAdsUserCantBeAssignedToCWMUser, "LoginWithActiveDirectoryUser - predefined - CreateAccount failed", err)
                        End Try
                    ElseIf Me.AutomateExternalAccountAssignment = AutomationMethod.DoNothing Then
                        cammWebManager.RedirectTo(RedirectUrlWhenAdsUserCantBeAssignedToCWMUser, "LoginWithActiveDirectoryUser - predefined - DoNothing", Nothing)
                    ElseIf Me.AutomateExternalAccountAssignment = AutomationMethod.UserDefined Then
                        'Prefill the formular
                        Dim ExistingUser As CompuMaster.camm.WebManager.WMSystem.UserInformation = SearchExistingUser(AdsUser)
                        If ExistingUser Is Nothing Then
                            If EnsureMustFieldsInAdsBeforeAsking AndAlso (AdsUser.EMailAddress = Nothing OrElse AdsUser.FirstName = Nothing OrElse AdsUser.LastName = Nothing) Then
                                'Missing must fields - do not bug the user with automatic logon
                                cammWebManager.RedirectTo(RedirectUrlWhenAdsUserCantBeAssignedToCWMUser, "LoginWithActiveDirectoryUser", Nothing)
                            Else
                                'Suggest to create new user account
                                Me.RadioRegisterNew.Checked = True
                                EMailAddressRegisterNew.Text = AdsUser.EMailAddress
                                Dim suggestedLoginNames As String() = AdsUser.SuggestedFreeLoginNames()
                                If Not suggestedLoginNames Is Nothing AndAlso suggestedLoginNames.Length > 0 Then
                                    LoginNameRegisterNew.Text = AdsUser.SuggestedFreeLoginNames()(0) 'Take the first suggestion
                                End If
                            End If
                        Else
                            'Suggest to assign to an existing user account
                            Me.RadioRegisterExisting.Checked = True
                            LoginNameRegisterExisting.Text = ExistingUser.LoginName
                        End If

                    Else
                        Throw New NotSupportedException("Invalid value for AutomationMethod")
                    End If
                End If
            End If

            Messages.Text = Nothing
        End Sub
        ''' <summary>
        '''     Possible automation methods
        ''' </summary>
        Protected Enum AutomationMethod As Byte
            ''' <summary>
            '''     Show a form to the user where he can decide to assign an existing, create a new account or to proceed without doing anything
            ''' </summary>
            UserDefined = 0
            ''' <summary>
            '''     Automatically create a new account when the required fields are available
            ''' </summary>
            CreateAccount = 1
            ''' <summary>
            '''     Never assign anything, don't ask, just forward the user without logging on
            ''' </summary>
            DoNothing = 2
        End Enum
        ''' <summary>
        '''     What shall happen when a user has been identified through ADS and doesn't exist in our camm Web-Manager user account system?
        ''' </summary>
        ''' <value></value>
        Protected Overridable ReadOnly Property AutomateExternalAccountAssignment() As AutomationMethod
            Get
                Return AutomationMethod.UserDefined
            End Get
        End Property
        ''' <summary>
        '''     Provide additional user information which is not available by the external account system
        ''' </summary>
        ''' <param name="user">The user profile as it has been read already</param>
        Protected Overridable Sub FillFieldsRequiredAsMinimum(ByVal user As CompuMaster.camm.WebManager.IUserInformation)
        End Sub
        ''' <summary>
        '''     Search for an already existing user account with the same first and last name
        ''' </summary>
        ''' <returns>The found user information object if there is only one matching user account available, otherwise in case of no match or several matches it will be nothing</returns>
        Private Function SearchExistingUser(ByVal adsUser As CompuMaster.camm.WebManager.WMSystem.UserInformation) As CompuMaster.camm.WebManager.WMSystem.UserInformation

            Try 'For the case that the search executes with exceptions (in case of some special charachters which would break down the SQL query execution (e. g. "[", "]" or "'")
                Dim UserFilter As WMSystem.UserFilter() = New WMSystem.UserFilter() {New WMSystem.UserFilter("firstname", WMSystem.UserFilter.SearchMethods.MatchExactly), New WMSystem.UserFilter("lastname", WMSystem.UserFilter.SearchMethods.MatchExactly)}
                UserFilter(0).MatchExpressions = New String() {adsUser.FirstName}
                UserFilter(1).MatchExpressions = New String() {adsUser.LastName}
                Dim UserSortArgument As WMSystem.UserSortArgument() = Nothing

                Dim UserIDs As Long() = Me.cammWebManager.SearchUsers(UserFilter, UserSortArgument)
                If UserIDs.Length = 1 Then
                    Return cammWebManager.System_GetUserInfo(UserIDs(0))
                End If
            Catch
            End Try
            Return Nothing

        End Function

        Sub RadioRegisterNew_Change(ByVal sender As Object, ByVal e As EventArgs) Handles RadioRegisterNew.CheckedChanged
            PanelRegisterNew.Visible = RadioRegisterNew.Checked
        End Sub

        Sub ButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonNext.Click
            Try
                If RadioRegisterExisting.Checked Then
                    'Validate the text boxes
                    Dim Valid As Boolean = True
                    Dim ExistingUserAccountID As Long
                    If Trim(LoginNameRegisterExisting.Text) = Nothing Then
                        AppendValidationErrorMessage("Please provide a login name")
                        Valid = False
                    Else
                        ExistingUserAccountID = CType(cammWebManager.System_GetUserID(Mid(Trim(LoginNameRegisterExisting.Text), 1, 20), True), Long)
                        If ExistingUserAccountID = -1& Then
                            AppendValidationErrorMessage("The given login name doesn't exist")
                            Valid = False
                        End If
                    End If
                    Dim ExistingUserAccountInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = Nothing
                    If Trim(LoginPasswordRegisterExisting.Text) = Nothing Then
                        AppendValidationErrorMessage("Please provide a password")
                        Valid = False
                    Else
                        ExistingUserAccountInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(ExistingUserAccountID, cammWebManager)
                        If ExistingUserAccountInfo.ValidatePassword(Trim(LoginPasswordRegisterExisting.Text)) = False Then
                            AppendValidationErrorMessage("The password hasn't been retyped correctly")
                            Valid = False
                        End If
                    End If
                    If Valid AndAlso Not ExistingUserAccountInfo Is Nothing Then
                        ExistingUserAccountInfo.LoginName = Mid(Trim(LoginNameRegisterExisting.Text), 1, 20)
                        ExistingUserAccountInfo.ExternalAccount = Page.User.Identity.Name
                        ExistingUserAccountInfo.Save(True)
                        'Re-validate now - this should now be possible successfully
                        RegisterUserDocument.Visible = False
                        Me.ValidateUserCredentialsAndLogon()
                    End If
                ElseIf RadioRegisterNew.Checked Then
                    'Validate the text boxes
                    Dim Valid As Boolean = True
                    If Trim(LoginNameRegisterNew.Text) = Nothing Then
                        AppendValidationErrorMessage("Please provide a login name")
                        Valid = False
                    ElseIf CType(cammWebManager.System_GetUserID(Mid(Trim(LoginNameRegisterNew.Text), 1, 20), True), Long) <> -1% Then
                        AppendValidationErrorMessage("The given login name is already in use, please choose another one")
                        Valid = False
                    End If
                    If Trim(LoginPasswordRegisterNew.Text) = Nothing Then
                        AppendValidationErrorMessage("Please provide a password")
                        Valid = False
                    ElseIf Me.LoginPasswordRegisterNew.Text <> Me.LoginPassword2RegisterNew.Text Then
                        AppendValidationErrorMessage("The password hasn't been retyped correctly")
                        Valid = False
                    End If
                    If Not System.Text.RegularExpressions.Regex.IsMatch(Me.EMailAddressRegisterNew.Text, "\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*") Then
                        AppendValidationErrorMessage("Your e-mail address must be valid")
                        Valid = False
                    End If
                    'Create the new account, now
                    If Valid Then
                        AdsUser.LoginName = Mid(Trim(LoginNameRegisterNew.Text), 1, 20)
                        AdsUser.EMailAddress = Trim(Me.EMailAddressRegisterNew.Text)
                        AdsUser.ExternalAccount = Page.User.Identity.Name
                        AdsUser.Save(Trim(LoginPasswordRegisterNew.Text))
                        'Re-validate now - this should now be possible successfully
                        RegisterUserDocument.Visible = False
                        Me.ValidateUserCredentialsAndLogon()
                    End If
                ElseIf RadioDoNothing.Checked Then
                    cammWebManager.RedirectTo(RedirectUrlWhenAdsUserCantBeAssignedToCWMUser, "LoginWithActiveDirectoryUser - no action selected", Nothing)
                Else
                    'Nothing selected
                    AppendValidationErrorMessage("Please select one of the options")
                End If
            Catch ex As Exception
                AppendValidationErrorMessage(ex.Message)
            End Try
        End Sub
        ''' <summary>
        '''     Add an additional error message
        ''' </summary>
        ''' <param name="text"></param>
        Sub AppendValidationErrorMessage(ByVal text As String)
            If Me.Messages.Text = Nothing Then
                Me.Messages.Text = text
            Else
                Me.Messages.Text &= "<br>" & text
            End If
        End Sub
        ''' <summary>
        '''     This is the name of the LDAP server which shall be asked for additional user details when the external account hasn't been assigned to a camm Web-Manager user account, yet
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     By default, this is the NETBIOS name of the user's domain which is the domain server, regulary
        ''' </remarks>
        Protected Overridable ReadOnly Property LdapServerName() As String
            Get
                Return Mid(Page.User.Identity.Name, 1, System.Math.Max(InStr(Page.User.Identity.Name, "\") - 1, 0))
            End Get
        End Property

        Protected ReadOnly Property SuggestedUserInfoByExternalAccountData() As CompuMaster.camm.WebManager.WMSystem.UserInformation
            Get
                Dim DomainName As String 'The first part of the identity's name
                Dim AccountName As String 'The second part of the identity's name
                DomainName = Page.User.Identity.Name.Substring(0, System.Math.Max(Page.User.Identity.Name.IndexOf("\"), 0))
                AccountName = Page.User.Identity.Name.Substring(InStr(Page.User.Identity.Name, "\"))

                If LdapServerName = Nothing Then
                    Throw New Exception("""" & Page.User.Identity.Name & """ must contain a domain part and and user account name part")
                End If

                Dim userdata As DataTable
                userdata = CompuMaster.camm.WebManager.Tools.Data.Ldap.QueryUsersByAccountName(LdapServerName, AccountName)
                If Not userdata Is Nothing AndAlso userdata.Rows.Count = 1 Then
                    Dim UserDataRow As DataRow = userdata.Rows(0)
                    Dim Result As CompuMaster.camm.WebManager.WMSystem.UserInformation
                    Result = New CompuMaster.camm.WebManager.WMSystem.UserInformation(
                        0&,
                        "",
                        CompuMaster.camm.WebManager.Utils.Nz(UserDataRow("EMail"), ""),
                        False,
                        CompuMaster.camm.WebManager.Utils.Nz(UserDataRow("Company"), ""),
                        WMSystem.Sex.Undefined,
                        "",
                        CompuMaster.camm.WebManager.Utils.Nz(UserDataRow("FirstName"), ""),
                        CompuMaster.camm.WebManager.Utils.Nz(UserDataRow("LastName"), ""),
                        "",
                        CompuMaster.camm.WebManager.Utils.Nz(UserDataRow("Street"), ""),
                        CompuMaster.camm.WebManager.Utils.Nz(UserDataRow("ZipCode"), ""),
                        CompuMaster.camm.WebManager.Utils.Nz(UserDataRow("City"), ""),
                        "",
                        CompuMaster.camm.WebManager.Utils.Nz(UserDataRow("Country"), ""),
                        Me.cammWebManager.UI.MarketID,
                        0,
                        0,
                        False,
                        False,
                        False,
                        Me.cammWebManager.CurrentServerInfo.ParentServerGroup.AccessLevelDefault.ID,
                        Me.cammWebManager,
                        Page.User.Identity.Name)
                    Result.MobileNumber = CompuMaster.camm.WebManager.Utils.Nz(UserDataRow("MobilePhone"), "")
                    Result.PhoneNumber = CompuMaster.camm.WebManager.Utils.Nz(UserDataRow("Phone"), "")
                    Result.FaxNumber = CompuMaster.camm.WebManager.Utils.Nz(UserDataRow("Fax"), "")
                    Result.Position = CompuMaster.camm.WebManager.Utils.Nz(UserDataRow("Title"), "")
                    Result.AdditionalFlags("Department") = CompuMaster.camm.WebManager.Utils.Nz(UserDataRow("Department"), CType(Nothing, String))

                    'Manager
                    Dim ManagerLdapName As String = WebManager.Utils.Nz(UserDataRow("Manager"), "")
                    If ManagerLdapName <> Nothing Then
                        Dim managerTable As DataTable = CompuMaster.camm.WebManager.Tools.Data.Ldap.Query(ManagerLdapName, Nothing)
                        Dim ManagerAdsAccountName As String = Nothing
                        If managerTable.Rows.Count = 1 Then
                            ManagerAdsAccountName = WebManager.Utils.Nz(managerTable.Rows(0)("sAMAccountName"), CType(Nothing, String))
                        End If
                        Dim ManagerUserID As Long
                        ManagerUserID = LookupUserIDFromExternalAccountName(Me.cammWebManager, DomainName & "\" & ManagerAdsAccountName)
                        If ManagerUserID = Nothing Then
                            Result.AdditionalFlags("ManagerID") = Nothing
                        Else
                            Result.AdditionalFlags("ManagerID") = ManagerUserID.ToString
                        End If
                    End If

                    'Use a free login name
                    Dim PossibleLoginNames As String() = Result.SuggestedFreeLoginNames
                    If PossibleLoginNames.Length = 0 Then
                        'Result.LoginName = Nz(UserDataRow("UserName"), "")
                        Result.LoginName = Mid(CompuMaster.camm.WebManager.Utils.Nz(UserDataRow("UserName"), ""), 1, 20)
                    Else
                        Result.LoginName = PossibleLoginNames(0)
                    End If

                    If Result.LastName = Nothing Then
                        Result.LastName = Result.LoginName
                    End If
                    If Result.FirstName = Nothing Then
                        Result.FirstName = Result.LoginName
                    End If

                    Return Result
                Else
                    Return Nothing
                End If
            End Get
        End Property
        ''' <summary>
        '''     Find a CWM user with a defined external account name and return its ID
        ''' </summary>
        ''' <param name="webManager">The current CWM which shall be used for searching for users with an external account information</param>
        ''' <param name="adsAccountName">The searched value</param>
        ''' <returns>A camm Web-Manager user ID or 0 if not found</returns>
        Private Function LookupUserIDFromExternalAccountName(ByVal webmanager As CompuMaster.camm.WebManager.WMSystem, ByVal adsAccountName As String) As Long
            Me.cammWebManager.Log.RuntimeInformation("SSO ADS / identified ADS user is: " & adsAccountName, WMSystem.DebugLevels.Medium_LoggingOfDebugInformation)
            Dim userIDs As Long() = webmanager.SearchUsers(New CompuMaster.camm.WebManager.WMSystem.UserFilter() {New CompuMaster.camm.WebManager.WMSystem.UserFilter("ExternalAccount", CompuMaster.camm.WebManager.WMSystem.UserFilter.SearchMethods.MatchExactly, New String() {adsAccountName})}, Nothing)
            If userIDs Is Nothing OrElse userIDs.Length > 1 Then
                Throw New InvalidOperationException("ExternalAccount field must be unique, but doubled entry has been found: """ & adsAccountName & """")
            ElseIf userIDs.Length = 0 Then
                Return Nothing
            Else
                Me.MarkExternalUserAccountAsAssigned("MS ADS", adsAccountName)
                Me.cammWebManager.Log.RuntimeInformation("SSO ADS / identified CWM user ID " & userIDs(0) & " from ADS user: " & adsAccountName, WMSystem.DebugLevels.Medium_LoggingOfDebugInformation)
                Return userIDs(0)
            End If
        End Function

    End Class

    ''' <summary>
    '''     An alternative start page for the root folder to redirect the browser to the single sign on page
    ''' </summary>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class RootPageForRedirectToLogonPageWithExternalAccount
        Inherits Page

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            If Request.Url.PathAndQuery = cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL AndAlso cammWebManager.CurrentServerInfo.ParentServerGroup.MasterServer.IPAddressOrHostHeader = cammWebManager.CurrentServerIdentString Then
                'Fact is: the URL is the CWM start URL and this we're currently on the master server
                'In this situation, a failed or ignored SSO process would lead to an endless loop
                'So show an exception message, here
                Response.Clear()
                Dim problemDescription As String = "This script is not allowed as start page of camm Web-Manager"
                Dim actionSuggestion As String = "Change the Internationalization.User_Auth_Validation_NoRefererURL property to a different address"
                cammWebManager.Log.Exception(problemDescription & vbNewLine & actionSuggestion, False)
                Response.Write("<html><body><h1>" & problemDescription & "<h1><p>" & actionSuggestion & "</p></body></html>")
                Response.End()
            ElseIf WebManager.Utils.IsRequestFromCrawlerAgent(Me.Request) Then
                'We can send a crawler always to the regular non-SSO-url
                Me.RedirectPermanently(cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL)
            ElseIf cammWebManager.IsLoggedOn = False Then
                cammWebManager.RedirectTo(LogonPageWithExternalAccount)
            Else
                cammWebManager.RedirectTo(cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL)
            End If

        End Sub

        Public Overridable ReadOnly Property LogonPageWithExternalAccount() As String
            Get
                'return "/sysdata/login/sso/"
                Return cammWebManager.Internationalization.User_Auth_Config_Paths_Login & "sso/"
            End Get
        End Property

    End Class

    ''' <summary>
    '''     The page which is responsible for all situations when the user can't be authorized successfully
    ''' </summary>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class NotAuthorizedWithExternalAccount
        Inherits Page

        Protected Overridable Sub OnPageLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.cammWebManager.RedirectTo(Me.cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL, "OnMissingAuthentication", Nothing)
        End Sub

    End Class

End Namespace