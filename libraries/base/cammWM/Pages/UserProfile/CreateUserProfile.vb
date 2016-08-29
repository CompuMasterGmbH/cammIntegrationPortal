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
        ''' <summary>
        ''' Prohibited password parts for password complexity check
        ''' </summary>
        Protected Overrides Function PasswordSeverityCheckStrings() As String()
            Dim MyString(2) As String
            MyString(0) = Mid(Me.TextboxLoginName.Text, 1, 4)
            MyString(1) = Mid(Me.TextboxFirstName.Text, 1, 4)
            MyString(2) = Mid(Me.TextboxLastName.Text, 1, 4)
            Return MyString
        End Function
        ''' <summary>
        '''     Fill the user profile with the new data which shall be saved
        ''' </summary>
        ''' <param name="userInfo">The current user profile which shall be created</param>
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

            Dim MyUserInfo As New WMSystem.UserInformation(0&,
                Trim(Mid(Trim(Me.TextboxLoginName.Text), 1, 20)),
                Trim(Me.TextboxEMail.Text),
                False,
                Trim(Me.TextboxCompany.Text),
                CType(IIf(Me.DropdownSalutation.SelectedValue = "Ms.", WMSystem.Sex.Feminine, IIf(Me.DropdownSalutation.SelectedValue = "Mr.", WMSystem.Sex.Masculine, WMSystem.Sex.Undefined)), WMSystem.Sex),
                Trim(Me.TextboxNameAffix.Text),
                Trim(Me.TextboxFirstName.Text),
                Trim(Me.TextboxLastName.Text),
                Trim(Me.TextboxAcademicTitle.Text),
                Trim(Me.TextboxStreet.Text),
                Trim(Me.TextboxZipCode.Text),
                Trim(Me.TextboxLocation.Text),
                Trim(Me.TextboxState.Text),
                Trim(Me.TextboxCountry.Text),
                CInt(Me.Dropdown1stPreferredLanguage.SelectedValue),
                Utils.TryCInt(Me.Dropdown2ndPreferredLanguage.SelectedValue),
                Utils.TryCInt(Me.Dropdown3rdPreferredLanguage.SelectedValue),
                False,
                False,
                False,
                Me.AccessLevelDefault,
                CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem),
                CType(Nothing, String),
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
        ''' <summary>
        ''' Create a string with OPTION tags for all activated languages for embedding into the SELECT tag
        ''' </summary>
        ''' <param name="languageDropdownList">A dropdownlist control which shall be filled</param>
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

End Namespace