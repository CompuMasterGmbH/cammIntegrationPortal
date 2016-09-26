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
Imports System.Collections.Generic
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     Updates user details
    ''' </summary>
    Public Class UsersUpdate
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg, Field_ID, Field_LoginName, Field_LoginCount, Field_LoginFailures As Label
        Protected Field_CreatedOn, Field_ModifiedOn, Field_LastLoginOn As Label
        Protected Field_Company, Field_Titel, Field_Vorname, Field_Nachname, Field_Namenszusatz, Field_e_mail As TextBox
        Protected Field_Strasse, Field_PLZ, Field_Ort, Field_State, Field_Land, Field_LoginLockedTill As TextBox
        Protected Field_ExternalAccount, Field_Phone, Field_Fax, Field_Mobile, Field_Position As TextBox
        Protected cmbAnrede, cmb1stPreferredLanguage, cmb2ndPreferredLanguage, cmb3rdPreferredLanguage, cmbLoginDisabled, cmbAccountAccessable, cmbIsImpersonationUser As DropDownList
        Protected WithEvents cmbCountry As DropDownList
        Protected WithEvents Button_Submit As Button
        Protected UserInfo As WMSystem.UserInformation
        Protected pnlSpecialUsers As Panel
#End Region

#Region "Property Declaration"
        Protected ReadOnly Property UserID() As Long
            Get
                If Request.QueryString("ID") = "" Then
                    Return 0
                Else
                    Return Utils.Nz(Request.QueryString("ID"), 0L)
                End If
            End Get
        End Property

        Private _DatabaseServerDateTime As DateTime
        Protected ReadOnly Property DatabaseServerDateTime As DateTime
            Get
                If _DatabaseServerDateTime = Nothing Then
                    _DatabaseServerDateTime = Me.CurrentDatabaseDateTime
                End If
                Return _DatabaseServerDateTime
            End Get
        End Property
#End Region

        ''' <summary>
        ''' The list of allowed values for the country field (or empty list in case of no limitation)
        ''' </summary>
        ''' <returns></returns>
        Private ReadOnly Property LimitedAllowedCountries As System.Collections.Generic.List(Of String)
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

        Private Function ConvertStringsToListItems(values As System.Collections.Generic.List(Of String)) As List(Of ListItem)
            Dim Result As New List(Of ListItem)
            For MyCounter As Integer = 0 To values.Count - 1
                Result.Add(New ListItem(values(MyCounter)))
            Next
            Return Result
        End Function

#Region "Page Events"
        Private Sub UsersUpdate_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Me.cmbCountry IsNot Nothing AndAlso Me.Field_Land Is Nothing Then
                'dropdown box available, free text field not available
                Me.cmbCountry.Visible = True
                If Me.IsPostBack = False Then
                    Me.cmbCountry.Items.AddRange(ConvertStringsToListItems(Me.LimitedAllowedCountries).ToArray)
                End If
                Me.Field_Land.Visible = False
            ElseIf Me.cmbCountry Is Nothing AndAlso Me.Field_Land IsNot Nothing Then
                'no dropdown control available --> use free text field
            ElseIf Me.cmbCountry Is Nothing AndAlso Me.Field_Land Is Nothing Then
                'no country field there?!? -> well, let's try to ignore this at this point
            Else 'both controls available - show just the best fitting option
                If LimitedAllowedCountries.Count = 0 Then
                    'no limits --> free text field
                    Me.cmbCountry.Visible = False
                    Me.Field_Land.Visible = True
                Else
                    'limited to defined allow-values -> dropdown box
                    Me.cmbCountry.Visible = True
                    If Me.IsPostBack = False Then
                        Me.cmbCountry.Items.AddRange(ConvertStringsToListItems(Me.LimitedAllowedCountries).ToArray)
                    End If
                    Me.Field_Land.Visible = False
                End If
            End If

            UserInfo = New WebManager.WMSystem.UserInformation(UserID, cammWebManager, False)
            lblErrMsg.Text = ""

            If Not Page.IsPostBack Then
                If camm.WebManager.WMSystem.SpecialUsers.User_Anonymous = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Code = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Invalid = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Public = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_UpdateProcessor = CInt(Request.QueryString("ID")) Then
                    AssignUserInfoDataToForm()
                    pnlSpecialUsers.Visible = False
                Else
                    pnlSpecialUsers.Visible = True
                    AssignUserInfoDataToForm()
                    FillDropDownLists()
                End If
            End If
        End Sub
#End Region

#Region "User-Defined Methods"
        ''' <summary>
        '''     ID and (English) title of all available languages (markets) + the currently selected language (when it is different)
        ''' </summary>
        ''' <param name="alwaysIncludeThisLanguage">The currently selected language should always appear</param>
        ''' <remarks>Intended for the preferred languages dropdown boxes
        ''' </remarks>
        Private Function AvailableLanguages(ByVal alwaysIncludeThisLanguage As Integer) As DictionaryEntry()
            Return ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(New SqlClient.SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID, Description_English FROM System_Languages WHERE (IsActive = 1 AND NOT ID = 10000) OR ID = " & alwaysIncludeThisLanguage & " ORDER BY Description_English", CommandType.Text, Nothing, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Function
        ''' <summary>
        '''     ID and title of the available access levels
        ''' </summary>
        ''' <remarks>Intended for the preferred languages dropdown boxes
        ''' </remarks>
        Private Function AvailableAccessLevels() As DictionaryEntry()
            Return ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(New SqlClient.SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID, Title FROM System_AccessLevels ORDER BY Title", CommandType.Text, Nothing, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Function

        Private Function LookupListItemWithValue(collection As ListItemCollection, value As String) As ListItem
            For MyCounter As Integer = 0 To collection.Count - 1
                If collection(MyCounter).Value.ToUpperInvariant = value.ToUpperInvariant Then
                    Return collection(MyCounter)
                End If
            Next
            Return Nothing
        End Function

        Protected Sub AssignUserInfoDataToForm()
            Field_ID.Text = Server.HtmlEncode(UserInfo.IDLong.ToString)
            Field_LoginName.Text = Server.HtmlEncode(UserInfo.LoginName)
            Field_Company.Text = UserInfo.Company
            Field_Titel.Text = UserInfo.AcademicTitle
            Field_Vorname.Text = UserInfo.FirstName
            Field_Nachname.Text = UserInfo.LastName
            Field_Namenszusatz.Text = UserInfo.NameAddition
            Field_e_mail.Text = UserInfo.EMailAddress
            Field_Strasse.Text = UserInfo.Street
            Field_PLZ.Text = UserInfo.ZipCode
            Field_Ort.Text = UserInfo.Location
            Field_State.Text = UserInfo.State
            If Field_Land IsNot Nothing Then Field_Land.Text = UserInfo.Country
            If cmbCountry IsNot Nothing Then
                Dim Item As ListItem = LookupListItemWithValue(cmbCountry.Items, UserInfo.Country)
                If Item Is Nothing Then
                    Item = New ListItem(UserInfo.Country)
                    cmbCountry.Items.Add(Item)
                End If
                cmbCountry.SelectedValue = Item.Value
            End If

            Field_Phone.Text = UserInfo.PhoneNumber
            Field_Fax.Text = UserInfo.FaxNumber
            Field_Mobile.Text = UserInfo.MobileNumber
            Field_Position.Text = UserInfo.Position

            Field_LoginCount.Text = Server.HtmlEncode(UserInfo.AccountSuccessfullLogins.ToString)
            Field_LoginFailures.Text = Server.HtmlEncode(UserInfo.AccountLoginFailures.ToString)
            If UserInfo.LoginLockedTemporaryTill = Nothing Then
                Field_LoginLockedTill.Text = String.Empty
            Else
                Field_LoginLockedTill.Text = UserInfo.LoginLockedTemporaryTill.ToString
            End If
            Field_CreatedOn.Text = Server.HtmlEncode(UserInfo.AccountCreatedOn.ToString)
            Field_ModifiedOn.Text = Server.HtmlEncode(UserInfo.AccountModifiedOn.ToString)
            If UserInfo.AccountLastLoginOn = Nothing Then
                Field_LastLoginOn.Text = Nothing
            Else
                Field_LastLoginOn.Text = Server.HtmlEncode(UserInfo.AccountLastLoginOn.ToString)
            End If
            'Field_LastLoginViaRemoteIP.Text = Server.HtmlEncode(UserInfo.AccountLastLoginFromAddress)
            If UserInfo.ExternalAccount = Nothing Then Field_ExternalAccount.Text = "" Else Field_ExternalAccount.Text = UserInfo.ExternalAccount.ToString
        End Sub

        Protected Sub AssignFormDataToUserInfo()
            UserInfo.Company = Trim(Me.Field_Company.Text)
            If cmbAnrede.SelectedValue <> Nothing Then
                If Integer.Parse(cmbAnrede.SelectedValue) = CType(IUserInformation.GenderType.Feminine, Integer) Then
                    UserInfo.Gender = CType(IUserInformation.GenderType.Feminine, WMSystem.Sex)
                ElseIf Integer.Parse(cmbAnrede.SelectedValue) = CType(IUserInformation.GenderType.Masculine, Integer) Then
                    UserInfo.Gender = CType(IUserInformation.GenderType.Masculine, WMSystem.Sex)
                End If
            Else
                UserInfo.Gender = WMSystem.Sex.Undefined
            End If
            UserInfo.AcademicTitle = Trim(Me.Field_Titel.Text)
            UserInfo.FirstName = Trim(Me.Field_Vorname.Text)
            UserInfo.LastName = Trim(Me.Field_Nachname.Text)
            UserInfo.NameAddition = Trim(Me.Field_Namenszusatz.Text)
            UserInfo.EMailAddress = Trim(Me.Field_e_mail.Text)
            UserInfo.Street = Trim(Me.Field_Strasse.Text)
            UserInfo.ZipCode = Trim(Me.Field_PLZ.Text)
            UserInfo.Location = Trim(Me.Field_Ort.Text)
            UserInfo.State = Trim(Me.Field_State.Text)
            If cmbCountry IsNot Nothing AndAlso cmbCountry.Visible = True Then
                UserInfo.Country = Trim(Me.cmbCountry.SelectedValue)
            Else
                UserInfo.Country = Trim(Me.Field_Land.Text)
            End If

            UserInfo.PhoneNumber = Trim(Me.Field_Phone.Text)
            UserInfo.FaxNumber = Trim(Me.Field_Fax.Text)
            UserInfo.MobileNumber = Trim(Me.Field_Mobile.Text)
            UserInfo.Position = Trim(Me.Field_Position.Text)

            UserInfo.PreferredLanguage1 = New WMSystem.LanguageInformation(CInt(cmb1stPreferredLanguage.SelectedValue), cammWebManager)
            If Trim(cmb2ndPreferredLanguage.SelectedValue) = "" Then
                UserInfo.PreferredLanguage2 = New WMSystem.LanguageInformation(0, cammWebManager)
            Else
                UserInfo.PreferredLanguage2 = New WMSystem.LanguageInformation(CInt(cmb2ndPreferredLanguage.SelectedValue), cammWebManager)
            End If
            If Trim(cmb3rdPreferredLanguage.SelectedValue) = "" Then
                UserInfo.PreferredLanguage3 = New WMSystem.LanguageInformation(0, cammWebManager)
            Else
                UserInfo.PreferredLanguage3 = New WMSystem.LanguageInformation(CInt(cmb3rdPreferredLanguage.SelectedValue), cammWebManager)
            End If
            UserInfo.AccessLevel = New WMSystem.AccessLevelInformation(Utils.Nz(cmbAccountAccessable.SelectedValue, 0), cammWebManager)
            UserInfo.LoginDisabled = CBool(CInt(cmbLoginDisabled.SelectedValue))
            If Me.cmbIsImpersonationUser IsNot Nothing Then
                UserInfo.IsImpersonationUser = CBool(CInt(cmbIsImpersonationUser.SelectedValue))
            End If
            If Trim(Me.Field_LoginLockedTill.Text) = "" Then
                UserInfo.LoginLockedTemporary = False
            Else
                UserInfo.LoginLockedTemporaryTill = DateTime.Parse(Trim(Me.Field_LoginLockedTill.Text))
            End If

            UserInfo.ExternalAccount = Trim(Field_ExternalAccount.Text)
        End Sub

        Protected Overridable Function UpdateAccount() As Boolean
            If UserInfo.IDLong = Nothing Then
                Throw New Exception("Invalid user, can't update")
            ElseIf UserInfo.LoginName = Nothing Then
                Throw New Exception("Missing login name")
            ElseIf UserInfo.EMailAddress = Nothing Then
                Throw New Exception("Missing e-mail address")
            Else
                Try
                    UserInfo.Save()
                    UserInfo.ReloadFullUserData() 'in case e.g. some fields had been cut off
                Catch ex As Exception
                    'unhandled exception.
                    If ex.Message.ToString.ToUpper = "UNIQUE KEY ERROR" Then
                        lblErrMsg.Text = "External Account already exists."
                    Else
                        Throw
                    End If
                    Return False
                End Try
            End If
            Return True
        End Function

        Private Sub FillDropDownLists()
            'bind Gender/Anrede dropdownlist
            cmbAnrede.Items.Clear()
            cmbAnrede.Items.Add(New ListItem(Nothing, Nothing))
            cmbAnrede.Items.Add(New ListItem("Mr.", CType(WMSystem.Sex.Masculine, String)))
            cmbAnrede.Items.Add(New ListItem("Ms.", CType(WMSystem.Sex.Feminine, String)))
            If UserInfo.Gender = WMSystem.Sex.Masculine Then
                cmbAnrede.SelectedIndex = 1
            ElseIf UserInfo.Gender = WMSystem.Sex.Feminine Then
                cmbAnrede.SelectedIndex = 2
            Else
                cmbAnrede.SelectedIndex = 0
            End If
            'bind LoginDisabled dropdownlist
            cmbLoginDisabled.Items.Clear()
            cmbLoginDisabled.Items.Add(New ListItem("Yes", "1"))
            cmbLoginDisabled.Items.Add(New ListItem("No", "0"))
            If UserInfo.LoginDisabled = True Then cmbLoginDisabled.SelectedValue = "1" Else cmbLoginDisabled.SelectedValue = "0"
            'bind IsImpersonationUser dropdownlist
            If Me.cmbIsImpersonationUser IsNot Nothing Then
                cmbIsImpersonationUser.Items.Clear()
                cmbIsImpersonationUser.Items.Add(New ListItem("Yes", "1"))
                cmbIsImpersonationUser.Items.Add(New ListItem("No", "0"))
                If UserInfo.IsImpersonationUser = True Then cmbIsImpersonationUser.SelectedValue = "1" Else cmbIsImpersonationUser.SelectedValue = "0"
            End If
            'bind AvailableAccessLevels dropdownlist
            cmbAccountAccessable.Items.Clear()
            Dim AccessLevels As DictionaryEntry() = AvailableAccessLevels()
            For MyCounter As Integer = 0 To AccessLevels.Length - 1
                cmbAccountAccessable.Items.Add(New ListItem(Server.HtmlEncode(Utils.Nz(AccessLevels(MyCounter).Value, String.Empty)), Utils.Nz(AccessLevels(MyCounter).Key, String.Empty)))
                If UserInfo.AccessLevel.ID = Utils.Nz(AccessLevels(MyCounter).Key, 0) Then cmbAccountAccessable.SelectedIndex = MyCounter
            Next
            'bind 1stPreferredLanguage dropdownlist
            cmb1stPreferredLanguage.Items.Clear()
            Dim AvailableLanguageInfos As DictionaryEntry()
            AvailableLanguageInfos = AvailableLanguages(UserInfo.PreferredLanguage1.ID)
            For MyCounter As Integer = 0 To AvailableLanguageInfos.Length - 1
                cmb1stPreferredLanguage.Items.Add(New ListItem(Utils.Nz(AvailableLanguageInfos(MyCounter).Value, String.Empty), Utils.Nz(AvailableLanguageInfos(MyCounter).Key, String.Empty)))
                If UserInfo.PreferredLanguage1.ID = Utils.Nz(AvailableLanguageInfos(MyCounter).Key, 0) Then cmb1stPreferredLanguage.SelectedIndex = MyCounter
            Next
            'bind 2ndPreferredLanguage dropdownlist
            cmb2ndPreferredLanguage.Items.Clear()
            cmb2ndPreferredLanguage.Items.Add(New ListItem("", ""))
            If UserInfo.PreferredLanguage2 Is Nothing Then
                AvailableLanguageInfos = AvailableLanguages(Nothing)
            Else
                AvailableLanguageInfos = AvailableLanguages(UserInfo.PreferredLanguage2.ID)
            End If
            For MyCounter As Integer = 0 To AvailableLanguageInfos.Length - 1
                cmb2ndPreferredLanguage.Items.Add(New ListItem(Utils.Nz(AvailableLanguageInfos(MyCounter).Value, String.Empty), Utils.Nz(AvailableLanguageInfos(MyCounter).Key, String.Empty)))
                If Not UserInfo.PreferredLanguage2 Is Nothing AndAlso UserInfo.PreferredLanguage2.ID = Utils.Nz(AvailableLanguageInfos(MyCounter).Key, 0) Then cmb2ndPreferredLanguage.SelectedIndex = MyCounter + 1
            Next
            'bind 3rdPreferredLanguage dropdownlist
            cmb3rdPreferredLanguage.Items.Clear()
            cmb3rdPreferredLanguage.Items.Add(New ListItem("", ""))
            If UserInfo.PreferredLanguage3 Is Nothing Then
                AvailableLanguageInfos = AvailableLanguages(Nothing)
            Else
                AvailableLanguageInfos = AvailableLanguages(UserInfo.PreferredLanguage3.ID)
            End If
            For MyCounter As Integer = 0 To AvailableLanguageInfos.Length - 1
                cmb3rdPreferredLanguage.Items.Add(New ListItem(Utils.Nz(AvailableLanguageInfos(MyCounter).Value, String.Empty), Utils.Nz(AvailableLanguageInfos(MyCounter).Key, String.Empty)))
                If Not UserInfo.PreferredLanguage3 Is Nothing AndAlso UserInfo.PreferredLanguage3.ID = Utils.Nz(AvailableLanguageInfos(MyCounter).Key, 0) Then cmb3rdPreferredLanguage.SelectedIndex = MyCounter + 1
            Next
        End Sub
#End Region

#Region "Control Events"
        Sub SaveChanges(ByVal sender As Object, ByVal e As EventArgs) Handles Button_Submit.Click
            Try
                AssignFormDataToUserInfo()
                If UpdateAccount() = True Then
                    If camm.WebManager.WMSystem.SpecialUsers.User_Anonymous = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Code = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Invalid = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Public = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_UpdateProcessor = CInt(Request.QueryString("ID")) Then
                        AssignUserInfoDataToForm()
                    End If

                    lblErrMsg.Style.Add("color", "#009900")
                    lblErrMsg.Text = "The record has been updated successfully!"
                End If
            Catch ex As FlagValidation.RequiredFlagException
                Dim ErrDetails As String = ""
                For Each result As FlagValidation.FlagValidationResult In ex.ValidationResults
                    If ErrDetails <> Nothing Then ErrDetails &= ", "
                    ErrDetails &= result.Flag & " (" & [Enum].GetName(GetType(FlagValidation.FlagValidationResultCode), result.ValidationResult) & ")"
                Next
                lblErrMsg.Text = ex.Message & ": " & ErrDetails
            Catch ex As UserInfoDataException
                lblErrMsg.Text = ex.Message
            Catch ex As Exception
                Throw New Exception("Cannot save changes to the user profile.", ex)
            End Try
        End Sub

        Private Sub UsersUpdate_Init(sender As Object, e As EventArgs) Handles Me.Init
            Me.lblErrMsg.EnableViewState = False 'waste-always any content between postbacks
        End Sub

#End Region

    End Class

End Namespace