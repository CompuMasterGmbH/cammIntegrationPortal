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

    <System.Runtime.InteropServices.ComVisible(False)> Public Class UpdateUserProfile
        Inherits BaseUpdateUserProfile

        Protected LabelLoginName As Web.UI.WebControls.Label
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
        Protected WithEvents DropdownCountry As Web.UI.WebControls.DropDownList
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

        ''' <summary>
        ''' Fill the motivation control elements to fit with the current user profile data
        ''' </summary>
        ''' <param name="flagValue"></param>
        Private Sub FillMotivationControlValues(flagValue As String)
            If Not Me.CheckboxListMotivation Is Nothing Then
                Dim SeparatedValues As List(Of String) = SplitFlagCommaSeparatedButWithParenthesisPriority(flagValue)
                For Each item As Web.UI.WebControls.ListItem In Me.CheckboxListMotivation.Items
                    If SeparatedValues.Contains(item.Value) Then
                        item.Selected = True
                        SeparatedValues.Remove(item.Value)
                    Else
                        item.Selected = False
                    End If
                Next
                If SeparatedValues.Count > 0 Then
                    Dim RemainingValues As String = Strings.Join(SeparatedValues.ToArray, ","c)
                    If RemainingValues.StartsWith("Other (") AndAlso RemainingValues.EndsWith(")") Then
                        Dim CleanedOtherValue As String = RemainingValues.Remove(0, "Other (".Length)
                        CleanedOtherValue = CleanedOtherValue.Substring(0, CleanedOtherValue.Length - 1)
                        Me.MotivationOtherText.Text = CleanedOtherValue
                    Else
                        Me.MotivationOtherText.Text = RemainingValues
                    End If
                    If LookupItemListElementOther(Me.CheckboxListMotivation.Items) IsNot Nothing Then
                        LookupItemListElementOther(Me.CheckboxListMotivation.Items).Selected = True
                    End If
                Else
                    Me.MotivationOtherText.Text = ""
                End If
            End If
        End Sub

        ''' <summary>
        ''' Fill the comes-from control elements to fit with the current user profile data
        ''' </summary>
        ''' <param name="flagValue"></param>
        Private Sub FillComesFromControlValues(flagValue As String)
            If Not Me.RadioListComesFrom Is Nothing Then
                Dim SeparatedValues As List(Of String) = SplitFlagCommaSeparatedButWithParenthesisPriority(flagValue)
                For Each item As Web.UI.WebControls.ListItem In Me.RadioListComesFrom.Items
                    If SeparatedValues.Contains(item.Value) Then
                        item.Selected = True
                        SeparatedValues.Remove(item.Value)
                    Else
                        item.Selected = False
                    End If
                Next
                If SeparatedValues.Count > 0 Then
                    Dim RemainingValues As String = Strings.Join(SeparatedValues.ToArray, ","c)
                    If RemainingValues.StartsWith("Other (") AndAlso RemainingValues.EndsWith(")") Then
                        Dim CleanedOtherValue As String = RemainingValues.Remove(0, "Other (".Length)
                        CleanedOtherValue = CleanedOtherValue.Substring(0, CleanedOtherValue.Length - 1)
                        Me.ComesFromOtherText.Text = CleanedOtherValue
                    Else
                        Me.ComesFromOtherText.Text = RemainingValues
                    End If
                    If LookupItemListElementOther(Me.RadioListComesFrom.Items) IsNot Nothing Then
                        LookupItemListElementOther(Me.RadioListComesFrom.Items).Selected = True
                    End If
                Else
                    Me.ComesFromOtherText.Text = ""
                End If
            End If
        End Sub

        ''' <summary>
        ''' Lookup the element of the list with value &quot;Other&quot;
        ''' </summary>
        ''' <param name="list"></param>
        ''' <returns></returns>
        Private Function LookupItemListElementOther(list As System.Web.UI.WebControls.ListItemCollection) As System.Web.UI.WebControls.ListItem
            Return MyBase.LookupListItemWithValue(list, "Other")
        End Function

        ''' <summary>
        ''' Split a string by comma, but not if the comma is within parenthesis
        ''' </summary>
        ''' <param name="flagValue">Comma-separated list of values, e.g. &quot;Abc,Def,Other (Some,other,commas)&quot;</param>
        ''' <returns>Separated text elements, e.g. &quot;Abc&quot;, &quot;Def&quot;, &quot;Other (Some,other,commas)&quot; </returns>
        Private Function SplitFlagCommaSeparatedButWithParenthesisPriority(flagValue As String) As List(Of String)
            Dim SplittedFlag As String() = flagValue.Split(","c)
            Dim Result As New List(Of String)
            Dim CurrentLogicLevel As Integer = 0
            Dim CurrentBuffer As String = ""
            For Each FlagPart As String In SplittedFlag
                Dim LogicLevelChange As Integer = Utils.CountOfOccurances(FlagPart, "(") - Utils.CountOfOccurances(FlagPart, ")")
                If CurrentLogicLevel = 0 And LogicLevelChange = 0 Then
                    Result.Add(FlagPart)
                Else
                    If CurrentBuffer = "" Then
                        CurrentBuffer = FlagPart
                    Else
                        CurrentBuffer &= "," & FlagPart
                    End If
                    CurrentLogicLevel = System.Math.Max(CurrentLogicLevel + LogicLevelChange, 0) 'never go into negative numbers
                    If CurrentLogicLevel = 0 Then
                        Result.Add(CurrentBuffer)
                        CurrentBuffer = ""
                    End If
                End If
            Next
            If CurrentBuffer <> "" Then
                Result.Add(CurrentBuffer)
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

        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Me.DropdownCountry IsNot Nothing AndAlso Me.TextboxCountry Is Nothing Then
                'dropdown box available, free text field not available
                Me.DropdownCountry.Visible = True
                If Me.IsPostBack = False Then
                    Me.DropdownCountry.Items.AddRange(ConvertStringsToListItems(Me.LimitedAllowedCountries).ToArray)
                End If
                Me.TextboxCountry.Visible = False
            ElseIf Me.DropdownCountry Is Nothing AndAlso Me.TextboxCountry IsNot Nothing Then
                'no dropdown control available --> use free text field
            ElseIf Me.DropdownCountry Is Nothing AndAlso Me.TextboxCountry Is Nothing Then
                'no country field there?!? -> well, let's try to ignore this at this point
            Else 'both controls available - show just the best fitting option
                If LimitedAllowedCountries.Count = 0 Then
                    'no limits --> free text field
                    Me.DropdownCountry.Visible = False
                    Me.TextboxCountry.Visible = True
                Else
                    'limited to defined allow-values -> dropdown box
                    Me.DropdownCountry.Visible = True
                    If Me.IsPostBack = False Then
                        Me.DropdownCountry.Items.AddRange(ConvertStringsToListItems(Me.LimitedAllowedCountries).ToArray)
                    End If
                    Me.TextboxCountry.Visible = False
                End If
            End If

            If Page.IsPostBack Then
                'Validate on post-back
                Page.Validate()
            Else
                'prevent Google Chrome (and others) to auto-fill (the very last textbox before the password field would be considered as the username field - which is definitely wrong, here)
                Me.Form.Attributes.Add("autocomplete", "false")
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

        Private Sub SubmitButton_Click(sender As Object, e As EventArgs) Handles SubmitButton.Click
            Me.Validate()
            If Page.IsValid Then
                CollectDataAndUpdateAccount()
            End If
        End Sub

