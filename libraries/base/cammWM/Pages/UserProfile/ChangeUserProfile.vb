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

    <Obsolete("Use UpdateUserProfile instead", False), System.Runtime.InteropServices.ComVisible(False)> Public Class ChangeUserProfile
        Inherits CompuMaster.camm.WebManager.Pages.ProtectedPage

        Protected ErrMsg As String = ""
        Protected MissingFields As String = Nothing
        Protected MyUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation
        Protected ShowMissingFieldsListInErrorMessage As Boolean = False
        Protected CheckboxListMotivation As System.Web.UI.WebControls.CheckBoxList
        Protected MotivationOtherText As System.Web.UI.WebControls.TextBox
        ''' <summary>
        '''     Clear the list of missing fields
        ''' </summary>
        Protected Sub ResetMissingFieldItems()
            MissingFields = Nothing
        End Sub
        ''' <summary>
        '''     Add an additional element to the list of missing fields
        ''' </summary>
        ''' <param name="name"></param>
        Protected Sub AddMissingFieldItem(ByVal name As String)
            If MissingFields = Nothing Then
                MissingFields = name
            Else
                MissingFields &= ", " & name
            End If
        End Sub
        ''' <summary>
        '''     Validate the form data to be complete
        ''' </summary>
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
        ''' <summary>
        '''     Fill the user profile with the new data which shall be saved
        ''' </summary>
        ''' <param name="userInfo">The current user profile which shall be updated</param>
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
        ''' <summary>
        ''' Create a string with OPTION tags for all activated languages for embedding into the SELECT tag
        ''' </summary>
        ''' <param name="preferredLanguageLevelID">1 for 1st language, 2 for the 2nd one, 3 for the 3rd one</param>
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

End Namespace