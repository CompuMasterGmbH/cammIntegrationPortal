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

Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Collections.Generic
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     A page to create a new user
    ''' </summary>
    Public Class New_Users
        Inherits Page

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

#Region "Variable Declaration"

        Protected lblMsg As Label
        Protected txtPhone, txtFax, txtMobile, txtPosition, txtLoginName, txtPassword1, txtPassword2, txtCompany As TextBox
        Protected cmbAnrede As DropDownList
        Protected WithEvents btnSubmit As Button
        Protected txtTitle, txtVorname, txtNachname, txtNamenszusatz, txtemail, txtStrasse, txtPLZ, txtOrt, txtState, txtLand As TextBox
        Protected cmbCountry, cmbFirstPreferredLanguage, cmbSecondPreferredLanguage, cmbThirdPreferredLanguage, cmbAccountAccessability As DropDownList

        Dim Field_Phone, Field_Fax, Field_Mobile, Field_Position, Field_LoginName, Field_Password1, Field_Password2 As String
        Dim Field_Company, Field_Anrede, Field_Titel, Field_Vorname, Field_Nachname, Field_Namenszusatz, Field_e_mail As String
        Dim Field_Strasse, Field_PLZ, Field_Ort, Field_State, Field_Land As String
        Dim Field_1stPreferredLanguage, Field_2ndPreferredLanguage, Field_3rdPreferredLanguage, Field_AccountAccessable As Integer

        'Declarations For UpdateRecord
        Dim MyCount As Object
        Dim ErrMsg As String
        Dim UpdateSuccessfull As Boolean
        Dim LoginIDOfUser As Integer
        Dim NewUserID As Long
        Dim MyUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation
        Dim MyUserInfoAdditionalFlags As New Collections.Specialized.NameValueCollection
        Dim MyUserInfoSex As CompuMaster.camm.WebManager.WMSystem.Sex
#End Region

#Region "Page Event"
        Private Sub New_Users_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Me.cmbCountry IsNot Nothing AndAlso Me.txtLand Is Nothing Then
                'dropdown box available, free text field not available
                Me.cmbCountry.Visible = True
                If Me.IsPostBack = False Then
                    Me.cmbCountry.Items.AddRange(ConvertStringsToListItems(Me.LimitedAllowedCountries).ToArray)
                End If
                Me.txtLand.Visible = False
            ElseIf Me.cmbCountry Is Nothing AndAlso Me.txtLand IsNot Nothing Then
                'no dropdown control available --> use free text field
            ElseIf Me.cmbCountry Is Nothing AndAlso Me.txtLand Is Nothing Then
                'no country field there?!? -> well, let's try to ignore this at this point
            Else 'both controls available - show just the best fitting option
                If LimitedAllowedCountries.Count = 0 Then
                    'no limits --> free text field
                    Me.cmbCountry.Visible = False
                    Me.txtLand.Visible = True
                Else
                    'limited to defined allow-values -> dropdown box
                    Me.cmbCountry.Visible = True
                    If Me.IsPostBack = False Then
                        Me.cmbCountry.Items.AddRange(ConvertStringsToListItems(Me.LimitedAllowedCountries).ToArray)
                    End If
                    Me.txtLand.Visible = False
                End If
            End If

            If cmbAnrede.Items.Count > 0 Then
                If cmbAnrede.SelectedItem.Text = "Ms." Then
                    MyUserInfoSex = CompuMaster.camm.WebManager.WMSystem.Sex.Feminine
                ElseIf cmbAnrede.SelectedItem.Text = "Mr." Then
                    MyUserInfoSex = CompuMaster.camm.WebManager.WMSystem.Sex.Masculine
                End If
            End If

            Update_Data(UpdateSuccessfull, ErrMsg)

            Me.lblMsg.EnableViewState = False
            If ErrMsg <> "" Then lblMsg.Text = ErrMsg
            txtLoginName.Text = Utils.Nz(Field_LoginName, String.Empty)
            txtPassword1.Text = Utils.Nz(Field_Password1, String.Empty)
            txtPassword1.TextMode = TextBoxMode.Password
            txtPassword2.Text = Utils.Nz(Field_Password2, String.Empty)
            txtPassword2.TextMode = TextBoxMode.Password
            txtCompany.Text = Utils.Nz(Field_Company, String.Empty)
            txtTitle.Text = Utils.Nz(Field_Titel, String.Empty)
            txtVorname.Text = Utils.Nz(Field_Vorname, String.Empty)
            txtNachname.Text = Utils.Nz(Field_Nachname, String.Empty)
            txtNamenszusatz.Text = Utils.Nz(Field_Namenszusatz, String.Empty)
            txtemail.Text = Utils.Nz(Field_e_mail, String.Empty)
            txtStrasse.Text = Utils.Nz(Field_Strasse, String.Empty)
            txtPLZ.Text = Utils.Nz(Field_PLZ, String.Empty)
            txtOrt.Text = Utils.Nz(Field_Ort, String.Empty)
            txtState.Text = Utils.Nz(Field_State, String.Empty)
            txtLand.Text = Utils.Nz(Field_Land, String.Empty)
            txtPhone.Text = Utils.Nz(Field_Phone, String.Empty)
            txtFax.Text = Utils.Nz(Field_Fax, String.Empty)
            txtMobile.Text = Utils.Nz(Field_Mobile, String.Empty)
            txtPosition.Text = Utils.Nz(Field_Position, String.Empty)
            If Not IsPostBack Then FillDropDownList(Field_AccountAccessable.ToString, Field_1stPreferredLanguage.ToString, Field_2ndPreferredLanguage.ToString, Field_3rdPreferredLanguage.ToString)
        End Sub
#End Region

#Region "Control Events"
        Private Sub btnSubmitClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
            If txtLoginName.Text <> "" AndAlso _
               txtPassword1.Text <> "" AndAlso _
               txtPassword2.Text <> "" AndAlso _
               txtPassword1.Text = txtPassword2.Text AndAlso _
               txtemail.Text <> "" AndAlso _
               cmbFirstPreferredLanguage.SelectedItem.Text <> "Please Select!" AndAlso _
               cmbAccountAccessability.SelectedItem.Text <> "Please Select!" Then

                If Me.cmbAnrede.SelectedIndex = 1 Then
                    MyUserInfoSex = WMSystem.Sex.Masculine
                ElseIf Me.cmbAnrede.SelectedIndex = 2 Then
                    MyUserInfoSex = WMSystem.Sex.Feminine
                Else
                    MyUserInfoSex = WMSystem.Sex.Undefined
                End If

                MyUserInfo = Me.CreateNewUserInfo()

                'Update record
                Select Case cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).ValidatePasswordComplexity(txtPassword1.Text, MyUserInfo)
                    Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_HigherPasswordComplexityRequired
                        ErrMsg = "The password doesn't match the current security policy for passwords!"
                    Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_LengthMaximum
                        ErrMsg = "The password requires to be not bigger than " & cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).RequiredMaximumPasswordLength & " characters!"
                    Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_LengthMinimum
                        ErrMsg = "The password requires to be not smaller than " & cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).RequiredPasswordLength & " characters!"
                    Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_NotAllowed_PartOfProfileInformation
                        ErrMsg = "The password shouldn't contain pieces of the user account profile, especially login name, first or last name!"
                    Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Success
                        Try
                            Dim dt As DataTable
                            Dim sqlParams As SqlParameter() = {New SqlParameter("@Loginname", Trim(txtLoginName.Text))}
                            dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select count(*) from Benutzer where Loginname=@Loginname", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                            Dim sqlParamsNew As SqlParameter() = {New SqlParameter("@Loginname", Trim(txtLoginName.Text))}
                            MyCount = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select count(*) from Benutzer where Loginname=@Loginname", CommandType.Text, sqlParamsNew, Automations.AutoOpenAndCloseAndDisposeConnection)

                            If Utils.Nz(MyCount, 0) > 0 Then
                                ErrMsg = "User account already exists!"
                                UpdateSuccessfull = False
                                ErrMsg = "User account already exists!"
                            Else
                                Dim NewPassword As String = txtPassword1.Text
                                MyUserInfo.Save(NewPassword)
                                NewUserID = MyUserInfo.IDLong
                                Dim tempstr As String

                                tempstr = ""
                                dt = FillDataTable(New SqlCommand("exec Public_SetUserDetailData @IDUser = " & NewUserID & ", @Type = N'Phone', @Value = N'" & Trim(Request.Form("Phone")) & "', @DoNotLogSuccess = 1", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                                Dim sqlParams1 As SqlParameter() = {New SqlParameter("@IDUser", NewUserID), New SqlParameter("@Type", "Phone"), New SqlParameter("@Value", txtPhone.Text), New SqlParameter("@DoNotLogSuccess", 1)}
                                tempstr = Utils.Nz(ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "Public_SetUserDetailData", CommandType.StoredProcedure, sqlParams1, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), String.Empty)

                                dt = FillDataTable(New SqlCommand("exec Public_SetUserDetailData @IDUser = " & NewUserID & ", @Type = N'Fax', @Value = N'" & Trim(Request.Form("Fax")) & "', @DoNotLogSuccess = 1", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                                Dim sqlParams2 As SqlParameter() = {New SqlParameter("@IDUser", NewUserID), New SqlParameter("@Type", "Fax"), New SqlParameter("@Value", txtFax.Text), New SqlParameter("@DoNotLogSuccess", 1)}
                                tempstr = Utils.Nz(ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "Public_SetUserDetailData", CommandType.StoredProcedure, sqlParams2, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), String.Empty)

                                dt = FillDataTable(New SqlCommand("exec Public_SetUserDetailData @IDUser = " & NewUserID & ", @Type = N'Mobile', @Value = N'" & Trim(Request.Form("Mobile")) & "', @DoNotLogSuccess = 1", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                                Dim sqlParams3 As SqlParameter() = {New SqlParameter("@IDUser", NewUserID), New SqlParameter("@Type", "Mobile"), New SqlParameter("@Value", txtMobile.Text), New SqlParameter("@DoNotLogSuccess", 1)}
                                tempstr = Utils.Nz(ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "Public_SetUserDetailData", CommandType.StoredProcedure, sqlParams3, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), String.Empty)

                                dt = FillDataTable(New SqlCommand("exec Public_SetUserDetailData @IDUser = " & NewUserID & ", @Type = N'Position', @Value = N'" & Trim(Request.Form("Position")) & "', @DoNotLogSuccess = 1", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                                Dim sqlParams4 As SqlParameter() = {New SqlParameter("@IDUser", NewUserID), New SqlParameter("@Type", "Position"), New SqlParameter("@Value", txtPosition.Text), New SqlParameter("@DoNotLogSuccess", 1)}
                                tempstr = Utils.Nz(ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "Public_SetUserDetailData", CommandType.StoredProcedure, sqlParams4, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), String.Empty)

                                ErrMsg = "The profile has been created successfully!"
                                UpdateSuccessfull = True
                            End If
                        Catch ex As UserInfoDataException
                            ErrMsg = ex.Message
                        Catch ex As Exception
                            Throw
                        End Try
                    Case Else
                        ErrMsg = "There are some unknown errors when validating with the security policy for passwords!"
                End Select
            Else
                ErrMsg = "Please specify a unique logon name, the password inclusive the confirmation, the complete name and address and at least one language preference to proceed!"
            End If
            Update_Data(UpdateSuccessfull, ErrMsg)
            If ErrMsg <> "" Then
                lblMsg.Text = ErrMsg
            End If
        End Sub
#End Region

        ''' <summary>
        ''' Create a new user information object from scratch and fill it with the details from of the form
        ''' </summary>
        ''' <returns></returns>
        Protected Overridable Function CreateNewUserInfo() As WMSystem.UserInformation
            Dim CountryValue As String
            If Me.cmbCountry IsNot Nothing AndAlso Me.cmbCountry.Visible = True Then
                CountryValue = Me.cmbCountry.SelectedValue
            Else
                CountryValue = Me.txtLand.Text
            End If
            Dim MyUserInfo As New CompuMaster.camm.WebManager.WMSystem.UserInformation(Nothing, (Trim(txtLoginName.Text & "")), (Trim(txtemail.Text & "")), False, Trim(txtCompany.Text & ""), MyUserInfoSex, Trim(txtNamenszusatz.Text & ""), Trim(txtVorname.Text & ""), Trim(txtNachname.Text & ""), Trim(txtTitle.Text & ""), Trim(txtStrasse.Text & ""), Trim(txtPLZ.Text & ""), Trim(txtOrt.Text & ""), Trim(txtState.Text & ""), Trim(CountryValue & ""), Utils.Nz(IIf(cmbFirstPreferredLanguage.SelectedValue = "", 0, cmbFirstPreferredLanguage.SelectedValue), 0), Utils.Nz(IIf(cmbSecondPreferredLanguage.SelectedValue = "", 0, cmbSecondPreferredLanguage.SelectedValue), 0), Utils.Nz(IIf(cmbThirdPreferredLanguage.SelectedValue = "", 0, cmbThirdPreferredLanguage.SelectedValue), 0), False, False, False, CInt(Val(cmbAccountAccessability.SelectedValue & "")), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem), "", CType(MyUserInfoAdditionalFlags, Collections.Specialized.NameValueCollection))
            MyUserInfo.AdditionalFlags("ComesFrom") = "Account created by Admin """ + cammWebManager.CurrentUserInfo.LoginName + """ (" + cammWebManager.CurrentUserInfo.FullName + ")"
            MyUserInfo.AdditionalFlags("Motivation") = "Account created by Admin"
            Return MyUserInfo
        End Function

#Region "User-Defined Methods"
        Sub Update_Data(ByVal Update_d As Boolean, ByVal ErrMsg As String)
            If Update_d = True Then
                Response.Redirect("users.aspx")
            Else
                Field_LoginName = txtLoginName.Text
                Field_Password1 = txtPassword1.Text
                Field_Password2 = txtPassword2.Text
                Field_Company = txtCompany.Text
                If cmbAnrede.Items.Count > 0 Then
                    Field_Anrede = cmbAnrede.SelectedValue
                End If
                Field_Titel = txtTitle.Text
                Field_Vorname = txtVorname.Text
                Field_Nachname = txtNachname.Text
                Field_Namenszusatz = txtNamenszusatz.Text
                Field_e_mail = txtemail.Text
                Field_Strasse = txtStrasse.Text
                Field_PLZ = txtPLZ.Text.ToString()
                Field_Ort = txtOrt.Text.ToString()
                Field_State = txtState.Text
                Field_Land = txtLand.Text
                Field_Phone = txtPhone.Text.ToString()
                Field_Fax = txtFax.Text.ToString()
                Field_Mobile = txtMobile.Text.ToString()
                Field_Position = txtPosition.Text

                If cmbFirstPreferredLanguage.Items.Count > 0 Then
                    Field_1stPreferredLanguage = Utils.Nz(cmbFirstPreferredLanguage.SelectedValue, 0)
                End If
                If cmbSecondPreferredLanguage.Items.Count > 0 Then
                    Field_2ndPreferredLanguage = Utils.Nz(cmbSecondPreferredLanguage.SelectedValue, 0)
                End If
                If cmbThirdPreferredLanguage.Items.Count > 0 Then
                    Field_3rdPreferredLanguage = Utils.Nz(cmbThirdPreferredLanguage.SelectedValue, 0)
                End If
                If cmbAccountAccessability.Items.Count > 0 Then
                    Field_AccountAccessable = Utils.Nz(cmbAccountAccessability.SelectedValue, 0)
                End If

                If Not txtLoginName.Text = "" AndAlso _
                  txtPassword1.Text = "" AndAlso _
                  txtPassword2.Text = "" AndAlso _
                  txtCompany.Text = "" AndAlso _
                  cmbAnrede.SelectedItem.Text = "" AndAlso _
                  txtTitle.Text = "" AndAlso _
                  txtVorname.Text = "" AndAlso _
                  txtNachname.Text = "" AndAlso _
                  txtNamenszusatz.Text = "" AndAlso _
                  txtemail.Text = "" AndAlso _
                  txtStrasse.Text = "" AndAlso _
                  txtPLZ.Text = "" AndAlso _
                  txtOrt.Text = "" AndAlso _
                  txtState.Text = "" AndAlso _
                  txtLand.Text = "" AndAlso _
                  cmbFirstPreferredLanguage.SelectedItem.Text = "" AndAlso _
                  cmbSecondPreferredLanguage.SelectedItem.Text = "" AndAlso _
                  cmbThirdPreferredLanguage.SelectedItem.Text = "" AndAlso _
                  cmbAccountAccessability.SelectedItem.Text = "" AndAlso _
                  txtPassword1.Text = "" AndAlso _
                  txtPassword2.Text = "" Then
                    If ErrMsg = "" Then ErrMsg = "Please specify a unique logon name, the password inclusive the confirmation, the complete name and address and at least one language preference to proceed!"
                End If
            End If
        End Sub

        Sub FillDropDownList(ByVal Field_AccountAccessable As String, ByVal Field_1stPreferredLanguage As String, ByVal Field_2ndPreferredLanguage As String, ByVal Field_3rdPreferredLanguage As String)
            'Fill Anrede DropDownList            
            cmbAnrede.Items.Clear()
            cmbAnrede.Items.Add(New ListItem(Nothing, Nothing))
            cmbAnrede.Items.Add("Mr.")
            cmbAnrede.Items.Add("Ms.")
            Dim temp As String
            If Field_Anrede = "" Then
                cmbAnrede.SelectedIndex = 0
                temp = cmbAnrede.SelectedItem.Text
            End If
            If Field_Anrede = "Mr." Then
                cmbAnrede.SelectedIndex = 1
                temp = cmbAnrede.SelectedItem.Text
            End If
            If Field_Anrede = "Ms." Then
                cmbAnrede.SelectedIndex = 2
                temp = cmbAnrede.SelectedItem.Text
            End If

            'Fill AccountAccessability DropDownList
            cmbAccountAccessability.Items.Clear()
            cmbAccountAccessability.Items.Add("Please Select!")
            If Field_AccountAccessable = Nothing Then
                cmbAccountAccessability.SelectedIndex = 2
            End If
            Dim ExistentAccessLevels As CompuMaster.camm.WebManager.WMSystem.AccessLevelInformation()
            ExistentAccessLevels = cammWebManager.System_GetAccessLevelInfos()
            For Each ExistentAccessLevel As CompuMaster.camm.WebManager.WMSystem.AccessLevelInformation In ExistentAccessLevels
                cmbAccountAccessability.Items.Add(New ListItem(Server.HtmlEncode(ExistentAccessLevel.Title), Utils.Nz(ExistentAccessLevel.ID, 0).ToString))
                If Utils.Nz(Field_AccountAccessable, 0) = ExistentAccessLevel.ID Then
                    cmbAccountAccessability.SelectedValue = Utils.Nz(ExistentAccessLevel.ID, 0).ToString
                End If
            Next

            'Fill Field_1stPreferredLanguage DropDownList
            cmbFirstPreferredLanguage.Items.Clear()
            cmbFirstPreferredLanguage.Items.Add(New ListItem("Please Select!", "-1"))
            If Field_1stPreferredLanguage = Nothing Then
                cmbFirstPreferredLanguage.SelectedIndex = 0
            End If
            Dim ActiveLanguages As CompuMaster.camm.WebManager.WMSystem.LanguageInformation()
            ActiveLanguages = cammWebManager.System_GetLanguagesInfo(False)
            For Each MyActiveLanguage As CompuMaster.camm.WebManager.WMSystem.LanguageInformation In ActiveLanguages
                cmbFirstPreferredLanguage.Items.Add(New ListItem(MyActiveLanguage.LanguageName_English, Utils.Nz(MyActiveLanguage.ID, 0).ToString))
                If Utils.Nz(Field_1stPreferredLanguage, 0) = MyActiveLanguage.ID Then
                    cmbFirstPreferredLanguage.SelectedValue = Utils.Nz(MyActiveLanguage.ID, 0).ToString
                End If
            Next

            'Fill Field_2ndPreferredLanguage DropDownList
            cmbSecondPreferredLanguage.Items.Clear()
            cmbSecondPreferredLanguage.Items.Add(New ListItem("", "-1"))
            cmbSecondPreferredLanguage.SelectedIndex = 0
            For Each MyActiveLanguage As CompuMaster.camm.WebManager.WMSystem.LanguageInformation In ActiveLanguages
                cmbSecondPreferredLanguage.Items.Add(New ListItem(MyActiveLanguage.LanguageName_English, Utils.Nz(MyActiveLanguage.ID, 0).ToString))
                If Utils.Nz(Field_2ndPreferredLanguage, 0) = MyActiveLanguage.ID Then
                    cmbSecondPreferredLanguage.SelectedValue = Utils.Nz(MyActiveLanguage.ID, String.Empty)
                End If
            Next

            'Fill Field_3rdPreferredLanguage DropDownList
            cmbThirdPreferredLanguage.Items.Clear()
            cmbThirdPreferredLanguage.Items.Add(New ListItem("", "-1"))
            cmbThirdPreferredLanguage.SelectedIndex = 0
            For Each MyActiveLanguage As CompuMaster.camm.WebManager.WMSystem.LanguageInformation In ActiveLanguages
                cmbThirdPreferredLanguage.Items.Add(New ListItem(MyActiveLanguage.LanguageName_English, Utils.Nz(MyActiveLanguage.ID, 0).ToString))
                If Utils.Nz(Field_3rdPreferredLanguage, 0) = MyActiveLanguage.ID Then
                    cmbThirdPreferredLanguage.SelectedValue = Utils.Nz(MyActiveLanguage.ID, 0).ToString
                End If
            Next
        End Sub
#End Region

    End Class

End Namespace