#End Region
        Private Sub SelectPreferredLanguage(dropdownList As System.Web.UI.WebControls.DropDownList, value As Integer)
            If dropdownList IsNot Nothing AndAlso value <> 0 AndAlso LookupListItemWithValue(dropdownList.Items, value.ToString) IsNot Nothing Then
                LookupListItemWithValue(dropdownList.Items, value.ToString).Selected = True
                Dim EmptyValueItem As Web.UI.WebControls.ListItem = LookupListItemWithValue(dropdownList.Items, "")
                If EmptyValueItem IsNot Nothing Then dropdownList.Items.Remove(EmptyValueItem)
            End If
        End Sub

        Protected Overrides Sub AssignUserInfoDataToForm()
            Me.LabelLoginName.Text = Server.HtmlEncode(Me.cammWebManager.CurrentUserInfo.LoginName)
            Me.TextboxCompany.Text = Me.cammWebManager.CurrentUserInfo.Company
            Select Case Me.cammWebManager.CurrentUserInfo.Gender
                Case WMSystem.Sex.Masculine
                    Me.DropdownSalutation.SelectedValue = "Mr."
                Case WMSystem.Sex.Feminine
                    Me.DropdownSalutation.SelectedValue = "Ms."
                Case Else
                    Me.DropdownSalutation.SelectedValue = ""
            End Select
            Me.TextboxAcademicTitle.Text = Me.cammWebManager.CurrentUserInfo.AcademicTitle
            Me.TextboxFirstName.Text = Me.cammWebManager.CurrentUserInfo.FirstName
            Me.TextboxLastName.Text = Me.cammWebManager.CurrentUserInfo.LastName
            Me.TextboxNameAffix.Text = Me.cammWebManager.CurrentUserInfo.NameAddition
            Me.TextboxEMail.Text = Me.cammWebManager.CurrentUserInfo.EMailAddress
            Me.TextboxStreet.Text = Me.cammWebManager.CurrentUserInfo.Street
            Me.TextboxZipCode.Text = Me.cammWebManager.CurrentUserInfo.ZipCode
            Me.TextboxLocation.Text = Me.cammWebManager.CurrentUserInfo.Location
            Me.TextboxState.Text = Me.cammWebManager.CurrentUserInfo.State
            If TextboxCountry IsNot Nothing Then TextboxCountry.Text = Me.cammWebManager.CurrentUserInfo.Country
            If DropdownCountry IsNot Nothing Then
                Dim Item As System.Web.UI.WebControls.ListItem = LookupListItemWithValue(DropdownCountry.Items, Me.cammWebManager.CurrentUserInfo.Country)
                If Item Is Nothing Then
                    Item = New System.Web.UI.WebControls.ListItem(Me.cammWebManager.CurrentUserInfo.Country)
                    DropdownCountry.Items.Add(Item)
                End If
                DropdownCountry.SelectedValue = Item.Value
            End If
            Me.SelectPreferredLanguage(Me.Dropdown1stPreferredLanguage, Me.cammWebManager.CurrentUserInfo.PreferredLanguage1.ID)
            Me.SelectPreferredLanguage(Me.Dropdown2ndPreferredLanguage, Me.cammWebManager.CurrentUserInfo.PreferredLanguage2.ID)
            Me.SelectPreferredLanguage(Me.Dropdown3rdPreferredLanguage, Me.cammWebManager.CurrentUserInfo.PreferredLanguage3.ID)
            Me.TextboxPhone.Text = Me.cammWebManager.CurrentUserInfo.PhoneNumber
            Me.TextboxFax.Text = Me.cammWebManager.CurrentUserInfo.FaxNumber
            Me.TextboxMobile.Text = Me.cammWebManager.CurrentUserInfo.MobileNumber
            Me.TextboxPositionInCompany.Text = Me.cammWebManager.CurrentUserInfo.Position
            Me.FillMotivationControlValues(Me.cammWebManager.CurrentUserInfo.AdditionalFlags("Motivation"))
            Me.FillComesFromControlValues(Me.cammWebManager.CurrentUserInfo.AdditionalFlags("ComesFrom"))
        End Sub

        Protected Overrides Sub AssignFormDataToUserInfo()
            Me.cammWebManager.CurrentUserInfo.EMailAddress = Trim(Me.TextboxEMail.Text)
            Me.cammWebManager.CurrentUserInfo.Company = Trim(Me.TextboxCompany.Text)
            Me.cammWebManager.CurrentUserInfo.Gender = CType(IIf(Me.DropdownSalutation.SelectedValue = "Ms.", WMSystem.Sex.Feminine, IIf(Me.DropdownSalutation.SelectedValue = "Mr.", WMSystem.Sex.Masculine, WMSystem.Sex.Undefined)), WMSystem.Sex)
            Me.cammWebManager.CurrentUserInfo.NameAddition = Trim(Me.TextboxNameAffix.Text)
            Me.cammWebManager.CurrentUserInfo.FirstName = Trim(Me.TextboxFirstName.Text)
            Me.cammWebManager.CurrentUserInfo.LastName = Trim(Me.TextboxLastName.Text)
            Me.cammWebManager.CurrentUserInfo.AcademicTitle = Trim(Me.TextboxAcademicTitle.Text)
            Me.cammWebManager.CurrentUserInfo.Street = Trim(Me.TextboxStreet.Text)
            Me.cammWebManager.CurrentUserInfo.ZipCode = Trim(Me.TextboxZipCode.Text)
            Me.cammWebManager.CurrentUserInfo.Location = Trim(Me.TextboxLocation.Text)
            Me.cammWebManager.CurrentUserInfo.State = Trim(Me.TextboxState.Text)
            If DropdownCountry IsNot Nothing AndAlso DropdownCountry.Visible = True Then
                Me.cammWebManager.CurrentUserInfo.Country = Trim(Me.DropdownCountry.SelectedValue)
            Else
                Me.cammWebManager.CurrentUserInfo.Country = Trim(Me.TextboxCountry.Text)
            End If
            Me.cammWebManager.CurrentUserInfo.PreferredLanguage1 = New WMSystem.LanguageInformation(CInt(Me.Dropdown1stPreferredLanguage.SelectedValue), Me.cammWebManager)
            Me.cammWebManager.CurrentUserInfo.PreferredLanguage2 = New WMSystem.LanguageInformation(Utils.TryCInt(Me.Dropdown2ndPreferredLanguage.SelectedValue), Me.cammWebManager)
            Me.cammWebManager.CurrentUserInfo.PreferredLanguage3 = New WMSystem.LanguageInformation(Utils.TryCInt(Me.Dropdown3rdPreferredLanguage.SelectedValue), Me.cammWebManager)
            Me.cammWebManager.CurrentUserInfo.PhoneNumber = Me.TextboxPhone.Text
            Me.cammWebManager.CurrentUserInfo.FaxNumber = Me.TextboxFax.Text
            Me.cammWebManager.CurrentUserInfo.MobileNumber = Me.TextboxMobile.Text
            Me.cammWebManager.CurrentUserInfo.Position = Me.TextboxPositionInCompany.Text
            Me.cammWebManager.CurrentUserInfo.AdditionalFlags("OnCreationComment") = Me.TextboxComment.Text
            Me.cammWebManager.CurrentUserInfo.AdditionalFlags("Motivation") = CollectMotivationDetails()
            Me.cammWebManager.CurrentUserInfo.AdditionalFlags("ComesFrom") = CollectComesFromDetails()
        End Sub

    End Class

End Namespace