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

#Region "Public Class CreateUserProfile"
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
        ''' <returns></returns>
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
        ''' <returns></returns>
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
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected MustOverride Function CreateUserInfo() As WebManager.IUserInformation

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Overridable method for customized actions after the new user account has been written
        ''' </summary>
        ''' <param name="userInfo">The created user account</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	31.08.2009	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub AfterUserCreation(ByVal userInfo As WebManager.IUserInformation)
        End Sub

        ''' <summary>
        ''' Pointing to the textbox with the new password which shall be used for the new user account
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected MustOverride Property NewUserPassword() As String

        ''' <summary>
        ''' Finally write the user account
        ''' </summary>
        ''' <param name="userInfo"></param>
        ''' <returns></returns>
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Prohibited password parts for password complexity check
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	12.12.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected MustOverride Function PasswordSeverityCheckStrings() As String()

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fill the user profile with the new data which shall be saved
        ''' </summary>
        ''' <param name="userInfo">The current user profile which shall be created</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	13.06.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected MustOverride Sub FillUserAccount(ByVal userInfo As WebManager.IUserInformation)

    End Class

    <System.Runtime.InteropServices.ComVisible(False)> Public Class CreateUserProfile
        Inherits BaseCreateUserProfile

        Protected TextboxLoginName As Web.UI.WebControls.TextBox
        Protected TextboxPassword1 As Web.UI.WebControls.TextBox
        Protected TextboxPassword2 As Web.UI.WebControls.TextBox
        Protected DropdownSalutation As Web.UI.WebControls.DropDownList
        Protected TextboxCompany As Web.UI.WebControls.TextBox
        Protected TextboxAcademicTitle As Web.UI.WebControls.TextBox
        Protected TextboxFirstName As Web.UI.WebControls.TextBox
        Protected TextboxNameAffix As Web.UI.WebControls.TextBox
        Protected TextboxLastName As Web.UI.WebControls.TextBox
        Protected TextboxEMail As Web.UI.WebControls.TextBox
        Protected TextboxStreet As Web.UI.WebControls.TextBox
        Protected TextboxZipCode As Web.UI.WebControls.TextBox
        Protected TextboxLocation As Web.UI.WebControls.TextBox
        Protected TextboxState As Web.UI.WebControls.TextBox
        Protected TextboxCountry As Web.UI.WebControls.TextBox
        Protected TextboxPhone As Web.UI.WebControls.TextBox
        Protected TextboxFax As Web.UI.WebControls.TextBox
        Protected TextboxMobile As Web.UI.WebControls.TextBox
        Protected TextboxPositionInCompany As Web.UI.WebControls.TextBox
        Protected Dropdown1stPreferredLanguage As Web.UI.WebControls.DropDownList
        Protected Dropdown2ndPreferredLanguage As Web.UI.WebControls.DropDownList
        Protected Dropdown3rdPreferredLanguage As Web.UI.WebControls.DropDownList
        Protected RadioListComesFrom As Web.UI.WebControls.RadioButtonList
        Protected CheckboxListMotivation As Web.UI.WebControls.CheckBoxList
        Protected TextboxComment As Web.UI.WebControls.TextBox
        Protected ComesFromOtherText As Web.UI.WebControls.TextBox
        Protected MotivationOtherText As Web.UI.WebControls.TextBox
        Protected ValidatorSummary As Web.UI.WebControls.ValidationSummary
        Protected WithEvents ValidatorFirstName As Web.UI.WebControls.RequiredFieldValidator
        Protected WithEvents ValidatorLastName As Web.UI.WebControls.RequiredFieldValidator
        Protected WithEvents ValidatorSalutation As Web.UI.WebControls.RequiredFieldValidator
        Protected WithEvents ValidatorLoginName As Web.UI.WebControls.CustomValidator
        Protected WithEvents ValidatorPassword1 As Web.UI.WebControls.CustomValidator
        Protected WithEvents ValidatorPassword2 As Web.UI.WebControls.CustomValidator
        Protected WithEvents ValidatorEMail As Web.UI.WebControls.CustomValidator
        Protected WithEvents SubmitButton As Web.UI.WebControls.Button

        Protected Overrides Property NewUserPassword() As String
            Get
                Return Me.TextboxPassword1.Text
            End Get
            Set(ByVal value As String)
                Me.TextboxPassword1.Text = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Prohibited password parts for password complexity check
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	12.12.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function PasswordSeverityCheckStrings() As String()
            Dim MyString(2) As String
            MyString(0) = Mid(Me.TextboxLoginName.Text, 1, 4)
            MyString(1) = Mid(Me.TextboxFirstName.Text, 1, 4)
            MyString(2) = Mid(Me.TextboxLastName.Text, 1, 4)
            Return MyString
        End Function


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fill the user profile with the new data which shall be saved
        ''' </summary>
        ''' <param name="userInfo">The current user profile which shall be created</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	13.06.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub FillUserAccount(ByVal userInfo As WebManager.IUserInformation)

            'Already prefilled values
            'userInfo.Gender = CType(IIf(CStr(Me.DropdownSalutation.SelectedValue) = "Ms.", WMSystem.Sex.Feminin, WMSystem.Sex.Masculin), WMSystem.Sex)
            'userInfo.PreferredLanguage1 = New WMSystem.LanguageInformation(CType(Me.Dropdown1stPreferredLanguage.SelectedValue, Integer), cammWebManager)
            'userInfo.PreferredLanguage2 = New WMSystem.LanguageInformation(CType(Utils.StringNotEmptyOrNothing(Me.Dropdown2ndPreferredLanguage.SelectedValue), Integer), cammWebManager)
            'userInfo.PreferredLanguage3 = New WMSystem.LanguageInformation(CType(Utils.StringNotEmptyOrNothing(Me.Dropdown3rdPreferredLanguage.SelectedValue), Integer), cammWebManager)
            'userInfo.Company = Me.TextboxCompany.Text
            'userInfo.AcademicTitle = Me.TextboxAcademicTitle.Text
            'userInfo.FirstName = Me.TextboxFirstName.Text
            'userInfo.LastName = Me.TextboxLastName.Text
            'userInfo.NameAddition = Me.TextboxNameAffix.Text
            'userInfo.EMailAddress = Me.TextboxEMail.Text
            'userInfo.Street = Me.TextboxStreet.Text
            'userInfo.ZipCode = Me.TextboxZipCode.Text
            'userInfo.Location = Me.TextboxLocation.Text
            'userInfo.State = Me.TextboxState.Text
            'userInfo.Country = Me.TextboxCountry.Text

            'Additional fields
            userInfo.PhoneNumber = Me.TextboxPhone.Text
            userInfo.FaxNumber = Me.TextboxFax.Text
            userInfo.MobileNumber = Me.TextboxMobile.Text
            userInfo.Position = Me.TextboxPositionInCompany.Text
            userInfo.AdditionalFlags("OnCreationComment") = Me.TextboxComment.Text
            userInfo.AdditionalFlags("Motivation") = CollectMotivationDetails()
            userInfo.AdditionalFlags("ComesFrom") = CollectComesFromDetails()

        End Sub

        Private Function CollectMotivationDetails() As String
            Dim Result As String = Nothing
            If Not Me.CheckboxListMotivation Is Nothing Then
                For Each item As Web.UI.WebControls.ListItem In Me.CheckboxListMotivation.Items
                    If item.Selected Then
                        If Result = Nothing Then
                            Result = item.Value
                        Else
                            Result &= ", " & item.Value
                        End If
                    End If
                Next
                If Me.MotivationOtherText.Text <> Nothing Then
                    Result &= " (" & Me.MotivationOtherText.Text & ")"
                End If
            End If
            Return Result
        End Function

        Private Function CollectComesFromDetails() As String
            Dim Result As String = Nothing
            If Not Me.RadioListComesFrom Is Nothing Then
                For Each item As Web.UI.WebControls.ListItem In Me.RadioListComesFrom.Items
                    If item.Selected Then
                        If Result = Nothing Then
                            Result = item.Value
                        Else
                            Result &= ", " & item.Value
                        End If
                    End If
                Next
                If Me.ComesFromOtherText.Text <> Nothing Then
                    Result &= " (" & Me.ComesFromOtherText.Text & ")"
                End If
            End If
            Return Result
        End Function

        Protected Overrides Function CreateUserInfo() As WebManager.IUserInformation

            Dim MyUserInfo As New WMSystem.UserInformation(0&, _
                Trim(Mid(Trim(Me.TextboxLoginName.Text), 1, 20)), _
                Trim(Me.TextboxEMail.Text), _
                False, _
                Trim(Me.TextboxCompany.Text), _
                CType(IIf(Me.DropdownSalutation.SelectedValue = "Ms.", WMSystem.Sex.Feminin, IIf(Me.DropdownSalutation.SelectedValue = "Mr.", WMSystem.Sex.Masculin, WMSystem.Sex.Undefined)), WMSystem.Sex), _
                Trim(Me.TextboxNameAffix.Text), _
                Trim(Me.TextboxFirstName.Text), _
                Trim(Me.TextboxLastName.Text), _
                Trim(Me.TextboxAcademicTitle.Text), _
                Trim(Me.TextboxStreet.Text), _
                Trim(Me.TextboxZipCode.Text), _
                Trim(Me.TextboxLocation.Text), _
                Trim(Me.TextboxState.Text), _
                Trim(Me.TextboxCountry.Text), _
                CInt(Me.Dropdown1stPreferredLanguage.SelectedValue), _
                Utils.TryCInt(Me.Dropdown2ndPreferredLanguage.SelectedValue), _
                Utils.TryCInt(Me.Dropdown3rdPreferredLanguage.SelectedValue), _
                False, _
                False, _
                False, _
                Me.AccessLevelDefault, _
                CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem), _
                CType(Nothing, String), _
                New Collections.Specialized.NameValueCollection)
            Return MyUserInfo

        End Function

        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'Perform actions only when this is a post back
            If Page.IsPostBack Then

                Page.Validate()
                If Page.IsValid Then
                    CollectDataAndCreateAccountAndStartLoginWorkflow()
                End If
            End If

        End Sub

        Protected Overrides Sub ShowErrorMessage(ByVal message As String)
            Me.Validators.Add(New DummyValidatorForAppearanceInValidationSummary(message))
            Page.Validate()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Create a string with OPTION tags for all activated languages for embedding into the SELECT tag
        ''' </summary>
        ''' <param name="languageDropdownList">A dropdownlist control which shall be filled</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	12.12.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub FillMarketsList(ByVal languageDropdownList As System.Web.UI.WebControls.DropDownList)

            Dim MarketList As WMSystem.LanguageInformation() = cammWebManager.System_GetLanguagesInfo(False)

            Dim sortedList As New Collections.SortedList

            For Each market As WMSystem.LanguageInformation In MarketList
                If market.ID <> 10000 Then
                    sortedList.Add(market.LanguageName_OwnLanguage, market.ID)
                End If
            Next

            For Each market As DictionaryEntry In sortedList
                languageDropdownList.Items.Add(New Web.UI.WebControls.ListItem(CStr(market.Key), CStr(market.Value)))
            Next

        End Sub

        Private Sub Localization()
            cammWebManager.PageTitle = cammWebManager.Internationalization.OfficialServerGroup_Title & " - " & cammWebManager.Internationalization.CreateAccount_Descr_PageTitle

            If Not Page.IsPostBack Then
                Me.SubmitButton.Text = cammWebManager.Internationalization.CreateAccount_Descr_Submit

                'Add salutation dropdown content
                Me.DropdownSalutation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_PleaseSelect, ""))
                Me.DropdownSalutation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Abbrev_Mister, "Mr."))
                Me.DropdownSalutation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Abbrev_Miss, "Ms."))

                'Fill markets/languages dropdowns
                Me.Dropdown1stPreferredLanguage.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_PleaseSelect, ""))
                FillMarketsList(Me.Dropdown1stPreferredLanguage)
                Me.Dropdown2ndPreferredLanguage.Items.Add(New Web.UI.WebControls.ListItem("", ""))
                FillMarketsList(Me.Dropdown2ndPreferredLanguage)
                Me.Dropdown3rdPreferredLanguage.Items.Add(New Web.UI.WebControls.ListItem("", ""))
                FillMarketsList(Me.Dropdown3rdPreferredLanguage)

                'CheckboxList Motivation
                If Not Me.CheckboxListMotivation Is Nothing Then
                    Me.CheckboxListMotivation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_MotivItemWebSiteVisitor, "Visitor"))
                    Me.CheckboxListMotivation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_MotivItemDealer, "Dealer"))
                    Me.CheckboxListMotivation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_MotivItemJournalist, "Journalist"))
                    Me.CheckboxListMotivation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_MotivItemSupplier, "Supplier"))
                    Me.CheckboxListMotivation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_MotivItemOther, "Other"))
                End If

                'RadioList ComesFrom
                If Not Me.RadioListComesFrom Is Nothing Then
                    Me.RadioListComesFrom.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_WhereItemFriend, "Friend"))
                    Me.RadioListComesFrom.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_WhereItemResellerDealer, "Dealer"))
                    Me.RadioListComesFrom.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_WhereItemExhibition, "Exhibition"))
                    Me.RadioListComesFrom.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_WhereItemMagazines, "Magazine"))
                    Me.RadioListComesFrom.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_WhereItemSearchEnginge, "SearchEngine"))
                    Me.RadioListComesFrom.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_WhereItemOther, "Other"))
                End If

                'Validator texts
                Me.ValidatorLoginName.Text = CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.CreateAccount_ErrorJS_Length.Replace("\""", """"), 6, cammWebManager.Internationalization.CreateAccount_Descr_NewLoginName, "", "", "", "", "", "", "", "", "", "")
                Me.ValidatorLoginName.Display = Web.UI.WebControls.ValidatorDisplay.Dynamic
                Me.ValidatorFirstName.Text = LocalizedTextRequiredField
                Me.ValidatorFirstName.EnableClientScript = True
                Me.ValidatorLastName.Text = LocalizedTextRequiredField
                Me.ValidatorLastName.EnableClientScript = True
                Me.ValidatorLastName.Text = LocalizedTextRequiredField
                Me.ValidatorLastName.EnableClientScript = True
                Me.ValidatorSalutation.Text = LocalizedTextRequiredField
                Me.ValidatorSalutation.EnableClientScript = True
                'me.ValidatorPassword1.Text=CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.CreateAccount_ErrorJS_Length.Replace("\""",""""), cammWebManager.PasswordSecurity.InspectionSeverities(cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "accesslevel_default").ID).RequiredPasswordLength, cammWebManager.Internationalization.CreateAccount_Descr_NewLoginPassword, "", "", "", "", "", "", "", "", "", "")
            End If

            'Remove Please-Select-Dropdown-Element as soon as possible
            If Me.DropdownSalutation.SelectedValue <> "" Then
                If Me.DropdownSalutation.Items.Contains(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_PleaseSelect, "")) Then
                    Me.DropdownSalutation.Items.Remove(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_PleaseSelect, ""))
                End If
            End If
            If Me.Dropdown1stPreferredLanguage.SelectedValue <> "" Then
                If Me.Dropdown1stPreferredLanguage.Items.Contains(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_PleaseSelect, "")) Then
                    Me.Dropdown1stPreferredLanguage.Items.Remove(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_PleaseSelect, ""))
                End If
            End If

            Me.DataBind()

        End Sub

        Private Sub PageOnInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Server.ScriptTimeout = 10

            'Ensure maximum login field length
            If Me.TextboxLoginName.MaxLength <= 0 OrElse Me.TextboxLoginName.MaxLength > 20 Then
                Me.TextboxLoginName.MaxLength = 20
            End If
            If Me.TextboxPassword1.MaxLength <= 0 OrElse Me.TextboxPassword1.MaxLength > 30 Then
                Me.TextboxPassword1.MaxLength = 30
            End If

            Localization()
        End Sub

#Region "Custom validators"

        Private Sub CustomValidatorPassword1_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles ValidatorPassword1.ServerValidate
            If Me.TextboxPassword1.Text = Nothing Then
                Me.ValidatorPassword1.Text = Me.LocalizedTextRequiredField
                args.IsValid = False
            ElseIf Me.TextboxPassword1.Text.Length < cammWebManager.PasswordSecurity.InspectionSeverities(AccessLevelDefault).RequiredPasswordLength Then
                Me.ValidatorPassword1.Text = CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.CreateAccount_ErrorJS_Length.Replace("\""", """").Replace("\""", """"), cammWebManager.PasswordSecurity.InspectionSeverities(AccessLevelDefault).RequiredPasswordLength, cammWebManager.Internationalization.CreateAccount_Descr_NewLoginPassword, "", "", "", "", "", "", "", "", "", "")
                args.IsValid = False
            ElseIf Me.TextboxPassword1.Text.Length > cammWebManager.PasswordSecurity.InspectionSeverities(AccessLevelDefault).RequiredMaximumPasswordLength Then
                Me.ValidatorPassword1.Text = "Too many letters"
                args.IsValid = False
            ElseIf cammWebManager.PasswordSecurity.InspectionSeverities(AccessLevelDefault).ValidatePasswordComplexity(Me.TextboxPassword1.Text, Me.PasswordSeverityCheckStrings) <> CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Success Then
                Me.ValidatorPassword1.Text = cammWebManager.PasswordSecurity.InspectionSeverities(AccessLevelDefault).ErrorMessageComplexityPoints(cammWebManager.UI.MarketID)
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        End Sub

        Private Sub CustomValidatorPassword2_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles ValidatorPassword2.ServerValidate
            If Me.TextboxPassword2.Text = Nothing Then
                Me.ValidatorPassword2.Text = Me.LocalizedTextRequiredField
                args.IsValid = False
            ElseIf Me.TextboxPassword2.Text <> Me.TextboxPassword1.Text Then
                Me.ValidatorPassword2.Text = cammWebManager.Internationalization.UpdateProfile_ErrMsg_MistypedPW
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        End Sub

        Private Sub CustomValidatorLoginName_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles ValidatorLoginName.ServerValidate
            If Me.TextboxLoginName.Text = Nothing Then
                Me.ValidatorLoginName.Text = Me.LocalizedTextRequiredField
                args.IsValid = False
            ElseIf Me.TextboxLoginName.Text.Length < 6 Then
                Me.ValidatorLoginName.Text = CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.CreateAccount_ErrorJS_Length.Replace("\""", """"), 6, cammWebManager.Internationalization.CreateAccount_Descr_NewLoginName, "", "", "", "", "", "", "", "", "", "")
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        End Sub

        Private Sub CustomValidatorEMail_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles ValidatorEMail.ServerValidate
            If Me.TextboxEMail.Text = Nothing Then
                Me.ValidatorEMail.Text = Me.LocalizedTextRequiredField
                args.IsValid = False
            ElseIf Me.TextboxEMail.Text.Length < 7 Then
                Me.ValidatorEMail.Text = CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.CreateAccount_ErrorJS_Length.Replace("\""", """"), 7, cammWebManager.Internationalization.CreateAccount_Descr_Email, "", "", "", "", "", "", "", "", "", "")
                args.IsValid = False
            ElseIf Not Utils.ValidateEmailAddress(Me.TextboxEMail.Text) Then
                Me.ValidatorEMail.Text = CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.CreateAccount_ErrorJS_InputValue.Replace("\""", """"), cammWebManager.Internationalization.CreateAccount_Descr_Email, "", "", "", "", "", "", "", "", "", "", "")
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        End Sub

#End Region

    End Class
#End Region

#Region " Public Class ChangeUserPassword "
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
#End Region

#Region " Public Class Update/ChangeUserProfile "
    <Obsolete("Use UpdateUserProfile instead", False), System.Runtime.InteropServices.ComVisible(False)> Public Class ChangeUserProfile
        Inherits CompuMaster.camm.WebManager.Pages.ProtectedPage

        Protected ErrMsg As String = ""
        Protected MissingFields As String = Nothing
        Protected MyUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation
        Protected ShowMissingFieldsListInErrorMessage As Boolean = False
        Protected CheckboxListMotivation As System.Web.UI.WebControls.CheckBoxList
        Protected MotivationOtherText As System.Web.UI.WebControls.TextBox

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Clear the list of missing fields
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.06.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub ResetMissingFieldItems()
            MissingFields = Nothing
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Add an additional element to the list of missing fields
        ''' </summary>
        ''' <param name="name"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.06.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub AddMissingFieldItem(ByVal name As String)
            If MissingFields = Nothing Then
                MissingFields = name
            Else
                MissingFields &= ", " & name
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Validate the form data to be complete
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.06.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function RequiredFormDataAvailable() As Boolean
            'Validate field by field
            If Request.Form("Company") = "" Then
                AddMissingFieldItem("Company")
            End If
            If Request.Form("Anrede") = "" Then
                AddMissingFieldItem("Salutation")
            End If
            If Request.Form("Vorname") = "" Then
                AddMissingFieldItem("First Name")
            End If
            If Request.Form("Nachname") = "" Then
                AddMissingFieldItem("Last Name")
            End If
            If Request.Form("e-mail") = "" Then
                AddMissingFieldItem("e-mail")
            End If
            If Request.Form("Strasse") = "" Then
                AddMissingFieldItem("Street")
            End If
            If Request.Form("PLZ") = "" Then
                AddMissingFieldItem("Zip Code")
            End If
            If Request.Form("Ort") = "" Then
                AddMissingFieldItem("Location")
            End If
            If Request.Form("Land") = "" Then
                AddMissingFieldItem("Country")
            End If
            If Request.Form("1stPreferredLanguage") = "" Then
                AddMissingFieldItem("1st preferred language")
            End If
            'Return success result
            If MissingFields <> Nothing Then
                Return False
            Else
                Return True
            End If
        End Function

        Private Sub Localization()
            cammWebManager.PageTitle = cammWebManager.Internationalization.OfficialServerGroup_Title & " - " & cammWebManager.Internationalization.CreateAccount_Descr_PageTitle

            If Not Page.IsPostBack Then
                If Not Me.CheckboxListMotivation Is Nothing Then
                    Dim UserInfo As New CompuMaster.camm.WebManager.WMSystem.UserInformation(cammWebManager.InternalCurrentUserID, cammWebManager)
                    Dim selectedMotivation As String = ""
                    If Not UserInfo.AdditionalFlags("Motivation") = Nothing Then
                        selectedMotivation = UserInfo.AdditionalFlags("Motivation")
                    End If
                    'CheckboxList Motivation
                    Dim chkVisitor As New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_MotivItemWebSiteVisitor, "Visitor")
                    If selectedMotivation.IndexOf("Visitor") > -1 Then
                        chkVisitor.Selected = True
                    End If
                    Me.CheckboxListMotivation.Items.Add(chkVisitor)

                    Dim chkDealer As New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_MotivItemDealer, "Dealer")
                    If selectedMotivation.IndexOf("Dealer") > -1 Then
                        chkDealer.Selected = True
                    End If
                    Me.CheckboxListMotivation.Items.Add(chkDealer)

                    Dim chkJournalist As New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_MotivItemJournalist, "Journalist")
                    If selectedMotivation.IndexOf("Journalist") > -1 Then
                        chkJournalist.Selected = True
                    End If
                    Me.CheckboxListMotivation.Items.Add(chkJournalist)

                    Dim chkSupplier As New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_MotivItemSupplier, "Supplier")
                    If selectedMotivation.IndexOf("Supplier") > -1 Then
                        chkSupplier.Selected = True
                    End If
                    Me.CheckboxListMotivation.Items.Add(chkSupplier)

                    Dim chkOther As New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.CreateAccount_Descr_MotivItemOther, "Other")
                    If selectedMotivation.Replace("Visitor", "").Replace("Dealer", "").Replace("Journalist", "").Replace("Supplier", "").Replace(",", "") <> "" Then
                        chkOther.Selected = True
                        Me.MotivationOtherText.Text = Server.HtmlEncode(selectedMotivation.Replace("Visitor", "").Replace("Dealer", "").Replace("Journalist", "").Replace("Supplier", "").Replace(",", "").Trim)
                    End If
                    Me.CheckboxListMotivation.Items.Add(chkOther)
                End If

                Me.DataBind()

            End If

        End Sub

        Private Function CollectMotivationDetails() As String
            Dim Result As String = Nothing
            If Not Me.CheckboxListMotivation Is Nothing Then
                For Each item As Web.UI.WebControls.ListItem In Me.CheckboxListMotivation.Items
                    If item.Selected Then
                        If Result = Nothing Then
                            Result = item.Value
                        Else
                            Result &= ", " & item.Value
                        End If
                    End If
                Next
                If Me.MotivationOtherText.Text <> Nothing Then
                    Result &= " (" & Me.MotivationOtherText.Text & ")"
                End If
            End If
            Return Result
        End Function

        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Server.ScriptTimeout = 10

            'This page is not available for anonymous users
            If Not cammWebManager.IsLoggedOn Then
                cammWebManager.RedirectToLogonPage("Logon required for update profile page", Nothing)
            End If

            Localization()

            'Perform actions only when this is a post back
            If Request.Form("loginname") <> Nothing Then

                'Ensure must fields
                Me.ResetMissingFieldItems()
                If Request.Form.Count <> 0 AndAlso RequiredFormDataAvailable() = False Then
                    ErrMsg = cammWebManager.Internationalization.UpdateProfile_ErrMsg_InsertAllRequiredFields
                    If ShowMissingFieldsListInErrorMessage AndAlso MissingFields <> Nothing Then
                        ErrMsg &= " (" & MissingFields & ")"
                    End If
                End If
                If Trim(Request.Form("loginpw")) = "" Then
                    ErrMsg = cammWebManager.Internationalization.UpdateProfile_ErrMsg_PWRequired
                End If

                'Required fields are available, start the update process
                If Request.Form("loginpw") <> "" And ErrMsg = "" Then
                    'Load the user data from database
                    Dim UserID As Long = CType(cammWebManager.System_GetUserID(cammWebManager.CurrentUserLoginName), Long)
                    Dim UserInfo As New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, cammWebManager)
                    'Validate if the update is allowed to be made
                    If UserInfo.IsSystemUser Then
                        Throw New Exception("Update of profiles only for real users")
                    End If
                    If UserInfo.ValidatePassword(Request.Form("loginpw")) = False Then
                        ErrMsg = cammWebManager.Internationalization.UpdateProfile_ErrMsg_MistypedPW
                    Else
                        ErrMsg = cammWebManager.Internationalization.UpdateProfile_ErrMsg_Success
                        'Update the user info data
                        UpdateProfileData(UserInfo)
                        Try
                            UserInfo.Save(True)
                        Catch ex As Exception
                            ErrMsg = ex.Message
                        End Try
                    End If
                End If

            End If

            'Load refreshed user info object from database
            Try
                MyUserInfo = cammWebManager.System_GetCurUserInfo()
            Catch ex As Exception
                cammWebManager.Log.RuntimeException(ex, False, True)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fill the user profile with the new data which shall be saved
        ''' </summary>
        ''' <param name="userInfo">The current user profile which shall be updated</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	13.06.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub UpdateProfileData(ByVal userInfo As WMSystem.UserInformation)
            userInfo.Gender = CType(IIf(CStr(Request.Form("Anrede")) = "Ms.", WMSystem.Sex.Feminin, IIf(CStr(Request.Form("Anrede")) = "Mr.", WMSystem.Sex.Masculin, WMSystem.Sex.Undefined)), WMSystem.Sex)
            userInfo.PreferredLanguage1 = New WMSystem.LanguageInformation(CType(Request.Form("1stPreferredLanguage"), Integer), cammWebManager)
            userInfo.PreferredLanguage2 = New WMSystem.LanguageInformation(CType(Utils.StringNotEmptyOrNothing(Request.Form("2ndPreferredLanguage")), Integer), cammWebManager)
            userInfo.PreferredLanguage3 = New WMSystem.LanguageInformation(CType(Utils.StringNotEmptyOrNothing(Request.Form("3rdPreferredLanguage")), Integer), cammWebManager)
            userInfo.Company = Request.Form("Company")
            userInfo.AcademicTitle = Request.Form("Titel")
            userInfo.FirstName = Request.Form("Vorname")
            userInfo.LastName = Request.Form("Nachname")
            userInfo.NameAddition = Request.Form("Namenszusatz")
            userInfo.EMailAddress = Request.Form("e-mail")
            userInfo.Street = Request.Form("Strasse")
            userInfo.ZipCode = Request.Form("PLZ")
            userInfo.Location = Request.Form("Ort")
            userInfo.State = Request.Form("State")
            userInfo.Country = Request.Form("Land")
            userInfo.FaxNumber = Request.Form("Fax")
            userInfo.MobileNumber = Request.Form("Mobile")
            userInfo.PhoneNumber = Request.Form("Phone")
            userInfo.Position = Request.Form("PositionInCompany")
            userInfo.AdditionalFlags("Motivation") = CollectMotivationDetails()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Create a string with OPTION tags for all activated languages for embedding into the SELECT tag
        ''' </summary>
        ''' <param name="preferredLanguageLevelID">1 for 1st language, 2 for the 2nd one, 3 for the 3rd one</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	12.12.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function MarketsListOptions(ByVal preferredLanguageLevelID As Integer) As String

            Dim Result As New Text.StringBuilder
            Dim MarketList As WMSystem.LanguageInformation() = cammWebManager.System_GetLanguagesInfo(False)

            Dim sortedList As New Collections.SortedList

            For Each market As WMSystem.LanguageInformation In MarketList
                If market.ID <> 10000 Then
                    sortedList.Add(market.LanguageName_OwnLanguage, market.ID.ToString)
                End If
            Next

            Dim autoSelectID As Integer
            Select Case preferredLanguageLevelID
                Case 1
                    autoSelectID = MyUserInfo.PreferredLanguage1.ID
                Case 2
                    autoSelectID = MyUserInfo.PreferredLanguage2.ID
                Case 3
                    autoSelectID = MyUserInfo.PreferredLanguage3.ID
                Case Else
                    Throw New ArgumentException("Invalid value, it must be 1, 2 or 3")
            End Select

            For Each market As DictionaryEntry In sortedList
                If autoSelectID.ToString = CStr(market.Value) Then
                    Result.Append("<option selected value=""" & CStr(market.Value) & """>" & Server.HtmlEncode(CStr(market.Key)) & "</option>")
                Else
                    Result.Append("<option value=""" & CStr(market.Value) & """>" & Server.HtmlEncode(CStr(market.Key)) & "</option>")
                End If
            Next

            Return Result.ToString

        End Function

    End Class

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
        ''' <returns></returns>
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
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected MustOverride Function UpdateBasicUserInfoOfCurrentUser() As WebManager.IUserInformation

        ''' <summary>
        ''' Overridable method for customized actions after the new user account has been written
        ''' </summary>
        ''' <param name="userInfo">The updated user account</param>
        ''' <remarks>
        ''' </remarks>
        Protected Overridable Sub AfterUserUpdate(ByVal userInfo As WebManager.IUserInformation)
        End Sub

        ''' <summary>
        ''' Finally write the user account
        ''' </summary>
        ''' <param name="userInfo"></param>
        ''' <returns></returns>
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
        ''' <remarks>
        ''' </remarks>
        Protected MustOverride Sub FillUserAccount(ByVal userInfo As WebManager.IUserInformation)

    End Class

    <System.Runtime.InteropServices.ComVisible(False)> Public Class UpdateUserProfile
        Inherits BaseUpdateUserProfile

        Protected TextboxPassword1 As Web.UI.WebControls.TextBox
        Protected DropdownSalutation As Web.UI.WebControls.DropDownList
        Protected TextboxCompany As Web.UI.WebControls.TextBox
        Protected TextboxAcademicTitle As Web.UI.WebControls.TextBox
        Protected TextboxFirstName As Web.UI.WebControls.TextBox
        Protected TextboxNameAffix As Web.UI.WebControls.TextBox
        Protected TextboxLastName As Web.UI.WebControls.TextBox
        Protected TextboxEMail As Web.UI.WebControls.TextBox
        Protected TextboxStreet As Web.UI.WebControls.TextBox
        Protected TextboxZipCode As Web.UI.WebControls.TextBox
        Protected TextboxLocation As Web.UI.WebControls.TextBox
        Protected TextboxState As Web.UI.WebControls.TextBox
        Protected TextboxCountry As Web.UI.WebControls.TextBox
        Protected TextboxPhone As Web.UI.WebControls.TextBox
        Protected TextboxFax As Web.UI.WebControls.TextBox
        Protected TextboxMobile As Web.UI.WebControls.TextBox
        Protected TextboxPositionInCompany As Web.UI.WebControls.TextBox
        Protected Dropdown1stPreferredLanguage As Web.UI.WebControls.DropDownList
        Protected Dropdown2ndPreferredLanguage As Web.UI.WebControls.DropDownList
        Protected Dropdown3rdPreferredLanguage As Web.UI.WebControls.DropDownList
        Protected RadioListComesFrom As Web.UI.WebControls.RadioButtonList
        Protected CheckboxListMotivation As Web.UI.WebControls.CheckBoxList
        Protected TextboxComment As Web.UI.WebControls.TextBox
        Protected ComesFromOtherText As Web.UI.WebControls.TextBox
        Protected MotivationOtherText As Web.UI.WebControls.TextBox
        Protected ValidatorSummary As Web.UI.WebControls.ValidationSummary
        Protected WithEvents ValidatorFirstName As Web.UI.WebControls.RequiredFieldValidator
        Protected WithEvents ValidatorLastName As Web.UI.WebControls.RequiredFieldValidator
        Protected WithEvents ValidatorSalutation As Web.UI.WebControls.RequiredFieldValidator
        Protected WithEvents ValidatorPassword1 As Web.UI.WebControls.CustomValidator
        Protected WithEvents ValidatorEMail As Web.UI.WebControls.CustomValidator
        Protected WithEvents SubmitButton As Web.UI.WebControls.Button

        Protected Overrides Property ConfirmationUserPassword() As String
            Get
                Return Me.TextboxPassword1.Text
            End Get
            Set(ByVal value As String)
                Me.TextboxPassword1.Text = value
            End Set
        End Property

        ''' <summary>
        '''     Fill the user profile with the new data which shall be saved
        ''' </summary>
        ''' <param name="userInfo">The current user profile which shall be updated</param>
        ''' <remarks>
        ''' </remarks>
        Protected Overrides Sub FillUserAccount(ByVal userInfo As WebManager.IUserInformation)

            'Already prefilled values
            'userInfo.Gender = CType(IIf(CStr(Me.DropdownSalutation.SelectedValue) = "Ms.", WMSystem.Sex.Feminin, WMSystem.Sex.Masculin), WMSystem.Sex)
            'userInfo.PreferredLanguage1 = New WMSystem.LanguageInformation(CType(Me.Dropdown1stPreferredLanguage.SelectedValue, Integer), cammWebManager)
            'userInfo.PreferredLanguage2 = New WMSystem.LanguageInformation(CType(Utils.StringNotEmptyOrNothing(Me.Dropdown2ndPreferredLanguage.SelectedValue), Integer), cammWebManager)
            'userInfo.PreferredLanguage3 = New WMSystem.LanguageInformation(CType(Utils.StringNotEmptyOrNothing(Me.Dropdown3rdPreferredLanguage.SelectedValue), Integer), cammWebManager)
            'userInfo.Company = Me.TextboxCompany.Text
            'userInfo.AcademicTitle = Me.TextboxAcademicTitle.Text
            'userInfo.FirstName = Me.TextboxFirstName.Text
            'userInfo.LastName = Me.TextboxLastName.Text
            'userInfo.NameAddition = Me.TextboxNameAffix.Text
            'userInfo.EMailAddress = Me.TextboxEMail.Text
            'userInfo.Street = Me.TextboxStreet.Text
            'userInfo.ZipCode = Me.TextboxZipCode.Text
            'userInfo.Location = Me.TextboxLocation.Text
            'userInfo.State = Me.TextboxState.Text
            'userInfo.Country = Me.TextboxCountry.Text

            'Additional fields
            userInfo.PhoneNumber = Me.TextboxPhone.Text
            userInfo.FaxNumber = Me.TextboxFax.Text
            userInfo.MobileNumber = Me.TextboxMobile.Text
            userInfo.Position = Me.TextboxPositionInCompany.Text
            userInfo.AdditionalFlags("OnCreationComment") = Me.TextboxComment.Text
            userInfo.AdditionalFlags("Motivation") = CollectMotivationDetails()
            userInfo.AdditionalFlags("ComesFrom") = CollectComesFromDetails()

        End Sub

        Private Function CollectMotivationDetails() As String
            Dim Result As String = Nothing
            If Not Me.CheckboxListMotivation Is Nothing Then
                For Each item As Web.UI.WebControls.ListItem In Me.CheckboxListMotivation.Items
                    If item.Selected Then
                        If Result = Nothing Then
                            Result = item.Value
                        Else
                            Result &= ", " & item.Value
                        End If
                    End If
                Next
                If Me.MotivationOtherText.Text <> Nothing Then
                    Result &= " (" & Me.MotivationOtherText.Text & ")"
                End If
            End If
            Return Result
        End Function

        Private Function CollectComesFromDetails() As String
            Dim Result As String = Nothing
            If Not Me.RadioListComesFrom Is Nothing Then
                For Each item As Web.UI.WebControls.ListItem In Me.RadioListComesFrom.Items
                    If item.Selected Then
                        If Result = Nothing Then
                            Result = item.Value
                        Else
                            Result &= ", " & item.Value
                        End If
                    End If
                Next
                If Me.ComesFromOtherText.Text <> Nothing Then
                    Result &= " (" & Me.ComesFromOtherText.Text & ")"
                End If
            End If
            Return Result
        End Function

        Protected Overrides Function UpdateBasicUserInfoOfCurrentUser() As WebManager.IUserInformation

            Dim MyUserInfo As WMSystem.UserInformation = Me.cammWebManager.CurrentUserInfo
            MyUserInfo.EMailAddress = Trim(Me.TextboxEMail.Text)
            MyUserInfo.Company = Trim(Me.TextboxCompany.Text)
            MyUserInfo.Gender = CType(IIf(Me.DropdownSalutation.SelectedValue = "Ms.", WMSystem.Sex.Feminin, IIf(Me.DropdownSalutation.SelectedValue = "Mr.", WMSystem.Sex.Masculin, WMSystem.Sex.Undefined)), WMSystem.Sex)
            MyUserInfo.NameAddition = Trim(Me.TextboxNameAffix.Text)
            MyUserInfo.FirstName = Trim(Me.TextboxFirstName.Text)
            MyUserInfo.LastName = Trim(Me.TextboxLastName.Text)
            MyUserInfo.AcademicTitle = Trim(Me.TextboxAcademicTitle.Text)
            MyUserInfo.Street = Trim(Me.TextboxStreet.Text)
            MyUserInfo.ZipCode = Trim(Me.TextboxZipCode.Text)
            MyUserInfo.Location = Trim(Me.TextboxLocation.Text)
            MyUserInfo.State = Trim(Me.TextboxState.Text)
            MyUserInfo.Country = Trim(Me.TextboxCountry.Text)
            MyUserInfo.PreferredLanguage1 = New WMSystem.LanguageInformation(CInt(Me.Dropdown1stPreferredLanguage.SelectedValue), Me.cammWebManager)
            MyUserInfo.PreferredLanguage2 = New WMSystem.LanguageInformation(Utils.TryCInt(Me.Dropdown2ndPreferredLanguage.SelectedValue), Me.cammWebManager)
            MyUserInfo.PreferredLanguage3 = New WMSystem.LanguageInformation(Utils.TryCInt(Me.Dropdown3rdPreferredLanguage.SelectedValue), Me.cammWebManager)
            Return MyUserInfo

        End Function

        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'Perform actions only when this is a post back
            If Page.IsPostBack Then
                Page.Validate()
                If Page.IsValid Then
                    CollectDataAndUpdateAccount()
                End If
            End If

        End Sub

        Protected Overrides Sub ShowErrorMessage(ByVal message As String)
            Me.Validators.Add(New DummyValidatorForAppearanceInValidationSummary(message))
            Page.Validate()
        End Sub

        ''' <summary>
        ''' Create a string with OPTION tags for all activated languages for embedding into the SELECT tag
        ''' </summary>
        ''' <param name="languageDropdownList">A dropdownlist control which shall be filled</param>
        ''' <remarks>
        ''' </remarks>
        Protected Sub FillMarketsList(ByVal languageDropdownList As System.Web.UI.WebControls.DropDownList)

            Dim MarketList As WMSystem.LanguageInformation() = cammWebManager.System_GetLanguagesInfo(False)

            Dim sortedList As New Collections.SortedList

            For Each market As WMSystem.LanguageInformation In MarketList
                If market.ID <> 10000 Then
                    sortedList.Add(market.LanguageName_OwnLanguage, market.ID)
                End If
            Next

            For Each market As DictionaryEntry In sortedList
                languageDropdownList.Items.Add(New Web.UI.WebControls.ListItem(CStr(market.Key), CStr(market.Value)))
            Next

        End Sub

        Private Sub Localization()
            cammWebManager.PageTitle = cammWebManager.Internationalization.OfficialServerGroup_Title & " - " & cammWebManager.Internationalization.UpdateProfile_Descr_Title

            If Not Page.IsPostBack Then
                Me.SubmitButton.Text = cammWebManager.Internationalization.UpdateProfile_Descr_Submit

                'Add salutation dropdown content
                Me.DropdownSalutation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_PleaseSelect, ""))
                Me.DropdownSalutation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Abbrev_Mister, "Mr."))
                Me.DropdownSalutation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Abbrev_Miss, "Ms."))

                'Fill markets/languages dropdowns
                Me.Dropdown1stPreferredLanguage.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_PleaseSelect, ""))
                FillMarketsList(Me.Dropdown1stPreferredLanguage)
                Me.Dropdown2ndPreferredLanguage.Items.Add(New Web.UI.WebControls.ListItem("", ""))
                FillMarketsList(Me.Dropdown2ndPreferredLanguage)
                Me.Dropdown3rdPreferredLanguage.Items.Add(New Web.UI.WebControls.ListItem("", ""))
                FillMarketsList(Me.Dropdown3rdPreferredLanguage)

                'CheckboxList Motivation
                If Not Me.CheckboxListMotivation Is Nothing Then
                    Me.CheckboxListMotivation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_MotivItemWebSiteVisitor, "Visitor"))
                    Me.CheckboxListMotivation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_MotivItemDealer, "Dealer"))
                    Me.CheckboxListMotivation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_MotivItemJournalist, "Journalist"))
                    Me.CheckboxListMotivation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_MotivItemSupplier, "Supplier"))
                    Me.CheckboxListMotivation.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_MotivItemOther, "Other"))
                End If

                'RadioList ComesFrom
                If Not Me.RadioListComesFrom Is Nothing Then
                    Me.RadioListComesFrom.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_WhereItemFriend, "Friend"))
                    Me.RadioListComesFrom.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_WhereItemResellerDealer, "Dealer"))
                    Me.RadioListComesFrom.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_WhereItemExhibition, "Exhibition"))
                    Me.RadioListComesFrom.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_WhereItemMagazines, "Magazine"))
                    Me.RadioListComesFrom.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_WhereItemSearchEnginge, "SearchEngine"))
                    Me.RadioListComesFrom.Items.Add(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_WhereItemOther, "Other"))
                End If

                'Validator texts
                Me.ValidatorFirstName.Text = LocalizedTextRequiredField
                Me.ValidatorFirstName.EnableClientScript = True
                Me.ValidatorLastName.Text = LocalizedTextRequiredField
                Me.ValidatorLastName.EnableClientScript = True
                Me.ValidatorLastName.Text = LocalizedTextRequiredField
                Me.ValidatorLastName.EnableClientScript = True
                Me.ValidatorSalutation.Text = LocalizedTextRequiredField
                Me.ValidatorSalutation.EnableClientScript = True
                'me.ValidatorPassword1.Text=CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.UpdateProfile_ErrorJS_Length.Replace("\""",""""), cammWebManager.PasswordSecurity.InspectionSeverities(cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "accesslevel_default").ID).RequiredPasswordLength, cammWebManager.Internationalization.UpdateProfile_Descr_NewLoginPassword, "", "", "", "", "", "", "", "", "", "")
            End If

            'Remove Please-Select-Dropdown-Element as soon as possible
            If Me.DropdownSalutation.SelectedValue <> "" Then
                If Me.DropdownSalutation.Items.Contains(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_PleaseSelect, "")) Then
                    Me.DropdownSalutation.Items.Remove(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_PleaseSelect, ""))
                End If
            End If
            If Me.Dropdown1stPreferredLanguage.SelectedValue <> "" Then
                If Me.Dropdown1stPreferredLanguage.Items.Contains(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_PleaseSelect, "")) Then
                    Me.Dropdown1stPreferredLanguage.Items.Remove(New Web.UI.WebControls.ListItem(cammWebManager.Internationalization.UpdateProfile_Descr_PleaseSelect, ""))
                End If
            End If

            Me.DataBind()

        End Sub

        Private Sub PageOnInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Server.ScriptTimeout = 10

            'Ensure maximum login field length
            If Me.TextboxPassword1.MaxLength <= 0 OrElse Me.TextboxPassword1.MaxLength > 30 Then
                Me.TextboxPassword1.MaxLength = 30
            End If

            Localization()
        End Sub

#Region "Custom validators"

        Private Sub CustomValidatorPassword1_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles ValidatorPassword1.ServerValidate
            If Me.TextboxPassword1.Text = Nothing Then
                Me.ValidatorPassword1.Text = Me.LocalizedTextRequiredField
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        End Sub

        Private Sub CustomValidatorEMail_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles ValidatorEMail.ServerValidate
            If Me.TextboxEMail.Text = Nothing Then
                Me.ValidatorEMail.Text = Me.LocalizedTextRequiredField
                args.IsValid = False
            ElseIf Me.TextboxEMail.Text.Length < 7 Then
                Me.ValidatorEMail.Text = CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.UpdateProfile_ErrorJS_Length.Replace("\""", """"), 7, cammWebManager.Internationalization.UpdateProfile_Descr_EMail, "", "", "", "", "", "", "", "", "", "")
                args.IsValid = False
            ElseIf Not Utils.ValidateEmailAddress(Me.TextboxEMail.Text) Then
                Me.ValidatorEMail.Text = CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.UpdateProfile_ErrorJS_InputValue.Replace("\""", """"), cammWebManager.Internationalization.UpdateProfile_Descr_EMail, "", "", "", "", "", "", "", "", "", "", "")
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        End Sub

#End Region

    End Class
#End Region

#Region " Public Class SendUserPassword "
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
                    Return
                End If
            End If

            Dim resetLinkGenerator As New PassswordReset(cammWebManager, UserInfo)
            cammWebManager.Notifications.NotificationForUser_PasswordResetLink(UserInfo, resetLinkGenerator.CreateResetUrl())
        End Sub

    End Class


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


#End Region
End Namespace